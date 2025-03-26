using System;
using System.Globalization;
using System.Threading;
using System.Windows.Forms;

namespace CalibreImport
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            // Load the saved language setting
            var savedCulture = CustomSettings.Config.AppSettings.Settings["language"].Value;
            if (!string.IsNullOrEmpty(savedCulture))
            {
                CultureInfo culture = new CultureInfo(savedCulture);
                Thread.CurrentThread.CurrentUICulture = culture;
                Thread.CurrentThread.CurrentCulture = culture;
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
        }
    }
}
