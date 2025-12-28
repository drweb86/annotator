using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ScreenshotAnnotator.Models;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace ScreenshotAnnotator.ViewModels;

public partial class ImageEditorViewModel : ViewModelBase
{
    [ObservableProperty]
    private Bitmap? _image;

    [ObservableProperty]
    private ToolType _currentTool = ToolType.None;

    [ObservableProperty]
    private ObservableCollection<AnnotationShape> _shapes = new();

    [ObservableProperty]
    private bool _isSelectToolSelected;

    [ObservableProperty]
    private bool _isArrowToolSelected;

    [ObservableProperty]
    private bool _isCalloutToolSelected;

    [ObservableProperty]
    private bool _isTrimToolSelected;

    [RelayCommand]
    private void SelectSelectTool()
    {
        CurrentTool = ToolType.None;
        UpdateToolSelection();
    }

    [RelayCommand]
    private void SelectArrowTool()
    {
        CurrentTool = ToolType.Arrow;
        UpdateToolSelection();
    }

    [RelayCommand]
    private void SelectCalloutTool()
    {
        CurrentTool = ToolType.Callout;
        UpdateToolSelection();
    }

    [RelayCommand]
    private void SelectTrimTool()
    {
        CurrentTool = ToolType.Trim;
        UpdateToolSelection();
    }

    [RelayCommand]
    private async Task LoadImage()
    {
        // This will be connected to a file picker
        // For now, create a sample image
        var width = 800;
        var height = 600;
        var bitmap = new WriteableBitmap(
            new Avalonia.PixelSize(width, height),
            new Avalonia.Vector(96, 96),
            Avalonia.Platform.PixelFormat.Bgra8888,
            Avalonia.Platform.AlphaFormat.Premul
        );

        using (var buffer = bitmap.Lock())
        {
            unsafe
            {
                var ptr = (uint*)buffer.Address;
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        // Create a gradient background
                        byte r = (byte)(x * 255 / width);
                        byte g = (byte)(y * 255 / height);
                        byte b = 200;
                        ptr[y * width + x] = 0xFF000000 | ((uint)b << 16) | ((uint)g << 8) | r;
                    }
                }
            }
        }

        Image = bitmap;
    }

    [RelayCommand]
    private void ClearShapes()
    {
        Shapes.Clear();
    }

    [RelayCommand]
    private void ApplyTrim()
    {
        // This would crop the image to the trim rectangle
        // Implementation depends on the trim rectangle from the canvas
    }

    private void UpdateToolSelection()
    {
        IsSelectToolSelected = CurrentTool == ToolType.None;
        IsArrowToolSelected = CurrentTool == ToolType.Arrow;
        IsCalloutToolSelected = CurrentTool == ToolType.Callout;
        IsTrimToolSelected = CurrentTool == ToolType.Trim;
    }

    partial void OnCurrentToolChanged(ToolType value)
    {
        UpdateToolSelection();
    }
}
