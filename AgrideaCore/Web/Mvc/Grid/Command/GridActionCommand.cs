using System.Web.Mvc;
using System.Web.Routing;

namespace Agridea.Web.Mvc.Grid.Command
{
    public  class GridActionCommand<T> : GridActionCommandBase<T> 
    {

        #region Initialization
        public GridActionCommand(ControllerContext context, string spanClass, string alternateText, RouteValueDictionary routeValues, object htmlAttributes)
            : base(context, spanClass, alternateText, routeValues, htmlAttributes) {}
        #endregion

        #region Helpers
        #endregion
    }

    public class GridCreateActionCommand<T> : GridActionCommandBase<T>
    {
        public GridCreateActionCommand(ControllerContext context, string imageUrl, string alternateText, RouteValueDictionary routeValues, object htmlAttributes)
            : base(context, imageUrl, alternateText, routeValues, htmlAttributes) {}
    }

    public class GridExcelActionCommand<T> : GridActionCommandBase<T>
    {
        public GridExcelActionCommand(ControllerContext context, string imageUrl, string alternateText, RouteValueDictionary routeValues, object htmlAttributes)
            : base(context, imageUrl, alternateText, routeValues, htmlAttributes) { }
    }

    public class GridActionLinkCommand<T> : GridActionCommandBase<T>
    {
        public GridActionLinkCommand(ControllerContext context, RouteValueDictionary routeValues)
            : base(context, null, null, routeValues, null)
        {
            TextOnly = true;
            
        }
    }

}
