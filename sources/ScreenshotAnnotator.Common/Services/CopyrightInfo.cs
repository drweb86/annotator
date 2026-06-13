using System;
using System.Globalization;
using System.Reflection;
using ScreenshotAnnotator.Resources;

namespace ScreenshotAnnotator.Services;

public static class CopyrightInfo
{
    public const string ApplicationId =
#if DEBUG
        "SiarheiKuchuk.ScreenshotAnnotator-DEBUG";
#else
        "SiarheiKuchuk.ScreenshotAnnotator";
#endif

    public static string Copyright { get; }

    public static Version Version { get; }

    static CopyrightInfo()
    {
        Version = Assembly.GetEntryAssembly()?.GetName().Version
            ?? Assembly.GetExecutingAssembly().GetName().Version
            ?? new Version(0, 0, 0);

        Copyright = string.Format(
            CultureInfo.CurrentUICulture,
            Strings.Copyright_Text,
            Version,
            DateTime.Now.Year);
    }
}
