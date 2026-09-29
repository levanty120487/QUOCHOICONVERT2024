namespace RJCodeUI_M1.TestAndDemo
{
    partial class GetCategorySP
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
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.txtUsername = new System.Windows.Forms.TextBox();
            this.txtSiteUrl = new System.Windows.Forms.TextBox();
            this.lblUserName = new RJCodeUI_M1.RJControls.RJLabel();
            this.lblMatKhau = new RJCodeUI_M1.RJControls.RJLabel();
            this.grSP = new System.Windows.Forms.GroupBox();
            this.btnDongBoPh = new RJCodeUI_M1.RJControls.RJButton();
            this.btnConvert = new RJCodeUI_M1.RJControls.RJButton();
            this.btnViewCateSQL = new RJCodeUI_M1.RJControls.RJButton();
            this.btnDeleteAll = new RJCodeUI_M1.RJControls.RJButton();
            this.bntConvert = new RJCodeUI_M1.RJControls.RJButton();
            this.btnConnect = new RJCodeUI_M1.RJControls.RJButton();
            this.lblUrlSP = new RJCodeUI_M1.RJControls.RJLabel();
            this.Rjpanelcate = new RJCodeUI_M1.RJControls.RJPanel();
            this.grCateSP = new System.Windows.Forms.GroupBox();
            this.rjProgressBar1 = new RJCodeUI_M1.RJControls.RJProgressBar();
            this.rgDongbo = new System.Windows.Forms.GroupBox();
            this.pnTreeSql = new RJCodeUI_M1.RJControls.RJPanel();
            this.grLog = new System.Windows.Forms.GroupBox();
            this.txtLog = new System.Windows.Forms.RichTextBox();
            this.processBar = new RJCodeUI_M1.RJControls.RJProgressBar();
            this.btnviewTreePhienHop = new RJCodeUI_M1.RJControls.RJButton();
            this.pnlClientArea.SuspendLayout();
            this.grSP.SuspendLayout();
            this.grCateSP.SuspendLayout();
            this.rgDongbo.SuspendLayout();
            this.grLog.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlClientArea
            // 
            this.pnlClientArea.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.pnlClientArea.Controls.Add(this.processBar);
            this.pnlClientArea.Controls.Add(this.grLog);
            this.pnlClientArea.Controls.Add(this.rgDongbo);
            this.pnlClientArea.Controls.Add(this.grCateSP);
            this.pnlClientArea.Controls.Add(this.grSP);
            this.pnlClientArea.Location = new System.Drawing.Point(1, 41);
            this.pnlClientArea.Size = new System.Drawing.Size(958, 523);
            // 
            // txtPassword
            // 
            this.txtPassword.Location = new System.Drawing.Point(85, 125);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.Size = new System.Drawing.Size(100, 22);
            this.txtPassword.TabIndex = 14;
            this.txtPassword.Text = "AdminCTTDT@2021@)@!";
            // 
            // txtUsername
            // 
            this.txtUsername.Location = new System.Drawing.Point(85, 92);
            this.txtUsername.Name = "txtUsername";
            this.txtUsername.Size = new System.Drawing.Size(100, 22);
            this.txtUsername.TabIndex = 13;
            this.txtUsername.Text = "administrator";
            // 
            // txtSiteUrl
            // 
            this.txtSiteUrl.Location = new System.Drawing.Point(6, 56);
            this.txtSiteUrl.Name = "txtSiteUrl";
            this.txtSiteUrl.Size = new System.Drawing.Size(185, 22);
            this.txtSiteUrl.TabIndex = 12;
            this.txtSiteUrl.Text = "https://noidung.quochoi.vn/content/tintuc/Lists/Category";
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
            // grSP
            // 
            this.grSP.Controls.Add(this.btnviewTreePhienHop);
            this.grSP.Controls.Add(this.btnDongBoPh);
            this.grSP.Controls.Add(this.btnConvert);
            this.grSP.Controls.Add(this.btnViewCateSQL);
            this.grSP.Controls.Add(this.btnDeleteAll);
            this.grSP.Controls.Add(this.bntConvert);
            this.grSP.Controls.Add(this.btnConnect);
            this.grSP.Controls.Add(this.lblUrlSP);
            this.grSP.Controls.Add(this.txtSiteUrl);
            this.grSP.Controls.Add(this.txtUsername);
            this.grSP.Controls.Add(this.txtPassword);
            this.grSP.Controls.Add(this.lblMatKhau);
            this.grSP.Controls.Add(this.lblUserName);
            this.grSP.Dock = System.Windows.Forms.DockStyle.Left;
            this.grSP.Location = new System.Drawing.Point(0, 0);
            this.grSP.Name = "grSP";
            this.grSP.Size = new System.Drawing.Size(200, 506);
            this.grSP.TabIndex = 17;
            this.grSP.TabStop = false;
            this.grSP.Text = "Thông tin SP";
            this.grSP.Enter += new System.EventHandler(this.grSP_Enter);
            // 
            // btnDongBoPh
            // 
            this.btnDongBoPh.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.btnDongBoPh.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.btnDongBoPh.BorderRadius = 0;
            this.btnDongBoPh.BorderSize = 0;
            this.btnDongBoPh.Design = RJCodeUI_M1.RJControls.ButtonDesign.Normal;
            this.btnDongBoPh.FlatAppearance.BorderSize = 0;
            this.btnDongBoPh.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(78)))), ((int)(((byte)(91)))), ((int)(((byte)(199)))));
            this.btnDongBoPh.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(73)))), ((int)(((byte)(85)))), ((int)(((byte)(186)))));
            this.btnDongBoPh.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDongBoPh.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDongBoPh.ForeColor = System.Drawing.Color.White;
            this.btnDongBoPh.IconChar = FontAwesome.Sharp.IconChar.None;
            this.btnDongBoPh.IconColor = System.Drawing.Color.White;
            this.btnDongBoPh.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnDongBoPh.IconSize = 24;
            this.btnDongBoPh.Location = new System.Drawing.Point(15, 450);
            this.btnDongBoPh.Name = "btnDongBoPh";
            this.btnDongBoPh.Size = new System.Drawing.Size(170, 40);
            this.btnDongBoPh.Style = RJCodeUI_M1.RJControls.ControlStyle.Solid;
            this.btnDongBoPh.TabIndex = 24;
            this.btnDongBoPh.Text = "Convert Phiên họp";
            this.btnDongBoPh.UseVisualStyleBackColor = false;
            this.btnDongBoPh.Click += new System.EventHandler(this.btnDongBoPh_Click);
            // 
            // btnConvert
            // 
            this.btnConvert.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.btnConvert.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.btnConvert.BorderRadius = 0;
            this.btnConvert.BorderSize = 0;
            this.btnConvert.Design = RJCodeUI_M1.RJControls.ButtonDesign.Normal;
            this.btnConvert.FlatAppearance.BorderSize = 0;
            this.btnConvert.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(78)))), ((int)(((byte)(91)))), ((int)(((byte)(199)))));
            this.btnConvert.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(73)))), ((int)(((byte)(85)))), ((int)(((byte)(186)))));
            this.btnConvert.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnConvert.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnConvert.ForeColor = System.Drawing.Color.White;
            this.btnConvert.IconChar = FontAwesome.Sharp.IconChar.None;
            this.btnConvert.IconColor = System.Drawing.Color.White;
            this.btnConvert.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnConvert.IconSize = 24;
            this.btnConvert.Location = new System.Drawing.Point(15, 356);
            this.btnConvert.Name = "btnConvert";
            this.btnConvert.Size = new System.Drawing.Size(170, 40);
            this.btnConvert.Style = RJCodeUI_M1.RJControls.ControlStyle.Solid;
            this.btnConvert.TabIndex = 23;
            this.btnConvert.Text = "Convert Kỳ họp";
            this.btnConvert.UseVisualStyleBackColor = false;
            this.btnConvert.Click += new System.EventHandler(this.btnConvert_Click);
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
            this.btnViewCateSQL.Location = new System.Drawing.Point(15, 310);
            this.btnViewCateSQL.Name = "btnViewCateSQL";
            this.btnViewCateSQL.Size = new System.Drawing.Size(170, 40);
            this.btnViewCateSQL.Style = RJCodeUI_M1.RJControls.ControlStyle.Solid;
            this.btnViewCateSQL.TabIndex = 21;
            this.btnViewCateSQL.Text = "View Tree Cate Kỳ họp";
            this.btnViewCateSQL.UseVisualStyleBackColor = false;
            this.btnViewCateSQL.Click += new System.EventHandler(this.btnViewCateSQL_Click);
            // 
            // btnDeleteAll
            // 
            this.btnDeleteAll.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.btnDeleteAll.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.btnDeleteAll.BorderRadius = 0;
            this.btnDeleteAll.BorderSize = 0;
            this.btnDeleteAll.Design = RJCodeUI_M1.RJControls.ButtonDesign.Normal;
            this.btnDeleteAll.FlatAppearance.BorderSize = 0;
            this.btnDeleteAll.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(78)))), ((int)(((byte)(91)))), ((int)(((byte)(199)))));
            this.btnDeleteAll.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(73)))), ((int)(((byte)(85)))), ((int)(((byte)(186)))));
            this.btnDeleteAll.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDeleteAll.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDeleteAll.ForeColor = System.Drawing.Color.White;
            this.btnDeleteAll.IconChar = FontAwesome.Sharp.IconChar.None;
            this.btnDeleteAll.IconColor = System.Drawing.Color.White;
            this.btnDeleteAll.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnDeleteAll.IconSize = 24;
            this.btnDeleteAll.Location = new System.Drawing.Point(15, 264);
            this.btnDeleteAll.Name = "btnDeleteAll";
            this.btnDeleteAll.Size = new System.Drawing.Size(170, 40);
            this.btnDeleteAll.Style = RJCodeUI_M1.RJControls.ControlStyle.Solid;
            this.btnDeleteAll.TabIndex = 20;
            this.btnDeleteAll.TabStop = false;
            this.btnDeleteAll.Text = "Xóa tất cả bản ghi CatNews";
            this.btnDeleteAll.UseVisualStyleBackColor = false;
            this.btnDeleteAll.Click += new System.EventHandler(this.btnDeleteAll_Click);
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
            this.bntConvert.Location = new System.Drawing.Point(15, 218);
            this.bntConvert.Name = "bntConvert";
            this.bntConvert.Size = new System.Drawing.Size(170, 40);
            this.bntConvert.Style = RJCodeUI_M1.RJControls.ControlStyle.Solid;
            this.bntConvert.TabIndex = 19;
            this.bntConvert.Text = "Cập nhật SP to SQL";
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
            this.btnConnect.Location = new System.Drawing.Point(15, 172);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(170, 40);
            this.btnConnect.Style = RJCodeUI_M1.RJControls.ControlStyle.Solid;
            this.btnConnect.TabIndex = 18;
            this.btnConnect.Text = "Kết Nối SP";
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
            // Rjpanelcate
            // 
            this.Rjpanelcate.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.Rjpanelcate.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(252)))), ((int)(((byte)(253)))));
            this.Rjpanelcate.BorderRadius = 0;
            this.Rjpanelcate.Customizable = false;
            this.Rjpanelcate.Location = new System.Drawing.Point(6, 21);
            this.Rjpanelcate.MinimumSize = new System.Drawing.Size(250, 0);
            this.Rjpanelcate.Name = "Rjpanelcate";
            this.Rjpanelcate.Size = new System.Drawing.Size(250, 280);
            this.Rjpanelcate.TabIndex = 18;
            // 
            // grCateSP
            // 
            this.grCateSP.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.grCateSP.Controls.Add(this.rjProgressBar1);
            this.grCateSP.Controls.Add(this.Rjpanelcate);
            this.grCateSP.Location = new System.Drawing.Point(206, 6);
            this.grCateSP.MinimumSize = new System.Drawing.Size(250, 0);
            this.grCateSP.Name = "grCateSP";
            this.grCateSP.Size = new System.Drawing.Size(264, 322);
            this.grCateSP.TabIndex = 19;
            this.grCateSP.TabStop = false;
            this.grCateSP.Text = "Dữ liệu chuyên mục tin SP";
            // 
            // rjProgressBar1
            // 
            this.rjProgressBar1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.rjProgressBar1.ChannelColor = System.Drawing.Color.LightSteelBlue;
            this.rjProgressBar1.ChannelHeight = 6;
            this.rjProgressBar1.Customizable = true;
            this.rjProgressBar1.ForeBackColor = System.Drawing.Color.RoyalBlue;
            this.rjProgressBar1.ForeColor = System.Drawing.Color.White;
            this.rjProgressBar1.Location = new System.Drawing.Point(158, 307);
            this.rjProgressBar1.Name = "rjProgressBar1";
            this.rjProgressBar1.ShowMaximun = false;
            this.rjProgressBar1.ShowValue = RJCodeUI_M1.RJControls.TextPosition.Right;
            this.rjProgressBar1.Size = new System.Drawing.Size(164, 23);
            this.rjProgressBar1.SliderColor = System.Drawing.Color.RoyalBlue;
            this.rjProgressBar1.SliderHeight = 6;
            this.rjProgressBar1.SymbolAfter = "";
            this.rjProgressBar1.SymbolBefore = "";
            this.rjProgressBar1.TabIndex = 22;
            // 
            // rgDongbo
            // 
            this.rgDongbo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.rgDongbo.Controls.Add(this.pnTreeSql);
            this.rgDongbo.Location = new System.Drawing.Point(485, 6);
            this.rgDongbo.MinimumSize = new System.Drawing.Size(270, 0);
            this.rgDongbo.Name = "rgDongbo";
            this.rgDongbo.Size = new System.Drawing.Size(270, 322);
            this.rgDongbo.TabIndex = 20;
            this.rgDongbo.TabStop = false;
            this.rgDongbo.Text = "Dữ liệu đồng bộ về SQL";
            // 
            // pnTreeSql
            // 
            this.pnTreeSql.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.pnTreeSql.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(252)))), ((int)(((byte)(253)))));
            this.pnTreeSql.BorderRadius = 0;
            this.pnTreeSql.Customizable = false;
            this.pnTreeSql.Location = new System.Drawing.Point(15, 21);
            this.pnTreeSql.MinimumSize = new System.Drawing.Size(250, 0);
            this.pnTreeSql.Name = "pnTreeSql";
            this.pnTreeSql.Size = new System.Drawing.Size(260, 280);
            this.pnTreeSql.TabIndex = 19;
            // 
            // grLog
            // 
            this.grLog.Controls.Add(this.txtLog);
            this.grLog.Dock = System.Windows.Forms.DockStyle.Right;
            this.grLog.Location = new System.Drawing.Point(755, 0);
            this.grLog.Name = "grLog";
            this.grLog.Size = new System.Drawing.Size(323, 506);
            this.grLog.TabIndex = 21;
            this.grLog.TabStop = false;
            this.grLog.Text = "Thông tin đồng bộ";
            // 
            // txtLog
            // 
            this.txtLog.Location = new System.Drawing.Point(6, 28);
            this.txtLog.Name = "txtLog";
            this.txtLog.Size = new System.Drawing.Size(311, 471);
            this.txtLog.TabIndex = 0;
            this.txtLog.Text = "";
            // 
            // processBar
            // 
            this.processBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.processBar.ChannelColor = System.Drawing.Color.LightSteelBlue;
            this.processBar.ChannelHeight = 6;
            this.processBar.Customizable = true;
            this.processBar.ForeBackColor = System.Drawing.Color.RoyalBlue;
            this.processBar.ForeColor = System.Drawing.Color.White;
            this.processBar.Location = new System.Drawing.Point(200, 373);
            this.processBar.Name = "processBar";
            this.processBar.ShowMaximun = false;
            this.processBar.ShowValue = RJCodeUI_M1.RJControls.TextPosition.Right;
            this.processBar.Size = new System.Drawing.Size(1269, 23);
            this.processBar.SliderColor = System.Drawing.Color.RoyalBlue;
            this.processBar.SliderHeight = 6;
            this.processBar.SymbolAfter = "";
            this.processBar.SymbolBefore = "";
            this.processBar.TabIndex = 22;
            // 
            // btnviewTreePhienHop
            // 
            this.btnviewTreePhienHop.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.btnviewTreePhienHop.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.btnviewTreePhienHop.BorderRadius = 0;
            this.btnviewTreePhienHop.BorderSize = 0;
            this.btnviewTreePhienHop.Design = RJCodeUI_M1.RJControls.ButtonDesign.Normal;
            this.btnviewTreePhienHop.FlatAppearance.BorderSize = 0;
            this.btnviewTreePhienHop.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(78)))), ((int)(((byte)(91)))), ((int)(((byte)(199)))));
            this.btnviewTreePhienHop.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(73)))), ((int)(((byte)(85)))), ((int)(((byte)(186)))));
            this.btnviewTreePhienHop.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnviewTreePhienHop.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnviewTreePhienHop.ForeColor = System.Drawing.Color.White;
            this.btnviewTreePhienHop.IconChar = FontAwesome.Sharp.IconChar.None;
            this.btnviewTreePhienHop.IconColor = System.Drawing.Color.White;
            this.btnviewTreePhienHop.IconFont = FontAwesome.Sharp.IconFont.Auto;
            this.btnviewTreePhienHop.IconSize = 24;
            this.btnviewTreePhienHop.Location = new System.Drawing.Point(15, 404);
            this.btnviewTreePhienHop.Name = "btnviewTreePhienHop";
            this.btnviewTreePhienHop.Size = new System.Drawing.Size(170, 40);
            this.btnviewTreePhienHop.Style = RJCodeUI_M1.RJControls.ControlStyle.Solid;
            this.btnviewTreePhienHop.TabIndex = 25;
            this.btnviewTreePhienHop.Text = "View Tree Cate Phiên họp";
            this.btnviewTreePhienHop.UseVisualStyleBackColor = false;
            this.btnviewTreePhienHop.Click += new System.EventHandler(this.btnviewTreePhienHop_Click);
            // 
            // GetCategorySP
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.BorderSize = 1;
            this.Caption = "My Profile";
            this.ClientSize = new System.Drawing.Size(960, 565);
            this.FormIcon = FontAwesome.Sharp.IconChar.User;
            this.Name = "GetCategorySP";
            this.Padding = new System.Windows.Forms.Padding(1);
            this.Text = "My Profile";
            this.pnlClientArea.ResumeLayout(false);
            this.grSP.ResumeLayout(false);
            this.grSP.PerformLayout();
            this.grCateSP.ResumeLayout(false);
            this.rgDongbo.ResumeLayout(false);
            this.grLog.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox grSP;
        private System.Windows.Forms.TextBox txtSiteUrl;
        private System.Windows.Forms.TextBox txtUsername;
        private System.Windows.Forms.TextBox txtPassword;
        private RJControls.RJLabel lblMatKhau;
        private RJControls.RJLabel lblUserName;
        private RJControls.RJLabel lblUrlSP;
        private RJControls.RJPanel Rjpanelcate;
        private System.Windows.Forms.GroupBox grLog;
        private System.Windows.Forms.RichTextBox txtLog;
        private System.Windows.Forms.GroupBox rgDongbo;
        private RJControls.RJPanel pnTreeSql;
        private System.Windows.Forms.GroupBox grCateSP;
        private RJControls.RJButton btnConnect;
        private RJControls.RJButton bntConvert;
        private RJControls.RJButton btnDeleteAll;
        private RJControls.RJProgressBar processBar;
        private RJControls.RJProgressBar rjProgressBar1;
        private RJControls.RJButton btnViewCateSQL;
        private RJControls.RJButton btnConvert;
        private RJControls.RJButton btnDongBoPh;
        private RJControls.RJButton btnviewTreePhienHop;
    }
}