using Avalonia.Media.Imaging;
using System.IO;

namespace ScreenshotAnnotator.Helpers;

static class ImageExtensions
{
    public static Stream ToStream(this Bitmap image)
    {
        var stream = new MemoryStream();
        image.Save(stream);
        stream.Position = 0;
        return stream;
    }
}
