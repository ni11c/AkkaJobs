using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Agridea.Web.Mvc.Grid.Columns;

namespace Agridea.Web.Mvc.Grid
{
    public interface IGridModel<T> : IGridModel
    {
        IList<T> PaginatedItems { get; }
        IList<IGridDataKey<T>> DataKeys { get; }
        HtmlHelper<T[]> ArrayHelper { get; }
        IEnumerable<GridColumnBase<T>> VisibleColumns { get; }
        IList<GridColumnBase<T>> HiddenColumns { get; }
        IList<T> GetAllItems();
    }

    public interface IGridModel
    {
        int TotalItems { get; }
        ControllerContext Context { get; }
        Type GridType { get; }
        IPagination Pagination { get; }
        IEnumerable<IGridColumn> DisplayColumns { get; }
        IOrdering Ordering { get; }
        ViewDataDictionary ViewData { get; }
        HtmlHelper Helper { get; }
        string BindingListName { get;}
        Type ViewModelType { get; }
        bool HasModelStateErrors { get;}
    }
}
