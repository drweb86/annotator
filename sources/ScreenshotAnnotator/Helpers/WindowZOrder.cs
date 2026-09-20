using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Threading;

namespace ScreenshotAnnotator.Helpers;

internal static class WindowZOrder
{
    public static async Task ShowOnTopAsync(Window window)
    {
        window.ShowActivated = true;
        window.Topmost = true;
        window.Show();
        BringToFront(window);

        await Dispatcher.UIThread.InvokeAsync(() =>
        {
            window.Topmost = true;
            BringToFront(window);
        }, DispatcherPriority.Loaded);
    }

    public static void BringToFront(Window window)
    {
        window.Topmost = true;
        window.Activate();
        window.Focus();

        if (OperatingSystem.IsWindows())
            TryBringToFrontWindows(window);
    }

    private static void TryBringToFrontWindows(Window window)
    {
        var handle = window.TryGetPlatformHandle()?.Handle ?? IntPtr.Zero;
        if (handle == IntPtr.Zero)
            return;

        ShowWindow(handle, SwRestore);
        SetWindowPos(handle, HwndTopmost, 0, 0, 0, 0, SwpNoMove | SwpNoSize | SwpShowWindow);
        SetForegroundWindow(handle);
        BringWindowToTop(handle);
    }

    private const int SwRestore = 9;
    private static readonly IntPtr HwndTopmost = new(-1);
    private const uint SwpNoMove = 0x0002;
    private const uint SwpNoSize = 0x0001;
    private const uint SwpShowWindow = 0x0040;

    [DllImport("user32.dll")]
    private static extern bool SetForegroundWindow(IntPtr hWnd);

    [DllImport("user32.dll")]
    private static extern bool BringWindowToTop(IntPtr hWnd);

    [DllImport("user32.dll")]
    private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

    [DllImport("user32.dll")]
    private static extern bool SetWindowPos(
        IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, uint uFlags);
}
