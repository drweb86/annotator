using Avalonia;
using Avalonia.Media.Imaging;
using ScreenshotAnnotator.Models;
using System;
using System.Collections.Generic;

namespace ScreenshotAnnotator.Services;

internal static class ProjectRenderer
{
    public static RenderTargetBitmap? Render(Bitmap? bitmap, IEnumerable<AnnotationShape> shapes, out Vector offset)
    {
        offset = new Vector();

        if (bitmap is null)
            return null;

        var width = bitmap.PixelSize.Width;
        var height = bitmap.PixelSize.Height;
        var imageRect = new Rect(0, 0, width, height);

        // Export with transparent area: include all shapes in bounds, draw image only in its rect
        var fullBounds = imageRect;
        foreach (var shape in shapes)
        {
            var b = shape.GetBounds();
            fullBounds = fullBounds.Union(b);
        }

        var outWidth = (int)Math.Ceiling(fullBounds.Width);
        var outHeight = (int)Math.Ceiling(fullBounds.Height);
        if (outWidth <= 0 || outHeight <= 0) return null;

        var renderTargetTransparent = new RenderTargetBitmap(new PixelSize(outWidth, outHeight), new Vector(96, 96));
        using (var context = renderTargetTransparent.CreateDrawingContext())
        {
            offset = new Vector(-fullBounds.X, -fullBounds.Y);
            using (context.PushTransform(Matrix.CreateTranslation(offset)))
            {
                context.DrawImage(bitmap, imageRect);
                foreach (var shape in shapes)
                    shape.Render(context);
            }
        }
        return renderTargetTransparent;
    }

    public static RenderTargetBitmap? Render(Bitmap? bitmap, IEnumerable<AnnotationShape> shapes, Rect area)
    {
        var fullRenderedImage = Render(bitmap, shapes, out var offset);

        if (fullRenderedImage is null)
            return null;

        var selectorRect = area.Translate(offset);

        // Crop the rendered image to the selector area
        var cropX = (int)Math.Max(0, Math.Min(selectorRect.X, fullRenderedImage.PixelSize.Width));
        var cropY = (int)Math.Max(0, Math.Min(selectorRect.Y, fullRenderedImage.PixelSize.Height));
        var cropWidth = (int)Math.Max(1, Math.Min(selectorRect.Width, fullRenderedImage.PixelSize.Width - cropX));
        var cropHeight = (int)Math.Max(1, Math.Min(selectorRect.Height, fullRenderedImage.PixelSize.Height - cropY));

        var croppedImage = new RenderTargetBitmap(new PixelSize(cropWidth, cropHeight));

        using (var context = croppedImage.CreateDrawingContext())
        {
            // Draw the cropped portion from the full rendered image
            var sourceRect = new Rect(cropX, cropY, cropWidth, cropHeight);
            var destRect = new Rect(0, 0, cropWidth, cropHeight);

            context.DrawImage(fullRenderedImage, sourceRect, destRect);
        }

        return croppedImage;
    }
}
