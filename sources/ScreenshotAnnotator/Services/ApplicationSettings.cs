using System;
using System.IO;
using System.Text.Json;

namespace ScreenshotAnnotator.Services;

public class ApplicationSettings
{
    public bool IsFileBrowserVisible { get; set; } = true;
    public uint SelectedHighlighterColorArgb { get; set; } = 0x64FFFF00; // Semi-transparent yellow default

    private static string GetSettingsFilePath()
    {
        var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        var settingsFolder = Path.Combine(appDataPath, "ScreenshotAnnotator");
        if (!Directory.Exists(settingsFolder))
        {
            Directory.CreateDirectory(settingsFolder);
        }
        return Path.Combine(settingsFolder, "settings.json");
    }

    public static ApplicationSettings Load()
    {
        try
        {
            var filePath = GetSettingsFilePath();
            if (File.Exists(filePath))
            {
                var json = File.ReadAllText(filePath);
                return JsonSerializer.Deserialize<ApplicationSettings>(json) ?? new ApplicationSettings();
            }
        }
        catch
        {
            // If loading fails, return default settings
        }

        return new ApplicationSettings();
    }

    public void Save()
    {
        try
        {
            var filePath = GetSettingsFilePath();
            var json = JsonSerializer.Serialize(this, new JsonSerializerOptions
            {
                WriteIndented = true
            });
            File.WriteAllText(filePath, json);
        }
        catch
        {
            // Silently handle save errors
        }
    }
}
