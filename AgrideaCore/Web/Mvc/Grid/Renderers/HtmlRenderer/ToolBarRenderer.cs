using System.Web;
using Agridea.Web.Mvc.Grid.ToolBar;

namespace Agridea.Web.Mvc.Grid.Renderers
{
    public class ToolBarRenderer<T> : IHtmlString
    {
        #region Fields
        private readonly GridToolBar<T> gridToolBar_;
        private readonly int colspan_;
        #endregion

        #region Initilalization
        public ToolBarRenderer(GridToolBar<T> toolBar, int colspan)
        {
            gridToolBar_ = toolBar;
            colspan_ = colspan;
        }
        #endregion

        #region Services
        public string ToHtmlString()
        {
            if (gridToolBar_ == null || !gridToolBar_.Enabled) return null;

            return Tag.Tr.Html(
                Tag.Td.Colspan(colspan_)
                   .Html(gridToolBar_.Render()))
                      .ToString();
        }
        public override string ToString()
        {
            return ToHtmlString();
        }
        #endregion
    }
}
