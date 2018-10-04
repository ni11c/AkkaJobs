using Agridea.Web.Mvc.Grid.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Agridea.Web.Mvc.Grid.Columns
{
    public class GridCheckImageColumn<T> : GridBoundColumn<T, bool>
    {
        public GridCheckImageColumn(IGridModel<T> gridModel, string name, Func<T, bool> func)
            : base(gridModel, name, func) { }

        public ICommand<T> Command { get; set; }

        #region Overrides of GridBoundColumn<T,TValue>

        public override string GetContent(T dataItem)
        {
            return GetCheckImage(dataItem);
        }

        #endregion Overrides of GridBoundColumn<T,TValue>

        protected string GetCheckImage(T dataItem)
        {
            if (HideWhenFunc != null && HideWhenFunc(dataItem))
                return string.Empty;

            return Value(dataItem)
                ? Tag.Span.Class("grid-icon", GridClass.CheckImage).Title("Oui").ToString()
                : string.Empty;
        }
    }
}