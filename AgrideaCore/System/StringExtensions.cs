using System.Data.Odbc;
using System.Globalization;
using Agridea.Diagnostics.Contracts;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Ajax.Utilities;

namespace System
{
    public static class StringExtensions
    {
        public static readonly string Elipses = "...";
        public static readonly char WhiteSpace = ' ';

        public static void WriteToFile(this string s, string path)
        {
            WriteToFile(s, path, Encoding.UTF8);
        }

        public static void WriteToFile(this string s, string path, Encoding encoding)
        {
            using (StreamWriter outfile = new StreamWriter(path, false, encoding))
            {
                outfile.Write(s);
            }
        }

        public static string ResetToDefaultIfWhiteSpace(this string s, string defaultValue = null)
        {
            if (string.IsNullOrWhiteSpace(s)) return defaultValue;
            return s;
        }
        public static string Truncate(this string s, int length)
        {
            Requires<ArgumentException>.GreaterOrEqual(length, 0);
            if (string.IsNullOrEmpty(s)) return string.Empty;
            if (s.Length <= length) return s;
            return s.Substring(0, length);
        }

        public static string TruncateWithElipses(this string s, int length)
        {
            Requires<ArgumentException>.GreaterOrEqual(length, 0);
            if (string.IsNullOrEmpty(s)) return string.Empty;
            if (s.Length <= length) return s;
            if (length <= Elipses.Length) return Truncate(s, length);

            return s.Substring(0, Math.Min(s.Length, length - Elipses.Length)) + Elipses;
        }

        public static string TruncateWithElipsesOnWordBoundary(this string s, int length)
        {
            Requires<ArgumentException>.GreaterOrEqual(length, 0);

            if (string.IsNullOrEmpty(s)) return string.Empty;
            if (s.Length <= length) return s;
            if (s.Length <= Elipses.Length) return Truncate(s, length);

            int lastSpace = s.Substring(0, length).LastIndexOf(WhiteSpace);
            return s.Substring(0, lastSpace) + Elipses; //Bug if lastSpace == -1, i.e. if first word lenght > length
        }

        public static string Remove(this string s, string toBeRemoved)
        {
            return s == null ? null : s.Replace(toBeRemoved, "");
        }

        public static string RemoveMany(this string s, params string[] toBeRemoved)
        {
            if (s == null)
                return null;

            toBeRemoved.ToList().ForEach(r => s = s.Replace(r, ""));
            return s;
        }

        public static string DefaultTo(this string s, string defaultValue)
        {
            return s ?? defaultValue;
        }

        public static string DefaultToEmpty(this string s)
        {
            return s.DefaultTo(string.Empty);
        }

        public static string NormalizeSpaces(this string s)
        {
            return Regex.Replace(s.DefaultToEmpty(), @"\s+", " ");
        }

        public static string TrimAndNormalizeSpaces(this string s)
        {
            return NormalizeSpaces(s.DefaultToEmpty().Trim());
        }

        /// <summary>
        /// Poor man's pluralization. Some better implementations sure exist.
        /// </summary>
        public static string Pluralize(this string s)
        {
            if (String.IsNullOrEmpty(s)) return String.Empty;
            if (s.EndsWith("y")) return s.TrimEnd('y') + "ies";
            if (s.EndsWith("sh") || s.EndsWith("ch")) return s + "es";
            return s + "s";
        }

        public static bool EndsWithVowel(this string s)
        {
            if (String.IsNullOrEmpty(s)) return false;
            return "aeiouy".ToCharArray().Contains(s[s.Length - 1]);
        }

        public static string PrePend(this string s, string value)
        {
            return s.Insert(0, value);
        }

        public static string Append(this string s, string value)
        {
            return s.Insert(s.Length, value);
        }

        public static bool Intersects(this string s, string charSet)
        {
            return charSet.Any(x => s.Contains(x));
        }

        /// <summary>
        /// Does not work for more than one level of generic type A<B,C> but not A<B<C>,D>
        /// </summary>
        public static string GetFullName(this string assemblyQualifiedTypeName)
        {
            var indexOfLeftBracket = assemblyQualifiedTypeName.IndexOf('[');
            if (indexOfLeftBracket == -1)
            {
                string[] components = assemblyQualifiedTypeName.Split(',');
                Requires<ArgumentException>.AreEqual(5, components.Length, string.Format("{0} is not an assembly qualified name", assemblyQualifiedTypeName));
                return components[0].Trim();
            }

            var mainType = assemblyQualifiedTypeName.Substring(0, indexOfLeftBracket);
            var indexOfRightBracket = assemblyQualifiedTypeName.LastIndexOf(']');
            var parameterTypes = assemblyQualifiedTypeName.Substring(indexOfLeftBracket + 1, indexOfRightBracket - indexOfLeftBracket - 1);

            const string regex = @"\[(?<PARAMETERTYPE>[^\[\]]+)\]";
            var matches = Regex.Matches(parameterTypes, regex);
            mainType += "[";
            var separator = "";
            foreach (Match match in matches)
            {
                mainType += separator + GetFullName(match.Groups["PARAMETERTYPE"].Value);
                separator = ",";
            }
            mainType += "]";
            return mainType;
        }

