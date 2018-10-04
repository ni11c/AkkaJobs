using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Agridea.Web.Mvc.Grid.Columns;

namespace Agridea.Web.Mvc.Grid.Renderers
{
    public class HeaderRenderer : IHtmlString
    {
        private readonly IGridModel model_;
        public HeaderRenderer(IGridModel model)
        {
            model_ = model;
        }

        // For backwards compatibility with WebFormViewEngine
        public override string ToString()
        {
            return ToHtmlString();
        }

        public string ToHtmlString()
        {
            var visibleColumns = model_.DisplayColumns.ToList();
            var hasRowspan = visibleColumns.Any(m => m.Rowspan == 2);
            var firstRow = Tag.Tr;
            if (!hasRowspan)
            {
                foreach (var column in visibleColumns.Where(column => column.HasTitle)) {
                    if (column.CustomHeader == null)
                        firstRow.Html(column.RenderHeader());
                    else
                        firstRow.Html(column.CustomHeader.Render());
                }
                return firstRow.ToString();
            }
            foreach (var column in visibleColumns.Where(column => column.HasTitle)) {
                if (column.Rowspan == 2)
                    firstRow.Html(column.RenderHeader());
                if (column.CustomHeader != null)
                    firstRow.Html(column.CustomHeader.Render());
            }
            var secondRow = Tag.Tr;
            foreach (var column in visibleColumns.Where(m => m.Rowspan == 1))
                secondRow.Html(column.RenderHeader());

            return firstRow.ToString() + secondRow.ToString();
        }
    }
}
