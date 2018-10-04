using System.IO;
using Agridea.Web.Mvc.Grid.Columns;
using Agridea.Web.Mvc.Grid.ToolBar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Helpers;

namespace Agridea.Web.Mvc.Grid
{
    public class GridModel<T> : IGridModel<T>
    {
        #region Members

        private readonly IQueryable<T> source_;
        private ViewContext viewContext_;
        private TempDataDictionary tempData_;
        private IList<T> paginatedItems_;
        private HtmlHelper<T[]> arrayHelper_;
        #endregion

        #region Initialization

        public GridModel(IQueryable<T> source, ViewDataDictionary viewData, ViewContext viewContext, ControllerContext controllerContext, TempDataDictionary tempData = null)
        {
            source_ = source;
            tempData_ = tempData;
            Context = controllerContext;
            viewContext_ = viewContext;
            ViewData = viewData;
            GridType = typeof (T);
            Ordering = new Ordering(Context.HttpContext.Request.QueryString[GridParameters.OrderKey], GridType);
            Pagination = new Pagination<T>(source_, this);

            ColumnList = new List<GridColumnBase<T>>();
            HiddenColumns = new List<GridColumnBase<T>>();
            DataKeys = new List<IGridDataKey<T>>();

            Width = GridParameters.GridDefaultWidth;
            ToolBar = new GridToolBar<T>(this);
            ViewModelType = viewData.Model != null ? viewData.Model.GetType() : null;
            FormAction = Context.HttpContext.Request.RawUrl;
            HasModelStateErrors = viewData.HasModelStateErrors();
            HasForm = true;
        }


        #endregion

        #region Properties

        public string Prefix { get; set; }
        public bool HasForm { get; set; }
        public string FormAction { get;  set; }
        public HtmlHelper Helper { get;  set; }
        public IOrdering Ordering { get; private set; }
        public IPagination Pagination { get; private set; }
        public Type GridType { get; private set; }
        public Type ViewModelType { get; private set; }
        public ControllerContext Context { get; private set; }
        public ViewDataDictionary ViewData { get; private set; }
        public bool IsOrdered { get; set; }
        public int TotalItems
        {
            get { return Pagination.TotalItems; }
        }

        public IList<T> PaginatedItems { get { return paginatedItems_ ?? (paginatedItems_ = GetPaginatedItems()); } }
        public IList<GridColumnBase<T>> ColumnList { get; private set; }
        public IList<GridColumnBase<T>> HiddenColumns { get; private set; }
        public IList<IGridDataKey<T>> DataKeys { get; private set; }
        public GridToolBar<T> ToolBar { get; private set; }
        public string BindingListName { get; private set; }
        public string Width { get; set; }
        public string SaveButtonValue { get; set; }

        public HtmlHelper<T[]> ArrayHelper
        {
            get { return arrayHelper_ ?? (arrayHelper_ = GetArrayHelper()); }
        }

        public bool HasModelStateErrors { get; private set; }

        public int DisplayColumnsCount
        {
            get { return DisplayColumns.Count(); }
        }

        public IEnumerable<GridColumnBase<T>> VisibleColumns
        {
            get { return ColumnList.Where(m => m.IsVisible); }
        }

        public IEnumerable<IGridColumn> DisplayColumns
        {
            get { return VisibleColumns.Where(m => !m.IsMerged); }
        }

        public IEnumerable<IGridColumn> DisplayColumnList
        {
            get { return VisibleColumns.Where(m => !m.IsMerged); }
        }

        public bool IsEditable()
        {
            return HasForm && ColumnList.OfType<IGridEditColumn<T>>().Any();
        }

        public bool HasCustomHeader()
        {
            return VisibleColumns.Any(m => m.CustomHeader != null);
        }

        public void SetWidth(int width, MeasuringUnit unit)
        {
            Width = string.Format("{0}{1}", width, unit == MeasuringUnit.Pixel ? "px" : "%");
        }

        internal void SetBindingListName(string bindingListName)
        {
            BindingListName = bindingListName;
            
        }

        #endregion

        #region Helpers

        private IList<T> GetPaginatedItems()
        {
            var ordered = GetOrderedItems();

            if (Pagination.IsDisplayed)
                ordered = ordered
                    .Skip(Pagination.SkipCount)
                    .Take(Pagination.PageSize)
                    ;
            return ordered.ToList();
        }

        public IList<T> GetAllItems()
        {
            return GetOrderedItems().ToList();
        }

        private IQueryable<T> GetOrderedItems()
        {
            var ordered = source_;
            

            if (!Ordering.Orders.Any())
            {
                if (IsOrdered)
                    return ordered;
                string orderProperty = GridHelper.GetDefaultOrderProperty<T>();
                ordered = ordered.OrderBy(orderProperty);
            }

            foreach (var order in Ordering.Orders)
            {
                if (order == Ordering.Orders.First())
                    ordered = order.Direction == GridParameters.Ascending
                                  ? ordered.OrderBy(order.PropertyName)
                                  : ordered.OrderByDescending(order.PropertyName);
                else
                    ordered = order.Direction == GridParameters.Ascending
                                  ? ((IOrderedQueryable<T>) ordered).ThenBy(order.PropertyName)
                                  : ((IOrderedQueryable<T>) ordered).ThenByDescending(order.PropertyName);
            }

            return ordered;
        }

        private HtmlHelper<T[]> GetArrayHelper()
        {
            

            if (viewContext_ == null)
            {
                viewContext_ = new ViewContext(Context, new FakeView(), ViewData,  tempData_, TextWriter.Null);
            }
            var helper = new HtmlHelper<T[]>(viewContext_, new GridViewDataContainer<T[]>(PaginatedItems.ToArray(), viewContext_.ViewData));
            return helper;
        }


        public class FakeView : IView
        {
            public void Render(ViewContext viewContext, TextWriter writer)
            {
                throw new InvalidOperationException();
            }
        }

        #endregion
    }
}
