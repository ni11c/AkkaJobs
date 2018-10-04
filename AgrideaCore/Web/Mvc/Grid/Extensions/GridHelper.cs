using Agridea.Diagnostics.Contracts;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using System.Web.Routing;

namespace Agridea.Web.Mvc.Grid
{
    public static class GridHelper
    {
        public static string GetDefaultOrderProperty<T>()
        {
            Asserts<ArgumentException>.IsTrue(typeof(T).GetProperty(GridParameters.DefaultPropertyName) != null,
                string.Format("If you don't define any ordering, {0} should have a property {1}", typeof(T).Name, GridParameters.DefaultPropertyName));
            return GridParameters.DefaultPropertyName;
        }
        public static string BuildUrl<T>(this ControllerContext context, T item, RouteValueDictionary routeValues, IList<IGridDataKey<T>> dataKeys)
        {
            foreach (var dataKey in dataKeys.Where(dataKey => routeValues.ContainsKey(dataKey.Name)))
            {
                routeValues[dataKey.Name] = dataKey.GetValue(item);
            }
            return context.BuildUrl(routeValues);
        }

        public static string BuildUrl(this ControllerContext context, RouteValueDictionary routeValues, IEnumerable<IGridDataKey> dataKeys)
        {
            foreach (var dataKey in dataKeys)
            {
                if (routeValues.ContainsKey(dataKey.Name))
                {
                    var queryString = context.HttpContext.Request.QueryString;
                    if (queryString.HasKeys() && queryString[dataKey.Name] != null)
                    {
                        routeValues[dataKey.Name] = queryString[dataKey.Name];
                    }
                }
            }
            return context.BuildUrl(routeValues);
        }

        private static string BuildUrl(this ControllerContext context, RouteValueDictionary routeValues)
        {
            return UrlHelper.GenerateUrl(null, null, null, routeValues, RouteTable.Routes, context.RequestContext, true);

        }
        public static IDictionary<string, object> DictionaryParse(this object withProperties)
        {
            return TypeDescriptor.GetProperties(withProperties)
                .Cast<PropertyDescriptor>()
                .ToDictionary(property => property.Name, property => property.GetValue(withProperties));
        }

        public static RouteValueDictionary GetRouteValueDictionary(this ControllerContext context, string methodName, bool usePostAction)
        {
            var routeValues = new RouteValueDictionary();
            var controller = context.Controller;
            var controllerName = MvcExpressionHelper.GetControllerName(controller.GetType());
            routeValues.Add(GridParameters.ControllerKey, controllerName);
            routeValues.Add(GridParameters.ActionKey, methodName);

            var methods = controller.GetType().GetMethods().Where(m => m.Name == methodName);
            MethodInfo methodInfo;

            if (methods.Count() == 1)
                methodInfo = methods.First();
            else
                methodInfo = methods.First(x => usePostAction
                                                    ? x.GetCustomAttributes(true).OfType<HttpPostAttribute>().Any()
                                                    : !x.GetCustomAttributes(true).OfType<HttpPostAttribute>().Any());

            Requires<ArgumentException>.IsNotNull(methodInfo, string.Format("There's no method {0} in controller {1}", methodName, controllerName));

            foreach (var parameter in methodInfo.GetParameters())
                routeValues.Add(parameter.Name, parameter.ParameterType.IsValueType ? Activator.CreateInstance(parameter.ParameterType) : null);
            return routeValues;
        }
    }
}
