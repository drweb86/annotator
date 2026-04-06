using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Input.Platform;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NLog;
using ScreenshotAnnotator.Helpers;
using ScreenshotAnnotator.Models;
using ScreenshotAnnotator.Services;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;

namespace ScreenshotAnnotator.ViewModels;

/// <summary>Wrapper for arrow color preset so we can bind IsSelected in the template.</summary>
public partial class ArrowColorPresetItem : ViewModelBase
{
    [ObservableProperty]
    private Color _color;

    [ObservableProperty]
    private bool _isSelected;
}

/// <summary>Wrapper for highlighter color preset so we can bind IsSelected in the template.</summary>
public partial class HighlighterColorPresetItem : ViewModelBase
{
    [ObservableProperty]
    private Color _color;

    [ObservableProperty]
    private bool _isSelected;
}

public partial class ImageEditorViewModel : ViewModelBase, IProjectUi
{
    internal readonly IClipboardService ClipboardService = new ClipboardService();

    #region IProjectUi

    public Bitmap? GetBitmap() // whats that about, why not expose getter?
    {
        return Image;
    }
    public IEnumerable<AnnotationShape> GetShapes()
    {
        return Shapes;
    }

    public AnnotationShape? GetSelectedShape()
    {
        return SelectedShape;
    }

    public void SetSelectedShape(AnnotationShape annotationShape)
    {
        SelectShape(annotationShape);
    }

    public async Task CreateNewFromBitmap(Bitmap bitmap)
    {
        await SaveCurrentProject();
        CloseProject();
        var project = await AllServices.ProjectManager.ImportImage(bitmap.ToStream());
        await FinishCreatingProject(project);
    }

    #endregion // IProjectUi

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
    private bool _isCalloutNoArrowToolSelected;

    [ObservableProperty]
    private bool _isBorderedRectangleToolSelected;

    [ObservableProperty]
    private bool _isBlurRectangleToolSelected;

    [ObservableProperty]
    private bool _isSelectorToolSelected;

    [ObservableProperty]
    private bool _isVerticalCutOutToolSelected;

    [ObservableProperty]
    private bool _isHorizontalCutOutToolSelected;

    [ObservableProperty]
    private bool _isHighlighterToolSelected;

    [ObservableProperty]
    private string _headerInformation = LocalizationManager.Instance.GetString("Header_ScreenshotAnnotator", CopyrightInfo.Version.ToString(3));

    [ObservableProperty]
    private string? _currentFileName;

    [ObservableProperty]
    private string? _currentFilePathTooltip;

    [ObservableProperty]
    private AnnotationShape? _selectedShape;

    /// <summary>Preset colors for arrow with selection state for the UI.</summary>
    [ObservableProperty]
    private ObservableCollection<ArrowColorPresetItem> _arrowColorPresetItems = new();

    /// <summary>Preset colors for highlighter with selection state for the UI.</summary>
    [ObservableProperty]
    private ObservableCollection<HighlighterColorPresetItem> _highlighterColorPresetItems = new();

    private Controls.ImageEditorCanvas? _editorCanvas;

    private static readonly Color[] ArrowPresetColorsDefault = new[]
    {
        Colors.Red,
        Colors.Blue,
        Colors.Green,
        Color.FromRgb(255, 165, 0),   // Orange
        Colors.Yellow,
        Colors.Purple,
        Colors.Black,
    };

    private static readonly Color[] HighlighterPresetColorsDefault = new[]
    {
        Color.FromArgb(100, 255, 255, 0),   // Yellow
        Color.FromArgb(100, 0, 255, 0),     // Green
        Color.FromArgb(100, 0, 255, 255),   // Cyan
        Color.FromArgb(100, 255, 105, 180), // Pink
        Color.FromArgb(100, 255, 165, 0),   // Orange
        Color.FromArgb(100, 230, 230, 250), // Lavender
        Color.FromArgb(100, 135, 206, 235), // Light Blue
        Color.FromArgb(100, 50, 205, 50),   // Lime
    };

    private Color _currentHighlighterColor = Color.FromArgb(100, 255, 255, 0);

    public void SetEditorCanvas(Controls.ImageEditorCanvas canvas)
    {
        _editorCanvas = canvas;
        canvas.SetViewModel(this);
    }

    private void RefreshCanvas()
    {
        _editorCanvas?.InvalidateVisual();
    }

