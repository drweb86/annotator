using ScreenshotAnnotator.Models;

namespace ScreenshotAnnotator.Services;

public interface IApplicationSettings
{
    ApplicationSettingsV1Dto Settings { get; }
    void Save();
}
