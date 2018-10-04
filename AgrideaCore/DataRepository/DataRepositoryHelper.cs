using System;
using System.Collections.Concurrent;
using System.Text.RegularExpressions;

namespace Agridea.DataRepository
{
    using Agridea.Diagnostics.Logging;
    using Microsoft.SqlServer.Management.Common;
    using System.Configuration;
    using System.Data.SqlClient;

    public static class DataRepositoryHelper
    {
        #region Members
        private static readonly ConcurrentDictionary<string, string> connectionString_ = new ConcurrentDictionary<string, string>();
        #endregion

        #region Services
        public static string SqlServerExpressUniqueConnectionString(string databaseName)
        {
            return SqlServerExpressConnectionString(UniqueNameFor(databaseName));
        }
        public static string SqlServerUniqueConnectionString(string databaseName)
        {
            return SqlServerConnectionString(UniqueNameFor(databaseName));
        }
        public static string SqlServerExpressConnectionString(string databaseName)
        {
            return string.Format(@"Server=.\SQLEXPRESS;Database={0};Trusted_Connection=True;", databaseName);
        }
        public static string SqlServerConnectionStringFromConfig(string connectionStringName)
        {
            return connectionString_.GetOrAdd(connectionStringName, key => ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString);
        }
        public static string SqlServerConnectionString(string databaseName)
        {
            return string.Format(@"Server=.;Database={0};Trusted_Connection=True;", databaseName);
        }
        public static string RemoteSqlServerConnectionString(string server, string username, string password, string databaseName)
        {
            return string.Format("Server={0};Database={1};User Id={2};Password={3};", server, databaseName, username, password);
        }
        public static string UniqueNameFor(string name)
        {
            return string.Format("{0}_{1}", name, Guid.NewGuid());
        }
        public static string ServerNameFor(string connectionString)
        {
            var regex = @".*\Server=(?<SERVER>[^\;]+)\;.*";
            var match = Regex.Match(connectionString, regex);
            return match.Success ? match.Groups["SERVER"].Value : null;

        }
        public static string DatabaseNameFor(string connectionString)
        {
            var regex = @".*\Database=(?<DATABASE>[^\;]+)\;.*";
            var match = Regex.Match(connectionString, regex);
            return match.Success ? match.Groups["DATABASE"].Value : null;
        }

        public static string GetActualDatabaseName(string databaseName, string cantonCode)
        {
            string databaseNameSuffix = cantonCode == null ? "" : "_" + cantonCode;
            return string.Format("{0}{1}", databaseName, databaseNameSuffix);
        }
        public static void DropDatabase(string connectionName)
        {
            using (
                var sqlConnection =
                    new SqlConnection(
                        ConfigurationManager.ConnectionStrings[connectionName]
                        .ConnectionString))
            {
                var serverConnection = new ServerConnection(sqlConnection);
                var server = new Microsoft.SqlServer.Management.Smo.Server(
                                 serverConnection);
                if (server.Databases[sqlConnection.Database] != null)
                    server.KillDatabase(sqlConnection.Database);
            }
        }

        #endregion Services
    }
}