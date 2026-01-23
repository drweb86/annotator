using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Input.Platform;
using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ScreenshotAnnotator.Models;
using ScreenshotAnnotator.Services;
using SkiaSharp;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.Json;
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

    private Controls.ImageEditorCanvas? _editorCanvas;

    public void SetEditorCanvas(Controls.ImageEditorCanvas canvas)
    {
        _editorCanvas = canvas;
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
    private ApplicationSettings _settings;

    [ObservableProperty]
    private ObservableCollection<ProjectFileInfo> _projectFiles = new();

    [ObservableProperty]
    private bool _isFileBrowserVisible = true;

    public ImageEditorViewModel()
    {
        _settings = ApplicationSettings.Load();
        _isFileBrowserVisible = _settings.IsFileBrowserVisible;
    }

    partial void OnIsFileBrowserVisibleChanged(bool value)
    {
        _settings.IsFileBrowserVisible = value;
        _settings.Save();
    }

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
        if (_editorCanvas == null || Image == null || _topLevel == null) return;

        try
        {
            var renderedImage = _editorCanvas.RenderToImage();
            if (renderedImage == null) return;

            var clipboard = _topLevel.Clipboard;
            if (clipboard == null) return;

            // Save bitmap to PNG stream
            using var stream = new MemoryStream();
            renderedImage.Save(stream);
            stream.Position = 0;

            // Copy as PNG image data
            await clipboard.SetValueAsync(DataFormat.Bitmap, new Bitmap(stream));
        }
        catch
        {
            // Handle clipboard errors silently
        }
    }

    [RelayCommand]
    private async Task PasteFromClipboard()
    {
        if (_topLevel == null) return;

        try
        {
            var clipboard = _topLevel.Clipboard;
            if (clipboard == null) return;

#pragma warning disable CS0618 // Type or member is obsolete
            var formats = await clipboard.GetFormatsAsync();

            // Try to get image data
            if (formats.Contains("image/png"))
            {
                var data = await clipboard.GetDataAsync("image/png");
                if (data is byte[] bytes)
                {
                    using var stream = new MemoryStream(bytes);
                    var bitmap = new Bitmap(stream);
                    Image = bitmap;
                    Shapes.Clear();

                    // Auto-save the pasted image to projects folder
                    var filePath = ProjectManager.GetTimestampedFilePath();
                    _currentFilePath = filePath;
                    UpdateCurrentFileNameDisplay();
                    await SaveCurrentProject();
                    RefreshProjectFiles();
                }
            }
#pragma warning restore CS0618 // Type or member is obsolete
        }
        catch
        {
            // Handle clipboard errors silently
        }
    }

    [RelayCommand]
    private async Task Export()
    {
        if (_editorCanvas == null || Image == null || _topLevel == null ||
            _currentFilePath is null) return;
        await SaveCurrentProject();
        try
        {
            var storageProvider = _topLevel.StorageProvider;
            if (storageProvider == null) return;

            var file = await storageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
            {
                Title = LocalizationManager.Instance["Dialog_Export_Title"],
                FileTypeChoices = new[]
                {
                    new FilePickerFileType(LocalizationManager.Instance["FileType_PNG"]) { Patterns = new[] { "*.png" } },
                    new FilePickerFileType(LocalizationManager.Instance["FileType_JPEG"]) { Patterns = new[] { "*.jpg", "*.jpeg" } },
                    new FilePickerFileType(LocalizationManager.Instance["FileType_WebP"]) { Patterns = new[] { "*.webp" } },

                    ProjectManager.PickerFilter,
                },
                SuggestedFileName = "annotated_image",
            });

            if (file != null)
            {
                var localFileName = file.TryGetLocalPath();
                if (localFileName is not null && localFileName.ToLowerInvariant().EndsWith(ProjectManager.Extension))
                {
                    await using var stream = await file.OpenWriteAsync();
                    await FileHelper.CopyFileAsync(_currentFilePath, stream);

                    return;
                }


                var renderedImage = _editorCanvas.RenderToImage();
                if (renderedImage != null && localFileName != null)
                {
                    var extension = Path.GetExtension(localFileName).ToLowerInvariant();

                    // Convert Avalonia bitmap to SkiaSharp bitmap for encoding
                    using var memStream = new MemoryStream();
                    renderedImage.Save(memStream); // Save as PNG first
                    memStream.Position = 0;

                    using var skBitmap = SKBitmap.Decode(memStream);
                    if (skBitmap != null)
                    {
                        await using var outputStream = await file.OpenWriteAsync();

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
                        data.SaveTo(outputStream);
                    }
                }
            }
        }
        catch
        {
            // Handle save errors
        }
    }

    [RelayCommand]
    private async Task Import()
    {
        if (_topLevel == null) return;

        await SaveCurrentProject();

        try
        {
            var storageProvider = _topLevel.StorageProvider;
            if (storageProvider == null) return;

            var files = await storageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                Title = LocalizationManager.Instance["Dialog_Import_Title"],
                FileTypeFilter = new[]
                {
                    new FilePickerFileType(LocalizationManager.Instance["FileType_AllSupported"])
                    {

                        Patterns = new[] { "*.png", "*.jpg", "*.jpeg", "*.webp", "*.bmp", "*" + ProjectManager.Extension }
                    },
                    ProjectManager.PickerFilter,
                    new FilePickerFileType(LocalizationManager.Instance["FileType_Images"])
                    {

                        Patterns = new[] { "*.png", "*.jpg", "*.jpeg", "*.webp", "*.bmp" }
                    },
                    new FilePickerFileType(LocalizationManager.Instance["FileType_AllFiles"]) { Patterns = new[] { "*.*" } }
                },
                AllowMultiple = false
            });

            if (files.Count > 0)
            {
                _currentFilePath = null;
                var file = files[0];
                var localFileName = file.TryGetLocalPath();
                Shapes.Clear();

                var filePath = ProjectManager.GetTimestampedFilePath();
                _currentFilePath = filePath;
                UpdateCurrentFileNameDisplay();
                if (localFileName is not null && localFileName.ToLowerInvariant().EndsWith(ProjectManager.Extension))
                {
                    await FileHelper.CopyFileAsync(file.Path.LocalPath, filePath);
                    await LoadCurrentProject();
                }
                else
                {
                    await using var stream = await file.OpenReadAsync();
                    Image = new Bitmap(stream);
                }
                await SaveCurrentProject();
                RefreshProjectFiles();
            }
        }
        catch
        {
            // Handle load errors
        }
    }

    private async Task SaveCurrentProject()
    {
        if (_editorCanvas == null || _editorCanvas.Image == null || _currentFilePath is null) return;

        try
        {
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

            // Save preview (with annotations) as Base64
            var renderedImage = _editorCanvas.RenderToImage();
            if (renderedImage != null)
            {
                using var previewStream = new MemoryStream();
                renderedImage.Save(previewStream);
                project.PreviewImageBase64 = Convert.ToBase64String(previewStream.ToArray());
            }

            // Save shapes
            foreach (var shape in Shapes)
            {
                if (shape is ArrowShape arrow)
                {
                    project.Shapes.Add(SerializableArrowShape.FromArrowShape(arrow));
                }
                else if (shape is CalloutShape callout)
                {
                    project.Shapes.Add(SerializableCalloutShape.FromCalloutShape(callout));
                }
                else if (shape is CalloutNoArrowShape calloutNoArrow)
                {
                    project.Shapes.Add(SerializableCalloutNoArrowShape.FromCalloutNoArrowShape(calloutNoArrow));
                }
                else if (shape is BorderedRectangleShape borderedRect)
                {
                    project.Shapes.Add(SerializableBorderedRectangleShape.FromBorderedRectangleShape(borderedRect));
                }
                else if (shape is BlurRectangleShape blurRect)
                {
                    project.Shapes.Add(SerializableBlurRectangleShape.FromBlurRectangleShape(blurRect));
                }
                else if (shape is HighlighterShape highlighter)
                {
                    project.Shapes.Add(SerializableHighlighterShape.FromHighlighterShape(highlighter));
                }
            }

            // Serialize to JSON and save
            var json = JsonSerializer.Serialize(project, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            await File.WriteAllTextAsync(_currentFilePath, json);
        }
        catch
        {
            // Handle save errors
        }
    }

    public async Task AutoSaveCurrentProject()
    {
        // Only autosave if we have a current project file and there are shapes or an image
        if (_currentFilePath != null && _editorCanvas?.Image != null)
        {
            try
            {
                await SaveCurrentProject();

                // Refresh project files to show updated thumbnails
                RefreshProjectFiles();
            }
            catch
            {
                // Silently handle autosave errors
            }
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
                if (shape is SerializableArrowShape arrow)
                {
                    Shapes.Add(arrow.ToArrowShape());
                }
                else if (shape is SerializableCalloutShape callout)
                {
                    Shapes.Add(callout.ToCalloutShape());
                }
                else if (shape is SerializableCalloutNoArrowShape calloutNoArrow)
                {
                    Shapes.Add(calloutNoArrow.ToCalloutNoArrowShape());
                }
                else if (shape is SerializableBorderedRectangleShape borderedRect)
                {
                    Shapes.Add(borderedRect.ToBorderedRectangleShape());
                }
                else if (shape is SerializableBlurRectangleShape blurRect)
                {
                    var blurShape = blurRect.ToBlurRectangleShape();
                    Shapes.Add(blurShape);

                    // Set up refresh callback for loaded blur shapes
                    if (_editorCanvas != null)
                    {
                        blurShape.RefreshBlur = rect => _editorCanvas.CreateBlurredImagePublic(rect);
                        blurShape.BlurredImage = _editorCanvas.CreateBlurredImagePublic(blurShape.Rectangle);
                    }
                }
                else if (shape is SerializableHighlighterShape highlighter)
                {
                    Shapes.Add(highlighter.ToHighlighterShape());
                }
            }
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


        // Create a default canvas with screen dimensions and a light background color
        var screenWidth = 1920;
        var screenHeight = 1080;

        // Try to get actual screen dimensions
        if (_mainWindow?.Screens?.Primary != null)
        {
            var primaryScreen = _mainWindow.Screens.Primary;
            screenWidth = primaryScreen.Bounds.Width;
            screenHeight = primaryScreen.Bounds.Height;
        }

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

            // Convert to Bitmap
            using (var memoryStream = new MemoryStream())
            {
                bitmap.Save(memoryStream);
                memoryStream.Position = 0;
                Image = new Bitmap(memoryStream);
            }
        }

        Shapes.Clear();

        // Reset tool to selector (None) to clear any active tool
        CurrentTool = ToolType.None;
        UpdateToolSelection();

        // Auto-save the new project
        var filePath = ProjectManager.GetTimestampedFilePath();
        _currentFilePath = filePath;
        UpdateCurrentFileNameDisplay();
        await SaveCurrentProject();
        RefreshProjectFiles();
    }

    [RelayCommand]
    private async Task TakeScreenshot()
    {
        if (_mainWindow is null)
            return;

        try
        {
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
            if (previewViewModel.CroppedImage != null)
            {
                await SaveCurrentProject();
                _editorCanvas?.ClearSelector();

                // Use the cropped image
                Image = previewViewModel.CroppedImage;
                Shapes.Clear();

                // Auto-save to projects folder
                var filePath = ProjectManager.GetTimestampedFilePath();
                _currentFilePath = filePath;
                UpdateCurrentFileNameDisplay();
                await SaveCurrentProject();

                // Refresh file list
                RefreshProjectFiles();
            }
        }
        catch
        {
            // Restore window in case of error
        }
        finally
        {
            _mainWindow.WindowState = WindowState.Normal;
        }
    }

    [RelayCommand]
    private void RefreshProjectFiles()
    {
        ProjectFiles.Clear();
        var files = ProjectManager.GetProjectFiles();
        foreach (var file in files)
        {
            // Mark the current file
            file.IsCurrentFile = !string.IsNullOrEmpty(_currentFilePath) &&
                                 file.FilePath.Equals(_currentFilePath, StringComparison.OrdinalIgnoreCase);
            ProjectFiles.Add(file);
        }
    }

    [RelayCommand]
    private async Task OpenProjectFile(ProjectFileInfo? fileInfo)
    {
        if (fileInfo is null) return;

        try
        {
            // Autosave current project before opening a new one
            await AutoSaveCurrentProject();

            // Open as project
            _currentFilePath = fileInfo.FilePath;
            UpdateCurrentFileNameDisplay();
            await LoadCurrentProject();
            RefreshProjectFiles();
        }
        catch
        {
            // Handle errors
        }
    }

    [RelayCommand]
    private void ToggleFileBrowser()
    {
        IsFileBrowserVisible = !IsFileBrowserVisible;
    }

    [RelayCommand]
    private void DeleteProjectFile(ProjectFileInfo? fileInfo)
    {
        if (fileInfo == null) return;
        
        try
        {
            if (fileInfo.IsCurrentFile)
            {
                // close
                _currentFilePath = null;
                Image?.Dispose();
                Image = null;
                Shapes.Clear();
                UpdateCurrentFileNameDisplay();
            }

            if (File.Exists(fileInfo.FilePath))
            {
                File.Delete(fileInfo.FilePath);
                RefreshProjectFiles();
            }
        }
        catch
        {
            // Handle errors
        }
    }

    [RelayCommand]
    private void OpenProjectsFolder()
    {
        try
        {
            var folder = ProjectManager.GetProjectsFolder();
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName = folder,
                UseShellExecute = true
            });
        }
        catch
        {
            // Handle errors
        }
    }

    [RelayCommand]
    private void OpenLogsFolder()
    {
        try
        {
            var folder = LoggingService.GetLogDirectory();
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName = folder,
                UseShellExecute = true
            });
        }
        catch
        {
            // Handle errors
        }
    }

    [RelayCommand]
    private void OpenWebsite()
    {
        try
        {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName = ApplicationLinks.AboutUrl,
                UseShellExecute = true
            });
        }
        catch
        {
            // Handle errors
        }
    }

    [RelayCommand]
    private void OpenLicense()
    {
        try
        {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName = ApplicationLinks.LicenseUrl,
                UseShellExecute = true
            });
        }
        catch
        {
            // Handle errors
        }
    }
}
