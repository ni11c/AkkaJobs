using System;
namespace Agridea.Web.Helpers
{
    public static class SortHelper
    {
        #region Constants

        private const string DefaultSortDirection = SortDirection.Ascending;

        #endregion

        #region Services
        [Obsolete("Use GetReverseOrder(...) instead")]
        public static string GetOrder(string propertyName, string currentDirection, string currentPropertyName)
        {
            return GetReverseOrder(propertyName, currentDirection, currentPropertyName);
        }
        public static string GetReverseOrder(string propertyName, string currentDirection, string currentPropertyName)
        {
            if (propertyName.Equals(currentPropertyName))
                return (currentDirection.Equals(DefaultSortDirection)) ? SortDirection.Descending : DefaultSortDirection;

            return DefaultSortDirection;
        }
        #endregion
    }
}
