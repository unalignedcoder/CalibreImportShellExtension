using System;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace CalibreImport
{
    public partial class SettingsForm : Form
    {
        private float initialFontSize;
        private Size initialFormSize;
        private bool isLoadingSettings = false;

        // Constructor
        public SettingsForm()
        {
            InitializeComponent();
            PopulateLanguageComboBox();
            LoadSettings();
            PopulateHiddenLibraries();
            initialFontSize = this.Font.Size;
            initialFormSize = this.ClientSize;
            
        }

        // Populate the language combo box with supported languages
        private void PopulateLanguageComboBox()
        {
            try
            {
                Logger.LogThis("Populating language combo box...", true);
                var cultures = new[] { "en", "fr", "de", "es", "it", "zh", "ru", "cs", "ja", "ko", "pt", "tr", "pl" };
                comboBoxLanguage.Items.Clear();

                foreach (var culture in cultures)
                {
                    var cultureInfo = new CultureInfo(culture);
                    comboBoxLanguage.Items.Add(new LanguageItem { Culture = cultureInfo.Name, Name = cultureInfo.DisplayName });
                }

                comboBoxLanguage.DisplayMember = "Name";
                comboBoxLanguage.ValueMember = "Culture";

                Logger.LogThis("Language combo box populated successfully.", true);
            }
            catch (Exception ex)
            {
                Logger.LogThis($"Error populating language combo box: {ex.Message}");
                throw;
            }
        }

        // Load settings from the configuration file
        private void LoadSettings()
        {
            try
            {
                isLoadingSettings = true;

                // Unsubscribe from the SelectedIndexChanged event
                comboBoxLanguage.SelectedIndexChanged -= comboBoxLanguage_SelectedIndexChanged;

                // Populate cmbAutomerge with AutomergeOption values
                cmbAutomerge.Items.AddRange(Enum.GetNames(typeof(AutomergeOption)));
                cmbAutomerge.SelectedItem = CustomSettings.Config.AppSettings.Settings["autoMerge"].Value;
                txtCalibreFolder.Text = CustomSettings.Config.AppSettings.Settings["calibreFolder"].Value;
                chkUseSubmenu.Checked = bool.Parse(CustomSettings.Config.AppSettings.Settings["useSubmenu"].Value);
                chkLog.Checked = bool.Parse(CustomSettings.Config.AppSettings.Settings["logThis"].Value);
                chkVerbose.Checked = bool.Parse(CustomSettings.Config.AppSettings.Settings["verboseLog"].Value);
                chkAutoKillCalibre.Checked = bool.Parse(CustomSettings.Config.AppSettings.Settings["autoKillCalibre"].Value);
                chkSkipSuccessMessage.Checked = bool.Parse(CustomSettings.Config.AppSettings.Settings["skipSuccessMessage"].Value);
                chkOpenCalibreAfterImport.Checked = bool.Parse(CustomSettings.Config.AppSettings.Settings["autoCalibreOpen"].Value);    

                // Set the initial state of chkVerbose
                chkVerbose.Enabled = chkLog.Checked;

                // Set the initial state of chkOpenCalibreAfterImport
                chkOpenCalibreAfterImport.Enabled = chkSkipSuccessMessage.Checked;

                // Load the saved language setting
                var savedCulture = CustomSettings.Config.AppSettings.Settings["language"].Value;
                Logger.LogThis($"language in the settings is: {savedCulture}", true);

                if (!string.IsNullOrEmpty(savedCulture))
                {
                    foreach (LanguageItem item in comboBoxLanguage.Items)
                    {
                        if (item.Culture == savedCulture)
                        {
                            comboBoxLanguage.SelectedItem = item;
                            break;
                        }
                    }
                    Logger.LogThis("Selected language set in LoadSettings.", true);
                }
                else
                {
                    Logger.LogThis("No language set, reverting to System language.", true);

                    // If no saved culture, default to the installed UI culture
                    var currentCulture = CultureInfo.InstalledUICulture.Name;
                    foreach (LanguageItem item in comboBoxLanguage.Items)
                    {
                        if (item.Culture == currentCulture)
                        {
                            comboBoxLanguage.SelectedItem = item;
                            break;
                        }
                    }
                }

                // Set the application's culture
                if (!string.IsNullOrEmpty(savedCulture))
                {
                    CultureInfo culture = new CultureInfo(savedCulture);
                    System.Threading.Thread.CurrentThread.CurrentUICulture = culture;
                    System.Threading.Thread.CurrentThread.CurrentCulture = culture;
                    ResourceStrings.LoadResourceStrings();
                }

                // Re-subscribe to the SelectedIndexChanged event
                comboBoxLanguage.SelectedIndexChanged += comboBoxLanguage_SelectedIndexChanged;
            }
            catch (Exception ex)
            {
                Logger.LogThis($"Error loading settings: {ex.Message}");
            }
            finally
            {
                isLoadingSettings = false;
            }
        }

        // Add the event handler method for SkipSuccessfulMessage checkbox
        private void chkSkipSuccessMessage_CheckedChanged(object sender, EventArgs e)
        {
            // Enable or disable the OpenCalibreAfterImport checkbox based on the state of SkipSuccessfulMessage checkbox
            this.chkOpenCalibreAfterImport.Enabled = this.chkSkipSuccessMessage.Checked;

            // If SkipSuccessfulMessage is unchecked, also uncheck OpenCalibreAfterImport
            if (!this.chkSkipSuccessMessage.Checked)
            {
                this.chkOpenCalibreAfterImport.Checked = false;
            }
        }


        // Populate the checked list box with libraries from CalibreLibraryManager
        private void PopulateHiddenLibraries()
        {
            try
            {
                // Retrieve all libraries from CalibreLibraryManager.
                var libraries = CalibreLibraryManager.GetLibraries();
                if (libraries == null || !libraries.Any())
                {
                    MessageBox.Show(ResourceStrings.NoLibrariesRes, ResourceStrings.ErrorRes, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Read the hidden libraries setting.
                string hiddenSetting = CustomSettings.Config.AppSettings.Settings["language"].Value ?? "";
                var hiddenList = hiddenSetting.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                                              .Select(s => s.Trim())
                                              .ToList();

                // Populate the checked list box.
                clbLibraries.Items.Clear();
                foreach (var lib in libraries)
                {
                    int index = clbLibraries.Items.Add(lib);
                    if (hiddenList.Contains(lib.Name))
                        clbLibraries.SetItemChecked(index, true);
                }
                clbLibraries.DisplayMember = "Name";
            }
            catch (Exception ex)
            {
                Logger.LogThis($"Error populating hidden libraries: {ex.Message}");
            }
        }

        // control checkbox for logging
        private void chkLog_CheckedChanged(object sender, EventArgs e)
        {
            chkVerbose.Enabled = chkLog.Checked;
        }

        // Event handler for the language combo box
        private void comboBoxLanguage_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (isLoadingSettings) return;

            var selectedItem = comboBoxLanguage.SelectedItem as LanguageItem;
            if (selectedItem != null)
            {
                var selectedCulture = selectedItem.Culture;
                CustomSettings.Config.AppSettings.Settings["language"].Value = selectedCulture;
                CustomSettings.Save();

                // Update the application's culture
                CultureInfo culture = new CultureInfo(selectedCulture);
                System.Threading.Thread.CurrentThread.CurrentUICulture = culture;
                System.Threading.Thread.CurrentThread.CurrentCulture = culture;

                // Reload resource strings
                ResourceStrings.LoadResourceStrings();

                // Reload the form to apply the new language
                this.Controls.Clear();
                InitializeComponent();
                PopulateLanguageComboBox();
                LoadSettings();
                PopulateHiddenLibraries();

                // Initialize dynamic strings with the new language
                ApplyResourceStrings();
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

        // Save button click to save settings to the configuration file
        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                CustomSettings.Config.AppSettings.Settings["calibreFolder"].Value = txtCalibreFolder.Text;
                CustomSettings.Config.AppSettings.Settings["useSubmenu"].Value = chkUseSubmenu.Checked.ToString();
                CustomSettings.Config.AppSettings.Settings["logThis"].Value = chkLog.Checked.ToString();
                CustomSettings.Config.AppSettings.Settings["verboseLog"].Value = chkVerbose.Checked.ToString();
                CustomSettings.Config.AppSettings.Settings["skipSuccessMessage"].Value = chkSkipSuccessMessage.Checked.ToString();

                if (cmbAutomerge.SelectedItem != null)
                {
                    CustomSettings.Config.AppSettings.Settings["autoMerge"].Value = cmbAutomerge.SelectedItem.ToString();
                }

                CustomSettings.Config.AppSettings.Settings["autoKillCalibre"].Value = chkAutoKillCalibre.Checked.ToString();

                // Build the comma-separated hidden libraries list based on checked items.
                var hiddenLibNames = clbLibraries.CheckedItems
                    .OfType<CalibreLibrary>()
                    .Select(lib => lib.Name);
                CustomSettings.Config.AppSettings.Settings["hiddenLibraries"].Value = string.Join(";", hiddenLibNames);

                // Save the selected language setting
                if (comboBoxLanguage.SelectedItem != null)
                {
                    CustomSettings.Config.AppSettings.Settings["language"].Value = (string)((dynamic)comboBoxLanguage.SelectedItem).Culture;
                }

                CustomSettings.Config.AppSettings.Settings["autoCalibreOpen"].Value = chkOpenCalibreAfterImport.Checked.ToString();

                CustomSettings.Config.AppSettings.Settings["skipSuccessMessage"].Value = chkSkipSuccessMessage.Checked.ToString();

                CustomSettings.Save();

                MessageBox.Show(ResourceStrings.SettingsSavedRes, ResourceStrings.SettingsRes, MessageBoxButtons.OK, MessageBoxIcon.Information);
                Close();
            }
            catch (Exception ex)
            {
                Logger.LogThis($"Error saving settings: {ex.Message}");
            }
        }

        // we do this to prevent vs from rewriting literal strings on top of dynamic values
        private void SettingsForm_Load(object sender, EventArgs e)
        {
            ApplyResourceStrings();
        }

        private void ApplyResourceStrings()
        {
            this.btnSave.Text = ResourceStrings.SaveRes;
            this.btnCancel.Text = ResourceStrings.CancelRes;
            this.groupLanguage.Text = ResourceStrings.SelectLanguageRes;
            this.groupOtherOptions.Text = ResourceStrings.SettingsRes;
            this.chkAutoKillCalibre.Text = ResourceStrings.KillCalibreRes;
            this.chkLog.Text = ResourceStrings.LogEbooksRes;
            this.chkVerbose.Text = ResourceStrings.AlsoDebugLogRes;
            this.chkUseSubmenu.Text = ResourceStrings.UseSubmenuRes;
            this.groupPath.Text = ResourceStrings.PathToCalibreRes;
            this.groupHiddenLibraries.Text = ResourceStrings.HideLibrariesRes;
            this.groupDuplicate.Text = ResourceStrings.DuplicatesWhatRes;
            this.Text = ResourceStrings.NameSettingsFormRes;
            this.chkSkipSuccessMessage.Text = "Skip successful import message";
            this.chkOpenCalibreAfterImport.Text = "Open Calibre after import";

        }
    }

    public class LanguageItem
    {
        public string Culture { get; set; }
        public string Name { get; set; }
    }
}
