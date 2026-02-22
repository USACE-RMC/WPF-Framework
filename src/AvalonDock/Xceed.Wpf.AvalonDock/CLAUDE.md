# Xceed.Wpf.AvalonDock

Modified Xceed AvalonDock docking framework for WPF. Provides Visual Studio-style dockable panels, documents, and floating windows.

## Purpose

Core docking library providing `DockingManager`, layout models (`LayoutAnchorable`, `LayoutDocument`, `LayoutDocumentPane`, etc.), and controls for drag-and-drop docking, auto-hide panels, tabbed documents, and floating windows.

## Key Files

- `DockingManager.cs` - Main control that hosts the docking layout. Central entry point for the framework.
- `Layout/LayoutRoot.cs` - Root of the layout tree containing panels, document panes, and anchorable panes.
- `Layout/LayoutDocument.cs`, `Layout/LayoutAnchorable.cs` - Content items that can be docked, floated, or auto-hidden.
- `Layout/LayoutDocumentPane.cs` - Container for tabbed documents.
- `Layout/LayoutAnchorablePane.cs` - Container for tool window panels.
- `Layout/LayoutFloatingWindow.cs` - Floating window layout model.
- `Controls/LayoutItem.cs` - Binds layout models to WPF visual tree.
- `Themes/generic.xaml` - Base theme styles (fallback when no theme is applied).
- `Controls/Shell/` - Win32 interop for window chrome, DPI, and native methods.

## Architecture

The layout is a tree structure: `LayoutRoot` > `LayoutPanel` > `LayoutDocumentPaneGroup`/`LayoutAnchorablePaneGroup` > individual panes > content items. The `DockingManager` walks this tree to create corresponding WPF controls.

## Dependencies

- `net9.0-windows` (WPF)
- Strong-named assembly (`sn.snk`)

## Gotchas

- **Floating windows are separate Win32 Window instances** -- they do NOT inherit from the main window's visual tree. Theme resources must be explicitly applied to floating windows.
- **ContentPresenter replaced with ContentControl** in `LayoutItem.View` bindings for .NET 9 compatibility. This fix must be applied in BOTH `Themes/generic.xaml` (this project) AND `Themes/Generic.xaml` (VS2013 theme project).
- `CreateFloatingWindowCore()` has a while loop that removes empty parents -- must protect the last `LayoutDocumentPane` to avoid removing all document panes.
- `GetVisibility()` in `LayoutDocumentPane` must return true for the last empty pane (both fixes needed together).
- `DynamicResource` does NOT work inside `DrawingBrush > DrawingGroup > GeometryDrawing.Brush` (Freezables are outside the visual tree). Use `Canvas > Path` with `Fill="{DynamicResource ...}"` instead.
