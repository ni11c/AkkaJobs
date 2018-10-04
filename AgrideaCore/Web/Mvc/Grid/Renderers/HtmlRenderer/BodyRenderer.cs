using Agridea.Web.Mvc.Grid.Columns;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;

namespace Agridea.Web.Mvc.Grid.Renderers
{
    public class BodyRenderer<T>
    {
        private readonly IGridModel<T> gridModel_;

        public BodyRenderer(IGridModel<T> gridModel)
        {
            gridModel_ = gridModel;
        }

        public FluentTagBuilder Render(FluentTagBuilder tbody, IList<T> items)
        {
            var columns = gridModel_.VisibleColumns.ToList();
            FluentTagBuilder tag = null;
            foreach (var item in items)
            {
                var innerTr = Tag.Tr;
                foreach (var column in gridModel_.HiddenColumns)
                    innerTr.Html(GetContent(column, item));

                foreach (var column in columns)
                {
                    var index = columns.IndexOf(column);
                    var nextColumnIsMerged = index < columns.Count - 1 && columns[index + 1].IsMerged;

                    if (!column.IsMerged)
                        tag = column.GetContentTag(item);

                    tag.Html(GetContent(column, item));

                    if (!nextColumnIsMerged)
                        innerTr.Html(tag);
                }

                tbody.Html(innerTr);
            }
            return tbody;
        }

        private string GetContent(GridColumnBase<T> column, T item)
        {
            return column.GetWarning(item) + column.GetContent(item);
        }
    }
}