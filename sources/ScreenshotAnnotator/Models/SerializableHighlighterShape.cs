using Avalonia;

namespace ScreenshotAnnotator.Models;

public class SerializableHighlighterShape : SerializableShape
{
    public double StartX { get; set; }
    public double StartY { get; set; }
    public double EndX { get; set; }
    public double EndY { get; set; }
    public uint FillColor { get; set; } = 0x64FFFF00; // Semi-transparent yellow default

    public SerializableHighlighterShape()
    {
        Type = "highlighter";
    }

    public static SerializableHighlighterShape FromHighlighterShape(HighlighterShape highlighter)
    {
        return new SerializableHighlighterShape
        {
            StartX = highlighter.StartPoint.X,
            StartY = highlighter.StartPoint.Y,
            EndX = highlighter.EndPoint.X,
            EndY = highlighter.EndPoint.Y,
            StrokeColor = ColorToUInt(highlighter.StrokeColor),
            StrokeThickness = highlighter.StrokeThickness,
            FillColor = ColorToUInt(highlighter.FillColor)
        };
    }

    public HighlighterShape ToHighlighterShape()
    {
        return new HighlighterShape
        {
            StartPoint = new Point(StartX, StartY),
            EndPoint = new Point(EndX, EndY),
            StrokeColor = UIntToColor(StrokeColor),
            StrokeThickness = StrokeThickness,
            FillColor = UIntToColor(FillColor)
        };
    }
}
