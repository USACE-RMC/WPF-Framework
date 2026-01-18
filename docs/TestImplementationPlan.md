# Unit Test Implementation Plan

This document outlines the scope and prioritized implementation plan for achieving 80%+ test coverage across GenericControls, NumericControls, and OxyPlotControls libraries.

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

**Estimated Test Classes: 35-40 | Estimated Tests: 200+**

### Priority 1: Converters (28 converters - ~140 tests)

| # | Test Class | Source File | Tests |
|---|------------|-------------|-------|
| 1 | `ReverseBooleanConverterTests` | Converters.cs | Convert true/false, ConvertBack |
| 2 | `BooleanToVisibilityConverterTests` | BooleanToVisibilityConverter.cs | Visible/Collapsed/Hidden, custom values |
| 3 | `BooleanToColorConverterTests` | Converters.cs | TrueValue/FalseValue colors |
| 4 | `BooleanToBrushConverterTests` | Converters.cs | TrueValue/FalseValue brushes |
| 5 | `BooleanToTextConverterTests` | Converters.cs | Custom text values |
| 6 | `BooleanToDoubleConverterTests` | Converters.cs | Numeric mappings |
| 7 | `VisibilityToBooleanConverterTests` | Converters.cs | All visibility states |
| 8 | `ColorToByteConverterTests` | ColorToByteConverter.cs | ARGB component extraction, bounds (0-255) |
| 9 | `ColorToSolidBrushConverterTests` | Converters.cs | Bidirectional conversion |
| 10 | `DrawingColorToSolidColorBrushConverterTests` | Converters.cs | System.Drawing.Color conversion |
| 11 | `GridlineColorLightConverterTests` | Converters.cs | Semi-transparent variant creation |
| 12 | `FontToFontFamilyConverterTests` | Converters.cs | String to FontFamily |
| 13 | `FontFamilyToFontStringConverterTests` | Converters.cs | FontFamily to string |
| 14 | `IntToDoubleConverterTests` | Converters.cs | Integer/double conversion |
| 15 | `DoubleToStringConverterTests` | Converters.cs | InvariantCulture formatting |
| 16 | `StringToDoubleConverterTests` | Converters.cs | Parse with error handling |
| 17 | `InRangeConverterTests` | Converters.cs | Bounds checking (inclusive/exclusive) |
| 18 | `DoubleToThicknessConverterTests` | Converters.cs | Side flags (IsLeft, IsTop, etc.) |
| 19 | `ThicknessToDoubleConverterTests` | Converters.cs | Average calculation |
| 20 | `DoubleToDataGridLengthConverterTests` | Converters.cs | DataGridLength conversion |
| 21 | `DoubleToGridLengthConverterTests` | Converters.cs | GridLength conversion |
| 22 | `DoubleToCornerRadiusConverterTests` | Converters.cs | CornerRadius conversion |
| 23 | `DataGridWidthConverterTests` | Converters.cs | Scrollbar adjustment |
| 24 | `VectorToPointConverterTests` | Converters.cs | Vector/Point conversion |
| 25 | `TimeTextConverterTests` | Converters.cs | 12/24 hour format, components |
| 26 | `TabSizeConverterTests` | Converters.cs | IMultiValueConverter tab width |
| 27 | `DoubleToNAConverterTests` | Converters.cs | NaN, Infinity handling |
| 28 | `StringToNAConverterTests` | Converters.cs | Invalid string handling |
| 29 | `HorizontalAlignmentToTextAlignmentConverterTests` | Converters.cs | Alignment mapping |
| 30 | `AlwaysVisibleConverterTests` | Converters.cs | Always returns Visible |

### Priority 2: Validation Rules (~20 tests)

| # | Test Class | Source File | Tests |
|---|------------|-------------|-------|
| 31 | `RangeValidationRuleTests` | ValidationRules.cs | Min/max bounds, exclusive/inclusive |
| 32 | `RangeWrapperTests` | ValidationRules.cs | Dependency property behavior |
| 33 | `PropertyRuleTests` | PropertyRule.cs | ExecuteRules(), error aggregation |

### Priority 3: Utility Methods (~30 tests)

| # | Test Class | Source File | Tests |
|---|------------|-------------|-------|
| 34 | `GeneralMethodsTests` | GeneralMethods.cs | IsNumericType, visual tree helpers |
| 35 | `NumberFormatHelperTests` | NumberFormatHelper.cs | Parsing, formatting |
| 36 | `BindingProxyTests` | ValidationRules.cs | Data property binding |

### Priority 4: Control Logic (~15 tests)

