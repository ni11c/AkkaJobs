namespace Agridea.Web.UI
{
    public interface ITrackEdition
    {
        EditionStates EditionState { get; set; }
        string Status { get; set; }
    }

    public static class TrackEditionChangesExtensions
    {
        public const string InvalidParcelClassName = "invalidparcel";
        public const string NewClassName = "newStatus";
        public const string ModifiedClassName = "modifiedStatus";
        public const string DeletedClassName = "deletedStatus";
        public const string DiffValueClassName = "diffvalue";
        public const string EditionStateClassName = "editionstate";

        public static string CssClass(this ITrackEdition item)
        {
            switch (item.EditionState)
            {
                case EditionStates.Deleted:
                    return DeletedClassName;
                case EditionStates.New:
                    return NewClassName;
                case EditionStates.Updated:
                    return ModifiedClassName;
            }
            return string.Empty;
        }

        public static string BuildContent(this ITrackEdition item, string url = null, string throbberUrl = null)
        {
            if (url == null)
                return "<span class='status " + item.CssClass() + "'>" + item.Status + "</span>";

            return "<span class='status " + item.CssClass() + "'><a href='" + url + "' throbber='" + throbberUrl + "' class='raiseToolTip'>" + item.Status + "</a></status>";
        }
    }
}