using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls.Expressions;
using System.Windows.Input;
using Agridea.Web.Mvc.Grid.Command;

namespace Agridea.Web.Mvc.Grid.Columns
{
    public class GridCheckEditColumn<T, TValue> : GridBoundColumn<T, TValue>
    {
        public GridCheckEditColumn(IGridModel<T> gridModel, ICommand<T> command, string name, Func<T, TValue> hasValueFunc)
            :
                base(gridModel, name, hasValueFunc)
        {
            Command = command;
        }

        public ICommand<T> Command { get; private set; }

        public override string GetContent(T dataItem)
        {
            if (HideWhenFunc != null && HideWhenFunc(dataItem))
                return string.Empty;
            return Command.Render(dataItem, GridModel.DataKeys, GridModel.Context);
        }
    }
}
