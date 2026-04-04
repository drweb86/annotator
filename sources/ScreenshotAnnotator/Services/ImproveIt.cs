using System;
using System.Globalization;
using System.IO;
using System.Text;

namespace ScreenshotAnnotator.Services;

public static class ImproveIt
{
#pragma warning disable CA2211 // Non-constant fields should not be visible
    public static Action<string>? HandleUiError;
#pragma warning restore CA2211

    private static readonly string BugReportFile =
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), "Annotator BUG report.txt");

    static ImproveIt()
    {
        AppDomain.CurrentDomain.UnhandledException += UnhandledException;
    }

    public static void ProcessUnhandledException(Exception exception)
    {
        try
        {
            var builder = new StringBuilder();
            builder.AppendLine();
            builder.AppendLine();
            builder.AppendLine($"Screenshot Annotator {CopyrightInfo.Version} - Bug report ({DateTime.Now.ToString("g", CultureInfo.InvariantCulture)})");
            builder.AppendLine("Please report about it here:");
            builder.AppendLine(ApplicationLinks.AboutUrl);
            builder.AppendLine(ExceptionToString(exception));
            builder.AppendLine(exception.StackTrace);
            builder.AppendLine(exception.Source);

            var inner = exception.InnerException;
            if (inner != null)
            {
                builder.AppendLine(inner.Message);
                builder.AppendLine(inner.StackTrace);
                builder.AppendLine(inner.Source);
            }

            try
            {
                LoggingService.GetLogger(nameof(ImproveIt)).Fatal(exception, "Unhandled exception");
            }
            catch { }

            try
            {
                File.AppendAllText(BugReportFile, builder.ToString());
            }
            catch { }

            try
            {
                HandleUiError?.Invoke(LocalizationManager.Instance.GetString("ImproveIt_Message", BugReportFile));
            }
            catch { }
        }
        finally
        {
            Environment.Exit(-1);
        }
    }

    private static string ExceptionToString(Exception ex)
    {
        var builder = new StringBuilder();
        builder.Append(ex.Message);

        var current = ex.InnerException;
        int depth = 5;
        while (current != null && depth > 0)
        {
            builder.AppendLine();
            builder.Append(current.Message);
            current = current.InnerException;
            depth--;
        }

        return builder.ToString();
    }

    private static void UnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        ProcessUnhandledException((Exception)e.ExceptionObject);
    }
}