    partial void OnSelectedShapeChanged(AnnotationShape? value)
    {
        UpdateArrowColorPresets();
        UpdateHighlighterColorPresets();
        OnPropertyChanged(nameof(IsTextShapeSelected));
        OnPropertyChanged(nameof(IsArrowShapeSelected));
        OnPropertyChanged(nameof(IsHighlighterShapeSelected));
        OnPropertyChanged(nameof(SelectedTextFontFamily));
        OnPropertyChanged(nameof(SelectedTextFontSize));
        OnPropertyChanged(nameof(SelectedTextFontBold));
        OnPropertyChanged(nameof(SelectedTextFontItalic));
        OnPropertyChanged(nameof(SelectedArrowColor));
        OnPropertyChanged(nameof(SelectedHighlighterColor));
        OnPropertyChanged(nameof(FontFamilyChoices));
        RefreshCanvas();
    }

    private void UpdateArrowColorPresets()
    {
        ArrowColorPresetItems.Clear();
        if (SelectedShape is ArrowShape arrow)
        {
            var selected = arrow.StrokeColor;
            foreach (var c in ArrowPresetColorsDefault)
            {
                ArrowColorPresetItems.Add(new ArrowColorPresetItem
                {
                    Color = c,
                    IsSelected = ColorsEqual(c, selected)
                });
            }
        }
    }

    private static bool ColorsEqual(Color a, Color b)
        => a.A == b.A && a.R == b.R && a.G == b.G && a.B == b.B;

    private void UpdateArrowColorPresetSelection()
    {
        var selected = SelectedArrowColor;
        foreach (var item in ArrowColorPresetItems)
            item.IsSelected = ColorsEqual(item.Color, selected);
    }

    public bool IsTextShapeSelected => SelectedShape is CalloutShape or CalloutNoArrowShape;
    public bool IsArrowShapeSelected => SelectedShape is ArrowShape;
    public bool IsHighlighterShapeSelected => SelectedShape is HighlighterShape;

    /// <summary>The current highlighter color used for new shapes and for the selected shape.</summary>
    public Color SelectedHighlighterColor
    {
        get => SelectedShape is HighlighterShape h ? h.FillColor : _currentHighlighterColor;
        set
        {
            if (SelectedShape is HighlighterShape h)
            {
                h.FillColor = value;
                UpdateHighlighterColorPresetSelection();
                RefreshCanvas();
            }
            _currentHighlighterColor = value;
            _settings.Settings.SelectedHighlighterColorArgb = ColorToUInt(value);
            _settings.Save();
            OnPropertyChanged();
        }
    }

    /// <summary>Used by the highlighter color preset buttons.</summary>
    [RelayCommand]
    private void SetHighlighterColorFromPreset(object? parameter)
    {
        if (parameter is Color color)
            SelectedHighlighterColor = color;
    }

    private void UpdateHighlighterColorPresets()
    {
        HighlighterColorPresetItems.Clear();
        if (SelectedShape is HighlighterShape highlighter)
        {
            var selected = highlighter.FillColor;
            foreach (var c in HighlighterPresetColorsDefault)
            {
                HighlighterColorPresetItems.Add(new HighlighterColorPresetItem
                {
                    Color = c,
                    IsSelected = ColorsEqual(c, selected)
                });
            }
        }
    }

    private void UpdateHighlighterColorPresetSelection()
    {
        var selected = SelectedHighlighterColor;
        foreach (var item in HighlighterColorPresetItems)
            item.IsSelected = ColorsEqual(item.Color, selected);
    }

    private static uint ColorToUInt(Color color)
        => ((uint)color.A << 24) | ((uint)color.R << 16) | ((uint)color.G << 8) | color.B;

    private static Color UIntToColor(uint color)
        => Color.FromArgb(
            (byte)((color >> 24) & 0xFF),
            (byte)((color >> 16) & 0xFF),
            (byte)((color >> 8) & 0xFF),
            (byte)(color & 0xFF));

    public string SelectedTextFontFamily
    {
        get => SelectedShape is CalloutShape c ? c.FontFamily : SelectedShape is CalloutNoArrowShape n ? n.FontFamily : "Arial";
        set
        {
            if (SelectedShape is CalloutShape cs) { cs.FontFamily = value; RefreshCanvas(); }
            if (SelectedShape is CalloutNoArrowShape cn) { cn.FontFamily = value; RefreshCanvas(); }
            OnPropertyChanged();
        }
    }

