using Avalonia.Threading;
using NLog;
using SharpHook;
using SharpHook.Native;
using System;
using System.Threading.Tasks;

namespace ScreenshotAnnotator.Services;

public class GlobalHotkeyService : IDisposable
{
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    public event Func<Task>? PrintScreenPressed;

    private IGlobalHook? _hook;

    public bool Enabled { get; set; }

    public bool IsRunning { get; private set; }

    public void Start()
    {
        try
        {
            _hook = new TaskPoolGlobalHook();
            _hook.KeyPressed += OnKeyPressed;
            _hook.RunAsync();
            IsRunning = true;
        }
        catch (Exception ex)
        {
            Logger.Warn(ex, "Failed to start global hotkey service.");
        }
    }

    private void OnKeyPressed(object? sender, KeyboardHookEventArgs e)
    {
        if (!Enabled) return;
        if (e.Data.KeyCode == KeyCode.VcPrintScreen)
        {
            var handler = PrintScreenPressed;
            if (handler is not null)
            {
                Dispatcher.UIThread.InvokeAsync(async () =>
                {
                    try { await handler(); }
                    catch (Exception ex) { Logger.Error(ex, "Error invoking PrintScreen handler."); }
                });
            }
        }
    }

    public void Dispose()
    {
        if (_hook is not null)
        {
            _hook.KeyPressed -= OnKeyPressed;
            _hook.Dispose();
            _hook = null;
        }
        IsRunning = false;
    }
}
