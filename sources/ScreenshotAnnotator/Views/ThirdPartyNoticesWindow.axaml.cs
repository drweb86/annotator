using Avalonia.Controls;
using Avalonia.Interactivity;
using ScreenshotAnnotator.Legal;

namespace ScreenshotAnnotator.Views;

public partial class ThirdPartyNoticesWindow : Window
{
    public ThirdPartyNoticesWindow()
    {
        InitializeComponent();
        MarkdownDocumentPresenter.Render(this, DocumentHost, ThirdPartyNotices.Load(), rtl: false);
    }

    private void OkButton_Click(object? sender, RoutedEventArgs e) => Close();
}
