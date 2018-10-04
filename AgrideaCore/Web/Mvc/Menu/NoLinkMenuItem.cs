using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Agridea.Web.Mvc.Menu
{
    public class NoLinkMenuItem : MenuItemBase
    {
        #region Initialization

        public NoLinkMenuItem(string title)
            : base(title)
        {
            IsNoLink = true;
        }

        #endregion Initialization

        #region MenuItemBase

        #region Properties

        public override string Controller
        {
            get { return null; }
        }
        public override string Action
        {
            get { return null; }
        }

        #endregion Properties

        #region Accessors

        public override string GetUrl()
        {
            return null;
        }

        #endregion Accessors

        #region Writers

        public override string BuildCrumb(HtmlHelper helper)
        {
            return Parent.ToCrumb(helper) + (IsVisibleInCrumb ? Title + CrumbSeparator : "");
        }

        public override string BuildMainMenu(HtmlHelper helper, IMenuItem currentItem)
        {
            if (!IsVisibleInMenu || !Children.Any(m => m.IsVisibleInMenu))
                return null;

            var liClass = (CssMainMenuItem +
                           (IsInThisSubTree(currentItem)
                                ? CssSelected
                                : "" +
                                  CssMainMenuSubMenu)).TrimEnd();

            return
                "<li class=\"" + liClass + "\">" +
                "<span>" + HttpUtility.HtmlEncode(Title) + "</span>" +
                (Children.Any()
                     ? "<ul>" + string.Join(" ", Children.Select(child => child.ToMainMenu(helper, currentItem))) + "</ul>"
                     : null) +
                "</li>";
        }

        public override string BuildAccordionMenu(HtmlHelper helper, IMenuItem currentItem)
        {
            if (!IsVisibleInMenu || !Children.Any(m => m.IsVisibleInMenu) || Children.All(m => m.IsNoLink && m.Children.All(x => !x.IsVisibleInMenu)))
                return null;

            string cssClass = GetSubMenuCssClass();
            string attribute = string.Empty;
            if (cssClass.Length > 0)

                //attribute = string.Format("class=\"{0}\"", cssClass.TrimEnd());
                attribute = "class=\"" + cssClass.TrimEnd() + "\"";

            var selected = this == currentItem.GetSelectedMenuItem() ? "class=\"" + CssSelected + "\"" : "";

            return
                "<li " + selected + ">" +

                //string.Format("<a href=\"#\" {0}><span>{1}</span></a>", attribute, Title) +
                "<a href=\"#\" " + attribute + "><span>" + Title + "</span></a>" +
                (HasChildren() ? base.BuildAccordionMenu(helper, currentItem) : "") +
                "</li>";
        }

        public override string BuildDropDownMenu(HtmlHelper helper, IMenuItem currentItem)
        {
            if (!IsVisibleInMenu)
                return null;

            return
                "<h3>" + GetTitle() + "</h3>" +
                "<ul>" +
                string.Join(" ", Children.Select(x => x.BuildDropDownMenu(helper, currentItem))) +
                "</ul>";
        }

        #endregion Writers

        #endregion MenuItemBase
    }
}