using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Agridea.Web.Mvc.Menu
{
    public class LinkMenuItem : MenuItemBase
    {
        #region Properties

        protected string Id { get; set; }

        #endregion Properties

        #region Initialization

        public LinkMenuItem(string controllerName, string actionName, string title)
            : base(controllerName, actionName, title)
        {
        }

        #endregion Initialization

        #region Helpers

        protected override MvcHtmlString GetLink(HtmlHelper helper)
        {
            string linkFormat = "<a href=\"{0}\" id=\"{1}\">{2}</a>";
            string link = string.Format(linkFormat, GetUrl(), GetKey(), GetTitle());
            return IsLinkActive
                       ? MvcHtmlString.Create(link)
                       : MvcHtmlString.Create(Title);

            //return IsLinkActive
            //           ? helper.ActionLink(GetTitle(), Action, Controller, RouteValues, new { id = GetKey() } )
            //           : MvcHtmlString.Create(Title);
        }

        #endregion Helpers

        #region MenuItemBase

        public override IMenuItem ActiveWhen(bool value, string cssClass)
        {
            IsLinkActive = value;
            CssIsActiveClass = cssClass + (value ? "-active" : "-inactive");
            return base.ActiveWhen(value, cssClass);
        }

        public override string GetHotKey()
        {
            string baseHotKey = base.GetHotKey().Trim();
            return string.IsNullOrWhiteSpace(baseHotKey)
                       ? null
                       : HotKeyAccessor + " " + baseHotKey;
        }

        #region Writers

        public override string BuildCrumb(HtmlHelper helper)
        {
            return Parent.ToCrumb(helper) + (IsVisibleInCrumb ? GetLink(helper) + CrumbSeparator : string.Empty);
        }

        public override string BuildMainMenu(HtmlHelper helper, IMenuItem currentItem)
        {
            return null;
        }

        public override string BuildAccordionMenu(HtmlHelper helper, IMenuItem currentItem)
        {
            if (!IsVisibleInCrumb)
                return null;

            IDictionary<string, string> htmlAttributes = new Dictionary<string, string>();
            string cssClass = GetSubMenuCssClass();
            if (cssClass.Length > 0)
                htmlAttributes.Add(HtmlAttributesClassKey, cssClass.TrimEnd());

            var selected = this == currentItem.GetSelectedMenuItem() ? "class=\"" + CssSelected + "\"" : "";

            return
                "<li " + selected + ">" +
                helper.ActionLinkSpan(Title, Action, Controller, RouteValues, htmlAttributes) +
                (HasChildren()
                     ? base.BuildAccordionMenu(helper, currentItem)
                     : "") +
                "</li>";
        }

        public override string BuildDropDownMenu(HtmlHelper helper, IMenuItem currentItem)
        {
            if (!IsVisibleInMenu)
                return null;

            var span = HasActiveCheck() ? "<span class=\"" + CssConditionalActive + " " + CssIsActiveClass + "\"></span>" : "";

            return
                "<li>" +
                span +
                GetLink(helper) +
                "</li>";
        }

        public override string BuildKeyboardBindings()
        {
            string id = GetKey();
            string hotKey = GetHotKey();

            return !string.IsNullOrEmpty(hotKey) && !string.IsNullOrEmpty(id)
                       ? string.Format("Mousetrap.bind('{0}', function () {{ document.getElementById('{1}').click(); }});\n", hotKey, id)
                         + string.Join("", Children.Select(x => x.BuildKeyboardBindings()))
                       : "";
        }

        #endregion Writers

        #endregion MenuItemBase
    }
}