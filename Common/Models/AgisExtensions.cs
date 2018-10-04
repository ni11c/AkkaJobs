using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Agridea.Acorda.Agis
{
    public static class Extensions
    {
        public static string ToXmlDateTimeWithoutMillisecondsAndGmt(this DateTime t)
        {
            return $"{t.Year.ToString("D4")}-{t.Month.ToString("D2")}-{t.Day.ToString("D2")}T{t.Hour.ToString("D2")}:{t.Minute.ToString("D2")}:{t.Second.ToString("D2")}";
        }

        public static DateTime ParseForDateTime(this string xmlDateTime)
        {
            if (string.IsNullOrEmpty(xmlDateTime))
                return default(DateTime);
            var e = new XElement("dummy", xmlDateTime);
            return (DateTime) e;
        }

        public static string TimeStampNow()
        {
            return DateTime.Now.ToString("yyyyMMddhhmmssfff");
        }

        public static XDocument FormatToXdocument(this IList<StructureModel> modelList)
        {
            return new XDocument(modelList.Export().AsXElement());
        }

        public static IBlwData Export(this IList<StructureModel> modelList)
        {
            var animalData = modelList.ToSafeFlattenedList(x => x.BindAnimals());
            var structure = new structuralDataRoot
            {
                animalData = animalData,
            };
            return structure;
        }

        public static T FormatCodeToItemEnum<T>(this int code, T defaultValue = default(T), string codeFormat = "00", bool warn = true) where T : struct
        {
            return String.Format("Item{0}", code.ToString(codeFormat)).ParseToEnum(defaultValue, warn);
        }

        public static List<T> ToSafeFlattenedList<T, TModel>(this IList<TModel> model, Func<TModel, List<T>> listFunc) 
        {
            var flattened = model.Where(x => listFunc(x) != null).SelectMany(listFunc);
            return flattened.Where(x => x != null).ToList();
        }

        public static int CheckAnimalCodes(this int code)
        {
            const int defaultCode = 1971;
            var validCodes = new[]
            {
                1110, 1123, 1124, 1128, 1129, 1141, 1142, 1143, 1144, 1150, //bovins
                1211, 1212, 1214, 1216, 1219, 1244, 1246, 1249, 1254, 1256, 1259, //chevaux
                1351, 1353, 1355, 1357, 1359, 1379, //Moutons
                1461, 1463, 1465, 1467, 1471, 1472, //Chevres
                1571, 1572, 1575, 1578, 1581, 1582, 1585, 1586, //Autres animaux cons. FG
                1611, 1615, 1621, 1631, 1635, 1639, //porcs
                1751, 1753, 1754, 1755, 1757, 1761, 1762, 1763, //Volaille de rente
                1861, 1862, 1863, //Lapins
                1871, 1872, 1876, 1877, 1878, 1883, 1884, 1886, 1887, 1888, 1890, //autres animaux et volaille
                1901, 1902, 1903, 1904, 1971//autres animaux
            };
            var correctionLookup = new Dictionary<int, int>
            {
                {1583, 1581}, //lamas stat
                {1587, 1585}, //alpagas stat
                {1874, 1890}, //perdrix stat
                {1875, 1877}, //autruches
                {1880, 1890}, //autres volailles
                {1881, 1863}, //lapins
                {1885, 1901}, //chèvres naines
            };

            if (correctionLookup.ContainsKey(code))
                code = correctionLookup[code];

            return code.In(validCodes)
                ? code
                : defaultCode;
        }
    }
}