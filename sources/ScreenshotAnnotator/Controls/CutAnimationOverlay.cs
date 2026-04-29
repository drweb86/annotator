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

    // 25 ticks @ 30 fps ≈ 833 ms total
    private const int TotalFrames  = 25;
    private const double SnipStartT = 0.68;  // cats start closing scissors
    private const double SnipEndT   = 0.88;  // scissors fully closed
    private const double FlashStartT = 0.80; // cut-zone flash begins

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

        // Movement progress: smooth-step over first SnipStartT fraction
        double moveRaw = Math.Min(t / SnipStartT, 1.0);
        double easedT  = moveRaw * moveRaw * (3 - 2 * moveRaw);

        // Scissors opening angle (radians): open during approach, snap shut at snip
        double maxAngle = 28.0 * Math.PI / 180.0;
        double scissorAngle;
        if (t < SnipStartT)
            scissorAngle = maxAngle;
        else if (t < SnipEndT)
            scissorAngle = maxAngle * (1 - (t - SnipStartT) / (SnipEndT - SnipStartT));
        else
            scissorAngle = 0;

        // Flash rectangle over cut zone when scissors close
        if (t > FlashStartT)
        {
            double flashT  = (t - FlashStartT) / (1.0 - FlashStartT);
            byte flashAlpha = (byte)(Math.Sin(flashT * Math.PI) * 155);
            var flashBrush = new SolidColorBrush(Color.FromArgb(flashAlpha, 255, 75, 75));
            var flashRect = _isVertical
                ? new Rect(_cutStart, 0, _cutEnd - _cutStart, bounds.Height)
                : new Rect(0, _cutStart, bounds.Width, _cutEnd - _cutStart);
            context.FillRectangle(flashBrush, flashRect);
        }

        if (_isVertical)
        {
            // Cat 1: at left edge of strip, comes from above, faces DOWN
            double c1x = _cutStart;
            double c1y = Lerp(-catOffset, bounds.Height * 0.5, easedT);

            // Cat 2: at right edge of strip, comes from below, faces UP
            double c2x = _cutEnd;
            double c2y = Lerp(bounds.Height + catOffset, bounds.Height * 0.5, easedT);

            using (context.PushTransform(
                Matrix.CreateScale(catScale, catScale) *
                Matrix.CreateRotation(Math.PI / 2) *
                Matrix.CreateTranslation(c1x, c1y)))
            {
                DrawCatWithScissors(context, scissorAngle);
            }

            using (context.PushTransform(
                Matrix.CreateScale(catScale, catScale) *
                Matrix.CreateRotation(-Math.PI / 2) *
                Matrix.CreateTranslation(c2x, c2y)))
            {
                DrawCatWithScissors(context, scissorAngle);
            }
        }
        else
        {
            // Cat 1: at top edge of strip, comes from left, faces RIGHT
            double c1x = Lerp(-catOffset, bounds.Width * 0.5, easedT);
            double c1y = _cutStart;

            // Cat 2: at bottom edge of strip, comes from right, faces LEFT (mirrored)
            double c2x = Lerp(bounds.Width + catOffset, bounds.Width * 0.5, easedT);
            double c2y = _cutEnd;

            using (context.PushTransform(
                Matrix.CreateScale(catScale, catScale) *
                Matrix.CreateTranslation(c1x, c1y)))
            {
                DrawCatWithScissors(context, scissorAngle);
            }

            using (context.PushTransform(
                Matrix.CreateScale(-catScale, catScale) *
                Matrix.CreateTranslation(c2x, c2y)))
            {
                DrawCatWithScissors(context, scissorAngle);
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
    private static readonly SolidColorBrush HaloShadow  = new(Color.FromArgb(70,  0,  0,  0));
    private static readonly SolidColorBrush HaloLight   = new(Color.FromArgb(130, 255, 255, 255));

    // Scissors colours
    private static readonly SolidColorBrush ScissorsBlade   = new(Color.FromRgb(185, 192, 208));
    private static readonly SolidColorBrush ScissorsHandle  = new(Color.FromRgb(48, 125, 200));
    private static readonly SolidColorBrush ScissorsPivot   = new(Color.FromRgb(148, 150, 155));
    private static readonly Pen             ScissorsPen     = new(ScissorsBlade, 2.2);
    private static readonly Pen             ScissorsEdge    = new(new SolidColorBrush(Color.FromArgb(90, 0, 0, 0)), 0.8);

    private static void DrawCatWithScissors(DrawingContext ctx, double scissorAngle)
    {
        // Soft halo so cat is readable on any image
        ctx.DrawEllipse(HaloShadow, null, new Point(2, 2),  32, 20);
        ctx.DrawEllipse(HaloLight,  null, new Point(0, 0),  32, 20);

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
