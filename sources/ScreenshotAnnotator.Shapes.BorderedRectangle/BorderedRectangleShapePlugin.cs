using Avalonia;
using ScreenshotAnnotator.Interop.Shapes.Common;
using ScreenshotAnnotator.Interop.Shapes;
using ScreenshotAnnotator.Models;
using ScreenshotAnnotator.Resources;

namespace ScreenshotAnnotator.Shapes.BorderedRectangle;

public sealed class BorderedRectangleShapePlugin : IShapePlugin
{
    public string TypeId => "borderedrectangle";
    public string DisplayName => Strings.Tool_Border;
    public ShapeToolbarIcon? ToolbarIcon { get; } = new()
    {
        PathData = "M2,2 L18,2 L18,18 L2,18 Z"
    };
    public int ToolbarOrder => 40;
    public ShapePropertyPanelKind PropertyPanelKind => ShapePropertyPanelKind.None;

    public AnnotationShape CreateShape(ShapeCreationContext context) => new BorderedRectangleShape
    {
        Rectangle = new Rect(context.StartPoint, new Size(0, 0)),
        StrokeColor = context.DefaultStrokeColor,
        StrokeThickness = context.DefaultStrokeThickness
    };

    public void UpdateWhileDrawing(AnnotationShape shape, ShapeDrawingContext context)
    {
        if (shape is BorderedRectangleShape bordered)
            bordered.Rectangle = new Rect(context.StartPoint, context.CurrentPoint);
    }

    public bool IsValidForAdd(AnnotationShape shape) =>
        shape is BorderedRectangleShape bordered &&
        bordered.Rectangle.Width > 10 &&
        bordered.Rectangle.Height > 10;

    public bool TryGetHandleAtPoint(AnnotationShape shape, Point point, out ShapeHandleKind handle)
    {
        handle = ShapeHandleKind.None;
        if (shape is not BorderedRectangleShape bordered)
            return false;

        if (!bordered.IsPointOnCornerHandle(point, out var corner))
            return false;

        handle = ToHandleKind(corner);
        return handle != ShapeHandleKind.None;
    }

    public void ApplyHandleDrag(AnnotationShape shape, ShapeHandleKind handle, Point point)
    {
        if (shape is not BorderedRectangleShape bordered)
            return;

        switch (handle)
        {
            case ShapeHandleKind.TopLeft:
                bordered.ResizeFromCorner(RectCorner.TopLeft, point);
                break;
            case ShapeHandleKind.TopRight:
                bordered.ResizeFromCorner(RectCorner.TopRight, point);
                break;
            case ShapeHandleKind.BottomLeft:
                bordered.ResizeFromCorner(RectCorner.BottomLeft, point);
                break;
            case ShapeHandleKind.BottomRight:
                bordered.ResizeFromCorner(RectCorner.BottomRight, point);
                break;
        }
    }

    public bool SupportsTextEditing(AnnotationShape shape) => false;

    public bool TryGetTextBounds(AnnotationShape shape, out Rect bounds)
    {
        bounds = default;
        return false;
    }

    public SerializableShape Serialize(AnnotationShape shape) =>
        SerializableBorderedRectangleShape.FromShape((BorderedRectangleShape)shape);

    public AnnotationShape Deserialize(SerializableShape serializable) =>
        ((SerializableBorderedRectangleShape)serializable).ToShape();

    public void AfterShapeAdded(AnnotationShape shape, ShapeHostContext host) { }
    public void AfterShapeLoaded(AnnotationShape shape, ShapeHostContext host) { }

    public void AdjustForVerticalCut(AnnotationShape shape, double cutX, double cutWidth)
    {
        if (shape is ICornerResizableShape resizable)
            resizable.Rectangle = ShapeCutHelpers.AdjustRectForVerticalCut(resizable.Rectangle, cutX, cutWidth);
    }

    public void AdjustForHorizontalCut(AnnotationShape shape, double cutY, double cutHeight)
    {
        if (shape is ICornerResizableShape resizable)
            resizable.Rectangle = ShapeCutHelpers.AdjustRectForHorizontalCut(resizable.Rectangle, cutY, cutHeight);
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
