using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Input.Platform;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.VisualTree;
using ScreenshotAnnotator.Interop.Shapes.Common;
using ScreenshotAnnotator.Interop.Shapes;
using ScreenshotAnnotator.Helpers;
using ScreenshotAnnotator.Models;
using ScreenshotAnnotator.Services;
using ScreenshotAnnotator.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;

namespace ScreenshotAnnotator.Controls;

public class ImageEditorCanvas : Control
{
    // Raised when the user finishes a cut drag; the cut is NOT performed yet.
    // Subscribers should play an animation then call ExecutePendingCut().
    // If nobody subscribes the cut is executed immediately (fallback).
    public event EventHandler<CutRequestedEventArgs>? CutRequested;

    /// <summary>Raised when the selector rectangle is created, completed after a drag, or cleared.</summary>
    public event EventHandler<SelectorRectChangedEventArgs>? SelectorRectChanged;

    private bool _pendingCutIsVertical;
    private double _pendingCutStart;
    private double _pendingCutEnd;
    private bool _cutPending;

    public void ExecutePendingCut()
    {
        if (!_cutPending) return;
        _cutPending = false;
        if (_pendingCutIsVertical)
            PerformVerticalCutOut(_pendingCutStart, _pendingCutEnd);
        else
            PerformHorizontalCutOut(_pendingCutStart, _pendingCutEnd);
        InvalidateVisual();
    }

    private Point _startPoint;
    private bool _isDrawing;
    private AnnotationShape? _currentShape;
    private SelectorRectangle? _currentSelectorRect;
    private AnnotationShape? _selectedShape;
    private bool _isDraggingShape;
    private Point _lastMousePosition;
    private bool _isDraggingHandle;
    private ShapeHandleKind _activeHandleDrag;
    private TextBox? _textEditor;
    private ITextEditableShape? _editingTextShape;
    private ObservableCollection<AnnotationShape>? _shapesSubscriptionTarget;

    public Canvas? OverlayCanvas { get; set; }

    public static readonly StyledProperty<Bitmap?> ImageProperty =
        AvaloniaProperty.Register<ImageEditorCanvas, Bitmap?>(nameof(Image));

    public static readonly StyledProperty<AppToolKind> AppToolProperty =
        AvaloniaProperty.Register<ImageEditorCanvas, AppToolKind>(nameof(AppTool), AppToolKind.Select);

    public static readonly StyledProperty<string?> ActiveShapeToolIdProperty =
        AvaloniaProperty.Register<ImageEditorCanvas, string?>(nameof(ActiveShapeToolId));

    public static readonly StyledProperty<ObservableCollection<AnnotationShape>> ShapesProperty =
        AvaloniaProperty.Register<ImageEditorCanvas, ObservableCollection<AnnotationShape>>(
            nameof(Shapes), new ObservableCollection<AnnotationShape>());

    private ImageEditorViewModel? _imageEditorViewModel;
    public void SetViewModel(ImageEditorViewModel model)
    {
        _imageEditorViewModel = model;
    }

    public Bitmap? Image
    {
        get => GetValue(ImageProperty);
        set => SetValue(ImageProperty, value);
    }

    public AppToolKind AppTool
    {
        get => GetValue(AppToolProperty);
        set => SetValue(AppToolProperty, value);
    }

    public string? ActiveShapeToolId
    {
        get => GetValue(ActiveShapeToolIdProperty);
        set => SetValue(ActiveShapeToolIdProperty, value);
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

    private bool IsSelectMode => AppTool == AppToolKind.Select && ActiveShapeToolId == null;

    /// <summary>Clears the current shape selection (e.g. before export/save preview).</summary>
    public void ClearSelection()
    {
        SetSelectedShape(null);
    }

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
        AffectsMeasure<ImageEditorCanvas>(ImageProperty);
        AppToolProperty.Changed.AddClassHandler<ImageEditorCanvas>((x, e) => x.OnAppToolChanged(e));
        ActiveShapeToolIdProperty.Changed.AddClassHandler<ImageEditorCanvas>((x, e) => x.OnActiveShapeToolIdChanged(e));
        ImageProperty.Changed.AddClassHandler<ImageEditorCanvas>((x, e) => x.OnImagePropertyChanged(e));
        ShapesProperty.Changed.AddClassHandler<ImageEditorCanvas>((x, e) => x.OnShapesPropertyChanged(e));
    }

