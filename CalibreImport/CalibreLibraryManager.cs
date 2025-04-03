using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace CalibreImport
{
    public static class CalibreLibraryManager
    {
        // This method retrieves the list of Calibre libraries from the "gui.json" file.
        // It parses it, and extracts the library paths and names.
        // If anyone knows a more appropriate source for the list of libraries, let me know.
        public static List<CalibreLibrary> GetLibraries()
        {
            try
            {
                var calibreConfigPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "calibre");
                var guiFilePath = Path.Combine(calibreConfigPath, "gui.json");

                // Can't do much with this app if the list of libraries can't be found.
                if (!File.Exists(guiFilePath))
                {
                    Logger.LogThis($"Calibre gui.json not found at {guiFilePath}. Cannot extract list of Libraries.");
                    return null;
                }

                // Read the JSON file and parse it
                var guiJson = JObject.Parse(File.ReadAllText(guiFilePath));
                var libraries = guiJson["library_usage_stats"]?
                    .ToObject<Dictionary<string, object>>()?.Keys
                    .Select(path => new CalibreLibrary { Path = path, Name = Path.GetFileName(path) })
                    .ToList();

                if (libraries == null || !libraries.Any())
                {
                    Logger.LogThis("No libraries found in Calibre GUI settings.", true);
                    return null;
                }

                return libraries;
            }
            catch (Exception ex)
            {
                Logger.LogThis($"Error retrieving Calibre libraries: {ex.Message}");
                return null;
            }
        }
    }
}
