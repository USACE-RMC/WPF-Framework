# GenericControls Library

The GenericControls library provides a comprehensive set of reusable WPF controls for building desktop applications. Originally developed in VB.NET, it has been fully converted to C# with enhanced international number support.

## Overview

GenericControls offers controls for:
- **Numeric input** with validation and formatting
- **Color selection** with HSV color picker
- **Date/time selection** with analog clock interface
- **Data grids** with copy/paste, validation, and row management
- **Property editors** for building property panels
- **File management** utilities

## Target Framework

The library targets:
- .NET 9.0 (Windows)

## Installation

### Project Reference

```xml
<ProjectReference Include="..\GenericControls\GenericControls.csproj" />
```

### NuGet (Coming Soon)

```
Install-Package RMC.GenericControls
```

## Core Controls

### NumericTextBox

A text box that validates numeric input with configurable bounds and formatting.

```xml
<cntrls:NumericTextBox
    Text="{Binding MyValue}"
    MinValue="0"
    MaxValue="100"
    CanBeNegative="False"
    CanBeBlank="True"
    IsWholeNumber="False"
    BoundsAreExclusive="False"/>
```

**Key Properties:**
| Property | Type | Description |
|----------|------|-------------|
| `Text` | string | The text content (bindable) |
| `MinValue` | double | Minimum allowed value |
| `MaxValue` | double | Maximum allowed value |
| `CanBeNegative` | bool | Allow negative numbers |
| `CanBeBlank` | bool | Allow empty input |
| `IsWholeNumber` | bool | Restrict to integers only |
| `BoundsAreExclusive` | bool | Values must be strictly between bounds |
| `ValueIsValid` | bool | Read-only validation state |

**Validation Properties (Read-Only):**
- `AboveMaxValue` - True if value exceeds maximum
- `BelowMinValue` - True if value is below minimum
- `InvalidText` - True if text cannot be parsed as a number

**International Number Support:**
NumericTextBox automatically handles culture-specific number formats:
- Decimal separators (`.` vs `,`)
- Thousands separators
- Negative sign positions
- Scientific notation (e.g., `1.5e-3`)
- Infinity values (`inf`, `+inf`, `-inf`)

### NumericUpDown

A numeric spinner control with increment/decrement buttons.

```xml
<cntrls:NumericUpDown
    Value="{Binding Count}"
    Minimum="0"
    Maximum="1000"
    Increment="1"
    DecimalPlaces="0"
    ThousandsSeparator="True"/>
```

### ColorPicker

A full-featured color picker with HSV spectrum, RGB sliders, and alpha channel.

```xml
<cntrls:ColorPicker Color="{Binding SelectedColor}"/>

<!-- Or use the popup variant -->
<cntrls:ColorPickerPopup Color="{Binding SelectedColor}"/>
```

**Features:**
- HSV color spectrum for intuitive selection
- RGB sliders for precise control
- Alpha channel slider for transparency
- Live preview

### NameTextBox

A text box with name validation (character limits, invalid characters, uniqueness).

```xml
<cntrls:NameTextBox
    Text="{Binding ElementName}"
    CharacterLimit="64"
    CanBeBlank="False"
    InvalidCharacters="{Binding ForbiddenChars}"
    InvalidStrings="{Binding ExistingNames}"/>
```

**Validation Features:**
- Maximum character limit
- Invalid character filtering (file path chars by default)
- Duplicate name detection via `InvalidStrings`
- Error tooltip with specific validation messages

## Date and Time Controls

### DateAndTimePickerControl

A combined calendar and clock control for selecting date and time.

```xml
<cntrls:DateAndTimePickerControl
    DateAndTime="{Binding SelectedDateTime}"
    Is24Hour="False"
    HasSeconds="True"/>
```

### DateAndTimeTextBoxControl

A text box with popup calendar/clock for date-time entry.

```xml
<cntrls:DateAndTimeTextBoxControl
    SelectedDateTime="{Binding EventTime}"
    Is24Hour="True"/>
```

### ClockControl

An analog clock face for time selection.

```xml
<cntrls:ClockControl
    Time="{Binding SelectedTime}"
    Is24Hour="True"
    HasMinutes="True"
    HasSeconds="False"
    FaceColor="White"
    HandColor="Black"/>
```

## DataGrid Controls

### CopyPasteDataGrid

An enhanced DataGrid with clipboard operations and row management.

```xml
<cntrls:CopyPasteDataGrid
    ItemsSource="{Binding Items}"
    CanUserAddInsertDeleteRows="True"
    PasteAddsRows="True"
    ShowSortContextMenu="True"/>
```

**Features:**
- Copy/paste with Excel compatibility
- Add, insert, and delete rows
- Column sorting with context menu
- NaN and infinity value handling
- Keyboard shortcuts (Ctrl+C, Ctrl+V, Delete)

**Events:**
- `PreviewPasteData` - Validate/transform clipboard data before paste
- `DataPasted` - Notification after paste completes
- `PreviewAddRows` / `RowsAdded` - Row addition events
- `PreviewDeleteRows` / `RowsDeleted` - Row deletion events

### ValidationDataGrid

Extends CopyPasteDataGrid with cell-level validation and error visualization.

```xml
<cntrls:ValidationDataGrid
    ItemsSource="{Binding ValidatedItems}"
    ErrorCellBorderBrush="Red"
    ErrorCellBackgroundBrush="#F7B6AF"/>
```

### DataGridToolbar

A toolbar with common DataGrid operations.

```xml
<cntrls:DataGridToolbar DataGrid="{Binding ElementName=MyDataGrid}"/>
```

