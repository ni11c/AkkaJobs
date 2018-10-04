using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;
using Agridea.Diagnostics.Logging;
using Agridea.ObjectMapping;

namespace System.Linq
{
    public static class QueryableExtensions
    {
        #region Services
        public static IOrderedQueryable<T> OrderBy<T>(this IQueryable<T> source, string property)
        {
            return ApplyOrder(source, property, "OrderBy");
        }

        public static IOrderedQueryable<T> OrderByDescending<T>(this IQueryable<T> source, string property)
        {
            return ApplyOrder(source, property, "OrderByDescending");
        }

        public static IOrderedQueryable<T> ThenBy<T>(this IOrderedQueryable<T> source, string property)
        {
            return ApplyOrder(source, property, "ThenBy");
        }

        public static IOrderedQueryable<T> ThenByDescending<T>(this IOrderedQueryable<T> source, string property)
        {
            return ApplyOrder(source, property, "ThenByDescending");
        }

        public static ProjectionExpression<TSource> Project<TSource>(this IQueryable<TSource> source) where TSource : class
        {
            return new ProjectionExpression<TSource>(source);
        }

        public static string ToTraceString<T>(this IQueryable<T> expression)
        {
            ObjectQuery<T> objectQuery = expression as ObjectQuery<T>;
            if (objectQuery != null)
                return objectQuery.ToTraceString();
            return "";
        }
        #endregion

        #region Helpers
        private static IOrderedQueryable<T> ApplyOrder<T>(IQueryable<T> source, string property, string methodName)
        {
            var props = property.Split('.');
            var type = typeof (T);
            var arg = Expression.Parameter(type, "x");
            Expression expr = arg;
            foreach (var pi in props.Select(prop => type.GetProperty(prop)))
            {
                expr = Expression.Property(expr, pi);
                type = pi.PropertyType;
            }
            var delegateType = typeof (Func<,>).MakeGenericType(typeof (T), type);
            var lambda = Expression.Lambda(delegateType, expr, arg);

            var result = typeof (Queryable).GetMethods().Single(
                    method => method.Name == methodName
                            && method.IsGenericMethodDefinition
                            && method.GetGenericArguments().Length == 2
                            && method.GetParameters().Length == 2)
                                           .MakeGenericMethod(typeof(T), type)
                                           .Invoke(null, new object[]
                                           {
                                               source, lambda
                                           });
            return (IOrderedQueryable<T>)result;
        }
        #endregion
    }

    public class ProjectionExpression<TSource> where TSource : class
    {
        #region Constants
        private const string SourceParameterName = "src";
        private const char Dot = '.';
        #endregion

        #region Members
        private static readonly ConcurrentDictionary<string, Expression> expressionCache_ = new ConcurrentDictionary<string, Expression>();
        private readonly IQueryable<TSource> source_;
        #endregion

        #region Initialization
        public ProjectionExpression(IQueryable<TSource> source)
        {
            source_ = source;
        }
        #endregion

        #region Services
        /// <summary>
        /// Specifies the target type
        /// </summary>
        public IQueryable<TDest> To<TDest>(Mapping<TSource, TDest> mapping) where TDest : class
        {
            
            var key = GetCacheKey<TDest>();
            var queryExpression = expressionCache_.GetOrAdd(
                key,
                x =>
                {

                    var destinationProperties = typeof(TDest).GetProperties().Where(dest => dest.CanWrite && ((!dest.PropertyType.IsGenericType || dest.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))));
                    var parameterExpression = Expression.Parameter(typeof(TSource), SourceParameterName);
                    var bindings = new List<MemberAssignment>();

                    foreach (var destinationProperty in destinationProperties)
                    {
                        var propertyPath = EasyMapper.GetMap<TDest, TSource>(destinationProperty.Name);
                        if (propertyPath != null)
                        {
                            var member = propertyPath.propertyInfos_.Aggregate<PropertyInfo, Expression>(parameterExpression, Expression.Property);
                            var binding = Expression.Bind(destinationProperty, member);
                            //var binding = BuildBinding(parameterExpression, destinationProperty, sourceProperties);

                            if (binding.Expression.ToString().Split(Dot).Skip(1).Count() > 1)
                                binding = NullCheck(binding, parameterExpression, destinationProperty);
                            bindings.Add(binding);

                        }

                    }

                    var expression = Expression.Lambda<Func<TSource, TDest>>(Expression.MemberInit(Expression.New(typeof (TDest)), bindings), parameterExpression);
                    return expression;



                }) as Expression<Func<TSource, TDest>>;

            return source_.Select(queryExpression);
        }
        #endregion

        #region Helpers
        private static MemberAssignment NullCheck(MemberAssignment assignment, ParameterExpression parameterExpression, PropertyInfo destinationProperty)
        {
            var parameterExpressionType = parameterExpression.Type;
            Expression expression = parameterExpression;
            var properties = assignment.Expression.ToString().Split(Dot).Skip(1).ToList();
            Expression globalExpression = null;
            foreach (var prop in properties)
            {
                var property = parameterExpressionType.GetProperty(prop);
                expression = Expression.Property(expression, property);
                if (prop != properties.Last())
                {
                    Expression nullValue = Expression.Constant(null, property.PropertyType);
                    Expression notEqualExpression = Expression.NotEqual(expression, nullValue);
                    globalExpression = globalExpression == null
                        ? notEqualExpression
                        : Expression.AndAlso(globalExpression, notEqualExpression);
                }

                parameterExpressionType = property.PropertyType;
            }
            var def = destinationProperty.PropertyType.IsValueType
                ? Activator.CreateInstance(destinationProperty.PropertyType)
                : null;
            Expression defaultValue = Expression.Constant(def, destinationProperty.PropertyType);
            Expression result = Expression.Condition(globalExpression, expression, defaultValue);

            return Expression.Bind(destinationProperty, result);
        }
        
        private static string GetCacheKey<TDest>()
        {
            return string.Concat(typeof (TSource).FullName, typeof (TDest).FullName);
        }

        #endregion
    }
}
