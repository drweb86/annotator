using System.Threading.Tasks;

namespace ScreenshotAnnotator.Services;

public delegate Task ProjectFileInfoHandler(ProjectFileInfo project);

public interface IApplicationEvents
{
    event ProjectFileInfoHandler OnDeleteProject;
    event ProjectFileInfoHandler OnOpenProject;

    Task DeleteProject(ProjectFileInfo project);
    Task OpenProject(ProjectFileInfo project);
}

public class ApplicationEvents : IApplicationEvents
{
    public event ProjectFileInfoHandler OnDeleteProject = _ => Task.CompletedTask;
    public event ProjectFileInfoHandler OnOpenProject = _ => Task.CompletedTask;

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
}