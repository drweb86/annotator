using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Input.Platform;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using ScreenshotAnnotator.Models;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace ScreenshotAnnotator.Controls;

public class ImageEditorCanvas : Control
{
    private Point _startPoint;
    private bool _isDrawing;
    private AnnotationShape? _currentShape;
    private SelectorRectangle? _currentSelectorRect;
    private bool _isDraggingBeak;
    private AnnotationShape? _selectedShape;
    private bool _isDraggingShape;
    private Point _lastMousePosition;
    private bool _isDraggingArrowStart;
    private bool _isDraggingArrowEnd;
    private bool _isDraggingCalloutCorner;
    private CalloutShape.Corner _draggedCorner;
    private TextBox? _textEditor;
    private CalloutShape? _editingCallout;
    private CalloutNoArrowShape? _editingCalloutNoArrow;

    public Canvas? OverlayCanvas { get; set; }

    public static readonly StyledProperty<Bitmap?> ImageProperty =
        AvaloniaProperty.Register<ImageEditorCanvas, Bitmap?>(nameof(Image));

    public static readonly StyledProperty<ToolType> CurrentToolProperty =
        AvaloniaProperty.Register<ImageEditorCanvas, ToolType>(nameof(CurrentTool));

    public static readonly StyledProperty<ObservableCollection<AnnotationShape>> ShapesProperty =
        AvaloniaProperty.Register<ImageEditorCanvas, ObservableCollection<AnnotationShape>>(
            nameof(Shapes), new ObservableCollection<AnnotationShape>());

    public Bitmap? Image
    {
        get => GetValue(ImageProperty);
        set => SetValue(ImageProperty, value);
    }

    public ToolType CurrentTool
    {
        get => GetValue(CurrentToolProperty);
        set => SetValue(CurrentToolProperty, value);
    }

    public ObservableCollection<AnnotationShape> Shapes
    {
        get => GetValue(ShapesProperty);
        set => SetValue(ShapesProperty, value);
    }

    public SelectorRectangle? SelectorRect
    {
        get => _currentSelectorRect;
        set
        {
            _currentSelectorRect = value;
            InvalidateVisual();
        }
    }

    /// <summary>Currently selected shape (when using Select tool). Null when none selected.</summary>
    public AnnotationShape? SelectedShape => _selectedShape;

    /// <summary>Raised when the selected shape changes (select, deselect, delete, or tool change).</summary>
    public event EventHandler? SelectedShapeChanged;

    private void SetSelectedShape(AnnotationShape? shape)
    {
        if (_selectedShape == shape) return;
        if (_selectedShape != null)
            _selectedShape.IsSelected = false;
        _selectedShape = shape;
        if (_selectedShape != null)
            _selectedShape.IsSelected = true;
        SelectedShapeChanged?.Invoke(this, EventArgs.Empty);
        InvalidateVisual();
    }

    static ImageEditorCanvas()
    {
        AffectsRender<ImageEditorCanvas>(ImageProperty, ShapesProperty);
        CurrentToolProperty.Changed.AddClassHandler<ImageEditorCanvas>((x, e) => x.OnCurrentToolChanged(e));
    }

    public ImageEditorCanvas()
    {
        Shapes.CollectionChanged += (s, e) =>
        {
            // Clear selector when shapes change (e.g., project loaded)
            ClearSelector();
            InvalidateVisual();
        };
        DoubleTapped += OnDoubleTapped;
        Focusable = true; // Allow the canvas to receive keyboard input
    }

    private void OnCurrentToolChanged(AvaloniaPropertyChangedEventArgs e)
    {
        // Clear selector rectangle when switching away from selector tool
        if (e.OldValue is ToolType oldTool && oldTool == ToolType.Selector)
        {
            ClearSelector();
        }

        // Also clear selected shape when switching tools
        SetSelectedShape(null);
    }

    public void ClearSelector()
    {
        _currentSelectorRect = null;
    }

    private void OnDoubleTapped(object? sender, TappedEventArgs e)
    {
        var point = e.GetPosition(this);

        // Check if double-clicking on a callout
        if (_selectedShape is CalloutShape callout && callout.HitTest(point))
        {
            ShowTextEditor(callout);
            e.Handled = true;
        }

        // Check if double-clicking on a callout no arrow
        if (_selectedShape is CalloutNoArrowShape calloutNoArrow && calloutNoArrow.HitTest(point))
        {
            ShowTextEditorForCalloutNoArrow(calloutNoArrow);
            e.Handled = true;
        }
    }

    private void ShowTextEditor(CalloutShape callout, string? initialText = null)
    {
        if (OverlayCanvas == null) return;

        // Hide any existing text editor
        HideTextEditor();

        _editingCallout = callout;

        // Create a TextBox for editing
        _textEditor = new TextBox
        {
            Text = callout.Text,
            Width = Math.Max(callout.Rectangle.Width, 100),
            Height = Math.Max(callout.Rectangle.Height, 50),
            TextWrapping = Avalonia.Media.TextWrapping.Wrap,
            AcceptsReturn = true,
            BorderThickness = new Thickness(2),
            BorderBrush = new SolidColorBrush(Color.FromRgb(255, 165, 0)), // Orange border
            Background = new SolidColorBrush(Color.FromRgb(40, 40, 40)), // Dark gray background
            Padding = new Thickness(10),
            FontFamily = new Avalonia.Media.FontFamily(callout.FontFamily),
            FontSize = callout.FontSize,
            FontWeight = callout.FontBold ? FontWeight.Bold : FontWeight.Normal,
            FontStyle = callout.FontItalic ? FontStyle.Italic : FontStyle.Normal,
            TextAlignment = TextAlignment.Center,
            VerticalContentAlignment = Avalonia.Layout.VerticalAlignment.Center
        };

        // Position the TextBox
        Canvas.SetLeft(_textEditor, callout.Rectangle.Left + 5);
        Canvas.SetTop(_textEditor, callout.Rectangle.Top + 5);

        // Handle when user finishes editing
        _textEditor.LostFocus += (s, e) => OnTextEditingComplete();
        _textEditor.KeyDown += (s, e) =>
        {
            if (e.Key == Key.Escape)
            {
                HideTextEditor();
                e.Handled = true;
            }
        };

        // Add to overlay canvas
        OverlayCanvas.Children.Add(_textEditor);
        _textEditor.Focus();

        // If initial text was provided (from keystroke), append it instead of selecting all
        if (!string.IsNullOrEmpty(initialText))
        {
            _textEditor.Text = callout.Text + initialText;
            _textEditor.CaretIndex = _textEditor.Text.Length;
        }
        else
        {
            _textEditor.SelectAll();
        }
    }

