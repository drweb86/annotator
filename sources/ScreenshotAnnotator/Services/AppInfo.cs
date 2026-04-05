namespace ScreenshotAnnotator.Services;

public static class AppInfo
{
    public const string ApplicationId =
#if DEBUG
        "SiarheiKuchuk.ScreenshotAnnotator-DEBUG";
#else
        "SiarheiKuchuk.ScreenshotAnnotator";
#endif
}
