using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Agridea.Web.Mvc.Grid.Columns;

namespace Agridea.Web.Mvc.Grid.Renderers
{
    public class CsvRenderer<T>
    {
        private readonly IGridModel<T> gridModel_;
        private StringBuilder builder_;
        private const string Format = "{0};";
        public CsvRenderer(IGridModel<T> gridModel)
        {
            gridModel_ = gridModel;
            builder_ = new StringBuilder();
        }

        public string Render()
        {
            var columns = gridModel_.VisibleColumns.Where(m => m.IsVisibleForExport).ToList();

            foreach (var column in columns)
                AddHeader(column);
            builder_.Append(Environment.NewLine);

            foreach (var item in gridModel_.GetAllItems())
            {
                foreach (var column in columns)
                    AddContent(column, item);
                builder_.Append(Environment.NewLine);
            }

            return builder_.ToString();
        }

        public override string ToString()
        {
            return Render();
        }

        private void AddHeader(GridColumnBase<T> column)
        {
            builder_.AppendFormat(Format, column.GetExportHeader());
        }
        private void AddContent(GridColumnBase<T> column, T dataItem)
        {
            builder_.AppendFormat(Format, column.GetExportContent(dataItem));
        }
    }
}
