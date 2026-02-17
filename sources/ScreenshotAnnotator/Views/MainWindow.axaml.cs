using Avalonia.Controls;
using Avalonia.Input;
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
        KeyDown += OnKeyDown;
    }

    private async void OnKeyDown(object? sender, KeyEventArgs e)
    {
        if (DataContext is not MainViewModel mainViewModel) return;
        var viewModel = mainViewModel.ImageEditor;

        var isCtrl = e.KeyModifiers.HasFlag(KeyModifiers.Control);
        var isShift = e.KeyModifiers.HasFlag(KeyModifiers.Shift);

        // Ctrl+C - Copy to clipboard
        if (isCtrl && !isShift && e.Key == Key.C)
        {
            await viewModel.CopyToClipboardCommand.ExecuteAsync(null);
            e.Handled = true;
        }
        // Ctrl+V - Paste from clipboard
        else if (isCtrl && !isShift && e.Key == Key.V)
        {
            await viewModel.PasteFromClipboardCommand.ExecuteAsync(null);
            e.Handled = true;
        }
        // Ctrl+N - New project
        else if (isCtrl && !isShift && e.Key == Key.N)
        {
            viewModel.NewProjectCommand.Execute(null);
            e.Handled = true;
        }
        // Ctrl+O - Open image
        else if (isCtrl && !isShift && e.Key == Key.O)
        {
            await viewModel.ImportCommand.ExecuteAsync(null);
            e.Handled = true;
        }
        // Ctrl+S - Save project
        else if (isCtrl && !isShift && e.Key == Key.S)
        {
            await viewModel.ExportCommand.ExecuteAsync(null);
            e.Handled = true;
        }
        // F9 - Toggle file browser
        else if (e.Key == Key.F9)
        {
            viewModel.ToggleFileBrowserCommand.Execute(null);
            e.Handled = true;
        }
        // F12 or PrintScreen - Take screenshot
        else if (e.Key == Key.F12 || e.Key == Key.PrintScreen)
        {
            await viewModel.TakeScreenshotCommand.ExecuteAsync(null);
            e.Handled = true;
        }
    }

    private async void OnClosing(object? sender, WindowClosingEventArgs e)
    {
        // Autosave before closing
        if (DataContext is MainViewModel viewModel)
        {
            // Cancel the close temporarily
            e.Cancel = true;

            // Perform autosave
            await viewModel.ImageEditor.AutoSaveCurrentProject();

            // Shutdown logging
            LoggingService.Shutdown();

            // Now actually close
            Closing -= OnClosing; // Remove handler to prevent recursion
            Close();
        }
    }

    public static bool CanExtendClientAreaToDecorationsHint => RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
}