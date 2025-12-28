# Screenshot Annotator - Image Editor Guide

## Overview
This is a complete image editing tool built with Avalonia UI that allows you to:
- Load and display images
- Draw arrows
- Create callouts with beaks (speech bubbles)
- Trim/crop images
- Clear all annotations

## Features

### Toolbar
The toolbar at the top contains the following tools:

1. **Load Image** (+ icon) - Loads a sample gradient image (can be extended to load from file)
2. **Select/Move Tool** (cursor icon) - Click to select shapes and drag to move them
3. **Arrow Tool** - Draw arrows to point at specific areas
4. **Callout Tool** - Create speech bubble callouts with automatic beak positioning
5. **Trim/Crop Tool** - Select an area to trim the image
6. **Clear All** (trash icon) - Remove all annotations
7. **Apply Trim** (checkmark icon) - Apply the trim operation (enabled only when trim tool is selected)

### How to Use

#### Selecting and Moving Shapes
1. Click the **Select/Move Tool** button (cursor icon) in the toolbar
2. Click on any existing arrow or callout to select it
3. Selected shapes will turn **blue** and show **selection handles**
4. Click and drag a selected shape to move it anywhere on the canvas
5. Click on empty space to deselect

#### Moving Callout Beaks
1. Select a callout using the Select/Move Tool
2. The callout's beak will display an **orange handle**
3. Click and drag the orange handle to reposition the beak independently
4. The beak can be moved to any position, and the callout will automatically adjust which side the beak appears on

#### Drawing Arrows
1. Click the Arrow Tool button in the toolbar
2. Click and drag on the canvas to draw an arrow
3. Release the mouse to complete the arrow
4. The arrow will have an arrowhead at the end point
5. Switch to Select/Move Tool to reposition the arrow later

#### Creating Callouts
1. Click the Callout Tool button in the toolbar
2. Click and drag to create a rectangular callout box
3. The beak (pointer) will automatically position itself below the rectangle
4. The beak intelligently chooses which side of the rectangle to appear on (top, bottom, left, or right)
5. Callouts have a white background with a border
6. After creating, use Select/Move Tool to adjust position or beak placement

#### Trimming/Cropping
1. Click the Trim/Crop Tool button in the toolbar
2. Click and drag to select the area you want to keep
3. The area outside the selection will be darkened to show what will be removed
4. The selection rectangle has corner handles for visualization
5. Click "Apply Trim" to crop the image (to be implemented)

#### Clearing Annotations
- Click the Clear All button (trash icon) to remove all arrows and callouts
- Note: This does not affect the trim selection

## Project Structure

### Files Created
- `Models/AnnotationShape.cs` - Base classes for all annotation shapes (Arrow, Callout, TrimRectangle)
- `Controls/ImageEditorCanvas.cs` - Custom canvas control that handles mouse input and rendering
- `ViewModels/ImageEditorViewModel.cs` - ViewModel managing tools and commands
- `Views/ImageEditorView.axaml` - Main UI layout with toolbar and canvas
- `Views/ImageEditorView.axaml.cs` - Code-behind for the view

### Key Classes

#### AnnotationShape (Abstract Base)
- `ArrowShape` - Draws an arrow from start to end point with arrowhead
- `CalloutShape` - Draws a rectangular callout with intelligent beak positioning
- `TrimRectangle` - Displays the trim selection with corner handles

#### ImageEditorCanvas
Custom control that:
- Displays the image
- Handles mouse events for drawing
- Renders all annotation shapes
- Manages the current tool state

#### ImageEditorViewModel
Manages:
- Current tool selection
- Collection of annotation shapes
- Commands for tool selection and actions
- Image loading

## Customization

### Changing Colors
Edit the stroke colors in `Models/AnnotationShape.cs`:
```csharp
public Color StrokeColor { get; set; } = Colors.Red; // Change to your preferred color
```

### Changing Line Thickness
Edit the stroke thickness:
```csharp
public double StrokeThickness { get; set; } = 2.0; // Change to your preferred thickness
```

### Adding More Tools
1. Add a new tool type to the `ToolType` enum in `AnnotationShape.cs`
2. Create a new shape class inheriting from `AnnotationShape`
3. Add handling in `ImageEditorCanvas` for mouse events
4. Add a button to the toolbar in `ImageEditorView.axaml`
5. Add a command and selection property in `ImageEditorViewModel.cs`

## Running the Application

Build and run the Desktop project:
```bash
cd ScreenshotAnnotator.Desktop
dotnet run
```

The application window will open with:
- A toolbar at the top with icon buttons for each tool
- A white canvas area for the image editor
- Click "Load Image" to load a sample image and start editing

## Future Enhancements

Potential improvements:
1. File picker integration to load actual images
2. Save functionality to export the annotated image
3. Undo/Redo support
4. Additional tools (rectangle, ellipse, text, highlight)
5. Color picker for annotations
6. Thickness/size controls
7. Actual crop implementation that modifies the image
8. Keyboard shortcuts for tools
9. Touch/stylus support for tablets
