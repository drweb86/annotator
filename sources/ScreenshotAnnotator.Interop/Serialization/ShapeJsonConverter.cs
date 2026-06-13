using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using ScreenshotAnnotator.Interop.Shapes;
using ScreenshotAnnotator.Models;

namespace ScreenshotAnnotator.Interop.Serialization;

public sealed class ShapeJsonConverter : JsonConverter<SerializableShape>
{
    public override SerializableShape? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using var document = JsonDocument.ParseValue(ref reader);
        var root = document.RootElement;

        if (!root.TryGetProperty("Type", out var typeElement))
            throw new JsonException("Shape JSON must contain a 'Type' property.");

        var typeId = typeElement.GetString()
            ?? throw new JsonException("Shape 'Type' property must be a string.");

        var plugin = ShapeRegistry.GetRequired(typeId);
        var concreteType = plugin.Serialize(plugin.CreateShape(new ShapeCreationContext { StartPoint = default })).GetType();

        return (SerializableShape?)JsonSerializer.Deserialize(root.GetRawText(), concreteType, options);
    }

    public override void Write(Utf8JsonWriter writer, SerializableShape value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value, value.GetType(), options);
    }
}

public static class ShapeJson
{
    public static JsonSerializerOptions CreateOptions()
    {
        return new JsonSerializerOptions
        {
            WriteIndented = true,
            Converters = { new ShapeJsonConverter() }
        };
    }
}
