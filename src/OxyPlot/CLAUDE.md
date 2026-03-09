# OxyPlot (Vendored)

Vendored fork of [oxyplot/oxyplot](https://github.com/oxyplot/oxyplot) with significant custom modifications. Maintained in-repo alongside AvalonDock.

## Projects

| Project | Target | Purpose |
|---------|--------|---------|
| OxyPlot | net9.0 | Core plotting library (platform-agnostic) |
| OxyPlot.Wpf | net9.0-windows | WPF PlotView control and rendering + serialization |
| OxyPlot.Wpf.Shared | net9.0-windows | Shared WPF base classes |

## Key Custom Additions (vs upstream OxyPlot)

- **Serialization** (`OxyPlot.Wpf/Serialization/`): PlotSerializer, AxisSerializer, SeriesSerializer, AnnotationSerializer, SerializerExtensions — full XML round-trip for plot configurations
- **Property change events** in Plot.cs and PlotView.cs
- **Enhanced rendering** in DrawingVisualRenderContext

## Dependencies

- All three projects are strong-name signed (`.snk` keys preserved)
- OxyPlot.Wpf depends on OxyPlot.Wpf.Shared and OxyPlot
- OxyPlot.Wpf.Shared depends on OxyPlot
- No NuGet package dependencies

## Consumed By

- OxyPlotControls (UI controls for plot configuration)
- DatabaseControls (chart rendering in table viewer)
- Their respective demos and tests
