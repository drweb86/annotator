using Avalonia;
using Avalonia.Media;
using System;

namespace ScreenshotAnnotator.Models;

public class ArrowShape : AnnotationShape
{
    public Point StartPoint { get; set; }
    public Point EndPoint { get; set; }

    public override void Render(DrawingContext context)
    {
        var color = StrokeColor;
        var thickness = StrokeThickness;
        var pen = new Pen(new SolidColorBrush(color), thickness)
        {
            LineCap = PenLineCap.Round // Rounded start
        };

        // Shadow parameters
        var shadowOffset = new Vector(3, 3);
        var shadowColor = Color.FromArgb(100, 0, 0, 0); // Semi-transparent black
        var shadowPen = new Pen(new SolidColorBrush(shadowColor), thickness)
        {
            LineCap = PenLineCap.Round
        };
        var shadowBrush = new SolidColorBrush(shadowColor);

        // Draw arrowhead as solid triangle (twice as large)
        var angle = Math.Atan2(EndPoint.Y - StartPoint.Y, EndPoint.X - StartPoint.X);
        var arrowLength = 40; // Twice as large (was 20)
        var arrowWidth = 30;  // Twice as large (was 15)

        // Calculate the three points of the triangle beak
        var perpAngle = angle + Math.PI / 2;
        var halfWidth = arrowWidth / 2;

        // Base left point
        var baseLeft = new Point(
            EndPoint.X - arrowLength * Math.Cos(angle) - halfWidth * Math.Cos(perpAngle),
            EndPoint.Y - arrowLength * Math.Sin(angle) - halfWidth * Math.Sin(perpAngle)
        );

        // Base right point
        var baseRight = new Point(
            EndPoint.X - arrowLength * Math.Cos(angle) + halfWidth * Math.Cos(perpAngle),
            EndPoint.Y - arrowLength * Math.Sin(angle) + halfWidth * Math.Sin(perpAngle)
        );

        // Base center point (where the line should end)
        var baseCenter = new Point(
            EndPoint.X - arrowLength * Math.Cos(angle),
            EndPoint.Y - arrowLength * Math.Sin(angle)
        );

        // Tip point (at the end)
        var tip = EndPoint;

        // Draw shadow for main line (ending at the base of the triangle)
        var shadowStart = new Point(StartPoint.X + shadowOffset.X, StartPoint.Y + shadowOffset.Y);
        var shadowLineEnd = new Point(baseCenter.X + shadowOffset.X, baseCenter.Y + shadowOffset.Y);
        context.DrawLine(shadowPen, shadowStart, shadowLineEnd);

        // Create geometry for the arrow beak triangle shadow
        var shadowBeakGeometry = new StreamGeometry();
        using (var ctx = shadowBeakGeometry.Open())
        {
            ctx.BeginFigure(new Point(baseLeft.X + shadowOffset.X, baseLeft.Y + shadowOffset.Y), true);
            ctx.LineTo(new Point(tip.X + shadowOffset.X, tip.Y + shadowOffset.Y));
            ctx.LineTo(new Point(baseRight.X + shadowOffset.X, baseRight.Y + shadowOffset.Y));
            ctx.EndFigure(true);
        }

        // Draw the shadow beak
        context.DrawGeometry(shadowBrush, null, shadowBeakGeometry);

        // Draw main line (ending at the base of the triangle to avoid overlap)
        context.DrawLine(pen, StartPoint, baseCenter);

        // Create geometry for the arrow beak triangle
        var beakGeometry = new StreamGeometry();
        using (var ctx = beakGeometry.Open())
        {
            ctx.BeginFigure(baseLeft, true);
            ctx.LineTo(tip);
            ctx.LineTo(baseRight);
            ctx.EndFigure(true);
        }

        // Draw the solid triangle beak
        var brush = new SolidColorBrush(color);
        context.DrawGeometry(brush, null, beakGeometry);

        // Draw selection handles if selected
        if (IsSelected)
        {
            var handleSize = 6;
            var handleBrush = Brushes.White;
            var handlePen = new Pen(Brushes.Black, 1);
            DrawHandle(context, StartPoint, handleSize, handleBrush, handlePen);
            DrawHandle(context, EndPoint, handleSize, handleBrush, handlePen);
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

    public override Rect GetBounds()
    {
        const double arrowHeadPadding = 45;
        var minX = Math.Min(StartPoint.X, EndPoint.X) - arrowHeadPadding;
        var minY = Math.Min(StartPoint.Y, EndPoint.Y) - arrowHeadPadding;
        var maxX = Math.Max(StartPoint.X, EndPoint.X) + arrowHeadPadding;
        var maxY = Math.Max(StartPoint.Y, EndPoint.Y) + arrowHeadPadding;
        return new Rect(minX, minY, maxX - minX, maxY - minY);
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

    private void DrawHandle(DrawingContext context, Point center, double size, IBrush brush, IPen pen)
    {
        var rect = new Rect(center.X - size / 2, center.Y - size / 2, size, size);
        context.DrawRectangle(brush, pen, rect);
    }
}
