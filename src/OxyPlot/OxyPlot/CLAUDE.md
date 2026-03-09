# OxyPlot (Core)

Vendored fork of [oxyplot/oxyplot](https://github.com/oxyplot/oxyplot) -- the platform-agnostic core plotting library.

## Key Folders
- `Axes/` - Axis types (linear, logarithmic, datetime, category, color, probability)
- `Series/` - Series types (line, scatter, bar, box plot, contour, heat map, pie, area, etc.)
- `Annotations/` - Annotation types (line, text, arrow, polygon, rectangle)
- `Rendering/` - Render context interfaces and base implementations
- `PlotModel/` - PlotModel and related classes
- `Svg/`, `Pdf/` - SVG and PDF export
- `Imaging/` - Image encoding (PNG, BMP)

## Dependencies
None. Pure .NET library targeting `net10.0`. Strong-name signed (`OxyPlot.snk`).

## Notes
- This is a vendored fork with custom modifications; do not replace with upstream NuGet packages
- LangVersion is set to 9 (not `latest`)
