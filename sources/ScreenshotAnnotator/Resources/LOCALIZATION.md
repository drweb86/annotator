# Localization Guide for Screenshot Annotator

This document explains how to localize Screenshot Annotator into different languages.

## Overview

Screenshot Annotator uses .NET Resource files (.resx) for localization. The default language is English (Strings.resx), and additional languages can be added by creating culture-specific resource files.

## File Structure

```
ScreenshotAnnotator/
├── Resources/
│   ├── Strings.resx              (Default/English)
│   ├── Strings.ru.resx           (Russian example)
│   ├── Strings.de.resx           (German - create as needed)
│   ├── Strings.fr.resx           (French - create as needed)
│   └── LOCALIZATION.md           (This file)
├── Services/
│   └── LocalizationManager.cs    (Localization service)
└── Markup/
    └── LocalizeExtension.cs       (XAML markup extension)
```

## How to Add a New Language

### Step 1: Create a New Resource File

1. Copy `Strings.resx` to a new file named `Strings.[culture].resx`
   - For Russian: `Strings.ru.resx`
   - For German: `Strings.de.resx`
   - For French: `Strings.fr.resx`
   - For Spanish: `Strings.es.resx`
   - For Chinese (Simplified): `Strings.zh-CN.resx`

2. The culture code follows ISO 639-1 (language) and ISO 3166-1 (country) standards.

### Step 2: Translate the Strings

Open the new .resx file and translate all `<value>` elements while keeping:
- The `<data name="...">` attributes unchanged
- Format placeholders like `{0}`, `{1}` in their correct positions
- Special characters like `✓`, `✕` if they are appropriate for your language

Example for Russian (`Strings.ru.resx`):

```xml
<data name="Window_Title" xml:space="preserve">
  <value>Аннотатор скриншотов - Редактор изображений</value>
  <comment>Main window title</comment>
</data>

<data name="Tool_New" xml:space="preserve">
  <value>Новый</value>
  <comment>New tool button</comment>
</data>
```

### Step 3: Update the Project File

The resource file should be automatically included if it's in the Resources folder, but verify that the .csproj contains:

```xml
<ItemGroup>
  <EmbeddedResource Update="Resources\Strings.*.resx">
    <Generator>PublicResXFileCodeGenerator</Generator>
  </EmbeddedResource>
</ItemGroup>
```

### Step 4: Test the Translation

To test your translation:

1. Change your system's display language to the target language
2. Run the application
3. The LocalizationManager will automatically load the appropriate resource file based on `CultureInfo.CurrentUICulture`

Or programmatically set the culture:

```csharp
LocalizationManager.Instance.CurrentCulture = new CultureInfo("ru");
```

## Resource Keys Reference

### Window & Header
- `Window_Title` - Main window title
- `Header_ScreenshotAnnotator` - Header with version (uses {0} for version)
- `Copyright_Text` - Copyright text (uses {0} for version, {1} for year)

### Menus
- `Menu_File`, `Menu_New`, `Menu_Import`, `Menu_Export`
- `Menu_OpenProjectsFolder`, `Menu_OpenLogsFolder`
- `Menu_WebSite`, `Menu_License`

### Tools
- `Tool_New`, `Tool_Import`, `Tool_Screenshot`
- `Tool_Copy`, `Tool_Paste`, `Tool_Select`
- `Tool_Arrow`, `Tool_Callout`, `Tool_Note`
- `Tool_Border`, `Tool_Highlight`, `Tool_Blur`
- `Tool_CutVertical`, `Tool_CutHorizontal`

### Tooltips
- `Tooltip_*` - Corresponding tooltips for each tool

### Dialogs
- `Dialog_Export_Title`, `Dialog_Import_Title`

### File Types
- `FileType_PNG`, `FileType_JPEG`, `FileType_WebP`
- `FileType_AllSupported`, `FileType_Images`, `FileType_AllFiles`
- `FileType_AnnotatorProject`

### Buttons
- `Button_OK`, `Button_Cancel`, `Button_Accept`, `Button_CancelX`
- `Button_Download`

### Context Menu
- `ContextMenu_Open` - Open project from context menu
- `ContextMenu_Delete` - Delete project from context menu

### Other
- `Panel_Projects` - Projects panel header
- `Screenshot_Instructions` - Screenshot selection instructions
- `Update_NewVersionAvailable` - Update notification (uses {0} for version)
- `Tooltip_CollapseExpandPanel`, `Tooltip_Refresh`

## Format String Placeholders

Some strings contain placeholders that must be preserved:

- `{0}`, `{1}`, etc. - Will be replaced with dynamic values at runtime
- Example: `"New {0} version is available."` → `"Новая версия {0} доступна."`

**Important**: Keep the placeholders in the translated string in the correct position for your language's grammar!

## Comments

Each resource entry includes a `<comment>` tag that provides context for translators. These comments are NOT displayed to users but help explain where and how the string is used.

## Best Practices

1. **Context Matters**: Read the comments to understand where each string appears
2. **Keep it Concise**: UI strings should be short to fit in buttons and menus
3. **Be Consistent**: Use consistent terminology throughout the translation
4. **Test in UI**: Always test translations in the actual UI to check for truncation
5. **Formatting**: Preserve any formatting like line breaks or special characters
6. **Keyboard Shortcuts**: The underscore (_) in menu items indicates keyboard shortcuts (e.g., "Open _Logs Folder" - Alt+L). Translate but keep one underscore per item.

## Example: Adding Russian Translation

Create `Strings.ru.resx` with translated values:

```xml
<?xml version="1.0" encoding="utf-8"?>
<root>
  <!-- ... schema definition same as Strings.resx ... -->

  <data name="Window_Title" xml:space="preserve">
    <value>Аннотатор скриншотов - Редактор изображений</value>
  </data>

  <data name="Tool_New" xml:space="preserve">
    <value>Новый</value>
  </data>

  <data name="Tool_Import" xml:space="preserve">
    <value>Импорт</value>
  </data>

  <!-- ... and so on for all strings ... -->
</root>
```

## Dynamic Language Switching

The application supports dynamic language switching at runtime:

```csharp
// Switch to Russian
LocalizationManager.Instance.CurrentCulture = new CultureInfo("ru");

// Switch to German
LocalizationManager.Instance.CurrentCulture = new CultureInfo("de");
```

All UI elements bound with `{local:Localize Key}` will automatically update when the culture changes.

## Contributing Translations

If you'd like to contribute a translation:

1. Create the appropriate `Strings.[culture].resx` file
2. Translate all string values
3. Test the translation in the application
4. Submit a pull request with your translation file

Thank you for helping make Screenshot Annotator accessible to users worldwide!
