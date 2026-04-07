using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using System;

namespace ScreenshotAnnotator.Controls;

/// <summary>
/// Draws a sitting cat with glowing X-ray eyes that track the mouse cursor.
/// Intended as an idle/empty-state decoration when no project is open.
/// </summary>
public class IdleCatControl : Control
{
    private Point _mouseInCat;

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        var topLevel = TopLevel.GetTopLevel(this);
        if (topLevel != null)
            topLevel.PointerMoved += OnTopLevelPointerMoved;
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        var topLevel = TopLevel.GetTopLevel(this);
        if (topLevel != null)
            topLevel.PointerMoved -= OnTopLevelPointerMoved;
        base.OnDetachedFromVisualTree(e);
    }

    private void OnTopLevelPointerMoved(object? sender, PointerEventArgs e)
    {
        if (!IsVisible) return;

        var pos = e.GetPosition(this);
        var bounds = Bounds;
        if (bounds.Width < 1 || bounds.Height < 1) return;

        var scale = Math.Clamp(Math.Min(bounds.Width / 300.0, bounds.Height / 350.0), 0.3, 1.5);
        _mouseInCat = new Point(
            (pos.X - bounds.Width / 2) / scale,
            (pos.Y - bounds.Height / 2) / scale);

        InvalidateVisual();
    }

    public override void Render(DrawingContext context)
    {
        if (!IsVisible) return;
        base.Render(context);

        var bounds = Bounds;
        if (bounds.Width < 1 || bounds.Height < 1) return;

        var cx = bounds.Width / 2;
        var cy = bounds.Height / 2;
        var scale = Math.Clamp(Math.Min(bounds.Width / 300.0, bounds.Height / 350.0), 0.3, 1.5);

        using var _ = context.PushTransform(
            Matrix.CreateScale(scale, scale) * Matrix.CreateTranslation(cx, cy));

        DrawXRayBeams(context, _mouseInCat.X, _mouseInCat.Y);
        DrawCat(context, _mouseInCat.X, _mouseInCat.Y);
    }

    #region Drawing

    private static readonly SolidColorBrush Fur       = new(Color.FromRgb(55, 55, 65));
    private static readonly SolidColorBrush FurDark    = new(Color.FromRgb(40, 40, 50));
    private static readonly SolidColorBrush EyeWhite   = new(Color.FromRgb(230, 235, 230));
    private static readonly SolidColorBrush XRay       = new(Color.FromRgb(0, 240, 200));
    private static readonly SolidColorBrush XRayGlow1  = new(Color.FromArgb(40, 0, 240, 200));
    private static readonly SolidColorBrush XRayGlow2  = new(Color.FromArgb(80, 0, 240, 200));
    private static readonly Pen             XRayRing   = new(new SolidColorBrush(Color.FromArgb(100, 0, 240, 200)), 2.5);
    private static readonly SolidColorBrush SlitBrush  = new(Color.FromRgb(15, 15, 20));
    private static readonly SolidColorBrush NoseBrush  = new(Color.FromRgb(170, 110, 120));
    private static readonly SolidColorBrush Highlight  = new(Color.FromArgb(200, 255, 255, 255));
    private static readonly Pen             WhiskerPen = new(new SolidColorBrush(Color.FromRgb(130, 130, 145)), 1.5);
    private static readonly Pen             MouthPen   = new(new SolidColorBrush(Color.FromRgb(90, 90, 105)), 1.5);
    private static readonly Pen             TailPen    = new(Fur, 12);

    private static readonly Point LeftEyeCenter  = new(-18, -42);
    private static readonly Point RightEyeCenter = new(18, -42);
    private const double EyeR   = 14;
    private const double PupilR = 6;

    private static void DrawCat(DrawingContext ctx, double mx, double my)
    {
        // Tail
        var tail = new StreamGeometry();
        using (var g = tail.Open())
        {
            g.BeginFigure(new Point(45, 40), false);
            g.CubicBezierTo(new Point(100, 20), new Point(120, -40), new Point(90, -75));
        }
        ctx.DrawGeometry(null, TailPen, tail);

        // Body
        ctx.DrawEllipse(Fur, null, new Point(0, 30), 55, 45);

        // Head
        ctx.DrawEllipse(Fur, null, new Point(0, -40), 45, 40);

        // Ears
        DrawEar(ctx, -1);
        DrawEar(ctx, 1);

        // Eyes
        DrawEye(ctx, LeftEyeCenter, mx, my);
        DrawEye(ctx, RightEyeCenter, mx, my);

        // Nose
        var nose = new StreamGeometry();
        using (var g = nose.Open())
        {
            g.BeginFigure(new Point(0, -26), true);
            g.LineTo(new Point(-5, -20));
            g.LineTo(new Point(5, -20));
            g.EndFigure(true);
        }
        ctx.DrawGeometry(NoseBrush, null, nose);

        // Mouth
        DrawMouth(ctx, -1);
        DrawMouth(ctx, 1);

        // Whiskers
        for (int s = -1; s <= 1; s += 2)
        {
            double wx = 14 * s;
            ctx.DrawLine(WhiskerPen, new Point(wx, -25), new Point(wx + 50 * s, -32));
            ctx.DrawLine(WhiskerPen, new Point(wx, -22), new Point(wx + 50 * s, -22));
            ctx.DrawLine(WhiskerPen, new Point(wx, -19), new Point(wx + 50 * s, -12));
        }

        // Paws
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
        // White
        ctx.DrawEllipse(EyeWhite, null, center, EyeR, EyeR);

        // Glow ring
        ctx.DrawEllipse(null, XRayRing, center, EyeR + 3, EyeR + 3);

        // Pupil tracks mouse
        var pupil = ClampToCircle(center, mx, my, EyeR - PupilR - 1);

        // Glow layers
        ctx.DrawEllipse(XRayGlow1, null, pupil, PupilR + 6, PupilR + 6);
        ctx.DrawEllipse(XRayGlow2, null, pupil, PupilR + 3, PupilR + 3);

        // Iris
        ctx.DrawEllipse(XRay, null, pupil, PupilR, PupilR);

        // Vertical slit
        ctx.DrawEllipse(SlitBrush, null, pupil, 1.8, PupilR - 1);

        // Specular highlight
        ctx.DrawEllipse(Highlight, null, new Point(pupil.X - 2, pupil.Y - 2.5), 2, 2);
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

    private static readonly Pen[] BeamPens =
    {
        new(new SolidColorBrush(Color.FromArgb(28, 0, 240, 200)), 3.0),
        new(new SolidColorBrush(Color.FromArgb(16, 0, 240, 200)), 2.0),
        new(new SolidColorBrush(Color.FromArgb(6, 0, 240, 200)),  1.2),
    };

    /// <summary>Fading X-ray beams from each eye toward the mouse cursor.</summary>
    private static void DrawXRayBeams(DrawingContext ctx, double mx, double my)
    {
        DrawSingleBeam(ctx, LeftEyeCenter, mx, my);
        DrawSingleBeam(ctx, RightEyeCenter, mx, my);
    }

    private static void DrawSingleBeam(DrawingContext ctx, Point eye, double mx, double my)
    {
        double dx = mx - eye.X;
        double dy = my - eye.Y;
        double dist = Math.Sqrt(dx * dx + dy * dy);
        if (dist < 25) return;

        double nx = dx / dist;
        double ny = dy / dist;
        double len = Math.Min(140, dist * 0.7);

        for (int i = 0; i < BeamPens.Length; i++)
        {
            double t0 = (double)i / BeamPens.Length;
            double t1 = (double)(i + 1) / BeamPens.Length;
            ctx.DrawLine(BeamPens[i],
                new Point(eye.X + nx * len * t0, eye.Y + ny * len * t0),
                new Point(eye.X + nx * len * t1, eye.Y + ny * len * t1));
        }
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
