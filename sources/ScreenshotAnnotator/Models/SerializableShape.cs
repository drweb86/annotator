using System.Text.Json.Serialization;
using Avalonia.Media;

namespace ScreenshotAnnotator.Models;

/// <summary>
/// Base class for serializable shapes
/// </summary>
[JsonDerivedType(typeof(SerializableArrowShape), "arrow")]
[JsonDerivedType(typeof(SerializableCalloutShape), "callout")]
[JsonDerivedType(typeof(SerializableCalloutNoArrowShape), "calloutnoarrow")]
[JsonDerivedType(typeof(SerializableBorderedRectangleShape), "borderedrectangle")]
[JsonDerivedType(typeof(SerializableBlurRectangleShape), "blurrectangle")]
[JsonDerivedType(typeof(SerializableHighlighterShape), "highlighter")]
public abstract class SerializableShape
{
    public string Type { get; set; } = "";
    public uint StrokeColor { get; set; } = 0xFFFF0000; // ARGB format
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
            (byte)(color & 0xFF)
        );
    }
}
