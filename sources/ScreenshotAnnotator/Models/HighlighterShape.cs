using System;
using Avalonia;
using Avalonia.Media;

namespace ScreenshotAnnotator.Models;

public class HighlighterShape : AnnotationShape
{
    public Point StartPoint { get; set; }
    public Point EndPoint { get; set; }
    public Color FillColor { get; set; } = Color.FromArgb(100, 255, 255, 0);

    public override void Render(DrawingContext context)
    {
        var rect = new Rect(StartPoint, EndPoint);

        var fillBrush = new SolidColorBrush(FillColor);

        if (IsSelected)
        {
            // Show a thin border when selected
            var pen = new Pen(Brushes.Orange, 1.0);
            pen.DashStyle = new DashStyle(new[] { 4.0, 4.0 }, 0);
            context.DrawRectangle(fillBrush, pen, rect);

            // Draw corner handles
            var handleSize = 8;
            var handleBrush = Brushes.White;
            var handlePen = new Pen(Brushes.Orange, 1);

            DrawHandle(context, rect.TopLeft, handleSize, handleBrush, handlePen);
            DrawHandle(context, rect.TopRight, handleSize, handleBrush, handlePen);
            DrawHandle(context, rect.BottomLeft, handleSize, handleBrush, handlePen);
            DrawHandle(context, rect.BottomRight, handleSize, handleBrush, handlePen);
        }
        else
        {
            context.DrawRectangle(fillBrush, null, rect);
        }
    }

    private void DrawHandle(DrawingContext context, Point center, double size, IBrush brush, IPen pen)
    {
        var rect = new Rect(center.X - size / 2, center.Y - size / 2, size, size);
        context.DrawRectangle(brush, pen, rect);
    }

    public override bool HitTest(Point point)
    {
        var rect = new Rect(StartPoint, EndPoint);
        return rect.Contains(point);
    }

    public override void Move(Vector offset)
    {
        StartPoint += offset;
        EndPoint += offset;
    }

    public override Rect GetBounds() => new Rect(StartPoint, EndPoint);

    public Corner? GetCornerAtPoint(Point point, double handleSize = 8)
    {
        var rect = new Rect(StartPoint, EndPoint);
        if (Math.Abs(point.X - rect.TopLeft.X) < handleSize && Math.Abs(point.Y - rect.TopLeft.Y) < handleSize)
            return Corner.TopLeft;
        if (Math.Abs(point.X - rect.TopRight.X) < handleSize && Math.Abs(point.Y - rect.TopRight.Y) < handleSize)
            return Corner.TopRight;
        if (Math.Abs(point.X - rect.BottomLeft.X) < handleSize && Math.Abs(point.Y - rect.BottomLeft.Y) < handleSize)
            return Corner.BottomLeft;
        if (Math.Abs(point.X - rect.BottomRight.X) < handleSize && Math.Abs(point.Y - rect.BottomRight.Y) < handleSize)
            return Corner.BottomRight;
        return null;
    }

    public void ResizeCorner(Corner corner, Point newPosition)
    {
        switch (corner)
        {
            case Corner.TopLeft:
                StartPoint = newPosition;
                break;
            case Corner.TopRight:
                StartPoint = new Point(StartPoint.X, newPosition.Y);
                EndPoint = new Point(newPosition.X, EndPoint.Y);
                break;
            case Corner.BottomLeft:
                StartPoint = new Point(newPosition.X, StartPoint.Y);
                EndPoint = new Point(EndPoint.X, newPosition.Y);
                break;
            case Corner.BottomRight:
                EndPoint = newPosition;
                break;
        }
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
