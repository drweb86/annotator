using System.IO;

namespace ScreenshotAnnotator.Services;

public sealed class LocalFileSystem : IFileSystem
{
    public void EnsureDirectoryExists(string path)
    {
        if (string.IsNullOrEmpty(path)) return;
        Directory.CreateDirectory(path);
    }

    public bool FileExists(string path) => File.Exists(path);

    public string ReadAllText(string path) => File.ReadAllText(path);

    public void WriteAllText(string path, string contents)
    {
        var dir = Path.GetDirectoryName(path);
        if (!string.IsNullOrEmpty(dir))
            EnsureDirectoryExists(dir);
        File.WriteAllText(path, contents);
    }
}
