#define DEBUGGING_OFF

using System;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Agridea.Reports
{
    public class HtmlReportMakerStringBuilder : IReportMaker
    {
        #region Constants
        private const string HorizontalAlignRight = "horizontalAlignRight";
        private const string FontWeightBold = "fontWeightBold";
        #endregion

        #region Initialization
        public HtmlReportMakerStringBuilder()
        {
        }
        #endregion

        #region IReportMaker
        public object Make(Report report, bool addHeader = false)
        {
            StringBuilder stringBuilder = new StringBuilder();

#if DEBUGGING_ON
            stringBuilder.Append(string.Format("  <style type=\"text/css\">{0}", Environment.NewLine));
            stringBuilder.Append(string.Format("    th {{ text-align:left; font-weight:normal;background: #97BF0D; border:1px solid #496D2D;color: #fff;}}{0}", Environment.NewLine));
            stringBuilder.Append(string.Format("    td {{ font-weight:normal;border:1px solid #496D2D;}}{0}", Environment.NewLine));
            stringBuilder.Append(string.Format("    .horizontalAlignRight {{ float:right; }}{0}", Environment.NewLine));
            stringBuilder.Append(string.Format("    .fontWeightBold {{ font-weight:bold;}}{0}", Environment.NewLine));
            stringBuilder.Append(string.Format("    .noborder {{  border:none;}}{0}", Environment.NewLine));
            stringBuilder.Append(string.Format("  </style>{0}", Environment.NewLine));
#endif
            MakeReport(report, stringBuilder);

            return MvcHtmlString.Create(stringBuilder.ToString());
        }
        public void Write(Report report, string filePath)
        {
            MvcHtmlString content = Make(report) as MvcHtmlString;
            File.WriteAllText(filePath, content.ToHtmlString(), Encoding.UTF8);
        }
        public void Append(Report report, string filePath)
        {
            MvcHtmlString content = Make(report) as MvcHtmlString;
            File.AppendAllText(filePath, content.ToHtmlString(), Encoding.UTF8);
        }
        #endregion

        #region Helpers
        private void MakeReport(Report report, StringBuilder stringBuilder)
        {
            foreach (var table in report.Tables) MakeTable(table, stringBuilder);
        }
        private void MakeTable(Table table, StringBuilder stringBuilder)
        {
            stringBuilder.Append(string.Format("<table style=\"width:{0}%\">{1}", table.TotalWidth, Environment.NewLine));

            MakeTableColumns(table, stringBuilder);
            MakeTableRows(table, stringBuilder);

            stringBuilder.Append(string.Format("</table>{0}", Environment.NewLine));
            stringBuilder.Append(string.Format("<br/>{0}", Environment.NewLine));
        }
        private void MakeTableColumns(Table table, StringBuilder stringBuilder)
        {
            foreach (int relativeWidth in table.RelativeWidths) MakeTableColumn(relativeWidth, stringBuilder);
        }
        private void MakeTableColumn(int relativeWidth, StringBuilder stringBuilder)
        {
            stringBuilder.Append(string.Format("  <col style=\"width:{0}%\">{1}", relativeWidth, Environment.NewLine));
        }
        private void MakeTableRows(Table table, StringBuilder stringBuilder)
        {
            foreach (var row in table.Rows) MakeTableRow(row, stringBuilder);
        }
        private void MakeTableRow(Row row, StringBuilder stringBuilder)
        {
            stringBuilder.Append(string.Format("  <tr>{0}", Environment.NewLine));

            MakeTableHeaders(row, stringBuilder);
            MakeTableCells(row, stringBuilder);

            stringBuilder.Append(string.Format("  </tr>{0}", Environment.NewLine));
        }
        private void MakeTableHeaders(Row row, StringBuilder stringBuilder)
        {
            foreach (var header in row.Headers) MakeTableCell(header, "th", stringBuilder);
        }
        private void MakeTableCells(Row row, StringBuilder stringBuilder)
        {
            foreach (var cell in row.Cells) MakeTableCell(cell, "td", stringBuilder);
        }
        private void MakeTableCell(Cell cell, string tag, StringBuilder stringBuilder)
        {
            stringBuilder.Append(string.Format("    <{0} {1} {2}>{3}</{0}>{4}", tag, ColSpanFor(cell), BorderFor(cell), ContentFor(cell), Environment.NewLine));
        }
        private string ColSpanFor(Cell cell)
        {
            if (cell.ColSpan == 1) return string.Empty;
            return string.Format("colspan={0}", cell.ColSpan);
        }
        private string BorderFor(Cell cell)
        {
            if (cell.Border) return string.Empty;
            return string.Format("class=\"noborder\"");
        }
        private string ContentFor(Cell cell)
        {
            string classes = ClassesFor(cell);
            if (string.IsNullOrEmpty(classes)) return Encode(cell.Text);
            return string.Format("<span class=\"{0}\">{1}</span>", classes, Encode(cell.Text));
        }
        private string Encode(string message)
        {
            return HttpUtility.HtmlEncode(message);
        }
        private string ClassesFor(Cell cell)
        {
            string classes = string.Empty;
            classes += AlignmentFor(cell);
            classes += StyleFor(cell);
            return classes;
        }
        private string AlignmentFor(Cell cell)
        {
            if (cell.Alignment != ReportAlignments.Right) return string.Empty;
            return HorizontalAlignRight + " ";
        }
        private string StyleFor(Cell cell)
        {
            if (cell.Style != ReportStyles.Bold) return string.Empty;
            return FontWeightBold + " ";
        }
        #endregion
    }
}