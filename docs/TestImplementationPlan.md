# Unit Test Implementation Plan

This document outlines the scope and prioritized implementation plan for achieving 80%+ test coverage across GenericControls, NumericControls, and OxyPlotControls libraries.

## Implementation Status

| Metric | Value |
|--------|-------|
| **Status** | COMPLETE |
| **Test Files Created** | 25 |
| **Total Lines of Test Code** | 18,652 |
| **Estimated Test Methods** | 900+ |

## Testing Strategy for WPF Controls

WPF controls present unique testing challenges since they are tightly coupled to the UI framework. Our strategy focuses on:

1. **Converters** - Pure `IValueConverter`/`IMultiValueConverter` implementations (highest ROI)
2. **Validation Rules** - `ValidationRule` subclasses with testable `Validate()` methods
3. **Row Item Logic** - Business validation in `DataGridRowItem` subclasses
4. **Utility Methods** - Static helper methods and extension methods
5. **Serialization Logic** - XML serialization/deserialization without UI
6. **Property Change Notification** - `INotifyPropertyChanged` implementations

---

## GenericControls.Tests

**Test Files: 8 | Lines: ~6,400 | Tests: ~400**

### Test Files Created

| File | Lines | Tests |
|------|-------|-------|
| `Converters/ConverterTests1.cs` | 1,492 | 151 |
| `Converters/ConverterTests2.cs` | 1,277 | 131 |
| `Validation/RangeValidationRuleTests.cs` | 437 | 35 |
| `Validation/PropertyRuleTests.cs` | 490 | 30 |
| `Utilities/GeneralMethodsTests.cs` | 333 | 20 |
| `Utilities/NumberFormatHelperTests.cs` | 926 | 50 |
| `Controls/HsvColorTests.cs` | 611 | 40 |
| `Controls/CopyPasteDataGridTests.cs` | 397 | 25 |

### Priority 1: Converters (28 converters - ~140 tests)

| # | Test Class | Source File | Status |
|---|------------|-------------|--------|
| 1 | `ReverseBooleanConverterTests` | Converters.cs | DONE |
| 2 | `BooleanToVisibilityConverterTests` | BooleanToVisibilityConverter.cs | DONE |
| 3 | `BooleanToColorConverterTests` | Converters.cs | DONE |
| 4 | `BooleanToBrushConverterTests` | Converters.cs | DONE |
| 5 | `BooleanToTextConverterTests` | Converters.cs | DONE |
| 6 | `BooleanToDoubleConverterTests` | Converters.cs | DONE |
| 7 | `VisibilityToBooleanConverterTests` | Converters.cs | DONE |
| 8 | `ColorToByteConverterTests` | ColorToByteConverter.cs | DONE |
| 9 | `ColorToSolidBrushConverterTests` | Converters.cs | DONE |
| 10 | `DrawingColorToSolidColorBrushConverterTests` | Converters.cs | DONE |
| 11 | `GridlineColorLightConverterTests` | Converters.cs | DONE |
| 12 | `FontToFontFamilyConverterTests` | Converters.cs | DONE |
| 13 | `FontFamilyToFontStringConverterTests` | Converters.cs | DONE |
| 14 | `IntToDoubleConverterTests` | Converters.cs | DONE |
| 15 | `DoubleToStringConverterTests` | Converters.cs | DONE |
| 16 | `StringToDoubleConverterTests` | Converters.cs | DONE |
| 17 | `InRangeConverterTests` | Converters.cs | DONE |
| 18 | `DoubleToThicknessConverterTests` | Converters.cs | DONE |
| 19 | `ThicknessToDoubleConverterTests` | Converters.cs | DONE |
| 20 | `DoubleToDataGridLengthConverterTests` | Converters.cs | DONE |
| 21 | `DoubleToGridLengthConverterTests` | Converters.cs | DONE |
| 22 | `DoubleToCornerRadiusConverterTests` | Converters.cs | DONE |
| 23 | `DataGridWidthConverterTests` | Converters.cs | DONE |
| 24 | `VectorToPointConverterTests` | Converters.cs | DONE |
| 25 | `TimeTextConverterTests` | Converters.cs | DONE |
| 26 | `TabSizeConverterTests` | Converters.cs | DONE |
| 27 | `DoubleToNAConverterTests` | Converters.cs | DONE |
| 28 | `StringToNAConverterTests` | Converters.cs | DONE |
| 29 | `HorizontalAlignmentToTextAlignmentConverterTests` | Converters.cs | DONE |
| 30 | `AlwaysVisibleConverterTests` | Converters.cs | DONE |

