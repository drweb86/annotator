using Avalonia;
using ScreenshotAnnotator.Interop.Shapes.Common;
using ScreenshotAnnotator.Interop.Shapes;
using ScreenshotAnnotator.Models;
using ScreenshotAnnotator.Resources;

namespace ScreenshotAnnotator.Shapes.BlurRectangle;

public sealed class BlurRectangleShapePlugin : IShapePlugin
{
    public string TypeId => "blurrectangle";
    public string DisplayName => Strings.Tool_Blur;
    public ShapeToolbarIcon? ToolbarIcon { get; } = new()
    {
        PathData = "M2,2 L18,2 L18,18 L2,18 Z M5,5 L15,5 L15,15 L5,15 Z",
        FillColorArgb = 0x80808080
    };
    public int ToolbarOrder => 60;
    public ShapePropertyPanelKind PropertyPanelKind => ShapePropertyPanelKind.None;

    public AnnotationShape CreateShape(ShapeCreationContext context) => new BlurRectangleShape
    {
        Rectangle = new Rect(context.StartPoint, new Size(0, 0)),
        StrokeColor = context.DefaultStrokeColor,
        StrokeThickness = context.DefaultStrokeThickness
    };

    public void UpdateWhileDrawing(AnnotationShape shape, ShapeDrawingContext context)
    {
        if (shape is BlurRectangleShape blur)
            blur.Rectangle = new Rect(context.StartPoint, context.CurrentPoint);
    }

    public bool IsValidForAdd(AnnotationShape shape) =>
        shape is BlurRectangleShape blur &&
        blur.Rectangle.Width > 10 &&
        blur.Rectangle.Height > 10;

    public bool TryGetHandleAtPoint(AnnotationShape shape, Point point, out ShapeHandleKind handle)
    {
        handle = ShapeHandleKind.None;
        if (shape is not BlurRectangleShape blur)
            return false;

        if (!blur.IsPointOnCornerHandle(point, out var corner))
            return false;

        handle = ToHandleKind(corner);
        return handle != ShapeHandleKind.None;
    }

    public void ApplyHandleDrag(AnnotationShape shape, ShapeHandleKind handle, Point point)
    {
        if (shape is not BlurRectangleShape blur)
            return;

        switch (handle)
        {
            case ShapeHandleKind.TopLeft:
                blur.ResizeFromCorner(RectCorner.TopLeft, point);
                break;
            case ShapeHandleKind.TopRight:
                blur.ResizeFromCorner(RectCorner.TopRight, point);
                break;
            case ShapeHandleKind.BottomLeft:
                blur.ResizeFromCorner(RectCorner.BottomLeft, point);
                break;
            case ShapeHandleKind.BottomRight:
                blur.ResizeFromCorner(RectCorner.BottomRight, point);
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
        SerializableBlurRectangleShape.FromShape((BlurRectangleShape)shape);

    public AnnotationShape Deserialize(SerializableShape serializable) =>
        ((SerializableBlurRectangleShape)serializable).ToShape();

    public void AfterShapeAdded(AnnotationShape shape, ShapeHostContext host) =>
        ConfigureBlurHost(shape, host);

    public void AfterShapeLoaded(AnnotationShape shape, ShapeHostContext host) =>
        ConfigureBlurHost(shape, host);

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

    private static void ConfigureBlurHost(AnnotationShape shape, ShapeHostContext host)
    {
        if (shape is not BlurRectangleShape blur || host.CreateBlurredImage == null)
            return;

        blur.RefreshBlur = host.CreateBlurredImage;
        blur.BlurredImage = host.CreateBlurredImage(blur.Rectangle);
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
