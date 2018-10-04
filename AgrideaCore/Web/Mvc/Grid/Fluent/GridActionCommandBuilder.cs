using Agridea.Web.Mvc.Grid.Command;

namespace Agridea.Web.Mvc.Grid.Fluent
{
    public class GridActionCommandBuilder<T> : GridActionCommandBuilderBase<T, GridActionCommand<T>, GridActionCommandBuilder<T>>, IHideObjectMembers
    {
        public GridActionCommandBuilder(GridActionCommand<T> command) : base(command) {}

    }

}
