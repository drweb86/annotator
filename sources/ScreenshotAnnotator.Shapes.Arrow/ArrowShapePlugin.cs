using Avalonia;
using ScreenshotAnnotator.Interop.Shapes.Common;
using ScreenshotAnnotator.Interop.Shapes;
using ScreenshotAnnotator.Models;
using ScreenshotAnnotator.Resources;
using System;

namespace ScreenshotAnnotator.Shapes.Arrow;

public sealed class ArrowShapePlugin : IShapePlugin
{
    public string TypeId => "arrow";
    public string DisplayName => Strings.Tool_Arrow;
    public ShapeToolbarIcon? ToolbarIcon { get; } = new()
    {
        PathData = "M2,2 L18,10 L10,12 L8,20 Z M10,12 L18,18"
    };
    public int ToolbarOrder => 10;
    public ShapePropertyPanelKind PropertyPanelKind => ShapePropertyPanelKind.ArrowColor;

    public AnnotationShape CreateShape(ShapeCreationContext context) => new ArrowShape
    {
        StartPoint = context.StartPoint,
        EndPoint = context.StartPoint,
        StrokeColor = context.DefaultStrokeColor,
        StrokeThickness = context.DefaultStrokeThickness
    };

    public void UpdateWhileDrawing(AnnotationShape shape, ShapeDrawingContext context)
    {
        if (shape is ArrowShape arrow)
            arrow.EndPoint = context.CurrentPoint;
    }

    public bool IsValidForAdd(AnnotationShape shape)
    {
        if (shape is not ArrowShape arrow)
            return false;

        var distance = Math.Sqrt(
            Math.Pow(arrow.EndPoint.X - arrow.StartPoint.X, 2) +
            Math.Pow(arrow.EndPoint.Y - arrow.StartPoint.Y, 2));
        return distance > 5;
    }

    public bool TryGetHandleAtPoint(AnnotationShape shape, Point point, out ShapeHandleKind handle)
    {
        handle = ShapeHandleKind.None;
        if (shape is not ArrowShape arrow)
            return false;

        if (arrow.IsPointOnStartHandle(point))
        {
            handle = ShapeHandleKind.Start;
            return true;
        }

        if (arrow.IsPointOnEndHandle(point))
        {
            handle = ShapeHandleKind.End;
            return true;
        }

        return false;
    }

    public void ApplyHandleDrag(AnnotationShape shape, ShapeHandleKind handle, Point point)
    {
        if (shape is not ArrowShape arrow)
            return;

        switch (handle)
        {
            case ShapeHandleKind.Start:
                arrow.MoveStartPoint(point);
                break;
            case ShapeHandleKind.End:
                arrow.MoveEndPoint(point);
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
        SerializableArrowShape.FromShape((ArrowShape)shape);

    public AnnotationShape Deserialize(SerializableShape serializable) =>
        ((SerializableArrowShape)serializable).ToShape();

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
}
