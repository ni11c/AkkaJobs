using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using Agridea.Prototypes.Akka.Common.Providers;
using Agridea.Sedex;

namespace Agridea.Acorda.Agis
{
    public class StructureExporter
    {
        private readonly ExportContext context_;
        private readonly IDataProvider dataProvider_;
        private readonly IStorageProvider tempStorageProvider_;

        public StructureExporter(IDataProvider dataProvider, IStorageProvider temStorageProvider, ExportContext context)
        {
            dataProvider_ = dataProvider;
            tempStorageProvider_ = temStorageProvider;
            context_ = context;
        }

        public async void ExportAsync(int chunkSize, Stream zipStream, Stream envlStream)
        {
            var topFarmIds = await dataProvider_.GetAllFarmIdsForAgis();
            var chunks = topFarmIds.Select((farmId, i) => new {i, farmId})
                                   .GroupBy(x => x.i/chunkSize)
                                   .Select(grp => grp.Select(x => x.farmId));

            var keys = new List<string>();
            foreach (var chunk in chunks)
            {
                var data = new List<StructureModel>(chunkSize);
                foreach (var farmId in chunk)
                {
                    var model = await dataProvider_.GetStructureData(farmId);
                    data.Add(model);
                }

                XDocument temp = data.FormatToXdocument();
                string key = tempStorageProvider_.Store(temp);
                keys.Add(key);
            }

            // write zip
            using (var archive = new ZipArchive(zipStream, ZipArchiveMode.Create))
            {
                var message = archive.CreateEntry("message.xml");
                var settings = new XmlWriterSettings
                {
                    CloseOutput = false,
                    NewLineOnAttributes = true,
                    Indent = true,
                    Encoding = Encoding.GetEncoding("utf-8")
                };
                using (var writer = XmlWriter.Create(message.Open(), settings))
                {
                    writer.WriteStartDocument();
                    AssembleMessage(keys, writer);
                    writer.WriteEndDocument();
                }

                var header = archive.CreateEntry("header.xml");
                using (var writer = new StreamWriter(header.Open()))
                {
                    BuildHeader(MessageTypes.Structure, context_.IsTest).Save(writer);
                }
            }

            // write enveloppe
            new Sedex.Sedex()
                .BuildEnvelope(context_.Canton.ToString(), MessageTypes.Structure, 2, Recipients.Blw, null)
                .Save(envlStream);
        }

