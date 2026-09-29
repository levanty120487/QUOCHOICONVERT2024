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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using RJCodeUI_M1.SPService;
using System.Xml;
using System.Web.UI.WebControls;
using static System.Windows.Forms.LinkLabel;
using System.Data.Entity.Validation;
using System.Web.ModelBinding;
using QHBASEPHIENHOP;
using System.Data.Entity;

namespace RJCodeUI_M1.TestAndDemo
{
    public partial class FormConvertVanKienSP : RJForms.RJChildForm
    {

        private string ListNewsName = "DanhSachVanKien";
        List<VanKien> LtsAllCategory = new List<VanKien>();
        List<VanKien> LtsAllCategoryTemp = new List<VanKien>();
        public FormConvertVanKienSP()
        {
            InitializeComponent();
        }

        public FormConvertVanKienSP(Models.User user)
        {
            InitializeComponent();
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

                    // Tạo XML truy vấn với tham số hoá
                    var xmlDoc = new System.Xml.XmlDocument();
                    var ndQuery = xmlDoc.CreateElement("Query");
                    var ndViewFields = xmlDoc.CreateElement("ViewFields");
                    var ndQueryOptions = xmlDoc.CreateElement("QueryOptions");

                    ndViewFields.InnerXml = CreateViewFieldXML();
                    int startID = Convert.ToInt32(txtIdStart.Text);
                    int endID = Convert.ToInt32(txtIdEnd.Text);
                    string soluong = Convert.ToString(txtSoluong.Text);
                    ndQuery.InnerXml = BuildQueryXML(startID, endID);
                    ndQueryOptions.InnerXml = "<IncludeMandatoryColumns>False</IncludeMandatoryColumns><ViewAttributes Scope=\"RecursiveAll\"/><DateInUtc>TRUE</DateInUtc>";

                    // Thực hiện truy vấn và xử lý kết quả
                    var ndListItems = listSV.GetListItems(ListNewsName, "", ndQuery, ndViewFields, soluong, ndQueryOptions, string.Empty);

                    var TableFull = UtilsBase.ConvertXmlToDataTable(ndListItems);
                    // var existingIds = new HashSet<int?>(db.VanKiens.Select(n => n.VanKienOldId).ToList()); // Lưu trữ OldID đã tồn tại

                    InsertDataIntoDatabase(TableFull);
                    MessageBox.Show("đã xong");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"*** ERR: {ex.Message}\r\n");
                }
            }

        }
        public string BuildQueryXML(int startID, int endID)
        {
            // Sử dụng StringBuilder để xây dựng chuỗi XML
            StringBuilder queryXml = new StringBuilder();
            queryXml.AppendLine("<Where>");
            queryXml.AppendLine("  <And>");
            queryXml.AppendLine("    <Geq>");
            queryXml.AppendLine("      <FieldRef Name=\"ID\" />");
            queryXml.AppendLine($"      <Value Type=\"Counter\">{startID}</Value>");
            queryXml.AppendLine("    </Geq>");
            queryXml.AppendLine("    <Lt>");
            queryXml.AppendLine("      <FieldRef Name=\"ID\" />");
            queryXml.AppendLine($"      <Value Type=\"Counter\">{endID}</Value>");
            queryXml.AppendLine("    </Lt>");
            queryXml.AppendLine("  </And>");
            queryXml.AppendLine("</Where>");
            queryXml.AppendLine("<OrderBy>");
            queryXml.AppendLine("  <FieldRef Name=\"ID\" Ascending=\"True\" />");
            queryXml.AppendLine("</OrderBy>");

            return queryXml.ToString();
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
            //Chuyên mục
            stbBuilder.AppendLine("<FieldRef Name=\"DMCoQuan\" LookupId=\"True\" />");
            stbBuilder.AppendLine("<FieldRef Name=\"DMLinhVuc\" LookupId=\"True\" />");
            stbBuilder.AppendLine("<FieldRef Name=\"DMVanKien\" LookupId=\"True\" />");
            stbBuilder.AppendLine("<FieldRef Name=\"NoiDungHTML\" />");
            stbBuilder.AppendLine("<FieldRef Name=\"NgayTao\" />");
            stbBuilder.AppendLine("<FieldRef Name=\"_ModerationStatus\" />");
            stbBuilder.AppendLine("<FieldRef Name=\"Attachments\" />");
            // Trả về chuỗi XML đã xây dựng
            return stbBuilder.ToString();
        }

        public async Task GetAttachmentSp(string idVanKien, string OldIdVankien)
        {
            try
            {
                string stSite = txtSiteUrl.Text.Substring(0, txtSiteUrl.Text.LastIndexOf('/'))
                                                .ToLower()
                                                .Replace("/lists", string.Empty)
                                                .Replace("/tt", "");

                var listSV = UtilsBase.GetListSV(stSite, txtUsername.Text, txtPassword.Text);
                //var ndListView = listSV.GetListAndView(ListNewsName, "");
                XmlNode ndAttach = listSV.GetAttachmentCollection(ListNewsName, OldIdVankien);
                var TableFull = UtilsBase.ConvertXmlToDataTableAttachments(ndAttach);
                await InsertAttachment(TableFull, idVanKien, Convert.ToInt32(OldIdVankien));
                //  MessageBox.Show("đã xong");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"*** ERR: {ex.Message}\r\n");
            }
        }

        public static async Task InsertAttachment(DataTable entities, string iditem, int? oldId)
        {

            using (CTTDTCDK_FilesServicesEntities db = new CTTDTCDK_FilesServicesEntities())
            {
                var totalattach = entities.Rows.Count;
                string urlattch = string.Empty;
                foreach (DataRow item in entities.Rows)
                {
                    try
                    {
                        CMSFile itemFile = new CMSFile();
                        itemFile.Id = Guid.NewGuid();
                        itemFile.FileType = 2;
                        itemFile.CreationTime = DateTime.Now;
                        itemFile.FileContainerName = "CMSContainerPublic";
                        itemFile.OwnerUserId = UtilsBase.GuidFromString("F740E5FE-5086-0314-4926-3A14B5173E2F");
                        itemFile.ConcurrencyStamp = itemFile.Id.ToString();
                        itemFile.CreatorId = UtilsBase.GuidFromString("F740E5FE-5086-0314-4926-3A14B5173E2F");
                        itemFile.ParentId = UtilsBase.GuidFromString("4DEDA0AD-F00A-DDF7-64B9-3A15548B05AB");
                        itemFile.ExtraProperties = "{}";
                        itemFile.MimeType = "image/jpeg";
                        if (totalattach > 1)
                        {
                            urlattch = Convert.ToString(item["Attachment_Text"]);
                        }
                        else
                        {
                            urlattch = Convert.ToString(item["Attachment"]);
                        }

                        #region Download file
                        if (urlattch != null)
                        {
                            List<string> LtsFileDownload = new List<string>() {
                                ".doc",
                                ".docx",
                                ".pdf",
                                ".xls",
                                ".xlsx",
                                ".zip",
                                ".ppt",
                                ".pptx",
                                ".rar",
                                ".bmp"
                            };
                            List<string> LtsFileImage = new List<string>() {
                                "image/jpeg",
                                "image/png",
                                "image/webp",
                                "image/gif",
                                "image/svg+xml"
                            };

                            List<string> ListDocment = new List<string>()
                        {
                            "application/xml","application / pdf",
                            "application/zip",
                            "application/x-www-form-urlencoded",
                            "application/vnd.ms-excel",
                            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                            "application/msword",
                            "application/vnd.openxmlformats-officedocument.wordprocessingml.document"
                        };
                            var urlfile = urlattch;
                            // var fileType = Convert.ToString(item["ows_Title"]).Substring(urlfile.LastIndexOf("."));

                            var fileName = urlattch.Substring(urlfile.LastIndexOf("/"));
                            var fileType = urlattch.Substring(urlfile.LastIndexOf("."));
                            //var mimetype = await UtilsBase.GetMimeTypeFromUrl(urlfile);
                            if (LtsFileDownload.Contains(fileType))
                            {
                                itemFile.FullPathServer = await ImageDownloader.DownloadDataFile(urlfile);
                                itemFile.FileExtention = 3;
                                itemFile.FileName = fileName;
                            }
                        }
                        #endregion
                        itemFile.OldId = oldId.ToString();
                        db.CMSFiles.Add(itemFile);
                        db.SaveChanges();

                        await InsertFileCMS(iditem, itemFile.Id, oldId);
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
                        throw e;
                    }

                }
            }
        }
        static void InsertDataIntoDatabase(DataTable entities)
        {
            try
            {
                string connectionString = "data source=172.16.1.181;initial catalog=QuocHoiVN;persist security info=True;user id=sa;password=Server@20!(;MultipleActiveResultSets=True;App=EntityFramework&quot"; // Thay đổi chuỗi kết nối

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection))
                    {
                        bulkCopy.DestinationTableName = "VanKien"; // Tên bảng trong DB

                        // Định nghĩa mapping cột nếu cần
                        //bulkCopy.ColumnMappings.Add("OldID", "OldID");

                        // Thêm các FieldRef vào StringBuilder
                        // ID bai viet
                        bulkCopy.ColumnMappings.Add("ows_ID", "VanKienOldId");
                        // Tiêu đề bài viết
                        bulkCopy.ColumnMappings.Add("ows_Title", "Title");
                        // hiển thị mô tả
                        bulkCopy.ColumnMappings.Add("ows_DMCoQuan", "DMCoQuan");
                        bulkCopy.ColumnMappings.Add("ows_DMLinhVuc", "DMLinhVuc");
                        bulkCopy.ColumnMappings.Add("ows_DMVanKien", "DMVanKien");
                        //bulkCopy.ColumnMappings.Add("ows_NoiDungHTML", "NoiDungHTML");
                        bulkCopy.ColumnMappings.Add("ows_NgayTao", "NgayTao");
                        bulkCopy.ColumnMappings.Add("ows_Attachments", "Attachments");
                        bulkCopy.ColumnMappings.Add("ows__ModerationStatus", "ModerationStatus");
                        bulkCopy.WriteToServer(entities);

                    }
                }
            }
            catch (Exception e)
            {

                throw;
            }

        }
        private void txtLog_TextChanged(object sender, EventArgs e)
        {

        }

        private async void bntConvert_Click(object sender, EventArgs e)
        {
            // Kỳ họp
            using (var context = new QuocHoiVNEntities())
            {
                var i = 1;
                txtLog.Text = $"Bắt đầu đồng bộ: {DateTime.Now}\n";
                Application.DoEvents();
                bool checkFile = false;
                var vanKiens = await context.VanKiens.Where(x => x.ModerationStatus == 0).OrderBy(x => x.VanKienOldId).ToListAsync();

                if (vanKiens != null && vanKiens.Count > 0)
                {
                    var logMessages = new List<string>();
                    var vanKienItemsToInsert = new List<QHBASE.VanKienTaiLieu>();
                    var coQuanMapItemsToInsert = new List<QHBASE.VanKienMapCQBH>();

                    // check trùng phiên và kỳ
                    var listId = vanKiens.Select(x => x.VanKienOldId).ToList();
                    using (var contextCheckPhienHop = new Preview_QuocHoi_PhienHopServiceEntities())
                    using (var contextCheckKyHop = new Preview_QuocHoi_KyHopServiceEntities())
                    {
                        var messages = new List<string>();

                        var allOldIdsInPhienHop = await contextCheckPhienHop.VanKienTaiLieus
                            .Where(x => listId.Contains(x.OldId))
                            .Select(x => x.OldId)
                            .ToListAsync();

                        var allOldIdsInKyHop = await contextCheckKyHop.VanKienTaiLieus
                            .Where(x => listId.Contains(x.OldId))
                            .Select(x => x.OldId)
                            .ToListAsync();

                        // Kết hợp OldId vào một HashSet để kiểm tra nhanh hơn
                        var allOldIdsToRemove = new HashSet<int?>(allOldIdsInPhienHop);
                        allOldIdsToRemove.UnionWith(allOldIdsInKyHop);

                        vanKiens = vanKiens.Where(vk => !allOldIdsToRemove.Contains(vk.VanKienOldId)).ToList();

                        // Đếm số lượng cho cả hai bối cảnh
                        var countPhienHop = allOldIdsInPhienHop.Count;
                        var countKyHop = allOldIdsInKyHop.Count;

                        if (countPhienHop > 0)
                        {
                            messages.Add($"- Văn kiện {string.Join(", ", allOldIdsInPhienHop)} đã được đồng bộ ở phiên họp.\n");
                        }
                        if (countKyHop > 0)
                        {
                            messages.Add($"- Văn kiện {string.Join(", ", allOldIdsInKyHop)} đã được đồng bộ\n");
                        }
                        if (messages.Any())
                        {
                            txtLog.Text += string.Join("", messages);
                            Application.DoEvents();
                        }
                    }
                    try
                    {
                        foreach (var item in vanKiens)
                        {
                            txtLog.Text += $"- Văn kiện {item.VanKienOldId} - {item.Title} bắt đầu được đồng bộ. {i}\n";
                            Application.DoEvents();
                            var vanKienItem = new QHBASE.VanKienTaiLieu
                            {
                                Id = Guid.NewGuid(),
                                Title = item.Title,
                                OldId = item.VanKienOldId,
                                ConcurrencyStamp = Guid.NewGuid().ToString(),
                                CreatorName = "admin",
                                PageUrlSEO = UtilsBase.ConvertToUrlString(item.Title),
                                DescriptionSEO = item.Title,
                                CreatorId = new Guid("C1B327B8-10CB-4668-A6D1-680CDFE65720"),
                                CreationTime = DateTime.Now,
                                ExtraProperties = "{}",
                                DMKyHopId = Guid.Empty,
                                Status = 3,
                                PublicDate = item.NgayTao,
                                AttachmentId = item.Attachments,
                                ShareSocial = true,
                                IsDeleted = false,
                                VanKienMapCQBHs = new List<QHBASE.VanKienMapCQBH>(),
                                Content = null,
                                LinhVucId = await GetGuidLinhVuc(item.DMLinhVuc)
                            };

                            await GetKyHopIdLoaiVanKienId(item.DMVanKien, vanKienItem);

                            if (vanKienItem.DMKyHopId != Guid.Empty)
                            {
                                i++;
                                vanKienItemsToInsert.Add(vanKienItem);
                                coQuanMapItemsToInsert.AddRange(await GetCoQuanBanHanhMappingsKyHop(item.DMCoQuan, vanKienItem.Id));
                            }
                            else
                            {
                                txtLog.Text += $"- Văn kiện {item.VanKienOldId} - {item.Title} không tìm thấy danh mục trong văn kiện.\n";
                                Application.DoEvents();
                            }
                        }

                        if (vanKienItemsToInsert.Any())
                        {
                            using (var insertContext = new Preview_QuocHoi_KyHopServiceEntities())
                            {
                                insertContext.VanKienTaiLieus.AddRange(vanKienItemsToInsert);
                                await insertContext.SaveChangesAsync();

                                if (coQuanMapItemsToInsert.Any())
                                {
                                    InsertVanKienTLKyHopMap(coQuanMapItemsToInsert);
                                }
                                txtLog.Text += $"- Đồng bộ Văn kiện chưa có file thành công. {DateTime.Now}\n";
                                Application.DoEvents();
                            }

                            // insertFile
                            foreach (var item in vanKienItemsToInsert)
                            {
                                if (item.AttachmentId == 1)
                                {
                                    //await GetAttachmentSp(item.Id, item.OldId.ToString());

                                }
                            }

                            //var tasksFile = vanKienItemsToInsert.Select(item => ProcessItemKyHop(item));
                            //await Task.WhenAll(tasksFile);

                            //await GetListAttachmentKyHopSp(vanKienItemsToInsert);
                            txtLog.Text += $"- Đồng bộ File thành công. {DateTime.Now}\n";
                            Application.DoEvents();
                        }

                    }
                    catch (DbEntityValidationException ex)
                    {
                        foreach (var eve in ex.EntityValidationErrors)
                        {
                            Console.WriteLine($"Entity of type \"{eve.Entry.Entity.GetType().Name}\" in state \"{eve.Entry.State}\" has the following validation errors:");
                            foreach (var ve in eve.ValidationErrors)
                            {
                                Console.WriteLine($"- Property: \"{ve.PropertyName}\", Error: \"{ve.ErrorMessage}\"");
                            }
                        }
                        MessageBox.Show(ex.Message);
                    }

                    txtLog.Text += $"- Đồng bộ thành công. {DateTime.Now}\n";
                    Application.DoEvents();
                    MessageBox.Show("Đồng bộ thành công.");

                }
                else
                {
                    txtLog.Text += "- Danh sách Văn kiện rỗng.\n";
                    Application.DoEvents();
                }
            }
        }
        public static async Task InsertFileCMS(string IdVanKienCMS, Guid idFileID, int? Oldid)
        {

            using (CTTDTCDK_FilesServicesEntities db = new CTTDTCDK_FilesServicesEntities())
            {
                try
                {
                    if (!string.IsNullOrWhiteSpace(IdVanKienCMS) && idFileID != Guid.Empty)
                    {
                        CMSFileAttachment itemCMS = new CMSFileAttachment();
                        itemCMS.Id = Guid.NewGuid();
                        itemCMS.FileId = idFileID;
                        itemCMS.EntityId = IdVanKienCMS;
                        itemCMS.ExtraProperties = "{}";
                        itemCMS.CreationTime = DateTime.Now;
                        itemCMS.CreatorId = UtilsBase.GuidFromString("F740E5FE-5086-0314-4926-3A14B5173E2F");
                        itemCMS.ConcurrencyStamp = itemCMS.Id.ToString();
                        //itemCMS.AttachmentId = Oldid;
                        itemCMS.FileAttachmentType = 9;
                        db.CMSFileAttachments.Add(itemCMS);
                        db.SaveChanges();
                    }
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
                    throw e;
                }
            }

        }


        #region get Guid Lĩnh vực của CMS Quốc hội
        private async Task<Guid?> GetGuidLinhVuc(string linhvuc)
        {
            Guid? result = null;
            // get Id lĩnh vực
            if (string.IsNullOrEmpty(linhvuc))
            {
                return null;
            }
            var linhvucId = UtilsBase.getLookup(linhvuc);
            using (var context = new Preview_QuocHoi_CommonsServiceEntities())
            {
                // Trả về trực tiếp giá trị từ truy vấn
                //result = await context.LinhVucs
                //    .Where(x => x.OldID == linhvucId)
                //    .Select(x => (Guid?)x.Id) // Ép kiểu để đảm bảo trả về Guid?
                //    .FirstOrDefaultAsync();
                //if (result != Guid.Empty)
                //{
                //    return result;
                //}
                return null;

            }
        }
        #endregion


        #region Get Guid loại văn kiện và kỳ họp của CMS Quốc hội
        //private void GetKyHopIdLoaiVanKienId(string loaiVanKien, VanKienTaiLieu vanKien)
        //{
        //    if (!string.IsNullOrEmpty(loaiVanKien))
        //    {
        //        // tìm tên văn kiện và parentId để tìm ra Kỳ họp.
        //        var loaiVanKienId = UtilsBase.getLookup(loaiVanKien);
        //        var tenLoaiVanKien = string.Empty;
        //        Guid? loaiVanKienGuid = null;
        //        using (var context = new QuocHoiVNEntities())
        //        {
        //            var allItems = context.VanKienCats.ToList();
        //            var item = context.VanKienCats.Where(x => x.CatOldID == loaiVanKienId).FirstOrDefault();
        //            if (item != null)
        //            {
        //                tenLoaiVanKien = item.CatName;
        //                // tìm Guid loại văn kiện của CMS Quốc hội theo Tên văn kiện vừa tìm được.
        //                loaiVanKienGuid = this.GetGuidLoaiVanKien(tenLoaiVanKien);

        //                // add loại văn kiện Guid vào CMS Quốc hội.
        //                vanKien.LoaiVanKienId = loaiVanKienGuid;

        //                // add DmKyHopId
        //                // lấy ra tên loại văn kiện của parentID => để so sánh với Tên Kỳ Họp của CMS Quốc Hội
        //                if (item.CatParentId != null && item.CatParentId > 0)
        //                {
        //                    var kyHopItem = allItems.Where(x => x.CatOldID == item.CatParentId).FirstOrDefault();
        //                    if (kyHopItem.CatParentId != null && kyHopItem.CatParentId > 0)
        //                    {
        //                        var khoaItem = allItems.Where(x => x.CatOldID == kyHopItem.CatParentId).FirstOrDefault();
        //                        //
        //                        var kyHopGuid = this.GetGuidKyHop(kyHopItem, khoaItem);
        //                        if (kyHopGuid != null)
        //                        {
        //                            vanKien.DMKyHopId = kyHopGuid.Value;
        //                            //using (var contextKH = new Preview_QuocHoi_KyHopServiceEntities())
        //                            //{
        //                            //    vanKien.DMKyHop = contextKH.DMKyHops.Where(x=>x.Id == vanKien.DMKyHopId).FirstOrDefault();
        //                            //}
        //                        }
        //                        else
        //                        {
        //                            txtLog.Text = "Không có kỳ họp của văn kiện.";
        //                        }
        //                    }
        //                    else
        //                    {
        //                        txtLog.Text = "Không có Khóa họp của văn kiện.";
        //                    }
        //                }
        //                else
        //                {
        //                    txtLog.Text = "Không có Kỳ họp của văn kiện.";
        //                }

        //            }

        //        }
        //    }
        //    // return vanKien;
        //}

        public async Task InsertVanKienTLMap(QHBASE.VanKienMapCQBH item)
        {
            using (Preview_QuocHoi_KyHopServiceEntities db = new Preview_QuocHoi_KyHopServiceEntities())
            {
                // Tìm danh mục theo OldId
                var kyhop = db.VanKienMapCQBHs.Add(item);
                // Lưu thay đổi vào cơ sở dữ liệu
                db.SaveChanges();
            }

        }

        private string CatChuoiDenKyTu(string text, string congChuoi)
        {
            string tempKyHop = string.Empty;
            int index = text.IndexOf("thứ");

            if (index != -1) // Kiểm tra nếu "thứ" có trong chuỗi
            {
                tempKyHop = $"{text.Substring(0, index)}thứ {congChuoi}";
            }

            return tempKyHop;
        }
        private string CatChuoiOfThuongVu(string text)
        {
            return text.Substring(text.IndexOf('-') + 2).Trim();

        }

        private async Task<Guid?> GetGuidLoaiVanKien(string loaiVanKien)
        {
            if (string.IsNullOrEmpty(loaiVanKien))
                return null;

            var normalizedLoaiVanKien = loaiVanKien.ToLower();

            var mappings = new Dictionary<string, string>
                {
                    { "báo cáo giám sát", "Báo cáo giám sát" },
                    { "báo cáo công tác", "Báo cáo công tác" },
                    { "các luật,nghị quyết", "Các Luật, Nghị quyết trình Quốc hội thông qua tại kỳ họp" },
                    { "các luật, nghị quyết", "Các Luật, Nghị quyết trình Quốc hội thông qua tại kỳ họp" },
                    { "tài liệu chất vấn, kiến nghị của cử tri", "Tài liệu chất vấn, trả lời kiến nghị cử tri" },
                    { "các dự án luật pháp lệnh", "Các dự án luật, pháp lệnh" },
                };

            // Tìm từ khóa phù hợp
            var matchedMapping = mappings.Keys.FirstOrDefault(key => normalizedLoaiVanKien.Contains(key));

            // Nếu không tìm thấy, giữ nguyên loaiVanKien
            var tempText = matchedMapping != null ? mappings[matchedMapping] : normalizedLoaiVanKien;

            using (var context = new Preview_QuocHoi_CommonsServiceEntities())
            {
                // Sử dụng Equals với StringComparison để so sánh không phân biệt chữ hoa chữ thường
                var item = await context.LoaiVanKiens
                    .Where(x => x.Title.ToLower().Trim().Equals(tempText.ToLower().Trim()))
                    .Select(x => x.Id)
                    .FirstOrDefaultAsync();
                //if (item != Guid.Empty) return item;
                return null;

            }
        }

        #endregion

        private async Task<string> GetConTentVanKien(string NoiDungHTML)
        {
            string result = string.Empty;
            if (!string.IsNullOrEmpty(NoiDungHTML))
            {
                HtmlAgilityPack.HtmlDocument newsDocument = new HtmlAgilityPack.HtmlDocument();
                newsDocument.LoadHtml(NoiDungHTML);

                #region Download ảnh
                var ImagesNode = newsDocument.DocumentNode.SelectNodes(".//img");
                if (ImagesNode != null && ImagesNode.Count > 0)
                {
                    foreach (var img in ImagesNode)
                    {
                        img.Attributes["src"].Value = await ImageDownloader.DownloadImageAsyncurl(img.Attributes["src"].Value.Trim());
                    }
                    result = newsDocument.DocumentNode.OuterHtml;
                }
                #endregion
            }
            return result;
        }

        private void btnViewCateSQL_Click(object sender, EventArgs e)
        {
            viewCatTreeCate2024();
        }
        public void viewCatTreeCate2024()
        {
            treeCateNews.Width = 400;
            treeCateNews.Dock = DockStyle.Left;
            // Lấy tất cả dữ liệu Category một lần duy nhất
            //var ltsAllCate2024 = GetAllCatNews2024().OrderBy(o => o.Id).ToList();

            //// Lấy các node gốc (CatParentId = 0)
            //var rootCategories = ltsAllCate2024.Where(c => c.ParentID == null).ToList();

            //// Duyệt qua danh sách root để thêm vào TreeView
            //foreach (var rootCategory in rootCategories)
            //{
            //    // Tạo node gốc
            //    TreeNode rootNode = new TreeNode(rootCategory.Title)
            //    {
            //        Tag = rootCategory.CategoryOldId  // Gán ID vào Tag
            //    };

            //    // Gọi hàm đệ quy để thêm các node con
            //    if (rootCategory.ParentID == null)
            //    {
            //        AddChildNodes(rootNode, rootCategory.Id);
            //    }

            //    // Thêm node gốc vào TreeView
            //    treeCateNews.Nodes.Add(rootNode);
            //}

            // Mở rộng tất cả các node và làm đậm các node cha
            treeCateNews.CollapseAll();
            UtilsBase.BoldParentNodes(treeCateNews);

            // Thêm TreeView vào panel
            pnTreeCate24.Controls.Add(treeCateNews);
        }

        private void rjButton1_Click(object sender, EventArgs e)
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

                    XmlNode ndAttach = listSV.GetAttachmentCollection(ListNewsName, "552");

                    MessageBox.Show(ndAttach.OuterXml);

                    var TableFull = UtilsBase.ConvertXmlToDataTableAttachments(ndAttach);
                    // var existingIds = new HashSet<int?>(db.VanKiens.Select(n => n.VanKienOldId).ToList()); // Lưu trữ OldID đã tồn tại

                    // processBar.Maximum = TableFull.Rows.Count;
                    DataTable dtbulk = new DataTable();
                    dtbulk = TableFull;
                    //  InsertDataIntoDatabase(dtbulk);
                    MessageBox.Show("đã xong");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"*** ERR: {ex.Message}\r\n");
                }
            }
        }

        private async void btnDongBoPhienHop_Click(object sender, EventArgs e)
        {
            using (var context = new QuocHoiVNEntities())
            {
                txtLog.Text = $"Bắt đầu đồng bộ. {DateTime.Now}\n";

                var vanKiens = await context.VanKiens.Where(x => x.ModerationStatus == 0).OrderBy(x => x.VanKienOldId).ToListAsync();

                if (vanKiens != null && vanKiens.Count > 0)
                {
                    var logMessages = new List<string>();
                    var vanKienItemsToInsert = new List<QHBASEPHIENHOP.VanKienTaiLieu>();
                    var coQuanMapItemsToInsert = new List<QHBASEPHIENHOP.VanKienMapCQBH>();

                    // check trùng phiên và kỳ
                    var listId = vanKiens.Select(x => x.VanKienOldId).ToList();
                    using (var contextCheckPhienHop = new Preview_QuocHoi_PhienHopServiceEntities())
                    using (var contextCheckKyHop = new Preview_QuocHoi_KyHopServiceEntities())
                    {
                        var messages = new List<string>();

                        var allOldIdsInPhienHop = await contextCheckPhienHop.VanKienTaiLieus
                            .Where(x => listId.Contains(x.OldId))
                            .Select(x => x.OldId)
                            .ToListAsync();

                        var allOldIdsInKyHop = await contextCheckKyHop.VanKienTaiLieus
                            .Where(x => listId.Contains(x.OldId))
                            .Select(x => x.OldId)
                            .ToListAsync();

                        // Kết hợp OldId vào một HashSet để kiểm tra nhanh hơn
                        var allOldIdsToRemove = new HashSet<int?>(allOldIdsInPhienHop);
                        allOldIdsToRemove.UnionWith(allOldIdsInKyHop);

                        vanKiens = vanKiens.Where(vk => !allOldIdsToRemove.Contains(vk.VanKienOldId)).ToList();

                        // Đếm số lượng cho cả hai bối cảnh
                        var countPhienHop = allOldIdsInPhienHop.Count;
                        var countKyHop = allOldIdsInKyHop.Count;

                        if (countPhienHop > 0)
                        {
                            messages.Add($"- Văn kiện {string.Join(", ", allOldIdsInPhienHop)} đã được đồng bộ.\n");
                        }
                        if (countKyHop > 0)
                        {
                            messages.Add($"- Văn kiện {string.Join(", ", allOldIdsInKyHop)} đã được đồng bộ ở kỳ họp\n");
                        }
                        if (messages.Any())
                        {
                            txtLog.Text += string.Join("", messages);
                            Application.DoEvents();
                        }
                    }
                    try
                    {
                        var i = 1;
                        foreach (var item in vanKiens)
                        {
                            txtLog.Text += $"- Văn kiện {item.VanKienOldId} - {item.Title} bắt đầu được đồng bộ-- {i}.\n";
                            Application.DoEvents();

                            var vanKienItem = new QHBASEPHIENHOP.VanKienTaiLieu
                            {
                                Id = Guid.NewGuid(),
                                Title = item.Title,
                                OldId = item.VanKienOldId,
                                ConcurrencyStamp = Guid.NewGuid().ToString(),
                                DescriptionSEO = item.Title,
                                PageUrlSEO = UtilsBase.ConvertToUrlString(item.Title),
                                CreatorName = "admin",
                                CreatorId = new Guid("C1B327B8-10CB-4668-A6D1-680CDFE65720"),
                                CreationTime = DateTime.Now,
                                ExtraProperties = "{}",
                                DMPhienHopId = Guid.Empty,
                                Status = 3,
                                PublicDate = item.NgayTao,
                                AttachmentId = item.Attachments,
                                ShareSocial = true,
                                IsDeleted = false,
                                VanKienMapCQBHs = new List<QHBASEPHIENHOP.VanKienMapCQBH>(),
                                Content = null,
                                LinhVucId = await GetGuidLinhVuc(item.DMLinhVuc)
                            };
                            await GetPhienHopIdLoaiVanKienId(item.DMVanKien, vanKienItem);
                            if (vanKienItem.DMPhienHopId != Guid.Empty)
                            {
                                i++;
                                vanKienItemsToInsert.Add(vanKienItem);
                                coQuanMapItemsToInsert.AddRange(await GetCoQuanBanHanhMappingsPhienHop(item.DMCoQuan, vanKienItem.Id));
                            }
                            else
                            {
                                txtLog.Text += $"- Văn kiện {item.VanKienOldId} - {item.Title} không tìm thấy danh mục trong văn kiện.\n";
                                Application.DoEvents();
                            }
                        }

                        if (vanKienItemsToInsert.Any())
                        {
                            using (var insertContext = new Preview_QuocHoi_PhienHopServiceEntities())
                            {
                                insertContext.VanKienTaiLieus.AddRange(vanKienItemsToInsert);
                                await insertContext.SaveChangesAsync();

                                if (coQuanMapItemsToInsert.Any())
                                {
                                    InsertVanKienTLPhienHopMap(coQuanMapItemsToInsert);
                                }
                            }
                            foreach (var item in vanKienItemsToInsert)
                            {
                                if (item.AttachmentId == 1)
                                {
                                    //await GetAttachmentSp(item.Id, item.OldId.ToString());
                                }
                            }
                        }

                    }
                    catch (DbEntityValidationException ex)
                    {
                        foreach (var eve in ex.EntityValidationErrors)
                        {
                            Console.WriteLine($"Entity of type \"{eve.Entry.Entity.GetType().Name}\" in state \"{eve.Entry.State}\" has the following validation errors:");
                            foreach (var ve in eve.ValidationErrors)
                            {
                                Console.WriteLine($"- Property: \"{ve.PropertyName}\", Error: \"{ve.ErrorMessage}\"");
                            }
                        }
                        MessageBox.Show(ex.Message);
                    }

                    txtLog.Text += $"- Đồng bộ thành công.{DateTime.Now}\n";
                    Application.DoEvents();
                    MessageBox.Show("Đồng bộ thành công.");
                }
                else
                {
                    txtLog.Text += "- Danh sách Văn kiện rỗng.\n";
                    Application.DoEvents();
                }
            }
        }

        private async Task GetPhienHopIdLoaiVanKienId(string loaiVanKien, QHBASEPHIENHOP.VanKienTaiLieu vanKien)
        {
            if (string.IsNullOrEmpty(loaiVanKien)) return;

            var loaiVanKienId = UtilsBase.getLookup(loaiVanKien);
            using (var context = new QuocHoiVNEntities())
            {
                var item = await context.VanKienCats.FirstOrDefaultAsync(x => x.CatOldID == loaiVanKienId);
                if (item == null)
                {
                    //txtLog.Text += "- Không tìm thấy văn kiện.";
                    return;
                }

                var tenLoaiVanKien = item.CatName;
                vanKien.LoaiVanKienId = await GetGuidLoaiVanKien(tenLoaiVanKien);

                if (item.CatParentId == null || item.CatParentId <= 0)
                {
                    //txtLog.Text += "- Không có Kỳ họp của văn kiện.\n";
                    return;
                }
                var kyHopItem = new VanKienCat();
                if (vanKien.LoaiVanKienId is null && vanKien.Title.ToLower().Contains("phiên họp"))
                {
                    kyHopItem = await context.VanKienCats.FirstOrDefaultAsync(x => x.CatOldID == item.CatOldID);

                }
                else
                {
                    kyHopItem = await context.VanKienCats.FirstOrDefaultAsync(x => x.CatOldID == item.CatParentId);

                }
                if (kyHopItem?.CatParentId == null || kyHopItem.CatParentId <= 0)
                {
                    //txtLog.Text += "- Không có Khóa họp của văn kiện.\n";
                    return;
                }

                var khoaItem = await context.VanKienCats.FirstOrDefaultAsync(x => x.CatOldID == kyHopItem.CatParentId);
                var kyHopGuid = await GetGuidPhienHopAsync(kyHopItem, khoaItem);
                if (kyHopGuid != null)
                {
                    vanKien.DMPhienHopId = kyHopGuid.Value;
                }

            }
        }
        private async Task<Guid?> GetGuidPhienHopAsync(VanKienCat kyHop, VanKienCat khoa)
        {
            // Chuyển đổi tên kỳ họp thành chữ thường
            var kyHopNameLower = kyHop.CatName.ToLower();
            var tempString1 = "tổng kết công tác hđnd năm 2023,triển khai kế hoạch năm 2024".ToLower();

            // Kiểm tra nếu là kỳ họp
            if (!(kyHopNameLower.Contains("phiên họp") || kyHopNameLower.Contains("hội nghị") || kyHopNameLower.Contains(tempString1)))
            {
                return null;
            }

            using (var context = new Preview_QuocHoi_PhienHopServiceEntities())
            {
                // Lấy danh sách Khóa
                var khoaCMS = await context.DMPhienHops
                    .Where(x => x.ParentId == null && !x.IsDeleted && x.Title.ToLower().Contains(khoa.CatName.ToLower()))
                    .FirstOrDefaultAsync();

                if (khoaCMS == null) return null;

                // Tạo từ điển cho các tên kỳ
                var kyMap = new Dictionary<string, string>
                    {
                        { "thứ nhất", "1" },
                        { "thứ hai", "2" },
                        { "thứ ba", "3" },
                        { "thứ tư", "4" },
                        { "thứ năm", "5" },
                        { "thứ sáu", "6" },
                        { "thứ bảy", "7" },
                        { "thứ tám", "8" },
                        { "thứ chín", "9" },
                        { "thứ mười", "10" },
                        { "thứ mười một", "11" }
                    };

                // Kiểm tra tên kỳ và lấy tempKyHop
                var tempKyHop = kyMap
                    .Where(k => kyHopNameLower.Contains(k.Key))
                    .Select(k => CatChuoiDenKyTu(kyHop.CatName, k.Value))
                    .FirstOrDefault() ?? kyHop.CatName;

                // Tạo danh sách các điều kiện để tìm kiếm
                var searchTerms = new List<string> { kyHopNameLower, tempKyHop };

                if (kyHopNameLower.Contains("hội nghị đbqh"))
                {
                    searchTerms.Add(kyHopNameLower.Replace("đbqh", "đại biểu Quốc hội").Trim());
                }

                if (kyHopNameLower.Contains("thường vụ -"))
                {
                    searchTerms.Add(CatChuoiOfThuongVu(kyHopNameLower));
                }

                if (kyHopNameLower.Contains("ủy ban thường vụ"))
                {
                    searchTerms.Add(kyHopNameLower.Substring(0, kyHopNameLower.IndexOf(',')).Trim());
                }

                if (kyHopNameLower.Contains(tempString1))
                {
                    var temp = "Tổng kết công tác HĐND toàn quốc năm 2023, triển khai kế hoạch năm 2024".ToLower();
                    searchTerms.Add(temp.Trim());
                }
                // Tìm kiếm kỳ họp
                return await context.DMPhienHops
                    .Where(x => x.ParentId == khoaCMS.Id && searchTerms.Contains(x.Title.ToLower()))
                    .Select(x => (Guid?)x.Id)
                    .FirstOrDefaultAsync();
            }
        }

        private async Task GetKyHopIdLoaiVanKienId(string loaiVanKien, QHBASE.VanKienTaiLieu vanKien)
        {
            if (string.IsNullOrEmpty(loaiVanKien)) return;

            var loaiVanKienId = UtilsBase.getLookup(loaiVanKien);
            using (var context = new QuocHoiVNEntities())
            {
                var item = await context.VanKienCats.FirstOrDefaultAsync(x => x.CatOldID == loaiVanKienId);
                if (item == null)
                {
                    //txtLog.Text += "- Không tìm thấy văn kiện.";
                    return;
                }

                var tenLoaiVanKien = item.CatName;
                vanKien.LoaiVanKienId = await GetGuidLoaiVanKien(tenLoaiVanKien);

                if (item.CatParentId == null || item.CatParentId <= 0)
                {
                    //txtLog.Text += "- Không có Kỳ họp của văn kiện.\n";
                    return;
                }
                var kyHopItem = new VanKienCat();
                if (vanKien.LoaiVanKienId is null && vanKien.Title.ToLower().Contains("kỳ họp"))
                {
                    kyHopItem = await context.VanKienCats.FirstOrDefaultAsync(x => x.CatOldID == item.CatOldID);

                }
                else
                {
                    kyHopItem = await context.VanKienCats.FirstOrDefaultAsync(x => x.CatOldID == item.CatParentId);

                }
                if (kyHopItem?.CatParentId == null || kyHopItem.CatParentId <= 0)
                {
                    //txtLog.Text += "- Không có Khóa họp của văn kiện.\n";
                    return;
                }

                var khoaItem = await context.VanKienCats.FirstOrDefaultAsync(x => x.CatOldID == kyHopItem.CatParentId);
                var kyHopGuid = await GetGuidKyHopAsync(kyHopItem, khoaItem);
                if (kyHopGuid != null)
                {
                    vanKien.DMKyHopId = kyHopGuid.Value;
                }

            }
        }

        private async Task<Guid?> GetGuidKyHopAsync(VanKienCat kyHop, VanKienCat khoa)
        {
            Guid? result = null;

            // Chuyển đổi tên kỳ họp thành chữ thường chỉ một lần
            var kyHopNameLower = kyHop.CatName.ToLower();

            // Kiểm tra nếu là kỳ họp
            if (kyHopNameLower.Contains("kỳ họp"))
            {
                using (var context = new Preview_QuocHoi_KyHopServiceEntities())
                {
                    // Lấy danh sách Khóa
                    var khoaCMS = await context.DMKyHops
                        .Where(x => x.ParentId == null && !x.IsDeleted && x.Title.ToLower().Contains(khoa.CatName.ToLower()))
                        .FirstOrDefaultAsync();

                    if (khoaCMS != null)
                    {
                        // Tạo từ điển cho các tên kỳ
                        var kyMap = new Dictionary<string, string>
                            {
                                { "thứ nhất", "1" },
                                { "thứ hai", "2" },
                                { "thứ ba", "3" },
                                { "thứ tư", "4" },
                                { "thứ năm", "5" },
                                { "thứ sáu", "6" },
                                { "thứ bảy", "7" },
                                { "thứ tám", "8" },
                                { "thứ chín", "9" },
                                { "thứ mười", "10" },
                                { "thứ mười một", "11" }
                            };

                        // Kiểm tra tên kỳ và lấy tempKyHop
                        var tempKyHop = kyMap
                            .Where(k => kyHopNameLower.Contains(k.Key))
                            .Select(k => CatChuoiDenKyTu(kyHop.CatName, k.Value))
                            .FirstOrDefault() ?? kyHop.CatName;

                        // Kiểm tra "lần" và tạo tempStringLan check Khóa quốc hội 15
                        var tempStringLan = string.Empty;
                        // khóa quốc hội 15 sai tên 
                        if (kyHop.CatOldID == 545)
                        {
                            tempStringLan = kyHopNameLower.Replace("thứ", "lần thứ").Trim();
                        }

                        // Tạo danh sách các tên kỳ họp để tìm kiếm
                        var kyHopNamesToSearch = new List<string>
                            {
                                kyHopNameLower,
                                tempKyHop.ToLower(),
                            };

                        // Thêm tempStringLan vào danh sách nếu có dữ liệu
                        if (!string.IsNullOrEmpty(tempStringLan))
                        {
                            var tempKyLan = kyMap
                                .Where(k => tempStringLan.Contains(k.Key))
                                .Select(k => CatChuoiDenKyTu(tempStringLan, k.Value))
                                .FirstOrDefault() ?? kyHop.CatName;

                            kyHopNamesToSearch.Add(tempKyLan.ToLower());
                            kyHopNamesToSearch.Add(tempStringLan);
                        }

                        // Tìm kiếm kỳ họp
                        result = await context.DMKyHops
                            .Where(x => x.ParentId == khoaCMS.Id &&
                                        kyHopNamesToSearch.Contains(x.Title.ToLower()))
                            .Select(x => (Guid?)x.Id)
                            .FirstOrDefaultAsync();
                    }
                }
            }

            return result;
        }

        // add cơ quan ban hành

        private async Task<List<QHBASEPHIENHOP.VanKienMapCQBH>> GetCoQuanBanHanhMappingsPhienHop(string dmCoQuan, Guid vanKienId)
        {
            var mappings = new List<QHBASEPHIENHOP.VanKienMapCQBH>();

            if (string.IsNullOrEmpty(dmCoQuan)) return mappings;

            var listIdInt = UtilsBase.getListLookup(dmCoQuan);
            if (listIdInt == null || !listIdInt.Any()) return mappings;

            using (var context = new Preview_QuocHoi_CommonsServiceEntities())
            {
                var coQuanItems = await context.CoQuanBanHanhs
                    .Where(x => listIdInt.Contains(x.OldID.Value))
                    .ToListAsync();

                var coQuanLookup = coQuanItems.ToDictionary(x => x.OldID, x => x.Id);

                foreach (var id in listIdInt)
                {
                    if (coQuanLookup.TryGetValue(id, out var guiIdCoQuan))
                    {
                        var mapping = new QHBASEPHIENHOP.VanKienMapCQBH
                        {
                            VanKienTaiLieuId = vanKienId,
                            //CQBHId = guiIdCoQuan,
                            OldVanKienId = id,
                        };
                        mappings.Add(mapping);
                    }
                }
            }

            return mappings;
        }
        private async Task<List<QHBASE.VanKienMapCQBH>> GetCoQuanBanHanhMappingsKyHop(string dmCoQuan, Guid vanKienId)
        {
            var mappings = new List<QHBASE.VanKienMapCQBH>();

            if (string.IsNullOrEmpty(dmCoQuan)) return mappings;

            var listIdInt = UtilsBase.getListLookup(dmCoQuan);
            if (listIdInt == null || !listIdInt.Any()) return mappings;

            using (var context = new Preview_QuocHoi_CommonsServiceEntities())
            {
                var coQuanItems = await context.CoQuanBanHanhs
                    .Where(x => listIdInt.Contains(x.OldID.Value))
                    .ToListAsync();

                var coQuanLookup = coQuanItems.ToDictionary(x => x.OldID, x => x.Id);

                foreach (var id in listIdInt)
                {
                    if (coQuanLookup.TryGetValue(id, out var guiIdCoQuan))
                    {
                        var mapping = new QHBASE.VanKienMapCQBH
                        {
                            VanKienTaiLieuId = vanKienId,
                            //CQBHId = guiIdCoQuan,
                            OldVanKienId = id,
                        };
                        mappings.Add(mapping);
                    }
                }
            }

            return mappings;
        }
        public void InsertVanKienTLPhienHopMap(List<QHBASEPHIENHOP.VanKienMapCQBH> items)
        {
            if (items == null || items.Count == 0)
                return; // Trả về nếu danh sách rỗng

            using (var db = new Preview_QuocHoi_PhienHopServiceEntities())
            {
                db.VanKienMapCQBHs.AddRange(items); // Thêm tất cả các mục vào DbSet
                db.SaveChanges(); // Lưu tất cả thay đổi vào cơ sở dữ liệu
            }
        }
        public void InsertVanKienTLKyHopMap(List<QHBASE.VanKienMapCQBH> items)
        {
            if (items == null || items.Count == 0)
                return; // Trả về nếu danh sách rỗng

            using (var db = new Preview_QuocHoi_KyHopServiceEntities())
            {
                db.VanKienMapCQBHs.AddRange(items); // Thêm tất cả các mục vào DbSet
                db.SaveChanges(); // Lưu tất cả thay đổi vào cơ sở dữ liệu
            }
        }

    }
}
