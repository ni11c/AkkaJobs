using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Xml.Linq;
using Agridea.Diagnostics.Logging;

namespace Agridea.Sedex
{
    public class Sedex
    {
        private const string Envl = "envl_";
        private const string Data = "data_";
        private const string Xsi = "http://www.w3.org/2001/XMLSchema-instance";
        private const string Ns = "http://www.ech.ch/xmlns/eCH-0090/1";
        private const string SchemaLocation = "http://www.ech.ch/xmlns/eCH-0090/1 http://www.ech.ch/xmlns/eCH-0090/1/eCH-0090-1-0.xsd";

        public XDocument BuildEnvelope(string canton, int messageType, int messageClass, string recipientId, string date, string feedbackEmail = null)
        {
            var messageId = MessageId(canton);
            date = date ?? DateNow();
            XNamespace xsi = Xsi;
            XNamespace ns = Ns;

            var envelope = new XElement(ns + "envelope",
                                        new XAttribute(XNamespace.Xmlns + "xsi", xsi),
                                        new XAttribute(xsi + "schemaLocation", SchemaLocation),
                                        new XAttribute("version", "1.0"),
                                        new XElement(ns + "messageId", messageId),
                                        new XElement(ns + "messageType", messageType),
                                        new XElement(ns + "messageClass", messageClass),
                                        new XElement(ns + "referenceMessageId", messageId),
                                        new XElement(ns + "senderId", SenderId(canton)),
                                        new XElement(ns + "recipientId", recipientId),
                                        new XElement(ns + "eventDate", date),
                                        new XElement(ns + "messageDate", date)
                );

            if (feedbackEmail != null)
            {
                envelope.Add(new XElement(ns + "testData",
                                          new XElement(ns + "name", "Email"),
                                          new XElement(ns + "value", feedbackEmail)));
            }

            return new XDocument(
                new XDeclaration("1.0", "utf-8", null),
                envelope);
        }

        public void WriteData(string path, IEnumerable<KeyValuePair<string, Stream>> streams)
        {
            if (File.Exists(path))
            {
                Log.Warning("Cannot create file " + path + ", file already exists.");
                return;
            }

            try
            {
                using (var zipStream = File.Create(path))
                using (var zip = new ZipArchive(zipStream, ZipArchiveMode.Create))
                {
                    foreach (var stream in streams)
                    {
                        var data = zip.CreateEntry(stream.Key);
                        using (var writer = new StreamWriter(data.Open()))
                        using (var reader = new StreamReader(stream.Value))
                        {
                            string line;
                            while ((line = reader.ReadLine()) != null)
                                writer.WriteLine(line);
                        }
                    }
                }
            }
            catch (IOException)
            {
                Log.Error("An unexpected file system error has happened while writing sedex message " + path + ".");
                throw;
            }
        }

        public static Message NewMessage(string basePath, string timestamp)
        {
            basePath = basePath ?? Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            timestamp = timestamp ?? TimeStampNow();
            return new Message
            {
                EnvlPath = Path.Combine(basePath, Envl + timestamp + ".xml"),
                DataPath = Path.Combine(basePath, Data + timestamp + ".zip")
            };
        }

        private string MessageId(string canton)
        {
            return string.Format("{0}{1}", canton, new Random().Next(999999999).ToString("D9"));
        }

        private string SenderId(string canton)
        {
            return string.Format("2-{0}-13", canton);
        }

        public static string DateNow()
        {
            var now = DateTime.Now;
            return string.Format("{0}-{1}-{2}T{3}:{4}:{5}",
                                 now.Year.ToString("D4"),
                                 now.Month.ToString("D2"),
                                 now.Day.ToString("D2"),
                                 now.Hour.ToString("D2"),
                                 now.Minute.ToString("D2"),
                                 now.Second.ToString("D2"));
        }

        public static string TimeStampNow()
        {
            return DateTime.Now.ToString("yyyyMMddhhmmssfff");
        }
    }
}