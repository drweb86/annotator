using System;

namespace ScreenshotAnnotator.Services;

public static class AllServices
{
    private static readonly IFileSystem _fileSystem = new LocalFileSystem();
    private static readonly Lazy<IApplicationSettings> _applicationSettings = new(() => new ApplicationSettings(_fileSystem));

    public static IFileSystem FileSystem => _fileSystem;
    public static IApplicationSettings ApplicationSettings => _applicationSettings.Value;
}
