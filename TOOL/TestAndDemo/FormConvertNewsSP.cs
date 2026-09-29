using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EntityFramework.BulkInsert;
using System.Windows.Forms;
using QHBASE;

using System.Data.SqlClient;
using System.Data.Entity;
using System.Xml;
using System.Xml.Linq;
using System.Data.Entity.Validation;


namespace RJCodeUI_M1.TestAndDemo
{
    public partial class FormConvertNewsSP : RJForms.RJChildForm
    {

        private string ListNewsName = "Danh sách Bài viết";
        List<News> LtsAllCategory = new List<News>();
        List<News> LtsAllCategoryTemp = new List<News>();
        List<DMKyHop> LtsAllCate = new List<DMKyHop>();
        public FormConvertNewsSP()
        {

            InitializeComponent();
        }
        public FormConvertNewsSP(Models.User user)
        {

            InitializeComponent();


        }

        private void btnSave_Click(object sender, EventArgs e)
        {

        }

        private void btnConnect_Click(object sender, EventArgs e)
        {

            using (var db = new QHBASE.QuocHoiVNEntities())
            {
                try
                {
                    string stSite = txtSiteUrl.Text.Substring(0, txtSiteUrl.Text.LastIndexOf('/'))
                                                    .ToLower()
                                                    .Replace("/lists", string.Empty)
                                                    .Replace("/tt", "");

                    var listSV = UtilsBase.GetListSV(stSite, txtUsername.Text, txtPassword.Text);
                    var ndListView = listSV.GetListAndView(ListNewsName, "");
                    string strListID = ndListView.ChildNodes[0].Attributes["Name"].Value;

                    //=> 33: tin tức chung, 23: tin tức chuyên nghành, 1: thử nghiệm xe cơ giới, 
                    //=> 22: đường thủy, 21: tin quốc tế, 158: thông tin báo chí, 32: Tin IMO
                    var categoryId = Convert.ToInt32(txtCategoryId.Text);

                    // Tạo XML truy vấn với tham số hoá
                    var xmlDoc = new System.Xml.XmlDocument();
                    var ndQuery = xmlDoc.CreateElement("Query");
                    var ndViewFields = xmlDoc.CreateElement("ViewFields");
                    var ndQueryOptions = xmlDoc.CreateElement("QueryOptions");

                    ndViewFields.InnerXml = CreateViewFieldXML2();
                    int startID = Convert.ToInt32(txtIdStart.Text);
                    int endID = Convert.ToInt32(txtIdEnd.Text);
                    string soluong = Convert.ToString(txtSoluong.Text);
                    ndQuery.InnerXml = BuildQueryXML2(startID, endID, categoryId);
                    ndQueryOptions.InnerXml = "<IncludeMandatoryColumns>False</IncludeMandatoryColumns><ViewAttributes Scope=\"RecursiveAll\"/><DateInUtc>TRUE</DateInUtc>";

                    // Thực hiện truy vấn và xử lý kết quả
                    var ndListItems = listSV.GetListItems(ListNewsName, "", ndQuery, ndViewFields, soluong, ndQueryOptions, string.Empty);
                    // MessageBox.Show(UtilsBase.ConverToString(listSV.GetListItems(ListNewsName, "", ndQuery, ndViewFields, soluong, ndQueryOptions, string.Empty), 100));

                    var TableFull = UtilsBase.ConvertXmlToDataTable(ndListItems);
                    var existingIds = new HashSet<int?>(db.News.Select(n => n.OldID).ToList()); // Lưu trữ OldID đã tồn tại

                    // processBar.Maximum = TableFull.Rows.Count;
                    if (TableFull != null)
                        InsertDataIntoDatabase(TableFull, 1);
                    MessageBox.Show(" tham mowis thanfh cong");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"*** ERR: {ex.Message}\r\n");
                }
            }

        }




