using Agridea.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;

namespace Agridea.Web.UI
{
    public static class PaginationHelper
    {
        #region Constants
        private const string OrderKey = "Order";
        private const string DirectionKey = "Direction";
        public const int DefaultPageSize = 20;
        private const string PageKey = "Page";
        private const string ActionRouteValueKey = "action";
        private const int DefaultPageNumber = 1;
        private const string Descending = "D";
        private const string Ascending = "A";
        private const string Separator = "~";
        private const string JQueryUiIcon = "grid-icon";
        private const string JQueryUiTriangleNorth = "grid-icon-triangle-1-n";
        private const string JQueryUiTriangleSouth = "grid-icon-triangle-1-s";
        private const string LinkShiftClass = "linkShift";
        #endregion

        #region Services
        #region OrderedPagination

        public static IQueryable<T> PaginateAndSort<T>(this IQueryable<T> query, PaginationOptions options, out int count)
        {
            var orderProperty = options.Order == null ? new Dictionary<string, string>() : options.Order.ParseForOrder<T>();
            var pageNumber = Math.Max(DefaultPageNumber, Convert.ToInt32(options.Page));
            var pageSize = Math.Max(DefaultPageSize, Convert.ToInt32(options.PageSize));
            var orderedPagination = new OrderedPagination<T>(query, pageNumber, pageSize, orderProperty);
            count = orderedPagination.TotalItems;
            return orderedPagination.PaginateAndSort();
        }
        public static IList<T> ToOrderedPagination<T>(this IQueryable<T> queryable, PaginationOptions options, out int totalCount)
        {
            var orderProperty = options.Order == null ? new Dictionary<string, string>() : options.Order.ParseForOrder<T>();
            var pageNumber = Math.Max(DefaultPageNumber, Convert.ToInt32(options.Page));
            var pageSize = Math.Max(DefaultPageSize, Convert.ToInt32(options.PageSize));
            var orderedPagination = new OrderedPagination<T>(queryable, pageNumber, pageSize, orderProperty);
            totalCount = orderedPagination.TotalItems;
            return orderedPagination.GetOrderedPaginatedList();
        }
        public static IList<T> ToOrderedPagination<T>(this IOrderedQueryable<T> queryable, PaginationOptions options, out int totalCount)
        {
            var pageNumber = Math.Max(DefaultPageNumber, Convert.ToInt32(options.Page));
            var pageSize = Math.Max(DefaultPageSize, Convert.ToInt32(options.PageSize));
            var orderedPagination = new OrderedPagination<T>(queryable, pageNumber, pageSize);
            totalCount = orderedPagination.TotalItems;
            return orderedPagination.GetPaginatedList();
        }
        #endregion

        #region SortLink
        public static IHtmlString SortLinkFor<T>(this HtmlHelper helper, string linkText, string sortPropertyName)
        {
            var requestContext = helper.ViewContext.RequestContext;
            var queryString = requestContext.HttpContext.Request.QueryString;
            var orderDirectionDictionary = queryString[OrderKey] == null ? new Dictionary<string, string>() : queryString[OrderKey].ParseForOrder<T>();
            var orderDirectionQuery = string.Empty;
            var routeDataDictionary = requestContext.RouteData.Values.ToDictionary(m => m.Key, m => m.Value);
            var arrowSpan = Tag.Span;
            if (orderDirectionDictionary.ContainsKey(sortPropertyName))
            {
                if (orderDirectionDictionary[sortPropertyName] == Ascending)
                {
                    orderDirectionQuery += sortPropertyName + Separator + Descending;
                    routeDataDictionary[OrderKey] = orderDirectionQuery;
                    arrowSpan.Class(JQueryUiIcon + " " + JQueryUiTriangleNorth);

                }
                else
                {
                    routeDataDictionary.Remove(OrderKey);
                    arrowSpan.Class(JQueryUiIcon + " " + JQueryUiTriangleSouth);
                }
            }
            else
            {
                orderDirectionQuery += sortPropertyName + Separator + Ascending;
                routeDataDictionary.Add(OrderKey, orderDirectionQuery);
            }
            IDictionary<string, object> htmlAttributes = new Dictionary<string, object>();
            htmlAttributes.Add("class", LinkShiftClass);
            htmlAttributes.Add("id", sortPropertyName);

            var link = helper.ActionLink(linkText, routeDataDictionary[ActionRouteValueKey].ToString(), new RouteValueDictionary(routeDataDictionary), htmlAttributes).ToString();
            return MvcHtmlString.Create(link + arrowSpan);
        }
        public static IHtmlString SortLinkFor<T>(this HtmlHelper helper, string linkText, Expression<Func<T, object>> sortProperty)
        {
            var sortPropertyName = string.Join(".", sortProperty.Body.RemoveUnary().ToString().Split('.').Skip(1));
            return helper.SortLinkFor<T>(linkText, sortPropertyName);
        }
        #endregion

        #region Pagination
        public static MvcHtmlString PaginationMenu(this HtmlHelper helper, int totalElement)
        {
            var routeDataDictionary = new Dictionary<string, object>();
            var querystring = helper.ViewContext.HttpContext.Request.QueryString;
            foreach (var key in querystring.Keys.Cast<string>().Where(key => !routeDataDictionary.ContainsKey(key)))
                routeDataDictionary.Add(key, querystring[key]);

            var orderName = routeDataDictionary.ContainsKey(OrderKey) ? routeDataDictionary[OrderKey].ToString() : string.Empty;
            var direction = routeDataDictionary.ContainsKey(DirectionKey) ? routeDataDictionary[DirectionKey].ToString() : string.Empty;
            var currentPage = routeDataDictionary.ContainsKey(PageKey) ? Convert.ToInt32(routeDataDictionary[PageKey]) : DefaultPageNumber;
            return helper.PaginationMenu(orderName, direction, totalElement, currentPage, DefaultPageSize);
        }
        #endregion
        #endregion

        #region Helpers
        private static IDictionary<string, string> ParseForOrder<T>(this string orderString)
        {
            var possibleDirection = new[] { Descending, Ascending };
            var orderDictionary = new Dictionary<string, string>();
            var type = typeof(T);
            foreach (var order in orderString.Split('-').Where(m => m.Contains(Separator)))
            {
                var propertyDirection = order.Split(Separator.ToCharArray());
                var propertyName = propertyDirection.First();
                var direction = propertyDirection.Last();
                if (!type.PropertyExists(propertyName)) continue;
                if (!possibleDirection.Contains(direction)) continue;
                orderDictionary.Add(propertyName, direction);
            }
            return orderDictionary;
        }
        private static int ParseForPage(this NameValueCollection querystring)
        {
            var page = DefaultPageNumber;
            if (querystring[PageKey] != null)
                Int32.TryParse(querystring[PageKey], out page);
            return page;
        }
        private static Expression RemoveUnary(this Expression body)
        {
            var unary = body as UnaryExpression;
            return unary != null ? unary.Operand : body;
        }
        #endregion
    }
}
