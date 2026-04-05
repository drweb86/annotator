using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ScreenshotAnnotator.Services;

namespace ScreenshotAnnotator.ViewModels;

public partial class RecentProjectsViewModel : ViewModelBase
{
    private readonly ImageEditorViewModel _editor;
    private readonly IApplicationSettings _settings;

    public RecentProjectsViewModel(ImageEditorViewModel editor, IApplicationSettings settings)
    {
        _editor = editor;
        _settings = settings;
        _isPanelExpanded = settings.Settings.IsFileBrowserVisible;
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

    public void Refresh()
    {
        ProjectFiles.Clear();
        var current = _editor.CurrentProjectFilePath;
        foreach (var file in ProjectManager.GetProjectFiles())
        {
            file.IsCurrentFile = !string.IsNullOrEmpty(current) &&
                                 file.FilePath.Equals(current, StringComparison.OrdinalIgnoreCase);
            ProjectFiles.Add(file);
        }
    }

    [RelayCommand]
    private void RefreshProjectFiles() => Refresh();

    [RelayCommand]
    private async Task OpenProjectFile(ProjectFileInfo? fileInfo)
    {
        if (fileInfo is null) return;
        await _editor.OpenProjectFromRecentAsync(fileInfo);
    }

    [RelayCommand]
    private void DeleteProjectFile(ProjectFileInfo? fileInfo)
    {
        if (fileInfo is null) return;
        _editor.DeleteProjectFromRecent(fileInfo);
    }

    [RelayCommand]
    private void TogglePanel()
    {
        IsPanelExpanded = !IsPanelExpanded;
    }
}
