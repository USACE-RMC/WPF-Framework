# OxyPlot.Wpf.Tests

Unit tests for the vendored OxyPlot WPF integration library.

## Framework
NUnit 3.14.0 with NSubstitute for mocking. Targets `net10.0-windows` (WPF).

## Key Test Areas
- `PngExporterTests.cs` - Tests PNG export to stream and file
- `PlotViewTests.cs` - Tests WPF PlotView control behavior
- `ExampleLibraryTests.cs` - Renders all ExampleLibrary plot models to verify they don't throw

## How to Run
```
dotnet test tests/OxyPlot.Wpf.Tests/OxyPlot.Wpf.Tests.csproj
```
