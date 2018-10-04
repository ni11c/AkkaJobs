using Agridea.Diagnostics.Contracts;
using Agridea.Resources;
using Agridea.Web.Mvc.Grid.Columns;
using Agridea.Web.Mvc.Grid.Fluent;
using Agridea.Web.Mvc.Grid.Renderers;
using System;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Agridea.Web.Mvc.Grid
{
    public class Grid<T> : IHtmlString
    {
        #region Members
        public GridModel<T> GridModel { get; private set; }
        #endregion

        #region Initialization
        public Grid(IQueryable<T> source, ViewDataDictionary viewData, ControllerContext controllerContext, TempDataDictionary tempData)
        {
            GridModel =new GridModel<T>(source, viewData, null, controllerContext, tempData);
            var viewContext = new ViewContext(controllerContext, new GridModel<T>.FakeView(), viewData, tempData, TextWriter.Null);
            GridModel.Helper = new HtmlHelper(viewContext, new GridViewDataContainer(viewData));
        }
        public Grid(HtmlHelper helper, IQueryable<T> source)
        {
            
            GridModel = new GridModel<T>(source, helper.ViewData, helper.ViewContext, helper.ViewContext);
            GridModel.Helper = helper;
            Asserts<ArgumentOutOfRangeException>.IsNotNull(source, string.Format("DataSource must be an IQueryable<{0}>", typeof(T)));
        }
        public Grid(HtmlHelper helper, IQueryable<T> source, string bindingListName)
            : this(helper, source)
        {
            if (!string.IsNullOrWhiteSpace(bindingListName))
                
                GridModel.SetBindingListName(bindingListName);
        }
        #endregion

        #region Services
        public Grid<T> Columns(Action<GridColumnFactory<T>> factoryBuilder)
        {
            var factory = new GridColumnFactory<T>(GridModel);
            factoryBuilder(factory);
            return this;
        }
        public Grid<T> DataKeys(Action<GridDataKeyFactory<T>> configurator)
        {
            configurator(new GridDataKeyFactory<T>(GridModel.DataKeys));
            return this;
        }
        public Grid<T> TableWidth(int width, MeasuringUnit unit)
        {
            GridModel.SetWidth(width, unit);
            return this;
        }
        public Grid<T> TableWidth(int width)
        {
            GridModel.SetWidth(width, MeasuringUnit.Percentage);
            return this;
        }
        public Grid<T> ToolBar(Action<GridToolBarCommandFactory<T>> toolBarAction)
        {

            toolBarAction(new GridToolBarCommandFactory<T>(GridModel.ToolBar, GridModel.Context));
            return this;
        }

        public Grid<T> DefaultPageSize(PageSizes pageSizes)
        {
            GridModel.Pagination.PageSize = (int) pageSizes;
            return this;
        }

        public Grid<T> DefaultOrdering(Action<GridSortFactory<T>> defaultOrderingAction)
        {
            defaultOrderingAction(new GridSortFactory<T>(GridModel.Ordering));
            return this;
        }

        public Grid<T> ReadOnlyRow(Func<T, bool> predicate)
        {
            foreach (var column in GridModel.VisibleColumns.OfType<IGridEditColumn<T>>())
            {
                column.DisabledFunc = predicate;
            }
            return this;
        }
        public Grid<T> EnablePaging(bool value = true)
        {
            GridModel.Pagination.IsDisplayed = value;
            return this;
        }
        public Grid<T> EnableSorting(bool value = true)
        {
            foreach (var column in GridModel.ColumnList.OfType<IGridBoundColumn<T>>())
            {
                column.IsSortable = value;
            }
            return this;
        }

        public Grid<T> DisableRowWhen(Func<T, bool> predicate)
        {
            foreach (var column in GridModel.VisibleColumns.OfType<IGridBoundColumn<T>>().Where(m => !m.CssClasses.Contains(GridClass.HasColor)))
            {
                column.DisabledFunc = predicate;
            }
            return this;
        }

        /// <summary>
        /// Force keeping source order.
        /// </summary>
        /// <returns></returns>
        public Grid<T> IsOrdered()
        {
            GridModel.IsOrdered = true;
            return this;
        }
        public Grid<T> EnableForm(bool value = true)
        {
            GridModel.HasForm = value;
            return this;
        }
        public Grid<T> SetFormAction(string url)
        {
            GridModel.FormAction = url;
            return this;
        }

        public Grid<T> SetSaveButtonValue(string value)
        {
            GridModel.SaveButtonValue = value;
            return this;
        }

        public string ToCsv()
        {
            var renderer = new CsvRenderer<T>(GridModel);
            return renderer.ToString();
        }

        public bool HasValues()
        {
            return GridModel.PaginatedItems.Any();
        }

        public string ToHtmlString()
        {
            var renderer = new Renderer<T>(GridModel);
            var table = Tag.Table.Class(GridClass.Grid, GridClass.RealGrid).Style("width:" + GridModel.Width);//.MergeAttribute("cellspacing", 0).MergeAttribute("cellpadding", 0).MergeAttribute("border", 0);


            var colGroup = Tag.ColGroup;
            foreach (var column in GridModel.VisibleColumns.Where(m => !m.IsMerged))
            {
                var col = Tag.Col;
                if (column.Width > 0)
                    col.Style(string.Format("width:{0}%", column.Width));
                colGroup.Html(col);
            }
            table.Html(colGroup);
            var thead = Tag.Thead;
            renderer.RenderToolBar(thead);

            thead.Html(renderer.RenderHeader());
            table.Html(thead);

            var tbody = Tag.Tbody;
            renderer.RenderBody(tbody, GridModel.PaginatedItems);

            if (GridModel.VisibleColumns.Any(m => m.NeedsFooter))
            {
                var footer = Tag.Tr;
                foreach (var column in GridModel.DisplayColumns)
                    footer.Html(column.RenderFooter());
                tbody.Html(footer);
            }
            table.Html(tbody);
            if (GridModel.IsEditable() && GridModel.Pagination.TotalItems > 0)
                table.Html(
                    Tag.Tr.Html(
                        Tag.Td.Colspan(GridModel.DisplayColumnsCount)
                            .Html(Tag.Input("submit", GridModel.SaveButtonValue ?? AgrideaCoreStrings.WebSaveButtonCaption))));

            if (GridModel.Pagination.IsDisplayed)
            {
                var paginationTr = Tag.Tr.Html(
                    Tag.Td.Class(GridClass.Pager).Colspan(GridModel.DisplayColumnsCount)
                       .Html(renderer.RenderPagination()));
                table.Html(paginationTr);
            }
            else
            {
                if (GridModel.Pagination.TotalItems == 0)
                {
                    var noResultTr = Tag.Tr.Html(Tag.Td.Colspan(GridModel.DisplayColumnsCount)
                       .Html(GridMessages.NoResult));
                    table.Html(noResultTr);
                }
            }

            return (GridModel.IsEditable()
                ? Tag.Form.MergeAttribute("method", "post").MergeAttribute("action", GridModel.FormAction).Html(table)
                : table)
                .ToString();
        }
        public override string ToString()
        {
            return ToHtmlString();
        }
        #endregion
    }
}
