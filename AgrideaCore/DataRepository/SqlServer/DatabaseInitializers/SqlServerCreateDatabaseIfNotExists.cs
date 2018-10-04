using System.Data.Entity;
using System.Linq;

namespace Agridea.DataRepository
{
    public class SqlServerCreateDatabaseIfNotExists<TSqlServerContext> : CreateDatabaseIfNotExists<TSqlServerContext>
        where TSqlServerContext : SqlServerContextBase, new()
    {
        protected override void Seed(TSqlServerContext context)
        {
            context.AlterCollationForVarcharColumns("FRENCH_CI_AI");

            context.CreateIndexes(context.GetUniqueConstraints());
            context.CreateIndexes(context.GetForeignKeyIndexes());

            context.ExecuteSqlCommands();

            base.Seed(context);
        }
    }
}
