using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Routing;

namespace Agridea.Web.Mvc.Grid.Command
{
    public class GridCheckEditActionCommand<T> : GridActionCommand<T>
    {
        public GridCheckEditActionCommand(ControllerContext context, string spanClass, string alternateText, RouteValueDictionary routeValues, object htmlAttributes, Func<T, bool> isAllowedFunc, Func<T, bool> checkValueFunc)
            : base(context, spanClass, alternateText, routeValues, htmlAttributes)
        {
            CheckValueFunc = checkValueFunc;
            IsAllowedFunc = isAllowedFunc;
        }

        public Func<T, bool> CheckValueFunc { get; set; }
        public Func<T, bool> IsAllowedFunc { get; set; }

        #region Overrides of GridActionCommand<T>
        public override string Render(T dataItem, IList<IGridDataKey<T>> dataKeys, ControllerContext context)
        {
            if (CheckIfHidden(dataItem)) return string.Empty;

                var isAllowed = IsAllowedFunc != null && IsAllowedFunc(dataItem);
                var hasValue = CheckValueFunc != null && CheckValueFunc(dataItem);

                if (!isAllowed && !hasValue)
                    return string.Empty;

                if (isAllowed)
                    ImageUrl = hasValue
                        ? GridClass.EditCheckIcon
                        : GridClass.EditIcon;
                else
                    ImageUrl = GridClass.BadEditCheckIcon;

            return base.Render(dataItem, dataKeys, context);
        }
        #endregion
    }
}
