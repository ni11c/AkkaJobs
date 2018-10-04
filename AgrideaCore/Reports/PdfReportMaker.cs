using Agridea.iTextSharp;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.IO;

namespace Agridea.Reports
{
    /// <summary>
    /// Generic Pdf report maker whose responsibility pertains only to
    /// - fill the body of a Pdf report from a report model
    /// The layout and styling is provided by a derivative of PdfBase
    /// </summary>
    public class PdfReportMaker : IReportMaker, IPdfBodyDelegate
    {
        #region Members

        private Report report_;
        private PdfBase pdfBase_;

        #endregion Members

        #region Initialization

        public PdfReportMaker(PdfBase pdfBase)
        {
            pdfBase_ = pdfBase;
            pdfBase_.BodyDelegate = this;
        }

        #endregion Initialization

        #region IReportMaker

        public object Make(Report report, bool addHeader = false)
        {
            report_ = report;
            return pdfBase_.CreatePdf();
        }

        public void Write(Report report, string filePath)
        {
            File.WriteAllBytes(filePath, Make(report) as byte[]);
        }

        public void Append(Report report, string filePath)
        {
            //reeopen doc, merge...
            throw new NotImplementedException();
        }

        #endregion IReportMaker

        #region IPdfBodyDelegate

        public void AddBody(Document document)
        {
            foreach (var table in report_.Tables)
            {
                PdfPTable pdfPTable = new PdfPTable(table.RelativeWidths)
                {
                    WidthPercentage = table.TotalWidth,
                    SpacingBefore = 5f,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };

                foreach (var row in table.Rows)
                {
                    foreach (var header in row.Headers)
                        AddNewCellTo(pdfPTable, header);
                    pdfPTable.HeaderRows = table.HeaderRows;

                    foreach (var cell in row.Cells)
                        AddNewCellTo(pdfPTable, cell);
                    pdfPTable.CompleteRow();
                }
                document.Add(pdfPTable);

                if (table.BreakPageAfter)
                    pdfBase_.AddNewPage(document);
                else
                    pdfBase_.AddNewLine(document);
            }
        }

        #endregion IPdfBodyDelegate

        #region Helpers

        private void AddNewCellTo(PdfPTable pdfPTable, Cell cell)
        {
            pdfPTable.AddCell(new PdfPCell(new Phrase(Encode(cell.Text), StyleFor(cell.Style)))
            {
                Colspan = cell.ColSpan,
                BackgroundColor = ColorFor(cell.Color),
                HorizontalAlignment = AlignmentFor(cell.Alignment),
                Border = BorderFor(cell.Border)
            });
        }

        private string Encode(string message)
        {
            return message;
        }

        private BaseColor ColorFor(ReportColors color)
        {
            switch (color)
            {
                case ReportColors.Dark:
                    return pdfBase_.DarkColor;

                case ReportColors.Light:
                    return pdfBase_.LightColor;

                default:
                    return BaseColor.WHITE;
            }
        }

        private int AlignmentFor(ReportAlignments alignment)
        {
            switch (alignment)
            {
                case ReportAlignments.Left:
                    return PdfPCell.ALIGN_LEFT;

                case ReportAlignments.Center:
                    return PdfPCell.ALIGN_CENTER;

                case ReportAlignments.Right:
                    return PdfPCell.ALIGN_RIGHT;

                default:
                    return PdfPCell.ALIGN_UNDEFINED;
            }
        }

        private int BorderFor(bool border)
        {
            switch (border)
            {
                case true:
                    return Rectangle.BOX;

                default:
                    return Rectangle.NO_BORDER;
            }
        }

        private Font StyleFor(ReportStyles style)
        {
            switch (style)
            {
                case ReportStyles.Bold:
                    return pdfBase_.Bold;

                case ReportStyles.Italic:
                    return pdfBase_.Italic;

                default:
                    return pdfBase_.Normal;
            }
        }

        #endregion Helpers
    }
}