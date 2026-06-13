using Avalonia;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using ScreenshotAnnotator.Interop.Shapes;
using ScreenshotAnnotator.Models;
using System;

namespace ScreenshotAnnotator.Shapes.BlurRectangle;

public sealed class BlurRectangleShape : AnnotationShape, ICornerResizableShape
{
    public Rect Rectangle { get; set; }
    public Bitmap? BlurredImage { get; set; }
    public Func<Rect, Bitmap?>? RefreshBlur { get; set; }

    public override void Render(DrawingContext context)
    {
        if (BlurredImage != null)
        {
            context.DrawImage(BlurredImage, Rectangle);
        }
        else
        {
            var fallbackBrush = new SolidColorBrush(Colors.Gray) { Opacity = 0.7 };
            context.DrawRectangle(fallbackBrush, null, Rectangle);
        }

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

    public override Rect GetBounds() => Rectangle;

    public override bool HitTest(Point point) => Rectangle.Contains(point);

    public override void Move(Vector offset)
    {
        Rectangle = new Rect(
            Rectangle.X + offset.X,
            Rectangle.Y + offset.Y,
            Rectangle.Width,
            Rectangle.Height);

        if (RefreshBlur != null)
            BlurredImage = RefreshBlur(Rectangle);
    }

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

        if (RefreshBlur != null)
            BlurredImage = RefreshBlur(Rectangle);
    }

    private static bool IsPointNearHandle(Point point, Point handleCenter, double handleSize) =>
        Math.Sqrt(Math.Pow(point.X - handleCenter.X, 2) + Math.Pow(point.Y - handleCenter.Y, 2)) < handleSize;

    private static void DrawHandle(DrawingContext context, Point center, double size, IBrush brush, IPen pen)
    {
        var rect = new Rect(center.X - size / 2, center.Y - size / 2, size, size);
        context.DrawRectangle(brush, pen, rect);
    }
}
