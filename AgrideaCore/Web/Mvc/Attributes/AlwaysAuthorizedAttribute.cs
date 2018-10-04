using System;

namespace Agridea.Web.Mvc
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true)]
    public class AlwaysAuthorizedAttribute : Attribute
    {
        #region Initialization
        public AlwaysAuthorizedAttribute()
        {
            On = true;
        }
        #endregion        
        
        #region Named Parameters
        public bool On { get; set; }
        #endregion
    }
}
