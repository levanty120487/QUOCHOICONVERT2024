using HtmlAgilityPack;
using QHBASE;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Windows.Forms;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RJCodeUI_M1.TestAndDemo
{
    public partial class FormCloneCongKhaiNganSach : RJForms.RJChildForm
    {
        private List<CongKhaiNganSachItem> parsedData = new List<CongKhaiNganSachItem>();

        public FormCloneCongKhaiNganSach()
        {
            InitializeComponent();
        }

        private void FormCloneCongKhaiNganSach_Load(object sender, EventArgs e)
        {
            LoadCategories();
        }

        private void LoadCategories()
        {
            try
            {
                using (var context = new Portal_Moet_CommonsServicesEntities())
                {
                    // Type = 4 for "Danh mục công khai ngân sách"
                    var categories = context.DanhMucChungs
                        .Where(x => x.DanhMucDungChungType == 4)
                        .Select(x => new { x.Id, x.Title })
                        .ToList();

                    cboCategory.DataSource = categories;
                    cboCategory.DisplayMember = "Title";
                    cboCategory.ValueMember = "Id";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải danh mục: " + ex.Message);
            }
        }

        private void btnReadData_Click(object sender, EventArgs e)
        {
            try
            {
                parsedData.Clear();
                string html = txtWebUrl.Text.Trim();
                if (string.IsNullOrEmpty(html))
                {
                    MessageBox.Show("Vui lòng nhập HTML vào textbox.");
                    return;
                }

                HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(html);

                var trNodes = doc.DocumentNode.SelectNodes("//tbody/tr");
                if (trNodes != null)
                {
                    foreach (var tr in trNodes)
                    {
                        var tdNodes = tr.SelectNodes("td");
                        if (tdNodes != null && tdNodes.Count >= 5)
                        {
                            var item = new CongKhaiNganSachItem();
                            item.Title = tdNodes[1].InnerText.Trim();
                            item.NamKyBaoCao = tdNodes[2].InnerText.Trim();
                            item.BieuMau = tdNodes[3].InnerText.Trim();
                            item.SoQuyetDinhCongBo = tdNodes[4].InnerText.Trim();
                            item.NgayCongBoStr = tdNodes[5].InnerText.Trim();
                            
                            var aNode = tr.SelectSingleNode(".//a");
                            if (aNode != null)
                            {
                                item.LinkChiTiet = aNode.GetAttributeValue("href", "").Trim();
                                
                                // Clean up link text slightly just in case
                                item.LinkChiTiet = System.Net.WebUtility.HtmlDecode(item.LinkChiTiet);
                            }

                            parsedData.Add(item);
                        }
                    }
                }

                dgvData.DataSource = null;
                dgvData.DataSource = parsedData;
                MessageBox.Show($"Đọc thành công {parsedData.Count} bản ghi.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi đọc dữ liệu: " + ex.Message);
            }
        }

        private void btnSaveData_Click(object sender, EventArgs e)
        {
            if (cboCategory.SelectedValue == null)
            {
                MessageBox.Show("Vui lòng chọn danh mục ngân sách.");
                return;
            }

            string categoryId = cboCategory.SelectedValue.ToString();
            int successCount = 0;
            int failCount = 0;
            List<string> failMessages = new List<string>();

            try
            {
                using (var context = new Portal_Moet_VanbanServiceEntities())
                {
                    foreach (var item in parsedData.OrderByDescending(a=>a.CreatedAt))
                    {
                        // Match link detail to get LawId
                        var law = context.Laws.FirstOrDefault(x => x.DetailLinkClone != null && x.DetailLinkClone.Contains(item.LinkChiTiet) && !x.IsDeleted);
                        if (law == null)
                        {
                            // Could also try parsing ItemID manually just in case
                            var match = Regex.Match(item.LinkChiTiet, @"ItemID=(\d+)");
                            if (match.Success)
                            {
                                string itemId = match.Groups[1].Value;
                                law = context.Laws.FirstOrDefault(x => x.DetailLinkClone != null && x.DetailLinkClone.Contains("ItemID=" + itemId) && !x.IsDeleted);
                            }
                        }

                        if (law == null)
                        {
                            failCount++;
                            failMessages.Add($"[{item.Title}] không tìm thấy văn bản gốc (Link: {item.LinkChiTiet}).");
                            //=> continue;
                        }

                        DateTime? ngayCongBo = null;
                        if (DateTime.TryParseExact(item.NgayCongBoStr, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime parsedDate))
                        {
                            ngayCongBo = parsedDate;
                        }

                        #region find DanhSachCongKhaiNganSach
                        var findDSCK = context.DanhSachCongKhaiNganSaches.Where(a=>a.SoQuyetDinhCongBo.Equals(item.SoQuyetDinhCongBo)
                                    && a.BieuMau.Equals(item.BieuMau)).FirstOrDefault();
                        if (findDSCK != null) continue;
                        #endregion

                        var entity = new DanhSachCongKhaiNganSach
                        {
                            Id = QHCommons.GenAutoId(),
                            Title = item.Title,
                            TitleUnicode = RemoveVietnameseAccents(item.Title),
                            NamKyBaoCao = item.NamKyBaoCao,
                            BieuMau = item.BieuMau,
                            SoQuyetDinhCongBo = item.SoQuyetDinhCongBo,
                            NgayCongBo = ngayCongBo,
                            LawId = law != null ? law.Id : null,
                            DMCongKhaiNganSachId = categoryId,
                            IsShow = true,
                            IsDeleted = false,
                            CreationTime = DateTime.Now,
                            ConcurrencyStamp = Guid.NewGuid().ToString(),
                            ExtraProperties = "{}",
                            Language = "vi"
                        };

                        context.DanhSachCongKhaiNganSaches.Add(entity);
                        context.SaveChanges();
                        successCount++;
                    }
                }

                string msg = $"Thêm thành công: {successCount}. Thất bại: {failCount}.\n";
                if (failCount > 0)
                {
                    msg += string.Join("\n", failMessages.Take(10));
                    if (failMessages.Count > 10) msg += $"\n...và {failMessages.Count - 10} bản ghi khác.";
                }
                MessageBox.Show(msg, "Kết quả lưu", MessageBoxButtons.OK, failCount > 0 ? MessageBoxIcon.Warning : MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi lưu dữ liệu: " + ex.Message);
            }
        }

        private string RemoveVietnameseAccents(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return text;

            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }
    }

    public class CongKhaiNganSachItem
    {
        public string Title { get; set; }
        public string NamKyBaoCao { get; set; }
        public string BieuMau { get; set; }
        public string SoQuyetDinhCongBo { get; set; }
        public string NgayCongBoStr { get; set; }
        public string LinkChiTiet { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
