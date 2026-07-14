using Avalonia.Media.Imaging;
using Avalonia.Platform;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ScreenshotAnnotator.Resources;
using ScreenshotAnnotator.Services;
using ScreenshotAnnotator.Services.Ocr;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ScreenshotAnnotator.ViewModels;

public partial class OcrLanguageItem : ViewModelBase
{
    public OcrLanguage Language { get; }

    [ObservableProperty]
    private bool _isSelected;

    public string DisplayName => Language.DisplayName;
    public string Tag => Language.Tag;

    public OcrLanguageItem(OcrLanguage language) => Language = language;
}

public partial class OcrViewModel : ViewModelBase
{
    private readonly IApplicationSettings _settings;
    private readonly byte[] _imageData;

    [ObservableProperty]
    private ObservableCollection<IOcrEngine> _availableEngines = new();

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanExtract))]
    private IOcrEngine? _selectedEngine;

    [ObservableProperty]
    private ObservableCollection<OcrLanguageItem> _availableLanguages = new();

    [ObservableProperty]
    private bool _hasEngines;

    [ObservableProperty]
    private bool _showLanguages;

    [ObservableProperty]
    private bool _showTesseractHint;

    public bool ShowWindowsTesseractHint => OperatingSystem.IsWindows();

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanExtract))]
    private bool _isProcessing;

    [ObservableProperty]
    private string _resultText = string.Empty;

    [ObservableProperty]
    private bool _hasResult;

    public bool CanExtract => HasEngines && !IsProcessing && SelectedEngine != null;

    public Bitmap? CatBitmap { get; }

    // Design-time only
    public OcrViewModel() : this(AllServices.ApplicationSettings, Array.Empty<byte>()) { }

    public OcrViewModel(IApplicationSettings settings, byte[] imageData)
    {
        _settings = settings;
        _imageData = imageData;

        CatBitmap = LoadRandomCat();
        LoadEngines();
    }

    private void LoadEngines()
    {
        var engines = OcrEngineRegistry.GetAvailableEngines();
        foreach (var e in engines)
            AvailableEngines.Add(e);

        HasEngines = AvailableEngines.Count > 0;

        if (!HasEngines) return;

        // Restore preferred engine
        var preferred = _settings.Settings.OcrPreferredEngine;
        var selectedEngine = preferred != null
            ? AvailableEngines.FirstOrDefault(e => e.Id == preferred)
            : null;
        SelectedEngine = selectedEngine ?? AvailableEngines[0];
    }

    partial void OnSelectedEngineChanged(IOcrEngine? value)
    {
        AvailableLanguages.Clear();
        ShowLanguages = false;
        ShowTesseractHint = false;

        if (value == null) return;

        ShowLanguages = value.SupportsLanguageSelection;
        ShowTesseractHint = value.Id == "tesseract";

        if (!value.SupportsLanguageSelection) return;

        var langs = value.GetAvailableLanguages();
        var preferred = _settings.Settings.OcrPreferredLanguages;

        foreach (var lang in langs)
        {
            var item = new OcrLanguageItem(lang)
            {
                IsSelected = preferred.Contains(lang.Tag)
            };
            AvailableLanguages.Add(item);
        }

        // Default selection if nothing is preferred
        if (!AvailableLanguages.Any(l => l.IsSelected) && AvailableLanguages.Count > 0)
        {
            var eng = AvailableLanguages.FirstOrDefault(l =>
                l.Tag.StartsWith("eng", StringComparison.OrdinalIgnoreCase) ||
                l.Tag.StartsWith("en", StringComparison.OrdinalIgnoreCase));
            var toSelect = eng ?? AvailableLanguages[0];
            toSelect.IsSelected = true;
        }

        // Save preferred engine
        _settings.Settings.OcrPreferredEngine = value.Id;
        _settings.Save();
    }

    [RelayCommand]
    private async Task ExtractText()
    {
        if (SelectedEngine == null || IsProcessing) return;

        IsProcessing = true;
        HasResult = false;
        ResultText = Strings.Ocr_Processing;

        try
        {
            var selectedLangs = AvailableLanguages
                .Where(l => l.IsSelected)
                .Select(l => l.Tag)
                .ToList();

            _settings.Settings.OcrPreferredLanguages = selectedLangs;
            _settings.Save();

            ResultText = await Task.Run(() => SelectedEngine.RecognizeTextAsync(_imageData, selectedLangs));
            HasResult = true;
        }
        catch (Exception ex)
        {
            ResultText = $"Error: {ex.Message}";
            HasResult = true;
        }
        finally
        {
            IsProcessing = false;
        }
    }

    private static readonly string[] CatNames =
    {
        "Cat-Default", "Cat-Hat", "Cat-Mouse", "Cat-Hearts", "Cat-Milk",
        "Cat-Sleeping", "Cat-Fish", "Cat-Yarn", "Cat-Santa", "Cat-Glasses",
        "Cat-Pirate", "Cat-Wizard", "Cat-Astronaut", "Cat-Butterfly", "Cat-Box",
        "Cat-Birthday", "Cat-Umbrella", "Cat-Samurai", "Cat-Doctor", "Cat-King", "Cat-Ninja"
    };

    private static Bitmap? LoadRandomCat()
    {
        try
        {
            var name = CatNames[Random.Shared.Next(CatNames.Length)];
            var uri = new Uri($"avares://ScreenshotAnnotator/Assets/Cats/{name}.png");
            using var stream = AssetLoader.Open(uri);
            return new Bitmap(stream);
        }
        catch
        {
            return null;
        }
    }
}
