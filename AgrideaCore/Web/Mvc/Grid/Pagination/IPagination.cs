
namespace Agridea.Web.Mvc.Grid
{
    public interface IPagination
    {
        /// <summary>
        /// The current page number
        /// </summary>
        int PageNumber { get; }
        /// <summary>
        /// The number of items in each page.
        /// </summary>
        int PageSize { get; set; }
        /// <summary>
        /// The total number of items.
        /// </summary>
        int TotalItems { get; }
        /// <summary>
        /// The total number of pages.
        /// </summary>
        int TotalPages { get; }
        /// <summary>
        /// The index of the first item in the page.
        /// </summary>
        int FirstItem { get; }
        /// <summary>
        /// The index of the last item in the page.
        /// </summary>
        int LastItem { get; }
        /// <summary>
        /// The number of items to skip when querying the source
        /// </summary>
        int SkipCount { get; }
        /// <summary>
        /// Whether there are pages before the current page.
        /// </summary>
        bool HasPreviousPage { get; }
        /// <summary>
        /// Whether there are pages after the current page.
        /// </summary>
        bool HasNextPage { get; }

        bool IsDisplayed { get; set; }
    }
    public interface IPagination<T> : IPagination
    {
    }
}
