using QHBASE;
using RJCodeUI_M1.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Reflection;
using System.Data;
using System.Xml;

namespace RJCodeUI_M1
{
    public class NewsDA
    {

        public static async Task InsertNews(New newItemp, NewCategory cateItem, NewTypeNew typenewmap, RichTextBox txtlog)
        {
            using (CTTDTCDK_NewsServicesEntities db = new CTTDTCDK_NewsServicesEntities())
            {
                try
                {
                    try
                    {
                        db.News.Add(newItemp);
                        db.NewCategories.Add(cateItem);
                        if (typenewmap.TypeNewId != Guid.Empty)
                        {
                            db.NewTypeNews.Add(typenewmap);
                        }

                        // Lưu thay đổi vào cơ sở dữ liệu một cách bất đồng bộ
                        await db.SaveChangesAsync();
                        txtlog.AppendText($"them moi ban ghi {newItemp.Title} \n");

                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message);
                    }
                }
                catch (DbEntityValidationException e)
                {
                    foreach (var eve in e.EntityValidationErrors)
                    {
                        Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                            eve.Entry.Entity.GetType().Name, eve.Entry.State);
                        foreach (var ve in eve.ValidationErrors)
                        {
                            Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                                ve.PropertyName, ve.ErrorMessage);
                        }
                    }
                    throw e;
                }
            }
        }
        public static void InsertCateNews(string idcate, string idnews, int? oldID)
        {
            using (CTTDTCDK_NewsServicesEntities db = new CTTDTCDK_NewsServicesEntities())
            {
                try
                {
                    try
                    {
                        var checkcateNew = db.NewCategories.Where(p => p.NewId.Equals(idnews) && p.CategoryId.Equals(idcate)).ToList();
                        if (checkcateNew.Count == 0)
                        {
                            // Tạo đối tượng liên kết New và Category
                            var catenew = new NewCategory
                            {
                                Id = Guid.NewGuid(),
                                NewId = idnews,
                                CategoryId = idcate,
                                OldNewId = oldID.ToString(),
                            };
                            //   cateItem.NewId = aaa;
                            db.NewCategories.Add(catenew);
                            db.SaveChanges();
                        }
                        else
                        {
                            Console.WriteLine(string.Format("ban ghi da ton tai: {0} | {1}", oldID, idnews));
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message);
                    }
                }
                catch (DbEntityValidationException e)
                {
                    foreach (var eve in e.EntityValidationErrors)
                    {
                        Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                            eve.Entry.Entity.GetType().Name, eve.Entry.State);
                        foreach (var ve in eve.ValidationErrors)
                        {
                            Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                                ve.PropertyName, ve.ErrorMessage);
                        }
                    }
                    throw e;
                }
            }
        }
        public static async Task InsertTypeNews(Guid idtype, string idnews, int? oldID)
        {
            using (CTTDTCDK_NewsServicesEntities db = new CTTDTCDK_NewsServicesEntities())
            {
                try
                {
                    var checktypeNew = db.NewTypeNews.Where(p => p.NewId.Equals(idnews) && p.TypeNewId.Equals(idtype)).ToList();
                    if (checktypeNew.Count == 0)
                    {
                        // Tạo đối tượng liên kết New và Category
                        var typenew = new NewTypeNew
                        {
                            Id = Guid.NewGuid(),
                            NewId = idnews,
                            TypeNewId = idtype,
                            OldId = oldID.ToString(),
                        };
                        db.NewTypeNews.Add(typenew);
                        db.SaveChanges();
                    }
                    else
                    {
                        Console.WriteLine(string.Format("ban ghi da ton tai: {0} | {1}", oldID, idnews));
                    }


                }
                catch (DbEntityValidationException e)
                {
                    foreach (var eve in e.EntityValidationErrors)
                    {
                        Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                            eve.Entry.Entity.GetType().Name, eve.Entry.State);
                        foreach (var ve in eve.ValidationErrors)
                        {
                            Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                                ve.PropertyName, ve.ErrorMessage);
                        }
                    }
                    throw;
                }
            }
        }
        public static string GetGuid(string old)
        {
            using (CTTDTCDK_NewsServicesEntities db = new CTTDTCDK_NewsServicesEntities())
            {

                try
                {
                    var result = db.Categories.FirstOrDefault(p => p.CategoryOldId == old);
                    return result.Id;
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }

        public static void InsertNew2024(string idCateMap, string idcate, Label lblcount)
        {
            using (QuocHoiVNEntities db = new QuocHoiVNEntities())
            {

                try
                {
                    int count = 0;
                    var result = db.News.Where(p => p.ModerationStatus == 0 
                                    && !string.IsNullOrEmpty(p.Category) 
                                        && (p.Category.Contains(string.Concat(",", idCateMap, ",")) || p.Category.StartsWith(string.Concat(idCateMap, ",")) || p.Category.EndsWith(string.Concat(",", idCateMap)) || p.Category.Equals(idCateMap))).ToList();
                    foreach (var itemN in result)
                    {
                        var newItems = new New();
                        newItems.Id = QHCommons.GenAutoId();
                        newItems.Title = itemN.Title;
                        newItems.ConcurrencyStamp = newItems.Id.ToString();
                        newItems.Content = itemN.ContentNew;
                        newItems.DatePublic = itemN.CreateDate;
                        newItems.Status = 6;
                        newItems.Author = itemN.AuthorNews;
                        newItems.ReadCount = UtilsBase.splitNumSP(itemN.ReadCount);
                        // newItems.CommentNews = UtilsBase.ConvertIntToBool((Convert.ToInt32(itemN.IsShowView))) ;
                        newItems.CreationTime = Convert.ToDateTime(itemN.CreateDate);
                        newItems.Description = itemN.Description;
                        newItems.DescriptionSEO = itemN.Description;
                        newItems.PageTitleSEO = itemN.Title;
                        newItems.Hot = UtilsBase.ConvertIntToBool(Convert.ToInt32(itemN.Hotnews));
                        //string savedImagePath = await ImageDownloader.DownloadImageAsyncurl(itemN.Image, "/var/www/mywebsite")
                        newItems.Image =
                        newItems.Source = itemN.SourceNews;
                        newItems.DatePublic = itemN.CreateDate;
                        newItems.Shared = true;
                        newItems.Language = "vi";
                        newItems.ExtraProperties = "{}";
                        newItems.OldId = Convert.ToString(itemN.OldID);
                        // Insert Maps 2 bang category va new
                        var catenew = new NewCategory();
                        catenew.Id = Guid.NewGuid();
                        catenew.NewId = newItems.Id;
                        catenew.CategoryId = idcate;
                        catenew.OldNewId = itemN.OldID.ToString();
                        //  InsertNews(newItems, catenew);
                        count++;
                    }
                    lblcount.Text = "Count: " + count.ToString();
                }
                catch (DbEntityValidationException e)
                {
                    foreach (var eve in e.EntityValidationErrors)
                    {
                        Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                            eve.Entry.Entity.GetType().Name, eve.Entry.State);
                        foreach (var ve in eve.ValidationErrors)
                        {
                            Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                                ve.PropertyName, ve.ErrorMessage);
                        }
                    }
                    throw;
                }
            }
        }

        public static string CheckTrungTIn(int? old)
        {
            using (CTTDTCDK_NewsServicesEntities db = new CTTDTCDK_NewsServicesEntities())
            {
                try
                {
                    // Kiểm tra old có giá trị không, nếu không thì bỏ qua điều kiện
                    //  var tintrung = db.News.Where(p => p.OldId.Equals(old)).FirstOrDefault();
                    var tintrung = db.News.FirstOrDefault(p => p.OldId == old.ToString());
                    if (tintrung != null)
                    {
                        return tintrung.Id; // Trả về Id của tin tìm thấy
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }

            }

            return string.Empty; // Trả về giá trị mặc định (Guid.Empty) nếu không tìm thấy
        }

        public static string CheckTrungTInvpqh(string old)
        {
            using (CTTDTCDK_NewsServicesEntities db = new CTTDTCDK_NewsServicesEntities())
            {
                try
                {
                    // Kiểm tra old có giá trị không, nếu không thì bỏ qua điều kiện
                    //  var tintrung = db.News.Where(p => p.OldId.Equals(old)).FirstOrDefault();
                    var tintrung = db.News.FirstOrDefault(p => p.OldId == old.ToString());
                    if (tintrung != null)
                    {
                        return tintrung.Id; // Trả về Id của tin tìm thấy
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }

            }

            return string.Empty; // Trả về giá trị mặc định (Guid.Empty) nếu không tìm thấy
        }

        public static Guid getIdType(int? old)
        {
            try
            {
                using (CTTDTCDK_NewsServicesEntities db = new CTTDTCDK_NewsServicesEntities())
                {
                    // Kiểm tra old có giá trị không, nếu không thì bỏ qua điều kiện
                    //  var tintrung = db.News.Where(p => p.OldId.Equals(old)).FirstOrDefault();
                    var tintrung = db.TypeNews.FirstOrDefault(p => p.CategoryOldId == old.ToString());
                    if (tintrung != null)
                    {
                        return tintrung.Id; // Trả về Id của tin tìm thấy
                    }

                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return Guid.Empty; // Trả về giá trị mặc định (Guid.Empty) nếu không tìm thấy


        }
        public static async Task InsertNew2024_old(string idCateMap, string idcate, Label lblcount, RichTextBox txtLog, Label thuchien)
        {


            using (QuocHoiVNEntities db = new QuocHoiVNEntities())
            {
                try
                {
                    int count = 0;
                    int countthuchien = 0;
                    var idloaitin = Guid.Empty;
                    // Tìm các bài viết cần xử lý
                    /* var result = db.News.Where(p => p.ModerationStatus == 0 &&
                                                     !string.IsNullOrEmpty(p.Category) &&
                                                     (p.Category.Contains(string.Concat(",", idCateMap, ",")) ||
                                                      p.Category.StartsWith(string.Concat(idCateMap, ",")) ||
                                                      p.Category.EndsWith(string.Concat(",", idCateMap)) ||
                                                      p.Category.Equals(idCateMap)) &&
                                                      (p.OldID == 76783)).ToList();
                    */
                    var result = db.News.Where(p => p.ModerationStatus == 0 &&
                                                    !string.IsNullOrEmpty(p.Category) &&
                                                    (p.Category.Contains(string.Concat(",", idCateMap, ",")) ||
                                                     p.Category.StartsWith(string.Concat(idCateMap, ",")) ||
                                                     p.Category.EndsWith(string.Concat(",", idCateMap)) ||
                                                     p.Category.Equals(idCateMap))).ToList();
                    lblcount.Text = result.Count.ToString();

                    foreach (var itemN in result)
                    {

                        var idnews = NewsDA.CheckTrungTIn(itemN.OldID);
                        if (!string.IsNullOrWhiteSpace(idnews))
                        {
                            if (!string.IsNullOrEmpty(itemN.IdLoaiTin.Trim()) && !(itemN.IdLoaiTin.Contains(",")))
                            {
                                var idloaitin1 = getIdType(Convert.ToInt32(itemN.IdLoaiTin));
                                InsertTypeNews(idloaitin1, idnews, itemN.OldID);
                            }
                            else if (itemN.IdLoaiTin.Contains(","))
                            {
                                // Cắt chuỗi thành mảng các phần tử, phân cách bởi dấu ','
                                string[] parts = itemN.IdLoaiTin.Split(',');
                                // Sử dụng foreach để lặp qua các phần tử trong mảng
                                foreach (string part in parts)
                                {
                                    idloaitin = getIdType(Convert.ToInt32(part));
                                    InsertTypeNews(idloaitin, idnews, itemN.OldID);
                                }
                            }
                            InsertCateNews(idcate, idnews, itemN.OldID);

                            countthuchien++;
                            thuchien.Text = "Count: " + countthuchien.ToString();
                        }
                        else
                        {

                            var newItems = new New
                            {
                                Id = QHCommons.GenAutoId(),
                                Title = itemN.Title,
                                ConcurrencyStamp = Guid.NewGuid().ToString(),
                                // Content = itemN.ContentNew,
                                DatePublic = itemN.CreateDate,
                                Status = 6,
                                Author = itemN.AuthorNews,
                                ReadCount = UtilsBase.splitNumSP(itemN.ReadCount),
                                CreationTime = Convert.ToDateTime(itemN.CreateDate),
                                Description = itemN.Description,
                                DescriptionSEO = itemN.Description,
                                PageTitleSEO = itemN.Title,
                                Hot = UtilsBase.ConvertIntToBool(Convert.ToInt32(itemN.Hotnews)),
                                Source = itemN.SourceNews,
                                Shared = true,
                                Language = "vi",
                                ExtraProperties = "{}",
                                OldId = Convert.ToString(itemN.OldID)
                            };
                            // Gọi hàm tải ảnh bất đồng bộ và lưu đường dẫn ảnh
                            #region  doownloaf anh
                            if (!string.IsNullOrEmpty(itemN.Image))
                            {
                                string savedImagePath = await ImageDownloader.DownloadImageAsyncurl(itemN.Image.Trim());
                                newItems.Image = savedImagePath;

                            }
                            #endregion
                            #region anh trong noi dung tin
                            if (!string.IsNullOrEmpty(itemN.ContentNew))
                            {
                                HtmlAgilityPack.HtmlDocument newsDocument = new HtmlAgilityPack.HtmlDocument();
                                newsDocument.LoadHtml(itemN.ContentNew);
                                var ImagesNode = newsDocument.DocumentNode.SelectNodes(".//img");
                                if (ImagesNode != null && ImagesNode.Count > 0)
                                {
                                    foreach (var img in ImagesNode)
                                    {
                                        if (img.Attributes.Count > 1)
                                        {
                                            img.Attributes["src"].Value = await ImageDownloader.DownloadImageAsyncurl(img.Attributes["src"].Value.Trim());
                                        }
                                    }
                                    newItems.Content = newsDocument.DocumentNode.OuterHtml;
                                }
                                else { newItems.Content = itemN.ContentNew; }
                                // newItems.Content = itemN.ContentNew;
                            }
                            #endregion

                            // Tạo đối tượng liên kết New và Category
                            var catenew = new NewCategory
                            {
                                Id = Guid.NewGuid(),
                                NewId = newItems.Id,
                                CategoryId = idcate,
                                OldNewId = itemN.OldID.ToString()
                            };
                            var typenew = new NewTypeNew
                            {
                                Id = Guid.NewGuid(),
                                NewId = newItems.Id,
                                TypeNewId = idloaitin,
                                OldId = newItems.OldId.ToString()
                            };

                            // InsertCateNews(idcate, idnews, itemN.OldID);
                            // Gọi hàm chèn bài viết mới và bản ghi danh mục
                            InsertNews(newItems, catenew, typenew, txtLog);

                            if (!string.IsNullOrEmpty(itemN.IdLoaiTin.Trim()) && !(itemN.IdLoaiTin.Contains(",")))
                            {
                                var idloaitin1 = getIdType(Convert.ToInt32(itemN.IdLoaiTin));
                                InsertTypeNews(idloaitin1, newItems.Id, itemN.OldID);
                            }
                            else if (itemN.IdLoaiTin.Contains(","))
                            {
                                // Cắt chuỗi thành mảng các phần tử, phân cách bởi dấu ','
                                string[] parts = itemN.IdLoaiTin.Split(',');
                                // Sử dụng foreach để lặp qua các phần tử trong mảng
                                foreach (string part in parts)
                                {
                                    idloaitin = getIdType(Convert.ToInt32(part));
                                    InsertTypeNews(idloaitin, newItems.Id, itemN.OldID);
                                }
                            }
                            countthuchien++;
                            thuchien.Text = "Count: " + countthuchien.ToString();
                        }


                    }
                }
                catch (Exception e)
                {
                    throw e;
                }
            }

        }

        public static async Task InsertNew2024_old2(string idCateMap, string idcate, Label lblcount, RichTextBox txtLog, Label thuchien)
        {
            using (QuocHoiVNEntities db = new QuocHoiVNEntities())
            {
                try
                {
                    int countthuchien = 0;
                    object lockObj = new object(); // Khóa cho biến đếm
                    var idloaitin = Guid.Empty;

                    // Tìm các bài viết cần xử lý
                    var result = db.News.Where(p => p.ModerationStatus == 0 &&
                                                    !string.IsNullOrEmpty(p.Category) &&
                                                    (p.Category.Contains(string.Concat(",", idCateMap, ",")) ||
                                                     p.Category.StartsWith(string.Concat(idCateMap, ",")) ||
                                                     p.Category.EndsWith(string.Concat(",", idCateMap)) ||
                                                     p.Category.Equals(idCateMap))).ToList();

                    lblcount.Text = result.Count.ToString();

                    var tasks = result.Select(async itemN =>
                    {
                        var idnews = NewsDA.CheckTrungTIn(itemN.OldID);
                        if (!string.IsNullOrWhiteSpace(idnews))
                        {
                            // Xử lý IdLoaiTin
                            var loaiTinTasks = ProcessIdLoaiTin(itemN.IdLoaiTin, idnews, itemN.OldID);

                            // Chèn danh mục cho bài viết
                            InsertCateNews(idcate, idnews, itemN.OldID);

                            await loaiTinTasks;
                        }
                        else
                        {
                            var newItems = new New
                            {
                                Id =QHCommons.GenAutoId(),
                                Title = itemN.Title,
                                ConcurrencyStamp = Guid.NewGuid().ToString(),
                                DatePublic = itemN.CreateDate,
                                Status = 6,
                                Author = itemN.AuthorNews,
                                ReadCount = UtilsBase.splitNumSP(itemN.ReadCount),
                                CreationTime = Convert.ToDateTime(itemN.CreateDate),
                                Description = itemN.Description,
                                DescriptionSEO = itemN.Description,
                                PageTitleSEO = itemN.Title,
                                Hot = UtilsBase.ConvertIntToBool(Convert.ToInt32(itemN.Hotnews)),
                                Source = itemN.SourceNews,
                                Shared = true,
                                Language = "vi",
                                ExtraProperties = "{}",
                                OldId = Convert.ToString(itemN.OldID)
                            };

                            // Tải ảnh bất đồng bộ
                            if (!string.IsNullOrEmpty(itemN.Image))
                            {
                                newItems.Image = await ImageDownloader.DownloadImageAsyncurl(itemN.Image.Trim());
                            }

                            // Xử lý ảnh trong nội dung tin
                            if (!string.IsNullOrEmpty(itemN.ContentNew))
                            {
                                HtmlAgilityPack.HtmlDocument newsDocument = new HtmlAgilityPack.HtmlDocument();
                                newsDocument.LoadHtml(itemN.ContentNew);
                                var imagesNode = newsDocument.DocumentNode.SelectNodes(".//img");
                                if (imagesNode != null)
                                {
                                    foreach (var img in imagesNode)
                                    {
                                        if (img.Attributes["src"] != null)
                                        {
                                            img.Attributes["src"].Value = await ImageDownloader.DownloadImageAsyncurl(img.Attributes["src"].Value.Trim());
                                        }
                                    }
                                }
                                newItems.Content = newsDocument.DocumentNode.OuterHtml;
                            }

                            // Tạo đối tượng liên kết New và Category
                            var catenew = new NewCategory
                            {
                                Id = Guid.NewGuid(),
                                NewId = newItems.Id,
                                CategoryId = idcate,
                                OldNewId = itemN.OldID.ToString()
                            };
                            var typenew = new NewTypeNew
                            {
                                Id = Guid.NewGuid(),
                                NewId = newItems.Id,
                                TypeNewId = idloaitin,
                                OldId = newItems.OldId.ToString()
                            };

                            // Chèn bài viết mới và bản ghi danh mục
                            InsertNews(newItems, catenew, typenew, txtLog);
                           
                            // Xử lý IdLoaiTin
                            await ProcessIdLoaiTin(itemN.IdLoaiTin, newItems.Id, itemN.OldID);

                        }

                        // Khóa và tăng biến countthuchien
                        lock (lockObj)
                        {
                            countthuchien++;
                        }

                        // Cập nhật UI sau mỗi lần thực hiện
                        thuchien.Invoke((Action)(() =>
                        {
                            thuchien.Text = "Count: " + countthuchien.ToString();
                        }));
                    }).ToList();

                    // Đợi tất cả các tác vụ hoàn thành
                    await Task.WhenAll(tasks);
                    thuchien.Text = $"Count: {countthuchien}"; // Hiển thị số bản ghi đã thực hiện thành công
                    txtLog.AppendText($"Processing completed. Total records processed: {countthuchien}\n");
                }
                catch (Exception ex)
                {
                    txtLog.AppendText($"Error in processing: {ex.Message}\n");
                }
            }
        }

        public static async Task InsertNew20241(string idCateMap, string idcate, Label lblcount, RichTextBox txtLog, Label thuchien)
        {
            using (QuocHoiVNEntities db = new QuocHoiVNEntities())
            {
                try
                {
                    int countthuchien = 0; // Biến đếm số lượng bản ghi đã thực hiện thành công
                    var idloaitin = Guid.Empty;
                    object lockObj = new object(); // Khóa cho biến đếm
                    // Tìm các bài viết cần xử lý
                    var result = db.News.Where(p => p.ModerationStatus == 0 &&
                                                    !string.IsNullOrEmpty(p.Category) &&
                                                    (p.Category.Contains(string.Concat(",", idCateMap, ",")) ||
                                                     p.Category.StartsWith(string.Concat(idCateMap, ",")) ||
                                                     p.Category.EndsWith(string.Concat(",", idCateMap)) ||
                                                     p.Category.Equals(idCateMap))).ToList();

                    lblcount.Text = result.Count.ToString();

                    var tasks = result.Select(async itemN =>
                    {
                        try
                        {
                            var idnews = NewsDA.CheckTrungTIn(itemN.OldID);
                            if (!string.IsNullOrWhiteSpace(idnews))
                            {
                                // Xử lý IdLoaiTin
                                await ProcessIdLoaiTin(itemN.IdLoaiTin, idnews, itemN.OldID);

                                // Chèn danh mục cho bài viết
                                InsertCateNews(idcate, idnews, itemN.OldID);

                                // Log khi hoàn thành một bản ghi
                                txtLog.AppendText($"Processed OldID {itemN.OldID} - Updated existing news.\n");
                            }
                            else
                            {
                                var newItems = new New
                                {
                                    Id = QHCommons.GenAutoId(),
                                    Title = itemN.Title,
                                    ConcurrencyStamp = Guid.NewGuid().ToString(),
                                    DatePublic = itemN.CreateDate,
                                    Status = 6,
                                    Author = itemN.AuthorNews,
                                    ReadCount = UtilsBase.splitNumSP(itemN.ReadCount),
                                    CreationTime = Convert.ToDateTime(itemN.CreateDate),
                                    Description = itemN.Description,
                                    DescriptionSEO = itemN.Description,
                                    PageTitleSEO = itemN.Title,
                                    Hot = UtilsBase.ConvertIntToBool(Convert.ToInt32(itemN.Hotnews)),
                                    Source = itemN.SourceNews,
                                    Shared = true,
                                    Language = "vi",
                                    ExtraProperties = "{}",
                                    OldId = Convert.ToString(itemN.OldID)
                                };

                                // Tải ảnh bất đồng bộ
                                if (!string.IsNullOrEmpty(itemN.Image))
                                {
                                    newItems.Image = await ImageDownloader.DownloadImageAsyncurl(itemN.Image.Trim());
                                }

                                // Xử lý ảnh trong nội dung tin
                                if (!string.IsNullOrEmpty(itemN.ContentNew))
                                {
                                    HtmlAgilityPack.HtmlDocument newsDocument = new HtmlAgilityPack.HtmlDocument();
                                    newsDocument.LoadHtml(itemN.ContentNew);
                                    var imagesNode = newsDocument.DocumentNode.SelectNodes(".//img");
                                    if (imagesNode != null)
                                    {
                                        foreach (var img in imagesNode)
                                        {
                                            if (img.Attributes["src"] != null)
                                            {
                                                img.Attributes["src"].Value = await ImageDownloader.DownloadImageAsyncurl(img.Attributes["src"].Value.Trim());
                                            }
                                        }
                                    }
                                    newItems.Content = newsDocument.DocumentNode.OuterHtml;
                                }

                                // Tạo đối tượng liên kết New và Category
                                var catenew = new NewCategory
                                {
                                    Id = Guid.NewGuid(),
                                    NewId = newItems.Id,
                                    CategoryId = idcate,
                                    OldNewId = itemN.OldID.ToString()
                                };
                                var typenew = new NewTypeNew
                                {
                                    Id = Guid.NewGuid(),
                                    NewId = newItems.Id,
                                    TypeNewId = idloaitin,
                                    OldId = newItems.OldId.ToString()
                                };

                                // Chèn bài viết mới và bản ghi danh mục
                                InsertNews(newItems, catenew, typenew, txtLog);

                                // Xử lý IdLoaiTin
                                await ProcessIdLoaiTin(itemN.IdLoaiTin, newItems.Id, itemN.OldID);

                                // Log khi hoàn thành một bản ghi mới
                                txtLog.AppendText($"Processed OldID {itemN.OldID} - Inserted new news.\n");
                            }
                            // Khóa và tăng biến countthuchien
                            lock (lockObj)
                            {
                                countthuchien++;
                            }

                            // Cập nhật UI sau mỗi lần thực hiện
                            thuchien.Invoke((Action)(() =>
                            {
                                thuchien.Text = "Count: " + countthuchien.ToString();
                            }));
                            // Tăng biến đếm nếu hoàn thành không lỗi
                            Interlocked.Increment(ref countthuchien); // Đảm bảo tăng biến đếm trong môi trường đa luồng
                        }
                        catch (Exception ex)
                        {
                            // Log lỗi nếu có
                            txtLog.AppendText($"Error processing OldID {itemN.OldID}: {ex.Message}\n");
                        }
                    }).ToList();

                    // Đợi tất cả các tác vụ hoàn thành
                    await Task.WhenAll(tasks);

                    // Cập nhật giao diện khi hoàn tất
                    thuchien.Text = $"Count: {countthuchien}"; // Hiển thị số bản ghi đã thực hiện thành công
                    txtLog.AppendText($"Processing completed. Total records processed: {countthuchien}\n");
                }
                catch (Exception ex)
                {
                    // Log lỗi tổng thể nếu xảy ra
                    txtLog.AppendText($"Error in processing: {ex.Message}\n");
                }
            }
        }

        public static async Task InsertNew2024_3(string idCateMap, string idcate, Label lblcount, RichTextBox txtLog, Label thuchien)
        {
            using (QuocHoiVNEntities db = new QuocHoiVNEntities())
            {
                try
                {
                    int countthuchien = 0; // Biến đếm số lượng bản ghi đã thực hiện thành công
                    var idloaitin = Guid.Empty;
                    int batchSize = 500; // Kích thước của mỗi batch
                    int maxConcurrency = 100; // Số lượng tác vụ chạy đồng thời tối đa
                    SemaphoreSlim semaphore = new SemaphoreSlim(maxConcurrency);

                    // Tìm các bài viết cần xử lý
                    var result = db.News.Where(p => p.ModerationStatus == 0 &&
                                                    !string.IsNullOrEmpty(p.Category) &&
                                                    (p.Category.Contains(string.Concat(",", idCateMap, ",")) ||
                                                     p.Category.StartsWith(string.Concat(idCateMap, ",")) ||
                                                     p.Category.EndsWith(string.Concat(",", idCateMap)) ||
                                                     p.Category.Equals(idCateMap))).ToList();

                    lblcount.Text = result.Count.ToString();
                    Application.DoEvents();
                    // Chia nhỏ danh sách bản ghi thành các batch
                    var batches = result.Select((item, index) => new { item, index })
                                        .GroupBy(x => x.index / batchSize)
                                        .Select(g => g.Select(x => x.item).ToList())
                                        .ToList();

                    foreach (var batch in batches)
                    {
                        var tasks = batch.Select(async itemN =>
                        {
                            await semaphore.WaitAsync(); // Giới hạn số tác vụ đồng thời

                            try
                            {
                                var idnews = NewsDA.CheckTrungTIn(itemN.OldID);
                                if (!string.IsNullOrWhiteSpace(idnews))
                                {
                                    // Xử lý IdLoaiTin
                                    await ProcessIdLoaiTin(itemN.IdLoaiTin, idnews, itemN.OldID);

                                    //Chèn danh mục cho bài viết
                                    InsertCateNews(idcate, idnews, itemN.OldID);

                                    // Log khi hoàn thành một bản ghi
                                    txtLog.AppendText($"Processed OldID {itemN.OldID} - Updated existing news.\n");
                                }
                                else
                                {
                                    var newItems = new New
                                    {
                                        Id = QHCommons.GenAutoId(),
                                        Title = itemN.Title,
                                        ConcurrencyStamp = Guid.NewGuid().ToString(),
                                        DatePublic = itemN.CreateDate,
                                        Status = 6,
                                        Author = itemN.AuthorNews,
                                        ReadCount = UtilsBase.splitNumSP(itemN.ReadCount),
                                        CreationTime = Convert.ToDateTime(itemN.CreateDate),
                                        Description = itemN.Description,
                                        DescriptionSEO = itemN.Description,
                                        PageTitleSEO = itemN.Title,
                                        Hot = UtilsBase.ConvertIntToBool(Convert.ToInt32(itemN.Hotnews)),
                                        Source = itemN.SourceNews,
                                        Shared = true,
                                        Language = "vi",
                                        ExtraProperties = "{}",
                                        OldId = Convert.ToString(itemN.OldID)
                                    };

                                    // Tải ảnh bất đồng bộ
                                    if (!string.IsNullOrEmpty(itemN.Image))
                                    {
                                        newItems.Image = await ImageDownloader.DownloadImageAsyncurl(itemN.Image.Trim());
                                    }

                                    // Xử lý ảnh trong nội dung tin
                                    if (!string.IsNullOrEmpty(itemN.ContentNew))
                                    {
                                        HtmlAgilityPack.HtmlDocument newsDocument = new HtmlAgilityPack.HtmlDocument();
                                        newsDocument.LoadHtml(itemN.ContentNew);
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
                                        newItems.Content = newsDocument.DocumentNode.OuterHtml;
                                    }

                                    // Tạo đối tượng liên kết New và Category
                                    var catenew = new NewCategory
                                    {
                                        Id = Guid.NewGuid(),
                                        NewId = newItems.Id,
                                        CategoryId = idcate,
                                        OldNewId = itemN.OldID.ToString()
                                    };
                                    var typenew = new NewTypeNew
                                    {
                                        Id = Guid.NewGuid(),
                                        NewId = newItems.Id,
                                        TypeNewId = idloaitin,
                                        OldId = newItems.OldId.ToString()
                                    };

                                    // Chèn bài viết mới và bản ghi danh mục
                                    await InsertNews(newItems, catenew, typenew, txtLog);

                                    // Xử lý IdLoaiTin
                                    await ProcessIdLoaiTin(itemN.IdLoaiTin, newItems.Id, itemN.OldID);

                                    if (itemN.Attachment == 1)
                                    {
                                        GetAttachmentSp(newItems.Id, itemN.OldID.ToString());
                                    }

                                    // Log khi hoàn thành một bản ghi mới
                                    txtLog.AppendText($"Processed OldID {itemN.OldID} - Inserted new news.\n {itemN.Title}");
                                }
                                countthuchien++;
                                if (countthuchien % 100 == 0) // Cập nhật giao diện mỗi khi xử lý 100 bản ghi
                                {
                                    thuchien.Text = $"Count: {countthuchien}";
                                    // Tăng biến đếm nếu hoàn thành không lỗi
                                    // Interlocked.Increment(ref countthuchien); // Đảm bảo tăng biến đếm trong môi trường đa luồng
                                    Application.DoEvents(); // Đảm bảo cập nhật giao diện
                                }
                                else
                                {
                                    thuchien.Text = $"Count: {countthuchien}";
                                    // Tăng biến đếm nếu hoàn thành không lỗi
                                    // Interlocked.Increment(ref countthuchien); // Đảm bảo tăng biến đếm trong môi trường đa luồng
                                    Application.DoEvents(); // Đảm bảo cập nhật giao diện}
                                }
                            }
                            catch (Exception ex)
                            {
                                // Log lỗi nếu có
                                txtLog.AppendText($"Error processing OldID {itemN.OldID}: {ex.Message}\n");
                            }
                            finally
                            {
                                semaphore.Release(); // Giải phóng semaphore sau khi hoàn thành tác vụ
                            }
                        }).ToList();

                        // Đợi tất cả các tác vụ trong batch hoàn thành
                        await Task.WhenAll(tasks);

                        txtLog.AppendText($"Batch completed. Total records processed so far: {countthuchien}\n");
                    }

                    // Cập nhật giao diện khi hoàn tất toàn bộ
                    txtLog.AppendText($"Processing completed. Total records processed: {countthuchien}\n");
                }
                catch (Exception ex)
                {
                    // Log lỗi tổng thể nếu xảy ra
                    txtLog.AppendText($"Error in processing: {ex.Message}\n");
                }
            }
        }


        public static async Task InsertNew2024_vpqh(string idCateMap, string idcate, Label lblcount, RichTextBox txtLog, Label thuchien, int oldIdFrom, int oldIdTo)
        {
            using (QuocHoiVNEntities db = new QuocHoiVNEntities())
            {
                try
                {
                    int countthuchien = 0; // Biến đếm số lượng bản ghi đã thực hiện thành công
                    var idloaitin = Guid.Empty;
                    int batchSize = 500; // Kích thước của mỗi batch
                    int maxConcurrency = 100; // Số lượng tác vụ chạy đồng thời tối đa
                    SemaphoreSlim semaphore = new SemaphoreSlim(maxConcurrency);

                    // Tìm các bài viết cần xử lý
                    var result = db.News.Where(p => p.ModerationStatus == 0 && !string.IsNullOrEmpty(p.Category) 
                                                    && p.OldID >= oldIdFrom && p.OldID <= oldIdTo
                                                        && (p.Category.Contains(string.Concat(";#", idCateMap, ";#")) ||
                                                         p.Category.StartsWith(string.Concat(idCateMap, ";#")) ||
                                                         p.Category.EndsWith(string.Concat(";#", idCateMap)) ||
                                                         p.Category.Equals(idCateMap)))
                                                    .ToList();

                    //var result = db.News.Where(p => p.ModerationStatus == 0 &&
                    //                             (p.OldID >= 10719)).ToList();

                    lblcount.Text = result.Count.ToString();
                    Application.DoEvents();
                    // Chia nhỏ danh sách bản ghi thành các batch
                    var batches = result.Select((item, index) => new { item, index })
                                        .GroupBy(x => x.index / batchSize)
                                        .Select(g => g.Select(x => x.item).ToList())
                                        .ToList();

                    foreach (var batch in batches)
                    {
                        var tasks = batch.Select(async itemN =>
                        {
                            await semaphore.WaitAsync(); // Giới hạn số tác vụ đồng thời

                            try
                            {
                                //var idnews = NewsDA.CheckTrungTInvpqh(itemN.OldID.ToString());
                                //if (!string.IsNullOrWhiteSpace(idnews))
                                //{
                                    // Xử lý IdLoaiTin
                                   // await ProcessIdLoaiTin(itemN.IdLoaiTin, idnews, itemN.OldID);

                                    //Chèn danh mục cho bài viết
                                   // InsertCateNews(idcate, idnews, itemN.OldID);

                                    // Log khi hoàn thành một bản ghi
                                    //txtLog.AppendText($"Processed OldID {itemN.OldID} - Updated existing news.\n");
                               // }
                                //else
                                //{
                                    var newItems = new New
                                    {
                                        Id = QHCommons.GenAutoId(),
                                        Title = itemN.Title,
                                        ConcurrencyStamp = Guid.NewGuid().ToString(),
                                        DatePublic = itemN.CreateDate,
                                        Status = 6,
                                        Author = itemN.AuthorNews,
                                        ReadCount = UtilsBase.splitNumSP(itemN.ReadCount),
                                        CreationTime = Convert.ToDateTime(itemN.Created),
                                        Description = itemN.Description,
                                        DescriptionSEO = itemN.Description,
                                        PageTitleSEO = itemN.Title,
                                        Hot = UtilsBase.ConvertIntToBool(Convert.ToInt32(itemN.Hotnews)),
                                        Source = itemN.SourceNews,
                                        Shared = true,
                                        Language = "vi",
                                        ExtraProperties = "{}",
                                        OldId = Convert.ToString(itemN.OldID),
                                        TypeNewContent = 3, //=> tin bai
                                        TypeNewId = Guid.Parse("41E8E719-30A7-4365-845A-CA1D8C91970F"), //=> tin bai
                                        CreatorName = itemN.Author.Split('#').Last(),
                                        ShowDescription = true,
                                        AllowComment = itemN.AllowComment.HasValue && itemN.AllowComment.Value == 1 ? true : false
                                    };

                                    // Tải ảnh bất đồng bộ
                                    if (!string.IsNullOrEmpty(itemN.Image))
                                    {
                                        newItems.Image = await ImageDownloader.DownloadImageAsyncurl(itemN.Image.Trim());
                                    }

                                    // Xử lý ảnh trong nội dung tin
                                    if (!string.IsNullOrEmpty(itemN.ContentNew))
                                    {
                                        HtmlAgilityPack.HtmlDocument newsDocument = new HtmlAgilityPack.HtmlDocument();
                                        newsDocument.LoadHtml(itemN.ContentNew);
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
                                        newItems.Content = newsDocument.DocumentNode.OuterHtml;
                                    }

                                    // Tạo đối tượng liên kết New và Category
                                    var catenew = new NewCategory
                                    {
                                        Id = Guid.NewGuid(),
                                        NewId = newItems.Id,
                                        CategoryId = idcate,
                                        OldNewId = itemN.OldID.ToString()
                                    };

                                    //var typenew = new NewTypeNew
                                    //{
                                    //    Id = Guid.NewGuid(),
                                    //    NewId = newItems.Id,
                                    //    TypeNewId = idloaitin,
                                    //    OldId = newItems.OldId.ToString()
                                    //};

                                    var typenew = new NewTypeNew
                                    {
                                        Id = Guid.Empty,
                                        NewId = newItems.Id,
                                        TypeNewId = idloaitin,
                                        OldId = newItems.OldId.ToString()
                                    };

                                    // Chèn bài viết mới và bản ghi danh mục
                                    await InsertNews(newItems, catenew, typenew, txtLog);

                                    // Xử lý IdLoaiTin
                                    // await ProcessIdLoaiTin(itemN.IdLoaiTin, newItems.Id, itemN.OldID);

                                    if (itemN.Attachment == 1)
                                    {
                                        GetAttachmentSp(newItems.Id, itemN.OldID.ToString());
                                    }

                                    // Log khi hoàn thành một bản ghi mới
                                    txtLog.AppendText($"Processed OldID {itemN.OldID} - Inserted new news.\n {itemN.Title}");
                               //}
                                countthuchien++;
                                if (countthuchien % 100 == 0) // Cập nhật giao diện mỗi khi xử lý 100 bản ghi
                                {
                                    thuchien.Text = $"Count: {countthuchien}";
                                    // Tăng biến đếm nếu hoàn thành không lỗi
                                    // Interlocked.Increment(ref countthuchien); // Đảm bảo tăng biến đếm trong môi trường đa luồng
                                    Application.DoEvents(); // Đảm bảo cập nhật giao diện
                                }
                                else
                                {
                                    thuchien.Text = $"Count: {countthuchien}";
                                    // Tăng biến đếm nếu hoàn thành không lỗi
                                    // Interlocked.Increment(ref countthuchien); // Đảm bảo tăng biến đếm trong môi trường đa luồng
                                    Application.DoEvents(); // Đảm bảo cập nhật giao diện}
                                }
                            }
                            catch (Exception ex)
                            {
                                // Log lỗi nếu có
                                txtLog.AppendText($"Error processing OldID {itemN.OldID}: {ex.Message}\n");
                            }
                            finally
                            {
                                semaphore.Release(); // Giải phóng semaphore sau khi hoàn thành tác vụ
                            }
                        }).ToList();

                        // Đợi tất cả các tác vụ trong batch hoàn thành
                        await Task.WhenAll(tasks);

                        txtLog.AppendText($"Batch completed. Total records processed so far: {countthuchien}\n");
                    }

                    // Cập nhật giao diện khi hoàn tất toàn bộ
                    txtLog.AppendText($"Processing completed. Total records processed: {countthuchien}\n");
                }
                catch (Exception ex)
                {
                    // Log lỗi tổng thể nếu xảy ra
                    txtLog.AppendText($"Error in processing: {ex.Message}\n");
                }
            }
        }

        private static async Task ProcessIdLoaiTin(string idLoaiTin, string idnews, int? oldID)
        {
            if (!string.IsNullOrEmpty(idLoaiTin.Trim()))
            {
                if (!idLoaiTin.Contains(","))
                {
                    var idloaitin = getIdType(Convert.ToInt32(idLoaiTin));
                    InsertTypeNews(idloaitin, idnews, oldID);
                }
                else
                {
                    // Chia chuỗi IdLoaiTin thành mảng và xử lý
                    string[] parts = idLoaiTin.Split(',');
                    var tasks = parts.Select(part =>
                    {
                        var idloaitin = getIdType(Convert.ToInt32(part));
                        return InsertTypeNews(idloaitin, idnews, oldID);
                    }).ToList();

                    await Task.WhenAll(tasks);
                }
            }
        }


        public static async void GetAttachmentSp(string idVanKien, string OldIdVankien)
        {
            try
            {
                String txtSiteUrl = "https://noidung.quochoi.vn/content/tintuc/Lists/News";
                string txtUsername = "Administrator";
                string txtPassword = "AdminCTTDT@2021@)@!";
                string ListNewsName = "TinBai";

                string stSite = txtSiteUrl.Substring(0, txtSiteUrl.LastIndexOf('/'))
                                                .ToLower()
                                                .Replace("/lists", string.Empty)
                                                .Replace("/tt", "");

                var listSV = UtilsBase.GetListSV(stSite, txtUsername, txtPassword);

                XmlNode ndAttach = listSV.GetAttachmentCollection(ListNewsName, OldIdVankien);
                var TableFull = UtilsBase.ConvertXmlToDataTableAttachments(ndAttach);
                await InsertAttachment(TableFull, idVanKien, Convert.ToInt32(OldIdVankien));
                //  MessageBox.Show("đã xong");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"*** ERR: {ex.Message}\r\n");
            }
        }
        public static async Task InsertAttachment(DataTable entities, string iditem, int? oldId)
        {
            using (CTTDTCDK_FilesServicesEntities db = new CTTDTCDK_FilesServicesEntities())
            {
                var totalattach = entities.Rows.Count;
                string urlattch = string.Empty;
                foreach (DataRow item in entities.Rows)
                {
                    try
                    {
                        CMSFile itemFile = new CMSFile();
                        itemFile.Id = Guid.NewGuid();
                        itemFile.FileType = 2;
                        itemFile.CreationTime = DateTime.Now;
                        itemFile.FileContainerName = "CMSContainerPublic";
                        itemFile.OwnerUserId = UtilsBase.GuidFromString("F740E5FE-5086-0314-4926-3A14B5173E2F");
                        itemFile.ConcurrencyStamp = itemFile.Id.ToString();
                        itemFile.CreatorId = UtilsBase.GuidFromString("F740E5FE-5086-0314-4926-3A14B5173E2F");
                        itemFile.ParentId = UtilsBase.GuidFromString("5B272A5C-0D5C-0575-DB73-3A1BF76AA17D");
                        itemFile.ExtraProperties = "{}";
                        itemFile.MimeType = "image/jpeg";
                        if (totalattach > 1)
                        {
                            urlattch = Convert.ToString(item["Attachment_Text"]);
                        }
                        else
                        {
                            urlattch = Convert.ToString(item["Attachment"]);
                        }

                        #region Download file
                        if (urlattch != null)
                        {
                            List<string> LtsFileDownload = new List<string>() {
                                ".doc",
                                ".docx",
                                ".pdf",
                                ".xls",
                                ".xlsx",
                                ".zip",
                                ".ppt",
                                ".pptx",
                                ".rar",
                                ".bmp"
                            };
                            List<string> LtsFileImage = new List<string>() {
                                "image/jpeg",
                                "image/png",
                                "image/webp",
                                "image/gif",
                                "image/svg+xml"
                            };

                            List<string> ListDocment = new List<string>()
                        {
                            "application/xml","application / pdf",
                            "application/zip",
                            "application/x-www-form-urlencoded",
                            "application/vnd.ms-excel",
                            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                            "application/msword",
                            "application/vnd.openxmlformats-officedocument.wordprocessingml.document"
                        };
                            var urlfile = urlattch;
                            // var fileType = Convert.ToString(item["ows_Title"]).Substring(urlfile.LastIndexOf("."));

                            var fileName = urlattch.Substring(urlfile.LastIndexOf("/"));
                            var fileType = urlattch.Substring(urlfile.LastIndexOf("."));
                            //var mimetype = await UtilsBase.GetMimeTypeFromUrl(urlfile);
                            if (LtsFileDownload.Contains(fileType))
                            {
                                itemFile.FullPathServer = await ImageDownloader.DownloadDataFile(urlfile);
                                itemFile.FileExtention = 3;
                                itemFile.FileName = fileName;
                            }
                        }
                        #endregion
                        itemFile.OldId = oldId.ToString();
                        db.CMSFiles.Add(itemFile);
                        db.SaveChanges();

                        // await InsertFileCMS(iditem, itemFile.Id, oldId);
                    }
                    catch (DbEntityValidationException e)
                    {
                        foreach (var eve in e.EntityValidationErrors)
                        {
                            Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                                eve.Entry.Entity.GetType().Name, eve.Entry.State);
                            foreach (var ve in eve.ValidationErrors)
                            {
                                Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                                    ve.PropertyName, ve.ErrorMessage);
                            }
                        }
                        throw e;
                    }
                }

            }
        }

        #region customs
        #endregion
    }
}
