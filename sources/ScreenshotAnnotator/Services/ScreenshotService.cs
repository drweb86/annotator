using Avalonia.Media.Imaging;
using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace ScreenshotAnnotator.Services;

public static class ScreenshotService
{
    public static async Task<Bitmap?> CaptureScreenshotAsync()
    {
        try
        {
            // Small delay to allow window to hide
            await Task.Delay(100);

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return CaptureScreenshotWindows();
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return await CaptureScreenshotLinuxAsync();
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return await CaptureScreenshotMacAsync();
            }

            return null;
        }
        catch
        {
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

            return bitmap;
        }
        catch
        {
            return null;
        }
    }

    private static async Task<Bitmap?> CaptureScreenshotLinuxAsync()
    {
        // Try multiple screenshot methods for better compatibility
        try
        {
            var tempFile = System.IO.Path.GetTempFileName() + ".png";

            // Try gnome-screenshot first
            var processInfo = new System.Diagnostics.ProcessStartInfo
            {
                FileName = "gnome-screenshot",
                Arguments = $"-f \"{tempFile}\"",
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardError = true
            };

            using (var process = System.Diagnostics.Process.Start(processInfo))
            {
                if (process != null)
                {
                    await process.WaitForExitAsync();

                    if (System.IO.File.Exists(tempFile))
                    {
                        var bitmap = new Bitmap(tempFile);
                        System.IO.File.Delete(tempFile);
                        return bitmap;
                    }
                }
            }

            // Try scrot as fallback
            processInfo = new System.Diagnostics.ProcessStartInfo
            {
                FileName = "scrot",
                Arguments = $"\"{tempFile}\"",
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardError = true
            };

            using (var process = System.Diagnostics.Process.Start(processInfo))
            {
                if (process != null)
                {
                    await process.WaitForExitAsync();

                    if (System.IO.File.Exists(tempFile))
                    {
                        var bitmap = new Bitmap(tempFile);
                        System.IO.File.Delete(tempFile);
                        return bitmap;
                    }
                }
            }

            // Try import (ImageMagick) as second fallback
            processInfo = new System.Diagnostics.ProcessStartInfo
            {
                FileName = "import",
                Arguments = $"-window root \"{tempFile}\"",
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardError = true
            };

            using (var process = System.Diagnostics.Process.Start(processInfo))
            {
                if (process != null)
                {
                    await process.WaitForExitAsync();

                    if (System.IO.File.Exists(tempFile))
                    {
                        var bitmap = new Bitmap(tempFile);
                        System.IO.File.Delete(tempFile);
                        return bitmap;
                    }
                }
            }

            // Clean up temp file if nothing worked
            if (System.IO.File.Exists(tempFile))
            {
                System.IO.File.Delete(tempFile);
            }
        }
        catch
        {
            // Ignore
        }

        return null;
    }

    private static async Task<Bitmap?> CaptureScreenshotMacAsync()
    {
        // Use screencapture command
        try
        {
            var tempFile = System.IO.Path.GetTempFileName() + ".png";

            var processInfo = new System.Diagnostics.ProcessStartInfo
            {
                FileName = "screencapture",
                Arguments = $"-x {tempFile}",
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (var process = System.Diagnostics.Process.Start(processInfo))
            {
                if (process != null)
                {
                    await process.WaitForExitAsync();

                    if (System.IO.File.Exists(tempFile))
                    {
                        var bitmap = new Bitmap(tempFile);
                        System.IO.File.Delete(tempFile);
                        return bitmap;
                    }
                }
            }
        }
        catch
        {
            // Ignore
        }

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
