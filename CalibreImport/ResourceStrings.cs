using System;
using System.Globalization;

namespace CalibreImport
{
    public static class ResourceStrings
    {
        public static string AlsoDebugLogRes;
        public static string CalibreProcessesRunningRes;
        public static string CalibreRunning2Res;
        public static string CalibreRunningRes;
        public static string CancelRes;
        public static string DoYouWantToProceedRes;
        public static string DuplicatesWhatRes;
        public static string ErrorLaunchingRes;
        public static string ErrorRes;
        public static string HideLibrariesRes;
        public static string ImportBtnRes;
        public static string ImportFailureRes;
        public static string ImportFormRes;
        public static string ImportingRes;
        public static string ImportSuccessRes;
        public static string InvalidFilesRes;
        public static string InvalidSelectionRes;
        public static string KillCalibreRes;
        public static string LogEbooksRes;
        public static string MenuTextRes;
        public static string NameAppRes;
        public static string NameAppShortRes;
        public static string NameSettingsFormRes;
        public static string NoLibrariesRes;
        public static string PathToCalibreRes;
        public static string PleaseSelectLibraryRes;
        public static string RegistrationFailedRes;
        public static string SaveRes;
        public static string SelectLanguageRes;
        public static string SelectLibraryRes;
        public static string SetEntryNameRes;
        public static string SettingsRes;
        public static string SettingsSavedRes;
        public static string UseSubmenuRes;
        public static string SkipmessageRes;
        public static string AutoOpenCalibreRes;

        static ResourceStrings()
        {
            try
            {
                LoadResourceStrings();
            }
            catch (Exception ex)
            {
                // Log the exception and rethrow
                Logger.LogThis($"Error initializing ResourceStrings: {ex.Message}", true);
                throw;
            }
        }

        public static void LoadResourceStrings()
        {
            try
            {
                AlsoDebugLogRes = Locales.GetString("AlsoDebugLog");
                CalibreProcessesRunningRes = Locales.GetString("CalibreProcessesRunning");
                CalibreRunning2Res = Locales.GetString("CalibreRunning2");
                CalibreRunningRes = Locales.GetString("CalibreRunning");
                CancelRes = Locales.GetString("Cancel");
                DoYouWantToProceedRes = Locales.GetString("DoYouWantToProceed");
                DuplicatesWhatRes = Locales.GetString("DuplicatesWhat");
                ErrorLaunchingRes = Locales.GetString("ErrorLaunching");
                ErrorRes = Locales.GetString("Error");
                HideLibrariesRes = Locales.GetString("HideLibraries");
                ImportBtnRes = Locales.GetString("ImportBtn");
                ImportFailureRes = Locales.GetString("ImportFailure");
                ImportFormRes = Locales.GetString("ImportForm");
                ImportingRes = Locales.GetString("Importing");
                ImportSuccessRes = Locales.GetString("ImportSuccess");
                InvalidFilesRes = Locales.GetString("InvalidFiles");
                InvalidSelectionRes = Locales.GetString("InvalidSelection");
                KillCalibreRes = Locales.GetString("KillCalibre");
                LogEbooksRes = Locales.GetString("LogEbooks");
                MenuTextRes = Locales.GetString("MenuText");
                NameAppRes = Locales.GetString("NameApp");
                NameAppShortRes = Locales.GetString("NameAppShort");
                NameSettingsFormRes = Locales.GetString("NameSettingsForm");
                NoLibrariesRes = Locales.GetString("NoLibraries");
                PathToCalibreRes = Locales.GetString("PathToCalibre");
                PleaseSelectLibraryRes = Locales.GetString("PleaseSelectLibrary");
                RegistrationFailedRes = Locales.GetString("RegistrationFailed");
                SaveRes = Locales.GetString("Save");
                SelectLanguageRes = Locales.GetString("SelectLanguage");
                SelectLibraryRes = Locales.GetString("SelectLibrary");
                SetEntryNameRes = Locales.GetString("SetEntryName");
                SettingsRes = Locales.GetString("Settings");
                SettingsSavedRes = Locales.GetString("SettingsSaved");
                UseSubmenuRes = Locales.GetString("UseSubmenu");
                SkipmessageRes = Locales.GetString("Skipmessage");
                AutoOpenCalibreRes = Locales.GetString("AutoOpenCalibre");
            }

            catch (Exception ex)
            {
                // Log the exception and rethrow
                Logger.LogThis($"Error loading resource strings: {ex.Message}", true);
                throw;
            }
        }
    }
}
