using Avalonia;

namespace ScreenshotAnnotator.Models;

public class SerializableBlurRectangleShape : SerializableShape
{
    public double RectX { get; set; }
    public double RectY { get; set; }
    public double RectWidth { get; set; }
    public double RectHeight { get; set; }

    public SerializableBlurRectangleShape()
    {
        Type = "blurrectangle";
    }

    public static SerializableBlurRectangleShape FromBlurRectangleShape(BlurRectangleShape blurRect)
    {
        return new SerializableBlurRectangleShape
        {
            RectX = blurRect.Rectangle.X,
            RectY = blurRect.Rectangle.Y,
            RectWidth = blurRect.Rectangle.Width,
            RectHeight = blurRect.Rectangle.Height,
            StrokeColor = ColorToUInt(blurRect.StrokeColor),
            StrokeThickness = blurRect.StrokeThickness
        };
    }

    public BlurRectangleShape ToBlurRectangleShape()
    {
        return new BlurRectangleShape
        {
            Rectangle = new Rect(RectX, RectY, RectWidth, RectHeight),
            StrokeColor = UIntToColor(StrokeColor),
            StrokeThickness = StrokeThickness
        };
    }
}
