using System.Collections.Generic;
using System.Threading.Tasks;

namespace ScreenshotAnnotator.Services.Ocr;

public interface IOcrEngine
{
    string Id { get; }
    string DisplayName { get; }
    bool SupportsLanguageSelection { get; }
    IReadOnlyList<OcrLanguage> GetAvailableLanguages();
    Task<string> RecognizeTextAsync(byte[] pngData, IReadOnlyList<string> languageTags);
}
