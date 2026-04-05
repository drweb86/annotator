using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.VisualTree;
using ScreenshotAnnotator.Models;
using ScreenshotAnnotator.ViewModels;

namespace ScreenshotAnnotator.Views;

public partial class RecentProjectsView : UserControl
{
    public RecentProjectsView()
    {
        InitializeComponent();

        Loaded += (_, _) =>
        {
            var projectListBox = this.FindControl<ListBox>("ProjectListBox");
            if (projectListBox != null)
            {
                projectListBox.AddHandler(PointerWheelChangedEvent, OnProjectListWheelChanged, handledEventsToo: false);
            }
        };
    }

    private void OnProjectListWheelChanged(object? sender, PointerWheelEventArgs e)
    {
        if (sender is not ListBox listBox) return;

        var scrollViewer = listBox.FindDescendantOfType<ScrollViewer>();
        if (scrollViewer == null) return;

        const double scrollSpeed = 80;
        var newOffset = scrollViewer.Offset.WithX(scrollViewer.Offset.X - e.Delta.Y * scrollSpeed);
        scrollViewer.Offset = newOffset;
        e.Handled = true;
    }

    private void OnFileListTapped(object? sender, TappedEventArgs e)
    {
        if (sender is ListBox listBox && listBox.SelectedItem is ProjectFileInfo fileInfo)
        {
            if (e.Source is Button)
                return;

            if (DataContext is RecentProjectsViewModel viewModel)
                _ = viewModel.OpenProjectFileCommand.ExecuteAsync(fileInfo);
        }
    }
}
