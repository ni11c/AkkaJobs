using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Agridea.Web.Mvc.Grid.Command;

namespace Agridea.Web.Mvc.Grid.Columns
{
    public class GridActionLinkColumn<T, TValue> : GridBoundColumn<T, TValue>
    {
        public GridActionLinkColumn(IGridModel<T> gridModel, string name, Func<T, TValue> func)
            : base(gridModel, name, func) {}

        public ICommand<T> Command { get; set; }

        #region Overrides of GridBoundColumn<T,TValue>
        public override string GetContent(T dataItem)
        {
            Command.AlternateText = base.GetContent(dataItem);
            return Command.Render(dataItem, GridModel.DataKeys, GridModel.Context);
        }
        #endregion
    }
}
