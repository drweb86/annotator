using System.IO;
using ScreenshotAnnotator.Services;
using Xunit;

namespace ScreenshotAnnotator.Tests;

public class ProjectManagerTests
{
    [Fact]
    public void ExportFileTypeChoices_contains_image_types_and_project_type()
    {
        var manager = new ProjectManager(new FakeFileSystem());

        var choices = manager.ExportFileTypeChoices;

        Assert.Contains(choices, c => (c.Patterns ?? []).SequenceEqual(["*.png"]));
        Assert.Contains(choices, c => (c.Patterns ?? []).SequenceEqual(["*.jpg", "*.jpeg"]));
        Assert.Contains(choices, c => (c.Patterns ?? []).SequenceEqual(["*.webp"]));
        Assert.Contains(choices, c => (c.Patterns ?? []).SequenceEqual(["*" + ProjectManager.Extension]));
    }

    [Fact]
    public void ImportFileTypeFilter_contains_supported_and_project_patterns()
    {
        var manager = new ProjectManager(new FakeFileSystem());

        var filters = manager.ImportFileTypeFilter;

        Assert.Contains(filters, f => (f.Patterns ?? []).Contains("*" + ProjectManager.Extension));
        Assert.Contains(filters, f => (f.Patterns ?? []).SequenceEqual(["*" + ProjectManager.Extension]));
        Assert.Contains(filters, f => (f.Patterns ?? []).Contains("*.png"));
        Assert.Contains(filters, f => (f.Patterns ?? []).Contains("*.*"));
    }

    [Fact]
    public void Delete_removes_project_and_rendered_image()
    {
        var fs = new FakeFileSystem();
        var manager = new ProjectManager(fs);
        var projectPath = Path.Combine(Path.GetTempPath(), "project.anp");
        var renderedPath = Path.ChangeExtension(projectPath, ".png");
        fs.WriteAllText(projectPath, "{}");
        fs.WriteAllText(renderedPath, "img");

        manager.Delete(new Models.ProjectFileInfo { FilePath = projectPath });

        Assert.False(fs.FileExists(projectPath));
        Assert.False(fs.FileExists(renderedPath));
    }
}
