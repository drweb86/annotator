using Avalonia;
using Avalonia.Data.Converters;
using System;
using System.Globalization;

namespace ScreenshotAnnotator.Converters;

/// <summary>Converts bool (selected) to border thickness: 2 when true, 1 when false.</summary>
public class BoolToThicknessConverter : IValueConverter
{
    public static readonly BoolToThicknessConverter Instance = new();

    private static readonly Thickness DefaultThickness = new(1);
    private static readonly Thickness SelectedThickness = new(2);

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is true ? SelectedThickness : DefaultThickness;

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotImplementedException();
}
