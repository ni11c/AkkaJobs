using Agridea.Web.Mvc.Grid.Command;
using System.Collections.Generic;
using System.Text;

namespace Agridea.Web.Mvc.Grid.Columns
{
    public class GridActionColumn<T> : GridColumnBase<T>
    {
        #region Initialization

        public GridActionColumn(GridModel<T> gridModel)
            : base(gridModel, "")
        {
            Commands = new List<ICommand<T>>();
            IsVisibleForExport = false;
        }

        #endregion Initialization

        #region Services

        public IList<ICommand<T>> Commands { get; private set; }

        public override string GetContent(T dataItem)
        {
            var sb = new StringBuilder();
            foreach (var command in Commands)
            {
                sb.Append(command.Render(dataItem, GridModel.DataKeys, GridModel.Context));
            }
            return sb.ToString();
        }

        public override string GetWarning(T dataItem)
        {
            return string.Empty;
        }

        public override string GetHeader()
        {
            return Title ?? string.Empty;
        }

        public override FluentTagBuilder GetContentTag(T dataItem)
        {
            return base.GetContentTag(dataItem).Style("white-space:nowrap;");
        }

        #endregion Services
    }
}