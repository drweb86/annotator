using CommunityToolkit.Mvvm.ComponentModel;

namespace ScreenshotAnnotator.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    [ObservableProperty]
    private ImageEditorViewModel _imageEditor = new();
}
