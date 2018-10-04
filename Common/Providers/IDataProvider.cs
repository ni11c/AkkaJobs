using System.Threading.Tasks;
using Agridea.Acorda.Agis;

namespace Agridea.Prototypes.Akka.Common.Providers
{
    public interface IDataProvider
    {
        Task<StructureModel> GetStructureData(int farmId);
        Task<int[]> GetAllFarmIdsForAgis();
    }
}