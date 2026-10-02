using QHBASE;
using RJCodeUI_M1.SPService;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Services.Description;
using System.Web.UI.WebControls;
using System.Windows.Forms;
using System.Xml.Linq;
namespace RJCodeUI_M1
{
    public class ImageDownloader
    {
        private static readonly HttpClient httpClient = new HttpClient();



        public static async Task<string> DownloadImageAsyncurl1(string imageUrl)
        {
            try
            {
                string hosturl = "https://noidung.quochoi.vn";
                string rootDirectoty = "D:\\uploadFckFiles";
                string fullurl = hosturl + imageUrl;
                // Lấy tên file từ URL
                string fileName = Path.GetFileName(hosturl + imageUrl);

                // Loại bỏ ký tự '/' ở đầu (nếu có)
                string relativePath = imageUrl.TrimStart('/');

                // Tạo đường dẫn đầy đủ để lưu ảnh trên máy chủ
                string fullDirectoryPath = Path.Combine(rootDirectoty, Path.GetDirectoryName(relativePath) ?? string.Empty);

                // Tạo thư mục nếu chưa tồn tại
                if (!Directory.Exists(fullDirectoryPath))
                {
                    Directory.CreateDirectory(fullDirectoryPath);
                }

                // Đường dẫn đầy đủ của file
                string fullFilePath = Path.Combine(fullDirectoryPath, fileName);
                string fullFilePathreturn = "/uploadFckFiles" + imageUrl.Trim();
                // Gửi yêu cầu tải ảnh
                byte[] imageBytes = await httpClient.GetByteArrayAsync(fullurl);

                // Lưu ảnh vào thư mục
                File.WriteAllBytes(fullFilePath, imageBytes);

                Console.WriteLine($"Đã tải và lưu ảnh vào: {fullFilePath}");

                // Trả về đường dẫn đầy đủ của file sau khi lưu
                return fullFilePathreturn;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Có lỗi xảy ra: {ex.Message}");
                return null;
            }
        }


        public static async Task<string> DownloadImageAsyncurl(string imageUrl)
        {
            try
            {
                string hosturl = "https://moet.gov.vn";
                string rootDirectory = "C:\\uploadFckFiles\\tinhdBGD";
                #region tên file/ đường dẫn
                var fileName = string.Empty;
                var fileUrl = imageUrl;
                #endregion
                #region lọc html nếu tồn tại
                try
                {
                    var docContent = new HtmlAgilityPack.HtmlDocument();
                    docContent.LoadHtml(imageUrl);
                    var imgNodes = docContent.DocumentNode.SelectNodes(".//img");
                    if (imgNodes != null)
                    {
                        fileUrl = imgNodes.FirstOrDefault().Attributes["src"]?.Value;
                    }
                }
                catch { }
                #endregion
                string fullUrl = hosturl + fileUrl;
                string fullFilePath = fileUrl.ToString();
                string relativePath = fileUrl.TrimStart('/');
                // Tạo một đối tượng Uri từ URL
                //Uri uri = new Uri(imageUrl);
                // Lấy tên file từ URL
                fileName = Path.GetFileName(fullUrl);
                if (imageUrl.Contains("https") || imageUrl.Contains("http"))
                {
                    // Tạo một đối tượng Uri từ URL
                    Uri uri = new Uri(imageUrl);

                    // Loại bỏ ký tự '/' ở đầu (nếu có)
                    relativePath = uri.PathAndQuery.TrimStart('/');
                    fullUrl = imageUrl.ToString();

                }
                relativePath = relativePath.Replace("%20", "");

                // Giải mã URL (ví dụ: %C3%AD -> í)
                fileName = Uri.UnescapeDataString(fileName).Replace("%20", "");
                
                // Chuyển tiếng Việt có dấu thành không dấu
                string normalizedStr = fileName.Normalize(System.Text.NormalizationForm.FormD);
                fileName = new System.Text.RegularExpressions.Regex("\\p{IsCombiningDiacriticalMarks}+").Replace(normalizedStr, string.Empty);
                fileName = fileName.Replace('đ', 'd').Replace('Đ', 'D');

                // Thay thế các ký tự không phải chữ, số, dấu chấm, gạch ngang thành dấu _
                fileName = System.Text.RegularExpressions.Regex.Replace(fileName, "[^a-zA-Z0-9.\\-_]", "_");

                if (fileName.Contains("_RenditionID"))
                {
                    fileName = fileName.Substring(0, fileName.IndexOf("_RenditionID"));
                }
                
                if (relativePath.Contains("_RenditionID"))
                {
                    relativePath = relativePath.Substring(0, relativePath.IndexOf("_RenditionID"));
                }
                // Tạo đường dẫn đầy đủ để lưu ảnh trên máy chủ
                string fullDirectoryPath = Path.Combine(rootDirectory, Path.GetDirectoryName(relativePath) ?? string.Empty);

                // Tạo thư mục nếu chưa tồn tại
                if (!Directory.Exists(fullDirectoryPath))
                {
                    Directory.CreateDirectory(fullDirectoryPath);
                }

                fullFilePath = Path.Combine(fullDirectoryPath, fileName);

                // Đường dẫn đầy đủ của file

                string fullFilePathReturn = "/uploadFckFiles/tinhdBGD/" + relativePath.Trim();

                // Kiểm tra nếu file đã tồn tại
                if (File.Exists(fullFilePath))
                {
                    Console.WriteLine($"File đã tồn tại: {fullFilePath}");
                    return fullFilePathReturn; // Trả về nếu file đã tồn tại
                }

                // Gửi yêu cầu tải ảnh
                byte[] imageBytes = await httpClient.GetByteArrayAsync(fullUrl);

                // Lưu ảnh vào thư mục
                File.WriteAllBytes(fullFilePath, imageBytes);

                Console.WriteLine($"Đã tải và lưu ảnh vào: {fullFilePath}");

                // Trả về đường dẫn đầy đủ của file sau khi lưu
                return fullFilePathReturn;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Có lỗi xảy ra: {ex.Message}");
                return null;
            }
        }


