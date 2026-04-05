using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ScreenshotAnnotator.Services;

namespace ScreenshotAnnotator.ViewModels;

public partial class RecentProjectsViewModel : ViewModelBase
{
    private readonly IApplicationSettings _settings;

    public RecentProjectsViewModel()
    {
        _settings = AllServices.ApplicationSettings;
        _isPanelExpanded = _settings.Settings.IsFileBrowserVisible;
        AllServices.ApplicationEvents.OnDeleteProject += OnDeleteProject;
        AllServices.ApplicationEvents.OnOpenProject += OnOpenProject;
    }

    private async Task OnOpenProject(ProjectFileInfo project)
    {
    }

    private async Task OnDeleteProject(ProjectFileInfo project)
    {
    }

    [ObservableProperty]
    private ObservableCollection<ProjectFileInfo> _projectFiles = new();

    [ObservableProperty]
    private bool _isPanelExpanded;

    partial void OnIsPanelExpandedChanged(bool value)
    {
        _settings.Settings.IsFileBrowserVisible = value;
        _settings.Save();
    }

    // TODO: eliminate.
    [Obsolete("Never call directly")]
    public void Refresh(string currentProjectFilePath)
    {
        ProjectFiles.Clear();
        var current = currentProjectFilePath;
        foreach (var file in ProjectManager.GetProjectFiles())
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
        await AllServices.ApplicationEvents.DeleteProject(fileInfo);
    }

    [RelayCommand]
    private void TogglePanel()
    {
        IsPanelExpanded = !IsPanelExpanded;
    }
}
