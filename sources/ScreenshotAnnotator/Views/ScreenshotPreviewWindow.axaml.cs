using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Input;
using ScreenshotAnnotator.ViewModels;
using System;
using System.Runtime.InteropServices;

namespace ScreenshotAnnotator.Views;

public partial class ScreenshotPreviewWindow : Window
{
    private ScreenshotPreviewViewModel? ViewModel => DataContext as ScreenshotPreviewViewModel;
    private string? _draggingAnchor;
    private bool _isDragging;
    private bool _isDraggingSelection;
    private Point _dragStartPoint;

    public ScreenshotPreviewWindow()
    {
        InitializeComponent();

        // Workaround with full screen not implemented by Avalonia for Ubuntu.
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            var screen = Screens.ScreenFromVisual(this);
            if (screen is not null)
            {
                this.Width = screen.Bounds.Width;
                this.Height = screen.Bounds.Height;
            }
        }
    }

    private void Canvas_PointerMoved(object? sender, PointerEventArgs e)
    {
        if (ViewModel == null) return;

        var position = e.GetPosition(MainCanvas);

        // Handle drag selection
        if (_isDraggingSelection)
        {
            var x = Math.Min(_dragStartPoint.X, position.X);
            var y = Math.Min(_dragStartPoint.Y, position.Y);
            var width = Math.Abs(position.X - _dragStartPoint.X);
            var height = Math.Abs(position.Y - _dragStartPoint.Y);

            ViewModel.UpdateSelectionRect(new Rect(x, y, width, height));
        }

        // Show magnifier when hovering over the canvas
        ViewModel.UpdateMagnifier(position, true);
    }

    private void Canvas_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (ViewModel == null) return;

        var position = e.GetPosition(MainCanvas);

        // Hide floating buttons while dragging
        ViewModel.HideFloatingButtons();

        // Start drag selection
        _isDraggingSelection = true;
        _dragStartPoint = position;
        e.Pointer.Capture(MainCanvas);
        e.Handled = true;
    }

    private void Canvas_PointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (ViewModel == null) return;

        // End drag selection
        if (_isDraggingSelection)
        {
            _isDraggingSelection = false;
            e.Pointer.Capture(null);

            // Show floating buttons after selection
            ViewModel.ShowFloatingButtonsIfValid();
        }

        // Hide magnifier when not dragging
        if (!_isDragging)
        {
            ViewModel.UpdateMagnifier(default, false);
        }
    }

    private void Anchor_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (sender is not Ellipse ellipse) return;
        if (ellipse.Tag is not string anchorName) return;
        if (ViewModel == null) return;

        // Prevent canvas drag selection when dragging anchor
        _isDraggingSelection = false;

        // Hide floating buttons while dragging anchor
        ViewModel.HideFloatingButtons();

        _draggingAnchor = anchorName;
        _isDragging = true;
        e.Pointer.Capture(ellipse);
        e.Handled = true;
    }

    private void Anchor_PointerMoved(object? sender, PointerEventArgs e)
    {
        if (!_isDragging || _draggingAnchor == null || ViewModel == null) return;

        var position = e.GetPosition(MainCanvas);
        ViewModel.UpdateAnchorPoint(_draggingAnchor, position);

        // Update magnifier position while dragging
        ViewModel.UpdateMagnifier(position, true);

        e.Handled = true;
    }

    private void Anchor_PointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (sender is not Ellipse ellipse) return;

        _isDragging = false;
        _draggingAnchor = null;
        e.Pointer.Capture(null);
        e.Handled = true;

        // Hide magnifier after dragging
        if (ViewModel != null)
        {
            ViewModel.UpdateMagnifier(default, false);

            // Show floating buttons after anchor adjustment
            ViewModel.ShowFloatingButtonsIfValid();
        }
    }

    protected override void OnDataContextChanged(System.EventArgs e)
    {
        base.OnDataContextChanged(e);

        if (DataContext is ScreenshotPreviewViewModel viewModel)
        {
            // Subscribe to property changes to close window when confirmed or cancelled
            viewModel.PropertyChanged += (s, args) =>
            {
                if (args.PropertyName == nameof(ScreenshotPreviewViewModel.IsConfirmed))
                {
                    Close();
                }
            };
        }
    }
}