| # | Test Class | Source File | Tests |
|---|------------|-------------|-------|
| 37 | `HsvColorTests` | ColorPicker.xaml.cs | HSV struct calculations |
| 38 | `CopyPasteDataGridTests` | CopyPasteDataGrid.cs | Copy/paste formatting |

---

## NumericControls.Tests

**Estimated Test Classes: 20-25 | Estimated Tests: 150+**

### Priority 1: Converters (~30 tests)

| # | Test Class | Source File | Tests |
|---|------------|-------------|-------|
| 1 | `DoubleToFontFamilyConverterTests` | TimeSeriesTable.xaml.cs | NaN/Infinity italic styling |
| 2 | `DateToStringConverterTests` | TimeSeriesTable.xaml.cs | DateTime formatting, culture |
| 3 | `MathFunctionTypeToNameConverterTests` | MathEditorControl.xaml.cs | All function names |
| 4 | `MathFunctionTypeToTooltipConverterTests` | MathEditorControl.xaml.cs | All tooltips |
| 5 | `MathFunctionTypeToIconConverterTests` | MathEditorControl.xaml.cs | Icon resource keys |
| 6 | `DistributionNameConverterTests` | UncertainOrderedDataTableEditor.xaml.cs | Distribution names |

### Priority 2: Row Item Validation (~60 tests)

| # | Test Class | Source File | Tests |
|---|------------|-------------|-------|
| 7 | `OrdinateRowItemTests` | OrdinateRowItem.cs | X/Y validation, ordering, min/max |
| 8 | `ProbabilityOrdinateRowItemTests` | ProbabilityOrdinateRowItem.cs | 0-1 range, ascending order |
| 9 | `DistributionRowItemTests` | DistributionRowItem.cs | Parameter validation, distribution calc |
| 10 | `StratificationOptionsRowItemTests` | StratificationOptionsRowItem.cs | Bin validation, probability range |
| 11 | `DistributionDataItemTests` | DistributionDataItem.cs | Collection sync, refresh |

### Priority 3: Math Operations (~40 tests)

| # | Test Class | Source File | Tests |
|---|------------|-------------|-------|
| 12 | `MathEditorApplyFunctionTests` | MathEditorControl.xaml.cs | Add, Subtract, Multiply, Divide |
| 13 | `MathEditorAdvancedFunctionsTests` | MathEditorControl.xaml.cs | Exp, Log, Inverse, Replace, Interpolate |
| 14 | `MathEditorHasOperandTests` | MathEditorControl.xaml.cs | Function classification |

### Priority 4: Data Models (~20 tests)

| # | Test Class | Source File | Tests |
|---|------------|-------------|-------|
| 15 | `ParameterTests` | Parameter.cs | Value changes, validation state |
| 16 | `SummaryStatisticTests` | SummaryStatistic.cs | Property getters |
| 17 | `AreaPointTests` | UncertainOrderedDataSelectorControl.xaml.cs | Construction from DataPoints |

---

## OxyPlotControls.Tests

**Estimated Test Classes: 25-30 | Estimated Tests: 180+**

### Priority 1: Extension Methods (~50 tests)

| # | Test Class | Source File | Tests |
|---|------------|-------------|-------|
| 1 | `DataPointExtensionsTests` | Extensions.cs | ToPrettyText, FromPrettyDataText |
| 2 | `ScreenVectorExtensionsTests` | Extensions.cs | ToPrettyText, FromPrettyVectorText |
| 3 | `ScreenPointExtensionsTests` | Extensions.cs | ToPrettyText, FromPrettyScreenText |
| 4 | `VectorExtensionsTests` | Extensions.cs | ToPrettyText, FromPrettyVectorString |
| 5 | `XmlSerializationExtensionsTests` | Extensions.cs | ToXElement, PointFromXElement |
| 6 | `TypeExtensionsTests` | Extensions.cs | GetFirstAbstractBaseType |

### Priority 2: Converters (~60 tests)

