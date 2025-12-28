using Avalonia;
using Avalonia.Media;
using System;

namespace ScreenshotAnnotator.Models;

public enum ToolType
{
    None,
    Arrow,
    Callout,
    Trim
}

public abstract class AnnotationShape
{
    public Color StrokeColor { get; set; } = Colors.Red;
    public double StrokeThickness { get; set; } = 2.0;
    public bool IsSelected { get; set; }

    public abstract void Render(DrawingContext context);
    public abstract bool HitTest(Point point);
    public abstract void Move(Vector offset);
}

public class ArrowShape : AnnotationShape
{
    public Point StartPoint { get; set; }
    public Point EndPoint { get; set; }

    public override void Render(DrawingContext context)
    {
        var color = IsSelected ? Colors.Blue : StrokeColor;
        var thickness = IsSelected ? StrokeThickness + 1 : StrokeThickness;
        var pen = new Pen(new SolidColorBrush(color), thickness);

        // Draw main line
        context.DrawLine(pen, StartPoint, EndPoint);

        // Draw arrowhead
        var angle = Math.Atan2(EndPoint.Y - StartPoint.Y, EndPoint.X - StartPoint.X);
        var arrowLength = 15;
        var arrowAngle = Math.PI / 6; // 30 degrees

        var point1 = new Point(
            EndPoint.X - arrowLength * Math.Cos(angle - arrowAngle),
            EndPoint.Y - arrowLength * Math.Sin(angle - arrowAngle)
        );

        var point2 = new Point(
            EndPoint.X - arrowLength * Math.Cos(angle + arrowAngle),
            EndPoint.Y - arrowLength * Math.Sin(angle + arrowAngle)
        );

        context.DrawLine(pen, EndPoint, point1);
        context.DrawLine(pen, EndPoint, point2);

        // Draw selection handles if selected
        if (IsSelected)
        {
            var handleSize = 6;
            var handleBrush = Brushes.Blue;
            DrawHandle(context, StartPoint, handleSize, handleBrush);
            DrawHandle(context, EndPoint, handleSize, handleBrush);
        }
    }

    public override bool HitTest(Point point)
    {
        // Check if point is near the line
        var lineStart = StartPoint;
        var lineEnd = EndPoint;

        var distance = DistanceFromPointToLine(point, lineStart, lineEnd);
        return distance < 10; // Hit tolerance
    }

    public override void Move(Vector offset)
    {
        StartPoint = new Point(StartPoint.X + offset.X, StartPoint.Y + offset.Y);
        EndPoint = new Point(EndPoint.X + offset.X, EndPoint.Y + offset.Y);
    }

    public bool IsPointOnStartHandle(Point point)
    {
        var distance = Math.Sqrt(Math.Pow(point.X - StartPoint.X, 2) + Math.Pow(point.Y - StartPoint.Y, 2));
        return distance < 8;
    }

    public bool IsPointOnEndHandle(Point point)
    {
        var distance = Math.Sqrt(Math.Pow(point.X - EndPoint.X, 2) + Math.Pow(point.Y - EndPoint.Y, 2));
        return distance < 8;
    }

    public void MoveStartPoint(Point newPosition)
    {
        StartPoint = newPosition;
    }

    public void MoveEndPoint(Point newPosition)
    {
        EndPoint = newPosition;
    }

    private double DistanceFromPointToLine(Point point, Point lineStart, Point lineEnd)
    {
        var dx = lineEnd.X - lineStart.X;
        var dy = lineEnd.Y - lineStart.Y;
        var lengthSquared = dx * dx + dy * dy;

        if (lengthSquared == 0)
            return Math.Sqrt(Math.Pow(point.X - lineStart.X, 2) + Math.Pow(point.Y - lineStart.Y, 2));

        var t = Math.Max(0, Math.Min(1, ((point.X - lineStart.X) * dx + (point.Y - lineStart.Y) * dy) / lengthSquared));
        var projectionX = lineStart.X + t * dx;
        var projectionY = lineStart.Y + t * dy;

        return Math.Sqrt(Math.Pow(point.X - projectionX, 2) + Math.Pow(point.Y - projectionY, 2));
    }

    private void DrawHandle(DrawingContext context, Point center, double size, IBrush brush)
    {
        var rect = new Rect(center.X - size / 2, center.Y - size / 2, size, size);
        context.DrawRectangle(brush, new Pen(Brushes.White, 1), rect);
    }
}

public class CalloutShape : AnnotationShape
{
    public Rect Rectangle { get; set; }
    public Point BeakPoint { get; set; }
    public string Text { get; set; } = "";

