using Avalonia;
using Avalonia.Media;
using ScreenshotAnnotator.Interop.Shapes;
using ScreenshotAnnotator.Models;
using System;

namespace ScreenshotAnnotator.Shapes.Highlighter;

public sealed class HighlighterShape : AnnotationShape, IFillColorShape, IHighlighterCornerShape
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
            var pen = new Pen(Brushes.Orange, 1.0) { DashStyle = new DashStyle(new[] { 4.0, 4.0 }, 0) };
            context.DrawRectangle(fillBrush, pen, rect);

            const double handleSize = 8;
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

    public override bool HitTest(Point point) => new Rect(StartPoint, EndPoint).Contains(point);

    public override void Move(Vector offset)
    {
        StartPoint += offset;
        EndPoint += offset;
    }

    public override Rect GetBounds() => new(StartPoint, EndPoint);

    public RectCorner? GetCornerAtPoint(Point point, double handleSize = 8)
    {
        var rect = new Rect(StartPoint, EndPoint);

        if (Math.Abs(point.X - rect.TopLeft.X) < handleSize && Math.Abs(point.Y - rect.TopLeft.Y) < handleSize)
            return RectCorner.TopLeft;
        if (Math.Abs(point.X - rect.TopRight.X) < handleSize && Math.Abs(point.Y - rect.TopRight.Y) < handleSize)
            return RectCorner.TopRight;
        if (Math.Abs(point.X - rect.BottomLeft.X) < handleSize && Math.Abs(point.Y - rect.BottomLeft.Y) < handleSize)
            return RectCorner.BottomLeft;
        if (Math.Abs(point.X - rect.BottomRight.X) < handleSize && Math.Abs(point.Y - rect.BottomRight.Y) < handleSize)
            return RectCorner.BottomRight;

        return null;
    }

    public void ResizeCorner(RectCorner corner, Point newPosition)
    {
        switch (corner)
        {
            case RectCorner.TopLeft:
                StartPoint = newPosition;
                break;
            case RectCorner.TopRight:
                StartPoint = new Point(StartPoint.X, newPosition.Y);
                EndPoint = new Point(newPosition.X, EndPoint.Y);
                break;
            case RectCorner.BottomLeft:
                StartPoint = new Point(newPosition.X, StartPoint.Y);
                EndPoint = new Point(EndPoint.X, newPosition.Y);
                break;
            case RectCorner.BottomRight:
                EndPoint = newPosition;
                break;
        }
    }

    private static void DrawHandle(DrawingContext context, Point center, double size, IBrush brush, IPen pen)
    {
        var rect = new Rect(center.X - size / 2, center.Y - size / 2, size, size);
        context.DrawRectangle(brush, pen, rect);
    }
}
