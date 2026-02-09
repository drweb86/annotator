using Avalonia;

namespace ScreenshotAnnotator.Models;

public class SerializableBorderedRectangleShape : SerializableShape
{
    public double RectX { get; set; }
    public double RectY { get; set; }
    public double RectWidth { get; set; }
    public double RectHeight { get; set; }

    public SerializableBorderedRectangleShape()
    {
        Type = "borderedrectangle";
    }

    public static SerializableBorderedRectangleShape FromBorderedRectangleShape(BorderedRectangleShape borderedRect)
    {
        return new SerializableBorderedRectangleShape
        {
            RectX = borderedRect.Rectangle.X,
            RectY = borderedRect.Rectangle.Y,
            RectWidth = borderedRect.Rectangle.Width,
            RectHeight = borderedRect.Rectangle.Height,
            StrokeColor = ColorToUInt(borderedRect.StrokeColor),
            StrokeThickness = borderedRect.StrokeThickness
        };
    }

    public BorderedRectangleShape ToBorderedRectangleShape()
    {
        return new BorderedRectangleShape
        {
            Rectangle = new Rect(RectX, RectY, RectWidth, RectHeight),
            StrokeColor = UIntToColor(StrokeColor),
            StrokeThickness = StrokeThickness
        };
    }
}
