namespace CalibreImport
{
    partial class SettingsForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingsForm));
            this.btnSave = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.groupBox1 = new global::CalibreImport.BorderlessGroupBox();
            this.comboBoxLanguage = new System.Windows.Forms.ComboBox();
            this.groupButtons = new global::CalibreImport.BorderlessGroupBox();
            this.groupOtherOptions = new global::CalibreImport.BorderlessGroupBox();
            this.chkAutoKillCalibre = new System.Windows.Forms.CheckBox();
            this.chkVerbose = new System.Windows.Forms.CheckBox();
            this.chkLog = new System.Windows.Forms.CheckBox();
            this.chkUseSubmenu = new System.Windows.Forms.CheckBox();
            this.groupDuplicate = new global::CalibreImport.BorderlessGroupBox();
            this.cmbAutomerge = new System.Windows.Forms.ComboBox();
            this.groupPath = new global::CalibreImport.BorderlessGroupBox();
            this.txtCalibreFolder = new System.Windows.Forms.TextBox();
            this.groupHiddenLibraries = new global::CalibreImport.BorderlessGroupBox();
            this.clbLibraries = new System.Windows.Forms.CheckedListBox();
            this.groupBox1.SuspendLayout();
            this.groupButtons.SuspendLayout();
            this.groupOtherOptions.SuspendLayout();
            this.groupDuplicate.SuspendLayout();
            this.groupPath.SuspendLayout();
            this.groupHiddenLibraries.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnSave
            // 
            this.btnSave.Text = ResourceStrings.SaveRes;
            this.btnSave.Name = "btnSave";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Text = ResourceStrings.CancelRes;
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.comboBoxLanguage);
            this.groupBox1.Text = ResourceStrings.SelectLanguageRes;
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // comboBoxLanguage
            // 
            this.comboBoxLanguage.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxLanguage.FormattingEnabled = true;
            this.comboBoxLanguage.Name = "comboBoxLanguage";
            this.comboBoxLanguage.SelectedIndexChanged += new System.EventHandler(this.comboBoxLanguage_SelectedIndexChanged);
            // 
            // groupButtons
            // 
            this.groupButtons.Controls.Add(this.btnCancel);
            this.groupButtons.Controls.Add(this.btnSave);
            this.groupButtons.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.groupButtons.Name = "groupButtons";
            this.groupButtons.TabStop = false;
            // 
            // groupOtherOptions
            // 
            this.groupOtherOptions.Controls.Add(this.chkAutoKillCalibre);
            this.groupOtherOptions.Controls.Add(this.chkVerbose);
            this.groupOtherOptions.Controls.Add(this.chkLog);
            this.groupOtherOptions.Controls.Add(this.chkUseSubmenu);
            this.groupOtherOptions.Text = ResourceStrings.SettingsRes;
            this.groupOtherOptions.Name = "groupOtherOptions";
            this.groupOtherOptions.TabStop = false;
            // 
            // chkAutoKillCalibre
            // 
            this.chkAutoKillCalibre.Text = ResourceStrings.KillCalibreRes;
            this.chkAutoKillCalibre.Name = "chkAutoKillCalibre";
            this.chkAutoKillCalibre.UseVisualStyleBackColor = true;
            // 
            // chkVerbose
            // 
            this.chkVerbose.Text = ResourceStrings.AlsoDebugLogRes;
            this.chkVerbose.Name = "chkVerbose";
            this.chkVerbose.UseVisualStyleBackColor = true;
            // 
            // chkLog
            // 
            this.chkLog.Text = ResourceStrings.LogEbooksRes;
            this.chkLog.Name = "chkLog";
            this.chkLog.UseVisualStyleBackColor = true;
            this.chkLog.CheckedChanged += new System.EventHandler(this.chkLog_CheckedChanged);
            // 
            // chkUseSubmenu
            // 
            this.chkUseSubmenu.Text = ResourceStrings.UseSubmenuRes;
            this.chkUseSubmenu.Name = "chkUseSubmenu";
            this.chkUseSubmenu.UseVisualStyleBackColor = true;
            // 
            // groupDuplicate
            // 
            this.groupDuplicate.Controls.Add(this.cmbAutomerge);
            this.groupDuplicate.Text = ResourceStrings.DuplicatesWhatRes;
            this.groupDuplicate.Name = "groupDuplicate";
            this.groupDuplicate.TabStop = false;
            // 
            // cmbAutomerge
            // 
            this.cmbAutomerge.FormattingEnabled = true;
            this.cmbAutomerge.Name = "cmbAutomerge";
            // 
            // groupPath
            // 
            this.groupPath.Controls.Add(this.txtCalibreFolder);
            this.groupPath.Text = ResourceStrings.PathToCalibreRes;
            this.groupPath.Name = "groupPath";
            this.groupPath.TabStop = false;
            // 
            // txtCalibreFolder
            // 
            this.txtCalibreFolder.Name = "txtCalibreFolder";
            // 
            // groupHiddenLibraries
            // 
            this.groupHiddenLibraries.Controls.Add(this.clbLibraries);
            this.groupHiddenLibraries.Text = ResourceStrings.HideLibrariesRes;
            this.groupHiddenLibraries.Name = "groupHiddenLibraries";
            this.groupHiddenLibraries.TabStop = false;
            // 
            // clbLibraries
            // 
            this.clbLibraries.FormattingEnabled = true;
            this.clbLibraries.Name = "clbLibraries";
            // 
            // SettingsForm
            // 
            this.AcceptButton = this.btnSave;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoValidate = System.Windows.Forms.AutoValidate.Disable;
            this.CancelButton = this.btnCancel;
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupButtons);
            this.Controls.Add(this.groupOtherOptions);
            this.Controls.Add(this.groupDuplicate);
            this.Controls.Add(this.groupPath);
            this.Controls.Add(this.groupHiddenLibraries);
            this.Name = "SettingsForm";
            this.Text = ResourceStrings.NameSettingsFormRes;
            this.Icon = new System.Drawing.Icon("CalibreImport/Resources/MainAppIcon.ico");
            this.groupBox1.ResumeLayout(false);
            this.groupButtons.ResumeLayout(false);
            this.groupOtherOptions.ResumeLayout(false);
            this.groupOtherOptions.PerformLayout();
            this.groupDuplicate.ResumeLayout(false);
            this.groupPath.ResumeLayout(false);
            this.groupPath.PerformLayout();
            this.groupHiddenLibraries.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TextBox txtCalibreFolder;
        private System.Windows.Forms.ComboBox cmbAutomerge;
        private System.Windows.Forms.CheckBox chkUseSubmenu;
        private System.Windows.Forms.CheckBox chkLog;
        private System.Windows.Forms.CheckBox chkVerbose;
        private System.Windows.Forms.CheckBox chkAutoKillCalibre;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnCancel;
        private global::CalibreImport.BorderlessGroupBox groupHiddenLibraries;
        private System.Windows.Forms.CheckedListBox clbLibraries;
        private global::CalibreImport.BorderlessGroupBox groupPath;
        private global::CalibreImport.BorderlessGroupBox groupDuplicate;
        private global::CalibreImport.BorderlessGroupBox groupOtherOptions;
        private global::CalibreImport.BorderlessGroupBox groupButtons;
        private System.Windows.Forms.ComboBox comboBoxLanguage;
        private global::CalibreImport.BorderlessGroupBox groupBox1;
    }
}
