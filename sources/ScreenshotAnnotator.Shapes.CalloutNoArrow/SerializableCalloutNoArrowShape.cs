using Avalonia;
using ScreenshotAnnotator.Models;

namespace ScreenshotAnnotator.Shapes.CalloutNoArrow;

public sealed class SerializableCalloutNoArrowShape : SerializableShape
{
    public double RectX { get; set; }
    public double RectY { get; set; }
    public double RectWidth { get; set; }
    public double RectHeight { get; set; }
    public string Text { get; set; } = "";
    public string? FontFamily { get; set; }
    public double? FontSize { get; set; }
    public bool? FontBold { get; set; }
    public bool? FontItalic { get; set; }

    public SerializableCalloutNoArrowShape() => Type = "calloutnoarrow";

    public static SerializableCalloutNoArrowShape FromShape(CalloutNoArrowShape shape) => new()
    {
        RectX = shape.Rectangle.X,
        RectY = shape.Rectangle.Y,
        RectWidth = shape.Rectangle.Width,
        RectHeight = shape.Rectangle.Height,
        Text = shape.Text,
        FontFamily = shape.FontFamily,
        FontSize = shape.FontSize,
        FontBold = shape.FontBold,
        FontItalic = shape.FontItalic,
        StrokeColor = ColorToUInt(shape.StrokeColor),
        StrokeThickness = shape.StrokeThickness
    };

    public CalloutNoArrowShape ToShape() => new()
    {
        Rectangle = new Rect(RectX, RectY, RectWidth, RectHeight),
        Text = Text,
        FontFamily = FontFamily ?? "Arial",
        FontSize = FontSize ?? 24,
        FontBold = FontBold ?? false,
        FontItalic = FontItalic ?? false,
        StrokeColor = UIntToColor(StrokeColor),
        StrokeThickness = StrokeThickness
    };
}
