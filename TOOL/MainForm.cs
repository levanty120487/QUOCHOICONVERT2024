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
using RJCodeUI_M1.TestAndDemo;
using System.Diagnostics;
using System.IO;
using System.Net;
using RJCodeUI_M1.SPService;
using RJCodeUI_M1.SPAuthentication;
using RJCodeUI_M1.SPImage;
using System.Xml;
using QHBASE;


namespace RJCodeUI_M1
{
    public partial class MainForm : RJForms.RJMainForm
    {
        #region -> Fields

        private User userConnected;
        private string ListCatName = "Chuyên mục bài viết";
        List<NewsCat> LtsAllCategory = new List<NewsCat>();
        List<NewsCat> LtsAllCategoryTemp = new List<NewsCat>();
        #endregion

        #region -> Constructor

        public MainForm()
        {
            InitializeComponent();
            InitializeItems();


        }

        public MainForm(User user)
        {
            InitializeComponent();
            InitializeItems();
            //
            userConnected = user;

        }
        private void InitializeItems()
        {

            if (Settings.UIAppearance.Style == Settings.UIStyle.Supernova)
                pbSideMenuLogo.Image = Properties.Resources.RJTitleBarLogoColor;

        }
        #endregion

        #region -> Events Methods Definition

        //How to use the OpenChildForm<childForm>(...) method

        /// You can use the Func<TResult> delegate with anonymous methods or lambda expression,
        /// for example, we can call this method as follows:
        /// With anonymous method:
        ///     <see cref="OpenChildForm( delegate () { return new MyForm('MyParameter'); });"/>    
        /// With lambda expression
        ///     <see cref="OpenChildForm( () => new MyForm('id', 'username'));"/>


        #region - Open Child Form
        //(User Options Dropdown Menu)
        /// Using [<see cref="OpenChildForm<childForm>(Func<childForm> _delegate) where childForm : RJChildForm"/>] Method

        private void miMyProfile_Click(object sender, EventArgs e)
        {
            this.OpenChildForm(() => new FormConvertNewsSP(userConnected));
            //()=> : Generic delegate call
        }
        private void miSettings_Click(object sender, EventArgs e)
        {
            this.OpenChildForm(() => new RJForms.RJSettingsForm());
        }

        #endregion

        #region - Open Child Form from a Menu Button
        //(Side Menu)
        /// Using [<see cref="OpenChildForm<childForm>(Func<childForm> _delegate, object senderMenuButton) where childForm : RJChildForm"/>] Method

        private void btnUserControls_Click(object sender, EventArgs e)
        {
            this.OpenChildForm(() => new FormUserControls(), sender);
            //()=> : Generic delegate call
            //sender: btnUserControls (MenuButton)
        }
        private void btnDashboard_Click(object sender, EventArgs e)
        {
            this.OpenChildForm(() => new FormDashboard(), sender);
        }
        private void btnProducts_Click(object sender, EventArgs e)
        {
            this.OpenChildForm(() => new FormProducts(), sender);
        }
        private void btnCustomers_Click(object sender, EventArgs e)
        {
            this.OpenChildForm(() => new FormConvertNewsSP(), sender);
        }
        #endregion

        #region - Open Child Form from a Dropdown Menu Item associated with a Menu Button
        //(Side Menu)
        /// Using [<see cref="OpenChildForm<childForm>(Func<childForm> _delegate, object senderMenuItem, RJMenuButton ownerMenuButton) where childForm : RJChildForm"/>] Method

       
       
        #endregion

        #region - User Options
        //(User Options Dropdown Menu)

        private void miExit_Click(object sender, EventArgs e)
        {
            this.CloseWindow();
        }
        private void miLogout_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void miHelp_Click(object sender, EventArgs e)
        {
            Process.Start(@"Files\Documentation.pdf");
        }
        private void miTermsCond_Click(object sender, EventArgs e)
        {
            Process.Start(@"Files\License.pdf");
        }



        #endregion

        #endregion

