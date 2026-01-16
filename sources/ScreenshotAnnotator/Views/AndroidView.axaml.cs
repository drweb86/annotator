using Avalonia.Controls;
using ScreenshotAnnotator.ViewModels;
using ScreenshotAnnotator.Services;

namespace ScreenshotAnnotator.Views;

public partial class AndroidView : UserControl
{
    public AndroidView()
    {
        InitializeComponent();

        // Handle cleanup when the control is unloaded
        Unloaded += OnUnloaded;
    }

    private async void OnUnloaded(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        // Autosave before the view is unloaded
        if (DataContext is MainViewModel viewModel)
        {
            await viewModel.ImageEditor.AutoSaveCurrentProject();

            // Shutdown logging
            LoggingService.Shutdown();
        }
    }
}
