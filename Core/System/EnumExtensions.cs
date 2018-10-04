using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;
using Agridea.Core;

namespace System
{
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
                return "-?-";

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
}