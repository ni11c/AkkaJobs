using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web;
using System.Web.Mvc.Html;

namespace Agridea.Web.Mvc.Grid.Columns
{
    public class GridEditorColumn<T, TValue> : GridEditColumn<T, TValue>
    {
        public GridEditorColumn(IGridModel<T> gridModel, string name, Func<T, TValue> func) : base(gridModel, name, func) { }
        public override IHtmlString RenderInput(T dataItem)
        {
            return GridModel.Helper.Editor(GetIndexedName(dataItem));
        }
    }
}