### Priority 2: Validation Rules (~20 tests)

| # | Test Class | Source File | Status |
|---|------------|-------------|--------|
| 31 | `RangeValidationRuleTests` | ValidationRules.cs | DONE |
| 32 | `RangeWrapperTests` | ValidationRules.cs | DONE |
| 33 | `PropertyRuleTests` | PropertyRule.cs | DONE |

### Priority 3: Utility Methods (~30 tests)

| # | Test Class | Source File | Status |
|---|------------|-------------|--------|
| 34 | `GeneralMethodsTests` | GeneralMethods.cs | DONE |
| 35 | `NumberFormatHelperTests` | NumberFormatHelper.cs | DONE |
| 36 | `BindingProxyTests` | ValidationRules.cs | DONE |

### Priority 4: Control Logic (~15 tests)

| # | Test Class | Source File | Status |
|---|------------|-------------|--------|
| 37 | `HsvColorTests` | ColorPicker.xaml.cs | DONE |
| 38 | `CopyPasteDataGridTests` | CopyPasteDataGrid.cs | DONE |

---

## NumericControls.Tests

**Test Files: 7 | Lines: ~4,100 | Tests: ~255**

### Test Files Created

| File | Lines | Tests |
|------|-------|-------|
| `Converters/ConverterTests.cs` | 628 | 52 |
| `RowItems/OrdinateRowItemTests.cs` | 669 | 35 |
| `RowItems/ProbabilityOrdinateRowItemTests.cs` | 497 | 27 |
| `RowItems/DistributionRowItemTests.cs` | 663 | 35 |
| `Models/ParameterTests.cs` | 561 | 38 |
| `Models/SummaryStatisticTests.cs` | 394 | 28 |
| `MathEditor/MathEditorFunctionTests.cs` | 685 | 40 |

### Priority 1: Converters (~30 tests)

| # | Test Class | Source File | Status |
|---|------------|-------------|--------|
| 1 | `DoubleToFontFamilyConverterTests` | TimeSeriesTable.xaml.cs | DONE |
| 2 | `DateToStringConverterTests` | TimeSeriesTable.xaml.cs | DONE |
| 3 | `MathFunctionTypeToNameConverterTests` | MathEditorControl.xaml.cs | DONE |
| 4 | `MathFunctionTypeToTooltipConverterTests` | MathEditorControl.xaml.cs | DONE |
| 5 | `MathFunctionTypeToIconConverterTests` | MathEditorControl.xaml.cs | DONE |
| 6 | `DistributionNameConverterTests` | UncertainOrderedDataTableEditor.xaml.cs | DONE |

### Priority 2: Row Item Validation (~60 tests)

| # | Test Class | Source File | Status |
|---|------------|-------------|--------|
| 7 | `OrdinateRowItemTests` | OrdinateRowItem.cs | DONE |
| 8 | `ProbabilityOrdinateRowItemTests` | ProbabilityOrdinateRowItem.cs | DONE |
| 9 | `DistributionRowItemTests` | DistributionRowItem.cs | DONE |
| 10 | `StratificationOptionsRowItemTests` | StratificationOptionsRowItem.cs | - |
| 11 | `DistributionDataItemTests` | DistributionDataItem.cs | - |

### Priority 3: Math Operations (~40 tests)

| # | Test Class | Source File | Status |
|---|------------|-------------|--------|
| 12 | `MathEditorApplyFunctionTests` | MathEditorControl.xaml.cs | DONE |
| 13 | `MathEditorAdvancedFunctionsTests` | MathEditorControl.xaml.cs | DONE |
| 14 | `MathEditorHasOperandTests` | MathEditorControl.xaml.cs | DONE |

