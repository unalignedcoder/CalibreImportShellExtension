using System;
using System.Diagnostics;
using System.IO;

namespace CalibreImport
{
    public static class Logger
    {
        public static string LogFile { get; set; }

        static Logger()
        {
            try
            {
                var dllDirectory = Path.GetDirectoryName(typeof(Logger).Assembly.Location);
                var logNameSetting = CustomSettings.Config.AppSettings.Settings["logName"];
                var logName = logNameSetting?.Value ?? "CalibreImport.log";
                LogFile = Path.Combine(dllDirectory, logName);
            }
            catch (Exception ex)
            {
                // Log the exception to a default log file
                var defaultLogFile = Path.Combine(Path.GetDirectoryName(typeof(Logger).Assembly.Location), "DefaultLog.log");
                File.AppendAllText(defaultLogFile, $"{DateTime.Now}: Error initializing Logger: {ex.Message}{Environment.NewLine}");
                throw;
            }
        }

        public static void LogThis(string message, bool isVerbose = false)
        {
            try
            {
                // Read the settings directly from the configuration file
                bool logIt = bool.Parse(CustomSettings.Config.AppSettings.Settings["logThis"].Value ?? "true");
                bool verboseLog = bool.Parse(CustomSettings.Config.AppSettings.Settings["verboseLog"].Value ?? "false");

                // if logging is enabled
                if (logIt)
                {
                    // If the log message itself is verbose and verbose logging is enabled, enrich it and log it
                    if (isVerbose && verboseLog)
                    {
                        string logMessage = $"{DateTime.Now}: {message}";

                        // this part enrichs the message with debug info
                        var stackFrame = new StackFrame(1, true);
                        var method = stackFrame.GetMethod();
                        var lineNumber = stackFrame.GetFileLineNumber();
                        logMessage += $" (Method: {method.Name}, Line: {lineNumber})";

                        File.AppendAllText(LogFile, $"{logMessage}{Environment.NewLine}");
                    }
                    // If the message isn't verbose, log it
                    else if (!isVerbose)
                    {
                        string logMessage = $"{DateTime.Now}: {message}";
                        File.AppendAllText(LogFile, $"{logMessage}{Environment.NewLine}");
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the exception to a default log file
                var defaultLogFile = Path.Combine(Path.GetDirectoryName(typeof(Logger).Assembly.Location), "DefaultLog.log");
                File.AppendAllText(defaultLogFile, $"{DateTime.Now}: Error logging message: {ex.Message}{Environment.NewLine}");
                throw;
            }
        }
    }
}
