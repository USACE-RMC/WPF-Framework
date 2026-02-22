# GenericControls.Tests

Unit tests for the GenericControls library.

## Framework

xUnit 2.9.2 with coverlet for code coverage. Targets `net9.0-windows` (WPF).

## Key Test Areas

- **Converters** (16 test files): Tests value converters including DoubleToGridLength, DoubleToThickness, StringToDouble, InRange, TimeText, VectorToPoint, AlwaysVisible, TabSize, and more
- **Controls**: `CopyPasteDataGridTests.cs` - Tests DataGrid copy/paste functionality; `HsvColorTests.cs` - Tests HSV color model; `MessageBoxTests.cs` - Tests custom MessageBox
- **Utilities**: `GeneralMethodsTests.cs` - Tests general helper methods; `NumberFormatHelperTests.cs` - Tests number formatting
- **Validation**: `PropertyRuleTests.cs`, `RangeValidationRuleTests.cs` - Tests validation rules for property and range checking

## How to Run

```
dotnet test tests/GenericControls.Tests/GenericControls.Tests.csproj
```
