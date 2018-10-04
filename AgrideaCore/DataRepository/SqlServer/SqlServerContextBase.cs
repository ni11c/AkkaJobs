using System.Configuration;
using System.Data;
using Agridea.Diagnostics.Logging;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Smo;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Web.UI.WebControls;
using Microsoft.SqlServer.Management.Smo.Agent;
using Table = Microsoft.SqlServer.Management.Smo.Table;
using View = Microsoft.SqlServer.Management.Smo.View;

namespace Agridea.DataRepository
{
    /// <summary>
    /// Check this http://blogs.msdn.com/b/adonet/archive/2010/12/14/ef-feature-ctp5-fluent-api-samples.aspx
    /// </summary>
    public abstract class SqlServerContextBase : DbContext
    {
        #region Initialization

        public SqlServerContextBase()
            : base()
        {
            //Requires<InvalidOperationException>.Fails("Should not execute this constructor but the one with connectionString as param");
        }

        public SqlServerContextBase(string connectionString)
            : base(connectionString)
        {
            Log.Verbose(string.Format("'{0}:{1}' New(...)", GetType().Name, GetHashCode()));
            Configuration.AutoDetectChangesEnabled = true;
            Configuration.LazyLoadingEnabled = true;
            Configuration.ValidateOnSaveEnabled = true;

            //Profiling of the DB usage (Sql query sent + time spent), see AgrideaCoreDbConfiguration : DbConfiguration
            Database.Log = Log.Verbose;
        }

        #endregion Initialization

        #region DbContext

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //This should not be changed, since it works in conjunction with EDMX and TT code generators
            //Diagrams
            //- indicate multiplicity which is 0..1, 1, *
            //- specify cascading (false by default)
            //Code generators generated explicitely Required / Optional for all multiplicity
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
            //modelBuilder.Conventions.Add<ManyToManyCascadeDeleteConvention>(); //From EF 4.1? onward only
        }

        #endregion DbContext

        #region Disposable

        protected override void Dispose(bool disposing)
        {
            Log.Verbose(string.Format("'{0}:{1}' Dispose({2})", GetType().Name, GetHashCode(), disposing));
            base.Dispose(disposing);
        }

        #endregion Disposable

        #region Services

        public virtual IList<NonUniqueIndex> GetForeignKeyIndexes()
        {
            return new List<NonUniqueIndex>();
        }

        public virtual IList<UniqueIndex> GetUniqueConstraints()
        {
            return new List<UniqueIndex>();
        }

        public virtual void ExecuteSqlCommands()
        {
        }

        public string GetDataBaseName()
        {
            return Database.Connection.Database;
        }    
        
        public IList<string> GetTableNames()
        {
            var tableNames = new List<string>();
            for (int index = 0; index < SmoDatabase.Tables.Count; index++) tableNames.Add(SmoDatabase.Tables[index].Name);
            return tableNames;
        }

        public long GetRowCount(string tableName)
        {
            return SmoDatabase.Tables[tableName].RowCount;
        }

        public IEnumerable<string> RemoveUniqueConstraints()
        {
            StringCollection scripts = null;
            foreach (var table in SmoDatabase.Tables.Cast<Table>())
            {
                var uniqueConstraints = table.Indexes.Cast<Index>().Where(m => m.IndexKeyType == IndexKeyType.DriUniqueKey);
                foreach (var constraint in uniqueConstraints)
                {
                    scripts = constraint.Script();
                    table.Indexes.Remove(constraint);
                    table.Alter();
                }
            }

            return scripts == null ? new string[] { } : scripts.Cast<string>();
        }

        public IList<UniqueIndex> GetExistingUniqueConstraints()
        {
            var result = new List<UniqueIndex>();
            foreach (var table in SmoDatabase.Tables.Cast<Table>())
            {
                result.AddRange(table.Indexes
                                     .Cast<Index>()
                                     .Where(m => (m.IndexKeyType == IndexKeyType.DriUniqueKey || m.IsUnique) && m.IndexKeyType != IndexKeyType.DriPrimaryKey)
                                     .Select(constraint => new UniqueIndex
                                     {
                                         TableName = table.Name,
                                         ColumnNames = constraint.IndexedColumns.Cast<IndexedColumn>().Select(m => m.Name).ToList()
                                     }));
            }
            return result;
        }

