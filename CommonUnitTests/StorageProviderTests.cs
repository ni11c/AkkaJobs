using System.Xml.Linq;
using Agridea.Prototypes.Akka.Common.Providers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Agridea.Acorda.Agis.UnitTest
{
    [TestClass]
    public class StorageProviderTests
    {
        private IStorageProvider storage_;

        [TestMethod]
        public void CanStoreInMemory()
        {
            storage_ = new InMemoryStorageProvider();
            CanReadAndWriteTestDoc(storage_, NewTestXDocument());
        }

        [TestMethod]
        public void CanStoreInFileSystem()
        {
            storage_ = new FileSystemStorageProvider();
            CanReadAndWriteTestDoc(storage_, NewTestXDocument());
        }

        private static XDocument NewTestXDocument()
        {
            return new XDocument(
                new XDeclaration("1.0", "utf-8", null),
                new XElement("test-data",
                             new XElement("blabla", 1),
                             new XElement("blabla", 2),
                             new XElement("blabla", 3),
                             new XElement("blabla", 4),
                             new XElement("blabla", 5))
                );
        }

        private bool CanReadAndWriteTestDoc(IStorageProvider storage, XDocument doc)
        {
            var key = storage.Store(doc);
            XDocument readDoc = storage.Read(key);
            Assert.IsTrue(XNode.DeepEquals(doc, readDoc));
            return true;
        }
    }
}