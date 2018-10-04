using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web;
using System.Web.Mvc.Html;

namespace Agridea.Web.Mvc.Grid.Columns
{
    public class GridCalendarNullableColumn<T> : GridEditColumn<T, DateTime?>
    {
        public GridCalendarNullableColumn(IGridModel<T> gridModel, string name, Func<T, DateTime?> func)
            : base(gridModel, name, func)
        {

        }
        public override IHtmlString RenderInput(T dataItem)
        {
            var value = Value(dataItem);
            var content = value == null ? string.Empty : Convert.ToDateTime(value).ToShortDateString();

            return GridModel.Helper.TextBox(GetIndexedName(dataItem), content, GetAttributes(dataItem));
        }

        public override IDictionary<string, object> GetAttributes(T dataItem)
        {
            var dic = new Dictionary<string, object>
            {
                {"class", "datePicker"}
            };

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

    public class GridCalendarColumn<T> : GridEditColumn<T, DateTime>
    {
        private readonly int? minRange_;

        public GridCalendarColumn(IGridModel<T> gridModel, string name, Func<T, DateTime> func, int? minRange = null)
            : base(gridModel, name, func)
        {
            minRange_ = minRange;
        }
        public override IHtmlString RenderInput(T dataItem)
        {
            var value = Value(dataItem);
            var content = value.ToShortDateString();

            return GridModel.Helper.TextBox(GetIndexedName(dataItem), content, GetAttributes(dataItem));
        }

        public override IDictionary<string, object> GetAttributes(T dataItem)
        {
            var dic = new Dictionary<string, object>
            {
                {"class", "datePicker"}
            };

            if (GetDisabled(dataItem))
            {
                dic.Add("disabled", "disabled");
            }
            if (GetReadOnly(dataItem))
            {
                dic.Add("readonly", "readonly");
            }

            if (minRange_ != null)
            {
                dic.Add("min_year", minRange_);
            }

            return dic;
        }
    }

}
