using Avalonia.Data.Converters;
using Avalonia.Media;
using System;
using System.Globalization;

namespace ScreenshotAnnotator.Converters;

/// <summary>Converts bool (selected) to border brush: White when true, #666 when false.</summary>
public class BoolToBorderBrushConverter : IValueConverter
{
    public static readonly BoolToBorderBrushConverter Instance = new();

    private static readonly SolidColorBrush DefaultBrush = new(Color.FromRgb(0x66, 0x66, 0x66));
    private static readonly SolidColorBrush SelectedBrush = new(Colors.White);

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is true ? SelectedBrush : DefaultBrush;

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotImplementedException();
}
