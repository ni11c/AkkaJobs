using Agridea.Diagnostics.Contracts;
using Agridea.Web.Mvc.Grid.Aggregates;
using Agridea.Web.Mvc.Grid.Columns;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace Agridea.Web.Mvc.Grid.Fluent
{
    public abstract class GridExpressionColumnBuilder<T, TColumn, TColumnBuilder> : GridColumnBuilderBase<TColumn, TColumnBuilder>
        where TColumnBuilder : GridExpressionColumnBuilder<T, TColumn, TColumnBuilder>
        where TColumn : IGridBoundColumn<T>
    {
        protected GridExpressionColumnBuilder(TColumn column)
            : base(column)
        {
        }

        public TColumnBuilder Format(string value)
        {
            Column.Format = value;
            return this as TColumnBuilder;
        }

        public TColumnBuilder Sortable(bool value)
        {
            Column.IsSortable = value;
            return this as TColumnBuilder;
        }

        public TColumnBuilder SortUsing(Expression<Func<T, object>> sortExpression)
        {
            Column.SortExpression = sortExpression;
            return this as TColumnBuilder;
        }

        public TColumnBuilder DisplaySum()
        {
            Column.AggregateFunction = new SumFunction();
            Column.NeedsFooter = true;
            return this as TColumnBuilder;
        }

        public TColumnBuilder HideWhen(Func<T, bool> predicate)
        {
            Column.HideWhenFunc = predicate;
            return this as TColumnBuilder;
        }

        public TColumnBuilder HideWhen(Func<T, bool> predicate, object valueWhenHidden)
        {
            Column.HideWhenFunc = predicate;
            Column.DefaultValue = valueWhenHidden;
            return this as TColumnBuilder;
        }

        public TColumnBuilder WarningFor(Func<T, bool> predicate, string warningText)
        {
            Column.WarningPredicate = predicate;
            Column.WarningText = warningText;
            return this as TColumnBuilder;
        }

        public TColumnBuilder CssClass(params Func<T, string>[] predicates)
        {
            Column.PredicateCssClasses.AddRange(predicates);
            return this as TColumnBuilder;
        }
    }
}