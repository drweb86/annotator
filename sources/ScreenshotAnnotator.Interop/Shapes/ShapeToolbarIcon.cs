namespace ScreenshotAnnotator.Interop.Shapes;

public sealed class ShapeToolbarIcon
{
    public required string PathData { get; init; }
    public uint StrokeColorArgb { get; init; } = 0xFFD8DEE3;
    public uint FillColorArgb { get; init; }
    public double StrokeThickness { get; init; } = 2;
}
