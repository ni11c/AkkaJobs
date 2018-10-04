using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Agridea.ObjectMapping
{
    public static class ExpressionExtensions
    {
        #region Services
        public static string GetName<TSource>(this Expression<Func<TSource, object>> expression)
            where TSource : class
        {
            return expression.GetMemberExpression().Member.Name;
        }
        public static string GetPath<TSource>(this Expression<Func<TSource, object>> expression)
            where TSource : class
        {
            var path = System.Web.Mvc.ExpressionHelper.GetExpressionText(expression);
            return !string.IsNullOrEmpty(path) ?
                path :
                string.Join(PropertyPath.PropertyPathSeparator.ToString(), RemoveUnary(expression.Body).ToString().Split(PropertyPath.PropertyPathSeparator).Skip(1));
        }
        public static IList<PropertyInfo> GetPropertyInfos<TSource>(this Expression<Func<TSource, object>> expression)
            where TSource : class
        {
            Type currentType = typeof(TSource);
            List<PropertyInfo> propertyInfos = new List<PropertyInfo>();

            var path = expression.GetPath();
            if(path.Length == 0 ) return propertyInfos;

            var pathComponents = path.Split(PropertyPath.PropertyPathSeparator);

            foreach (var pathComponent in pathComponents)
            {
                var currentPropertyInfo = currentType.GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(x => x.Name == pathComponent).FirstOrDefault();
                propertyInfos.Add(currentPropertyInfo);
                currentType = currentPropertyInfo.PropertyType;
            }

            return propertyInfos;
        }
        #endregion

        #region Helpers
        private static MemberExpression GetMemberExpression<TSource>(this Expression<Func<TSource, object>> expression) where TSource : class
        {
            var body = RemoveUnary(expression.Body);
            return body as MemberExpression;
        }
        public static Expression RemoveUnary(this Expression body)
        {
            var unary = body as UnaryExpression;
            return unary != null ? unary.Operand : body;
        }
        #endregion
    }
}
