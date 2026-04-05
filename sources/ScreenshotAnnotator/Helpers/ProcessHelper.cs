namespace ScreenshotAnnotator.Helpers;

static class ProcessHelper
{
    public static void OpenWithShell(string fileName)
    {
        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
        {
            FileName = fileName,
            UseShellExecute = true
        });
    }
}
