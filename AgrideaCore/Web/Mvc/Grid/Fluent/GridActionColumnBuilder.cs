using Agridea.Web.Mvc.Grid.Columns;

namespace Agridea.Web.Mvc.Grid.Fluent
{
    public class GridActionColumnBuilder<T> : GridColumnBuilderBase<IGridColumn, GridActionColumnBuilder<T>>
    {
        public GridActionColumnBuilder(IGridColumn column) : base(column) {}
    }
}
