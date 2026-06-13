using ScreenshotAnnotator.Interop.Shapes;
using ScreenshotAnnotator.Models;

namespace ScreenshotAnnotator.Models;

public static class ShapeSerializationExtensions
{
    public static SerializableShape ToSerializableShape(this AnnotationShape shape)
        => ShapeRegistry.ToSerializableShape(shape);

    public static AnnotationShape ToAnnotationShape(this SerializableShape shape)
        => ShapeRegistry.ToAnnotationShape(shape);
}
