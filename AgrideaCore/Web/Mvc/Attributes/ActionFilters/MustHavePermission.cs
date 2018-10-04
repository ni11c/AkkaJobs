using Agridea.Diagnostics.Contracts;
using Agridea.Diagnostics.Logging;
using Agridea.Security;
using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;

namespace Agridea.Web.Mvc.ActionFilters
{
    public class MustHavePermissionAttribute : ActionFilterAttribute
    {
        #region Named Parameters

        public bool Off { get; set; }

        public string RedirectionController { get; set; }

        public string RedirectionAction { get; set; }

        public static bool Disabled { get; set; }

        #endregion Named Parameters

        #region Members
        public const string NotImplementingPermissionErrorMessage = "The controller doesn't implement IPermissionChecker";
        #endregion Members

        #region Initialization

        public MustHavePermissionAttribute()
        {
            RedirectionController = "Home";
            RedirectionAction = "Index";
        }

        #endregion Initialization

        #region ActionFilterAttribute

        public override sealed void OnActionExecuting(ActionExecutingContext filterContext)
        {
            string message = WillRedirect(filterContext);
            if (message != null)
            {
                SetMessage(filterContext, message);

                Log.Warning(string.Format("'{0}' Redirecting from '{1}.{2}({3})child {4}, off inherited {5}' to '{6}.{7}' '{8}'",
                      GetType().Name,
                      ActionExecutingContextHelper.GetControllerName(filterContext),
                      ActionExecutingContextHelper.GetActionName(filterContext),
                      ActionExecutingContextHelper.GetHttpMethod(filterContext),
                      ActionExecutingContextHelper.IsChildAction(filterContext),
                      IsOffInherited(filterContext),
                      RedirectionController,
                      RedirectionAction,
                      message));

                filterContext.Result = new RedirectToRouteResult(
                    new RouteValueDictionary
                    {
                        {MvcConstants.ControllerRouteValueKey, RedirectionController},
                        {MvcConstants.ActionRouteValueKey, RedirectionAction}
                    });

                OnRedirecting(filterContext);
            }

            base.OnActionExecuting(filterContext);
        }

        #endregion ActionFilterAttribute

        #region Hooks

        protected virtual string NeedRedirection(ActionExecutingContext filterContext)
        {
            var permissionChecker = filterContext.Controller as IPermissionChecker;
            var action = ActionExecutingContextHelper.GetAction(filterContext);
            if (action == null)
                return "Url non existante";
            if (action.IsAlwaysAuthorized()) return null;

            if (permissionChecker == null)
                return NotImplementingPermissionErrorMessage;

            return permissionChecker.CheckUserPermissions(filterContext);
        }

        protected virtual void OnRedirecting(ActionExecutingContext filterContext)
        {
        }

        #endregion Hooks

        #region Helpers

        private string WillRedirect(ActionExecutingContext filterContext)
        {
            if (Disabled) return null;
            if (Off) return null;
            if (IsOffInherited(filterContext)) return null;
            if (ActionExecutingContextHelper.IsChildAction(filterContext)) return null;
            if (ActionExecutingContextHelper.GetActionName(filterContext) == RedirectionAction && ActionExecutingContextHelper.GetControllerName(filterContext) == RedirectionController) return null;
            return NeedRedirection(filterContext);
        }

        protected bool IsOffInherited(ActionExecutingContext filterContext)
        {
            //Prevents from re-enabling some filter if it was disabled in some super controller
            var offInherited = filterContext
                .ActionDescriptor
                .GetCustomAttributes(inherit: true)
                .OfType<MustHavePermissionAttribute>()
                .FirstOrDefault(m => m.GetType() == GetType());
            return offInherited != null && offInherited.Off;
        }

        protected void SetMessage(ActionExecutingContext filterContext, string message)
        {
            Asserts<ArgumentNullException>.IsNotNull(filterContext.ActionDescriptor);
            Asserts<ArgumentNullException>.IsNotNull(message);
            filterContext.Controller.TempData[MvcConstants.RedirectMessageKey] = message;
        }

        #endregion Helpers
    }
}