        public string BuildQueryXML(int startID, int endID)
        {
            StringBuilder queryXml = new StringBuilder(); queryXml.AppendLine("<Where>");
            queryXml.AppendLine("  <And>");
            queryXml.AppendLine("    <Eq>");
            queryXml.AppendLine("      <FieldRef Name=\"_ModerationStatus\" />");
            queryXml.AppendLine("      <Value Type=\"ModStat\">0</Value>");  // Sửa lại giá trị là Approved thay vì 0
            queryXml.AppendLine("    </Eq>");
            queryXml.AppendLine("    <And>");
            queryXml.AppendLine("      <Geq>");
            queryXml.AppendLine("        <FieldRef Name=\"ID\" />");
            queryXml.AppendLine($"        <Value Type=\"Counter\">{startID}</Value>");
            queryXml.AppendLine("      </Geq>");
            queryXml.AppendLine("      <Lt>");
            queryXml.AppendLine("        <FieldRef Name=\"ID\" />");
            queryXml.AppendLine($"        <Value Type=\"Counter\">{endID}</Value>");
            queryXml.AppendLine("      </Lt>");
            queryXml.AppendLine("    </And>");
            queryXml.AppendLine("  </And>");
            queryXml.AppendLine("</Where>");

            queryXml.AppendLine("<OrderBy>");
            queryXml.AppendLine("  <FieldRef Name=\"ID\" Ascending=\"True\" />");
            queryXml.AppendLine("</OrderBy>");

            return queryXml.ToString();
        }
        public string BuildQuery(int startID, int endID)
        {
            var sb = new StringBuilder();
            sb.AppendLine("   <Where>");
            sb.AppendLine("      <And>");
            // sb.AppendLine("         <And>");
            // sb.AppendLine("            <IsNotNull>");
            // sb.AppendLine("               <FieldRef Name='Category' />");
            //  sb.AppendLine("            </IsNotNull>");
            sb.AppendLine("            <Eq>");
            sb.AppendLine("               <FieldRef Name='_ModerationStatus' />");
            sb.AppendLine("                <Value Type=\"ModStat\">0</Value>");
            sb.AppendLine("            </Eq>");
            //  sb.AppendLine("         </And>");
            sb.AppendLine("         <And>");
            sb.AppendLine("            <Geq>");
            sb.AppendLine($"               <FieldRef Name='ID' />");
            sb.AppendLine($"               <Value Type='Counter'>{startID}</Value>");
            sb.AppendLine("            </Geq>");
            sb.AppendLine("            <Leq>");
            sb.AppendLine($"               <FieldRef Name='ID' />");
            sb.AppendLine($"               <Value Type='Counter'>{endID}</Value>");
            sb.AppendLine("            </Leq>");
            sb.AppendLine("         </And>");
            sb.AppendLine("      </And>");
            sb.AppendLine("   </Where>");

            return sb.ToString();
        }
        public string CreateViewUBTVFieldXML()
        {
            // Sử dụng StringBuilder để tạo chuỗi XML
            StringBuilder stbBuilder = new StringBuilder();

            // Thêm các FieldRef vào StringBuilder
            // ID bai viet
            stbBuilder.AppendLine("<FieldRef Name=\"ID\" />");
            // Tiêu đề bài viết
            stbBuilder.AppendLine("<FieldRef Name=\"Title\" />");
            // ảnh đại diện
            stbBuilder.AppendLine("<FieldRef Name=\"Image\" />");

            // Cho pheo gui bình luận
            stbBuilder.AppendLine("<FieldRef Name=\"AllowComment\" />");
            // hiển thị mô tả
            stbBuilder.AppendLine("<FieldRef Name=\"ShowDescription\" />");
            //Chuyên mục
            stbBuilder.AppendLine("<FieldRef Name=\"Category\" LookupId=\"True\" />");
            //Ngày tạo
            stbBuilder.AppendLine("<FieldRef Name=\"CreateDate\" />");

            stbBuilder.AppendLine("<FieldRef Name=\"ShowImage\" />");
            stbBuilder.AppendLine("<FieldRef Name=\"Description\" />");
            stbBuilder.AppendLine("<FieldRef Name=\"Content\" />");
            stbBuilder.AppendLine("<FieldRef Name=\"ReadCount\" />");
            stbBuilder.AppendLine("<FieldRef Name=\"AuthorNews\" />");
            stbBuilder.AppendLine("<FieldRef Name=\"SourceNews\" />");
            stbBuilder.AppendLine("<FieldRef Name=\"Created\" />");
            stbBuilder.AppendLine("<FieldRef Name=\"Author\" />");

            stbBuilder.AppendLine("<FieldRef Name=\"Keywords\" />");
            stbBuilder.AppendLine("<FieldRef Name=\"Event\" LookupId=\"True\" />");
            stbBuilder.AppendLine("<FieldRef Name=\"Hotnews\" />");
            stbBuilder.AppendLine("<FieldRef Name=\"IsLive\" />");
            stbBuilder.AppendLine("<FieldRef Name=\"IsShare\" />");
            stbBuilder.AppendLine("<FieldRef Name=\"IsShowView\" />");
            stbBuilder.AppendLine("<FieldRef Name=\"_ModerationStatus\" />");
            stbBuilder.AppendLine("<FieldRef Name=\"Attachments\" />");
            // Trả về chuỗi XML đã xây dựng
            return stbBuilder.ToString();
        }
        public string CreateViewFieldXML()
        {
            // Sử dụng StringBuilder để tạo chuỗi XML
            StringBuilder stbBuilder = new StringBuilder();

            // Thêm các FieldRef vào StringBuilder
            // ID bai viet
            stbBuilder.AppendLine("<FieldRef Name=\"ID\" />");
            // Tiêu đề bài viết
            stbBuilder.AppendLine("<FieldRef Name=\"Title\" />");
            // ảnh đại diện
            stbBuilder.AppendLine("<FieldRef Name=\"Image\" />");
            //Hiển thị mô tả
            stbBuilder.AppendLine("<FieldRef Name=\"AutoDescription\" />");
            // Cho pheo gui bình luận
            stbBuilder.AppendLine("<FieldRef Name=\"AllowComment\" />");
            // hiển thị mô tả
            stbBuilder.AppendLine("<FieldRef Name=\"ShowDescription\" />");
            //Chuyên mục
            stbBuilder.AppendLine("<FieldRef Name=\"Category\" LookupId=\"True\" />");
            //Ngày tạo
            stbBuilder.AppendLine("<FieldRef Name=\"CreateDate\" />");
            //Loại Tin
            stbBuilder.AppendLine("<FieldRef Name=\"IdLoaiTin\" LookupId=\"True\"/>");
            stbBuilder.AppendLine("<FieldRef Name=\"ShowImage\" />");
            stbBuilder.AppendLine("<FieldRef Name=\"Description\" />");
            stbBuilder.AppendLine("<FieldRef Name=\"Content\" />");
            stbBuilder.AppendLine("<FieldRef Name=\"ReadCount\" />");
            stbBuilder.AppendLine("<FieldRef Name=\"AuthorNews\" />");
            stbBuilder.AppendLine("<FieldRef Name=\"SourceNews\" />");
            stbBuilder.AppendLine("<FieldRef Name=\"Created\" />");
            stbBuilder.AppendLine("<FieldRef Name=\"Author\" />");

            stbBuilder.AppendLine("<FieldRef Name=\"TrangThaiTin\" />");
            stbBuilder.AppendLine("<FieldRef Name=\"Keywords\" />");
            stbBuilder.AppendLine("<FieldRef Name=\"Event\" LookupId=\"True\" />");
            stbBuilder.AppendLine("<FieldRef Name=\"Hotnews\" />");
            stbBuilder.AppendLine("<FieldRef Name=\"IsLive\" />");
            stbBuilder.AppendLine("<FieldRef Name=\"IsShare\" />");
            stbBuilder.AppendLine("<FieldRef Name=\"IsShowView\" />");
            stbBuilder.AppendLine("<FieldRef Name=\"_ModerationStatus\" />");
            stbBuilder.AppendLine("<FieldRef Name=\"Attachments\" />");
            // Trả về chuỗi XML đã xây dựng
            return stbBuilder.ToString();
        }


