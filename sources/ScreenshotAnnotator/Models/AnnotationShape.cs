using Avalonia;
using Avalonia.Media;

namespace ScreenshotAnnotator.Models;

public abstract class AnnotationShape
{
    public Color StrokeColor { get; set; } = Colors.Red;
    public double StrokeThickness { get; set; } = 10.0;
    public bool IsSelected { get; set; }

    public abstract void Render(DrawingContext context);
    public abstract bool HitTest(Point point);
    public abstract void Move(Vector offset);
}
