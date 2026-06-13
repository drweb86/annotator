using Avalonia;
using ScreenshotAnnotator.Interop.Shapes.Common;
using ScreenshotAnnotator.Interop.Shapes;
using ScreenshotAnnotator.Models;
using ScreenshotAnnotator.Resources;

namespace ScreenshotAnnotator.Shapes.Highlighter;

public sealed class HighlighterShapePlugin : IShapePlugin
{
    public string TypeId => "highlighter";
    public string DisplayName => Strings.Tool_Highlight;
    public ShapeToolbarIcon? ToolbarIcon { get; } = new()
    {
        PathData = "M2,2 L18,2 L18,18 L2,18 Z",
        FillColorArgb = 0x80FFFF00,
        StrokeColorArgb = 0,
        StrokeThickness = 0
    };
    public int ToolbarOrder => 50;
    public ShapePropertyPanelKind PropertyPanelKind => ShapePropertyPanelKind.HighlighterColor;

    public AnnotationShape CreateShape(ShapeCreationContext context) => new HighlighterShape
    {
        StartPoint = context.StartPoint,
        EndPoint = context.StartPoint,
        FillColor = context.DefaultHighlighterFillColor,
        StrokeColor = context.DefaultStrokeColor,
        StrokeThickness = context.DefaultStrokeThickness
    };

    public void UpdateWhileDrawing(AnnotationShape shape, ShapeDrawingContext context)
    {
        if (shape is HighlighterShape highlighter)
            highlighter.EndPoint = context.CurrentPoint;
    }

    public bool IsValidForAdd(AnnotationShape shape)
    {
        if (shape is not HighlighterShape highlighter)
            return false;

        var rect = new Rect(highlighter.StartPoint, highlighter.EndPoint);
        return rect.Width > 5 && rect.Height > 5;
    }

    public bool TryGetHandleAtPoint(AnnotationShape shape, Point point, out ShapeHandleKind handle)
    {
        handle = ShapeHandleKind.None;
        if (shape is not HighlighterShape highlighter)
            return false;

        var corner = highlighter.GetCornerAtPoint(point);
        if (!corner.HasValue)
            return false;

        handle = ToHandleKind(corner.Value);
        return handle != ShapeHandleKind.None;
    }

    public void ApplyHandleDrag(AnnotationShape shape, ShapeHandleKind handle, Point point)
    {
        if (shape is not HighlighterShape highlighter)
            return;

        var corner = handle switch
        {
            ShapeHandleKind.TopLeft => RectCorner.TopLeft,
            ShapeHandleKind.TopRight => RectCorner.TopRight,
            ShapeHandleKind.BottomLeft => RectCorner.BottomLeft,
            ShapeHandleKind.BottomRight => RectCorner.BottomRight,
            _ => RectCorner.None
        };

        if (corner != RectCorner.None)
            highlighter.ResizeCorner(corner, point);
    }

    public bool SupportsTextEditing(AnnotationShape shape) => false;

    public bool TryGetTextBounds(AnnotationShape shape, out Rect bounds)
    {
        bounds = default;
        return false;
    }

    public SerializableShape Serialize(AnnotationShape shape) =>
        SerializableHighlighterShape.FromShape((HighlighterShape)shape);

    public AnnotationShape Deserialize(SerializableShape serializable) =>
        ((SerializableHighlighterShape)serializable).ToShape();

    public void AfterShapeAdded(AnnotationShape shape, ShapeHostContext host) { }
    public void AfterShapeLoaded(AnnotationShape shape, ShapeHostContext host) { }

    public void AdjustForVerticalCut(AnnotationShape shape, double cutX, double cutWidth)
    {
        if (shape is not IHighlighterCornerShape highlighter)
            return;

        highlighter.StartPoint = ShapeCutHelpers.AdjustPointForVerticalCut(highlighter.StartPoint, cutX, cutWidth);
        highlighter.EndPoint = ShapeCutHelpers.AdjustPointForVerticalCut(highlighter.EndPoint, cutX, cutWidth);
    }

    public void AdjustForHorizontalCut(AnnotationShape shape, double cutY, double cutHeight)
    {
        if (shape is not IHighlighterCornerShape highlighter)
            return;

        highlighter.StartPoint = ShapeCutHelpers.AdjustPointForHorizontalCut(highlighter.StartPoint, cutY, cutHeight);
        highlighter.EndPoint = ShapeCutHelpers.AdjustPointForHorizontalCut(highlighter.EndPoint, cutY, cutHeight);
    }

    private static ShapeHandleKind ToHandleKind(RectCorner corner) => corner switch
    {
        RectCorner.TopLeft => ShapeHandleKind.TopLeft,
        RectCorner.TopRight => ShapeHandleKind.TopRight,
        RectCorner.BottomLeft => ShapeHandleKind.BottomLeft,
        RectCorner.BottomRight => ShapeHandleKind.BottomRight,
        _ => ShapeHandleKind.None
    };
}
