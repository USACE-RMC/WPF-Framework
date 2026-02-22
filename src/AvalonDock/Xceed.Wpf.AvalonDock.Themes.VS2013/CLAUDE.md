# Xceed.Wpf.AvalonDock.Themes.VS2013

VS2013-style theme for AvalonDock with Blue, Dark, and Light variants.

## Purpose

Provides Visual Studio 2013-themed styles and templates for all AvalonDock controls. Three color variants: Blue, Dark, and Light.

## Key Files

- `Vs2013BlueTheme.cs`, `Vs2013DarkTheme.cs`, `Vs2013LightTheme.cs` - Theme classes that return resource URIs.
- `BlueTheme.xaml`, `DarkTheme.xaml`, `LightTheme.xaml` - Top-level theme ResourceDictionaries.
- `BlueBrushs.xaml`, `DarkBrushs.xaml`, `LightBrushs.xaml` - Color brush definitions per variant.
- `Themes/Generic.xaml` - Control templates and styles for all AvalonDock controls (overrides base `generic.xaml`).
- `Themes/IconDictionary.xaml` - Vector icons for context menus and buttons.
- `Themes/Menu/MenuItem.xaml` - Custom menu item styles.
- `Themes/Menu/DarkBrushs.xaml`, `Themes/Menu/LightBrushs.xaml` - Menu-specific brush definitions.
- `Themes/ResourceKeys.cs` - Static resource key definitions.
- `OverlayButtons.xaml` - Dock indicator overlay button styles.

## Dependencies

- `net9.0-windows` (WPF)
- References `Xceed.Wpf.AvalonDock` project

## Gotchas

- **Theme is applied via MergedDictionaries, NOT DockingManager.Theme property.** FrameworkUI adds VS2013 theme dictionaries to `MainWindow.Resources.MergedDictionaries`.
- `Themes/Generic.xaml` in this project overrides the base `generic.xaml` in the core AvalonDock project. The .NET 9 `ContentPresenter` -> `ContentControl` fix must be applied in BOTH files.
- Floating windows need `UpdateThemeResources()` to walk up to the parent Window's resources via `Window.GetWindow(manager)` to find theme dictionaries. Without this, floating windows fall back to base `generic.xaml` which uses `SystemColors.ControlBrushKey` (light grey).
- Use `Canvas > Path` pattern for theme-aware icons instead of `DrawingBrush > GeometryDrawing` (Freezables cannot resolve DynamicResource).
