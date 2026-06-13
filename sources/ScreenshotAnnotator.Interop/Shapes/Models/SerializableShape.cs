using Avalonia.Media;

namespace ScreenshotAnnotator.Models;

/// <summary>
/// Base class for serializable shapes. Polymorphic JSON is handled by <see cref="Interop.Serialization.ShapeJsonConverter"/>.
/// </summary>
public abstract class SerializableShape
{
    public string Type { get; set; } = "";
    public uint StrokeColor { get; set; } = 0xFFFF0000;
    public double StrokeThickness { get; set; } = 2.0;

    protected static uint ColorToUInt(Color color)
    {
        return ((uint)color.A << 24) | ((uint)color.R << 16) | ((uint)color.G << 8) | color.B;
    }

    protected static Color UIntToColor(uint color)
    {
        return Color.FromArgb(
            (byte)((color >> 24) & 0xFF),
            (byte)((color >> 16) & 0xFF),
            (byte)((color >> 8) & 0xFF),
            (byte)(color & 0xFF));
    }
}
