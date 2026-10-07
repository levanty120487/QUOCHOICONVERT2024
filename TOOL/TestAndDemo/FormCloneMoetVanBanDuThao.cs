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
    public partial class FormCloneMoetVanBanDuThao : RJForms.RJChildForm
    {
        private List<VanBanDuThaoItem> parsedData = new List<VanBanDuThaoItem>();
        private Label lblStatus;

        public FormCloneMoetVanBanDuThao()
        {
            InitializeComponent();
        }

        private void FormCloneMoetVanBanDuThao_Load(object sender, EventArgs e)
        {
            lblStatus = new Label();
            lblStatus.AutoSize = true;
            lblStatus.ForeColor = System.Drawing.Color.Red;
            lblStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            lblStatus.Location = new System.Drawing.Point(20, 168);
            lblStatus.Text = "";
            pnlClientArea.Controls.Add(lblStatus);
        }

        private async void btnReadData_Click(object sender, EventArgs e)
        {
            try
            {
                parsedData.Clear();
                string html = txtWebUrl.Text.Trim(); // Read HTML from TextBox

                if (string.IsNullOrWhiteSpace(html))
                {
                    MessageBox.Show("Vui lòng nhập HTML danh sách!");
                    return;
                }

                string originalBtnText = btnReadData.Text;
                btnReadData.Enabled = false;
                lblStatus.Text = "Bắt đầu đọc dữ liệu...";

                var items = await ReadListPageAsync(html);
                if (items != null)
                    parsedData.AddRange(items);

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

        private async Task<List<VanBanDuThaoItem>> ReadListPageAsync(string htmlContent)
        {
            var result = new List<VanBanDuThaoItem>();
            using (var httpClient = new HttpClient())
            {
                httpClient.Timeout = TimeSpan.FromSeconds(60);
                var doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(htmlContent);

                var listNodes = doc.DocumentNode.SelectNodes("//div[contains(@class, 'items padding-xs')]");
                if (listNodes == null) return result;

                int itemIndex = 0;
                foreach (var itemNode in listNodes)
                {
                    itemIndex++;
                    lblStatus.Text = $"Đã đọc bản ghi thứ {itemIndex}";
                    Application.DoEvents();

                    var item = new VanBanDuThaoItem();

                    var titleLinkNode = itemNode.SelectSingleNode(".//a[contains(@class, 'text-bold')]");
                    if (titleLinkNode != null)
                    {
                        item.SoKyHieu = CleanText(titleLinkNode.InnerText);
                        item.DetailUrl = ToAbsoluteUrl("https://moet.gov.vn", titleLinkNode.GetAttributeValue("href", ""));
                    }

                    var divs = itemNode.SelectNodes(".//div");
                    if (divs != null)
                    {
                        for (int k = 0; k < divs.Count; k++)
                        {
                            var text = CleanText(divs[k].InnerText);
                            if (text.Contains("Ngày bắt đầu:"))
                            {
                                if (k + 1 < divs.Count) item.NgayCoHieuLuc = ParseDate(CleanText(divs[k + 1].InnerText));
                            }
                            else if (text.Contains("Ngày hết hạn:"))
                            {
                                if (k + 1 < divs.Count) item.NgayHetHieuLuc = ParseDate(CleanText(divs[k + 1].InnerText));
                            }
                        }
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

        private async Task ParseDetailAsync(HttpClient httpClient, VanBanDuThaoItem item, string detailUrl)
        {
            try
            {
                string html = await httpClient.GetStringAsync(detailUrl);
                var doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(html);

                var loaiVbNode = doc.DocumentNode.SelectSingleNode("//th[contains(text(), 'Loại văn bản')]/following-sibling::td");
                if (loaiVbNode != null) item.LoaiVanBan = CleanText(loaiVbNode.InnerText);

                var linhVucNode = doc.DocumentNode.SelectSingleNode("//th[contains(text(), 'Lĩnh vực văn bản')]/following-sibling::td");
                if (linhVucNode != null) item.LinhVuc = CleanText(linhVucNode.InnerText);

                var noiDungNode = doc.DocumentNode.SelectSingleNode("//div[@class='brief-vb']");
                if (noiDungNode != null) item.NoiDung = noiDungNode.InnerHtml.Trim();

                var aNodes = doc.DocumentNode.SelectNodes("//a[contains(@href, '/upload/')]");
                if (aNodes != null)
                {
                    foreach (var aNode in aNodes)
                    {
                        var href = aNode.GetAttributeValue("href", "");
                        var fileName = CleanText(aNode.InnerText);
                        
                        // Ignore links that only contain icons (like the download icon)
                        if (string.IsNullOrWhiteSpace(fileName))
                        {
                            continue;
                        }

                        if (!string.IsNullOrWhiteSpace(href))
                        {
                            if (!Path.HasExtension(fileName))
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

            string downloadFolder = @"C:\uploadFckFiles\vanbanduthao";
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
                                continue;
                            }
                                
                            currentIndex++;
                            lblStatus.Text = $"Đang lưu bản ghi thứ {currentIndex}/{totalItems}...";
                            Application.DoEvents();

                            var existingDuThao = await dbVanBan.VanBanDuThaos.FirstOrDefaultAsync(l => l.DetailLinkClone == item.DetailUrl);
                            if (existingDuThao != null)
                            {
                                duplicateList.Add($"- Tên dự thảo: {item.SoKyHieu}\n  Link: {item.DetailUrl}");
                                continue;
                            }

                            string idTypeOfDocument = string.Empty;
                            if (string.IsNullOrEmpty(item.LoaiVanBan))
                            {
                                item.LoaiVanBan = "Khác";
                            }
                            var type = dbVanBan.TypeOfDocuments.FirstOrDefault(x => x.Title == item.LoaiVanBan);
                            if (type == null)
                            {
                                var typeID = QHCommons.GenAutoId();
                                type = new TypeOfDocument { Id = typeID, Title = item.LoaiVanBan, Language = "vi", CreatedBy = "admin", IsShow = true };
                                dbVanBan.TypeOfDocuments.Add(type);
                                dbVanBan.SaveChanges();
                                idTypeOfDocument = typeID;
                            }
                            else
                            {
                                idTypeOfDocument = type.Id;
                            }

                            // Tạo VanBanDuThao
                            var duThao = new VanBanDuThao
                            {
                                Id = QHCommons.GenAutoId(),
                                TrichYeu = string.Empty,
                                NoiDung = item.NoiDung,
                                NgayBatDau = item.NgayCoHieuLuc,
                                NgayKetThuc = item.NgayHetHieuLuc,
                                Status = 1,
                                CreatedBy = "admin",
                                IdTypeOfDocument = string.IsNullOrWhiteSpace(idTypeOfDocument) ? null : idTypeOfDocument,
                                ConcurrencyStamp = Guid.NewGuid().ToString(),
                                ExtraProperties = "{}",
                                Language = "vi",
                                CreationTime = DateTime.Now,
                                DetailLinkClone = item.DetailUrl,
                                IsDeleted = false,
                                SoKyHieu = item.SoKyHieu
                            };
                            dbVanBan.VanBanDuThaos.Add(duThao);
                            dbVanBan.SaveChanges();

                            // Tạo VBDTField
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
                                var vbdtField = new VBDTField
                                {
                                    Id = Guid.NewGuid(),
                                    IdLaw = duThao.Id,
                                    IdField = idField
                                };
                                dbVanBan.VBDTFields.Add(vbdtField);
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
                                        EntityId = duThao.Id,
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
                            errorList.Add($"- Tên dự thảo: {item.SoKyHieu}\n  Lỗi: {ex.Message}");
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

        private void btnPreview_Click(object sender, EventArgs e)
        {

        }
    }

    public class VanBanDuThaoItem
    {
        public string LoaiVanBan { get; set; }

        public string LinhVuc { get; set; }

        public string SoKyHieu { get; set; }

        public DateTime? NgayBanHanh { get; set; }

        public string DetailUrl { get; set; }

        public DateTime? NgayCoHieuLuc { get; set; }

        public DateTime? NgayHetHieuLuc { get; set; }

        public List<AttachmentFile> Attachments { get; set; } = new List<AttachmentFile>();

        public DateTime CreateAt {  get; set; } = DateTime.Now;

        public string NoiDung { get; set; }
    }
}


