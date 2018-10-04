using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agridea.Service.Repository
{
    public class AgrideaCoreDbConfiguration : DbConfiguration
    {
        public AgrideaCoreDbConfiguration()
        {
            SetProviderServices(
                SqlProviderServices.ProviderInvariantName,
                SqlProviderServices.Instance);
        }
    }
}
