using Avalonia.Controls;
using ScreenshotAnnotator.ViewModels;
using ScreenshotAnnotator.Services;
using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace ScreenshotAnnotator.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        Closing += OnClosing;
    }

    private async void OnClosing(object? sender, WindowClosingEventArgs e)
    {
        // Autosave before closing
        if (DataContext is ImageEditorViewModel viewModel)
        {
            // Cancel the close temporarily
            e.Cancel = true;

            // Perform autosave
            await viewModel.AutoSaveCurrentProject();

            // Shutdown logging
            LoggingService.Shutdown();

            // Now actually close
            Closing -= OnClosing; // Remove handler to prevent recursion
            Close();
        }
    }

    public static bool CanExtendClientAreaToDecorationsHint => RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
}