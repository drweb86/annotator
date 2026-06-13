using ScreenshotAnnotator.Interop.Logging;
using ScreenshotAnnotator.Interop.Shapes;
using ScreenshotAnnotator.Services;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;

namespace ScreenshotAnnotator.Services.Shapes;

public static class ShapePluginLoader
{
    private static bool _initialized;

    public static void Initialize()
    {
        if (_initialized)
            return;

        BuiltInShapeRegistration.RegisterAll();

        var logger = PluginLogging.GetLogger(nameof(ShapePluginLoader));
        var appBaseDirectory = GetAppBaseDirectory();
        LoadFromFolder(GetPluginsRoot(appBaseDirectory), logger);
        LoadFromFolder(GetUserPluginsRoot(), logger);

        _initialized = true;
    }

    public static string GetPluginsRoot(string appBaseDirectory)
        => Path.Combine(appBaseDirectory, "plugins", "shapes");

    public static string GetUserPluginsRoot()
    {
        var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        return Path.Combine(appDataPath, CopyrightInfo.ApplicationId, "plugins", "shapes");
    }

    private static string GetAppBaseDirectory()
    {
        var location = Assembly.GetExecutingAssembly().Location;
        return Path.GetDirectoryName(location) ?? AppContext.BaseDirectory;
    }

    public static void LoadFromFolder(string shapesRoot, IPluginLogger logger)
    {
        if (!Directory.Exists(shapesRoot))
            return;

        foreach (var pluginDirectory in Directory.GetDirectories(shapesRoot))
        {
            try
            {
                LoadFromPluginDirectory(pluginDirectory, logger);
            }
            catch (Exception ex)
            {
                logger.Error($"Failed to load shape plugins from '{pluginDirectory}'.", ex);
            }
        }
    }

    private static void LoadFromPluginDirectory(string pluginDirectory, IPluginLogger logger)
    {
        var pluginName = Path.GetFileName(pluginDirectory);
        var dllPaths = Directory.GetFiles(pluginDirectory, "*.dll", SearchOption.TopDirectoryOnly);
        if (dllPaths.Length == 0)
        {
            logger.Warn($"Shape plugin folder '{pluginName}' does not contain any assemblies.");
            return;
        }

        foreach (var dllPath in dllPaths)
        {
            try
            {
                var context = new PluginAssemblyLoadContext(pluginDirectory);
                var assembly = context.LoadFromAssemblyPath(dllPath);
                RegisterPluginsFromAssembly(assembly, pluginName, logger);
            }
            catch (Exception ex)
            {
                logger.Error($"Failed to load shape plugin assembly '{dllPath}'.", ex);
            }
        }
    }

    private static void RegisterPluginsFromAssembly(Assembly assembly, string pluginName, IPluginLogger logger)
    {
        foreach (var type in assembly.GetTypes())
        {
            if (type.IsAbstract || !typeof(IShapePlugin).IsAssignableFrom(type))
                continue;

            if (Activator.CreateInstance(type) is not IShapePlugin plugin)
                continue;

            if (ShapeRegistry.All.Any(p => p.TypeId.Equals(plugin.TypeId, StringComparison.OrdinalIgnoreCase)))
            {
                logger.Warn($"Skipping plugin '{plugin.TypeId}' from '{pluginName}' because it is already registered.");
                continue;
            }

            ShapeRegistry.Register(plugin);
            logger.Info($"Registered custom shape plugin '{plugin.TypeId}' from '{pluginName}'.");
        }
    }

    private sealed class PluginAssemblyLoadContext(string pluginDirectory) : AssemblyLoadContext(isCollectible: false)
    {
        private readonly AssemblyDependencyResolver _resolver = new(ResolveMainAssembly(pluginDirectory));

        private static string ResolveMainAssembly(string pluginDirectory)
        {
            var dll = Directory.GetFiles(pluginDirectory, "*.dll").FirstOrDefault();
            return dll ?? Path.Combine(pluginDirectory, "plugin.dll");
        }

        protected override Assembly? Load(AssemblyName assemblyName)
        {
            var path = _resolver.ResolveAssemblyToPath(assemblyName);
            if (path is not null)
                return LoadFromAssemblyPath(path);

            return null;
        }
    }
}
