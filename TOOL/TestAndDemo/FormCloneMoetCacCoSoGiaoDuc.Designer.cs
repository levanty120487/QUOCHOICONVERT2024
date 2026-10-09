namespace RJCodeUI_M1.TestAndDemo
{
    partial class FormCloneMoetCacCoSoGiaoDuc
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.txtWebUrl = new System.Windows.Forms.TextBox();
            this.btnReadData = new System.Windows.Forms.Button();
            this.btnSaveData = new System.Windows.Forms.Button();
            this.dgvData = new System.Windows.Forms.DataGridView();
            this.label2 = new System.Windows.Forms.Label();
            this.pnlClientArea.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvData)).BeginInit();
            this.SuspendLayout();
            // 
            // pnlClientArea
            // 
            this.pnlClientArea.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.pnlClientArea.Controls.Add(this.label2);
            this.pnlClientArea.Controls.Add(this.dgvData);
            this.pnlClientArea.Controls.Add(this.btnSaveData);
            this.pnlClientArea.Controls.Add(this.btnReadData);
            this.pnlClientArea.Controls.Add(this.txtWebUrl);
            this.pnlClientArea.Location = new System.Drawing.Point(1, 41);
            this.pnlClientArea.Size = new System.Drawing.Size(958, 755);
            this.pnlClientArea.Paint += new System.Windows.Forms.PaintEventHandler(this.pnlClientArea_Paint);
            // 
            // txtWebUrl
            // 
            this.txtWebUrl.Location = new System.Drawing.Point(93, 0);
            this.txtWebUrl.MaxLength = 0;
            this.txtWebUrl.Multiline = true;
            this.txtWebUrl.Name = "txtWebUrl";
            this.txtWebUrl.Size = new System.Drawing.Size(791, 123);
            this.txtWebUrl.TabIndex = 1;
            // 
            // btnReadData
            // 
            this.btnReadData.Location = new System.Drawing.Point(20, 633);
            this.btnReadData.Name = "btnReadData";
            this.btnReadData.Size = new System.Drawing.Size(100, 30);
            this.btnReadData.TabIndex = 7;
            this.btnReadData.Text = "Đọc dữ liệu";
            this.btnReadData.UseVisualStyleBackColor = true;
            this.btnReadData.Click += new System.EventHandler(this.btnReadData_Click);
            // 
            // btnSaveData
            // 
            this.btnSaveData.Location = new System.Drawing.Point(136, 633);
            this.btnSaveData.Name = "btnSaveData";
            this.btnSaveData.Size = new System.Drawing.Size(100, 30);
            this.btnSaveData.TabIndex = 8;
            this.btnSaveData.Text = "Lưu dữ liệu";
            this.btnSaveData.UseVisualStyleBackColor = true;
            this.btnSaveData.Click += new System.EventHandler(this.btnSaveData_Click);
            // 
            // dgvData
            // 
            this.dgvData.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvData.Location = new System.Drawing.Point(20, 314);
            this.dgvData.Name = "dgvData";
            this.dgvData.RowHeadersWidth = 51;
            this.dgvData.RowTemplate.Height = 24;
            this.dgvData.Size = new System.Drawing.Size(864, 296);
            this.dgvData.TabIndex = 9;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(30, 3);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(46, 20);
            this.label2.TabIndex = 7;
            this.label2.Text = "Html:";
            // 
            // FormCloneMoetCacCoSoGiaoDuc
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.BorderSize = 1;
            this.Caption = "Clone Moet News";
            this.ClientSize = new System.Drawing.Size(960, 797);
            this.Name = "FormCloneMoetCacCoSoGiaoDuc";
            this.Padding = new System.Windows.Forms.Padding(1);
            this.Text = "Clone Moet News";
            this.Load += new System.EventHandler(this.FormCloneMoetCacCoSoGiaoDuc_Load);
            this.pnlClientArea.ResumeLayout(false);
            this.pnlClientArea.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvData)).EndInit();
            this.ResumeLayout(false);

        }
        private System.Windows.Forms.TextBox txtWebUrl;
        private System.Windows.Forms.Button btnReadData;
        private System.Windows.Forms.Button btnSaveData;
        private System.Windows.Forms.DataGridView dgvData;
        private System.Windows.Forms.Label label2;
    }
}