    public override void Render(DrawingContext context)
    {
        var color = IsSelected ? Colors.Blue : StrokeColor;
        var thickness = IsSelected ? StrokeThickness + 1 : StrokeThickness;
        var pen = new Pen(new SolidColorBrush(color), thickness);
        var brush = new SolidColorBrush(Colors.White);

        // Create path for callout with beak
        var geometry = new PathGeometry();
        var figure = new PathFigure { StartPoint = Rectangle.TopLeft, IsClosed = true };

        // Determine which side the beak is on
        var rectCenter = Rectangle.Center;
        var beakSide = GetBeakSide();

        var segments = figure.Segments;

        switch (beakSide)
        {
            case BeakSide.Bottom:
                segments!.Add(new LineSegment { Point = Rectangle.TopRight });
                segments.Add(new LineSegment { Point = Rectangle.BottomRight });

                // Add beak on bottom
                var beakX = Math.Max(Rectangle.Left, Math.Min(Rectangle.Right, BeakPoint.X));
                segments.Add(new LineSegment { Point = new Point(beakX + 10, Rectangle.Bottom) });
                segments.Add(new LineSegment { Point = BeakPoint });
                segments.Add(new LineSegment { Point = new Point(beakX - 10, Rectangle.Bottom) });

                segments.Add(new LineSegment { Point = Rectangle.BottomLeft });
                break;

            case BeakSide.Top:
                var topBeakX = Math.Max(Rectangle.Left, Math.Min(Rectangle.Right, BeakPoint.X));
                segments!.Add(new LineSegment { Point = new Point(topBeakX - 10, Rectangle.Top) });
                segments.Add(new LineSegment { Point = BeakPoint });
                segments.Add(new LineSegment { Point = new Point(topBeakX + 10, Rectangle.Top) });

                segments.Add(new LineSegment { Point = Rectangle.TopRight });
                segments.Add(new LineSegment { Point = Rectangle.BottomRight });
                segments.Add(new LineSegment { Point = Rectangle.BottomLeft });
                break;

            case BeakSide.Left:
                segments!.Add(new LineSegment { Point = Rectangle.TopRight });
                segments.Add(new LineSegment { Point = Rectangle.BottomRight });
                segments.Add(new LineSegment { Point = Rectangle.BottomLeft });

                var leftBeakY = Math.Max(Rectangle.Top, Math.Min(Rectangle.Bottom, BeakPoint.Y));
                segments.Add(new LineSegment { Point = new Point(Rectangle.Left, leftBeakY + 10) });
                segments.Add(new LineSegment { Point = BeakPoint });
                segments.Add(new LineSegment { Point = new Point(Rectangle.Left, leftBeakY - 10) });
                break;

            case BeakSide.Right:
                segments!.Add(new LineSegment { Point = Rectangle.TopRight });

                var rightBeakY = Math.Max(Rectangle.Top, Math.Min(Rectangle.Bottom, BeakPoint.Y));
                segments.Add(new LineSegment { Point = new Point(Rectangle.Right, rightBeakY - 10) });
                segments.Add(new LineSegment { Point = BeakPoint });
                segments.Add(new LineSegment { Point = new Point(Rectangle.Right, rightBeakY + 10) });

                segments.Add(new LineSegment { Point = Rectangle.BottomRight });
                segments.Add(new LineSegment { Point = Rectangle.BottomLeft });
                break;
        }

        geometry.Figures!.Add(figure);

        // Draw the callout
        context.DrawGeometry(brush, pen, geometry);

        // Draw text if any
        if (!string.IsNullOrWhiteSpace(Text))
        {
            var formattedText = new FormattedText(
                Text,
                System.Globalization.CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                new Typeface("Arial"),
                14,
                Brushes.Black
            );

            var textPoint = new Point(
                Rectangle.Left + 5,
                Rectangle.Top + 5
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

            // Draw beak handle
            var beakHandleSize = 8;
            var beakHandleBrush = Brushes.Orange;
            DrawHandle(context, BeakPoint, beakHandleSize, beakHandleBrush);
        }
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

public class TrimRectangle
{
    public Rect Rectangle { get; set; }

    public void Render(DrawingContext context)
    {
        var pen = new Pen(new SolidColorBrush(Colors.Blue) { Opacity = 0.8 }, 2.0);
        pen.DashStyle = new DashStyle(new[] { 4.0, 4.0 }, 0);

        context.DrawRectangle(null, pen, Rectangle);

        // Draw corner handles
        var handleSize = 8;
        var handleBrush = Brushes.White;
        var handlePen = new Pen(Brushes.Blue, 1);

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
}
