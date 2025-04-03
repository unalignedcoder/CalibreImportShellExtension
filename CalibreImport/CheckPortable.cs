using System;
using System.IO;
using System.Reflection;

namespace CalibreImport
{
    public static class CheckPortable
    {
        // Check if the application is running in portable mode
        // It is not really "portable", though, as registry entries are still needed.
        // It just checks if the config file is in the same directory as the assembly.
        public static bool IsPortable()
        {
            string assemblyName = Assembly.GetExecutingAssembly().GetName().Name;
            string configFileName = $"{assemblyName}.config";

            string appDataConfigPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), assemblyName, configFileName);
            string assemblyConfigPath = Path.Combine(Path.GetDirectoryName(typeof(CheckPortable).Assembly.Location), configFileName);

            // Check if the config file is in the same directory as the assembly
            return File.Exists(assemblyConfigPath) && !File.Exists(appDataConfigPath);
        }
    }
}
