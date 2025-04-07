using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CalibreImport
{
    // Centralizes the definitions of file types supported by the Calibre Import extension.
    public static class SupportedFileTypes
    {
        // Registry path for Calibre file associations
        private const string CalibreRegistryPath = @"SOFTWARE\calibre\calibre64bit\Capabilities\FileAssociations";

        // The complete list of file extensions supported by Calibre.
        // Used as fallback when registry lookup fails.
        public static readonly string[] DefaultExtensions = new[]
        {
            ".epub", ".pdf", ".mobi", ".azw", ".azw3", ".fb2", ".djvu",
            ".lrf", ".rtf", ".txt", ".doc", ".docx", ".odt", ".htm", ".html",
            ".cbz", ".cbr", ".pdb", ".snb", ".tcr", ".zip", ".rar"
        };

        // Gets the extensions from the registry if available, otherwise returns the default list
        public static List<string> GetExtensions()
        {
            var extensions = new List<string>();

            try
            {
                using (var key = Registry.LocalMachine.OpenSubKey(CalibreRegistryPath))
                {
                    if (key != null)
                    {
                        // Get extensions from registry and convert to lowercase for consistency
                        extensions = key.GetValueNames()
                            .Select(ext => ext.ToLower())
                            .ToList();

                        if (extensions.Count > 0)
                            return extensions;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogThis($"Error reading Calibre extensions from registry: {ex.Message}");
                // Continue to fallback
            }

            // Fallback to our default extensions if registry access failed
            return GetDefaultExtensionsList();
        }

        // Gets the extensions from the registry if available, otherwise returns the default list
        // Returns them as a HashSet for efficient lookups
        public static HashSet<string> GetExtensionsHashSet(StringComparer comparer = null)
        {
            var extensions = GetExtensions();
            return new HashSet<string>(extensions, comparer ?? StringComparer.OrdinalIgnoreCase);
        }

        // Gets the default extensions as a List for methods that require that format.
        public static List<string> GetDefaultExtensionsList()
        {
            return new List<string>(DefaultExtensions);
        }
    }
}
