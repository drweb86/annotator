using Avalonia.Controls;
using Avalonia.Input;
using ScreenshotAnnotator.ViewModels;

namespace ScreenshotAnnotator.Views;

public partial class ImageEditorView : UserControl
{
    public ImageEditorView()
    {
        InitializeComponent();

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

        // Add keyboard shortcuts
        this.KeyDown += OnKeyDown;
    }

    private void OnFileListDoubleTapped(object? sender, Avalonia.Input.TappedEventArgs e)
    {
        if (sender is ListBox listBox && listBox.SelectedItem is Services.ProjectFileInfo fileInfo)
        {
            if (DataContext is ImageEditorViewModel viewModel)
            {
                _ = viewModel.OpenProjectFileCommand.ExecuteAsync(fileInfo);
            }
        }
    }

    private async void OnKeyDown(object? sender, KeyEventArgs e)
    {
        if (DataContext is not ImageEditorViewModel viewModel) return;

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
            await viewModel.LoadImageCommand.ExecuteAsync(null);
            e.Handled = true;
        }
        // Ctrl+Shift+O - Open project
        else if (isCtrl && isShift && e.Key == Key.O)
        {
            await viewModel.OpenProjectCommand.ExecuteAsync(null);
            e.Handled = true;
        }
        // Ctrl+S - Save project
        else if (isCtrl && !isShift && e.Key == Key.S)
        {
            await viewModel.SaveProjectCommand.ExecuteAsync(null);
            e.Handled = true;
        }
        // Ctrl+Shift+S - Save project as
        else if (isCtrl && isShift && e.Key == Key.S)
        {
            await viewModel.SaveProjectAsCommand.ExecuteAsync(null);
            e.Handled = true;
        }
        // Ctrl+E - Export/Save as image
        else if (isCtrl && !isShift && e.Key == Key.E)
        {
            await viewModel.SaveAsImageCommand.ExecuteAsync(null);
            e.Handled = true;
        }
        // F9 - Toggle file browser
        else if (e.Key == Key.F9)
        {
            viewModel.ToggleFileBrowserCommand.Execute(null);
            e.Handled = true;
        }
        // F12 - Take screenshot
        else if (e.Key == Key.F12)
        {
            await viewModel.TakeScreenshotCommand.ExecuteAsync(null);
            e.Handled = true;
        }
    }
}
