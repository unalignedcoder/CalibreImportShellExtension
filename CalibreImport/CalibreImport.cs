using Microsoft.Win32;
using Newtonsoft.Json.Linq;
using SharpShell.Attributes;
using SharpShell.ServerRegistration;
using SharpShell.SharpContextMenu;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using System;
using System.Threading.Tasks;

// Libri non sunt multiplicandi praeter necessitatem
namespace CalibreImport
{
    //[ComVisible(true)]
    [ClassInterface(ClassInterfaceType.None)]
    [Guid("8E5CD5CA-64E0-479A-B62F-B1FC00FF0227"), ComVisible(true)]
    [DisplayName("Calibre Import Shell Extension")]
    [COMServerAssociation(AssociationType.AllFiles)] //needed for Directory Opus, apparently.
    [COMServerAssociation(AssociationType.ClassOfExtension, ".epub", ".pdf", ".mobi", ".azw", ".azw3", ".fb2", ".djvu", ".lrf", ".rtf", ".txt", ".doc", ".docx", ".odt", ".htm", ".html", ".cbz", ".cbr", ".pdb", ".snb", ".tcr", ".zip", ".rar")]
        public class CalibreImport : SharpContextMenu
    {
        private List<string> _supportedExtensions = new List<string>();

        // variables from the Settings file and their defaults
        private string MenuText = ResourceStrings.MenuTextRes;
        private string calibreFolder = CustomSettings.Config.AppSettings.Settings["calibreFolder"].Value ?? @"C:\Program Files\Calibre2\";
        private bool useSubmenu = bool.Parse(CustomSettings.Config.AppSettings.Settings["useSubmenu"].Value ?? "true");
        private string automerge = CustomSettings.Config.AppSettings.Settings["autoMerge"].Value.ToLower() ?? "overwrite";
        private bool autoKillCalibre = bool.Parse(CustomSettings.Config.AppSettings.Settings["autoKillCalibre"].Value ?? "false");
        private string hiddenLibraries = CustomSettings.Config.AppSettings.Settings["hiddenLibraries"].Value ?? "";

        // List of Calibre-related processes to check
        private static readonly string[] CalibreProcesses = new[]
        {
            "calibre",
            "calibre-complete",
            "calibre-customize",
            "calibre-debug",
            "calibre-parallel",
            "calibre-server",
            "calibre-smtp"
        };

        // other variables
        private string dllDirectory;
        private string calibredbPath;
        private string calibrePath;

        // Constructor
        public CalibreImport()
        {
            // Load the saved language setting
            var savedCulture = CustomSettings.Config.AppSettings.Settings["language"].Value;
            if (!string.IsNullOrEmpty(savedCulture))
            {
                CultureInfo culture = new CultureInfo(savedCulture);
                Thread.CurrentThread.CurrentUICulture = culture;
                Thread.CurrentThread.CurrentCulture = culture;
            }

            // Initialize other components or settings if needed
            InitializePaths();
            LoadSupportedExtensions();
        }

        // Initialize paths
        private void InitializePaths()
        {
            dllDirectory = Path.GetDirectoryName(typeof(CalibreImport).Assembly.Location);
            calibredbPath = Path.Combine(calibreFolder, "calibredb.exe");
            calibrePath = Path.Combine(calibreFolder, "calibre.exe");
        }

        // This is called by the SharpShell framework when determining whether to display the context menu.								
        protected override bool CanShowMenu()
        {
            try
            {
                InitializePaths();
                LoadSupportedExtensions();

                // Log the selected item paths and supported extensions
                Logger.LogThis($"SelectedItemPaths: {string.Join(", ", SelectedItemPaths)}", true);
                Logger.LogThis($"SupportedExtensions: {string.Join(", ", _supportedExtensions)}", true);

                // Ensure all selected items have supported extensions
                bool allSupported = SelectedItemPaths.Any(p => _supportedExtensions.Contains(Path.GetExtension(p).ToLower()));
                Logger.LogThis($"AllSupported: {allSupported}", true);

                return allSupported;
                //return true;
            }
            catch (Exception ex)
            {
                Logger.LogThis($"Error in CanShowMenu: {ex.Message}");
                return false;
            }
        }

