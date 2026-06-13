#if WINDOWS
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Globalization;
using Windows.Graphics.Imaging;
using Windows.Media.Ocr;
using Windows.Storage.Streams;

namespace ScreenshotAnnotator.Services.Ocr;

public class WindowsMediaOcrEngine : IOcrEngine
{
    public string Id => "windows-media-ocr";
    public string DisplayName => "Windows OCR";
    public bool SupportsLanguageSelection => true;

    public IReadOnlyList<OcrLanguage> GetAvailableLanguages()
    {
        try
        {
            return OcrEngine.AvailableRecognizerLanguages
                .Select(l => new OcrLanguage(l.LanguageTag, l.DisplayName))
                .ToList();
        }
        catch
        {
            return Array.Empty<OcrLanguage>();
        }
    }

    public async Task<string> RecognizeTextAsync(byte[] pngData, IReadOnlyList<string> languageTags)
    {
        var softwareBitmap = await DecodePngToSoftwareBitmapAsync(pngData);

        var sb = new StringBuilder();

        var tags = languageTags.Count > 0 ? languageTags : new[] { "en" };
        foreach (var tag in tags)
        {
            try
            {
                var language = new Language(tag);
                var engine = OcrEngine.TryCreateFromLanguage(language);
                if (engine == null) continue;
                var result = await engine.RecognizeAsync(softwareBitmap);
                foreach (var line in result.Lines)
                {
                    sb.AppendLine(line.Text);
                }
            }
            catch
            {
                // skip unavailable language
            }
        }

        return sb.ToString().Trim();
    }

    private static async Task<SoftwareBitmap> DecodePngToSoftwareBitmapAsync(byte[] pngData)
    {
        var stream = new InMemoryRandomAccessStream();
        var writer = new DataWriter(stream.GetOutputStreamAt(0));
        writer.WriteBytes(pngData);
        await writer.StoreAsync();
        stream.Seek(0);

        var decoder = await BitmapDecoder.CreateAsync(stream);
        var softwareBitmap = await decoder.GetSoftwareBitmapAsync();

        // OcrEngine requires Bgra8 premultiplied alpha
        if (softwareBitmap.BitmapPixelFormat != BitmapPixelFormat.Bgra8 ||
            softwareBitmap.BitmapAlphaMode != BitmapAlphaMode.Premultiplied)
        {
            softwareBitmap = SoftwareBitmap.Convert(
                softwareBitmap,
                BitmapPixelFormat.Bgra8,
                BitmapAlphaMode.Premultiplied);
        }

        return softwareBitmap;
    }
}
#endif
