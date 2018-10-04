using System.Linq;

namespace System.Reflection
{
    public static class PropertyInfoExtensions
    {

        public static Type GetInnerTypeFromGenericList(this PropertyInfo property)
        {
            return property == null ? null : property.PropertyType.GetGenericArguments()[0];
        }
        public static object GetValue(this PropertyInfo property, object obj)
        {
            return property.GetValue(obj, null);
        }
        public static void SetValue(this PropertyInfo property, object obj, object value)
        {
            property.SetValue(obj, value, null);
        }
        public static bool IsDiscriminant(this PropertyInfo propertyInfo)
        {
            return propertyInfo.GetCustomAttributes(true).Any(m => m.GetType().IsDiscriminant());
        }
        public static bool IsMandatory(this PropertyInfo propertyInfo)
        {
            return propertyInfo.GetCustomAttributes(true).Any(m => m.GetType().IsMandatory());
        }
        public static bool IsTransient(this PropertyInfo propertyInfo)
        {
            return propertyInfo.GetCustomAttributes(true).Any(m => m.GetType().IsTransient());
        }
        public static bool IsCollection(this PropertyInfo property)
        {
            return property != null && property.PropertyType.IsCollection();
        }
        public static bool IsReference(this PropertyInfo property)
        {
            return property != null && property.PropertyType.IsReference();
        }
        public static bool IsPrimitive(this PropertyInfo property)
        {
            return property != null && !property.IsReference() && !property.IsCollection();
        }
        public static bool IsNavigation(this PropertyInfo property)
        {
            return property != null && (property.IsReference() || property.IsCollection());
        }
    }
}
