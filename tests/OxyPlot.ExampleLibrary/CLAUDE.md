# OxyPlot.ExampleLibrary

Collection of example PlotModel instances used for testing and demonstration of OxyPlot capabilities. Not a test project itself -- consumed by OxyPlot.Tests and OxyPlot.Wpf.Tests.

## Key Files
- `Examples.cs` - Reflection-based discovery of all example methods via `[Example]` attribute
- `ExampleInfo.cs` / `Example.cs` - Example metadata and model holder
- `ExampleFlags.cs` - Flags for categorizing examples
- `Series/` - Example models for each series type (line, scatter, bar, box plot, etc.)
- `Axes/` - Example models for each axis type
- `Annotations/` - Example models for annotations
- `Resources/` - Embedded test data (CSV, TSV, images, XML)

## Dependencies
- **OxyPlot** (project reference) - Core plotting library

## Notes
- Namespace is `ExampleLibrary` (not `OxyPlot.ExampleLibrary`)
- Example methods are discovered via `[Examples]` class attribute and `[Example]` method attribute
