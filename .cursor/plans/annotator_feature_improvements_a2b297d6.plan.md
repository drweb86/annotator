---
name: Annotator Feature Improvements
overview: Fix keyboard shortcuts not working on startup, add PrintScreen shortcut, remove blue selection overlay, fix arrow color preview, and add configurable highlighter colors with persistence.
todos:
  - id: fix-shortcuts
    content: A. Move KeyDown handler from ImageEditorView to MainWindow to fix Ctrl+O/S/N not working on startup
    status: completed
  - id: add-printscreen
    content: B. Add Key.PrintScreen as shortcut for TakeScreenshotCommand alongside F12
    status: completed
  - id: remove-blue-selection
    content: C. Remove blue color override in IsSelected rendering across all 5 shape models; keep neutral handles only
    status: completed
  - id: fix-arrow-preview
    content: D. Replace arrow SVG path in color preview with a full arrow (line + head) instead of just beak
    status: completed
  - id: highlighter-fillcolor
    content: E1. Add FillColor property to HighlighterShape and use it in Render()
    status: completed
  - id: highlighter-serialize
    content: E2. Add FillColor to SerializableHighlighterShape for save/load
    status: completed
  - id: highlighter-vm
    content: E3. Add highlighter color presets, commands, and properties to ImageEditorViewModel
    status: completed
  - id: highlighter-ui
    content: E4. Add highlighter color picker panel in ImageEditorView.axaml
    status: completed
  - id: highlighter-canvas
    content: E5. Apply selected highlighter color when creating new HighlighterShape in ImageEditorCanvas
    status: completed
  - id: highlighter-settings
    content: E6. Persist selected highlighter color in ApplicationSettings
    status: completed
isProject: false
---

# Annotator Feature Improvements Plan

## A. Fix Ctrl+O, Ctrl+S, Ctrl+N Not Working After Opening

**Root Cause:** Keyboard shortcuts are handled via `KeyDown` on `ImageEditorView` (a `UserControl`), but focus goes to `ImageEditorCanvas` or menu items on startup. Since `ImageEditorView` is not `Focusable` and sits in the middle of the visual tree, `KeyDown` events from focused child elements may not bubble up to it reliably.

**Fix:** Move keyboard shortcut handling to the `MainWindow` level (the top-level `Window`), which always receives keyboard events regardless of which child has focus.

**Files to change:**

- [MainWindow.axaml.cs](sources/ScreenshotAnnotator/Views/MainWindow.axaml.cs) -- Add `KeyDown` handler that dispatches Ctrl+N/O/S/C/V, F9, F12 to the `ImageEditorViewModel`
- [ImageEditorView.axaml.cs](sources/ScreenshotAnnotator/Views/ImageEditorView.axaml.cs) -- Remove the `this.KeyDown += OnKeyDown` handler and the `OnKeyDown` method (lines 57, 104-153), since it will be handled at window level

The `MainWindow` already has access to the `MainViewModel` via `DataContext`, which contains `ImageEditor` (`ImageEditorViewModel`). The `KeyDown` handler will cast `DataContext` to `MainViewModel` and call commands on `viewModel.ImageEditor`.

---

## B. Add PrintScreen Shortcut for Create Screenshot

**Change:** Add `Key.PrintScreen` alongside the existing `Key.F12` case in the keyboard handler.

**Files to change:**

- [MainWindow.axaml.cs](sources/ScreenshotAnnotator/Views/MainWindow.axaml.cs) -- In the new `KeyDown` handler (from part A), add `Key.PrintScreen` as an additional trigger for `TakeScreenshotCommand`
- [ImageEditorView.axaml](sources/ScreenshotAnnotator/Views/ImageEditorView.axaml) -- Optionally update the menu/tooltip to mention PrintScreen as a shortcut

---

## C. Remove Blue Selection for Selected Shapes

**Current behavior:** When a shape is selected, its stroke color changes to `Colors.Blue` and thickness increases by 1. Selection handles are also blue. This makes the shape look different from its actual color.

**New behavior:** Keep the shape's original color when selected. Only show selection handles (small corner/endpoint squares) to indicate selection. Use a neutral color (e.g., white with dark outline) for handles instead of blue, to be visible against any background.

**Files to change (6 shape models):**

- [ArrowShape.cs](sources/ScreenshotAnnotator/Models/ArrowShape.cs) -- Line 14-15: Remove the `IsSelected ? Colors.Blue : ...` ternary; always use `StrokeColor` and `StrokeThickness`. Change handle brush from `Brushes.Blue` to white-with-dark-border style.
- [CalloutShape.cs](sources/ScreenshotAnnotator/Models/CalloutShape.cs) -- Line 22-23: Same change.
- [CalloutNoArrowShape.cs](sources/ScreenshotAnnotator/Models/CalloutNoArrowShape.cs) -- Line 21-22: Same change.
- [BorderedRectangleShape.cs](sources/ScreenshotAnnotator/Models/BorderedRectangleShape.cs) -- Line 13-14: Same change. Line 40: Change handle brush.
- [BlurRectangleShape.cs](sources/ScreenshotAnnotator/Models/BlurRectangleShape.cs) -- Line 32: Change handle brush.
- [HighlighterShape.cs](sources/ScreenshotAnnotator/Models/HighlighterShape.cs) -- Already uses orange handles; no change needed.

