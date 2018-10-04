using Agridea.DataRepository;
using Agridea.Diagnostics.Logging;
using System;
using System.Configuration;
using System.IO;
using System.Text.RegularExpressions;
using System.Web;

namespace Agridea.Configuration
{
    /// <summary>
    /// See http://www.4guysfromrolla.com/articles/032807-1.aspx#postadlink
    /// And http://msdn.microsoft.com/en-us/library/2tw134k3.aspx
    /// </summary>
    public class AgrideaConfiguration : ConfigurationSection
    {
        public const string Code = "code";
        public const string None = "none";
        
        public static AgrideaConfiguration Configuration { get { return ConfigurationManager.GetSection("AgrideaConfiguration") as AgrideaConfiguration; } }

        [ConfigurationProperty("version", IsRequired = false, DefaultValue = "1.0.0.0")]
        public string Version { get { return this["version"] as string; } }

        [ConfigurationProperty("Error", IsRequired = true)]
        public Error Error { get { return this["Error"] as Error; } }

        public string GetFilePath(string filePath, bool versionned = true)
        {
            string actualFilePath = versionned ? GetVersionnedFilePath(filePath) : filePath;
            string physicalPathToFile = GetPhysicalPath(actualFilePath);
            string directory = Path.GetDirectoryName(physicalPathToFile);
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            return physicalPathToFile;
        }
        private string GetVersionnedFilePath(string filePath)
        {
            return Path.Combine(
                Path.GetDirectoryName(filePath),
                Path.GetFileNameWithoutExtension(filePath) + ProductInfo.Version) +
                Path.GetExtension(filePath);
        }
        private string GetPhysicalPath(string path)
        {
            if (Regex.Match(path, @"\A[a-zA-Z]{1}:\\.*").Success)
                return path;

            return HttpContext.Current != null ? HttpContext.Current.Server.MapPath(path) : path;
        }

        [ConfigurationProperty("Application", IsRequired = false)]
        public Application Application { get { return this["Application"] as Application; } }

        [ConfigurationProperty("Database", IsRequired = true)]
        public Database Database { get { return this["Database"] as Database; } }

        [ConfigurationProperty("Performance", IsRequired = false)]
        public Performance Performance { get { return this["Performance"] as Performance; } }

        [ConfigurationProperty("BatchQueue", IsRequired = false)]
        public BatchQueue BatchQueue { get { return this["BatchQueue"] as BatchQueue; } }
    }
    public class Error : ConfigurationElement
    {
        [ConfigurationProperty("enableBugReport", IsRequired = false, DefaultValue = "true")]
        public bool EnableBugReport { get { return Convert.ToBoolean(this["enableBugReport"]); } }
    }
    public class Application : ConfigurationElement
    {
        [ConfigurationProperty("product", IsRequired = true)]
        public string ProductName { get { return this["product"] as string; } }

        [ConfigurationProperty("name", IsRequired = true)]
        public string Name { get { return this["name"] as string; } }
    }
    public class Database : ConfigurationElement
    {
        [ConfigurationProperty("name", IsRequired = false, DefaultValue = "Agridea")]
        public string Name { get { return this["name"] as string; } }
    }
    public class Performance : ConfigurationElement
    {
        [ConfigurationProperty("on", IsRequired = false, DefaultValue = "false")]
        public bool On { get { return Convert.ToBoolean(this["on"]); } }
        [ConfigurationProperty("optim", IsRequired = false, DefaultValue = "false")]
        public bool Optim { get { return Convert.ToBoolean(this["optim"]); } }
    }
    public class BatchQueue : ConfigurationElement
    {
        [ConfigurationProperty("schedulingInterval", IsRequired = false, DefaultValue = "60000")]
        public int SchedulingInterval { get { return Convert.ToInt32(this["schedulingInterval"]); } }
    }
}
