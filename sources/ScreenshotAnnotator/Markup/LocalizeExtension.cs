using System;
using System.Globalization;
using Avalonia;
using Avalonia.Data;
using Avalonia.Markup.Xaml;
using ScreenshotAnnotator.Resources;

namespace ScreenshotAnnotator.Markup;

/// <summary>
/// XAML markup extension for localized strings.
/// Usage: {local:Localize Key=ResourceKey}
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

        return Strings.ResourceManager.GetString(Key, CultureInfo.CurrentUICulture) ?? Key;
    }
}
