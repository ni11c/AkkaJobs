using Agridea.Web.Mvc.Grid.Aggregates;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Agridea.Web.Mvc.Grid.Columns
{
    public interface IGridBoundColumn<T> : IGridColumn
    {
        string Format { get; set; }

        bool IsSortable { get; set; }

        Func<T, bool> HideWhenFunc { get; set; }

        Func<T, bool> DisabledFunc { get; set; }

        Expression<Func<T, object>> SortExpression { get; set; }

        Func<T, bool> WarningPredicate { get; set; }

        string WarningText { get; set; }

        AggregateFunction AggregateFunction { get; set; }

        string GetContent(T dataItem);

        object DefaultValue { get; set; }

        List<Func<T, string>> PredicateCssClasses { get; set; }

    }
}