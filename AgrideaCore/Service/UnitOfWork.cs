using System;
using Agridea.DataRepository;
using Agridea.Diagnostics.Contracts;

namespace Agridea
{
    public static class UnitOfWork
    {
        #region Services
        public static IAgrideaService GetService(string databaseName, DbInitializationModes contextInitialization = DbInitializationModes.CreateIfNotExists)
        {
            Requires<ArgumentException>.IsNotEmpty(databaseName);
            return new AgrideaService(
                GetRepository(
                    DataRepositoryHelper.SqlServerConnectionString(databaseName),
                    contextInitialization));
        }

        public static IAgrideaService GetUniqueTestService(string databaseName, DbInitializationModes contextInitialization = DbInitializationModes.CreateIfNotExists)
        {
            Requires<ArgumentException>.IsNotEmpty(databaseName);
            return new AgrideaService(
                GetRepository(
                    DataRepositoryHelper.SqlServerUniqueConnectionString(databaseName),
                    contextInitialization));
        }
        #endregion

        #region Helpers
        private static IDataRepository GetRepository(string connectionString, DbInitializationModes contextInitialization)
        {
            return new AgrideaCoreDataRepositoryFactory().CreateRepository(
                connectionString,
                contextInitialization);
        }
        #endregion
    }
}
