using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Agridea.Resources;

namespace Agridea.Web.Mvc.Grid.Renderers
{
    public class Pager : IHtmlString
    {
        #region Members
        private readonly IPagination pagination_;
        private readonly ControllerContext viewContext_;

        private static readonly string PaginationFormat = AgrideaCoreStrings.GridPaginationFormat + " ";
        private static readonly string PaginationSingleFormat = AgrideaCoreStrings.GridPaginationFormat + " ";
        private const int PagerLength = GridParameters.PagerLength;
        private readonly FluentTagBuilder paginationFirst_ = Tag.Span.Class(GridClass.GridArrow, GridClass.IconSeekFirst);
        private readonly FluentTagBuilder paginationPrev_ = Tag.Span.Class(GridClass.GridArrow, GridClass.IconSeekPrevious);
        private readonly FluentTagBuilder paginationNext_ = Tag.Span.Class(GridClass.GridArrow, GridClass.IconSeekNext);
        private readonly FluentTagBuilder paginationLast_ = Tag.Span.Class(GridClass.GridArrow, GridClass.IconSeekLast);
        private const string PageQueryName = GridParameters.PageKey;
        private readonly Func<int, string> urlBuilder_;
        #endregion

        #region Initialization
        /// <summary>
        /// Creates a new instance of the Pager class.
        /// </summary>
        /// <param name="pagination">The IPagination datasource</param>
        /// <param name="context">The view context</param>
        public Pager(IPagination pagination, ControllerContext context)
        {
            pagination_ = pagination;
            viewContext_ = context;
         
            urlBuilder_ = CreateDefaultUrl;
        }
        #endregion


        #region Services
        // For backwards compatibility with WebFormViewEngine
        public override string ToString()
        {
            return ToHtmlString();
        }
        public string ToHtmlString()
        {
            var pagerDiv = Tag.Div;

            if (pagination_.TotalItems == 0)
                return pagerDiv.Html(Tag.P.SetInnerText(GridMessages.NoResult)).ToString();

            if (pagination_.TotalPages > 1)
            {
                pagerDiv.Html(RenderLeftSideOfPager());
            }

            //pageSize selector
            pagerDiv.Html(RenderNumberOfPageSelector());

            pagerDiv.Html(RenderRightSideOfPager());


            return pagerDiv.ToString();
        }
        #endregion

        #region Helpers
        protected ControllerContext ViewContext
        {
            get { return viewContext_; }
        }

        protected virtual FluentTagBuilder RenderRightSideOfPager()
        {
            var div = Tag.Div.Class(GridClass.StatusText);

            //Special case handling where the page only contains 1 item (ie it's a details-view rather than a grid)
            div.Html(pagination_.PageSize == 1
                ? RenderNumberOfItemsWhenThereIsOnlyOneItemPerPage()
                : RenderNumberOfItemsWhenThereAreMultipleItemsPerPage());
            return div;
        }
        protected virtual FluentTagBuilder RenderLeftSideOfPager()
        {
            var ul = Tag.Ul;

            //If we're on page 1 then there's no need to render a link to the first page.
            ul.Html(Tag.Li.Html(GetPrevNext(1, paginationFirst_, pagination_.PageNumber > 1, GridMessages.FirstPage)));

            //If we're on page 2 or later, then render a link to the previous page. 
            //If we're on the first page, then there is no need to render a link to the previous page. 
            ul.Html(Tag.Li.Html(GetPrevNext(pagination_.PageNumber - 1, paginationPrev_, pagination_.HasPreviousPage, GridMessages.PreviousPage)));


            if (PaginationStart > 1)
                ul.Html(Tag.Li.Html(Tag.A(urlBuilder_(PreviousPagination)).SetInnerText("..").Title(GridMessages.PreviousPagerElements)));

            for (var i = PaginationStart; i <= MaxPagination; i++)
                ul.Html(Tag.Li.Html(GetNumeratedLink(i)));

            if (MaxPagination < pagination_.TotalPages)
                ul.Html(Tag.Li.Html(Tag.A(urlBuilder_(NextPagination)).SetInnerText("..").Title(GridMessages.NextPagerElements)));



            //Only render a link to the next page if there is another page after the current page.
            ul.Html(Tag.Li.Html(GetPrevNext(pagination_.PageNumber + 1, paginationNext_, pagination_.HasNextPage, GridMessages.NextPage)));


            var lastPage = pagination_.TotalPages;

            //Only render a link to the last page if we're not on the last page already.
            ul.Html(Tag.Li.Html(GetPrevNext(lastPage, paginationLast_, pagination_.PageNumber < lastPage, GridMessages.LastPage)));

            return ul;
        }
        protected FluentTagBuilder RenderNumberOfPageSelector()
        {
            var selectedPageSize = pagination_.PageSize;
            var pageSizes = new[] { 5, 10, 20, 50 };
            var combo = Tag.Select.Id("pageSizeSelector").Class("no-dirty");
            foreach (var pageSize in pageSizes)
            {
                combo.Html(Tag.Option(pageSize.ToString(), pageSize.ToString(), selectedPageSize == pageSize));
            }
            return combo;
        }
        protected virtual string RenderNumberOfItemsWhenThereIsOnlyOneItemPerPage()
        {
            return string.Format(PaginationSingleFormat, pagination_.FirstItem, pagination_.TotalItems);
        }
        protected virtual string RenderNumberOfItemsWhenThereAreMultipleItemsPerPage()
        {
            return string.Format(PaginationFormat, pagination_.FirstItem, pagination_.LastItem, pagination_.TotalItems);
        }

        private FluentTagBuilder GetPrevNext(int pageNumber, FluentTagBuilder span, bool activateLink, string title)
        {
            return activateLink
                       ? Tag.A(urlBuilder_(pageNumber)).Title(title).Html(span)
                       : span.AddCssClass(GridClass.GridDisabled);
        }
        private FluentTagBuilder GetNumeratedLink(int pageNumber)
        {
            return pageNumber == pagination_.PageNumber
                ? Tag.Span.Class(GridClass.ActivePage).SetInnerText(pageNumber.ToString())
                : Tag.A(urlBuilder_(pageNumber)).Html(pageNumber.ToString());
        }

        private string CreateDefaultUrl(int pageNumber)
        {
            var routeValues = new RouteValueDictionary();

            foreach (var key in viewContext_.RequestContext.HttpContext.Request.QueryString.AllKeys.Where(key => key != null))
            {
                routeValues[key] = viewContext_.RequestContext.HttpContext.Request.QueryString[key];
            }

            routeValues[PageQueryName] = pageNumber;

            var url = UrlHelper.GenerateUrl(null, null, null, routeValues, RouteTable.Routes, viewContext_.RequestContext, true);
            return url;
        }
        private int PaginationStart
        {
            get { return ((((pagination_.PageNumber - 1) / PagerLength) * PagerLength) + 1); }
        }
        private int PreviousPagination
        {
            get { return PaginationStart - PagerLength; }
        }
        private int NextPagination
        {
            get { return (PaginationStart + PagerLength); }
        }
        private int MaxPagination
        {
            get { return Math.Min((PaginationStart + PagerLength) - 1, pagination_.TotalPages); }
        }
        #endregion
    }
}
