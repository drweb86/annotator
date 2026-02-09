using Avalonia;
using Avalonia.Media;
using System;

namespace ScreenshotAnnotator.Models;

public class SelectorRectangle
{
    public Rect Rectangle { get; set; }

    public void Render(DrawingContext context)
    {
        // Dashed border for selector
        var pen = new Pen(new SolidColorBrush(Colors.Red) { Opacity = 0.8 }, 2.0);
        pen.DashStyle = new DashStyle(new[] { 4.0, 4.0 }, 0);

        context.DrawRectangle(null, pen, Rectangle);

        // Draw corner handles
        var handleSize = 8;
        var handleBrush = Brushes.White;
        var handlePen = new Pen(Brushes.Red, 1);

        DrawHandle(context, Rectangle.TopLeft, handleSize, handleBrush, handlePen);
        DrawHandle(context, Rectangle.TopRight, handleSize, handleBrush, handlePen);
        DrawHandle(context, Rectangle.BottomLeft, handleSize, handleBrush, handlePen);
        DrawHandle(context, Rectangle.BottomRight, handleSize, handleBrush, handlePen);
    }

    private void DrawHandle(DrawingContext context, Point center, double size, IBrush brush, IPen pen)
    {
        var rect = new Rect(center.X - size / 2, center.Y - size / 2, size, size);
        context.DrawRectangle(brush, pen, rect);
    }

    public bool IsPointOnCornerHandle(Point point, out Corner corner)
    {
        corner = Corner.None;
        var handleSize = 8;

        if (IsPointNearHandle(point, Rectangle.TopLeft, handleSize))
        {
            corner = Corner.TopLeft;
            return true;
        }
        if (IsPointNearHandle(point, Rectangle.TopRight, handleSize))
        {
            corner = Corner.TopRight;
            return true;
        }
        if (IsPointNearHandle(point, Rectangle.BottomLeft, handleSize))
        {
            corner = Corner.BottomLeft;
            return true;
        }
        if (IsPointNearHandle(point, Rectangle.BottomRight, handleSize))
        {
            corner = Corner.BottomRight;
            return true;
        }

        return false;
    }

    private bool IsPointNearHandle(Point point, Point handleCenter, double handleSize)
    {
        var distance = Math.Sqrt(Math.Pow(point.X - handleCenter.X, 2) + Math.Pow(point.Y - handleCenter.Y, 2));
        return distance < handleSize;
    }

    public void ResizeFromCorner(Corner corner, Point newPosition)
    {
        var left = Rectangle.Left;
        var top = Rectangle.Top;
        var right = Rectangle.Right;
        var bottom = Rectangle.Bottom;

        switch (corner)
        {
            case Corner.TopLeft:
                left = newPosition.X;
                top = newPosition.Y;
                break;
            case Corner.TopRight:
                right = newPosition.X;
                top = newPosition.Y;
                break;
            case Corner.BottomLeft:
                left = newPosition.X;
                bottom = newPosition.Y;
                break;
            case Corner.BottomRight:
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

    public enum Corner
    {
        None,
        TopLeft,
        TopRight,
        BottomLeft,
        BottomRight
    }
}
