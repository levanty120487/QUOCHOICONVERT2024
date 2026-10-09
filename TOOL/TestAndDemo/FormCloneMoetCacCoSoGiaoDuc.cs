using HtmlAgilityPack;
using QHBASE;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RJCodeUI_M1.TestAndDemo
{
    public partial class FormCloneMoetCacCoSoGiaoDuc : RJForms.RJChildForm
    {
        public class CoSoGiaoDucItem
        {
            public string TenTruong { get; set; }
            public string LinkChiTiet { get; set; }
            public string LoaiHinhDaoTao { get; set; }
            public string LoaiTruong { get; set; }
            public string KyHieu { get; set; }
            public string TenTiengAnh { get; set; }
            public string Website { get; set; }
            public string TinhThanhPho { get; set; }
            public string NgayCap { get; set; }
            public string NgayHetHan { get; set; }
            public string ToChucKiemDinhChatLuongGD { get; set; }
            public DateTime CreadAt { get; set; } = DateTime.Now;
        }

        private List<CoSoGiaoDucItem> parsedData = new List<CoSoGiaoDucItem>();
        private Label lblStatus;

        public FormCloneMoetCacCoSoGiaoDuc()
        {
            InitializeComponent();
        }

        private void FormCloneMoetCacCoSoGiaoDuc_Load(object sender, EventArgs e)
        {
            lblStatus = new Label();
            lblStatus.AutoSize = true;
            lblStatus.ForeColor = System.Drawing.Color.Red;
            lblStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            lblStatus.Location = new System.Drawing.Point(20, 200);
            lblStatus.Text = "";
            pnlClientArea.Controls.Add(lblStatus);

        }

        private async void btnReadData_Click(object sender, EventArgs e)
        {
            try
            {
                parsedData.Clear();
                string htmlList = txtWebUrl.Text.Trim();
                string baseUrl = "https://moet.gov.vn";

                if (string.IsNullOrEmpty(htmlList))
                {
                    MessageBox.Show("Vui lòng nhập HTML List!");
                    return;
                }

                btnReadData.Enabled = false;
                
                var doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(htmlList);
                var trs = doc.DocumentNode.SelectNodes("//table//tr");
                if (trs == null) return;
                
                using (var httpClient = new HttpClient())
                {
                    httpClient.Timeout = TimeSpan.FromSeconds(60);
                    for (int i = 1; i < trs.Count; i++) // Bỏ qua tr đầu tiên
                    {
                        var tr = trs[i];
                        var aNode = tr.SelectSingleNode("./td[2]/a");
                        if (aNode == null) continue;

                        var item = new CoSoGiaoDucItem();
                        item.TenTruong = aNode.InnerText.Trim();
                        item.LinkChiTiet = aNode.GetAttributeValue("href", "").Trim();
                        
                        string detailUrl = item.LinkChiTiet;
                        if (!detailUrl.StartsWith("http") && !string.IsNullOrEmpty(baseUrl))
                        {
                            detailUrl = baseUrl.TrimEnd('/') + "/" + detailUrl.TrimStart('/');
                        }
                        
                        lblStatus.Text = $"Đang đọc: {item.TenTruong}";
                        Application.DoEvents();

                        try 
                        {
                            string detailHtml = await httpClient.GetStringAsync(detailUrl);
                            var detailDoc = new HtmlAgilityPack.HtmlDocument();
                            detailDoc.LoadHtml(detailHtml);
                            
                            var detailTrs = detailDoc.DocumentNode.SelectNodes("//table//tr");
                            if (detailTrs != null)
                            {
                                foreach (var dtr in detailTrs)
                                {
                                    var th = dtr.SelectSingleNode("./th");
                                    var td = dtr.SelectSingleNode("./td");
                                    if (th == null || td == null) continue;
                                    
                                    string thText = th.InnerText.Trim();
                                    string tdText = td.InnerText.Trim();
                                    
                                    if (thText.Contains("Loại hình cơ sở đào tạo")) item.LoaiHinhDaoTao = tdText;
                                    else if (thText.Contains("Loại trường")) item.LoaiTruong = tdText;
                                    else if (thText.Contains("Ký hiệu")) item.KyHieu = tdText;
                                    else if (thText.Contains("Tên tiếng Anh")) item.TenTiengAnh = tdText;
                                    else if (thText.Contains("Website")) item.Website = tdText;
                                    else if (thText.Contains("Tỉnh, thành phố")) item.TinhThanhPho = tdText;
                                    else if (thText.Contains("Ngày cấp giấy chứng nhận")) item.NgayCap = tdText;
                                    else if (thText.Contains("Ngày hết hạn giá trị")) item.NgayHetHan = tdText;
                                    else if (thText.Contains("Được kiểm định bởi tổ chức"))
                                    {
                                        if (!string.IsNullOrWhiteSpace(tdText))
                                            tdText = tdText.Replace("&amp;", "&");
                                        item.ToChucKiemDinhChatLuongGD = tdText;
                                    } 
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }

                        parsedData.Add(item);
                    }
                }

                dgvData.DataSource = null;
                dgvData.DataSource = parsedData;
                lblStatus.Text = $"Hoàn thành: {parsedData.Count} bản ghi";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi: " + ex.Message);
            }
            finally
            {
                btnReadData.Enabled = true;
            }
        }

        private void btnSaveData_Click(object sender, EventArgs e)
        {
            if (parsedData.Count == 0) return;
            btnSaveData.Enabled = false;
            try
            {
                using (var db = new Portal_Moet_CommonsServicesEntities())
                {
                    foreach (var item in parsedData.OrderByDescending(a=>a.CreadAt))
                    {
                        var entity = new DanhSachCacCoSoGiaoDuc();
                        entity.Id = QHCommons.GenAutoId();
                        entity.Title = item.TenTruong;
                        entity.KyHieu = item.KyHieu;
                        entity.TenTiengAnh = item.TenTiengAnh;
                        entity.Website = item.Website;
                        entity.IsShow = true;
                        entity.Language = "vi";
                        entity.CreationTime = DateTime.Now;
                        entity.IsDeleted = false;
                        entity.ToChucKiemDinhChatLuongGD = item.ToChucKiemDinhChatLuongGD;
                        entity.CreatedBy = "admin";

                        if (item.NgayCap != "-" && !string.IsNullOrEmpty(item.NgayCap))
                        {
                            if (DateTime.TryParseExact(item.NgayCap, "dd/MM/yyyy", null, DateTimeStyles.None, out var dt))
                                entity.NgayCapGiayChungNhan = dt;
                        }
                        if (item.NgayHetHan != "-" && !string.IsNullOrEmpty(item.NgayHetHan))
                        {
                            if (DateTime.TryParseExact(item.NgayHetHan, "dd/MM/yyyy", null, DateTimeStyles.None, out var dt2))
                                entity.NgayHetHanGiayChungNhan = dt2;
                        }

                        entity.LoaiHinhCoSoDaoTaoId = GetOrInsertDanhMuc(db, item.LoaiHinhDaoTao, 2);
                        entity.LoaiTruongId = GetOrInsertDanhMuc(db, item.LoaiTruong, 1);
                        entity.TinhThanhPhoId = GetOrInsertDanhMuc(db, item.TinhThanhPho, 3);

                        db.DanhSachCacCoSoGiaoDucs.Add(entity);
                    }
                    db.SaveChanges();
                    MessageBox.Show("Lưu thành công!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi lưu: " + ex.Message);
            }
            finally
            {
                btnSaveData.Enabled = true;
            }
        }

        private string GetOrInsertDanhMuc(Portal_Moet_CommonsServicesEntities db, string title, int type)
        {
            if (string.IsNullOrEmpty(title)) return null;
            var exist = db.DanhMucChungs.FirstOrDefault(x => x.Title == title && x.DanhMucDungChungType == type);
            if (exist != null) return exist.Id;
            
            var n = new DanhMucChung
            {
                Id = QHCommons.GenAutoId(),
                Title = title,
                DanhMucDungChungType = type,
                IsShow = true,
                Language = "vi",
                CreationTime = DateTime.Now
            };
            db.DanhMucChungs.Add(n);
            db.SaveChanges();
            return n.Id;
        }

        // Dummy methods to satisfy the designer if they were hooked up
        private void pnlClientArea_Paint(object sender, PaintEventArgs e) { }
        private void btnUpdateContent_Click(object sender, EventArgs e) { }
        private void btnPreview_Click(object sender, EventArgs e) { }
    }
}
