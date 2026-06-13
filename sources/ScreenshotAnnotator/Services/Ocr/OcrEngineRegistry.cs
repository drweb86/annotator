using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace ScreenshotAnnotator.Services.Ocr;

public static class OcrEngineRegistry
{
    public static IReadOnlyList<IOcrEngine> GetAvailableEngines()
    {
        var engines = new List<IOcrEngine>();

#if WINDOWS
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            engines.Add(new WindowsMediaOcrEngine());
#endif

        if (TesseractOcrEngine.IsAvailable())
            engines.Add(new TesseractOcrEngine());

        return engines;
    }
}
