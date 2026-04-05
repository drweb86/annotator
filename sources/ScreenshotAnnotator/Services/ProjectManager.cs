using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;
using ScreenshotAnnotator.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using ScreenshotAnnotator.Helpers;

namespace ScreenshotAnnotator.Services;

public interface IProjectManager
{
    void Delete(ProjectFileInfo project);
    Task<ProjectFileInfo> Import(string fileNameWithoutPath, Stream fileStream);
}

public class ProjectManager(IFileSystem fileSystem) : IProjectManager
{
    private string GetRenderedImageFile(string projectPath)
    {
        return Path.ChangeExtension(projectPath, ".png");
    }

    public async Task<ProjectFileInfo> Import(string fileNameWithoutPath, Stream fileStream)
    {
        var projectPath = GetTimestampedFilePath();

        if (fileNameWithoutPath.EndsWith(Extension, StringComparison.OrdinalIgnoreCase))
            return await ImportProject(fileStream, projectPath);

        return await ImportPicture(fileStream, projectPath);
    }

    private string CreatePreviewImageBase64(RenderTargetBitmap renderedImage)
        // Bug 1. Full scale is bad!
        // Bug 2. Use separate file.
    {
        using (var previewStream = new MemoryStream())
        {
            renderedImage.Save(previewStream);
            return Convert.ToBase64String(previewStream.ToArray());
        }
    }

    private async Task<ProjectFileInfo> ImportPicture(Stream pictureFileStream, string projectPath)
    {
        var backgroundImageBytes = await pictureFileStream.GetBytes();
        using var backgroundImageStream = new MemoryStream(backgroundImageBytes);
        using var backgroundImage = new Bitmap(backgroundImageStream);
        
        using var renderedImage = ProjectRenderer.Render(backgroundImage, Array.Empty<AnnotationShape>(), out _);
        if (renderedImage is null)
            throw new InvalidOperationException("Could not render imported image.");

        var project = new AnnotatorProject { Version = 1 };
        project.BaseImageBase64 = Convert.ToBase64String(backgroundImageBytes);
        project.PreviewImageBase64 = CreatePreviewImageBase64(renderedImage);
        var json = JsonSerializer.Serialize(project, new JsonSerializerOptions { WriteIndented = true });
        await File.WriteAllTextAsync(projectPath, json);

        var pngRenderedImageFile = GetRenderedImageFile(projectPath);
        renderedImage.Save(pngRenderedImageFile);

        return CreateProjectFileInfo(projectPath);
    }

    private static async Task<ProjectFileInfo> ImportProject(Stream fileStream, string projectPath)
    {
        await using (var dest = new FileStream(projectPath, FileMode.CreateNew, FileAccess.Write, FileShare.None, 4096, FileOptions.Asynchronous))
            await fileStream.CopyToAsync(dest);

        return CreateProjectFileInfo(projectPath);
    }

    public void Delete(ProjectFileInfo project)
    {
        fileSystem.FileDelete(project.FilePath);

        var image = GetRenderedImageFile(project.FilePath);
        if (fileSystem.FileExists(image))
            fileSystem.FileDelete(image);
    }

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
                files.Add(CreateProjectFileInfo(file));

            // Sort by modified date descending (newest first)
            files = files.OrderByDescending(f => f.FileName).ToList();
        }
        catch
        {
            // Handle errors silently
        }

        return files;
    }

    private static ProjectFileInfo CreateProjectFileInfo(string filePath)
    {
        var fileInfo = new FileInfo(filePath);
        return new ProjectFileInfo
        {
            FilePath = filePath,
            FileName = Path.GetFileName(filePath),
            ModifiedDate = fileInfo.LastWriteTime,
            Thumbnail = LoadProjectThumbnail(filePath)
        };
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