        // Create the context menu entry
        protected override ContextMenuStrip CreateMenu()
        {
            var menu = new ContextMenuStrip();
            Logger.LogThis("Creating context menu.", true);

            var libraries = GetCalibreLibraries();
            if (libraries == null || !libraries.Any())
            {
                Logger.LogThis("No Calibre libraries found. Ensure Calibre is configured properly.");
                return menu;
            }

            // Retrieve hidden libraries from custom settings and filter them out.
            var hiddenLibs = !string.IsNullOrEmpty(hiddenLibraries)
                ? hiddenLibraries.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                                 .Select(s => s.Trim())
                : Enumerable.Empty<string>();

            libraries = libraries.Where(lib => !hiddenLibs.Contains(lib.Name)).ToList();

            if (useSubmenu)
            {
                var submenu = new ToolStripMenuItem(MenuText);
                var calibreIcon = new Icon("CalibreImport/Resources/MainAppIcon.ico");
                submenu.Image = calibreIcon.ToBitmap();

                // Check if there are any invalid files
                bool hasInvalidFiles = SelectedItemPaths.Any(p => !_supportedExtensions.Contains(Path.GetExtension(p).ToLower()));

                if (hasInvalidFiles)
                {
                    var invalidItem = new ToolStripMenuItem(ResourceStrings.InvalidFilesRes)
                    {
                        Enabled = false
                    };
                    submenu.DropDownItems.Add(invalidItem);

                }
                // if the selection is valid
                else
                {
                    foreach (var library in libraries)
                    {
                        var libraryItem = new ToolStripMenuItem(library.Name);
                        //
                        //libraryItem.Click += (sender, args) => ExecuteImport(library.Path);
                        libraryItem.Click += (sender, args) => ExecuteImportWithProgress(library.Path, submenu);
                        var booksIcon = new Icon("CalibreImport/Resources/MainAppIcon.ico");
                        libraryItem.Image = booksIcon.ToBitmap();
                        submenu.DropDownItems.Add(libraryItem);
                    }
                }

                // Add separator
                submenu.DropDownItems.Add(new ToolStripSeparator());

                // Add settings menu item
                var settingsItem = new ToolStripMenuItem(ResourceStrings.SettingsRes);
                settingsItem.Click += (sender, args) => ShowSettingsForm();
                submenu.DropDownItems.Add(settingsItem);
                Logger.LogThis("Added settings item to submenu.", true);

                menu.Items.Add(submenu);
                Logger.LogThis("Added submenu to context menu.", true);

            }
            // if using the import dialog
            else
            {
                Logger.LogThis("Creating single menu entry.", true);
                var item = new ToolStripMenuItem(MenuText);
                var calibreIcon = new Icon("CalibreImport/Resources/MainAppIcon.ico");
                item.Image = calibreIcon.ToBitmap();
                item.Click += (sender, args) => ExecuteImport();

                menu.Items.Add(item); // Add the item to the menu
            }

            return menu;
        }

