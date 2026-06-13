using ScreenshotAnnotator.Interop.Serialization;
using ScreenshotAnnotator.Interop.Shapes;
using ScreenshotAnnotator.Models;
using ScreenshotAnnotator.Shapes.Arrow;
using ScreenshotAnnotator.Services.Shapes;
using Xunit;

namespace ScreenshotAnnotator.Tests;

public class ShapeRegistryTests
{
    public ShapeRegistryTests()
    {
        ShapeRegistry.Clear();
        BuiltInShapeRegistration.RegisterAll();
    }

    [Fact]
    public void BuiltInRegistration_IncludesArrowShape()
    {
        var plugin = ShapeRegistry.GetRequired("arrow");
        Assert.Equal("arrow", plugin.TypeId);
    }

    [Fact]
    public void ArrowShape_RoundTripsThroughJson()
    {
        var plugin = ShapeRegistry.GetRequired("arrow");
        var shape = plugin.CreateShape(new ShapeCreationContext
        {
            StartPoint = new Avalonia.Point(1, 2),
            DefaultStrokeColor = Avalonia.Media.Colors.Red
        });
        shape.GetType().GetProperty("EndPoint")!.SetValue(shape, new Avalonia.Point(10, 20));

        var dto = plugin.Serialize(shape);
        var json = System.Text.Json.JsonSerializer.Serialize(dto, ShapeJson.CreateOptions());
        var restoredDto = System.Text.Json.JsonSerializer.Deserialize<SerializableShape>(json, ShapeJson.CreateOptions());

        Assert.NotNull(restoredDto);
        var restoredShape = plugin.Deserialize(restoredDto);
        Assert.IsType<ArrowShape>(restoredShape);
    }
}
