using QHBASE;
using QHBASEPHIENHOP;
using RJCodeUI_M1.Models;
using RJCodeUI_M1.SPAuthentication;
using RJCodeUI_M1.SPImage;
using RJCodeUI_M1.SPService;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace RJCodeUI_M1
{
    public static class UtilsBase
    {
        public static string ConverToString(this System.Xml.XmlNode node, int indentation)
        {
            using (var sw = new System.IO.StringWriter())
            {
                using (var xw = new System.Xml.XmlTextWriter(sw))
                {
                    xw.Formatting = System.Xml.Formatting.Indented;
                    xw.Indentation = indentation;
                    node.WriteContentTo(xw);
                }
                return sw.ToString();
            }
        }
        public static DataTable ConvertXmlToDataTable(XmlNode XmlValue)
        {
            DataSet listDataSet = new DataSet();
            System.Xml.XmlNodeReader listDataReader = new System.Xml.XmlNodeReader(XmlValue);
            listDataSet.ReadXml(listDataReader);
            if (listDataSet.Tables.Count > 1)
                return listDataSet.Tables[1];
            else
                return null;
        }
        public static DataTable ConvertXmlToDataTableAttachments(XmlNode XmlValue)
        {
            // Khởi tạo một DataSet để chứa dữ liệu từ XmlNode
            DataSet listDataSet = new DataSet();

            // Sử dụng XmlNodeReader để đọc dữ liệu từ XmlNode
            using (XmlNodeReader listDataReader = new XmlNodeReader(XmlValue))
            {
                // Đọc dữ liệu XML vào DataSet
                listDataSet.ReadXml(listDataReader);
            }

            // Kiểm tra nếu DataSet có chứa bảng nào
            if (listDataSet.Tables.Count > 0)
            {
                // Trả về bảng đầu tiên từ DataSet
                return listDataSet.Tables[0]; // Nếu bạn muốn bảng đầu tiên
            }
            else
            {
                // Nếu không có bảng nào, trả về null
                return null;
            }
        }
        public static int getLookup(string values)
        {
            if (!string.IsNullOrEmpty(values))
            {
                if (values.Contains('#') || values.Contains('#'))
                {
                    return Convert.ToInt32(values.Split('#')[0].Replace(";", ""));
                }
            }
            return 0;
        }

        public static List<int> getListLookup(string values)
        {
            if (!string.IsNullOrEmpty(values))
            {
                return values
                    .Split(new[] { ";#" }, StringSplitOptions.None)
                    .Select(p =>
                    {
                        if (int.TryParse(p, out var number))
                        {
                            return (int?)number; // Trả về giá trị số nếu chuyển đổi thành công
                        }
                        return null; // Trả về null nếu không chuyển đổi được
                    })
                    .Where(n => n.HasValue) // Lọc các giá trị không null
                    .Select(n => n.Value) // Chuyển đổi thành số nguyên
                    .ToList();
            }

            return null;
        }

        public static int splitNumSP(string values)
        {
            if (!string.IsNullOrEmpty(values))
            {
                if (values.Contains('.'))
                {
                    return Convert.ToInt32(values.Split('.')[0].Replace(";", ""));
                }
            }
            return 0;
        }

        // Hàm đệ quy để thêm các node con
        public static void AddChildNodes(TreeNode parentNode, List<NewsCat> categories)
        {
            int parentId = (int)parentNode.Tag;
            var childCategories = categories.Where(c => c.CatParentId == parentId).ToList();

            foreach (var childCategory in childCategories)
            {
                TreeNode childNode = new TreeNode(childCategory.CatName);
                childNode.Tag = childCategory.CatID;
                if (childNode.Nodes.Count > 0)
                {
                    childNode.NodeFont = new Font(parentNode.TreeView.Font, FontStyle.Bold);
                }

                parentNode.Nodes.Add(childNode);

                // Đệ quy gọi lại chính nó để thêm các node con
                AddChildNodes(childNode, categories);
            }
        }

        public static void AddChildNodesLinhVuc(TreeNode parentNode, List<TempLinhVuc> categories)
        {
            int parentId = (int)parentNode.Tag;
            var childCategories = categories.Where(c => c.ParentId == parentId).ToList();

            foreach (var childCategory in childCategories)
            {
                TreeNode childNode = new TreeNode(childCategory.Title);
                childNode.Tag = childCategory.LinhVucId;
                if (childNode.Nodes.Count > 0)
                {
                    childNode.NodeFont = new Font(parentNode.TreeView.Font, FontStyle.Bold);
                }

                parentNode.Nodes.Add(childNode);

                // Đệ quy gọi lại chính nó để thêm các node con
                AddChildNodesLinhVuc(childNode, categories);
            }
        }

        public static void AddChildNodesSQLLinhVuc(TreeNode parentNode, List<TempLinhVuc> categories)
        {
            int parentId = (int)parentNode.Tag;
            var childCategories = categories.Where(c => c.ParentId == parentId).ToList();

            foreach (var childCategory in childCategories)
            {
                TreeNode childNode = new TreeNode(childCategory.Title);
                childNode.Tag = childCategory.LinhVucOldId;
                parentNode.Nodes.Add(childNode);
                // Đệ quy gọi lại chính nó để thêm các node con
                AddChildNodesSQLLinhVuc(childNode, categories);
            }
        }

        public static void AddChildNodesCoQuanBanHanh(TreeNode parentNode, List<TempCoQuanBanHanh> categories)
        {
            int parentId = (int)parentNode.Tag;
            var childCategories = categories.Where(c => c.ParentId == parentId).ToList();

            foreach (var childCategory in childCategories)
            {
                TreeNode childNode = new TreeNode(childCategory.Title);
                childNode.Tag = childCategory.Id;
                if (childNode.Nodes.Count > 0)
                {
                    childNode.NodeFont = new Font(parentNode.TreeView.Font, FontStyle.Bold);
                }

                parentNode.Nodes.Add(childNode);

                // Đệ quy gọi lại chính nó để thêm các node con
                AddChildNodesCoQuanBanHanh(childNode, categories);
            }
        }

        public static void AddChildNodesSQLCoQuanBanHanh(TreeNode parentNode, List<TempCoQuanBanHanh> categories)
        {
            int parentId = (int)parentNode.Tag;
            var childCategories = categories.Where(c => c.ParentId == parentId).ToList();

            foreach (var childCategory in childCategories)
            {
                TreeNode childNode = new TreeNode(childCategory.Title);
                childNode.Tag = childCategory.OldId;
                parentNode.Nodes.Add(childNode);
                // Đệ quy gọi lại chính nó để thêm các node con
                AddChildNodesSQLCoQuanBanHanh(childNode, categories);
            }
        }


        public static void AddChildNodesVanKien(TreeNode parentNode, List<VanKienCat> categories)
        {
            int parentId = (int)parentNode.Tag;
            var childCategories = categories.Where(c => c.CatParentId == parentId).ToList();

            foreach (var childCategory in childCategories)
            {
                TreeNode childNode = new TreeNode(childCategory.CatName);
                childNode.Tag = childCategory.CatID;
                if (childNode.Nodes.Count > 0)
                {
                    childNode.NodeFont = new Font(parentNode.TreeView.Font, FontStyle.Bold);
                }

                parentNode.Nodes.Add(childNode);

                // Đệ quy gọi lại chính nó để thêm các node con
                AddChildNodesVanKien(childNode, categories);
            }
        }

        public static void AddChildNodesSQLVanKien(TreeNode parentNode, List<VanKienCat> categories)
        {
            int parentId = (int)parentNode.Tag;
            var childCategories = categories.Where(c => c.CatParentId == parentId).ToList();

            foreach (var childCategory in childCategories)
            {
                TreeNode childNode = new TreeNode(childCategory.CatName);
                childNode.Tag = childCategory.CatOldID;
                parentNode.Nodes.Add(childNode);
                // Đệ quy gọi lại chính nó để thêm các node con
                AddChildNodesSQLVanKien(childNode, categories);
            }
        }



        public static void AddChildNodesSQL(TreeNode parentNode, List<NewsCat> categories)
        {
            int parentId = (int)parentNode.Tag;
            var childCategories = categories.Where(c => c.CatParentId == parentId).ToList();

            foreach (var childCategory in childCategories)
            {
                TreeNode childNode = new TreeNode(childCategory.CatName + "------" + childCategory.CatOldID);
                childNode.Tag = childCategory.CatOldID;
                parentNode.Nodes.Add(childNode);
                // Đệ quy gọi lại chính nó để thêm các node con
                AddChildNodesSQL(childNode, categories);
            }
        }
        public static SPService.Lists GetListSV(string spsite, string Username, string Pass)
        {

            SPAuthentication.Authentication spAuthentication = new SPAuthentication.Authentication();
            spAuthentication.Url = string.Format("{0}/_vti_bin/Authentication.asmx", spsite);
            spAuthentication.CookieContainer = new CookieContainer();
            Lists spLists = new Lists();
            spLists.Url = string.Format("{0}/_vti_bin/Lists.asmx", spsite);
            //Try to login to SharePoint site with Form based authentication
            LoginResult loginResult = spAuthentication.Login(Username, Pass);
            Cookie cookie = new Cookie();
            //If login is successfull
            if (loginResult.ErrorCode == LoginErrorCode.NoError)
            {
                spLists.CookieContainer = spAuthentication.CookieContainer;
            }
            return spLists;
        }

        public static SPImage.Imaging GetListImages(string spsite, string Username, string Pass)
        {
            SPAuthentication.Authentication spAuthentication = new SPAuthentication.Authentication();
            spAuthentication.Url = string.Format("{0}/_vti_bin/Authentication.asmx", spsite);
            spAuthentication.CookieContainer = new CookieContainer();
            Imaging spListsImages = new Imaging();
            spListsImages.Url = string.Format("{0}/_vti_bin/Imaging.asmx", spsite);
            //Try to login to SharePoint site with Form based authentication
            LoginResult loginResult = spAuthentication.Login(Username, Pass);
            Cookie cookie = new Cookie();
            //If login is successfull
            if (loginResult.ErrorCode == LoginErrorCode.NoError)
            {
                spListsImages.CookieContainer = spAuthentication.CookieContainer;
            }
            return spListsImages;
        }
        // Hàm để tô đậm các node cha
        public static void BoldParentNodes(TreeView treeView)
        {
            foreach (TreeNode node in treeView.Nodes)
            {
                // Kiểm tra nếu node có node con (là node cha)
                if (node.Nodes.Count > 0)
                {
                    // Tô đậm node cha
                    node.NodeFont = new Font(treeView.Font, FontStyle.Bold);
                }

                // Gọi đệ quy để xử lý các node con
                BoldChildNodes(node);
            }
        }

        // Hàm đệ quy để tô đậm các node cha của các node con
        public static void BoldChildNodes(TreeNode parentNode)
        {
            foreach (TreeNode node in parentNode.Nodes)
            {
                if (node.Nodes.Count > 0)
                {
                    // Tô đậm node cha
                    node.NodeFont = new Font(parentNode.TreeView.Font, FontStyle.Bold);
                }

                // Đệ quy tiếp tục với các node con
                BoldChildNodes(node);
            }
        }

        public static bool ConvertIntToBool(int intValue)
        {
            // Trả về true nếu intValue khác 0, ngược lại false
            return intValue != 0;
        }

        public static string ConvertToUrlString(string input)
        {
            input = input.ToLower();

            // Loại bỏ dấu
            var normalizedString = input.Normalize(NormalizationForm.FormD);
            var stringBuilder = new System.Text.StringBuilder();
            foreach (char c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            // Chuyển đổi thành chuỗi không dấu
            string noDiacritics = stringBuilder.ToString();

            // Loại bỏ ký tự đặc biệt và thay thế khoảng trắng bằng dấu gạch nối
            string urlString = Regex.Replace(noDiacritics, @"[^a-z0-9]", "-");
            urlString = Regex.Replace(urlString, @"-+", "-").Trim('-');

            return urlString;
        }
        public static async Task<string> GetMimeTypeFromUrl(string url)
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    HttpResponseMessage response = await client.GetAsync(url);
                    if (response.IsSuccessStatusCode)
                    {
                        return response.Content.Headers.ContentType.ToString();
                    }
                    else
                    {
                        Console.WriteLine("Failed to retrieve file. Status code: " + response.StatusCode);
                        return null;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("An error occurred: " + ex.Message);
                    return null;
                }
            }
        }
        public static Guid GuidFromString(string input)
        {
            byte[] stringbytes = System.Text.Encoding.UTF8.GetBytes(input);
            byte[] hashedBytes = new System.Security.Cryptography.SHA1CryptoServiceProvider().ComputeHash(stringbytes);
            Array.Resize(ref hashedBytes, 16); // Resize to match GUID format size
            return new Guid(hashedBytes);
        }

        public static string ConvertURLKyHop(string tieudekyhop) 
        {
            string urlnoidung = string.Empty;
            if (tieudekyhop.Contains("Thông cáo"))
            {
                urlnoidung = "thong-cao-ky-hop";
            }
            else if (tieudekyhop.Contains("Văn kiện tài liệu"))
            {
                urlnoidung = "van-kien-tai-lieu";
            }
            else if (tieudekyhop.Contains("Trả lời chất vấn"))
            {
                urlnoidung = "tra-loi-chat-van-ky-hop";
            }
            else if (tieudekyhop.Contains("Biên bản ghi âm thảo luận tại hội trường"))
            {
                urlnoidung = "bien-ban-ghi-am-ky-hop";
            }
            else if (tieudekyhop.Contains("Tin ảnh hoạt động") || tieudekyhop.Contains("Hình ảnh") || tieudekyhop.ToLower().Contains("hoạt động") )
            {
                urlnoidung = "tin-anh-hoat-dong-ky-hop";
            }
            else if (tieudekyhop.Contains("Biên bản ghi âm"))
            {
                urlnoidung = "bien-ban-ghi-am-ky-hop";
            }
            else if (tieudekyhop.Contains("Chương trình làm việc"))
            {
                urlnoidung = "chuong-trinh-lam-viec-ky-hop";
            }
            else if (tieudekyhop.Contains("Thông tin khác"))
            {
                urlnoidung = "thong-tin-khac-ky-hop";
            }
           
            return urlnoidung;
        }

        //public static string ConvertURLPhienHop(string tieudekyhop)
        //{
        //    string urlnoidung = string.Empty;
        //    if (tieudekyhop.Contains("Thông cáo"))
        //    {
        //        urlnoidung = "thong-cao-phien-hop";
        //    }
        //    else if (tieudekyhop.Contains("Văn kiện tài liêu")
        //        || tieudekyhop.Contains("Văn kiện tài liệu") 
        //        || tieudekyhop.Contains("Văn kiên tài liệu"))
        //    {
        //        urlnoidung = "van-kien-tai-lieu-phien-hop";
        //    }
        //    else if (tieudekyhop.Contains("Nghị quyết"))
        //    {
        //        urlnoidung = "nghi-quyet-phien-hop";
        //    }
        //    else if (tieudekyhop.Contains("Ảnh hoạt động")||)
        //    {
        //        urlnoidung = "tin-anh-hoat-dong-phien-hop";
        //    }
        //    else if (tieudekyhop.Contains("Tin ảnh hoạt động") || tieudekyhop.Contains("Hình ảnh") || tieudekyhop.ToLower().Contains("hoạt động"))
        //    {
        //        urlnoidung = "tin-anh-hoat-dong-ky-hop";
        //    }
        //    else if (tieudekyhop.Contains("Biên bản ghi âm"))
        //    {
        //        urlnoidung = "bien-ban-ghi-am-ky-hop";
        //    }
        //    else if (tieudekyhop.Contains("Chương trình làm việc"))
        //    {
        //        urlnoidung = "chuong-trinh-lam-viec-phien-hop";
        //    }
        //    else if (tieudekyhop.Contains("Thông tin khác"))
        //    {
        //        urlnoidung = "thong-tin-khac-ky-hop";
        //    }

        //    return urlnoidung;
        //}
    }
}
