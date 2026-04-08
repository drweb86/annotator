using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using System;

namespace ScreenshotAnnotator.Controls;

/// <summary>
/// Wireframe cat head peeking from the bottom edge with red laser eyes that track the cursor.
/// Shown on odd days as an alternative idle decoration.
/// </summary>
public class IdleCatLaserControl : Control
{
    private Point _mouseLocal;

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
        _mouseLocal = new Point(
            (pos.X - bounds.Width / 2) / scale,
            (pos.Y - bounds.Height) / scale);

        InvalidateVisual();
    }

    public override void Render(DrawingContext context)
    {
        if (!IsVisible) return;
        base.Render(context);

        var bounds = Bounds;
        if (bounds.Width < 1 || bounds.Height < 1) return;

        var cx = bounds.Width / 2;
        var scale = Math.Clamp(Math.Min(bounds.Width / 300.0, bounds.Height / 350.0), 0.3, 1.5);

        using var clip = context.PushClip(new Rect(bounds.Size));
        using var transform = context.PushTransform(
            Matrix.CreateScale(scale, scale) * Matrix.CreateTranslation(cx, bounds.Height));

        DrawLaserBeams(context, _mouseLocal.X, _mouseLocal.Y);
        DrawCatHead(context, _mouseLocal.X, _mouseLocal.Y);
    }

    #region Drawing

    private static readonly Pen HeadPen      = new(new SolidColorBrush(Color.FromArgb(160, 200, 200, 210)), 2.0);
    private static readonly Pen EarPen        = new(new SolidColorBrush(Color.FromArgb(170, 210, 210, 220)), 2.0);
    private static readonly Pen EarInnerPen   = new(new SolidColorBrush(Color.FromArgb(90, 160, 160, 170)), 1.2);
    private static readonly Pen EyeOutlinePen = new(new SolidColorBrush(Color.FromArgb(190, 220, 220, 230)), 1.8);
    private static readonly Pen BrowPen       = new(new SolidColorBrush(Color.FromArgb(110, 180, 180, 190)), 1.2);
    private static readonly Pen PupilCrossPen = new(new SolidColorBrush(Color.FromArgb(180, 255, 255, 255)), 1.2);

    private static readonly SolidColorBrush LaserPupil  = new(Color.FromArgb(210, 255, 35, 35));
    private static readonly SolidColorBrush LaserGlow1  = new(Color.FromArgb(45, 255, 40, 40));
    private static readonly SolidColorBrush LaserGlow2  = new(Color.FromArgb(80, 255, 40, 40));
    private static readonly Pen             LaserRing   = new(new SolidColorBrush(Color.FromArgb(70, 255, 50, 50)), 2.0);

    private const double HeadRX = 50;
    private const double HeadRY = 42;
    private const double HeadCY = 10;

    private static readonly Point LeftEyeCenter  = new(-20, HeadCY - 16);
    private static readonly Point RightEyeCenter = new(20, HeadCY - 16);
    private const double EyeR   = 12;
    private const double PupilR = 5;

    private static void DrawCatHead(DrawingContext ctx, double mx, double my)
    {
        ctx.DrawEllipse(null, HeadPen, new Point(0, HeadCY), HeadRX, HeadRY);

        DrawEar(ctx, -1);
        DrawEar(ctx, 1);

        DrawEye(ctx, LeftEyeCenter, mx, my);
        DrawEye(ctx, RightEyeCenter, mx, my);

        var brow = new StreamGeometry();
        using (var g = brow.Open())
        {
            g.BeginFigure(new Point(-28, HeadCY - 26), false);
            g.CubicBezierTo(
                new Point(-12, HeadCY - 31),
                new Point(12, HeadCY - 31),
                new Point(28, HeadCY - 26));
        }
        ctx.DrawGeometry(null, BrowPen, brow);
    }

    private static void DrawEar(DrawingContext ctx, int side)
    {
        double s = side;

        var ear = new StreamGeometry();
        using (var g = ear.Open())
        {
            g.BeginFigure(new Point(s * 22, HeadCY - 32), false);
            g.LineTo(new Point(s * 36, HeadCY - 72));
            g.LineTo(new Point(s * 38, HeadCY - 28));
        }
        ctx.DrawGeometry(null, EarPen, ear);

        var inner = new StreamGeometry();
        using (var g = inner.Open())
        {
            g.BeginFigure(new Point(s * 25, HeadCY - 33), false);
            g.LineTo(new Point(s * 35, HeadCY - 65));
            g.LineTo(new Point(s * 36, HeadCY - 30));
        }
        ctx.DrawGeometry(null, EarInnerPen, inner);
    }

    private static void DrawEye(DrawingContext ctx, Point center, double mx, double my)
    {
        ctx.DrawEllipse(null, EyeOutlinePen, center, EyeR, EyeR);
        ctx.DrawEllipse(null, LaserRing, center, EyeR + 3, EyeR + 3);

        var pupil = ClampToCircle(center, mx, my, EyeR - PupilR - 1);

        ctx.DrawEllipse(LaserGlow1, null, pupil, PupilR + 6, PupilR + 6);
        ctx.DrawEllipse(LaserGlow2, null, pupil, PupilR + 3, PupilR + 3);
        ctx.DrawEllipse(LaserPupil, null, pupil, PupilR, PupilR);

        ctx.DrawLine(PupilCrossPen,
            new Point(pupil.X - PupilR + 1, pupil.Y),
            new Point(pupil.X + PupilR - 1, pupil.Y));
        ctx.DrawLine(PupilCrossPen,
            new Point(pupil.X, pupil.Y - PupilR + 1),
            new Point(pupil.X, pupil.Y + PupilR - 1));
    }

    private static readonly Pen[] BeamGlowPens =
    {
        new(new SolidColorBrush(Color.FromArgb(100, 255, 30, 30)), 8.0),
        new(new SolidColorBrush(Color.FromArgb(70,  255, 30, 30)), 6.0),
        new(new SolidColorBrush(Color.FromArgb(40,  255, 40, 40)), 4.0),
        new(new SolidColorBrush(Color.FromArgb(16,  255, 50, 50)), 2.5),
    };

    private static readonly Pen BeamCorePen = new(new SolidColorBrush(Color.FromArgb(140, 255, 70, 70)), 2.5);

    private static void DrawLaserBeams(DrawingContext ctx, double mx, double my)
    {
        DrawSingleBeam(ctx, LeftEyeCenter, mx, my);
        DrawSingleBeam(ctx, RightEyeCenter, mx, my);
    }

    private static void DrawSingleBeam(DrawingContext ctx, Point eye, double mx, double my)
    {
        double dx = mx - eye.X;
        double dy = my - eye.Y;
        double dist = Math.Sqrt(dx * dx + dy * dy);
        if (dist < 20) return;

        double nx = dx / dist;
        double ny = dy / dist;
        double len = Math.Min(400, dist * 0.85);

        for (int i = 0; i < BeamGlowPens.Length; i++)
        {
            double t0 = (double)i / BeamGlowPens.Length;
            double t1 = (double)(i + 1) / BeamGlowPens.Length;
            ctx.DrawLine(BeamGlowPens[i],
                new Point(eye.X + nx * len * t0, eye.Y + ny * len * t0),
                new Point(eye.X + nx * len * t1, eye.Y + ny * len * t1));
        }

        double coreLen = len * 0.55;
        ctx.DrawLine(BeamCorePen,
            new Point(eye.X, eye.Y),
            new Point(eye.X + nx * coreLen, eye.Y + ny * coreLen));
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
