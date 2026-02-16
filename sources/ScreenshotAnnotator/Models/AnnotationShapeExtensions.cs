using System;

namespace ScreenshotAnnotator.Models;

internal static class AnnotationShapeExtensions
{
    public static SerializableShape ToSerializableShape(this AnnotationShape shape)
    {
        return shape switch
        {
            ArrowShape arrow => SerializableArrowShape.FromArrowShape(arrow),
            CalloutShape callout => SerializableCalloutShape.FromCalloutShape(callout),
            CalloutNoArrowShape calloutNoArrow => SerializableCalloutNoArrowShape.FromCalloutNoArrowShape(calloutNoArrow),
            BorderedRectangleShape borderedRect => SerializableBorderedRectangleShape.FromBorderedRectangleShape(borderedRect),
            BlurRectangleShape blurRect => SerializableBlurRectangleShape.FromBlurRectangleShape(blurRect),
            HighlighterShape highlighter => SerializableHighlighterShape.FromHighlighterShape(highlighter),
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}
