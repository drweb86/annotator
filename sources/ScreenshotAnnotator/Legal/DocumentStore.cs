using ScreenshotAnnotator.Services;

namespace ScreenshotAnnotator.Legal;

public static class DocumentStore
{
    public static string? LanguageCode()
    {
        var code = AllServices.ApplicationSettings.Settings.DocumentLanguage?.Trim();
        return string.IsNullOrEmpty(code) ? null : code;
    }

    public static void SetLanguageCode(string code)
    {
        var settings = AllServices.ApplicationSettings;
        settings.Settings.DocumentLanguage = code;
        settings.Save();
    }
}
