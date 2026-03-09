# WPF Framework

USACE-RMC WPF application framework built on .NET 10.0. Provides a complete desktop application shell with docking layout, theming, undo/redo, and specialized controls.

## Build & Test

```bash
dotnet build WPF-Framework.sln
dotnet test WPF-Framework.sln
```

**External dependencies** (must be built separately):
- `C:\GIT\numerics\` - Numerics library for statistical distributions

## Solution Structure

| Folder | Projects |
|--------|----------|
| Core | FrameworkInterfaces, FrameworkUI, Themes |
| Controls | GenericControls, NumericControls, OxyPlotControls, DatabaseControls, ExpressionParserControls |
| Models | DatabaseManager, ExpressionParser, OxyPlot, OxyPlot.Wpf, OxyPlot.Wpf.Shared |
| Support | SoftwareUpdate, SoftwareUpdate.Updater |
| AvalonDock | Xceed.Wpf.AvalonDock, Xceed.Wpf.AvalonDock.Themes.VS2013 |
| Demos | One demo per control library |
| Tests | 12 test projects (xunit, MSTest, NUnit) + ExampleLibrary |

## Key Architecture Patterns

- **Theme system**: Colors in `src/Themes/Resources/Colors/` (Blue/Dark/LightColors.xaml). Themes applied via `ThemeService.Instance.SetTheme()`. Uses `DynamicResource` bindings throughout.
- **Undo/Redo**: `UndoableStateBridge` and `UndoableCollectionBridge` in FrameworkInterfaces automatically record property/collection changes.
- **AvalonDock**: Modified Xceed AvalonDock with VS2013 theme. Floating windows are separate Win32 windows - they don't inherit MainWindow's visual tree. Theme dictionaries added to `MainWindow.Resources.MergedDictionaries`.
- **OxyPlot**: Vendored fork of oxyplot/oxyplot at `src/OxyPlot/` (same pattern as AvalonDock). Includes custom serialization in `OxyPlot.Wpf.Serialization` namespace (PlotSerializer, AxisSerializer, SerializerExtensions).

## Critical Gotchas

- **DynamicResource in Freezables**: `DynamicResource` does NOT work inside `DrawingBrush > DrawingGroup > GeometryDrawing.Brush`. Use `Canvas > Path` with `Fill="{DynamicResource ...}"` instead.
- **Global implicit TextBlock style**: Overrides `TextBlock.Foreground` set via triggers. Use named styles (`x:Key="BasicTextBlockStyle"`) instead.
- **ContentPresenter in AvalonDock**: Causes visual parent conflicts. Use `ContentControl` for `LayoutItem.View` bindings in AvalonDock templates.
- **DatabaseControls.Demo**: External Numerics DLL must exist at HintPath location.

## Git Workflow

- Commit after completing each logical unit of work
- Use conventional commit messages (e.g. `feat:`, `fix:`, `refactor:`)
- Do not push unless explicitly asked

## VB Projects

Legacy VB.NET projects exist in `src/` but are NOT part of the solution. Ignore them.