        private void AssembleMessage(IEnumerable<string> chunkKeys, XmlWriter writer)
        {
            XNamespace blwStructure = Namespaces.blwStructure;

            writer.WriteStartElement("blw-message", "message", Namespaces.blwMessage);
            writer.WriteAttributeString("minorVersion", "1");
            writer.WriteAttributeString("xmlns", "xml", null, Namespaces.xml);
            writer.WriteAttributeString("xmlns", "xsi", null, Namespaces.xsi);
            writer.WriteAttributeString("xsi", "schemaLocation", null, Namespaces.MessageSchemaLocation);
            writer.WriteAttributeString("xmlns", "blw-message", null, Namespaces.blwMessage);
            writer.WriteAttributeString("xmlns", "blw-register", null, Namespaces.blwRegister);
            writer.WriteAttributeString("xmlns", "blw-payments", null, Namespaces.blwPayments);
            writer.WriteAttributeString("xmlns", "blw-structure", null, Namespaces.blwStructure);
            writer.WriteAttributeString("xmlns", "blw-ecoEtho", null, Namespaces.blwEcoEtho);
            writer.WriteAttributeString("xmlns", "eCH-0007", null, Namespaces.eCH0007);
            writer.WriteAttributeString("xmlns", "eCH-0010ext", null, Namespaces.eCH0010Ext);
            writer.WriteAttributeString("xmlns", "eCH-0044", null, Namespaces.eCH0044);
            writer.WriteAttributeString("xmlns", "eCH-0046ext", null, Namespaces.eCH0046Ext);
            writer.WriteAttributeString("xmlns", "eCH-0058", null, Namespaces.eCH0058);

            BuildHeaderElement(Namespaces.blwMessage, Namespaces.eCH0058, Namespaces.xsi, MessageTypes.Structure, context_.IsTest, false)
                .WriteTo(writer); // </blw-message:header>

            writer.WriteStartElement("blw-message", "content", Namespaces.blwMessage);
            writer.WriteStartElement("blw-structure", "structuralDataRoot", Namespaces.blwStructure);
            new XElement(blwStructure + "cantonFlAbbreviation", context_.Canton).WriteTo(writer);
            new XElement(blwStructure + "surveyYear", context_.SurveyYear.Year).WriteTo(writer);

            AssembleTopLevelElement(chunkKeys, blwStructure, writer, "generalData", "farmGeneralData", "farmGeneralDataDeclaration");
            AssembleTopLevelElement(chunkKeys, blwStructure, writer, "animalData", "animalStock");
            AssembleTopLevelElement(chunkKeys, blwStructure, writer, "animalSummeringData", "animalStock");
            AssembleTopLevelElement(chunkKeys, blwStructure, writer, "generalSurfaceData", "surface");
            AssembleTopLevelElement(chunkKeys, blwStructure, writer, "slopeSurfaceData", "slopeSurface");
            AssembleTopLevelElement(chunkKeys, blwStructure, writer, "grapeSurfaceData", "grapeSurface");
            AssembleTopLevelElement(chunkKeys, blwStructure, writer, "biodiversitySurfaceData", "biodiversitySurface");
            AssembleTopLevelElement(chunkKeys, blwStructure, writer, "networkSurfaceData", "networkSurface");
            AssembleTopLevelElement(chunkKeys, blwStructure, writer, "countrysideQualityData", "countrysideQualityProject");
            AssembleTopLevelElement(chunkKeys, blwStructure, writer, "resourcesEfficiencyData", "lowEmissionFertilization", "mildSoilTreatment", "precisePesticideApplication");
            AssembleTopLevelElement(chunkKeys, blwStructure, writer, "aquacultureData", "aquaticAnimalData");
            AssembleTopLevelElement(chunkKeys, blwStructure, writer, "apicultureData", "farmApicultureData");

            writer.WriteEndElement(); // </blw-structure:structuralDataRoot>
            writer.WriteEndElement(); // </blw-message:content>
            writer.WriteEndElement(); // </blw-message:message>
        }

        /// <summary>
        /// DO NOT write this method using .SelectMany() and .Any() on all files ==> this would load all data to memory (and defeat the whole purpose).
        /// DO use foreach loops instead to visit the specified xml elements and write them to the writer.
        /// </summary>
        private void AssembleTopLevelElement(IEnumerable<string> chunkKeys, XNamespace ns, XmlWriter writer, string eltName, params string[] subEltNames)
        {
            var isEmpty = true;
            foreach (var subEltName in subEltNames)
            {
                foreach (var key in chunkKeys)
                {
                    var doc = tempStorageProvider_.Read(key);
                    var elements = doc.Descendants(ns + eltName).Descendants(ns + subEltName).ToList();
                    if (elements.Any())
                    {
                        if (isEmpty)
                        {
                            isEmpty = false;
                            writer.WriteStartElement("blw-structure", eltName, Namespaces.blwStructure);
                        }
                        elements.ForEach(e => e.WriteTo(writer));
                    }
                }
            }
            if (!isEmpty)
                writer.WriteEndElement();

            // --- Dont do this ! ---
            //var elements = subEltNames.SelectMany(e => partFiles.SelectMany(file => XDocument.Load(file).Descendants(ns + eltName).Descendants(ns + e))).ToList();
            //if (elements.Any())
            //{
            //    writer.WriteStartElement("blw-structure", eltName, Constants.blwStructure);
            //    elements.ForEach(e => e.WriteTo(writer));
            //    writer.WriteEndElement();
            //}
        }

