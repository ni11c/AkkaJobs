using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Agridea.Acorda.Agis;
using Agridea.Prototypes.Akka.Common.Providers;

namespace CommonUnitTests
{
    /// <summary>
    /// To simulate asynchrony, can use Task.FromResult (immediate), await Task.Delay(timespan) to force real wait, or Task.Yield() to force aync without real delay.
    /// See http://stackoverflow.com/questions/32008662/re-usable-c-sharp-test-code-that-waits-for-io
    /// </summary>
    public class TestDataProvider: IDataProvider
    {
        public Task<StructureModel> GetStructureData(int farmId)
        {
            return Task.FromResult(new StructureModel());
        }

        public Task<int[]> GetAllFarmIdsForAgis()
        {
            return Task.FromResult(new[] {1});
        }
    }
}
