using Avalonia;
using ScreenshotAnnotator.Models;

namespace ScreenshotAnnotator.Shapes.Highlighter;

public sealed class SerializableHighlighterShape : SerializableShape
{
    public double StartX { get; set; }
    public double StartY { get; set; }
    public double EndX { get; set; }
    public double EndY { get; set; }
    public uint FillColor { get; set; } = 0x64FFFF00;

    public SerializableHighlighterShape() => Type = "highlighter";

    public static SerializableHighlighterShape FromShape(HighlighterShape shape) => new()
    {
        StartX = shape.StartPoint.X,
        StartY = shape.StartPoint.Y,
        EndX = shape.EndPoint.X,
        EndY = shape.EndPoint.Y,
        StrokeColor = ColorToUInt(shape.StrokeColor),
        StrokeThickness = shape.StrokeThickness,
        FillColor = ColorToUInt(shape.FillColor)
    };

    public HighlighterShape ToShape() => new()
    {
        StartPoint = new Point(StartX, StartY),
        EndPoint = new Point(EndX, EndY),
        StrokeColor = UIntToColor(StrokeColor),
        StrokeThickness = StrokeThickness,
        FillColor = UIntToColor(FillColor)
    };
}
