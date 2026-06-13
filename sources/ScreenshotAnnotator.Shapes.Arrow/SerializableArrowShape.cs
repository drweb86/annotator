using Avalonia;
using ScreenshotAnnotator.Models;

namespace ScreenshotAnnotator.Shapes.Arrow;

public sealed class SerializableArrowShape : SerializableShape
{
    public double StartX { get; set; }
    public double StartY { get; set; }
    public double EndX { get; set; }
    public double EndY { get; set; }

    public SerializableArrowShape() => Type = "arrow";

    public static SerializableArrowShape FromShape(ArrowShape arrow) => new()
    {
        StartX = arrow.StartPoint.X,
        StartY = arrow.StartPoint.Y,
        EndX = arrow.EndPoint.X,
        EndY = arrow.EndPoint.Y,
        StrokeColor = ColorToUInt(arrow.StrokeColor),
        StrokeThickness = arrow.StrokeThickness
    };

    public ArrowShape ToShape() => new()
    {
        StartPoint = new Point(StartX, StartY),
        EndPoint = new Point(EndX, EndY),
        StrokeColor = UIntToColor(StrokeColor),
        StrokeThickness = StrokeThickness
    };
}
