using System;
using System.IO;

namespace ScreenshotAnnotator.Services;

public interface IFileSystem
{
    void EnsureDirectoryExists(string path);

    string[] GetFiles(string path, string searchPattern);
    DateTime GetLastWriteTime(string path);

    bool FileExists(string path);
    void FileDelete(string path);

    string ReadAllText(string path);

    void WriteAllText(string path, string contents);

    Stream CreateFile(string path);
}
