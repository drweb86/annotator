using System;

namespace ScreenshotAnnotator.Services;

public static class AllServices
{
    private static readonly IFileSystem _fileSystem = new LocalFileSystem();
    private static readonly IApplicationEvents _applicationEvents = new ApplicationEvents();
    private static readonly Lazy<IApplicationSettings> _applicationSettings = new(() => new ApplicationSettings(_fileSystem));
    private static readonly IProjectManager _projectManager = new ProjectManager(_fileSystem);
    private static readonly Lazy<GlobalHotkeyService> _globalHotkeyService = new(() => new GlobalHotkeyService());

    public static IFileSystem FileSystem => _fileSystem;
    public static IApplicationSettings ApplicationSettings => _applicationSettings.Value;
    public static IApplicationEvents ApplicationEvents => _applicationEvents;
    public static IProjectManager ProjectManager => _projectManager;
    public static GlobalHotkeyService GlobalHotkeyService => _globalHotkeyService.Value;
}