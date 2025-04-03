using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
//using System.Runtime.InteropServices;

namespace CalibreImport
{
    public static class Logger
    {
        private static readonly string assemblyName;
        public static string LogFile { get; set; }

        static Logger()
        {
            try
            {
                assemblyName = Assembly.GetExecutingAssembly().GetName().Name;
                string LogName = $"{assemblyName}.log";
                string LogDirectory;

                if (CheckPortable.IsPortable())
                {
                    LogDirectory = Path.GetDirectoryName(typeof(Logger).Assembly.Location);
                }
                else
                {
                    LogDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), assemblyName);
                }

                // Ensure the directory exists
                if (!Directory.Exists(LogDirectory))
                {
                    Directory.CreateDirectory(LogDirectory);
                }

                LogFile = Path.Combine(LogDirectory, LogName);

                // Ensure the log file is created if it doesn't exist
                if (!File.Exists(LogFile))
                {
                    File.Create(LogFile).Dispose();
                }

                // Log initialization success
                File.AppendAllText(LogFile, $"{DateTime.Now}: Logger initialized successfully.{Environment.NewLine}");
            }
            catch (Exception ex)
            {
                // Log the exceptions to a default log file in the AppData directory
                var defaultLogFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), assemblyName, "Initial.log");
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
                // Log the exceptions to a default log file in the AppData directory
                var defaultLogFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), assemblyName, "Initial.log");
                File.AppendAllText(defaultLogFile, $"{DateTime.Now}: Error logging message: {ex.Message}{Environment.NewLine}");
                throw;
            }
        }
    }
}
