using Avalonia;
using Avalonia.Input;
using Avalonia.Input.Platform;
using Avalonia.Media.Imaging;
using NLog;
using ScreenshotAnnotator.Models;
using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ScreenshotAnnotator.Services;

interface IClipboardService
{
    Task CopySingleShape(IProjectUi projectUi, IClipboard? clipboard);
    Task CopyArea(IProjectUi projectUi, IClipboard? clipboard, Rect area);
    Task CopyAll(IProjectUi projectUi, IClipboard? clipboard);
    Task Paste(IProjectUi projectUi, IClipboard? clipboard);
}

internal class ClipboardService: IClipboardService
{
    private static readonly Logger Logger = LoggingService.GetLogger("ClipboardService");
    private readonly DataFormat<byte[]> _singleShapeAppFormat = DataFormat.CreateBytesApplicationFormat("annotator-single-shape");

    public async Task CopyAll(IProjectUi projectUi, IClipboard? clipboard)
    {
        if (clipboard == null)
            return;

        try
        {
            // First, render the full image with all shapes using the same method as CopyToClipboard
            var image = ProjectRenderer.Render(projectUi.GetBitmap(), projectUi.GetShapes(), out var _);
            if (image is null)
                return;

            await CopyImage(clipboard, image);
        }
        catch (Exception e)
        {
            Logger.Fatal(e);
        }
    }
    public async Task CopyArea(IProjectUi projectUi, IClipboard? clipboard, Rect area)
    {
        if (clipboard == null)
            return;

        try
        {
            // First, render the full image with all shapes using the same method as CopyToClipboard
            var image = ProjectRenderer.Render(projectUi.GetBitmap(), projectUi.GetShapes(), area);
            if (image is null)
                return;

            await CopyImage(clipboard, image);
        }
        catch (Exception e)
        {
            Logger.Fatal(e);
        }
    }

    private static async Task CopyImage(IClipboard clipboard, RenderTargetBitmap image)
    {
        using var stream = new MemoryStream();
        image.Save(stream);
        stream.Position = 0;

        await clipboard.SetValueAsync(DataFormat.Bitmap, new Bitmap(stream));
    }

    public async Task CopySingleShape(IProjectUi projectUi, IClipboard? clipboard)
    {
        if (clipboard is null)
            return;

        var selectedShape = projectUi.GetSelectedShape();
        if (selectedShape is null)
            return;

        try
        {
            await CopyShapeDataForPaste(clipboard, selectedShape);
        }
        catch (Exception e)
        {
            Logger.Fatal(e);
        }
    }

    private async Task CopyShapeDataForPaste(IClipboard clipboard, AnnotationShape annotationShape)
    {
        var clipboardShape = new ClipboardSingleShape { Shape = annotationShape.ToSerializableShape() };

        var dataTransfer = new DataTransfer();
        var contentString = JsonSerializer.Serialize(clipboardShape);
        var contentBytes = Encoding.UTF8.GetBytes(contentString);
        dataTransfer.Add(DataTransferItem.Create(_singleShapeAppFormat, contentBytes));

        await clipboard.SetDataAsync(dataTransfer);
    }

    public async Task Paste(IProjectUi projectUi, IClipboard? clipboard)
    {
        try
        {
            if (clipboard is null)
                return;

            var clipboardData = await clipboard.TryGetDataAsync();
            if (clipboardData is null)
                return;

            var singleShapeContentBytes = await clipboardData.TryGetValueAsync(_singleShapeAppFormat);
            if (singleShapeContentBytes is not null)
            {
                var contentString = Encoding.UTF8.GetString(singleShapeContentBytes);
                var clipboardShape = JsonSerializer.Deserialize<ClipboardSingleShape>(contentString);
                if (clipboardShape is not null)
                {
                    var annotationShape = clipboardShape.Shape.ToAnnotationShape();

                    const double pasteOffsetX = 20;
                    const double pasteOffsetY = 20;
                    var offset = new Vector(pasteOffsetX, pasteOffsetY);

                    annotationShape.Move(offset);

                    projectUi.AddShape(annotationShape, refreshUi: true);
                    projectUi.SetSelectedShape(annotationShape);
                }
            }
        }
        catch (Exception e)
        {
            Logger.Fatal(e);
        }
    }
}
