using System;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
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
            try
            {
                // Set the application culture before initializing components
                Logger.LogThis("Initializing SettingsForm with current culture", true);
                CultureManager.SetApplicationCulture();

                InitializeComponent();

                // Log current culture state
                Logger.LogThis($"Current thread culture: {Thread.CurrentThread.CurrentCulture.Name}, UI Culture: {Thread.CurrentThread.CurrentUICulture.Name}", true);

                PopulateLanguageComboBox();
                LoadSettings();
                PopulateHiddenLibraries();
                initialFontSize = this.Font.Size;
                initialFormSize = this.ClientSize;

                // Apply resource strings immediately
                ApplyResourceStrings();

                Logger.LogThis("SettingsForm initialized successfully", true);
            }
            catch (Exception ex)
            {
                Logger.LogThis($"Error initializing SettingsForm: {ex.Message}\n{ex.StackTrace}", true);
                MessageBox.Show($"Error initializing settings form: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Populate the language combo box with supported languages
        private void PopulateLanguageComboBox()
        {
            try
            {
                Logger.LogThis("Populating language combo box...", true);
                var cultures = Locales.GetSupportedCultures();
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

                // Populate the language combo box with available languages
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

                // Use CultureManager to set the application culture
                CultureManager.SetApplicationCulture();

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

        // Populate the checked list box with libraries from CalibreLibraryManager
        private void PopulateHiddenLibraries()
        {
            try
            {
                // Ensure culture is set correctly before showing any messages
                CultureManager.SetApplicationCulture();

                // Retrieve all libraries from CalibreLibraryManager.
                var libraries = CalibreLibraryManager.GetLibraries();
                if (libraries == null || !libraries.Any())
                {
                    MessageBox.Show(ResourceStrings.NoLibrariesRes, ResourceStrings.ErrorRes, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Read the hidden libraries setting.
                string hiddenSetting = CustomSettings.Config.AppSettings.Settings["hiddenLibraries"].Value ?? "";
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

        // we do this to prevent vs from rewriting literal strings on top of dynamic values
        private void SettingsForm_Load(object sender, EventArgs e)
        {
            // Ensure culture is set correctly before showing any messages
            // CultureManager.SetApplicationCulture();
            ApplyResourceStrings();
        }

        // This method applies the resource strings to the form controls.
        private void ApplyResourceStrings()
        {
            // Form title
            this.Text = ResourceStrings.NameSettingsFormRes;

            // Group boxes
            this.groupHiddenLibraries.Text = ResourceStrings.HideLibrariesRes;
            this.groupPath.Text = ResourceStrings.PathToCalibreRes;
            this.groupDuplicate.Text = ResourceStrings.DuplicatesWhatRes;
            this.groupLanguage.Text = ResourceStrings.SelectLanguageRes;
            this.groupOtherOptions.Text = ResourceStrings.SettingsRes;

            // Checkboxes
            this.chkUseSubmenu.Text = ResourceStrings.UseSubmenuRes;
            this.chkAutoKillCalibre.Text = ResourceStrings.KillCalibreRes;
            this.chkLog.Text = ResourceStrings.LogEbooksRes;
            this.chkVerbose.Text = ResourceStrings.AlsoDebugLogRes;
            this.chkSkipSuccessMessage.Text = ResourceStrings.SkipmessageRes;
            this.chkOpenCalibreAfterImport.Text = ResourceStrings.AutoOpenCalibreRes;

            // Buttons
            this.btnSave.Text = ResourceStrings.SaveRes;
            this.btnCancel.Text = ResourceStrings.CancelRes;

            CultureManager.ApplyRightToLeftLayout(this);
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

        // control checkbox for logging
        private void chkLog_CheckedChanged(object sender, EventArgs e)
        {
            chkVerbose.Enabled = chkLog.Checked;
        }

        // Event handler for the language combo box
        private void comboBoxLanguage_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (isLoadingSettings) return;

            Logger.LogThis("Language selection changed", true);

            var selectedItem = comboBoxLanguage.SelectedItem as LanguageItem;
            if (selectedItem != null)
            {
                // Log the selected item details to understand what's happening
                Logger.LogThis($"Selected culture: {selectedItem.Culture}, Display name: {selectedItem.Name}", true);

                // Save the selected culture code to settings
                var selectedCulture = selectedItem.Culture;
                CustomSettings.Config.AppSettings.Settings["language"].Value = selectedCulture;
                CustomSettings.Save();

                Logger.LogThis($"Saved culture to settings: {selectedCulture}", true);

                try
                {
                    // Set culture explicitly first
                    CultureInfo culture = new CultureInfo(selectedCulture);
                    Thread.CurrentThread.CurrentUICulture = culture;
                    Thread.CurrentThread.CurrentCulture = culture;

                    // Use CultureManager to set the application culture
                    Logger.LogThis("Calling SetApplicationCulture", true);
                    CultureManager.SetApplicationCulture();

                    // Check if we need to create a new form or can reuse this one
                    // For drastic UI language changes, sometimes a new form works better
                    Logger.LogThis("Creating a new settings form instance with the updated culture", true);

                    // Create and show a new settings form with the new culture
                    SettingsForm newForm = new SettingsForm();
                    newForm.StartPosition = FormStartPosition.CenterScreen;

                    // We'll hide this form instead of closing it to avoid any issues
                    this.Hide();

                    // Show the new form as a dialog
                    if (newForm.ShowDialog() == DialogResult.OK)
                    {
                        // If the new form was saved and closed with OK, we'll close this one too
                        this.DialogResult = DialogResult.OK;
                    }

                    // Always close this form when the new one is closed
                    this.Close();
                }
                catch (Exception ex)
                {
                    Logger.LogThis($"Error changing language: {ex.Message}\n{ex.StackTrace}", true);

                    // If something went wrong, ensure the form is still usable
                    MessageBox.Show($"Error changing language: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                Logger.LogThis("Selected item was null", true);
            }
        }

        // Save button click to save settings to the configuration file
        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                // Ensure culture is set correctly before showing any messages
                CultureManager.SetApplicationCulture();

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

        // Cancel button click to close the form
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }

    // This class represents a language item with culture and name properties.
    public class LanguageItem
    {
        public string Culture { get; set; }
        public string Name { get; set; }
    }
}
