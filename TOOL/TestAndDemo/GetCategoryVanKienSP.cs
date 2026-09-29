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
    public partial class GetCategoryVanKienSP : RJForms.RJChildForm
    {
        private string ListCatName = "Danh mục kỳ họp";
        List<QHBASE.VanKienCat> LtsAllCategory = new List<VanKienCat>();
        List<VanKienCat> LtsAllCategoryTemp = new List<VanKienCat>();
        TreeView treeView = new TreeView();
        TreeView treeView2024 = new TreeView();
        public GetCategoryVanKienSP()
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
                    "<FieldRef Name=\"IsShow\" />" +
                    "<FieldRef Name=\"IsDBQH\" />" +
                    "<FieldRef Name=\"IsTopMenu\" />" +
                    "<FieldRef Name=\"PageUrl\" />" +
                    "<FieldRef Name=\"Index\" />" +
                    "<FieldRef Name=\"TitleSort\" /> " +
                    "<FieldRef Name=\"CateID\" />" +
                    "<FieldRef Name=\"CateType\" />" +
                    "<FieldRef Name=\"CategoryIDTintuc\" />";

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
                    VanKienCat catItem = new VanKienCat();
                    catItem.CatID = Convert.ToInt32(item["ows_ID"]);
                    catItem.CatName = Convert.ToString(item["ows_Title"]);
                    if (ndListItems.InnerXml.Contains("ows_Parent"))
                    {
                        if (item["ows_Parent"] != null)
                        {

                            catItem.CatParentId = UtilsBase.getLookup(Convert.ToString(item["ows_Parent"]));
                        }
                    }
                    rowCount++;
                    processBar.Value = rowCount;
                    LtsAllCategoryTemp.Add(catItem);
                }
                LtsAllCategory = LtsAllCategoryTemp.OrderBy(o => o.CatID).ToList();
                TreeView treeView = new TreeView();
                treeView.Width = 400;
                treeView.Dock = DockStyle.Left;
                // Lấy các node gốc (ParentId = null)
                var rootCategories = LtsAllCategoryTemp.Where(c => c.CatParentId == 0).ToList();

                // Duyệt qua danh sách root để thêm vào TreeView
                foreach (var rootCategory in rootCategories)
                {
                    TreeNode rootNode = new TreeNode(rootCategory.CatName);
                    rootNode.Tag = rootCategory.CatID; // Đính kèm ID của Category để dễ dàng thao tác sau này
                    rootNode.NodeFont = new Font(treeView.Font, FontStyle.Bold);
                    // Gọi hàm đệ quy để thêm các node con
                    UtilsBase.AddChildNodesVanKien(rootNode, LtsAllCategory);

                    treeView.Nodes.Add(rootNode);

                }

                treeView.CollapseAll();
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
            var rootCategories = GetAllDataCate().Where(c => c.CatParentId == 0).ToList();
            LtsAllCategory = GetAllDataCate().OrderBy(o => o.CatID).ToList();
            // Duyệt qua danh sách root để thêm vào TreeView
            foreach (var rootCategory in rootCategories)
            {
                TreeNode rootNode = new TreeNode(rootCategory.CatName);
                rootNode.Tag = rootCategory.CatOldID;
                // Đính kèm ID của Category để dễ dàng thao tác sau này

                // Gọi hàm đệ quy để thêm các node con
                UtilsBase.AddChildNodesSQLVanKien(rootNode, LtsAllCategory);
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
       
      
        public static List<VanKienCat> GetAllDataCate()
        {
            using (var context = new QuocHoiVNEntities())
            {
                // Get all items from the table
                var allItems = context.VanKienCats.ToList();
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
                var allItems = context.VanKienCats.ToList();

                // Remove all items
                context.VanKienCats.RemoveRange(allItems);

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
                    "<FieldRef Name=\"TitleSort\" /> " +
                    "<FieldRef Name=\"CateID\" />" +
                    "<FieldRef Name=\"CateType\" />" +
                    "<FieldRef Name=\"CategoryIDTintuc\" />";
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
                    bulkCopy.DestinationTableName = "VanKienCat";

                    // Ánh xạ các cột từ DataTable sang bảng SQL
                    bulkCopy.ColumnMappings.Add("CatOldID", "CatOldID");
                    bulkCopy.ColumnMappings.Add("CatName", "CatName");
                    bulkCopy.ColumnMappings.Add("CatParentId", "CatParentId");
                    bulkCopy.ColumnMappings.Add("CategoryAllID", "CategoryAllID");
                    bulkCopy.ColumnMappings.Add("Description", "Description");
                    bulkCopy.ColumnMappings.Add("IsDBQH", "IsDBQH");
                    bulkCopy.ColumnMappings.Add("IsShow", "IsShow");
                    bulkCopy.ColumnMappings.Add("IsTopMenu", "IsTopMenu");
                    bulkCopy.ColumnMappings.Add("TitleSort", "TitleSort");
                    //bulkCopy.ColumnMappings.Add("CatUrl", "CatUrl");
                    bulkCopy.ColumnMappings.Add("IndexCat", "IndexCat");
                    bulkCopy.ColumnMappings.Add("CateID", "CateID");
                    bulkCopy.ColumnMappings.Add("CateType", "CateType");
                    bulkCopy.ColumnMappings.Add("CategoryIDTintuc", "CategoryIDTintuc");

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

                System.Xml.XmlNode ndListItems = listSV.GetListItems(ListCatName, "", ndQuery, ndViewFields, "10000", ndQueryOptions, string.Empty);
                DataTable tableFull = UtilsBase.ConvertXmlToDataTable(ndListItems);

                DataTable dtBulk = new DataTable();
                dtBulk.Columns.Add("CatOldID", typeof(int));
                dtBulk.Columns.Add("CatName", typeof(string));
                dtBulk.Columns.Add("CatParentId", typeof(int));
                dtBulk.Columns.Add("CategoryAllID", typeof(string));
                dtBulk.Columns.Add("Description", typeof(string));
                dtBulk.Columns.Add("IsDBQH", typeof(int));
                dtBulk.Columns.Add("IsShow", typeof(int));
                dtBulk.Columns.Add("IsTopMenu", typeof(int));
                dtBulk.Columns.Add("TitleSort", typeof(string));
                dtBulk.Columns.Add("CateID", typeof(int));
                dtBulk.Columns.Add("CateType", typeof(int));
                dtBulk.Columns.Add("CategoryIDTintuc", typeof(int));
                dtBulk.Columns.Add("IndexCat", typeof(int));
                foreach (DataRow item in tableFull.Rows)
                {
                    DataRow row = dtBulk.NewRow();
                    row["CatOldID"] = Convert.ToInt32(item["ows_ID"]);
                    row["CatName"] = Convert.ToString(item["ows_Title"]);

                    if (ndListItems.InnerXml.Contains("ows_Parent") && item["ows_Parent"] != null)
                        row["CatParentId"] = UtilsBase.getLookup(Convert.ToString(item["ows_Parent"]));
                    else
                        row["CatParentId"] = DBNull.Value;

                    row["CategoryAllID"] = Convert.ToString(item["ows_CategoryAllID"]);
                    row["Description"] = Convert.ToString(item["ows_Description"]);
                    row["IsDBQH"] = Convert.ToInt32(item["ows_IsDBQH"]);
                    row["IsShow"] = Convert.ToInt32(item["ows_IsShow"]);
                    row["IsTopMenu"] = ndListItems.InnerXml.Contains("ows_IsTopMenu") && !Convert.IsDBNull(item["ows_IsTopMenu"]) ?
                                       Convert.ToInt32(item["ows_IsTopMenu"]) : 0;
                    row["TitleSort"] = Convert.ToString(item["ows_TitleSort"]);
                    //row["CatUrl"] = Convert.ToString(item["ows_PageUrl"]);

                    if (item["ows_Index"].ToString().Length == 1)
                    {
                        row["IndexCat"] = Convert.ToString(item["ows_Index"]);
                    }
                    else
                    {
                        int index = Convert.ToString(item["ows_Index"]).IndexOf('.');
                        row["IndexCat"] = Convert.ToString(item["ows_Index"]).Substring(0, index);
                    }

                    if (!string.IsNullOrEmpty(item["ows_CateID"].ToString()))
                    {
                        if (item["ows_CateID"].ToString().Length == 1)
                        {
                            row["CateID"] = Convert.ToString(item["ows_CateID"]);
                        }
                        else
                        {
                            int index = Convert.ToString(item["ows_CateID"]).IndexOf('.');
                            row["CateID"] = Convert.ToString(item["ows_CateID"]).Substring(0, index);
                        }
                    }
                    else
                    {
                        row["CateID"] = DBNull.Value;
                    }
                    if (!string.IsNullOrEmpty(item["ows_CateType"].ToString()))
                    {
                        if (item["ows_CateType"].ToString().Length == 1)
                        {
                            row["CateType"] = Convert.ToString(item["ows_CateType"]);
                        }
                        else
                        {
                            int index = Convert.ToString(item["ows_CateType"]).IndexOf('.');
                            row["CateType"] = Convert.ToString(item["ows_CateType"]).Substring(0, index);
                        }
                    }
                    else
                    {
                        row["CateType"] = DBNull.Value;
                    }
                    if (!string.IsNullOrEmpty(item["ows_CategoryIDTintuc"].ToString()))
                    {
                        if (item["ows_CategoryIDTintuc"].ToString().Length == 1)
                        {
                            row["CategoryIDTintuc"] = Convert.ToString(item["ows_CategoryIDTintuc"]);
                        }
                        else
                        {
                            int index = Convert.ToString(item["ows_CategoryIDTintuc"]).IndexOf('.');
                            row["CategoryIDTintuc"] = Convert.ToString(item["ows_CategoryIDTintuc"]).Substring(0, index);
                        }
                    }
                    else
                    {
                        row["CategoryIDTintuc"] = DBNull.Value;
                    }



                    dtBulk.Rows.Add(row);
                }

                // Thực hiện bulk insert
                BulkInsertData(dtBulk, db.Database.Connection.ConnectionString);

                txtLog.Text = "Dữ liệu đã được thêm mới thành công.\r\n" + txtLog.Text;
            }
        }
        // Convert DB trung gian vào DB kỳ hop


        private void grSP_Enter(object sender, EventArgs e)
        {

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
                    AddChildNodesVanKien(rootNode, rootCategory.Id);
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


        void AddChildNodesVanKien(TreeNode parentNode, Guid? parentId)
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
                AddChildNodesVanKien(childNode, childCategory.Id);

                // Thêm node con vào node cha
                parentNode.Nodes.Add(childNode);
            }
        }

        private void btnConvert_Click(object sender, EventArgs e)
        {

            var (tree2024, treeole) = GetIDsFromTreeViews();
            if (treeole.HasValue && tree2024.HasValue)
            {
                LtsAllCategory = GetAllDataCate().OrderBy(o => o.CatID).ToList();
                BuildFlatCategoryList(LtsAllCategory, treeole, tree2024);
                MessageBox.Show("chuyển đổi dữ liệu danh mục thành công");
            }
            else
            {
                MessageBox.Show(" bấm nút view tree và chọn chuyên mục cần chuuyeenr đổi");
            }
        }

        public static void BuildFlatCategoryList(List<VanKienCat> allCategories, int? parentId, Guid? Id2024)
        {
            // Lấy tất cả các danh mục con của parent hiện tại
            var subCategories = allCategories.Where(c => c.CatParentId == parentId).OrderByDescending(c =>c.CatOldID).ToList();
            foreach (var subCategory in subCategories)
            {

                var kyhop = new DMKyHop();
                kyhop.Id = Guid.NewGuid();
                kyhop.ParentId = Id2024;
                kyhop.OldId = subCategory.CatOldID;
                kyhop.Title = subCategory.CatName;
                kyhop.ParentOldId = subCategory.CatParentId;
                //kyhop.UrlLink = subCategory.CatUrl;
                kyhop.IsShow = UtilsBase.ConvertIntToBool(Convert.ToInt32(subCategory.IsShow));
               // kyhop.Order = Convert.ToInt32(subCategory.IndexCat);
                var id2024 = InsertCategoryInfoKH(kyhop);
                BuildFlatCategoryList(allCategories, subCategory.CatOldID, kyhop.Id); // Đệ quy cho các danh mục con
            }
        }
       
        public static void BuildFlatCategoryListKH(List<DMKyHop> allCategories1, int? parentId)
        {

            // Lấy tất cả các danh mục con của parent hiện tại
            var subCategories = allCategories1.Where(c => c.ParentOldId == parentId).ToList();

            foreach (var subCategorydm in subCategories)
            {
                //resultList1.Add(subCategorydm);
                UpdateCategoryInfoKH(allCategories1, subCategorydm.ParentOldId);
                BuildFlatCategoryListKH(allCategories1, subCategorydm.OldId); // Đệ quy cho các danh mục con
            }
        }

        public static void UpdateCategoryInfoKH(List<DMKyHop> allCategories, int? oldId)
        {
            using (Preview_QuocHoi_KyHopServiceEntities db = new Preview_QuocHoi_KyHopServiceEntities())
            {
                // Tìm danh mục theo OldId

                var categoryToUpdate = db.DMKyHops.FirstOrDefault(c => c.ParentOldId == oldId);
                var subCateOld = allCategories.FirstOrDefault(c => c.OldId == oldId);

                if (categoryToUpdate != null)
                {
                    // Cập nhật thông tin danh mục
                    categoryToUpdate.ParentId = subCateOld.Id;

                    try
                    {
                        // Lưu thay đổi vào cơ sở dữ liệu
                        db.SaveChanges();
                        Console.WriteLine($"Danh mục {categoryToUpdate.Title} đã được cập nhật và lưu thành công.");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Có lỗi xảy ra khi lưu vào cơ sở dữ liệu: {ex.Message}");
                    }
                }
                else
                {
                    Console.WriteLine("Không tìm thấy danh mục cần cập nhật.");
                }
            }


        }
       
        public static Guid InsertCategoryInfoKH(DMKyHop kyhopitem)
        {
            using (Preview_QuocHoi_KyHopServiceEntities db = new Preview_QuocHoi_KyHopServiceEntities())
            {
                // Tìm danh mục theo OldId
                var kyhop = db.DMKyHops.Add(kyhopitem);

                // Lưu thay đổi vào cơ sở dữ liệu
                db.SaveChanges();
                return kyhop.Id;
            }


        }
    }
}

