# OxyPlotControls.Tests

Unit tests for the OxyPlotControls library.

## Framework

xUnit 2.9.2 with **Xunit.StaFact** 1.1.11 (for STA thread tests required by WPF). Targets `net9.0-windows`.

## Key Test Areas

- **Serialization** (4 files): `OxyPlotSettingsSerializerTests.cs`, `GeneralPropertiesSerializationTests.cs`, `LegendSerializationTests.cs`, `AxisSerializationTests.cs` - Tests plot settings round-trip serialization
- **Converters** (18 files): Tests OxyPlot-specific converters including `AreaSeriesColor2Converter`, `BarSeriesFillConverter`, `LineSeriesColorConverter`, `OxyAutomaticColorConverter`, `OxyDefaultFontSizeConverter`, `OxyLineStyleToDashArrayConverter`, `ReverseAxisConverter`, `SolidColorBrushConverter`, scatter/line marker fill/stroke converters, alignment converters, etc.
- **Extensions** (5 files): `GetFirstAbstractBaseTypeTests.cs`, `DataPointExtensionsTests.cs`, `ScreenPointExtensionsTests.cs`, `ScreenVectorExtensionsTests.cs`, `VectorExtensionsTests.cs`
- **Axes**: `AxisTypeConversionTests.cs` - Tests axis type conversion logic
- **SavePlotImage**: `SavePlotImageDialogTests.cs` - Tests plot image export dialog

## How to Run

```
dotnet test tests/OxyPlotControls.Tests/OxyPlotControls.Tests.csproj
```

OxyPlot is vendored in this solution at `src/OxyPlot/` (project references).
