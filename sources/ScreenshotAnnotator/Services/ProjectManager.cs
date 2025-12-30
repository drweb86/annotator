using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Avalonia.Media.Imaging;

namespace ScreenshotAnnotator.Services;

public class ProjectFileInfo
{
    public string FilePath { get; set; } = "";
    public string FileName { get; set; } = "";
    public DateTime ModifiedDate { get; set; }
    public bool IsProject { get; set; }
    public string Extension { get; set; } = "";
    public Bitmap? Thumbnail { get; set; }
}

public static class ProjectManager
{
    private static string? _projectsFolder;

    public static string GetProjectsFolder()
    {
        if (_projectsFolder != null)
            return _projectsFolder;

        var picturesFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
        _projectsFolder = Path.Combine(picturesFolder, "ScreenshotAnnotator");

        if (!Directory.Exists(_projectsFolder))
        {
            Directory.CreateDirectory(_projectsFolder);
        }

        return _projectsFolder;
    }

    public static List<ProjectFileInfo> GetProjectFiles()
    {
        var folder = GetProjectsFolder();
        var files = new List<ProjectFileInfo>();

        try
        {
            var allFiles = Directory.GetFiles(folder);

            foreach (var file in allFiles)
            {
                var ext = Path.GetExtension(file).ToLowerInvariant();

                // Only include supported file types
                if (ext == ".anp" || ext == ".png" || ext == ".jpg" ||
                    ext == ".jpeg" || ext == ".webp" || ext == ".bmp")
                {
                    var fileInfo = new FileInfo(file);
                    var projectFileInfo = new ProjectFileInfo
                    {
                        FilePath = file,
                        FileName = Path.GetFileName(file),
                        ModifiedDate = fileInfo.LastWriteTime,
                        IsProject = ext == ".anp",
                        Extension = ext
                    };

                    // Load thumbnail for image files
                    if (ext != ".anp")
                    {
                        projectFileInfo.Thumbnail = LoadThumbnail(file);
                    }

                    files.Add(projectFileInfo);
                }
            }

            // Sort by modified date descending (newest first)
            files = files.OrderByDescending(f => f.ModifiedDate).ToList();
        }
        catch
        {
            // Handle errors silently
        }

        return files;
    }

    private static Bitmap? LoadThumbnail(string filePath)
    {
        try
        {
            using var stream = File.OpenRead(filePath);
            var bitmap = new Bitmap(stream);

            // Create a thumbnail (max 100x100)
            var maxSize = 100.0;
            var scale = Math.Min(maxSize / bitmap.PixelSize.Width, maxSize / bitmap.PixelSize.Height);

            if (scale < 1)
            {
                var newWidth = (int)(bitmap.PixelSize.Width * scale);
                var newHeight = (int)(bitmap.PixelSize.Height * scale);
                return bitmap.CreateScaledBitmap(new Avalonia.PixelSize(newWidth, newHeight));
            }

            return bitmap;
        }
        catch
        {
            return null;
        }
    }

    public static string GenerateTimestampedFileName(string extension)
    {
        var timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
        return $"screenshot_{timestamp}{extension}";
    }

    public static string GetTimestampedFilePath(string extension)
    {
        var folder = GetProjectsFolder();
        var fileName = GenerateTimestampedFileName(extension);
        return Path.Combine(folder, fileName);
    }
}
