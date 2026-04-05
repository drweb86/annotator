using Avalonia.Media.Imaging;
using System;

namespace ScreenshotAnnotator.Models;

public class ProjectFileInfo
{
    public string FilePath { get; set; } = "";
    public string FileName { get; set; } = "";
    public DateTime ModifiedDate { get; set; }
    public Bitmap? Thumbnail { get; set; }
    public bool IsCurrentFile { get; set; }
}
