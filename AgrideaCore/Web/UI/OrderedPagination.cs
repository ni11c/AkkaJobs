using Agridea.Web.Mvc.Grid;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Agridea.Web.UI
{
    public class OrderedPagination<T>
    {
        #region Members
        private readonly IQueryable<T> source_;
        private readonly IOrderedQueryable<T> orderedSource_;
        #endregion

        #region Initialization
        public OrderedPagination(IOrderedQueryable<T> source, int pageNumber, int pageSize)
        {
            orderedSource_ = source;
            TotalItems = source.Count();
            PageNumber = pageNumber;
            PageSize = pageSize;

        }
        public OrderedPagination(IQueryable<T> source, int pageNumber, int pageSize, IDictionary<string, string> orderProperty)
        {
            source_ = source;
            TotalItems = source.Count();
            PageNumber = pageNumber;
            PageSize = pageSize;
            OrderProperty = orderProperty;
        }
        #endregion

        #region Properties
        public int PageNumber { get; private set; }
        public int PageSize { get; private set; }
        public int TotalItems { get; private set; }
        public IDictionary<string, string> OrderProperty { get; private set; }

        public int TotalPages
        {
            get { return Convert.ToInt32(Math.Ceiling(((double)TotalItems) / PageSize)); }
        }
        public int FirstItem
        {
            get { return ((PageNumber - 1) * PageSize) + 1; }
        }
        public int LastItem
        {
            get { return FirstItem + TotalItems - 1; }
        }
        public bool HasPreviousPage
        {
            get { return PageNumber > 1; }
        }
        public bool HasNextPage
        {
            get { return PageNumber < TotalPages; }
        }
        private int SkipCount
        {
            get { return (Math.Max(0, PageNumber - 1)) * PageSize; }
        }
        public IList<T> GetOrderedPaginatedList()
        {
            return PaginateAndSort().ToList();
        }
        public IList<T> GetPaginatedList()
        {
            return orderedSource_.Skip(SkipCount).Take(PageSize).ToList();
        }

        public IQueryable<T> PaginateAndSort()
        {
            var ordered = source_;
            if (!OrderProperty.Any())
                ordered = ordered.OrderBy(GridParameters.DefaultPropertyName);

            foreach (var order in OrderProperty)
            {
                if (order.Key == OrderProperty.First().Key)
                    ordered = order.Value == "A" ? ordered.OrderBy(order.Key) : ordered.OrderByDescending(order.Key);
                else
                    ordered = order.Value == "A" ? ((IOrderedQueryable<T>)ordered).ThenBy(order.Key) : ((IOrderedQueryable<T>)ordered).ThenByDescending(order.Key);
            }

            return ordered
                .Skip(SkipCount)
                .Take(PageSize);
        }
        #endregion
    }

}