    public double SelectedTextFontSize
    {
        get => SelectedShape is CalloutShape c ? c.FontSize : SelectedShape is CalloutNoArrowShape n ? n.FontSize : 24;
        set
        {
            if (SelectedShape is CalloutShape cs) { cs.FontSize = value; RefreshCanvas(); }
            if (SelectedShape is CalloutNoArrowShape cn) { cn.FontSize = value; RefreshCanvas(); }
            OnPropertyChanged();
        }
    }

    public bool SelectedTextFontBold
    {
        get => SelectedShape is CalloutShape c ? c.FontBold : SelectedShape is CalloutNoArrowShape n ? n.FontBold : false;
        set
        {
            if (SelectedShape is CalloutShape cs) { cs.FontBold = value; RefreshCanvas(); }
            if (SelectedShape is CalloutNoArrowShape cn) { cn.FontBold = value; RefreshCanvas(); }
            OnPropertyChanged();
        }
    }

    public bool SelectedTextFontItalic
    {
        get => SelectedShape is CalloutShape c ? c.FontItalic : SelectedShape is CalloutNoArrowShape n ? n.FontItalic : false;
        set
        {
            if (SelectedShape is CalloutShape cs) { cs.FontItalic = value; RefreshCanvas(); }
            if (SelectedShape is CalloutNoArrowShape cn) { cn.FontItalic = value; RefreshCanvas(); }
            OnPropertyChanged();
        }
    }

    public Color SelectedArrowColor
    {
        get => SelectedShape is ArrowShape a ? a.StrokeColor : Colors.Red;
        set
        {
            if (SelectedShape is ArrowShape a)
            {
                a.StrokeColor = value;
                UpdateArrowColorPresetSelection();
                RefreshCanvas();
            }
            OnPropertyChanged();
        }
    }

    /// <summary>Used by the color preset buttons; parameter is the Color from CommandParameter.</summary>
    [RelayCommand]
    private void SetArrowColorFromPreset(object? parameter)
    {
        if (parameter is Color color)
            SelectedArrowColor = color;
    }

    private static readonly string[] FontFamilyChoicesDefault = new[]
    {
        "Arial", "Segoe UI", "Calibri", "Times New Roman", "Verdana", "Tahoma", "Georgia", "Courier New"
    };

    /// <summary>Font families for the combo: default list plus current shape's font if not in list (e.g. from loaded project).</summary>
    public IEnumerable<string> FontFamilyChoices
    {
        get
        {
            var current = SelectedTextFontFamily;
            if (string.IsNullOrEmpty(current) || FontFamilyChoicesDefault.Contains(current, StringComparer.OrdinalIgnoreCase))
                return FontFamilyChoicesDefault;
            return new[] { current }.Concat(FontFamilyChoicesDefault);
        }
    }

    private void UpdateCurrentFileNameDisplay()
    {
        if (string.IsNullOrEmpty(_currentFilePath))
        {
            CurrentFileName = null;
            CurrentFilePathTooltip = null;
        }
        else
        {
            CurrentFileName = Path.GetFileNameWithoutExtension(_currentFilePath);
            CurrentFilePathTooltip = _currentFilePath;
        }
    }

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
    private void SelectCalloutNoArrowTool()
    {
        CurrentTool = ToolType.CalloutNoArrow;
        UpdateToolSelection();
    }

    [RelayCommand]
    private void SelectBorderedRectangleTool()
    {
        CurrentTool = ToolType.BorderedRectangle;
        UpdateToolSelection();
    }

    [RelayCommand]
    private void SelectBlurRectangleTool()
    {
        CurrentTool = ToolType.BlurRectangle;
        UpdateToolSelection();
    }

    [RelayCommand]
    private void SelectSelectorTool()
    {
        CurrentTool = ToolType.Selector;
        UpdateToolSelection();
    }

    [RelayCommand]
    private void SelectVerticalCutOutTool()
    {
        CurrentTool = ToolType.VerticalCutOut;
        UpdateToolSelection();
    }

    [RelayCommand]
    private void SelectHorizontalCutOutTool()
    {
        CurrentTool = ToolType.HorizontalCutOut;
        UpdateToolSelection();
    }

    [RelayCommand]
    private void SelectHighlighterTool()
    {
        CurrentTool = ToolType.Highlighter;
        UpdateToolSelection();
    }