    private void ShowTextEditorForCalloutNoArrow(CalloutNoArrowShape calloutNoArrow, string? initialText = null)
    {
        if (OverlayCanvas == null) return;

        // Hide any existing text editor
        HideTextEditor();

        _editingCalloutNoArrow = calloutNoArrow;

        // Create a TextBox for editing
        _textEditor = new TextBox
        {
            Text = calloutNoArrow.Text,
            Width = Math.Max(calloutNoArrow.Rectangle.Width, 100),
            Height = Math.Max(calloutNoArrow.Rectangle.Height, 50),
            TextWrapping = Avalonia.Media.TextWrapping.Wrap,
            AcceptsReturn = true,
            BorderThickness = new Thickness(2),
            BorderBrush = new SolidColorBrush(Color.FromRgb(255, 165, 0)), // Orange border
            Padding = new Thickness(10),
            Background = new SolidColorBrush(Color.FromRgb(40, 40, 40)), // Dark gray background
            FontFamily = new Avalonia.Media.FontFamily(calloutNoArrow.FontFamily),
            FontSize = calloutNoArrow.FontSize,
            FontWeight = calloutNoArrow.FontBold ? FontWeight.Bold : FontWeight.Normal,
            FontStyle = calloutNoArrow.FontItalic ? FontStyle.Italic : FontStyle.Normal,
            TextAlignment = TextAlignment.Center,
            VerticalContentAlignment = Avalonia.Layout.VerticalAlignment.Center
        };

        // Position the TextBox
        Canvas.SetLeft(_textEditor, calloutNoArrow.Rectangle.Left + 5);
        Canvas.SetTop(_textEditor, calloutNoArrow.Rectangle.Top + 5);

        // Handle when user finishes editing
        _textEditor.LostFocus += (s, e) => OnTextEditingComplete();
        _textEditor.KeyDown += (s, e) =>
        {
            if (e.Key == Key.Escape)
            {
                HideTextEditor();
                e.Handled = true;
            }
        };

        // Add to overlay canvas
        OverlayCanvas.Children.Add(_textEditor);
        _textEditor.Focus();

        // If initial text was provided (from keystroke), append it instead of selecting all
        if (!string.IsNullOrEmpty(initialText))
        {
            _textEditor.Text = calloutNoArrow.Text + initialText;
            _textEditor.CaretIndex = _textEditor.Text.Length;
        }
        else
        {
            _textEditor.SelectAll();
        }
    }

    private void OnTextEditingComplete()
    {
        if (_textEditor != null && _editingCallout != null)
        {
            _editingCallout.Text = _textEditor.Text ?? "";
            InvalidateVisual();
        }
        if (_textEditor != null && _editingCalloutNoArrow != null)
        {
            _editingCalloutNoArrow.Text = _textEditor.Text ?? "";
            InvalidateVisual();
        }
        HideTextEditor();
    }

