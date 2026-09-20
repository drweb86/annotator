using System.Collections.Generic;

namespace ScreenshotAnnotator.Models;

public class ApplicationSettingsV1Dto
{
    public bool IsFileBrowserVisible { get; set; } = true;
    public uint SelectedHighlighterColorArgb { get; set; } = 0x64FFFF00;
    public bool EnablePrintScreenHotkey { get; set; } = true;
    public bool StartWithSystem { get; set; }
    public string? DocumentLanguage { get; set; }
    public string? OcrPreferredEngine { get; set; }
    public List<string> OcrPreferredLanguages { get; set; } = new();
}
