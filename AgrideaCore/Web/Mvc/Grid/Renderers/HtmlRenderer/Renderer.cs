
using System.Collections.Generic;

namespace Agridea.Web.Mvc.Grid.Renderers
{
    public class Renderer<T>
    {
        #region Members
        private readonly Pager pager_;
        private readonly HeaderRenderer headerRenderer_;
        private readonly BodyRenderer<T> bodyRenderer_;
        private readonly ToolBarRenderer<T> toolBarRenderer_;
        #endregion

        #region Initialization
        public Renderer(GridModel<T> model)
        {
            pager_ = new Pager(model.Pagination, model.Context);
            headerRenderer_ = new HeaderRenderer(model);
            bodyRenderer_ = new BodyRenderer<T>(model);
            toolBarRenderer_ = new ToolBarRenderer<T>(model.ToolBar, model.DisplayColumnsCount);
        }
        #endregion

        #region Services
        public string RenderPagination()
        {
            return pager_.ToString();
        }

        public string RenderHeader()
        {
            return headerRenderer_.ToString();
        }

        public FluentTagBuilder RenderBody(FluentTagBuilder tbody, IList<T> items)
        {
            return bodyRenderer_.Render(tbody, items);
        }

        public FluentTagBuilder RenderToolBar(FluentTagBuilder tag)
        {
            return tag.Html(toolBarRenderer_.ToString());
        }
        #endregion
    }
}
