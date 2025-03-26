using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using CalibreImport;

namespace CalibreImport
{
    public partial class ImportForm : Form
    {
        private float initialFontSize;
        private Size initialFormSize;

        public string SelectedLibraryPath { get; private set; }

        public ImportForm(List<CalibreLibrary> libraries)
        {
            InitializeComponent();
            PopulateLibraries(libraries);
            this.StartPosition = FormStartPosition.Manual;
            this.Location = Cursor.Position;
            SetFormIcon();
            initialFontSize = this.Font.Size;
            initialFormSize = this.ClientSize;
        }

        private void PopulateLibraries(List<CalibreLibrary> libraries)
        {
            // Retrieve hidden libraries from settings
            string hiddenSetting = CustomSettings.Config.AppSettings.Settings["hiddenLibraries"].Value ?? "";
            var hiddenList = hiddenSetting.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                                          .Select(s => s.Trim())
                                          .ToList();

            // Filter out hidden libraries
            var visibleLibraries = libraries.Where(lib => !hiddenList.Contains(lib.Name)).ToList();

            listBoxLibraries.DataSource = visibleLibraries;
            listBoxLibraries.DisplayMember = "Name";
        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            if (listBoxLibraries.SelectedItem is CalibreLibrary selectedLibrary)
            {
                SelectedLibraryPath = selectedLibrary.Path;
                DialogResult = DialogResult.OK;
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
                settingsForm.ShowDialog();
            }
        }

        private void SetFormIcon()
        {
            try
            {
                string base64Icon = CustomSettings.Config.AppSettings.Settings["calibreIconBase64"].Value;
                if (!string.IsNullOrEmpty(base64Icon))
                {
                    byte[] iconBytes = Convert.FromBase64String(base64Icon);
                    using (var ms = new MemoryStream(iconBytes))
                    {
                        Bitmap bitmap = new Bitmap(ms);
                        this.Icon = Icon.FromHandle(bitmap.GetHicon());
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogThis($"Error setting form icon: {ex.Message}");
            }
        }

        private void StartImportProcess()
        {
            // Simulate import process and update progress bar
            for (int i = 0; i <= 100; i++)
            {
                UpdateProgress(i);
                System.Threading.Thread.Sleep(50); // Simulate work
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

            if (progress == 100)
            {
                MessageBox.Show(ResourceStrings.ImportSuccessRes, ResourceStrings.ImportFormRes, MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
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
    }
}
