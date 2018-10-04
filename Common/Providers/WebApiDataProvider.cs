using System;
using System.Threading.Tasks;
using Agridea.Acorda.Agis;

namespace Agridea.Prototypes.Akka.Common.Providers
{
    public class WebApiDataProvider : IDataProvider
    {
        public Task<StructureModel> GetStructureData(int farmId)
        {
            return Task.FromResult(new StructureModel());
        }

        public Task<int[]> GetAllFarmIdsForAgis()
        {
            return Task.FromResult(new[] { 1 });
        }
    }
}