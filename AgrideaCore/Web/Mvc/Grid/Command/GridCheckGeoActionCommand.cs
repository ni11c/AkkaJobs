using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Routing;
using Org.BouncyCastle.Asn1.Esf;

namespace Agridea.Web.Mvc.Grid.Command
{
    public class GridCheckGeoActionCommand<T> : GridActionCommand<T>
    {
        public GridCheckGeoActionCommand(ControllerContext context, string spanClass, string alternateText, RouteValueDictionary routeValues, object htmlAttributes, Func<T, bool> checkValueFunc)
            : base(context, spanClass, alternateText, routeValues, htmlAttributes)
        {
            CheckValueFunc = checkValueFunc;
        }

        public Func<T, bool> CheckValueFunc { get; set; }

        #region Overrides of GridActionCommand<T>
        public override string Render(T dataItem, IList<IGridDataKey<T>> dataKeys, ControllerContext context)
        {
            if (CheckIfHidden(dataItem)) return string.Empty;

            var hasValue = CheckValueFunc != null && CheckValueFunc(dataItem);

            ImageUrl = hasValue
                ? GridClass.GeoCheckIcon
                : GridClass.GeoIcon;

            return base.Render(dataItem, dataKeys, context);
        }
        #endregion
    }
}
