using Avalonia.Controls;
using Avalonia.Interactivity;

namespace ScreenshotAnnotator.Views;

public partial class OcrWindow : Window
{
    public OcrWindow()
    {
        InitializeComponent();
    }

    private void OnCloseClick(object? sender, RoutedEventArgs e) => Close();
}
