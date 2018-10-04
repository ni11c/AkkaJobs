using System.Collections.Generic;

namespace Agridea.Web.Mvc.Grid.Columns
{
    public interface IGridColumn
    {
        string Title { get; set; }

        string Name { get; }

        int Width { get; set; }

        bool IsVisible { get; set; }

        bool IsVisibleForExport { get; set; }

        bool NeedsFooter { get; set; }

        int Rowspan { get; set; }

        CustomHeader CustomHeader { get; set; }

        bool HasTitle { get; set; }

        FluentTagBuilder RenderHeader();

        bool IsMerged { get; set; }

        //FluentTagBuilder GetContentTag();

        FluentTagBuilder GetHeaderTag();

        FluentTagBuilder RenderFooter();

        string GetExportHeader();

        IDictionary<string, object> HtmlAttributes { get; set; }

        List<string> CssClasses { get; set; }
    }
}