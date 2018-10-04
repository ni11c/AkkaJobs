using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace Agridea.Web.Mvc.Grid.Columns
{
    public class GridTextBoxColumn<T, TValue> : GridEditColumn<T, TValue>
    {
        public GridTextBoxColumn(IGridModel<T> gridModel, string name, Func<T, TValue> func, int? inputSize)
            : base(gridModel, name, func)
        {
            this.InputSize = inputSize;
        }
        public override IHtmlString RenderInput(T dataItem)
        {
            return GridModel.Helper.TextBox(GetIndexedName(dataItem), Value(dataItem), GetAttributes(dataItem));
        }

    }
}
