using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace CalibreImport
{
    public static class CalibreLibraryManager
    {
        /// <summary>
        /// Retrieves the list of Calibre libraries from gui.json.
        /// </summary>
        /// <param name="logger">
        /// Optional logger action to write log messages (e.g., (message, color) => Logger.LogThis(message, color)).
        /// </param>
        /// <returns>List of CalibreLibrary objects or null on error.</returns>
        public static List<CalibreLibrary> GetLibraries()
        {
            try
            {
                var calibreConfigPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "calibre");
                var guiFilePath = Path.Combine(calibreConfigPath, "gui.json");

                if (!File.Exists(guiFilePath))
                {
                    Logger.LogThis($"Calibre gui.json not found at {guiFilePath}. Cannot extract list of Libraries.");
                    return null;
                }

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
