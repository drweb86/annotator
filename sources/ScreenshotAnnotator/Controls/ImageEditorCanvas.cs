using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using ScreenshotAnnotator.Models;
using System;
using System.Collections.ObjectModel;

namespace ScreenshotAnnotator.Controls;

public class ImageEditorCanvas : Control
{
    private Point _startPoint;
    private bool _isDrawing;
    private AnnotationShape? _currentShape;
    private TrimRectangle? _currentTrimRect;
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

    public TrimRectangle? TrimRect
    {
        get => _currentTrimRect;
        set
        {
            _currentTrimRect = value;
            InvalidateVisual();
        }
    }

    static ImageEditorCanvas()
    {
        AffectsRender<ImageEditorCanvas>(ImageProperty, ShapesProperty);
    }

    public ImageEditorCanvas()
    {
        Shapes.CollectionChanged += (s, e) => InvalidateVisual();
        DoubleTapped += OnDoubleTapped;
        Focusable = true; // Allow the canvas to receive keyboard input
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
    }

    private void ShowTextEditor(CalloutShape callout)
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
            BorderThickness = new Thickness(0),
            BorderBrush = Brushes.Transparent,
            Background = Brushes.Transparent,
            Foreground = Brushes.White,
            Padding = new Thickness(10),
            FontSize = 24,
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
        _textEditor.SelectAll();
    }

    private void OnTextEditingComplete()
    {
        if (_textEditor != null && _editingCallout != null)
        {
            _editingCallout.Text = _textEditor.Text ?? "";
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

            // Check if clicking on any existing shape (reverse order to select top-most)
            for (int i = Shapes.Count - 1; i >= 0; i--)
            {
                if (Shapes[i].HitTest(point))
                {
                    // Deselect previous shape
                    if (_selectedShape != null)
                        _selectedShape.IsSelected = false;

                    // Select this shape
                    _selectedShape = Shapes[i];
                    _selectedShape.IsSelected = true;
                    _isDraggingShape = true;
                    Focus(); // Set focus to receive keyboard events
                    InvalidateVisual();
                    return;
                }
            }

            // Click on empty space - deselect
            if (_selectedShape != null)
            {
                _selectedShape.IsSelected = false;
                _selectedShape = null;
                InvalidateVisual();
            }
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

                    // Deselect previous shape
                    if (_selectedShape != null)
                        _selectedShape.IsSelected = false;

                    // Select this shape
                    _selectedShape = Shapes[i];
                    _selectedShape.IsSelected = true;
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

                    case ToolType.Trim:
                        _currentTrimRect = new TrimRectangle
                        {
                            Rectangle = new Rect(point, new Size(0, 0))
                        };
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

            case ToolType.Trim:
                if (_currentTrimRect != null)
                {
                    _currentTrimRect.Rectangle = new Rect(_startPoint, point);
                    InvalidateVisual();
                }
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

        // Add the completed shape to the collection
        if (_currentShape != null && CurrentTool != ToolType.Trim)
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

        // Draw trim rectangle
        _currentTrimRect?.Render(context);

        // Draw overlay for area outside trim rectangle
        if (_currentTrimRect != null && Image != null)
        {
            var fullRect = new Rect(0, 0, Image.PixelSize.Width, Image.PixelSize.Height);
            var trimRect = _currentTrimRect.Rectangle;

            var darkBrush = new SolidColorBrush(Colors.Black) { Opacity = 0.5 };

            // Top
            if (trimRect.Top > 0)
            {
                context.DrawRectangle(darkBrush, null,
                    new Rect(0, 0, fullRect.Width, trimRect.Top));
            }

            // Bottom
            if (trimRect.Bottom < fullRect.Height)
            {
                context.DrawRectangle(darkBrush, null,
                    new Rect(0, trimRect.Bottom, fullRect.Width, fullRect.Height - trimRect.Bottom));
            }

            // Left
            context.DrawRectangle(darkBrush, null,
                new Rect(0, trimRect.Top, trimRect.Left, trimRect.Height));

            // Right
            if (trimRect.Right < fullRect.Width)
            {
                context.DrawRectangle(darkBrush, null,
                    new Rect(trimRect.Right, trimRect.Top, fullRect.Width - trimRect.Right, trimRect.Height));
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

        // Delete selected shape on Delete key press
        if (e.Key == Key.Delete && _selectedShape != null)
        {
            Shapes.Remove(_selectedShape);
            _selectedShape = null;
            InvalidateVisual();
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
                e.Key != Key.Delete)
            {
                ShowTextEditor(callout);
                e.Handled = true;
            }
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
}
