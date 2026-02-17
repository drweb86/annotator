using System.Linq;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Platform.Storage;
using ScreenshotAnnotator.ViewModels;

namespace ScreenshotAnnotator.Views;

public partial class ImageEditorView : UserControl
{
    public ImageEditorView()
    {
        InitializeComponent();

        var mainGrid = this.FindControl<Grid>("MainContentGrid");
        if (mainGrid != null)
        {
            DragDrop.SetAllowDrop(mainGrid, true);
            mainGrid.AddHandler(DragDrop.DragOverEvent, OnDragOver);
            mainGrid.AddHandler(DragDrop.DropEvent, OnDrop);
        }

        // Connect the overlay canvas to the editor canvas
        this.Loaded += (s, e) =>
        {
            var editorCanvas = this.FindControl<Controls.ImageEditorCanvas>("EditorCanvas");
            var overlayCanvas = this.FindControl<Canvas>("OverlayCanvas");

                if (editorCanvas != null && overlayCanvas != null)
            {
                editorCanvas.OverlayCanvas = overlayCanvas;

                // Connect the editor canvas to the view model
                if (DataContext is ImageEditorViewModel viewModel)
                {
                    viewModel.SetEditorCanvas(editorCanvas);
                    editorCanvas.SelectedShapeChanged += (_, _) =>
                    {
                        viewModel.SelectedShape = editorCanvas.SelectedShape;
                    };

                    // Set the TopLevel for clipboard access
                    var topLevel = TopLevel.GetTopLevel(this);
                    if (topLevel != null)
                    {
                        viewModel.SetTopLevel(topLevel);
                    }

                    // Load project files on startup
                    viewModel.RefreshProjectFilesCommand.Execute(null);
                }
            }
        };

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

    private void OnFileListTapped(object? sender, Avalonia.Input.TappedEventArgs e)
    {
        // Single click to open project
        if (sender is ListBox listBox && listBox.SelectedItem is Services.ProjectFileInfo fileInfo)
        {
            // Check if the tap was on the delete button by checking if it's a Button
            if (e.Source is Button)
            {
                // Don't open if delete button was clicked
                return;
            }

            if (DataContext is ImageEditorViewModel viewModel)
            {
                _ = viewModel.OpenProjectFileCommand.ExecuteAsync(fileInfo);
            }
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

}
