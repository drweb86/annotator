using ScreenshotAnnotator.Models;
using System.Threading.Tasks;

namespace ScreenshotAnnotator.Services;

public delegate Task ProjectFileInfoHandler(ProjectFileInfo project);

public interface IApplicationEvents
{
    event ProjectFileInfoHandler OnDeleteProject;
    event ProjectFileInfoHandler OnOpenProject;
    event ProjectFileInfoHandler OnCreateProject;

    Task DeleteProject(ProjectFileInfo project);
    Task OpenProject(ProjectFileInfo project);
    Task CreatedProject(ProjectFileInfo project);
}

public class ApplicationEvents : IApplicationEvents
{
    public event ProjectFileInfoHandler OnDeleteProject = _ => Task.CompletedTask;
    public event ProjectFileInfoHandler OnOpenProject = _ => Task.CompletedTask;
    public event ProjectFileInfoHandler OnCreateProject = _ => Task.CompletedTask;

    public async Task DeleteProject(ProjectFileInfo project)
    {
        foreach (var handler in OnDeleteProject.GetInvocationList())
            await ((ProjectFileInfoHandler)handler)(project);
    }

    public async Task OpenProject(ProjectFileInfo project)
    {
        foreach (var handler in OnOpenProject.GetInvocationList())
            await ((ProjectFileInfoHandler)handler)(project);
    }

    public async Task CreatedProject(ProjectFileInfo project)
    {
        foreach (var handler in OnCreateProject.GetInvocationList())
            await ((ProjectFileInfoHandler)handler)(project);
    }
}