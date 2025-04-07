using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CalibreImport
{
    public partial class ProgressForm : Form
    {
        // This constructor initializes the ProgressForm.
        public ProgressForm()
        {
            // Set application culture before initializing components
            CultureManager.SetApplicationCulture();

            Logger.LogThis("ProgressForm constructor called.", true);
            InitializeComponent();

            // Register the Load event to apply resource strings
            this.Load += ProgressForm_Load;

            // Set a default text for the form while it's loading
            this.Text = ResourceStrings.ImportingRes;
        }

        // This method updates the progress bar value.
        public void UpdateProgress(int progress)
        {
            Logger.LogThis($"Updating progress to {progress}%.", true);

            if (progressBar.InvokeRequired)
            {
                progressBar.Invoke(new Action(() => progressBar.Value = progress));
            }
            else
            {
                progressBar.Value = progress;
            }
        }

        // This method is called when the form is loaded.
        private void ProgressForm_Load(object sender, EventArgs e)
        {
            // Set application culture before initializing components
            CultureManager.SetApplicationCulture();
            ApplyResourceStrings();
        }

        // This method applies resource strings to the form controls.
        private void ApplyResourceStrings()
        {
            this.borderlessGroupBox1.Text = ResourceStrings.ImportingRes;
            this.Text = ResourceStrings.NameAppRes;
            CultureManager.ApplyRightToLeftLayout(this);
        }
    }
}