using Avalonia;
using Avalonia.Media;
using System;

namespace ScreenshotAnnotator.Models;

public class BorderedRectangleShape : AnnotationShape
{
    public Rect Rectangle { get; set; }

    public override void Render(DrawingContext context)
    {
        var color = IsSelected ? Colors.Blue : StrokeColor;
        var thickness = IsSelected ? StrokeThickness + 1 : StrokeThickness;
        // Use half the thickness for this tool
        var actualThickness = thickness / 2.0;
        var pen = new Pen(new SolidColorBrush(color), actualThickness);

        // Shadow parameters
        var shadowOffset = new Vector(3, 3);
        var shadowColor = Color.FromArgb(100, 0, 0, 0);
        var shadowPen = new Pen(new SolidColorBrush(shadowColor), actualThickness);

        // Draw shadow
        var shadowRect = new Rect(
            Rectangle.X + shadowOffset.X,
            Rectangle.Y + shadowOffset.Y,
            Rectangle.Width,
            Rectangle.Height
        );
        context.DrawRectangle(null, shadowPen, shadowRect);

        // Draw transparent rectangle with border
        context.DrawRectangle(null, pen, Rectangle);

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
        // Hit test on the border (not inside)
        if (Rectangle.Contains(point))
        {
            // Check if point is near the border
            var borderThreshold = Math.Max(10, StrokeThickness * 2);
            var innerRect = new Rect(
                Rectangle.X + borderThreshold,
                Rectangle.Y + borderThreshold,
                Rectangle.Width - borderThreshold * 2,
                Rectangle.Height - borderThreshold * 2
            );
            return !innerRect.Contains(point);
        }
        return false;
    }

    public override void Move(Vector offset)
    {
        Rectangle = new Rect(
            Rectangle.X + offset.X,
            Rectangle.Y + offset.Y,
            Rectangle.Width,
            Rectangle.Height
        );
    }

    public override Rect GetBounds() => Rectangle;

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
