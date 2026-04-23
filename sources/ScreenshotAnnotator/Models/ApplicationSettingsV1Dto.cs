namespace ScreenshotAnnotator.Models;

public class ApplicationSettingsV1Dto
{
    public bool IsFileBrowserVisible { get; set; } = true;
    public uint SelectedHighlighterColorArgb { get; set; } = 0x64FFFF00;
    public bool EnablePrintScreenHotkey { get; set; } = true;
}
