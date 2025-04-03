using System;
using System.Reflection;
using System.Windows.Forms;

namespace CalibreImport
{
    partial class ProgressForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProgressForm));
            this.borderlessGroupBox1 = new System.Windows.Forms.GroupBox();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.borderlessGroupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // borderlessGroupBox1
            // 
            this.borderlessGroupBox1.Controls.Add(this.progressBar);
            this.borderlessGroupBox1.Dock = System.Windows.Forms.DockStyle.Top;
            this.borderlessGroupBox1.Location = new System.Drawing.Point(20, 10);
            this.borderlessGroupBox1.Name = "borderlessGroupBox1";
            this.borderlessGroupBox1.Size = new System.Drawing.Size(366, 62);
            this.borderlessGroupBox1.TabIndex = 1;
            this.borderlessGroupBox1.TabStop = false;
            // 
            // progressBar
            // 
            this.progressBar.Dock = System.Windows.Forms.DockStyle.Fill;
            this.progressBar.Location = new System.Drawing.Point(3, 22);
            this.progressBar.MarqueeAnimationSpeed = 30;
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(360, 37);
            this.progressBar.TabIndex = 0;
            // 
            // ProgressForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(406, 87);
            this.Controls.Add(this.borderlessGroupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ProgressForm";
            this.Padding = new System.Windows.Forms.Padding(20, 10, 20, 10);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.borderlessGroupBox1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.TopMost = true;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterScreen;

        }
        #endregion

        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.GroupBox borderlessGroupBox1;

    }
}