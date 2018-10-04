using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Agridea.Diagnostics.Contracts;
using Agridea.Diagnostics.Logging;
using Agridea.Web.Helpers;
using System;
using System.Linq;

namespace Agridea.Linq
{
    /// <summary>
    /// Consider using DynamicLinq extensions instead
    /// </summary>
    [Obsolete("Use DynamicLinq (System.Linq.Dymamic) instead")]
    public static class LinqExtensions
    {
        #region Constants
        private static readonly string DefaultSortDirection = SortDirection.Ascending;
        private static readonly string OrderBy_ = "OrderBy";
        private static readonly string OrderByDesc_ = "OrderByDescending";
        #endregion

        #region Excluding
        public static IEnumerable<t> Excluding<t>(this IEnumerable<t> enumerable, Func<t, bool> predicate)
        {
            if (predicate == null)
                throw new ArgumentNullException("func");

            t[] inputArray = enumerable.ToArray();
            ProgressIndicator progress = new ProgressIndicator(inputArray.Length);
            var list = new List<t>(enumerable);
            for (var i = 0; i < inputArray.Count(); i++)
            {
                progress.Tick();
                if (predicate(inputArray[i]))
                    list.Remove(inputArray[i]);
            }
            return list;
        }
        #endregion

        #region OrderBy
        public static IEnumerable<T> OrderBy<T>(this IEnumerable<T> datasource, string propertyName, string direction)
        {
            if (direction == SortDirection.Ascending)
                return datasource.AsQueryable().OrderBy(propertyName); //OrderBy(propertyName, direction);
            else
                return datasource.AsQueryable().OrderByDescending(propertyName);
        }
        public static IQueryable<T> OrderBy<T>(this IQueryable<T> datasource, string propertyName, string direction)
        {

            if (string.IsNullOrEmpty(propertyName))
                return datasource;

            var type = typeof(T);
            var property = type.GetProperty(propertyName);

            Requires<InvalidOperationException>.IsNotNull(
                property,
                string.Format("Could not find a property called '{0}' on type {1}", propertyName, type));

            var parameter = Expression.Parameter(type, "p");
            var propertyAccess = Expression.MakeMemberAccess(parameter, property);
            var orderByExp = Expression.Lambda(propertyAccess, parameter);

            string methodToInvoke = direction.Equals(SortDirection.Ascending) ? OrderBy_ : OrderByDesc_;

            var orderByCall = Expression.Call(typeof(Queryable),
                methodToInvoke,
                new[] { type, property.PropertyType },
                datasource.Expression,
                Expression.Quote(orderByExp));

            return datasource.Provider.CreateQuery<T>(orderByCall);
        }
        #endregion

        #region Helpers
        public static string GetOrder(string propertyName, string currentDirection, string currentPropertyName)
        {
            if (propertyName.Equals(currentPropertyName))
                return (currentDirection.Equals(DefaultSortDirection)) ? SortDirection.Descending : DefaultSortDirection;

            return DefaultSortDirection;
        }
        public static IOrderedQueryable<T> OrderBy<T>(this IQueryable<T> source, string property)
        {
            return ApplyOrder<T>(source, property, OrderBy_);
        }
        public static IOrderedQueryable<T> OrderByDescending<T>(this IQueryable<T> source, string property)
        {
            return ApplyOrder<T>(source, property, OrderByDesc_);
        }
        public static IOrderedQueryable<T> ThenBy<T>(this IOrderedQueryable<T> source, string property)
        {
            return ApplyOrder<T>(source, property, "ThenBy");
        }
        public static IOrderedQueryable<T> ThenByDescending<T>(this IOrderedQueryable<T> source, string property)
        {
            return ApplyOrder<T>(source, property, "ThenByDescending");
        }
        public static IOrderedQueryable<T> ApplyOrder<T>(IQueryable<T> source, string property, string methodName)
        {
            string[] props = property.Split('.');
            Type type = typeof(T);
            ParameterExpression arg = Expression.Parameter(type, "x");
            Expression expr = arg;
            foreach (string prop in props)
            {
                // use reflection (not ComponentModel) to mirror LINQ
                PropertyInfo pi = type.GetProperty(prop);
                expr = Expression.Property(expr, pi);
                type = pi.PropertyType;
            }
            Type delegateType = typeof(Func<,>).MakeGenericType(typeof(T), type);
            LambdaExpression lambda = Expression.Lambda(delegateType, expr, arg);

            object result = typeof(Queryable).GetMethods().Single(
                    method => method.Name == methodName
                            && method.IsGenericMethodDefinition
                            && method.GetGenericArguments().Length == 2
                            && method.GetParameters().Length == 2)
                    .MakeGenericMethod(typeof(T), type)
                    .Invoke(null, new object[] { source, lambda });
            return (IOrderedQueryable<T>)result;
        }
        #endregion
    }
}
