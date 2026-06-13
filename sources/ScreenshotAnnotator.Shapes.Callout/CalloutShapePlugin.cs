using Avalonia;
using ScreenshotAnnotator.Interop.Shapes;
using ScreenshotAnnotator.Models;
using ScreenshotAnnotator.Resources;

namespace ScreenshotAnnotator.Shapes.Callout;

public sealed class CalloutShapePlugin : IShapePlugin
{
    public string TypeId => "callout";
    public string DisplayName => Strings.Tool_Callout;
    public ShapeToolbarIcon? ToolbarIcon { get; } = new()
    {
        PathData = "M4,4 L16,4 L16,12 L10,12 L8,16 L8,12 L4,12 Z"
    };
    public int ToolbarOrder => 20;
    public ShapePropertyPanelKind PropertyPanelKind => ShapePropertyPanelKind.Text;

    public AnnotationShape CreateShape(ShapeCreationContext context) => new CalloutShape
    {
        Rectangle = new Rect(context.StartPoint, new Size(0, 0)),
        BeakPoint = context.StartPoint,
        StrokeColor = context.DefaultStrokeColor,
        StrokeThickness = context.DefaultStrokeThickness
    };

    public void UpdateWhileDrawing(AnnotationShape shape, ShapeDrawingContext context)
    {
        if (shape is not CalloutShape callout || context.IsDraggingBeak)
            return;

        var rect = new Rect(context.StartPoint, context.CurrentPoint);
        callout.Rectangle = rect;
        callout.BeakPoint = new Point(rect.Center.X, rect.Bottom + 30);
    }

    public bool IsValidForAdd(AnnotationShape shape) =>
        shape is CalloutShape callout &&
        callout.Rectangle.Width > 10 &&
        callout.Rectangle.Height > 10;

    public bool TryGetHandleAtPoint(AnnotationShape shape, Point point, out ShapeHandleKind handle)
    {
        handle = ShapeHandleKind.None;
        if (shape is not CalloutShape callout)
            return false;

        if (callout.IsPointOnBeak(point))
        {
            handle = ShapeHandleKind.Beak;
            return true;
        }

        if (callout.IsPointOnCornerHandle(point, out var corner))
        {
            handle = ToHandleKind(corner);
            return handle != ShapeHandleKind.None;
        }

        return false;
    }

    public void ApplyHandleDrag(AnnotationShape shape, ShapeHandleKind handle, Point point)
    {
        if (shape is not CalloutShape callout)
            return;

        switch (handle)
        {
            case ShapeHandleKind.Beak:
                callout.MoveBeak(point);
                break;
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

    public bool SupportsTextEditing(AnnotationShape shape) => shape is CalloutShape;

    public bool TryGetTextBounds(AnnotationShape shape, out Rect bounds)
    {
        if (shape is CalloutShape callout)
        {
            bounds = callout.TextBounds;
            return true;
        }

        bounds = default;
        return false;
    }

    public SerializableShape Serialize(AnnotationShape shape) =>
        SerializableCalloutShape.FromShape((CalloutShape)shape);

    public AnnotationShape Deserialize(SerializableShape serializable) =>
        ((SerializableCalloutShape)serializable).ToShape();

    public void AfterShapeAdded(AnnotationShape shape, ShapeHostContext host) { }
    public void AfterShapeLoaded(AnnotationShape shape, ShapeHostContext host) { }

    public void AdjustForVerticalCut(AnnotationShape shape, double cutX, double cutWidth)
    {
        if (shape is IVerticalCutAdjustable adjustable)
            adjustable.AdjustForVerticalCut(cutX, cutWidth);
    }

    public void AdjustForHorizontalCut(AnnotationShape shape, double cutY, double cutHeight)
    {
        if (shape is IHorizontalCutAdjustable adjustable)
            adjustable.AdjustForHorizontalCut(cutY, cutHeight);
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
