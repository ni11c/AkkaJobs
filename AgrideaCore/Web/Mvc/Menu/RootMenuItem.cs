using System.Linq;
using System.Web.Mvc;

namespace Agridea.Web.Mvc.Menu
{
    public class RootMenuItem : MenuItemBase
    {
        #region Initialization

        public RootMenuItem(string controllerName, string actionName, string title)
            : base(controllerName, actionName, title)
        {
            IsRoot = true;
        }

        #endregion Initialization

        #region MenuItemBase

        public override IMenuItem GetMainMenuItem()
        {
            return this;
        }

        public override string BuildCrumb(HtmlHelper helper)
        {
            return GetLink(helper) + CrumbSeparator;
        }

        public override string BuildMainMenu(HtmlHelper helper, IMenuItem currentItem)
        {
            var itemClass = (CssMainMenuItem + (this == currentItem ? CssSelected : string.Empty)).TrimEnd();

            return
                "<div class=\"" + CssMainMenu + "\">" +
                "<ul>" +
                (IsVisibleInMenu ?
                     "<li class=\"" + itemClass + "\">" + GetLink(helper) + "</li>"
                     : null) +
                string.Join(" ", Children.Select(x => x.ToMainMenu(helper, currentItem))) +
                "</ul>" +
                "</div>";
        }

        public override string BuildAccordionMenu(HtmlHelper helper, IMenuItem currentItem)
        {
            return null;
        }

        public override string BuildKeyboardBindings()
        {
            return "<script>" + string.Join("", Children.Select(x => x.BuildKeyboardBindings())) + "</script>";
        }

        public override string BuildKeyboardShortcutsHelp()
        {
            return "<table class=\"grid hotkeys\"><tbody>" + string.Join("", Children.Select(c => c.BuildKeyboardShortcutsHelp())) + "</tbody></table>";
        }

        #endregion MenuItemBase
    }
}