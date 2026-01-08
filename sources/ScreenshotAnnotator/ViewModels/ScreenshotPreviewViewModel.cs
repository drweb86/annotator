using Avalonia;
using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ScreenshotAnnotator.Services;
using System;

namespace ScreenshotAnnotator.ViewModels;

public partial class ScreenshotPreviewViewModel : ViewModelBase
{
    [ObservableProperty]
    private Bitmap? _screenshot;

    private WriteableBitmap? _screenshotWriteable;

    [ObservableProperty]
    private Rect _selectionRect;

    [ObservableProperty]
    private Point _topLeft;

    [ObservableProperty]
    private Point _topRight;

    [ObservableProperty]
    private Point _bottomLeft;

    [ObservableProperty]
    private Point _bottomRight;

    [ObservableProperty]
    private bool _showMagnifier;

    [ObservableProperty]
    private Point _magnifierPosition;

    [ObservableProperty]
    private Bitmap? _magnifierContent;

    [ObservableProperty]
    private bool _isConfirmed;

    [ObservableProperty]
    private bool _showFloatingButtons;

    [ObservableProperty]
    private Point _floatingButtonsPosition;

    public Bitmap? CroppedImage { get; private set; }

    public ScreenshotPreviewViewModel()
    {
    }

    public void SetScreenshot(Bitmap screenshot)
    {
        Screenshot = screenshot;

        // Convert to WriteableBitmap if needed
        if (screenshot is WriteableBitmap wb)
        {
            _screenshotWriteable = wb;
        }
        else
        {
            // Create a WriteableBitmap copy
            _screenshotWriteable = new WriteableBitmap(
                screenshot.PixelSize,
                screenshot.Dpi,
                Avalonia.Platform.PixelFormat.Bgra8888,
                Avalonia.Platform.AlphaFormat.Premul
            );

            // Copy pixels - we'll need to use a render target for this
            // For now, just use the original if it's not writable
            _screenshotWriteable = screenshot as WriteableBitmap;
        }

        // Initialize selection to full image
        var width = screenshot.PixelSize.Width;
        var height = screenshot.PixelSize.Height;

        SelectionRect = new Rect(0, 0, width, height);
        UpdateAnchorPoints();
    }

    public void UpdateSelectionRect(Rect newRect)
    {
        // Ensure selection is within image bounds
        if (Screenshot == null) return;

        var x = Math.Max(0, Math.Min(newRect.X, Screenshot.PixelSize.Width - 1));
        var y = Math.Max(0, Math.Min(newRect.Y, Screenshot.PixelSize.Height - 1));
        var width = Math.Max(1, Math.Min(newRect.Width, Screenshot.PixelSize.Width - x));
        var height = Math.Max(1, Math.Min(newRect.Height, Screenshot.PixelSize.Height - y));

        SelectionRect = new Rect(x, y, width, height);
        UpdateAnchorPoints();
        UpdateFloatingButtons();
    }

    private void UpdateAnchorPoints()
    {
        TopLeft = SelectionRect.TopLeft;
        TopRight = SelectionRect.TopRight;
        BottomLeft = SelectionRect.BottomLeft;
        BottomRight = SelectionRect.BottomRight;
    }

    private void UpdateFloatingButtons()
    {
        if (Screenshot == null) return;

        // Show buttons if selection has a reasonable size
        ShowFloatingButtons = SelectionRect.Width > 50 && SelectionRect.Height > 50;

        if (ShowFloatingButtons)
        {
            // Position buttons below the selection rectangle, centered
            const double buttonWidth = 200; // Approximate width of both buttons together
            var buttonX = SelectionRect.Center.X - buttonWidth / 2;
            var buttonY = SelectionRect.Bottom + 15;

            // If buttons would go off the bottom, put them above
            if (buttonY + 60 > Screenshot.PixelSize.Height)
            {
                buttonY = SelectionRect.Top - 60;
            }

            // If buttons would go off the right edge, adjust left
            if (buttonX + buttonWidth > Screenshot.PixelSize.Width)
            {
                buttonX = Screenshot.PixelSize.Width - buttonWidth - 10;
            }

            // If buttons would go off the left edge, adjust right
            if (buttonX < 10)
            {
                buttonX = 10;
            }

            FloatingButtonsPosition = new Point(buttonX, buttonY);
        }
    }

    public void UpdateAnchorPoint(string anchor, Point newPosition)
    {
        if (Screenshot == null) return;

        // Clamp position to image bounds
        var x = Math.Max(0, Math.Min(newPosition.X, Screenshot.PixelSize.Width));
        var y = Math.Max(0, Math.Min(newPosition.Y, Screenshot.PixelSize.Height));
        newPosition = new Point(x, y);

        var rect = SelectionRect;

        switch (anchor)
        {
            case "TopLeft":
                rect = new Rect(newPosition, rect.BottomRight);
                break;
            case "TopRight":
                rect = new Rect(new Point(rect.Left, newPosition.Y), new Point(newPosition.X, rect.Bottom));
                break;
            case "BottomLeft":
                rect = new Rect(new Point(newPosition.X, rect.Top), new Point(rect.Right, newPosition.Y));
                break;
            case "BottomRight":
                rect = new Rect(rect.TopLeft, newPosition);
                break;
        }

        // Normalize rectangle (handle inverted selection)
        var normalizedRect = new Rect(
            Math.Min(rect.Left, rect.Right),
            Math.Min(rect.Top, rect.Bottom),
            Math.Abs(rect.Width),
            Math.Abs(rect.Height)
        );

        UpdateSelectionRect(normalizedRect);
    }

