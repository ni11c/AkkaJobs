using Agridea.Web.Mvc.Grid.Command;

namespace Agridea.Web.Mvc.Grid.Fluent
{
    public class GridCreateActionCommandBuilder<T> : GridActionCommandBuilderBase<T, ICommand<T>, GridCreateActionCommandBuilder<T>>
    {
        public GridCreateActionCommandBuilder(ICommand<T> command) : base(command) {}
    }

    public class GridExcelActionCommandBuilder<T>:GridActionCommandBuilderBase<T, ICommand<T>, GridExcelActionCommandBuilder<T>>
    {
        public GridExcelActionCommandBuilder(ICommand<T> command) : base(command) {}
    }
}