        public static string GetAssemblyName(this string assemblyQualifiedTypeName)
        {
            string[] components;
            var indexOfLeftBracket = assemblyQualifiedTypeName.IndexOf('[');
            if (indexOfLeftBracket == -1)
            {
                components = assemblyQualifiedTypeName.Split(',');
                Requires<ArgumentException>.AreEqual(5, components.Length, string.Format("{0} is not an assembly qualified name", assemblyQualifiedTypeName));
                return components[1].Trim();
            }

            var indexOfRightBracket = assemblyQualifiedTypeName.LastIndexOf(']');
            var lastPart = assemblyQualifiedTypeName.Substring(indexOfRightBracket + 2);
            components = lastPart.Split(',');
            Requires<ArgumentException>.AreEqual(4, components.Length, string.Format("{0} is not an assembly qualified name", assemblyQualifiedTypeName));
            return components[0].Trim();
        }

        public static Stream ToStream(this string s)
        {
            using (var stream = new MemoryStream())
            {
                StreamWriter writer = new StreamWriter(stream);
                writer.Write(s);
                writer.Flush();
                stream.Position = 0;
                return stream;
            }
        }

        public static string ToBase64(this string s)
        {
            if (string.IsNullOrEmpty(s)) return "";
            string b64 = "";
            using (MemoryStream stream = new MemoryStream())
            {
                new BinaryFormatter().Serialize(stream, s);
                b64 = Convert.ToBase64String(stream.ToArray());
            }
            return b64;
        }

        public static string SafeJoin(char separator, params string[] strings)
        {
            return strings.Aggregate((x, y) => String.IsNullOrEmpty(y) ? x : string.Concat(x, separator, y));
        }

        public static string SafeJoin(params string[] strings)
        {
            return SafeJoin(' ', strings);
        }

        /// <remarks>
        /// Better with http://stackoverflow.com/questions/1479364/c-sharp-params-keyword-with-two-parameters-of-the-same-type. No better idea
        /// at this time. Having this method named SafeJoin "steals" calls to the previous SafeJoin(params string[] strings).
        /// Also, if an item is null or empty, it is not joined.
        /// Extra guard added if all strings are null or empty, returns ""
        /// </remarks>
        public static string SafeJoinWithSeparator(string separator, params string[] strings)
        {
            if (strings.All(x => string.IsNullOrEmpty(x))) return string.Empty;
            return strings.Where(s => !string.IsNullOrEmpty(s)).Aggregate((x, y) => string.Concat(x, separator, y));
        }

        public static T ToEnum<T>(this string value, IDictionary<string, T> dictionary, T defaultValue) where T : struct
        {
            return dictionary.ContainsKey(value) ? dictionary[value] : defaultValue;
        }

        public static T ToEnum<T>(this int value, IDictionary<int, T> dictionary, T defaultValue) where T : struct
        {
            return dictionary.ContainsKey(value) ? dictionary[value] : defaultValue;
        }

        /// <summary>
        /// http://stackoverflow.com/questions/7343465/compression-decompression-string-with-c-sharp
        /// </summary>
        public static byte[] Zip(this string str)
        {
            var bytes = Encoding.UTF8.GetBytes(str);

            using (var msi = new MemoryStream(bytes))
            using (var mso = new MemoryStream())
            {
                using (var gs = new GZipStream(mso, CompressionMode.Compress))
                {
                    msi.CopyTo(gs);
                }

                return mso.ToArray();
            }
        }

        public static byte[] ToByteArray(this string base64Str)
        {
            return Convert.FromBase64String(base64Str);
        }

        public static IEnumerable<string> Split(this string s, int chunkSize)
        {
            Requires<ArgumentOutOfRangeException>.GreaterThan(chunkSize, 1, "chunk size must be at least 1");

            if (string.IsNullOrEmpty(s) || s.Length <= chunkSize)
                return new List<string>() { s };

            var output = new List<string>();
            int len = s.Length;
            for (int i = 0; i < len; i += chunkSize)
            {
                if (i + chunkSize > len)
                    output.Add(s.Substring(i, len - i));
                else
                    output.Add(s.Substring(i, chunkSize));
            }
            return output;
        }

