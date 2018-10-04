using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace Agridea.Web.Mvc.Menu
{
    public class MainMenuItem : MenuItemBase
    {
        #region Initialization

        public MainMenuItem(string controllerName, string actionName, string title)
            : base(controllerName, actionName, title)
        {
        }

        #endregion Initialization

        #region Helpers

        protected override MvcHtmlString GetLink(HtmlHelper helper)
        {
            var htmlAttributes = new Dictionary<string, object> {{"id", GetKey()}};
            return Action == null ?
                       MvcHtmlString.Create(Title) :
                       helper.ActionLink(Title, Action, Controller, RouteValueDictionary, htmlAttributes);
        }

        #endregion Helpers

        #region MenuItemBase

        #region Accessors

        public override IMenuItem GetMainMenuItem()
        {
            return IsVisibleInMenu ? this : Parent.GetMainMenuItem();
        }

        public override string GetUrl()
        {
            return RouteValueDictionary.Keys.Count > 0 ? RouteValueDictionary.ToUrl() : null;
        }

        #endregion Accessors

        #region Writers

        public override string BuildCrumb(HtmlHelper helper)
        {
            return Parent.ToCrumb(helper) + (IsVisibleInCrumb ? GetLink(helper) + CrumbSeparator : string.Empty);
        }

        public override string BuildMainMenu(HtmlHelper helper, IMenuItem currentItem)
        {
            if (!IsVisibleInMenu)
                return null;

            var isMainMenuItemCssClass = Parent is NoLinkMenuItem ? "" : CssMainMenuItem;
            var isSelectedCssClass = IsInThisSubTree(currentItem) ? CssSelected : "";
            var liClass = (isMainMenuItemCssClass + isSelectedCssClass).TrimEnd();
            return "<li class=\"" + liClass + "\">" + GetLink(helper) + "</li>";
        }

        public override string BuildDropDownMenu(HtmlHelper helper, IMenuItem currentItem)
        {
            const int Count = 5; //Children.Count;

            return
                "<li class=\"head\">" +
                "<a class=\"head\" href=\"#\">" +
                GetTitle() +
                "</a>" +
                "<div class=\"" + "megacolumn _" + Count + "column\">" +
                string.Join(" ", Children.Select(x => "<div class=\"col_1\">" + x.BuildDropDownMenu(helper, currentItem) + "</div>")) +
                "</div>" +
                "</li>";
        }

        #endregion Writers

        #endregion MenuItemBase
    }
}