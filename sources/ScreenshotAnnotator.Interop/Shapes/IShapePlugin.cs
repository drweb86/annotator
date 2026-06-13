using Avalonia;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using ScreenshotAnnotator.Models;
using System;

namespace ScreenshotAnnotator.Interop.Shapes;

public sealed class ShapeCreationContext
{
    public required Point StartPoint { get; init; }
    public Color DefaultStrokeColor { get; init; } = Colors.Red;
    public double DefaultStrokeThickness { get; init; } = 10.0;
    public Color DefaultHighlighterFillColor { get; init; } = Color.FromArgb(100, 255, 255, 0);
}

public sealed class ShapeDrawingContext
{
    public required Point StartPoint { get; init; }
    public required Point CurrentPoint { get; init; }
    public Bitmap? SourceImage { get; init; }
    public Func<Rect, Bitmap?>? CreateBlurredImage { get; init; }
    public bool IsDraggingBeak { get; init; }
}

public sealed class ShapeHostContext
{
    public Func<Rect, Bitmap?>? CreateBlurredImage { get; init; }
}

public enum ShapePropertyPanelKind
{
    None,
    Text,
    ArrowColor,
    HighlighterColor
}

public interface IShapePlugin
{
    string TypeId { get; }
    string DisplayName { get; }
    ShapeToolbarIcon? ToolbarIcon { get; }
    int ToolbarOrder { get; }
    ShapePropertyPanelKind PropertyPanelKind { get; }

    AnnotationShape CreateShape(ShapeCreationContext context);
    void UpdateWhileDrawing(AnnotationShape shape, ShapeDrawingContext context);
    bool IsValidForAdd(AnnotationShape shape);

    bool TryGetHandleAtPoint(AnnotationShape shape, Point point, out ShapeHandleKind handle);
    void ApplyHandleDrag(AnnotationShape shape, ShapeHandleKind handle, Point point);

    bool SupportsTextEditing(AnnotationShape shape);
    bool TryGetTextBounds(AnnotationShape shape, out Rect bounds);

    SerializableShape Serialize(AnnotationShape shape);
    AnnotationShape Deserialize(SerializableShape serializable);

    void AfterShapeAdded(AnnotationShape shape, ShapeHostContext host);
    void AfterShapeLoaded(AnnotationShape shape, ShapeHostContext host);

    void AdjustForVerticalCut(AnnotationShape shape, double cutX, double cutWidth);
    void AdjustForHorizontalCut(AnnotationShape shape, double cutY, double cutHeight);
}
