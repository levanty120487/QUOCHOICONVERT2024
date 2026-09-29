using HtmlAgilityPack;
using QHBASE;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RJCodeUI_M1.TestAndDemo
{
    public partial class FormHoiDap : RJForms.RJChildForm
    {
        public FormHoiDap()
        {
            InitializeComponent();
        }

        private void btnKetNoi_Click(object sender, EventArgs e)
        {
            using (var hoidap = new portalcdk_QuestionsAnswersEntities())
            {
                var dm = hoidap.Topics.OrderBy(a=>a.Order).ToList();
                cboDanhMucHoiDap.Items.Clear();
                cboDanhMucHoiDap.DataSource = dm;
                cboDanhMucHoiDap.DisplayMember = "TopicTitle";
                cboDanhMucHoiDap.ValueMember = "Id";
            };
        }

        private async void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                DateTimeFormatInfo dtfi = new DateTimeFormatInfo();
                dtfi.ShortDatePattern = "dd/MM/yyyy";
                dtfi.DateSeparator = "/";
                var pageStep = Convert.ToInt32(txtSoTrang.Text);
                //var isCheck = false;
                for (var iPage = pageStep; iPage >= 1; iPage--)
                {
                    //if(isCheck)
                    //{
                    //    break;
                    //}    
                    var pageUrl = string.Concat(txtLinkPageHoiDap.Text.Trim(), iPage);
                    var items = await ReadFromUrlAsync(pageUrl);
                    foreach (var item in items)
                    {
                        using (var httpClient = new HttpClient())
                        {
                            string html = await httpClient.GetStringAsync(item.LinkChiTiet);
                            var detail = HtmlDetailParser.ReadQuestionDetail(html);
                            #region insert vào db hỏi đáp
                            using (var hoidap = new portalcdk_QuestionsAnswersEntities())
                            {
                                var cauhoi = new Question()
                                {
                                    Id = QHCommons.GenAutoId(),
                                    ConcurrencyStamp = QHCommons.GenAutoId(),
                                    CreationTime = Convert.ToDateTime(detail.NgayHoi, dtfi),
                                    HoTen = detail.TacGiaHoi.Split('-')[0].Trim(),
                                    Email = detail.TacGiaHoi.Split('-')[1].Trim(),
                                    Language = "vi",
                                    IsShow = true,
                                    Phone = "",
                                    PublicQuestion = true,
                                    NgayGuiCauHoi = Convert.ToDateTime(detail.NgayHoi, dtfi),
                                    QuestionTitle = detail.TieuDe,
                                    QuestionContent = detail.NoiDungCauHoiHtml,
                                    TopicId = Guid.Parse(cboDanhMucHoiDap.SelectedValue.ToString()),
                                    ViewCount = RandomNumber(),
                                    Status = 2,
                                    ReplyStatus = !string.IsNullOrWhiteSpace(detail.NoiDungTraLoiText),
                                    ExtraProperties = "{}",
                                    QuestionTitleUnicode = RemoveVietnameseAccents(detail.TieuDe)
                                };
                                hoidap.Questions.Add(cauhoi);
                                hoidap.SaveChanges();

                                if(!string.IsNullOrWhiteSpace(detail.NoiDungTraLoiText))
                                {
                                    var ans = new Answer()
                                    {
                                        AnswerContent = detail.NoiDungTraLoiHtml,
                                        Id = Guid.NewGuid(),
                                        ConcurrencyStamp = Guid.NewGuid().ToString(),
                                        AnswersPrivate = false,
                                        ExtraProperties ="{}",
                                        FullName = detail.TacGiaTraLoi,
                                        IsShow = true,
                                        Language ="vi",
                                        QuestionId = cauhoi.Id,
                                        PublicTime = Convert.ToDateTime(detail.NgayTraLoi, dtfi),
                                        CreationTime = Convert.ToDateTime(detail.NgayTraLoi, dtfi),
                                        Status = 2,
                                        TotalLike = RandomNumber()
                                    };
                                    hoidap.Answers.Add(ans);
                                    hoidap.SaveChanges();
                                }
                            }
                            #endregion
                        }
                    }

                    //isCheck = true;
                }
                MessageBox.Show("Done hoi dap");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        public static async Task<List<NewsItemModel>> ReadFromUrlAsync(string url)
        {
            using (var httpClient = new HttpClient())
            {
                string html = await httpClient.GetStringAsync(url);
                return ReadNewsItems(html, url);
            }
        }

        public static List<NewsItemModel> ReadNewsItems(string html, string baseUrl)
        {
            var result = new List<NewsItemModel>();

            var doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(html);

            // Lấy tất cả contentContainer
            var contentContainers = doc.DocumentNode.SelectNodes(
                "//div[contains(concat(' ', normalize-space(@class), ' '), ' contentContainer ')]");

            if (contentContainers == null || contentContainers.Count == 0)
                return result;

            HtmlNode contentContainerHasNews = null;

            foreach (var node in contentContainers)
            {
                var newsItemsTest = node.SelectNodes(
                    ".//div[contains(concat(' ', normalize-space(@class), ' '), ' newsItem ')]");

                if (newsItemsTest != null && newsItemsTest.Count > 0)
                {
                    contentContainerHasNews = node;
                    break;
                }
            }

            if (contentContainerHasNews == null)
                return result;

            var newsItems = contentContainerHasNews.SelectNodes(
                ".//div[contains(concat(' ', normalize-space(@class), ' '), ' newsItem ')]");

            if (newsItems == null)
                return result;

            foreach (var newsItem in newsItems)
            {
                var container = newsItem.SelectSingleNode(
                    ".//div[contains(concat(' ', normalize-space(@class), ' '), ' container ')]");

                if (container == null)
                    continue;

                string tieuDe = "";
                string linkChiTiet = "";
                string thongTinNguoiDung = "";
                string moTa = "";

                var titleNode = container.SelectSingleNode(
                    ".//p[contains(concat(' ', normalize-space(@class), ' '), ' title ')]");

                if (titleNode != null)
                {
                    tieuDe = HtmlEntity.DeEntitize(titleNode.InnerText).Trim();

                    var aNode = titleNode.SelectSingleNode(".//a[@href]");
                    if (aNode != null)
                    {
                        string href = aNode.GetAttributeValue("href", "").Trim();
                        if (!string.IsNullOrEmpty(href))
                        {
                            linkChiTiet = new Uri(new Uri(baseUrl), href).ToString();
                        }
                    }
                }

                var quesInforNode = container.SelectSingleNode(
                    ".//div[contains(concat(' ', normalize-space(@class), ' '), ' ques-infor ')]");

                if (quesInforNode != null)
                {
                    thongTinNguoiDung = HtmlEntity.DeEntitize(quesInforNode.InnerText).Trim();
                }

                var desNode = container.SelectSingleNode(
                    ".//p[contains(concat(' ', normalize-space(@class), ' '), ' des ')]");

                if (desNode != null)
                {
                    moTa = HtmlEntity.DeEntitize(desNode.InnerText).Trim();
                }

                result.Add(new NewsItemModel
                {
                    TieuDe = tieuDe,
                    LinkChiTiet = linkChiTiet,
                    ThongTinNguoiDung = thongTinNguoiDung
                });
            }

            return result;
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

        public static int RandomNumber()
        {
            Random rnd = new Random();
            return rnd.Next(100, 100000); // 100000 là exclusive (không lấy)
        }
    }

    public class NewsItemModel
    {
        public string TieuDe { get; set; }
        public string LinkChiTiet { get; set; }
        public string ThongTinNguoiDung { get; set; }
    }
}
