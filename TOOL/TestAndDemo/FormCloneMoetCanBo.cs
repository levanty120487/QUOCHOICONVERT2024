using HtmlAgilityPack;
using QHBASE;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RJCodeUI_M1.TestAndDemo
{
    public partial class FormCloneMoetCanBo : RJForms.RJChildForm
    {
        private List<ThongTinCanBoItem> parsedData = new List<ThongTinCanBoItem>();
        private ThongTinCoQuanItem parsedCoQuan = new ThongTinCoQuanItem();
        private Label lblStatus;

        public FormCloneMoetCanBo()
        {
            InitializeComponent();
        }

        private void FormCloneMoetCanBo_Load(object sender, EventArgs e)
        {
            lblStatus = new Label();
            lblStatus.AutoSize = true;
            lblStatus.ForeColor = System.Drawing.Color.Red;
            lblStatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold);
            lblStatus.Location = new System.Drawing.Point(20, 168);
            lblStatus.Text = "";
            pnlClientArea.Controls.Add(lblStatus);

            LoadCategories();
        }

        public class CoQuanDisplayItem
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
        }

        private void LoadCategories()
        {
            try
            {
                using (var db = new Portal_Moet_CoCauToChucServiceEntities())
                {
                    var categories = db.DmCoQuanQuocHois.ToList();
                    var flatList = new List<CoQuanDisplayItem>();
                    BuildCategoryTree(categories, null, "", flatList);

                    cboCategory.DataSource = flatList;
                    cboCategory.DisplayMember = "Name";
                    cboCategory.ValueMember = "Id";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void BuildCategoryTree(IEnumerable<DmCoQuanQuocHoi> allCategories, Guid? parentId, string prefix, List<CoQuanDisplayItem> result)
        {
            var children = allCategories.Where(c => c.ParentId == parentId).OrderBy(a => a.Order).ToList();

            foreach (var child in children)
            {
                result.Add(new CoQuanDisplayItem { Id = child.Id, Name = prefix + child.Name });
                BuildCategoryTree(allCategories, child.Id, prefix + "--- ", result);
            }

            if (parentId == null && result.Count == 0)
            {
                foreach (var c in allCategories)
                {
                    result.Add(new CoQuanDisplayItem { Id = c.Id, Name = c.Name });
                }
            }
        }

        private async void btnReadData_Click(object sender, EventArgs e)
        {
            try
            {
                parsedData.Clear();
                parsedCoQuan = new ThongTinCoQuanItem();

                string baseUrl = txtWebUrl.Text.Trim();
                if (string.IsNullOrEmpty(baseUrl))
                {
                    MessageBox.Show("Vui lòng nhập Link Page!");
                    return;
                }

                string originalBtnText = btnReadData.Text;
                btnReadData.Enabled = false;
                lblStatus.Text = "Bắt đầu đọc dữ liệu...";

                using (var httpClient = new HttpClient())
                {
                    httpClient.Timeout = TimeSpan.FromSeconds(60);
                    string html = await httpClient.GetStringAsync(baseUrl);
                    var doc = new HtmlAgilityPack.HtmlDocument();
                    doc.LoadHtml(html);

                    // Đọc thông tin cơ quan
                    var pNodes = doc.DocumentNode.SelectNodes("//p");
                    if (pNodes != null)
                    {
                        foreach (var p in pNodes)
                        {
                            string text = CleanText(p.InnerText);
                            if (text.Contains("Địa chỉ:") || text.Contains("Điện thoại:"))
                            {
                                parsedCoQuan.Address = ExtractValue(text, "Địa chỉ:", ";");
                                string phone = ExtractValue(text, "Điện thoại:", ";");
                                string fax = ExtractValue(text, "Fax:", ";");
                                parsedCoQuan.Phone = !string.IsNullOrEmpty(fax) ? phone + "; Fax: " + fax : phone;
                                parsedCoQuan.Email = ExtractValue(text, "Email:", ";");
                                parsedCoQuan.Webstite = ExtractValue(text, "Website:", "");
                                break;
                            }
                        }
                    }

                    // Đọc thông tin cán bộ
                    var itemNodes = doc.DocumentNode.SelectNodes("//div[contains(@class, 'article-item')]");
                    if (itemNodes != null)
                    {
                        foreach (var itemNode in itemNodes)
                        {
                            var cb = new ThongTinCanBoItem();

                            // Tên cán bộ
                            var nameNode = itemNode.SelectSingleNode(".//span[contains(@class, 'text-bold') and contains(@class, 'color-primary')]");
                            if (nameNode != null)
                            {
                                cb.TenCanBo = CleanText(nameNode.InnerText);
                                
                                // Chức vụ: Lấy nội dung text trước thẻ span Tên cán bộ
                                string textBeforeName = CleanText(itemNode.InnerText);
                                int nameIndex = textBeforeName.IndexOf(cb.TenCanBo);
                                if (nameIndex > 0)
                                {
                                    string chucVuRaw = textBeforeName.Substring(0, nameIndex).Trim();
                                    chucVuRaw = Regex.Replace(chucVuRaw, @"^\d+\.", "").Trim(); // remove "1. "
                                    cb.ChucVu = chucVuRaw;
                                }
                            }

                            // Điện thoại và Email
                            var phoneNodes = itemNode.SelectNodes(".//div[contains(@class, 'article-phone')]");
                            if (phoneNodes != null)
                            {
                                foreach (var pn in phoneNodes)
                                {
                                    string pt = CleanText(pn.InnerText);
                                    if (pt.StartsWith("Điện thoại:")) cb.DienThoai = pt.Replace("Điện thoại:", "").Trim();
                                    else if (pt.StartsWith("Email:")) cb.Email = pt.Replace("Email:", "").Trim();
                                }
                            }

                            // Ảnh đại diện
                            var imgNode = itemNode.SelectSingleNode(".//img[contains(@class, 'post-image')]");
                            if (imgNode != null)
                            {
                                cb.AnhDaiDien = ToAbsoluteUrl(baseUrl, imgNode.GetAttributeValue("src", ""));
                            }

                            if (!string.IsNullOrEmpty(cb.TenCanBo))
                            {
                                parsedData.Add(cb);
                            }
                        }
                    }
                }

                btnReadData.Text = originalBtnText;
                btnReadData.Enabled = true;
                lblStatus.Text = $"Hoàn tất! Đã đọc thông tin cơ quan và {parsedData.Count} cán bộ.";

                dgvData.DataSource = null;
                dgvData.DataSource = parsedData;
                MessageBox.Show($"Đã đọc xong {parsedData.Count} cán bộ.", "Thông báo");
            }
            catch (Exception ex)
            {
                btnReadData.Enabled = true;
                btnReadData.Text = "Đọc dữ liệu";
                MessageBox.Show("Lỗi đọc dữ liệu: " + ex.Message);
            }
        }

        private string ExtractValue(string source, string startStr, string endStr)
        {
            int startIndex = source.IndexOf(startStr);
            if (startIndex < 0) return "";
            startIndex += startStr.Length;

            int endIndex = -1;
            if (!string.IsNullOrEmpty(endStr))
            {
                endIndex = source.IndexOf(endStr, startIndex);
            }

            if (endIndex < 0)
            {
                return source.Substring(startIndex).Trim();
            }
            else
            {
                return source.Substring(startIndex, endIndex - startIndex).Trim();
            }
        }

        private async void btnSaveData_Click(object sender, EventArgs e)
        {
            if (cboCategory.SelectedValue == null)
            {
                MessageBox.Show("Vui lòng chọn cơ quan!");
                return;
            }

            Guid donViId = (Guid)cboCategory.SelectedValue;

            string downloadFolderImg = @"C:\uploadFckFiles\news";
            if (!string.IsNullOrWhiteSpace(txtFolderChua.Text))
            {
                downloadFolderImg = string.Concat(downloadFolderImg, "\\", txtFolderChua.Text.Trim());
            }
            Directory.CreateDirectory(downloadFolderImg);

            string originalBtnText = btnSaveData.Text;
            btnSaveData.Enabled = false;
            lblStatus.Text = "Bắt đầu lưu dữ liệu...";

            try
            {
                using (var db = new Portal_Moet_CoCauToChucServiceEntities())
                using (var httpClient = new HttpClient())
                {
                    // 1. Cập nhật thông tin cơ quan
                    var coQuan = db.DmCoQuanQuocHois.FirstOrDefault(x => x.Id == donViId);
                    if (coQuan != null)
                    {
                        if (!string.IsNullOrEmpty(parsedCoQuan.Address)) coQuan.Address = parsedCoQuan.Address;
                        if (!string.IsNullOrEmpty(parsedCoQuan.Phone)) coQuan.Phone = parsedCoQuan.Phone;
                        if (!string.IsNullOrEmpty(parsedCoQuan.Email)) coQuan.Email = parsedCoQuan.Email;
                        if (!string.IsNullOrEmpty(parsedCoQuan.Webstite)) coQuan.WebLink = parsedCoQuan.Webstite;
                    }

                    int currentIndex = 0;
                    foreach (var item in parsedData)
                    {
                        currentIndex++;
                        lblStatus.Text = $"Đang lưu cán bộ {currentIndex}/{parsedData.Count}...";
                        Application.DoEvents();

                        // Tìm Chức vụ
                        Guid chucVuId = Guid.Empty;
                        if (!string.IsNullOrEmpty(item.ChucVu))
                        {
                            var cv = db.DmChucDanhs.FirstOrDefault(x => x.Name.ToLower() == item.ChucVu.ToLower());
                            if (cv == null)
                            {
                                cv = new DmChucDanh
                                {
                                    Id = Guid.NewGuid(),
                                    Name = item.ChucVu,
                                    IsShow = true,
                                    Order = 0,
                                    CreationTime = DateTime.Now,
                                    ConcurrencyStamp = Guid.NewGuid().ToString(),
                                    ExtraProperties = "{}",
                                    IsDeleted = false,
                                    CreatedBy = "admin",
                                    Language = "vi"
                                };
                                db.DmChucDanhs.Add(cv);
                                db.SaveChanges();
                            }
                            chucVuId = cv.Id;
                        }

                        // Kiểm tra cán bộ đã tồn tại (trùng Tên và Email)
                        var existingCanBo = db.DaiBieuQuocHois.FirstOrDefault(x => x.FullName == item.TenCanBo && x.Email == item.Email);
                        string daiBieuId = "";

                        if (existingCanBo != null)
                        {
                            daiBieuId = existingCanBo.Id;
                        }
                        else
                        {
                            string surName = item.TenCanBo;
                            string name = item.TenCanBo;
                            if (!string.IsNullOrEmpty(item.TenCanBo))
                            {
                                var parts = item.TenCanBo.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                                if (parts.Length > 1)
                                {
                                    surName = parts[0];
                                    name = string.Join(" ", parts.Skip(1));
                                }
                            }

                            var newDaiBieu = new DaiBieuQuocHoi
                            {
                                Id = QHCommons.GenAutoId(),
                                FullName = item.TenCanBo,
                                SurName = surName,
                                TenThuongGoi = item.TenCanBo,
                                GioiTinhId = item.TenCanBo.Contains("Thị") ? Guid.Parse("CD170AF3-C027-4BAD-B536-4403EAE98EAB") : Guid.Parse("1E0A326B-900D-45C7-A892-C26F41EEFEC7"),
                                DanTocId = Guid.Parse("3130501E-75D0-457D-AE2D-52E7298DD8FF"),
                                TonGiaoId = Guid.Parse("04F13D14-E5B9-4CF8-B2FA-DFA8B34248A1"),
                                IsShow = true,
                                Language = "vi",
                                HienThiThongTinChiTiet = false,
                                ExtraProperties = "{}",
                                ConcurrencyStamp = DateTime.Now.ToString("o"),
                                CreationTime = DateTime.Now,
                                Email = item.Email,
                                DienThoaiCoQuan = item.DienThoai,
                                IsDeleted = false,
                                Order = 0,
                                CreatedBy = "admin",
                                Name = name
                            };

                            if (!string.IsNullOrEmpty(item.AnhDaiDien))
                            {
                                newDaiBieu.Image = await ImageDownloader.DownloadImageAsyncurl(item.AnhDaiDien.Trim(), folder: txtFolderChua.Text);
                            }

                            db.DaiBieuQuocHois.Add(newDaiBieu);
                            daiBieuId = newDaiBieu.Id;
                        }

                        // Thêm vào DonViMapChucVu
                        if (chucVuId != Guid.Empty && !string.IsNullOrEmpty(daiBieuId))
                        {
                            var existMap = db.DonViMapChucVus.FirstOrDefault(x => x.DaiBieuQuocHoiId == daiBieuId && x.DonViId == donViId && x.ChucVuId == chucVuId);
                            if (existMap == null)
                            {
                                var map = new DonViMapChucVu
                                {
                                    Id = Guid.NewGuid(),
                                    DaiBieuQuocHoiId = daiBieuId,
                                    DonViId = donViId,
                                    ChucVuId = chucVuId,
                                    DmChucDanhId = chucVuId,
                                    DmCoQuanQuocHoiId = donViId
                                };
                                db.DonViMapChucVus.Add(map);
                            }
                        }

                        db.SaveChanges();
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
            if (string.IsNullOrEmpty(text)) return "";
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

        private void pnlClientArea_Paint(object sender, PaintEventArgs e)
        {
        }

        private void btnPreview_Click(object sender, EventArgs e)
        {
        }
    }

    public class ThongTinCoQuanItem
    {
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Webstite { get; set; }
    }

    public class ThongTinCanBoItem
    {
        public string ChucVu { get; set; }
        public string TenCanBo { get; set; }
        public string DienThoai { get; set; }
        public string Email { get; set; }
        public string AnhDaiDien { get; set; }
    }
}
