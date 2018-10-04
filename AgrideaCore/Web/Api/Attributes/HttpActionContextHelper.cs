using Agridea.Diagnostics.Contracts;
using Agridea.Web.Mvc.Session;
using System;
using System.Linq;
using System.Reflection;
using System.Web.Http.Controllers;

namespace Agridea.Web.Api.ActionFilters
{
    public static class HttpActionContextHelper
    {
        #region Constants
        public const string SessionId = "SessionId";
        private const string UndefinedUser = "?";
        #endregion

        #region Services
        public static string GetSessionId(HttpActionContext actionContext)
        {
            Asserts<ArgumentNullException>.IsNotNull(actionContext);
            var sessionId = actionContext.Request.Headers.Contains(SessionId)
                ? actionContext.Request.Headers.GetValues(SessionId).FirstOrDefault()
                : UndefinedUser;
            return sessionId;
        }

        public static Type GetController(HttpActionContext actionContext)
        {
            Asserts<ArgumentNullException>.IsNotNull(actionContext.ActionDescriptor);
            Asserts<ArgumentNullException>.IsNotNull(actionContext.ActionDescriptor.ControllerDescriptor);
            return actionContext.ActionDescriptor.ControllerDescriptor.ControllerType;
        }
        public static MethodInfo GetAction(HttpActionContext actionContext)
        {
            Asserts<ArgumentNullException>.IsNotNull(actionContext);

            var controllerType = GetController(actionContext);
            return controllerType.GetMethod(actionContext.ActionDescriptor.ActionName, actionContext.ActionDescriptor.GetParameters().Select(x => x.ParameterType).ToArray());
        }
        public static string GetControllerName(HttpActionContext actionContext)
        {
            Asserts<ArgumentNullException>.IsNotNull(actionContext.ActionDescriptor);
            Asserts<ArgumentNullException>.IsNotNull(actionContext.ActionDescriptor.ControllerDescriptor);
            return actionContext.ActionDescriptor.ControllerDescriptor.ControllerName;
        }
        public static string GetActionName(HttpActionContext actionContext)
        {
            Asserts<ArgumentNullException>.IsNotNull(actionContext.ActionDescriptor);
            return actionContext.ActionDescriptor.ActionName;
        }
        public static string GetHttpMethod(HttpActionContext actionContext)
        {
            Asserts<ArgumentNullException>.IsNotNull(actionContext.Request);
            return string.Format("{0}", actionContext.Request.Method);
        }
        public static string GetParameters(HttpActionContext actionContext)
        {
            Asserts<ArgumentNullException>.IsNotNull(actionContext.ActionArguments);
            string parameters = string.Empty;
            foreach (var parameter in actionContext.ActionArguments)
                parameters += string.Format("{0}={1};", parameter.Key, parameter.Value);
            return parameters;
        }
        #endregion
    }
}
