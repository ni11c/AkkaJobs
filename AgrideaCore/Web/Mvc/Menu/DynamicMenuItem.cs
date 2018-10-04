using System;
using System.Linq.Expressions;
using System.Web.Mvc;

namespace Agridea.Web.Mvc.Menu
{
    public class DynamicMenuItem : MenuItemBase
    {
        #region Initialization



        public DynamicMenuItem(string controllerName, string actionName, string title)
            : base(controllerName, actionName, title)
        {
            IsVisibleInMenu = false;
            IsDynamic = true;
        }

        #endregion Initialization

        #region MenuItemBase

        #region Accessors

        public override IMenuItem GetSelectedMenuItem()
        {
            return Parent.GetSelectedMenuItem();
        }

        #endregion Accessors

        #region Writers

        public override string BuildCrumb(HtmlHelper helper)
        {
            return Parent.ToCrumb(helper) + (IsVisibleInCrumb ? (GetLink(helper) + CrumbSeparator) : string.Empty);
        }

        public override string BuildMainMenu(HtmlHelper helper, IMenuItem currentItem)
        {
            return null;
        }

        public override string BuildAccordionMenu(HtmlHelper helper, IMenuItem currentItem)
        {
            return null;
        }

        #endregion Writers

        #endregion MenuItemBase
    }
}