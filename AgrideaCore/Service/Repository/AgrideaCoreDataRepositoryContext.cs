using System.Collections.Generic;
using System.Data.Entity;
using Agridea.Calendar;
using Agridea.DataRepository;
using Agridea.News;
using Agridea.Metadata;
using Agridea.Security;

namespace Agridea
{
    public class AgrideaCoreDataRepositoryContext : SqlServerContextBase
    {
        #region Initialization
        public AgrideaCoreDataRepositoryContext()
            : base()
        {
        }
        public AgrideaCoreDataRepositoryContext(string connectionString)
            : base(connectionString)
        {
        }
        #endregion

        #region SqlServerContextBase
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            MetadataDomainModelContext.Configure(this, modelBuilder);
            CalendarDomainModelContext.Configure(this, modelBuilder);
            NewsDomainModelContext.Configure(this, modelBuilder);
            SecurityDomainModelContext.Configure(this, modelBuilder);
            Configure(modelBuilder);
        }
        public override IList<UniqueIndex> GetUniqueConstraints()
        {
            var uniqueConstraints = base.GetUniqueConstraints();
            MetadataDomainModelContext.AddUniqueConstraints(uniqueConstraints, this);
            CalendarDomainModelContext.AddUniqueConstraints(uniqueConstraints, this);
            NewsDomainModelContext.AddUniqueConstraints(uniqueConstraints, this);
            SecurityDomainModelContext.AddUniqueConstraints(uniqueConstraints, this);
            return uniqueConstraints;
        }
        public override IList<NonUniqueIndex> GetForeignKeyIndexes()
        {
            var foreignKeyIndexes = base.GetForeignKeyIndexes();
            MetadataDomainModelContext.AddForeignKeyIndexes(foreignKeyIndexes, this);
            CalendarDomainModelContext.AddForeignKeyIndexes(foreignKeyIndexes, this);
            NewsDomainModelContext.AddForeignKeyIndexes(foreignKeyIndexes, this);
            SecurityDomainModelContext.AddForeignKeyIndexes(foreignKeyIndexes, this);
            return foreignKeyIndexes;
        }
        #endregion

        #region Helpers
        private void Configure(DbModelBuilder modelBuilder)
        {
        }
        #endregion
    }
}
