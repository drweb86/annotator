using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;
using ScreenshotAnnotator.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace ScreenshotAnnotator.Services;

public class ProjectFileInfo
{
    public string FilePath { get; set; } = "";
    public string FileName { get; set; } = "";
    public DateTime ModifiedDate { get; set; }
    public Bitmap? Thumbnail { get; set; }
    public bool IsCurrentFile { get; set; }
}

public static class ProjectManager
{
    private static string? _projectsFolder;
    public const string Extension = ".anp";
    public static FilePickerFileType PickerFilter => new FilePickerFileType(LocalizationManager.Instance["FileType_AnnotatorProject"]) { Patterns = ["*" + Extension ] };

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
            var allFiles = Directory.GetFiles(folder, "*" + Extension);

            foreach (var file in allFiles)
            {
                // Only include supported file types
                var fileInfo = new FileInfo(file);
                var projectFileInfo = new ProjectFileInfo
                {
                    FilePath = file,
                    FileName = Path.GetFileName(file),
                    ModifiedDate = fileInfo.LastWriteTime,
                    Thumbnail = LoadProjectThumbnail(file)
                };

                files.Add(projectFileInfo);
            }

            // Sort by modified date descending (newest first)
            files = files.OrderByDescending(f => f.FileName).ToList();
        }
        catch
        {
            // Handle errors silently
        }

        return files;
    }

    private static Bitmap? LoadProjectThumbnail(string filePath)
    {
        try
        {
            var json = File.ReadAllText(filePath);
            var project = JsonSerializer.Deserialize<AnnotatorProject>(json);

            if (project == null || string.IsNullOrEmpty(project.PreviewImageBase64))
            {
                return null;
            }

            // Decode base64 image
            var imageBytes = Convert.FromBase64String(project.PreviewImageBase64);
            using var memoryStream = new MemoryStream(imageBytes);
            var bitmap = new Bitmap(memoryStream);

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

    public static string GenerateTimestampedFileName()
    {
        var timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
        return $"{timestamp}{Extension}";
    }

    public static string GetTimestampedFilePath()
    {
        var folder = GetProjectsFolder();
        var fileName = GenerateTimestampedFileName();
        return Path.Combine(folder, fileName);
    }
}
