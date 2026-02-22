# NumericControls.Tests

Unit tests for the NumericControls library.

## Framework

xUnit 2.9.2 with **Xunit.StaFact** 1.1.11 (for STA thread tests required by WPF controls). Targets `net9.0-windows`.

## Key Test Areas

- **Converters**: `DateToStringConverterTests.cs`, `DoubleToFontFamilyConverterTests.cs`, `MathFunctionTypeToIconConverterTests.cs`, `MathFunctionTypeToNameConverterTests.cs`, `MathFunctionTypeToTooltipConverterTests.cs`
- **Models**: `ParameterTests.cs` - Tests statistical parameter models; `SummaryStatisticTests.cs` - Tests summary statistic calculations
- **Row Items**: `DistributionRowItemTests.cs`, `OrdinateRowItemTests.cs`, `ProbabilityOrdinateRowItemTests.cs` - Tests data row item models
- **MathEditor**: `MathEditorFunctionTests.cs` - Tests math editor function definitions

## How to Run

```
dotnet test tests/NumericControls.Tests/NumericControls.Tests.csproj
```

Requires external Numerics.dll from the sibling `numerics` repo.
