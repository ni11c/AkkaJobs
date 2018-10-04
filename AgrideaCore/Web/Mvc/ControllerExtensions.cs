using Agridea.Diagnostics.Contracts;
using Agridea.Web.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;

namespace Agridea.Web.Mvc
{
    public static class ControllerExtensions
    {
        private static readonly Type HttpPostAttributeType = typeof(HttpPostAttribute);

        //TODO: Manually replace with native RecirectToAction
        public static RedirectToRouteResult RedirectToActionWithParameters<TController>(this TController controller, string actionName, object[] parameters)
            where TController : Controller
        {
            return RedirectToActionWithParameters<TController>(actionName, parameters);
        }

        //TODO: Manually replace with native RecirectToAction
        public static RedirectToRouteResult RedirectToActionWithParameters<TController>(this Controller controller, string actionName, object[] parameters)
            where TController : Controller
        {
            return RedirectToActionWithParameters<TController>(actionName, parameters);
        }

        //TODO: Manually replace with native RecirectToAction
        public static RedirectToRouteResult RedirectToActionWithParameters<TController>(this TController controller, string actionName, object[] parameters, object options)
            where TController : Controller
        {
            return RedirectToActionWithParameters<TController>(actionName, parameters, new RouteValueDictionary(options));
        }

        //TODO: Manually replace with native RecirectToAction
        public static RedirectToRouteResult RedirectToActionWithParameters<TController>(this Controller controller, string actionName, object[] parameters, object options)
            where TController : Controller
        {
            return RedirectToActionWithParameters<TController>(actionName, parameters, new RouteValueDictionary(options));
        }

        //TODO: Manually replace with native RecirectToAction
        public static RedirectToRouteResult RedirectToActionWithParameters<TController>(this TController controller, string actionName, object[] parameters, PaginationOptions options)
            where TController : Controller
        {
            return RedirectToActionWithParameters<TController>(actionName, parameters, new RouteValueDictionary(new { options.Order, options.Page, options.PageSize }));
        }

        private static RedirectToRouteResult RedirectToActionWithParameters<TController>(string actionName, object[] parameters, RouteValueDictionary otherValuesToMerge = null)
            where TController : Controller
        {
            Type controllerType = typeof(TController);
            int parametersCount = parameters.Count();

            var rvd = new RouteValueDictionary();
            rvd.Add(MvcConstants.ControllerRouteValueKey, MvcExpressionHelper.GetControllerName(controllerType));

            MethodInfo method = controllerType.GetMethods().SingleOrDefault(m => m.Name == actionName && !Attribute.IsDefined(m, HttpPostAttributeType) && m.GetParameters().Count() == parametersCount);
            Requires<ArgumentNullException>.IsNotNull(method);
            rvd.Add(MvcConstants.ActionRouteValueKey, method.Name);

            ParameterInfo[] methodParameters = method.GetParameters();
            for (int i = 0; i < parametersCount; ++i)
            {
                rvd.Add(methodParameters[i].Name, parameters[i]);
            }

            if (otherValuesToMerge != null)
            {
                rvd = rvd.Merge(otherValuesToMerge);
            }

            return new RedirectToRouteResult(rvd);
        }

        public static RouteValueDictionary GetRouteValuesFrom<TController>(Expression<Action<TController>> editLinkAction, int id)
            where TController : Controller
        {
            var routeValues = MvcExpressionHelper.GetRouteValuesFromExpression(editLinkAction);
            var idEntry = routeValues.First(entry => entry.Key.ToUpper().EndsWith("ID"));
            routeValues.Remove(idEntry.Key);
            routeValues.Add(idEntry.Key, id);
            return routeValues;
        }

        /*
        public static RedirectToRouteResult RedirectToAction<T>(this T controller, Expression<Action<T>> action)
            where T : Controller
        {
            return RedirectToActionInternal<T>(action, null);
        }

        public static RedirectToRouteResult RedirectToAction<T>(this Controller controller, Expression<Action<T>> action)
            where T : Controller
        {
            return RedirectToActionInternal<T>(action, null);
        }
        */

        public static RedirectToRouteResult RedirectToAction<T>(this T controller, Expression<Action<T>> action, object values)
            where T : Controller
        {
            return RedirectToActionInternal<T>(action, new RouteValueDictionary(values));
        }

        public static RedirectToRouteResult RedirectToAction<T>(this T controller, Expression<Action<T>> action, PaginationOptions options)
            where T : Controller
        {
            return controller.RedirectToAction(action, new { options.Order, options.Page, options.PageSize });
        }

        /*
        public static RedirectToRouteResult RedirectToAction<T>(this Controller controller, Expression<Action<T>> action, object values)
            where T : Controller
        {
            return RedirectToActionInternal<T>(action, new RouteValueDictionary(values));
        }
         *
        public static RedirectToRouteResult RedirectToAction<T>(this T controller, Expression<Action<T>> action, RouteValueDictionary values)
            where T : Controller
        {
            return RedirectToActionInternal<T>(action, values);
        }
         *
        public static RedirectToRouteResult RedirectToAction<T>(this Controller controller, Expression<Action<T>> action, RouteValueDictionary values)
            where T : Controller
        {
            return RedirectToActionInternal<T>(action, values);
        }
        */

        #region Helpers

        private static RedirectToRouteResult RedirectToActionInternal<T>(Expression<Action<T>> action, RouteValueDictionary values) where T : Controller
        {
            RouteValueDictionary parameters = MvcExpressionHelper.GetRouteValuesFromExpression(action)
                .Merge(values);
            return new RedirectToRouteResult(parameters);
        }

        #endregion Helpers
    }
}