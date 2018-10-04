using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Agridea.Reports
{
    /// <summary>
    /// Transforms a report model into a csv string / file
    /// </summary>
    public class CsvReportMaker : IReportMaker
    {
        #region Members
        private List<Header> appendedHeaders_;
        private List<Cell> appendedCells_;
        #endregion

        #region Initialization
        public CsvReportMaker()
        {
            appendedHeaders_ = new List<Header>();
            appendedCells_ = new List<Cell>();
        }
        #endregion

        #region IReportMaker
        public object Make(Report report, bool addHeader = false)
        {
            var stringBuilder = new StringBuilder();
            var headersAlreadyAppended = addHeader;

            foreach (var table in report.Tables)
            {
                if (TableContainsAppendedCells(table))
                {
                    RecordAppendedCells(table);
                    continue;
                }

                GenerateCells(stringBuilder, ref headersAlreadyAppended, table);
            }

            return stringBuilder.ToString();
        }
        public void Write(Report report, string filePath)
        {
            File.WriteAllText(filePath, Make(report) as string);
        }
        public void Append(Report report, string filePath)
        {
            File.AppendAllText(filePath, Make(report) as string);
        }
        #endregion

        #region Helpers
        private bool TableContainsAppendedCells(Table table)
        {
            return table.Rows.Any(x => x.Headers.Any(y => y.Export == ExportModes.Append) || x.Cells.Any(y => y.Export == ExportModes.Append));
        }
        private void RecordAppendedCells(Table table)
        {
            appendedHeaders_.Clear();
            appendedCells_.Clear();

            foreach (var row in table.Rows)
            {
                foreach (var header in row.Headers)
                    if (header.Export == ExportModes.Append)
                        appendedHeaders_.Add(header);

                foreach (var cell in row.Cells)
                    if (cell.Export == ExportModes.Append)
                        appendedCells_.Add(cell);
            }
        }
        private void GenerateCells(StringBuilder stringBuilder, ref bool headersAlreadyAppended, Table table)
        {
            foreach (var row in table.Rows)
            {
                GenerateHeadersForRow(stringBuilder, ref headersAlreadyAppended, row);
                GenerateCellsForRow(stringBuilder, row);
            }
        }
        private void GenerateHeadersForRow(StringBuilder stringBuilder, ref bool headersAlreadyAppended, Row row)
        {
            if (row.Headers.All(x => x.Export == ExportModes.None) || headersAlreadyAppended) return;

            foreach (var appendedHeader in appendedHeaders_)
                stringBuilder.Append(string.Format("{0};", TextFor(appendedHeader)));

            foreach (var header in row.Headers)
            {
                if (header.Export == ExportModes.None) continue;

                stringBuilder.Append(string.Format("{0};", TextFor(header)));
                headersAlreadyAppended = true;
            }
            stringBuilder.Append(Environment.NewLine);
        }
        private void GenerateCellsForRow(StringBuilder stringBuilder, Row row)
        {
            if (row.Cells.All(x => x.Export == ExportModes.None) || row.Cells.Length == 0) return;

            foreach (var appendedCell in appendedCells_)
                stringBuilder.Append(string.Format("{0};", TextFor(appendedCell)));

            foreach (var cell in row.Cells)
            {
                if (cell.Export == ExportModes.None) continue;

                stringBuilder.Append(string.Format("{0};", TextFor(cell)));
            }
            stringBuilder.Append(Environment.NewLine);
        }
        private string TextFor(Cell cell)
        {
            return Encode(cell.ExportText ?? cell.Text);
        }
        private string Encode(string message)
        {
            //return UTF32Encoding.UTF8.GetString(UTF32Encoding.UTF8.GetBytes(message));
            return message;
        }
        #endregion
    }
}
