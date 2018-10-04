using Agridea.Diagnostics.Logging;
using Agridea.ObjectMapping;
using Agridea.Security;
using Agridea.Web.Mvc;
using Agridea.Web.Mvc.Menu;
using Agridea.Web.Mvc.Session;
using Agridea.Web.UI;
using System;
using System.Web;

namespace Agridea.Web.Mvc.Session
{
    [Serializable]
    public static class WebSiteSession
    {
        #region Constants
        private static readonly string CultureKey = "AgrideaCulture";
        private static readonly string MenuBuilderKey = "MenuBuilder";
        private static readonly string MenuRefreshNeededKey = "MenuRefreshNeeded";
        private const string RedirectUrlToolKey = "RedirectUrlReferrer";
        private const string BugReportKey = "BugReport";
        private static readonly string MapContextKey = "MapContext";
        #endregion Constants

        #region Services
        public static void Reset()
        {
            HttpContext.Current.Session.Clear();
            HttpContext.Current.Session.Abandon();
        }

        public static MapContext MapContext
        {
            get { return HttpContext.Current.Session[MapContextKey] as MapContext; }
            set { HttpContext.Current.Session[MapContextKey] = value; }
        }
        public static string Culture
        {
            get { return HttpContext.Current.Session[CultureKey] as string; }
            set { HttpContext.Current.Session[CultureKey] = value; }
        }
        public static bool MenuRefreshNeeded
        {
            get { return Convert.ToBoolean(HttpContext.Current.Session[MenuRefreshNeededKey]); }
            set { HttpContext.Current.Session[MenuRefreshNeededKey] = value; }
        }
        public static RedirectUrlTool RedirectUrlTool
        {
            get
            {
                if (HttpContext.Current.Session[RedirectUrlToolKey] == null)
                    HttpContext.Current.Session[RedirectUrlToolKey] = new RedirectUrlTool();

                return HttpContext.Current.Session[RedirectUrlToolKey] as RedirectUrlTool;
            }
            set { HttpContext.Current.Session[RedirectUrlToolKey] = value; }
        }
        public static BugReportDTO BugReport
        {
            get { return HttpContext.Current.Session[BugReportKey] as BugReportDTO; }
            set { HttpContext.Current.Session[BugReportKey] = value; }
        }
        public static IMenuBuilder MenuBuilder
        {
            get { return HttpContext.Current.Session[MenuBuilderKey] as IMenuBuilder; }
            set { HttpContext.Current.Session[MenuBuilderKey] = value; }
        }

        #endregion
    }
}