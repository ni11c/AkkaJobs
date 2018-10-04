using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;
using Agridea.Diagnostics.Contracts;
using Agridea.Web.Mvc.Session;

namespace Agridea.Web.Mvc.Menu
{
    public abstract class MenuItemBase : IMenuItem
    {
        #region Constants

        protected const string ControllerRouteValueKey = "controller";
        protected const string ControllerTypeRouteValueKey = "controllerType";
        protected const string ActionRouteValueKey = "action";
        protected const string HtmlAttributesClassKey = "class";
        protected const string CrumbSeparator = " &gt; ";
        protected const string CssSelected = "selected";
        protected const string CssExpandable = "expandable ";
        protected const string CssCompleted = "completed ";
        protected const string CssIncomplete = "incomplete ";
        protected const string CssAccordion = "accordion";
        protected const string CssMainMenu = "main-menu";
        protected const string CssMainMenuItem = "main-menu-item ";
        protected const string CssMainMenuSubMenu = "sub-menu";
        protected const string CssConditionalActive = "conditional-active";
        protected const string CssRelatedToParcel = "menuitem-parcel";
        protected const string HotKeyAccessor = "§";

        #endregion Constants

        #region Members

        protected IMenuBuilder MenuBuilder;
        protected RouteValueDictionary RouteValueDictionary;
        protected string cssIsActiveClass_;
        private bool isSuperUser_;

        #endregion Members

        #region Initialization

        protected MenuItemBase(string title)
        {
            Initialize();
            Title = title;
        }

        private static readonly ConcurrentDictionary<string, RouteValues> routeValuesFromAction_ = new ConcurrentDictionary<string, RouteValues>();

        protected MenuItemBase(string controllerName, string actionName, string title)
        {
            Initialize();
            Title = title;
            if (!string.IsNullOrWhiteSpace(actionName) && !string.IsNullOrWhiteSpace(controllerName))
                SetRouteValuesFromAction(controllerName, actionName);
        }

        private void Initialize()
        {
            MenuBuilder = WebSiteSession.MenuBuilder;
            Children = new List<IMenuItem>();
            RouteValueDictionary = new RouteValueDictionary();
            IsVisibleInMenu = IsVisibleInCrumb = IsEnabled = IsLinkActive = true;
            IsDynamic = false;
        }

        #endregion Initialization

        #region IMenuItem

        #region Properties

        public IMenuBuilder Builder { get; set; }
        public string Title { get; set; }
        public bool IsVisibleInMenu { get; protected set; }
        public bool IsNoLink { get; protected set; }
        public bool IsVisibleInCrumb { get; protected set; }
        public bool IsEnabled { get; set; } //not got set should be at most protected
        public bool IsMandatory { get; protected set; }
        public bool IsRelatedToParcel { get; protected set; }
        public bool IsCompleted { get; protected set; }
        public bool IsDynamic { get; protected set; }
        public bool IsRoot { get; protected set; }
        public string Key { get; protected set; }
        public string HotKey { get; protected set; }
        public string CssIsActiveClass { get; protected set; }
        public bool IsLinkActive { get; protected set; }
        public IMenuItem Parent { get; set; }
        public bool IsSuperUser
        {
            get
            {
                return !isSuperUser_ && Parent != null
                           ? Parent.IsSuperUser
                           : isSuperUser_;
            }
            protected set { isSuperUser_ = value; }
        }
        public virtual string Controller
        {
            get
            {
                return RouteValueDictionary.ContainsKey(ControllerRouteValueKey)
                           ? RouteValueDictionary[ControllerRouteValueKey].ToString()
                           : null;
            }
        }
        public virtual string Action
        {
            get
            {
                return RouteValueDictionary.ContainsKey(ActionRouteValueKey)
                           ? RouteValueDictionary[ActionRouteValueKey].ToString()
                           : null;
            }
        }
        public RouteValueDictionary RouteValues
        {
            get { return RouteValueDictionary; }
        }
        public IList<IMenuItem> Children { get; private set; }

        #endregion Properties

        #region Property setters