        static void InsertDataIntoDatabase(DataTable entities, int siteid)
        {
            string connectionString = "data source=172.16.1.191;initial catalog=QuocHoiVN;persist security info=True;user id=sa;password=Server@20@#;MultipleActiveResultSets=True;App=EntityFramework&quot"; // Thay đổi chuỗi kết nối

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection))
                {
                    bulkCopy.DestinationTableName = "News"; // Tên bảng trong DB

                    // Định nghĩa mapping cột nếu cần
                    //bulkCopy.ColumnMappings.Add("OldID", "OldID");

                    // Thêm các FieldRef vào StringBuilder
                    // ID bai viet
                    bulkCopy.ColumnMappings.Add("ows_ID", "OldID");
                    // Tiêu đề bài viết
                    bulkCopy.ColumnMappings.Add("ows_Title", "Title");
                    // ảnh đại diện
                    if (entities.Columns.Contains("ows_Image"))
                    {
                        bulkCopy.ColumnMappings.Add("ows_Image", "Image");
                    }
                    //Hiển thị mô tả
                    //   bulkCopy.ColumnMappings.Add("ows_AutoDescription", "AutoDescription");
                    // Cho pheo gui bình luận
                    if (entities.Columns.Contains("ows_AllowComment"))
                    {
                        bulkCopy.ColumnMappings.Add("ows_AllowComment", "AllowComment");
                    }
                    // hiển thị mô tả
                    //bulkCopy.ColumnMappings.Add("ows_ShowDescription", "ShowDescription");
                    //Chuyên mục
                    if (entities.Columns.Contains("ows_Categories"))
                        bulkCopy.ColumnMappings.Add("ows_Categories", "Category");
                    if (entities.Columns.Contains("ows_DisplayDate"))
                        bulkCopy.ColumnMappings.Add("ows_DisplayDate", "CreateDate");
                    if (siteid == 1)
                    {
                        //bulkCopy.ColumnMappings.Add("ows_IdLoaiTin", "IdLoaiTin");
                    }
                    //bulkCopy.ColumnMappings.Add("ows_ShowImage", "ShowImage");
                    if (entities.Columns.Contains("ows_Descriptions"))
                        bulkCopy.ColumnMappings.Add("ows_Descriptions", "Description");
                    if (entities.Columns.Contains("ows_Contents"))
                        bulkCopy.ColumnMappings.Add("ows_Contents", "ContentNew");
                    if (entities.Columns.Contains("ows_ReadCount"))
                        bulkCopy.ColumnMappings.Add("ows_ReadCount", "ReadCount");
                    if (entities.Columns.Contains("ows__Author"))
                        bulkCopy.ColumnMappings.Add("ows__Author", "AuthorNews");
                    if (entities.Columns.Contains("ows_SourceNews"))
                        bulkCopy.ColumnMappings.Add("ows_SourceNews", "SourceNews");
                    if (entities.Columns.Contains("ows_Created"))
                        bulkCopy.ColumnMappings.Add("ows_Created", "Created");
                    if (entities.Columns.Contains("ows_Author"))
                        bulkCopy.ColumnMappings.Add("ows_Author", "Author");

                    // bulkCopy.ColumnMappings.Add("ows_TrangThaiTin", "TrangThaiTin");
                    if (siteid == 1)
                    {
                        //bulkCopy.ColumnMappings.Add("ows_Keywords", "Keywords");
                    }
                    if (entities.Columns.Contains("ows_Event"))
                        bulkCopy.ColumnMappings.Add("ows_Event", "Event");
                    if (entities.Columns.Contains("ows_IsHotNews"))
                        bulkCopy.ColumnMappings.Add("ows_IsHotNews", "Hotnews");
                    if (siteid == 1)
                    {
                        //bulkCopy.ColumnMappings.Add("ows_IsLive", "IsLive");
                    }
                    //bulkCopy.ColumnMappings.Add("ows_IsShare", "IsShare");
                    //bulkCopy.ColumnMappings.Add("ows_IsShowView", "IsShowView");
                    if (entities.Columns.Contains("ows__ModerationStatus"))
                        bulkCopy.ColumnMappings.Add("ows__ModerationStatus", "ModerationStatus");
                    if (entities.Columns.Contains("ows_Attachments"))
                        bulkCopy.ColumnMappings.Add("ows_Attachments", "Attachment");

                    bulkCopy.WriteToServer(entities);

                }
            }
        }
        static void InsertDataIntoDatabaseold(DataTable entities)
        {
            string connectionString = "data source=172.16.1.181;initial catalog=QuocHoiVN;persist security info=True;user id=sa;password=Server@20!(;MultipleActiveResultSets=True;App=EntityFramework&quot"; // Thay đổi chuỗi kết nối

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection))
                {
                    bulkCopy.DestinationTableName = "News"; // Tên bảng trong DB

                    // Định nghĩa mapping cột nếu cần
                    //bulkCopy.ColumnMappings.Add("OldID", "OldID");

                    // Thêm các FieldRef vào StringBuilder
                    // ID bai viet
                    bulkCopy.ColumnMappings.Add("ows_ID", "OldID");
                    // Tiêu đề bài viết
                    bulkCopy.ColumnMappings.Add("ows_Title", "Title");
                    // ảnh đại diện
                    bulkCopy.ColumnMappings.Add("ows_Image", "Image");
                    //Hiển thị mô tả
                    //   bulkCopy.ColumnMappings.Add("ows_AutoDescription", "AutoDescription");
                    // Cho pheo gui bình luận
                    //bulkCopy.ColumnMappings.Add("ows_AllowComment", "AllowComment");
                    // hiển thị mô tả
                    // bulkCopy.ColumnMappings.Add("ows_ShowDescription", "ShowDescription");
                    //Chuyên mục
                    bulkCopy.ColumnMappings.Add("ows_Category", "Category");

                    bulkCopy.ColumnMappings.Add("ows_CreateDate", "CreateDate");

                    //bulkCopy.ColumnMappings.Add("ows_IdLoaiTin", "IdLoaiTin");
                    //bulkCopy.ColumnMappings.Add("ows_ShowImage", "ShowImage");
                    bulkCopy.ColumnMappings.Add("ows_Description", "Description");
                    bulkCopy.ColumnMappings.Add("ows_Content", "ContentNew");
                    // bulkCopy.ColumnMappings.Add("ows_ReadCount", "ReadCount");
                    bulkCopy.ColumnMappings.Add("ows_AuthorNews", "AuthorNews");
                    bulkCopy.ColumnMappings.Add("ows_SourceNews", "SourceNews");
                    bulkCopy.ColumnMappings.Add("ows_Created", "Created");
                    bulkCopy.ColumnMappings.Add("ows_Author", "Author");

                    // bulkCopy.ColumnMappings.Add("ows_TrangThaiTin", "TrangThaiTin");

                    // bulkCopy.ColumnMappings.Add("ows_Keywords", "Keywords");

                    // bulkCopy.ColumnMappings.Add("ows_Event", "Event");
                    //bulkCopy.ColumnMappings.Add("ows_Hotnews", "Hotnews");
                    //bulkCopy.ColumnMappings.Add("ows_IsLive", "IsLive");
                    //bulkCopy.ColumnMappings.Add("ows_IsShare", "IsShare");
                    //bulkCopy.ColumnMappings.Add("ows_IsShowView", "IsShowView");
                    bulkCopy.ColumnMappings.Add("ows__ModerationStatus", "ModerationStatus");

                    bulkCopy.WriteToServer(entities);

                }
            }
        }
        private void txtLog_TextChanged(object sender, EventArgs e)
        {

        }

        private async void bntConvert_Click(object sender, EventArgs e)
        {

            //=> var idcatmap = Convert.ToString(GetIDsDMMapTD());
            // Danh sách ID bạn muốn tìm, có thể là 1 hoặc nhiều
            //=> var idcate24 = NewsDA.GetGuid(idcatmap);
            // Kiểm tra nếu danh sách rỗng

            var idCateCDK = txtCategoryIdNew.Text; //=> id trên danh mục category của cổng mới CĐK
            var idcatmap = txtCategoryId.Text; //=> id danh mục category của cổng cũ CĐK

            if (!string.IsNullOrWhiteSpace(idcatmap))
            {
                // Sử dụng Join để tìm kiếm hiệu quả hơn
                // await NewsDA.InsertNew2024_3(idcatmap, idcate24, lblCount, txtLog, lblthuchien);
                var oldIdFrom = Convert.ToInt32(tOldIdFrom.Text);
                var oldIdTo = Convert.ToInt32(tOldIdTo.Text);
                await NewsDA.InsertNew2024_vpqh(idcatmap, idCateCDK, lblCount, txtLog, lblthuchien, oldIdFrom, oldIdTo);
                MessageBox.Show("Đã Insert thành công.");
            }
            else
            {
                Console.WriteLine("Danh sách categoryIds rỗng.");
            }


        }
        public int? GetIDsDMMapTD()
        {
            // Lấy ID từ treeView1
            if (treeCateNews.SelectedNode != null)
            {
                // Kiểm tra nếu Tag là kiểu int
                if (treeCateNews.SelectedNode.Tag is int nodeOld)
                {
                    return nodeOld; // Nếu đúng, trả về giá trị của Tag
                }
                else
                {
                    MessageBox.Show("Giá trị Tag không phải là kiểu int.");
                    return null;
                }
            }
            else
            {
                MessageBox.Show("Chưa có node nào được chọn trong TreeView 1.");
                return null; // Trả về null nếu không có node nào được chọn
            }
        }


        private void btnViewCateSQL_Click(object sender, EventArgs e)
        {
            treeCateNews.Nodes.Clear();
            viewCatTreeCate2024();

        }
        public void viewCatTreeCate2024()
        {
            treeCateNews.Width = 400;
            treeCateNews.Dock = DockStyle.Left;
            // Lấy tất cả dữ liệu Category một lần duy nhất
            var ltsAllCate2024 = GetAllCatNews2024().OrderBy(o => o.Id).ToList();

            // Lấy các node gốc (CatParentId = 0)
            var rootCategories = ltsAllCate2024.Where(c => c.ParentID == null).ToList();

            // Duyệt qua danh sách root để thêm vào TreeView
            foreach (var rootCategory in rootCategories)
            {
                // Tạo node gốc
                TreeNode rootNode = new TreeNode(rootCategory.Title)
                {
                    Tag = rootCategory.CategoryOldId  // Gán ID vào Tag
                };

                // Gọi hàm đệ quy để thêm các node con
                if (rootCategory.ParentID == null)
                {
                    AddChildNodes(rootNode, rootCategory.Id);
                }

                // Thêm node gốc vào TreeView
                treeCateNews.Nodes.Add(rootNode);
            }

            // Mở rộng tất cả các node và làm đậm các node cha
            treeCateNews.CollapseAll();
            UtilsBase.BoldParentNodes(treeCateNews);

            // Thêm TreeView vào panel
            pnTreeCate24.Controls.Add(treeCateNews);
        }
        public List<Category> GetAllCatNews2024()
        {
            using (var context = new CTTDTCDK_NewsServicesEntities())
            {
                // Get all items from the table
                var allItems = context.Categories.ToList();
                return allItems;
            }
        }
        void AddChildNodes(TreeNode parentNode, string parentId)
        {
            var ltsAllCate2024 = GetAllCatNews2024().OrderBy(o => o.Id).ToList();
            // Tìm các node con có CatParentId = parentId
            var childCategories = ltsAllCate2024.Where(c => c.ParentID == parentId).ToList();

            foreach (var childCategory in childCategories)
            {
                // Tạo node con
                TreeNode childNode = new TreeNode(childCategory.Title)
                {
                    Tag = Convert.ToInt32(childCategory.CategoryOldId)  // Gán ID vào Tag
                };

                // Gọi đệ quy để thêm các node con của childNode
                AddChildNodes(childNode, childCategory.Id);

                // Thêm node con vào node cha
                parentNode.Nodes.Add(childNode);
            }
        }

        void AddChildNodesKH(TreeNode parentNode, int? oldId)
        {
            var ltsAllCate2024 = DMKyHop2024DA.GetAllCatKyHop2024().OrderBy(o => o.OldId).ToList();
            // Tìm các node con có CatParentId = parentId
            var childCategories = ltsAllCate2024.Where(c => c.ParentOldId == oldId).ToList();

            foreach (var childCategory in childCategories)
            {
                // Tạo node con
                TreeNode childNode = new TreeNode(childCategory.Title)
                {
                    Tag = Convert.ToInt32(childCategory.OldId)  // Gán ID vào Tag
                };

                // Gọi đệ quy để thêm các node con của childNode
                AddChildNodesKH(childNode, childCategory.OldId);

                // Thêm node con vào node cha
                parentNode.Nodes.Add(childNode);
            }
        }

        private void btnMapCat_Click(object sender, EventArgs e)
        {
            UpdateRecordsInBatches();
        }


        public void UpdateRecordsInBatches(int batchSize = 1000)
        {
            using (QuocHoiVNEntities db = new QuocHoiVNEntities())
            {
                int totalRecords = db.News.Count();
                int totalBatches = (int)Math.Ceiling((double)totalRecords / batchSize);

                for (int batch = 0; batch < totalBatches; batch++)
                {
                    var recordsToUpdate = db.News.Where(p => p.Category != null && p.Category != "").OrderBy(r => r.ID)
                                                     .Skip(batch * batchSize)
                                                     .Take(batchSize)
                                                     .ToList();

                    foreach (var record in recordsToUpdate)
                    {
                        if (!string.IsNullOrEmpty(record.Category))
                        {
                            // Thực hiện logic cập nhật cần thiết
                            record.Category = CutString(record.Category);
                            record.IdLoaiTin = CutString(record.IdLoaiTin);
                            if (!string.IsNullOrEmpty(record.Description))
                            {
                                record.Description = record.Description.Trim();// Chỉnh sửa field phù hợp
                            }

                        }
                    }
                    try
                    {
                        // Lưu batch hiện tại vào database
                        db.SaveChanges();
                    }
                    catch (DbEntityValidationException e)
                    {
                        foreach (var eve in e.EntityValidationErrors)
                        {
                            Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                                eve.Entry.Entity.GetType().Name, eve.Entry.State);
                            foreach (var ve in eve.ValidationErrors)
                            {
                                Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                                    ve.PropertyName, ve.ErrorMessage);
                            }
                        }
                        throw;
                    }
                }
            }
        }
        public static void UpdateCategoryNews()
        {
            using (QuocHoiVNEntities db = new QuocHoiVNEntities())
            {
                // Sử dụng AsNoTracking để tăng hiệu suất khi không cần theo dõi thay đổi
                var categoryToUpdate = db.News.ToList();

                foreach (var newsItem in categoryToUpdate)
                {
                    // Cập nhật thông tin danh mục

                    string updatedCategory = CutString(newsItem.Category);
                    newsItem.Category = updatedCategory;
                    //  db.SaveChanges();
                }

                try
                {
                    // Lưu tất cả thay đổi vào cơ sở dữ liệu chỉ một lần
                    db.SaveChanges();
                    MessageBox.Show("Tất cả danh mục đã được cập nhật và lưu thành công.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Có lỗi xảy ra khi lưu vào cơ sở dữ liệu: {ex.Message}");
                }

                MessageBox.Show("Hoàn thành rồi");
            }
        }
        public static void UpdateCategoryNews111()
        {
            using (QuocHoiVNEntities db = new QuocHoiVNEntities())
            {
                // Sử dụng AsNoTracking để tăng hiệu suất
                var categoryToUpdate = db.News.AsNoTracking().ToList();

                bool hasChanges = false;

                foreach (var newsItem in categoryToUpdate)
                {
                    // Cắt chuỗi Category
                    string updatedCategory = CutString(newsItem.Category);

                    // Chỉ cập nhật khi có thay đổi
                    if (newsItem.Category != updatedCategory)
                    {
                        newsItem.Category = updatedCategory;
                        db.Entry(newsItem).State = EntityState.Modified;
                        hasChanges = true;
                    }
                }

                if (hasChanges)
                {
                    try
                    {
                        // Lưu tất cả thay đổi vào cơ sở dữ liệu chỉ một lần
                        db.SaveChanges();
                        MessageBox.Show("Tất cả danh mục đã được cập nhật và lưu thành công.");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Có lỗi xảy ra khi lưu vào cơ sở dữ liệu: {ex.Message}");
                    }
                }
                else
                {
                    MessageBox.Show("Không có danh mục nào cần cập nhật.");
                }

                MessageBox.Show("Hoàn thành rồi");
            }
        }
        public static string CutString(string input)
        {
            // Kiểm tra chuỗi đầu vào có rỗng không
            if (string.IsNullOrEmpty(input))
            {
                return string.Empty;
            }
            if (input.Contains(";#"))
            {
                // Tách chuỗi theo ký tự ";#"
                var parts = input.Split(new string[] { ";#" }, StringSplitOptions.None);
                // Trường hợp chỉ có một phần tử (không có ";#")
                if (parts.Length == 2)
                {
                    return parts[0]; // Trả về giá trị duy nhất
                }

                // Trường hợp có nhiều phần tử, trả về phần tử đầu tiên và phần tử gần cuối
                if (parts.Length > 1)
                {
                    string firstElement = parts[0]; // Phần tử đầu tiên
                    string lastElement = parts[parts.Length - 2]; // Phần tử gần cuối

                    return $"{firstElement},{lastElement}";
                }
            }
            else
            {
                return input;
            }
            // Nếu không khớp bất kỳ điều kiện nào (trường hợp hiếm), trả về chuỗi rỗng
            return string.Empty;
        }

        private void btnConvertKyHop_Click(object sender, EventArgs e)
        {
            var idcatmap = Convert.ToString(GetIDsDMMapTD());
            LtsAllCate = DMKyHop2024DA.GetAllCatKyHop2024().OrderBy(o => o.OldId).ToList();
            var idcate24 = DMKyHop2024DA.GetGuid(Convert.ToInt32(idcatmap));
            // Kiểm tra nếu danh sách rỗng
            if (idcatmap != null && idcate24 != null)
            {
                // Sử dụng Join để tìm kiếm hiệu quả hơn

                BuildFlatCategoryList(LtsAllCate, Convert.ToInt32(idcatmap), idcate24, lblCount, txtLog, lblthuchien);

            }
            else
            {
                Console.WriteLine("Danh sách categoryIds rỗng.");
            }


        }
        public static async void BuildFlatCategoryList(List<DMKyHop> allCategories, int? idold, Guid idDMKH, Label lblCount, RichTextBox txtLog, Label lblthuchien)
        {
            // Lấy tất cả các danh mục con của parent hiện tại
            var subCategories = allCategories.Where(c => c.ParentOldId == idold).OrderBy(c => c.OldId).ToList();

            foreach (var subCategory in subCategories)
            {
                var idcate24 = DMKyHop2024DA.GetGuid(Convert.ToInt32(subCategory.OldId));
                await DMKyHop2024DA.InsertNDKyHop(subCategory.OldId.ToString(), idcate24, lblCount, txtLog, lblthuchien);
                // Sử dụng Join để tìm kiếm hiệu quả hơn
                // await NewsDA.InsertNew2024_3(idcatmap, idcate24, lblCount, txtLog, lblthuchien);
                //    MessageBox.Show("met roi di ngu thoi");

                BuildFlatCategoryList(allCategories, subCategory.OldId, idcate24, lblCount, txtLog, lblthuchien);

            }
        }

        private void btnViewCateKH_Click(object sender, EventArgs e)
        {
            treeCateNews.Nodes.Clear();
            viewCatTreeQH2024();
        }
        List<QHBASE.DMKyHop> LtsAllCatKH2024 = new List<DMKyHop>();

        public void viewCatTreeQH2024()
        {
            treeCateNews.Width = 600;
            treeCateNews.Dock = DockStyle.Left;
            // Lấy tất cả dữ liệu Category một lần duy nhất
            LtsAllCatKH2024 = DMKyHop2024DA.GetAllCatKyHop2024().OrderBy(o => o.OldId).ToList();

            // Lấy các node gốc (CatParentId = 0)
            var rootCategories = LtsAllCatKH2024.Where(c => c.ParentId == null).OrderBy(o => o.OldId).ToList();

            // Duyệt qua danh sách root để thêm vào TreeView
            foreach (var rootCategory in rootCategories)
            {
                // Tạo node gốc
                TreeNode rootNode = new TreeNode(rootCategory.Title + "--Old ID:--" + rootCategory.OldId)
                {
                    Tag = rootCategory.OldId  // Gán ID vào Tag
                };

                // Gọi hàm đệ quy để thêm các node con
                if (rootCategory.ParentId == null)
                {
                    AddChildNodesKH(rootNode, rootCategory.OldId);
                }

                // Thêm node gốc vào TreeView
                treeCateNews.Nodes.Add(rootNode);
            }

            // Mở rộng tất cả các node và làm đậm các node cha
            treeCateNews.CollapseAll();
            UtilsBase.BoldParentNodes(treeCateNews);

            // Thêm TreeView vào panel
            pnTreeCate24.Controls.Add(treeCateNews);
        }

        private void btnLichLamViec_Click(object sender, EventArgs e)
        {
            var idcatmap = Convert.ToString(GetIDsDMMapTD());
            LtsAllCate = DMKyHop2024DA.GetAllCatKyHop2024().OrderBy(o => o.OldId).ToList();
            var idcate24 = DMKyHop2024DA.GetGuid(Convert.ToInt32(idcatmap));
            // Kiểm tra nếu danh sách rỗng
            if (idcatmap != null && idcate24 != null)
            {
                // Sử dụng Join để tìm kiếm hiệu quả hơn

                BuildtreeLich(LtsAllCate, Convert.ToInt32(idcatmap), idcate24, lblCount, txtLog, lblthuchien);

            }
            else
            {
                Console.WriteLine("Danh sách categoryIds rỗng.");
            }
        }
        public static async void BuildtreeLich(List<DMKyHop> allCategories, int? idold, Guid idDMKH, Label lblCount, RichTextBox txtLog, Label lblthuchien)
        {
            // Lấy tất cả các danh mục con của parent hiện tại
            var subCategories = allCategories.Where(c => c.ParentOldId == idold).OrderBy(c => c.OldId).ToList();

            foreach (var subCategory in subCategories)
            {
                var idcate24 = DMKyHop2024DA.GetGuid(Convert.ToInt32(subCategory.ParentOldId));
                if (subCategory.Title.Contains("Chương trình làm việc"))
                {

                    await DMKyHop2024DA.InsertCTLVKyHop(subCategory.OldId.ToString(), idcate24, lblCount, txtLog, lblthuchien);
                    // Sử dụng Join để tìm kiếm hiệu quả hơn
                    // await NewsDA.InsertNew2024_3(idcatmap, idcate24, lblCount, txtLog, lblthuchien);
                    //    MessageBox.Show("met roi di ngu thoi");

                }

                BuildtreeLich(allCategories, subCategory.OldId, idcate24, lblCount, txtLog, lblthuchien);
            }
        }

        private void rjButton1_Click(object sender, EventArgs e)
        {

            using (var db = new QHBASE.QuocHoiVNEntities())
            {
                try
                {
                    //=> string siteurl = "https://noidung.quochoi.vn/UBTVQH/content/tintuc/Lists/News";

                    string siteurl = "https://www.vr.org.vn/contents/News/Lists/News";

                    string stSite = siteurl.Substring(0, siteurl.LastIndexOf('/'))
                                                    .ToLower()
                                                    .Replace("/lists", string.Empty)
                                                    .Replace("/tt", "");

                    var listSV = UtilsBase.GetListSV(stSite, txtUsername.Text, txtPassword.Text);
                    var ndListView = listSV.GetListAndView("Danh sách Bài viết", "");
                    string strListID = ndListView.ChildNodes[0].Attributes["Name"].Value;

                    // Tạo XML truy vấn với tham số hoá
                    var xmlDoc = new System.Xml.XmlDocument();
                    var ndQuery = xmlDoc.CreateElement("Query");
                    var ndViewFields = xmlDoc.CreateElement("ViewFields");
                    var ndQueryOptions = xmlDoc.CreateElement("QueryOptions");

                    ndViewFields.InnerXml = CreateViewFieldXML2();
                    int startID = Convert.ToInt32(txtIdStart.Text);
                    int endID = Convert.ToInt32(txtIdEnd.Text);
                    string soluong = Convert.ToString(txtSoluong.Text);
                    ndQuery.InnerXml = BuildQueryXML(startID, endID);
                    ndQueryOptions.InnerXml = "<IncludeMandatoryColumns>False</IncludeMandatoryColumns><ViewAttributes Scope=\"RecursiveAll\"/><DateInUtc>TRUE</DateInUtc>";

                    // Thực hiện truy vấn và xử lý kết quả
                    var ndListItems = listSV.GetListItems("Danh sách Bài viết", "", ndQuery, ndViewFields, soluong, ndQueryOptions, string.Empty);
                    // MessageBox.Show(UtilsBase.ConverToString(listSV.GetListItems(ListNewsName, "", ndQuery, ndViewFields, soluong, ndQueryOptions, string.Empty), 100));

                    var TableFull = UtilsBase.ConvertXmlToDataTable(ndListItems);
                    //var existingIds = new HashSet<int?>(db.News.Select(n => n.OldID).ToList()); // Lưu trữ OldID đã tồn tại

                    // processBar.Maximum = TableFull.Rows.Count;

                    InsertDataIntoDatabase(TableFull, 2);
                    MessageBox.Show(" tham mowis thanfh cong");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"*** ERR: {ex.Message}\r\n");
                }
            }
        }

        #region customs
        public string CreateViewFieldXML2()
        {
            // Sử dụng StringBuilder để tạo chuỗi XML
            StringBuilder stbBuilder = new StringBuilder();

            // Thêm các FieldRef vào StringBuilder
            // ID bai viet
            stbBuilder.AppendLine("<FieldRef Name=\"ID\" />");
            // Tiêu đề bài viết
            stbBuilder.AppendLine("<FieldRef Name=\"Title\" />");
            // ảnh đại diện
            stbBuilder.AppendLine("<FieldRef Name=\"Image\" />");
            //Hiển thị mô tả
            //stbBuilder.AppendLine("<FieldRef Name=\"AutoDescription\" />");
            // Cho pheo gui bình luận
            stbBuilder.AppendLine("<FieldRef Name=\"AllowComment\" />");
            // hiển thị mô tả
            //stbBuilder.AppendLine("<FieldRef Name=\"ShowDescription\" />");
            //Chuyên mục
            stbBuilder.AppendLine("<FieldRef Name=\"Categories\" LookupId=\"True\" />");
            //Ngày tạo
            stbBuilder.AppendLine("<FieldRef Name=\"DisplayDate\" />");
            //Loại Tin
            //stbBuilder.AppendLine("<FieldRef Name=\"IdLoaiTin\" LookupId=\"True\"/>");
            //stbBuilder.AppendLine("<FieldRef Name=\"ShowImage\" />");
            stbBuilder.AppendLine("<FieldRef Name=\"Descriptions\" />");
            stbBuilder.AppendLine("<FieldRef Name=\"Contents\" />");
            stbBuilder.AppendLine("<FieldRef Name=\"ReadCount\" />");
            stbBuilder.AppendLine("<FieldRef Name=\"_Author\" />");
            stbBuilder.AppendLine("<FieldRef Name=\"SourceNews\" />");
            stbBuilder.AppendLine("<FieldRef Name=\"Created\" />");
            stbBuilder.AppendLine("<FieldRef Name=\"Author\" />");

            //stbBuilder.AppendLine("<FieldRef Name=\"TrangThaiTin\" />");
            //stbBuilder.AppendLine("<FieldRef Name=\"Keywords\" />");
            stbBuilder.AppendLine("<FieldRef Name=\"Event\" LookupId=\"True\" />");
            stbBuilder.AppendLine("<FieldRef Name=\"IsHotNews\" />");
            //stbBuilder.AppendLine("<FieldRef Name=\"IsLive\" />");
            //stbBuilder.AppendLine("<FieldRef Name=\"IsShare\" />");
            //stbBuilder.AppendLine("<FieldRef Name=\"IsShowView\" />");
            stbBuilder.AppendLine("<FieldRef Name=\"_ModerationStatus\" />");
            stbBuilder.AppendLine("<FieldRef Name=\"Attachments\" />");
            // Trả về chuỗi XML đã xây dựng
            return stbBuilder.ToString();
        }

        public string BuildQueryXML2(int startID, int endID,int categoryId)
        {
            StringBuilder queryXml = new StringBuilder(); queryXml.AppendLine("<Where>");
            queryXml.AppendLine("  <And>");
            queryXml.AppendLine("    <Eq>");
            queryXml.AppendLine("      <FieldRef Name=\"Categories\" LookupId=\"True\" />");
            queryXml.AppendFormat("      <Value Type=\"Lookup\">{0}</Value>", categoryId);  //=> query theo categoryId
            queryXml.AppendLine("    </Eq>");
            queryXml.AppendLine("  <And>");
            queryXml.AppendLine("    <Eq>");
            queryXml.AppendLine("      <FieldRef Name=\"_ModerationStatus\" />");
            queryXml.AppendLine("      <Value Type=\"ModStat\">0</Value>");  // Sửa lại giá trị là Approved thay vì 0
            queryXml.AppendLine("    </Eq>");
            queryXml.AppendLine("    <And>");
            queryXml.AppendLine("      <Geq>");
            queryXml.AppendLine("        <FieldRef Name=\"ID\" />");
            queryXml.AppendLine($"        <Value Type=\"Counter\">{startID}</Value>");
            queryXml.AppendLine("      </Geq>");
            queryXml.AppendLine("      <Lt>");
            queryXml.AppendLine("        <FieldRef Name=\"ID\" />");
            queryXml.AppendLine($"        <Value Type=\"Counter\">{endID}</Value>");
            queryXml.AppendLine("      </Lt>");
            queryXml.AppendLine("    </And>");
            queryXml.AppendLine("  </And>");
            queryXml.AppendLine("  </And>");
            queryXml.AppendLine("</Where>");

            queryXml.AppendLine("<OrderBy>");
            queryXml.AppendLine("  <FieldRef Name=\"ID\" Ascending=\"True\" />");
            queryXml.AppendLine("</OrderBy>");

            return queryXml.ToString();
        }

        public string BuildQueryXML2Fix(int startID, int endID, int categoryId)
        {
            StringBuilder queryXml = new StringBuilder(); queryXml.AppendLine("<Where>");
            queryXml.AppendLine("  <And>");
            queryXml.AppendLine("    <Eq>");
            queryXml.AppendLine("      <FieldRef Name=\"Categories\" LookupId=\"True\" />");
            queryXml.AppendFormat("      <Value Type=\"Lookup\">{0}</Value>", categoryId);  //=> query theo categoryId
            queryXml.AppendLine("    </Eq>");
            queryXml.AppendLine("  <And>");
            queryXml.AppendLine("    <Eq>");
            queryXml.AppendLine("      <FieldRef Name=\"_ModerationStatus\" />");
            queryXml.AppendLine("      <Value Type=\"ModStat\">0</Value>");  // Sửa lại giá trị là Approved thay vì 0
            queryXml.AppendLine("    </Eq>");
            queryXml.AppendLine("    <And>");
            queryXml.AppendLine("      <Neq>");
            queryXml.AppendLine("        <FieldRef Name=\"ID\" />");
            queryXml.AppendLine($"        <Value Type=\"Counter\">8457</Value>");
            queryXml.AppendLine("      </Neq>");
            queryXml.AppendLine("      <Neq>");
            queryXml.AppendLine("        <FieldRef Name=\"ID\" />");
            queryXml.AppendLine($"        <Value Type=\"Counter\">6571</Value>");
            queryXml.AppendLine("      </Neq>");
            queryXml.AppendLine("    </And>");
            queryXml.AppendLine("  </And>");
            queryXml.AppendLine("  </And>");
            queryXml.AppendLine("</Where>");

            queryXml.AppendLine("<OrderBy>");
            queryXml.AppendLine("  <FieldRef Name=\"ID\" Ascending=\"True\" />");
            queryXml.AppendLine("</OrderBy>");

            return queryXml.ToString();
        }
        #endregion

        private void btnviewPhien_Click(object sender, EventArgs e)
        {

        }

        private void rjLabel7_Click(object sender, EventArgs e)
        {

        }
    }
}
