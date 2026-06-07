using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace pinball_board_angle_tool
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            this.KeyPreview = true;

            pictureBox.Paint += pictureBox_Paint;
            pictureBox.MouseClick += pictureBox_MouseClick;
            //pictureBox.MouseEnter += (s, e) => pictureBox.Focus();
            pictureBox.MouseWheel += pictureBox_MouseWheel;
            pictureBox.MouseMove += pictureBox_MouseMove;

            snapEnabled = false;   // snap OFF by default

        }

        private Bitmap boardImage;
        private const int TILE_SIZE = 8;
        private Point? lastSnapPoint = null;
        private float zoom = 2.0f;   // 1× zoom to start
        private Point? hoverTile = null;
        private Point? angleP1 = null;
        private Point? angleP2 = null;
        private bool snapEnabled = false;
        private int currentTileX = -1;
        private int currentTileY = -1;
        private float lastAngleDeg = -1f;
        private HashSet<Point> shadedTiles = new HashSet<Point>();
        private int[,] angleGrid = new int[26, 48];

        private FormAngles angleForm;
        private controlForm control_form;

        private bool cellLocked = false;

        //private float[,] angleGrid = new float[26, 48];

        public void UnlockCell()
        {
            cellLocked = false;
        }

        private void ResetAngleGrid()
        {
            for (int x = 0; x < 26; x++)
                for (int y = 0; y < 48; y++)
                    angleGrid[x, y] = 255;   // or 0xFF
        }

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);

            if ((Control.ModifierKeys & Keys.Control) == 0)
                return;

            if (pictureBox.Bounds.Contains(PointToClient(MousePosition)))
            {
                pictureBox_MouseWheel(pictureBox, e);
            }
        }

        

        private void pictureBox_MouseWheel(object sender, MouseEventArgs e)
        {
            if ((Control.ModifierKeys & Keys.Control) == 0)
                return;

            if (e.Delta > 0)
                zoom += 0.5f;
            else if (zoom > 1.0f)
                zoom -= 0.5f;

            ApplyZoom();
        }

        private void pictureBox_MouseClick(object sender, MouseEventArgs e)
        {
            if (boardImage == null)
                return;

            // Convert from zoomed screen coords → real image coords
            Point real = TranslateZoomClick(e.Location);

            // Clamp to image bounds
            if (real.X < 0 || real.Y < 0 ||
                real.X >= boardImage.Width || real.Y >= boardImage.Height)
            {
                return;
            }

            // ---------------------------------------------------------
            // RIGHT CLICK: TOGGLE TILE SHADE
            // ---------------------------------------------------------
            if (e.Button == MouseButtons.Right)
            {
                int tx = real.X / TILE_SIZE;
                int ty = real.Y / TILE_SIZE;

                Point tile = new Point(tx * TILE_SIZE, ty * TILE_SIZE);

                if (shadedTiles.Contains(tile))
                    shadedTiles.Remove(tile);
                else
                    shadedTiles.Add(tile);

                // Lock the datagrid highlight on this cell
                cellLocked = true;
                if (angleForm != null && !angleForm.IsDisposed && angleForm.GridReady)
                {
                    angleForm.HighlightCell(tx, ty, Color.Cyan);
                    angleForm.SelectCell(tx, ty);  // <-- select the cell too
                }

                pictureBox.Invalidate();
                return;
            }

            // ---------------------------------------------------------
            // LEFT CLICK: ANGLE MEASUREMENT
            // ---------------------------------------------------------

            // Snap to nearest tile center (if enabled)
            Point snapped = snapEnabled ? SnapToGrid(real) : real;

            if (angleP1 == null)
            {
                angleP1 = snapped;
            }
            else if (angleP2 == null)
            {
                angleP2 = snapped;

                // Compute angle
                float dx = angleP2.Value.X - angleP1.Value.X;
                float dy = angleP2.Value.Y - angleP1.Value.Y;

                // Match your engine: invert Y
                float angleRad = (float)Math.Atan2(-dy, dx);
                float angleDeg = angleRad * (180.0f / (float)Math.PI);

                if (angleDeg < 0)
                    angleDeg += 360.0f;

                // Fold into 0–180 (wall orientation)
                if (angleDeg > 180.0f)
                    angleDeg -= 180.0f;

                // Store for UI
                lastAngleDeg = angleDeg;

//---------------------------------------------------------
                // STORE ANGLE IN THE 26×48 GRID
// ---------------------------------------------------------
                if (currentTileX >= 0 && currentTileX < 26 &&
    currentTileY >= 0 && currentTileY < 48)
                {
                    int angleInt = (int)Math.Round(angleDeg);
                    angleGrid[currentTileX, currentTileY] = angleInt;

                    if (angleForm != null && !angleForm.IsDisposed && angleForm.GridReady)
                    {
                        angleForm.SetCell(currentTileX, currentTileY, angleInt);
                        UpdateProgress();
                    }
                }

                // Update label
                lblPixel.Text = string.Format(
                    "Angle: {0:F2}°    Pixel: {1}, {2}    Tile: {3}, {4}",
                    lastAngleDeg,
                    real.X, real.Y,
                    currentTileX, currentTileY
                );
                angleLabel.Text = string.Format(
                    "Angle: {0:F2}°",lastAngleDeg);

            }
            else
            {
                // Third click resets
                angleP1 = snapped;
                angleP2 = null;
            }

            // Keep your crosshair logic
            lastSnapPoint = snapped;

            pictureBox.Invalidate();//attention 2
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Image Files|*.png;*.bmp;*.jpg;*.jpeg";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                boardImage = new Bitmap(ofd.FileName);
                pictureBox.Image = boardImage;

                zoom = 2.0f;
                ApplyZoom();
            }

            // Reset angle grid
            for (int x = 0; x < 26; x++)
                for (int y = 0; y < 48; y++)
                    angleGrid[x, y] = 256;

            UpdateProgress();

            // Reset spreadsheet UI
            /*for (int y = 0; y < 48; y++)
                for (int x = 0; x < 26; x++)
                    gridAngles[x, y].Value = "";*/
        }

        private void pictureBox_Paint(object sender, PaintEventArgs e)
        {
            if (boardImage == null)
                return;

            Graphics g = e.Graphics;

            // Draw scaled image
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.Half;

            int scaledW = (int)(boardImage.Width * zoom);
            int scaledH = (int)(boardImage.Height * zoom);

            g.DrawImage(boardImage, new Rectangle(0, 0, scaledW, scaledH));

            // --- GRID OVERLAY ---
            using (Pen gridPen = new Pen(Color.FromArgb(80, Color.Cyan), 1))
            {
                for (int x = 0; x <= boardImage.Width; x += TILE_SIZE)
                {
                    int sx = (int)(x * zoom);
                    g.DrawLine(gridPen, sx, 0, sx, scaledH);
                }

                for (int y = 0; y <= boardImage.Height; y += TILE_SIZE)
                {
                    int sy = (int)(y * zoom);
                    g.DrawLine(gridPen, 0, sy, scaledW, sy);
                }
            }

            // --- FILLED TILES (angle assigned) ---
            using (SolidBrush filledBrush = new SolidBrush(Color.FromArgb(100, Color.Green)))
            {
                for (int tx = 0; tx < 26; tx++)
                {
                    for (int ty = 0; ty < 48; ty++)
                    {
                        if (angleGrid[tx, ty] != 256)
                        {
                            int sx = (int)(tx * TILE_SIZE * zoom);
                            int sy = (int)(ty * TILE_SIZE * zoom);
                            int ss = (int)(TILE_SIZE * zoom);

                            g.FillRectangle(filledBrush, sx, sy, ss, ss);
                        }
                    }
                }
            }

            // --- SHADED TILES ---
            using (SolidBrush shadeBrush = new SolidBrush(Color.FromArgb(100, Color.Cyan)))
            {
                foreach (Point tile in shadedTiles)
                {
                    int sx = (int)(tile.X * zoom);
                    int sy = (int)(tile.Y * zoom);
                    int ss = (int)(TILE_SIZE * zoom);

                    g.FillRectangle(shadeBrush, sx, sy, ss, ss);
                }
            }

            // --- TILE HIGHLIGHT ---
            if (hoverTile.HasValue)
            {
                Point ht = hoverTile.Value;

                int hx = (int)(ht.X * zoom);
                int hy = (int)(ht.Y * zoom);
                int hs = (int)((zoom >= 4.0f ? 1 : TILE_SIZE) * zoom);

                using (Pen highlightPen = new Pen(Color.Yellow, 2))
                {
                    g.DrawRectangle(highlightPen, hx, hy, hs, hs);
                }
            }

            // --- SNAPPED POINT CROSSHAIR ---
            if (lastSnapPoint.HasValue)
            {
                Point sp = lastSnapPoint.Value;

                int sx = (int)(sp.X * zoom);
                int sy = (int)(sp.Y * zoom);

                using (Pen p = new Pen(Color.Red, 1))
                {
                    g.DrawLine(p, sx - 5, sy, sx + 5, sy);
                    g.DrawLine(p, sx, sy - 5, sx, sy + 5);
                }
            }

            // --- ANGLE MEASUREMENT LINE ---
            if (angleP1.HasValue && angleP2.HasValue)
            {
                Point p1 = angleP1.Value;
                Point p2 = angleP2.Value;

                int x1 = (int)(p1.X * zoom);
                int y1 = (int)(p1.Y * zoom);
                int x2 = (int)(p2.X * zoom);
                int y2 = (int)(p2.Y * zoom);

                using (Pen anglePen = new Pen(Color.Lime, 2))
                {
                    g.DrawLine(anglePen, x1, y1, x2, y2);
                }
            }
        }

        private void ApplyZoom()
        {
            if (boardImage == null)
                return;

            pictureBox.Width = (int)(boardImage.Width * zoom);
            pictureBox.Height = (int)(boardImage.Height * zoom);

            pictureBox.Invalidate();
        }

        private Point TranslateZoomClick(Point mousePoint)
        {
            if (boardImage == null)
                return mousePoint;

            int x = (int)(mousePoint.X / zoom);
            int y = (int)(mousePoint.Y / zoom);

            return new Point(x, y);
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private Point SnapToGrid(Point p)
        {
            int tileX = (p.X / TILE_SIZE) * TILE_SIZE + TILE_SIZE / 2;
            int tileY = (p.Y / TILE_SIZE) * TILE_SIZE + TILE_SIZE / 2;

            return new Point(tileX, tileY);
        }


        private void pictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (boardImage == null)
                return;

            // 1. Compute real pixel coords FIRST
            Point real = TranslateZoomClick(e.Location);

            // 2. Bounds check
            if (real.X < 0 || real.Y < 0 ||
                real.X >= boardImage.Width || real.Y >= boardImage.Height)
            {
                lblPixel.Text = "";
                hoverTile = null;

                pictureBox.Invalidate();//attention 4

                return;
            }

            // 3. Compute tile indices
            currentTileX = real.X / TILE_SIZE;
            currentTileY = real.Y / TILE_SIZE;

            // 4. Update label
            lblPixel.Text = string.Format(
            "Angle: {0:F2}°    Pixel: {1}, {2}    Tile: {3}, {4}",
            lastAngleDeg,
            real.X, real.Y,
            currentTileX, currentTileY
        );

            // 5. Update hover tile
            if (zoom >= 4.0f)
            {
                // Highlight individual pixel
                hoverTile = new Point(real.X, real.Y);
            }
            else
            {
                // Highlight full 8x8 tile
                hoverTile = new Point(currentTileX * TILE_SIZE, currentTileY * TILE_SIZE);
            }

            lblDebug.Text = string.Format(
    "Tile={0},{1}  Real={2},{3}",
    currentTileX,
    currentTileY,
    real.X,
    real.Y
);

            if (angleForm != null && !angleForm.IsDisposed && angleForm.GridReady && !cellLocked)
            {
                angleForm.HighlightCell(currentTileX, currentTileY, Color.Cyan);
            }

            pictureBox.Invalidate(); //attention 5
        }

        /*private void btnSnap_Click(object sender, EventArgs e)
        {
            snapEnabled = !snapEnabled;
            btnSnap.Text = snapEnabled ? "Snap: ON" : "Snap: OFF";
        }*/

        private void btnShowAngles_Click_1(object sender, EventArgs e)
        {
            if (angleForm == null || angleForm.IsDisposed)
            {
                angleForm = new FormAngles(this);
                angleForm.AngleGrid = angleGrid;
            }

            angleForm.Show();
            angleForm.BringToFront();

            if (control_form == null || control_form.IsDisposed)
            {
                control_form = new controlForm(angleForm);
            }

            control_form.Show();
            control_form.BringToFront();
        }


        public void UpdateProgress()
        {
            int total = 26 * 48;
            int filled = 0;

            for (int x = 0; x < 26; x++)
                for (int y = 0; y < 48; y++)
                    if (angleGrid[x, y] != 256)
                        filled++;

            int remaining = total - filled;
            float percent = (filled / (float)total) * 100f;

            lblProgress.Text = string.Format("Filled: {0} / {1}  |  Remaining: {2}  |  {3:F1}%",
                filled, total, remaining, percent);
        }

    }
}

