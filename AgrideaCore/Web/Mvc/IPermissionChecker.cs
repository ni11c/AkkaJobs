using System.Web.Mvc;

namespace Agridea.Web.Mvc
{
    public interface IPermissionChecker
    {
        string CheckUserPermissions(ActionExecutingContext filterContext);
    }
}