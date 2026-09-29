namespace RJCodeUI_M1.TestAndDemo
{
    partial class FormThongBao
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
            this.txtLink = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.cboCategory = new System.Windows.Forms.ComboBox();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnGetDanhMuc = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.txtPageStep = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtApatit = new System.Windows.Forms.TextBox();
            this.btnSaveApatit = new System.Windows.Forms.Button();
            this.btnCategoryApatit = new System.Windows.Forms.Button();
            this.pnlClientArea.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlClientArea
            // 
            this.pnlClientArea.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.pnlClientArea.Controls.Add(this.btnCategoryApatit);
            this.pnlClientArea.Controls.Add(this.btnSaveApatit);
            this.pnlClientArea.Controls.Add(this.txtApatit);
            this.pnlClientArea.Controls.Add(this.label4);
            this.pnlClientArea.Controls.Add(this.txtPageStep);
            this.pnlClientArea.Controls.Add(this.button2);
            this.pnlClientArea.Controls.Add(this.button1);
            this.pnlClientArea.Controls.Add(this.txtLink);
            this.pnlClientArea.Controls.Add(this.cboCategory);
            this.pnlClientArea.Dock = System.Windows.Forms.DockStyle.None;
            this.pnlClientArea.Size = new System.Drawing.Size(958, 652);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(583, 39);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(221, 20);
            this.label1.TabIndex = 0;
            this.label1.Text = "Convert Danh sách thông báo";
            // 
            // txtLink
            // 
            this.txtLink.Location = new System.Drawing.Point(79, 96);
            this.txtLink.Name = "txtLink";
            this.txtLink.Size = new System.Drawing.Size(748, 25);
            this.txtLink.TabIndex = 1;
            this.txtLink.Text = "https://vinaapaco.com/category/cong-bo-thong-tin/page/";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(102, 113);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(94, 20);
            this.label2.TabIndex = 2;
            this.label2.Text = "Link convert";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(102, 202);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(133, 20);
            this.label3.TabIndex = 3;
            this.label3.Text = "Danh mục đổ vào";
            // 
            // cboCategory
            // 
            this.cboCategory.FormattingEnabled = true;
            this.cboCategory.Location = new System.Drawing.Point(79, 141);
            this.cboCategory.Name = "cboCategory";
            this.cboCategory.Size = new System.Drawing.Size(748, 28);
            this.cboCategory.TabIndex = 4;
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(279, 259);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(141, 48);
            this.btnSave.TabIndex = 5;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            // 
            // btnGetDanhMuc
            // 
            this.btnGetDanhMuc.Location = new System.Drawing.Point(449, 259);
            this.btnGetDanhMuc.Name = "btnGetDanhMuc";
            this.btnGetDanhMuc.Size = new System.Drawing.Size(141, 48);
            this.btnGetDanhMuc.TabIndex = 6;
            this.btnGetDanhMuc.Text = "Get Danh mục";
            this.btnGetDanhMuc.UseVisualStyleBackColor = true;
            this.btnGetDanhMuc.Click += new System.EventHandler(this.btnGetDanhMuc_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(79, 195);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(86, 35);
            this.button1.TabIndex = 5;
            this.button1.Text = "Save";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(198, 195);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(222, 35);
            this.button2.TabIndex = 5;
            this.button2.Text = "Get Category";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.btnGetDanhMuc_Click);
            // 
            // txtPageStep
            // 
            this.txtPageStep.Location = new System.Drawing.Point(79, 45);
            this.txtPageStep.Name = "txtPageStep";
            this.txtPageStep.Size = new System.Drawing.Size(100, 25);
            this.txtPageStep.TabIndex = 6;
            this.txtPageStep.Text = "11";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(369, 270);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(204, 20);
            this.label4.TabIndex = 7;
            this.label4.Text = "Dùng cho Thông báo Apatit";
            // 
            // txtApatit
            // 
            this.txtApatit.Location = new System.Drawing.Point(79, 317);
            this.txtApatit.Name = "txtApatit";
            this.txtApatit.Size = new System.Drawing.Size(748, 25);
            this.txtApatit.TabIndex = 8;
            this.txtApatit.Text = "https://vinaapaco.com/category/cong-bo-thong-tin/page/11/";
            // 
            // btnSaveApatit
            // 
            this.btnSaveApatit.Location = new System.Drawing.Point(79, 389);
            this.btnSaveApatit.Name = "btnSaveApatit";
            this.btnSaveApatit.Size = new System.Drawing.Size(100, 38);
            this.btnSaveApatit.TabIndex = 9;
            this.btnSaveApatit.Text = "Save Apatit";
            this.btnSaveApatit.UseVisualStyleBackColor = true;
            this.btnSaveApatit.Click += new System.EventHandler(this.btnSaveApatit_Click);
            // 
            // btnCategoryApatit
            // 
            this.btnCategoryApatit.Location = new System.Drawing.Point(198, 389);
            this.btnCategoryApatit.Name = "btnCategoryApatit";
            this.btnCategoryApatit.Size = new System.Drawing.Size(222, 38);
            this.btnCategoryApatit.TabIndex = 10;
            this.btnCategoryApatit.Text = "Get categoty Apatit";
            this.btnCategoryApatit.UseVisualStyleBackColor = true;
            this.btnCategoryApatit.Click += new System.EventHandler(this.btnCategoryApatit_Click);
            // 
            // FormThongBao
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(97)))), ((int)(((byte)(212)))));
            this.BorderSize = 1;
            this.Caption = "FormThongBao";
            this.ClientSize = new System.Drawing.Size(960, 694);
            this.Controls.Add(this.btnGetDanhMuc);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "FormThongBao";
            this.Padding = new System.Windows.Forms.Padding(1);
            this.Text = "FormThongBao";
            this.Controls.SetChildIndex(this.label1, 0);
            this.Controls.SetChildIndex(this.label2, 0);
            this.Controls.SetChildIndex(this.label3, 0);
            this.Controls.SetChildIndex(this.btnSave, 0);
            this.Controls.SetChildIndex(this.btnGetDanhMuc, 0);
            this.Controls.SetChildIndex(this.pnlClientArea, 0);
            this.pnlClientArea.ResumeLayout(false);
            this.pnlClientArea.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtLink;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cboCategory;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnGetDanhMuc;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.TextBox txtPageStep;
        private System.Windows.Forms.Button btnCategoryApatit;
        private System.Windows.Forms.Button btnSaveApatit;
        private System.Windows.Forms.TextBox txtApatit;
        private System.Windows.Forms.Label label4;
    }
}