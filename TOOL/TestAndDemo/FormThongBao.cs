using HtmlAgilityPack;
using QHBASE;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RJCodeUI_M1.TestAndDemo
{
    public partial class FormThongBao : RJForms.RJChildForm
    {
        public FormThongBao()
        {
            InitializeComponent();
        }

        private void btnGetDanhMuc_Click(object sender, EventArgs e)
        {
            using (var db = new portalcdk_CommonsServiceEntities())
            {
                var dbCategory = db.LoaiThongBaos.ToList();
                cboCategory.Items.Clear();
                cboCategory.DataSource = dbCategory;
                cboCategory.ValueMember = "Id";
                cboCategory.DisplayMember = "Title";
            };
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            try 
            {
                var pageStep = Convert.ToInt32(txtPageStep.Text);
                string downloadFolder = @"D:\fileThongBao";
                for (var iPage = pageStep; iPage >= 1; iPage--)
                {
                    var pageUrl = string.Concat(txtLink.Text.Trim(), iPage + "/");
                    var data = await ReadTableAndDownloadFilesAsync(pageUrl, downloadFolder);
                    DateTimeFormatInfo dtfi = new DateTimeFormatInfo();
                    dtfi.ShortDatePattern = "dd/MM/yyyy";
                    dtfi.DateSeparator = "/";
                    foreach (var item in data)
                    {
                        #region insert DB
                        DateTime? ngayThongBao = null;
                        if (!string.IsNullOrWhiteSpace(item.NgayDang))
                        {
                            ngayThongBao = Convert.ToDateTime(item.NgayDang, dtfi);
                        }
                        using (var commons = new portalcdk_CommonsServiceEntities())
                        {
                            var dsThongBao = new DanhSachThongBao()
                            {
                                Id = QHCommons.GenAutoId(),
                                Language = "vi",
                                ExtraProperties = "{}",
                                CreationTime = DateTime.Now,
                                IdLoaiThongBao = cboCategory.SelectedValue.ToString(),
                                MoTa = item.Mota,
                                NoiDung = item.Content,
                                TieuDe = item.TieuDe,
                                SoThongBao = item.SoKyHieu,
                                Status = 1,
                                NgayThongBao = ngayThongBao,
                                ConcurrencyStamp = QHCommons.GenAutoId()
                            };
                            commons.DanhSachThongBaos.Add(dsThongBao);
                            commons.SaveChanges();
                            #region insert files
                            if (!string.IsNullOrWhiteSpace(item.FileUrl))
                            {
                                using (var files = new CTTDTCDK_FilesServicesEntities())
                                {
                                    var file = new CMSFile()
                                    {
                                        Id = Guid.NewGuid(),
                                        FileType = 2,
                                        CreationTime = DateTime.Now,
                                        FileContainerName = "CMSContainerPublic",
                                        ConcurrencyStamp = Guid.NewGuid().ToString(),
                                        ExtraProperties = "{}",
                                        MimeType = GetMimeType(item.FileUrl),
                                        FileExtention = 2,
                                        Language = "vi",
                                        FullPathServer = string.Concat("/uploadFckFiles/fileThongBao/", item.FileName),
                                        FileName = item.FileName
                                    };
                                    files.CMSFiles.Add(file);
                                    files.SaveChanges();
                                    #region Insert fileAttachment
                                    var fileAttach = new CMSFileAttachment()
                                    {
                                        Id = Guid.NewGuid(),
                                        EntityId = dsThongBao.Id,
                                        FileId = file.Id,
                                        ExtraProperties = "{}",
                                        ConcurrencyStamp = Guid.NewGuid().ToString(),
                                        CreationTime = DateTime.Now,
                                        FileAttachmentType = 8
                                    };
                                    files.CMSFileAttachments.Add(fileAttach);
                                    files.SaveChanges();
                                    #endregion
                                }
                            }
                            #endregion
                        }
                        ;
                        #endregion
                    }
                }
                MessageBox.Show("Done");
            }
            catch(Exception ex) {
                throw new Exception(ex.Message);
            }
        }

        public static async Task<List<BaiVietItem>> ReadTableAndDownloadFilesAsync(string pageUrl, string downloadFolder)
        {
            Directory.CreateDirectory(downloadFolder);

            using (var httpClient = new HttpClient())
            {
                httpClient.Timeout = TimeSpan.FromSeconds(60);

                string html = await httpClient.GetStringAsync(pageUrl);

                var doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(html);

                // Tìm table có class chứa "tabl"
                var tableNode = doc.DocumentNode.SelectSingleNode(
                    "//table[contains(concat(' ', normalize-space(@class), ' '), ' tableList ')]") ?? throw new Exception("Không tìm thấy table có class = 'tableList'.");

                // Lấy tất cả tr trong table
                var rowNodes = tableNode.SelectNodes(".//tr");
                var result = new List<BaiVietItem>();

                if (rowNodes == null || rowNodes.Count <= 1)
                {
                    return result;
                }

                // Bỏ qua dòng đầu tiên
                foreach (var row in rowNodes.Skip(1))
                {
                    var tdNodes = row.SelectNodes("./td");
                    if (tdNodes == null || tdNodes.Count < 4)
                    {
                        continue;
                    }

                    string soKyHieu = CleanText(tdNodes[0].InnerText);
                    string tieuDe = string.Empty;
                    string moTa = CleanText(tdNodes[1].InnerText);
                    string ngayDang = CleanText(tdNodes[2].InnerText);
                    string noiDung = string.Empty;

                    string fileUrl = null;
                    string fileLocalPath = null;
                    string fileName = null;

                    // Lấy link file trong cột thứ 4
                    var fileLinkNode = tdNodes[3].SelectSingleNode(".//a[@href]");
                    if (fileLinkNode != null)
                    {
                        string href = fileLinkNode.GetAttributeValue("href", "").Trim();
                        if (!string.IsNullOrWhiteSpace(href))
                        {
                            fileUrl = ToAbsoluteUrl(pageUrl, href);
                            fileLocalPath = await DownloadFileAsync(httpClient, fileUrl, downloadFolder);
                            fileName = GetFileNameFromUrl(fileLocalPath);
                        }
                    }

                    #region đọc chi tiết để lấy về tiêu đề và nội dung
                    try
                    {
                        var linkDetail = tdNodes[1].SelectSingleNode(".//a[@href]");
                        if (linkDetail != null)
                        {
                            string href = linkDetail.GetAttributeValue("href", "").Trim();
                            string viewDetail = new Uri(new Uri("https://www.vr.org.vn/Pages/thong-bao.aspx"), href).ToString();

                            string detailHtml = await httpClient.GetStringAsync(viewDetail);

                            var contentDetail = new HtmlAgilityPack.HtmlDocument();
                            contentDetail.LoadHtml(detailHtml);

                            var readContent = contentDetail.DocumentNode.SelectSingleNode(
                                "//div[contains(concat(' ', normalize-space(@class), ' '), ' qc-qp-tc ')]");

                            if (readContent != null)
                            {
                                var detailTieuDe = readContent.SelectSingleNode(
                                    ".//h3[contains(concat(' ', normalize-space(@class), ' '), ' title ')]");

                                if (detailTieuDe != null)
                                    tieuDe = HtmlEntity.DeEntitize(detailTieuDe.InnerText).Trim();

                                var detailNoiDung = readContent.SelectSingleNode(
                                    ".//div[contains(concat(' ', normalize-space(@class), ' '), ' content ')]");

                                if (detailNoiDung != null)
                                    noiDung = detailNoiDung.InnerHtml.Trim();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Lỗi đọc trang chi tiết: " + ex.Message);
                    }
                    #endregion

                    result.Add(new BaiVietItem
                    {
                        SoKyHieu = soKyHieu,
                        TieuDe = tieuDe,
                        NgayDang = ngayDang,
                        FileUrl = fileUrl,
                        FileLocalPath = fileLocalPath,
                        Mota = moTa,
                        Content = noiDung,
                        FileName = fileName
                    });
                }
                return result;
            }
        }

        static string CleanText(string text)
        {
            return HtmlEntity.DeEntitize(text).Trim();
        }

        static string ToAbsoluteUrl(string pageUrl, string href)
        {
            var baseUri = new Uri(pageUrl);
            var fullUri = new Uri(baseUri, href);
            return fullUri.ToString();
        }

        static async Task<string> DownloadFileAsync(HttpClient httpClient, string fileUrl, string downloadFolder)
        {
            try
            {
                using (var response = await httpClient.GetAsync(fileUrl))
                {
                    response.EnsureSuccessStatusCode();

                    string fileName = GetFileNameFromUrl(fileUrl);
                    fileName = Regex.Replace(fileName, @"[^a-zA-Z0-9.\s]", "");
                    fileName = fileName.Replace(" ", "_");
                    fileName = RemoveVietnameseAccents(fileName);
                    string fullPath = GetUniqueFilePath(downloadFolder, fileName);
                    var fs = new FileStream(fullPath, FileMode.Create, FileAccess.Write, FileShare.None);
                    await response.Content.CopyToAsync(fs);

                    return fullPath;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Tải file lỗi: {fileUrl}");
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        static string GetFileNameFromUrl(string fileUrl)
        {
            try
            {
                var uri = new Uri(fileUrl);
                string fileName = Path.GetFileName(uri.LocalPath);

                if (string.IsNullOrWhiteSpace(fileName))
                {
                    fileName = "file_" + DateTime.Now.Ticks;
                }

                return fileName;
            }
            catch
            {
                return "file_" + DateTime.Now.Ticks;
            }
        }

        static string GetUniqueFilePath(string folder, string fileName)
        {
            string fullPath = Path.Combine(folder, fileName);

            if (!File.Exists(fullPath))
                return fullPath;

            string nameOnly = Path.GetFileNameWithoutExtension(fileName);
            string ext = Path.GetExtension(fileName);
            int count = 1;

            while (true)
            {
                string newFileName = $"{nameOnly}_{count}{ext}";
                string newFullPath = Path.Combine(folder, newFileName);

                if (!File.Exists(newFullPath))
                    return newFullPath;

                count++;
            }
        }

        public static string RemoveVietnameseAccents(string text)
        {
            if (string.IsNullOrEmpty(text))
                return text;

            // Chuẩn hóa về dạng tách dấu
            var normalized = text.Normalize(NormalizationForm.FormD);

            StringBuilder sb = new StringBuilder();

            foreach (var c in normalized)
            {
                var unicodeCategory = Char.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(c);
                }
            }

            // Xử lý riêng chữ đ/Đ
            return sb.ToString()
                     .Replace('đ', 'd')
                     .Replace('Đ', 'D');
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

        private async void btnSaveApatit_Click(object sender, EventArgs e)
        {
            var items = await ParseListPage(txtApatit.Text.Trim());
            if (items != null && items.Count > 0)
            {
                var downloadFolder = "C:\\apatits";
                var handler = new HttpClientHandler
                {
                    CookieContainer = new CookieContainer(),
                    AutomaticDecompression = DecompressionMethods.GZip
                };
                using (var httpClient = new HttpClient(handler))
                {
                    httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(
                        "Mozilla/5.0 (Windows NT 10.0; Win64; x64) ApatitScraper/1.0");
                    items.Reverse();
                    foreach (var item in items)
                    {
                        if (!string.IsNullOrWhiteSpace(item.ThumbnailUrl))
                        {
                            item.LocalThumbnailUrl = await DownloadThumbnailAsync(
                                httpClient,
                                item.ThumbnailUrl,
                                downloadFolder,
                                item.Title);
                        }
                        var detailHtml = await httpClient.GetStringAsync(item.DetailUrl);
                        item.AttachmentUrl = ParseAttachmentUrl(detailHtml);

                        if (!string.IsNullOrWhiteSpace(item.AttachmentUrl))
                        {
                            item.LocalFilePath = await DownloadAttachmentAsync(
                                httpClient,
                                item.AttachmentUrl,
                                downloadFolder,
                                item.Title);
                        }
                    }
                }
                #region insert data
                DateTimeFormatInfo dtfi = new DateTimeFormatInfo();
                dtfi.ShortDatePattern = "dd/MM/yyyy";
                dtfi.DateSeparator = "/";
                foreach (var item in items)
                {
                    #region insert DB
                    DateTime? ngayThongBao = DateTime.Now;
                    using (var commons = new APATIT_CommonsServicesEntities())
                    {
                        var img = string.Empty;
                        if(!string.IsNullOrWhiteSpace(item.LocalThumbnailUrl))
                        {
                            img = string.Concat("/uploadFckFiles/apatits/", item.LocalThumbnailUrl.Split('\\').LastOrDefault());
                        }
                        var dsThongBao = new DanhSachThongBao()
                        {
                            Id = QHCommons.GenAutoId(),
                            Language = "vi",
                            ExtraProperties = "{}",
                            CreationTime = DateTime.Now,
                            IdLoaiThongBao = cboCategory.SelectedValue.ToString(),
                            MoTa = string.Empty,
                            NoiDung = string.Empty,
                            TieuDe = item.Title,
                            SoThongBao = string.Empty,
                            Status = 1,
                            NgayThongBao = ngayThongBao,
                            ConcurrencyStamp = QHCommons.GenAutoId(),
                            Image = img,
                            MoTaUnicode = string.Empty,
                            TieuDeUnicode = item.Title
                        };
                        commons.DanhSachThongBaos.Add(dsThongBao);
                        commons.SaveChanges();
                        #region insert files
                        if (!string.IsNullOrWhiteSpace(item.LocalFilePath))
                        {
                            using (var files = new APATIT_FilesServicesEntities())
                            {
                                var fileName = string.Empty;
                                if(!string.IsNullOrWhiteSpace(item.LocalFilePath))
                                {
                                    fileName = item.LocalFilePath.Split('\\').LastOrDefault();
                                }
                                var file = new CMSFile()
                                {
                                    Id = Guid.NewGuid(),
                                    FileType = 2,
                                    CreationTime = DateTime.Now,
                                    FileContainerName = "CMSContainerPublic",
                                    ConcurrencyStamp = Guid.NewGuid().ToString(),
                                    ExtraProperties = "{}",
                                    MimeType = GetMimeType(item.LocalFilePath),
                                    FileExtention = 2,
                                    Language = "vi",
                                    FullPathServer = string.Concat("/uploadFckFiles/apatits/", fileName),
                                    FileName = fileName
                                };
                                files.CMSFiles.Add(file);
                                files.SaveChanges();
                                #region Insert fileAttachment
                                var fileAttach = new CMSFileAttachment()
                                {
                                    Id = Guid.NewGuid(),
                                    EntityId = dsThongBao.Id,
                                    FileId = file.Id,
                                    ExtraProperties = "{}",
                                    ConcurrencyStamp = Guid.NewGuid().ToString(),
                                    CreationTime = DateTime.Now,
                                    FileAttachmentType = 8,
                                };
                                files.CMSFileAttachments.Add(fileAttach);
                                files.SaveChanges();
                                #endregion
                            }
                        }
                        #endregion
                    }
                    ;
                    #endregion
                }
                #endregion
            }
            MessageBox.Show("Done.");
        }

        #region Apatit
        public async Task<List<ApatitDisclosureItem>> ParseListPage(string pageUrl)
        {
            using (var httpClient = new HttpClient())
            {
                string html = await httpClient.GetStringAsync(pageUrl);

                var doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(html);

                var nodes = doc.DocumentNode.SelectNodes(
                    "//*[@id='main']//div[contains(concat(' ', normalize-space(@class), ' '), ' col ') " +
                    "and contains(concat(' ', normalize-space(@class), ' '), ' post-item ')]");

                if (nodes is null)
                {
                    return new List<ApatitDisclosureItem>();
                }

                return nodes.Select(node =>
                {
                    var anchor = node.SelectSingleNode(".//a[contains(concat(' ', normalize-space(@class), ' '), ' plain ')]")
                        ?? node.SelectSingleNode(".//a[@href]");
                    var image = node.SelectSingleNode(".//img");
                    var title = WebUtility.HtmlDecode(
                        node.SelectSingleNode(".//h5[contains(concat(' ', normalize-space(@class), ' '), ' post-title ')]")
                            ?.InnerText
                            ?.Trim() ?? "");

                    return new ApatitDisclosureItem
                    {
                        DetailUrl = anchor?.GetAttributeValue("href", "") ?? "",
                        Title = NormalizeWhitespace(title),
                        ThumbnailUrl = image?.GetAttributeValue("data-src", "")
                            ?? image?.GetAttributeValue("src", "")
                            ?? ""
                    };
                })
                .Where(x => !string.IsNullOrWhiteSpace(x.DetailUrl))
                .ToList();
            }
        }

        private static readonly Regex GoogleDriveFileRegex =
                new Regex(@"/file/d/(?<id>[^/]+)", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private static readonly Regex GoogleDriveIdRegex =
                new Regex(@"[?&]id=(?<id>[^&]+)", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private static readonly Regex WhitespaceRegex =
                new Regex(@"\s+", RegexOptions.Compiled);

        private static readonly Regex NonSlugRegex =
                new Regex("[^a-z0-9]+", RegexOptions.Compiled);

        private static readonly Regex VietnameseMarksRegex =
                new Regex("[đ]", RegexOptions.Compiled);

        private static string NormalizeWhitespace(string value) =>
                WhitespaceRegex.Replace(value, " ").Trim();

        public static string ParseAttachmentUrl(string detailHtml)
        {
            var doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(detailHtml);

            return doc.DocumentNode
                .SelectSingleNode(
                    "//pre[contains(concat(' ', normalize-space(@class), ' '), ' wp-block-preformatted ')]//a[@href]")
                ?.GetAttributeValue("href", null);
        }

        private static bool IsPdf(byte[] bytes) =>
                bytes.Length >= 4 &&
                bytes[0] == 0x25 &&
                bytes[1] == 0x50 &&
                bytes[2] == 0x44 &&
                bytes[3] == 0x46;

        private static string Slugify(string value)
        {
            var normalized = value.ToLowerInvariant();
            var withoutMarks = new string(normalized
                .Normalize(System.Text.NormalizationForm.FormD)
                .Where(ch => CharUnicodeInfo.GetUnicodeCategory(ch)
                    != UnicodeCategory.NonSpacingMark)
                .ToArray());

            var slug = NonSlugRegex.Replace(withoutMarks, "-").Trim('-');
            return string.IsNullOrWhiteSpace(slug) ? "tai-lieu" : slug;
        }

        private static string GuessExtension(HttpContentHeaders headers, byte[] bytes)
        {
            var mediaType = headers.ContentType?.MediaType?.ToLowerInvariant();

            if (mediaType == "application/pdf" || IsPdf(bytes))
            {
                return ".pdf";
            }

            if (mediaType == "application/vnd.openxmlformats-officedocument.wordprocessingml.document")
            {
                return ".docx";
            }

            if (mediaType == "application/msword")
            {
                return ".doc";
            }

            return ".bin";
        }

        private static string GuessExtension(HttpContentHeaders headers, byte[] bytes, string sourceUrl)
        {
            var mediaType = headers.ContentType?.MediaType?.ToLowerInvariant();

            if (mediaType == "application/pdf" || IsPdf(bytes))
            {
                return ".pdf";
            }

            if (mediaType == "application/vnd.openxmlformats-officedocument.wordprocessingml.document")
            {
                return ".docx";
            }

            if (mediaType == "application/msword")
            {
                return ".doc";
            }

            if (mediaType == "image/jpeg")
            {
                return ".jpg";
            }

            if (mediaType == "image/png")
            {
                return ".png";
            }

            if (mediaType == "image/webp")
            {
                return ".webp";
            }

            if (mediaType == "image/gif")
            {
                return ".gif";
            }

            var extension = Path.GetExtension(Uri.TryCreate(sourceUrl, UriKind.Absolute, out var uri)
                ? uri.AbsolutePath
                : sourceUrl);

            if (!string.IsNullOrWhiteSpace(extension) && extension.Length <= 6)
            {
                return extension;
            }

            return ".bin";
        }

        private static string TryGetGoogleDriveConfirmUrl(byte[] htmlBytes)
        {
            var doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(Encoding.UTF8.GetString(htmlBytes));

            var href = doc.DocumentNode
                .SelectSingleNode("//a[contains(@href, 'confirm=') and contains(@href, 'export=download')]")
                ?.GetAttributeValue("href", null);

            if (string.IsNullOrWhiteSpace(href))
            {
                return null;
            }

            return new Uri(new Uri("https://drive.google.com"), href).ToString();
        }
        private static bool IsHtml(HttpContentHeaders headers, byte[] bytes)
        {
            var mediaType = headers.ContentType?.MediaType?.ToLowerInvariant();
            if (mediaType == "text/html")
            {
                return true;
            }

            var sample = Encoding.UTF8.GetString(bytes.Take(Math.Min(bytes.Length, 256)).ToArray());
            return sample.Contains("<html");
        }
        private static bool IsGoogleDriveUrl(string url) =>
            Uri.TryCreate(url, UriKind.Absolute, out var uri) &&
            uri.Host.EndsWith("drive.google.com", StringComparison.OrdinalIgnoreCase);

        private static string ToDirectGoogleDriveUrl(string url)
        {
            var fileMatch = GoogleDriveFileRegex.Match(url);
            if (fileMatch.Success)
            {
                return $"https://drive.google.com/uc?export=download&id={fileMatch.Groups["id"].Value}";
            }

            var idMatch = GoogleDriveIdRegex.Match(url);
            if (idMatch.Success)
            {
                return $"https://drive.google.com/uc?export=download&id={idMatch.Groups["id"].Value}";
            }

            return null;
        }

        public static async Task<string> DownloadAttachmentAsync(
        HttpClient httpClient,
        string attachmentUrl,
        string downloadFolder,
        string title,
        CancellationToken cancellationToken = default)
        {
            try
            {
                Directory.CreateDirectory(downloadFolder);

                var downloadUrl = ToDirectGoogleDriveUrl(attachmentUrl) ?? attachmentUrl;
                using (var response = await httpClient.GetAsync(downloadUrl, cancellationToken))
                {
                    response.EnsureSuccessStatusCode();

                    var bytes = await response.Content.ReadAsByteArrayAsync();
                    var headers = response.Content.Headers;

                    if (IsGoogleDriveUrl(downloadUrl) && IsHtml(headers, bytes))
                    {
                        var confirmUrl = TryGetGoogleDriveConfirmUrl(bytes);
                        if (!string.IsNullOrWhiteSpace(confirmUrl))
                        {
                            using (var confirmedResponse = await httpClient.GetAsync(confirmUrl, cancellationToken))
                            {
                                confirmedResponse.EnsureSuccessStatusCode();
                                bytes = await confirmedResponse.Content.ReadAsByteArrayAsync();
                                headers = confirmedResponse.Content.Headers;
                            }
                        }
                    }

                    var extension = GuessExtension(headers, bytes);
                    var filePath = Path.Combine(downloadFolder, $"{Slugify(title)}{extension}");
                    File.WriteAllBytes(filePath, bytes);
                    return Path.GetFullPath(filePath);
                }
            }
            catch(Exception ex)
            {
                return string.Empty;
            }
        }
        #endregion

        private void btnCategoryApatit_Click(object sender, EventArgs e)
        {
            using (var db = new APATIT_CommonsServicesEntities())
            {
                var dbCategory = db.LoaiThongBaos.ToList();
                cboCategory.Items.Clear();
                cboCategory.DataSource = dbCategory;
                cboCategory.ValueMember = "Id";
                cboCategory.DisplayMember = "Title";
            }
        }

        public static async Task<string> DownloadThumbnailAsync(
            HttpClient httpClient,
            string thumbnailUrl,
            string downloadFolder,
            string title,
            CancellationToken cancellationToken = default)
        {
            Directory.CreateDirectory(downloadFolder);

            var bytesWithHeaders = await DownloadBytesAsync(httpClient, thumbnailUrl, cancellationToken);
            var extension = GuessExtension(bytesWithHeaders.Headers, bytesWithHeaders.Bytes, thumbnailUrl);
            var filePath = Path.Combine(downloadFolder, $"{Slugify(title)}-thumbnail{extension}");

            File.WriteAllBytes(filePath, bytesWithHeaders.Bytes);
            return Path.GetFullPath(filePath);
        }

        private static async Task<(byte[] Bytes, HttpContentHeaders Headers)> DownloadBytesAsync(
        HttpClient httpClient,
        string downloadUrl,
        CancellationToken cancellationToken)
        {
            using (var response = await httpClient.GetAsync(downloadUrl, cancellationToken))
            {
                response.EnsureSuccessStatusCode();

                var bytes = await response.Content.ReadAsByteArrayAsync();
                var headers = response.Content.Headers;

                if (IsGoogleDriveUrl(downloadUrl) && IsHtml(headers, bytes))
                {
                    var confirmUrl = TryGetGoogleDriveConfirmUrl(bytes);
                    if (!string.IsNullOrWhiteSpace(confirmUrl))
                    {
                        using (var confirmedResponse = await httpClient.GetAsync(confirmUrl, cancellationToken))
                        {
                            confirmedResponse.EnsureSuccessStatusCode();
                            bytes = await confirmedResponse.Content.ReadAsByteArrayAsync();
                            headers = confirmedResponse.Content.Headers;
                        }
                    }
                }

                return (bytes, headers);
            }
        }
    }

    public class BaiVietItem
    {
        public string SoKyHieu { get; set; } = "";

        public string TieuDe { get; set; } = "";

        public string NgayDang { get; set; } = "";

        public string FileUrl { get; set; }

        public string FileLocalPath { get; set; }

        public string LinkDeTail {  get; set; }

        public string Content {  get; set; }

        public string Mota {  get; set; }

        public string FileName {  get; set; }
    }

    #region dùng cho apatit
    public class ApatitDisclosureItem
    {
        public string DetailUrl { get; set; } = "";

        public string Title { get; set; } = "";

        public string ThumbnailUrl { get; set; } = "";

        public string AttachmentUrl { get; set; }

        public string LocalFilePath { get; set; }

        public string LocalThumbnailUrl { get; set; }
    }
    #endregion
}
