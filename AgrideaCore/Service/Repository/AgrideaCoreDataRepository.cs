using Agridea.DataRepository;

namespace Agridea
{
    public class AgrideaCoreDataRepository<TSqlServerContext> :
        SqlServerDataRepositoryBase<TSqlServerContext>,
        IDataRepository
        where TSqlServerContext : AgrideaCoreDataRepositoryContext, new()
    {
        #region Initialization
        public AgrideaCoreDataRepository(string connectionString, DbInitializationModes dbInitializationMode, bool autoDetectChangesEnabled = true)
            : base(connectionString, dbInitializationMode, autoDetectChangesEnabled)
        {
        }
        #endregion
    }
}
