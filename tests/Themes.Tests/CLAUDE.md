# Themes.Tests

Unit tests for the Themes library.

## Framework

xUnit 2.9.2 with coverlet for code coverage. Targets `net9.0-windows` (WPF).

## Key Test Areas

- **Converters**: `CutoffConverterTests.cs` - Tests the cutoff value converter; `TabSizeConverterTests.cs` - Tests tab size calculation converter
- **Core**: `ThemeTests.cs` - Tests theme enumeration and properties; `ThemeChangedEventArgsTests.cs` - Tests theme changed event arguments; `ThemeResourceHelperTests.cs` - Tests theme resource dictionary loading and switching

## How to Run

```
dotnet test tests/Themes.Tests/Themes.Tests.csproj
```
