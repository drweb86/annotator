using System;
using System.ComponentModel;
using System.Globalization;
using System.Resources;

namespace ScreenshotAnnotator.Services;

/// <summary>
/// Manages application localization and provides access to localized strings.
/// Implements INotifyPropertyChanged to support dynamic language switching.
/// </summary>
public sealed class LocalizationManager : INotifyPropertyChanged
{
    private static readonly Lazy<LocalizationManager> _instance = new(() => new LocalizationManager());
    private readonly ResourceManager _resourceManager;
    private CultureInfo _currentCulture;

    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Gets the singleton instance of the LocalizationManager.
    /// </summary>
    public static LocalizationManager Instance => _instance.Value;

    private LocalizationManager()
    {
        _resourceManager = new ResourceManager(
            "ScreenshotAnnotator.Resources.Strings",
            typeof(LocalizationManager).Assembly);
        _currentCulture = CultureInfo.CurrentUICulture;
    }

    /// <summary>
    /// Gets or sets the current culture for localization.
    /// </summary>
    public CultureInfo CurrentCulture
    {
        get => _currentCulture;
        set
        {
            if (_currentCulture != value)
            {
                _currentCulture = value;
                CultureInfo.CurrentUICulture = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
            }
        }
    }

    /// <summary>
    /// Gets a localized string by its resource key.
    /// </summary>
    /// <param name="key">The resource key.</param>
    /// <returns>The localized string, or the key if not found.</returns>
    public string GetString(string key)
    {
        try
        {
            return _resourceManager.GetString(key, _currentCulture) ?? key;
        }
        catch
        {
            return key;
        }
    }

    /// <summary>
    /// Gets a formatted localized string by its resource key.
    /// </summary>
    /// <param name="key">The resource key.</param>
    /// <param name="args">Format arguments.</param>
    /// <returns>The formatted localized string, or the key if not found.</returns>
    public string GetString(string key, params object[] args)
    {
        try
        {
            var format = _resourceManager.GetString(key, _currentCulture) ?? key;
            return string.Format(_currentCulture, format, args);
        }
        catch
        {
            return key;
        }
    }

    /// <summary>
    /// Indexer for easy access to localized strings.
    /// </summary>
    /// <param name="key">The resource key.</param>
    /// <returns>The localized string.</returns>
    public string this[string key] => GetString(key);
}
