using Agridea.Diagnostics.Contracts;
using Agridea.Web.Mvc.Session;
using System;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;

namespace Agridea.Web.Mvc.ActionFilters
{
    public static class ActionExecutingContextHelper
    {
        #region Constants
        private const string UndefinedUser = "?";
        #endregion

        #region Services
        public static Type GetController(ActionExecutingContext filterContext)
        {
            Asserts<ArgumentNullException>.IsNotNull(filterContext.ActionDescriptor);
            Asserts<ArgumentNullException>.IsNotNull(filterContext.ActionDescriptor.ControllerDescriptor);
            return filterContext.ActionDescriptor.ControllerDescriptor.ControllerType;
        }
        public static MethodInfo GetAction(ActionExecutingContext filterContext)
        {
            Asserts<ArgumentNullException>.IsNotNull(filterContext);

            var controllerType = GetController(filterContext);
            return controllerType.GetMethod(filterContext.ActionDescriptor.ActionName, filterContext.ActionDescriptor.GetParameters().Select(x => x.ParameterType).ToArray());
        }
        public static string GetControllerName(ActionExecutingContext filterContext)
        {
            Asserts<ArgumentNullException>.IsNotNull(filterContext.ActionDescriptor);
            Asserts<ArgumentNullException>.IsNotNull(filterContext.ActionDescriptor.ControllerDescriptor);
            return filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;
        }

        public static bool IsChildAction(ActionExecutingContext filterContext)
        {
            Asserts<ArgumentNullException>.IsNotNull(filterContext.ActionDescriptor);
            return filterContext.Controller.ControllerContext.IsChildAction;
        }
        public static bool ActionReturnsViewResult(ActionExecutingContext filterContext)
        {
            Asserts<ArgumentNullException>.IsNotNull(filterContext.ActionDescriptor);
            string actionName = filterContext.ActionDescriptor.ActionName;
            Type controllerType = filterContext.Controller.GetType();
            var actionInfo = controllerType.GetMethod(actionName, filterContext.ActionParameters.Select(p => p.Value.GetType() as Type).ToArray());
            return actionInfo.ReturnType.Name == "ViewResult";
        }
        public static string GetActionName(ActionExecutingContext filterContext)
        {
            Asserts<ArgumentNullException>.IsNotNull(filterContext.ActionDescriptor);
            return filterContext.ActionDescriptor.ActionName;
        }
        public static string GetHttpMethod(ActionExecutingContext filterContext)
        {
            Asserts<ArgumentNullException>.IsNotNull(filterContext.ActionDescriptor);
            return string.Format("{0}", filterContext.HttpContext.Request.HttpMethod);
        }
        public static string GetParameters(ActionExecutingContext filterContext)
        {
            Asserts<ArgumentNullException>.IsNotNull(filterContext.ActionParameters);
            string parameters = string.Empty;
            foreach (var parameter in filterContext.ActionParameters)
                parameters += string.Format("{0}={1};", parameter.Key, parameter.Value);
            return parameters;
        }
        public static string GetQueryString(ActionExecutingContext filterContext)
        {
            string parameters = string.Empty;
            if (filterContext.HttpContext.Request == null) return parameters;

            var queryString = filterContext.HttpContext.Request.QueryString;
            var queryStringAllKeys = queryString.AllKeys;
            if (queryStringAllKeys.Length == 0) return parameters;

            parameters = "QueryString='";
            foreach (var key in queryStringAllKeys)
                parameters += string.Format("{0}={1};", key, queryString[key].ToString());
            parameters += "'";
            return parameters;
        }
        #endregion
    }
}