| # | Test Class | Source File | Tests |
|---|------------|-------------|-------|
| 7 | `OxyAutomaticColorConverterTests` | GeneralPlotControl.xaml.cs | Automatic color handling |
| 8 | `OxyDefaultFontSizeConverterTests` | GeneralPlotControl.xaml.cs | NaN to default (12.0) |
| 9 | `SolidColorBrushConverterTests` | GeneralPlotControl.xaml.cs | Brush casting |
| 10 | `DataPointToPointConverterTests` | AnnotationControl.xaml.cs | DataPoint to WPF Point |
| 11 | `ScreenVectorToPointConverterTests` | AnnotationControl.xaml.cs | ScreenVector to Point |
| 12 | `OxyHorizontalAlignmentConverterTests` | AnnotationControl.xaml.cs | Alignment mapping |
| 13 | `OxyVerticalAlignmentConverterTests` | AnnotationControl.xaml.cs | Alignment mapping |
| 14 | `ReverseAxisConverterTests` | AxisControl.xaml.cs | IMultiValueConverter |
| 15 | `OxyLineStyleToDashArrayConverterTests` | AxisControl.xaml.cs | Line style mapping |
| 16 | `EmptyStringToNullConverterTests` | AxisControl.xaml.cs | Empty string handling |
| 17 | `DateToNumberConverterTests` | AxisControl.xaml.cs | Date numeric conversion |
| 18 | `LineSeriesColorConverterTests` | LineSeriesControl.xaml.cs | Color with opacity |
| 19 | `AreaSeriesColor2ConverterTests` | GenericSeriesControl.xaml.cs | Second color conversion |
| 20 | `AreaSeriesFillConverterTests` | GenericSeriesControl.xaml.cs | Fill conversion |
| 21 | `LineSeriesMarkerFillConverterTests` | LineSeriesControl.xaml.cs | Marker fill |
| 22 | `LineSeriesMarkerStrokeConverterTests` | LineSeriesControl.xaml.cs | Marker stroke |
| 23 | `BarSeriesFillConverterTests` | BarSeriesControl.xaml.cs | Bar fill with opacity |
| 24 | `BoxPlotSeriesFillConverterTests` | BoxPlotSeriesControl.xaml.cs | Box plot fill |
| 25 | `ScatterSeriesMarkerFillConverterTests` | ScatterSeriesControl.xaml.cs | Scatter marker fill |
| 26 | `ScatterSeriesMarkerStrokeConverterTests` | ScatterSeriesControl.xaml.cs | Scatter marker stroke |

### Priority 3: Serialization (~50 tests)

| # | Test Class | Source File | Tests |
|---|------------|-------------|-------|
| 27 | `OxyPlotSettingsSerializerAttributeTests` | OxyPlotSettingsSerializer.cs | GetColorAttribute, GetDoubleAttribute, etc. |
| 28 | `OxyPlotSettingsSerializerEnumTests` | OxyPlotSettingsSerializer.cs | GetEnumAttribute for all types |
| 29 | `GeneralPropertiesSerializationTests` | GeneralPlotControl.xaml.cs | Round-trip serialization |
| 30 | `LegendPropertiesSerializationTests` | LegendControl.xaml.cs | Round-trip serialization |
| 31 | `AxisPropertiesSerializationTests` | AxisControl.xaml.cs | All axis types |
| 32 | `AnnotationPropertiesSerializationTests` | AnnotationControl.xaml.cs | All annotation types |
| 33 | `SeriesPropertiesSerializationTests` | GenericSeriesControl.xaml.cs | All series types |

### Priority 4: Axis Type Conversion (~20 tests)

| # | Test Class | Source File | Tests |
|---|------------|-------------|-------|
| 34 | `AxisTypeConversionTests` | AxisControl.xaml.cs | Linear, Log, DateTime, Normal, Gumbel |

---

## Implementation Checklist

### Phase 1: GenericControls.Tests - Converters
- [ ] 1. ReverseBooleanConverterTests
- [ ] 2. BooleanToVisibilityConverterTests
- [ ] 3. BooleanToColorConverterTests
- [ ] 4. BooleanToBrushConverterTests
- [ ] 5. BooleanToTextConverterTests
- [ ] 6. BooleanToDoubleConverterTests
- [ ] 7. VisibilityToBooleanConverterTests
- [ ] 8. ColorToByteConverterTests
- [ ] 9. ColorToSolidBrushConverterTests
- [ ] 10. DrawingColorToSolidColorBrushConverterTests
- [ ] 11. GridlineColorLightConverterTests
- [ ] 12. FontToFontFamilyConverterTests
- [ ] 13. FontFamilyToFontStringConverterTests
- [ ] 14. IntToDoubleConverterTests
- [ ] 15. DoubleToStringConverterTests
- [ ] 16. StringToDoubleConverterTests
- [ ] 17. InRangeConverterTests
- [ ] 18. DoubleToThicknessConverterTests
- [ ] 19. ThicknessToDoubleConverterTests
- [ ] 20. DoubleToDataGridLengthConverterTests
- [ ] 21. DoubleToGridLengthConverterTests
- [ ] 22. DoubleToCornerRadiusConverterTests
- [ ] 23. DataGridWidthConverterTests
- [ ] 24. VectorToPointConverterTests
- [ ] 25. TimeTextConverterTests
- [ ] 26. TabSizeConverterTests
- [ ] 27. DoubleToNAConverterTests
- [ ] 28. StringToNAConverterTests
- [ ] 29. HorizontalAlignmentToTextAlignmentConverterTests
- [ ] 30. AlwaysVisibleConverterTests

