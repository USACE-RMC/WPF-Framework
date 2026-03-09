# DatabaseControls.Demo

Demo application showcasing the DatabaseControls library.

## Purpose

Demonstrates WPF controls for database table viewing and editing, including SQLite database integration and data grid visualization.

## How to Run

```
dotnet run --project src/DatabaseControls.Demo/DatabaseControls.Demo.csproj
```

Requires external DLL reference for Numerics from sibling repo. OxyPlot is vendored in-solution.

## What It Demonstrates

- Database table viewing and editing controls
- SQLite database integration
- Integration with GenericControls, OxyPlotControls, and Themes

## Known Issue

This project has a pre-existing build error (missing MainWindow.xaml) -- ignore it.