    public void HideFloatingButtons()
    {
        ShowFloatingButtons = false;
    }

    public void ShowFloatingButtonsIfValid()
    {
        UpdateFloatingButtons();
    }

    public void UpdateMagnifier(Point position, bool show)
    {
        ShowMagnifier = show;
        MagnifierPosition = position;

        if (show && Screenshot != null)
        {
            // Create magnified view of the area around the cursor
            CreateMagnifierContent(position);
        }
    }

    private void CreateMagnifierContent(Point position)
    {
        if (Screenshot == null) return;

        const int magnifierSize = 200;
        const int magnification = 6;
        const int sourceSize = magnifierSize / magnification;

        // Calculate source region centered on cursor
        var sourceX = (int)Math.Max(0, position.X - sourceSize / 2);
        var sourceY = (int)Math.Max(0, position.Y - sourceSize / 2);
        var sourceWidth = Math.Min(sourceSize, Screenshot.PixelSize.Width - sourceX);
        var sourceHeight = Math.Min(sourceSize, Screenshot.PixelSize.Height - sourceY);

        if (sourceWidth <= 0 || sourceHeight <= 0) return;

        try
        {
            // Create a new bitmap for the scaled version
            var scaledWidth = sourceWidth * magnification;
            var scaledHeight = sourceHeight * magnification;

            var scaledBitmap = new WriteableBitmap(
                new PixelSize(scaledWidth, scaledHeight),
                new Vector(96, 96),
                Avalonia.Platform.PixelFormat.Bgra8888,
                Avalonia.Platform.AlphaFormat.Premul
            );

            // Copy and scale pixels directly from source
            if (_screenshotWriteable == null) return;

            using (var srcLock = _screenshotWriteable.Lock())
            using (var dstLock = scaledBitmap.Lock())
            {
                unsafe
                {
                    var srcPtr = (byte*)srcLock.Address.ToPointer();
                    var dstPtr = (byte*)dstLock.Address.ToPointer();

                    for (int y = 0; y < scaledHeight; y++)
                    {
                        for (int x = 0; x < scaledWidth; x++)
                        {
                            int srcX = sourceX + (x / magnification);
                            int srcY = sourceY + (y / magnification);

                            if (srcX < Screenshot.PixelSize.Width && srcY < Screenshot.PixelSize.Height)
                            {
                                int srcIndex = (srcY * srcLock.RowBytes) + (srcX * 4);
                                int dstIndex = (y * dstLock.RowBytes) + (x * 4);

                                dstPtr[dstIndex] = srcPtr[srcIndex];         // B
                                dstPtr[dstIndex + 1] = srcPtr[srcIndex + 1]; // G
                                dstPtr[dstIndex + 2] = srcPtr[srcIndex + 2]; // R
                                dstPtr[dstIndex + 3] = srcPtr[srcIndex + 3]; // A
                            }
                        }
                    }
                }
            }

            MagnifierContent = scaledBitmap;
        }
        catch
        {
            // Ignore magnifier errors
        }
    }

    [RelayCommand]
    private void Confirm()
    {
        var previewLogger = LoggingService.GetLogger("PreviewLogger");
        if (Screenshot is null)
        {
            previewLogger.Info("Screenshot is null");
            return;
        }
        if (_screenshotWriteable is null)
        {
            previewLogger.Info("_screenshotWriteable is null");
            return;
        }

        try
        {
            // Crop the image to the selection by copying pixels
            var width = (int)SelectionRect.Width;
            var height = (int)SelectionRect.Height;
            var startX = (int)SelectionRect.X;
            var startY = (int)SelectionRect.Y;

            if (startX > 0 && (startX + 1) < _screenshotWriteable.PixelSize.Width)
            {
                startX++;
            }
            if (startX > 0 && (startX + width + 1) < _screenshotWriteable.PixelSize.Width)
            {
                width++;
            }
            if (startY > 0 && (startY + 1) < _screenshotWriteable.PixelSize.Height)
            {
                startY++;
            }
            if (startY > 0 && (startY + height + 1) < _screenshotWriteable.PixelSize.Height)
            {
                height++;
            }

            var bitmap = new WriteableBitmap(
                new PixelSize(width, height),
                new Vector(96, 96),
                Avalonia.Platform.PixelFormat.Bgra8888,
                Avalonia.Platform.AlphaFormat.Premul
            );

            using (var srcLock = _screenshotWriteable.Lock())
            using (var dstLock = bitmap.Lock())
            {
                unsafe
                {
                    var srcPtr = (byte*)srcLock.Address.ToPointer();
                    var dstPtr = (byte*)dstLock.Address.ToPointer();

                    for (int y = 0; y < height; y++)
                    {
                        int srcIndex = ((startY + y) * srcLock.RowBytes) + (startX * 4);
                        int dstIndex = y * dstLock.RowBytes;
                        Buffer.MemoryCopy(srcPtr + srcIndex, dstPtr + dstIndex, width * 4, width * 4);
                    }
                }
            }

            CroppedImage = bitmap;
            IsConfirmed = true;
        }
        catch (Exception ex)
        {
            previewLogger.Fatal(ex);
        }
    }

    [RelayCommand]
    private void Cancel()
    {
        CroppedImage = null;
        IsConfirmed = true; // Set to true to trigger window close
    }
}
