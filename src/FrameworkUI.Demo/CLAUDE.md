# FrameworkUI.Demo

Demo application for the full WPF framework, showcasing project management, AvalonDock layout, theming, and integrated controls.

## Purpose

Demonstrates the complete FrameworkUI framework including dockable panels, theme switching (Light/Blue/Dark), and integration of all control libraries (GenericControls, NumericControls, OxyPlotControls, DatabaseControls, ExpressionParserControls).

## How to Run

```
dotnet run --project src/FrameworkUI.Demo/FrameworkUI.Demo.csproj
```

Requires external DLL references for OxyPlot and Numerics (built separately from sibling repos).

## Dependencies

References nearly all framework projects: FrameworkUI, FrameworkInterfaces, GenericControls, NumericControls, OxyPlotControls, DatabaseControls, ExpressionParserControls, Themes, SoftwareUpdate. Post-build target moves DLLs to a `libraries` subfolder.
