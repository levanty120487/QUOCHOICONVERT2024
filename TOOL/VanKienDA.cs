using QHBASE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RJCodeUI_M1
{
    public class VanKienDA
    {

        public static Guid InsertVanKienTL(VanKienTaiLieu item)
        {
            using (Preview_QuocHoi_KyHopServiceEntities db = new Preview_QuocHoi_KyHopServiceEntities())
            {
                // Tìm danh mục theo OldId
                var kyhop = db.VanKienTaiLieus.Add(item);

                // Lưu thay đổi vào cơ sở dữ liệu
                db.SaveChanges();
                return kyhop.Id;

            }
        }

        #region get Guid Lĩnh vực của CMS Quốc hội
        public static Guid? GetGuidDmLinhVucCMSQuocHoi(string linhVuc)
        {
            Guid? result = null;
            if (!string.IsNullOrEmpty(linhVuc))
            {
                // lấy ra lĩnh vực Id của trang quốc hội.vn
                var linhVucId = UtilsBase.getLookup(linhVuc);
                // get lĩnh vực theo Id của trang quốc hội. => tên lĩnh vực đó
              
                // so sánh tên vừa lấy với DB CMSQuocHoi để lấy ra ID lĩnh vực của CMS Quốc hội
                result = GetGuidLinhVuc(linhVucId);
            }
            return result;
        }

        public static Guid? GetGuidLinhVuc(int? linhvuc)
        {
            Guid? result = null;
            if (!string.IsNullOrEmpty(linhvuc.ToString()))
            {
                using (var context = new Preview_QuocHoi_CommonsServiceEntities())
                {
                    
                    var item = context.LinhVucs.Where(p =>p.OldID == linhvuc).FirstOrDefault();
                    if (item != null)
                    {
                        //result = item.Id;
                    }
                }
            }
            return result;
        }

        #endregion


        #region Get Guid loại văn kiện và kỳ họp của CMS Quốc hội
        //private void GetKyHopIdLoaiVanKienId(string loaiVanKien, VanKienTaiLieu vanKien)
        //{
        //    if (!string.IsNullOrEmpty(loaiVanKien))
        //    {
        //        // tìm tên văn kiện và parentId để tìm ra Kỳ họp.
        //        var loaiVanKienId = UtilsBase.getLookup(loaiVanKien);
        //        var tenLoaiVanKien = string.Empty;
        //        Guid? loaiVanKienGuid = null;
        //        using (var context = new QuocHoiVNEntities())
        //        {
        //            var allItems = context.VanKienCats.ToList();
        //            var item = context.VanKienCats.Where(x => x.CatOldID == loaiVanKienId).FirstOrDefault();
        //            if (item != null)
        //            {
        //                tenLoaiVanKien = item.CatName;
        //                // tìm Guid loại văn kiện của CMS Quốc hội theo Tên văn kiện vừa tìm được.
        //                loaiVanKienGuid = this.GetGuidLoaiVanKien(tenLoaiVanKien);

        //                // add loại văn kiện Guid vào CMS Quốc hội.
        //                vanKien.LoaiVanKienId = loaiVanKienGuid;

        //                // add DmKyHopId
        //                // lấy ra tên loại văn kiện của parentID => để so sánh với Tên Kỳ Họp của CMS Quốc Hội
        //                if (item.CatParentId != null && item.CatParentId > 0)
        //                {
        //                    var kyHopItem = allItems.Where(x => x.CatOldID == item.CatParentId).FirstOrDefault();
        //                    if (kyHopItem.CatParentId != null && kyHopItem.CatParentId > 0)
        //                    {
        //                        var khoaItem = allItems.Where(x => x.CatOldID == kyHopItem.CatParentId).FirstOrDefault();
        //                        //
        //                        var kyHopGuid = this.GetGuidKyHop(kyHopItem, khoaItem);
        //                        if (kyHopGuid != null)
        //                        {
        //                            vanKien.DMKyHopId = kyHopGuid.Value;
        //                            //using (var contextKH = new Preview_QuocHoi_KyHopServiceEntities())
        //                            //{
        //                            //    vanKien.DMKyHop = contextKH.DMKyHops.Where(x=>x.Id == vanKien.DMKyHopId).FirstOrDefault();
        //                            //}
        //                        }
        //                        else
        //                        {
        //                            txtLog.Text = "Không có kỳ họp của văn kiện.";
        //                        }
        //                    }
        //                    else
        //                    {
        //                        txtLog.Text = "Không có Khóa họp của văn kiện.";
        //                    }
        //                }
        //                else
        //                {
        //                    txtLog.Text = "Không có Kỳ họp của văn kiện.";
        //                }

        //            }

        //        }
        //    }
        //    // return vanKien;
        //}


        private void GetKyHopIdLoaiVanKienId(string loaiVanKien, VanKienTaiLieu vanKien)
        {
            if (string.IsNullOrEmpty(loaiVanKien)) return;

            var loaiVanKienId = UtilsBase.getLookup(loaiVanKien);
            using (var context = new QuocHoiVNEntities())
            {
                var item = context.VanKienCats.FirstOrDefault(x => x.CatOldID == loaiVanKienId);
                if (item == null)
                {
                   // txtLog.Text += "- Không tìm thấy văn kiện.";
                    return;
                }

                var tenLoaiVanKien = item.CatName;
                var loaiVanKienGuid = this.GetGuidLoaiVanKien(tenLoaiVanKien);
                vanKien.LoaiVanKienId = loaiVanKienGuid;

                if (item.CatParentId == null || item.CatParentId <= 0)
                {
                  //  txtLog.Text += "- Không có Kỳ họp của văn kiện.\n";
                    return;
                }

                var kyHopItem = context.VanKienCats.FirstOrDefault(x => x.CatOldID == item.CatParentId);
                if (kyHopItem?.CatParentId == null || kyHopItem.CatParentId <= 0)
                {
                   // txtLog.Text += "- Không có Khóa họp của văn kiện.\n";
                    return;
                }

                var khoaItem = context.VanKienCats.FirstOrDefault(x => x.CatOldID == kyHopItem.CatParentId);
                var kyHopGuid = this.GetGuidKyHop(kyHopItem, khoaItem);
                if (kyHopGuid != null)
                {
                    vanKien.DMKyHopId = kyHopGuid.Value;
                }
                else
                {
                   // txtLog.Text += "- Không có kỳ họp của văn kiện.\n";
                }
            }
        }

        // add cơ quan ban hành
        private void InsertListCoQuanBanHanh(string dmCoQuan, Guid IdVanKienCMS)
        {
            if (!string.IsNullOrEmpty(dmCoQuan))
            {
                // get list Id của Cơ quan ban hành trên SP
                var listIdInt = UtilsBase.getListLookup(dmCoQuan);
                if (listIdInt != null)
                {
                    foreach (var id in listIdInt)
                    {
                        using (var context = new QuocHoiVNEntities())
                        {
                            // var allItems = context.TempCoQuanBanHanhs.ToList();
                            var coQuanItem = context.TempCoQuanBanHanhs.Where(x => x.OldId == id).FirstOrDefault();
                            // tên cơ quan của SP vừa tìm được so sánh với tên trong Danh mục Cơ quan ban hành của CMS => lấy được Id cơ quan ban hành của CMS
                            var guiIdCoQuan = GetGuidIdCoQuanBanHanh(coQuanItem.Title);
                            if (guiIdCoQuan != null)
                            {
                                // insert vào bảng Map văn kiện với Cơ quan ban hành
                                var vanKienMapItem = new VanKienMapCQBH
                                {
                                    VanKienTaiLieuId = IdVanKienCMS,
                                    CQBHId = guiIdCoQuan.Value,
                                    OldVanKienId = id,
                                };
                                InsertVanKienTL(vanKienMapItem);
                            }
                        }
                    }
                }
            }
        }

        public void InsertVanKienTL(VanKienMapCQBH item)
        {
            using (Preview_QuocHoi_KyHopServiceEntities db = new Preview_QuocHoi_KyHopServiceEntities())
            {
                // Tìm danh mục theo OldId
                var kyhop = db.VanKienMapCQBHs.Add(item);
                // Lưu thay đổi vào cơ sở dữ liệu
                db.SaveChanges();
            }


        }

        private Guid? GetGuidIdCoQuanBanHanh(string name)
        {
            Guid? result = null;
            if (!string.IsNullOrEmpty(name))
            {
                using (var context = new Preview_QuocHoi_CommonsServiceEntities())
                {
                    //var allItems = context.CoQuanBanHanhs.ToList();
                    var coQuanItem = context.CoQuanBanHanhs.Where(x => x.Title.ToLower() == name.ToLower()).FirstOrDefault();
                    //result = coQuanItem.Id;
                }
            }
            return result;
        }

        private Guid? GetGuidKyHop(VanKienCat kyHop, VanKienCat khoa)
        {
            Guid? result = null;
            // check kỳ họp
            if (kyHop.CatName.ToLower().Contains("kỳ họp"))
            {
                using (var context = new Preview_QuocHoi_KyHopServiceEntities())
                {
                    var allItems = context.DMKyHops;
                    // check Khóa
                    var listKhoaCMS = allItems.Where(x => x.ParentId == null).ToList();
                    var khoaCMS = listKhoaCMS.Where(x => x.Title.ToLower().Contains(khoa.CatName.ToLower()) && x.IsDeleted != true).FirstOrDefault();
                    if (khoaCMS != null)
                    {
                        var tempKyHop = string.Empty;
                        if (kyHop.CatName.ToLower().Contains("thứ nhất"))
                        {
                            tempKyHop = CatChuoiDenKyTu(kyHop.CatName, "1");
                        }
                        else if (kyHop.CatName.ToLower().Contains("thứ hai"))
                        {
                            tempKyHop = CatChuoiDenKyTu(kyHop.CatName, "2");
                        }
                        else if (kyHop.CatName.ToLower().Contains("thứ ba"))
                        {
                            tempKyHop = CatChuoiDenKyTu(kyHop.CatName, "3");
                        }
                        else if (kyHop.CatName.ToLower().Contains("thứ tư"))
                        {
                            tempKyHop = CatChuoiDenKyTu(kyHop.CatName, "4");
                        }
                        else if (kyHop.CatName.ToLower().Contains("thứ năm"))
                        {
                            tempKyHop = CatChuoiDenKyTu(kyHop.CatName, "5");
                        }
                        else if (kyHop.CatName.ToLower().Contains("thứ sáu"))
                        {
                            tempKyHop = CatChuoiDenKyTu(kyHop.CatName, "6");
                        }
                        else if (kyHop.CatName.ToLower().Contains("thứ bảy"))
                        {
                            tempKyHop = CatChuoiDenKyTu(kyHop.CatName, "7");
                        }
                        else if (kyHop.CatName.ToLower().Contains("thứ tám"))
                        {
                            tempKyHop = CatChuoiDenKyTu(kyHop.CatName, "8");
                        }
                        else if (kyHop.CatName.ToLower().Contains("thứ chín"))
                        {
                            tempKyHop = CatChuoiDenKyTu(kyHop.CatName, "9");
                        }
                        else if (kyHop.CatName.ToLower().Contains("thứ mười"))
                        {
                            tempKyHop = CatChuoiDenKyTu(kyHop.CatName, "10");
                        }
                        else if (kyHop.CatName.ToLower().Contains("thứ mười một"))
                        {
                            tempKyHop = CatChuoiDenKyTu(kyHop.CatName, "11");
                        }
                        else
                        {
                            tempKyHop = kyHop.CatName;
                        }
                        var kyCMS = allItems.Where(x => x.ParentId == khoaCMS.Id
                        && (x.Title.ToLower() == kyHop.CatName.ToLower() || x.Title.ToLower() == tempKyHop.ToLower())
                        ).FirstOrDefault();
                        if (kyCMS != null)
                        {
                            result = kyCMS.Id;
                        }
                    }
                }
            }

            return result;
        }

        private string CatChuoiDenKyTu(string text, string congChuoi)
        {
            string tempKyHop = string.Empty;
            int index = text.IndexOf("thứ");
            if (index != -1) // Kiểm tra nếu "thứ" có trong chuỗi
            {
                tempKyHop = text.Substring(0, index) + "thứ " + congChuoi;
            }
            return tempKyHop;
        }

        private Guid? GetGuidLoaiVanKien(string loaiVanKien)
        {
            Guid? result = null;
            if (!string.IsNullOrEmpty(loaiVanKien))
            {
                using (var context = new Preview_QuocHoi_CommonsServiceEntities())
                {
                    var tempText = loaiVanKien;
                    // Get all items from the table
                    var baoCaoGiamSat = "Báo cáo giám sát";
                    var baoCaoCongTac = "Báo cáo công tác";
                    var cacLuatNghiQuyet = "Các Luật,Nghị quyết";
                    if (loaiVanKien.ToLower().Contains(baoCaoGiamSat.ToLower()))
                    {
                        tempText = "Báo cáo giám sát";
                    }
                    if (loaiVanKien.ToLower().Contains(baoCaoCongTac.ToLower()))
                    {
                        tempText = "Báo cáo công tác";
                    }
                    if (loaiVanKien.ToLower().Contains(cacLuatNghiQuyet.ToLower()))
                    {
                        tempText = "Các Luật, Nghị quyết trình Quốc hội thông qua tại kỳ họp";
                    }

                    var allItems = context.LoaiVanKiens.ToList();
                    var item = allItems.Where(x => x.Title.ToLower() == tempText.ToLower()).FirstOrDefault();
                    if (item != null)
                    {
                        //result = item.Id;
                    }
                }
            }
            return result;
        }

        #endregion

        private async Task<string> GetConTentVanKien(string NoiDungHTML)
        {
            string result = string.Empty;
            if (!string.IsNullOrEmpty(NoiDungHTML))
            {
                HtmlAgilityPack.HtmlDocument newsDocument = new HtmlAgilityPack.HtmlDocument();
                newsDocument.LoadHtml(NoiDungHTML);

                #region Download ảnh
                var ImagesNode = newsDocument.DocumentNode.SelectNodes(".//img");
                if (ImagesNode != null && ImagesNode.Count > 0)
                {
                    foreach (var img in ImagesNode)
                    {
                        img.Attributes["src"].Value = await ImageDownloader.DownloadImageAsyncurl(img.Attributes["src"].Value.Trim());
                    }
                    result = newsDocument.DocumentNode.OuterHtml;
                }
                #endregion
            }
            return result;
        }
    }
}
