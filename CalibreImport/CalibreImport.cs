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
using System.Reflection;

// Libri non sunt multiplicandi praeter necessitatem
namespace CalibreImport
{
    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.None)]//, ComVisible(true)]
    [Guid("8E5CD5CA-64E0-479A-B62F-B1FC00FF0227")]
    [DisplayName("CalibreImport")]
    //[COMServerAssociation(AssociationType.AllFiles)] //needed for Directory Opus, apparently.
    [COMServerAssociation(AssociationType.ClassOfExtension, ".epub", ".pdf", ".mobi", ".azw", ".azw3", ".fb2", ".djvu", ".lrf", ".rtf", ".txt", ".doc", ".docx", ".odt", ".htm", ".html", ".cbz", ".cbr", ".pdb", ".snb", ".tcr", ".zip", ".rar")]

    // Welcome to CalibreImport, the main class of this Shell Extension.
    // This class is responsible for creating the context menu and handling the calibredb.exe import process.
    // It uses the SharpShell library to create a COM server that integrates with Windows Explorer.
    // The class is decorated with various attributes to specify its COM visibility and associations with file types.
    public class CalibreImport : SharpContextMenu
    {
        // HashSet for supported extensions to improve lookup performance
        private HashSet<string> _supportedExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        // variables from the Settings file and their defaults
        private string MenuText = ResourceStrings.MenuTextRes;
        private string calibreFolder = CustomSettings.Config.AppSettings.Settings["calibreFolder"].Value ?? @"C:\Program Files\Calibre2\";
        private bool useSubmenu = bool.Parse(CustomSettings.Config.AppSettings.Settings["useSubmenu"].Value ?? "true");
        private string automerge = CustomSettings.Config.AppSettings.Settings["autoMerge"].Value.ToLower() ?? "overwrite";
        private bool autoKillCalibre = bool.Parse(CustomSettings.Config.AppSettings.Settings["autoKillCalibre"].Value ?? "false");
        private string hiddenLibraries = CustomSettings.Config.AppSettings.Settings["hiddenLibraries"].Value ?? "";
        private bool skipSuccessMessage = bool.Parse(CustomSettings.Config.AppSettings.Settings["skipSuccessMessage"].Value ?? "False");
        private bool autoCalibreOpen = bool.Parse(CustomSettings.Config.AppSettings.Settings["autoCalibreOpen"].Value ?? "False");
        private string savedCulture = CustomSettings.Config.AppSettings.Settings["language"].Value;

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

        // Time checks for retrieveing file types in CanShowMenu
        private DateTime _lastExtensionsUpdate = DateTime.MinValue;
        private readonly TimeSpan _extensionCacheDuration = TimeSpan.FromMinutes(5);

        // other variables
        private string dllDirectory;
        private string calibredbPath;
        private string calibrePath;

        // Constructor: calls on the CultureManager class to set culture,
        // and loads paths and supported file extensions.
        public CalibreImport()
        {
            // Ensure the default application culture is set FIRST before any other initialization
            CultureManager.SetApplicationCulture();

            // Set default thread culture to ensure new threads use the correct culture
            CultureInfo culture = Thread.CurrentThread.CurrentUICulture;
            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;

            // Initialize other components or settings
            InitializePaths();
            LoadSupportedExtensions();
        }

        // Initialize paths
        // Sets up directories and file paths for Calibre binaries.
        private void InitializePaths()
        {
            dllDirectory = Path.GetDirectoryName(typeof(CalibreImport).Assembly.Location);
            calibredbPath = Path.Combine(calibreFolder, "calibredb.exe");
            calibrePath = Path.Combine(calibreFolder, "calibre.exe");
        }

        // This is called by the SharpShell framework when determining
        // whether to display the context menu.								
        protected override bool CanShowMenu()
        {
            try
            {
                // Only reload extensions if needed
                if (_supportedExtensions == null || _supportedExtensions.Count == 0 ||
                    DateTime.Now - _lastExtensionsUpdate > _extensionCacheDuration)
                {
                    InitializePaths();
                    LoadSupportedExtensions();
                    _lastExtensionsUpdate = DateTime.Now;
                }
                // Log the selected item paths and supported extensions
                Logger.LogThis($"SelectedItemPaths: {string.Join(", ", SelectedItemPaths)}", true);
                //Logger.LogThis($"SupportedExtensions: {string.Join(", ", _supportedExtensions)}", true);

                // Ensure all selected items have supported extensions
                bool allSupported = SelectedItemPaths.Any(p => _supportedExtensions.Contains(Path.GetExtension(p).ToLower()));
                Logger.LogThis($"AllSupported: {allSupported}", true);

                return allSupported;
            }
            catch (Exception ex)
            {
                Logger.LogThis($"Error in CanShowMenu: {ex.Message}");
                return false;
            }
        }

        // Creates and displays the context menu for file import operations.
        // Handles both submenu and single menu item display modes based on user settings.
        protected override ContextMenuStrip CreateMenu()
        {
            // Make sure culture is set correctly before creating any UI text
            CultureManager.SetApplicationCulture();

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

            // Get the appropriate size for the icons based on the current DPI settings
            Size iconSize;
            using (Graphics g = Graphics.FromHwnd(IntPtr.Zero))
            {
                float dpi = g.DpiX;
                int size = (int)(16 * (
                    dpi / 96)); // 16 is the base size for 96 DPI
                iconSize = new Size(size, size);
            }

            if (useSubmenu)
            {
                var submenu = new ToolStripMenuItem(MenuText);
                var mainAppIcon = new Icon(Properties.Resources.MainAppIcon, iconSize);
                submenu.Image = mainAppIcon.ToBitmap(); // Set main menu icon

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
                    var booksIcon = new Icon(Properties.Resources.ImportSubmenuIcon, iconSize);
                    foreach (var library in libraries)
                    {
                        var libraryItem = new ToolStripMenuItem(library.Name);
                        libraryItem.Click += (sender, args) => ExecuteImportWithProgress(library.Path, submenu);
                        libraryItem.Image = booksIcon.ToBitmap(); // Set the icon for each submenu item
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
                var mainAppIcon = new Icon(Properties.Resources.MainAppIcon, iconSize);
                item.Image = mainAppIcon.ToBitmap();
                item.Click += (sender, args) => ExecuteImport();

                menu.Items.Add(item); // Add the item to the menu
            }

            return menu;
        }

        // Define a non-generic delegate type
        // whatever that means
        public delegate void ReportProgressDelegate(int progress);

        // Primary import handler that processes single or multiple files
        // Manages Calibre process state, and handles user interaction during import
        private void ExecuteImport(string selectedLibrary = null, ReportProgressDelegate reportProgress = null)
        {
            try
            {
                // Ensure culture is set correctly before displaying any messages
                CultureManager.SetApplicationCulture();

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

                Logger.LogThis($"Found {filePaths.Count} supported files.", true);

                // make sure calibredb.exe exists, otherwise bye-bye
                if (!File.Exists(calibredbPath))
                {
                    Logger.LogThis($"Could not find calibredb.exe at {calibredbPath}. Please update the script with the correct path.");
                    MessageBox.Show($"Could not find calibredb.exe at {calibredbPath}. Please check your Calibre installation path in settings.",
                        ResourceStrings.NameAppRes, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                Logger.LogThis("calibredb.exe found.", true);

                // What to do if Calibre is running
                if (IsCalibreRunning())
                {
                    Logger.LogThis("Calibre is running.", true);

                    if (autoKillCalibre)
                    {
                        Logger.LogThis("Auto-killing Calibre without asking.", true);
                        KillCalibre();
                    }
                    else
                    {
                        Logger.LogThis("Prompting user to close Calibre.", true);
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
                    Logger.LogThis("No library selected, prompting user to select a library.", true);

                    var libraries = GetCalibreLibraries();
                    if (libraries == null || !libraries.Any())
                    {
                        Logger.LogThis("No Calibre libraries found. Ensure Calibre is configured properly.");
                        MessageBox.Show("No Calibre libraries found. Ensure Calibre is configured properly.",
                            ResourceStrings.NameAppRes, MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    // Define the import function for the form to use
                    Func<string, bool> importFunction = (libPath) => {
                        // Import files individually with progress reporting
                        int totalFiles = filePaths.Count;
                        bool allSuccessful = true;

                        for (int i = 0; i < totalFiles; i++)
                        {
                            Logger.LogThis($"Importing file {i + 1} of {totalFiles}: {filePaths[i]}", true);
                            bool success = ImportSingleFile(libPath, filePaths[i]);
                            if (!success) allSuccessful = false;

                            // Report progress to the form
                            reportProgress?.Invoke((i + 1) * 100 / totalFiles);
                        }

                        return allSuccessful;
                    };

                    var importForm = new ImportForm(libraries, importFunction);
                    importForm.ShowDialog();

                }
                else
                {
                    // Direct import with the specified library
                    // Method 1: Import files individually with progress reporting
                    int totalFiles = filePaths.Count;
                    bool allSuccessful = true;

                    for (int i = 0; i < totalFiles; i++)
                    {
                        Logger.LogThis($"Importing file {i + 1} of {totalFiles}: {filePaths[i]}", true);
                        bool success = ImportSingleFile(selectedLibrary, filePaths[i]);
                        if (!success) allSuccessful = false;

                        // Report progress
                        reportProgress?.Invoke((i + 1) * 100 / totalFiles);
                    }

                    // Method 2: Import all files at once
                    if (allSuccessful)
                    {
                        Logger.LogThis("Files imported successfully.", true);

                        // Check if we should skip the success message
                        if (skipSuccessMessage)
                        {
                            Logger.LogThis("Skipping success message due to user settings.", true);

                            if (autoCalibreOpen)
                            {
                                Logger.LogThis("Auto-opening Calibre due to user settings.", true);
                                LaunchCalibre(selectedLibrary);
                            }
                            else
                            {
                                Logger.LogThis("Not opening Calibre due to user settings.", true);
                            }
                        }
                        else
                        {
                            // Show success message as before
                            var result = MessageBox.Show(ResourceStrings.ImportSuccessRes, ResourceStrings.NameAppRes, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                            if (result == DialogResult.Yes)
                            {
                                LaunchCalibre(selectedLibrary);
                            }
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                Logger.LogThis($"Error executing import: {ex.Message}");
                MessageBox.Show($"Error executing import: {ex.Message}", ResourceStrings.NameAppRes, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Helper method to import a single file and return success/failure
        private bool ImportSingleFile(string libraryPath, string filePath)
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

                Logger.LogThis($"Process output: {output}", true);
                // Only log error output if it's not empty
                if (!string.IsNullOrWhiteSpace(error))
                {
                    Logger.LogThis($"Process error: {error}", true);
                }

                if (process.ExitCode == 0)
                {
                    Logger.LogThis($"Successfully imported file: {filePath}");
                    return true;
                }
                else
                {
                    Logger.LogThis($"Failed to import file: {filePath}. Error: {error}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Logger.LogThis($"Error importing file: {filePath}. Exception: {ex.Message}");
                return false;
            }
        }

        // handles ebook import with progress bar
        private void ExecuteImportWithProgress(string selectedLibrary, ToolStripMenuItem parentMenu)
        {
            Logger.LogThis("Starting ExecuteImportWithProgress method.", true);

            var progressForm = new ProgressForm();
            progressForm.Show();

            Task.Run(() =>
            {
                try
                {
                    // For single file imports, we need more granular progress updates
                    var filePaths = SelectedItemPaths.Where(p =>
                        _supportedExtensions.Contains(Path.GetExtension(p).ToLower()))
                        .ToList();

                    if (filePaths.Count == 1)
                    {
                        // Set initial progress to show activity
                        progressForm.UpdateProgress(10);

                        // Special handling for single file import
                        var success = ImportSingleFileWithProgress(selectedLibrary, filePaths[0],
                            (subProgress) => progressForm.UpdateProgress(subProgress));

                        // Ensure the progress bar shows completion
                        progressForm.UpdateProgress(100);
                    }
                    else
                    {
                        // Multiple files - use standard progress reporting
                        ExecuteImport(selectedLibrary, progress =>
                        {
                            Logger.LogThis($"Updating progress to {progress}%.", true);
                            progressForm.UpdateProgress(progress);
                        });
                    }

                    progressForm.Invoke(new Action(() =>
                    {
                        Logger.LogThis("Closing ProgressForm.", true);
                        progressForm.Close();
                    }));
                }
                catch (Exception ex)
                {
                    Logger.LogThis($"Error in ExecuteImportWithProgress: {ex.Message}");
                    progressForm.Invoke(new Action(() =>
                    {
                        progressForm.Close();
                    }));
                }
            });
        }

        // Modified import method for single files with progress reporting
        private bool ImportSingleFileWithProgress(string libraryPath, string filePath, Action<int> reportProgress)
        {
            try
            {
                Logger.LogThis($"Importing single file with progress: {filePath} into library: {libraryPath}", true);

                // Report starting the import
                reportProgress(20);

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

                reportProgress(40); // Starting process

                var process = Process.Start(startInfo);

                reportProgress(50); // Process started

                // Create a timer to update progress while waiting for the process
                var processTimer = new System.Timers.Timer(200);
                processTimer.Elapsed += (s, e) => {
                    if (!process.HasExited)
                    {
                        // Alternate between 60% and 80% to show activity
                        int CalculateProgress(int current) =>
                            current < 80 ? current + 5 : 60;

                        int currentProgress = 60;
                        currentProgress = CalculateProgress(currentProgress);
                        reportProgress(currentProgress);
                    }
                };
                processTimer.Start();

                var output = process.StandardOutput.ReadToEnd();
                var error = process.StandardError.ReadToEnd();
                process.WaitForExit();

                // Stop the timer
                processTimer.Stop();
                processTimer.Dispose();

                reportProgress(90); // Process completed

                Logger.LogThis($"Process output: {output}", true);
                Logger.LogThis($"Process error: {error}", true);

                if (process.ExitCode == 0)
                {
                    Logger.LogThis($"Successfully imported file: {filePath}");
                    reportProgress(100); // Fully complete
                    return true;
                }
                else
                {
                    Logger.LogThis($"Failed to import file: {filePath}. Error: {error}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Logger.LogThis($"Error importing file: {filePath}. Exception: {ex.Message}");
                return false;
            }
        }

        // Call on the SupportedFileTypes class
        // In order to rertrieve the list of supported file extensions
        private void LoadSupportedExtensions()
        {
            _supportedExtensions.Clear();

            // Use the centralized file extensions manager with HashSet for performance
            _supportedExtensions = SupportedFileTypes.GetExtensionsHashSet();

            Logger.LogThis($"Loaded {_supportedExtensions.Count} supported extensions from " +
                (_supportedExtensions.Count > SupportedFileTypes.DefaultExtensions.Length ?
                "registry." : "default list."), true);
        }

        // Check if any Calibre-related process is running
        private bool IsCalibreRunning()
        {
            return CalibreProcesses.Any(processName => Process.GetProcessesByName(processName).Any());
        }

        // Attempts to terminate all Calibre-related processes, if any.
        private void KillCalibre()
        {
            var runningProcesses = CalibreProcesses
                .SelectMany(processName => Process.GetProcessesByName(processName))
                .ToList();

            if (runningProcesses.Any())
            {
                var processNames = runningProcesses.Select(p => p.ProcessName).Distinct();
                Logger.LogThis($"Terminating Calibre processes: {string.Join(", ", processNames)}", true);

                foreach (var process in runningProcesses)
                {
                    try
                    {
                        process.Kill();
                        process.WaitForExit(1000); // Wait up to 1 second for the process to exit
                    }
                    catch (Exception ex)
                    {
                        Logger.LogThis($"Error killing process {process.ProcessName}: {ex.Message}", true);
                    }
                }
                Logger.LogThis("Calibre-related processes have been terminated.", true);
            }
            else
            {
                Logger.LogThis("No Calibre-related processes were running.", true);
            }
        }

        // Call on the CalibreLibraryManager class to retrieve the list of Calibre libraries
        private List<CalibreLibrary> GetCalibreLibraries()
        {
            return CalibreLibraryManager.GetLibraries();
        }

        // Open the Settings form
        public void ShowSettingsForm()
        {
            try
            {
                Logger.LogThis("Attempting to show Settings form.", true);

                // Make sure culture is set correctly before showing the form
                CultureManager.SetApplicationCulture();

                var form = new SettingsForm();
                form.StartPosition = FormStartPosition.CenterScreen;
                form.Show();

                Logger.LogThis("Settings form displayed successfully.", true);
            }
            catch (Exception ex)
            {
                Logger.LogThis($"Error showing Settings form: {ex.Message}");
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
                // Ensure culture is set correctly before displaying any messages
                CultureManager.SetApplicationCulture();

                Logger.LogThis("Registering shell extension...", true);
                KeyRegistryExtensions.RegisterExtension(t.GUID, "ImportToCalibre", SupportedFileTypes.GetExtensions());
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
                // Ensure culture is set correctly before displaying any messages
                CultureManager.SetApplicationCulture();

                Logger.LogThis("Unregistering shell extension...", true);

                KeyRegistryExtensions.UnregisterExtension(t.GUID, SupportedFileTypes.GetExtensions());
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

    // Class to manage registry keys upon registration and unregistration.
    // Called by [ComRegisterFunction] and [ComUnregisterFunction] in the main class.
    public static class KeyRegistryExtensions
    {
        public static void RegisterExtension(Guid guid, string menuName, List<string> extensions)
        {
            Logger.LogThis("Starting RegisterExtension...", true);
            try
            {
                // Registering the CLSID is not needed as it is done by SharpShell																	  
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
                        Logger.LogThis($"Unregistered context menu handler for extension: {ext}", true);
                    }
                    catch (ArgumentException)
                    {
                        // Key does not exist, ignore
                        Logger.LogThis($"Registry key does not exist for extension: {ext}", true);
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