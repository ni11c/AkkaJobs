using Agridea.DataRepository;

namespace Agridea
{
    public class AgrideaCoreDataRepositoryFactory : IDataRepositoryFactory
    {
        public IDataRepository CreateRepository(string databaseConnectionString, DbInitializationModes contextInitialization, bool autoDetectChangesEnabled = true)
        {
            return new AgrideaCoreDataRepository<AgrideaCoreDataRepositoryContext>(databaseConnectionString, contextInitialization);
        }
    }
}
