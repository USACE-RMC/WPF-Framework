# OxyPlot.Wpf

Vendored fork of OxyPlot's WPF integration. Provides the `PlotView` control, WPF render contexts, image exporters, and custom XML serialization for plot configurations.

## Key Files
- `Plot.cs` / `PlotView.cs` - WPF PlotView control with enhanced property change events
- `DrawingVisualRenderContext.cs` - High-performance WPF render context using DrawingVisual
- `CanvasRenderContext.cs` / `XamlRenderContext.cs` - Alternative WPF render contexts
- `PngExporter.cs` / `SvgExporter.cs` / `XpsExporter.cs` / `XamlExporter.cs` - Image and document exporters
- `Serialization/` - **Custom addition** (not in upstream): PlotSerializer, AxisSerializer, SeriesSerializer, AnnotationSerializer, SerializerExtensions for full XML round-trip of plot configurations

## Dependencies
- **OxyPlot** (project reference) - Core plotting library
- **OxyPlot.Wpf.Shared** (project reference) - Shared WPF base classes

## Notes
- Strong-name signed (`OxyPlot.Wpf.snk`); LangVersion 8
- The `Serialization/` folder is the authoritative location for all OxyPlot serialization logic; `OxyPlotControls.OxyPlotSettingsSerializer` is just a thin wrapper
