using Avalonia;
using ScreenshotAnnotator.Models;

namespace ScreenshotAnnotator.Shapes.BlurRectangle;

public sealed class SerializableBlurRectangleShape : SerializableShape
{
    public double RectX { get; set; }
    public double RectY { get; set; }
    public double RectWidth { get; set; }
    public double RectHeight { get; set; }

    public SerializableBlurRectangleShape() => Type = "blurrectangle";

    public static SerializableBlurRectangleShape FromShape(BlurRectangleShape shape) => new()
    {
        RectX = shape.Rectangle.X,
        RectY = shape.Rectangle.Y,
        RectWidth = shape.Rectangle.Width,
        RectHeight = shape.Rectangle.Height,
        StrokeColor = ColorToUInt(shape.StrokeColor),
        StrokeThickness = shape.StrokeThickness
    };

    public BlurRectangleShape ToShape() => new()
    {
        Rectangle = new Rect(RectX, RectY, RectWidth, RectHeight),
        StrokeColor = UIntToColor(StrokeColor),
        StrokeThickness = StrokeThickness
    };
}