    public ImageEditorCanvas()
    {
        DoubleTapped += OnDoubleTapped;
        Focusable = true; // Allow the canvas to receive keyboard input
    }

    private void OnImagePropertyChanged(AvaloniaPropertyChangedEventArgs e)
    {
        // New or removed image (import, delete, screenshot): drop selector rect; it is tied to the previous bitmap.
        ClearSelector();
    }

    private void OnShapesPropertyChanged(AvaloniaPropertyChangedEventArgs e)
    {
        ResubscribeShapesCollection();
    }

    private void ResubscribeShapesCollection()
    {
        var newCollection = Shapes;
        if (ReferenceEquals(_shapesSubscriptionTarget, newCollection))
            return;
        if (_shapesSubscriptionTarget is not null)
            _shapesSubscriptionTarget.CollectionChanged -= OnShapesCollectionChanged;
        _shapesSubscriptionTarget = newCollection;
        if (_shapesSubscriptionTarget is not null)
            _shapesSubscriptionTarget.CollectionChanged += OnShapesCollectionChanged;
    }

    private void OnShapesCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        // Clear selector when shapes change (e.g., project loaded). Subscribed to the bound collection, not the property default.
        ClearSelector();
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        ResubscribeShapesCollection();
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        if (_shapesSubscriptionTarget is not null)
        {
            _shapesSubscriptionTarget.CollectionChanged -= OnShapesCollectionChanged;
            _shapesSubscriptionTarget = null;
        }
        base.OnDetachedFromVisualTree(e);
    }

    private void OnAppToolChanged(AvaloniaPropertyChangedEventArgs e)
    {
        // Clear selector rectangle when switching away from selector tool
        if (e.OldValue is AppToolKind oldTool && oldTool == AppToolKind.Selector)
        {
            ClearSelector();
        }

        // Also clear selected shape when switching tools
        SetSelectedShape(null);
    }

    private void OnActiveShapeToolIdChanged(AvaloniaPropertyChangedEventArgs e)
    {
        SetSelectedShape(null);
    }

    public void ClearSelector()
    {
        _currentSelectorRect = null;
        InvalidateVisual();
        SelectorRectChanged?.Invoke(this, new SelectorRectChangedEventArgs(null));
    }

    /// <summary>White-outs the area covered by the current selector rectangle, then clears the selector.</summary>
    public void WhiteOutCurrentSelectorArea()
    {
        if (_currentSelectorRect == null) return;
        WhiteOutImageArea(_currentSelectorRect.Rectangle);
        ClearSelector();
    }

    private void OnDoubleTapped(object? sender, TappedEventArgs e)
    {
        var point = e.GetPosition(this);

        if (_selectedShape == null)
            return;

        var plugin = ShapeRegistry.GetForAnnotationShape(_selectedShape);
        if (plugin?.SupportsTextEditing(_selectedShape) == true
            && _selectedShape is ITextEditableShape textShape
            && _selectedShape.HitTest(point))
        {
            ShowTextEditor(textShape);
            e.Handled = true;
        }
    }

    private void ShowTextEditor(ITextEditableShape shape, string? initialText = null)
    {
        if (OverlayCanvas == null) return;

        HideTextEditor();

        _editingTextShape = shape;
        var bounds = shape.TextBounds;

        _textEditor = new TextBox
        {
            Text = shape.Text,
            Width = Math.Max(bounds.Width, 100),
            Height = Math.Max(bounds.Height, 50),
            TextWrapping = Avalonia.Media.TextWrapping.Wrap,
            AcceptsReturn = true,
            BorderThickness = new Thickness(2),
            BorderBrush = new SolidColorBrush(Color.FromRgb(255, 165, 0)), // Orange border
            Background = new SolidColorBrush(Color.FromRgb(40, 40, 40)), // Dark gray background
            Padding = new Thickness(10),
            FontFamily = new Avalonia.Media.FontFamily(shape.FontFamily),
            FontSize = shape.FontSize,
            FontWeight = shape.FontBold ? FontWeight.Bold : FontWeight.Normal,
            FontStyle = shape.FontItalic ? FontStyle.Italic : FontStyle.Normal,
            TextAlignment = TextAlignment.Center,
            VerticalContentAlignment = Avalonia.Layout.VerticalAlignment.Center
        };

        Canvas.SetLeft(_textEditor, bounds.Left + 5);
        Canvas.SetTop(_textEditor, bounds.Top + 5);

        _textEditor.LostFocus += (s, e) => OnTextEditingComplete();
        _textEditor.KeyDown += (s, e) =>
        {
            if (e.Key == Key.Escape)
            {
                HideTextEditor();
                e.Handled = true;
            }
        };

        OverlayCanvas.Children.Add(_textEditor);

        var scrollViewer = this.FindAncestorOfType<ScrollViewer>();
        var savedOffset = scrollViewer?.Offset;

        _textEditor.Focus();

        if (scrollViewer != null && savedOffset.HasValue)
            scrollViewer.Offset = savedOffset.Value;

        if (!string.IsNullOrEmpty(initialText))
        {
            _textEditor.Text = shape.Text + initialText;
            _textEditor.CaretIndex = _textEditor.Text.Length;
        }
        else
        {
            _textEditor.SelectAll();
        }
    }

    private void OnTextEditingComplete()
    {
        if (_textEditor != null && _editingTextShape != null)
        {
            _editingTextShape.Text = _textEditor.Text ?? "";
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
            _editingTextShape = null;
        }
    }

    private ShapeCreationContext CreateShapeCreationContext(Point startPoint) => new()
    {
        StartPoint = startPoint,
        DefaultHighlighterFillColor = _imageEditorViewModel?.SelectedHighlighterColor
            ?? Color.FromArgb(100, 255, 255, 0)
    };

    private ShapeHostContext CreateShapeHostContext() => new()
    {
        CreateBlurredImage = CreateBlurredImage
    };

    private ShapeDrawingContext CreateShapeDrawingContext(Point currentPoint) => new()
    {
        StartPoint = _startPoint,
        CurrentPoint = currentPoint,
        SourceImage = Image,
        CreateBlurredImage = CreateBlurredImage,
        IsDraggingBeak = false
    };

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);

        var point = e.GetPosition(this);
        _startPoint = point;
        _lastMousePosition = point;

        if (IsSelectMode)
        {
            if (_selectedShape != null)
            {
                var plugin = ShapeRegistry.GetForAnnotationShape(_selectedShape);
                if (plugin?.TryGetHandleAtPoint(_selectedShape, point, out var handle) == true)
                {
                    _isDraggingHandle = true;
                    _activeHandleDrag = handle;
                    return;
                }
            }

            for (int i = Shapes.Count - 1; i >= 0; i--)
            {
                if (Shapes[i].HitTest(point))
                {
                    SetSelectedShape(Shapes[i]);
                    _isDraggingShape = true;
                    Focus();
                    return;
                }
            }

            SetSelectedShape(null);
            return;
        }

        if (!string.IsNullOrEmpty(ActiveShapeToolId))
        {
            for (int i = Shapes.Count - 1; i >= 0; i--)
            {
                if (Shapes[i].HitTest(point))
                {
                    AppTool = AppToolKind.Select;
                    ActiveShapeToolId = null;
                    SetSelectedShape(Shapes[i]);
                    _isDraggingShape = true;
                    Focus();
                    InvalidateVisual();
                    return;
                }
            }

            _isDrawing = true;
            var shapePlugin = ShapeRegistry.GetRequired(ActiveShapeToolId);
            _currentShape = shapePlugin.CreateShape(CreateShapeCreationContext(point));
            InvalidateVisual();
            return;
        }

        _isDrawing = true;

        if (AppTool == AppToolKind.Selector)
        {
            _currentSelectorRect = new SelectorRectangle
            {
                Rectangle = new Rect(point, new Size(0, 0))
            };
        }

        InvalidateVisual();
    }

    protected override void OnPointerMoved(PointerEventArgs e)
    {
        base.OnPointerMoved(e);

        var point = e.GetPosition(this);

        if (_isDraggingHandle && _selectedShape != null)
        {
            var plugin = ShapeRegistry.GetForAnnotationShape(_selectedShape);
            plugin?.ApplyHandleDrag(_selectedShape, _activeHandleDrag, point);
            InvalidateVisual();
            return;
        }

        if (_isDraggingShape && _selectedShape != null)
        {
            var offset = new Vector(point.X - _lastMousePosition.X, point.Y - _lastMousePosition.Y);
            _selectedShape.Move(offset);
            _lastMousePosition = point;
            InvalidateVisual();
            return;
        }

        _lastMousePosition = point;

        if (!_isDrawing) return;

        if (!string.IsNullOrEmpty(ActiveShapeToolId) && _currentShape != null)
        {
            var plugin = ShapeRegistry.GetRequired(ActiveShapeToolId);
            plugin.UpdateWhileDrawing(_currentShape, CreateShapeDrawingContext(point));
            InvalidateVisual();
            return;
        }

        if (AppTool == AppToolKind.Selector && _currentSelectorRect != null)
        {
            _currentSelectorRect.Rectangle = new Rect(_startPoint, point);
            InvalidateVisual();
            return;
        }

        if (AppTool is AppToolKind.VerticalCutOut or AppToolKind.HorizontalCutOut)
        {
            InvalidateVisual();
        }
    }

    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        base.OnPointerReleased(e);

        if (_isDraggingHandle)
        {
            _isDraggingHandle = false;
            _activeHandleDrag = ShapeHandleKind.None;
            return;
        }

        if (_isDraggingShape)
        {
            _isDraggingShape = false;
            return;
        }

        if (!_isDrawing) return;

        _isDrawing = false;

        var point = e.GetPosition(this);

        if (AppTool == AppToolKind.VerticalCutOut)
        {
            var start = ClampX(_startPoint.X);
            var end = ClampX(point.X);
            if (Math.Abs(end - start) >= 5)
            {
                _pendingCutIsVertical = true;
                _pendingCutStart = start;
                _pendingCutEnd = end;
                _cutPending = true;
                var args = new CutRequestedEventArgs(true, start, end);
                if (CutRequested != null)
                    CutRequested(this, args);
                else
                    ExecutePendingCut();
            }
            InvalidateVisual();
            return;
        }

        if (AppTool == AppToolKind.HorizontalCutOut)
        {
            var start = ClampY(_startPoint.Y);
            var end = ClampY(point.Y);
            if (Math.Abs(end - start) >= 5)
            {
                _pendingCutIsVertical = false;
                _pendingCutStart = start;
                _pendingCutEnd = end;
                _cutPending = true;
                var args = new CutRequestedEventArgs(false, start, end);
                if (CutRequested != null)
                    CutRequested(this, args);
                else
                    ExecutePendingCut();
            }
            InvalidateVisual();
            return;
        }

        if (AppTool == AppToolKind.Selector && _currentSelectorRect != null)
        {
            var r = _currentSelectorRect.Rectangle;
            if (r.Width > 5 && r.Height > 5)
                SelectorRectChanged?.Invoke(this, new SelectorRectChangedEventArgs(_currentSelectorRect));
            else
                ClearSelector();
        }

        if (!string.IsNullOrEmpty(ActiveShapeToolId) && _currentShape != null)
        {
            var plugin = ShapeRegistry.GetRequired(ActiveShapeToolId);
            if (plugin.IsValidForAdd(_currentShape))
            {
                Shapes.Add(_currentShape);
                plugin.AfterShapeAdded(_currentShape, CreateShapeHostContext());
            }

            _currentShape = null;
        }

        InvalidateVisual();
    }

    public override void Render(DrawingContext context)
    {
        base.Render(context);

        if (Image != null)
        {
            var imageRect = new Rect(0, 0, Image.PixelSize.Width, Image.PixelSize.Height);
            context.DrawImage(Image, imageRect);
        }

        foreach (var shape in Shapes)
        {
            shape.Render(context);
        }

        _currentShape?.Render(context);
        _currentSelectorRect?.Render(context);

        if (_isDrawing && Image != null)
        {
            if (AppTool == AppToolKind.VerticalCutOut)
            {
                var clampedStartX = ClampX(_startPoint.X);
                var clampedEndX = ClampX(_lastMousePosition.X);
                var leftX = Math.Min(clampedStartX, clampedEndX);
                var rightX = Math.Max(clampedStartX, clampedEndX);
                var height = Image.PixelSize.Height;

                var cutBrush = new SolidColorBrush(Color.FromArgb(100, 255, 0, 0));
                var cutPen = new Pen(new SolidColorBrush(Colors.Red), 2.0);
                cutPen.DashStyle = new DashStyle(new[] { 4.0, 4.0 }, 0);

                var cutRect = new Rect(leftX, 0, rightX - leftX, height);
                context.DrawRectangle(cutBrush, cutPen, cutRect);
            }
            else if (AppTool == AppToolKind.HorizontalCutOut)
            {
                var clampedStartY = ClampY(_startPoint.Y);
                var clampedEndY = ClampY(_lastMousePosition.Y);
                var topY = Math.Min(clampedStartY, clampedEndY);
                var bottomY = Math.Max(clampedStartY, clampedEndY);
                var width = Image.PixelSize.Width;

                var cutBrush = new SolidColorBrush(Color.FromArgb(100, 255, 0, 0));
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

        var topLevel = TopLevel.GetTopLevel(this);

        if (topLevel is null ||
            _imageEditorViewModel is null)
            return;

        if (_textEditor is not null)
            return;

        if (e.Key == Key.A && e.KeyModifiers.HasFlag(KeyModifiers.Control) && Image != null)
        {
            if (AppTool != AppToolKind.Selector)
            {
                AppTool = AppToolKind.Selector;
            }

            _currentSelectorRect = new SelectorRectangle
            {
                Rectangle = new Rect(0, 0, Image.PixelSize.Width, Image.PixelSize.Height)
            };
            InvalidateVisual();
            SelectorRectChanged?.Invoke(this, new SelectorRectChangedEventArgs(_currentSelectorRect));
            e.Handled = true;
            return;
        }

        if (e.Key == Key.Delete && _selectedShape != null)
        {
            var toRemove = _selectedShape;
            SetSelectedShape(null);
            Shapes.Remove(toRemove);
            e.Handled = true;
            return;
        }

        if (e.Key == Key.Delete && _currentSelectorRect != null && AppTool == AppToolKind.Selector)
        {
            WhiteOutImageArea(_currentSelectorRect.Rectangle);
            _currentSelectorRect = null;
            InvalidateVisual();
            e.Handled = true;
            return;
        }

        if (e.Key == Key.C && e.KeyModifiers.HasFlag(KeyModifiers.Control))
        {
            _imageEditorViewModel.CopyToClipboardCommand.Execute(null);
            e.Handled = true;
            return;
        }

        if (e.Key == Key.V && e.KeyModifiers.HasFlag(KeyModifiers.Control))
        {
            this._imageEditorViewModel.ClipboardService.Paste(_imageEditorViewModel, topLevel.Clipboard);
            e.Handled = true;
            return;
        }

        if (_selectedShape != null && !e.Handled)
        {
            var plugin = ShapeRegistry.GetForAnnotationShape(_selectedShape);
            if (plugin?.SupportsTextEditing(_selectedShape) == true
                && _selectedShape is ITextEditableShape textShape)
            {
                if (e.Key != Key.Escape && e.Key != Key.Tab &&
                    e.Key != Key.LeftCtrl && e.Key != Key.RightCtrl &&
                    e.Key != Key.LeftAlt && e.Key != Key.RightAlt &&
                    e.Key != Key.LeftShift && e.Key != Key.RightShift &&
                    e.Key != Key.Delete && e.Key != Key.Back)
                {
                    string? initialChar = GetCharFromKey(e.Key, e.KeyModifiers.HasFlag(KeyModifiers.Shift));
                    ShowTextEditor(textShape, initialChar);
                    e.Handled = true;
                }
            }
        }
    }

    private string? GetCharFromKey(Key key, bool shiftPressed)
    {
        if (key >= Key.A && key <= Key.Z)
        {
            char c = (char)('a' + (key - Key.A));
            if (shiftPressed)
                c = char.ToUpper(c);
            return c.ToString();
        }

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

        if (key >= Key.NumPad0 && key <= Key.NumPad9)
        {
            return ((char)('0' + (key - Key.NumPad0))).ToString();
        }

        if (key == Key.Space)
            return " ";

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

    public Bitmap? CreateBlurredImagePublic(Rect rect)
    {
        return CreateBlurredImage(rect);
    }

    private Bitmap? CreateBlurredImage(Rect rect)
    {
        if (Image == null) return null;

        try
        {
            var x = (int)Math.Max(0, rect.X);
            var y = (int)Math.Max(0, rect.Y);
            var width = (int)Math.Min(rect.Width, Image.PixelSize.Width - x);
            var height = (int)Math.Min(rect.Height, Image.PixelSize.Height - y);

            if (width <= 0 || height <= 0) return null;

            var tempBitmap = new RenderTargetBitmap(new PixelSize(width, height));

            using (var context = tempBitmap.CreateDrawingContext())
            {
                var sourceRect = new Rect(x, y, width, height);
                var destRect = new Rect(0, 0, width, height);
                context.DrawImage(Image, sourceRect, destRect);
            }

            return ApplyBoxBlur(tempBitmap, 15);
        }
        catch
        {
            return null;
        }
    }

    private Bitmap ApplyBoxBlur(RenderTargetBitmap source, int radius)
    {
        var width = source.PixelSize.Width;
        var height = source.PixelSize.Height;

        var downscaleFactor = Math.Max(1, radius / 3);
        var smallWidth = Math.Max(1, width / downscaleFactor);
        var smallHeight = Math.Max(1, height / downscaleFactor);

        var downsampled = new RenderTargetBitmap(new PixelSize(smallWidth, smallHeight));
        using (var context = downsampled.CreateDrawingContext())
        {
            context.DrawImage(source,
                new Rect(0, 0, width, height),
                new Rect(0, 0, smallWidth, smallHeight));
        }

        var blurred = new RenderTargetBitmap(new PixelSize(width, height));
        using (var context = blurred.CreateDrawingContext())
        {
            context.DrawImage(downsampled,
                new Rect(0, 0, smallWidth, smallHeight),
                new Rect(0, 0, width, height));
        }

        using var stream = new MemoryStream();
        blurred.SavePng(stream);
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

            var newImage = new RenderTargetBitmap(new PixelSize(width, height));

            using (var context = newImage.CreateDrawingContext())
            {
                context.DrawImage(Image, new Rect(0, 0, width, height));

                var whiteBrush = new SolidColorBrush(Colors.White);
                context.DrawRectangle(whiteBrush, null, rect);
            }

            using var stream = new MemoryStream();
            newImage.SavePng(stream);
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

            if (cutWidth < 5) return;

            var oldWidth = Image.PixelSize.Width;
            var oldHeight = Image.PixelSize.Height;
            var newWidth = oldWidth - (int)cutWidth;

            if (newWidth <= 0) return;

            var newImage = new RenderTargetBitmap(new PixelSize(newWidth, oldHeight), new Vector(96, 96));

            using (var context = newImage.CreateDrawingContext())
            {
                if (leftX > 0)
                {
                    var sourceRect = new Rect(0, 0, leftX, oldHeight);
                    var destRect = new Rect(0, 0, leftX, oldHeight);
                    context.DrawImage(Image, sourceRect, destRect);
                }

                if (rightX < oldWidth)
                {
                    var sourceRect = new Rect(rightX, 0, oldWidth - rightX, oldHeight);
                    var destRect = new Rect(leftX, 0, oldWidth - rightX, oldHeight);
                    context.DrawImage(Image, sourceRect, destRect);
                }
            }

            foreach (var shape in Shapes.ToList())
            {
                AdjustShapeForVerticalCut(shape, leftX, cutWidth);
            }

            using var stream = new MemoryStream();
            newImage.SavePng(stream);
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

            if (cutHeight < 5) return;

            var oldWidth = Image.PixelSize.Width;
            var oldHeight = Image.PixelSize.Height;
            var newHeight = oldHeight - (int)cutHeight;

            if (newHeight <= 0) return;

            var newImage = new RenderTargetBitmap(new PixelSize(oldWidth, newHeight), new Vector(96, 96));

            using (var context = newImage.CreateDrawingContext())
            {
                if (topY > 0)
                {
                    var sourceRect = new Rect(0, 0, oldWidth, topY);
                    var destRect = new Rect(0, 0, oldWidth, topY);
                    context.DrawImage(Image, sourceRect, destRect);
                }

                if (bottomY < oldHeight)
                {
                    var sourceRect = new Rect(0, bottomY, oldWidth, oldHeight - bottomY);
                    var destRect = new Rect(0, topY, oldWidth, oldHeight - bottomY);
                    context.DrawImage(Image, sourceRect, destRect);
                }
            }

            foreach (var shape in Shapes.ToList())
            {
                AdjustShapeForHorizontalCut(shape, topY, cutHeight);
            }

            using var stream = new MemoryStream();
            newImage.SavePng(stream);
            stream.Position = 0;
            Image = new Bitmap(stream);
        }
        catch
        {
            // Silently handle errors
        }
    }

    private static void AdjustShapeForVerticalCut(object shape, double cutX, double cutWidth)
    {
        if (shape is SelectorRectangle selector)
        {
            selector.Rectangle = ShapeCutHelpers.AdjustRectForVerticalCut(selector.Rectangle, cutX, cutWidth);
            return;
        }

        if (shape is AnnotationShape annotationShape)
        {
            ShapeRegistry.GetForAnnotationShape(annotationShape)
                ?.AdjustForVerticalCut(annotationShape, cutX, cutWidth);
        }
    }

    private static void AdjustShapeForHorizontalCut(object shape, double cutY, double cutHeight)
    {
        if (shape is SelectorRectangle selector)
        {
            selector.Rectangle = ShapeCutHelpers.AdjustRectForHorizontalCut(selector.Rectangle, cutY, cutHeight);
            return;
        }

        if (shape is AnnotationShape annotationShape)
        {
            ShapeRegistry.GetForAnnotationShape(annotationShape)
                ?.AdjustForHorizontalCut(annotationShape, cutY, cutHeight);
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

public sealed class CutRequestedEventArgs(bool isVertical, double start, double end) : EventArgs
{
    public bool   IsVertical { get; } = isVertical;
    public double Start      { get; } = start;
    public double End        { get; } = end;
}

public sealed class SelectorRectChangedEventArgs(SelectorRectangle? rect) : EventArgs
{
    public SelectorRectangle? Rect { get; } = rect;
}
