using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Routing;

namespace Agridea.Web.Mvc.Menu
{
    public interface IMenuItem
    {
        #region Properties

        IMenuBuilder Builder { get; set; }
        string Title { get; set; }
        bool IsVisibleInMenu { get; }
        bool IsNoLink { get; }
        bool IsVisibleInCrumb { get; }
        bool IsEnabled { get; }
        bool IsMandatory { get; }
        bool IsCompleted { get; }
        bool IsDynamic { get; }
        bool IsRoot { get; }
        bool IsSuperUser { get; }
        string Key { get; }
        string HotKey { get; }
        string Controller { get; }
        string Action { get; }
        IMenuItem Parent { get; set; }
        RouteValueDictionary RouteValues { get; }
        IList<IMenuItem> Children { get; }

        #endregion Properties

        #region Menu Construction

        IMenuItem Add(IMenuItem child, bool condition = true);
        IMenuItem AddChild(IMenuItem menuItem, bool condition = true);

        #endregion Menu Construction

        #region Accessors

        string GetKey();
        string GetHotKey();
        string GetUrl();
        bool HasUrl { get; }
        IMenuItem GetMainMenuItem();
        IMenuItem NextSibling { get; }
        IMenuItem PreviousSibling { get; }
        IMenuItem GetSelectedMenuItem();
        bool HasHotKey { get; }
        #endregion Accessors

        #region Writers

        string BuildCrumb(HtmlHelper helper);
        string BuildMainMenu(HtmlHelper helper, IMenuItem currentItem);
        string BuildAccordionMenu(HtmlHelper helper, IMenuItem currentItem);
        string BuildDropDownMenu(HtmlHelper helper, IMenuItem currentItem);
        string BuildKeyboardBindings();
        string BuildKeyboardShortcutsHelp();

        #endregion Writers

        #region RouteValues

        IMenuItem AddRouteValues(params KeyValuePair<string, object>[] keyValuePairs);
        IMenuItem AddRouteValue(string key, object value);
        IMenuItem AddRouteValue(KeyValuePair<string, object> kvp);

        #endregion RouteValues

        #region properties setters

        IMenuItem SetVisible(bool value);
        IMenuItem Hide();
        IMenuItem SetMandatory();
        IMenuItem SetCompleted(bool value);
        IMenuItem SetEnabled(bool value);
        IMenuItem ActiveWhen(bool value, string cssClass);
        IMenuItem SetSuperUser();
        IMenuItem SetHotKey(string hotKey);

        #endregion properties setters
    }
}