        public IMenuItem SetVisible(bool value)
        {
            IsVisibleInMenu = value;
            IsVisibleInCrumb = value;
            return this;
        }

        public IMenuItem Hide()
        {
            return SetVisible(false);
        }

        public virtual IMenuItem ActiveWhen(bool value, string cssClass)
        {
            return this;
        }

        public IMenuItem SetMandatory()
        {
            IsMandatory = true;
            return this;
        }

        public IMenuItem SetCompleted(bool value)
        {
            IsCompleted = value;
            return this;
        }

        public IMenuItem SetEnabled(bool value)
        {
            IsEnabled = value;
            return this;
        }

        public IMenuItem SetSuperUser()
        {
            IsSuperUser = true;
            return this;
        }

        public virtual IMenuItem SetHotKey(string hotkey)
        {
            HotKey = hotkey;
            return this;
        }

        #endregion Property setters

        #region Menu Construction

        public IMenuItem Add(IMenuItem child, bool condition = true)
        {
            MenuBuilder.Add(this, child, condition);
            return this;
        }

        public IMenuItem AddChild(IMenuItem menuItem, bool condition = true)
        {
            Children.Add(menuItem);
            menuItem.Parent = this;
            return this;
        }

        #endregion Menu Construction

        #region Accessors

        public string GetKey()
        {
            Key = (RouteValues.Values.Any() ? string.Join("_", RouteValues.Values) : Title)
                .Replace(" ", "_");

            var parentKey = Parent != null ? Parent.Key : string.Empty;

            return string.Concat(parentKey, Key);
        }

        public virtual string GetHotKey()
        {
            var parentHotKey = Parent != null ? Parent.GetHotKey() : string.Empty;
            return string.Join(" ", parentHotKey, HotKey);
        }

        public virtual string GetUrl()
        {
            var retValue = RouteValueDictionary != null
                               ? RouteValueDictionary.ToUrl()
                               : null;
            return retValue;
        }

        protected virtual string GetTitle()
        {
            if (string.IsNullOrEmpty(HotKey))
                return Title;

            int index = Title.ToLower().IndexOf(HotKey, StringComparison.Ordinal);
            if (index < 0)
                return Title;

            bool wasUpper = char.IsUpper(Title[index]);
            string underlinedChar = "<span style=\"text-decoration: underline\">" + (wasUpper ? HotKey.ToUpper() : HotKey) + "</span>";
            string leftPart = HttpUtility.HtmlEncode(index > 0 ? Title.Substring(0, index) : "");
            string rightPart = HttpUtility.HtmlEncode(index < Title.Length - 1 ? Title.Substring(index + 1) : "");
            string title = leftPart + underlinedChar + rightPart;
            return title;
        }

        public bool HasUrl
        {
            get { return GetUrl() != null; }
        }
        public virtual bool HasHotKey
        {
            get { return !string.IsNullOrWhiteSpace(HotKey); }
        }

        public virtual IMenuItem GetMainMenuItem()
        {
            Requires<ArgumentNullException>.IsNotNull(Parent);
            return Parent.GetMainMenuItem();
        }

        public IMenuItem NextSibling
        {
            get
            {
                if (Parent == null)
                    return null;
                IList<IMenuItem> siblingMenuItems = Parent.Children;
                if (siblingMenuItems != null)
                {
                    int index = siblingMenuItems.IndexOf(this);
                    if ((index >= 0) && (index < siblingMenuItems.Count - 1))
                        return siblingMenuItems[index + 1];
                }
                return null;
            }
        }
        public IMenuItem PreviousSibling
        {
            get
            {
                if (Parent == null)
                    return null;
                IList<IMenuItem> siblingMenuItems = Parent.Children;
                if (siblingMenuItems != null)
                {
                    int index = siblingMenuItems.IndexOf(this);
                    if ((index > 0) && (index <= siblingMenuItems.Count - 1))
                        return siblingMenuItems[index - 1];
                }
                return null;
            }
        }

        public virtual IMenuItem GetSelectedMenuItem()
        {
            return IsVisibleInMenu
                       ? this
                       : Parent.GetSelectedMenuItem();
        }

        #endregion Accessors

        #region Writers

