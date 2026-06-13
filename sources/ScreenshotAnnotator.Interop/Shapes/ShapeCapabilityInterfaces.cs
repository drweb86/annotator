using Avalonia;
using Avalonia.Media;
using ScreenshotAnnotator.Models;
using System;

namespace ScreenshotAnnotator.Interop.Shapes;

public interface ICornerResizableShape
{
    Rect Rectangle { get; set; }
    bool IsPointOnCornerHandle(Point point, out RectCorner corner);
    void ResizeFromCorner(RectCorner corner, Point newPosition);
}

public interface ITextEditableShape
{
    string Text { get; set; }
    string FontFamily { get; set; }
    double FontSize { get; set; }
    bool FontBold { get; set; }
    bool FontItalic { get; set; }
    Rect TextBounds { get; }
}

public interface IFillColorShape
{
    Color FillColor { get; set; }
}

public interface IHighlighterCornerShape
{
    Point StartPoint { get; set; }
    Point EndPoint { get; set; }
    RectCorner? GetCornerAtPoint(Point point, double handleSize = 8);
    void ResizeCorner(RectCorner corner, Point newPosition);
}

public interface IVerticalCutAdjustable
{
    void AdjustForVerticalCut(double cutX, double cutWidth);
}

public interface IHorizontalCutAdjustable
{
    void AdjustForHorizontalCut(double cutY, double cutHeight);
}
