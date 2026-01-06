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
    private TrimRectangle? _currentTrimRect;
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

    public TrimRectangle? TrimRect
    {
        get => _currentTrimRect;
        set
        {
            _currentTrimRect = value;
            InvalidateVisual();
        }
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
        _selectedShape = null;
        InvalidateVisual();
    }

    private void ClearSelector()
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

    private void ShowTextEditorForCalloutNoArrow(CalloutNoArrowShape calloutNoArrow)
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
        _textEditor.SelectAll();
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
            Shapes.Remove(_selectedShape);
            _selectedShape = null;
            InvalidateVisual();
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

        // If a callout no arrow is selected and user presses a key, open text editor
        if (_selectedShape is CalloutNoArrowShape calloutNoArrow && !e.Handled)
        {
            // Don't trigger on special keys
            if (e.Key != Key.Escape && e.Key != Key.Tab &&
                e.Key != Key.LeftCtrl && e.Key != Key.RightCtrl &&
                e.Key != Key.LeftAlt && e.Key != Key.RightAlt &&
                e.Key != Key.LeftShift && e.Key != Key.RightShift &&
                e.Key != Key.Delete)
            {
                ShowTextEditorForCalloutNoArrow(calloutNoArrow);
                e.Handled = true;
            }
        }
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
}
