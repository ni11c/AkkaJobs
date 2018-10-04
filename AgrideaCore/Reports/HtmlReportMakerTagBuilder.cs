#define DEBUGGING_OFF

using System.IO;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Agridea.Web.Mvc;

namespace Agridea.Reports
{
    public class HtmlReportMakerTagBuilder : IReportMaker
    {
        #region Constants
        private const string HorizontalAlignRight = "horizontalAlignRight";
        private const string FontWeightBold = "fontWeightBold";
        #endregion

        #region IReportMaker
        public object Make(Report report, bool addHeader = false)
        {
            var tagBuilder = new FluentTagBuilder(FluentTagBuilder.Div);

#if DEBUGGING_ON
            FluentTagBuilder styleTagBuilder = new FluentTagBuilder(FluentTagBuilder.Style);
            styleTagBuilder.Attributes.Add(FluentTagBuilder.Type, "text/css");
            styleTagBuilder.Html(string.Format("th {{ text-align:left; font-weight:normal;background: #97BF0D; border:1px solid #496D2D;color: #fff;}}"));
            styleTagBuilder.Html(string.Format("td {{ font-weight:normal;border:1px solid #496D2D;}}"));
            styleTagBuilder.Html(string.Format(".horizontalAlignRight {{ float:right; }}"));
            styleTagBuilder.Html(string.Format(".fontWeightBold {{ font-weight:bold;}}"));
            styleTagBuilder.Html(string.Format(".noborder {{  border:none;}}"));
            tagBuilder.Html(styleTagBuilder);
#endif

            MakeReport(report, tagBuilder);

            return MvcHtmlString.Create(tagBuilder.ToString());
        }
        public void Write(Report report, string filePath)
        {
            var content = Make(report) as MvcHtmlString;
            File.WriteAllText(filePath, content.ToHtmlString(), Encoding.UTF8);
        }
        public void Append(Report report, string filePath)
        {
            var content = Make(report) as MvcHtmlString;
            File.AppendAllText(filePath, content.ToHtmlString(), Encoding.UTF8);
        }
        #endregion

        #region Helpers
        private void MakeReport(Report report, FluentTagBuilder tagBuilder)
        {
            foreach (var table in report.Tables) MakeTable(table, tagBuilder);
        }
        private void MakeTable(Table table, FluentTagBuilder tagBuilder)
        {
            var tableTagBuilder = Tag.Table
                .Style(string.Format("width:{0}%", table.TotalWidth));

            MakeTableColumns(table, tableTagBuilder);
            MakeTableRows(table, tableTagBuilder);

            tagBuilder
                .Html(tableTagBuilder)
                .Html(Tag.Br);
        }
        private void MakeTableColumns(Table table, FluentTagBuilder tagBuilder)
        {
            foreach (int relativeWidth in table.RelativeWidths) MakeTableColumn(relativeWidth, tagBuilder);
        }
        private void MakeTableColumn(int relativeWidth, FluentTagBuilder tagBuilder)
        {
            //FluentTagBuilder colTagBuilder = new FluentTagBuilder(FluentTagBuilder.Col);
            //colTagBuilder.Attributes.Add(FluentTagBuilder.Style, string.Format("width:{0}%", relativeWidth));
            tagBuilder.Html(string.Format("<col style=\"width:{0}%\">", relativeWidth));
        }
        private void MakeTableRows(Table table, FluentTagBuilder tagBuilder)
        {
            foreach (var row in table.Rows) MakeTableRow(row, tagBuilder);
        }
        private void MakeTableRow(Row row, FluentTagBuilder tagBuilder)
        {
            var rowTagBuilder = Tag.Tr;

            MakeTableHeaders(row, rowTagBuilder);
            MakeTableCells(row, rowTagBuilder);

            tagBuilder.Html(rowTagBuilder);
        }
        private void MakeTableHeaders(Row row, FluentTagBuilder tagBuilder)
        {
            foreach (var header in row.Headers) MakeTableCell(header, FluentTagBuilder.Th, tagBuilder);
        }
        private void MakeTableCells(Row row, FluentTagBuilder tagBuilder)
        {
            foreach (var cell in row.Cells) MakeTableCell(cell, FluentTagBuilder.Td, tagBuilder);
        }
        private void MakeTableCell(Cell cell, string tag, FluentTagBuilder tagBuilder)
        {
            var cellTagBuilder = new FluentTagBuilder(tag);

            ColSpanFor(cell, cellTagBuilder);
            BorderFor(cell, cellTagBuilder);
            ContentFor(cell, cellTagBuilder);

            tagBuilder.Html(cellTagBuilder);
        }
        private void ColSpanFor(Cell cell, FluentTagBuilder tagBuilder)
        {
            if (cell.ColSpan == 1) return;
            tagBuilder.Colspan(cell.ColSpan);
        }
        private void BorderFor(Cell cell, FluentTagBuilder tagBuilder)
        {
            if (cell.Border) return;
            tagBuilder.Class("noborder");
        }
        private void ContentFor(Cell cell, FluentTagBuilder tagBuilder)
        {
            var dummyTagBuilder = new FluentTagBuilder("dummy");

            if (!ClassesFor(cell, dummyTagBuilder))
            {
                tagBuilder.Html(Encode(cell.Text));
                return;
            }

            var spanTagBuilder = Tag.Span;
            ClassesFor(cell, spanTagBuilder);
            spanTagBuilder.Html(Encode(cell.Text));

            tagBuilder.Html(spanTagBuilder);
        }
        private string Encode(string message)
        {
            return HttpUtility.HtmlEncode(message);
        }
        private bool ClassesFor(Cell cell, FluentTagBuilder tagBuilder)
        {
            var classesExist = false;
            classesExist |= AlignmentFor(cell, tagBuilder);
            classesExist |= StyleFor(cell, tagBuilder);
            return classesExist;
        }
        private bool AlignmentFor(Cell cell, FluentTagBuilder tagBuilder)
        {
            if (cell.Alignment != ReportAlignments.Right) return false;
            tagBuilder.Class(HorizontalAlignRight);
            return true;
        }
        private bool StyleFor(Cell cell, FluentTagBuilder tagBuilder)
        {
            if (cell.Style != ReportStyles.Bold) return false;
            tagBuilder.Class(FontWeightBold);
            return true;
        }
        #endregion
    }
}