        public UniqueIndex GetUniqueIndexFor<T>(params string[] columns)
        {
            return new UniqueIndex
            {
                TableName = CodeGenerationHelper.GetTableName<T>(),
                ColumnNames = columns.ToList()
            };
        }

        public NonUniqueIndex GetNonUniqueIndexFor<T>(params string[] columns)
        {
            return new NonUniqueIndex
            {
                TableName = CodeGenerationHelper.GetTableName<T>(),
                ColumnNames = columns.ToList()
            };
        }

        public void CreateIndexes(IEnumerable<DbIndex> dbIndexList)
        {
            foreach (var dbIndex in dbIndexList)
            {
                var table = SmoDatabase.Tables[dbIndex.TableName];
                var index = new Index(table, dbIndex.ToString())
                {
                    IndexKeyType = dbIndex.IndexKeyType
                };
                foreach (var column in dbIndex.ColumnNames)
                    index.IndexedColumns.Add(new IndexedColumn(index, column));

                Log.Info("Adding {0} index to table {1} with columns {2}",
                    dbIndex.IndexKeyType == IndexKeyType.DriUniqueKey
                    ? "Unique"
                    : "NonUnique",
                    dbIndex.TableName,
                    string.Join(",", dbIndex.ColumnNames));

                index.Create();
            }
        }

        public void DropIndexes(IEnumerable<DbIndex> dbIndexList)
        {
            foreach (var dbIndex in dbIndexList)
            {
                var table = SmoDatabase.Tables[dbIndex.TableName];
                var index = table.Indexes[dbIndex.ToString()];
                Log.Info("Droping {0} index from table {1} with columns {2}",
                         dbIndex.IndexKeyType == IndexKeyType.DriUniqueKey
                             ? "Unique"
                             : "NonUnique",
                         dbIndex.TableName,
                         string.Join(",", dbIndex.ColumnNames));
                if (index != null)
                    index.Drop();
            }
        }

        public void DropColumn<T, TValue>(string columnName)
        {
            var table = SmoTableFor<T>();
            if (table.Columns.Cast<Column>().Any(x => x.Name == columnName))
            {
                table.Columns[columnName].Drop();
                table.Alter();
            }
        }

        public void DropColumn<T, TValue>(Expression<Func<T, TValue>> columnNameExpression)
        {
            var table = SmoTableFor<T>();
            var columnName = (columnNameExpression.Body as MemberExpression).Member.Name;
            if (table.Columns.Cast<Column>().Any(x => x.Name == columnName))
            {
                table.Columns[columnName].Drop();
                table.Alter();
            }
        }

        //public void CreateSequenceIfNotExists(string name, int start, int increment)
        //{
        //    var sequence = SmoDatabase.Sequences.Cast<Sequence>().FirstOrDefault(m => m.Name == name);
        //    if (sequence != null) return;
        //    sequence = new Sequence(SmoDatabase, name)
        //    {
        //        StartValue = start,
        //        IncrementValue = increment
        //    };
        //    sequence.Create();
        //}

        public void AddComputedColumn<T, TValue>(Expression<Func<T, TValue>> columnNameExpression, DataType dataType, string computedValue, bool isPersisted = false)
        {
            var table = SmoTableFor<T>();
            var columnName = (columnNameExpression.Body as MemberExpression).Member.Name;
            var column = new Column(table, columnName, dataType);
            column.Computed = true;
            column.IsPersisted = isPersisted;
            column.ComputedText = computedValue;
            table.Columns.Add(column);
            table.Alter();
        }

        public void AddOrUpdateComputedColumn<T, TValue>(Expression<Func<T, TValue>> columnNameExpression, DataType dataType, string computedValue, bool isPersisted = false)
        {
            var table = SmoTableFor<T>();
            var columnName = (columnNameExpression.Body as MemberExpression).Member.Name;
            Log.Info(string.Format("  Create computed column {0}.{1}", table, columnName));
            DropColumn(columnNameExpression);
            AddComputedColumn(columnNameExpression, dataType, computedValue, isPersisted);
        }

