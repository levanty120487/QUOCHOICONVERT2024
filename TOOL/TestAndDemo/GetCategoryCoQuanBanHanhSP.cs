using QHBASE;
using RJCodeUI_M1.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;



namespace RJCodeUI_M1.TestAndDemo
{
    public partial class GetCategoryCoQuanBanHanhSP : RJForms.RJChildForm
    {
        private string ListCatName = "Cơ quan ban hành";
        List<QHBASE.TempCoQuanBanHanh> LtsAllCategory = new List<TempCoQuanBanHanh>();
        List<TempCoQuanBanHanh> LtsAllCategoryTemp = new List<TempCoQuanBanHanh>();
        TreeView treeView = new TreeView();
        TreeView treeView2024 = new TreeView();
        public GetCategoryCoQuanBanHanhSP()
        {

            InitializeComponent();

        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            this.Rjpanelcate.Controls.Clear();
            ConnectSPCate();
        }
        void ConnectSPCate()
        {
            processBar.Minimum = 0;
            processBar.Step = 1;
            using (QHBASE.QuocHoiVNEntities db = new QHBASE.QuocHoiVNEntities())
            {
                SPService.Lists listSV;
                // getCateSP();
                string stSite = txtSiteUrl.Text.Substring(0, txtSiteUrl.Text.LastIndexOf('/')).ToLower().Replace("/lists", string.Empty);
                //   SPService.Lists listSV = GetListSV("http://10.8.2.54/knd");

                //if (cbkBT.Checked)
                //{
                //    ListCatName = "Danh mục trang Bộ Trưởng";
                //    listSV = GetListSV(stSite);
                //}
                //else
                //{
                listSV = UtilsBase.GetListSV(stSite.Replace("/tt", ""), txtUsername.Text, txtPassword.Text);
                //}
                System.Xml.XmlNode ndListView = listSV.GetListAndView(ListCatName, "");
                string strListID = ndListView.ChildNodes[0].Attributes["Name"].Value;
                string strViewID = ndListView.ChildNodes[1].Attributes["Name"].Value;

                System.Xml.XmlDocument xmlDoc = new System.Xml.XmlDocument();
                System.Xml.XmlNode ndQuery = xmlDoc.CreateNode(System.Xml.XmlNodeType.Element, "Query", "");
                System.Xml.XmlNode ndViewFields = xmlDoc.CreateNode(System.Xml.XmlNodeType.Element, "ViewFields", "");
                System.Xml.XmlNode ndQueryOptions = xmlDoc.CreateNode(System.Xml.XmlNodeType.Element, "QueryOptions", "");

                ndViewFields.InnerXml = "<FieldRef Name=\"ID\" />" +
                    "<FieldRef Name=\"Title\" />" +
                    "<FieldRef Name=\"Parent\" LookupId=\"True\" />" +
                    "<FieldRef Name=\"CategoryAllID\" />" +
                    "<FieldRef Name=\"Description\" />" +
                    "<FieldRef Name=\"IsDBQH\" />" +
                    "<FieldRef Name=\"IsShow\" />" +
                    "<FieldRef Name=\"IsTopMenu\" />" +
                    "<FieldRef Name=\"PageUrl\" />" +
                    "<FieldRef Name=\"Index\" />" +
                    "<FieldRef Name=\"TitleSort\" />";

                ndQuery.InnerXml = "<OrderBy><FieldRef Name=\"ID\" Ascending=\"True\" /></OrderBy>";

                ndQueryOptions.InnerXml = "<IncludeMandatoryColumns>False</IncludeMandatoryColumns><ViewAttributes Scope=\"RecursiveAll\"/><DateInUtc>TRUE</DateInUtc>";

                System.Xml.XmlNode ndListItems = listSV.GetListItems(ListCatName, "", ndQuery, ndViewFields, "10000", ndQueryOptions, string.Empty);
                DataTable TableFull = UtilsBase.ConvertXmlToDataTable(ndListItems);
                processBar.Maximum = TableFull.Rows.Count;
                LtsAllCategory.Clear();
                LtsAllCategoryTemp.Clear();
                int rowCount = 0;
                foreach (DataRow item in TableFull.Rows)
                {
                    TempCoQuanBanHanh catItem = new TempCoQuanBanHanh();
                    catItem.Id = Convert.ToInt32(item["ows_ID"]);
                    catItem.Title = Convert.ToString(item["ows_Title"]);
                    if (ndListItems.InnerXml.Contains("ows_Parent"))
                    {
                        if (item["ows_Parent"] != null)
                        {

                            catItem.ParentId = UtilsBase.getLookup(Convert.ToString(item["ows_Parent"]));
                        }
                    }
                    rowCount++;
                    processBar.Value = rowCount;
                    LtsAllCategoryTemp.Add(catItem);
                }
                LtsAllCategory = LtsAllCategoryTemp.OrderBy(o => o.Id).ToList();
                TreeView treeView = new TreeView();
                treeView.Width = 400;
                treeView.Dock = DockStyle.Left;
                // Lấy các node gốc (ParentId = null)
                var rootCategories = LtsAllCategoryTemp.Where(c => c.ParentId == 0).ToList();

                // Duyệt qua danh sách root để thêm vào TreeView
                foreach (var rootCategory in rootCategories)
                {
                    TreeNode rootNode = new TreeNode(rootCategory.Title);
                    rootNode.Tag = rootCategory.Id; // Đính kèm ID của Category để dễ dàng thao tác sau này
                    rootNode.NodeFont = new Font(treeView.Font, FontStyle.Bold);
                    // Gọi hàm đệ quy để thêm các node con
                    UtilsBase.AddChildNodesCoQuanBanHanh(rootNode, LtsAllCategory);

                    treeView.Nodes.Add(rootNode);

                }

                treeView.ExpandAll();
                UtilsBase.BoldParentNodes(treeView);
                Rjpanelcate.Controls.Add(treeView);

            }
        }

