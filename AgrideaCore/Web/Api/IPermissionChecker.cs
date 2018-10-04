using System.Web.Http.Controllers;

namespace Agridea.Web.Api
{
    public interface IPermissionChecker
    {
        void Initialize();

        string CheckPermissions(HttpActionContext actionContext);
    }
}