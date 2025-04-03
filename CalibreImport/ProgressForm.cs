using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CalibreImport
{
    public partial class ProgressForm : Form
    {
        public ProgressForm()
        {
            Logger.LogThis("ProgressForm constructor called.", true);
            InitializeComponent();

            // Register the Load event to apply resource strings
            this.Load += ProgressForm_Load;

            // Set a default text for the form while it's loading
            this.Text = "Importing...";
        }

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

        private void ProgressForm_Load(object sender, EventArgs e)
        {
            ApplyResourceStrings();
        }

        private void ApplyResourceStrings()
        {
            this.borderlessGroupBox1.Text = ResourceStrings.ImportingRes;
            this.Text = ResourceStrings.NameAppRes;
        }
    }

}