using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Threading;
using ScreenshotAnnotator.Services;

namespace ScreenshotAnnotator.Desktop;

sealed class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args)
    {
        ImproveIt.HandleUiError = HandleUiError;
        TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
        AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        try
        {
            BuildAvaloniaApp()
                .StartWithClassicDesktopLifetime(args);
        }
        catch (Exception ex)
        {
            ImproveIt.ProcessUnhandledException(ex);
        }
    }

    private static void HandleUiError(string message)
    {
        try
        {
            Dispatcher.UIThread.InvokeAsync(async () =>
            {
                var tcs = new TaskCompletionSource();
                var okButton = new Button
                {
                    Content = "OK",
                    HorizontalAlignment = HorizontalAlignment.Center
                };
                var window = new Window
                {
                    Title = "Screenshot Annotator",
                    Width = 500,
                    SizeToContent = SizeToContent.Height,
                    CanResize = false,
                    WindowStartupLocation = WindowStartupLocation.CenterScreen,
                    Content = new StackPanel
                    {
                        Margin = new Thickness(20),
                        Spacing = 16,
                        Children =
                        {
                            new TextBlock
                            {
                                Text = message,
                                TextWrapping = TextWrapping.Wrap
                            },
                            okButton
                        }
                    }
                };
                okButton.Click += (_, _) => window.Close();
                window.Closed += (_, _) => tcs.TrySetResult();
                window.Show();
                await tcs.Task;
            }).Wait(TimeSpan.FromSeconds(10));
        }
        catch
        {
            // Best effort — UI may not be available in crash scenarios
        }
    }

    private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        ImproveIt.ProcessUnhandledException((Exception)e.ExceptionObject);
    }

    private static void TaskScheduler_UnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
    {
        ImproveIt.ProcessUnhandledException(e.Exception);
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
}
