using Agridea.Resources;

namespace Agridea.Web.Mvc.Grid
{
    public static class GridParameters
    {
        public const int DefaultPageSize = 20;
        public const int DefaultPage = 1;
        public const int PagerPageDisplay = 10;
        public const string GridDefaultWidth = "100%";
        public const int PagerLength = 5;

        public const string EditMethod = "Edit";
        public const string DeleteMethod = "Delete";
        public const string UnDeleteMethod = "UnDelete";
        public const string CreateMethod = "Create";
        public const string ConfirmDeleteMessageMethod = "ConfirmDeleteMessage";
        public const string ToolTipMethod = "Tooltip";

        public const string Ascending = "A";
        public const string Descending = "D";
        public const char OrderSeparator = '-';
        public const string PropertyDirectionSeparator = "~";
        public const string PagerSeparator = " | ";

        public const string DefaultPropertyName = "Id";

        #region Keys
        public const string PageKey = "Page";
        public const string OrderKey = "Order";
        public const string PageSizeKey = "PageSize";
        public const string ActionKey = "action";
        public const string ControllerKey = "controller";
        #endregion Keys

        #region Images
        public const string EditImgUrl = "~/Content/Images/edit_16x16_transparent.png";
        public const string DeleteImgUrl = "~/Content/Images/delete_16x16.png";
        public const string UnDeleteImgUrl = "~/Content/Images/undelete_16x16.png";
        public const string ReinitializeImgUrl = "~/Content/Images/reinit_16x16.png";
        public const string UpdateImageUrl = "~/Content/Images/edit_16x16_transparent.png";
        public const string DetailsImageUrl = "~/Content/Images/info_16x16.png";
        public const string CheckImageUrl = "~/Content/Images/menu_valid.png";
        #endregion Images

        #region Alternate texts
        public static readonly string Edit = AgrideaCoreStrings.GridEditAlternateText;
        public static readonly string Delete = AgrideaCoreStrings.GridDeleteAlternateText;
        public static readonly string UnDelete = AgrideaCoreStrings.GridUnDeleteAlternateText;
        public static readonly string Reinitilalize = AgrideaCoreStrings.GridResetAlternateText;
        public static readonly string Details = AgrideaCoreStrings.GridDetailsAlternateText;
        public static readonly string Validate = AgrideaCoreStrings.GridValidateAlternateText;
        public static readonly string Update = AgrideaCoreStrings.GridUpdateAlternateText;
        public static readonly string Create = AgrideaCoreStrings.GridCreateAlternateText;
        public static readonly string ExportToExcel = AgrideaCoreStrings.GridExportToExcelAlternateText;
        public static readonly string ExportToPdf = AgrideaCoreStrings.GridExportToPdfAlternateText;
        public static readonly string Clone = AgrideaCoreStrings.GridCloneAlternateText;
        public static readonly string Geo = AgrideaCoreStrings.GridGeoAlternateText;
        #endregion Alternate texts
    }

    public static class GridMessages
    {
        public static readonly string NoResult = AgrideaCoreStrings.GridNoResult;
        public static readonly string NextPagerElements = string.Format(AgrideaCoreStrings.GridNextPagerElements, GridParameters.PagerLength);
        public static readonly string PreviousPagerElements = string.Format(AgrideaCoreStrings.GridPreviousPagerElements, GridParameters.PagerLength);
        public static readonly string NextPage = AgrideaCoreStrings.GridNextPage;
        public static readonly string PreviousPage = AgrideaCoreStrings.GridPreviousPage;
        public static readonly string FirstPage = AgrideaCoreStrings.GridFirstPage;
        public static readonly string LastPage = AgrideaCoreStrings.GridLastPage;
    }
}