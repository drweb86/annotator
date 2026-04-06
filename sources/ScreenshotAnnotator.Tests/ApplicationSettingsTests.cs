using System.IO;
using System.Text.Json;
using ScreenshotAnnotator.Services;
using Xunit;

namespace ScreenshotAnnotator.Tests;

public class ApplicationSettingsTests
{
    private static string TestSettingsPath(string name) =>
        Path.Combine(Path.GetTempPath(), "ScreenshotAnnotatorTests", name, "settings.json");

    [Fact]
    public void Load_missing_file_returns_defaults_and_ensures_directory()
    {
        var fs = new FakeFileSystem();
        var path = TestSettingsPath(nameof(Load_missing_file_returns_defaults_and_ensures_directory));
        var settings = new ApplicationSettings(fs);

        Assert.True(fs.FileExists(path) is false);
        Assert.True(settings.Settings.IsFileBrowserVisible);
        Assert.Equal(0x64FFFF00u, settings.Settings.SelectedHighlighterColorArgb);
        Assert.Contains(Path.GetDirectoryName(path)!, fs.CreatedDirectories);
    }

    [Fact]
    public void Load_existing_file_restores_properties()
    {
        var fs = new FakeFileSystem();
        var path = TestSettingsPath(nameof(Load_existing_file_restores_properties));
        var dir = Path.GetDirectoryName(path)!;
        fs.EnsureDirectoryExists(dir);
        fs.WriteAllText(path, """
            {
              "IsFileBrowserVisible": false,
              "SelectedHighlighterColorArgb": 4278190080
            }
            """);

        var settings = new ApplicationSettings(fs);

        Assert.False(settings.Settings.IsFileBrowserVisible);
        Assert.Equal(4278190080u, settings.Settings.SelectedHighlighterColorArgb);
    }

    [Fact]
    public void Save_writes_indented_json_round_trips()
    {
        var fs = new FakeFileSystem();
        var path = TestSettingsPath(nameof(Save_writes_indented_json_round_trips));
        var settings = new ApplicationSettings(fs);
        settings.Settings.IsFileBrowserVisible = false;
        settings.Settings.SelectedHighlighterColorArgb = 12345;
        settings.Save();

        Assert.True(fs.FileExists(path));
        var raw = fs.ReadAllText(path);
        using var doc = JsonDocument.Parse(raw);
        Assert.True(doc.RootElement.TryGetProperty("IsFileBrowserVisible", out var vis));
        Assert.False(vis.GetBoolean());
        Assert.Contains('\n', raw);

        var loaded = new ApplicationSettings(fs);
        Assert.False(loaded.Settings.IsFileBrowserVisible);
        Assert.Equal(12345u, loaded.Settings.SelectedHighlighterColorArgb);
    }

    [Fact]
    public void Load_invalid_json_returns_defaults()
    {
        var fs = new FakeFileSystem();
        var path = TestSettingsPath(nameof(Load_invalid_json_returns_defaults));
        var dir = Path.GetDirectoryName(path)!;
        fs.EnsureDirectoryExists(dir);
        fs.WriteAllText(path, "not json");

        var settings = new ApplicationSettings(fs);

        Assert.True(settings.Settings.IsFileBrowserVisible);
        Assert.Equal(0x64FFFF00u, settings.Settings.SelectedHighlighterColorArgb);
    }
}
