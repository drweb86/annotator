using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Input.Platform;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Platform.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ScreenshotAnnotator.Models;
using ScreenshotAnnotator.Services;
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
    private bool _isTrimToolSelected;

    [ObservableProperty]
    private string _headerInformation = "Annotator - V" + CopyrightInfo.Version.ToString(3);

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
    private void SelectTrimTool()
    {
        CurrentTool = ToolType.Trim;
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
        IsTrimToolSelected = CurrentTool == ToolType.Trim;
    }

    partial void OnCurrentToolChanged(ToolType value)
    {
        UpdateToolSelection();
    }

    private Avalonia.Controls.TopLevel? _topLevel;
    private string? _currentFilePath;
    private Avalonia.Controls.Window? _mainWindow;

    [ObservableProperty]
    private ObservableCollection<ProjectFileInfo> _projectFiles = new();

    [ObservableProperty]
    private bool _isFileBrowserVisible = true;

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
        await SaveProject();
        try
        {
            var storageProvider = _topLevel.StorageProvider;
            if (storageProvider == null) return;

            var file = await storageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
            {
                Title = "Export",
                FileTypeChoices = new[]
                {
                    ProjectManager.PickerFilter,
                    new FilePickerFileType("PNG Image") { Patterns = new[] { "*.png" } },
                    new FilePickerFileType("JPEG Image") { Patterns = new[] { "*.jpg", "*.jpeg" } },
                    new FilePickerFileType("WebP Image") { Patterns = new[] { "*.webp" } },
                    new FilePickerFileType("BMP Image") { Patterns = new[] { "*.bmp" } }
                },
                SuggestedFileName = "annotated_image",
                DefaultExtension = "png"
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
                if (renderedImage != null)
                {
                    await using var stream = await file.OpenWriteAsync();
                    renderedImage.Save(stream);
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

        if (_currentFilePath is not null)
            await SaveProject();

        try
        {
            var storageProvider = _topLevel.StorageProvider;
            if (storageProvider == null) return;

            var files = await storageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                Title = "Import Project or Image",
                FileTypeFilter = new[]
                {
                    ProjectManager.PickerFilter,
                    new FilePickerFileType("Image Files")
                    {

                        Patterns = new[] { "*.png", "*.jpg", "*.jpeg", "*.webp", "*.bmp" }
                    },
                    new FilePickerFileType("All Files") { Patterns = new[] { "*.*" } }
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
                    await LoadProjectFromFile();
                }
                else
                {
                    await using var stream = await file.OpenReadAsync();
                    Image = new Bitmap(stream);
                    await SaveProject();
                }
                await SaveProject();
                RefreshProjectFiles();
            }
        }
        catch
        {
            // Handle load errors
        }
    }

    [RelayCommand]
    private async Task SaveProject()
    {
        if (_currentFilePath is not null)
        {
            await SaveProjectToFile(_currentFilePath);
        }
    }

    private async Task SaveProjectToFile(string filePath)
    {
        if (_editorCanvas == null || Image == null) return;

        try
        {
            var project = new AnnotatorProject
            {
                Version = 1
            };

            // Save base image as Base64
            using (var imageStream = new MemoryStream())
            {
                Image.Save(imageStream);
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
            }

            // Serialize to JSON and save
            var json = JsonSerializer.Serialize(project, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            await File.WriteAllTextAsync(filePath, json);
        }
        catch
        {
            // Handle save errors
        }
    }

    public async Task AutoSaveCurrentProject()
    {
        // Only autosave if we have a current project file and there are shapes or an image
        if (_currentFilePath != null && Image != null && _currentFilePath is not null)
        {
            try
            {
                await SaveProjectToFile(_currentFilePath);

                // Refresh project files to show updated thumbnails
                RefreshProjectFiles();
            }
            catch
            {
                // Silently handle autosave errors
            }
        }
    }

    private async Task LoadProjectFromFile()
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
        if (_currentFilePath != null)
        {
            await SaveProjectToFile(_currentFilePath);
        }


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

        // Auto-save the new project
        var filePath = ProjectManager.GetTimestampedFilePath();
        _currentFilePath = filePath;
        UpdateCurrentFileNameDisplay();
        await SaveProjectToFile(filePath);
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
                // Use the cropped image
                Image = previewViewModel.CroppedImage;
                Shapes.Clear();

                // Auto-save to projects folder
                var filePath = ProjectManager.GetTimestampedFilePath();
                _currentFilePath = filePath;
                UpdateCurrentFileNameDisplay();
                await SaveProjectToFile(filePath);

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
            await LoadProjectFromFile();
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