        // Handles ebook import
        private void ExecuteImport(string selectedLibrary = null, Action<int> reportProgress = null)
        {
            try
            {
                Logger.LogThis("ExecuteImport method called.", true);

                InitializePaths();

                var filePaths = SelectedItemPaths.Where(p =>
                    _supportedExtensions.Contains(Path.GetExtension(p).ToLower()))
                    .ToList();

                if (!filePaths.Any())
                {
                    Logger.LogThis("No supported files selected.");
                    return;
                }

                if (!File.Exists(calibredbPath))
                {
                    Logger.LogThis($"Could not find calibredb.exe at {calibredbPath}. Please update the script with the correct path.");
                    return;
                }

                if (IsCalibreRunning())
                {
                    if (autoKillCalibre)
                    {
                        KillCalibre();
                    }
                    else
                    {
                        // message prompt on whether to close calibre, otherwise break import
                        if (MessageBox.Show(ResourceStrings.CalibreRunningRes, ResourceStrings.CalibreRunning2Res, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            KillCalibre();
                        }
                        else
                        {
                            Logger.LogThis("calibre.exe must be closed to proceed.");
                            return;
                        }
                    }
                }

                if (selectedLibrary == null)
                {
                    var libraries = GetCalibreLibraries();
                    if (libraries == null || !libraries.Any())
                    {
                        // Logger.LogThis("No Calibre libraries found. Ensure Calibre is configured properly.");
                        return;
                    }

                    using (var importForm = new ImportForm(libraries))
                    {
                        if (importForm.ShowDialog() == DialogResult.OK)
                        {
                            selectedLibrary = importForm.SelectedLibraryPath;
                        }
                        else
                        {
                            Logger.LogThis("Library selection was canceled.");
                            return;
                        }
                    }
                }

                int totalFiles = filePaths.Count;
                for (int i = 0; i < totalFiles; i++)
                {
                    // Import each file
                    ImportFileIntoCalibre(selectedLibrary, filePaths[i]);

                    // Report progress
                    reportProgress?.Invoke((i + 1) * 100 / totalFiles);
                }

                var importSuccess = ImportFilesIntoCalibre(selectedLibrary, filePaths);

                if (importSuccess)
                {
                    // MessageBox after successful import, prompt the user to start calibre
                    var result = MessageBox.Show(ResourceStrings.ImportSuccessRes, ResourceStrings.NameAppRes, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        LaunchCalibre(selectedLibrary);
                    }
                }
                else
                {
                    // otherwise invite to check log
                    MessageBox.Show(ResourceStrings.ImportFailureRes, ResourceStrings.NameAppRes, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                Logger.LogThis($"Error executing import: {ex.Message}");
            }
        }

        // handles ebook import with progress bar
        private void ExecuteImportWithProgress(string selectedLibrary, ToolStripMenuItem parentMenu)
        {
            var progressForm = new ProgressForm();
            progressForm.Show();

            Task.Run(() =>
            {
                ExecuteImport(selectedLibrary, progress =>
                {
                    progressForm.UpdateProgress(progress);
                });

                progressForm.Invoke(new Action(() =>
                {
                    progressForm.Close();
                }));
            });
        }

        // Load ebook extensions supported by Calibre from Registry														   
        private void LoadSupportedExtensions()
        {
            using (var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\calibre\calibre64bit\Capabilities\FileAssociations"))
            {
                if (key != null)
                {
                    _supportedExtensions = key.GetValueNames().ToList();
                    return;
                }
            }

            _supportedExtensions = new List<string>
            {
                ".epub", ".pdf", ".mobi", ".azw", ".azw3", ".fb2", ".djvu",
                ".lrf", ".rtf", ".txt", ".doc", ".docx", ".odt", ".htm", ".html",
                ".cbz", ".cbr", ".pdb", ".snb", ".tcr", ".zip", ".rar"
            };
        }

        // Get list of supported file extensions										
        private static List<string> GetFileExtensions()
        {
            var extensions = new List<string>();
            using (var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\calibre\calibre64bit\Capabilities\FileAssociations"))
            {
                if (key != null)
                {
                    extensions = key.GetValueNames().ToList();
                    return extensions;
                }
            }

            return new List<string>
            {
                ".epub", ".pdf", ".mobi", ".azw", ".azw3", ".fb2", ".djvu",
                ".lrf", ".rtf", ".txt", ".doc", ".docx", ".odt", ".htm", ".html",
                ".cbz", ".cbr", ".pdb", ".snb", ".tcr", ".zip", ".rar"
            };
        }

        // Check if any Calibre-related process is running
        private bool IsCalibreRunning()
        {
            return CalibreProcesses.Any(processName => Process.GetProcessesByName(processName).Any());
        }

        // Kill Calibre-related processes
        private void KillCalibre()
        {
            var runningProcesses = CalibreProcesses
                .SelectMany(processName => Process.GetProcessesByName(processName))
                .ToList();

            if (runningProcesses.Any())
            {
                var processNames = runningProcesses.Select(p => p.ProcessName).Distinct();
                var warningMessage = $"{ResourceStrings.CalibreProcessesRunningRes}\n\n{string.Join("\n", processNames)}\n\n{ResourceStrings.DoYouWantToProceedRes}";

                if (MessageBox.Show(warningMessage, ResourceStrings.CalibreRunning2Res, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    foreach (var process in runningProcesses)
                    {
                        process.Kill();
                    }
                    Logger.LogThis("Calibre-related processes were running and have been terminated.", true);
                }
                else
                {
                    Logger.LogThis("User chose not to terminate Calibre-related processes.", true);
                }
            }
            else
            {
                Logger.LogThis("No Calibre-related processes were running.", true);
            }
        }

        // Get list of Calibre libraries from gui.json											  
        private List<CalibreLibrary> GetCalibreLibraries()
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
                var libraries = guiJson["library_usage_stats"]?.ToObject<Dictionary<string, object>>()?.Keys
                    .Select(path => new CalibreLibrary { Path = path, Name = Path.GetFileName(path) })
                    .ToList();

                if (libraries == null || !libraries.Any())
                {
                    Logger.LogThis("No libraries found in Calibre GUI settings.");
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

        // Open the Settings form
        public void ShowSettingsForm()
        {
            try
            {
                Logger.LogThis("Attempting to show Settings form.",true);
                using (var form = new SettingsForm())
                {
                    form.StartPosition = FormStartPosition.Manual;
                    form.Location = Cursor.Position;
                    form.ShowDialog();
                }
                Logger.LogThis("Settings form displayed successfully.", true);
            }
            catch (Exception ex)
            {
                Logger.LogThis($"Error showing Settings form: {ex.Message}");
            }
        }

        // Import multiple ebook files into Calibre library										  
        private bool ImportFilesIntoCalibre(string libraryPath, List<string> filePaths)
        {
            try
            {
                var arguments = $"add --library-path \"{libraryPath}\" --automerge {automerge} {string.Join(" ", filePaths.Select(f => $"\"{f}\""))}";
                var startInfo = new ProcessStartInfo
                {
                    FileName = calibredbPath,
                    Arguments = arguments,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                };

                var process = Process.Start(startInfo);
                var output = process.StandardOutput.ReadToEnd();
                var error = process.StandardError.ReadToEnd();
                process.WaitForExit();

                if (process.ExitCode == 0)
                {
                    Logger.LogThis($"Successfully imported files: {string.Join(", ", filePaths)}");
                    return true;
                }
                else
                {
                    Logger.LogThis($"Failed to import files: {error}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Logger.LogThis($"Error during file import: {ex.Message}");
                return false;
            }
        }

        // Import one single ebook file into Calibre library			
        private void ImportFileIntoCalibre(string libraryPath, string filePath)
        {
            try
            {
                Logger.LogThis($"Importing file: {filePath} into library: {libraryPath}", true);

                var arguments = $"add --library-path \"{libraryPath}\" --automerge {automerge} \"{filePath}\"";
                var startInfo = new ProcessStartInfo
                {
                    FileName = calibredbPath,
                    Arguments = arguments,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                };

                var process = Process.Start(startInfo);
                var output = process.StandardOutput.ReadToEnd();
                var error = process.StandardError.ReadToEnd();
                process.WaitForExit();

                if (process.ExitCode == 0)
                {
                    Logger.LogThis($"Successfully imported file: {filePath}");
                }
                else
                {
                    Logger.LogThis($"Failed to import file: {filePath}. Error: {error}");
                }
            }
            catch (Exception ex)
            {
                Logger.LogThis($"Error importing file: {filePath}. Exception: {ex.Message}");
            }
        }

        // Launch Calibre with specified library
        private void LaunchCalibre(string libraryPath)
        {
            try
            {
                var startInfo = new ProcessStartInfo
                {
                    FileName = @"C:\Program Files\Calibre2\calibre.exe",
                    Arguments = $"--with-library \"{libraryPath}\"",
                    UseShellExecute = true
                };

                Process.Start(startInfo);
            }
            catch (Exception ex)
            {
                Logger.LogThis($"Error launching Calibre: {ex.Message}");
                MessageBox.Show($"{ResourceStrings.ErrorLaunchingRes} {ex.Message}", ResourceStrings.NameAppRes, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Register the shell extension
        [ComRegisterFunction]
        public static void Register(Type t)
        {
            try
            {
                Logger.LogThis("Registering shell extension...", true);
                KeyRegistryExtensions.RegisterExtension(t.GUID, "ImportToCalibre", GetFileExtensions());
                Logger.LogThis("Shell extension registered successfully.", true);
            }
            catch (Exception ex)
            {
                Logger.LogThis($"Registration failed: {ex.Message}");
                MessageBox.Show($"{ResourceStrings.RegistrationFailedRes} {ex.Message}");
            }
        }

        // Unregister the shell extension
        [ComUnregisterFunction]
        public static void Unregister(Type t)
        {
            try
            {
                Logger.LogThis("Unregistering shell extension...", true);
                KeyRegistryExtensions.UnregisterExtension(t.GUID, GetFileExtensions());
                Logger.LogThis("Shell extension unregistered successfully.", true);
            }
            catch (Exception ex)
            {
                Logger.LogThis($"Unregistration failed: {ex.Message}");
            }
        }

    }

    // Class to represent Calibre library									 
    public class CalibreLibrary
    {
        public string Path { get; set; }
        public string Name { get; set; }
    }

    // Class to manage registry keys upon registration and unregistration																	 
    public static class KeyRegistryExtensions
    {
        public static void RegisterExtension(Guid guid, string menuName, List<string> extensions)
        {
            Logger.LogThis("Starting RegisterExtension...", true);
            try
            {
                // Register the context menu handler for each extension
                foreach (var ext in extensions)
                {
                    var shellExKeyPath = $@"SOFTWARE\Classes\SystemFileAssociations\{ext}\Shellex\ContextMenuHandlers\CalibreImport";
                    using (var shellExKey = Registry.CurrentUser.CreateSubKey(shellExKeyPath))
                    {
                        if (shellExKey == null)
                        {
                            throw new InvalidOperationException($"Failed to create registry key: {shellExKeyPath}");
                        }

                        shellExKey.SetValue(null, $"{{{guid}}}");
                    }
                    Logger.LogThis($"Registered context menu handler for extension: {ext}", true);
                }

                // Register the server programmatically
                var server = new CalibreImport();
                ServerRegistrationManager.InstallServer(server, RegistrationType.OS64Bit, true);
                ServerRegistrationManager.RegisterServer(server, RegistrationType.OS64Bit);
                Logger.LogThis("Server registered successfully.", true);
            }
            catch (Exception ex)
            {
                Logger.LogThis($"RegisterExtension failed: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Logger.LogThis($"Inner Exception: {ex.InnerException.Message}");
                }
                throw;
            }
        }

        public static void UnregisterExtension(Guid guid, List<string> extensions)
        {
            Logger.LogThis("Starting UnregisterExtension...", true);
            try
            {
                // Unregister the context menu handler for each extension
                foreach (var ext in extensions)
                {
                    var shellExKeyPath = $@"SOFTWARE\Classes\SystemFileAssociations\{ext}\Shellex\ContextMenuHandlers\CalibreImport";
                    try
                    {
                        Registry.CurrentUser.DeleteSubKeyTree(shellExKeyPath, false);
                        //Logger.LogThis($"Unregistered context menu handler for extension: {ext}", true);
                    }
                    catch (ArgumentException)
                    {
                        // Key does not exist, ignore
                        //Logger.LogThis($"Registry key does not exist for extension: {ext}", true);
                    }
                }

                // Unregister the CLSID
                var clsidKeyPath = $@"CLSID\{{{guid}}}";
                try
                {
                    Registry.ClassesRoot.DeleteSubKeyTree(clsidKeyPath, false);
                    Logger.LogThis("Unregistered CLSID.", true);
                }
                catch (ArgumentException)
                {
                    // Key does not exist, ignore
                    Logger.LogThis("CLSID key does not exist.", true);
                }

                // Unregister the server programmatically
                var server = new CalibreImport();
                ServerRegistrationManager.UnregisterServer(server, RegistrationType.OS64Bit);
                ServerRegistrationManager.UninstallServer(server, RegistrationType.OS64Bit);
                Logger.LogThis("Server unregistered successfully.", true);
            }
            catch (Exception ex)
            {
                Logger.LogThis($"UnregisterExtension failed: {ex.Message}");
                throw;
            }
        }
    }

}