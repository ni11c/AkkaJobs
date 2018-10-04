using System.Threading.Tasks;
using System.Xml.Linq;

namespace Agridea.Prototypes.Akka.Common.Providers
{
    public interface IStorageProvider
    {
        string Store(XDocument doc);
        XDocument Read(string key);
    }

    public interface IStorageProviderAsync
    {
        Task<string> StoreAsync(XDocument doc);
        Task<XDocument> ReadAsync(string key);
    }
}