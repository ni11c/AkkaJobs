using System.Web.Mvc;

namespace Agridea.Web.Mvc.Menu
{
    /// <remarks>
    /// NDE: what is the reason of this ? Can't we just call the underlying IMenuItem methods directly ?
    /// </remarks>>
    public static class MenuItemExtensions
    {
        public static string ToCrumb(this IMenuItem menuItem, HtmlHelper helper)
        {
            return menuItem.BuildCrumb(helper);
        }
        public static string ToMainMenu(this IMenuItem menuItem, HtmlHelper helper, IMenuItem currentItem)
        {
            return menuItem.BuildMainMenu(helper, currentItem);
        }
        public static string ToAccordionMenu(this IMenuItem menuItem, HtmlHelper helper, IMenuItem currentItem)
        {
            return menuItem.BuildAccordionMenu(helper, currentItem);
        }
        public static string ToDropDownMenu(this IMenuItem menuItem, HtmlHelper helper, IMenuItem currentItem)
        {
            return menuItem.BuildDropDownMenu(helper, currentItem);
        }
    }
}