**Buttons included:**
- Add row
- Insert row
- Delete row
- Select all
- Copy / Copy with headers
- Paste

## Property Controls

A suite of controls designed for building property panels. All share common layout properties:

| Property | Description |
|----------|-------------|
| `Title` | Label text |
| `PropertyWidth` | Width of the value area |
| `MinPropertyWidth` | Minimum value area width |
| `MaxPropertyWidth` | Maximum value area width |
| `ShowLeaderLine` | Show dotted line between label and value |

### NumericPropertyControl

```xml
<cntrls:NumericPropertyControl
    Title="Value:"
    Number="{Binding NumericValue}"
    MinValue="0"
    MaxValue="100"
    CanHaveNegative="False"
    IsWholeNumber="False"/>
```

### BooleanPropertyControl

```xml
<cntrls:BooleanPropertyControl
    Title="Enabled:"
    IsSelected="{Binding IsEnabled}"/>
```

### TextPropertyControl

```xml
<cntrls:TextPropertyControl
    Title="Description:"
    Text="{Binding Description}"/>
```

### ColorPropertyControl

```xml
<cntrls:ColorPropertyControl
    Title="Color:"
    SelectedColor="{Binding ItemColor}"/>
```

### FileSelectorControl / DirectorySelectorControl

```xml
<cntrls:FileSelectorControl
    Title="File:"
    FilePath="{Binding SelectedFile}"
    Filter="Text files|*.txt"/>

<cntrls:DirectorySelectorControl
    Title="Folder:"
    DirectoryPath="{Binding OutputFolder}"/>
```

## Utility Classes

### NumberFormatHelper

Static helper for culture-aware number parsing and formatting.

```csharp
// Parse with current culture
if (NumberFormatHelper.TryParseDouble(text, out double value))
{
    // Use value
}

// Format with current culture
string formatted = NumberFormatHelper.FormatDouble(value, 2, useThousandsSeparator: true);

// Check for special values
if (NumberFormatHelper.IsInfinityText(text))
{
    // Handle infinity input
}

// Validate input during typing
bool isValid = NumberFormatHelper.IsValidNumericInput(
    newChar, currentText, caretPosition, selectedText,
    allowNegative: true, allowDecimal: true, allowScientific: true);
```

### Converters

The library includes many value converters for XAML bindings:

| Converter | Purpose |
|-----------|---------|
| `ReverseBooleanConverter` | Invert boolean |
| `BooleanToVisibilityConverter` | Bool to Visibility |
| `BooleanToColorConverter` | Bool to Color |
| `BooleanToBrushConverter` | Bool to Brush |
| `DoubleToStringConverter` | Number to string |
| `DoubleToNAConverter` | NaN/Infinity to "N/A" |
| `ColorToSolidBrushConverter` | Color to brush |
| `TimeTextConverter` | DateTime to time component |

### GeneralMethods

Static utility methods:

```csharp
// Find element in visual tree
var textBox = GeneralMethods.FindElementByName<TextBox>(parent, "MyTextBox");

// Get all elements of type
var allButtons = GeneralMethods.GetAllElementsOfType(window, typeof(Button));

// File dialogs
string file = GeneralMethods.FileOpenDialog("Text files|*.txt");
string folder = GeneralMethods.FolderBrowserDialog(owner, "Select folder");

// Image conversion
BitmapSource source = GeneralMethods.Bitmap2BitmapSource(bitmap);
```

## XAML Resources

Include the GenericResources dictionary for styles and icons:

```xml
<ResourceDictionary.MergedDictionaries>
    <ResourceDictionary Source="pack://application:,,,/GenericControls;Component/Resources/GenericResources.xaml"/>
</ResourceDictionary.MergedDictionaries>
```

**Available Styles:**
- `DotLeaderLineStyle` - Dotted separator line
- `TitleTextStyle` - Property label style

**Available Arrays:**
- `LineWidthOptions` - Common line widths
- `FontSizeOptions` - Common font sizes
- `PaddingOptions` - Common padding values
- `AngleOptions` - Rotation angles (0-360)

## Best Practices

### 1. Use x:Name for ElementName Bindings

When using `ElementName` in bindings, always use `x:Name`:

```xml
<!-- Correct -->
<cntrls:NumericTextBox x:Name="MyNumeric"/>
<Border Height="{Binding ElementName=MyNumeric, Path=ActualHeight}"/>

<!-- May fail -->
<cntrls:NumericTextBox Name="MyNumeric"/>
```

### 2. Handle Culture-Specific Number Input

The controls automatically use `CultureInfo.CurrentCulture`. For explicit culture:

```csharp
NumberFormatHelper.TryParseDouble(text, CultureInfo.InvariantCulture, out double value);
```

### 3. Validate Before Using Values

Check validation state before using numeric values:

```csharp
if (myNumericTextBox.ValueIsValid)
{
    double value = myNumericTextBox.GetValueAsDouble();
    // Use value
}
```

### 4. Use ValidationDataGrid for Editable Data

For tables where users edit data, use ValidationDataGrid with custom row items:

```csharp
public class MyDataItem : DataGridRowItem
{
    protected override void AddValidationRules()
    {
        AddRule(nameof(Value), () => Value < 0, "Value must be non-negative");
    }
}
```

## Demo Application

See the `Demo_GenericControls` project for working examples of all controls.

## Migration from VB.NET Version

If migrating from the VB.NET GenericControls:

1. **Namespace**: Use `GenericControls` (not `GenericControls` from VB namespace)
2. **Class Names**: `Color_Picker` is now `ColorPicker`
3. **x:Name**: Ensure all ElementName binding targets use `x:Name`
4. **International Support**: New `NumberFormatHelper` handles culture-aware formatting
