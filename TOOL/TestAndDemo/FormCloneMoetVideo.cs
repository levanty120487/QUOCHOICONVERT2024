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
    public partial class FormCloneMoetVideo : RJForms.RJChildForm
    {
        private List<VideoCloneItem> parsedData = new List<VideoCloneItem>();

        public FormCloneMoetVideo()
        {
            InitializeComponent();
        }

        private Label lblStatus;

        private void FormCloneMoetVideo_Load(object sender, EventArgs e)
        {
            lblStatus = new Label();
            lblStatus.AutoSize = true;
            lblStatus.ForeColor = System.Drawing.Color.Red;
            lblStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            lblStatus.Location = new System.Drawing.Point(20, 150);
            lblStatus.Text = "";
            pnlClientArea.Controls.Add(lblStatus);
            LoadCategories();
        }

        private void LoadCategories()
        {
            try
            {
                using (var db = new Portal_Moet_HinhanhServiceEntities())
                {
                    var categories = db.CategoryVideos.ToList();
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
                string inputHtml = txtWebUrl.Text.Trim(); // Người dùng nhập trực tiếp HTML vào đây

                string classList = "//div[contains(@class, 'article-item')]";

                string originalBtnText = btnReadData.Text;
                btnReadData.Enabled = false;
                lblStatus.Text = "Bắt đầu đọc dữ liệu...";

                var items = await ReadListPageAsync(inputHtml, classList);
                if (items != null)
                    parsedData.AddRange(items);

                btnReadData.Text = originalBtnText;
                btnReadData.Enabled = true;
                lblStatus.Text = $"Hoàn tất! Đã đọc {parsedData.Count} bản ghi.";

                dgvData.DataSource = null;
                dgvData.DataSource = parsedData;
                MessageBox.Show($"Đã đọc xong {parsedData.Count} video.", "Thông báo");
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

        private async Task<List<VideoCloneItem>> ReadListPageAsync(string htmlContent, string classList)
        {
            var result = new List<VideoCloneItem>();
            using (var httpClient = new HttpClient())
            {
                httpClient.Timeout = TimeSpan.FromSeconds(60);
                var doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(htmlContent);

                string xPathList = ConvertToXPath(classList, true);
                var listNodes = doc.DocumentNode.SelectNodes(xPathList);
                if (listNodes == null) return result;

                string baseUrl = "https://moet.gov.vn"; // Default Moet domain cho link tương đối

                int itemIndex = 0;
                foreach (var itemNode in listNodes)
                {
                    itemIndex++;
                    lblStatus.Text = $"Đã đọc bản ghi thứ {itemIndex}";
                    Application.DoEvents();

                    var item = new VideoCloneItem();

                    var aNode = itemNode.SelectSingleNode(".//a");
                    if (aNode != null)
                    {
                        string detailHref = aNode.GetAttributeValue("href", "");
                        item.LinkDetail = ToAbsoluteUrl(baseUrl, detailHref);
                        item.SourceUrl = item.LinkDetail;
                    }

                    var imgNode = itemNode.SelectSingleNode(".//img");
                    if (imgNode != null)
                    {
                        string src = imgNode.GetAttributeValue("data-original", "");
                        if(string.IsNullOrWhiteSpace(src))
                            src = imgNode.GetAttributeValue("src", "");
                        item.Image = ToAbsoluteUrl(baseUrl, src);
                    }

                    var titleNode = itemNode.SelectSingleNode(".//div[contains(@class, 'article-title')]");
                    if (titleNode != null)
                    {
                        item.Title = CleanText(titleNode.InnerText);
                    }
                    else if (aNode != null)
                    {
                        item.Title = CleanText(aNode.GetAttributeValue("title", ""));
                    }

                    if (!string.IsNullOrWhiteSpace(item.LinkDetail))
                    {
                        await ParseDetailAsync(httpClient, item, item.LinkDetail);
                        result.Add(item);
                    }
                }
            }
            return result;
        }

        private async Task ParseDetailAsync(HttpClient httpClient, VideoCloneItem item, string detailUrl)
        {
            try
            {
                string html = await httpClient.GetStringAsync(detailUrl);
                var doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(html);

                var detailNode = doc.DocumentNode.SelectSingleNode("//div[contains(@class, 'detail-video')]");
                if (detailNode != null)
                {
                    var videoNode = detailNode.SelectSingleNode(".//video");
                    if (videoNode != null)
                    {
                        item.VideoUrl = ToAbsoluteUrl(detailUrl, videoNode.GetAttributeValue("src", ""));
                        if(string.IsNullOrWhiteSpace(item.VideoUrl))
                        {
                            var sourceNode = videoNode.SelectSingleNode(".//source");
                            if(sourceNode != null)
                            {
                                item.VideoUrl = ToAbsoluteUrl(detailUrl, sourceNode.GetAttributeValue("src", ""));
                            }
                        }
                    }

                    var descNode = detailNode.SelectSingleNode(".//div[contains(@class, 'article-brief')]");
                    if (descNode != null)
                    {
                        item.Description = CleanText(descNode.InnerText);
                    }

                    var contentNode = detailNode.SelectSingleNode(".//div[contains(@class, 'article-content')]");
                    if (contentNode != null)
                    {
                        item.Contents = contentNode.InnerHtml;
                    }

                    var timeNode = detailNode.SelectSingleNode(".//div[contains(@class, 'article-time')]");
                    if (timeNode != null)
                    {
                        string dateText = CleanText(timeNode.InnerText);
                        var match = Regex.Match(dateText, @"\d{2}/\d{2}/\d{4}");
                        if (match.Success)
                        {
                            if (DateTime.TryParseExact(match.Value, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out var dt))
                            {
                                item.NgayTao = dt;
                            }
                        }
                    }

                    var authorNode = detailNode.SelectSingleNode(".//div[contains(@class, 'author')]");
                    if (authorNode != null)
                    {
                        item.Author = CleanText(authorNode.InnerText);
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

            var categoryIdStr = cboCategory.SelectedValue.ToString();
            if (string.IsNullOrWhiteSpace(categoryIdStr) || !Guid.TryParse(categoryIdStr, out Guid categoryId))
            {
                MessageBox.Show("Vui lòng chọn danh mục hợp lệ!");
                return;
            }

            string downloadFolderRoot = @"C:\uploadFckFiles\";
            string subFolder = string.IsNullOrWhiteSpace(txtFolderChua.Text) ? "tinvideo" : txtFolderChua.Text.Trim();
            string downloadFolder = Path.Combine(downloadFolderRoot, subFolder);
            Directory.CreateDirectory(downloadFolder);

            string originalBtnText = btnSaveData.Text;
            btnSaveData.Enabled = false;
            lblStatus.Text = "Bắt đầu lưu dữ liệu...";

            try
            {
                using (var dbHinhAnh = new Portal_Moet_HinhanhServiceEntities())
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
                            currentIndex++;
                            lblStatus.Text = $"Đang lưu bản ghi thứ {currentIndex}/{totalItems}...";
                            Application.DoEvents();

                            var findItem = await dbHinhAnh.Videos
                                                    .Where(a => a.DetailLinkClone.Equals(item.LinkDetail))
                                                    .FirstOrDefaultAsync();
                            if(findItem != null)
                            {
                                duplicateList.Add($"- Tên (Title): {item.Title}\r\n  Link (DetailUrl): {item.LinkDetail}");
                                #region update contents
                                findItem.Contents = item.Contents;
                                dbHinhAnh.SaveChanges();
                                #endregion
                                continue;
                            }    

                            // Tải ảnh đại diện
                            string localImagePath = "";
                            if (!string.IsNullOrWhiteSpace(item.Image))
                            {
                                string fileNameImg = GetFileNameFromUrl(item.Image);
                                localImagePath = await DownloadFileAsync(httpClient, item.Image, downloadFolder, fileNameImg);
                                if (!string.IsNullOrEmpty(localImagePath))
                                {
                                    item.Image = "/uploadFckFiles/" + subFolder + "/" + Path.GetFileName(localImagePath);
                                }
                            }

                            // Tải video
                            string localVideoPath = "";
                            if (!string.IsNullOrWhiteSpace(item.VideoUrl))
                            {
                                string fileNameVid = GetFileNameFromUrl(item.VideoUrl);
                                localVideoPath = await DownloadFileAsync(httpClient, item.VideoUrl, downloadFolder, fileNameVid);
                                if (!string.IsNullOrEmpty(localVideoPath))
                                {
                                    item.VideoUrl = "/uploadFckFiles/" + subFolder + "/" + Path.GetFileName(localVideoPath);
                                }
                            }

                            // Insert vào bảng Video
                            var newEntity = new Video()
                            {
                                Id = Guid.NewGuid(),
                                Title = item.Title,
                                Description = item.Description,
                                Author = item.Author,
                                View = item.ViewCount,
                                NgayTao = item.NgayTao ?? DateTime.Now,
                                Image = item.Image,
                                File = item.VideoUrl,
                                DetailLinkClone = item.LinkDetail,
                                CategoryVideoId = categoryId,
                                CategoryVideoTitle = cboCategory.Text,
                                ConcurrencyStamp = Guid.NewGuid().ToString(),
                                CreationTime = DateTime.Now,
                                Status = 1,
                                Language = "vi",
                                ExtraProperties = "{}",
                                CreatedBy = "admin",
                                CategoryVideoTree = cboCategory.Text,
                                Contents = item.Contents
                            };
                            
                            dbHinhAnh.Videos.Add(newEntity);
                            dbHinhAnh.SaveChanges();
                        }
                        catch (Exception ex)
                        {
                            errorList.Add($"- Tên (Title): {item.Title}\n  Link (DetailUrl): {item.LinkDetail}\n  Lỗi: {ex.Message}");
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
                if (string.IsNullOrWhiteSpace(fileUrl)) return string.Empty;
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
                if (string.IsNullOrWhiteSpace(fileUrl)) return $"file_{DateTime.Now.Ticks}";
                var uri = new Uri(fileUrl);
                string fileName = Path.GetFileName(uri.LocalPath);
                return string.IsNullOrWhiteSpace(fileName) ? $"file_{DateTime.Now.Ticks}" : fileName;
            }
            catch { return $"file_{DateTime.Now.Ticks}"; }
        }

        private void pnlClientArea_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }
    }

    public class VideoCloneItem
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Author { get; set; }
        public int? ViewCount { get; set; }
        public string SourceUrl { get; set; }
        public string VideoUrl { get; set; }
        public string Image { get; set; }
        public string LinkDetail { get; set; }
        public DateTime? NgayTao { get; set; }
        public DateTime CreateAt { get; set; } = DateTime.Now;

        public string Contents { get; set; }
    }
}