    private void UpdateToolSelection()
    {
        IsSelectToolSelected = CurrentTool == ToolType.None;
        IsArrowToolSelected = CurrentTool == ToolType.Arrow;
        IsCalloutToolSelected = CurrentTool == ToolType.Callout;
        IsCalloutNoArrowToolSelected = CurrentTool == ToolType.CalloutNoArrow;
        IsBorderedRectangleToolSelected = CurrentTool == ToolType.BorderedRectangle;
        IsBlurRectangleToolSelected = CurrentTool == ToolType.BlurRectangle;
        IsSelectorToolSelected = CurrentTool == ToolType.Selector;
        IsVerticalCutOutToolSelected = CurrentTool == ToolType.VerticalCutOut;
        IsHorizontalCutOutToolSelected = CurrentTool == ToolType.HorizontalCutOut;
        IsHighlighterToolSelected = CurrentTool == ToolType.Highlighter;
        _editorCanvas?.Focus();
    }

    partial void OnCurrentToolChanged(ToolType value)
    {
        UpdateToolSelection();
    }

    private Avalonia.Controls.TopLevel? _topLevel;
    private string? _currentFilePath;
    private Avalonia.Controls.Window? _mainWindow;
    private IApplicationSettings _settings;

    public RecentProjectsViewModel RecentProjects { get; }

    public ImageEditorViewModel()
    {
        _settings = AllServices.ApplicationSettings;
        RecentProjects = new RecentProjectsViewModel();
        _currentHighlighterColor = UIntToColor(_settings.Settings.SelectedHighlighterColorArgb);
        AllServices.ApplicationEvents.OnDeleteProject += OnDeleteProject;
        AllServices.ApplicationEvents.OnOpenProject += OnOpenProject;
    }

    private async Task OnOpenProject(ProjectFileInfo project)
    {
        await SaveCurrentProject();
        CloseProject();

        _currentFilePath = project.FilePath;
        await LoadCurrentProject();
    }

    private async Task OnDeleteProject(ProjectFileInfo project)
    {
        if (project.IsCurrentFile)
            CloseProject();
    }

    internal string? CurrentProjectFilePath => _currentFilePath;

    public void SetTopLevel(Avalonia.Controls.TopLevel topLevel)
    {
        _topLevel = topLevel;
        if (topLevel is Avalonia.Controls.Window window)
        {
            _mainWindow = window;
        }
    }

    [RelayCommand]
    private async Task CopyToClipboard()
    {
        if (_topLevel is null ||
            _editorCanvas is null)
            return;

        if (_editorCanvas.SelectorRect is not null)
        {
            await ClipboardService.CopyArea(this, _topLevel.Clipboard, _editorCanvas.SelectorRect.Rectangle);
        }
        else if (SelectedShape is not null)
        {
            await ClipboardService.CopySingleShape(this, _topLevel.Clipboard);
        }
        else
        {
            await ClipboardService.CopyAll(this, _topLevel.Clipboard);
        }
    }

    [RelayCommand]
    private async Task PasteFromClipboard()
    {
        if (_topLevel is null ||
            _editorCanvas is null)
            return;

        await ClipboardService.Paste(this, _topLevel.Clipboard);
    }

