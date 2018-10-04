using Agridea.Web.Helpers;

namespace Agridea.Calendar
{
    public partial class CalendarEvent : IFile
    {
        public static readonly string[] AuthorizedExtensions = 
        {
            FileExtensions.Doc, FileExtensions.DocX, FileExtensions.Xls, FileExtensions.Xlsx, FileExtensions.Pdf
        };
    }
}
