using Microsoft.Identity.Client;

namespace Hrms.Provider
{
    public sealed class ConfigProvider
    {
        private const string ConfigFile = "appsettings.json";
        private ApplicationConfig AppConfig = new();
        private static readonly Lazy<ConfigProvider> _constantsProvider = new(() => new ConfigProvider());
        public static ConfigProvider Provider => _constantsProvider.Value;
        public static ApplicationSettings Settings => _constantsProvider.Value.AppConfig.AppSettings;

        private ConfigProvider()
        {
            if (ReadAppSettings())
            {
                CreateFolderStructure(MasterTenantName);
                PathProvider.CreateDirectory(Path.Combine(BaseDirectory, AppConfig.AppSettings.TempFolderName));
                PathProvider.CreateDirectory(Path.Combine(BaseDirectory, AppConfig.AppSettings.BrowserFolderName));
                PathProvider.CreateDirectory(Path.Combine(BaseDirectory, AppConfig.AppSettings.LogsFolderName));
            }
        }
        private bool ReadAppSettings()
        {
            bool isSuccess = false;
            if (!File.Exists(ConfigFile)) { throw new FileNotFoundException(); }
            else
            {
                using StreamReader streamReader = new(ConfigFile);
                string dataStream = streamReader.ReadToEnd();
                if (!string.IsNullOrWhiteSpace(dataStream))
                {
                    AppConfig = dataStream.FromJson<ApplicationConfig>();
                    AppConfig ??= new();
                    isSuccess = true;
                }
                else { throw new ArgumentNullException(); }
                streamReader.Dispose();
            }
            return isSuccess;
        }
        private void CreateFolderStructure(string tenantName)
        {
            PathProvider.CreateDirectory(Path.Combine(BaseDirectory, tenantName, AppConfig.AppSettings.HtmlFolderName));
            PathProvider.CreateDirectory(Path.Combine(BaseDirectory, tenantName, AppConfig.AppSettings.ImageFolderName));
            PathProvider.CreateDirectory(Path.Combine(BaseDirectory, tenantName, AppConfig.AppSettings.DocumentFolderName));
            PathProvider.CreateDirectory(Path.Combine(BaseDirectory, tenantName, AppConfig.AppSettings.JsonBackupFolderName));
            PathProvider.CreateDirectory(Path.Combine(BaseDirectory, tenantName, AppConfig.AppSettings.DatabaseBackupFolderName));
            PathProvider.CreateDirectory(Path.Combine(BaseDirectory, tenantName, AppConfig.AppSettings.PageConfigFolderName));
        }
        public void CreateTenentFolderStructure(string tenantName)
        {
            PathProvider.CreateDirectory(Path.Combine(BaseDirectory, tenantName));
            CreateFolderStructure(tenantName);
        }
        public static string EncryptionKey => "b14ca5898a4e4133bbce2ea2315a1916";
        public static string MasterTenantName => "Master";
        public string BaseDirectory => Path.Combine(@"C:/", AppConfig.AppSettings.BaseFolderName);
        public string LogsDirectory => Path.Combine(BaseDirectory, Settings.LogsFolderName);
        public string TempDirectory => Path.Combine(BaseDirectory, Settings.TempFolderName);
        public string BrowserDirectory => Path.Combine(BaseDirectory, Settings.BrowserFolderName);

        internal class ApplicationConfig { public ApplicationSettings AppSettings { get; set; } }
        public class ApplicationSettings
        {
            public string BaseFolderName { get; set; }
            public string TempFolderName { get; set; }
            public string LogsFolderName { get; set; }
            public string BrowserFolderName { get; set; }

            public string LightDataExtension { get; set; }
            public string MasterEncryption { get; set; }

            public string ImageFolderName { get; set; }
            public string HtmlFolderName { get; set; }
            public string DocumentFolderName { get; set; }
            public string DatabaseBackupFolderName { get; set; }

            public string JsonBackupFolderName { get; set; }
            public string PageConfigFolderName { get; set; }
        }
    }
}

