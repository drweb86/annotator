using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using System;

namespace ScreenshotAnnotator.Models;

public partial class ProjectFileInfo : ObservableObject
{
    public string FilePath { get; set; } = "";
    public string FileName { get; set; } = "";
    public DateTime ModifiedDate { get; set; }
    public Bitmap? Thumbnail { get; set; }

    [ObservableProperty]
    private bool _isCurrentFile;
}
