using Agridea.Diagnostics.Contracts;
using Agridea.Web.Mvc.Grid.Aggregates;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Agridea.Web.Mvc.Grid.Columns
{
    public class GridListColumn<T, TValue> : GridColumnBase<T>, IGridBoundColumn<T>
    {
        public GridListColumn(IGridModel<T> gridModel, string name, Func<T, IEnumerable<TValue>> func, string separator ) : base(gridModel, name)
        {
            IsSortable = false;
            Value = func;
            Separator = separator;
        }

        public string Format { get; set; }

        public bool IsSortable { get; set; }

        public Func<T, bool> HideWhenFunc { get; set; }

        public Func<T, bool> DisabledFunc { get; set; }

        public Expression<Func<T, object>> SortExpression { get; set; }

        public Func<T, bool> WarningPredicate { get; set; }

        public string WarningText { get; set; }

        public AggregateFunction AggregateFunction { get; set; }

        public object DefaultValue { get; set; }
        public Expression<Func<T, IEnumerable<TValue>>> Expression { get; private set; }
        public Func<T, IEnumerable<TValue>> Value { get; private set; }

        public string Separator { get; set; }

        public List<Func<T, string>> PredicateCssClasses { get; set; }

        public override string GetContent(T dataItem)
        {
            return (HideWhenFunc != null && HideWhenFunc(dataItem))
                       ? string.Empty
                       : GetValue(dataItem, Separator ?? Tag.Br);
        }

        public override string GetWarning(T dataItem)
        {
            if (WarningPredicate != null && WarningPredicate(dataItem))
                return Tag.Span.Class("grid-icon", GridClass.WarningIcon).Title(WarningText).ToString();

            return string.Empty;
        }

        public override string GetHeader()
        {
            return Title ?? base.GetHeader();
        }

        public override string GetExportContent(T dataItem)
        {
            return GetValue(dataItem, " / ");
        }

        private string GetValue(T dataItem, string separator)
        {
            var builder = new StringBuilder();
            var content = Value(dataItem);
            foreach (var element in content)
            {
                builder.Append(element + separator);
            }
            return builder.ToString();
        }
    }
}