        public void DropDefaultConstraint<T, TColumn>(Expression<Func<T, TColumn>> columnNameExpression, string constraintNameToKeep = null )
        {
            var table = SmoTableFor<T>();
            var columnName = (columnNameExpression.Body as MemberExpression).Member.Name;
            if (table.Columns.Cast<Column>().Any(x => x.Name == columnName))
            {
                var defaultConstraint = table.Columns[columnName].DefaultConstraint;
                if (defaultConstraint != null && (constraintNameToKeep == null || defaultConstraint.Name != constraintNameToKeep))
                    defaultConstraint.Drop();
            }
        }

        public void DropView(string name)
        {
            var view = SmoDatabase.Views[name];
            if (view != null)
                view.Drop();
        }

        public void DropTable(string name)
        {
            var table = SmoDatabase.Tables[name];
            if (table != null)
                table.Drop();
        }

        public void DropViewOrTable(string name)
        {
            DropView(name);
            DropTable(name);
        }

        public void CreateOrAlterView(string name, string body, bool isSchemaBound = true)
        {
            DropTable(name);
            var view = SmoDatabase.Views[name];
            var exists = view != null;
            if (!exists)
                view = new View(SmoDatabase, name);

            var createOrAlter = exists
                ? "ALTER"
                : "CREATE";
            view.TextHeader = string.Format("{0} VIEW {1}  AS", createOrAlter, name);
            view.TextBody = body;

            if (!exists)
                view.Create();
            else
                view.Alter();

        }

        public void DropUdfOrSp(string name)
        {
            var function = SmoDatabase.UserDefinedFunctions.Cast<UserDefinedFunction>().FirstOrDefault(m => m.Name.ToLower() == name.ToLower());
            var storedProcedure = SmoDatabase.StoredProcedures.Cast<StoredProcedure>().FirstOrDefault(m => m.Name.ToLower() == name.ToLower());
            if (function != null)
                function.Drop();
            if (storedProcedure != null)
                storedProcedure.Drop();
        }


        public void CreateUdfFunction(string name, UserDefinedFunctionType functionType, DataType dataType, string body, params DbParameter[] parameters)
        {
            var function = new UserDefinedFunction(SmoDatabase, name);
            function.TextMode = false;
            function.IsSchemaBound = true;
            function.FunctionType = functionType;
            foreach (var parameter in parameters)
            {
                var fParameter = new UserDefinedFunctionParameter(function, parameter.Name, parameter.DataType);
                function.Parameters.Add(fParameter);
            }
            function.DataType = dataType;

            function.TextBody = body;
            function.Create();
        }

        public void CreateStoredProcedure(string name, string body, params DbParameter[] parameters)
        {
            var sp = new StoredProcedure(SmoDatabase, name);
            sp.TextMode = false;
            foreach (var parameter in parameters)
            {
                var spParameter = new StoredProcedureParameter(sp, parameter.Name, parameter.DataType);
                sp.Parameters.Add(spParameter);
            }
            sp.TextMode = false;
            sp.TextBody = body;
            sp.Create();
        }

        public void CreateOrReplaceSynonym(string synonymName, string srcDbName, string sourceName)
        {
            var synonym = SmoDatabase.Synonyms[synonymName];
            if (synonym != null)
                synonym.Drop();
            synonym = new Synonym(SmoDatabase, synonymName)
            {
                BaseDatabase = srcDbName,
                BaseSchema = "dbo",
                BaseObject = sourceName
            };
            synonym.Create();
        }

