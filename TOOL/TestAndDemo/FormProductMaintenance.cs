using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using RJCodeUI_M1.Models;

namespace RJCodeUI_M1.TestAndDemo
{
    public partial class FormProductMaintenance : RJForms.RJChildForm
    {
        public FormProductMaintenance()
        {
            InitializeComponent();
            lblTitle.Text = "Add new product";
            btnSave.Text = "Add";
            btnSave.BackColor = Settings.RJColors.Confirm;
            
        }

       
        private void btnSave_Click(object sender, EventArgs e)
        {

        }
    }
}
