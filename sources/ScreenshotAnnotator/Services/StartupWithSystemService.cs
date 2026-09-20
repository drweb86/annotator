using System;
using System.IO;
using System.Threading.Tasks;

namespace ScreenshotAnnotator.Services;

public static class StartupWithSystemService
{
    public const string WindowsStartupTaskId = "ScreenshotAnnotatorStartup";
    private const string RunValueName = "ScreenshotAnnotator";
    private const string LinuxDesktopFileName = "screenshot-annotator.desktop";

    public static bool IsSupported =>
        OperatingSystem.IsWindows() || OperatingSystem.IsLinux();

    public static async Task<bool> GetEnabledAsync()
    {
        if (OperatingSystem.IsWindows())
            return await GetWindowsEnabledAsync().ConfigureAwait(false);
        if (OperatingSystem.IsLinux())
            return File.Exists(GetLinuxDesktopPath());
        return false;
    }

    public static async Task SetEnabledAsync(bool enabled)
    {
        if (OperatingSystem.IsWindows())
        {
            await SetWindowsEnabledAsync(enabled);
            return;
        }

        if (OperatingSystem.IsLinux())
            SetLinuxEnabled(enabled);
    }

    private static async Task<bool> GetWindowsEnabledAsync()
    {
#if WINDOWS
        if (WindowsMsixPackage.IsCurrentProcessPackaged)
        {
            try
            {
                return await GetPackagedStartupEnabledAsync().ConfigureAwait(false);
            }
            catch
            {
                // Fall through to the unpackaged Run key.
            }
        }
#endif
        return GetRunKeyEnabled();
    }

    private static async Task SetWindowsEnabledAsync(bool enabled)
    {
#if WINDOWS
        if (WindowsMsixPackage.IsCurrentProcessPackaged)
        {
            try
            {
                await SetPackagedStartupEnabledAsync(enabled);
                return;
            }
            catch
            {
                // Fall through to the unpackaged Run key.
            }
        }
#endif
        SetRunKeyEnabled(enabled);
        await Task.CompletedTask;
    }

    private static bool GetRunKeyEnabled()
    {
        if (!OperatingSystem.IsWindows())
            return false;

        try
        {
            using var key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(
                @"Software\Microsoft\Windows\CurrentVersion\Run", writable: false);
            return key?.GetValue(RunValueName) is string;
        }
        catch
        {
            return false;
        }
    }

    private static void SetRunKeyEnabled(bool enabled)
    {
        if (!OperatingSystem.IsWindows())
            return;

        using var key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(
            @"Software\Microsoft\Windows\CurrentVersion\Run", writable: true)
            ?? Microsoft.Win32.Registry.CurrentUser.CreateSubKey(
                @"Software\Microsoft\Windows\CurrentVersion\Run");

        if (enabled)
        {
            var exe = Environment.ProcessPath;
            if (string.IsNullOrWhiteSpace(exe))
                return;
            key.SetValue(RunValueName, $"\"{exe}\" --startup");
        }
        else
        {
            key.DeleteValue(RunValueName, throwOnMissingValue: false);
        }
    }

#if WINDOWS
    private static async Task<bool> GetPackagedStartupEnabledAsync()
    {
        var task = await Windows.ApplicationModel.StartupTask.GetAsync(WindowsStartupTaskId);
        return task.State is Windows.ApplicationModel.StartupTaskState.Enabled
            or Windows.ApplicationModel.StartupTaskState.EnabledByPolicy;
    }

    private static async Task SetPackagedStartupEnabledAsync(bool enabled)
    {
        var task = await Windows.ApplicationModel.StartupTask.GetAsync(WindowsStartupTaskId);
        if (enabled)
            await task.RequestEnableAsync();
        else
            task.Disable();
    }
#endif

    private static string GetLinuxDesktopPath()
    {
        var config = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        return Path.Combine(config, "autostart", LinuxDesktopFileName);
    }

    private static void SetLinuxEnabled(bool enabled)
    {
        var path = GetLinuxDesktopPath();
        if (!enabled)
        {
            if (File.Exists(path))
                File.Delete(path);
            return;
        }

        var exe = Environment.ProcessPath;
        if (string.IsNullOrWhiteSpace(exe))
            return;

        var directory = Path.GetDirectoryName(path);
        if (directory is not null)
            Directory.CreateDirectory(directory);

        var contents =
            "[Desktop Entry]\n" +
            "Type=Application\n" +
            "Name=Screenshot Annotator\n" +
            $"Exec=\"{exe}\" --startup\n" +
            "X-GNOME-Autostart-enabled=true\n" +
            "Hidden=false\n";
        File.WriteAllText(path, contents);
    }
}
