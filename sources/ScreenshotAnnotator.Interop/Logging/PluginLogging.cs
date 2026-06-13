using System;

namespace ScreenshotAnnotator.Interop.Logging;

public interface IPluginLogger
{
    void Trace(string message);
    void Debug(string message);
    void Info(string message);
    void Warn(string message);
    void Error(string message, Exception? exception = null);
}

public interface IPluginLoggerFactory
{
    IPluginLogger GetLogger(string name);
}

public static class PluginLogging
{
    private static IPluginLoggerFactory? _factory;

    public static void Initialize(IPluginLoggerFactory factory)
    {
        _factory = factory;
    }

    public static IPluginLogger GetLogger(string name)
    {
        if (_factory is null)
            return NullPluginLogger.Instance;

        return _factory.GetLogger(name);
    }

    private sealed class NullPluginLogger : IPluginLogger
    {
        public static NullPluginLogger Instance { get; } = new();

        public void Trace(string message) { }
        public void Debug(string message) { }
        public void Info(string message) { }
        public void Warn(string message) { }
        public void Error(string message, Exception? exception = null) { }
    }
}
