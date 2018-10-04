using System.Collections.Generic;
using System.Linq;
using System.Text;
using Agridea.Web.Mvc.Grid.Command;

namespace Agridea.Web.Mvc.Grid.ToolBar
{
    public class GridToolBar<T>
    {
        #region Members
        private readonly GridModel<T> model_;
        #endregion

        #region Initialization
        public GridToolBar(GridModel<T> model)
        {
            model_ = model;
            Commands = new List<ICommand<T>>();
        }
        #endregion

        #region Services
        public IList<ICommand<T>> Commands { get; private set; }
        public bool Enabled { get { return Commands.Any(); } }
        public string Render()
        {
            var sb = new StringBuilder();
            foreach (var command in Commands)
            {
                sb.Append(command.Render(default(T), model_.DataKeys, model_.Context));
            }
            return sb.ToString();
        }
        #endregion
    }
}
