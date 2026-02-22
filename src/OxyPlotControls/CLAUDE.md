# OxyPlotControls

## Purpose
WPF controls for configuring and interacting with OxyPlot charts, providing property editors for plot settings, axes, series, annotations, legends, a toolbar with pan/zoom/export, and a save-image dialog.

## Key Files
- `OxyPlotToolbar.xaml.cs` - Main toolbar with pan, zoom, annotation, point editing, and export; implements IDisposable
- `GeneralPlotControl.xaml.cs` - Property editor for plot title, subtitle, background, and plot area styling
- `OxyPlotPropertiesControl.xaml.cs` - Aggregates all property panels (general, axes, series, annotations, legend)
- `OxyPlotSettingsSerializer.cs` - Thin wrapper delegating to `OxyPlot.Wpf.Serialization.PlotSerializer`
- `LegendControl.xaml.cs` - Legend position, placement, and styling editor
- `SavePlotImageDialog.xaml.cs` - Dialog for exporting plot as PNG/SVG with DPI and size options
- `Extensions.cs` - OxyPlot type extension methods and data conversion utilities
- `Axes/AxisControl.xaml.cs` - Single axis property editor (title, range, tick, grid lines)
- `Axes/AxesControl.xaml.cs` - Multi-axis selector and editor container
- `Series/SeriesControl.xaml.cs` - Series property editor with type-specific sub-controls
- `Series/LineSeriesControl.xaml.cs` - Line series style (color, width, markers, dash style)
- `Series/ScatterSeriesControl.xaml.cs` - Scatter series marker and color configuration
- `Series/BarSeriesControl.xaml.cs` - Bar series fill and label configuration
- `Series/BoxPlotSeriesControl.xaml.cs` - Box plot whisker and outlier settings
- `Annotations/AnnotationControl.xaml.cs` - Annotation property editor (line, text, arrow, polygon)
- `Theming/OxyPlotThemeManager.cs` - Applies framework theme colors to OxyPlot plot models
- `Theming/OxyPlotTheme.cs` - Theme definition for OxyPlot colors

## Dependencies
- **GenericControls** (project reference) - ColorPicker, NumericTextBox, property controls
- **Themes** (project reference) - Theme colors and styles
- **OxyPlot.dll** (external) - Core OxyPlot library
- **OxyPlot.Wpf.dll / OxyPlot.Wpf.Shared.dll** (external) - WPF integration
- **DatabaseManager.dll** (external) - Data table support for plot data

## Patterns
- Controls bind to `OxyPlot.Wpf.Plot` via dependency properties (e.g., `PlotProperty`)
- Toolbar tracks theme changes via `_lastAppliedTheme` and reapplies on load (handles inactive tabs)
- Custom cursors loaded from embedded resources (pan hand, zoom, point select)
- Expander style defined in `Expander/ExpanderStyle.xaml` for property panels

## Gotchas
- Serialization lives in the oxyplot repo (`OxyPlot.Wpf.Serialization` namespace), NOT in this project; `OxyPlotSettingsSerializer` is just a pass-through wrapper
- OxyPlot DLLs are referenced from sibling repo (`../../../oxyplot/`) -- must be built first
- OxyPlotToolbar implements IDisposable for cursor cleanup -- callers should dispose or use in `using`
- Theme changes on inactive tabs are deferred; `_lastAppliedTheme` comparison catches missed updates on next load
- Nullable is enabled (`<Nullable>enable</Nullable>`) unlike most other projects in this framework
