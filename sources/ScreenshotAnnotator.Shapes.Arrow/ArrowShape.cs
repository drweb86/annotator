using Avalonia;
using Avalonia.Media;
using ScreenshotAnnotator.Interop.Shapes.Common;
using ScreenshotAnnotator.Interop.Shapes;
using ScreenshotAnnotator.Models;
using System;

namespace ScreenshotAnnotator.Shapes.Arrow;

public sealed class ArrowShape : AnnotationShape, IVerticalCutAdjustable, IHorizontalCutAdjustable
{
    public Point StartPoint { get; set; }
    public Point EndPoint { get; set; }

    public override void Render(DrawingContext context)
    {
        var color = StrokeColor;
        var thickness = StrokeThickness;
        var pen = new Pen(new SolidColorBrush(color), thickness) { LineCap = PenLineCap.Round };

        var shadowOffset = new Vector(3, 3);
        var shadowColor = Color.FromArgb(100, 0, 0, 0);
        var shadowPen = new Pen(new SolidColorBrush(shadowColor), thickness) { LineCap = PenLineCap.Round };
        var shadowBrush = new SolidColorBrush(shadowColor);

        var angle = Math.Atan2(EndPoint.Y - StartPoint.Y, EndPoint.X - StartPoint.X);
        var arrowLength = 40;
        var arrowWidth = 30;
        var perpAngle = angle + Math.PI / 2;
        var halfWidth = arrowWidth / 2;

        var baseLeft = new Point(
            EndPoint.X - arrowLength * Math.Cos(angle) - halfWidth * Math.Cos(perpAngle),
            EndPoint.Y - arrowLength * Math.Sin(angle) - halfWidth * Math.Sin(perpAngle));
        var baseRight = new Point(
            EndPoint.X - arrowLength * Math.Cos(angle) + halfWidth * Math.Cos(perpAngle),
            EndPoint.Y - arrowLength * Math.Sin(angle) + halfWidth * Math.Sin(perpAngle));
        var baseCenter = new Point(
            EndPoint.X - arrowLength * Math.Cos(angle),
            EndPoint.Y - arrowLength * Math.Sin(angle));
        var tip = EndPoint;

        var shadowStart = new Point(StartPoint.X + shadowOffset.X, StartPoint.Y + shadowOffset.Y);
        var shadowLineEnd = new Point(baseCenter.X + shadowOffset.X, baseCenter.Y + shadowOffset.Y);
        context.DrawLine(shadowPen, shadowStart, shadowLineEnd);

        var shadowBeakGeometry = new StreamGeometry();
        using (var ctx = shadowBeakGeometry.Open())
        {
            ctx.BeginFigure(new Point(baseLeft.X + shadowOffset.X, baseLeft.Y + shadowOffset.Y), true);
            ctx.LineTo(new Point(tip.X + shadowOffset.X, tip.Y + shadowOffset.Y));
            ctx.LineTo(new Point(baseRight.X + shadowOffset.X, baseRight.Y + shadowOffset.Y));
            ctx.EndFigure(true);
        }
        context.DrawGeometry(shadowBrush, null, shadowBeakGeometry);
        context.DrawLine(pen, StartPoint, baseCenter);

        var beakGeometry = new StreamGeometry();
        using (var ctx = beakGeometry.Open())
        {
            ctx.BeginFigure(baseLeft, true);
            ctx.LineTo(tip);
            ctx.LineTo(baseRight);
            ctx.EndFigure(true);
        }
        context.DrawGeometry(new SolidColorBrush(color), null, beakGeometry);

        if (IsSelected)
        {
            const double handleSize = 6;
            DrawHandle(context, StartPoint, handleSize);
            DrawHandle(context, EndPoint, handleSize);
        }
    }

    public override bool HitTest(Point point) => DistanceFromPointToLine(point, StartPoint, EndPoint) < 10;

    public override void Move(Vector offset)
    {
        StartPoint += offset;
        EndPoint += offset;
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

    public bool IsPointOnStartHandle(Point point) => Distance(point, StartPoint) < 8;
    public bool IsPointOnEndHandle(Point point) => Distance(point, EndPoint) < 8;
    public void MoveStartPoint(Point newPosition) => StartPoint = newPosition;
    public void MoveEndPoint(Point newPosition) => EndPoint = newPosition;

    public void AdjustForVerticalCut(double cutX, double cutWidth)
    {
        StartPoint = ShapeCutHelpers.AdjustPointForVerticalCut(StartPoint, cutX, cutWidth);
        EndPoint = ShapeCutHelpers.AdjustPointForVerticalCut(EndPoint, cutX, cutWidth);
    }

    public void AdjustForHorizontalCut(double cutY, double cutHeight)
    {
        StartPoint = ShapeCutHelpers.AdjustPointForHorizontalCut(StartPoint, cutY, cutHeight);
        EndPoint = ShapeCutHelpers.AdjustPointForHorizontalCut(EndPoint, cutY, cutHeight);
    }

    private static double Distance(Point a, Point b) => Math.Sqrt(Math.Pow(a.X - b.X, 2) + Math.Pow(a.Y - b.Y, 2));

    private static double DistanceFromPointToLine(Point point, Point lineStart, Point lineEnd)
    {
        var dx = lineEnd.X - lineStart.X;
        var dy = lineEnd.Y - lineStart.Y;
        var lengthSquared = dx * dx + dy * dy;
        if (lengthSquared == 0)
            return Distance(point, lineStart);

        var t = Math.Max(0, Math.Min(1, ((point.X - lineStart.X) * dx + (point.Y - lineStart.Y) * dy) / lengthSquared));
        var projectionX = lineStart.X + t * dx;
        var projectionY = lineStart.Y + t * dy;
        return Distance(point, new Point(projectionX, projectionY));
    }

    private static void DrawHandle(DrawingContext context, Point center, double size)
    {
        var rect = new Rect(center.X - size / 2, center.Y - size / 2, size, size);
        context.DrawRectangle(Brushes.White, new Pen(Brushes.Black, 1), rect);
    }
}
