using System;
using System.Configuration;
using System.IO;
using System.Reflection;

namespace CalibreImport
{
    public static class CustomSettings
    {
        private static Configuration config;

        static CustomSettings()
        {
            string configFilePath;
            string appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string assemblyFolder = Path.GetDirectoryName(typeof(CalibreImport).Assembly.Location);
            string assemblyName = Assembly.GetExecutingAssembly().GetName().Name;
            string configFileName = $"{assemblyName}.config";
            string portableFilePath = Path.Combine(assemblyFolder, "portable");

            if (File.Exists(portableFilePath))
            {
                configFilePath = Path.Combine(assemblyFolder, configFileName);
                //Logger.LogThis("Portable mode detected. Using configuration file in the same folder as the DLL.", true);
            }
            else
            {
                configFilePath = Path.Combine(appDataFolder, ResourceStrings.NameAppRes, configFileName);
                //Logger.LogThis("Portable mode not detected. Using configuration file in AppData.", true);
            }

            ExeConfigurationFileMap configMap = new ExeConfigurationFileMap { ExeConfigFilename = configFilePath };

            if (!File.Exists(configFilePath))
            {
                CreateDefaultConfig(configFilePath);
            }

            config = ConfigurationManager.OpenMappedExeConfiguration(configMap, ConfigurationUserLevel.None);
        }

        public static Configuration Config => config;

        public static void Save()
        {
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
            config.AppSettings.Settings.Add("verboseLog", "False");
            config.AppSettings.Settings.Add("autoKillCalibre", "False");
            config.AppSettings.Settings.Add("autoMerge", "overwrite");
            config.AppSettings.Settings.Add("hiddenLibraries", "");
            config.AppSettings.Settings.Add("logName", "CalibreImport.log");
            config.AppSettings.Settings.Add("language", "en");
            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection(config.AppSettings.SectionInformation.Name);
        }
    }
}
