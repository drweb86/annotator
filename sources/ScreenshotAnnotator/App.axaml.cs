using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using ScreenshotAnnotator.ViewModels;
using ScreenshotAnnotator.Views;
using ScreenshotAnnotator.Services;

namespace ScreenshotAnnotator;

public partial class App : Application
{
    public override void Initialize()
    {
        LoggingService.Initialize();

        // Initialize localization manager
        _ = LocalizationManager.Instance;

        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext = new MainViewModel()
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}