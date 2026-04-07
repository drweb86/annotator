using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using System;

namespace ScreenshotAnnotator.Models;

public partial class ProjectFileInfo : ObservableObject
{
    public string FilePath { get; set; } = "";
    public string RenderedImageFilePath { get; set; } = "";
    public string FileNameWithoutExtension { get; set; } = "";
    public DateTime ModifiedDate { get; set; }
    [ObservableProperty]
    private Bitmap? _thumbnail;
    [ObservableProperty]
    private bool _isCurrentFile;
}