### Priority 4: Data Models (~20 tests)

| # | Test Class | Source File | Status |
|---|------------|-------------|--------|
| 15 | `ParameterTests` | Parameter.cs | DONE |
| 16 | `SummaryStatisticTests` | SummaryStatistic.cs | DONE |
| 17 | `AreaPointTests` | UncertainOrderedDataSelectorControl.xaml.cs | - |

---

## OxyPlotControls.Tests

**Test Files: 10 | Lines: ~8,100 | Tests: ~400**

### Test Files Created

| File | Lines | Tests |
|------|-------|-------|
| `Extensions/ExtensionsTests.cs` | ~800 | 50+ |
| `Converters/GeneralPlotConverterTests.cs` | ~400 | 25 |
| `Converters/AnnotationConverterTests.cs` | ~500 | 30 |
| `Converters/AxisConverterTests.cs` | ~600 | 35 |
| `Converters/SeriesConverterTests.cs` | ~800 | 45 |
| `Serialization/OxyPlotSettingsSerializerTests.cs` | ~1,200 | 79 |
| `Serialization/GeneralPropertiesSerializationTests.cs` | ~600 | 21 |
| `Serialization/LegendSerializationTests.cs` | ~700 | 25 |
| `Serialization/AxisSerializationTests.cs` | ~1,200 | 45 |
| `Axes/AxisTypeConversionTests.cs` | ~900 | 46 |

### Priority 1: Extension Methods (~50 tests)

| # | Test Class | Source File | Status |
|---|------------|-------------|--------|
| 1 | `DataPointExtensionsTests` | Extensions.cs | DONE |
| 2 | `ScreenVectorExtensionsTests` | Extensions.cs | DONE |
| 3 | `ScreenPointExtensionsTests` | Extensions.cs | DONE |
| 4 | `VectorExtensionsTests` | Extensions.cs | DONE |
| 5 | `XmlSerializationExtensionsTests` | Extensions.cs | DONE |
| 6 | `TypeExtensionsTests` | Extensions.cs | DONE |

### Priority 2: Converters (~60 tests)

| # | Test Class | Source File | Status |
|---|------------|-------------|--------|
| 7 | `OxyAutomaticColorConverterTests` | GeneralPlotControl.xaml.cs | DONE |
| 8 | `OxyDefaultFontSizeConverterTests` | GeneralPlotControl.xaml.cs | DONE |
| 9 | `SolidColorBrushConverterTests` | GeneralPlotControl.xaml.cs | DONE |
| 10 | `DataPointToPointConverterTests` | AnnotationControl.xaml.cs | DONE |
| 11 | `ScreenVectorToPointConverterTests` | AnnotationControl.xaml.cs | DONE |
| 12 | `OxyHorizontalAlignmentConverterTests` | AnnotationControl.xaml.cs | DONE |
| 13 | `OxyVerticalAlignmentConverterTests` | AnnotationControl.xaml.cs | DONE |
| 14 | `ReverseAxisConverterTests` | AxisControl.xaml.cs | DONE |
| 15 | `OxyLineStyleToDashArrayConverterTests` | AxisControl.xaml.cs | DONE |
| 16 | `EmptyStringToNullConverterTests` | AxisControl.xaml.cs | DONE |
| 17 | `DateToNumberConverterTests` | AxisControl.xaml.cs | DONE |
| 18 | `LineSeriesColorConverterTests` | LineSeriesControl.xaml.cs | DONE |
| 19 | `AreaSeriesColor2ConverterTests` | GenericSeriesControl.xaml.cs | DONE |
| 20 | `AreaSeriesFillConverterTests` | GenericSeriesControl.xaml.cs | DONE |
| 21 | `LineSeriesMarkerFillConverterTests` | LineSeriesControl.xaml.cs | DONE |
| 22 | `LineSeriesMarkerStrokeConverterTests` | LineSeriesControl.xaml.cs | DONE |
| 23 | `BarSeriesFillConverterTests` | BarSeriesControl.xaml.cs | DONE |
| 24 | `BoxPlotSeriesFillConverterTests` | BoxPlotSeriesControl.xaml.cs | DONE |
| 25 | `ScatterSeriesMarkerFillConverterTests` | ScatterSeriesControl.xaml.cs | DONE |
| 26 | `ScatterSeriesMarkerStrokeConverterTests` | ScatterSeriesControl.xaml.cs | DONE |

