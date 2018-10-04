using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Web.Mvc;
using System.Web.Routing;
using Agridea.Diagnostics.Contracts;
using Agridea.Diagnostics.Logging;

namespace Agridea.Web.Mvc
{
    public static class MvcExpressionHelper
    {
        #region Constants
        private const string ControllerMarker = "Controller";
        #endregion

        #region Services
        public static RouteValueDictionary GetRouteValuesFromExpression<TController>(Expression<Action<TController>> action) where TController : Controller
        {
            Requires<ArgumentNullException>.IsNotNull(action);

            MethodCallExpression call = action.Body as MethodCallExpression;
            Requires<ArgumentException>.IsNotNull(call);

            var controllerName = GetControllerName(typeof(TController));
            Requires<ArgumentException>.GreaterThan(controllerName.Length, 0);

            //How do we know that this method is even web callable?
            //For now, we just let the call itself throw an exception.

            var rvd = new RouteValueDictionary();
            rvd.Add(MvcConstants.ControllerRouteValueKey, controllerName);
            rvd.Add(MvcConstants.ActionRouteValueKey, call.Method.Name);
            AddParameterValuesFromExpressionToDictionary(rvd, call);
            return rvd;
        }
        public static string GetControllerName(Type controllerType)
        {
            Asserts<ArgumentNullException>.IsNotNull(controllerType);
            var controllerName = controllerType.Name;
            Requires<ArgumentException>.IsTrue(controllerName.Contains(ControllerMarker));
            return controllerName.Substring(0, controllerName.IndexOf(ControllerMarker, StringComparison.Ordinal));
        }

        public static string GetInputName<TModel, TProperty>(Expression<Func<TModel, TProperty>> expression)
        {
            if (expression.Body.NodeType == ExpressionType.Call)
            {
                MethodCallExpression methodCallExpression = (MethodCallExpression)expression.Body;
                string name = GetInputName(methodCallExpression);
                return name.Substring(expression.Parameters[0].Name.Length + 1);

            }
            return expression.Body.ToString().Substring(expression.Parameters[0].Name.Length + 1);
        }
        static void AddParameterValuesFromExpressionToDictionary(RouteValueDictionary rvd, MethodCallExpression call)
        {
            ParameterInfo[] parameters = call.Method.GetParameters();

            if (parameters.Length > 0)
            {
                for (int i = 0; i < parameters.Length; i++)
                {
                    Expression arg = call.Arguments[i];
                    object value = null;
                    ConstantExpression ce = arg as ConstantExpression;
                    if (ce != null)
                    {
                        // If argument is a constant expression, just get the value
                        value = ce.Value;
                    }
                    else
                    {
                        // Otherwise, convert the argument subexpression to type object,
                        // make a lambda out of it, compile it, and invoke it to get the value
                        Expression<Func<object>> lambdaExpression = Expression.Lambda<Func<object>>(Expression.Convert(arg, typeof(object)));
                        Func<object> func = lambdaExpression.Compile();
                        value = func();
                    }
                    rvd.Add(parameters[i].Name, value);
                }
            }
        }
        #endregion

        #region Helpers
        private static string GetInputName(MethodCallExpression expression)
        {
            // p => p.Foo.Bar().Baz.ToString() => p.Foo OR throw...

            MethodCallExpression methodCallExpression = expression.Object as MethodCallExpression;
            if (methodCallExpression != null)
            {
                return GetInputName(methodCallExpression);
            }
            return expression.Object.ToString();
        }
        #endregion
    }
}
