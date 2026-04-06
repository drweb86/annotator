using System.Collections.Generic;
using System.IO;
using ScreenshotAnnotator.Services;

namespace ScreenshotAnnotator.Tests;

public sealed class FakeFileSystem : IFileSystem
{
    private readonly HashSet<string> _directories = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, string> _files = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, DateTime> _lastWrite = new(StringComparer.OrdinalIgnoreCase);

    public IReadOnlyCollection<string> CreatedDirectories => _directories;

    public IReadOnlyDictionary<string, string> Files => _files;

    public void EnsureDirectoryExists(string path)
    {
        if (string.IsNullOrEmpty(path)) return;
        _directories.Add(NormalizeDirectory(path));
    }

    public string[] GetFiles(string path, string searchPattern)
    {
        var normalizedDir = NormalizeDirectory(path);
        var wildcard = searchPattern.StartsWith("*", StringComparison.Ordinal);
        var suffix = wildcard ? searchPattern.Substring(1) : searchPattern;

        return _files.Keys
            .Where(file => string.Equals(Path.GetDirectoryName(file), normalizedDir, StringComparison.OrdinalIgnoreCase))
            .Where(file => !wildcard || file.EndsWith(suffix, StringComparison.OrdinalIgnoreCase))
            .ToArray();
    }

    public DateTime GetLastWriteTime(string path)
    {
        var normalized = NormalizeFile(path);
        return _lastWrite.TryGetValue(normalized, out var value) ? value : DateTime.MinValue;
    }

    public bool FileExists(string path) => _files.ContainsKey(NormalizeFile(path));

    public void FileDelete(string path)
    {
        var normalized = NormalizeFile(path);
        _files.Remove(normalized);
        _lastWrite.Remove(normalized);
    }

    public string ReadAllText(string path) => _files[NormalizeFile(path)];

    public void WriteAllText(string path, string contents)
    {
        var normalized = NormalizeFile(path);
        var dir = Path.GetDirectoryName(normalized);
        if (!string.IsNullOrEmpty(dir))
            EnsureDirectoryExists(dir);
        _files[normalized] = contents;
        _lastWrite[normalized] = DateTime.UtcNow;
    }

    public Stream CreateFile(string path)
    {
        var normalized = NormalizeFile(path);
        var dir = Path.GetDirectoryName(normalized);
        if (!string.IsNullOrEmpty(dir))
            EnsureDirectoryExists(dir);
        return new MemoryWriteStream(contents =>
        {
            _files[normalized] = contents;
            _lastWrite[normalized] = DateTime.UtcNow;
        });
    }

    private static string NormalizeDirectory(string path) =>
        path.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

    private static string NormalizeFile(string path) => path;

    private sealed class MemoryWriteStream(Action<string> onDispose) : MemoryStream
    {
        private readonly Action<string> _onDispose = onDispose;
        private bool _disposed;

        protected override void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                _disposed = true;
                if (disposing)
                {
                    var text = System.Text.Encoding.UTF8.GetString(ToArray());
                    _onDispose(text);
                }
            }

            base.Dispose(disposing);
        }
    }
}
