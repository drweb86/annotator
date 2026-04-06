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
    IReadOnlyList<FilePickerFileType> ImportFileTypeFilter { get; }
    IReadOnlyList<FilePickerFileType> ExportFileTypeChoices { get; }
    public string ProjectsFolder { get; }
    IEnumerable <ProjectFileInfo> GetProjects();
    void Delete(ProjectFileInfo project);
    Task<ProjectFileInfo> Import(string fileNameWithoutPath, Stream fileStream);
    Task<ProjectFileInfo> ImportImage(Stream fileStream);
    Task<AnnotatorProject> SaveProjectAsync(
        string projectFilePath,
        byte[] baseImageBytes,
        Bitmap? compositeImage,
        IReadOnlyList<AnnotationShape> shapes);
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
        fileSystem.WriteAllText(projectPath, json);

        var pngRenderedImageFile = GetRenderedImageFile(projectPath);
        await using (var pngFile = fileSystem.CreateFile(pngRenderedImageFile))
            renderedImage.Save(pngFile);

        return CreateProjectFileInfo(projectPath);
    }

    private async Task<ProjectFileInfo> ImportProject(Stream fileStream, string projectPath)
    {
        await using (var dest = fileSystem.CreateFile(projectPath))
            await fileStream.CopyToAsync(dest);

        return CreateProjectFileInfo(projectPath);
    }

    public async Task<AnnotatorProject> SaveProjectAsync(
        string projectFilePath,
        byte[] baseImageBytes,
        Bitmap? compositeImage,
        IReadOnlyList<AnnotationShape> shapes)
    {
        ArgumentNullException.ThrowIfNull(baseImageBytes);

        var project = new AnnotatorProject { Version = 1 };
        project.BaseImageBase64 = Convert.ToBase64String(baseImageBytes);

        using var renderedImage = ProjectRenderer.Render(compositeImage, shapes, out _);
        if (renderedImage is not null)
        {
            project.PreviewImageBase64 = ProjectRenderer.CreatePreviewImage(renderedImage);

            var pngPath = GetRenderedImageFile(projectFilePath);
            if (fileSystem.FileExists(pngPath))
                fileSystem.FileDelete(pngPath);
            await using (var pngFileStream = fileSystem.CreateFile(pngPath))
                renderedImage.Save(pngFileStream);
        }

        foreach (var shape in shapes)
            project.Shapes.Add(shape.ToSerializableShape());

        var json = JsonSerializer.Serialize(project, new JsonSerializerOptions { WriteIndented = true });
        fileSystem.WriteAllText(projectFilePath, json);
        return project;
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

    public IReadOnlyList<FilePickerFileType> ImportFileTypeFilter
    {
        get
        {
            var imageExtensions = ImageFileManager.SupportedImageExtensions.Select(x => "*" + x).ToArray();
            return
            [
                new FilePickerFileType(LocalizationManager.Instance["FileType_AllSupported"])
                {
                    Patterns = imageExtensions.Union(["*" + Extension]).ToArray()
                },
                PickerFilter,
                new FilePickerFileType(LocalizationManager.Instance["FileType_Images"])
                {
                    Patterns = imageExtensions
                },
                new FilePickerFileType(LocalizationManager.Instance["FileType_AllFiles"]) { Patterns = ["*.*"] }
            ];
        }
    }

    public IReadOnlyList<FilePickerFileType> ExportFileTypeChoices =>
    [
        new FilePickerFileType(LocalizationManager.Instance["FileType_PNG"]) { Patterns = ["*.png"] },
        new FilePickerFileType(LocalizationManager.Instance["FileType_JPEG"]) { Patterns = ["*.jpg", "*.jpeg"] },
        new FilePickerFileType(LocalizationManager.Instance["FileType_WebP"]) { Patterns = ["*.webp"] },
        PickerFilter
    ];

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

        var allFiles = fileSystem.GetFiles(ProjectsFolder, "*" + Extension);
        return allFiles
            .Select(CreateProjectFileInfo)
            .OrderByDescending(f => f.FileName);
    }

    private ProjectFileInfo CreateProjectFileInfo(string filePath)
    {
        return new ProjectFileInfo
        {
            FilePath = filePath,
            RenderedImageFilePath = GetRenderedImageFile(filePath),
            FileName = Path.GetFileName(filePath),
            ModifiedDate = fileSystem.GetLastWriteTime(filePath),
            Thumbnail = LoadProjectThumbnail(filePath)
        };
    }

    private Bitmap? LoadProjectThumbnail(string filePath)
    {
        try
        {
            var json = fileSystem.ReadAllText(filePath);
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
