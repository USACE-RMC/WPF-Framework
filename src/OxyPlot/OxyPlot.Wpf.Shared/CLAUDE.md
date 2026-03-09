# OxyPlot.Wpf.Shared

Vendored fork of OxyPlot's shared WPF base classes. Provides the `PlotViewBase` foundation, converters, tracker overlay, theme resources, and export extension methods shared between WPF PlotView implementations.

## Key Files
- `PlotViewBase.cs` / `PlotViewBase.Properties.cs` / `PlotViewBase.Events.cs` - Base class for WPF plot views with dependency properties and input event handling
- `Converters/` - OxyColor, thickness, and alignment converters for WPF bindings
- `Tracker/` - Tracker overlay control for interactive data point inspection
- `Themes/` - Default OxyPlot theme resource dictionary
- `PlotCommands.cs` - Routed commands for plot interaction (zoom, pan, reset)
- `ExporterExtensions.cs` - Extension methods for exporting PlotModel to file/stream

## Dependencies
- **OxyPlot** (project reference) - Core plotting library

## Notes
- Strong-name signed (`OxyPlot.Wpf.Shared.snk`); LangVersion 8
- Targets `net10.0-windows` with WPF enabled
