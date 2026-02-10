using Avalonia;
using Avalonia.Media;
using System;

namespace ScreenshotAnnotator.Models;

public class CalloutShape : AnnotationShape
{
    public Rect Rectangle { get; set; }
    public Point BeakPoint { get; set; }
    public string Text { get; set; } = "";

    /// <summary>Font family name for the callout text. Default "Arial" when not set (existing projects).</summary>
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
        var shadowColor = Color.FromArgb(100, 0, 0, 0); // Semi-transparent black
        var shadowBrush = new SolidColorBrush(shadowColor);

        // Create rounded rectangle geometry
        var cornerRadius = 10.0;

        // Calculate arrow geometry first
        var rectCenter = Rectangle.Center;
        Point arrowStart = GetEdgePoint(rectCenter, BeakPoint);
        Point arrowEnd = BeakPoint;

        var angle = Math.Atan2(arrowEnd.Y - arrowStart.Y, arrowEnd.X - arrowStart.X);
        var arrowLength = 40; // Twice as large (was 20)
        var arrowWidth = 30;  // Twice as large (was 15)

        var perpAngle = angle + Math.PI / 2;
        var halfWidth = arrowWidth / 2;

        var baseLeft = new Point(
            arrowEnd.X - arrowLength * Math.Cos(angle) - halfWidth * Math.Cos(perpAngle),
            arrowEnd.Y - arrowLength * Math.Sin(angle) - halfWidth * Math.Sin(perpAngle)
        );

        var baseRight = new Point(
            arrowEnd.X - arrowLength * Math.Cos(angle) + halfWidth * Math.Cos(perpAngle),
            arrowEnd.Y - arrowLength * Math.Sin(angle) + halfWidth * Math.Sin(perpAngle)
        );

        var baseCenter = new Point(
            arrowEnd.X - arrowLength * Math.Cos(angle),
            arrowEnd.Y - arrowLength * Math.Sin(angle)
        );

        // Draw all shadows first (so they appear behind everything)

        // Shadow for rounded rectangle
        var shadowRect = new Rect(
            Rectangle.X + shadowOffset.X,
            Rectangle.Y + shadowOffset.Y,
            Rectangle.Width,
            Rectangle.Height
        );
        context.DrawRectangle(shadowBrush, null, shadowRect, cornerRadius, cornerRadius);

        // Shadow for arrow line (only if it's outside the rectangle)
        var shadowPen = new Pen(shadowBrush, thickness)
        {
            LineCap = PenLineCap.Round
        };
        var shadowArrowStart = new Point(arrowStart.X + shadowOffset.X, arrowStart.Y + shadowOffset.Y);
        var shadowArrowBaseCenter = new Point(baseCenter.X + shadowOffset.X, baseCenter.Y + shadowOffset.Y);

        // Only draw arrow shadow outside rectangle bounds
        if (!Rectangle.Contains(shadowArrowStart) || !Rectangle.Contains(shadowArrowBaseCenter))
        {
            context.DrawLine(shadowPen, shadowArrowStart, shadowArrowBaseCenter);
        }

        // Shadow for arrow head
        var shadowArrowGeometry = new StreamGeometry();
        using (var ctx = shadowArrowGeometry.Open())
        {
            ctx.BeginFigure(new Point(baseLeft.X + shadowOffset.X, baseLeft.Y + shadowOffset.Y), true);
            ctx.LineTo(new Point(arrowEnd.X + shadowOffset.X, arrowEnd.Y + shadowOffset.Y));
            ctx.LineTo(new Point(baseRight.X + shadowOffset.X, baseRight.Y + shadowOffset.Y));
            ctx.EndFigure(true);
        }
        context.DrawGeometry(shadowBrush, null, shadowArrowGeometry);

        // Draw filled rounded rectangle (on top of shadows)
        context.DrawRectangle(fillBrush, pen, Rectangle, cornerRadius, cornerRadius);

        // Draw arrow line
        var arrowPen = new Pen(fillBrush, thickness)
        {
            LineCap = PenLineCap.Round
        };
        context.DrawLine(arrowPen, arrowStart, baseCenter);

        // Draw arrow head triangle
        var arrowGeometry = new StreamGeometry();
        using (var ctx = arrowGeometry.Open())
        {
            ctx.BeginFigure(baseLeft, true);
            ctx.LineTo(arrowEnd);
            ctx.LineTo(baseRight);
            ctx.EndFigure(true);
        }
        context.DrawGeometry(fillBrush, null, arrowGeometry);

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

            // Draw arrow handle
            var arrowHandleSize = 8;
            var arrowHandleBrush = Brushes.Orange;
            DrawHandle(context, BeakPoint, arrowHandleSize, arrowHandleBrush);
        }
    }

    private Point GetEdgePoint(Point center, Point target)
    {
        // Find intersection of line from center to target with rectangle edge
        var dx = target.X - center.X;
        var dy = target.Y - center.Y;

        if (dx == 0 && dy == 0)
            return center;

        // Calculate intersections with all four edges
        double t = double.MaxValue;

        // Top edge
        if (dy < 0)
        {
            var tTop = (Rectangle.Top - center.Y) / dy;
            if (tTop > 0) t = Math.Min(t, tTop);
        }
        // Bottom edge
        if (dy > 0)
        {
            var tBottom = (Rectangle.Bottom - center.Y) / dy;
            if (tBottom > 0) t = Math.Min(t, tBottom);
        }
        // Left edge
        if (dx < 0)
        {
            var tLeft = (Rectangle.Left - center.X) / dx;
            if (tLeft > 0) t = Math.Min(t, tLeft);
        }
        // Right edge
        if (dx > 0)
        {
            var tRight = (Rectangle.Right - center.X) / dx;
            if (tRight > 0) t = Math.Min(t, tRight);
        }

        return new Point(center.X + t * dx, center.Y + t * dy);
    }

    public bool IsPointOnBeak(Point point)
    {
        var distance = Math.Sqrt(Math.Pow(point.X - BeakPoint.X, 2) + Math.Pow(point.Y - BeakPoint.Y, 2));
        return distance < 10;
    }

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
        BeakPoint = new Point(BeakPoint.X + offset.X, BeakPoint.Y + offset.Y);
    }

    public void MoveBeak(Point newPosition)
    {
        BeakPoint = newPosition;
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

    private void DrawHandle(DrawingContext context, Point center, double size, IBrush brush)
    {
        var rect = new Rect(center.X - size / 2, center.Y - size / 2, size, size);
        context.DrawRectangle(brush, new Pen(Brushes.White, 1), rect);
    }

    private BeakSide GetBeakSide()
    {
        var rectCenter = Rectangle.Center;
        var dx = BeakPoint.X - rectCenter.X;
        var dy = BeakPoint.Y - rectCenter.Y;

        if (Math.Abs(dx) > Math.Abs(dy))
        {
            return dx > 0 ? BeakSide.Right : BeakSide.Left;
        }
        else
        {
            return dy > 0 ? BeakSide.Bottom : BeakSide.Top;
        }
    }

    private enum BeakSide
    {
        Top, Bottom, Left, Right
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
