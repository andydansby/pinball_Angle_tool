using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace pinball_board_angle_tool
{
    public partial class FormAngles : Form
    {
        private Form1 mainForm;
        private int selectedCol = -1;
        private int selectedRow = -1;
        private int lastX = -1;
        private int lastY = -1;

        public int[,] AngleGrid { get; set; }
        public bool GridReady { get; private set; }

        public FormAngles(Form1 main)
        {
            InitializeComponent();
            mainForm = main;
        }

        private void FormAngles_Load(object sender, EventArgs e)
        {
            gridAngles.ColumnCount = 26;
            gridAngles.RowCount = 48;

            for (int x = 0; x < 26; x++)
            {
                gridAngles.Columns[x].HeaderText = x.ToString();
                gridAngles.Columns[x].Width = 30;
            }

            gridAngles.ClipboardCopyMode =
                DataGridViewClipboardCopyMode.EnableAlwaysIncludeHeaderText;

            gridAngles.RowHeadersWidth = 45;
            gridAngles.RowHeadersDefaultCellStyle.Alignment =
                DataGridViewContentAlignment.MiddleCenter;

            for (int y = 0; y < 48; y++)
                gridAngles.Rows[y].HeaderCell.Value = y.ToString();

            gridAngles.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;

            //gridAngles.SelectionChanged += gridAngles_SelectionChanged;
            gridAngles.CellClick += gridAngles_CellClick;
        }

        private void FormAngles_Shown(object sender, EventArgs e)
        {
            GridReady = true;
            gridAngles.EditMode = DataGridViewEditMode.EditOnKeystrokeOrF2;
        }

        public void SelectCell(int x, int y)
        {
            gridAngles.CurrentCell = gridAngles[x, y];
            selectedCol = x;
            selectedRow = y;
        }

        private void gridAngles_SelectionChanged(object sender, EventArgs e)
        {
            if (gridAngles.CurrentCell != null)
            {
                selectedCol = gridAngles.CurrentCell.ColumnIndex;
                selectedRow = gridAngles.CurrentCell.RowIndex;
            }
        }

        private void gridAngles_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex >= 0 && e.RowIndex >= 0)
            {
                selectedCol = e.ColumnIndex;
                selectedRow = e.RowIndex;
            }
            mainForm.UnlockCell();
        }

        public void SetCell(int x, int y, int val)
        {
            if (AngleGrid == null) return;
            gridAngles[x, y].Value = val.ToString("D3");
            gridAngles[x, y].Style.BackColor = Color.Green;
            gridAngles[x, y].Style.ForeColor = Color.White;
            AngleGrid[x, y] = val;
        }

        public void WriteToSelectedCell(int val)
        {
            //MessageBox.Show(string.Format("selectedCol={0}  selectedRow={1}  val={2}", selectedCol, selectedRow, val));

            if (selectedCol < 0 || selectedRow < 0)
            {
                //MessageBox.Show("Please select a cell in the angle grid first.", "No Cell Selected", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            SetCell(selectedCol, selectedRow, val);

            mainForm.UpdateProgress();
        }

        public void HighlightCell(int x, int y, Color color)
        {
            if (AngleGrid == null) return;

            if (lastX >= 0 && lastY >= 0)
            {
                if (AngleGrid[lastX, lastY] != 256)
                {
                    gridAngles[lastX, lastY].Style.BackColor = Color.Green;
                    gridAngles[lastX, lastY].Style.ForeColor = Color.White;
                }
                else
                {
                    gridAngles[lastX, lastY].Style.BackColor = Color.White;
                    gridAngles[lastX, lastY].Style.ForeColor = Color.Black;
                }
            }

            gridAngles[x, y].Style.BackColor = color;
            lastX = x;
            lastY = y;
        }

        private void gridAngles_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            object cellValue = gridAngles[e.ColumnIndex, e.RowIndex].Value;
            string text = cellValue == null ? "" : cellValue.ToString();

            int val;
            if (int.TryParse(text, out val))
            {
                AngleGrid[e.ColumnIndex, e.RowIndex] = val;
                gridAngles[e.ColumnIndex, e.RowIndex].Style.BackColor = Color.Green;
                gridAngles[e.ColumnIndex, e.RowIndex].Style.ForeColor = Color.White;
            }
            else
            {
                AngleGrid[e.ColumnIndex, e.RowIndex] = 256;
                gridAngles[e.ColumnIndex, e.RowIndex].Style.BackColor = Color.White;
                gridAngles[e.ColumnIndex, e.RowIndex].Style.ForeColor = Color.Black;
            }
            mainForm.UpdateProgress();
        }

        private void gridAngles_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(e.FormattedValue.ToString()))
                return;

            int val;
            if (!int.TryParse(e.FormattedValue.ToString(), out val) || val < 0 || val > 255)
            {
                MessageBox.Show("Please enter a number from 0 to 255.");
                e.Cancel = true;
            }
        }

        private void SafeSetClipboard(string text)
        {
            int retries = 10;
            while (retries > 0)
            {
                try
                {
                    DataObject dataObj = new DataObject();
                    dataObj.SetData(DataFormats.Text, text);
                    Clipboard.SetDataObject(dataObj, true);
                    return;
                }
                catch (System.Runtime.InteropServices.ExternalException)
                {
                    System.Threading.Thread.Sleep(50);
                    Application.DoEvents();
                    retries--;
                }
            }
            MessageBox.Show("Clipboard is busy. Try again.");
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            if (AngleGrid == null)
            {
                MessageBox.Show("AngleGrid is not initialized.");
                return;
            }

            StringBuilder sb = new StringBuilder();
            for (int y = 0; y < 48; y++)
            {
                for (int x = 0; x < 26; x++)
                {
                    int val = AngleGrid[x, y];
                    sb.Append(val == 256 ? "" : val.ToString());
                    if (x < 25)
                        sb.Append("\t");
                }
                sb.AppendLine();
            }
            SafeSetClipboard(sb.ToString());
        }

        private void SaveCsv(string path)
        {
            using (StreamWriter sw = new StreamWriter(path))
            {
                for (int y = 0; y < 48; y++)
                {
                    for (int x = 0; x < 26; x++)
                    {
                        object cell = gridAngles[x, y].Value;
                        string text = cell == null ? "" : cell.ToString().Trim();
                        if (text == "")
                            text = "XXX";
                        sw.Write(text);
                        if (x < 25)
                            sw.Write(",");
                    }
                    sw.WriteLine();
                }
            }
        }

        private void LoadCsv(string path)
        {
            string[] lines = File.ReadAllLines(path);
            for (int y = 0; y < 48 && y < lines.Length; y++)
            {
                string[] parts = lines[y].Split(',');
                for (int x = 0; x < 26 && x < parts.Length; x++)
                {
                    string text = parts[x].Trim().ToUpper();
                    if (text == "XXX")
                    {
                        gridAngles[x, y].Value = "";
                        gridAngles[x, y].Style.BackColor = Color.White;
                        gridAngles[x, y].Style.ForeColor = Color.Black;
                        AngleGrid[x, y] = 256;
                    }
                    else
                    {
                        int val;
                        if (int.TryParse(text, out val))
                        {
                            SetCell(x, y, val);
                        }
                        else
                        {
                            gridAngles[x, y].Value = "";
                            gridAngles[x, y].Style.BackColor = Color.White;
                            gridAngles[x, y].Style.ForeColor = Color.Black;
                            AngleGrid[x, y] = 256;
                        }
                    }
                }
            }
        }

        private void btnSaveCsv_Click_1(object sender, EventArgs e)
        {
            gridAngles.EndEdit();
            this.Validate();
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "CSV Files (*.csv)|*.csv";
            if (dlg.ShowDialog() == DialogResult.OK)
                SaveCsv(dlg.FileName);
        }

        private void btnLoadCsv_Click_1(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "CSV Files (*.csv)|*.csv";
            if (dlg.ShowDialog() == DialogResult.OK)
                LoadCsv(dlg.FileName);
        }

        private void export_data_Click(object sender, EventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "SSV Files (*.ssv)|*.ssv";
            dlg.Title = "Export Angle Data | Space Seperated Value";

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                using (StreamWriter sw = new StreamWriter(dlg.FileName))
                {
                    for (int y = 0; y < 48; y++)
                    {
                        for (int x = 0; x < 26; x++)
                        {
                            int val = AngleGrid[x, y];
                            if (val == 256)
                                val = 255;
                            sw.Write(val.ToString("D3"));
                            if (x < 25)
                                sw.Write(" ");
                        }
                        sw.Write("\r\n");
                    }
                }
                MessageBox.Show("Export complete!", "Export",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}