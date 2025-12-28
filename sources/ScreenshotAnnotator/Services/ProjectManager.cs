using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ScreenshotAnnotator.Services;

public class ProjectFileInfo
{
    public string FilePath { get; set; } = "";
    public string FileName { get; set; } = "";
    public DateTime ModifiedDate { get; set; }
    public bool IsProject { get; set; }
    public string Extension { get; set; } = "";
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
                    files.Add(new ProjectFileInfo
                    {
                        FilePath = file,
                        FileName = Path.GetFileName(file),
                        ModifiedDate = fileInfo.LastWriteTime,
                        IsProject = ext == ".anp",
                        Extension = ext
                    });
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
