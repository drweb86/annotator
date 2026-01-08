using Avalonia.Media.Imaging;
using NLog;
using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace ScreenshotAnnotator.Services;

public static class ScreenshotService
{
    private static readonly Logger Logger = LoggingService.GetLogger("ScreenshotService");

    public static async Task<Bitmap?> CaptureScreenshotAsync()
    {
        try
        {
            Logger.Info("Starting screenshot capture");
            Logger.Debug($"Platform: {RuntimeInformation.OSDescription}");
            Logger.Debug($"Framework: {RuntimeInformation.FrameworkDescription}");
            Logger.Debug($"Architecture: {RuntimeInformation.ProcessArchitecture}");

            // Small delay to allow window to hide
            await Task.Delay(100);

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Logger.Info("Using Windows screenshot method");
                return CaptureScreenshotWindows();
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                Logger.Info("Using Linux screenshot method");
                return await CaptureScreenshotLinuxAsync();
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                Logger.Info("Using macOS screenshot method");
                return await CaptureScreenshotMacAsync();
            }

            Logger.Warn("Unknown platform, screenshot not supported");
            return null;
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "Failed to capture screenshot");
            return null;
        }
    }

    private static Bitmap? CaptureScreenshotWindows()
    {
        try
        {
            // Get screen dimensions
            var screenWidth = GetSystemMetrics(0);  // SM_CXSCREEN
            var screenHeight = GetSystemMetrics(1); // SM_CYSCREEN
            Logger.Debug($"Windows screen dimensions: {screenWidth}x{screenHeight}");

            // Create bitmap
            var bitmap = new WriteableBitmap(
                new Avalonia.PixelSize(screenWidth, screenHeight),
                new Avalonia.Vector(96, 96),
                Avalonia.Platform.PixelFormat.Bgra8888,
                Avalonia.Platform.AlphaFormat.Premul
            );

            // Get screen DC
            var screenDC = GetDC(IntPtr.Zero);
            var memDC = CreateCompatibleDC(screenDC);
            var hBitmap = CreateCompatibleBitmap(screenDC, screenWidth, screenHeight);
            var oldBitmap = SelectObject(memDC, hBitmap);

            // Copy screen to bitmap
            BitBlt(memDC, 0, 0, screenWidth, screenHeight, screenDC, 0, 0, 0x00CC0020); // SRCCOPY

            // Get bitmap data
            using (var lockBuffer = bitmap.Lock())
            {
                var bitmapInfo = new BITMAPINFO();
                bitmapInfo.bmiHeader.biSize = Marshal.SizeOf(typeof(BITMAPINFOHEADER));
                bitmapInfo.bmiHeader.biWidth = screenWidth;
                bitmapInfo.bmiHeader.biHeight = -screenHeight; // Top-down
                bitmapInfo.bmiHeader.biPlanes = 1;
                bitmapInfo.bmiHeader.biBitCount = 32;
                bitmapInfo.bmiHeader.biCompression = 0; // BI_RGB

                GetDIBits(memDC, hBitmap, 0, (uint)screenHeight, lockBuffer.Address, ref bitmapInfo, 0);
            }

            // Cleanup
            SelectObject(memDC, oldBitmap);
            DeleteObject(hBitmap);
            DeleteDC(memDC);
            ReleaseDC(IntPtr.Zero, screenDC);

            Logger.Info("Windows screenshot captured successfully");
            return bitmap;
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "Failed to capture Windows screenshot");
            return null;
        }
    }

    private static async Task<Bitmap?> CaptureScreenshotLinuxAsync()
    {
        // Try multiple screenshot methods for better compatibility
        var tempFile = "";
        try
        {
            tempFile = System.IO.Path.GetTempFileName() + ".png";
            Logger.Debug($"Linux temp file: {tempFile}");

            // Try gnome-screenshot first
            Logger.Info("Attempting gnome-screenshot");
            var processInfo = new System.Diagnostics.ProcessStartInfo
            {
                FileName = "gnome-screenshot",
                Arguments = $"-f \"{tempFile}\"",
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            using (var process = System.Diagnostics.Process.Start(processInfo))
            {
                if (process != null)
                {
                    await process.WaitForExitAsync();
                    var stderr = await process.StandardError.ReadToEndAsync();
                    var stdout = await process.StandardOutput.ReadToEndAsync();

                    Logger.Debug($"gnome-screenshot exit code: {process.ExitCode}");
                    if (!string.IsNullOrWhiteSpace(stdout)) Logger.Debug($"gnome-screenshot stdout: {stdout}");
                    if (!string.IsNullOrWhiteSpace(stderr)) Logger.Warn($"gnome-screenshot stderr: {stderr}");

                    if (System.IO.File.Exists(tempFile))
                    {
                        var fileInfo = new System.IO.FileInfo(tempFile);
                        Logger.Info($"gnome-screenshot succeeded, file size: {fileInfo.Length} bytes");
                        var bitmap = new Bitmap(tempFile);
                        System.IO.File.Delete(tempFile);
                        return bitmap;
                    }
                    else
                    {
                        Logger.Warn($"gnome-screenshot did not create file at {tempFile}");
                    }
                }
                else
                {
                    Logger.Error("Failed to start gnome-screenshot process");
                }
            }

            // Try scrot as fallback
            Logger.Info("Attempting scrot");
            processInfo = new System.Diagnostics.ProcessStartInfo
            {
                FileName = "scrot",
                Arguments = $"\"{tempFile}\"",
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            using (var process = System.Diagnostics.Process.Start(processInfo))
            {
                if (process != null)
                {
                    await process.WaitForExitAsync();
                    var stderr = await process.StandardError.ReadToEndAsync();
                    var stdout = await process.StandardOutput.ReadToEndAsync();

                    Logger.Debug($"scrot exit code: {process.ExitCode}");
                    if (!string.IsNullOrWhiteSpace(stdout)) Logger.Debug($"scrot stdout: {stdout}");
                    if (!string.IsNullOrWhiteSpace(stderr)) Logger.Warn($"scrot stderr: {stderr}");

                    if (System.IO.File.Exists(tempFile))
                    {
                        var fileInfo = new System.IO.FileInfo(tempFile);
                        Logger.Info($"scrot succeeded, file size: {fileInfo.Length} bytes");
                        var bitmap = new Bitmap(tempFile);
                        System.IO.File.Delete(tempFile);
                        return bitmap;
                    }
                    else
                    {
                        Logger.Warn($"scrot did not create file at {tempFile}");
                    }
                }
                else
                {
                    Logger.Error("Failed to start scrot process");
                }
            }

            // Try import (ImageMagick) as second fallback
            Logger.Info("Attempting import (ImageMagick)");
            processInfo = new System.Diagnostics.ProcessStartInfo
            {
                FileName = "import",
                Arguments = $"-window root \"{tempFile}\"",
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            using (var process = System.Diagnostics.Process.Start(processInfo))
            {
                if (process != null)
                {
                    await process.WaitForExitAsync();
                    var stderr = await process.StandardError.ReadToEndAsync();
                    var stdout = await process.StandardOutput.ReadToEndAsync();

                    Logger.Debug($"import exit code: {process.ExitCode}");
                    if (!string.IsNullOrWhiteSpace(stdout)) Logger.Debug($"import stdout: {stdout}");
                    if (!string.IsNullOrWhiteSpace(stderr)) Logger.Warn($"import stderr: {stderr}");

                    if (System.IO.File.Exists(tempFile))
                    {
                        var fileInfo = new System.IO.FileInfo(tempFile);
                        Logger.Info($"import succeeded, file size: {fileInfo.Length} bytes");
                        var bitmap = new Bitmap(tempFile);
                        System.IO.File.Delete(tempFile);
                        return bitmap;
                    }
                    else
                    {
                        Logger.Warn($"import did not create file at {tempFile}");
                    }
                }
                else
                {
                    Logger.Error("Failed to start import process");
                }
            }

            Logger.Error("All Linux screenshot methods failed");

            // Clean up temp file if nothing worked
            if (System.IO.File.Exists(tempFile))
            {
                System.IO.File.Delete(tempFile);
            }
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "Exception during Linux screenshot capture");
            if (!string.IsNullOrEmpty(tempFile) && System.IO.File.Exists(tempFile))
            {
                try { System.IO.File.Delete(tempFile); } catch { }
            }
        }

        return null;
    }

    private static async Task<Bitmap?> CaptureScreenshotMacAsync()
    {
        // Use screencapture command
        var tempFile = "";
        try
        {
            tempFile = System.IO.Path.GetTempFileName() + ".png";
            Logger.Debug($"macOS temp file: {tempFile}");
            Logger.Info("Attempting screencapture");

            var processInfo = new System.Diagnostics.ProcessStartInfo
            {
                FileName = "screencapture",
                Arguments = $"-x {tempFile}",
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            using (var process = System.Diagnostics.Process.Start(processInfo))
            {
                if (process != null)
                {
                    await process.WaitForExitAsync();
                    var stderr = await process.StandardError.ReadToEndAsync();
                    var stdout = await process.StandardOutput.ReadToEndAsync();

                    Logger.Debug($"screencapture exit code: {process.ExitCode}");
                    if (!string.IsNullOrWhiteSpace(stdout)) Logger.Debug($"screencapture stdout: {stdout}");
                    if (!string.IsNullOrWhiteSpace(stderr)) Logger.Warn($"screencapture stderr: {stderr}");

                    if (System.IO.File.Exists(tempFile))
                    {
                        var fileInfo = new System.IO.FileInfo(tempFile);
                        Logger.Info($"screencapture succeeded, file size: {fileInfo.Length} bytes");
                        var bitmap = new Bitmap(tempFile);
                        System.IO.File.Delete(tempFile);
                        return bitmap;
                    }
                    else
                    {
                        Logger.Warn($"screencapture did not create file at {tempFile}");
                    }
                }
                else
                {
                    Logger.Error("Failed to start screencapture process");
                }
            }
        }
        catch (Exception ex)
        {
            Logger.Error(ex, "Exception during macOS screenshot capture");
            if (!string.IsNullOrEmpty(tempFile) && System.IO.File.Exists(tempFile))
            {
                try { System.IO.File.Delete(tempFile); } catch { }
            }
        }

        Logger.Error("macOS screenshot failed");
        return null;
    }

    #region Windows API

    [DllImport("gdi32.dll")]
    private static extern bool BitBlt(IntPtr hdcDest, int xDest, int yDest, int width, int height,
        IntPtr hdcSrc, int xSrc, int ySrc, int rop);

    [DllImport("user32.dll")]
    private static extern IntPtr GetDC(IntPtr hWnd);

    [DllImport("user32.dll")]
    private static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);

    [DllImport("gdi32.dll")]
    private static extern IntPtr CreateCompatibleDC(IntPtr hdc);

    [DllImport("gdi32.dll")]
    private static extern IntPtr CreateCompatibleBitmap(IntPtr hdc, int nWidth, int nHeight);

    [DllImport("gdi32.dll")]
    private static extern IntPtr SelectObject(IntPtr hdc, IntPtr hObject);

    [DllImport("gdi32.dll")]
    private static extern bool DeleteDC(IntPtr hdc);

    [DllImport("gdi32.dll")]
    private static extern bool DeleteObject(IntPtr hObject);

    [DllImport("user32.dll")]
    private static extern int GetSystemMetrics(int nIndex);

    [DllImport("gdi32.dll")]
    private static extern int GetDIBits(IntPtr hdc, IntPtr hbmp, uint start, uint cLines,
        IntPtr lpvBits, ref BITMAPINFO lpbmi, uint usage);

    [StructLayout(LayoutKind.Sequential)]
    private struct BITMAPINFO
    {
        public BITMAPINFOHEADER bmiHeader;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1)]
        public uint[] bmiColors;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct BITMAPINFOHEADER
    {
        public int biSize;
        public int biWidth;
        public int biHeight;
        public short biPlanes;
        public short biBitCount;
        public int biCompression;
        public int biSizeImage;
        public int biXPelsPerMeter;
        public int biYPelsPerMeter;
        public int biClrUsed;
        public int biClrImportant;
    }

    #endregion
}
