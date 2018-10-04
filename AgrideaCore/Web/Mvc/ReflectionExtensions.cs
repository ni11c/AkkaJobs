using System;
using System.Linq;
using System.Reflection;

namespace Agridea.Web.Mvc
{
    public static class ReflectionExtensions
    {
        #region AlwaysAuthorized
        public static bool IsAlwaysAuthorized(this Type type)
        {
            var alwaysAuthorizedAttribute = type.GetCustomAttributes(true).FirstOrDefault(m => m.GetType() == typeof (AlwaysAuthorizedAttribute)) as AlwaysAuthorizedAttribute;
            if (alwaysAuthorizedAttribute == null) return false;

            return alwaysAuthorizedAttribute.On;
        }

        public static bool IsAlwaysAuthorized(this MethodInfo methodInfo)
        {
            var alwaysAuthorizedAttribute = methodInfo.GetCustomAttributes(true).FirstOrDefault(m => m.GetType() == typeof (AlwaysAuthorizedAttribute)) as AlwaysAuthorizedAttribute;
            if (alwaysAuthorizedAttribute != null)
                return alwaysAuthorizedAttribute.On;

            //Inheritance of attribute from class
            return methodInfo.DeclaringType.IsAlwaysAuthorized();
        }
        #endregion
    }
}
