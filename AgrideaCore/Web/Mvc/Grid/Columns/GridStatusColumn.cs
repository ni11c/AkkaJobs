using Agridea.Web.UI;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Routing;

namespace Agridea.Web.Mvc.Grid.Columns
{
    public class GridStatusColumn<T> : GridBoundColumn<T, string>
    {


        #region Overrides of GridColumnBase<T>
        public GridStatusColumn(IGridModel<T> gridModel,  RouteValueDictionary routeValues)
            : base(gridModel, "Status", m => ((ITrackEdition)m).Status)
        {
            routeValues_ = routeValues;
            useTooltip_ = routeValues != null;
            Title = string.Empty;
        }

        private readonly RouteValueDictionary routeValues_;
        private bool useTooltip_;
        #endregion

        #region Overrides of GridBoundColumn<T,string>
        public override string GetContent(T dataItem)
        {
            string url = null;
            string throbber = null;
            var item = (ITrackEdition)dataItem;
            if (useTooltip_ && item.EditionState == EditionStates.Updated)
            {
                url = GridModel.Context.BuildUrl(dataItem, routeValues_, GridModel.DataKeys);
                throbber = UrlHelper.GenerateContentUrl("~/Content/Images/throbber.gif", GridModel.Context.HttpContext);
            }

            return item.BuildContent(url, throbber);
        }
        #endregion
    }
}
