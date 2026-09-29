namespace RJCodeUI_M1.TestAndDemo
{
    partial class FormHoiDap
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
            this.label1 = new System.Windows.Forms.Label();
            this.txtLinkPageHoiDap = new System.Windows.Forms.TextBox();
            this.txtSoTrang = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.txtLinkChiTiet = new System.Windows.Forms.TextBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnKetNoi = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.cboDanhMucHoiDap = new System.Windows.Forms.ComboBox();
            this.pnlClientArea.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlClientArea
            // 
            this.pnlClientArea.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.pnlClientArea.Controls.Add(this.cboDanhMucHoiDap);
            this.pnlClientArea.Controls.Add(this.label4);
            this.pnlClientArea.Controls.Add(this.btnKetNoi);
            this.pnlClientArea.Controls.Add(this.btnSave);
            this.pnlClientArea.Controls.Add(this.txtLinkChiTiet);
            this.pnlClientArea.Controls.Add(this.label3);
            this.pnlClientArea.Controls.Add(this.label2);
            this.pnlClientArea.Controls.Add(this.txtSoTrang);
            this.pnlClientArea.Controls.Add(this.txtLinkPageHoiDap);
            this.pnlClientArea.Controls.Add(this.label1);
            this.pnlClientArea.Location = new System.Drawing.Point(1, 41);
            this.pnlClientArea.Size = new System.Drawing.Size(958, 556);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(52, 87);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(134, 20);
            this.label1.TabIndex = 0;
            this.label1.Text = "Link page hỏi đáp";
            // 
            // txtLinkPageHoiDap
            // 
            this.txtLinkPageHoiDap.Location = new System.Drawing.Point(217, 87);
            this.txtLinkPageHoiDap.Name = "txtLinkPageHoiDap";
            this.txtLinkPageHoiDap.Size = new System.Drawing.Size(631, 25);
            this.txtLinkPageHoiDap.TabIndex = 1;
            this.txtLinkPageHoiDap.Text = "https://www.vr.org.vn/hoi-dap/Pages/default.aspx?search=ad&Category=14&Page=";
            // 
            // txtSoTrang
            // 
            this.txtSoTrang.Location = new System.Drawing.Point(217, 134);
            this.txtSoTrang.Name = "txtSoTrang";
            this.txtSoTrang.Size = new System.Drawing.Size(135, 25);
            this.txtSoTrang.TabIndex = 2;
            this.txtSoTrang.Text = "7";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(52, 139);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(70, 20);
            this.label2.TabIndex = 3;
            this.label2.Text = "Số trang";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(52, 33);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(88, 20);
            this.label3.TabIndex = 4;
            this.label3.Text = "Link chi tiết";
            // 
            // txtLinkChiTiet
            // 
            this.txtLinkChiTiet.Location = new System.Drawing.Point(217, 30);
            this.txtLinkChiTiet.Name = "txtLinkChiTiet";
            this.txtLinkChiTiet.Size = new System.Drawing.Size(631, 25);
            this.txtLinkChiTiet.TabIndex = 5;
            this.txtLinkChiTiet.Text = "https://www.vr.org.vn/hoi-dap/Pages/default.aspx";
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(217, 227);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(186, 43);
            this.btnSave.TabIndex = 6;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnKetNoi
            // 
            this.btnKetNoi.Location = new System.Drawing.Point(436, 227);
            this.btnKetNoi.Name = "btnKetNoi";
            this.btnKetNoi.Size = new System.Drawing.Size(248, 43);
            this.btnKetNoi.TabIndex = 7;
            this.btnKetNoi.Text = "Kết nối danh mục hỏi đáp";
            this.btnKetNoi.UseVisualStyleBackColor = true;
            this.btnKetNoi.Click += new System.EventHandler(this.btnKetNoi_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(52, 188);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(138, 20);
            this.label4.TabIndex = 8;
            this.label4.Text = "Danh mục hỏi đáp";
            // 
            // cboDanhMucHoiDap
            // 
            this.cboDanhMucHoiDap.FormattingEnabled = true;
            this.cboDanhMucHoiDap.Location = new System.Drawing.Point(217, 185);
            this.cboDanhMucHoiDap.Name = "cboDanhMucHoiDap";
            this.cboDanhMucHoiDap.Size = new System.Drawing.Size(631, 28);
            this.cboDanhMucHoiDap.TabIndex = 9;
            // 
            // FormHoiDap
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.BorderSize = 1;
            this.Caption = "FormHoiDap";
            this.ClientSize = new System.Drawing.Size(960, 598);
            this.Name = "FormHoiDap";
            this.Padding = new System.Windows.Forms.Padding(1);
            this.Text = "FormHoiDap";
            this.pnlClientArea.ResumeLayout(false);
            this.pnlClientArea.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox txtLinkChiTiet;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtSoTrang;
        private System.Windows.Forms.TextBox txtLinkPageHoiDap;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnKetNoi;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.ComboBox cboDanhMucHoiDap;
    }
}