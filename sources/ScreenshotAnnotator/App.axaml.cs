using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using ScreenshotAnnotator.ViewModels;
using ScreenshotAnnotator.Views;
using ScreenshotAnnotator.Services;
using ScreenshotAnnotator.Services.Shapes;
using System;
using System.Linq;

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
            var mainWindow = new MainWindow
            {
                DataContext = new ImageEditorViewModel()
            };

            if (ShouldStartMinimized(desktop.Args))
                mainWindow.WindowState = Avalonia.Controls.WindowState.Minimized;

            desktop.MainWindow = mainWindow;
        }

        base.OnFrameworkInitializationCompleted();
    }

    private static bool ShouldStartMinimized(string[]? args)
    {
        if (args is not null
            && args.Any(a => string.Equals(a, "--startup", StringComparison.OrdinalIgnoreCase)))
            return true;

#if WINDOWS
        try
        {
            var activated = Windows.ApplicationModel.AppInstance.GetActivatedEventArgs();
            if (activated?.Kind == Windows.ApplicationModel.Activation.ActivationKind.StartupTask)
                return true;
        }
        catch
        {
            // Unpackaged builds do not expose Store activation args.
        }
#endif
        return false;
    }
}
