using Agridea.Diagnostics.Contracts;
using Agridea.ObjectMapping;
using System;
using System.Collections.Generic;
using System.Data.Entity.SqlServer;
using System.Linq.Expressions;
using Agridea.DataRepository;

namespace Agridea.Web.Mvc
{
    public static class AutoCompleteExtensions
    {
        public static AutoCompleteEntity<TPoco, T> ToAutoCompleteEntity<TPoco, T>(this IEnumerable<T> results, int total) where T : class, IAutoComplete<TPoco>, new()
            where TPoco : class, IPocoBase
        {
            return new AutoCompleteEntity<TPoco, T>
            {
                Total = total,
                Results = results
            };
        }

        //public static Expression<Func<TViewModel, bool>> PredicateFor<TViewModel>(string query)
        //    where TViewModel : class, IAutoComplete, new()
        //{
        //    //TODO refactor this part
        //    var parameter = Expression.Parameter(typeof(TViewModel), "x");
        //    var viewModel = new TViewModel();
        //    Expression member = parameter;
        //    Func<Expression, Expression> converter = x => x.Type == typeof(string)
        //        ? x
        //        : Expression.Call(
        //            Expression.Call(
        //                null,
        //                typeof(SqlFunctions).GetMethod("StringConvert", new[] { typeof(double) }),
        //                Expression.Convert(x, typeof(double?))),
        //            typeof(string).GetMethod("Trim", new Type[0]));

        //    var bodyExpression = viewModel.BuildInnerExpression(
        //        member,
        //        converter
        //        );



        //    var stringContainsMethod = typeof(string).GetMethod("Contains");
        //    Expression searchExpression = Expression.Call(bodyExpression, stringContainsMethod, Expression.Constant(query));


        //    var additionalExpressions = viewModel.GetAdditionalSearchExpressions();
        //    foreach (var additionalExpression in additionalExpressions)
        //    {
        //        var additionalBody = additionalExpression.Body.RemoveUnary() as MemberExpression;
        //        Expression additionalMember = Expression.Property(member, additionalBody.Member.Name);
        //        searchExpression = Expression.OrElse(searchExpression, Expression.Call(converter(additionalMember), stringContainsMethod, Expression.Constant(query)));
        //    }

        //    return Expression.Lambda<Func<TViewModel, bool>>(searchExpression, new[] { parameter });

        //}

        //public static Expression BuildInnerExpression<TViewModel>(
        //    this TViewModel model,
        //    Expression member,
        //    Func<Expression, Expression> strFunc)
        //    where TViewModel : class, IAutoComplete
        //{
        //    Expression returnExpression = null;
        //    var stringConcatMethod = typeof(string).GetMethod("Concat", new[] { typeof(string), typeof(string) });
        //    foreach (var lambda in model.GetTextExpressions())
        //    {
        //        var body = lambda.Body.RemoveUnary();
        //        var memberExpression = body as MemberExpression;
        //        var strExpression = (memberExpression != null) ?
        //            (Expression)Expression.Property(member, memberExpression.Member.Name)
        //            : Expression.Constant((body as ConstantExpression).Value);

        //        Asserts<ArgumentException>.IsNotNull(strExpression);
        //        if (strExpression.Type != typeof(string))
        //            strExpression = strFunc(strExpression);

        //        strExpression = Expression.Condition(Expression.Equal(strExpression, Expression.Constant(null)), Expression.Constant(string.Empty, typeof(string)), strExpression);

        //        returnExpression = returnExpression == null
        //            ? strExpression
        //            : Expression.Call(stringConcatMethod, new[] { returnExpression, strExpression });
        //    }
        //    return returnExpression;
        //}

    }
}