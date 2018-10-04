using Agridea.Service.Repository;
using System.Data.Entity;

namespace Agridea.DataRepository
{
    [DbConfigurationType(typeof(AgrideaCoreDbConfiguration))]
    public class SqlServerDropCreateDatabaseAlways<TSqlServerContext> : DropCreateDatabaseAlways<TSqlServerContext>
        where TSqlServerContext : SqlServerContextBase, new()
    {
        protected override void Seed(TSqlServerContext context)
        {
            context.AlterCollationForVarcharColumns("FRENCH_CI_AI");

            context.CreateIndexes(context.GetUniqueConstraints());
            context.CreateIndexes(context.GetForeignKeyIndexes());

            context.ExecuteSqlCommands();
        }
    }
}