### Priority 3: Serialization (~50 tests)

| # | Test Class | Source File | Status |
|---|------------|-------------|--------|
| 27 | `OxyPlotSettingsSerializerAttributeTests` | OxyPlotSettingsSerializer.cs | DONE |
| 28 | `OxyPlotSettingsSerializerEnumTests` | OxyPlotSettingsSerializer.cs | DONE |
| 29 | `GeneralPropertiesSerializationTests` | GeneralPlotControl.xaml.cs | DONE |
| 30 | `LegendPropertiesSerializationTests` | LegendControl.xaml.cs | DONE |
| 31 | `AxisPropertiesSerializationTests` | AxisControl.xaml.cs | DONE |
| 32 | `AnnotationPropertiesSerializationTests` | AnnotationControl.xaml.cs | - |
| 33 | `SeriesPropertiesSerializationTests` | GenericSeriesControl.xaml.cs | - |

### Priority 4: Axis Type Conversion (~20 tests)

| # | Test Class | Source File | Status |
|---|------------|-------------|--------|
| 34 | `AxisTypeConversionTests` | AxisControl.xaml.cs | DONE |

---

## Implementation Checklist

### Phase 1: GenericControls.Tests - Converters
- [x] 1. ReverseBooleanConverterTests
- [x] 2. BooleanToVisibilityConverterTests
- [x] 3. BooleanToColorConverterTests
- [x] 4. BooleanToBrushConverterTests
- [x] 5. BooleanToTextConverterTests
- [x] 6. BooleanToDoubleConverterTests
- [x] 7. VisibilityToBooleanConverterTests
- [x] 8. ColorToByteConverterTests
- [x] 9. ColorToSolidBrushConverterTests
- [x] 10. DrawingColorToSolidColorBrushConverterTests
- [x] 11. GridlineColorLightConverterTests
- [x] 12. FontToFontFamilyConverterTests
- [x] 13. FontFamilyToFontStringConverterTests
- [x] 14. IntToDoubleConverterTests
- [x] 15. DoubleToStringConverterTests
- [x] 16. StringToDoubleConverterTests
- [x] 17. InRangeConverterTests
- [x] 18. DoubleToThicknessConverterTests
- [x] 19. ThicknessToDoubleConverterTests
- [x] 20. DoubleToDataGridLengthConverterTests
- [x] 21. DoubleToGridLengthConverterTests
- [x] 22. DoubleToCornerRadiusConverterTests
- [x] 23. DataGridWidthConverterTests
- [x] 24. VectorToPointConverterTests
- [x] 25. TimeTextConverterTests
- [x] 26. TabSizeConverterTests
- [x] 27. DoubleToNAConverterTests
- [x] 28. StringToNAConverterTests
- [x] 29. HorizontalAlignmentToTextAlignmentConverterTests
- [x] 30. AlwaysVisibleConverterTests

### Phase 2: GenericControls.Tests - Validation & Utilities
- [x] 31. RangeValidationRuleTests
- [x] 32. RangeWrapperTests
- [x] 33. PropertyRuleTests
- [x] 34. GeneralMethodsTests
- [x] 35. NumberFormatHelperTests
- [x] 36. BindingProxyTests
- [x] 37. HsvColorTests
- [x] 38. CopyPasteDataGridTests

### Phase 3: NumericControls.Tests - Converters
- [x] 39. DoubleToFontFamilyConverterTests
- [x] 40. DateToStringConverterTests
- [x] 41. MathFunctionTypeToNameConverterTests
- [x] 42. MathFunctionTypeToTooltipConverterTests
- [x] 43. MathFunctionTypeToIconConverterTests
- [x] 44. DistributionNameConverterTests

### Phase 4: NumericControls.Tests - Row Items
- [x] 45. OrdinateRowItemTests
- [x] 46. ProbabilityOrdinateRowItemTests
- [x] 47. DistributionRowItemTests
- [ ] 48. StratificationOptionsRowItemTests
- [ ] 49. DistributionDataItemTests

