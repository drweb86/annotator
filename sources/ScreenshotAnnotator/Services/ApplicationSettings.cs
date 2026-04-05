using ScreenshotAnnotator.Models;
using System;
using System.IO;
using System.Text.Json;

namespace ScreenshotAnnotator.Services;

public class ApplicationSettings : IApplicationSettings
{
    private readonly IFileSystem _fileSystem;
    public ApplicationSettingsV1Dto Settings { get; private set; }

    public ApplicationSettings(IFileSystem fileSystem)
    {
        _fileSystem = fileSystem;
        Settings = new ApplicationSettingsV1Dto();
        Load();
    }

    private static string GetSettingsFile()
    {
        var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        return Path.Combine(appDataPath, AppInfo.ApplicationId, "settings-v1.json");
    }

    private void Load()
    {
        var file = GetSettingsFile();
        if (!_fileSystem.FileExists(file))
            return;

        var json = _fileSystem.ReadAllText(file);
        var dto = JsonSerializer.Deserialize<ApplicationSettingsV1Dto>(json);
        if (dto is not null)
            Settings = dto;
    }

    public void Save()
    {
        var file = GetSettingsFile();
        var dir = Path.GetDirectoryName(file);
        if (dir is not null)
            _fileSystem.EnsureDirectoryExists(dir);
        
        var json = JsonSerializer.Serialize(Settings, new JsonSerializerOptions { WriteIndented = true });
        _fileSystem.WriteAllText(file, json);
    }
}
