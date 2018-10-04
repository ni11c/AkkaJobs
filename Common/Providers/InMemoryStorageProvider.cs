using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Agridea.Prototypes.Akka.Common.Providers
{
    public class InMemoryStorageProvider : IStorageProvider
    {
        private readonly Dictionary<string, XDocument> dataStore_ = new Dictionary<string, XDocument>();

        public string Store(XDocument doc)
        {
            var key = Guid.NewGuid().ToString();
            dataStore_.Add(key, doc);
            return key;
        }

        public XDocument Read(string key)
        {
            XDocument value;
            dataStore_.TryGetValue(key, out value);
            return value;
        }
    }
}