        private void bntConvert_Click(object sender, EventArgs e)
        {

            InsertData();
            buidTreeSql();
        }

        void buidTreeSql()
        {

            treeView.Width = 400;
            treeView.Dock = DockStyle.Left;
            // LtsAllCategory = GetAllDataCate();
            // Lấy các node gốc (ParentId = null)
            var rootCategories = GetAllDataCate().Where(c => c.ParentId == 0).ToList();
            LtsAllCategory = GetAllDataCate().OrderBy(o => o.Id).ToList();
            // Duyệt qua danh sách root để thêm vào TreeView
            foreach (var rootCategory in rootCategories)
            {
                TreeNode rootNode = new TreeNode(rootCategory.Title);
                rootNode.Tag = rootCategory.OldId;
                // Đính kèm ID của Category để dễ dàng thao tác sau này

                // Gọi hàm đệ quy để thêm các node con
                UtilsBase.AddChildNodesSQLCoQuanBanHanh(rootNode, LtsAllCategory);
                treeView.Nodes.Add(rootNode);
            }

            treeView.CollapseAll();
            UtilsBase.BoldParentNodes(treeView);
            pnTreeSql.Controls.Add(treeView);
        }


        public (Guid? node2024, int? nodeOld) GetIDsFromTreeViews()
        {
            Guid? node2024 = null;
            int? nodeOld = null;

            // Lấy ID từ treeView1
            if (treeView.SelectedNode != null)
            {
                nodeOld = (int)treeView.SelectedNode.Tag;
            }
            else
            {
                MessageBox.Show("Chưa có node nào được chọn trong TreeView 1.");
            }

            // Lấy ID từ treeView2
            if (treeView2024.SelectedNode != null)
            {
                node2024 = (Guid)treeView2024.SelectedNode.Tag;
            }
            else
            {
                MessageBox.Show("Chưa có node nào được chọn trong TreeView 2.");
            }

            return (node2024, nodeOld);
        }
       
      
        public static List<TempCoQuanBanHanh> GetAllDataCate()
        {
            using (var context = new QuocHoiVNEntities())
            {
                // Get all items from the table
                var allItems = context.TempCoQuanBanHanhs.ToList();
                return allItems;
            }

        }
        public List<DMKyHop> GetAllCatKyHop2024()
        {
            using (var context = new Preview_QuocHoi_KyHopServiceEntities())
            {
                // Get all items from the table
                var allItems = context.DMKyHops.ToList();
                return allItems;
            }

        }
       

