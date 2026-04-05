using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ScreenshotAnnotator.Models;
using ScreenshotAnnotator.Services;

namespace ScreenshotAnnotator.ViewModels;

public partial class RecentProjectsViewModel : ViewModelBase
{
    public RecentProjectsViewModel()
    {
        _isPanelExpanded = AllServices.ApplicationSettings.Settings.IsFileBrowserVisible;
        AllServices.ApplicationEvents.OnDeleteProject += OnDeleteProject;
        AllServices.ApplicationEvents.OnOpenProject += OnOpenProject;
        AllServices.ApplicationEvents.OnCreateProject += OnCreateProject;
    }

    private async Task OnOpenProject(ProjectFileInfo project)
    {
    }

    private async Task OnCreateProject(ProjectFileInfo project)
    {
        ProjectFiles.Insert(0, project);
        await Task.CompletedTask;
    }

    private async Task OnDeleteProject(ProjectFileInfo project)
    {
        this.ProjectFiles.Remove(project);
    }

    [ObservableProperty]
    private ObservableCollection<ProjectFileInfo> _projectFiles = new();

    [ObservableProperty]
    private bool _isPanelExpanded;

    partial void OnIsPanelExpandedChanged(bool value)
    {
        AllServices.ApplicationSettings.Settings.IsFileBrowserVisible = value;
        AllServices.ApplicationSettings.Save();
    }

    // TODO: eliminate.
    [Obsolete("Never call directly")]
    public void Refresh(string currentProjectFilePath)
    {
        ProjectFiles.Clear();
        var current = currentProjectFilePath;
        foreach (var file in AllServices.ProjectManager.GetProjects())
        {
            file.IsCurrentFile = !string.IsNullOrEmpty(current) &&
                                 file.FilePath.Equals(current, StringComparison.OrdinalIgnoreCase);
            ProjectFiles.Add(file);
        }
    }

    [RelayCommand]
    private async Task OpenProjectFile(ProjectFileInfo? fileInfo)
    {
        if (fileInfo is null) return;
        await AllServices.ApplicationEvents.OpenProject(fileInfo);
    }

    [RelayCommand]
    private async Task DeleteProjectFile(ProjectFileInfo? fileInfo)
    {
        if (fileInfo is null) return;
        AllServices.ProjectManager.Delete(fileInfo);
        await AllServices.ApplicationEvents.DeleteProject(fileInfo);
    }
}
