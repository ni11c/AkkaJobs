using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Agridea.Xml
{
    /// <summary>
    /// Helper class to query in-memory xml data
    /// </summary>
    public class XmlHelper
    {
        /// <summary>
        /// query value of element in xml tree filtered by an attribute value
        /// </summary>
        public static string GetElementValue(XElement tree, string element, string attribute, string attrValue)
        {
            var names = (from e in tree.Elements(element)
                         where (string)e.Attribute(attribute) == attrValue
                         select e.Value
                        );
            if (names.Count() > 0)
                return names.First();

            return null;
        }

        /// <summary>
        /// Validate xml fragement against an xsd xchema
        /// </summary>
        public static bool ValidateFragmentAgainstSchema(string xml, string xsdns, string xsdUri)
        {
            bool isValid = true;

            // define validation settings
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.ValidationType = ValidationType.Schema;
            settings.ValidationFlags |= System.Xml.Schema.XmlSchemaValidationFlags.ProcessInlineSchema;
            settings.ValidationFlags |= System.Xml.Schema.XmlSchemaValidationFlags.ReportValidationWarnings;
            settings.ValidationEventHandler += (object sender, ValidationEventArgs args) =>
            {
                // only deal with the exception if the event severity level is Error (as opposed to Warning)
                if (args.Severity == XmlSeverityType.Error)
                {
                    isValid = false;
                    Console.WriteLine(args.Message);
                }
            };
            XmlSchemaSet sc = new XmlSchemaSet();
            sc.Add(xsdns, xsdUri);
            settings.Schemas = sc;

            // create XmlReader with validation settings
            XmlReader reader = XmlReader.Create(new MemoryStream(new UTF8Encoding(false).GetBytes(xml)), settings);

            // Parse the xml
            while (reader.Read()) ;
            return isValid;
        }

        /// <summary>
        /// To convert a Byte Array of Unicode values (UTF-8 encoded) to a complete String.
        /// </summary>
        /// <param name="characters">Unicode Byte Array to be converted to String</param>
        /// <returns>String converted from Unicode Byte Array</returns>
        public static string UTF8ByteArrayToString(Byte[] characters)
        {
            UTF8Encoding encoding = new UTF8Encoding();
            String constructedString = encoding.GetString(characters);
            return (constructedString);
        }

        /// <summary>
        /// Validate xml against schema
        /// </summary>
        /// <param name="xml">xml string to validate</param>
        /// <param name="pathToSchema">path to schema used to validate</param>
        public static void ValidateXml(string xml, string pathToSchema)
        {
            ValidateXml(XDocument.Parse(xml), pathToSchema);
        }

        public static void ValidateXml(XDocument doc, string pathToSchema)
        {
            var schemas = new XmlSchemaSet();
            schemas.Add(XmlSchema.Read(new XmlTextReader(pathToSchema), null));
            doc.Validate(schemas, null);
        }

        public static T Deserialize<T>(string requestFile) where T : class
        {
            T request = null;
            using (var file = new FileStream(requestFile, FileMode.Open, FileAccess.Read))
            {
                var sr = new StreamReader(file);
                using (var reader = new StringReader(sr.ReadToEnd()))
                {
                    request = new XmlSerializer(typeof(T))
                        .Deserialize(XmlReader.Create(reader)) as T;
                }
            }
            return request;
        }

        public static T DeserializeString<T>(string xml, XmlRootAttribute root = null)
            where T : class
        {
            T t = null;
            using (var reader = new StringReader(xml))
            {
                t = new XmlSerializer(typeof(T), root)
                    .Deserialize(XmlReader.Create(reader)) as T;
            }
            return t;
        }

        public static bool TryValidate(string xml, params string[] schemaFiles)
        {
            bool isValid = true;
            XDocument doc = XDocument.Parse(xml);
            var schemas = new XmlSchemaSet();
            foreach (string xsdFile in schemaFiles)
            {
                schemas.Add(XmlSchema.Read(new XmlTextReader(xsdFile), null));
            }
            doc.Validate(schemas, (sender, eventargs) => { isValid = false; });
            return isValid;
        }
    }
}