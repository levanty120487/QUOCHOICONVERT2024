using HtmlAgilityPack;
using QHBASE;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RJCodeUI_M1.TestAndDemo
{
    public partial class FormCloneMoetVanBan : RJForms.RJChildForm
    {
        private List<VanBanItem> parsedData = new List<VanBanItem>();
        private Label lblStatus;

        public FormCloneMoetVanBan()
        {
            InitializeComponent();
        }

        private void FormCloneMoetVanBan_Load(object sender, EventArgs e)
        {
            lblStatus = new Label();
            lblStatus.AutoSize = true;
            lblStatus.ForeColor = System.Drawing.Color.Red;
            lblStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            lblStatus.Location = new System.Drawing.Point(20, 168);
            lblStatus.Text = "";
            pnlClientArea.Controls.Add(lblStatus);

            LoadCategories();
        }

        private void LoadCategories()
        {
            var categories = new[]
            {
                new { Id = 1, Title = "Văn bản quy phạm pháp luật" },
                new { Id = 2, Title = "Văn bản chỉ đạo, điều hành" }
            };

            cboCategory.DataSource = categories;
            cboCategory.DisplayMember = "Title";
            cboCategory.ValueMember = "Id";
        }

        private async void btnReadData_Click(object sender, EventArgs e)
        {
            try
            {
                parsedData.Clear();
                int totalPages = 1;
                int.TryParse(txtTotalPages.Text, out totalPages);
                string baseUrl = txtWebUrl.Text.Trim();

                string originalBtnText = btnReadData.Text;
                btnReadData.Enabled = false;
                lblStatus.Text = "Bắt đầu đọc dữ liệu...";

                int startPage = 1;
                int endPage = totalPages;

                if (!string.IsNullOrWhiteSpace(txtTuPage.Text) && !string.IsNullOrWhiteSpace(txtDenPage.Text) &&
                    int.TryParse(txtTuPage.Text, out int tuPage) && int.TryParse(txtDenPage.Text, out int denPage))
                {
                    startPage = tuPage;
                    endPage = denPage;
                }

                for (int i = startPage; i <= endPage; i++)
                {
                    string pageUrl = baseUrl;
                    if (i > 1)
                    {
                        // Update pageNo for pagination
                        if (baseUrl.Contains("pageNo="))
                        {
                            pageUrl = Regex.Replace(baseUrl, @"pageNo=\d+", $"pageNo={i}");
                        }
                        else
                        {
                            pageUrl = baseUrl.Contains("?") ? $"{baseUrl}&pageNo={i}" : $"{baseUrl}?pageNo={i}";
                        }
                    }

                    var items = await ReadListPageAsync(pageUrl, i);
                    if (items != null)
                        parsedData.AddRange(items);
                }

                btnReadData.Text = originalBtnText;
                btnReadData.Enabled = true;
                lblStatus.Text = $"Hoàn tất! Đã đọc {parsedData.Count} bản ghi.";

                dgvData.DataSource = null;
                dgvData.DataSource = parsedData;
                MessageBox.Show($"Đã đọc xong {parsedData.Count} bài viết.", "Thông báo");
            }
            catch (Exception ex)
            {
                btnReadData.Enabled = true;
                btnReadData.Text = "Đọc dữ liệu";
                MessageBox.Show("Lỗi đọc dữ liệu: " + ex.Message);
            }
        }

        private async Task<List<VanBanItem>> ReadListPageAsync(string pageUrl, int currentPageIndex)
        {
            var result = new List<VanBanItem>();
            using (var httpClient = new HttpClient())
            {
                httpClient.Timeout = TimeSpan.FromSeconds(60);
                string html = await httpClient.GetStringAsync(pageUrl);
                var doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(html);

                var listNodes = doc.DocumentNode.SelectNodes("//div[contains(@class, 'list-legal-document-table')]//div[contains(@class, 'items')]");
                if (listNodes == null) return result;

                int itemIndex = 0;
                foreach (var itemNode in listNodes)
                {
                    itemIndex++;
                    lblStatus.Text = $"Đã đọc trang {currentPageIndex}/ bản ghi thứ {itemIndex}";
                    Application.DoEvents();

                    var item = new VanBanItem();

                    var trichYeuNode = itemNode.SelectSingleNode(".//div[contains(@class, 'border-right') and contains(@class, 'border-dotted')]");
                    if (trichYeuNode != null) item.TrichYeu = CleanText(trichYeuNode.InnerText);

                    var loaiVanBanNode = itemNode.SelectSingleNode(".//div[contains(@class, 'properties')]//table//tbody//tr[2]//td[1]");
                    if (loaiVanBanNode != null) item.LoaiVanBan = CleanText(loaiVanBanNode.InnerText);

                    var linhVucNode = itemNode.SelectSingleNode(".//div[contains(@class, 'properties')]//table//tbody//tr[3]//td[1]");
                    if (linhVucNode != null) item.LinhVuc = CleanText(linhVucNode.InnerText);

                    var soKyHieuNode = itemNode.SelectSingleNode(".//div[contains(@class, 'properties')]//table//tbody//tr[1]//td[1]");
                    if (soKyHieuNode != null) item.SoKyHieu = CleanText(soKyHieuNode.InnerText);

                    var ngayBanHanhNode = itemNode.SelectSingleNode(".//div[contains(@class, 'properties')]//table//tbody//tr[1]//td[2]");
                    if (ngayBanHanhNode != null) item.NgayBanHanh = ParseDate(CleanText(ngayBanHanhNode.InnerText));

                    var tinhTrangHieuLucNode = itemNode.SelectSingleNode(".//div[contains(@class, 'properties')]//table//tbody//tr[4]//td[1]");
                    if (tinhTrangHieuLucNode != null) item.TinhTrangHieuLuc = CleanText(tinhTrangHieuLucNode.InnerText);

                    var titleLinkNode = itemNode.SelectSingleNode(".//div[contains(@class, 'title')]//a");
                    if (titleLinkNode != null)
                    {
                        item.DetailUrl = ToAbsoluteUrl(pageUrl, titleLinkNode.GetAttributeValue("href", ""));
                    }

                    if (!string.IsNullOrEmpty(item.DetailUrl))
                    {
                        await ParseDetailAsync(httpClient, item, item.DetailUrl);
                    }

                    result.Add(item);
                }
            }
            return result;
        }

        private async Task ParseDetailAsync(HttpClient httpClient, VanBanItem item, string detailUrl)
        {
            try
            {
                string html = await httpClient.GetStringAsync(detailUrl);
                var doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(html);

                var tableNode = doc.DocumentNode.SelectSingleNode("//div[contains(@id, 'tabs-description')]//div[@class='table-responsive']//table");
                if (tableNode != null)
                {
                    var tbody = tableNode.SelectSingleNode(".//tbody");
                    if (tbody != null)
                    {
                        var coQuanBanHanhNode = tbody.SelectSingleNode(".//tr[3]//td[1]//div[1]");
                        if (coQuanBanHanhNode != null) item.CoQuanBanHanh = CleanText(coQuanBanHanhNode.InnerText);

                        var nguoiKyNode = tbody.SelectSingleNode(".//tr[3]//td[1]//div[2]");
                        if (nguoiKyNode != null) item.NguoiKy = CleanText(nguoiKyNode.InnerText);

                        var ngayCoHieuLucNode = tbody.SelectSingleNode(".//tr[3]//td[2]");
                        if (ngayCoHieuLucNode != null) item.NgayCoHieuLuc = ParseDate(CleanText(ngayCoHieuLucNode.InnerText));

                        var ngayHetHieuLucNode = tbody.SelectSingleNode(".//tr[4]//td[2]");
                        if (ngayHetHieuLucNode != null) item.NgayHetHieuLuc = ParseDate(CleanText(ngayHetHieuLucNode.InnerText));
                    }
                }

                var attachTableNode = doc.DocumentNode.SelectSingleNode("//div[@id='tabs-description-3-11']//table");
                if (attachTableNode != null)
                {
                    var aNodes = attachTableNode.SelectNodes(".//tr//td//a");
                    if (aNodes != null)
                    {
                        foreach (var aNode in aNodes)
                        {
                            var href = aNode.GetAttributeValue("href", "");
                            var fileName = CleanText(aNode.InnerText);
                            if (!string.IsNullOrWhiteSpace(href))
                            {
                                if (string.IsNullOrWhiteSpace(fileName))
                                {
                                    fileName = GetFileNameFromUrl(href);
                                }
                                else if (!Path.HasExtension(fileName))
                                {
                                    var ext = Path.GetExtension(href.Split('?')[0]);
                                    if (!string.IsNullOrEmpty(ext))
                                    {
                                        fileName += ext;
                                    }
                                }
                                
                                string absUrl = ToAbsoluteUrl(detailUrl, href);
                                if (!item.Attachments.Any(a => a.Url == absUrl))
                                {
                                    item.Attachments.Add(new AttachmentFile { FileName = fileName, Url = absUrl });
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi chi tiết: " + ex.Message);
            }
        }

        private async void btnSaveData_Click(object sender, EventArgs e)
        {
            if (parsedData == null || parsedData.Count == 0)
            {
                MessageBox.Show("Chưa có dữ liệu để lưu!");
                return;
            }

            if (cboCategory.SelectedValue == null)
            {
                MessageBox.Show("Vui lòng chọn loại văn bản!");
                return;
            }

            int vanBanQCTC = (int)cboCategory.SelectedValue;

            string downloadFolder = @"C:\uploadFckFiles\vanban";
            if (!string.IsNullOrWhiteSpace(txtFolderChua.Text))
            {
                downloadFolder = Path.Combine(downloadFolder, txtFolderChua.Text.Trim());
            }
            Directory.CreateDirectory(downloadFolder);

            string originalBtnText = btnSaveData.Text;
            btnSaveData.Enabled = false;
            lblStatus.Text = "Bắt đầu lưu dữ liệu...";

            try
            {
                using (var dbVanBan = new Portal_Moet_VanbanServiceEntities()) // Using Moet_VanBans.Context.cs Entity
                using (var dbFiles = new Portal_Moet_FilesServicesEntities()) // Using Moet_Files.Context.cs Entity
                using (var httpClient = new HttpClient())
                {
                    int currentIndex = 0;
                    int totalItems = parsedData.Count;
                    var errorList = new List<string>();
                    var duplicateList = new List<string>();

                    foreach (var item in parsedData.OrderByDescending(a=>a.CreateAt))
                    {
                        try
                        {
                            if (string.IsNullOrWhiteSpace(item.SoKyHieu))
                            {
                                item.SoKyHieu = item.LoaiVanBan;
                            }
                            if (string.IsNullOrWhiteSpace(item.SoKyHieu))
                            {
                                continue;
                            }
                                
                                
                            currentIndex++;
                            lblStatus.Text = $"Đang lưu bản ghi thứ {currentIndex}/{totalItems}...";
                            Application.DoEvents();

                            var existingLaw = await dbVanBan.Laws.FirstOrDefaultAsync(l => l.DetailLinkClone == item.DetailUrl);
                            if (existingLaw != null)
                            {
                                duplicateList.Add($"- Số KH: {item.SoKyHieu}\n  Link: {item.DetailUrl}");
                                continue;
                            }

                            // Xử lý Category / Status
                            string idEffectStatus  = string.Empty;
                            if (!string.IsNullOrEmpty(item.TinhTrangHieuLuc))
                            {
                                var effect = dbVanBan.EffectStatus.FirstOrDefault(x => x.Title == item.TinhTrangHieuLuc);
                                if (effect == null)
                                {
                                    effect = new EffectStatu { Id = QHCommons.GenAutoId(), Title = item.TinhTrangHieuLuc,
                                        Language = "vi", IsShow = true
                                    };
                                    dbVanBan.EffectStatus.Add(effect);
                                    dbVanBan.SaveChanges();
                                }
                                idEffectStatus = effect.Id;
                            }

                            string idTypeOfDocument = string.Empty;
                            if (!string.IsNullOrEmpty(item.LoaiVanBan))
                            {
                                var type = dbVanBan.TypeOfDocuments.FirstOrDefault(x => x.Title == item.LoaiVanBan);
                                if (type == null)
                                {
                                    type = new TypeOfDocument { Id = QHCommons.GenAutoId(), Title = item.LoaiVanBan , Language = "vi", CreatedBy = "admin" , IsShow = true };
                                    dbVanBan.TypeOfDocuments.Add(type);
                                    dbVanBan.SaveChanges();
                                }
                                idTypeOfDocument = type.Id;
                            }

                            // Tạo Law
                            var law = new Law
                            {
                                Id = QHCommons.GenAutoId(),
                                Title = item.LoaiVanBan + " " + item.SoKyHieu,
                                Status = 1,
                                OfficialNumber = item.SoKyHieu,
                                PublishedDate = item.NgayBanHanh ?? DateTime.Now,
                                EffectiveDate = item.NgayCoHieuLuc,
                                ExpiryDate = item.NgayHetHieuLuc,
                                PublicDate = item.NgayBanHanh ?? DateTime.Now,
                                Source = item.TrichYeu,
                                EffectiveArea = string.Empty,
                                Content = string.Empty,
                                CreatedBy = "admin",
                                IdEffectStatus = idEffectStatus,
                                IdTypeOfDocument = idTypeOfDocument,
                                VanBanQCTC = vanBanQCTC,
                                ConcurrencyStamp = Guid.NewGuid().ToString(),
                                ExtraProperties = "{}",
                                Language = "vi",
                                CreationTime = DateTime.Now,
                                DetailLinkClone = item.DetailUrl,
                                TypeOfDocumentTitle = item.LoaiVanBan
                            };
                            dbVanBan.Laws.Add(law);
                            dbVanBan.SaveChanges();

                            // Tạo LawSigner
                            string idSigner = string.Empty;
                            if (!string.IsNullOrEmpty(item.NguoiKy))
                            {
                                var signer = dbVanBan.Signers.FirstOrDefault(x => x.Title == item.NguoiKy);
                                if (signer == null)
                                {
                                    signer = new Signer { Id = QHCommons.GenAutoId(), Title = item.NguoiKy, CreatedBy = "admin" , IsShow = true};
                                    dbVanBan.Signers.Add(signer);
                                    dbVanBan.SaveChanges();
                                }
                                idSigner = signer.Id;
                            }

                            string idPromulgator = string.Empty;
                            if (!string.IsNullOrEmpty(item.CoQuanBanHanh))
                            {
                                var promulgator = dbVanBan.Promulgators.FirstOrDefault(x => x.Title == item.CoQuanBanHanh);
                                if (promulgator == null)
                                {
                                    promulgator = new Promulgator { Id = QHCommons.GenAutoId(), Title = item.CoQuanBanHanh, Language = "vi", CreatedBy = "admin" , IsShow = true };
                                    dbVanBan.Promulgators.Add(promulgator);
                                    dbVanBan.SaveChanges();
                                }
                                idPromulgator = promulgator.Id;
                            }

                            if (!string.IsNullOrWhiteSpace(idSigner))
                            {
                                var lawSigner = new LawSigner
                                {
                                    Id = Guid.NewGuid(),
                                    IdLaw = law.Id,
                                    IdSigner = idSigner,
                                    IdPromulgator = idPromulgator,
                                    Position = ""
                                };
                                dbVanBan.LawSigners.Add(lawSigner);
                                dbVanBan.SaveChanges();
                            }

                            // Tạo LawField
                            string idField = null;
                            if (!string.IsNullOrEmpty(item.LinhVuc))
                            {
                                var field = dbVanBan.Fields.FirstOrDefault(x => x.Title == item.LinhVuc);
                                if (field == null)
                                {
                                    field = new Field { Id = QHCommons.GenAutoId(), Title = item.LinhVuc, Language = "vi", CreatedBy = "admin", IsShow = true };
                                    dbVanBan.Fields.Add(field);
                                    dbVanBan.SaveChanges();
                                }
                                idField = field.Id;
                            }

                            if (!string.IsNullOrWhiteSpace(idField))
                            {
                                var lawField = new LawField
                                {
                                    Id = Guid.NewGuid(),
                                    IdLaw = law.Id,
                                    IdField = idField
                                };
                                dbVanBan.LawFields.Add(lawField);
                                dbVanBan.SaveChanges();
                            }

                            // Tải file đính kèm
                            foreach (var attach in item.Attachments)
                            {
                                string fileNameToDownload = string.IsNullOrWhiteSpace(attach.FileName) ? GetFileNameFromUrl(attach.Url) : attach.FileName;
                                string localFilePath = await DownloadFileAsync(httpClient, attach.Url, downloadFolder, fileNameToDownload);

                                if (!string.IsNullOrEmpty(localFilePath))
                                {
                                    var file = new CMSFile()
                                    {
                                        Id = Guid.NewGuid(),
                                        FileType = 2,
                                        CreationTime = DateTime.Now,
                                        FileContainerName = "CMSContainerPublic",
                                        ConcurrencyStamp = Guid.NewGuid().ToString(),
                                        ExtraProperties = "{}",
                                        MimeType = GetMimeType(attach.FileName),
                                        FileExtention = 2,
                                        Language = "vi",
                                        FullPathServer = localFilePath.Replace("C:\\", "/").Replace("\\", "/"),
                                        FileName = attach.FileName
                                    };
                                    dbFiles.CMSFiles.Add(file);
                                    dbFiles.SaveChanges();
                                    
                                    var fileAttach = new CMSFileAttachment()
                                    {
                                        Id = Guid.NewGuid(),
                                        EntityId = law.Id,
                                        FileId = file.Id,
                                        ExtraProperties = "{}",
                                        ConcurrencyStamp = Guid.NewGuid().ToString(),
                                        CreationTime = DateTime.Now,
                                        FileAttachmentType = 2
                                    };
                                    dbFiles.CMSFileAttachments.Add(fileAttach);
                                    dbFiles.SaveChanges();
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            errorList.Add($"- Số KH: {item.SoKyHieu}\n  Lỗi: {ex.Message}");
                        }
                    }

                    if (errorList.Count > 0 || duplicateList.Count > 0)
                    {
                        string msg = "";
                        if (errorList.Count > 0)
                        {
                            msg += $"--- DANH SÁCH LỖI ({errorList.Count} bản ghi) ---\r\n";
                            msg += string.Join("\r\n\r\n", errorList) + "\r\n\r\n";
                        }
                        if (duplicateList.Count > 0)
                        {
                            msg += $"--- DANH SÁCH TRÙNG ({duplicateList.Count} bản ghi) ---\r\n";
                            msg += string.Join("\r\n\r\n", duplicateList);
                        }

                        using (Form msgForm = new Form())
                        {
                            msgForm.Text = $"Kết quả lưu: {errorList.Count} lỗi, {duplicateList.Count} trùng";
                            msgForm.Size = new System.Drawing.Size(800, 600);
                            msgForm.StartPosition = FormStartPosition.CenterScreen;

                            TextBox txtMsg = new TextBox();
                            txtMsg.Multiline = true;
                            txtMsg.ReadOnly = true;
                            txtMsg.ScrollBars = ScrollBars.Vertical;
                            txtMsg.Dock = DockStyle.Fill;
                            txtMsg.Text = msg;

                            msgForm.Controls.Add(txtMsg);
                            msgForm.ShowDialog();
                        }
                    }
                }

                btnSaveData.Text = originalBtnText;
                btnSaveData.Enabled = true;
                MessageBox.Show("Lưu dữ liệu thành công!");
            }
            catch (Exception ex)
            {
                btnSaveData.Enabled = true;
                btnSaveData.Text = "Lưu dữ liệu";
                MessageBox.Show("Lỗi lưu dữ liệu: " + ex.Message);
            }
        }

        static string CleanText(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return "";
            return HtmlEntity.DeEntitize(text).Trim();
        }

        static DateTime? ParseDate(string text)
        {
            if (string.IsNullOrWhiteSpace(text) || text == "-") return null;
            var match = Regex.Match(text, @"\d{2}/\d{2}/\d{4}");
            if (match.Success)
            {
                if (DateTime.TryParseExact(match.Value, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out var dt))
                {
                    return dt;
                }
            }
            return null;
        }

        static string ToAbsoluteUrl(string pageUrl, string href)
        {
            if (string.IsNullOrWhiteSpace(href)) return "";
            if (href.StartsWith("http")) return href;
            try
            {
                var baseUri = new Uri(pageUrl);
                var fullUri = new Uri(baseUri, href);
                return fullUri.ToString();
            }
            catch { return href; }
        }

        static async Task<string> DownloadFileAsync(HttpClient httpClient, string fileUrl, string folder, string fileName)
        {
            try
            {
                using (var response = await httpClient.GetAsync(fileUrl))
                {
                    if (!response.IsSuccessStatusCode) return string.Empty;

                    fileName = Regex.Replace(fileName ?? "", @"[^a-zA-Z0-9.\s_-]", "").Replace(" ", "_");
                    string ext = Path.GetExtension(fileName);
                    if (string.IsNullOrEmpty(ext) && !string.IsNullOrEmpty(fileUrl))
                    {
                        string cleanUrl = fileUrl.Contains("?") ? fileUrl.Substring(0, fileUrl.IndexOf("?")) : fileUrl;
                        ext = Path.GetExtension(cleanUrl);
                        if (!string.IsNullOrEmpty(ext))
                        {
                            fileName += ext;
                        }
                    }

                    if (string.IsNullOrEmpty(fileName) || fileName == ext)
                    {
                        fileName = Guid.NewGuid().ToString("N") + ext;
                    }

                    string fullPath = Path.Combine(folder, fileName);

                    if (File.Exists(fullPath))
                    {
                        return fullPath;
                    }

                    using (var fs = new FileStream(fullPath, FileMode.Create, FileAccess.Write, FileShare.None))
                    {
                        await response.Content.CopyToAsync(fs);
                    }
                    return fullPath;
                }
            }
            catch { return string.Empty; }
        }

        static string GetFileNameFromUrl(string fileUrl)
        {
            try
            {
                var uri = new Uri(fileUrl);
                string fileName = Path.GetFileName(uri.LocalPath);
                return string.IsNullOrWhiteSpace(fileName) ? $"file_{DateTime.Now.Ticks}" : fileName;
            }
            catch { return $"file_{DateTime.Now.Ticks}"; }
        }

        public static string GetMimeType(string fileName)
        {
            string ext = Path.GetExtension(fileName).ToLower();

            switch (ext)
            {
                case ".pdf": return "application/pdf";
                case ".doc": return "application/msword";
                case ".docx": return "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                case ".xls": return "application/vnd.ms-excel";
                case ".xlsx": return "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                case ".png": return "image/png";
                case ".jpg":
                case ".jpeg": return "image/jpeg";
                default: return "application/octet-stream";
            }
        }
    }

    public class VanBanItem
    {
        public string TrichYeu { get; set; }
        public string LoaiVanBan { get; set; }
        public string LinhVuc { get; set; }
        public string SoKyHieu { get; set; }
        public DateTime? NgayBanHanh { get; set; }
        public string TinhTrangHieuLuc { get; set; }
        public string DetailUrl { get; set; }

        public string CoQuanBanHanh { get; set; }
        public string NguoiKy { get; set; }
        public DateTime? NgayCoHieuLuc { get; set; }
        public DateTime? NgayHetHieuLuc { get; set; }

        public List<AttachmentFile> Attachments { get; set; } = new List<AttachmentFile>();

        public DateTime CreateAt {  get; set; } = DateTime.Now;
    }
}
