using System;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Threading;

namespace CalibreImport
{
    public static class CustomSettings
    {
        private static Configuration config;

        public static readonly string assemblyName;

        static CustomSettings()
        {
            // Load AssemblyName
            assemblyName = Assembly.GetExecutingAssembly().GetName().Name;

            string ConfigName = $"{assemblyName}.config";
            string ConfDirectory;

            // Check if the application is running in a portable mode
            if (CheckPortable.IsPortable())
            {
                ConfDirectory = Path.GetDirectoryName(typeof(CustomSettings).Assembly.Location);
            }
            else
            {
                ConfDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), assemblyName);
            }

            // Ensure the directory exists, if not create it
            if (!Directory.Exists(ConfDirectory))
            {
                Directory.CreateDirectory(ConfDirectory);
            }

            string configFilePath = Path.Combine(ConfDirectory, ConfigName);

            // Check if the config file exists, if not create a default one
            if (!File.Exists(configFilePath))
            {
                CreateDefaultConfig(configFilePath);
            }

            var configMap = new ExeConfigurationFileMap { ExeConfigFilename = configFilePath };
            config = ConfigurationManager.OpenMappedExeConfiguration(configMap, ConfigurationUserLevel.None);
        }

        public static Configuration Config => config;

        public static void Save()
        {
            // Save the configuration settings
            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection(config.AppSettings.SectionInformation.Name);
        }

        // Create a default configuration file
        private static void CreateDefaultConfig(string configFilePath)
        {
            var config = ConfigurationManager.OpenMappedExeConfiguration(new ExeConfigurationFileMap { ExeConfigFilename = configFilePath }, ConfigurationUserLevel.None);
            config.AppSettings.Settings.Add("calibreFolder", @"C:\Program Files\Calibre2");
            config.AppSettings.Settings.Add("useSubmenu", "True");
            config.AppSettings.Settings.Add("logThis", "True");
            config.AppSettings.Settings.Add("verboseLog", "True");
            config.AppSettings.Settings.Add("autoKillCalibre", "False");
            config.AppSettings.Settings.Add("autoMerge", "overwrite");
            config.AppSettings.Settings.Add("hiddenLibraries", "");
            config.AppSettings.Settings.Add("language", "");
            config.AppSettings.Settings.Add("skipSuccessMessage", "False");
            config.AppSettings.Settings.Add("autoCalibreOpen", "False");
            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection(config.AppSettings.SectionInformation.Name);
        }
    }
}
