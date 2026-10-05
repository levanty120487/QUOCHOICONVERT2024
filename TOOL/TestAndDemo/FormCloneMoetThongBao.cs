using HtmlAgilityPack;
using QHBASE;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RJCodeUI_M1.TestAndDemo
{
    public partial class FormCloneMoetThongBao : RJForms.RJChildForm
    {
        private List<ThongBaoMoetItem> parsedData = new List<ThongBaoMoetItem>();

        public FormCloneMoetThongBao()
        {
            InitializeComponent();
        }

        private Label lblStatus;

        private void FormCloneMoetThongBao_Load(object sender, EventArgs e)
        {
            lblStatus = new Label();
            lblStatus.AutoSize = true;
            lblStatus.ForeColor = System.Drawing.Color.Red;
            lblStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            lblStatus.Location = new System.Drawing.Point(20, 150);
            lblStatus.Text = "";
            pnlClientArea.Controls.Add(lblStatus);

            // Bố cục lại các control giữ lại
            label2.Location = new System.Drawing.Point(20, 20);      // Web URL
            txtWebUrl.Location = new System.Drawing.Point(120, 17);
            txtWebUrl.Width = 500;

            label12.Location = new System.Drawing.Point(20, 60);     // Folder chứa
            txtFolderChua.Location = new System.Drawing.Point(120, 57);
            txtFolderChua.Width = 500;

            label1.Location = new System.Drawing.Point(20, 100);     // Danh mục
            cboCategory.Location = new System.Drawing.Point(120, 97);
            cboCategory.Width = 500;

            btnReadData.Location = new System.Drawing.Point(20, 180);
            btnSaveData.Location = new System.Drawing.Point(130, 180);

            dgvData.Location = new System.Drawing.Point(20, 220);
            dgvData.Size = new System.Drawing.Size(900, 500);

            LoadCategories();
        }

        private void LoadCategories()
        {
            try
            {
                using (var db = new Portal_Moet_CommonsServicesEntities())
                {
                    var categories = db.LoaiThongBaos.ToList();
                    var flatList = new List<CategoryDisplayItem>();
                    foreach (var c in categories)
                    {
                        flatList.Add(new CategoryDisplayItem { Id = c.Id, Title = c.Title });
                    }

                    cboCategory.DataSource = flatList;
                    cboCategory.DisplayMember = "Title";
                    cboCategory.ValueMember = "Id";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private async void btnReadData_Click(object sender, EventArgs e)
        {
            try
            {
                parsedData.Clear();
                int totalPages = 1;
                string baseUrl = txtWebUrl.Text.Trim();

                string classList = "//div[contains(@class, 'table-responsive')]//table[contains(@class, 'table-cms-v2')]//tbody";
                string classItem = ".//tr";
                string classTitle = ".//td[contains(@class, 'tg-yw4l')]//a";
                string classDate = ".//td[contains(@class, 'text-center')]";
                string classAvatar = "";
                string classDesc = "";

                string originalBtnText = btnReadData.Text;
                btnReadData.Enabled = false;
                lblStatus.Text = "Bắt đầu đọc dữ liệu...";

                int startPage = 1;
                int endPage = totalPages;

                for (int i = startPage; i <= endPage; i++)
                {
                    string pageUrl = baseUrl;
                    if (i > 1)
                    {
                        pageUrl = $"{baseUrl}?&orderBy=publishTime DESC&itemsPerPage=10&pageNo={i}";
                    }

                    var items = await ReadListPageAsync(pageUrl, classList, classItem, classTitle, classDate, classAvatar, classDesc, i);
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

        private string ConvertToXPath(string input, bool isRoot)
        {
            if (string.IsNullOrWhiteSpace(input)) return "";
            if (input.StartsWith("/") || input.StartsWith("./")) return input;

            // Xử lý cú pháp pseudo (ví dụ: div[class='article-item'])
            string xpath = input.Replace("[class=", "[@class=");
            if (isRoot)
            {
                if (!xpath.StartsWith("//")) xpath = "//" + xpath;
            }
            else
            {
                if (!xpath.StartsWith(".//")) xpath = ".//" + xpath;
            }
            return xpath;
        }

        private async Task<List<ThongBaoMoetItem>> ReadListPageAsync(string pageUrl, string classList, string classItem, string classTitle, string classDate, string classAvatar, string classDesc, int currentPageIndex)
        {
            var result = new List<ThongBaoMoetItem>();
            using (var httpClient = new HttpClient())
            {
                httpClient.Timeout = TimeSpan.FromSeconds(60);
                string html = await httpClient.GetStringAsync(pageUrl);
                var doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(html);

                string xPathList = ConvertToXPath(classList, true);
                var listNodes = doc.DocumentNode.SelectNodes(xPathList);
                if (listNodes == null) return result;

                string xPathItem = ConvertToXPath(classItem, false);
                string xPathTitle = ConvertToXPath(classTitle, false);
                string xPathDate = ConvertToXPath(classDate, false);
                string xPathAvatar = ConvertToXPath(classAvatar, false);
                string xPathDesc = ConvertToXPath(classDesc, false);

                int itemIndex = 0;
                foreach (var listNode in listNodes)
                {
                    var itemNodes = listNode.SelectNodes(xPathItem);
                    if (itemNodes == null)
                    {
                        itemNodes = new HtmlAgilityPack.HtmlNodeCollection(listNode);
                        itemNodes.Add(listNode);
                    }

                    foreach (var itemNode in itemNodes)
                    {
                        itemIndex++;
                        lblStatus.Text = $"Đã đọc trang {currentPageIndex}/ bản ghi thứ {itemIndex}";
                        Application.DoEvents();

                        var tds = itemNode.SelectNodes(".//td");
                        if (tds != null && tds.Count >= 4)
                        {
                            var tNode = tds[1].SelectSingleNode(".//a");
                            if (tNode != null)
                            {
                                var item = new ThongBaoMoetItem();
                                item.Title = CleanText(tNode.InnerText);
                                string detailHref = tNode.GetAttributeValue("href", "");
                                string detailUrl = ToAbsoluteUrl(pageUrl, detailHref);
                                item.DetailUrl = detailUrl;
                                
                                var dateText = CleanText(tds[2].InnerText);
                                var match = Regex.Match(dateText, @"\d{2}/\d{2}/\d{4}");
                                if (match.Success)
                                {
                                    if (DateTime.TryParseExact(match.Value, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out var dt))
                                    {
                                        item.CreateDate = dt;
                                    }
                                }

                                var fileNode = tds[3].SelectSingleNode(".//a");
                                if (fileNode != null)
                                {
                                    var href = fileNode.GetAttributeValue("href", "");
                                    if (!string.IsNullOrWhiteSpace(href))
                                    {
                                        string fileName = href.Contains("/") ? href.Substring(href.LastIndexOf('/') + 1) : href;
                                        fileName = fileName.Contains("?") ? fileName.Substring(0, fileName.IndexOf('?')) : fileName;
                                        if (string.IsNullOrWhiteSpace(fileName)) fileName = "unknown_file";

                                        item.Attachments.Add(new AttachmentFile { FileName = fileName, Url = ToAbsoluteUrl(pageUrl, href) });
                                    }
                                }
                                
                                await ParseDetailAsync(httpClient, item, detailUrl, classDesc);
                                result.Add(item);
                            }
                        }
                    }
                }
            }
            return result;
        }

        private async Task ParseDetailAsync(HttpClient httpClient, ThongBaoMoetItem item, string detailUrl, string classDesc)
        {
            try
            {
                string html = await httpClient.GetStringAsync(detailUrl);
                var doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(html);

                // Lấy toàn bộ tham số từ dấu ? trở đi
                string decodedUrl = System.Net.WebUtility.UrlDecode(detailUrl);
                int queryIndex = decodedUrl.IndexOf('?');
                if (queryIndex >= 0)
                {
                    item.OldId = decodedUrl.Substring(queryIndex);
                }

                // Lượt xem
                var viewNode = doc.DocumentNode.SelectSingleNode("//div[contains(@class, 'news-date')]");
                if (viewNode != null)
                {
                    string innerText = viewNode.InnerText;
                    var match = Regex.Match(innerText, @"Lượt xem:\s*([\d\.,]+)", RegexOptions.IgnoreCase);
                    if (match.Success)
                    {
                        string viewText = Regex.Replace(match.Groups[1].Value, @"[^\d]", "");
                        if (int.TryParse(viewText, out int viewCount))
                            item.ViewCount = viewCount;
                    }
                    else
                    {
                        // Fallback: tìm text ngay sau icon mắt
                        var iEyeNode = viewNode.SelectSingleNode(".//i[contains(@class, 'vi-eye')]");
                        if (iEyeNode != null && iEyeNode.NextSibling != null)
                        {
                            string textNextToEye = iEyeNode.NextSibling.InnerText;
                            string viewText = Regex.Replace(textNextToEye, @"[^\d]", "");
                            if (int.TryParse(viewText, out int viewCount))
                                item.ViewCount = viewCount;
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
                MessageBox.Show("Vui lòng chọn danh mục!");
                return;
            }

            var categoryId = cboCategory.SelectedValue.ToString();
            if (string.IsNullOrWhiteSpace(categoryId))
            {
                MessageBox.Show("Vui lòng chọn danh mục!");
                return;
            }


            string downloadFolderImg = @"C:\uploadFckFiles\thongtinbaochi";
            if(!string.IsNullOrWhiteSpace(txtFolderChua.Text))
            {
                downloadFolderImg = string.Concat(downloadFolderImg, "\\", txtFolderChua.Text.Trim());
            }    
            Directory.CreateDirectory(downloadFolderImg);

            string originalBtnText = btnSaveData.Text;
            btnSaveData.Enabled = false;
            lblStatus.Text = "Bắt đầu lưu dữ liệu...";

            try
            {
                using (var dbNews = new Portal_Moet_CommonsServicesEntities())
                using (var dbFiles = new Portal_Moet_FilesServicesEntities())
                using (var httpClient = new HttpClient())
                {
                    int currentIndex = 0;
                    int totalItems = parsedData.Count;
                    var errorList = new List<string>();
                    var duplicateList = new List<string>();

                    foreach (var item in parsedData.OrderByDescending(a => a.CreateAt))
                    {
                        try
                        {
                            currentIndex++;
                            lblStatus.Text = $"Đang lưu bản ghi thứ {currentIndex}/{totalItems}...";
                            Application.DoEvents();

                            var findItem = await dbNews.DanhSachThongBaos
                                                    .Where(a => a.DetailLinkClone.Equals(item.DetailUrl))
                                                    .FirstOrDefaultAsync();
                            if(findItem != null && !string.IsNullOrWhiteSpace(findItem.Id))
                            {
                                duplicateList.Add($"- Tên (Title): {item.Title}\r\n  Link (DetailUrl): {item.DetailUrl}");
                                continue;
                            }    

                            // Insert vào bảng DanhSachThongBao
                            var newEntity = new DanhSachThongBao()
                            {
                                Id = QHCommons.GenAutoId(),
                                TieuDe = item.Title,
                                MoTa = string.Empty,
                                ConcurrencyStamp = Guid.NewGuid().ToString(),
                                CreationTime = DateTime.Now,
                                NgayThongBao = item.CreateDate,
                                CreatedBy = "admin",
                                ViewCount = item.ViewCount,
                                IdLoaiThongBao = categoryId,
                                Status = 6,
                                Language = "vi",
                                ExtraProperties = "{}",
                                DetailLinkClone = item.DetailUrl
                            };
                            
                            dbNews.DanhSachThongBaos.Add(newEntity);
                            dbNews.SaveChanges();

                            // Tải file đính kèm & Insert vào Moet_Files
                            foreach (var attach in item.Attachments)
                            {
                                string fileNameToDownload = string.IsNullOrWhiteSpace(attach.FileName) ? GetFileNameFromUrl(attach.Url) : attach.FileName;
                                string localFilePath = await DownloadFileAsync(httpClient, attach.Url, downloadFolderImg, fileNameToDownload);

                                if (!string.IsNullOrEmpty(localFilePath))
                                {
                                    using (var files = new Portal_Moet_FilesServicesEntities())
                                    {
                                        var file = new CMSFile()
                                        {
                                            Id = Guid.NewGuid(),
                                            FileType = 8,
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
                                        files.CMSFiles.Add(file);
                                        files.SaveChanges();
                                        #region Insert fileAttachment
                                        var fileAttach = new CMSFileAttachment()
                                        {
                                            Id = Guid.NewGuid(),
                                            EntityId = newEntity.Id,
                                            FileId = file.Id,
                                            ExtraProperties = "{}",
                                            ConcurrencyStamp = Guid.NewGuid().ToString(),
                                            CreationTime = DateTime.Now,
                                            FileAttachmentType = 1
                                        };
                                        files.CMSFileAttachments.Add(fileAttach);
                                        files.SaveChanges();
                                        #endregion
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            errorList.Add($"- Tên (Title): {item.Title}\n  Link (DetailUrl): {item.DetailUrl}\n  Lỗi: {ex.Message}");
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
            return HtmlEntity.DeEntitize(text).Trim();
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

                    fileName = Regex.Replace(fileName ?? "", @"[^a-zA-Z0-9.\s]", "").Replace(" ", "_");
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

        private void pnlClientArea_Paint(object sender, PaintEventArgs e)
        {

        }

        public static string GetMimeType(string fileName)
        {
            string ext = Path.GetExtension(fileName).ToLower();

            switch (ext)
            {
                case ".pdf":
                    return "application/pdf";
                case ".doc":
                    return "application/msword";
                case ".docx":
                    return "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                case ".xls":
                    return "application/vnd.ms-excel";
                case ".xlsx":
                    return "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                case ".png":
                    return "image/png";
                case ".jpg":
                case ".jpeg":
                    return "image/jpeg";
                case ".gif":
                    return "image/gif";
                case ".txt":
                    return "text/plain";
                case ".zip":
                    return "application/zip";
                default:
                    return "application/octet-stream";
            }
        }

        public class ThongBaoMoetItem
        {
            public string Title { get; set; } = "";

            public string DetailUrl { get; set; } = "";

            public int ViewCount { get; set; } = 0;

            public List<AttachmentFile> Attachments { get; set; } = new List<AttachmentFile>();

            // Thuộc tính để DataGridView tự động bind và hiển thị thông tin các file đính kèm
            public string FilesDisplay => Attachments != null && Attachments.Count > 0 ? string.Join(", ", Attachments.Select(x => x.FileName)) : "";

            public string OldId { get; set; }

            public DateTime CreateAt { get; set; } = DateTime.Now;

            public DateTime? CreateDate { get; set; }
        }
    }
}
