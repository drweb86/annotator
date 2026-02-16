using Avalonia;
using Avalonia.Input;
using Avalonia.Input.Platform;
using ScreenshotAnnotator.Models;
using ScreenshotAnnotator.ViewModels;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ScreenshotAnnotator.Services;

enum ClipboardScope
{
    Unknown,
    Global
}

interface IClipboardService
{
    Task CopySingleShape(ImageEditorViewModel imageEditorViewModel, IClipboard? clipboard);
    Task Paste(ImageEditorViewModel imageEditorViewModel, IClipboard? clipboard);
}

internal class ClipboardService: IClipboardService
{
    private readonly DataFormat<byte[]> _singleShapeCopy = DataFormat.CreateBytesApplicationFormat("annotator-data");

    public async Task CopySingleShape(ImageEditorViewModel imageEditorViewModel, IClipboard? clipboard)
    {
        if (clipboard is null)
            return;

        if (ShapeIsSelected(imageEditorViewModel))
        {
            await CopyShapeDataForPaste(clipboard, imageEditorViewModel.SelectedShape!);
        }
    }

    private async Task CopyShapeDataForPaste(IClipboard clipboard, AnnotationShape annotationShape)
    {
        var clipboardShape = new ClipboardSingleShape { Shape = annotationShape.ToSerializableShape() };

        var dataTransfer = new DataTransfer();
        var contentString = JsonSerializer.Serialize(clipboardShape);
        var contentBytes = Encoding.UTF8.GetBytes(contentString);
        dataTransfer.Add(DataTransferItem.Create(_singleShapeCopy, contentBytes));

        await clipboard.SetDataAsync(dataTransfer);
    }

    private bool ShapeIsSelected(ImageEditorViewModel imageEditorViewModel)
    {
        return imageEditorViewModel.SelectedShape != null;
    }

    public async Task Paste(ImageEditorViewModel imageEditorViewModel, IClipboard? clipboard)
    {
        if (clipboard is null)
            return;

        var clipboardData = await clipboard.TryGetDataAsync();
        if (clipboardData is null)
            return;

        var singleShapeContentBytes = await clipboardData.TryGetValueAsync(_singleShapeCopy);
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

                imageEditorViewModel.AddShape(annotationShape, refreshUi: true);
                imageEditorViewModel.SelectShape(annotationShape);
            }
        }
    }
}