        private XDocument BuildHeader(int messageType, bool isTest = false)
        {
            XNamespace blwMessage = Namespaces.blwMessage;
            XNamespace eCH0058 = Namespaces.eCH0058;
            XNamespace xsi = "http://www.w3.org/2001/XMLSchema-instance";

            return new XDocument(
                new XDeclaration("1.0", "utf-8", null),
                BuildHeaderElement(blwMessage, eCH0058, xsi, messageType, isTest)
                );
        }

        private XElement BuildHeaderElement(XNamespace blwMessage, XNamespace eCH0058, XNamespace xsi, int messageType, bool isTest, bool includeNamespaces = true)
        {
            var messageId = Sedex.Sedex.MessageId(context_.Canton.ToString());
            var thisType = GetType();
            var date = Sedex.Sedex.DateNow();

            var header = new XElement(blwMessage + "header",
                                      new XElement(eCH0058 + "senderId", Sedex.Sedex.SenderId(context_.Canton.ToString())),
                                      new XElement(eCH0058 + "originalSenderId", "token"),
                                      new XElement(eCH0058 + "declarationLocalReference", "token"),
                                      new XElement(eCH0058 + "recipientId", context_.RecipientId),
                                      new XElement(eCH0058 + "messageId", messageId),
                                      new XElement(eCH0058 + "referenceMessageId", messageId),
                                      new XElement(eCH0058 + "ourBusinessReferenceId", "token"),
                                      new XElement(eCH0058 + "yourBusinessReferenceId", "token"),
                                      new XElement(eCH0058 + "uniqueIdBusinessTransaction", "token"),
                                      new XElement(eCH0058 + "messageType", messageType),
                                      new XElement(eCH0058 + "subMessageType", 1),
                                      new XElement(eCH0058 + "sendingApplication",
                                                   new XElement(eCH0058 + "manufacturer", "Agridea"),
                                                   new XElement(eCH0058 + "product", thisType.Assembly.GetName().Name),
                                                   new XElement(eCH0058 + "productVersion", string.Join(".", thisType.Assembly.GetName().Version.ToString().Split('.').Take(3)))
                                          ),
                                      new XElement(eCH0058 + "partialDelivery",
                                                   new XElement(eCH0058 + "uniqueIdBusinessCase", "token"),
                                                   new XElement(eCH0058 + "totalNumberOfPackages", 0),
                                                   new XElement(eCH0058 + "numberOfActualPackage", 0)
                                          ),
                                      new XElement(eCH0058 + "subject", "token"),
                                      new XElement(eCH0058 + "comment", "token"),
                                      new XElement(eCH0058 + "messageDate", date),
                                      new XElement(eCH0058 + "initialMessageDate", date),
                                      new XElement(eCH0058 + "eventDate", date),
                                      new XElement(eCH0058 + "modificationDate", date),
                                      new XElement(eCH0058 + "action", 1),
                                      new XElement(eCH0058 + "testDeliveryFlag", isTest)
                );

            if (includeNamespaces)
            {
                header.Add(new XAttribute("minorVersion", 1));
                header.Add(new XAttribute(XNamespace.Xmlns + "xsi", xsi));
                header.Add(new XAttribute(XNamespace.Xmlns + "blw-message", blwMessage));
                header.Add(new XAttribute(XNamespace.Xmlns + "eCH-0058", eCH0058));
            }

            return header;
        }
    }