        public static IEnumerable<string> SplitOnWordBoundary(this string s, int chunkSize)
        {
            Requires<ArgumentOutOfRangeException>.GreaterThan(chunkSize, 1, "chunk size must be at least 1");

            if (string.IsNullOrEmpty(s) || s.Length <= chunkSize)
                return new List<string>() { s };

            var output = new List<string>();
            string current = s;
            while (current.Length > 0)
            {
                if (current.Length <= chunkSize)
                {
                    output.Add(current);
                    break;
                }
                else
                {
                    int i = current.Substring(0, chunkSize).LastIndexOf(WhiteSpace);
                    output.Add(current.Substring(0, i).Trim());
                    current = current.Substring(i);
                }
            }

            return output;
        }

        /// <summary>
        /// Transforms a non empty string using an optional mapping dictionnary
        /// </summary>
        public static string Map(this string s, IDictionary<string, string> mappings)
        {
            Requires<ArgumentException>.IsNotEmpty(s);
            if (mappings == null) return s;
            return mappings.ContainsKey(s) ? mappings[s] : s;
        }

        public static string Last(this string s, int length)
        {
            if (s == null) return null;
            if (s == string.Empty) return string.Empty;
            if (s.Length < length) return s;
            return s.Substring(s.Length - length, length);
        }

        public static bool In(this string s, params string[] list)
        {
            return list.Contains(s);
        }

        public static bool ContainsAny(this string s, params char[] list)
        {
            return list.Any(item => s.Contains(item));
        }

        public static TimeSpan ToTimeSpan(this string s)
        {
            if (string.IsNullOrEmpty(s)) return new TimeSpan();

            var components = s.Split(new char[] { ':' });
            var componentCount = components.Length;

            var day = componentCount - 4 >= 0 ? Convert.ToInt32(components[componentCount - 4]) : 0;
            var hour = componentCount - 3 >= 0 ? Convert.ToInt32(components[componentCount - 3]) : 0;
            var minutes = componentCount - 2 >= 0 ? Convert.ToInt32(components[componentCount - 2]) : 0;
            var seconds = componentCount - 1 >= 0 ? Convert.ToInt32(components[componentCount - 1]) : 0;

            return new TimeSpan(day, hour, minutes, seconds);
        }

        /// <summary>
        /// http://www.levibotelho.com/development/c-remove-diacritics-accents-from-a-string/
        /// </summary>
        public static string RemoveDiacritics(this string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return text;

            text = text.Normalize(NormalizationForm.FormD);
            var chars = text.Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark).ToArray();
            return new string(chars).Normalize(NormalizationForm.FormC);
        }

        /// <summary>
        /// Index of the nth occurence of a pattern 
        /// </summary>
        public static int IndexOfNth(this string str, string pattern, int nth = 1)
        {
            Requires<ArgumentException>.GreaterOrEqual(nth, 1, "Can only find the 1st, 2nd, 3rd... pattern in a string");

            int offset = str.IndexOf(pattern);
            for (int i = 1; i < nth; i++)
            {
                if (offset == -1) return -1;
                offset = str.IndexOf(pattern, offset + 1);
            }
            return offset;
        }

        /// <summary>
        /// Substring upto the nth occurence of a pattern
        /// </summary>
        public static string SubstringUptoNth(this string str, string pattern, int from, int nth = 1)
        {
            Requires<ArgumentException>.GreaterOrEqual(from, 0, "Can only find the nth pattern from 0 onwards");
            Requires<ArgumentException>.GreaterOrEqual(nth, 1, "Can only find the 1st, 2nd, 3rd... pattern in a string");
            var substring = str.Substring(from);
            return substring.Substring(0, substring.IndexOfNth(pattern, nth));
        }

        /// <summary>
        /// Substring after the nth occurence of a pattern
        /// </summary>
        public static string SubstringAfterNth(this string str, string pattern, int from, int nth = 1)
        {
            Requires<ArgumentException>.GreaterOrEqual(from, 0, "Can only find the nth pattern from 0 onwards");
            Requires<ArgumentException>.GreaterOrEqual(nth, 1, "Can only find the 1st, 2nd, 3rd... pattern in a string");
            var substring = str.Substring(from);
            return substring.Substring(substring.IndexOfNth(pattern, nth)+1);
        }
    }
}