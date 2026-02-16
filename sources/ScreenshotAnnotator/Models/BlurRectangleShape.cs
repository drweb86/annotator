using System;
using Avalonia;
using Avalonia.Media;
using Avalonia.Media.Imaging;

namespace ScreenshotAnnotator.Models;

public class BlurRectangleShape : AnnotationShape
{
    public Rect Rectangle { get; set; }
    public Bitmap? BlurredImage { get; set; }
    public Func<Rect, Bitmap?>? RefreshBlur { get; set; }

    public override void Render(DrawingContext context)
    {
        if (BlurredImage != null)
        {
            // Draw the blurred image
            context.DrawImage(BlurredImage, Rectangle);
        }
        else
        {
            // Fallback: draw semi-transparent gray rectangle
            var fallbackBrush = new SolidColorBrush(Colors.Gray) { Opacity = 0.7 };
            context.DrawRectangle(fallbackBrush, null, Rectangle);
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

    public override Rect GetBounds() => Rectangle;

    public override bool HitTest(Point point)
    {
        return Rectangle.Contains(point);
    }

    public override void Move(Vector offset)
    {
        Rectangle = new Rect(
            Rectangle.X + offset.X,
            Rectangle.Y + offset.Y,
            Rectangle.Width,
            Rectangle.Height
        );

        // Regenerate blur for new position
        if (RefreshBlur != null)
        {
            BlurredImage = RefreshBlur(Rectangle);
        }
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

        // Regenerate blur for new size/position
        if (RefreshBlur != null)
        {
            BlurredImage = RefreshBlur(Rectangle);
        }
    }

    private void DrawHandle(DrawingContext context, Point center, double size, IBrush brush)
    {
        var rect = new Rect(center.X - size / 2, center.Y - size / 2, size, size);
        context.DrawRectangle(brush, new Pen(Brushes.White, 1), rect);
    }
}
