using Avalonia;

namespace ScreenshotAnnotator.Models;

public class SerializableCalloutNoArrowShape : SerializableShape
{
    public double RectX { get; set; }
    public double RectY { get; set; }
    public double RectWidth { get; set; }
    public double RectHeight { get; set; }
    public string Text { get; set; } = "";

    /// <summary>Optional. When missing (old projects), default Arial is used.</summary>
    public string? FontFamily { get; set; }
    /// <summary>Optional. When missing (old projects), default 24 is used.</summary>
    public double? FontSize { get; set; }
    public bool? FontBold { get; set; }
    public bool? FontItalic { get; set; }

    public SerializableCalloutNoArrowShape()
    {
        Type = "calloutnoarrow";
    }

    public static SerializableCalloutNoArrowShape FromCalloutNoArrowShape(CalloutNoArrowShape calloutNoArrow)
    {
        return new SerializableCalloutNoArrowShape
        {
            RectX = calloutNoArrow.Rectangle.X,
            RectY = calloutNoArrow.Rectangle.Y,
            RectWidth = calloutNoArrow.Rectangle.Width,
            RectHeight = calloutNoArrow.Rectangle.Height,
            Text = calloutNoArrow.Text,
            FontFamily = calloutNoArrow.FontFamily,
            FontSize = calloutNoArrow.FontSize,
            FontBold = calloutNoArrow.FontBold,
            FontItalic = calloutNoArrow.FontItalic,
            StrokeColor = ColorToUInt(calloutNoArrow.StrokeColor),
            StrokeThickness = calloutNoArrow.StrokeThickness
        };
    }

    public CalloutNoArrowShape ToCalloutNoArrowShape()
    {
        return new CalloutNoArrowShape
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
}
