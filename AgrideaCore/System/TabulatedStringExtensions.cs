using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.TabulatedString
{
    public static class TabulatedStringHelper
    {
        public static string SubPadRight(this string val, int totalWidth, char paddingChar = ' ')
        {
            return val == null
                ? string.Empty.PadRight(totalWidth, paddingChar)
                : val.PadRight(totalWidth, paddingChar).Substring(0, totalWidth);
        }

        public static string SubPadRight(this int? val, int totalWidth, char paddingChar = ' ')
        {
            return (val == null
                ? string.Empty
                : val.ToString()).SubPadRight(totalWidth, paddingChar);
        }

        public static string SubPadLeft(this string val, int totalWidth, char paddingChar)
        {
            return val == null
                       ? string.Empty.PadLeft(totalWidth, paddingChar)
                       : val.PadLeft(totalWidth, paddingChar).Substring(0, totalWidth);
        }

        public static string ConvertDiacritics(this string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                char[] oldChar = { 'À', 'Á', 'Â', 'Ã', 'Ä', 'Å', 'à', 'á', 'â', 'ã', 'ä', 'å', 'Ò', 'Ó', 'Ô', 'Õ', 'Ö', 'Ø', 'ò', 'ó', 'ô', 'õ', 'ö', 'ø', 'È', 'É', 'Ê', 'Ë', 'è', 'é', 'ê', 'ë', 'Ì', 'Í', 'Î', 'Ï', 'ì', 'í', 'î', 'ï', 'Ù', 'Ú', 'Û', 'Ü', 'ù', 'ú', 'û', 'ü', 'ÿ', 'Ñ', 'ñ', 'Ç', 'ç', '°' };
                char[] newChar = { 'A', 'A', 'A', 'A', 'A', 'A', 'a', 'a', 'a', 'a', 'a', 'a', 'O', 'O', 'O', 'O', 'O', 'O', 'o', 'o', 'o', 'o', 'o', 'o', 'E', 'E', 'E', 'E', 'e', 'e', 'e', 'e', 'I', 'I', 'I', 'I', 'i', 'i', 'i', 'i', 'U', 'U', 'U', 'U', 'u', 'u', 'u', 'u', 'y', 'N', 'n', 'C', 'c', ' ' };

                for (var i = 0; i < oldChar.Length; i++)
                {
                    value = value.Replace(oldChar[i], newChar[i]);
                }
            }
            return value;
        }

        public static bool CheckKtidFormat(this string ktidb)
        {
            if (string.IsNullOrWhiteSpace(ktidb))
                return false;

            if (ktidb.Length != 10)
                return false;

            int num = 0;
            if (!Int32.TryParse(ktidb.Substring(2), out num))
                return false;

            return true;
        }
    }
}
