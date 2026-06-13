using Avalonia;

namespace ScreenshotAnnotator.Interop.Shapes.Common;

public static class ShapeCutHelpers
{
    public static Point AdjustPointForVerticalCut(Point point, double cutX, double cutWidth)
    {
        if (point.X >= cutX + cutWidth)
            return new Point(point.X - cutWidth, point.Y);
        if (point.X >= cutX)
            return new Point(cutX, point.Y);
        return point;
    }

    public static Point AdjustPointForHorizontalCut(Point point, double cutY, double cutHeight)
    {
        if (point.Y >= cutY + cutHeight)
            return new Point(point.X, point.Y - cutHeight);
        if (point.Y >= cutY)
            return new Point(point.X, cutY);
        return point;
    }

    public static Rect AdjustRectForVerticalCut(Rect rect, double cutX, double cutWidth)
    {
        var topLeft = AdjustPointForVerticalCut(rect.TopLeft, cutX, cutWidth);
        var bottomRight = AdjustPointForVerticalCut(rect.BottomRight, cutX, cutWidth);
        return new Rect(topLeft, bottomRight);
    }

    public static Rect AdjustRectForHorizontalCut(Rect rect, double cutY, double cutHeight)
    {
        var topLeft = AdjustPointForHorizontalCut(rect.TopLeft, cutY, cutHeight);
        var bottomRight = AdjustPointForHorizontalCut(rect.BottomRight, cutY, cutHeight);
        return new Rect(topLeft, bottomRight);
    }
}
