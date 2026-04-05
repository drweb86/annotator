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
    FilePickerFileType PickerFilter { get; }
    public string ProjectsFolder { get; }
    IEnumerable <ProjectFileInfo> GetProjects();
    void Delete(ProjectFileInfo project);
    Task<ProjectFileInfo> Import(string fileNameWithoutPath, Stream fileStream);
    Task<ProjectFileInfo> ImportImage(Stream fileStream);
}

public class ProjectManager(IFileSystem fileSystem) : IProjectManager
{
    private static string GetRenderedImageFile(string projectPath)
    {
        return Path.ChangeExtension(projectPath, ".png");
    }

    public async Task<ProjectFileInfo> ImportImage(Stream fileStream)
    {
        var projectPath = GenerateProjectFileName();
        return await ImportPicture(fileStream, projectPath);
    }

    public async Task<ProjectFileInfo> Import(string fileNameWithoutPath, Stream fileStream)
    {
        var projectPath = GenerateProjectFileName();

        if (fileNameWithoutPath.EndsWith(Extension, StringComparison.OrdinalIgnoreCase))
            return await ImportProject(fileStream, projectPath);

        return await ImportPicture(fileStream, projectPath);
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
        project.PreviewImageBase64 = ProjectRenderer.CreatePreviewImage(renderedImage);
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

    public const string Extension = ".anp";

    public FilePickerFileType PickerFilter => new FilePickerFileType(LocalizationManager.Instance["FileType_AnnotatorProject"]) { Patterns = ["*" + Extension ] };

    public string ProjectsFolder 
    {
        get
        {
            return Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyPictures),
                "ScreenshotAnnotator");
        }
    } 

    public IEnumerable<ProjectFileInfo> GetProjects()
    {
        fileSystem.EnsureDirectoryExists(ProjectsFolder);

        var allFiles = Directory.GetFiles(ProjectsFolder, "*" + Extension);
        return allFiles
            .Select(CreateProjectFileInfo)
            .OrderByDescending(f => f.FileName);
    }

    private static ProjectFileInfo CreateProjectFileInfo(string filePath)
    {
        var fileInfo = new FileInfo(filePath);
        return new ProjectFileInfo
        {
            FilePath = filePath,
            RenderedImageFilePath = GetRenderedImageFile(filePath),
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
            if (project == null)
                return null;

            return ProjectRenderer.CreatePreviewImage(project.PreviewImageBase64);
        }
        catch
        {
            return null;
        }
    }

    private string GenerateProjectFileName()
    {
        var timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
        return Path.Combine(ProjectsFolder, $"{timestamp}{Extension}");
    }
}
