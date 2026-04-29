using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Threading;
using System;

namespace ScreenshotAnnotator.Controls;

/// <summary>
/// Overlay drawn on top of the image canvas: two tiny cats with scissors
/// run toward each other along the cut-strip edges, snip, then the cut fires.
/// Place this inside the same Grid as ImageEditorCanvas so it shares the
/// same coordinate space (image pixels).
/// </summary>
public class CutAnimationOverlay : Control
{
    private DispatcherTimer? _timer;
    private int _frame;
    private bool _isVertical;
    private double _cutStart;
    private double _cutEnd;
    private Action? _onComplete;

    // 40 ticks @ 30 fps ≈ 1.33 s total
    private const int TotalFrames = 200;

    // -----------------------------------------------------------------------
    // Public API
    // -----------------------------------------------------------------------

    public void Play(bool isVertical, double cutStart, double cutEnd, Action onComplete)
    {
        _isVertical  = isVertical;
        _cutStart    = Math.Min(cutStart, cutEnd);
        _cutEnd      = Math.Max(cutStart, cutEnd);
        _onComplete  = onComplete;
        _frame       = 0;
        IsVisible    = true;

        _timer?.Stop();
        _timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(1000.0 / 30) };
        _timer.Tick += OnTick;
        _timer.Start();
        InvalidateVisual();
    }

    // -----------------------------------------------------------------------
    // Timer / render loop
    // -----------------------------------------------------------------------

    private void OnTick(object? sender, EventArgs e)
    {
        _frame++;
        InvalidateVisual();

        if (_frame < TotalFrames) return;

        _timer!.Stop();
        _timer = null;
        IsVisible = false;
        var cb = _onComplete;
        _onComplete = null;
        cb?.Invoke();
    }

    public override void Render(DrawingContext context)
    {
        base.Render(context);
        if (!IsVisible) return;

        var bounds = Bounds;
        if (bounds.Width < 1 || bounds.Height < 1) return;

        double t = (double)_frame / TotalFrames;

        // Cat scale: 6% of shorter side, clamped 26–65 px body-width
        double catScale = Math.Clamp(Math.Min(bounds.Width, bounds.Height) * 0.06, 26, 65) / 40.0;
        double catOffset = catScale * 58; // how far off-screen they start

        // Smooth-step easing for the full traversal (0 → 1 over the whole animation)
        double easedT = t * t * (3 - 2 * t);

        // Scissors + legs share the same phase so movement is synchronised
        double maxAngle     = 28.0 * Math.PI / 180.0;
        const int snipCount = 15;
        double rawPhase     = t * Math.PI * snipCount;
        double scissorAngle = maxAngle * Math.Abs(Math.Cos(rawPhase));

        // Red zone: fade in quickly, hold while cats traverse, fade out at the end
        byte redAlpha;
        if      (t < 0.10) redAlpha = (byte)(t / 0.10 * 140);
        else if (t < 0.90) redAlpha = 140;
        else               redAlpha = (byte)((1.0 - t) / 0.10 * 140);

        var redBrush = new SolidColorBrush(Color.FromArgb(redAlpha, 255, 65, 65));
        var redRect  = _isVertical
            ? new Rect(_cutStart, 0, _cutEnd - _cutStart, bounds.Height)
            : new Rect(0, _cutStart, bounds.Width, _cutEnd - _cutStart);
        context.FillRectangle(redBrush, redRect);

        // Scissors tip is 48 local units ahead of origin; use it as the visibility boundary.
        // A cat is drawn only while its scissors tip is inside the image bounds —
        // this prevents any part of the cat appearing outside the cut area.
        double fwd = 48.0 * catScale;

        if (_isVertical)
        {
            // Cat 1: left edge of strip, enters from top, exits at bottom
            double c1x = _cutStart;
            double c1y = Lerp(-catOffset, bounds.Height + catOffset, easedT);
            // scissors tip (screen y) = c1y + fwd  (local +X → screen +Y after 90° rotation)
            if (c1y + fwd > 0 && c1y + fwd < bounds.Height)
            {
                using (context.PushTransform(
                    Matrix.CreateScale(catScale, catScale) *
                    Matrix.CreateRotation(Math.PI / 2) *
                    Matrix.CreateTranslation(c1x, c1y)))
                {
                    DrawCatWithScissors(context, scissorAngle, rawPhase);
                }
            }

            // Cat 2: right edge of strip, enters from bottom, exits at top
            double c2x = _cutEnd;
            double c2y = Lerp(bounds.Height + catOffset, -catOffset, easedT);
            // scissors tip (screen y) = c2y - fwd  (local +X → screen -Y after -90° rotation)
            if (c2y - fwd > 0 && c2y - fwd < bounds.Height)
            {
                using (context.PushTransform(
                    Matrix.CreateScale(catScale, catScale) *
                    Matrix.CreateRotation(-Math.PI / 2) *
                    Matrix.CreateTranslation(c2x, c2y)))
                {
                    DrawCatWithScissors(context, scissorAngle, rawPhase);
                }
            }
        }
        else
        {
            // Cat 1: top edge of strip, enters from left, exits at right
            double c1x = Lerp(-catOffset, bounds.Width + catOffset, easedT);
            double c1y = _cutStart;
            // scissors tip (screen x) = c1x + fwd  (no rotation, faces right)
            if (c1x + fwd > 0 && c1x + fwd < bounds.Width)
            {
                using (context.PushTransform(
                    Matrix.CreateScale(catScale, catScale) *
                    Matrix.CreateTranslation(c1x, c1y)))
                {
                    DrawCatWithScissors(context, scissorAngle, rawPhase);
                }
            }

            // Cat 2: bottom edge of strip, enters from right, exits at left
            double c2x = Lerp(bounds.Width + catOffset, -catOffset, easedT);
            double c2y = _cutEnd;
            // scissors tip (screen x) = c2x - fwd  (local +X → screen -X after horizontal flip)
            if (c2x - fwd > 0 && c2x - fwd < bounds.Width)
            {
                using (context.PushTransform(
                    Matrix.CreateScale(-catScale, catScale) *
                    Matrix.CreateTranslation(c2x, c2y)))
                {
                    DrawCatWithScissors(context, scissorAngle, rawPhase);
                }
            }
        }
    }

    // -----------------------------------------------------------------------
    // Drawing — all coordinates are in "cat-local units" (40 units ≈ body width).
    // The cat always faces RIGHT; the caller applies a rotation/flip transform.
    // -----------------------------------------------------------------------

    // Cat colours
    private static readonly SolidColorBrush CatFur      = new(Color.FromRgb(220, 215, 205));
    private static readonly SolidColorBrush CatFurDark  = new(Color.FromRgb(175, 165, 152));
    private static readonly SolidColorBrush CatEarInner = new(Color.FromArgb(145, 220, 175, 180));
    private static readonly SolidColorBrush CatEyeDot   = new(Color.FromRgb(28, 28, 28));
    private static readonly SolidColorBrush CatNose     = new(Color.FromRgb(200, 118, 128));
    private static readonly Pen             CatOutline  = new(new SolidColorBrush(Color.FromArgb(130, 20, 20, 20)), 1.2);

    // Halo behind cat for readability against any background
    private static readonly SolidColorBrush HaloLight = new(Color.FromArgb(55, 255, 255, 255));

    // Leg colours
    private static readonly Pen LegPen = new(new SolidColorBrush(Color.FromRgb(155, 145, 132)), 2.0);

    // Scissors colours
    private static readonly SolidColorBrush ScissorsBlade   = new(Color.FromRgb(185, 192, 208));
    private static readonly SolidColorBrush ScissorsHandle  = new(Color.FromRgb(48, 125, 200));
    private static readonly SolidColorBrush ScissorsPivot   = new(Color.FromRgb(148, 150, 155));
    private static readonly Pen             ScissorsPen     = new(ScissorsBlade, 2.2);
    private static readonly Pen             ScissorsEdge    = new(new SolidColorBrush(Color.FromArgb(90, 0, 0, 0)), 0.8);

    private static void DrawCatWithScissors(DrawingContext ctx, double scissorAngle, double legPhase)
    {
        // Legs drawn first so the body overlaps their roots naturally
        double swing = Math.Sin(legPhase) * 4.0; // forward/back swing in local X
        ctx.DrawLine(LegPen, new Point( 9, 8), new Point( 9 + swing, 17));
        ctx.DrawLine(LegPen, new Point( 4, 8), new Point( 4 - swing, 17));
        ctx.DrawLine(LegPen, new Point(-4, 8), new Point(-4 + swing, 17));
        ctx.DrawLine(LegPen, new Point(-9, 8), new Point(-9 - swing, 17));

        // Soft halo so cat is readable on any image
        ctx.DrawEllipse(HaloLight, null, new Point(0, 0), 32, 20);

        // Body
        ctx.DrawEllipse(CatFur,     CatOutline, new Point(0,  0), 14, 9);

        // Head
        ctx.DrawEllipse(CatFur,     CatOutline, new Point(18, -1), 10, 10);

        // Ears (two small triangles above head)
        ctx.DrawEllipse(CatFurDark, CatOutline, new Point(10, -13), 4, 5);
        ctx.DrawEllipse(CatEarInner, null,       new Point(10, -13), 2, 3);
        ctx.DrawEllipse(CatFurDark, CatOutline, new Point(22, -13), 4, 5);
        ctx.DrawEllipse(CatEarInner, null,       new Point(22, -13), 2, 3);

        // Eye
        ctx.DrawEllipse(CatEyeDot,  null,        new Point(23, -3), 2.5, 2.5);

        // Nose
        ctx.DrawEllipse(CatNose,    null,        new Point(27,  0), 1.8, 1.8);

        // ---- Scissors ----
        var pivot = new Point(30, 0);
        double bladeLen  = 18;
        double handleLen = 11;

        // Blade tips
        double bx1 = pivot.X + Math.Cos(scissorAngle)  * bladeLen;
        double by1 = pivot.Y + Math.Sin(scissorAngle)  * bladeLen;
        double bx2 = pivot.X + Math.Cos(-scissorAngle) * bladeLen;
        double by2 = pivot.Y + Math.Sin(-scissorAngle) * bladeLen;

        // Handle ends (opposite direction)
        double hx1 = pivot.X - Math.Cos(scissorAngle)  * handleLen;
        double hy1 = pivot.Y - Math.Sin(scissorAngle)  * handleLen;
        double hx2 = pivot.X - Math.Cos(-scissorAngle) * handleLen;
        double hy2 = pivot.Y - Math.Sin(-scissorAngle) * handleLen;

        // Blade lines (edge shadow, then bright blade)
        ctx.DrawLine(ScissorsEdge, pivot, new Point(bx1, by1));
        ctx.DrawLine(ScissorsEdge, pivot, new Point(bx2, by2));
        ctx.DrawLine(ScissorsPen,  pivot, new Point(bx1, by1));
        ctx.DrawLine(ScissorsPen,  pivot, new Point(bx2, by2));

        // Handle rings
        ctx.DrawEllipse(ScissorsHandle, ScissorsEdge, new Point(hx1, hy1), 4.5, 4.5);
        ctx.DrawEllipse(ScissorsHandle, ScissorsEdge, new Point(hx2, hy2), 4.5, 4.5);

        // Pivot screw
        ctx.DrawEllipse(ScissorsPivot, ScissorsEdge, pivot, 2.5, 2.5);
    }

    private static double Lerp(double a, double b, double t) => a + (b - a) * t;
}
