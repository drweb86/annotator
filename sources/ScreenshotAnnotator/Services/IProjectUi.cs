using Avalonia.Media.Imaging;
using ScreenshotAnnotator.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ScreenshotAnnotator.Services;

internal interface IProjectUi
{
    Task CreateNewFromBitmap(Bitmap bitmap);

    Bitmap? GetBitmap();

    void AddShape(AnnotationShape annotationShape, bool refreshUi);
    IEnumerable<AnnotationShape> GetShapes();
    AnnotationShape? GetSelectedShape();
    void SetSelectedShape(AnnotationShape annotationShape);
}
