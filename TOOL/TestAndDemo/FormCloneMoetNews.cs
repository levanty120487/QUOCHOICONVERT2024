using HtmlAgilityPack;
using QHBASE;
using System;
using System.Collections.Generic;
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

        private void FormCloneMoetNews_Load(object sender, EventArgs e)
        {
            LoadCategories();
        }

        private void LoadCategories()
        {
            try
            {
                using (var db = new Portal_Moet_NewsServicesEntities()) // Giả sử tên context là Moet_NewsEntities, đổi lại nếu sai
                {
                    // Lấy danh mục, nếu có phân cấp có thể format lại chuỗi hiển thị
                    var categories = db.Categories.ToList();
                    cboCategory.DataSource = categories;
                    cboCategory.DisplayMember = "Title"; // Cần map đúng cột tên danh mục
                    cboCategory.ValueMember = "Id";
                }
            }
            catch (Exception ex)
            {
                // Nếu chưa build project hoặc tên Entities chưa đúng sẽ lỗi. Tạm bắt lỗi.
                Console.WriteLine(ex.Message);
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
                
                string classList = string.IsNullOrWhiteSpace(txtClassList.Text) ? "nav-item list-news-one" : txtClassList.Text.Trim();

                for (int i = 1; i <= totalPages; i++)
                {
                    // Giả sử phân trang theo param page hoặc url. Tùy trang web. Ở đây ví dụ append url.
                    string pageUrl = baseUrl; 
                    if (totalPages > 1 && i > 1) 
                    {
                        // TODO: Adjust paging logic if needed.
                        pageUrl = pageUrl.EndsWith("/") ? $"{pageUrl}page/{i}" : $"{pageUrl}/page/{i}";
                    }
                    
                    var items = await ReadListPageAsync(pageUrl, classList);
                    if (items != null)
                        parsedData.AddRange(items);
                }

                dgvData.DataSource = null;
                dgvData.DataSource = parsedData;
                MessageBox.Show($"Đã đọc xong {parsedData.Count} bài viết.", "Thông báo");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi đọc dữ liệu: " + ex.Message);
            }
        }

        private async Task<List<BaiVietMoetItem>> ReadListPageAsync(string pageUrl, string classList)
        {
            var result = new List<BaiVietMoetItem>();
            using (var httpClient = new HttpClient())
            {
                httpClient.Timeout = TimeSpan.FromSeconds(60);
                string html = await httpClient.GetStringAsync(pageUrl);
                var doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(html);

                // Tạo XPath động dựa vào class người dùng nhập
                string xPathList = $"//div[contains(@class, '{classList.Split(' ')[0]}')] | //li[contains(@class, '{classList.Split(' ')[0]}')]";
                
                var nodes = doc.DocumentNode.SelectNodes(xPathList);
                if (nodes == null) return result;

                foreach (var node in nodes)
                {
                    var aNode = node.SelectSingleNode(".//a[@href]");
                    if (aNode == null) continue;

                    string detailHref = aNode.GetAttributeValue("href", "");
                    string detailUrl = ToAbsoluteUrl(pageUrl, detailHref);

                    var item = new BaiVietMoetItem();
                    item.DetailUrl = detailUrl;

                    // Parse avatar
                    var imgNode = node.SelectSingleNode(".//img");
                    if (imgNode != null)
                    {
                        item.AvatarUrl = ToAbsoluteUrl(pageUrl, imgNode.GetAttributeValue("src", ""));
                    }

                    // Tải trang chi tiết để bóc thông tin
                    await ParseDetailAsync(httpClient, item, detailUrl);
                    
                    result.Add(item);
                }
            }
            return result;
        }

        private async Task ParseDetailAsync(HttpClient httpClient, BaiVietMoetItem item, string detailUrl)
        {
            try
            {
                string html = await httpClient.GetStringAsync(detailUrl);
                var doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(html);

                // Bóc tiêu đề (có thể dùng class title truyền vào)
                var titleNode = doc.DocumentNode.SelectSingleNode("//h1 | //h2[contains(@class,'title')]");
                if (titleNode != null)
                    item.Title = CleanText(titleNode.InnerText);

                // Mô tả
                var briefNode = doc.DocumentNode.SelectSingleNode("//div[contains(@class, 'article-brief')]");
                if (briefNode != null)
                    item.Description = briefNode.InnerHtml.Trim();

                // Nội dung
                var contentNode = doc.DocumentNode.SelectSingleNode("//div[contains(@class, 'content-detail')]");
                if (contentNode != null)
                    item.Content = contentNode.InnerHtml.Trim();

                // Tác giả
                var authorNode = doc.DocumentNode.SelectSingleNode("//div[contains(@class, 'author')]");
                if (authorNode != null)
                    item.Author = CleanText(authorNode.InnerText);

                // Lượt xem
                var viewNode = doc.DocumentNode.SelectSingleNode("//div[contains(@class, 'vi vi-eye')]");
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
                    var fileLinks = fileAttachNode.SelectNodes(".//a[@href]");
                    if (fileLinks != null)
                    {
                        foreach (var fNode in fileLinks)
                        {
                            var href = fNode.GetAttributeValue("href", "");
                            if (string.IsNullOrWhiteSpace(href)
                                || href.ToLower().StartsWith("javascript:"))
                            {
                                continue;
                            }
                            item.AttachmentUrls.Add(ToAbsoluteUrl(detailUrl, href));
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

            Guid categoryId;
            if (!Guid.TryParse(cboCategory.SelectedValue.ToString(), out categoryId))
            {
                // Nếu ID kiểu int thì chỉnh sửa lại ở đây
            }

            string downloadFolderImg = @"C:\uploadFckFiles\news";
            Directory.CreateDirectory(downloadFolderImg);

            try
            {
                using (var dbNews = new Portal_Moet_NewsServicesEntities())
                using (var dbFiles = new Portal_Moet_FilesServicesEntities())
                using (var httpClient = new HttpClient())
                {
                    foreach (var item in parsedData)
                    {
                        // Tải file avatar nếu có
                        string localAvatarUrl = string.Empty;
                        if (!string.IsNullOrEmpty(item.AvatarUrl))
                        {
                            string fileName = GetFileNameFromUrl(item.AvatarUrl);
                            localAvatarUrl = await DownloadFileAsync(httpClient, item.AvatarUrl, downloadFolderImg, fileName);
                        }

                        // Insert vào bảng New
                        var newEntity = new New()
                        {
                            //Id = Guid.NewGuid(), // Nếu auto generate thì không cần
                            //CategoryId = categoryId, // Điều chỉnh kiểu nếu cần
                            Title = item.Title,
                            Description = item.Description,
                            Content = item.Content,
                            Author = item.Author,
                            //ViewCount = item.ViewCount,
                            //ImageUrl = string.IsNullOrEmpty(localAvatarUrl) ? "" : $"/uploadFckFiles/news/{Path.GetFileName(localAvatarUrl)}",
                            //CreatedDate = DateTime.Now,
                            //CreatedBy = "Admin"
                        };
                        dbNews.News.Add(newEntity);
                        dbNews.SaveChanges();

                        // Tải file đính kèm & Insert vào Moet_Files
                        foreach (var attachUrl in item.AttachmentUrls)
                        {
                            string fileName = GetFileNameFromUrl(attachUrl);
                            string localFilePath = await DownloadFileAsync(httpClient, attachUrl, downloadFolderImg, fileName);

                            if (!string.IsNullOrEmpty(localFilePath))
                            {
                                var fileEntity = new Portal_Moet_FilesServicesEntities() // Tên entity có thể là Moet_File
                                {
                                    //Id = Guid.NewGuid(),
                                    //NewId = newEntity.Id,
                                    //FileName = Path.GetFileName(localFilePath),
                                    //FilePath = $"/uploadFckFiles/news/{Path.GetFileName(localFilePath)}",
                                    //CreatedDate = DateTime.Now
                                };
                                //dbFiles.Moet_Files.Add(fileEntity); // Điều chỉnh tên DbSet nếu khác
                            }
                        }
                        dbFiles.SaveChanges();
                    }
                }

                MessageBox.Show("Lưu dữ liệu thành công!");
            }
            catch (Exception ex)
            {
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
                    
                    fileName = Regex.Replace(fileName, @"[^a-zA-Z0-9.\s]", "").Replace(" ", "_");
                    string fullPath = Path.Combine(folder, fileName);

                    // Handle exist file
                    int count = 1;
                    string nameOnly = Path.GetFileNameWithoutExtension(fileName);
                    string ext = Path.GetExtension(fileName);
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
        public List<string> AttachmentUrls { get; set; } = new List<string>();
    }
}
