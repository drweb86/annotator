using System.Collections.Generic;
using System.Text.Json.Serialization;
using Avalonia;

namespace ScreenshotAnnotator.Models;

/// <summary>
/// Represents a serializable project that can be saved and loaded
/// </summary>
public class AnnotatorProject
{
    public int Version { get; set; } = 1;
    public string PreviewImageBase64 { get; set; } = "";
    public string BaseImageBase64 { get; set; } = "";
    public List<SerializableShape> Shapes { get; set; } = new();
}

/// <summary>
/// Base class for serializable shapes
/// </summary>
[JsonDerivedType(typeof(SerializableArrowShape), "arrow")]
[JsonDerivedType(typeof(SerializableCalloutShape), "callout")]
public abstract class SerializableShape
{
    public string Type { get; set; } = "";
    public uint StrokeColor { get; set; } = 0xFFFF0000; // ARGB format
    public double StrokeThickness { get; set; } = 2.0;
}

public class SerializableArrowShape : SerializableShape
{
    public double StartX { get; set; }
    public double StartY { get; set; }
    public double EndX { get; set; }
    public double EndY { get; set; }

    public SerializableArrowShape()
    {
        Type = "arrow";
    }

    public static SerializableArrowShape FromArrowShape(ArrowShape arrow)
    {
        return new SerializableArrowShape
        {
            StartX = arrow.StartPoint.X,
            StartY = arrow.StartPoint.Y,
            EndX = arrow.EndPoint.X,
            EndY = arrow.EndPoint.Y,
            StrokeColor = ColorToUInt(arrow.StrokeColor),
            StrokeThickness = arrow.StrokeThickness
        };
    }

    public ArrowShape ToArrowShape()
    {
        return new ArrowShape
        {
            StartPoint = new Point(StartX, StartY),
            EndPoint = new Point(EndX, EndY),
            StrokeColor = UIntToColor(StrokeColor),
            StrokeThickness = StrokeThickness
        };
    }

    private static uint ColorToUInt(Avalonia.Media.Color color)
    {
        return ((uint)color.A << 24) | ((uint)color.R << 16) | ((uint)color.G << 8) | color.B;
    }

    private static Avalonia.Media.Color UIntToColor(uint color)
    {
        return Avalonia.Media.Color.FromArgb(
            (byte)((color >> 24) & 0xFF),
            (byte)((color >> 16) & 0xFF),
            (byte)((color >> 8) & 0xFF),
            (byte)(color & 0xFF)
        );
    }
}

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

    private static uint ColorToUInt(Avalonia.Media.Color color)
    {
        return ((uint)color.A << 24) | ((uint)color.R << 16) | ((uint)color.G << 8) | color.B;
    }

    private static Avalonia.Media.Color UIntToColor(uint color)
    {
        return Avalonia.Media.Color.FromArgb(
            (byte)((color >> 24) & 0xFF),
            (byte)((color >> 16) & 0xFF),
            (byte)((color >> 8) & 0xFF),
            (byte)(color & 0xFF)
        );
    }
}