        public abstract string BuildCrumb(HtmlHelper helper);
        public abstract string BuildMainMenu(HtmlHelper helper, IMenuItem currentItem);

        public virtual string BuildAccordionMenu(HtmlHelper helper, IMenuItem currentItem)
        {
            return
                "<ol id=\"" + GetKey() + "\" class=\"" + CssAccordion + "\">" +
                string.Join(" ", Children.Select(x => x.ToAccordionMenu(helper, currentItem))) +
                "</ol>";
        }

        public virtual string BuildDropDownMenu(HtmlHelper helper, IMenuItem currentItem)
        {
            return
                "<div class=\"megamenu\">" +
                "<ul class = \"clearfix\">" +
                string.Join(" ", Children.Select(x => x.ToDropDownMenu(helper, currentItem))) +
                "</ul>" +
                "</div>";
        }

        public virtual string BuildKeyboardBindings()
        {
            return string.Join("", Children.Select(x => x.BuildKeyboardBindings()));
        }

        public virtual string BuildKeyboardShortcutsHelp()
        {
            if (!HasHotKey)
                return "";

            string title = "<p>" + Title + "</p>";
            if (Children.Any(x => x.HasHotKey))
            {
                title += "<table><tbody>" + string.Join("", Children.Select(c => c.BuildKeyboardShortcutsHelp())) + "</tbody></table>";
            }

            return string.Format("<tr><td><kbd>{0}</kbd></td><td>{1}</td></tr>", HotKey, title);
        }

        #endregion Writers

        #region RouteValues

        public virtual IMenuItem AddRouteValues(params KeyValuePair<string, object>[] keyValuePairs)
        {
            keyValuePairs.ToList().ForEach(item => RouteValues.Add(item.Key, item.Value));
            return this;
        }

        public IMenuItem AddRouteValue(string key, object value)
        {
            RouteValues.Add(key, value);
            return this;
        }

        public void AddRouteValue(IMenuItem item, string key, object value)
        {
            item.RouteValues.Add(key, value);
        }

        public IMenuItem AddRouteValue(KeyValuePair<string, object> kvp)
        {
            return AddRouteValue(kvp.Key, kvp.Value);
        }

        #endregion RouteValues

        #endregion IMenuItem

        #region Helpers

        protected bool HasActiveCheck()
        {
            return !string.IsNullOrWhiteSpace(CssIsActiveClass);
        }

        protected bool HasChildren()
        {
            return Children.Count > 0 && Children.Any(m => m.IsVisibleInMenu);
        }

        protected string GetSubMenuCssClass()
        {
            string cssClass = string.Empty;
            if (HasChildren())
                cssClass += CssExpandable;

            if (IsCompleted)
                cssClass += CssCompleted;

            if (IsMandatory && !IsCompleted)
                cssClass += CssIncomplete;

            if (IsRelatedToParcel)
                cssClass += CssRelatedToParcel;

            return cssClass;
        }

        protected virtual MvcHtmlString GetLink(HtmlHelper helper)
        {
            return helper.ActionLink(Title, Action, Controller, RouteValues, null);
        }

        protected virtual bool IsInThisSubTree(IMenuItem currentItem)
        {
            Asserts<ArgumentNullException>.IsNotNull(currentItem);

            if (currentItem == this)
                return true;

            while (currentItem.Parent != null && !currentItem.Parent.IsRoot)
                currentItem = currentItem.Parent;
            return currentItem == this;
        }

        protected void SetRouteValuesFromAction(string controllerName, string actionName)
        {
            var routeValues = routeValuesFromAction_.GetOrAdd(
                actionName + "~" + controllerName,
                expression =>
                {
                    return new RouteValues
                    {
                        ControllerName = controllerName,
                        ActionName = actionName
                    };
                }
                );
            RouteValueDictionary[ControllerRouteValueKey] = routeValues.ControllerName;
            RouteValueDictionary[ActionRouteValueKey] = routeValues.ActionName;
        }

        #endregion Helpers
    }

    public class RouteValues
    {
        public string ControllerName { get; set; }
        public string ActionName { get; set; }
    }
}