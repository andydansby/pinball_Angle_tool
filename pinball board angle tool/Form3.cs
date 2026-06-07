using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace pinball_board_angle_tool
{
    public partial class controlForm : Form
    {
        private FormAngles angleForm;
        private Panel scrollPanel;

        public controlForm(FormAngles angleForm)
        {
            InitializeComponent();
            this.angleForm = angleForm;

            // Create persistent scroll panel
            scrollPanel = new Panel();
            scrollPanel.Dock = DockStyle.Fill;
            scrollPanel.AutoScroll = true;
            this.Controls.Add(scrollPanel);

            this.FormBorderStyle = FormBorderStyle.Sizable;
        }

        private void controlForm_Load(object sender, EventArgs e)
        {
            BuildTileButtons();
        }

        public void BuildTileButtons()
        {
            scrollPanel.Controls.Clear();

            List<Tuple<int, string>> tiles = new List<Tuple<int, string>>();

            tiles.Add(Tuple.Create(0, "0-180 - Wall Angle"));
            tiles.Add(Tuple.Create(181, "181 - Triangle Tip"));
            tiles.Add(Tuple.Create(190, "190 - Lane Side Wall 90°"));
            tiles.Add(Tuple.Create(191, "191 - Lane Top 180°"));
            tiles.Add(Tuple.Create(192, "192 - Lane Bottom 180°"));
            tiles.Add(Tuple.Create(194, "194 - Lane Switch 1"));
            tiles.Add(Tuple.Create(195, "195 - Lane Switch 2"));
            tiles.Add(Tuple.Create(196, "196 - Lane Switch 3"));
            tiles.Add(Tuple.Create(200, "200 - Bumper 45°"));
            tiles.Add(Tuple.Create(201, "201 - Bumper 90°"));
            tiles.Add(Tuple.Create(202, "202 - Bumper 135°"));
            tiles.Add(Tuple.Create(203, "203 - Bumper 180°"));
            tiles.Add(Tuple.Create(204, "204 - Bumper Center"));
            tiles.Add(Tuple.Create(210, "210 - Drop Target 1"));
            tiles.Add(Tuple.Create(211, "211 - Drop Target 2"));
            tiles.Add(Tuple.Create(212, "212 - Drop Target 3"));
            tiles.Add(Tuple.Create(216, "216 - Target 1"));
            tiles.Add(Tuple.Create(217, "217 - Target 2"));
            tiles.Add(Tuple.Create(218, "218 - Target 3"));
            tiles.Add(Tuple.Create(221, "221 - Ejector 1"));
            tiles.Add(Tuple.Create(222, "222 - Ejector 2"));

            tiles.Add(Tuple.Create(225, "225 - SlingShot All Types Passive Top 0°"));//
            tiles.Add(Tuple.Create(226, "226 - Reserve / Not assigned"));//
            tiles.Add(Tuple.Create(227, "227 - Reserve / Not assigned"));//
            tiles.Add(Tuple.Create(228, "228 - Reserve / Not assigned"));//
            tiles.Add(Tuple.Create(229, "229 - Reserve / Not assigned"));//
            
            tiles.Add(Tuple.Create(230, "230 - SlingShot All Types Passive Front/Back 90°"));//
            tiles.Add(Tuple.Create(231, "231 - SlingShot Passive Bottom / Type 4 Top Anchor 0°"));
            tiles.Add(Tuple.Create(232, "232 - SlingShot Type 1,2,3 Passive Bottom Left 135°"));//
            tiles.Add(Tuple.Create(233, "233 - SlingShot Type 1,2,3 Passive Bottom Right 45°"));//

            tiles.Add(Tuple.Create(234, "234 - SlingShot Type 3,4 Passive Back Left 45°"));
            tiles.Add(Tuple.Create(235, "235 - SlingShot Type 3,4 Passive Back Right 135°"));
            tiles.Add(Tuple.Create(236, "236 - SlingShot Type 4 Passive Bottom Right 0°"));
            tiles.Add(Tuple.Create(237, "237 - Reserve / Not assigned"));//
            tiles.Add(Tuple.Create(238, "238 - Reserve / Not assigned"));//
            tiles.Add(Tuple.Create(239, "239 - Reserve / Not assigned"));//

            tiles.Add(Tuple.Create(240, "240 - SlingShot Type 1 Active Front Left 110°"));//
            tiles.Add(Tuple.Create(241, "241 - SlingShot Type 1 Active Front Right 70°"));//
            tiles.Add(Tuple.Create(242, "242 - SlingShot Type 2 Active Front Left 150°"));
            tiles.Add(Tuple.Create(243, "243 - SlingShot Type 2 Active Front Right 30°"));
            tiles.Add(Tuple.Create(244, "244 - SlingShot Type 3 Active Front Top 70°"));
            tiles.Add(Tuple.Create(245, "245 - SlingShot Type 3 Active Front Bottom 110°"));
            tiles.Add(Tuple.Create(246, "246 - SlingShot Type 4 Active Front Bottom 25°"));
            tiles.Add(Tuple.Create(247, "247 - SlingShot Type 4 Active Front Top 155°"));

            tiles.Add(Tuple.Create(250, "250 - Going Down 45°"));
            tiles.Add(Tuple.Create(251, "251 - Going Down 135°"));
            tiles.Add(Tuple.Create(253, "253 - Flipper Area"));
            tiles.Add(Tuple.Create(254, "254 - Dead Ball"));
            tiles.Add(Tuple.Create(255, "255 - Free Area"));

            int y = 10;

            foreach (Tuple<int, string> tile in tiles)
            {
                int val = tile.Item1;
                string label = tile.Item2;

                Button btn = new Button();
                btn.Text = label;
                btn.Size = new Size(300, 28);
                btn.Location = new Point(10, y);
                btn.TextAlign = ContentAlignment.MiddleLeft;
                btn.Tag = val;
                btn.Click += new EventHandler(TileButton_Click);

                // Color coding
                if (val == 0)
                    btn.BackColor = Color.FromArgb(181, 181, 181);
                else if (val == 181)
                    btn.BackColor = Color.FromArgb(150, 150, 150);
                else if (val >= 190 && val <= 192)
                    btn.BackColor = Color.FromArgb(255, 255, 180);
                else if (val >= 193 && val <= 196)
                    btn.BackColor = Color.FromArgb(255, 255, 120);
                else if (val >= 200 && val <= 204)
                    btn.BackColor = Color.FromArgb(180, 220, 255);
                else if (val >= 210 && val <= 212)
                    btn.BackColor = Color.FromArgb(200, 255, 200);
                else if (val >= 216 && val <= 218)
                    btn.BackColor = Color.FromArgb(200, 255, 110);
                else if (val >= 221 && val <= 222)
                    btn.BackColor = Color.FromArgb(100, 255, 100);
                else if (val >= 225 && val <= 228)
                    btn.BackColor = Color.FromArgb(255, 220, 180);
                else if (val >= 230 && val <= 239)
                    btn.BackColor = Color.FromArgb(250, 100, 100);
                else if (val >= 240 && val <= 249)
                    btn.BackColor = Color.FromArgb(255, 50, 50);
                else if (val >= 250 && val <= 251)
                    btn.BackColor = Color.FromArgb(110, 160, 220);
                else if (val == 253)
                    btn.BackColor = Color.FromArgb(240, 88, 88);
                else if (val == 254)
                    btn.BackColor = Color.FromArgb(240, 8, 8);
                else if (val == 255)
                    btn.BackColor = Color.FromArgb(104, 222, 104);

                scrollPanel.Controls.Add(btn);
                y += 32;
            }

            this.MinimumSize = new Size(300, 600);
            this.MaximizeBox = true;
            this.Text = "Tile Types";
        }

        private void TileButton_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            int val = (int)btn.Tag;

            if (angleForm != null && !angleForm.IsDisposed)
            {
                angleForm.WriteToSelectedCell(val);
            }
        }
    }
}
