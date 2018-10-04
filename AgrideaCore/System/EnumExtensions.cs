using Agridea.Diagnostics.Contracts;
using Agridea.Web.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using System.Xml.Serialization;

namespace System
{
    using Agridea.Diagnostics.Logging;
    using Agridea.Resources;

    public static class EnumExtensions
    {
        private const string EnumResourceType = "EnumLabels";

        /// <summary>
        /// Retrieves a string "label" from an Enum
        /// - from a EnumStringValueAttribute
        /// - from a resxFile (must be represented as [enumType]_[enumValue] in a resx file in the same assembly as the Enum)
        ///   The resx file Type can be specified by "resourceTypeName" parameter (default : looking for an "EnumLabels" resx in a "Resources" folder)
        /// - as  [enumValue].toString()
        /// </summary>
        /// <param name="value">selected enumeration value</param>
        /// <param name="resourceTypeName">resx type name</param>
        /// <returns></returns>
        public static string ToDisplayName(this Enum value, string resourceTypeName = null)
        {
            var defaultValue = value.ToString();
            var enumType = value.GetType();

            //check for EnumStringValueAttribute

            var fieldInfo = enumType.GetField(defaultValue);
            var stringValueAttribute = fieldInfo.GetCustomAttributes(false).OfType<EnumStringValueAttribute>().FirstOrDefault();
            if (stringValueAttribute != null)
                return stringValueAttribute.EnumStringValue;

            //check if value exists in enum
            if (value.ToString() == "Default")
                return AgrideaCoreStrings.WebComboChoose;

            var assembly = Assembly.GetAssembly(enumType);
            var resourceType = assembly.GetTypes().SingleOrDefault(m => m.Name == (resourceTypeName ?? EnumResourceType));
            if (resourceType == null) return defaultValue;

            var propertyInfo = resourceType.GetProperty(string.Join("_", enumType.Name, value.ToString()));
            return propertyInfo == null ? defaultValue : propertyInfo.GetValue(resourceType, null).ToString();
        }

        public static TEnum AsEnum<TEnum>(this int enumValue) where TEnum : struct, IConvertible
        {
            Requires<ArgumentException>.IsTrue(typeof(TEnum).IsEnum);
            if (Enum.IsDefined(typeof(TEnum), enumValue))
                return (TEnum)Enum.ToObject(typeof(TEnum), enumValue);
            return default(TEnum);
        }

        public static T ParseToEnum<T>(this string value, T defaultValue = default(T), bool warn = true) where T : struct
        {
            T parsed;
            if (!Enum.TryParse(value, out parsed))
            {
                if (warn)
                    Log.Warning("Could not parse value {0} into enum {1}, using default value {2}", value, typeof(T).Name, defaultValue);
                parsed = defaultValue;
            }

            return parsed;
        }

        public static string ToXmlName<T>(this T value) where T : struct, IConvertible
        {
            Requires<ArgumentException>.IsTrue(typeof(T).IsEnum);
            var enumType = typeof(T);
            if (!enumType.IsEnum) return null;//or string.Empty, or throw exception

            var member = enumType.GetMember(value.ToString()).FirstOrDefault();
            if (member == null) return null;//or string.Empty, or throw exception

            var attribute = member.GetCustomAttributes(false).OfType<XmlEnumAttribute>().FirstOrDefault();
            if (attribute == null) return value.ToString();//or string.Empty, or throw exception
            return attribute.Name;
        }
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class EnumStringValueAttribute : Attribute
    {
        public string EnumStringValue { get; set; }

        public EnumStringValueAttribute(string enumStringValue)
        {
            EnumStringValue = enumStringValue;
        }
    }

    public static class Enum<T> where T : struct, IConvertible
    {
        public static IEnumerable<SelectListItem> ToSelectListItem()
        {
            Requires<ArgumentException>.IsTrue(typeof(T).IsEnum);

            var enumerable = Enum.GetValues(typeof(T)).OfType<T>().ToList();
            enumerable.Sort();
            return enumerable.ToSelectListItem(m => (m as Enum).ToDisplayName(), m => Convert.ToInt32(m));
        }

        public static IEnumerable<SelectListItem> ToSelectListItem(int selectedValue)
        {
            Requires<ArgumentException>.IsTrue(typeof(T).IsEnum);
            Requires<ArgumentException>.IsTrue(Enum.IsDefined(typeof(T), selectedValue));

            var enumerable = Enum.GetValues(typeof(T)).OfType<T>().ToList();
            enumerable.Sort();
            return enumerable.ToSelectListItem(m => (m as Enum).ToDisplayName(), m => Convert.ToInt32(m), selectedValue);
        }

        public static IEnumerable<SelectListItem> ToSelectListItem(T selectedValue)
        {
            Requires<ArgumentException>.IsTrue(typeof(T).IsEnum);
            var enumerable = Enum.GetValues(typeof(T)).OfType<T>().ToList();
            enumerable.Sort();
            return enumerable.ToSelectListItem(m => (m as Enum).ToDisplayName(), m => m, selectedValue);
        }

        public static Dictionary<T, string> ToXmlEnumNameDictionary()
        {
            Requires<ArgumentException>.IsTrue(typeof(T).IsEnum);
            var enumerable = Enum.GetValues(typeof(T)).OfType<T>().ToList();
            return enumerable.ToDictionary(m => m,
                m => typeof(T).GetMember(m.ToString()).First().GetCustomAttributes(false).OfType<XmlEnumAttribute>().First().Name);
        }
    }
}