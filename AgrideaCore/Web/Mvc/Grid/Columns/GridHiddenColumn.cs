using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace Agridea.Web.Mvc.Grid.Columns
{
    public class GridHiddenColumn<T, TValue> : GridEditColumn<T, TValue>
    {
        public GridHiddenColumn(IGridModel<T> gridModel, string name, Func<T, TValue> func, object defaultvalue = null)
            : base(gridModel, name, func)
        {
            DefaultValue = defaultvalue;
            IsVisibleForExport = false;
        }
        public override IHtmlString RenderInput(T dataItem)
        {
            return GridModel.Helper.Hidden(GetIndexedName(dataItem), DefaultValue ?? Value(dataItem));
        }
    }
}
