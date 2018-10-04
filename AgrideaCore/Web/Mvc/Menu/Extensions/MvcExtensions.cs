using System.Web.Routing;

namespace Agridea.Web.Mvc.Menu
{
    public static class MvcExtensions
    {
        public static string ToUrl(this RouteValueDictionary routeValueDictionary)
        {
            if (routeValueDictionary.ContainsKey("id"))
                routeValueDictionary.Remove("id");
            var vdp = RouteTable.Routes.GetVirtualPath(null, routeValueDictionary);
            return vdp.VirtualPath;
        }
    }
}