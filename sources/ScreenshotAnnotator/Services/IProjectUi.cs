using Avalonia.Media.Imaging;
using ScreenshotAnnotator.Models;
using System.Collections.Generic;

namespace ScreenshotAnnotator.Services;

internal interface IProjectUi
{
    Bitmap? GetBitmap();


    void AddShape(AnnotationShape annotationShape, bool refreshUi);
    IEnumerable<AnnotationShape> GetShapes();
    AnnotationShape? GetSelectedShape();
    void SetSelectedShape(AnnotationShape annotationShape);
}
