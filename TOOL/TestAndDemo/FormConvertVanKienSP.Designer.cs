namespace RJCodeUI_M1.TestAndDemo
{
    partial class FormConvertVanKienSP
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
            this.rjLabel3 = new RJCodeUI_M1.RJControls.RJLabel();
            this.rjLabel4 = new RJCodeUI_M1.RJControls.RJLabel();
            this.grSP = new System.Windows.Forms.GroupBox();
            this.txtSoluong = new System.Windows.Forms.TextBox();
            this.txtIdEnd = new System.Windows.Forms.TextBox();
            this.txtIdStart = new System.Windows.Forms.TextBox();
            this.btnViewCateSQL = new RJCodeUI_M1.RJControls.RJButton();
            this.bntConvert = new RJCodeUI_M1.RJControls.RJButton();
            this.btnConnect = new RJCodeUI_M1.RJControls.RJButton();
            this.lblUrlSP = new RJCodeUI_M1.RJControls.RJLabel();
            this.txtSiteUrl = new System.Windows.Forms.TextBox();
            this.txtUsername = new System.Windows.Forms.TextBox();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.lblMatKhau = new RJCodeUI_M1.RJControls.RJLabel();
            this.lblUserName = new RJCodeUI_M1.RJControls.RJLabel();
            this.pnTreeCate24 = new System.Windows.Forms.Panel();
            this.treeCateNews = new System.Windows.Forms.TreeView();
            this.txtLog = new System.Windows.Forms.RichTextBox();
            this.btnDongBoPhienHop = new RJCodeUI_M1.RJControls.RJButton();
            this.pnlClientArea.SuspendLayout();
            this.grSP.SuspendLayout();
            this.pnTreeCate24.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlClientArea
            // 
            this.pnlClientArea.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.pnlClientArea.Controls.Add(this.txtLog);
            this.pnlClientArea.Controls.Add(this.pnTreeCate24);
            this.pnlClientArea.Controls.Add(this.grSP);
            this.pnlClientArea.Location = new System.Drawing.Point(1, 41);
            this.pnlClientArea.Size = new System.Drawing.Size(958, 523);
            // 
            // rjLabel3
            // 
            this.rjLabel3.AutoSize = true;
            this.rjLabel3.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.rjLabel3.Font = new System.Drawing.Font("Verdana", 9.5F);
            this.rjLabel3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.rjLabel3.LinkLabel = false;
            this.rjLabel3.Location = new System.Drawing.Point(7, 163);
            this.rjLabel3.Name = "rjLabel3";
            this.rjLabel3.Size = new System.Drawing.Size(60, 16);
            this.rjLabel3.Style = RJCodeUI_M1.RJControls.LabelStyle.Normal;
            this.rjLabel3.TabIndex = 7;
            this.rjLabel3.Text = "IdStart:";
            // 
            // rjLabel4
            // 
            this.rjLabel4.AutoSize = true;
            this.rjLabel4.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.rjLabel4.Font = new System.Drawing.Font("Verdana", 9.5F);
            this.rjLabel4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.rjLabel4.LinkLabel = false;
            this.rjLabel4.Location = new System.Drawing.Point(7, 201);
            this.rjLabel4.Name = "rjLabel4";
            this.rjLabel4.Size = new System.Drawing.Size(50, 16);
            this.rjLabel4.Style = RJCodeUI_M1.RJControls.LabelStyle.Normal;
            this.rjLabel4.TabIndex = 9;
            this.rjLabel4.Text = "IdEnd:";
            // 
            // grSP
            // 
            this.grSP.Controls.Add(this.btnDongBoPhienHop);
            this.grSP.Controls.Add(this.txtSoluong);
            this.grSP.Controls.Add(this.txtIdEnd);
            this.grSP.Controls.Add(this.txtIdStart);
            this.grSP.Controls.Add(this.btnViewCateSQL);
            this.grSP.Controls.Add(this.bntConvert);
            this.grSP.Controls.Add(this.btnConnect);
            this.grSP.Controls.Add(this.lblUrlSP);
            this.grSP.Controls.Add(this.txtSiteUrl);
            this.grSP.Controls.Add(this.txtUsername);
            this.grSP.Controls.Add(this.txtPassword);
            this.grSP.Controls.Add(this.lblMatKhau);
            this.grSP.Controls.Add(this.rjLabel4);
            this.grSP.Controls.Add(this.lblUserName);
            this.grSP.Controls.Add(this.rjLabel3);
            this.grSP.Dock = System.Windows.Forms.DockStyle.Left;
            this.grSP.Location = new System.Drawing.Point(0, 0);
            this.grSP.Name = "grSP";
            this.grSP.Size = new System.Drawing.Size(214, 523);
            this.grSP.TabIndex = 26;
            this.grSP.TabStop = false;
            this.grSP.Text = "Thông tin SP";
            // 
            // txtSoluong
            // 
            this.txtSoluong.Location = new System.Drawing.Point(74, 227);
            this.txtSoluong.Name = "txtSoluong";
            this.txtSoluong.Size = new System.Drawing.Size(100, 22);
            this.txtSoluong.TabIndex = 24;
            this.txtSoluong.Text = "2000";
            // 
            // txtIdEnd
            // 
            this.txtIdEnd.Location = new System.Drawing.Point(74, 198);
            this.txtIdEnd.Name = "txtIdEnd";
            this.txtIdEnd.Size = new System.Drawing.Size(100, 22);
            this.txtIdEnd.TabIndex = 23;
            this.txtIdEnd.Text = "10000";
            // 
            // txtIdStart
            // 
            this.txtIdStart.Location = new System.Drawing.Point(74, 158);
            this.txtIdStart.Name = "txtIdStart";
            this.txtIdStart.ScrollBars = System.Windows.Forms.ScrollBars.Horizontal;
            this.txtIdStart.Size = new System.Drawing.Size(100, 22);
            this.txtIdStart.TabIndex = 22;
            this.txtIdStart.Text = "400";
            // 
            // btnViewCateSQL
            // 
            this.btnViewCateSQL.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.btnViewCateSQL.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.btnViewCateSQL.BorderRadius = 0;
            this.btnViewCateSQL.BorderSize = 0;
            this.btnViewCateSQL.Design = RJCodeUI_M1.RJControls.ButtonDesign.Normal;
            this.btnViewCateSQL.FlatAppearance.BorderSize = 0;
            this.btnViewCateSQL.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(78)))), ((int)(((byte)(91)))), ((int)(((byte)(199)))));
            this.btnViewCateSQL.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(73)))), ((int)(((byte)(85)))), ((int)(((byte)(186)))));
            this.btnViewCateSQL.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnViewCateSQL.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnViewCateSQL.ForeColor = System.Drawing.Color.White;
            this.btnViewCateSQL.IconChar = FontAwesome.Sharp.IconChar.None;
            this.btnViewCateSQL.IconColor = System.Drawing.Color.White;
            this.btnViewCateSQL.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnViewCateSQL.IconSize = 24;
            this.btnViewCateSQL.Location = new System.Drawing.Point(13, 313);
            this.btnViewCateSQL.Name = "btnViewCateSQL";
            this.btnViewCateSQL.Size = new System.Drawing.Size(170, 40);
            this.btnViewCateSQL.Style = RJCodeUI_M1.RJControls.ControlStyle.Solid;
            this.btnViewCateSQL.TabIndex = 21;
            this.btnViewCateSQL.Text = "View Tree Cate";
            this.btnViewCateSQL.UseVisualStyleBackColor = false;
            this.btnViewCateSQL.Click += new System.EventHandler(this.btnViewCateSQL_Click);
            // 
            // bntConvert
            // 
            this.bntConvert.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.bntConvert.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.bntConvert.BorderRadius = 0;
            this.bntConvert.BorderSize = 0;
            this.bntConvert.Design = RJCodeUI_M1.RJControls.ButtonDesign.Normal;
            this.bntConvert.FlatAppearance.BorderSize = 0;
            this.bntConvert.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(78)))), ((int)(((byte)(91)))), ((int)(((byte)(199)))));
            this.bntConvert.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(73)))), ((int)(((byte)(85)))), ((int)(((byte)(186)))));
            this.bntConvert.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.bntConvert.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bntConvert.ForeColor = System.Drawing.Color.White;
            this.bntConvert.IconChar = FontAwesome.Sharp.IconChar.None;
            this.bntConvert.IconColor = System.Drawing.Color.White;
            this.bntConvert.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.bntConvert.IconSize = 24;
            this.bntConvert.Location = new System.Drawing.Point(13, 359);
            this.bntConvert.Name = "bntConvert";
            this.bntConvert.Size = new System.Drawing.Size(170, 40);
            this.bntConvert.Style = RJCodeUI_M1.RJControls.ControlStyle.Solid;
            this.bntConvert.TabIndex = 19;
            this.bntConvert.Text = "SQL to QH Kỳ họp 2024";
            this.bntConvert.UseVisualStyleBackColor = false;
            this.bntConvert.Click += new System.EventHandler(this.bntConvert_Click);
            // 
            // btnConnect
            // 
            this.btnConnect.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.btnConnect.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.btnConnect.BorderRadius = 0;
            this.btnConnect.BorderSize = 0;
            this.btnConnect.Design = RJCodeUI_M1.RJControls.ButtonDesign.Normal;
            this.btnConnect.FlatAppearance.BorderSize = 0;
            this.btnConnect.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(78)))), ((int)(((byte)(91)))), ((int)(((byte)(199)))));
            this.btnConnect.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(73)))), ((int)(((byte)(85)))), ((int)(((byte)(186)))));
            this.btnConnect.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnConnect.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnConnect.ForeColor = System.Drawing.Color.White;
            this.btnConnect.IconChar = FontAwesome.Sharp.IconChar.None;
            this.btnConnect.IconColor = System.Drawing.Color.White;
            this.btnConnect.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnConnect.IconSize = 24;
            this.btnConnect.Location = new System.Drawing.Point(13, 267);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(170, 40);
            this.btnConnect.Style = RJCodeUI_M1.RJControls.ControlStyle.Solid;
            this.btnConnect.TabIndex = 18;
            this.btnConnect.Text = "Convert SP to SQL";
            this.btnConnect.UseVisualStyleBackColor = false;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // lblUrlSP
            // 
            this.lblUrlSP.AutoSize = true;
            this.lblUrlSP.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.lblUrlSP.Font = new System.Drawing.Font("Verdana", 9.5F);
            this.lblUrlSP.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.lblUrlSP.LinkLabel = false;
            this.lblUrlSP.Location = new System.Drawing.Point(10, 32);
            this.lblUrlSP.Name = "lblUrlSP";
            this.lblUrlSP.Size = new System.Drawing.Size(100, 16);
            this.lblUrlSP.Style = RJCodeUI_M1.RJControls.LabelStyle.Normal;
            this.lblUrlSP.TabIndex = 17;
            this.lblUrlSP.Text = "Đường dẫn SP";
            // 
            // txtSiteUrl
            // 
            this.txtSiteUrl.Location = new System.Drawing.Point(6, 56);
            this.txtSiteUrl.Name = "txtSiteUrl";
            this.txtSiteUrl.Size = new System.Drawing.Size(185, 22);
            this.txtSiteUrl.TabIndex = 12;
            this.txtSiteUrl.Text = "https://noidung.quochoi.vn/content/vankien/Lists/DanhSachVanKien";
            // 
            // txtUsername
            // 
            this.txtUsername.Location = new System.Drawing.Point(85, 92);
            this.txtUsername.Name = "txtUsername";
            this.txtUsername.Size = new System.Drawing.Size(100, 22);
            this.txtUsername.TabIndex = 13;
            this.txtUsername.Text = "administrator";
            // 
            // txtPassword
            // 
            this.txtPassword.Location = new System.Drawing.Point(85, 125);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.Size = new System.Drawing.Size(100, 22);
            this.txtPassword.TabIndex = 14;
            this.txtPassword.Text = "AdminCTTDT@2021@)@!";
            // 
            // lblMatKhau
            // 
            this.lblMatKhau.AutoSize = true;
            this.lblMatKhau.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.lblMatKhau.Font = new System.Drawing.Font("Verdana", 9.5F);
            this.lblMatKhau.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.lblMatKhau.LinkLabel = false;
            this.lblMatKhau.Location = new System.Drawing.Point(7, 131);
            this.lblMatKhau.Name = "lblMatKhau";
            this.lblMatKhau.Size = new System.Drawing.Size(68, 16);
            this.lblMatKhau.Style = RJCodeUI_M1.RJControls.LabelStyle.Normal;
            this.lblMatKhau.TabIndex = 16;
            this.lblMatKhau.Text = "Mật khẩu";
            // 
            // lblUserName
            // 
            this.lblUserName.AutoSize = true;
            this.lblUserName.Cursor = System.Windows.Forms.Cursors.Arrow;
            this.lblUserName.Font = new System.Drawing.Font("Verdana", 9.5F);
            this.lblUserName.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(132)))), ((int)(((byte)(129)))), ((int)(((byte)(132)))));
            this.lblUserName.LinkLabel = false;
            this.lblUserName.Location = new System.Drawing.Point(7, 95);
            this.lblUserName.Name = "lblUserName";
            this.lblUserName.Size = new System.Drawing.Size(71, 16);
            this.lblUserName.Style = RJCodeUI_M1.RJControls.LabelStyle.Normal;
            this.lblUserName.TabIndex = 15;
            this.lblUserName.Text = "Tài khoản";
            // 
            // pnTreeCate24
            // 
            this.pnTreeCate24.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnTreeCate24.Controls.Add(this.treeCateNews);
            this.pnTreeCate24.Location = new System.Drawing.Point(755, 6);
            this.pnTreeCate24.Name = "pnTreeCate24";
            this.pnTreeCate24.Size = new System.Drawing.Size(200, 513);
            this.pnTreeCate24.TabIndex = 27;
            // 
            // treeCateNews
            // 
            this.treeCateNews.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.treeCateNews.Location = new System.Drawing.Point(0, 1);
            this.treeCateNews.Name = "treeCateNews";
            this.treeCateNews.Size = new System.Drawing.Size(200, 512);
            this.treeCateNews.TabIndex = 0;
            // 
            // txtLog
            // 
            this.txtLog.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtLog.Location = new System.Drawing.Point(220, 95);
            this.txtLog.Name = "txtLog";
            this.txtLog.Size = new System.Drawing.Size(529, 367);
            this.txtLog.TabIndex = 30;
            this.txtLog.Text = "";
            // 
            // btnDongBoPhienHop
            // 
            this.btnDongBoPhienHop.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.btnDongBoPhienHop.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.btnDongBoPhienHop.BorderRadius = 0;
            this.btnDongBoPhienHop.BorderSize = 0;
            this.btnDongBoPhienHop.Design = RJCodeUI_M1.RJControls.ButtonDesign.Normal;
            this.btnDongBoPhienHop.FlatAppearance.BorderSize = 0;
            this.btnDongBoPhienHop.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(78)))), ((int)(((byte)(91)))), ((int)(((byte)(199)))));
            this.btnDongBoPhienHop.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(73)))), ((int)(((byte)(85)))), ((int)(((byte)(186)))));
            this.btnDongBoPhienHop.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDongBoPhienHop.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDongBoPhienHop.ForeColor = System.Drawing.Color.White;
            this.btnDongBoPhienHop.IconChar = FontAwesome.Sharp.IconChar.None;
            this.btnDongBoPhienHop.IconColor = System.Drawing.Color.White;
            this.btnDongBoPhienHop.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnDongBoPhienHop.IconSize = 24;
            this.btnDongBoPhienHop.Location = new System.Drawing.Point(13, 405);
            this.btnDongBoPhienHop.Name = "btnDongBoPhienHop";
            this.btnDongBoPhienHop.Size = new System.Drawing.Size(170, 40);
            this.btnDongBoPhienHop.Style = RJCodeUI_M1.RJControls.ControlStyle.Solid;
            this.btnDongBoPhienHop.TabIndex = 25;
            this.btnDongBoPhienHop.Text = "SQL to QH Phieen họp 2024";
            this.btnDongBoPhienHop.UseVisualStyleBackColor = false;
            this.btnDongBoPhienHop.Click += new System.EventHandler(this.btnDongBoPhienHop_Click);
            // 
            // FormConvertVanKienSP
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.BorderSize = 1;
            this.Caption = "My Profile";
            this.ClientSize = new System.Drawing.Size(960, 565);
            this.FormIcon = FontAwesome.Sharp.IconChar.User;
            this.Name = "FormConvertVanKienSP";
            this.Padding = new System.Windows.Forms.Padding(1);
            this.Text = "My Profile";
            this.pnlClientArea.ResumeLayout(false);
            this.grSP.ResumeLayout(false);
            this.grSP.PerformLayout();
            this.pnTreeCate24.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private RJControls.RJLabel rjLabel3;
        private RJControls.RJLabel rjLabel4;
        private System.Windows.Forms.GroupBox grSP;
        private RJControls.RJButton btnViewCateSQL;
        private RJControls.RJButton bntConvert;
        private RJControls.RJButton btnConnect;
        private RJControls.RJLabel lblUrlSP;
        private System.Windows.Forms.TextBox txtSiteUrl;
        private System.Windows.Forms.TextBox txtUsername;
        private System.Windows.Forms.TextBox txtPassword;
        private RJControls.RJLabel lblMatKhau;
        private RJControls.RJLabel lblUserName;
        private System.Windows.Forms.TextBox txtIdEnd;
        private System.Windows.Forms.TextBox txtIdStart;
        private System.Windows.Forms.TextBox txtSoluong;
        private System.Windows.Forms.Panel pnTreeCate24;
        private System.Windows.Forms.TreeView treeCateNews;
        private System.Windows.Forms.RichTextBox txtLog;
        private RJControls.RJButton btnDongBoPhienHop;
    }
}