using HtmlAgilityPack;
using QHBASE;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RJCodeUI_M1.TestAndDemo
{
    public partial class FormCloneVioitNews : RJForms.RJChildForm
    {
        private List<BaiVietVioitItem> parsedData = new List<BaiVietVioitItem>();
        private Label lblStatus;

        public FormCloneVioitNews()
        {
            InitializeComponent();
        }

        private void FormCloneVioitNews_Load(object sender, EventArgs e)
        {
            lblStatus = new Label();
            lblStatus.AutoSize = true;
            lblStatus.ForeColor = System.Drawing.Color.Red;
            lblStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            lblStatus.Location = new System.Drawing.Point(20, 168);
            lblStatus.Text = "";
            pnlClientArea.Controls.Add(lblStatus);

            // Gán giá trị mặc định nếu rỗng
            if (string.IsNullOrWhiteSpace(txtWebUrl.Text))
                txtWebUrl.Text = "https://vioit.moit.gov.vn/vn/tin-hoat-dong-vien.4048.html";
            if (string.IsNullOrWhiteSpace(txtClassList.Text))
                txtClassList.Text = "//table[contains(@id, 'DataList1')]";
            if (string.IsNullOrWhiteSpace(txtClassItem.Text))
                txtClassItem.Text = ".//tr";
            if (string.IsNullOrWhiteSpace(txtClassTitle.Text))
                txtClassTitle.Text = ".//div[contains(@class, 'col-lg-8')]//a";
            if (string.IsNullOrWhiteSpace(txtClassDate.Text))
                txtClassDate.Text = ".//i[contains(@class, 'fa-calendar')]/..";
            if (string.IsNullOrWhiteSpace(txtClassAvatar.Text))
                txtClassAvatar.Text = ".//img";
            if (string.IsNullOrWhiteSpace(txtClassDescription.Text))
                txtClassDescription.Text = ".//div[contains(@style, 'font-weight:400')]";
            if (string.IsNullOrWhiteSpace(txtFolderChua.Text))
                txtFolderChua.Text = "vioit";

            LoadCategories();
        }

        private void LoadCategories()
        {
            try
            {
                using (var db = new Portal_Vioit_NewsServicesEntities())
                {
                    var categories = db.Categories.ToList();
                    if (!string.IsNullOrWhiteSpace(txtSite.Text))
                    {
                        categories = categories.Where(a => !string.IsNullOrWhiteSpace(a.Site) && a.Site.Equals(txtSite.Text.Trim())).ToList();
                    }
                    if (!string.IsNullOrWhiteSpace(txtLanguage.Text))
                    {
                        categories = categories.Where(a => !string.IsNullOrWhiteSpace(a.Language) && a.Language.Equals(txtLanguage.Text.Trim())).ToList();
                    }
                    var flatList = new List<CategoryDisplayItem>();
                    BuildCategoryTree(categories, "", "", flatList);

                    cboCategory.DataSource = flatList;
                    cboCategory.DisplayMember = "Title";
                    cboCategory.ValueMember = "Id";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi nạp danh mục: " + ex.Message);
            }
        }

        private void BuildCategoryTree(IEnumerable<Category> allCategories, string parentIdStr, string prefix, List<CategoryDisplayItem> result)
        {
            var children = allCategories.Where(c =>
                string.Equals(c.ParentID ?? "", parentIdStr ?? "", StringComparison.OrdinalIgnoreCase)
            ).OrderBy(a => a.Order).ToList();

            foreach (var child in children)
            {
                result.Add(new CategoryDisplayItem { Id = child.Id, Title = prefix + child.Title });
                BuildCategoryTree(allCategories, child.Id ?? "", prefix + "--- ", result);
            }

            if (string.IsNullOrEmpty(parentIdStr) && result.Count == 0)
            {
                foreach (var c in allCategories)
                {
                    result.Add(new CategoryDisplayItem { Id = c.Id, Title = c.Title });
                }
            }
        }

        private string GetPageUrl(string baseUrl, int pageIndex)
        {
            if (string.IsNullOrWhiteSpace(baseUrl)) return "";

            // Nếu đã có tham số Page=\d+ trên URL thì thay thế
            if (Regex.IsMatch(baseUrl, @"([?&]Page=)\d+", RegexOptions.IgnoreCase))
            {
                return Regex.Replace(baseUrl, @"([?&]Page=)\d+", $"${{1}}{pageIndex}", RegexOptions.IgnoreCase);
            }

            // Trang đầu tiên giữ nguyên URL gốc
            if (pageIndex <= 1)
            {
                return baseUrl;
            }

            // Nhảy page thì thêm ?Page={pageIndex} hoặc &Page={pageIndex}
            return baseUrl.Contains("?") ? $"{baseUrl}&Page={pageIndex}" : $"{baseUrl}?Page={pageIndex}";
        }

        private async void btnReadData_Click(object sender, EventArgs e)
        {
            try
            {
                parsedData.Clear();
                int totalPages = 1;
                int.TryParse(txtTotalPages.Text, out totalPages);
                string baseUrl = txtWebUrl.Text.Trim();

                string classList = string.IsNullOrWhiteSpace(txtClassList.Text) ? "//table[contains(@id, 'DataList1')]" : txtClassList.Text.Trim();
                string classItem = string.IsNullOrWhiteSpace(txtClassItem.Text) ? ".//tr" : txtClassItem.Text.Trim();
                string classTitle = string.IsNullOrWhiteSpace(txtClassTitle.Text) ? ".//div[contains(@class, 'col-lg-8')]//a" : txtClassTitle.Text.Trim();
                string classDate = string.IsNullOrWhiteSpace(txtClassDate.Text) ? ".//i[contains(@class, 'fa-calendar')]/.." : txtClassDate.Text.Trim();
                string classAvatar = string.IsNullOrWhiteSpace(txtClassAvatar.Text) ? ".//img" : txtClassAvatar.Text.Trim();
                string classDesc = string.IsNullOrWhiteSpace(txtClassDescription.Text) ? ".//div[contains(@style, 'font-weight:400')]" : txtClassDescription.Text.Trim();

                string originalBtnText = btnReadData.Text;
                btnReadData.Enabled = false;
                lblStatus.Text = "Bắt đầu đọc dữ liệu VIOIT...";

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
                    string pageUrl = GetPageUrl(baseUrl, i);
                    lblStatus.Text = $"Đang đọc trang {i}/{endPage}...";
                    Application.DoEvents();

                    var items = await ReadListPageAsync(pageUrl, classList, classItem, classTitle, classDate, classAvatar, classDesc, i);
                    if (items != null)
                        parsedData.AddRange(items);
                }

                btnReadData.Text = originalBtnText;
                btnReadData.Enabled = true;
                lblStatus.Text = $"Hoàn tất! Đã đọc {parsedData.Count} bài viết từ VIOIT.";

                dgvData.DataSource = null;
                dgvData.DataSource = parsedData;
                MessageBox.Show($"Đã đọc xong {parsedData.Count} bài viết VIOIT.", "Thông báo");
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

        private async Task<List<BaiVietVioitItem>> ReadListPageAsync(string pageUrl, string classList, string classItem, string classTitle, string classDate, string classAvatar, string classDesc, int currentPageIndex)
        {
            var result = new List<BaiVietVioitItem>();
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
                        var titleNode = itemNode.SelectSingleNode(xPathTitle);
                        if (titleNode == null) continue;

                        itemIndex++;
                        lblStatus.Text = $"Trang {currentPageIndex} - Đang đọc bài {itemIndex}: {CleanText(titleNode.InnerText)}";
                        Application.DoEvents();

                        string detailHref = titleNode.GetAttributeValue("href", "");
                        string detailUrl = ToAbsoluteUrl(pageUrl, detailHref);

                        var item = new BaiVietVioitItem();
                        item.DetailUrl = detailUrl;
                        item.Title = CleanText(titleNode.InnerText);

                        // Ngày tạo bài viết
                        var dateNode = itemNode.SelectSingleNode(xPathDate);
                        if (dateNode != null)
                        {
                            string dateText = CleanText(dateNode.InnerText);
                            var match = Regex.Match(dateText, @"\d{1,2}/\d{1,2}/\d{4}");
                            if (match.Success)
                            {
                                if (DateTime.TryParseExact(match.Value, "dd/MM/yyyy", null, DateTimeStyles.None, out var dt) ||
                                    DateTime.TryParseExact(match.Value, "d/M/yyyy", null, DateTimeStyles.None, out dt))
                                {
                                    item.CreateDate = dt;
                                }
                            }
                        }

                        // Ảnh đại diện
                        var imgNode = itemNode.SelectSingleNode(xPathAvatar);
                        if (imgNode != null)
                        {
                            item.AvatarUrl = ToAbsoluteUrl(pageUrl, imgNode.GetAttributeValue("src", ""));
                        }

                        // Mô tả ngắn ở danh sách (nếu có)
                        var desc = itemNode.SelectSingleNode(xPathDesc);
                        if (desc != null && !string.IsNullOrWhiteSpace(desc.InnerText))
                        {
                            item.Description = CleanText(desc.InnerText);
                        }

                        // Đọc trang chi tiết để lấy nội dung bài viết và các thông tin khác
                        await ParseDetailAsync(httpClient, item, detailUrl, classDesc);

                        result.Add(item);
                    }
                }
            }
            return result;
        }

        private async Task ParseDetailAsync(HttpClient httpClient, BaiVietVioitItem item, string detailUrl, string classDesc)
        {
            try
            {
                string html = await httpClient.GetStringAsync(detailUrl);
                var doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(html);

                // Lấy ID cũ từ link chi tiết (dạng: ...-7312.4048.html)
                var idMatch = Regex.Match(detailUrl, @"-(\d+)\.\d+\.html");
                if (idMatch.Success)
                {
                    item.OldId = idMatch.Groups[1].Value;
                }
                else
                {
                    var anyMatch = Regex.Match(detailUrl, @"-(\d+)\.html");
                    if (anyMatch.Success)
                    {
                        item.OldId = anyMatch.Groups[1].Value;
                    }
                    else
                    {
                        item.OldId = Path.GetFileNameWithoutExtension(detailUrl);
                    }
                }

                // Tiêu đề (nếu chưa có hoặc muốn lấy lại từ h1 chi tiết)
                var titleNode = doc.DocumentNode.SelectSingleNode("//div[contains(@class, 'article-title')]//h1 | //h1");
                if (titleNode != null && string.IsNullOrWhiteSpace(item.Title))
                    item.Title = CleanText(titleNode.InnerText);

                // Mô tả tóm tắt (article-lead)
                var leadNode = doc.DocumentNode.SelectSingleNode("//div[contains(@class, 'article-lead')]");
                if (leadNode != null && !string.IsNullOrWhiteSpace(leadNode.InnerText))
                {
                    string leadText = CleanText(leadNode.InnerText);
                    if (!string.IsNullOrWhiteSpace(leadText))
                        item.Description = leadText;
                }

                // Nội dung bài viết từ thẻ div.article-content
                var contentNode = doc.DocumentNode.SelectSingleNode("//div[contains(@class, 'article-content')]");
                if (contentNode != null)
                {
                    item.Content = contentNode.InnerHtml.Trim();

                    // Tác giả ở cuối bài viết (thường căn phải dạng ThS. Thái Quốc Hải / VIOIT)
                    var authorNode = contentNode.SelectSingleNode(".//p[contains(@style, 'text-align: right') or contains(@style, 'text-align:right')]//strong");
                    if (authorNode != null && !string.IsNullOrWhiteSpace(authorNode.InnerText))
                    {
                        item.Author = CleanText(authorNode.InnerText);
                    }
                }

                // Tác giả fallback
                if (string.IsNullOrWhiteSpace(item.Author))
                {
                    var authorDiv = doc.DocumentNode.SelectSingleNode("//div[contains(@class, 'author')]");
                    if (authorDiv != null)
                        item.Author = CleanText(authorDiv.InnerText);
                }

                // Ngày xuất bản chi tiết (có thể kèm giờ phút)
                var dateNodeDetail = doc.DocumentNode.SelectSingleNode("//div[contains(@style, 'bababa')] | //i[contains(@class, 'fa-calendar')]/..");
                if (dateNodeDetail != null)
                {
                    string dateText = CleanText(dateNodeDetail.InnerText);
                    var dateMatch = Regex.Match(dateText, @"(?<day>\d{1,2})/(?<month>\d{1,2})/(?<year>\d{4})");
                    var timeMatch = Regex.Match(dateText, @"(?<hour>\d{1,2}):(?<minute>\d{2})(?::(?<second>\d{2}))?");

                    if (dateMatch.Success)
                    {
                        int day = int.Parse(dateMatch.Groups["day"].Value);
                        int month = int.Parse(dateMatch.Groups["month"].Value);
                        int year = int.Parse(dateMatch.Groups["year"].Value);

                        int hour = 0, minute = 0, second = 0;
                        if (timeMatch.Success)
                        {
                            int.TryParse(timeMatch.Groups["hour"].Value, out hour);
                            int.TryParse(timeMatch.Groups["minute"].Value, out minute);
                            if (timeMatch.Groups["second"].Success)
                            {
                                int.TryParse(timeMatch.Groups["second"].Value, out second);
                            }
                        }
                        else if (item.CreateDate.HasValue)
                        {
                            hour = item.CreateDate.Value.Hour;
                            minute = item.CreateDate.Value.Minute;
                            second = item.CreateDate.Value.Second;
                        }

                        try
                        {
                            item.CreateDate = new DateTime(year, month, day, hour, minute, second);
                        }
                        catch { }
                    }
                }

                // File đính kèm trong bài viết nếu có link tài liệu
                if (!string.IsNullOrEmpty(item.Content))
                {
                    HtmlAgilityPack.HtmlDocument contentDoc = new HtmlAgilityPack.HtmlDocument();
                    contentDoc.LoadHtml(item.Content);
                    var fileLinks = contentDoc.DocumentNode.SelectNodes(".//a[@href]");
                    if (fileLinks != null)
                    {
                        string[] documentExtensions = { ".pdf", ".doc", ".docx", ".xls", ".xlsx", ".ppt", ".pptx", ".zip", ".rar", ".7z", ".txt", ".mp4" };
                        foreach (var a in fileLinks)
                        {
                            string href = a.GetAttributeValue("href", "");
                            if (string.IsNullOrWhiteSpace(href) || href.ToLower().StartsWith("javascript:")) continue;
                            string cleanUrl = href.Split('?')[0].ToLower();
                            if (documentExtensions.Any(ext => cleanUrl.EndsWith(ext)))
                            {
                                string fileName = CleanText(a.InnerText).Trim();
                                if (string.IsNullOrWhiteSpace(fileName))
                                    fileName = GetFileNameFromUrl(href);
                                item.Attachments.Add(new AttachmentFile { FileName = fileName, Url = ToAbsoluteUrl(detailUrl, href) });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Lỗi đọc chi tiết bài viết: " + ex.Message);
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

            string downloadFolderImg = @"C:\uploadFckFiles\updatenews";
            if (!string.IsNullOrWhiteSpace(txtFolderChua.Text))
            {
                downloadFolderImg = Path.Combine(downloadFolderImg, txtFolderChua.Text.Trim());
            }
            Directory.CreateDirectory(downloadFolderImg);

            string originalBtnText = btnSaveData.Text;
            btnSaveData.Enabled = false;
            lblStatus.Text = "Bắt đầu lưu dữ liệu VIOIT...";
            var temp = new New();
            try
            {
                using (var dbNews = new Portal_Vioit_NewsServicesEntities())
                using (var dbFiles = new Portal_Vioit_FilesServicesEntities())
                using (var httpClient = new HttpClient())
                {
                    int currentIndex = 0;
                    int totalItems = parsedData.Count;
                    var errorList = new List<string>();
                    var duplicateList = new List<string>();

                    var defaultType = dbNews.TypeNews.FirstOrDefault();
                    Guid defaultTypeNewId = defaultType != null ? defaultType.Id : Guid.Parse("87EDEB17-D757-4F71-8B22-4F734CD325FD");

                    foreach (var item in parsedData.OrderByDescending(a => a.CreateAt))
                    {
                        try
                        {
                            currentIndex++;
                            lblStatus.Text = $"Đang lưu bản ghi thứ {currentIndex}/{totalItems}: {item.Title}";
                            Application.DoEvents();

                            // Kiểm tra trùng theo Title hoặc DetailUrlClone
                            var findItem = await dbNews.News
                                                    .Where(a => (a.Title.Equals(item.Title) || a.DetailUrlClone.Equals(item.DetailUrl)) && !a.IsDeleted)
                                                    .Include(a => a.NewCategories)
                                                    .FirstOrDefaultAsync();

                            if (findItem != null && !string.IsNullOrWhiteSpace(findItem.Id))
                            {
                                #region Update Category
                                try
                                {
                                    if (findItem.NewCategories != null && findItem.NewCategories.Count > 0)
                                    {
                                        var findCategorys = findItem.NewCategories.Where(a => a.CategoryId.Equals(categoryId)).ToList();
                                        if (findCategorys != null && findCategorys.Count > 0)
                                        {
                                            duplicateList.Add($"- Tên (Title): {item.Title}\r\n  Link (DetailUrl): {item.DetailUrl}");
                                            continue;
                                        }
                                        else
                                        {
                                            var newCategoryUpdate = new NewCategory()
                                            {
                                                CategoryId = categoryId,
                                                Id = Guid.NewGuid(),
                                                NewId = findItem.Id
                                            };
                                            dbNews.NewCategories.Add(newCategoryUpdate);
                                            await dbNews.SaveChangesAsync();

                                            duplicateList.Add($"- Tên (Title): {item.Title}\r\n  Link (DetailUrl): {item.DetailUrl}. Đã thêm danh mục thành công.");
                                        }
                                    }
                                    else
                                    {
                                        duplicateList.Add($"- Tên (Title): {item.Title}\r\n  Link (DetailUrl): {item.DetailUrl}");
                                    }
                                }
                                catch (Exception exxx)
                                {
                                    duplicateList.Add($"- Tên (Title): {item.Title}\r\n  Link (DetailUrl): {item.DetailUrl}. Lỗi {exxx.Message}");
                                }
                                #endregion
                                continue;
                            }

                            // Tạo entity New
                            var newEntity = new New()
                            {
                                Id = QHCommons.GenAutoId(),
                                Title = item.Title,
                                ConcurrencyStamp = Guid.NewGuid().ToString(),
                                DatePublic = item.CreateDate ?? DateTime.Now,
                                Status = 6,
                                Author = item.Author,
                                ReadCount = item.ViewCount,
                                CreationTime = DateTime.Now,
                                Description = item.Description,
                                DescriptionSEO = item.Description,
                                PageTitleSEO = item.Title,
                                Hot = false,
                                Source = "VIOIT",
                                Shared = true,
                                Language = string.IsNullOrWhiteSpace(txtLanguage.Text) ? "vi" : txtLanguage.Text.Trim(),
                                ExtraProperties = "{}",
                                OldId = string.IsNullOrEmpty(item.OldId) ? string.Empty : item.OldId,
                                TypeNewContent = 3,
                                TypeNewId = defaultTypeNewId,
                                CreatorName = "admin",
                                ShowDescription = true,
                                AllowComment = false,
                                PageUrlSEO = GenerateSlug(item.Title),
                                DetailUrlClone = item.DetailUrl,
                                HienThiInSlide = true,
                                Site = txtSite.Text
                            };

                            // Tải ảnh đại diện
                            if (!string.IsNullOrEmpty(item.AvatarUrl))
                            {
                                string absAvatar = ToAbsoluteUrl("https://vioit.moit.gov.vn", item.AvatarUrl.Trim());
                                newEntity.Image = await ImageDownloader.DownloadImageAsyncurl(absAvatar, folder: txtFolderChua.Text);
                            }

                            // Xử lý ảnh và file trong nội dung tin
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
                                            string rawSrc = img.Attributes["src"].Value.Replace("https&#58;//", "https://").Replace("http&#58;//", "http://").Trim();
                                            string absImgUrl = ToAbsoluteUrl("https://vioit.moit.gov.vn", rawSrc);
                                            img.Attributes["src"].Value = await ImageDownloader.DownloadImageAsyncurl(absImgUrl, folder: txtFolderChua.Text);
                                        }
                                    }
                                }

                                // Xử lý link tài liệu trong nội dung
                                var aNodes = newsDocument.DocumentNode.SelectNodes(".//a");
                                if (aNodes != null)
                                {
                                    string[] documentExtensions = { ".mp4", ".mp3", ".pdf", ".doc", ".docx", ".xls", ".xlsx", ".ppt", ".pptx", ".zip", ".rar", ".7z", ".txt", ".csv" };
                                    foreach (var a in aNodes)
                                    {
                                        if (a.Attributes["href"] != null)
                                        {
                                            string fileUrl = a.Attributes["href"].Value.Replace("https&#58;//", "https://").Replace("http&#58;//", "http://").Trim();
                                            string cleanUrl = fileUrl.Split('?')[0].ToLower();

                                            bool isDocument = false;
                                            foreach (var ext in documentExtensions)
                                            {
                                                if (cleanUrl.EndsWith(ext))
                                                {
                                                    isDocument = true;
                                                    break;
                                                }
                                            }

                                            if (isDocument)
                                            {
                                                fileUrl = ToAbsoluteUrl(item.DetailUrl, fileUrl);
                                                string fileNameToDownload = GetFileNameFromUrl(fileUrl);
                                                string localFilePath = await DownloadFileAsync(httpClient, fileUrl, downloadFolderImg, fileNameToDownload);
                                                if (!string.IsNullOrEmpty(localFilePath))
                                                {
                                                    a.Attributes["href"].Value = localFilePath.Replace("C:\\", "/").Replace("\\", "/");
                                                }
                                            }
                                        }
                                    }
                                }

                                newEntity.Content = newsDocument.DocumentNode.OuterHtml;
                            }

                            temp = newEntity;

                            dbNews.News.Add(newEntity);
                            await dbNews.SaveChangesAsync();

                            // Tải file đính kèm & Insert vào CMSFiles
                            foreach (var attach in item.Attachments)
                            {
                                string fileNameToDownload = string.IsNullOrWhiteSpace(attach.FileName) ? GetFileNameFromUrl(attach.Url) : attach.FileName;
                                string localFilePath = await DownloadFileAsync(httpClient, attach.Url, downloadFolderImg, fileNameToDownload);

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
                                        MimeType = FormCloneMoetNews.GetMimeType(attach.FileName),
                                        FileExtention = 2,
                                        Language = "vi",
                                        FullPathServer = localFilePath.Replace("C:\\", "/").Replace("\\", "/"),
                                        FileName = attach.FileName
                                    };
                                    dbFiles.CMSFiles.Add(file);
                                    await dbFiles.SaveChangesAsync();

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
                                    dbFiles.CMSFileAttachments.Add(fileAttach);
                                    await dbFiles.SaveChangesAsync();
                                }
                            }

                            // Thêm NewCategory
                            var newCategory = new NewCategory()
                            {
                                CategoryId = categoryId,
                                Id = Guid.NewGuid(),
                                NewId = newEntity.Id
                            };
                            dbNews.NewCategories.Add(newCategory);
                            await dbNews.SaveChangesAsync();
                        }
                        catch (Exception ex)
                        {
                            var resultss = temp;
                            errorList.Add($"- Tên (Title): {item.Title}\r\n  Link (DetailUrl): {item.DetailUrl}\r\n  Lỗi: {ex.Message}");
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
                            msgForm.Text = $"Kết quả lưu VIOIT: {errorList.Count} lỗi, {duplicateList.Count} trùng";
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
                lblStatus.Text = "Lưu dữ liệu VIOIT hoàn tất!";
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

        static string ToAbsoluteUrl(string pageUrl, string href)
        {
            if (string.IsNullOrWhiteSpace(href)) return "";
            if (href.StartsWith("http://", StringComparison.OrdinalIgnoreCase) || href.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
                return href;
            try
            {
                var baseUri = new Uri(pageUrl);
                var fullUri = new Uri(baseUri, href);
                return fullUri.ToString();
            }
            catch
            {
                if (href.StartsWith("/"))
                    return "https://vioit.moit.gov.vn" + href;
                return href;
            }
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

        private static string GenerateSlug(string title)
        {
            if (string.IsNullOrWhiteSpace(title)) return "bai-viet";
            string str = title.ToLowerInvariant();
            str = str.Replace("đ", "d");
            var withoutMarks = new string(str
                .Normalize(System.Text.NormalizationForm.FormD)
                .Where(ch => System.Globalization.CharUnicodeInfo.GetUnicodeCategory(ch) != System.Globalization.UnicodeCategory.NonSpacingMark)
                .ToArray());
            var slug = Regex.Replace(withoutMarks, "[^a-z0-9]+", "-").Trim('-');
            return string.IsNullOrWhiteSpace(slug) ? "bai-viet" : slug;
        }

        private async void btnPreview_Click(object sender, EventArgs e)
        {
            try
            {
                lblStatus.Text = "Đang duyệt tìm các bài viết cần sửa link...";
                var previewList = new List<PreviewUpdateItem>();

                using (var dbNews = new Portal_Vioit_NewsServicesEntities())
                {
                    var allNews = await dbNews.News.Where(a => !a.IsDeleted).ToListAsync();
                    int count = 0;
                    foreach (var item in allNews)
                    {
                        if (string.IsNullOrEmpty(item.Content)) continue;
                        if (Regex.IsMatch(item.Content, @"_\d+(\.mp4|\.mp3|\.pdf|\.docx|\.doc)", RegexOptions.IgnoreCase))
                        {
                            previewList.Add(new PreviewUpdateItem
                            {
                                Id = item.Id?.ToString(),
                                Title = item.Title,
                                DetailUrlClone = item.DetailUrlClone
                            });
                            count++;
                        }
                    }

                    dgvData.DataSource = null;
                    dgvData.DataSource = previewList;
                    lblStatus.Text = $"Hoàn tất! Tìm thấy {count} bản ghi.";
                    MessageBox.Show($"Tìm thấy {count} bản ghi cần cập nhật.", "Thông báo");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
            }
        }

        private async void btnUpdateContent_Click(object sender, EventArgs e)
        {
            try
            {
                lblStatus.Text = "Đang cập nhật lại link bài viết...";
                using (var dbNews = new Portal_Vioit_NewsServicesEntities())
                {
                    var allNews = await dbNews.News.Where(a => !a.IsDeleted).ToListAsync();
                    int updateCount = 0;

                    foreach (var item in allNews)
                    {
                        if (string.IsNullOrEmpty(item.Content)) continue;

                        string newContent = Regex.Replace(item.Content, @"_\d+(\.mp4|\.mp3|\.pdf|\.docx|\.doc)", "$1", RegexOptions.IgnoreCase);
                        if (newContent != item.Content)
                        {
                            item.Content = newContent;
                            updateCount++;
                        }
                    }

                    if (updateCount > 0)
                    {
                        await dbNews.SaveChangesAsync();
                    }

                    lblStatus.Text = $"Hoàn tất! Đã cập nhật {updateCount} bản ghi.";
                    MessageBox.Show($"Đã cập nhật xong {updateCount} bài viết.", "Thông báo");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi cập nhật: " + ex.Message);
            }
        }

        private void btnfresh_Click(object sender, EventArgs e)
        {
            LoadCategories();
        }
    }

    public class BaiVietVioitItem
    {
        public string Title { get; set; } = "";
        public string DetailUrl { get; set; } = "";
        public string AvatarUrl { get; set; } = "";
        public string Description { get; set; } = "";
        public string Content { get; set; } = "";
        public string Author { get; set; } = "";
        public int ViewCount { get; set; } = 0;
        public List<AttachmentFile> Attachments { get; set; } = new List<AttachmentFile>();

        public string FilesDisplay => Attachments != null && Attachments.Count > 0 ? string.Join(", ", Attachments.Select(x => x.FileName)) : "";

        public DateTime? CreateDate { get; set; }
        public string OldId { get; set; }
        public DateTime CreateAt { get; set; } = DateTime.Now;
    }
}