For all shapes, the selection handles will use `Brushes.White` fill with a `Pen(Brushes.Black, 1)` outline to stay visible on any background (similar to how `HighlighterShape` and `ArrowShape` already do it).

---

## D. Fix Arrow Color Preview: Show Full Arrow Instead of Just Beak

**Current behavior:** The arrow color preview in the properties panel uses an SVG path `"M2,2 L18,10 L10,12 L8,20 Z M10,12 L18,18"` which looks like a beak/chevron shape rather than a full arrow.

**Fix:** Replace the SVG path data with one that clearly depicts a full solid arrow (line + arrowhead), for example:

```
M2,14 L14,14 L14,8 L22,16 L14,24 L14,18 L2,18 Z
```

This draws a right-pointing arrow with a shaft and arrowhead, which is more recognizable.

**Files to change:**

- [ImageEditorView.axaml](sources/ScreenshotAnnotator/Views/ImageEditorView.axaml) -- Line 417: Replace the `Path Data` with a proper full arrow path

---

## E. Make Highlighter Background Configurable

This is the most complex feature. Currently the highlighter color is hardcoded to semi-transparent yellow (`Color.FromArgb(100, 255, 255, 0)`) in [HighlighterShape.cs](sources/ScreenshotAnnotator/Models/HighlighterShape.cs) line 17.

### E1. Add FillColor Property to HighlighterShape

- [HighlighterShape.cs](sources/ScreenshotAnnotator/Models/HighlighterShape.cs) -- Add a `public Color FillColor { get; set; }` property with default `Color.FromArgb(100, 255, 255, 0)`. Use it in `Render()` instead of the hardcoded value.

### E2. Serialize/Deserialize FillColor

- [SerializableHighlighterShape.cs](sources/ScreenshotAnnotator/Models/SerializableHighlighterShape.cs) -- Add `FillColor` field (as uint). Map it in `FromHighlighterShape()` and `ToHighlighterShape()`.

### E3. Add Highlighter Color Presets to ViewModel

- [ImageEditorViewModel.cs](sources/ScreenshotAnnotator/ViewModels/ImageEditorViewModel.cs) -- Add:
  - `HighlighterColorPresetItem` class (similar to `ArrowColorPresetItem`)
  - `HighlighterColorPresetItems` observable collection
  - `HighlighterPresetColorsDefault` array with 6-8 popular highlight colors:
    - Semi-transparent Yellow (`#64FFFF00`)
    - Semi-transparent Green (`#6400FF00`)
    - Semi-transparent Cyan (`#6400FFFF`)
    - Semi-transparent Pink (`#64FF69B4`)
    - Semi-transparent Orange (`#64FFA500`)
    - Semi-transparent Lavender (`#64E6E6FA`)
    - Semi-transparent Light Blue (`#6487CEEB`)
    - Semi-transparent Lime (`#6432CD32`)
  - `SelectedHighlighterColor` property
  - `SetHighlighterColorFromPreset` command
  - `IsHighlighterShapeSelected` property
  - `UpdateHighlighterColorPresets()` and `UpdateHighlighterColorPresetSelection()` methods

### E4. Add Highlighter Properties Panel in XAML

- [ImageEditorView.axaml](sources/ScreenshotAnnotator/Views/ImageEditorView.axaml) -- Add a new properties section (similar to the arrow color section at lines 394-424) that shows when a `HighlighterShape` is selected. Include color preset swatches using the rectangle icon.
- [ImageEditorView.axaml.cs](sources/ScreenshotAnnotator/Views/ImageEditorView.axaml.cs) -- Add `OnHighlighterColorPresetPressed` event handler.

### E5. Apply Selected Color When Creating New Highlighters

- [ImageEditorCanvas.cs](sources/ScreenshotAnnotator/Controls/ImageEditorCanvas.cs) -- At line 485-491, when creating a new `HighlighterShape`, set `FillColor` from the ViewModel's current selected highlighter color.

### E6. Persist Selected Highlighter Color in Settings

- [ApplicationSettings.cs](sources/ScreenshotAnnotator/Services/ApplicationSettings.cs) -- Add `SelectedHighlighterColorArgb` property (stored as uint or hex string).
- [ImageEditorViewModel.cs](sources/ScreenshotAnnotator/ViewModels/ImageEditorViewModel.cs) -- Save/load the selected highlighter color via `ApplicationSettings` on change and at startup.

