# OxyPlot.Tests

Unit tests for the vendored OxyPlot core library.

## Framework
NUnit 3.14.0 with NSubstitute for mocking. Targets `net10.0`.

## Key Test Areas
- `Axes/` - Axis type tests (linear, logarithmic, datetime, category, color)
- `Series/` - Series type tests (line, scatter, bar, box plot, contour, heat map, etc.)
- `Svg/`, `Pdf/` - Export format tests with XSD schema validation
- `Imaging/` - Image encoding tests with test images in `Imaging/TestImages/`
- `Foundation/` - Core model and utility tests
- `Rendering/` - Render context tests

## How to Run
```
dotnet test tests/OxyPlot.Tests/OxyPlot.Tests.csproj
```

## Notes
- 3 known floating-point precision test failures are pre-existing, not regressions
- SVG schema validation uses XSD files in `Schemas/`
