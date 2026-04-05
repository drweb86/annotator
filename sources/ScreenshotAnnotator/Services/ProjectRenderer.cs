using Avalonia;
using Avalonia.Media.Imaging;
using ScreenshotAnnotator.Models;
using System;
using System.Collections.Generic;
using System.IO;

namespace ScreenshotAnnotator.Services;

internal static class ProjectRenderer
{
    private const double PreviewMaxSize = 100.0;

    /// <summary>
    /// Encodes a rendered project image as PNG base64, downscaled so the longest side is at most 100px.
    /// </summary>
    public static string CreatePreviewImage(RenderTargetBitmap renderedImage)
    {
        var w = renderedImage.PixelSize.Width;
        var h = renderedImage.PixelSize.Height;
        var scale = Math.Min(PreviewMaxSize / w, PreviewMaxSize / h);

        if (scale < 1)
        {
            var newWidth = (int)(w * scale);
            var newHeight = (int)(h * scale);
            using var scaled = new RenderTargetBitmap(new PixelSize(newWidth, newHeight), new Vector(96, 96));
            using (var context = scaled.CreateDrawingContext())
            {
                var sourceRect = new Rect(0, 0, w, h);
                var destRect = new Rect(0, 0, newWidth, newHeight);
                context.DrawImage(renderedImage, sourceRect, destRect);
            }

            using var previewStream = new MemoryStream();
            scaled.Save(previewStream);
            return Convert.ToBase64String(previewStream.ToArray());
        }

        using (var previewStream = new MemoryStream())
        {
            renderedImage.Save(previewStream);
            return Convert.ToBase64String(previewStream.ToArray());
        }
    }

    /// <summary>
    /// Decodes stored preview base64 to a bitmap. If it already fits within 100×100, returns it as-is; otherwise scales down.
    /// </summary>
    public static Bitmap? CreatePreviewImage(string? previewImageBase64)
    {
        if (string.IsNullOrEmpty(previewImageBase64))
            return null;

        var imageBytes = Convert.FromBase64String(previewImageBase64);
        using var memoryStream = new MemoryStream(imageBytes);
        var bitmap = new Bitmap(memoryStream);

        var scale = Math.Min(PreviewMaxSize / bitmap.PixelSize.Width, PreviewMaxSize / bitmap.PixelSize.Height);
        if (scale < 1)
        {
            var newWidth = (int)(bitmap.PixelSize.Width * scale);
            var newHeight = (int)(bitmap.PixelSize.Height * scale);
            var scaled = bitmap.CreateScaledBitmap(new PixelSize(newWidth, newHeight));
            bitmap.Dispose();
            return scaled;
        }

        return bitmap;
    }

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