        private void btnDeleteAll_Click(object sender, EventArgs e)
        {
            using (var context = new QuocHoiVNEntities())
            {
                // Get all items from the table
                var allItems = context.TempCoQuanBanHanhs.ToList();

                // Remove all items
                context.TempCoQuanBanHanhs.RemoveRange(allItems);

                // Submit the changes to the database
                context.SaveChanges();
            }
        }

        private void btnViewCateSQL_Click(object sender, EventArgs e)
        {
            treeView.Nodes.Clear();
            treeView2024.Nodes.Clear();
            buidTreeSql();
            viewCatTreeQH2024();

        }



        // Tách phần xử lý thành các phương thức riêng
        private System.Xml.XmlNode CreateQueryNode(System.Xml.XmlDocument xmlDoc)
        {
            var ndQuery = xmlDoc.CreateNode(System.Xml.XmlNodeType.Element, "Query", "");
            ndQuery.InnerXml = "<OrderBy><FieldRef Name=\"ID\" Ascending=\"True\" /></OrderBy>";
            return ndQuery;
        }

        private System.Xml.XmlNode CreateViewFieldsNode(System.Xml.XmlDocument xmlDoc)
        {
            var ndViewFields = xmlDoc.CreateNode(System.Xml.XmlNodeType.Element, "ViewFields", "");
            ndViewFields.InnerXml = "<FieldRef Name=\"ID\" />" +
                                    "<FieldRef Name=\"Title\" />" +
                                    "<FieldRef Name=\"Parent\" LookupId=\"True\" />" +
                                    "<FieldRef Name=\"CategoryAllID\" />" +
                                    "<FieldRef Name=\"Description\" />" +
                                    "<FieldRef Name=\"IsShow\" />" +
                                    "<FieldRef Name=\"IsDBQH\" />" +
                                    "<FieldRef Name=\"IsTopMenu\" />" +
                                    "<FieldRef Name=\"PageUrl\" />" +
                                    "<FieldRef Name=\"Index\" />" +
                                    "<FieldRef Name=\"TitleSort\" />";
            return ndViewFields;
        }

        private System.Xml.XmlNode CreateQueryOptionsNode(System.Xml.XmlDocument xmlDoc)
        {
            var ndQueryOptions = xmlDoc.CreateNode(System.Xml.XmlNodeType.Element, "QueryOptions", "");
            ndQueryOptions.InnerXml = "<IncludeMandatoryColumns>False</IncludeMandatoryColumns>" +
                                      "<ViewAttributes Scope=\"RecursiveAll\"/><DateInUtc>TRUE</DateInUtc>";
            return ndQueryOptions;
        }

