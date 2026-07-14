using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Platform.Storage;
using ScreenshotAnnotator.Controls;
using ScreenshotAnnotator.Resources;
using ScreenshotAnnotator.ViewModels;
using ScreenshotAnnotator.Services;
using System.Runtime.InteropServices;

namespace ScreenshotAnnotator.Views;

public partial class MainWindow : Window
{
    private Border? _selectorFloatingPanel;

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
            var editorCanvas = this.FindControl<ImageEditorCanvas>("EditorCanvas");
            var overlayCanvas = this.FindControl<Canvas>("OverlayCanvas");
            var cutOverlay = this.FindControl<CutAnimationOverlay>("CutOverlay");

            if (editorCanvas != null && overlayCanvas != null)
            {
                editorCanvas.OverlayCanvas = overlayCanvas;

                if (cutOverlay != null)
                {
                    editorCanvas.CutRequested += (_, args) =>
                        cutOverlay.Play(args.IsVertical, args.Start, args.End,
                            editorCanvas.ExecutePendingCut);
                }

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

                    SetupSelectorFloatingButtons(editorCanvas, overlayCanvas, viewModel);

                    var hotkeyService = AllServices.GlobalHotkeyService;
                    hotkeyService.Enabled = viewModel.EnablePrintScreenHotkey;
                    hotkeyService.PrintScreenPressed += async () =>
                        await viewModel.TakeScreenshotCommand.ExecuteAsync(null);
                    hotkeyService.Start();
                }
            }
        };
    }

    private void SetupSelectorFloatingButtons(
        ImageEditorCanvas editorCanvas, Canvas overlayCanvas, ImageEditorViewModel viewModel)
    {
        var panel = BuildSelectorFloatingPanel(editorCanvas, viewModel);
        panel.IsVisible = false;
        overlayCanvas.Children.Add(panel);
        _selectorFloatingPanel = panel;

        // Hide buttons when user starts a new drag (pointer pressed on canvas)
        editorCanvas.PointerPressed += (_, _) =>
        {
            if (_selectorFloatingPanel != null)
                _selectorFloatingPanel.IsVisible = false;
        };

        // Show/hide based on selector rect state
        editorCanvas.SelectorRectChanged += (_, args) =>
        {
            if (_selectorFloatingPanel == null) return;

            if (args.Rect == null)
            {
                _selectorFloatingPanel.IsVisible = false;
                return;
            }

            var r = args.Rect.Rectangle.Normalize();
            var imageSize = editorCanvas.Image?.PixelSize;
            var imageW = imageSize?.Width ?? editorCanvas.Bounds.Width;
            var imageH = imageSize?.Height ?? editorCanvas.Bounds.Height;
            var margin = 8.0;

            _selectorFloatingPanel.Measure(Size.Infinity);
            var panelW = _selectorFloatingPanel.DesiredSize.Width;
            var panelH = _selectorFloatingPanel.DesiredSize.Height;

            var left = Clamp(r.Center.X - panelW / 2.0, margin, imageW - panelW - margin);

            // Prefer outside the selected pixels, but keep the menu visible for full-image and tiny selections.
            var top = r.Bottom + margin + panelH <= imageH
                ? r.Bottom + margin
                : r.Top - panelH - margin;
            top = Clamp(top, margin, imageH - panelH - margin);

            Canvas.SetLeft(_selectorFloatingPanel, left);
            Canvas.SetTop(_selectorFloatingPanel, top);
            _selectorFloatingPanel.IsVisible = true;
        };
    }

    private Border BuildSelectorFloatingPanel(ImageEditorCanvas editorCanvas, ImageEditorViewModel viewModel)
    {
        var copyBtn = MakeFloatingButton(
            Strings.Selection_Copy,
            Strings.Tooltip_Selection_Copy,
            "#2D2D30",
            async () => await viewModel.CopyToClipboardCommand.ExecuteAsync(null));

        var deleteBtn = MakeFloatingButton(
            Strings.Selection_Delete,
            Strings.Tooltip_Selection_Delete,
            "#2D2D30",
            () =>
            {
                editorCanvas.WhiteOutCurrentSelectorArea();
                if (_selectorFloatingPanel != null)
                    _selectorFloatingPanel.IsVisible = false;
            });

        var ocrBtn = MakeFloatingButton(
            Strings.Selection_ExtractText,
            Strings.Tooltip_Selection_ExtractText,
            "#1A3A5C",
            async () =>
            {
                if (_selectorFloatingPanel != null)
                    _selectorFloatingPanel.IsVisible = false;
                await viewModel.ExtractTextCommand.ExecuteAsync(null);
                // Restore visibility if selector still active
                if (editorCanvas.SelectorRect != null && _selectorFloatingPanel != null)
                    _selectorFloatingPanel.IsVisible = true;
            });

        var stack = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Spacing = 6,
            Children = { copyBtn, deleteBtn, ocrBtn }
        };

        var panel = new Border
        {
            Background = new SolidColorBrush(Color.FromArgb(220, 30, 30, 30)),
            CornerRadius = new CornerRadius(6),
            Padding = new Thickness(8, 6),
            Opacity = 0.7,
            Child = stack,
            BoxShadow = new BoxShadows(new BoxShadow
            {
                Blur = 8, OffsetX = 0, OffsetY = 2,
                Color = Color.FromArgb(160, 0, 0, 0)
            })
        };

        panel.PointerEntered += (_, _) => panel.Opacity = 1.0;
        panel.PointerExited += (_, _) => panel.Opacity = 0.7;

        return panel;
    }

    private static Button MakeFloatingButton(string label, string tooltip, string bgHex, System.Action action)
    {
        var btn = new Button
        {
            Content = label,
            Background = new SolidColorBrush(Color.Parse(bgHex)),
            Foreground = new SolidColorBrush(Colors.White),
            BorderThickness = new Thickness(0),
            CornerRadius = new CornerRadius(4),
            Padding = new Thickness(10, 5),
            FontSize = 12
        };
        ToolTip.SetTip(btn, tooltip);
        btn.Click += (_, _) => action();
        return btn;
    }

    private static Button MakeFloatingButton(string label, string tooltip, string bgHex, System.Func<System.Threading.Tasks.Task> action)
    {
        var btn = new Button
        {
            Content = label,
            Background = new SolidColorBrush(Color.Parse(bgHex)),
            Foreground = new SolidColorBrush(Colors.White),
            BorderThickness = new Thickness(0),
            CornerRadius = new CornerRadius(4),
            Padding = new Thickness(10, 5),
            FontSize = 12
        };
        ToolTip.SetTip(btn, tooltip);
        btn.Click += async (_, _) => await action();
        return btn;
    }

    private static double Clamp(double value, double min, double max)
    {
        if (max < min) return min;
        return value < min ? min : value > max ? max : value;
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
        else if (e.Key == Key.PrintScreen && !AllServices.GlobalHotkeyService.IsRunning)
        {
            if (viewModel.EnablePrintScreenHotkey)
            {
                await viewModel.TakeScreenshotCommand.ExecuteAsync(null);
                e.Handled = true;
            }
        }
    }

    private async void OnClosing(object? sender, WindowClosingEventArgs e)
    {
        if (DataContext is ImageEditorViewModel viewModel)
        {
            e.Cancel = true;

            await viewModel.SaveCurrentProject();
            viewModel.CloseProject();
            AllServices.GlobalHotkeyService.Dispose();
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
