using ScreenshotAnnotator.Interop.Shapes;
using ScreenshotAnnotator.Shapes.Arrow;
using ScreenshotAnnotator.Shapes.BorderedRectangle;
using ScreenshotAnnotator.Shapes.BlurRectangle;
using ScreenshotAnnotator.Shapes.Callout;
using ScreenshotAnnotator.Shapes.CalloutNoArrow;
using ScreenshotAnnotator.Shapes.Highlighter;

namespace ScreenshotAnnotator.Services.Shapes;

public static class BuiltInShapeRegistration
{
    public static void RegisterAll()
    {
        ShapeRegistry.Register(new ArrowShapePlugin());
        ShapeRegistry.Register(new CalloutShapePlugin());
        ShapeRegistry.Register(new CalloutNoArrowShapePlugin());
        ShapeRegistry.Register(new BorderedRectangleShapePlugin());
        ShapeRegistry.Register(new HighlighterShapePlugin());
        ShapeRegistry.Register(new BlurRectangleShapePlugin());
    }
}
