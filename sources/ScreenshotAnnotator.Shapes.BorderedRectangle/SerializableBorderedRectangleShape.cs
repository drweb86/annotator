using Avalonia;
using ScreenshotAnnotator.Models;

namespace ScreenshotAnnotator.Shapes.BorderedRectangle;

public sealed class SerializableBorderedRectangleShape : SerializableShape
{
    public double RectX { get; set; }
    public double RectY { get; set; }
    public double RectWidth { get; set; }
    public double RectHeight { get; set; }

    public SerializableBorderedRectangleShape() => Type = "borderedrectangle";

    public static SerializableBorderedRectangleShape FromShape(BorderedRectangleShape shape) => new()
    {
        RectX = shape.Rectangle.X,
        RectY = shape.Rectangle.Y,
        RectWidth = shape.Rectangle.Width,
        RectHeight = shape.Rectangle.Height,
        StrokeColor = ColorToUInt(shape.StrokeColor),
        StrokeThickness = shape.StrokeThickness
    };

    public BorderedRectangleShape ToShape() => new()
    {
        Rectangle = new Rect(RectX, RectY, RectWidth, RectHeight),
        StrokeColor = UIntToColor(StrokeColor),
        StrokeThickness = StrokeThickness
    };
}
