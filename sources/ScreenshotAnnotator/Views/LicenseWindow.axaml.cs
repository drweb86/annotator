using Avalonia.Controls;
using Avalonia.Interactivity;
using ScreenshotAnnotator.Legal;

namespace ScreenshotAnnotator.Views;

public partial class LicenseWindow : Window
{
    private bool _suppressLanguageChange;

    public LicenseWindow()
    {
        InitializeComponent();

        LanguageCombo.ItemsSource = DocumentLanguages.License;
        var selected = DocumentStore.LanguageCode() is { } code
            ? DocumentLanguages.ByCode(DocumentLanguages.License, code)
            : DocumentLanguages.MatchDevice(DocumentLanguages.License);

        _suppressLanguageChange = true;
        LanguageCombo.SelectedItem = selected;
        _suppressLanguageChange = false;

        if (DocumentStore.LanguageCode() == null)
            DocumentStore.SetLanguageCode(selected.Code);

        LoadDocument(selected);
    }

    private void LanguageCombo_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (_suppressLanguageChange || LanguageCombo.SelectedItem is not DocumentLanguage language)
            return;

        DocumentStore.SetLanguageCode(language.Code);
        LoadDocument(language);
    }

    private void OkButton_Click(object? sender, RoutedEventArgs e) => Close();

    private void LoadDocument(DocumentLanguage language)
    {
        MarkdownDocumentPresenter.Render(
            this,
            DocumentHost,
            LicenseDocuments.LoadMarkdown(language.AssetFile),
            language.Rtl);
    }
}
