using System;

namespace Agridea.Web.UI
{
    [Serializable]
    public class RedirectUrlTool
    {
        #region Members
        private NavigationHistory navigationHistory_;
        #endregion

        #region Initialization
        public RedirectUrlTool()
        {
            navigationHistory_ = new NavigationHistory();
        }
        public RedirectUrlTool(Uri urlReferrer)
        {
            RedirectUrl = urlReferrer.PathAndQuery;
        }
        #endregion

        #region Services
        public string RedirectUrl { get; set; }
        public void Add(Uri request)
        {
            navigationHistory_.Add(request);
        }
        public override string ToString()
        {
            return navigationHistory_.ToString();
        }
        public string RetreiveBackUrl()
        {
            var backUrl = navigationHistory_.RetreiveBackUrl();
            if (backUrl == null) return null;
            return backUrl.PathAndQuery;
        }
        #endregion

        #region Helpers
        #endregion
    }
}
