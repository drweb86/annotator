using System;

namespace ScreenshotAnnotator.Models;

internal static class SerializableShapeExtensions
{
    public static AnnotationShape ToAnnotationShape(this SerializableShape serializableShape)
    {
        return serializableShape switch
        {
            SerializableArrowShape arrow => arrow.ToArrowShape(),
            SerializableCalloutShape callout => callout.ToCalloutShape(),
            SerializableCalloutNoArrowShape calloutNoArrow => calloutNoArrow.ToCalloutNoArrowShape(),
            SerializableBorderedRectangleShape borderedRect => borderedRect.ToBorderedRectangleShape(),
            SerializableBlurRectangleShape blurRect => blurRect.ToBlurRectangleShape(),
            SerializableHighlighterShape highlighter => highlighter.ToHighlighterShape(),
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}
