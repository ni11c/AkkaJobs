using System;
using System.IO;
using System.Xml.Linq;

namespace Agridea.Prototypes.Akka.Common.Providers
{
    public class FileSystemStorageProvider : IStorageProvider
    {
        private readonly string path_;

        public FileSystemStorageProvider(string path)
        {
            path_ = path;
        }

        public FileSystemStorageProvider()
            : this(Path.GetTempPath())
        {
        }

        public string Store(XDocument doc)
        {
            var key = Guid.NewGuid().ToString();
            doc.Save(GetPath(key));
            return key;
        }

        public XDocument Read(string key)
        {
            XDocument value = XDocument.Load(GetPath(key));
            return value;
        }

        private string GetPath(string key)
        {
            return Path.Combine(path_, key + ".xml");
        }
    }
}