# Custom Shapes

Screenshot Annotator supports custom annotation shapes delivered as plugin assemblies. Custom shapes reference **only** `ScreenshotAnnotator.Interop` and must not reference `ScreenshotAnnotator.Common` (built-in localization/resources).

## Quick start

1. Create a class library targeting `net10.0`.
2. Add a project reference to `ScreenshotAnnotator.Interop`.
3. Implement `IShapePlugin` and register your shape types through the plugin loader folder layout (see below).
4. Copy the built plugin folder to one of the supported locations and restart the app.

## Interop contract

Reference assembly: `ScreenshotAnnotator.Interop`

Key types:

| Type | Purpose |
|------|---------|
| `IShapePlugin` | Entry point: creation, drawing, handles, serialization, cut adjustment |
| `AnnotationShape` | Runtime shape base class |
| `SerializableShape` | JSON DTO base class |
| `ShapeRegistry` | Read-only access at runtime (registration is performed by the host) |
| `PluginLogging` | Access application logs via `IPluginLogger` (no direct NLog dependency) |

Example logging:

```csharp
var log = PluginLogging.GetLogger("MyCompany.MyShape");
log.Info("Plugin initialized");
```

## Plugin folder layout

The host scans **two** roots (in order):

1. **Application folder** (for portable/shipped plugins):  
   `{AppDirectory}/plugins/shapes/{friendly-name}/`
2. **User AppData folder** (for per-user plugins):  
   - Release: `%AppData%/SiarheiKuchuk.ScreenshotAnnotator/plugins/shapes/{friendly-name}/`
   - Debug: `%AppData%/SiarheiKuchuk.ScreenshotAnnotator-DEBUG/plugins/shapes/{friendly-name}/`

Each `{friendly-name}` folder is one plugin package. Place your plugin DLL and **all dependent assemblies** in that folder. The host uses `AssemblyLoadContext` + `AssemblyDependencyResolver` to resolve dependencies from the same directory.

Example:

```
plugins/shapes/my-stamp/
  MyCompany.StampShape.dll
  SomeDependency.dll
```

## Implementing a shape plugin

1. Subclass `AnnotationShape` for rendering/hit-testing.
2. Subclass `SerializableShape` with a unique `Type` string (e.g. `"mystamp"`).
3. Implement `IShapePlugin`:
   - `TypeId` must match the serializable `Type` value.
   - Implement `Serialize` / `Deserialize` for project save/load.
   - Implement drawing helpers used by the canvas (`CreateShape`, `UpdateWhileDrawing`, `IsValidForAdd`, handle drag methods).
4. Build for `net10.0` against the same Avalonia major version as the host.

Optional capability interfaces (in Interop) reduce boilerplate for common behaviors:

- `ICornerResizableShape`, `ITextEditableShape`, `IFillColorShape`, `IHighlighterCornerShape`, `IVerticalCutAdjustable`, `IHorizontalCutAdjustable`

## Shipping with the application

To bundle plugins in a portable ZIP/installer, copy your plugin folder under:

```
plugins/shapes/{friendly-name}/
```

next to the main executable. The host scans this location on startup before loading user AppData plugins.

## Shipping to end users

Distribute a folder per plugin:

```
MyStampShape/
  MyCompany.StampShape.dll
  (any dependency DLLs)
```

Users copy it to:

```
%AppData%/SiarheiKuchuk.ScreenshotAnnotator/plugins/shapes/MyStampShape/
```

(replace `SiarheiKuchuk.ScreenshotAnnotator-DEBUG` when running debug builds).

## Project file save format

Shapes are stored in `.anp` project JSON under `Shapes[]` with a `"Type"` discriminator. Custom types deserialize through the registry-backed `ShapeJsonConverter` as long as the plugin is loaded when the project opens.

## Limitations for custom shapes

- **No access to `ScreenshotAnnotator.Common`** — do not rely on built-in localization RESX files; provide your own display strings in the plugin.
- **Type IDs must be unique** — conflicts with built-in or other plugin IDs are skipped at load time.
- **Same runtime** — target `net10.0` and compatible Avalonia packages.

## Built-in shapes

Built-in shapes live in separate projects (`ScreenshotAnnotator.Shapes.*`) and are registered explicitly at startup via `BuiltInShapeRegistration`. They may reference `ScreenshotAnnotator.Common` for localized tool names.
