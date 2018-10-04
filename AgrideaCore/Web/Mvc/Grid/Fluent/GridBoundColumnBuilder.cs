using Agridea.Web.Mvc.Grid.Columns;

namespace Agridea.Web.Mvc.Grid.Fluent
{
    public class GridBoundColumnBuilder<T> : GridExpressionColumnBuilder<T, IGridBoundColumn<T>, GridBoundColumnBuilder<T>>
    {
        public GridBoundColumnBuilder(IGridBoundColumn<T> column)
            : base(column) { }
    }
}