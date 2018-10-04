using System.Collections.Generic;

namespace Agridea.Web.Mvc.Grid.Columns
{

    public abstract class GridColumnBase<T> : IGridColumn
    {
        #region Initialization

        protected GridColumnBase(IGridModel<T> gridModel, string name)
        {
            GridModel = gridModel;
            Name = name;
            IsVisible = true;
            HasTitle = true;
            IsVisibleForExport = true;
            Rowspan = 1;
            HtmlAttributes = new Dictionary<string, object>();
            CssClasses = new List<string>();
        }

        #endregion Initialization

        #region Properties

        internal IGridModel<T> GridModel { get; private set; }

        public string Title { get; set; }

        public string Name { get; private set; }

        public int Width { get; set; }

        public bool IsVisible { get; set; }

        public bool IsVisibleForExport { get; set; }

        public bool IsAlternate { get; set; }

        public int Rowspan { get; set; }

        public bool HasTitle { get; set; }

        public CustomHeader CustomHeader { get; set; }

        public bool NeedsFooter { get; set; }

        public bool IsMerged { get; set; }

        public IDictionary<string, object> HtmlAttributes { get; set; }

        public List<string> CssClasses { get; set; }

        #endregion Properties

        #region Services

        public virtual string GetHeader()
        {
            return string.Empty;
        }

        public virtual string GetContent(T dataItem)
        {
            return string.Empty;
        }

        public string GetExportHeader()
        {
            return (Title ?? string.Empty).Replace("<br />", " ");
        }

        public virtual string GetExportContent(T dataItem)
        {
            return string.Empty;
        }

        public FluentTagBuilder RenderHeader()
        {
            return GetHeaderTag().Html(GetHeader());
        }

        public virtual FluentTagBuilder RenderFooter()
        {
            return Tag.Th;
        }

        public virtual FluentTagBuilder GetContentTag(T dataItem)
        {
            var tag = Tag.Td;
            CssClasses.ForEach(cssClass => tag.AddCssClass(cssClass));
            return tag;
        }

        public virtual FluentTagBuilder GetHeaderTag()
        {
            return Tag.Th.Rowspan(Rowspan); //.Style("white-space:nowrap");
        }

        public abstract string GetWarning(T dataItem);

        #endregion Services
    }
}