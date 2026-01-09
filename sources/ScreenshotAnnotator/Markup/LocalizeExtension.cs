using System;
using System.ComponentModel;
using Avalonia;
using Avalonia.Data;
using Avalonia.Markup.Xaml;
using Avalonia.Markup.Xaml.MarkupExtensions;
using ScreenshotAnnotator.Services;

namespace ScreenshotAnnotator.Markup;

/// <summary>
/// XAML markup extension for localized string bindings.
/// Usage: {local:Localize Key=ResourceKey}
/// Supports dynamic language switching through INotifyPropertyChanged.
/// </summary>
public class LocalizeExtension : MarkupExtension
{
    public LocalizeExtension()
    {
    }

    public LocalizeExtension(string key)
    {
        Key = key;
    }

    /// <summary>
    /// Gets or sets the resource key for the localized string.
    /// </summary>
    public string Key { get; set; } = string.Empty;

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        if (string.IsNullOrEmpty(Key))
        {
            return new BindingNotification(new ArgumentException("Key cannot be empty"), BindingErrorType.Error);
        }

        var localizationManager = LocalizationManager.Instance;

        // Create a binding to the LocalizationManager that updates when culture changes
        var binding = new ReflectionBindingExtension($"[{Key}]")
        {
            Source = localizationManager,
            Mode = BindingMode.OneWay
        };

        return binding.ProvideValue(serviceProvider);
    }
}