        public void CreateOrReplaceTrigger<T>(string triggerName, string body, TriggerParameters[]parameters)
        {
            var table =SmoTableFor<T>();
            var isNew = false;
            var trigger = table.Triggers[triggerName];
            if (trigger == null)
            {
                trigger = new Trigger(table, triggerName);
                isNew = true;

            }
            trigger.TextMode = false;
            var insertParameters = parameters.FirstOrDefault(m => m.TriggerType == TriggerType.Insert);
            var updateParameters =  parameters.FirstOrDefault(m => m.TriggerType == TriggerType.Update);
            var deleteParameters = parameters.FirstOrDefault(m => m.TriggerType == TriggerType.Insert);
            if (insertParameters != null)
            {
                trigger.Insert = true;
                trigger.InsertOrder = insertParameters.ActivationOrder;
            }
            if (updateParameters != null)
            {
                trigger.Update = true;
                trigger.UpdateOrder = updateParameters.ActivationOrder;
            }
            if (deleteParameters != null)
            {
                trigger.Delete = true;
                trigger.DeleteOrder = deleteParameters.ActivationOrder;
            }

            
            trigger.TextBody = body;
            if (!isNew)
                trigger.Alter();
            else
                trigger.Create();
            

        }

        public void TestExistingStoredProcedures()
        {
            foreach (var storedProcedure in SmoDatabase.StoredProcedures.Cast<StoredProcedure>().Where(m => m.Schema == "dbo"))
                storedProcedure.Alter();
        }

        public void TestExistingViews()
        {
            foreach (var view in SmoDatabase.Views.Cast<View>().Where(m => m.Schema == "dbo"))
                view.Alter();
        }

        public void AlterCollationForVarcharColumns(string collation)
        {
            var columns = SmoDatabase.Tables
                                     .Cast<Table>().Where(x => x.Name != "__MigrationHistory")
                                     .SelectMany(table => table.Columns
                                                               .Cast<Column>()
                                                               .Where(col => !col.Computed && col.Collation.ToLower() != collation.ToLower() &&
                                                                             (col.DataType.SqlDataType == SqlDataType.NVarChar
                                                                              || col.DataType.SqlDataType == SqlDataType.NVarCharMax)));

            foreach (var col in columns)
            {
                var dbIndexes = GetUniqueIndexFor(col);
                DropIndexes(dbIndexes);
                col.Collation = collation;
                col.Alter();
                CreateIndexes(dbIndexes);
            }
        }

        private static IList<UniqueIndex> GetUniqueIndexFor(Column column)
        {
            var table = column.Parent as Table;
            var indexes = table.Indexes.Cast<Index>()
                .Where(m => m.IndexedColumns.Cast<IndexedColumn>()
                .Select(x => x.Name).Contains(column.Name))
                .ToList();

            return indexes.Select(idx => new UniqueIndex
            {
                TableName = table.Name,
                ColumnNames = idx.IndexedColumns.Cast<IndexedColumn>().Select(ic => ic.Name).ToList()
            }).ToList();
        }

        public IList<string> GetColumnNamesFor<T>()
        {
            var table = SmoTableFor<T>();
            return table.Columns.Cast<Column>().Select(c => "[" + c.Name + "]").OrderBy(n => n).ToList();
        }

        public IList<string> GetColumnNamesFor<T>(string prefixToRemoveForOrdering)
        {
            var table = SmoTableFor<T>();
            return table.Columns.Cast<Column>().Select(c => "[" + c.Name + "]").OrderBy(n => n.Replace(prefixToRemoveForOrdering, string.Empty)).ToList();
        }

        public IList<string> GetColumnNamesWithoutIdFor<T>()
        {
            return GetColumnNamesFor<T>().Where(x => x != "[Id]").ToList();
        }

        public IList<string> GetColumnNamesWithoutIdFor<T>(string prefixToRemoveForOrdering)
        {
            return GetColumnNamesFor<T>(prefixToRemoveForOrdering).Where(x => x != "[Id]").ToList();
        }

        #endregion Services

        #region Helpers

        private Table SmoTableFor<T>()
        {
            return SmoDatabase.Tables[CodeGenerationHelper.GetTableName<T>()];
        }

        private Synonym SmoSynonymFor(string synonymName)
        {
            return SmoDatabase.Synonyms[synonymName];
        }

        private Microsoft.SqlServer.Management.Smo.Database SmoDatabase
        {
            get
            {
                ServerConnection connection = null;
                try
                {
                    Server server = new Server(connection = new ServerConnection(new SqlConnection(Database.Connection.ConnectionString)));
                    return server.Databases[Database.Connection.Database];
                }
                finally
                {
                    if (connection != null)
                        connection.Disconnect();
                }
            }
        }

        #endregion Helpers
    }
}