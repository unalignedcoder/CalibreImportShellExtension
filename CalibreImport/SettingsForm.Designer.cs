using System;
using System.Reflection;
using System.Windows.Forms;

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
            this.comboBoxLanguage = new System.Windows.Forms.ComboBox();
            this.chkAutoKillCalibre = new System.Windows.Forms.CheckBox();
            this.chkLog = new System.Windows.Forms.CheckBox();
            this.chkVerbose = new System.Windows.Forms.CheckBox();
            this.chkUseSubmenu = new System.Windows.Forms.CheckBox();
            this.cmbAutomerge = new System.Windows.Forms.ComboBox();
            this.txtCalibreFolder = new System.Windows.Forms.TextBox();
            this.clbLibraries = new System.Windows.Forms.CheckedListBox();
            this.groupLanguage = new System.Windows.Forms.GroupBox();
            this.groupButtons = new System.Windows.Forms.GroupBox();
            this.groupDuplicate = new System.Windows.Forms.GroupBox();
            this.groupHiddenLibraries = new System.Windows.Forms.GroupBox();
            this.groupOtherOptions = new System.Windows.Forms.GroupBox();
            this.chkOpenCalibreAfterImport = new System.Windows.Forms.CheckBox();
            this.chkSkipSuccessMessage = new System.Windows.Forms.CheckBox();
            this.groupPath = new System.Windows.Forms.GroupBox();
            this.groupLanguage.SuspendLayout();
            this.groupButtons.SuspendLayout();
            this.groupDuplicate.SuspendLayout();
            this.groupHiddenLibraries.SuspendLayout();
            this.groupOtherOptions.SuspendLayout();
            this.groupPath.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnSave
            // 
            resources.ApplyResources(this.btnSave, "btnSave");
            this.btnSave.Name = "btnSave";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnCancel
            // 
            resources.ApplyResources(this.btnCancel, "btnCancel");
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // comboBoxLanguage
            // 
            resources.ApplyResources(this.comboBoxLanguage, "comboBoxLanguage");
            this.comboBoxLanguage.FormattingEnabled = true;
            this.comboBoxLanguage.Name = "comboBoxLanguage";
            this.comboBoxLanguage.SelectedIndexChanged += new System.EventHandler(this.comboBoxLanguage_SelectedIndexChanged);
            // 
            // chkAutoKillCalibre
            // 
            resources.ApplyResources(this.chkAutoKillCalibre, "chkAutoKillCalibre");
            this.chkAutoKillCalibre.Name = "chkAutoKillCalibre";
            this.chkAutoKillCalibre.UseVisualStyleBackColor = true;
            // 
            // chkLog
            // 
            resources.ApplyResources(this.chkLog, "chkLog");
            this.chkLog.Name = "chkLog";
            this.chkLog.UseVisualStyleBackColor = true;
            this.chkLog.CheckedChanged += new System.EventHandler(this.chkLog_CheckedChanged);
            // 
            // chkVerbose
            // 
            resources.ApplyResources(this.chkVerbose, "chkVerbose");
            this.chkVerbose.Name = "chkVerbose";
            this.chkVerbose.UseVisualStyleBackColor = true;
            // 
            // chkUseSubmenu
            // 
            resources.ApplyResources(this.chkUseSubmenu, "chkUseSubmenu");
            this.chkUseSubmenu.Name = "chkUseSubmenu";
            this.chkUseSubmenu.UseVisualStyleBackColor = true;
            // 
            // cmbAutomerge
            // 
            resources.ApplyResources(this.cmbAutomerge, "cmbAutomerge");
            this.cmbAutomerge.FormattingEnabled = true;
            this.cmbAutomerge.Name = "cmbAutomerge";
            // 
            // txtCalibreFolder
            // 
            resources.ApplyResources(this.txtCalibreFolder, "txtCalibreFolder");
            this.txtCalibreFolder.Name = "txtCalibreFolder";
            // 
            // clbLibraries
            // 
            resources.ApplyResources(this.clbLibraries, "clbLibraries");
            this.clbLibraries.FormattingEnabled = true;
            this.clbLibraries.Name = "clbLibraries";
            // 
            // groupLanguage
            // 
            this.groupLanguage.Controls.Add(this.comboBoxLanguage);
            resources.ApplyResources(this.groupLanguage, "groupLanguage");
            this.groupLanguage.Name = "groupLanguage";
            this.groupLanguage.TabStop = false;
            // 
            // groupButtons
            // 
            this.groupButtons.Controls.Add(this.btnCancel);
            this.groupButtons.Controls.Add(this.btnSave);
            resources.ApplyResources(this.groupButtons, "groupButtons");
            this.groupButtons.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.groupButtons.Name = "groupButtons";
            this.groupButtons.TabStop = false;
            // 
            // groupDuplicate
            // 
            this.groupDuplicate.Controls.Add(this.cmbAutomerge);
            resources.ApplyResources(this.groupDuplicate, "groupDuplicate");
            this.groupDuplicate.Name = "groupDuplicate";
            this.groupDuplicate.TabStop = false;
            // 
            // groupHiddenLibraries
            // 
            this.groupHiddenLibraries.Controls.Add(this.clbLibraries);
            resources.ApplyResources(this.groupHiddenLibraries, "groupHiddenLibraries");
            this.groupHiddenLibraries.Name = "groupHiddenLibraries";
            this.groupHiddenLibraries.TabStop = false;
            // 
            // groupOtherOptions
            // 
            this.groupOtherOptions.Controls.Add(this.chkOpenCalibreAfterImport);
            this.groupOtherOptions.Controls.Add(this.chkSkipSuccessMessage);
            this.groupOtherOptions.Controls.Add(this.chkVerbose);
            this.groupOtherOptions.Controls.Add(this.chkLog);
            this.groupOtherOptions.Controls.Add(this.chkAutoKillCalibre);
            this.groupOtherOptions.Controls.Add(this.chkUseSubmenu);
            resources.ApplyResources(this.groupOtherOptions, "groupOtherOptions");
            this.groupOtherOptions.Name = "groupOtherOptions";
            this.groupOtherOptions.TabStop = false;
            // 
            // chkOpenCalibreAfterImport
            // 
            resources.ApplyResources(this.chkOpenCalibreAfterImport, "chkOpenCalibreAfterImport");
            this.chkOpenCalibreAfterImport.Enabled = this.chkSkipSuccessMessage.Checked;
            this.chkOpenCalibreAfterImport.Name = "chkOpenCalibreAfterImport";
            this.chkOpenCalibreAfterImport.UseVisualStyleBackColor = true;
            // 
            // chkSkipSuccessMessage
            // 
            resources.ApplyResources(this.chkSkipSuccessMessage, "chkSkipSuccessMessage");
            this.chkSkipSuccessMessage.Name = "chkSkipSuccessMessage";
            this.chkSkipSuccessMessage.UseVisualStyleBackColor = true;
            this.chkSkipSuccessMessage.CheckedChanged += new System.EventHandler(this.chkSkipSuccessMessage_CheckedChanged);
            // 
            // groupPath
            // 
            this.groupPath.Controls.Add(this.txtCalibreFolder);
            resources.ApplyResources(this.groupPath, "groupPath");
            this.groupPath.Name = "groupPath";
            this.groupPath.TabStop = false;
            // 
            // SettingsForm
            // 
            this.AcceptButton = this.btnSave;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoValidate = System.Windows.Forms.AutoValidate.Disable;
            this.CancelButton = this.btnCancel;
            this.Controls.Add(this.groupOtherOptions);
            this.Controls.Add(this.groupLanguage);
            this.Controls.Add(this.groupButtons);
            this.Controls.Add(this.groupDuplicate);
            this.Controls.Add(this.groupPath);
            this.Controls.Add(this.groupHiddenLibraries);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "SettingsForm";
            this.TopMost = true;
            this.groupLanguage.ResumeLayout(false);
            this.groupButtons.ResumeLayout(false);
            this.groupButtons.PerformLayout();
            this.groupDuplicate.ResumeLayout(false);
            this.groupHiddenLibraries.ResumeLayout(false);
            this.groupOtherOptions.ResumeLayout(false);
            this.groupOtherOptions.PerformLayout();
            this.groupPath.ResumeLayout(false);
            this.groupPath.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupLanguage;
        private System.Windows.Forms.GroupBox groupPath;
        private System.Windows.Forms.GroupBox groupDuplicate;
        private System.Windows.Forms.GroupBox groupOtherOptions;
        private System.Windows.Forms.GroupBox groupButtons;
        private System.Windows.Forms.GroupBox groupHiddenLibraries;
        private System.Windows.Forms.TextBox txtCalibreFolder;
        private System.Windows.Forms.ComboBox cmbAutomerge;
        private System.Windows.Forms.CheckBox chkUseSubmenu;
        private System.Windows.Forms.CheckBox chkLog;
        private System.Windows.Forms.CheckBox chkVerbose;
        private System.Windows.Forms.CheckBox chkAutoKillCalibre;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.CheckedListBox clbLibraries;
        private System.Windows.Forms.ComboBox comboBoxLanguage;
        private System.Windows.Forms.CheckBox chkSkipSuccessMessage;
        private System.Windows.Forms.CheckBox chkOpenCalibreAfterImport;
    }
}
