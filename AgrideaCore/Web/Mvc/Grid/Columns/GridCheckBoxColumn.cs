using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web;
using System.Web.Mvc.Html;

namespace Agridea.Web.Mvc.Grid.Columns
{
    public class GridCheckBoxColumn<T> : GridEditColumn<T, bool>
    {
        public GridCheckBoxColumn(IGridModel<T> gridModel, string name, Func<T, bool> func) : base(gridModel, name, func) { }
        public override IHtmlString RenderInput(T dataItem)
        {
            return GridModel.Helper.CheckBox(GetIndexedName(dataItem), Value(dataItem), GetAttributes(dataItem));
        }


        public override IDictionary<string, object> GetAttributes(T dataItem)
        {
            var dic = new Dictionary<string, object>();
            if (GetDisabled(dataItem))
            {
                dic.Add("disabled", "disabled");
            }
            if (GetReadOnly(dataItem))
            {
                dic.Add("readonly", "readonly");
            }
            
            return dic;
        }
    }
}