    public static class Namespaces
    {
        public const string MessageXsd = "message-1-96.xsd";
        public const string xml = "http://www.w3.org/XML/1998/namespace";
        public const string xsi = "http://www.w3.org/2001/XMLSchema-instance";
        public const string blwMessage = "http://www.admin.ch/xmlns/DataTypes/evd/asa/message/1";
        public const string blwRegister = "http://www.admin.ch/xmlns/DataTypes/evd/register/1";
        public const string blwStructure = "http://www.admin.ch/xmlns/DataTypes/evd/structure/2";
        public const string blwEcoEtho = "http://www.admin.ch/xmlns/DataTypes/evd/ecoEtho/2";
        public const string blwPayments = "http://www.admin.ch/xmlns/DataTypes/evd/payment/2";
        public const string eCH0007 = "http://www.ech.ch/xmlns/eCH-0007/4";
        public const string eCH0010Ext = "http://www.ech.ch/xmlns/eCH-0010/3";
        public const string eCH0044 = "http://www.ech.ch/xmlns/eCH-0044/1";
        public const string eCH0046Ext = "http://www.ech.ch/xmlns/eCH-0046/1";
        public const string eCH0058 = "http://www.ech.ch/xmlns/eCH-0058/3";
        public const string MessageSchemaLocation = "http://www.admin.ch/xmlns/DataTypes/evd/asa/message/1 ../" + MessageXsd;
    }

    public static class Constants
    {
        public const string PathToXsd = "Xsd//" + Namespaces.MessageXsd;
        public const string ZipFileName = "data_{0}.zip";
        public const string HeaderZipEntryName = "header.xml";
        public const string MessageZipEntryName = "message.xml";
        public const int Code100 = 100;
        public const string KtidbFarmIdCategory = "KT_ID_B";
        public const string KtidpPersonIdCategory = "KT_ID_P";
        public const string Switzerland = "CH";
        public static readonly int[] LegalFormCodesNotToSend = {100};
        public static readonly int[] FarmTypeCodesNotToSend = {100};
    }

    public class ExportContext
    {
        public cantonFlAbbreviationType Canton { get; set; }
        public DateTime SurveyYear { get; set; }
        public DateTime PaymentYear { get; set; }
        public string RecipientId { get; set; }
        public DateTime HistoryFrom { get; set; }
        public DateTime HistoryTo { get; set; }
        public bool IsTest { get; set; }
        public static ExportContext Default => new ExportContext();

        public ExportContext()
        {
            Canton = cantonFlAbbreviationType.GE;
            RecipientId = "3-CH-39";
            SurveyYear = DateTime.Now;
            PaymentYear = DateTime.Now.AddYears(-1);
            HistoryTo = DateTime.Now;
            HistoryFrom = HistoryTo.AddDays(-7);
            IsTest = false;
        }
    }

    public interface IBlwData
    {
        int MessageType { get; set; }
        string Serialize();
        string Serialize(Encoding encoding);
        XElement AsXElement();
    }

    [GeneratedCode("System.Xml", "4.0.30319.1")]
    [Serializable]
    public class BlwDataBase<T> : IBlwData
    {
        #region Members

        private XmlSerializerNamespaces namespaces_;

        #endregion Members

        public XmlSerializerNamespaces Namespaces
        {
            get
            {
                if (namespaces_ == null)
                {
                    namespaces_ = new XmlSerializerNamespaces();
                    namespaces_.Add("xsi", "http://www.w3.org/2001/XMLSchema-instance");
                }

                return namespaces_;
            }
        }
        [XmlIgnore]
        public int MessageType { get; set; }

        public virtual string Serialize()
        {
            return Serialize(Encoding.UTF8);
        }

        public virtual string Serialize(Encoding encoding)
        {
            string result = null;
            using (var stream = new MemoryStream())
            {
                using (var writer = XmlWriter.Create(stream, new XmlWriterSettings {Encoding = encoding, Indent = true, CloseOutput = true}))
                {
                    new XmlSerializer(typeof(T)).Serialize(writer, this, Namespaces);
                    stream.Seek(0, SeekOrigin.Begin);
                    var reader = new StreamReader(stream);
                    result = reader.ReadToEnd();
                    writer.Flush();
                }
            }
            return result;
        }

        public virtual XElement AsXElement()
        {
            var root = XDocument.Parse(Serialize()).Root;
            if (root.HasAttributes)
                root.Attributes().Remove();
            return root;
        }

        protected void AddNamespace(string alias, string ns)
        {
            Namespaces.Add(alias, ns);
        }
    }
}