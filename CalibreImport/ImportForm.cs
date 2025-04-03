using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace CalibreImport
{
    public partial class ImportForm : Form
    {
        private float initialFontSize;
        private Size initialFormSize;
        private List<CalibreLibrary> _libraries;
        private bool skipSuccessMessage = bool.Parse(CustomSettings.Config.AppSettings.Settings["skipSuccessMessage"].Value ?? "False");
        private string autoCalibreOpen = CustomSettings.Config.AppSettings.Settings["autoCalibreOpen"].Value ?? "ask";


        // Add a delegate for the import function
        private Func<string, bool> _importFunction;

        public string SelectedLibraryPath { get; private set; }

        public ImportForm()
        {
            InitializeComponent();
            ApplyResourceStrings();
            PopulateLibraries();
            this.StartPosition = FormStartPosition.Manual;
            this.Location = Cursor.Position;
            initialFontSize = this.Font.Size;
            initialFormSize = this.ClientSize;
        }

        public ImportForm(List<CalibreLibrary> libraries, Func<string, bool> importFunction = null)
        {
            _libraries = libraries;
            _importFunction = importFunction;
            InitializeComponent();
            ApplyResourceStrings();
            PopulateLibraries();
            this.StartPosition = FormStartPosition.Manual;
            this.Location = Cursor.Position;
            initialFontSize = this.Font.Size;
            initialFormSize = this.ClientSize;
        }

        private void PopulateLibraries()
        {
            var libraries = _libraries ?? CalibreLibraryManager.GetLibraries();
            if (libraries == null)
            {
                MessageBox.Show("No libraries found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Retrieve hidden libraries from settings
            string hiddenSetting = CustomSettings.Config.AppSettings.Settings["language"].Value;
            var hiddenList = hiddenSetting.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                                          .Select(s => s.Trim())
                                          .ToList();

            // Filter out hidden libraries
            var visibleLibraries = libraries.Where(lib => !hiddenList.Contains(lib.Name)).ToList();

            listBoxLibraries.DataSource = visibleLibraries;
            listBoxLibraries.DisplayMember = "Name";
        }

        // the import button click event handler
        private void btnImport_Click(object sender, EventArgs e)
        {
            if (listBoxLibraries.SelectedItem is CalibreLibrary selectedLibrary)
            {
                SelectedLibraryPath = selectedLibrary.Path;

                // If we have an import function, use it
                if (_importFunction != null)
                {
                    // Disable controls during import
                    btnImport.Enabled = false;
                    btnCancel.Enabled = false;
                    btnSettings.Enabled = false;
                    listBoxLibraries.Enabled = false;

                    // Reset progress bar
                    progressBar1.Value = 0;

                    // Run the import in a background thread
                    System.Threading.ThreadPool.QueueUserWorkItem(state =>
                    {
                        try
                        {
                            // Start the real import process with progress reporting
                            StartImportProcess();

                            // Show success message and prompt to open Calibre
                            this.Invoke(new Action(() =>
                            {
                                if (skipSuccessMessage)
                                {
                                    // Handle auto open based on settings
                                    if (autoCalibreOpen == "always")
                                    {
                                        LaunchCalibre(SelectedLibraryPath);
                                    }
                                    // Close the form regardless
                                    DialogResult = DialogResult.OK;
                                }
                                else
                                {
                                    var result = MessageBox.Show(
                                        ResourceStrings.ImportSuccessRes,
                                        ResourceStrings.ImportFormRes,
                                        MessageBoxButtons.YesNo,
                                        MessageBoxIcon.Question);

                                    if (result == DialogResult.Yes)
                                    {
                                        LaunchCalibre(SelectedLibraryPath);
                                    }

                                    // Now we can close the form
                                    DialogResult = DialogResult.OK;
                                }
                            }));
                        }
                        catch (Exception ex)
                        {
                            this.Invoke(new Action(() =>
                            {
                                MessageBox.Show(
                                    $"Error during import: {ex.Message}",
                                    ResourceStrings.ErrorRes,
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Error);

                                // Re-enable controls
                                btnImport.Enabled = true;
                                btnCancel.Enabled = true;
                                btnSettings.Enabled = true;
                                listBoxLibraries.Enabled = true;
                            }));
                        }
                    });
                }
                else
                {
                    // No import function provided, just return success
                    DialogResult = DialogResult.OK;
                }
            }
            else
            {
                MessageBox.Show(ResourceStrings.PleaseSelectLibraryRes, ResourceStrings.ErrorRes, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void btnSettings_Click(object sender, EventArgs e)
        {
            using (var settingsForm = new SettingsForm())
            {
                settingsForm.StartPosition = FormStartPosition.Manual;
                settingsForm.Location = Cursor.Position;
                settingsForm.Show();
            }
        }

        private void StartImportProcess()
        {
            bool success = false;

            try
            {
                // Call the provided import function
                if (_importFunction != null)
                {
                    // Simulate progress
                    for (int i = 0; i <= 90; i += 10)
                    {
                        UpdateProgress(i);
                        System.Threading.Thread.Sleep(100); // Simulate work
                    }

                    // Execute the actual import
                    success = _importFunction(SelectedLibraryPath);

                    // Final update
                    UpdateProgress(100);
                }
            }
            catch (Exception)
            {
                throw;
            }

            if (!success)
            {
                throw new Exception("Import process failed");
            }
        }

        public void UpdateProgress(int progress)
        {
            if (progressBar1.InvokeRequired)
            {
                progressBar1.Invoke(new Action(() => progressBar1.Value = progress));
            }
            else
            {
                progressBar1.Value = progress;
            }
        }

        private void LaunchCalibre(string libraryPath)
        {
            try
            {
                string calibreFolder = CustomSettings.Config.AppSettings.Settings["calibreFolder"].Value ?? @"C:\Program Files\Calibre2\";
                string calibrePath = Path.Combine(calibreFolder, "calibre.exe");

                var startInfo = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = calibrePath,
                    Arguments = $"--with-library \"{libraryPath}\"",
                    UseShellExecute = true
                };

                System.Diagnostics.Process.Start(startInfo);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{ResourceStrings.ErrorLaunchingRes} {ex.Message}",
                    ResourceStrings.ImportFormRes,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // Event handler for the DPI change
        protected override void OnDpiChanged(DpiChangedEventArgs e)
        {
            base.OnDpiChanged(e);
            float scaleFactor = e.DeviceDpiNew / 96f;
            this.Font = new Font(this.Font.FontFamily, initialFontSize * scaleFactor, this.Font.Style);
            this.ClientSize = new Size((int)(initialFormSize.Width * scaleFactor), (int)(initialFormSize.Height * scaleFactor));
            foreach (Control control in this.Controls)
            {
                control.Font = new Font(control.Font.FontFamily, initialFontSize * scaleFactor, control.Font.Style);
            }
        }

        private void ImportForm_Load(object sender, EventArgs e)
        {
            ApplyResourceStrings();
        }

        private void ApplyResourceStrings()
        {
            this.btnImport.Text = ResourceStrings.ImportBtnRes;
            this.btnCancel.Text = ResourceStrings.CancelRes;
            this.btnSettings.Text = ResourceStrings.SettingsRes;
            this.Text = ResourceStrings.SelectLibraryRes;
        }
    }
}
