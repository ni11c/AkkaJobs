using System;

namespace Agridea.Web.Mvc
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Event, Inherited = false)]
    public class WebFarmCompatibleAttribute : Attribute
    {
        #region Initialization
        public WebFarmCompatibleAttribute()
        {
            Compatible = true;
            Reason = "Unknown";
        }
        #endregion

        #region Named Parameters
        public bool Compatible { get; set; }
        public string Reason { get; set; }
        #endregion
    }
}