    [RelayCommand]
    private async Task Export()
    {
        if (_editorCanvas == null || Image == null || _topLevel == null ||
            _currentFilePath is null) return;
        _editorCanvas.ClearSelection();

        await SaveCurrentProject();

        var storageProvider = _topLevel.StorageProvider;
        if (storageProvider == null) return;

        var file = await storageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = LocalizationManager.Instance["Dialog_Export_Title"],
            FileTypeChoices = AllServices.ProjectManager.ExportFileTypeChoices,
            SuggestedFileName = "annotated_image",
        });

        if (file is null)
            return;

        var localFileName = file.TryGetLocalPath();
        if (localFileName is null)
            return;

        if (localFileName.ToLowerInvariant().EndsWith(ProjectManager.Extension))
        {
            await using var stream = await file.OpenWriteAsync();
            await FileHelper.CopyFileAsync(_currentFilePath, stream);

            return;
        }

        var renderedImageFilePath = RecentProjects.ProjectFiles
            .First(x => x.IsCurrentFile)
            .RenderedImageFilePath;

        using var renderedImageStream = File.OpenRead(renderedImageFilePath);

        using var skBitmap = SKBitmap.Decode(renderedImageStream);
        if (skBitmap is null)
            return;

        var extension = Path.GetExtension(localFileName).ToLowerInvariant();
        SKEncodedImageFormat format = extension switch
        {
            ".jpg" or ".jpeg" => SKEncodedImageFormat.Jpeg,
            ".webp" => SKEncodedImageFormat.Webp,
            ".bmp" => SKEncodedImageFormat.Bmp,
            _ => SKEncodedImageFormat.Png
        };

        int quality = (format == SKEncodedImageFormat.Jpeg || format == SKEncodedImageFormat.Webp) ? 90 : 100;

        using var image = SKImage.FromBitmap(skBitmap);
        using var data = image.Encode(format, quality);
        await using var outputStream = await file.OpenWriteAsync();
        data.SaveTo(outputStream);
    }

    [RelayCommand]
    private async Task Import()
    {
        if (_topLevel == null) return;

        try
        {
            var storageProvider = _topLevel.StorageProvider;
            if (storageProvider == null) return;

            var files = await storageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                Title = LocalizationManager.Instance["Dialog_Import_Title"],
                FileTypeFilter = AllServices.ProjectManager.ImportFileTypeFilter,
                AllowMultiple = false
            });

            if (files.Count > 0)
            {
                await ImportByFile(files[0]);
            }
        }
        catch
        {
            // Handle load errors
        }
    }

    public async Task ImportByFile(IStorageFile file)
    {
        if (_topLevel is null)
            return;

        await SaveCurrentProject();
        CloseProject();

        await using var stream = await file.OpenReadAsync();
        var project = await AllServices.ProjectManager.Import(file.Name, stream);
        await FinishCreatingProject(project);
    }

    public void CloseProject()
    {
        _currentFilePath = null;

        Image?.Dispose();
        Image = null;

        Shapes.Clear();
        _editorCanvas?.ClearSelector();
        CurrentTool = ToolType.None;
        UpdateToolSelection();

        foreach (var f in RecentProjects.ProjectFiles)
            f.IsCurrentFile = false;
        UpdateCurrentFileNameDisplay();
    }

    private async Task FinishCreatingProject(ProjectFileInfo project)
    {
        _currentFilePath = project.FilePath;
        await LoadCurrentProject();
        project.IsCurrentFile = true;
        await AllServices.ApplicationEvents.CreatedProject(project);
    }

    public async Task SaveCurrentProject()
    {
        // TODO: move saving out!
        if (_editorCanvas == null || _editorCanvas.Image == null || _currentFilePath is null) return;

        try
        {
            _editorCanvas.ClearSelection();

            var project = new AnnotatorProject
            {
                Version = 1
            };

            // Save base image as Base64 - use canvas image as it may have been modified by cut operations
            using (var imageStream = new MemoryStream())
            {
                _editorCanvas.Image.Save(imageStream);
                project.BaseImageBase64 = Convert.ToBase64String(imageStream.ToArray());
            }

            // Save preview thumbnail in project JSON; full-resolution PNG alongside
            var renderedImage = ProjectRenderer.Render(Image, Shapes, out var _);
            if (renderedImage != null)
            {
                project.PreviewImageBase64 = ProjectRenderer.CreatePreviewImage(renderedImage);

                var pngPath = Path.ChangeExtension(_currentFilePath, ".png");
                await using var pngFileStream = File.Create(pngPath);
                renderedImage.Save(pngFileStream);
            }

            // Save shapes
            foreach (var shape in Shapes)
            {
                var serializableShape = shape.ToSerializableShape();
                project.Shapes.Add(serializableShape);
            }

            // Serialize to JSON and save
            var json = JsonSerializer.Serialize(project, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            await File.WriteAllTextAsync(_currentFilePath, json);

            var projectInfo = this.RecentProjects.ProjectFiles.First(x => x.FilePath == _currentFilePath);
            projectInfo.Thumbnail?.Dispose();
            projectInfo.Thumbnail = null;
            projectInfo.Thumbnail = ProjectRenderer.CreatePreviewImage(project.PreviewImageBase64);
        }
        catch (Exception e)
        {
            // Handle save errors
        }
    }

    private async Task LoadCurrentProject()
    {
        if (_currentFilePath is null)
            return;

        try
        {
            var json = await File.ReadAllTextAsync(_currentFilePath);
            var project = JsonSerializer.Deserialize<AnnotatorProject>(json);

            if (project == null) return;

            // Load base image
            var imageBytes = Convert.FromBase64String(project.BaseImageBase64);
            using (var stream = new MemoryStream(imageBytes))
            {
                Image = new Bitmap(stream);
            }

            // Load shapes
            _editorCanvas?.ClearSelector();
            _editorCanvas?.Focus();
            Shapes.Clear();
            foreach (var shape in project.Shapes)
            {
                var annotationShape = shape.ToAnnotationShape();
                Shapes.Add(annotationShape);
                if (annotationShape is BlurRectangleShape blurShape)
                {
                    // Set up refresh callback for loaded blur shapes
                    if (_editorCanvas != null)
                    {
                        blurShape.RefreshBlur = rect => _editorCanvas.CreateBlurredImagePublic(rect);
                        blurShape.BlurredImage = _editorCanvas.CreateBlurredImagePublic(blurShape.Rectangle);
                    }
                }
            }

            UpdateCurrentFileNameDisplay();
        }
        catch
        {
            // Handle load errors
        }
    }

    [RelayCommand]
    private async Task NewProject()
    {
        await SaveCurrentProject();
        CloseProject();

        // Create a default canvas with screen dimensions and a light background color
        var screenWidth = 600;
        var screenHeight = 400;

        // Create a bitmap with a light beige/cream background color good for annotations
        var backgroundColor = new Avalonia.Media.Color(255, 245, 245, 240); // Light beige/cream

        using (var bitmap = new Avalonia.Media.Imaging.RenderTargetBitmap(
            new Avalonia.PixelSize(screenWidth, screenHeight),
            new Avalonia.Vector(96, 96)))
        {
            using (var context = bitmap.CreateDrawingContext())
            {
                context.DrawRectangle(
                    new Avalonia.Media.SolidColorBrush(backgroundColor),
                    null,
                    new Avalonia.Rect(0, 0, screenWidth, screenHeight));
            }

            var project = await AllServices.ProjectManager.ImportImage(bitmap.ToStream());
            await FinishCreatingProject(project);
        }
    }

    [RelayCommand]
    private async Task TakeScreenshot()
    {
        if (_mainWindow is null)
            return;

        // Hide the main window
        _mainWindow.WindowState = WindowState.Minimized;

        // Wait a bit for window to hide
        await Task.Delay(300);

        // Capture screenshot
        var screenshot = await ScreenshotService.CaptureScreenshotAsync();
        if (screenshot is null)
            return;

        // Show preview window for area selection
        var previewViewModel = new ScreenshotPreviewViewModel();
        previewViewModel.SetScreenshot(screenshot);

        var previewWindow = new Views.ScreenshotPreviewWindow
        {
            DataContext = previewViewModel
        };

        await previewWindow.ShowDialog(_mainWindow);

        // Check if user confirmed the selection
        if (previewViewModel.CroppedImage == null)
            return;

        await SaveCurrentProject();
        CloseProject();
        var project = await AllServices.ProjectManager.ImportImage(previewViewModel.CroppedImage.ToStream());
        await FinishCreatingProject(project);
     
        _mainWindow.WindowState = WindowState.Normal;
    }

    [RelayCommand]
    private void OpenProjectsFolder() => ProcessHelper.OpenWithShell(AllServices.ProjectManager.ProjectsFolder);
    [RelayCommand]
    private void OpenLogsFolder()
    {
        var folder = LoggingService.GetLogDirectory();
        if (!Directory.Exists(folder))
        {
            Directory.CreateDirectory(folder);
        }
        ProcessHelper.OpenWithShell(folder);
    }

    [RelayCommand]
    private void OpenWebsite() => ProcessHelper.OpenWithShell(ApplicationLinks.AboutUrl);
    [RelayCommand]
    private void OpenLicense() => ProcessHelper.OpenWithShell(ApplicationLinks.LicenseUrl);

    public void SelectShape(AnnotationShape annotationShape)
    {
        if (SelectedShape != null)
        {
            SelectedShape.IsSelected = false;
        }
        SelectedShape = annotationShape;
        annotationShape.IsSelected = true;
    }
    public void AddShape(AnnotationShape annotationShape, bool refreshUi)
    {
        // If we have copied shape(s), paste them with offset
        if (Image is null && _editorCanvas is not null)
            return;

        if (annotationShape is BlurRectangleShape blurRect)
        {
            blurRect.RefreshBlur = rect => _editorCanvas!.CreateBlurredImagePublic(rect);
            blurRect.BlurredImage = _editorCanvas!.CreateBlurredImagePublic(blurRect.Rectangle);
        }
        
        Shapes.Add(annotationShape);

        if (refreshUi)
            RefreshCanvas();
    }
}
