using Avalonia;
using ScreenshotAnnotator.Interop.Shapes.Common;
using ScreenshotAnnotator.Interop.Shapes;
using ScreenshotAnnotator.Models;
using ScreenshotAnnotator.Resources;

namespace ScreenshotAnnotator.Shapes.CalloutNoArrow;

public sealed class CalloutNoArrowShapePlugin : IShapePlugin
{
    public string TypeId => "calloutnoarrow";
    public string DisplayName => Strings.Tool_Note;
    public ShapeToolbarIcon? ToolbarIcon { get; } = new()
    {
        PathData = "M4,4 L16,4 L16,12 L4,12 Z"
    };
    public int ToolbarOrder => 30;
    public ShapePropertyPanelKind PropertyPanelKind => ShapePropertyPanelKind.Text;

    public AnnotationShape CreateShape(ShapeCreationContext context) => new CalloutNoArrowShape
    {
        Rectangle = new Rect(context.StartPoint, new Size(0, 0)),
        StrokeColor = context.DefaultStrokeColor,
        StrokeThickness = context.DefaultStrokeThickness
    };

    public void UpdateWhileDrawing(AnnotationShape shape, ShapeDrawingContext context)
    {
        if (shape is CalloutNoArrowShape callout)
            callout.Rectangle = new Rect(context.StartPoint, context.CurrentPoint);
    }

    public bool IsValidForAdd(AnnotationShape shape) =>
        shape is CalloutNoArrowShape callout &&
        callout.Rectangle.Width > 10 &&
        callout.Rectangle.Height > 10;

    public bool TryGetHandleAtPoint(AnnotationShape shape, Point point, out ShapeHandleKind handle)
    {
        handle = ShapeHandleKind.None;
        if (shape is not CalloutNoArrowShape callout)
            return false;

        if (!callout.IsPointOnCornerHandle(point, out var corner))
            return false;

        handle = ToHandleKind(corner);
        return handle != ShapeHandleKind.None;
    }

    public void ApplyHandleDrag(AnnotationShape shape, ShapeHandleKind handle, Point point)
    {
        if (shape is not CalloutNoArrowShape callout)
            return;

        switch (handle)
        {
            case ShapeHandleKind.TopLeft:
                callout.ResizeFromCorner(RectCorner.TopLeft, point);
                break;
            case ShapeHandleKind.TopRight:
                callout.ResizeFromCorner(RectCorner.TopRight, point);
                break;
            case ShapeHandleKind.BottomLeft:
                callout.ResizeFromCorner(RectCorner.BottomLeft, point);
                break;
            case ShapeHandleKind.BottomRight:
                callout.ResizeFromCorner(RectCorner.BottomRight, point);
                break;
        }
    }

    public bool SupportsTextEditing(AnnotationShape shape) => shape is CalloutNoArrowShape;

    public bool TryGetTextBounds(AnnotationShape shape, out Rect bounds)
    {
        if (shape is CalloutNoArrowShape callout)
        {
            bounds = callout.TextBounds;
            return true;
        }

        bounds = default;
        return false;
    }

    public SerializableShape Serialize(AnnotationShape shape) =>
        SerializableCalloutNoArrowShape.FromShape((CalloutNoArrowShape)shape);

    public AnnotationShape Deserialize(SerializableShape serializable) =>
        ((SerializableCalloutNoArrowShape)serializable).ToShape();

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
