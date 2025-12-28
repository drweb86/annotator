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
            // Check if clicking on a selected shape's beak (for callouts)
            if (_selectedShape is CalloutShape selectedCallout && selectedCallout.IsPointOnBeak(point))
            {
                _isDraggingBeak = true;
                return;
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
            // Tool is selected - create new shape
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

    protected override void OnPointerMoved(PointerEventArgs e)
    {
        base.OnPointerMoved(e);

        var point = e.GetPosition(this);

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
}
