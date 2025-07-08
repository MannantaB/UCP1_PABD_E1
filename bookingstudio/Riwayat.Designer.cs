namespace bookingstudio
{
    partial class Riwayat
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
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.label1 = new System.Windows.Forms.Label();
            this.btnexit = new System.Windows.Forms.Button();
            this.btnCetakLporan = new System.Windows.Forms.Button();
            this.btnRefresh = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.GridColor = System.Drawing.Color.DarkGoldenrod;
            this.dataGridView1.Location = new System.Drawing.Point(90, 93);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowHeadersWidth = 82;
            this.dataGridView1.RowTemplate.Height = 33;
            this.dataGridView1.Size = new System.Drawing.Size(1304, 441);
            this.dataGridView1.TabIndex = 0;
            this.dataGridView1.Click += new System.EventHandler(this.Riwayat_Load);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("Palatino Linotype", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.label1.Location = new System.Drawing.Point(548, 29);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(348, 44);
            this.label1.TabIndex = 1;
            this.label1.Text = "RIWAYAT BOOKING ";
            // 
            // btnexit
            // 
            this.btnexit.BackColor = System.Drawing.Color.Tan;
            this.btnexit.Location = new System.Drawing.Point(1247, 39);
            this.btnexit.Name = "btnexit";
            this.btnexit.Size = new System.Drawing.Size(147, 32);
            this.btnexit.TabIndex = 5;
            this.btnexit.Text = "EXIT";
            this.btnexit.UseVisualStyleBackColor = false;
            this.btnexit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // btnCetakLporan
            // 
            this.btnCetakLporan.BackColor = System.Drawing.Color.Tan;
            this.btnCetakLporan.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.btnCetakLporan.Location = new System.Drawing.Point(1125, 556);
            this.btnCetakLporan.Name = "btnCetakLporan";
            this.btnCetakLporan.Size = new System.Drawing.Size(269, 36);
            this.btnCetakLporan.TabIndex = 7;
            this.btnCetakLporan.Text = "Cetak Laporan ";
            this.btnCetakLporan.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnCetakLporan.UseVisualStyleBackColor = false;
            this.btnCetakLporan.Click += new System.EventHandler(this.btnCetak_Click);
            // 
            // btnRefresh
            // 
            this.btnRefresh.BackColor = System.Drawing.Color.Tan;
            this.btnRefresh.Location = new System.Drawing.Point(921, 556);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(186, 35);
            this.btnRefresh.TabIndex = 8;
            this.btnRefresh.Text = "Refresh";
            this.btnRefresh.UseVisualStyleBackColor = false;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // Riwayat
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::bookingstudio.Properties.Resources.Abstract_Brown_Studio_Professional_Portrait_Backdrop_M10_31___10_W_10_H_3_3m_;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(1478, 658);
            this.Controls.Add(this.btnRefresh);
            this.Controls.Add(this.btnCetakLporan);
            this.Controls.Add(this.btnexit);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.dataGridView1);
            this.Name = "Riwayat";
            this.Text = "RIWAYAT BOOKING";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnexit;
        private System.Windows.Forms.Button btnCetakLporan;
        private System.Windows.Forms.Button btnRefresh;
    }
}