        private void BulkInsertData(DataTable dtBulk, string connectionString)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (var bulkCopy = new SqlBulkCopy(conn))
                {
                    bulkCopy.DestinationTableName = "TempCoQuanBanHanh";

                    // Ánh xạ các cột từ DataTable sang bảng SQL
                    bulkCopy.ColumnMappings.Add("Title", "Title");
                    bulkCopy.ColumnMappings.Add("TitleSort", "TitleSort");
                    bulkCopy.ColumnMappings.Add("IndexCoQuan", "IndexCoQuan");
                    bulkCopy.ColumnMappings.Add("ParentId", "ParentId");
                    bulkCopy.ColumnMappings.Add("PageUrl", "PageUrl");
                    bulkCopy.ColumnMappings.Add("Description", "Description");
                    bulkCopy.ColumnMappings.Add("IsTopMenu", "IsTopMenu");
                    bulkCopy.ColumnMappings.Add("IsShow", "IsShow");
                    bulkCopy.ColumnMappings.Add("IsDBQH", "IsDBQH");
                    bulkCopy.ColumnMappings.Add("CategoryAllID", "CategoryAllID");
                    bulkCopy.ColumnMappings.Add("OldId", "OldId");

                    // Thực hiện ghi hàng loạt vào bảng
                    bulkCopy.WriteToServer(dtBulk);
                }
            }
        }
        // Insert data danh mục SP và DB trung gian
        public void InsertData()
        {
            using (QHBASE.QuocHoiVNEntities db = new QHBASE.QuocHoiVNEntities())
            {
                string stSite = txtSiteUrl.Text.Substring(0, txtSiteUrl.Text.LastIndexOf('/'))
                                .ToLower().Replace("/lists", "").Replace("/tt", "");
                SPService.Lists listSV = UtilsBase.GetListSV(stSite, txtUsername.Text, txtPassword.Text);

                var ndListView = listSV.GetListAndView(ListCatName, "");
                var xmlDoc = new System.Xml.XmlDocument();

                var ndQuery = CreateQueryNode(xmlDoc);
                var ndViewFields = CreateViewFieldsNode(xmlDoc);
                var ndQueryOptions = CreateQueryOptionsNode(xmlDoc);

                System.Xml.XmlNode ndListItems = listSV.GetListItems(ListCatName, "", ndQuery, ndViewFields, "1000", ndQueryOptions, string.Empty);
                DataTable tableFull = UtilsBase.ConvertXmlToDataTable(ndListItems);

                DataTable dtBulk = new DataTable();
                dtBulk.Columns.Add("OldId", typeof(int));
                dtBulk.Columns.Add("Title", typeof(string));
                dtBulk.Columns.Add("ParentId", typeof(int));
                dtBulk.Columns.Add("CategoryAllID", typeof(string));
                dtBulk.Columns.Add("Description", typeof(string));
                dtBulk.Columns.Add("IsDBQH", typeof(int));
                dtBulk.Columns.Add("IsShow", typeof(int));
                dtBulk.Columns.Add("IsTopMenu", typeof(int));
                dtBulk.Columns.Add("TitleSort", typeof(string));
                dtBulk.Columns.Add("PageUrl", typeof(string));
                dtBulk.Columns.Add("IndexCoQuan", typeof(string));
                foreach (DataRow item in tableFull.Rows)
                {
                    DataRow row = dtBulk.NewRow();
                    row["OldId"] = Convert.ToInt32(item["ows_ID"]);
                    row["Title"] = Convert.ToString(item["ows_Title"]);

                    if (ndListItems.InnerXml.Contains("ows_Parent") && item["ows_Parent"] != null)
                        row["ParentId"] = UtilsBase.getLookup(Convert.ToString(item["ows_Parent"]));
                    else
                        row["ParentId"] = DBNull.Value;

                    row["CategoryAllID"] = Convert.ToString(item["ows_CategoryAllID"]);
                    //row["Description"] = Convert.ToString(item["ows_Description"]);
                    row["IsDBQH"] = Convert.ToInt32(item["ows_IsDBQH"]);
                    row["IsShow"] = Convert.ToInt32(item["ows_IsShow"]);
                    row["IsTopMenu"] = ndListItems.InnerXml.Contains("ows_IsTopMenu") && !Convert.IsDBNull(item["ows_IsTopMenu"]) ?
                                       Convert.ToInt32(item["ows_IsTopMenu"]) : 0;
                    row["TitleSort"] = Convert.ToString(item["ows_TitleSort"]);
                   // row["PageUrl"] = Convert.ToString(item["ows_PageUrl"]);
                    if (item["ows_Index"].ToString().Length == 1)
                    {
                        row["IndexCoQuan"] = Convert.ToString(item["ows_Index"]);
                    }
                    else
                    {
                        int index = Convert.ToString(item["ows_Index"]).IndexOf('.');
                        row["IndexCoQuan"] = Convert.ToString(item["ows_Index"]).Substring(0, index);
                    }

                    dtBulk.Rows.Add(row);
                }

                // Thực hiện bulk insert
                BulkInsertData(dtBulk, db.Database.Connection.ConnectionString);

                txtLog.Text = "Dữ liệu đã được thêm mới thành công.\r\n" + txtLog.Text;
            }
        }


        private void rjButton1_Click(object sender, EventArgs e)
        {
            //var a =  GetIDtree();
            // updateCatTreeQH2024();
        }

        List<QHBASE.DMKyHop> LtsAllCatKH2024 = new List<DMKyHop>();

        public void viewCatTreeQH2024()
        {
            treeView2024.Width = 400;
            treeView2024.Dock = DockStyle.Left;
            // Lấy tất cả dữ liệu Category một lần duy nhất
            LtsAllCatKH2024 = GetAllCatKyHop2024().OrderBy(o => o.OldId).ToList();

            // Lấy các node gốc (CatParentId = 0)
            var rootCategories = LtsAllCatKH2024.Where(c => c.ParentId == null).ToList();

            // Duyệt qua danh sách root để thêm vào TreeView
            foreach (var rootCategory in rootCategories)
            {
                // Tạo node gốc
                TreeNode rootNode = new TreeNode(rootCategory.Title)
                {
                    Tag = rootCategory.Id  // Gán ID vào Tag
                };

                // Gọi hàm đệ quy để thêm các node con
                if (rootCategory.ParentId == null)
                {
                    AddChildNodes(rootNode, rootCategory.Id);
                }

                // Thêm node gốc vào TreeView
                treeView2024.Nodes.Add(rootNode);
            }

            // Mở rộng tất cả các node và làm đậm các node cha
            treeView2024.CollapseAll();
            UtilsBase.BoldParentNodes(treeView2024);

            // Thêm TreeView vào panel
            Rjpanelcate.Controls.Add(treeView2024);
        }


        void AddChildNodes(TreeNode parentNode, Guid? parentId)
        {
            // Tìm các node con có CatParentId = parentId
            var childCategories = LtsAllCatKH2024.Where(c => c.ParentId == parentId).ToList();

            foreach (var childCategory in childCategories)
            {
                // Tạo node con
                TreeNode childNode = new TreeNode(childCategory.Title)
                {
                    Tag = childCategory.Id  // Gán ID vào Tag
                };

                // Gọi đệ quy để thêm các node con của childNode
                AddChildNodes(childNode, childCategory.Id);

                // Thêm node con vào node cha
                parentNode.Nodes.Add(childNode);
            }
        }

        private void btnConvert_Click(object sender, EventArgs e)
        {
            try
            {
                using (var context = new QuocHoiVNEntities())
                {
                    // Get ALL lĩnh vực đã đồng bộ
                    var coQuanBanhHanhs = context.TempCoQuanBanHanhs.ToList();
                    if (coQuanBanhHanhs != null && coQuanBanhHanhs.Count > 0)
                    {
                        txtLog.Text = "Bắt đầu đồng bộ.\r\n" ;
                        var LtsAllData = coQuanBanhHanhs.OrderBy(o => o.Id).ToList();
                        BuildFlatCategoryList(LtsAllData, 0, null, null);
                        Console.WriteLine($"Danh mục Cơ quan ban hành đã được cập nhật và lưu thành công.");
                        txtLog.Text += "Danh mục lĩnh vực đã được cập nhật và lưu thành công.\r\n";

                    }
                    else
                    {
                        txtLog.Text = "Danh mục Trỗng.\r\n" + txtLog.Text;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static void BuildFlatCategoryList(List<TempCoQuanBanHanh> allCategories, int? parentId, Guid? Id2024, string name2024)
        {
            // Lấy tất cả các danh mục con của parent hiện tại
            var subCategories = allCategories.Where(c => c.ParentId == parentId).OrderByDescending(c => c.OldId).ToList();
            if (subCategories != null && subCategories.Count > 0)
            {

                foreach (var item in subCategories)
                {
                    // insert Thông tin chung
                    var coQuanItem = new CoQuanBanHanh
                    {
                        //Id = Guid.NewGuid(),
                        Title = item.Title,
                        OldID = item.OldId,
                        ParentOldID = item.ParentId,
                        Order = item.IndexCoQuan,
                        CreatorName = "admin",
                        CreatorId = new Guid("FF72B967-9734-4186-2035-3A13FBAD897B"),
                        CreationTime = DateTime.Now,
                        IsShow = true,
                        //ParentId = Id2024,
                        DetailCategoryRoot = name2024 != null ? name2024 : string.Empty,
                        TreeParentIds = Id2024 != null ? Id2024.ToString() : string.Empty,
                    };
                    var id2024 = InsertCategoryInfoCQBH(coQuanItem);
                    BuildFlatCategoryList(allCategories, item.OldId, id2024, item.Title); // Đệ quy cho các danh mục con)
                }
            }

        }

        public static Guid InsertCategoryInfoCQBH(CoQuanBanHanh item)
        {
            using (Preview_QuocHoi_CommonsServiceEntities db = new Preview_QuocHoi_CommonsServiceEntities())
            {
                // Tìm danh mục theo OldId
                var kyhop = db.CoQuanBanHanhs.Add(item);

                // Lưu thay đổi vào cơ sở dữ liệu
                db.SaveChanges();
                return Guid.Empty;
            }


        }

    }
}

