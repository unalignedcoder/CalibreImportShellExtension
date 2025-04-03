public class AppSettings
{
    public string CalibreFolder { get; set; } = @"C:\Program Files\Calibre2";
    public bool UseSubmenu { get; set; } = true;
    public bool LogThis { get; set; } = true;
    public bool VerboseLog { get; set; } = false;
    public bool AutoKillCalibre { get; set; } = false;
    public string AutoMerge { get; set; } = "overwrite";
    public string HiddenLibraries { get; set; } = "";
    public string Language { get; set; } = "en";
}
