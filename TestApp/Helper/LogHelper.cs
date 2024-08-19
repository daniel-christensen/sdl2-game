using Serilog;

namespace TestApp.Helper
{
    /// <summary>
    /// Provides utility functions for logging.
    /// </summary>
    internal static class LogHelper
    {
        // Constants for log configuration
        private const int _maxLogFiles = 10;
        private const string _logFileExtension = ".log";
        private const string _logDirectory = "logs";

        // Lazily evaluated log file name, generated from the current timestamp.
        private static readonly string _logFileName = TimeStamp;

        /// <summary>
        /// Gets a timestamp formatted for log file naming.
        /// </summary>
        internal static string TimeStamp => DateTime.Now.ToString("yyyyMMdd_HHmmss");

        /// <summary>
        /// Gets the full path for the current instance log file.
        /// </summary>
        internal static string InstanceLogDirectory => GetInstanceLogDirectory();

        /// <summary>
        /// Constructs the full path for the log file using configured directory and file extension.
        /// </summary>
        private static string GetInstanceLogDirectory()
        {
            string path = Path.Combine(_logDirectory, _logFileName);
            return Path.ChangeExtension(path, _logFileExtension);
        }

        /// <summary>
        /// Ensures that the number of log files does not exceed the configured maximum.
        /// </summary>
        /// <param name="logDirectory">The directory to check for log files.</param>
        /// <param name="maxFiles">The maximum number of log files to retain.</param>
        private static void EnsureMaxLogFiles(string logDirectory, int maxFiles)
        {
            var directoryInfo = new DirectoryInfo(logDirectory);
            var files = directoryInfo.GetFiles("*.log")
                                     .OrderByDescending(f => f.CreationTime)
                                     .Skip(maxFiles);

            foreach (var file in files)
                file.Delete();
        }

        /// <summary>
        /// Initialises Serilog logging configuration.
        /// </summary>
        internal static void Initialise()
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.Console()
                .WriteTo.File(InstanceLogDirectory, rollingInterval: RollingInterval.Infinite)
                .CreateLogger();

            EnsureMaxLogFiles(_logDirectory, _maxLogFiles);
        }
    }
}