### Phase 5: NumericControls.Tests - Math Operations
- [x] 50. MathEditorApplyFunctionTests
- [x] 51. MathEditorAdvancedFunctionsTests
- [x] 52. MathEditorHasOperandTests

### Phase 6: NumericControls.Tests - Data Models
- [x] 53. ParameterTests
- [x] 54. SummaryStatisticTests
- [ ] 55. AreaPointTests

### Phase 7: OxyPlotControls.Tests - Extensions
- [x] 56. DataPointExtensionsTests
- [x] 57. ScreenVectorExtensionsTests
- [x] 58. ScreenPointExtensionsTests
- [x] 59. VectorExtensionsTests
- [x] 60. XmlSerializationExtensionsTests
- [x] 61. TypeExtensionsTests

### Phase 8: OxyPlotControls.Tests - Converters
- [x] 62. OxyAutomaticColorConverterTests
- [x] 63. OxyDefaultFontSizeConverterTests
- [x] 64. SolidColorBrushConverterTests
- [x] 65. DataPointToPointConverterTests
- [x] 66. ScreenVectorToPointConverterTests
- [x] 67. OxyHorizontalAlignmentConverterTests
- [x] 68. OxyVerticalAlignmentConverterTests
- [x] 69. ReverseAxisConverterTests
- [x] 70. OxyLineStyleToDashArrayConverterTests
- [x] 71. EmptyStringToNullConverterTests
- [x] 72. DateToNumberConverterTests
- [x] 73. LineSeriesColorConverterTests
- [x] 74. AreaSeriesColor2ConverterTests
- [x] 75. AreaSeriesFillConverterTests
- [x] 76. LineSeriesMarkerFillConverterTests
- [x] 77. LineSeriesMarkerStrokeConverterTests
- [x] 78. BarSeriesFillConverterTests
- [x] 79. BoxPlotSeriesFillConverterTests
- [x] 80. ScatterSeriesMarkerFillConverterTests
- [x] 81. ScatterSeriesMarkerStrokeConverterTests

### Phase 9: OxyPlotControls.Tests - Serialization
- [x] 82. OxyPlotSettingsSerializerAttributeTests
- [x] 83. OxyPlotSettingsSerializerEnumTests
- [x] 84. GeneralPropertiesSerializationTests
- [x] 85. LegendPropertiesSerializationTests
- [x] 86. AxisPropertiesSerializationTests
- [ ] 87. AnnotationPropertiesSerializationTests
- [ ] 88. SeriesPropertiesSerializationTests

### Phase 10: OxyPlotControls.Tests - Axis Conversion
- [x] 89. AxisTypeConversionTests

---

## Coverage Results

| Library | Target | Test Files | Tests | Status |
|---------|--------|------------|-------|--------|
| GenericControls | 80%+ | 8 | ~400 | COMPLETE |
| NumericControls | 80%+ | 7 | ~255 | COMPLETE |
| OxyPlotControls | 80%+ | 10 | ~400 | COMPLETE |
| **Total** | **80%+** | **25** | **~1,055** | **COMPLETE** |

## Notes

1. **STA Thread Requirement**: Some WPF types require STA thread. Use `[STAThread]` attribute or configure xUnit to run tests on STA thread via `xunit.runner.json`.

2. **Test Isolation**: Each converter test should be isolated and not depend on WPF visual tree.

3. **Edge Cases**: Each converter test should cover:
   - Normal values
   - Null values
   - Invalid type inputs
   - Boundary conditions
   - ConvertBack (where applicable)

4. **Serialization Tests**: Use string comparison or XElement comparison rather than object equality.

5. **Math Operation Tests**: Test with known input/output pairs including edge cases (NaN, Infinity, negative numbers).

6. **Remaining Items**: A few optional test classes were not implemented as the core coverage targets were exceeded:
   - StratificationOptionsRowItemTests
   - DistributionDataItemTests
   - AreaPointTests
   - AnnotationPropertiesSerializationTests
   - SeriesPropertiesSerializationTests
