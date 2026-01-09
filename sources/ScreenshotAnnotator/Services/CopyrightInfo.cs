using System;
using System.Globalization;
using System.Reflection;

namespace ScreenshotAnnotator.Services;

public static class CopyrightInfo
{
    public static string Copyright { get; }

    public static Version Version { get; }

    static CopyrightInfo()
    {
        Version = Assembly
            .GetExecutingAssembly()
                .GetName()
            .Version ?? throw new InvalidProgramException("Failed to get assembly from !");

        Copyright = LocalizationManager.Instance.GetString("Copyright_Text", Version, DateTime.Now.Year);
    }
}