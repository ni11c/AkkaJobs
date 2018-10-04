using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Agridea.Diagnostics.Contracts;

namespace Agridea.ObjectMapping
{
    public static class ReflectionExtensions
    {
        #region Services
        public static IList<string> CandidateNames(this string fullName)
        {
            if (fullName.Contains(PropertyPath.PropertyPathSeparator))
                return CandidateNamesForPath(fullName);
            return CandidateNamesForCamelCase(fullName);
        }
        public static string Suffix(this string name, string fullName)
        {
            Asserts<ArgumentException>.IsNotEmpty(fullName);
            Asserts<ArgumentException>.IsTrue(fullName.StartsWith(name));

            if (fullName.Contains(PropertyPath.PropertyPathSeparator))
                return fullName.Substring(name.Length + 1);
            return fullName.Substring(name.Length);
        }
        public static string GetPath(this IList<PropertyInfo> propertyInfos)
        {
            return string.Join(PropertyPath.PropertyPathSeparator.ToString(), propertyInfos.Select(x => x.Name));
        }
        public static IList<PropertyPath> GetPropertyPathsForLeafProperties(this Type type)
        {
            List<PropertyPath> propertyPaths = new List<PropertyPath>();
            foreach (var propertyChain in GetPropertyChains(type))
                propertyPaths.Add(new PropertyPath(type, propertyChain.GetPath(), propertyChain));
            return propertyPaths;
        }
        public static IList<PropertyInfo> GetMatchingPropertyChain(this string path, PropertyInfo targetProperty, Type sourceType)
        {
            Asserts<ArgumentException>.IsNotNull(targetProperty);
            Asserts<ArgumentException>.IsNotNull(sourceType);

            List<PropertyInfo> matchingPropertyChain = new List<PropertyInfo>();

            foreach (var candidateName in path.CandidateNames().OrderByDescending(x => x.Length) /*longer names first*/)
            {
                var sourceProperty = sourceType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                     .Where(x => x.Name == candidateName)
                     .FirstOrDefault();
                if (sourceProperty != null)
                {
                    matchingPropertyChain.Add(sourceProperty);

                    var suffix = candidateName.Suffix(path);
                    if (string.IsNullOrEmpty(suffix))
                    {
                        //Asserts<InvalidProgramException>.IsTrue(sourceProperty.PropertyType is compatible with targetProperty.PropertyType);
                        //    - primitive types : equal
                        //    - ref types : types are mapped
                        //    - collection : item types are mapped
                        return matchingPropertyChain;
                    }

                    var suffixMatchingPropertyChain = GetMatchingPropertyChain(suffix, targetProperty, sourceProperty.PropertyType);
                    if (suffixMatchingPropertyChain.Count == 0)
                        matchingPropertyChain.Clear();
                    else
                        matchingPropertyChain.AddRange(suffixMatchingPropertyChain);
                    return matchingPropertyChain;
                }
            }

            return matchingPropertyChain;
        }
        public static IList<IList<PropertyInfo>> GetPropertyChains(this Type type)
        {
            List<IList<PropertyInfo>> propertyChains = new List<IList<PropertyInfo>>();
            foreach (var property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(x => x.CanWrite))
            {
                List<PropertyInfo> propertyChain = new List<PropertyInfo>();
                propertyChain.Add(property);
                if (property.PropertyType.IsClass && property.PropertyType != typeof(string))
                {
                    var subPropertyChains = GetPropertyChains(property.PropertyType);
                    foreach (List<PropertyInfo> subPropertyChain in subPropertyChains)
                    {
                        subPropertyChain.InsertRange(0, propertyChain);
                        propertyChains.Add(subPropertyChain);
                    }
                }
                else
                {
                    propertyChains.Add(propertyChain);
                }
            }
            return propertyChains;
        }
        public static IList<string> CandidateNamesForCamelCase(this string name)
        {
            var words = SplitCamelCaseIntoWords(name);
            List<string> candidates = new List<string>();
            string candidate = string.Empty;
            foreach (var word in words)
                candidates.Add(candidate += word);
            return candidates;
        }
        public static IList<string> CandidateNamesForPath(this string path)
        {
            Asserts<ArgumentException>.IsTrue(path.Contains(PropertyPath.PropertyPathSeparator));
            List<string> candidates = new List<string>();
            var word = path.Substring(0, path.IndexOf(PropertyPath.PropertyPathSeparator));
            candidates.Add(word);
            return candidates;
        }
        public static IList<string> SplitCamelCaseIntoWords(this string name)
        {
            Asserts<ArgumentNullException>.IsNotNull(name);
            //Match right before every capital letter 
            //unless the capital letter is at the beginning of the string
            return new List<string>(Regex.Split(name, @"(?<!^)(?=[A-Z])"));
        }
        public static bool ImplementsGenericInterface(this Type type, Type @interface)
        {
            Asserts<NullReferenceException>.IsNotNull(type);
            Asserts<ArgumentException>.IsTrue(@interface != null && @interface.IsInterface && @interface.IsGenericType);

            return type.GetInterfaces().Any(x =>
                                            x.IsGenericType &&
                                            x.GetGenericTypeDefinition() == @interface);
        }
        public static bool IsOrImplementsGenericInterface(this Type type, Type @interface)
        {
            Asserts<NullReferenceException>.IsNotNull(type);
            return
                type.ImplementsGenericInterface(@interface) ||
                type.IsInterface && type.IsGenericType && type.GetGenericTypeDefinition() == @interface;
        }
        #endregion
    }
}
