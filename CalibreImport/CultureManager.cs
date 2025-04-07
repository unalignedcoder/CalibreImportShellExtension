using System;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

namespace CalibreImport
{
    // This class manages the application's culture settings.
    // It sets the culture based on user preferences or system settings.
    public static class CultureManager
    {
        // There isn't a good C# system to retrieve the system UI language.
        // CultureInfo.CurrentUICulture is not reliable for this purpose.
        // The GetUserDefaultUILanguage function retrieves the default user interface language
        // for the system via the Kernel32 Windows dll file.
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        private static extern ushort GetUserDefaultUILanguage();

        // This method sets the application's culture based on user preferences or system settings.
        public static void SetApplicationCulture()
        {
            // retrieve System UI language
            ushort langId = GetUserDefaultUILanguage();
            CultureInfo detectedCulture = new CultureInfo(langId);

            // Log the result of GetUserDefaultUILanguage
            Logger.LogThis($"Detected system UI culture using GetUserDefaultUILanguage: {detectedCulture.Name} ({detectedCulture.DisplayName})", true);

            // config file Language value
            var savedCulture = CustomSettings.Config.AppSettings.Settings["language"].Value;

            // If the config file has no value for the language setting
            if (string.IsNullOrEmpty(savedCulture))
            {
                // assign System UI Culture to the config file
                CustomSettings.Config.AppSettings.Settings["language"].Value = detectedCulture.Name;
                CustomSettings.Save();
                Logger.LogThis("Assigning System UI culture to the app.)", true);

            }
            // If the config file has a value for the language setting
            else
            {
                // Set the culture based on the saved value
                detectedCulture = new CultureInfo(savedCulture);
                Logger.LogThis("Using UI culture indicated in the config file.", true);
            }

            // make sure the application reflects the culture detected above
            Thread.CurrentThread.CurrentUICulture = detectedCulture;
            Thread.CurrentThread.CurrentCulture = detectedCulture;
            ResourceStrings.LoadResourceStrings();

            // log it
            Logger.LogThis($"Application culture set to the following culture: {detectedCulture.DisplayName}", true);
        }

        // This method applies a right-to-left layout to the form if the current culture is RTL.
        public static void ApplyRightToLeftLayout(Form form)
        {
            // Check if current culture is RTL
            bool isRtl = Thread.CurrentThread.CurrentCulture.TextInfo.IsRightToLeft;

            form.RightToLeft = isRtl ? RightToLeft.Yes : RightToLeft.No;
            form.RightToLeftLayout = isRtl;
        }
    }
}
