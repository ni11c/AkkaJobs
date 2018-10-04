using Agridea.Web.Mvc.Grid.Columns;
using System;

namespace Agridea.Web.Mvc.Grid.Fluent
{
    public class GridEditColumnBuilder<T> : GridExpressionColumnBuilder<T, IGridEditColumn<T>, GridEditColumnBuilder<T>>
    {
        public GridEditColumnBuilder(IGridEditColumn<T> column)
            : base(column)
        {
        }

        public GridEditColumnBuilder<T> IsDisabled(Func<T, bool> predicate)
        {
            Column.DisabledFunc = predicate;
            return this;
        }

        public GridEditColumnBuilder<T> IsDisabled(bool value)
        {
            Column.IsDisabled = value;
            return this;
        }

        public GridEditColumnBuilder<T> IsReadOnly(Func<T, bool> predicate)
        {
            Column.ReadOnlyFunc = predicate;
            return this;
        }

        public GridEditColumnBuilder<T> IsReadOnly(bool value)
        {
            Column.IsReadOnly = value;
            return this;
        }

        public GridEditColumnBuilder<T> StyleWidth(int value)
        {
            Column.StyleWidth = value;
            return this;
        }
        public GridEditColumnBuilder<T> PlaceHolder(string value)
        {
            Column.PlaceHolder = value;
            return this;
        }
    }
}