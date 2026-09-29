using HtmlAgilityPack;
using System.Text;

namespace RJCodeUI_M1
{
    public class QuestionAnswerDetail
    {
        public string TieuDe { get; set; }

        public string NgayHoi { get; set; }
        public string NoiDungCauHoiHtml { get; set; }
        public string NoiDungCauHoiText { get; set; }
        public string TacGiaHoi { get; set; }

        public string NgayTraLoi { get; set; }
        public string NoiDungTraLoiHtml { get; set; }
        public string NoiDungTraLoiText { get; set; }
        public string TacGiaTraLoi { get; set; }
    }

    public static class HtmlDetailParser
    {
        public static QuestionAnswerDetail ReadQuestionDetail(string html)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            var containerFull = doc.DocumentNode.SelectSingleNode(
                "//div[contains(concat(' ', normalize-space(@class), ' '), ' Question-Details ')]" +
                "//div[contains(concat(' ', normalize-space(@class), ' '), ' container-full ')]");

            if (containerFull == null)
                return null;

            var model = new QuestionAnswerDetail();

            var nameNode = containerFull.SelectSingleNode(
                "./p[contains(concat(' ', normalize-space(@class), ' '), ' name ')]");
            if (nameNode != null)
                model.TieuDe = CleanText(nameNode.InnerText);

            var questionDateNode = containerFull.SelectSingleNode(
                "./p[contains(concat(' ', normalize-space(@class), ' '), ' date ')]");
            if (questionDateNode != null)
                model.NgayHoi = CleanText(questionDateNode.InnerText);

            var questionAuthorNode = containerFull.SelectSingleNode(
                "./p[contains(concat(' ', normalize-space(@class), ' '), ' author ')]");
            if (questionAuthorNode != null)
                model.TacGiaHoi = CleanText(questionAuthorNode.InnerText);

            var questionStartNode = containerFull.SelectSingleNode(
                "./p[contains(concat(' ', normalize-space(@class), ' '), ' des ')]");

            if (questionStartNode != null && questionAuthorNode != null)
            {
                model.NoiDungCauHoiHtml = GetHtmlBetweenSiblings(questionStartNode, questionAuthorNode);
                model.NoiDungCauHoiText = GetTextBetweenSiblings(questionStartNode, questionAuthorNode);
            }

            var ansContentNode = containerFull.SelectSingleNode(
                "./div[contains(concat(' ', normalize-space(@class), ' '), ' ans-content ')]");

            if (ansContentNode != null)
            {
                var answerDateNode = ansContentNode.SelectSingleNode(
                    "./p[contains(concat(' ', normalize-space(@class), ' '), ' date ')]");
                if (answerDateNode != null)
                    model.NgayTraLoi = CleanText(answerDateNode.InnerText);

                var answerAuthorNode = ansContentNode.SelectSingleNode(
                    "./p[contains(concat(' ', normalize-space(@class), ' '), ' author ')]");
                if (answerAuthorNode != null)
                    model.TacGiaTraLoi = CleanText(answerAuthorNode.InnerText);

                var answerStartNode = ansContentNode.SelectSingleNode(
                    "./p[contains(concat(' ', normalize-space(@class), ' '), ' des ')]");

                if (answerStartNode != null && answerAuthorNode != null)
                {
                    model.NoiDungTraLoiHtml = GetHtmlBetweenSiblings(answerStartNode, answerAuthorNode);
                    model.NoiDungTraLoiText = GetTextBetweenSiblings(answerStartNode, answerAuthorNode);
                }
            }

            return model;
        }

        private static string GetHtmlBetweenSiblings(HtmlNode startNode, HtmlNode endNode)
        {
            var sb = new StringBuilder();
            HtmlNode current = startNode;

            while (current != null && current != endNode)
            {
                if (current.NodeType == HtmlNodeType.Element)
                {
                    sb.AppendLine(current.OuterHtml);
                }

                current = current.NextSibling;
            }

            return sb.ToString().Trim();
        }

        private static string GetTextBetweenSiblings(HtmlNode startNode, HtmlNode endNode)
        {
            var sb = new StringBuilder();
            HtmlNode current = startNode;

            while (current != null && current != endNode)
            {
                if (current.NodeType == HtmlNodeType.Element)
                {
                    string text = ExtractReadableText(current);
                    if (!string.IsNullOrWhiteSpace(text))
                    {
                        if (sb.Length > 0)
                            sb.AppendLine().AppendLine();

                        sb.Append(text);
                    }
                }

                current = current.NextSibling;
            }

            return sb.ToString().Trim();
        }

        private static string ExtractReadableText(HtmlNode node)
        {
            if (node == null)
                return "";

            var childPs = node.SelectNodes("./p");
            if (childPs != null && childPs.Count > 0)
            {
                var sb = new StringBuilder();

                foreach (var p in childPs)
                {
                    string text = CleanText(p.InnerText);
                    if (!string.IsNullOrWhiteSpace(text))
                    {
                        if (sb.Length > 0)
                            sb.AppendLine().AppendLine();

                        sb.Append(text);
                    }
                }

                return sb.ToString().Trim();
            }

            return CleanText(node.InnerText);
        }

        private static string CleanText(string input)
        {
            return HtmlEntity.DeEntitize(input ?? "").Trim();
        }
    }
}