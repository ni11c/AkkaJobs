using System.Linq.Dynamic;
using Agridea.Diagnostics.Contracts;
using Agridea.Diagnostics.Logging;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using EntityState = System.Data.Entity.EntityState;

namespace Agridea.DataRepository
{
    /// <summary>
    /// Further refactoring seems difficult:
    /// - Initialize is required by Reset(), hence creating the TSqlServerContext and passing it to the constructor is not usefull
    /// - TSqlServerContext is required by SetInitializer<TSqlServerContext>(new SqlServerCreateDatabaseIfNotExists<TSqlServerContext>())
    ///   though we could use reflection to invoke.
    /// </summary>
    /// <typeparam name="TSqlServerContext"></typeparam>
    public class SqlServerDataRepositoryBase<TSqlServerContext> :
        Disposable,
        IDataRepository
        where TSqlServerContext : SqlServerContextBase, new()
    {
        #region Members
        protected string connectionString_;

        protected readonly DbInitializationModes dbInitializationMode_;
        protected TSqlServerContext database_;
        #endregion Members

        #region Initialization

        public SqlServerDataRepositoryBase(string connectionString, DbInitializationModes dbInitializationMode, bool autoDetectChangesEnabled = true)
        {
            Requires<ArgumentException>.IsNotEmpty(connectionString);

            Log.Verbose(string.Format("'{0}:{1}' New(...)", GetType().Name, GetHashCode()));
            connectionString_ = connectionString;
            dbInitializationMode_ = dbInitializationMode;

            Initialize(autoDetectChangesEnabled);
        }

        #endregion Initialization

        #region Identity

        public override string ToString()
        {
            return string.Format("[{0} ConnectionString='{1}']",
                GetType().Name,
                connectionString_);
        }

        #endregion Identity

        #region IDisposable

        protected override void Dispose(bool disposing)
        {
            Log.Verbose(string.Format("'{0}:{1}' Dispose({2})", GetType().Name, GetHashCode(), disposing));

            if (disposing)
            {
                Database.Database.Connection.Close();
                Database.Dispose();
            }
        }

        #endregion IDisposable

        #region IDataRepository
        //public DbContextTransaction BeginTransation(IsolationLevel isolationLevel)
        //{
        //    return Database.Database.BeginTransaction(isolationLevel);
        //}
        public string ConnectionString { get { return connectionString_; } }

        public string DatabaseName { get { return database_.GetDataBaseName(); } }
        public IList<string> TableNames{ get { return database_.GetTableNames(); } }

        public long GetRowCount(string tableName)
        {
            return database_.GetRowCount(tableName);
        }

        public bool Exists
        {
            get { return Database.Database.Exists(); }
        }

        public int ExecuteSqlCommand(string command, bool ensureTransaction=true)
        {
            return Database.Database.ExecuteSqlCommand(ensureTransaction ? TransactionalBehavior.EnsureTransaction : TransactionalBehavior.DoNotEnsureTransaction, command);
        }

        public int ExecuteSqlCommand(string command, int timeout, bool ensureTransaction=true)
        {
            this.SetCommandTimeout(timeout);
            return Database.Database.ExecuteSqlCommand(ensureTransaction ? TransactionalBehavior.EnsureTransaction : TransactionalBehavior.DoNotEnsureTransaction, command);
        }

        public List<T> ExecuteSqlQuery<T>(string commandText)
        {
            return Database.Database.SqlQuery<T>(commandText).ToList();
        }

        public bool CompatibleWithModel(bool throwIfNoMetadata)
        {
            return Database.Database.CompatibleWithModel(throwIfNoMetadata);
        }

        public void Reset()
        {
            Dispose(true);
            Initialize();
        }

        public IDataRepository Save()
        {
            Database.SaveChanges();
            return this;
        }

        public void Close()
        {
            Database.SaveChanges();
            Database.Database.Connection.Close();
            database_ = null;
        }

        public void Delete()
        {
            Database.Database.ExecuteSqlCommand(
                TransactionalBehavior.DoNotEnsureTransaction,
                "ALTER DATABASE [" + Database.Database.Connection.Database + "] SET SINGLE_USER WITH ROLLBACK IMMEDIATE");
            Database.Database.Delete();
        }

        public void Detach<TItem>(TItem item) where TItem : class, IPocoBase
        {
            Database.Entry(item).State = EntityState.Detached;
        }
        public IQueryable<TItem> All<TItem>() where TItem : class, IPocoBase
        {
            return Database.Set<TItem>();
        }

        public IQueryable<IPocoBase> All(Type type)
        {
            return Database.Set(type).AsQueryable() as IQueryable<IPocoBase>;
        }        
        public IQueryable<IPocoBase> All(Type type, string predicate, params object[] values)
        {
            return Database.Set(type).Where(predicate, values).AsQueryable() as IQueryable<IPocoBase>;
        }

        public IQueryable<TItem> All<TItem>(Expression<Func<TItem, bool>> predicate) where TItem : class, IPocoBase
        {
            return Database.Set<TItem>().Where(predicate);
        }

        public TItem GetFirst<TItem>() where TItem : class, IPocoBase
        {
            return Database.Set<TItem>().First();
        }

        public TItem GetFirstOrDefault<TItem>() where TItem : class, IPocoBase
        {
            return Database.Set<TItem>().FirstOrDefault();
        }

        public bool Any<TItem>() where TItem : class, IPocoBase
        {
            return Database.Set<TItem>().Any();
        }

        public bool Any<TItem>(Expression<Func<TItem, bool>> predicate) where TItem : class, IPocoBase
        {
            return Database.Set<TItem>().Any(predicate);
        }

        public IDataRepository Add<TItem>(TItem item) where TItem : class, IPocoBase
        {
            Database.Set<TItem>().Add(item);
            return this;
        }

        public IDataRepository AddRange<TItem>(IEnumerable<TItem> items) where TItem : class, IPocoBase
        {
            foreach (var item in items)
                Add(item);
            return this;
        }

        public IDataRepository Modify<TItem>(TItem item) where TItem : class, IPocoBase
        {
            MarkEntityDirty(item);
            return this;
        }

        public IDataRepository Remove<TItem>(TItem item) where TItem : class, IPocoBase
        {
            Database.Set<TItem>().Remove(item);

            return this;
        }

        public IDataRepository RemoveItem(object item) 
        {
            Database.Set(item.GetType()).Remove(item);

            return this;
        }

        public IEnumerable<TItem> SqlQuery<TItem>(string rawSql, params object[] parameters)
        {
            return Database.Database.SqlQuery<TItem>(rawSql, parameters);
        }

        public void SetCommandTimeout(int seconds)
        {
            ((IObjectContextAdapter)database_).ObjectContext.CommandTimeout = seconds;
        }
        #endregion IRepository

        #region Helpers

        protected void Initialize(bool autoDetectChangesEnabled = true)
        {
            switch (dbInitializationMode_)
            {
                case DbInitializationModes.RecreateAlways:
                    System.Data.Entity.Database.SetInitializer<TSqlServerContext>(new SqlServerDropCreateDatabaseAlways<TSqlServerContext>());
                    break;

                case DbInitializationModes.LeaveUntouched:
                    System.Data.Entity.Database.SetInitializer<TSqlServerContext>(null);
                    break;

                default: // DbInitializationModes.CreateIfNotExists
                    System.Data.Entity.Database.SetInitializer<TSqlServerContext>(new SqlServerCreateDatabaseIfNotExists<TSqlServerContext>());
                    break;
            }

            //UNDERWORK change this reflection based creation
            database_ = Activator.CreateInstance(typeof(TSqlServerContext), new object[] { connectionString_ }) as TSqlServerContext;

            database_.Configuration.AutoDetectChangesEnabled = autoDetectChangesEnabled;
        }

        protected TSqlServerContext Database
        {
            get
            {
                Asserts<InvalidOperationException>.IsNotNull(database_);
                return database_;
            }
        }

        protected void MarkEntityDirty(object item)
        {
            //Database.Entry(item).State = EntityState.Modified;
            //do nothing
        }

        #endregion Helpers
    }

    public enum DbInitializationModes
    {
        CreateIfNotExists,
        RecreateAlways,
        LeaveUntouched
    }
}