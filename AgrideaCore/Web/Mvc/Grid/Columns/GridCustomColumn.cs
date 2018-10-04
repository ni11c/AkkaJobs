using Agridea.Web.Mvc.Grid.Extensions;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;

namespace Agridea.Web.Mvc.Grid.Columns
{
    public class GridCustomColumn<T> : GridColumnBase<T>
    {
        #region Initialization

        public GridCustomColumn(IGridModel<T> gridModel, Func<T, string> customContent)
            : base(gridModel, "")
        {
            Content = customContent;
        }

        #endregion Initialization

        public Func<T, string> Content { get; set; }

        public override string GetContent(T dataItem)
        {
            return Content(dataItem);
        }

        public override string GetWarning(T dataItem)
        {
            return "";
        }
    }
}