### Phase 2: GenericControls.Tests - Validation & Utilities
- [ ] 31. RangeValidationRuleTests
- [ ] 32. RangeWrapperTests
- [ ] 33. PropertyRuleTests
- [ ] 34. GeneralMethodsTests
- [ ] 35. NumberFormatHelperTests
- [ ] 36. BindingProxyTests
- [ ] 37. HsvColorTests
- [ ] 38. CopyPasteDataGridTests

### Phase 3: NumericControls.Tests - Converters
- [ ] 39. DoubleToFontFamilyConverterTests
- [ ] 40. DateToStringConverterTests
- [ ] 41. MathFunctionTypeToNameConverterTests
- [ ] 42. MathFunctionTypeToTooltipConverterTests
- [ ] 43. MathFunctionTypeToIconConverterTests
- [ ] 44. DistributionNameConverterTests

### Phase 4: NumericControls.Tests - Row Items
- [ ] 45. OrdinateRowItemTests
- [ ] 46. ProbabilityOrdinateRowItemTests
- [ ] 47. DistributionRowItemTests
- [ ] 48. StratificationOptionsRowItemTests
- [ ] 49. DistributionDataItemTests

### Phase 5: NumericControls.Tests - Math Operations
- [ ] 50. MathEditorApplyFunctionTests
- [ ] 51. MathEditorAdvancedFunctionsTests
- [ ] 52. MathEditorHasOperandTests

### Phase 6: NumericControls.Tests - Data Models
- [ ] 53. ParameterTests
- [ ] 54. SummaryStatisticTests
- [ ] 55. AreaPointTests

### Phase 7: OxyPlotControls.Tests - Extensions
- [ ] 56. DataPointExtensionsTests
- [ ] 57. ScreenVectorExtensionsTests
- [ ] 58. ScreenPointExtensionsTests
- [ ] 59. VectorExtensionsTests
- [ ] 60. XmlSerializationExtensionsTests
- [ ] 61. TypeExtensionsTests

### Phase 8: OxyPlotControls.Tests - Converters
- [ ] 62. OxyAutomaticColorConverterTests
- [ ] 63. OxyDefaultFontSizeConverterTests
- [ ] 64. SolidColorBrushConverterTests
- [ ] 65. DataPointToPointConverterTests
- [ ] 66. ScreenVectorToPointConverterTests
- [ ] 67. OxyHorizontalAlignmentConverterTests
- [ ] 68. OxyVerticalAlignmentConverterTests
- [ ] 69. ReverseAxisConverterTests
- [ ] 70. OxyLineStyleToDashArrayConverterTests
- [ ] 71. EmptyStringToNullConverterTests
- [ ] 72. DateToNumberConverterTests
- [ ] 73. LineSeriesColorConverterTests
- [ ] 74. AreaSeriesColor2ConverterTests
- [ ] 75. AreaSeriesFillConverterTests
- [ ] 76. LineSeriesMarkerFillConverterTests
- [ ] 77. LineSeriesMarkerStrokeConverterTests
- [ ] 78. BarSeriesFillConverterTests
- [ ] 79. BoxPlotSeriesFillConverterTests
- [ ] 80. ScatterSeriesMarkerFillConverterTests
- [ ] 81. ScatterSeriesMarkerStrokeConverterTests

### Phase 9: OxyPlotControls.Tests - Serialization
- [ ] 82. OxyPlotSettingsSerializerAttributeTests
- [ ] 83. OxyPlotSettingsSerializerEnumTests
- [ ] 84. GeneralPropertiesSerializationTests
- [ ] 85. LegendPropertiesSerializationTests
- [ ] 86. AxisPropertiesSerializationTests
- [ ] 87. AnnotationPropertiesSerializationTests
- [ ] 88. SeriesPropertiesSerializationTests

### Phase 10: OxyPlotControls.Tests - Axis Conversion
- [ ] 89. AxisTypeConversionTests

---

## Coverage Targets

| Library | Target Coverage | Estimated Tests |
|---------|----------------|-----------------|
| GenericControls | 80%+ | ~200 |
| NumericControls | 80%+ | ~150 |
| OxyPlotControls | 80%+ | ~180 |
| **Total** | **80%+** | **~530** |

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
