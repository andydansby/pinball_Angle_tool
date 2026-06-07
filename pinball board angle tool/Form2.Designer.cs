namespace pinball_board_angle_tool
{
    partial class FormAngles
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.gridAngles = new System.Windows.Forms.DataGridView();
            this.panel1 = new System.Windows.Forms.Panel();
            this.export_data = new System.Windows.Forms.Button();
            this.btnLoadCsv = new System.Windows.Forms.Button();
            this.btnSaveCsv = new System.Windows.Forms.Button();
            this.btnCopy = new System.Windows.Forms.Button();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.panel2 = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.gridAngles)).BeginInit();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // gridAngles
            // 
            this.gridAngles.AllowUserToAddRows = false;
            this.gridAngles.AllowUserToDeleteRows = false;
            this.gridAngles.AllowUserToResizeColumns = false;
            this.gridAngles.AllowUserToResizeRows = false;
            this.gridAngles.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridAngles.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridAngles.Location = new System.Drawing.Point(0, 0);
            this.gridAngles.MultiSelect = false;
            this.gridAngles.Name = "gridAngles";
            this.gridAngles.RowHeadersWidth = 40;
            this.gridAngles.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.gridAngles.Size = new System.Drawing.Size(512, 559);
            this.gridAngles.TabIndex = 0;
            this.gridAngles.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.gridAngles_CellEndEdit);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.export_data);
            this.panel1.Controls.Add(this.btnLoadCsv);
            this.panel1.Controls.Add(this.btnSaveCsv);
            this.panel1.Controls.Add(this.btnCopy);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(512, 41);
            this.panel1.TabIndex = 1;
            // 
            // export_data
            // 
            this.export_data.Location = new System.Drawing.Point(305, 11);
            this.export_data.Name = "export_data";
            this.export_data.Size = new System.Drawing.Size(75, 23);
            this.export_data.TabIndex = 3;
            this.export_data.Text = "Export";
            this.export_data.UseVisualStyleBackColor = true;
            this.export_data.Click += new System.EventHandler(this.export_data_Click);
            // 
            // btnLoadCsv
            // 
            this.btnLoadCsv.Location = new System.Drawing.Point(93, 12);
            this.btnLoadCsv.Name = "btnLoadCsv";
            this.btnLoadCsv.Size = new System.Drawing.Size(75, 23);
            this.btnLoadCsv.TabIndex = 2;
            this.btnLoadCsv.Text = "Load";
            this.btnLoadCsv.UseVisualStyleBackColor = true;
            this.btnLoadCsv.Click += new System.EventHandler(this.btnLoadCsv_Click_1);
            // 
            // btnSaveCsv
            // 
            this.btnSaveCsv.Location = new System.Drawing.Point(174, 11);
            this.btnSaveCsv.Name = "btnSaveCsv";
            this.btnSaveCsv.Size = new System.Drawing.Size(75, 23);
            this.btnSaveCsv.TabIndex = 1;
            this.btnSaveCsv.Text = "Save";
            this.btnSaveCsv.UseVisualStyleBackColor = true;
            this.btnSaveCsv.Click += new System.EventHandler(this.btnSaveCsv_Click_1);
            // 
            // btnCopy
            // 
            this.btnCopy.Location = new System.Drawing.Point(12, 12);
            this.btnCopy.Name = "btnCopy";
            this.btnCopy.Size = new System.Drawing.Size(75, 23);
            this.btnCopy.TabIndex = 0;
            this.btnCopy.Text = "Copy All";
            this.btnCopy.UseVisualStyleBackColor = true;
            this.btnCopy.Click += new System.EventHandler(this.btnCopy_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.gridAngles);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 41);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(512, 559);
            this.panel2.TabIndex = 2;
            // 
            // FormAngles
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(512, 600);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Name = "FormAngles";
            this.Text = "FormAngles";
            this.Load += new System.EventHandler(this.FormAngles_Load);
            this.Shown += new System.EventHandler(this.FormAngles_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.gridAngles)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.DataGridView gridAngles;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnCopy;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Button btnLoadCsv;
        private System.Windows.Forms.Button btnSaveCsv;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button export_data;
    }
}