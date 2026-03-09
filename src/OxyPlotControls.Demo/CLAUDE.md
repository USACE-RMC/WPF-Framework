# OxyPlotControls.Demo

Demo application showcasing the OxyPlotControls library.

## Purpose

Demonstrates OxyPlot charting integration with WPF, including plot toolbars, plot serialization, theme-aware plotting, and property editors for plot customization.

## How to Run

```
dotnet run --project src/OxyPlotControls.Demo/OxyPlotControls.Demo.csproj
```

Requires external DLL references for Numerics and DatabaseManager from sibling repos. OxyPlot is vendored in-solution.

## What It Demonstrates

- OxyPlot chart rendering with toolbar controls
- Plot settings serialization and deserialization
- Theme-aware color converters for plot series
- Embedded USGS sample data (USGS_01134500.xml)
