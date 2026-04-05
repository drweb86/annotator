namespace ScreenshotAnnotator.Services;

public interface IFileSystem
{
    void EnsureDirectoryExists(string path);

    bool FileExists(string path);
    void FileDelete(string path);

    string ReadAllText(string path);

    void WriteAllText(string path, string contents);
}
