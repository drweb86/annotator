using System.Linq;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Platform.Storage;
using ScreenshotAnnotator.ViewModels;
using ScreenshotAnnotator.Services;
using System.Runtime.InteropServices;

namespace ScreenshotAnnotator.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        if (CanExtendClientAreaToDecorationsHint)
        {
            ExtendClientAreaToDecorationsHint = true;
        }

        Closing += OnClosing;
        KeyDown += OnKeyDown;

        var mainGrid = this.FindControl<Grid>("MainContentGrid");
        if (mainGrid != null)
        {
            DragDrop.SetAllowDrop(mainGrid, true);
            mainGrid.AddHandler(DragDrop.DragOverEvent, OnDragOver);
            mainGrid.AddHandler(DragDrop.DropEvent, OnDrop);
        }

        Loaded += (s, e) =>
        {
            var editorCanvas = this.FindControl<Controls.ImageEditorCanvas>("EditorCanvas");
            var overlayCanvas = this.FindControl<Canvas>("OverlayCanvas");

            if (editorCanvas != null && overlayCanvas != null)
            {
                editorCanvas.OverlayCanvas = overlayCanvas;

                if (DataContext is ImageEditorViewModel viewModel)
                {
                    viewModel.SetEditorCanvas(editorCanvas);
                    editorCanvas.SelectedShapeChanged += (_, _) =>
                    {
                        viewModel.SelectedShape = editorCanvas.SelectedShape;
                    };

                    var topLevel = TopLevel.GetTopLevel(this);
                    if (topLevel != null)
                    {
                        viewModel.SetTopLevel(topLevel);
                    }

                    viewModel.RecentProjects.Initialize();
                }
            }
        };
    }

    private async void OnKeyDown(object? sender, KeyEventArgs e)
    {
        if (DataContext is not ImageEditorViewModel viewModel) return;

        var isCtrl = e.KeyModifiers.HasFlag(KeyModifiers.Control);
        var isShift = e.KeyModifiers.HasFlag(KeyModifiers.Shift);

        if (isCtrl && !isShift && e.Key == Key.C)
        {
            await viewModel.CopyToClipboardCommand.ExecuteAsync(null);
            e.Handled = true;
        }
        else if (isCtrl && !isShift && e.Key == Key.V)
        {
            await viewModel.PasteFromClipboardCommand.ExecuteAsync(null);
            e.Handled = true;
        }
        else if (isCtrl && !isShift && e.Key == Key.N)
        {
            viewModel.NewProjectCommand.Execute(null);
            e.Handled = true;
        }
        else if (isCtrl && !isShift && e.Key == Key.O)
        {
            await viewModel.ImportCommand.ExecuteAsync(null);
            e.Handled = true;
        }
        else if (isCtrl && !isShift && e.Key == Key.S)
        {
            await viewModel.ExportCommand.ExecuteAsync(null);
            e.Handled = true;
        }
        else if (e.Key == Key.PrintScreen)
        {
            await viewModel.TakeScreenshotCommand.ExecuteAsync(null);
            e.Handled = true;
        }
    }

    private async void OnClosing(object? sender, WindowClosingEventArgs e)
    {
        if (DataContext is ImageEditorViewModel viewModel)
        {
            e.Cancel = true;

            await viewModel.SaveCurrentProject();
            viewModel.CloseProject();

            LoggingService.Shutdown();

            Closing -= OnClosing;
            Close();
        }
    }

    private void OnColorPresetPressed(object? sender, PointerPressedEventArgs e)
    {
        if (DataContext is not ImageEditorViewModel viewModel) return;
        if (sender is Border border && border.DataContext is ArrowColorPresetItem item)
        {
            viewModel.SetArrowColorFromPresetCommand.Execute(item.Color);
        }
    }

    private void OnHighlighterColorPresetPressed(object? sender, PointerPressedEventArgs e)
    {
        if (DataContext is not ImageEditorViewModel viewModel) return;
        if (sender is Border border && border.DataContext is HighlighterColorPresetItem item)
        {
            viewModel.SetHighlighterColorFromPresetCommand.Execute(item.Color);
        }
    }

    private void OnDragOver(object? sender, DragEventArgs e)
    {
        if (e.DataTransfer.TryGetFiles()?.Any() == true)
            e.DragEffects = DragDropEffects.Copy;
    }

    private async void OnDrop(object? sender, DragEventArgs e)
    {
        if (DataContext is not ImageEditorViewModel viewModel) return;
        var file = e.DataTransfer.TryGetFiles()?.First(x => x is IStorageFile);
        if (file is null)
            return;

        await viewModel.ImportByFile((IStorageFile)file);
    }

    public static bool CanExtendClientAreaToDecorationsHint => RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
}
