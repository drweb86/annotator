using ScreenshotAnnotator.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ScreenshotAnnotator.Interop.Shapes;

public static class ShapeRegistry
{
    private static readonly Dictionary<string, IShapePlugin> ByTypeId = new(StringComparer.OrdinalIgnoreCase);
    private static readonly Dictionary<Type, IShapePlugin> ByAnnotationType = new();
    private static readonly Dictionary<Type, IShapePlugin> BySerializableType = new();
    private static readonly List<IShapePlugin> AllPlugins = new();

    public static IReadOnlyList<IShapePlugin> All => AllPlugins;

    public static void Register(IShapePlugin plugin)
    {
        ArgumentNullException.ThrowIfNull(plugin);

        if (ByTypeId.ContainsKey(plugin.TypeId))
            throw new InvalidOperationException($"Shape plugin '{plugin.TypeId}' is already registered.");

        ByTypeId[plugin.TypeId] = plugin;
        AllPlugins.Add(plugin);

        var sample = plugin.CreateShape(new ShapeCreationContext { StartPoint = default });
        ByAnnotationType[sample.GetType()] = plugin;

        var sampleDto = plugin.Serialize(sample);
        BySerializableType[sampleDto.GetType()] = plugin;
    }

    public static IShapePlugin GetRequired(string typeId)
    {
        if (!ByTypeId.TryGetValue(typeId, out var plugin))
            throw new KeyNotFoundException($"Shape plugin '{typeId}' is not registered.");

        return plugin;
    }

    public static IShapePlugin? GetForAnnotationShape(AnnotationShape shape)
    {
        return ByAnnotationType.GetValueOrDefault(shape.GetType());
    }

    public static IShapePlugin? GetForSerializableShape(SerializableShape shape)
    {
        if (!string.IsNullOrEmpty(shape.Type) && ByTypeId.TryGetValue(shape.Type, out var byId))
            return byId;

        return BySerializableType.GetValueOrDefault(shape.GetType());
    }

    public static SerializableShape ToSerializableShape(AnnotationShape shape)
    {
        var plugin = GetForAnnotationShape(shape)
            ?? throw new InvalidOperationException($"No plugin registered for shape type '{shape.GetType().FullName}'.");

        return plugin.Serialize(shape);
    }

    public static AnnotationShape ToAnnotationShape(SerializableShape serializableShape)
    {
        var plugin = GetForSerializableShape(serializableShape)
            ?? throw new InvalidOperationException($"No plugin registered for serializable shape type '{serializableShape.Type}'.");

        return plugin.Deserialize(serializableShape);
    }

    public static void Clear()
    {
        ByTypeId.Clear();
        ByAnnotationType.Clear();
        BySerializableType.Clear();
        AllPlugins.Clear();
    }
}
