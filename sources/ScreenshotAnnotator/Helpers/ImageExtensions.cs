using Avalonia.Media.Imaging;
using System.IO;

namespace ScreenshotAnnotator.Helpers;

static class ImageExtensions
{
    public static void SavePng(this Bitmap image, Stream stream)
    {
        image.Save(stream, PngBitmapEncoderOptions.Default);
    }

    public static Stream ToStream(this Bitmap image)
    {
        var stream = new MemoryStream();
        image.SavePng(stream);
        stream.Position = 0;
        return stream;
    }
}