    private void HideTextEditor()
    {
        if (_textEditor != null && OverlayCanvas != null)
        {
            if (OverlayCanvas.Children.Contains(_textEditor))
            {
                OverlayCanvas.Children.Remove(_textEditor);
            }
            _textEditor = null;
            _editingCallout = null;
            _editingCalloutNoArrow = null;
        }
    }

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);

        var point = e.GetPosition(this);
        _startPoint = point;
        _lastMousePosition = point;

        // If no specific tool is selected, try to select/drag existing shapes
        if (CurrentTool == ToolType.None)
        {
            // Check if clicking on a selected arrow's handles
            if (_selectedShape is ArrowShape selectedArrow)
            {
                if (selectedArrow.IsPointOnStartHandle(point))
                {
                    _isDraggingArrowStart = true;
                    return;
                }
                if (selectedArrow.IsPointOnEndHandle(point))
                {
                    _isDraggingArrowEnd = true;
                    return;
                }
            }

            // Check if clicking on a selected callout's handles
            if (_selectedShape is CalloutShape selectedCallout)
            {
                if (selectedCallout.IsPointOnBeak(point))
                {
                    _isDraggingBeak = true;
                    return;
                }

                if (selectedCallout.IsPointOnCornerHandle(point, out var corner))
                {
                    _isDraggingCalloutCorner = true;
                    _draggedCorner = corner;
                    return;
                }
            }

            // Check if clicking on a selected callout no arrow's handles
            if (_selectedShape is CalloutNoArrowShape selectedCalloutNoArrow)
            {
                if (selectedCalloutNoArrow.IsPointOnCornerHandle(point, out var corner))
                {
                    _isDraggingCalloutCorner = true;
                    _draggedCorner = corner;
                    return;
                }
            }

            // Check if clicking on a selected bordered rectangle's handles
            if (_selectedShape is BorderedRectangleShape selectedBorderedRect)
            {
                if (selectedBorderedRect.IsPointOnCornerHandle(point, out var corner))
                {
                    _isDraggingCalloutCorner = true;
                    _draggedCorner = corner;
                    return;
                }
            }

            // Check if clicking on a selected blur rectangle's handles
            if (_selectedShape is BlurRectangleShape selectedBlurRect)
            {
                if (selectedBlurRect.IsPointOnCornerHandle(point, out var corner))
                {
                    _isDraggingCalloutCorner = true;
                    _draggedCorner = corner;
                    return;
                }
            }

            // Check if clicking on a selected highlighter's handles
            if (_selectedShape is HighlighterShape selectedHighlighter)
            {
                var highlighterCorner = selectedHighlighter.GetCornerAtPoint(point);
                if (highlighterCorner != null)
                {
                    _isDraggingCalloutCorner = true;
                    _draggedCorner = (CalloutShape.Corner)(int)highlighterCorner;
                    return;
                }
            }

            // Check if clicking on any existing shape (reverse order to select top-most)
            for (int i = Shapes.Count - 1; i >= 0; i--)
            {
                if (Shapes[i].HitTest(point))
                {
                    // Select this shape
                    SetSelectedShape(Shapes[i]);
                    _isDraggingShape = true;
                    Focus(); // Set focus to receive keyboard events
                    return;
                }
            }

            // Click on empty space - deselect
            SetSelectedShape(null);
        }
        else
        {
            // Tool is selected - check if clicking on existing shape first
            bool clickedOnShape = false;
            for (int i = Shapes.Count - 1; i >= 0; i--)
            {
                if (Shapes[i].HitTest(point))
                {
                    // User clicked on existing shape - switch to selector tool
                    CurrentTool = ToolType.None;

                    // Select this shape
                    SetSelectedShape(Shapes[i]);
                    _isDraggingShape = true;
                    Focus(); // Set focus to receive keyboard events
                    InvalidateVisual();
                    clickedOnShape = true;
                    return;
                }
            }

            // No shape was clicked - create new shape
            if (!clickedOnShape)
            {
                _isDrawing = true;

                switch (CurrentTool)
                {
                    case ToolType.Arrow:
                        _currentShape = new ArrowShape
                        {
                            StartPoint = point,
                            EndPoint = point
                        };
                        break;

                    case ToolType.Callout:
                        _currentShape = new CalloutShape
                        {
                            Rectangle = new Rect(point, new Size(0, 0)),
                            BeakPoint = point
                        };
                        _isDraggingBeak = false;
                        break;

                    case ToolType.CalloutNoArrow:
                        _currentShape = new CalloutNoArrowShape
                        {
                            Rectangle = new Rect(point, new Size(0, 0))
                        };
                        break;

                    case ToolType.BorderedRectangle:
                        _currentShape = new BorderedRectangleShape
                        {
                            Rectangle = new Rect(point, new Size(0, 0))
                        };
                        break;

                    case ToolType.BlurRectangle:
                        _currentShape = new BlurRectangleShape
                        {
                            Rectangle = new Rect(point, new Size(0, 0))
                        };
                        break;

                    case ToolType.Selector:
                        _currentSelectorRect = new SelectorRectangle
                        {
                            Rectangle = new Rect(point, new Size(0, 0))
                        };
                        break;

                    case ToolType.Highlighter:
                        _currentShape = new HighlighterShape
                        {
                            StartPoint = point,
                            EndPoint = point
                        };
                        break;

                    case ToolType.VerticalCutOut:
                    case ToolType.HorizontalCutOut:
                        // Cut out tools will be handled in OnPointerReleased
                        _isDrawing = true;
                        break;
                }

                InvalidateVisual();
            }
        }
    }

    protected override void OnPointerMoved(PointerEventArgs e)
    {
        base.OnPointerMoved(e);

        var point = e.GetPosition(this);

        // Handle dragging arrow start handle
        if (_isDraggingArrowStart && _selectedShape is ArrowShape arrowStart)
        {
            arrowStart.MoveStartPoint(point);
            InvalidateVisual();
            return;
        }

        // Handle dragging arrow end handle
        if (_isDraggingArrowEnd && _selectedShape is ArrowShape arrowEnd)
        {
            arrowEnd.MoveEndPoint(point);
            InvalidateVisual();
            return;
        }

        // Handle dragging callout corner
        if (_isDraggingCalloutCorner && _selectedShape is CalloutShape calloutResize)
        {
            calloutResize.ResizeFromCorner(_draggedCorner, point);
            InvalidateVisual();
            return;
        }

        // Handle dragging callout no arrow corner
        if (_isDraggingCalloutCorner && _selectedShape is CalloutNoArrowShape calloutNoArrowResize)
        {
            calloutNoArrowResize.ResizeFromCorner(_draggedCorner, point);
            InvalidateVisual();
            return;
        }

        // Handle dragging bordered rectangle corner
        if (_isDraggingCalloutCorner && _selectedShape is BorderedRectangleShape borderedRectResize)
        {
            borderedRectResize.ResizeFromCorner(_draggedCorner, point);
            InvalidateVisual();
            return;
        }

        // Handle dragging blur rectangle corner
        if (_isDraggingCalloutCorner && _selectedShape is BlurRectangleShape blurRectResize)
        {
            blurRectResize.ResizeFromCorner(_draggedCorner, point);
            InvalidateVisual();
            return;
        }

        // Handle dragging highlighter corner
        if (_isDraggingCalloutCorner && _selectedShape is HighlighterShape highlighterResize)
        {
            highlighterResize.ResizeCorner((HighlighterShape.Corner)(int)_draggedCorner, point);
            InvalidateVisual();
            return;
        }

        // Handle dragging existing shapes
        if (_isDraggingShape && _selectedShape != null)
        {
            var offset = new Vector(point.X - _lastMousePosition.X, point.Y - _lastMousePosition.Y);
            _selectedShape.Move(offset);
            _lastMousePosition = point;
            InvalidateVisual();
            return;
        }

        // Handle dragging callout beak
        if (_isDraggingBeak && _selectedShape is CalloutShape callout)
        {
            callout.MoveBeak(point);
            InvalidateVisual();
            return;
        }

        // Update last mouse position for cut tool visual feedback
        _lastMousePosition = point;

        // Handle drawing new shapes
        if (!_isDrawing) return;

        switch (CurrentTool)
        {
            case ToolType.Arrow:
                if (_currentShape is ArrowShape arrow)
                {
                    arrow.EndPoint = point;
                    InvalidateVisual();
                }
                break;

            case ToolType.Callout:
                if (_currentShape is CalloutShape newCallout)
                {
                    if (!_isDraggingBeak)
                    {
                        var rect = new Rect(_startPoint, point);
                        newCallout.Rectangle = rect;
                        // Position beak below the rectangle center by default
                        newCallout.BeakPoint = new Point(rect.Center.X, rect.Bottom + 30);
                    }
                    InvalidateVisual();
                }
                break;

            case ToolType.CalloutNoArrow:
                if (_currentShape is CalloutNoArrowShape newCalloutNoArrow)
                {
                    var rect = new Rect(_startPoint, point);
                    newCalloutNoArrow.Rectangle = rect;
                    InvalidateVisual();
                }
                break;

            case ToolType.BorderedRectangle:
                if (_currentShape is BorderedRectangleShape newBorderedRect)
                {
                    var rect = new Rect(_startPoint, point);
                    newBorderedRect.Rectangle = rect;
                    InvalidateVisual();
                }
                break;

            case ToolType.BlurRectangle:
                if (_currentShape is BlurRectangleShape newBlurRect)
                {
                    var rect = new Rect(_startPoint, point);
                    newBlurRect.Rectangle = rect;
                    InvalidateVisual();
                }
                break;

            case ToolType.Selector:
                if (_currentSelectorRect != null)
                {
                    _currentSelectorRect.Rectangle = new Rect(_startPoint, point);
                    InvalidateVisual();
                }
                break;

            case ToolType.Highlighter:
                if (_currentShape is HighlighterShape highlighter)
                {
                    highlighter.EndPoint = point;
                    InvalidateVisual();
                }
                break;

            case ToolType.VerticalCutOut:
            case ToolType.HorizontalCutOut:
                // Visual feedback will be shown in Render
                InvalidateVisual();
                break;
        }
    }

    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        base.OnPointerReleased(e);

        // Stop dragging
        if (_isDraggingArrowStart)
        {
            _isDraggingArrowStart = false;
            return;
        }

        if (_isDraggingArrowEnd)
        {
            _isDraggingArrowEnd = false;
            return;
        }

        if (_isDraggingCalloutCorner)
        {
            _isDraggingCalloutCorner = false;
            return;
        }

        if (_isDraggingShape)
        {
            _isDraggingShape = false;
            return;
        }

        if (_isDraggingBeak)
        {
            _isDraggingBeak = false;
            return;
        }

        if (!_isDrawing) return;

        _isDrawing = false;

        // Handle cut out tools
        var point = e.GetPosition(this);
        if (CurrentTool == ToolType.VerticalCutOut)
        {
            // Clamp to image boundaries
            var clampedStartX = ClampX(_startPoint.X);
            var clampedEndX = ClampX(point.X);
            PerformVerticalCutOut(clampedStartX, clampedEndX);
            InvalidateVisual();
            return;
        }
        else if (CurrentTool == ToolType.HorizontalCutOut)
        {
            // Clamp to image boundaries
            var clampedStartY = ClampY(_startPoint.Y);
            var clampedEndY = ClampY(point.Y);
            PerformHorizontalCutOut(clampedStartY, clampedEndY);
            InvalidateVisual();
            return;
        }

        // Add the completed shape to the collection
        if (_currentShape != null)
        {
            // Only add if the shape has some size
            bool shouldAdd = false;

            if (_currentShape is ArrowShape arrow)
            {
                var distance = Math.Sqrt(
                    Math.Pow(arrow.EndPoint.X - arrow.StartPoint.X, 2) +
                    Math.Pow(arrow.EndPoint.Y - arrow.StartPoint.Y, 2)
                );
                shouldAdd = distance > 5;
            }
            else if (_currentShape is CalloutShape callout)
            {
                shouldAdd = callout.Rectangle.Width > 10 && callout.Rectangle.Height > 10;
            }
            else if (_currentShape is CalloutNoArrowShape calloutNoArrow)
            {
                shouldAdd = calloutNoArrow.Rectangle.Width > 10 && calloutNoArrow.Rectangle.Height > 10;
            }
            else if (_currentShape is BorderedRectangleShape borderedRect)
            {
                shouldAdd = borderedRect.Rectangle.Width > 10 && borderedRect.Rectangle.Height > 10;
            }
            else if (_currentShape is BlurRectangleShape blurRect)
            {
                shouldAdd = blurRect.Rectangle.Width > 10 && blurRect.Rectangle.Height > 10;
                if (shouldAdd)
                {
                    // Generate blurred image from the underlying image area
                    blurRect.BlurredImage = CreateBlurredImage(blurRect.Rectangle);
                    // Set up callback to refresh blur when moved/resized
                    blurRect.RefreshBlur = CreateBlurredImage;
                }
            }
            else if (_currentShape is HighlighterShape highlighter)
            {
                var rect = new Rect(highlighter.StartPoint, highlighter.EndPoint);
                shouldAdd = rect.Width > 5 && rect.Height > 5;
            }

            if (shouldAdd)
            {
                Shapes.Add(_currentShape);
            }

            _currentShape = null;
        }

        InvalidateVisual();
    }

    public override void Render(DrawingContext context)
    {
        base.Render(context);

        // Draw the image
        if (Image != null)
        {
            var imageRect = new Rect(0, 0, Image.PixelSize.Width, Image.PixelSize.Height);
            context.DrawImage(Image, imageRect);
        }

        // Draw all completed shapes
        foreach (var shape in Shapes)
        {
            shape.Render(context);
        }

        // Draw the shape being created
        _currentShape?.Render(context);

        // Draw selector rectangle
        _currentSelectorRect?.Render(context);

        // Draw visual feedback for cut tools
        if (_isDrawing && Image != null)
        {
            if (CurrentTool == ToolType.VerticalCutOut)
            {
                // Clamp to image boundaries
                var clampedStartX = ClampX(_startPoint.X);
                var clampedEndX = ClampX(_lastMousePosition.X);
                var leftX = Math.Min(clampedStartX, clampedEndX);
                var rightX = Math.Max(clampedStartX, clampedEndX);
                var height = Image.PixelSize.Height;

                var cutBrush = new SolidColorBrush(Color.FromArgb(100, 255, 0, 0)); // Semi-transparent red
                var cutPen = new Pen(new SolidColorBrush(Colors.Red), 2.0);
                cutPen.DashStyle = new DashStyle(new[] { 4.0, 4.0 }, 0);

                var cutRect = new Rect(leftX, 0, rightX - leftX, height);
                context.DrawRectangle(cutBrush, cutPen, cutRect);
            }
            else if (CurrentTool == ToolType.HorizontalCutOut)
            {
                // Clamp to image boundaries
                var clampedStartY = ClampY(_startPoint.Y);
                var clampedEndY = ClampY(_lastMousePosition.Y);
                var topY = Math.Min(clampedStartY, clampedEndY);
                var bottomY = Math.Max(clampedStartY, clampedEndY);
                var width = Image.PixelSize.Width;

                var cutBrush = new SolidColorBrush(Color.FromArgb(100, 255, 0, 0)); // Semi-transparent red
                var cutPen = new Pen(new SolidColorBrush(Colors.Red), 2.0);
                cutPen.DashStyle = new DashStyle(new[] { 4.0, 4.0 }, 0);

                var cutRect = new Rect(0, topY, width, bottomY - topY);
                context.DrawRectangle(cutBrush, cutPen, cutRect);
            }
        }
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        if (Image != null)
        {
            return new Size(Image.PixelSize.Width, Image.PixelSize.Height);
        }
        return base.MeasureOverride(availableSize);
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);

        // If text editor is already open, let it handle the keys
        if (_textEditor != null)
            return;

        // Select entire canvas on Ctrl+A
        if (e.Key == Key.A && e.KeyModifiers.HasFlag(KeyModifiers.Control) && Image != null)
        {
            // Switch to selector tool if not already selected
            if (CurrentTool != ToolType.Selector)
            {
                CurrentTool = ToolType.Selector;
            }

            // Create selector rectangle for entire image
            _currentSelectorRect = new SelectorRectangle
            {
                Rectangle = new Rect(0, 0, Image.PixelSize.Width, Image.PixelSize.Height)
            };
            InvalidateVisual();
            e.Handled = true;
            return;
        }

        // Delete selected shape on Delete key press
        if (e.Key == Key.Delete && _selectedShape != null)
        {
            var toRemove = _selectedShape;
            SetSelectedShape(null);
            Shapes.Remove(toRemove);
            e.Handled = true;
            return;
        }

        // Delete selector area on Delete key press (whites out the background image area only)
        if (e.Key == Key.Delete && _currentSelectorRect != null && CurrentTool == ToolType.Selector)
        {
            // White out the selected area of the base image
            WhiteOutImageArea(_currentSelectorRect.Rectangle);

            // Clear the selector
            _currentSelectorRect = null;
            InvalidateVisual();
            e.Handled = true;
            return;
        }

        // Copy selector area on Ctrl+C
        if (e.Key == Key.C && e.KeyModifiers.HasFlag(KeyModifiers.Control) &&
            _currentSelectorRect != null && CurrentTool == ToolType.Selector)
        {
            CopySelectorToClipboard();
            e.Handled = true;
            return;
        }

        if (e.Key == Key.C && e.KeyModifiers.HasFlag(KeyModifiers.Control) &&
            this.SelectedShape is not null && this._textEditor is null && this._imageEditorViewModel is not null &&
            TopLevel.GetTopLevel(this) is TopLevel topLevel)
        {
            this._imageEditorViewModel.ClipboardService.Copy(_imageEditorViewModel, topLevel.Clipboard, Services.ClipboardScope.Unknown);
            e.Handled = true;
            return;
        }

        if (e.Key == Key.V && e.KeyModifiers.HasFlag(KeyModifiers.Control) &&
            this.SelectedShape is not null && this._textEditor is null && this._imageEditorViewModel is not null &&
            TopLevel.GetTopLevel(this) is TopLevel topLevel2)
        {
            this._imageEditorViewModel.ClipboardService.Paste(_imageEditorViewModel, topLevel2.Clipboard);
            e.Handled = true;
            return;
        }

        // If a callout is selected and user presses a key, open text editor
        if (_selectedShape is CalloutShape callout && !e.Handled)
        {
            // Don't trigger on special keys
            if (e.Key != Key.Escape && e.Key != Key.Tab &&
                e.Key != Key.LeftCtrl && e.Key != Key.RightCtrl &&
                e.Key != Key.LeftAlt && e.Key != Key.RightAlt &&
                e.Key != Key.LeftShift && e.Key != Key.RightShift &&
                e.Key != Key.Delete && e.Key != Key.Back)
            {
                // Get the character representation of the key
                string? initialChar = GetCharFromKey(e.Key, e.KeyModifiers.HasFlag(KeyModifiers.Shift));
                ShowTextEditor(callout, initialChar);
                e.Handled = true;
            }
        }

        // If a callout no arrow is selected and user presses a key, open text editor
        if (_selectedShape is CalloutNoArrowShape calloutNoArrow && !e.Handled)
        {
            // Don't trigger on special keys
            if (e.Key != Key.Escape && e.Key != Key.Tab &&
                e.Key != Key.LeftCtrl && e.Key != Key.RightCtrl &&
                e.Key != Key.LeftAlt && e.Key != Key.RightAlt &&
                e.Key != Key.LeftShift && e.Key != Key.RightShift &&
                e.Key != Key.Delete && e.Key != Key.Back)
            {
                // Get the character representation of the key
                string? initialChar = GetCharFromKey(e.Key, e.KeyModifiers.HasFlag(KeyModifiers.Shift));
                ShowTextEditorForCalloutNoArrow(calloutNoArrow, initialChar);
                e.Handled = true;
            }
        }
    }

    private string? GetCharFromKey(Key key, bool shiftPressed)
    {
        // Convert Key enum to character string
        // Handle letters
        if (key >= Key.A && key <= Key.Z)
        {
            char c = (char)('a' + (key - Key.A));
            if (shiftPressed)
                c = char.ToUpper(c);
            return c.ToString();
        }

        // Handle numbers and their shift symbols
        if (key >= Key.D0 && key <= Key.D9)
        {
            if (shiftPressed)
            {
                return (key - Key.D0) switch
                {
                    0 => ")",
                    1 => "!",
                    2 => "@",
                    3 => "#",
                    4 => "$",
                    5 => "%",
                    6 => "^",
                    7 => "&",
                    8 => "*",
                    9 => "(",
                    _ => null
                };
            }
            return ((char)('0' + (key - Key.D0))).ToString();
        }

        // Handle numpad numbers
        if (key >= Key.NumPad0 && key <= Key.NumPad9)
        {
            return ((char)('0' + (key - Key.NumPad0))).ToString();
        }

        // Handle space
        if (key == Key.Space)
            return " ";

        // Handle common punctuation
        return key switch
        {
            Key.OemPeriod => shiftPressed ? ">" : ".",
            Key.OemComma => shiftPressed ? "<" : ",",
            Key.OemQuestion => shiftPressed ? "?" : "/",
            Key.OemSemicolon => shiftPressed ? ":" : ";",
            Key.OemQuotes => shiftPressed ? "\"" : "'",
            Key.OemOpenBrackets => shiftPressed ? "{" : "[",
            Key.OemCloseBrackets => shiftPressed ? "}" : "]",
            Key.OemPipe => shiftPressed ? "|" : "\\",
            Key.OemMinus => shiftPressed ? "_" : "-",
            Key.OemPlus => shiftPressed ? "+" : "=",
            Key.OemTilde => shiftPressed ? "~" : "`",
            _ => null
        };
    }

    private async void CopySelectorToClipboard()
    {
        if (_currentSelectorRect == null || Image == null) return;

        try
        {
            // First, render the full image with all shapes using the same method as CopyToClipboard
            var fullRenderedImage = RenderToImage();
            if (fullRenderedImage == null) return;

            var selectorRect = _currentSelectorRect.Rectangle;

            // Crop the rendered image to the selector area
            var cropX = (int)Math.Max(0, Math.Min(selectorRect.X, fullRenderedImage.PixelSize.Width));
            var cropY = (int)Math.Max(0, Math.Min(selectorRect.Y, fullRenderedImage.PixelSize.Height));
            var cropWidth = (int)Math.Max(1, Math.Min(selectorRect.Width, fullRenderedImage.PixelSize.Width - cropX));
            var cropHeight = (int)Math.Max(1, Math.Min(selectorRect.Height, fullRenderedImage.PixelSize.Height - cropY));

            var croppedImage = new RenderTargetBitmap(new PixelSize(cropWidth, cropHeight));

            using (var context = croppedImage.CreateDrawingContext())
            {
                // Draw the cropped portion from the full rendered image
                var sourceRect = new Rect(cropX, cropY, cropWidth, cropHeight);
                var destRect = new Rect(0, 0, cropWidth, cropHeight);

                context.DrawImage(fullRenderedImage, sourceRect, destRect);
            }

            // Copy to clipboard
            if (TopLevel.GetTopLevel(this) is TopLevel topLevel && topLevel.Clipboard != null)
            {
                using var stream = new MemoryStream();
                croppedImage.Save(stream);
                stream.Position = 0;

                var clipboard = topLevel.Clipboard;
                if (clipboard == null) return;
                await clipboard.SetValueAsync(DataFormat.Bitmap, new Bitmap(stream));
            }
        }
        catch
        {
            // Silently handle clipboard errors
        }
    }

    public RenderTargetBitmap? RenderToImage()
    {
        if (Image == null) return null;

        var width = Image.PixelSize.Width;
        var height = Image.PixelSize.Height;

        var renderTarget = new RenderTargetBitmap(new PixelSize(width, height), new Vector(96, 96));

        using (var context = renderTarget.CreateDrawingContext())
        {
            // Draw the base image
            var imageRect = new Rect(0, 0, width, height);
            context.DrawImage(Image, imageRect);

            // Draw all completed shapes
            foreach (var shape in Shapes)
            {
                shape.Render(context);
            }
        }

        return renderTarget;
    }

    public Bitmap? CreateBlurredImagePublic(Rect rect)
    {
        return CreateBlurredImage(rect);
    }

    private Bitmap? CreateBlurredImage(Rect rect)
    {
        if (Image == null) return null;

        try
        {
            // Clamp rectangle to image bounds
            var x = (int)Math.Max(0, rect.X);
            var y = (int)Math.Max(0, rect.Y);
            var width = (int)Math.Min(rect.Width, Image.PixelSize.Width - x);
            var height = (int)Math.Min(rect.Height, Image.PixelSize.Height - y);

            if (width <= 0 || height <= 0) return null;

            // Create a cropped version of the source area
            var croppedRect = new Rect(x, y, width, height);
            var tempBitmap = new RenderTargetBitmap(new PixelSize(width, height));

            using (var context = tempBitmap.CreateDrawingContext())
            {
                var sourceRect = new Rect(x, y, width, height);
                var destRect = new Rect(0, 0, width, height);
                context.DrawImage(Image, sourceRect, destRect);
            }

            // Apply box blur effect (multiple passes for smooth blur)
            return ApplyBoxBlur(tempBitmap, 15); // 15 pixel blur radius
        }
        catch
        {
            return null;
        }
    }

    private Bitmap ApplyBoxBlur(RenderTargetBitmap source, int radius)
    {
        // For simplicity, we'll downsample and upsample to create blur effect
        // This is more performant than implementing a full convolution kernel
        var width = source.PixelSize.Width;
        var height = source.PixelSize.Height;

        // Downsample factor creates blur effect
        var downscaleFactor = Math.Max(1, radius / 3);
        var smallWidth = Math.Max(1, width / downscaleFactor);
        var smallHeight = Math.Max(1, height / downscaleFactor);

        // Downsample
        var downsampled = new RenderTargetBitmap(new PixelSize(smallWidth, smallHeight));
        using (var context = downsampled.CreateDrawingContext())
        {
            context.DrawImage(source,
                new Rect(0, 0, width, height),
                new Rect(0, 0, smallWidth, smallHeight));
        }

        // Upsample back to original size
        var blurred = new RenderTargetBitmap(new PixelSize(width, height));
        using (var context = blurred.CreateDrawingContext())
        {
            context.DrawImage(downsampled,
                new Rect(0, 0, smallWidth, smallHeight),
                new Rect(0, 0, width, height));
        }

        // Convert to bitmap
        using var stream = new MemoryStream();
        blurred.Save(stream);
        stream.Position = 0;
        return new Bitmap(stream);
    }

    private void WhiteOutImageArea(Rect rect)
    {
        if (Image == null) return;

        try
        {
            var width = Image.PixelSize.Width;
            var height = Image.PixelSize.Height;

            // Create a new bitmap with the whited-out area
            var newImage = new RenderTargetBitmap(new PixelSize(width, height));

            using (var context = newImage.CreateDrawingContext())
            {
                // Draw the original image
                context.DrawImage(Image, new Rect(0, 0, width, height));

                // Draw white rectangle over the selected area
                var whiteBrush = new SolidColorBrush(Colors.White);
                context.DrawRectangle(whiteBrush, null, rect);
            }

            // Convert to bitmap and replace the image
            using var stream = new MemoryStream();
            newImage.Save(stream);
            stream.Position = 0;
            Image = new Bitmap(stream);
        }
        catch
        {
            // Silently handle errors
        }
    }

    private void PerformVerticalCutOut(double startX, double endX)
    {
        if (Image == null) return;

        try
        {
            var leftX = Math.Min(startX, endX);
            var rightX = Math.Max(startX, endX);
            var cutWidth = rightX - leftX;

            if (cutWidth < 5) return; // Too small to cut

            var oldWidth = Image.PixelSize.Width;
            var oldHeight = Image.PixelSize.Height;
            var newWidth = oldWidth - (int)cutWidth;

            if (newWidth <= 0) return;

            // Create new image without the vertical slice
            var newImage = new RenderTargetBitmap(new PixelSize(newWidth, oldHeight), new Vector(96, 96));

            using (var context = newImage.CreateDrawingContext())
            {
                // Draw left part
                if (leftX > 0)
                {
                    var sourceRect = new Rect(0, 0, leftX, oldHeight);
                    var destRect = new Rect(0, 0, leftX, oldHeight);
                    context.DrawImage(Image, sourceRect, destRect);
                }

                // Draw right part
                if (rightX < oldWidth)
                {
                    var sourceRect = new Rect(rightX, 0, oldWidth - rightX, oldHeight);
                    var destRect = new Rect(leftX, 0, oldWidth - rightX, oldHeight);
                    context.DrawImage(Image, sourceRect, destRect);
                }
            }

            // Adjust shapes
            foreach (var shape in Shapes.ToList())
            {
                AdjustShapeForVerticalCut(shape, leftX, cutWidth);
            }

            // Convert to bitmap and replace the image
            using var stream = new MemoryStream();
            newImage.Save(stream);
            stream.Position = 0;
            Image = new Bitmap(stream);
        }
        catch
        {
            // Silently handle errors
        }
    }

    private void PerformHorizontalCutOut(double startY, double endY)
    {
        if (Image == null) return;

        try
        {
            var topY = Math.Min(startY, endY);
            var bottomY = Math.Max(startY, endY);
            var cutHeight = bottomY - topY;

            if (cutHeight < 5) return; // Too small to cut

            var oldWidth = Image.PixelSize.Width;
            var oldHeight = Image.PixelSize.Height;
            var newHeight = oldHeight - (int)cutHeight;

            if (newHeight <= 0) return;

            // Create new image without the horizontal slice
            var newImage = new RenderTargetBitmap(new PixelSize(oldWidth, newHeight), new Vector(96, 96));

            using (var context = newImage.CreateDrawingContext())
            {
                // Draw top part
                if (topY > 0)
                {
                    var sourceRect = new Rect(0, 0, oldWidth, topY);
                    var destRect = new Rect(0, 0, oldWidth, topY);
                    context.DrawImage(Image, sourceRect, destRect);
                }

                // Draw bottom part
                if (bottomY < oldHeight)
                {
                    var sourceRect = new Rect(0, bottomY, oldWidth, oldHeight - bottomY);
                    var destRect = new Rect(0, topY, oldWidth, oldHeight - bottomY);
                    context.DrawImage(Image, sourceRect, destRect);
                }
            }

            // Adjust shapes
            foreach (var shape in Shapes.ToList())
            {
                AdjustShapeForHorizontalCut(shape, topY, cutHeight);
            }

            // Convert to bitmap and replace the image
            using var stream = new MemoryStream();
            newImage.Save(stream);
            stream.Position = 0;
            Image = new Bitmap(stream);
        }
        catch
        {
            // Silently handle errors
        }
    }

    private void AdjustShapeForVerticalCut(object shape, double cutX, double cutWidth)
    {
        if (shape is ArrowShape arrow)
        {
            arrow.StartPoint = AdjustPointForVerticalCut(arrow.StartPoint, cutX, cutWidth);
            arrow.EndPoint = AdjustPointForVerticalCut(arrow.EndPoint, cutX, cutWidth);
        }
        else if (shape is CalloutShape callout)
        {
            var rect = callout.Rectangle;
            var newTopLeft = AdjustPointForVerticalCut(rect.TopLeft, cutX, cutWidth);
            var newBottomRight = AdjustPointForVerticalCut(rect.BottomRight, cutX, cutWidth);
            callout.Rectangle = new Rect(newTopLeft, newBottomRight);
            callout.BeakPoint = AdjustPointForVerticalCut(callout.BeakPoint, cutX, cutWidth);
        }
        else if (shape is CalloutNoArrowShape calloutNoArrow)
        {
            var rect = calloutNoArrow.Rectangle;
            var newTopLeft = AdjustPointForVerticalCut(rect.TopLeft, cutX, cutWidth);
            var newBottomRight = AdjustPointForVerticalCut(rect.BottomRight, cutX, cutWidth);
            calloutNoArrow.Rectangle = new Rect(newTopLeft, newBottomRight);
        }
        else if (shape is BorderedRectangleShape borderedRect)
        {
            var rect = borderedRect.Rectangle;
            var newTopLeft = AdjustPointForVerticalCut(rect.TopLeft, cutX, cutWidth);
            var newBottomRight = AdjustPointForVerticalCut(rect.BottomRight, cutX, cutWidth);
            borderedRect.Rectangle = new Rect(newTopLeft, newBottomRight);
        }
        else if (shape is BlurRectangleShape blurRect)
        {
            var rect = blurRect.Rectangle;
            var newTopLeft = AdjustPointForVerticalCut(rect.TopLeft, cutX, cutWidth);
            var newBottomRight = AdjustPointForVerticalCut(rect.BottomRight, cutX, cutWidth);
            blurRect.Rectangle = new Rect(newTopLeft, newBottomRight);
        }
        else if (shape is HighlighterShape highlighter)
        {
            highlighter.StartPoint = AdjustPointForVerticalCut(highlighter.StartPoint, cutX, cutWidth);
            highlighter.EndPoint = AdjustPointForVerticalCut(highlighter.EndPoint, cutX, cutWidth);
        }
        else if (shape is SelectorRectangle selector)
        {
            var rect = selector.Rectangle;
            var newTopLeft = AdjustPointForVerticalCut(rect.TopLeft, cutX, cutWidth);
            var newBottomRight = AdjustPointForVerticalCut(rect.BottomRight, cutX, cutWidth);
            selector.Rectangle = new Rect(newTopLeft, newBottomRight);
        }
    }

    private void AdjustShapeForHorizontalCut(object shape, double cutY, double cutHeight)
    {
        if (shape is ArrowShape arrow)
        {
            arrow.StartPoint = AdjustPointForHorizontalCut(arrow.StartPoint, cutY, cutHeight);
            arrow.EndPoint = AdjustPointForHorizontalCut(arrow.EndPoint, cutY, cutHeight);
        }
        else if (shape is CalloutShape callout)
        {
            var rect = callout.Rectangle;
            var newTopLeft = AdjustPointForHorizontalCut(rect.TopLeft, cutY, cutHeight);
            var newBottomRight = AdjustPointForHorizontalCut(rect.BottomRight, cutY, cutHeight);
            callout.Rectangle = new Rect(newTopLeft, newBottomRight);
            callout.BeakPoint = AdjustPointForHorizontalCut(callout.BeakPoint, cutY, cutHeight);
        }
        else if (shape is CalloutNoArrowShape calloutNoArrow)
        {
            var rect = calloutNoArrow.Rectangle;
            var newTopLeft = AdjustPointForHorizontalCut(rect.TopLeft, cutY, cutHeight);
            var newBottomRight = AdjustPointForHorizontalCut(rect.BottomRight, cutY, cutHeight);
            calloutNoArrow.Rectangle = new Rect(newTopLeft, newBottomRight);
        }
        else if (shape is BorderedRectangleShape borderedRect)
        {
            var rect = borderedRect.Rectangle;
            var newTopLeft = AdjustPointForHorizontalCut(rect.TopLeft, cutY, cutHeight);
            var newBottomRight = AdjustPointForHorizontalCut(rect.BottomRight, cutY, cutHeight);
            borderedRect.Rectangle = new Rect(newTopLeft, newBottomRight);
        }
        else if (shape is BlurRectangleShape blurRect)
        {
            var rect = blurRect.Rectangle;
            var newTopLeft = AdjustPointForHorizontalCut(rect.TopLeft, cutY, cutHeight);
            var newBottomRight = AdjustPointForHorizontalCut(rect.BottomRight, cutY, cutHeight);
            blurRect.Rectangle = new Rect(newTopLeft, newBottomRight);
        }
        else if (shape is HighlighterShape highlighter)
        {
            highlighter.StartPoint = AdjustPointForHorizontalCut(highlighter.StartPoint, cutY, cutHeight);
            highlighter.EndPoint = AdjustPointForHorizontalCut(highlighter.EndPoint, cutY, cutHeight);
        }
        else if (shape is SelectorRectangle selector)
        {
            var rect = selector.Rectangle;
            var newTopLeft = AdjustPointForHorizontalCut(rect.TopLeft, cutY, cutHeight);
            var newBottomRight = AdjustPointForHorizontalCut(rect.BottomRight, cutY, cutHeight);
            selector.Rectangle = new Rect(newTopLeft, newBottomRight);
        }
    }

    private Point AdjustPointForVerticalCut(Point point, double cutX, double cutWidth)
    {
        if (point.X <= cutX)
        {
            return point;
        }
        else if (point.X > cutX + cutWidth)
        {
            return new Point(point.X - cutWidth, point.Y);
        }
        else
        {
            // Point is within cut area, move to cut edge
            return new Point(cutX, point.Y);
        }
    }

    private Point AdjustPointForHorizontalCut(Point point, double cutY, double cutHeight)
    {
        if (point.Y <= cutY)
        {
            return point;
        }
        else if (point.Y > cutY + cutHeight)
        {
            return new Point(point.X, point.Y - cutHeight);
        }
        else
        {
            // Point is within cut area, move to cut edge
            return new Point(point.X, cutY);
        }
    }

    private double ClampX(double x)
    {
        if (Image == null) return x;
        return Math.Max(0, Math.Min(x, Image.PixelSize.Width));
    }

    private double ClampY(double y)
    {
        if (Image == null) return y;
        return Math.Max(0, Math.Min(y, Image.PixelSize.Height));
    }
}
