using System;
using System.IO;

namespace ScreenshotAnnotator.Legal;

public static class ThirdPartyNotices
{
    public const string ResourceName = "ScreenshotAnnotator.ThirdPartyNotices.md";

    public static string Load()
    {
        using var stream = typeof(ThirdPartyNotices).Assembly.GetManifestResourceStream(ResourceName)
            ?? throw new InvalidOperationException("Missing embedded third-party notices.");
        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }
}