        private void btnCustomControls_Click(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnConvertDM_Click(object sender, EventArgs e)
        {
            //SPService.Lists listSV;
            //// getCateSP();
            //string stSite = txtSiteUrl.Text.Substring(0, txtSiteUrl.Text.LastIndexOf('/')).ToLower().Replace("/lists", string.Empty);
            ////   SPService.Lists listSV = GetListSV("http://10.8.2.54/knd");

            ////if (cbkBT.Checked)
            ////{
            ////    ListCatName = "Danh mục trang Bộ Trưởng";
            ////    listSV = GetListSV(stSite);
            ////}
            ////else
            ////{
            //listSV = UtilsBase.GetListSV(stSite.Replace("/tt", ""), txtUsername.Text, txtPassword.Text);
            ////}
            //System.Xml.XmlNode ndListView = listSV.GetListAndView(ListCatName, "");
            //string strListID = ndListView.ChildNodes[0].Attributes["Name"].Value;
            //string strViewID = ndListView.ChildNodes[1].Attributes["Name"].Value;

            //System.Xml.XmlDocument xmlDoc = new System.Xml.XmlDocument();
            //System.Xml.XmlNode ndQuery = xmlDoc.CreateNode(System.Xml.XmlNodeType.Element, "Query", "");
            //System.Xml.XmlNode ndViewFields = xmlDoc.CreateNode(System.Xml.XmlNodeType.Element, "ViewFields", "");
            //System.Xml.XmlNode ndQueryOptions = xmlDoc.CreateNode(System.Xml.XmlNodeType.Element, "QueryOptions", "");

            //ndViewFields.InnerXml = "<FieldRef Name=\"ID\" /><FieldRef Name=\"Title\" /><FieldRef Name=\"Parent\" LookupId=\"True\" />";

            //ndQuery.InnerXml = "<OrderBy><FieldRef Name=\"ID\" Ascending=\"True\" /></OrderBy>";

            //ndQueryOptions.InnerXml = "<IncludeMandatoryColumns>False</IncludeMandatoryColumns><ViewAttributes Scope=\"RecursiveAll\"/><DateInUtc>TRUE</DateInUtc>";

            //System.Xml.XmlNode ndListItems = listSV.GetListItems(ListCatName, "", ndQuery, ndViewFields, "1000", ndQueryOptions, string.Empty);
            //DataTable TableFull = UtilsBase.ConvertXmlToDataTable(ndListItems);

            //LtsAllCategory.Clear();
            //LtsAllCategoryTemp.Clear();
            //foreach (DataRow item in TableFull.Rows)
            //{
            //    CategoryItem catItem = new CategoryItem();
            //    catItem.CatID = Convert.ToInt32(item["ows_ID"]);
            //    catItem.CatName = Convert.ToString(item["ows_Title"]);
            //    if (ndListItems.InnerXml.Contains("ows_Parent"))
            //    {
            //        if (item["ows_Parent"] != null)
            //        {

            //            catItem.CatParentId = UtilsBase.getLookup(Convert.ToString(item["ows_Parent"]));
            //        }
            //    }

            //    LtsAllCategoryTemp.Add(catItem);
            //}

            //LtsAllCategory = LtsAllCategoryTemp.OrderBy(o => o.CatID).ToList();
            //TreeView treeView = new TreeView();
            //treeView.Width = 400;
            //treeView.Dock = DockStyle.Left;
            //// Lấy các node gốc (ParentId = null)
            //var rootCategories = LtsAllCategoryTemp.Where(c => c.CatParentId == 0).ToList();

            //// Duyệt qua danh sách root để thêm vào TreeView
            //foreach (var rootCategory in rootCategories)
            //{
            //    TreeNode rootNode = new TreeNode(rootCategory.CatName);
            //    rootNode.Tag = rootCategory.CatID; // Đính kèm ID của Category để dễ dàng thao tác sau này
            //    rootNode.NodeFont = new Font(treeView.Font, FontStyle.Bold);
            //    // Gọi hàm đệ quy để thêm các node con
            //    UtilsBase.AddChildNodes(rootNode, LtsAllCategory);

            //    treeView.Nodes.Add(rootNode);
            //}

            //treeView.ExpandAll();
            //Rjpanel.Controls.Add(treeView);



        }



        private void btntestree_Click(object sender, EventArgs e)
        {

            this.OpenChildForm(() => new GetCategorySP(), sender);
            //this.OpenChildForm(() => new FormDataControls(), sender, );
        }

        private void pnlDesktop_Paint(object sender, PaintEventArgs e)
        {

        }

        private void btnConvertVanKien_Click(object sender, EventArgs e)
        {
            this.OpenChildForm(() => new FormConvertVanKienSP(), sender);
        }

        private void btnDMVanKien_Click(object sender, EventArgs e)
        {
            this.OpenChildForm(() => new GetCategoryVanKienSP(), sender);
        }

        private void rjBtnLinhVuc_Click(object sender, EventArgs e)
        {
            this.OpenChildForm(() => new GetCategoryLinhVucSP(), sender);
        }

        private void rjBtnCoQuanBanHanh_Click(object sender, EventArgs e)
        {
            this.OpenChildForm(() => new GetCategoryCoQuanBanHanhSP(), sender);

        }

        private void rjMenuButton1_Click(object sender, EventArgs e)
        {
            //this.OpenChildForm(() => new FormThongBao(), sender);
            this.OpenChildForm(() => new TestAndDemo.FormCloneMoetThongBao(), sender);
        }

        private void rjMenuButton1_Click_1(object sender, EventArgs e)
        {
            this.OpenChildForm(() => new FormHoiDap(), sender);
        }

        private void btnCloneMoetNews_Click(object sender, EventArgs e)
        {
            this.OpenChildForm(() => new TestAndDemo.FormCloneMoetNews(), sender);
        }

        private void rjMenuButton2_Click(object sender, EventArgs e)
        {
            this.OpenChildForm(() => new TestAndDemo.FormCloneMoetNews(), sender);
        }

        private void rjMenuButtonThongBaoMoet_Click(object sender, EventArgs e)
        {
            this.OpenChildForm(() => new TestAndDemo.FormCloneMoetThongBao(), sender);
        }
    }
}
