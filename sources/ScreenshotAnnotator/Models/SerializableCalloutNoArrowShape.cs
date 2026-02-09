using Avalonia;

namespace ScreenshotAnnotator.Models;

public class SerializableCalloutNoArrowShape : SerializableShape
{
    public double RectX { get; set; }
    public double RectY { get; set; }
    public double RectWidth { get; set; }
    public double RectHeight { get; set; }
    public string Text { get; set; } = "";

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
            StrokeColor = UIntToColor(StrokeColor),
            StrokeThickness = StrokeThickness
        };
    }
}
