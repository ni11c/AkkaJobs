using Agridea.Diagnostics.Contracts;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Agridea.DataRepository
{
    /// <summary>
    /// Manages a dictionary of string values suitable for storing custom properties
    /// Provides untyped and typed routine for adding and retreiving value (with overridable default values)
    /// </summary>
    public class CustomProperties
    {
        #region Members
        private string customProperties_;
        #endregion

        #region Initialization
        public CustomProperties(string customProperties = "")
        {
            customProperties_ = customProperties;
        }
        #endregion

        #region Services
        public string Clear()
        {
            customProperties_ = Serialize(new Dictionary<string, string>());
            return customProperties_;

        }
        public string Remove(string key)
        {
            Dictionary<string, string> settings = Deserialize(customProperties_);
            if (settings.ContainsKey(key))
                settings.Remove(key);
            customProperties_ = Serialize(settings);
            return customProperties_;
        }
        public string GetValue(string key)
        {
            Dictionary<string, string> settings = Deserialize(customProperties_);
            if (settings.ContainsKey(key))
                return settings[key];
            return null;
        }
        public T GetValue<T>(string key)
        {
            var rawValue = GetValue(key);
            if (rawValue != null) return (T)Convert.ChangeType(rawValue, typeof(T));
            return default(T);
        }
        public T GetValue<T>(string key, T defaultValue)
        {
            var rawValue = GetValue(key);
            if (rawValue != null) return (T)Convert.ChangeType(rawValue, typeof(T));
            return defaultValue;
        }
        public string AddValue<T>(string key, T value)
        {
            return AddValue(key, value.ToString());
        }
        #endregion

        #region Helpers
        private string AddValue(string key, string value)
        {
            Dictionary<string, string> dictionary = Deserialize(customProperties_);
            if (dictionary.ContainsKey(key))
                dictionary.Remove(key);
            dictionary.Add(key, value);
            customProperties_ = Serialize(dictionary);
            return customProperties_;
        }
        private Dictionary<string, string> Deserialize(string dictionary)
        {
            dictionary = dictionary ?? string.Empty;
            var deserialized = new Dictionary<string, string>();
            string regex = @"^(?<KEY>.+)\=(?<VALUE>.+)?";
            foreach (var setting in dictionary.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries))
            {
                Match match = Regex.Match(setting, regex);
                Asserts<InvalidOperationException>.IsTrue(match.Success);
                deserialized.Add(match.Groups["KEY"].Value, match.Groups["VALUE"].Value);
            }
            return deserialized;
        }
        private string Serialize(Dictionary<string, string> dictionary)
        {
            string serialized = string.Empty;
            foreach (var setting in dictionary)
                serialized += string.Format("{0}={1};", setting.Key, setting.Value);
            return serialized;
        }
        #endregion
    }
}