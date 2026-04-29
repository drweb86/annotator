using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Threading;
using System;

namespace ScreenshotAnnotator.Controls;

/// <summary>
/// Sitting cat with alert green eyes that track a small animated mouse lurking across the bottom.
/// </summary>
public class IdleCatMouseControl : Control
{
    private DispatcherTimer? _timer;
    private double _rodentX = double.NaN; // screen-coord X; NaN = off-screen / paused
    private double _rodentDir = 1.0;
    private int _pauseFrames;
    private int _legPhase;

    private const double RodentSpeed = 3.0; // screen pixels per tick

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        _rodentDir = Random.Shared.Next(2) == 0 ? 1.0 : -1.0;
        _timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(30) };
        _timer.Tick += OnTick;
        _timer.Start();
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        _timer?.Stop();
        _timer = null;
        base.OnDetachedFromVisualTree(e);
    }

    private void OnTick(object? sender, EventArgs e)
    {
        if (!IsVisible) return;

        double w = Bounds.Width;
        if (w < 1) return;

        if (double.IsNaN(_rodentX))
            _rodentX = _rodentDir > 0 ? -40 : w + 40;

        if (_pauseFrames > 0) { _pauseFrames--; return; }

        _rodentX += _rodentDir * RodentSpeed;
        _legPhase = (_legPhase + 1) % 20;

        if (_rodentX > w + 40 || _rodentX < -40)
        {
            _pauseFrames = 50 + Random.Shared.Next(100);
            _rodentDir = Random.Shared.Next(2) == 0 ? 1.0 : -1.0;
            _rodentX = double.NaN;
        }

        InvalidateVisual();
    }

    public override void Render(DrawingContext context)
    {
        if (!IsVisible) return;
        base.Render(context);

        var bounds = Bounds;
        if (bounds.Width < 1 || bounds.Height < 1) return;

        double cx = bounds.Width / 2;
        double cy = bounds.Height / 2;
        double scale = Math.Clamp(Math.Min(bounds.Width / 300.0, bounds.Height / 350.0), 0.3, 1.5);

        // Rodent screen position; keep it off-screen when paused/uninitialized
        double rx = double.IsNaN(_rodentX)
            ? (_rodentDir > 0 ? -40 : bounds.Width + 40)
            : _rodentX;

        // Vertical bob synchronized with leg cycle
        double bob = Math.Sin(_legPhase * Math.PI / 10.0) * 1.5;

        // Convert rodent screen-coords to cat-local coords for eye tracking & drawing
        double catMx = (rx - cx) / scale;
        double catMy = cy / scale - 12 + bob; // near bottom, slightly bobbing

        using var clip = context.PushClip(new Rect(bounds.Size));
        using var tf = context.PushTransform(Matrix.CreateScale(scale, scale) * Matrix.CreateTranslation(cx, cy));

        DrawCat(context, catMx, catMy);
        DrawRodent(context, catMx, catMy, _rodentDir, _legPhase);
    }

    #region Cat drawing

    private static readonly SolidColorBrush Fur      = new(Color.FromRgb(55, 55, 65));
    private static readonly SolidColorBrush FurDark  = new(Color.FromRgb(40, 40, 50));
    private static readonly SolidColorBrush EyeWhite = new(Color.FromRgb(230, 235, 230));
    private static readonly SolidColorBrush Iris     = new(Color.FromRgb(110, 185, 55));
    private static readonly SolidColorBrush PupilSlit = new(Color.FromRgb(15, 15, 20));
    private static readonly SolidColorBrush NoseBrush = new(Color.FromRgb(170, 110, 120));
    private static readonly SolidColorBrush CatHighlight = new(Color.FromArgb(200, 255, 255, 255));
    private static readonly Pen EyeRimPen  = new(new SolidColorBrush(Color.FromArgb(70, 110, 185, 55)), 2.0);
    private static readonly Pen WhiskerPen = new(new SolidColorBrush(Color.FromRgb(130, 130, 145)), 1.5);
    private static readonly Pen MouthPen  = new(new SolidColorBrush(Color.FromRgb(90, 90, 105)), 1.5);
    private static readonly Pen TailPen   = new(Fur, 12);

    private static readonly Point LeftEyeCenter  = new(-18, -42);
    private static readonly Point RightEyeCenter = new(18, -42);
    private const double EyeR   = 14;
    private const double PupilR = 6;

    private static void DrawCat(DrawingContext ctx, double mx, double my)
    {
        var tail = new StreamGeometry();
        using (var g = tail.Open())
        {
            g.BeginFigure(new Point(45, 40), false);
            g.CubicBezierTo(new Point(100, 20), new Point(120, -40), new Point(90, -75));
        }
        ctx.DrawGeometry(null, TailPen, tail);

        ctx.DrawEllipse(Fur, null, new Point(0, 30), 55, 45);
        ctx.DrawEllipse(Fur, null, new Point(0, -40), 45, 40);

        DrawEar(ctx, -1);
        DrawEar(ctx, 1);

        DrawEye(ctx, LeftEyeCenter, mx, my);
        DrawEye(ctx, RightEyeCenter, mx, my);

        var nose = new StreamGeometry();
        using (var g = nose.Open())
        {
            g.BeginFigure(new Point(0, -26), true);
            g.LineTo(new Point(-5, -20));
            g.LineTo(new Point(5, -20));
            g.EndFigure(true);
        }
        ctx.DrawGeometry(NoseBrush, null, nose);

        DrawMouth(ctx, -1);
        DrawMouth(ctx, 1);

        for (int s = -1; s <= 1; s += 2)
        {
            double wx = 14 * s;
            ctx.DrawLine(WhiskerPen, new Point(wx, -25), new Point(wx + 50 * s, -32));
            ctx.DrawLine(WhiskerPen, new Point(wx, -22), new Point(wx + 50 * s, -22));
            ctx.DrawLine(WhiskerPen, new Point(wx, -19), new Point(wx + 50 * s, -12));
        }

        ctx.DrawEllipse(FurDark, null, new Point(-25, 68), 16, 10);
        ctx.DrawEllipse(FurDark, null, new Point(25, 68), 16, 10);
    }

    private static void DrawEar(DrawingContext ctx, int side)
    {
        double s = side;
        var outer = new StreamGeometry();
        using (var g = outer.Open())
        {
            g.BeginFigure(new Point(s * 15, -72), true);
            g.LineTo(new Point(s * 52, -120));
            g.LineTo(new Point(s * 40, -65));
            g.EndFigure(true);
        }
        ctx.DrawGeometry(Fur, null, outer);

        var inner = new StreamGeometry();
        using (var g = inner.Open())
        {
            g.BeginFigure(new Point(s * 20, -73), true);
            g.LineTo(new Point(s * 48, -112));
            g.LineTo(new Point(s * 38, -68));
            g.EndFigure(true);
        }
        ctx.DrawGeometry(FurDark, null, inner);
    }

    private static void DrawEye(DrawingContext ctx, Point center, double mx, double my)
    {
        ctx.DrawEllipse(EyeWhite, null, center, EyeR, EyeR);
        ctx.DrawEllipse(null, EyeRimPen, center, EyeR + 2, EyeR + 2);

        var pupil = ClampToCircle(center, mx, my, EyeR - PupilR - 1);

        ctx.DrawEllipse(Iris, null, pupil, PupilR, PupilR);
        ctx.DrawEllipse(PupilSlit, null, pupil, 1.8, PupilR - 1);
        ctx.DrawEllipse(CatHighlight, null, new Point(pupil.X - 2, pupil.Y - 2.5), 2, 2);
    }

    private static void DrawMouth(DrawingContext ctx, int side)
    {
        double s = side;
        var mouth = new StreamGeometry();
        using (var g = mouth.Open())
        {
            g.BeginFigure(new Point(0, -20), false);
            g.CubicBezierTo(
                new Point(s * 3, -16),
                new Point(s * 9, -14),
                new Point(s * 11, -17));
        }
        ctx.DrawGeometry(null, MouthPen, mouth);
    }

    #endregion

    #region Rodent drawing

    private static readonly SolidColorBrush RodentFur      = new(Color.FromRgb(110, 100, 90));
    private static readonly SolidColorBrush RodentFurDark  = new(Color.FromRgb(80, 72, 65));
    private static readonly SolidColorBrush RodentEarInner = new(Color.FromArgb(110, 200, 150, 150));
    private static readonly SolidColorBrush RodentEyeDot   = new(Color.FromRgb(20, 20, 20));
    private static readonly SolidColorBrush RodentNose     = new(Color.FromRgb(195, 110, 115));
    private static readonly Pen RodentTailPen    = new(new SolidColorBrush(Color.FromRgb(90, 82, 74)), 2.5);
    private static readonly Pen RodentWhiskerPen = new(new SolidColorBrush(Color.FromArgb(140, 215, 210, 205)), 1.0);
    private static readonly Pen LegPen          = new(RodentFurDark, 2.5);

    private static void DrawRodent(DrawingContext ctx, double cx, double cy, double dir, int legPhase)
    {
        bool phaseA = legPhase < 10;
        double legYA = cy + (phaseA ? 11 : 7);
        double legYB = cy + (phaseA ? 7 : 11);

        // Legs drawn before body so they appear behind it
        ctx.DrawLine(LegPen, new Point(cx + dir * 12, cy + 5), new Point(cx + dir * 14, legYA));
        ctx.DrawLine(LegPen, new Point(cx + dir * 4,  cy + 5), new Point(cx + dir * 5,  legYB));
        ctx.DrawLine(LegPen, new Point(cx - dir * 4,  cy + 5), new Point(cx - dir * 5,  legYA));
        ctx.DrawLine(LegPen, new Point(cx - dir * 12, cy + 5), new Point(cx - dir * 13, legYB));

        // Body
        ctx.DrawEllipse(RodentFur, null, new Point(cx, cy), 18, 9);

        double headX = cx + dir * 20;
        double headY = cy - 2;

        // Head
        ctx.DrawEllipse(RodentFur, null, new Point(headX, headY), 10, 9);

        // Ears
        ctx.DrawEllipse(RodentFurDark, null, new Point(headX - 3, headY - 10), 4, 5);
        ctx.DrawEllipse(RodentEarInner, null, new Point(headX - 3, headY - 10), 2, 3);
        ctx.DrawEllipse(RodentFurDark, null, new Point(headX + 3, headY - 10), 4, 5);
        ctx.DrawEllipse(RodentEarInner, null, new Point(headX + 3, headY - 10), 2, 3);

        // Eye
        ctx.DrawEllipse(RodentEyeDot, null, new Point(headX + dir * 4, headY - 2), 2.5, 2.5);

        // Nose
        ctx.DrawEllipse(RodentNose, null, new Point(headX + dir * 9, headY + 1), 2, 2);

        // Whiskers
        ctx.DrawLine(RodentWhiskerPen, new Point(headX + dir * 6, headY - 1), new Point(headX + dir * 16, headY - 4));
        ctx.DrawLine(RodentWhiskerPen, new Point(headX + dir * 6, headY + 2), new Point(headX + dir * 16, headY + 4));

        // Tail
        double tailX = cx - dir * 16;
        var tail = new StreamGeometry();
        using (var g = tail.Open())
        {
            g.BeginFigure(new Point(tailX, cy), false);
            g.CubicBezierTo(
                new Point(tailX - dir * 8,  cy - 10),
                new Point(tailX - dir * 16, cy - 5),
                new Point(tailX - dir * 22, cy - 14));
        }
        ctx.DrawGeometry(null, RodentTailPen, tail);
    }

    #endregion

    private static Point ClampToCircle(Point center, double tx, double ty, double maxR)
    {
        double dx = tx - center.X;
        double dy = ty - center.Y;
        double dist = Math.Sqrt(dx * dx + dy * dy);
        if (dist < 0.5) return center;
        double r = Math.Min(dist, maxR);
        return new Point(center.X + dx / dist * r, center.Y + dy / dist * r);
    }
}
