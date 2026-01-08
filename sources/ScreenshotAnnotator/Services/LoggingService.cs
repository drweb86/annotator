using NLog;
using NLog.Config;
using NLog.Targets;
using System;
using System.IO;

namespace ScreenshotAnnotator.Services;

public static class LoggingService
{
    private static Logger? _logger;
    private static string? _logDirectory;

    public static void Initialize()
    {
        try
        {
            _logDirectory = GetLogDirectory();

            if (!Directory.Exists(_logDirectory))
            {
                Directory.CreateDirectory(_logDirectory);
            }

            var config = new LoggingConfiguration();

            var fileTarget = new FileTarget("logfile")
            {
                FileName = Path.Combine(_logDirectory, "annotator-${shortdate}.log"),
                Layout = "${longdate}|${level:uppercase=true}|${logger}|${message}${onexception:inner=${newline}${exception:format=tostring}}",
                ArchiveEvery = FileArchivePeriod.Day,
                MaxArchiveFiles = 30,
                KeepFileOpen = false
            };

            config.AddRule(LogLevel.Debug, LogLevel.Fatal, fileTarget);

            LogManager.Configuration = config;
            _logger = LogManager.GetCurrentClassLogger();

            _logger.Info("=== Screenshot Annotator Started ===");
            _logger.Info($"OS: {Environment.OSVersion}");
            _logger.Info($"Runtime: {Environment.Version}");
            _logger.Info($"Log Directory: {_logDirectory}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to initialize logging: {ex}");
        }
    }

    public static string GetLogDirectory()
    {
        var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        return Path.Combine(appDataPath, "ScreenshotAnnotator", "Logs");
    }

    public static Logger GetLogger(string name)
    {
        return LogManager.GetLogger(name);
    }

    public static void Shutdown()
    {
        try
        {
            _logger?.Info("=== Screenshot Annotator Shutdown ===");
            LogManager.Shutdown();
        }
        catch
        {
            // Ignore shutdown errors
        }
    }
}
