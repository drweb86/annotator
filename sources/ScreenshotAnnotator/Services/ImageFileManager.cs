using System;
using System.Collections.Generic;

namespace ScreenshotAnnotator.Services;

internal static class ImageFileManager
{
    public static readonly HashSet<string> SupportedImageExtensions = new(StringComparer.OrdinalIgnoreCase) { ".png", ".jpg", ".jpeg", ".webp", ".bmp" };

    public static bool IsSupportedImageExtension(string? extension) => SupportedImageExtensions.Contains(extension?.ToLowerInvariant() ?? string.Empty);
}
