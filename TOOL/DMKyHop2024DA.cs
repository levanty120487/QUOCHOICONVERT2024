using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QHBASE;
using System.Configuration;
using System.Windows.Forms;
using System.Drawing.Design;
using System.Threading;
using System.Data.Entity.Validation;
namespace RJCodeUI_M1
{

    public class DMKyHop2024DA
    {


        public static List<DMKyHop> GetAllCatKyHop2024()
        {
            using (var context = new Preview_QuocHoi_KyHopServiceEntities())
            {
                // Get all items from the table
                var allItems = context.DMKyHops.ToList();
                return allItems;
            }

        }

        public static async Task InsertNDKyHop(string idCateMap, Guid idKyhop, Label lblcount, RichTextBox txtLog, Label thuchien)
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
                                var idnews = DMKyHop2024DA.CheckTrungND(itemN.OldID);
                                if (idnews != Guid.Empty)
                                {
                                    // Log khi hoàn thành một bản ghi
                                    txtLog.AppendText($"Processed OldID {itemN.OldID} - Updated existing news.\n");
                                    Application.DoEvents();
                                }
                                else
                                {
                                    var newItems = new NoiDung
                                    {
                                        Id = Guid.NewGuid(),
                                        Title = itemN.Title,
                                        ConcurrencyStamp = Guid.NewGuid().ToString(),
                                        PublicDate = itemN.CreateDate,
                                        DMKyHopId = idKyhop,
                                        Status = 3,
                                        Author = itemN.AuthorNews,
                                        ReadCount = UtilsBase.splitNumSP(itemN.ReadCount),
                                        CreationTime = Convert.ToDateTime(itemN.CreateDate),
                                        Description = itemN.Description,
                                        DescriptionSEO = itemN.Description,
                                        PageTitleSEO = itemN.Title,
                                        Source = itemN.SourceNews,
                                        ExtraProperties = "{}",
                                        OldId = Convert.ToInt32(itemN.OldID)
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

                                    // Chèn bài viết mới và bản ghi danh mục
                                    await InsertToDB(newItems, txtLog);

                                    // Log khi hoàn thành một bản ghi mới
                                    txtLog.AppendText($"Processed OldID {itemN.OldID} - Inserted new news.\n {itemN.Title}");
                                    Application.DoEvents();
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

        public static async Task InsertToDB(NoiDung newItemp, RichTextBox txtlog)
        {
            using (Preview_QuocHoi_KyHopServiceEntities db = new Preview_QuocHoi_KyHopServiceEntities())
            {
                try
                {
                    try
                    {
                        db.NoiDungs.Add(newItemp);
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

        public static Guid CheckTrungND(int? old)
        {
            using (Preview_QuocHoi_KyHopServiceEntities db = new Preview_QuocHoi_KyHopServiceEntities())
            {
                try
                {
                    // Kiểm tra old có giá trị không, nếu không thì bỏ qua điều kiện
                    //  var tintrung = db.News.Where(p => p.OldId.Equals(old)).FirstOrDefault();
                    var tintrung = db.NoiDungs.FirstOrDefault(p => p.OldId == old);
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

            return Guid.Empty; // Trả về giá trị mặc định (Guid.Empty) nếu không tìm thấy
        }
        public static Guid GetGuid(int old)
        {
            using (Preview_QuocHoi_KyHopServiceEntities db = new Preview_QuocHoi_KyHopServiceEntities())
            {

                try
                {
                    var result = db.DMKyHops.FirstOrDefault(p => p.OldId == old);
                    return result.Id;

                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }


        #region them chuong trinh lafm viec to CSM 2024
        public static async Task InsertCTLVKyHop(string idCateMap, Guid idKyhop, Label lblcount, RichTextBox txtLog, Label thuchien)
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
                                var idnews = DMKyHop2024DA.CheckTrungChuongTrinh(itemN.OldID);
                                if (idnews != Guid.Empty)
                                {
                                    // Log khi hoàn thành một bản ghi
                                    txtLog.AppendText($"Processed OldID {itemN.OldID} - Updated existing news.\n");
                                    Application.DoEvents();
                                }
                                else
                                {
                                    var newItems = new DMChuongTrinhLamViec
                                    {
                                        Id = Guid.NewGuid(),
                                        Title = itemN.Title,
                                        ConcurrencyStamp = Guid.NewGuid().ToString(),
                                        PublicDate = itemN.CreateDate,
                                        DMKyHopId = idKyhop,
                                        Status = 2,
                                        Type =2,
                                        IsDeleted = false,
                                        Author = "C1B327B8-10CB-4668-A6D1-680CDFE65720",
                                        ReadCount = UtilsBase.splitNumSP(itemN.ReadCount),
                                        CreationTime = Convert.ToDateTime(itemN.CreateDate),
                                        Description = itemN.Description,
                                        DescriptionSEO = itemN.Description,
                                        PageTitleSEO = itemN.Title,
                                        Source = itemN.SourceNews,
                                        ExtraProperties = "{}",
                                        OldId = Convert.ToInt32(itemN.OldID),
                                        Workday= itemN.CreateDate,
                                        CreatorId = new Guid("C1B327B8-10CB-4668-A6D1-680CDFE65720"),
                                    };

                                    // Tải ảnh bất đồng bộ
                                   

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

                                    // Chèn bài viết mới và bản ghi danh mục
                                    await InsertToChuongTrinhLV(newItems, txtLog);

                                    // Log khi hoàn thành một bản ghi mới
                                    txtLog.AppendText($"Processed OldID {itemN.OldID} - Inserted new news.\n {itemN.Title}");
                                    Application.DoEvents();
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

        public static Guid CheckTrungChuongTrinh(int? old)
        {
            using (Preview_QuocHoi_KyHopServiceEntities db = new Preview_QuocHoi_KyHopServiceEntities())
            {
                try
                {
                    // Kiểm tra old có giá trị không, nếu không thì bỏ qua điều kiện
                    //  var tintrung = db.News.Where(p => p.OldId.Equals(old)).FirstOrDefault();
                    var tintrung = db.DMChuongTrinhLamViecs.FirstOrDefault(p => p.OldId == old);
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

            return Guid.Empty; // Trả về giá trị mặc định (Guid.Empty) nếu không tìm thấy
        }
        public static async Task InsertToChuongTrinhLV(DMChuongTrinhLamViec newItemp, RichTextBox txtlog)
        {
            using (Preview_QuocHoi_KyHopServiceEntities db = new Preview_QuocHoi_KyHopServiceEntities())
            {
                try
                {
                    try
                    {
                        db.DMChuongTrinhLamViecs.Add(newItemp);
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

        #endregion
    }

}
