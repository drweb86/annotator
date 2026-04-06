using System;
using System.IO;

namespace ScreenshotAnnotator.Services;

public sealed class LocalFileSystem : IFileSystem
{
    public void EnsureDirectoryExists(string path)
    {
        if (string.IsNullOrEmpty(path)) return;
        Directory.CreateDirectory(path);
    }

    public string[] GetFiles(string path, string searchPattern) => Directory.GetFiles(path, searchPattern);

    public DateTime GetLastWriteTime(string path) => File.GetLastWriteTime(path);

    public void FileDelete(string path) => File.Delete(path);

    public bool FileExists(string path) => File.Exists(path);

    public string ReadAllText(string path) => File.ReadAllText(path);

    public void WriteAllText(string path, string contents)
    {
        var dir = Path.GetDirectoryName(path);
        if (!string.IsNullOrEmpty(dir))
            EnsureDirectoryExists(dir);
        File.WriteAllText(path, contents);
    }

    public Stream CreateFile(string path)
    {
        var dir = Path.GetDirectoryName(path);
        if (!string.IsNullOrEmpty(dir))
            EnsureDirectoryExists(dir);
        return new FileStream(path, FileMode.CreateNew, FileAccess.Write, FileShare.None, 4096, FileOptions.Asynchronous);
    }
}
