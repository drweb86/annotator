using System;
using Avalonia;
using Avalonia.Media;

namespace ScreenshotAnnotator.Models;

public class CalloutNoArrowShape : AnnotationShape
{
    public Rect Rectangle { get; set; }
    public string Text { get; set; } = "";

    /// <summary>Font family name for the text. Default "Arial" when not set (existing projects).</summary>
    public string FontFamily { get; set; } = "Arial";
    /// <summary>Font size in pixels. Default 24 when not set (existing projects).</summary>
    public double FontSize { get; set; } = 24;
    public bool FontBold { get; set; }
    public bool FontItalic { get; set; }

    public override void Render(DrawingContext context)
    {
        var color = IsSelected ? Colors.Blue : StrokeColor;
        var thickness = IsSelected ? StrokeThickness + 1 : StrokeThickness;
        var pen = new Pen(new SolidColorBrush(color), thickness);
        var fillBrush = new SolidColorBrush(color);

        // Shadow parameters
        var shadowOffset = new Vector(3, 3);
        var shadowColor = Color.FromArgb(100, 0, 0, 0);
        var shadowBrush = new SolidColorBrush(shadowColor);

        // Draw shadow (no corner radius)
        var shadowRect = new Rect(
            Rectangle.X + shadowOffset.X,
            Rectangle.Y + shadowOffset.Y,
            Rectangle.Width,
            Rectangle.Height
        );
        context.DrawRectangle(shadowBrush, null, shadowRect);

        // Draw filled rectangle (no corner radius)
        context.DrawRectangle(fillBrush, pen, Rectangle);

        // Draw text if any - centered horizontally and vertically, white color
        if (!string.IsNullOrWhiteSpace(Text))
        {
            var padding = 20.0; // Padding inside the rectangle
            var maxWidth = Math.Max(50, Rectangle.Width - padding * 2); // Max width for text wrapping

            var typeface = new Typeface(
                FontFamily,
                FontItalic ? FontStyle.Italic : FontStyle.Normal,
                FontBold ? FontWeight.Bold : FontWeight.Normal);
            var formattedText = new FormattedText(
                Text,
                System.Globalization.CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                typeface,
                FontSize,
                Brushes.White
            )
            {
                TextAlignment = TextAlignment.Center,
                MaxTextWidth = maxWidth,
                MaxTextHeight = Math.Max(30, Rectangle.Height - padding * 2)
            };

            // Center text in rectangle using actual bounds
            var textPoint = new Point(
                Rectangle.Left + padding,
                Rectangle.Top + (Rectangle.Height - formattedText.Height) / 2
            );

            context.DrawText(formattedText, textPoint);
        }

        // Draw selection handles if selected
        if (IsSelected)
        {
            var handleSize = 6;
            var handleBrush = Brushes.Blue;
            DrawHandle(context, Rectangle.TopLeft, handleSize, handleBrush);
            DrawHandle(context, Rectangle.TopRight, handleSize, handleBrush);
            DrawHandle(context, Rectangle.BottomLeft, handleSize, handleBrush);
            DrawHandle(context, Rectangle.BottomRight, handleSize, handleBrush);
        }
    }

    public override bool HitTest(Point point)
    {
        return Rectangle.Contains(point);
    }

    public override Rect GetBounds() => Rectangle;

    public override void Move(Vector offset)
    {
        Rectangle = new Rect(
            Rectangle.X + offset.X,
            Rectangle.Y + offset.Y,
            Rectangle.Width,
            Rectangle.Height
        );
    }

    public bool IsPointOnCornerHandle(Point point, out CalloutShape.Corner corner)
    {
        corner = CalloutShape.Corner.None;
        var handleSize = 8;

        if (IsPointNearHandle(point, Rectangle.TopLeft, handleSize))
        {
            corner = CalloutShape.Corner.TopLeft;
            return true;
        }
        if (IsPointNearHandle(point, Rectangle.TopRight, handleSize))
        {
            corner = CalloutShape.Corner.TopRight;
            return true;
        }
        if (IsPointNearHandle(point, Rectangle.BottomLeft, handleSize))
        {
            corner = CalloutShape.Corner.BottomLeft;
            return true;
        }
        if (IsPointNearHandle(point, Rectangle.BottomRight, handleSize))
        {
            corner = CalloutShape.Corner.BottomRight;
            return true;
        }

        return false;
    }

    private bool IsPointNearHandle(Point point, Point handleCenter, double handleSize)
    {
        var distance = Math.Sqrt(Math.Pow(point.X - handleCenter.X, 2) + Math.Pow(point.Y - handleCenter.Y, 2));
        return distance < handleSize;
    }

    public void ResizeFromCorner(CalloutShape.Corner corner, Point newPosition)
    {
        var left = Rectangle.Left;
        var top = Rectangle.Top;
        var right = Rectangle.Right;
        var bottom = Rectangle.Bottom;

        switch (corner)
        {
            case CalloutShape.Corner.TopLeft:
                left = newPosition.X;
                top = newPosition.Y;
                break;
            case CalloutShape.Corner.TopRight:
                right = newPosition.X;
                top = newPosition.Y;
                break;
            case CalloutShape.Corner.BottomLeft:
                left = newPosition.X;
                bottom = newPosition.Y;
                break;
            case CalloutShape.Corner.BottomRight:
                right = newPosition.X;
                bottom = newPosition.Y;
                break;
        }

        // Ensure minimum size
        if (right - left < 20) return;
        if (bottom - top < 20) return;

        Rectangle = new Rect(
            new Point(left, top),
            new Point(right, bottom)
        );
    }

    private void DrawHandle(DrawingContext context, Point center, double size, IBrush brush)
    {
        var rect = new Rect(center.X - size / 2, center.Y - size / 2, size, size);
        context.DrawRectangle(brush, new Pen(Brushes.White, 1), rect);
    }
}
