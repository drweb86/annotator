using System.Collections.Generic;

namespace ScreenshotAnnotator.Models;

/// <summary>
/// Represents a serializable project that can be saved and loaded
/// </summary>
public class AnnotatorProject
{
    public int Version { get; set; } = 1;
    public string PreviewImageBase64 { get; set; } = "";
    public string BaseImageBase64 { get; set; } = "";
    public List<SerializableShape> Shapes { get; set; } = new();
}
