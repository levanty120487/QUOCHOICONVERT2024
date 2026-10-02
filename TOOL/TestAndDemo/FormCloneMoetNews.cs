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
    public partial class FormCloneMoetNews : RJForms.RJChildForm
    {
        private List<BaiVietMoetItem> parsedData = new List<BaiVietMoetItem>();

        public FormCloneMoetNews()
        {
            InitializeComponent();
        }

        private Label lblStatus;

        private void FormCloneMoetNews_Load(object sender, EventArgs e)
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
            try
            {
                using (var db = new Portal_Moet_NewsServicesEntities()) // Giả sử tên context là Moet_NewsEntities, đổi lại nếu sai
                {
                    var categories = db.Categories.ToList();
                    var flatList = new List<CategoryDisplayItem>();
                    BuildCategoryTree(categories, "", "", flatList);

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

        private void BuildCategoryTree(IEnumerable<Category> allCategories, string parentIdStr, string prefix, List<CategoryDisplayItem> result)
        {
            // Lấy danh sách con dựa trên ParentID (chú ý: nếu parentIdStr rỗng thì lấy những Category không có ParentID)
            var children = allCategories.Where(c =>
                string.Equals(c.ParentID ?? "", parentIdStr ?? "", StringComparison.OrdinalIgnoreCase)
            ).OrderBy(a => a.Order).ToList();

            foreach (var child in children)
            {
                result.Add(new CategoryDisplayItem { Id = child.Id, Title = prefix + child.Title });
                BuildCategoryTree(allCategories, child.Id ?? "", prefix + "--- ", result);
            }

            // Fallback: Nếu không tìm thấy node gốc nào (do dữ liệu rác, không có ParentID rỗng), 
            // đẩy tất cả vào danh sách phẳng để không bị mất dữ liệu.
            if (string.IsNullOrEmpty(parentIdStr) && result.Count == 0)
            {
                foreach (var c in allCategories)
                {
                    result.Add(new CategoryDisplayItem { Id = c.Id, Title = c.Title });
                }
            }
        }

        private async void btnReadData_Click(object sender, EventArgs e)
        {
            try
            {
                parsedData.Clear();
                int totalPages = 1;
                int.TryParse(txtTotalPages.Text, out totalPages);
                string baseUrl = txtWebUrl.Text.Trim();

                string classList = string.IsNullOrWhiteSpace(txtClassList.Text) ? "//div[contains(@class, 'nav-item list-news-one')]//div[contains(@class, 'list-new')]" : txtClassList.Text.Trim();
                string classItem = string.IsNullOrWhiteSpace(txtClassItem.Text) ? ".//div[contains(@class, 'article-item')]" : txtClassItem.Text.Trim();
                string classTitle = string.IsNullOrWhiteSpace(txtClassTitle.Text) ? ".//div[contains(@class, 'right-type2')]//a" : txtClassTitle.Text.Trim();
                string classDate = string.IsNullOrWhiteSpace(txtClassDate.Text) ? ".//div[contains(@class, 'right-type2')]//div[contains(@class, 'article-date')]" : txtClassDate.Text.Trim();
                string classAvatar = string.IsNullOrWhiteSpace(txtClassAvatar.Text) ? ".//div[contains(@class, 'left-type2')]//div[contains(@class, 'post-image')]//img" : txtClassAvatar.Text.Trim();
                string classDesc = string.IsNullOrWhiteSpace(txtClassDescription.Text) ? "//div[contains(@class, 'article-brief')]" : txtClassDescription.Text.Trim();

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

        private async Task<List<BaiVietMoetItem>> ReadListPageAsync(string pageUrl, string classList, string classItem, string classTitle, string classDate, string classAvatar, string classDesc, int currentPageIndex)
        {
            var result = new List<BaiVietMoetItem>();
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

                        var titleNode = itemNode.SelectSingleNode(xPathTitle);
                        if (titleNode == null) continue;

                        string detailHref = titleNode.GetAttributeValue("href", "");
                        string detailUrl = ToAbsoluteUrl(pageUrl, detailHref);

                        //if (!detailUrl.Equals("https://moet.gov.vn/tintuc/Pages/tin-tong-hop.aspx%3FItemID=7393?categoryId=101914884")) continue;

                        var item = new BaiVietMoetItem();
                        item.DetailUrl = detailUrl;
                        item.Title = CleanText(titleNode.InnerText);

                        var dateNode = itemNode.SelectSingleNode(xPathDate);
                        if (dateNode != null)
                        {
                            // Có thể xử lý Regex để lấy đúng dd/MM/yyyy ở đây
                            string dateText = CleanText(dateNode.InnerText);
                            var match = Regex.Match(dateText, @"\d{2}/\d{2}/\d{4}");
                            if (match.Success)
                            {
                                if (DateTime.TryParseExact(match.Value, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out var dt))
                                {
                                    item.CreateDate = dt;
                                }
                            }
                        }

                        var imgNode = itemNode.SelectSingleNode(xPathAvatar);
                        if (imgNode != null)
                        {
                            item.AvatarUrl = ToAbsoluteUrl(pageUrl, imgNode.GetAttributeValue("src", ""));
                        }

                        // Tải trang chi tiết để bóc thông tin Description và các thông tin khác
                        await ParseDetailAsync(httpClient, item, detailUrl, classDesc);

                        result.Add(item);
                    }
                }
            }
            return result;
        }

        private async Task ParseDetailAsync(HttpClient httpClient, BaiVietMoetItem item, string detailUrl, string classDesc)
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

                // Bóc tiêu đề (có thể dùng class title truyền vào) - giữ lại nếu titleNode = null ở ngoài
                var titleNode = doc.DocumentNode.SelectSingleNode("//h1 | //h2[contains(@class,'title')]");
                if (titleNode != null && string.IsNullOrWhiteSpace(item.Title))
                    item.Title = CleanText(titleNode.InnerText);

                // Mô tả
                string xpathDesc = ConvertToXPath(classDesc, true);
                var briefNode = doc.DocumentNode.SelectSingleNode(xpathDesc);
                if (briefNode != null)
                    item.Description = briefNode.InnerText.Trim();

                // Nội dung
                var contentNode = doc.DocumentNode.SelectSingleNode("//div[contains(@class, 'content-detail')]");
                if (contentNode != null)
                    item.Content = contentNode.InnerHtml.Trim();

                // Tác giả
                var authorNode = doc.DocumentNode.SelectSingleNode("//div[contains(@class, 'author')]");
                if (authorNode != null)
                    item.Author = CleanText(authorNode.InnerText);

                // Lượt xem
                var viewNode = doc.DocumentNode.SelectSingleNode("//div[contains(@class, 'mt-10')]//i[contains(@class, 'vi vi-eye')]/..");
                if (viewNode == null)
                    viewNode = doc.DocumentNode.SelectSingleNode("//*[contains(@class, 'vi vi-eye')]");

                if (viewNode != null)
                {
                    string viewText = Regex.Replace(viewNode.InnerText, @"[^\d]", "");
                    if (int.TryParse(viewText, out int viewCount))
                        item.ViewCount = viewCount;
                }

                // File đính kèm
                var fileAttachNode = doc.DocumentNode.SelectSingleNode("//div[contains(@class, 'ul-fileattach')]");
                if (fileAttachNode != null)
                {
                    var itemNodes = fileAttachNode.SelectNodes(".//div[contains(@class, 'items')]");
                    if (itemNodes != null)
                    {
                        foreach (var iNode in itemNodes)
                        {
                            var spanName = iNode.SelectSingleNode(".//span[contains(@class, 'click-show')]");
                            var aDownload = iNode.SelectSingleNode(".//a[@download]") ?? iNode.SelectSingleNode(".//a[@href and not(starts-with(@href, 'javascript:'))]");

                            if (aDownload != null)
                            {
                                var href = aDownload.GetAttributeValue("href", "");
                                if (!string.IsNullOrWhiteSpace(href) && !href.ToLower().StartsWith("javascript:"))
                                {
                                    string fileName = "";
                                    if (spanName != null)
                                    {
                                        fileName = CleanText(spanName.InnerText).Replace("&nbsp;", " ").Replace("\u00A0", " ").Trim();
                                    }

                                    if (string.IsNullOrWhiteSpace(fileName))
                                    {
                                        fileName = GetFileNameFromUrl(href);
                                    }

                                    item.Attachments.Add(new AttachmentFile { FileName = fileName, Url = ToAbsoluteUrl(detailUrl, href) });
                                }
                            }
                        }
                    }
                    else
                    {
                        // Fallback cũ nếu không có cấu trúc div.items
                        var fileLinks = fileAttachNode.SelectNodes(".//a[@href]");
                        if (fileLinks != null)
                        {
                            foreach (var fNode in fileLinks)
                            {
                                var href = fNode.GetAttributeValue("href", "");
                                if (string.IsNullOrWhiteSpace(href) || href.ToLower().StartsWith("javascript:")) continue;
                                item.Attachments.Add(new AttachmentFile { FileName = CleanText(fNode.InnerText).Replace("\u00A0", " ").Trim(), Url = ToAbsoluteUrl(detailUrl, href) });
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
                MessageBox.Show("Vui lòng chọn danh mục!");
                return;
            }

            var categoryId = cboCategory.SelectedValue.ToString();
            if (string.IsNullOrWhiteSpace(categoryId))
            {
                MessageBox.Show("Vui lòng chọn danh mục!");
                return;
            }


            string downloadFolderImg = @"C:\uploadFckFiles\tinhdBGD";
            Directory.CreateDirectory(downloadFolderImg);

            string originalBtnText = btnSaveData.Text;
            btnSaveData.Enabled = false;
            lblStatus.Text = "Bắt đầu lưu dữ liệu...";

            try
            {
                using (var dbNews = new Portal_Moet_NewsServicesEntities())
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

                            // nếu tồn tại thì không lưu mà tiếp bản ghi khác
                            var findItem = await dbNews.News
                                                    .Where(a => a.DetailUrlClone.Equals(item.DetailUrl))
                                                    .FirstOrDefaultAsync();
                            if(findItem != null 
                                && !string.IsNullOrWhiteSpace(findItem.Id))
                            {
                                duplicateList.Add($"- Tên (Title): {item.Title}\r\n  Link (DetailUrl): {item.DetailUrl}");
                                continue;
                            }    

                            // Insert vào bảng New
                            var newEntity = new New()
                            {
                                Id = QHCommons.GenAutoId(),
                                Title = item.Title,
                                ConcurrencyStamp = Guid.NewGuid().ToString(),
                                DatePublic = item.CreateDate,
                                Status = 6,
                                Author = item.Author,
                                ReadCount = item.ViewCount,
                                CreationTime = DateTime.Now,
                                Description = item.Description,
                                DescriptionSEO = item.Description,
                                PageTitleSEO = item.Title,
                                Hot = false,
                                Source = string.Empty,
                                Shared = true,
                                Language = "vi",
                                ExtraProperties = "{}",
                                OldId = string.IsNullOrEmpty(item.OldId) ? string.Empty : item.OldId,
                                TypeNewContent = 3, //=> tin bai
                                TypeNewId = Guid.Parse("69E83D8B-3C4B-43AA-B554-B6B23C935718"), //=> tin bai
                                CreatorName = "admin",
                                ShowDescription = true,
                                AllowComment = false,
                                PageUrlSEO = GenerateSlug(item.Title),
                                DetailUrlClone = item.DetailUrl
                            };
                            // Tải ảnh bất đồng bộ
                            if (!string.IsNullOrEmpty(item.AvatarUrl))
                            {
                                newEntity.Image = await ImageDownloader.DownloadImageAsyncurl(item.AvatarUrl.Trim());
                            }
                            // Xử lý ảnh trong nội dung tin
                            if (!string.IsNullOrEmpty(item.Content))
                            {
                                HtmlAgilityPack.HtmlDocument newsDocument = new HtmlAgilityPack.HtmlDocument();
                                newsDocument.LoadHtml(item.Content);
                                var imagesNode = newsDocument.DocumentNode.SelectNodes(".//img");
                                if (imagesNode != null)
                                {
                                    foreach (var img in imagesNode)
                                    {
                                        if (img.Attributes["src"] != null)
                                        {
                                            img.Attributes["src"].Value = img.Attributes["src"].Value.Replace("https&#58;//", "https://").Replace("http&#58;//", "http://");
                                            img.Attributes["src"].Value = await ImageDownloader.DownloadImageAsyncurl(img.Attributes["src"].Value.Trim());
                                        }
                                    }
                                }
                                newEntity.Content = newsDocument.DocumentNode.OuterHtml;
                            }
                            dbNews.News.Add(newEntity);
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
                            dbFiles.SaveChanges();

                            #region NewCategory
                            var newCategory = new NewCategory()
                            {
                                CategoryId = categoryId,
                                Id = Guid.NewGuid(),
                                NewId = newEntity.Id
                            };
                            dbNews.NewCategories.Add(newCategory);
                            dbNews.SaveChanges();
                            #endregion
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

                    // Handle exist file
                    int count = 1;
                    string nameOnly = Path.GetFileNameWithoutExtension(fileName);
                    while (File.Exists(fullPath))
                    {
                        fullPath = Path.Combine(folder, $"{nameOnly}_{count++}{ext}");
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

        private static string GenerateSlug(string title)
        {
            if (string.IsNullOrWhiteSpace(title)) return "bai-viet";
            string str = title.ToLowerInvariant();
            str = str.Replace("đ", "d");
            var withoutMarks = new string(str
                .Normalize(System.Text.NormalizationForm.FormD)
                .Where(ch => System.Globalization.CharUnicodeInfo.GetUnicodeCategory(ch) != System.Globalization.UnicodeCategory.NonSpacingMark)
                .ToArray());
            var slug = System.Text.RegularExpressions.Regex.Replace(withoutMarks, "[^a-z0-9]+", "-").Trim('-');
            return string.IsNullOrWhiteSpace(slug) ? "bai-viet" : slug;
        }
    }

    public class CategoryDisplayItem
    {
        public object Id { get; set; }
        public string Title { get; set; }
    }

    public class AttachmentFile
    {
        public string FileName { get; set; }
        public string Url { get; set; }
    }

    public class BaiVietMoetItem
    {
        public string Title { get; set; } = "";
        public string DetailUrl { get; set; } = "";
        public string AvatarUrl { get; set; } = "";
        public string Description { get; set; } = "";
        public string Content { get; set; } = "";
        public string Author { get; set; } = "";
        public int ViewCount { get; set; } = 0;
        public List<AttachmentFile> Attachments { get; set; } = new List<AttachmentFile>();

        // Thuộc tính để DataGridView tự động bind và hiển thị thông tin các file đính kèm
        public string FilesDisplay => Attachments != null && Attachments.Count > 0 ? string.Join(", ", Attachments.Select(x => x.FileName)) : "";

        public DateTime? CreateDate { get; set; }

        public string OldId { get; set; }

        public DateTime CreateAt { get; set; } = DateTime.Now;
    }
}
