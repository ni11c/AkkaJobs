namespace Agridea.DataRepository
{
    public interface IDataRepositoryFactory
    {
        IDataRepository CreateRepository(string databaseConnectionString, DbInitializationModes contextInitialization, bool autoDetectChangesEnabled = true);
    }
}
