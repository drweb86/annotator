using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using ScreenshotAnnotator.ViewModels;
using ScreenshotAnnotator.Views;
using ScreenshotAnnotator.Services;
using ScreenshotAnnotator.Services.Shapes;

namespace ScreenshotAnnotator;

public partial class App : Application
{
    public override void Initialize()
    {
        LoggingService.Initialize();
        ShapePluginLoader.Initialize();

        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext = new ImageEditorViewModel()
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}
