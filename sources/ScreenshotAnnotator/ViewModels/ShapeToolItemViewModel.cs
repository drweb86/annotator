using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ScreenshotAnnotator.Interop.Shapes;

namespace ScreenshotAnnotator.ViewModels;

public partial class ShapeToolItemViewModel : ViewModelBase
{
    public required string TypeId { get; init; }
    public required string DisplayName { get; init; }
    public required int ToolbarOrder { get; init; }
    public Geometry? IconGeometry { get; init; }
    public Color IconStroke { get; init; } = Color.Parse("#D8DEE3");
    public Color IconFill { get; init; } = Colors.Transparent;
    public double IconStrokeThickness { get; init; } = 2;
    public bool HasIcon => IconGeometry is not null;

    [ObservableProperty]
    private bool _isSelected;

    public ImageEditorViewModel? Owner { get; set; }

    public static ShapeToolItemViewModel FromPlugin(IShapePlugin plugin)
    {
        if (plugin.ToolbarIcon is not { } icon)
        {
            return new ShapeToolItemViewModel
            {
                TypeId = plugin.TypeId,
                DisplayName = plugin.DisplayName,
                ToolbarOrder = plugin.ToolbarOrder
            };
        }

        return new ShapeToolItemViewModel
        {
            TypeId = plugin.TypeId,
            DisplayName = plugin.DisplayName,
            ToolbarOrder = plugin.ToolbarOrder,
            IconGeometry = ParseGeometry(icon.PathData),
            IconStroke = UIntToColor(icon.StrokeColorArgb),
            IconFill = UIntToColor(icon.FillColorArgb),
            IconStrokeThickness = icon.StrokeThickness
        };
    }

    private static Geometry? ParseGeometry(string pathData)
    {
        if (string.IsNullOrWhiteSpace(pathData))
            return null;

        return StreamGeometry.Parse(pathData);
    }

    private static Color UIntToColor(uint color) =>
        Color.FromArgb(
            (byte)((color >> 24) & 0xFF),
            (byte)((color >> 16) & 0xFF),
            (byte)((color >> 8) & 0xFF),
            (byte)(color & 0xFF));

    [RelayCommand]
    private void Select()
    {
        Owner?.SelectShapeTool(TypeId);
    }
}
