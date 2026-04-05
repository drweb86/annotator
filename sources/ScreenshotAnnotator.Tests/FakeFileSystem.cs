using System.Collections.Generic;
using System.IO;
using ScreenshotAnnotator.Services;

namespace ScreenshotAnnotator.Tests;

public sealed class FakeFileSystem : IFileSystem
{
    private readonly HashSet<string> _directories = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, string> _files = new(StringComparer.OrdinalIgnoreCase);

    public IReadOnlyCollection<string> CreatedDirectories => _directories;

    public IReadOnlyDictionary<string, string> Files => _files;

    public void EnsureDirectoryExists(string path)
    {
        if (string.IsNullOrEmpty(path)) return;
        _directories.Add(NormalizeDirectory(path));
    }

    public bool FileExists(string path) => _files.ContainsKey(NormalizeFile(path));

    public string ReadAllText(string path) => _files[NormalizeFile(path)];

    public void WriteAllText(string path, string contents)
    {
        var normalized = NormalizeFile(path);
        var dir = Path.GetDirectoryName(normalized);
        if (!string.IsNullOrEmpty(dir))
            EnsureDirectoryExists(dir);
        _files[normalized] = contents;
    }

    private static string NormalizeDirectory(string path) =>
        path.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

    private static string NormalizeFile(string path) => path;
}
