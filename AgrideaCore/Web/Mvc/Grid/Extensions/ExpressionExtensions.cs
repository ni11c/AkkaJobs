using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;
using Agridea.Diagnostics.Contracts;
using System.Collections.Generic;
using System.Reflection;

namespace Agridea.Web.Mvc.Grid.Extensions
{
    public static class ExpressionExtensions
    {

        private class ArrayVisitor : ExpressionVisitor
        {
            private readonly int index_;
            public ParameterExpression Parameter { get; set; }

            public ArrayVisitor(int index)
            {
                index_ = index;
            }

            protected override Expression VisitParameter(ParameterExpression node)
            {
                return Expression.ArrayIndex(GetArrayParameter(node), Expression.Constant(index_));
            }

            private ParameterExpression GetArrayParameter(ParameterExpression parameter)
            {
                var arrayType = parameter.Type.MakeArrayType();
                Parameter = Expression.Parameter(arrayType, parameter.Name);
                return Parameter;
            }
        }

        public static Expression<Func<T[], TValue>> BuildExpressionArrayFromExpression<T, TValue>(
            this Expression<Func<T, TValue>> expression,
            int index
            )
        {
            var visitor = new ArrayVisitor(index);
            var nexExpression = visitor.Visit(expression.Body);
            var parameter = visitor.Parameter;
            return Expression.Lambda<Func<T[], TValue>>(nexExpression, parameter);
        }

        public static Expression<Func<TViewModel, TValue>> BuildExpressionArrayFromExpression<T, TValue, TViewModel>(
            this Expression<Func<T, TValue>> expression,
            int index,
            string bindingPropertyName,
            TViewModel model
            )
        {
            var parameter = Expression.Parameter(model.GetType(), "m");
            Expression member = parameter;
            var viewModelProperty = model.GetType().GetProperty(bindingPropertyName);

            MemberExpression memberExpr = Expression.Property(member, viewModelProperty);
            var indexProperty = typeof (IList<T>).GetProperty("Item");
            var indexExpr = Expression.MakeIndex(memberExpr, indexProperty, new Expression[] {Expression.Constant(index)});
            var contents = ExpressionHelper.GetExpressionText(expression).Split('.');
            var listType = typeof (T);
            foreach (var content in contents)
            {
                var proeprty = listType.GetProperty(content);
                member = Expression.Property(member, proeprty);
                listType = proeprty.PropertyType;
            }
            return Expression.Lambda<Func<TViewModel, TValue>>(member, parameter);
        }
    }

    public class methodof<T>
    {
        private MethodInfo method;

        public methodof(T func)
        {
            Delegate del = (Delegate) (object) func;
            this.method = del.Method;
        }

        public static implicit operator methodof<T>(T methodof)
        {
            return new methodof<T>(methodof);
        }

        public static implicit operator MethodInfo(methodof<T> methodof)
        {
            return methodof.method;
        }
    }
}
