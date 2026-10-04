namespace RJCodeUI_M1.TestAndDemo
{
    partial class FormCloneMoetNews
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
            this.cboCategory = new System.Windows.Forms.ComboBox();
            this.txtWebUrl = new System.Windows.Forms.TextBox();
            this.txtClassList = new System.Windows.Forms.TextBox();
            this.txtClassTitle = new System.Windows.Forms.TextBox();
            this.txtClassDate = new System.Windows.Forms.TextBox();
            this.txtClassAvatar = new System.Windows.Forms.TextBox();
            this.txtTotalPages = new System.Windows.Forms.TextBox();
            this.btnReadData = new System.Windows.Forms.Button();
            this.btnSaveData = new System.Windows.Forms.Button();
            this.dgvData = new System.Windows.Forms.DataGridView();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.txtClassItem = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.txtClassDescription = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.txtTuPage = new System.Windows.Forms.TextBox();
            this.txtDenPage = new System.Windows.Forms.TextBox();
            this.txtFolderChua = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.btnUpdateContent = new System.Windows.Forms.Button();
            this.pnlClientArea.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvData)).BeginInit();
            this.SuspendLayout();
            // 
            // pnlClientArea
            // 
            this.pnlClientArea.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.pnlClientArea.Controls.Add(this.btnUpdateContent);
            this.pnlClientArea.Controls.Add(this.label12);
            this.pnlClientArea.Controls.Add(this.txtFolderChua);
            this.pnlClientArea.Controls.Add(this.txtDenPage);
            this.pnlClientArea.Controls.Add(this.txtTuPage);
            this.pnlClientArea.Controls.Add(this.label11);
            this.pnlClientArea.Controls.Add(this.label10);
            this.pnlClientArea.Controls.Add(this.label9);
            this.pnlClientArea.Controls.Add(this.txtClassDescription);
            this.pnlClientArea.Controls.Add(this.label8);
            this.pnlClientArea.Controls.Add(this.txtClassItem);
            this.pnlClientArea.Controls.Add(this.label7);
            this.pnlClientArea.Controls.Add(this.label6);
            this.pnlClientArea.Controls.Add(this.label5);
            this.pnlClientArea.Controls.Add(this.label4);
            this.pnlClientArea.Controls.Add(this.label3);
            this.pnlClientArea.Controls.Add(this.label2);
            this.pnlClientArea.Controls.Add(this.label1);
            this.pnlClientArea.Controls.Add(this.dgvData);
            this.pnlClientArea.Controls.Add(this.btnSaveData);
            this.pnlClientArea.Controls.Add(this.btnReadData);
            this.pnlClientArea.Controls.Add(this.txtTotalPages);
            this.pnlClientArea.Controls.Add(this.txtClassAvatar);
            this.pnlClientArea.Controls.Add(this.txtClassDate);
            this.pnlClientArea.Controls.Add(this.txtClassTitle);
            this.pnlClientArea.Controls.Add(this.txtClassList);
            this.pnlClientArea.Controls.Add(this.txtWebUrl);
            this.pnlClientArea.Controls.Add(this.cboCategory);
            this.pnlClientArea.Location = new System.Drawing.Point(1, 41);
            this.pnlClientArea.Size = new System.Drawing.Size(958, 755);
            this.pnlClientArea.Paint += new System.Windows.Forms.PaintEventHandler(this.pnlClientArea_Paint);
            // 
            // cboCategory
            // 
            this.cboCategory.FormattingEnabled = true;
            this.cboCategory.Location = new System.Drawing.Point(127, 273);
            this.cboCategory.Name = "cboCategory";
            this.cboCategory.Size = new System.Drawing.Size(757, 28);
            this.cboCategory.TabIndex = 0;
            // 
            // txtWebUrl
            // 
            this.txtWebUrl.Location = new System.Drawing.Point(127, 95);
            this.txtWebUrl.Name = "txtWebUrl";
            this.txtWebUrl.Size = new System.Drawing.Size(250, 25);
            this.txtWebUrl.TabIndex = 1;
            // 
            // txtClassList
            // 
            this.txtClassList.Location = new System.Drawing.Point(127, 135);
            this.txtClassList.Name = "txtClassList";
            this.txtClassList.Size = new System.Drawing.Size(250, 25);
            this.txtClassList.TabIndex = 2;
            // 
            // txtClassTitle
            // 
            this.txtClassTitle.Location = new System.Drawing.Point(634, 60);
            this.txtClassTitle.Name = "txtClassTitle";
            this.txtClassTitle.Size = new System.Drawing.Size(250, 25);
            this.txtClassTitle.TabIndex = 3;
            // 
            // txtClassDate
            // 
            this.txtClassDate.Location = new System.Drawing.Point(634, 142);
            this.txtClassDate.Name = "txtClassDate";
            this.txtClassDate.Size = new System.Drawing.Size(250, 25);
            this.txtClassDate.TabIndex = 4;
            // 
            // txtClassAvatar
            // 
            this.txtClassAvatar.Location = new System.Drawing.Point(634, 182);
            this.txtClassAvatar.Name = "txtClassAvatar";
            this.txtClassAvatar.Size = new System.Drawing.Size(250, 25);
            this.txtClassAvatar.TabIndex = 5;
            // 
            // txtTotalPages
            // 
            this.txtTotalPages.Location = new System.Drawing.Point(127, 234);
            this.txtTotalPages.Name = "txtTotalPages";
            this.txtTotalPages.Size = new System.Drawing.Size(250, 25);
            this.txtTotalPages.TabIndex = 6;
            this.txtTotalPages.Text = "1";
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
            this.label1.Location = new System.Drawing.Point(30, 281);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(86, 20);
            this.label1.TabIndex = 8;
            this.label1.Text = "Danh mục:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(27, 98);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(83, 20);
            this.label2.TabIndex = 7;
            this.label2.Text = "Web URL:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(27, 138);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(81, 20);
            this.label3.TabIndex = 6;
            this.label3.Text = "Class List:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(430, 63);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(85, 20);
            this.label4.TabIndex = 5;
            this.label4.Text = "Class Title:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(430, 145);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(91, 20);
            this.label5.TabIndex = 4;
            this.label5.Text = "Class Date:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(430, 185);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(102, 20);
            this.label6.TabIndex = 3;
            this.label6.Text = "Class Avatar:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(27, 237);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(89, 20);
            this.label7.TabIndex = 2;
            this.label7.Text = "Tổng page:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(430, 21);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(88, 20);
            this.label8.TabIndex = 11;
            this.label8.Text = "Class Item:";
            // 
            // txtClassItem
            // 
            this.txtClassItem.Location = new System.Drawing.Point(634, 18);
            this.txtClassItem.Name = "txtClassItem";
            this.txtClassItem.Size = new System.Drawing.Size(250, 25);
            this.txtClassItem.TabIndex = 10;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(430, 106);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(136, 20);
            this.label9.TabIndex = 13;
            this.label9.Text = "Class Description:";
            // 
            // txtClassDescription
            // 
            this.txtClassDescription.Location = new System.Drawing.Point(634, 103);
            this.txtClassDescription.Name = "txtClassDescription";
            this.txtClassDescription.Size = new System.Drawing.Size(250, 25);
            this.txtClassDescription.TabIndex = 12;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(31, 18);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(71, 20);
            this.label10.TabIndex = 14;
            this.label10.Text = "Từ page:";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(223, 18);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(83, 20);
            this.label11.TabIndex = 15;
            this.label11.Text = "Đến page:";
            // 
            // txtTuPage
            // 
            this.txtTuPage.Location = new System.Drawing.Point(127, 21);
            this.txtTuPage.Name = "txtTuPage";
            this.txtTuPage.Size = new System.Drawing.Size(90, 25);
            this.txtTuPage.TabIndex = 16;
            this.txtTuPage.Text = "1";
            // 
            // txtDenPage
            // 
            this.txtDenPage.Location = new System.Drawing.Point(303, 18);
            this.txtDenPage.Name = "txtDenPage";
            this.txtDenPage.Size = new System.Drawing.Size(74, 25);
            this.txtDenPage.TabIndex = 17;
            this.txtDenPage.Text = "1";
            // 
            // txtFolderChua
            // 
            this.txtFolderChua.Location = new System.Drawing.Point(634, 222);
            this.txtFolderChua.Name = "txtFolderChua";
            this.txtFolderChua.Size = new System.Drawing.Size(250, 25);
            this.txtFolderChua.TabIndex = 18;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(430, 222);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(97, 20);
            this.label12.TabIndex = 19;
            this.label12.Text = "Folder chứa:";
            // 
            // btnUpdateContent
            // 
            this.btnUpdateContent.Location = new System.Drawing.Point(258, 637);
            this.btnUpdateContent.Name = "btnUpdateContent";
            this.btnUpdateContent.Size = new System.Drawing.Size(240, 26);
            this.btnUpdateContent.TabIndex = 20;
            this.btnUpdateContent.Text = "Update Content";
            this.btnUpdateContent.UseVisualStyleBackColor = true;
            this.btnUpdateContent.Click += new System.EventHandler(this.btnUpdateContent_Click);
            // 
            // FormCloneMoetNews
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.BorderSize = 1;
            this.Caption = "Clone Moet News";
            this.ClientSize = new System.Drawing.Size(960, 797);
            this.Name = "FormCloneMoetNews";
            this.Padding = new System.Windows.Forms.Padding(1);
            this.Text = "Clone Moet News";
            this.Load += new System.EventHandler(this.FormCloneMoetNews_Load);
            this.pnlClientArea.ResumeLayout(false);
            this.pnlClientArea.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvData)).EndInit();
            this.ResumeLayout(false);

        }

        private System.Windows.Forms.ComboBox cboCategory;
        private System.Windows.Forms.TextBox txtWebUrl;
        private System.Windows.Forms.TextBox txtClassList;
        private System.Windows.Forms.TextBox txtClassTitle;
        private System.Windows.Forms.TextBox txtClassDate;
        private System.Windows.Forms.TextBox txtClassAvatar;
        private System.Windows.Forms.TextBox txtTotalPages;
        private System.Windows.Forms.Button btnReadData;
        private System.Windows.Forms.Button btnSaveData;
        private System.Windows.Forms.DataGridView dgvData;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox txtClassItem;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox txtClassDescription;
        private System.Windows.Forms.TextBox txtDenPage;
        private System.Windows.Forms.TextBox txtTuPage;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox txtFolderChua;
        private System.Windows.Forms.Button btnUpdateContent;
    }
}
