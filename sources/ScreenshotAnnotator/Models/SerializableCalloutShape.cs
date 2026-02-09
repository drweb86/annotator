using Avalonia;

namespace ScreenshotAnnotator.Models;

public class SerializableCalloutShape : SerializableShape
{
    public double RectX { get; set; }
    public double RectY { get; set; }
    public double RectWidth { get; set; }
    public double RectHeight { get; set; }
    public double BeakX { get; set; }
    public double BeakY { get; set; }
    public string Text { get; set; } = "";

    public SerializableCalloutShape()
    {
        Type = "callout";
    }

    public static SerializableCalloutShape FromCalloutShape(CalloutShape callout)
    {
        return new SerializableCalloutShape
        {
            RectX = callout.Rectangle.X,
            RectY = callout.Rectangle.Y,
            RectWidth = callout.Rectangle.Width,
            RectHeight = callout.Rectangle.Height,
            BeakX = callout.BeakPoint.X,
            BeakY = callout.BeakPoint.Y,
            Text = callout.Text,
            StrokeColor = ColorToUInt(callout.StrokeColor),
            StrokeThickness = callout.StrokeThickness
        };
    }

    public CalloutShape ToCalloutShape()
    {
        return new CalloutShape
        {
            Rectangle = new Rect(RectX, RectY, RectWidth, RectHeight),
            BeakPoint = new Point(BeakX, BeakY),
            Text = Text,
            StrokeColor = UIntToColor(StrokeColor),
            StrokeThickness = StrokeThickness
        };
    }
}
