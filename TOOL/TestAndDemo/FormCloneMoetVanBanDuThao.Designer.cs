namespace RJCodeUI_M1.TestAndDemo
{
    partial class FormCloneMoetVanBanDuThao
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
            this.label1 = new System.Windows.Forms.Label();
            this.txtFolderChua = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.pnlClientArea.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvData)).BeginInit();
            this.SuspendLayout();
            // 
            // pnlClientArea
            // 
            this.pnlClientArea.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.pnlClientArea.Controls.Add(this.label12);
            this.pnlClientArea.Controls.Add(this.txtFolderChua);
            this.pnlClientArea.Controls.Add(this.label1);
            this.pnlClientArea.Controls.Add(this.dgvData);
            this.pnlClientArea.Controls.Add(this.btnSaveData);
            this.pnlClientArea.Controls.Add(this.btnReadData);
            this.pnlClientArea.Controls.Add(this.txtWebUrl);
            this.pnlClientArea.Location = new System.Drawing.Point(1, 41);
            this.pnlClientArea.Size = new System.Drawing.Size(958, 755);
            // 
            // txtWebUrl
            // 
            this.txtWebUrl.Location = new System.Drawing.Point(147, 9);
            this.txtWebUrl.MaxLength = 0;
            this.txtWebUrl.Multiline = true;
            this.txtWebUrl.Name = "txtWebUrl";
            this.txtWebUrl.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtWebUrl.Size = new System.Drawing.Size(757, 150);
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
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(23, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(98, 20);
            this.label1.TabIndex = 8;
            this.label1.Text = "Nhập HTML:";
            // 
            // txtFolderChua
            // 
            this.txtFolderChua.Location = new System.Drawing.Point(147, 274);
            this.txtFolderChua.Name = "txtFolderChua";
            this.txtFolderChua.Size = new System.Drawing.Size(250, 25);
            this.txtFolderChua.TabIndex = 18;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(24, 274);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(97, 20);
            this.label12.TabIndex = 19;
            this.label12.Text = "Folder chứa:";
            // 
            // FormCloneMoetVanBanDuThao
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.BorderSize = 1;
            this.Caption = "Clone Moet News";
            this.ClientSize = new System.Drawing.Size(960, 797);
            this.Name = "FormCloneMoetVanBanDuThao";
            this.Padding = new System.Windows.Forms.Padding(1);
            this.Text = "Clone Moet News";
            this.Load += new System.EventHandler(this.FormCloneMoetVanBanDuThao_Load);
            this.pnlClientArea.ResumeLayout(false);
            this.pnlClientArea.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvData)).EndInit();
            this.ResumeLayout(false);

        }
        private System.Windows.Forms.TextBox txtWebUrl;
        private System.Windows.Forms.Button btnReadData;
        private System.Windows.Forms.Button btnSaveData;
        private System.Windows.Forms.DataGridView dgvData;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox txtFolderChua;
    }
}
