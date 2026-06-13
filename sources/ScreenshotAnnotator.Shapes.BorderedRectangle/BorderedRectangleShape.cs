using Avalonia;
using Avalonia.Media;
using ScreenshotAnnotator.Interop.Shapes;
using ScreenshotAnnotator.Models;
using System;

namespace ScreenshotAnnotator.Shapes.BorderedRectangle;

public sealed class BorderedRectangleShape : AnnotationShape, ICornerResizableShape
{
    public Rect Rectangle { get; set; }

    public override void Render(DrawingContext context)
    {
        var color = StrokeColor;
        var actualThickness = StrokeThickness / 2.0;
        var pen = new Pen(new SolidColorBrush(color), actualThickness);

        var shadowOffset = new Vector(3, 3);
        var shadowColor = Color.FromArgb(100, 0, 0, 0);
        var shadowPen = new Pen(new SolidColorBrush(shadowColor), actualThickness);

        var shadowRect = new Rect(
            Rectangle.X + shadowOffset.X,
            Rectangle.Y + shadowOffset.Y,
            Rectangle.Width,
            Rectangle.Height);
        context.DrawRectangle(null, shadowPen, shadowRect);

        context.DrawRectangle(null, pen, Rectangle);

        if (IsSelected)
        {
            const double handleSize = 6;
            var handleBrush = Brushes.White;
            var handlePen = new Pen(Brushes.Black, 1);
            DrawHandle(context, Rectangle.TopLeft, handleSize, handleBrush, handlePen);
            DrawHandle(context, Rectangle.TopRight, handleSize, handleBrush, handlePen);
            DrawHandle(context, Rectangle.BottomLeft, handleSize, handleBrush, handlePen);
            DrawHandle(context, Rectangle.BottomRight, handleSize, handleBrush, handlePen);
        }
    }

    public override bool HitTest(Point point)
    {
        if (!Rectangle.Contains(point))
            return false;

        var borderThreshold = Math.Max(10, StrokeThickness * 2);
        var innerRect = new Rect(
            Rectangle.X + borderThreshold,
            Rectangle.Y + borderThreshold,
            Rectangle.Width - borderThreshold * 2,
            Rectangle.Height - borderThreshold * 2);
        return !innerRect.Contains(point);
    }

    public override void Move(Vector offset)
    {
        Rectangle = new Rect(
            Rectangle.X + offset.X,
            Rectangle.Y + offset.Y,
            Rectangle.Width,
            Rectangle.Height);
    }

    public override Rect GetBounds() => Rectangle;

    public bool IsPointOnCornerHandle(Point point, out RectCorner corner)
    {
        corner = RectCorner.None;
        const double handleSize = 8;

        if (IsPointNearHandle(point, Rectangle.TopLeft, handleSize))
        {
            corner = RectCorner.TopLeft;
            return true;
        }

        if (IsPointNearHandle(point, Rectangle.TopRight, handleSize))
        {
            corner = RectCorner.TopRight;
            return true;
        }

        if (IsPointNearHandle(point, Rectangle.BottomLeft, handleSize))
        {
            corner = RectCorner.BottomLeft;
            return true;
        }

        if (IsPointNearHandle(point, Rectangle.BottomRight, handleSize))
        {
            corner = RectCorner.BottomRight;
            return true;
        }

        return false;
    }

    public void ResizeFromCorner(RectCorner corner, Point newPosition)
    {
        var left = Rectangle.Left;
        var top = Rectangle.Top;
        var right = Rectangle.Right;
        var bottom = Rectangle.Bottom;

        switch (corner)
        {
            case RectCorner.TopLeft:
                left = newPosition.X;
                top = newPosition.Y;
                break;
            case RectCorner.TopRight:
                right = newPosition.X;
                top = newPosition.Y;
                break;
            case RectCorner.BottomLeft:
                left = newPosition.X;
                bottom = newPosition.Y;
                break;
            case RectCorner.BottomRight:
                right = newPosition.X;
                bottom = newPosition.Y;
                break;
        }

        if (right - left < 20 || bottom - top < 20)
            return;

        Rectangle = new Rect(new Point(left, top), new Point(right, bottom));
    }

    private static bool IsPointNearHandle(Point point, Point handleCenter, double handleSize) =>
        Math.Sqrt(Math.Pow(point.X - handleCenter.X, 2) + Math.Pow(point.Y - handleCenter.Y, 2)) < handleSize;

    private static void DrawHandle(DrawingContext context, Point center, double size, IBrush brush, IPen pen)
    {
        var rect = new Rect(center.X - size / 2, center.Y - size / 2, size, size);
        context.DrawRectangle(brush, pen, rect);
    }
}
