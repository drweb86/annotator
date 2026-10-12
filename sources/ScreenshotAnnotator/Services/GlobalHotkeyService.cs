using Avalonia.Threading;
using NLog;
using SharpHook;
using SharpHook.Data;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ScreenshotAnnotator.Services;

public class GlobalHotkeyService : IDisposable
{
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    private const EventMask ModifierMask = EventMask.Alt | EventMask.Ctrl | EventMask.Meta | EventMask.Shift;

    public event Func<Task>? PrintScreenPressed;

    private IGlobalHook? _hook;
    private int _handling;

    public bool Enabled { get; set; }

    public bool IsRunning { get; private set; }

    public void Start()
    {
        if (_hook is not null)
            return;

        try
        {
            // Handlers must run on the hook thread. TaskPoolGlobalHook and EventLoopGlobalHook
            // ignore SuppressEvent, so Print Screen would still reach Windows Snipping Tool
            // (ScreenClippingHost.exe) and its overlay would be captured.
            var hook = new SimpleGlobalHook();
            hook.KeyPressed += OnKeyPressed;
            hook.KeyReleased += OnKeyReleased;
            _hook = hook;
            _ = hook.RunAsync(GlobalHookType.Keyboard, useBackgroundThread: true)
                .ContinueWith(
                    task => Logger.Warn(task.Exception, "Global hotkey hook stopped."),
                    CancellationToken.None,
                    TaskContinuationOptions.OnlyOnFaulted,
                    TaskScheduler.Default);
            IsRunning = true;
        }
        catch (Exception ex)
        {
            Logger.Warn(ex, "Failed to start global hotkey service.");
            Dispose();
        }
    }

    private void OnKeyPressed(object? sender, KeyboardHookEventArgs e)
    {
        if (!TrySuppressBarePrintScreen(e))
            return;

        if (Interlocked.CompareExchange(ref _handling, 1, 0) != 0)
            return;

        var handler = PrintScreenPressed;
        if (handler is null)
        {
            Interlocked.Exchange(ref _handling, 0);
            return;
        }

        try
        {
            Dispatcher.UIThread.Post(() => { _ = RunHandlerAsync(handler); });
        }
        catch (Exception ex)
        {
            Interlocked.Exchange(ref _handling, 0);
            Logger.Error(ex, "Error scheduling PrintScreen handler.");
        }
    }

    private void OnKeyReleased(object? sender, KeyboardHookEventArgs e)
    {
        // Snipping Tool also reacts to the key-up. Swallow it without starting another capture.
        TrySuppressBarePrintScreen(e);
    }

    private bool TrySuppressBarePrintScreen(KeyboardHookEventArgs e)
    {
        if (!Enabled || e.IsEventSimulated || e.Data.KeyCode != KeyCode.VcPrintScreen)
            return false;

        if ((e.RawEvent.Mask & ModifierMask) != 0)
            return false;

        e.SuppressEvent = true;
        return true;
    }

    private async Task RunHandlerAsync(Func<Task> handler)
    {
        try
        {
            await handler();
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "Error invoking PrintScreen handler.");
        }
        finally
        {
            Interlocked.Exchange(ref _handling, 0);
        }
    }

    public void Dispose()
    {
        if (_hook is not null)
        {
            _hook.KeyPressed -= OnKeyPressed;
            _hook.KeyReleased -= OnKeyReleased;
            _hook.Dispose();
            _hook = null;
        }
        IsRunning = false;
    }
}
