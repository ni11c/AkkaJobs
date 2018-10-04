using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace Agridea.Web.Mvc.Grid
{
    public class Pagination<T> : IPagination<T>
    {
        #region Members
        private readonly NameValueCollection queryString_;
        private int pageSize_;
        #endregion

        #region Initialization
        /// <summary>
        /// Creates a new instance of the <see cref="Pagination{T}"/> class.
        /// </summary>
        /// <param name="query">The query to page.</param>
        /// <param name="model">The grid model</param>
        public Pagination(IQueryable<T> query, IGridModel model)
        {
            queryString_ = model.Context.HttpContext.Request.QueryString;
            TotalItems = query.Count();
            PageNumber = ParsePageNumber();
            IsDisplayed = true;

        }
        public Pagination(IEnumerable<T> query, IGridModel model)
        {
            queryString_ = model.Context.HttpContext.Request.QueryString;
            TotalItems = query.Count();
            PageNumber = ParsePageNumber();
            IsDisplayed = true;
        }
        #endregion

        #region Properties
        public int PageSize
        {
            get
            {
                return ParsePageSize() == 0
                           ? pageSize_ == 0 ? GridParameters.DefaultPageSize : pageSize_
                           : ParsePageSize();
            } 
            set { pageSize_ = value; }
        }

        public int PageNumber { get; private set; }
        public int TotalItems { get; private set; }

        public int TotalPages
        {
            get { return (int)Math.Ceiling(((double)TotalItems) / PageSize); }
        }

        public int FirstItem
        {
            get
            {
                return ((PageNumber - 1) * PageSize) + 1;
            }
        }
        public int SkipCount
        {
            get { return Math.Max(0, (PageNumber - 1) * PageSize); }
        }

        public int LastItem
        {
            get
            {
                return Math.Min(FirstItem + PageSize - 1, TotalItems);
            }
        }

        public bool HasPreviousPage
        {
            get { return PageNumber > 1; }
        }

        public bool HasNextPage
        {
            get { return PageNumber < TotalPages; }
        }

        public bool IsDisplayed { get; set; }
        #endregion

        #region Helpers
        private int ParsePageSize()
        {
            int pageSize;
            var pageSizeQuery = queryString_[GridParameters.PageSizeKey];
            return string.IsNullOrEmpty(pageSizeQuery) || !Int32.TryParse(pageSizeQuery, out pageSize)
                           ? 0
                           : pageSize;
        }
        private int ParsePageNumber()
        {
            const int defaultPageNumber = GridParameters.DefaultPage;
            int pageNumber;
            var pageNumberQuery = queryString_[GridParameters.PageKey];
            var pageFromQuery = string.IsNullOrEmpty(pageNumberQuery) || !Int32.TryParse(pageNumberQuery, out pageNumber)
                                    ? defaultPageNumber
                                    : Math.Max(pageNumber, defaultPageNumber);
            return Math.Min(pageFromQuery, TotalPages);
        }

        #endregion
    }
}