        public static async Task<string> DownloadDataFile(string imageUrl)
        {
            try
            {
                string rootDirectory = "C:\\uploadFckFiles\\tinhdBGD";
                string relativePath = imageUrl.TrimStart('/');
                string fileName = Path.GetFileName(imageUrl);
                Uri uri = new Uri(imageUrl);

                // Loại bỏ ký tự '/' ở đầu (nếu có)
                relativePath = uri.PathAndQuery.TrimStart('/');
                string fullDirectoryPath = Path.Combine(rootDirectory, Path.GetDirectoryName(relativePath) ?? string.Empty);

                // Tạo thư mục nếu chưa tồn tại
                if (!Directory.Exists(fullDirectoryPath))
                {
                   
                    Directory.CreateDirectory(fullDirectoryPath);
                }

                string fullFilePath = Path.Combine(fullDirectoryPath, fileName);
                string fullFilePathReturn = "/uploadFckFiles/tinhdBGD/" + relativePath.Trim();

                if (File.Exists(fullFilePath))
                {
                    Console.WriteLine($"File đã tồn tại: {fullFilePath}");
                    return fullFilePathReturn;
                }

                using (WebClient client = new WebClient())
                {
                    try
                    {

                        byte[] fileFromSourceData = client.DownloadData(imageUrl); //Download file đính kèm
                        File.WriteAllBytes(fullFilePath, fileFromSourceData);
                    }
                    catch
                    {

                    }
                }

                return fullFilePathReturn;
            }
            catch (HttpRequestException httpEx)
            {
                Console.WriteLine($"Lỗi kết nối HTTP: {httpEx.Message}");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Có lỗi xảy ra: {ex.Message}");
                return null;
            }
        }

        #region backup
        public static async Task<string> DownloadImageAsyncurlBackup(string imageUrl)
        {
            try
            {
                string hosturl = "https://www.vr.org.vn";
                string rootDirectory = "D:\\uploadFckFiles";
                #region tên file/ đường dẫn
                var fileName = string.Empty;
                var fileUrl = imageUrl;
                #endregion
                #region lọc html nếu tồn tại
                try
                {
                    var docContent = new HtmlAgilityPack.HtmlDocument();
                    docContent.LoadHtml(imageUrl);
                    var imgNodes = docContent.DocumentNode.SelectNodes(".//img");
                    if (imgNodes != null)
                    {
                        fileUrl = imgNodes.FirstOrDefault().Attributes["src"]?.Value;
                    }
                }
                catch { }
                #endregion
                string fullUrl = hosturl + fileUrl;
                string fullFilePath = imageUrl.ToString();
                string relativePath = imageUrl.TrimStart('/');
                // Tạo một đối tượng Uri từ URL
                //Uri uri = new Uri(imageUrl);
                // Lấy tên file từ URL
                fileName = Path.GetFileName(fullUrl);
                if (imageUrl.Contains("https") || imageUrl.Contains("http"))
                {
                    // Tạo một đối tượng Uri từ URL
                    Uri uri = new Uri(imageUrl);

                    // Loại bỏ ký tự '/' ở đầu (nếu có)
                    relativePath = uri.PathAndQuery.TrimStart('/');
                    fullUrl = imageUrl.ToString();

                }
                // Tạo đường dẫn đầy đủ để lưu ảnh trên máy chủ
                string fullDirectoryPath = Path.Combine(rootDirectory, Path.GetDirectoryName(relativePath) ?? string.Empty);

                // Tạo thư mục nếu chưa tồn tại
                if (!Directory.Exists(fullDirectoryPath))
                {
                    Directory.CreateDirectory(fullDirectoryPath);
                }

                fullFilePath = Path.Combine(fullDirectoryPath, fileName);

                // Đường dẫn đầy đủ của file

                string fullFilePathReturn = "/uploadFckFiles/" + relativePath.Trim();

                // Kiểm tra nếu file đã tồn tại
                if (File.Exists(fullFilePath))
                {
                    Console.WriteLine($"File đã tồn tại: {fullFilePath}");
                    return fullFilePathReturn; // Trả về nếu file đã tồn tại
                }

                // Gửi yêu cầu tải ảnh
                byte[] imageBytes = await httpClient.GetByteArrayAsync(fullUrl);

                // Lưu ảnh vào thư mục
                File.WriteAllBytes(fullFilePath, imageBytes);

                Console.WriteLine($"Đã tải và lưu ảnh vào: {fullFilePath}");

                // Trả về đường dẫn đầy đủ của file sau khi lưu
                return fullFilePathReturn;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Có lỗi xảy ra: {ex.Message}");
                return null;
            }
        }
        #endregion
    }
}
