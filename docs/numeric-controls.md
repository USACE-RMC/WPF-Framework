# NumericControls Library

The NumericControls library provides specialized WPF controls for statistical and numeric data management. It builds on GenericControls and is designed for applications requiring probability distribution configuration, curve editing, and time series management.

## Overview

NumericControls offers controls for:
- **Distribution selection** with PDF visualization and parameter estimation
- **Curve editors** for ordered and uncertain paired data
- **Time series tables** for temporal data management
- **Stratification binning** for sampling configuration
- **Bivariate distributions** for two-dimensional empirical data

## Target Framework

The library targets:
- .NET 9.0 (Windows)

## Installation

### Project Reference

```xml
<ProjectReference Include="..\NumericControls\NumericControls.csproj" />
```

### Required Dependencies

NumericControls requires:
- GenericControls (for NumberFormatHelper and utilities)
- Themes (for theme support)
- Numerics library (for probability distributions)
- OxyPlot.Wpf (for plotting)

## Distribution Selector Controls

### DistributionSelectorControl

A comprehensive control for selecting, configuring, and visualizing univariate probability distributions.

```xml
<uni:DistributionSelectorControl
    SelectedDistribution="{Binding MyDistribution, Mode=TwoWay}"
    SampleData="{Binding MySampleData}"
    ShowPlot="True"
    ShowStatistics="True"
    ShowAxisTitle="True"
    DistributionTitle="My Distribution"/>
```

**Key Properties:**

| Property | Type | Description |
|----------|------|-------------|
| `SelectedDistribution` | UnivariateDistributionBase | The currently selected distribution |
| `Distributions` | IList&lt;UnivariateDistributionBase&gt; | Available distributions to choose from |
| `SampleData` | double[] | Optional sample data for comparison |
| `ShowPlot` | bool | Show/hide the PDF plot |
| `ShowStatistics` | bool | Show/hide the summary statistics table |
| `ExpandPlot` | bool | Whether the plot expander is expanded |
| `ShowAxisTitle` | bool | Show/hide axis titles on the plot |
| `ShowAxisLabel` | bool | Show/hide axis labels on the plot |
| `BackgroundColor` | Brush | Control background color |
| `DistributionTitle` | string | Optional title for the distribution |

**Features:**
- Dropdown selection from available distribution types
- Interactive parameter grid with real-time validation
- PDF (Probability Density Function) plot with OxyPlot
- Histogram overlay when sample data is provided
- Summary statistics table comparing distribution vs. data
- Automatic parameter estimation (Fit to Data button)
- Goodness-of-fit statistics (RMSE, Chi-Squared, K-S)

**Supported Distributions:**
- Deterministic
- Normal
- Log-Normal
- Truncated Normal
- Triangular
- PERT
- And all other distributions from the Numerics library

### DistributionWithSelectorControl

A compact control that shows the distribution parameters with a popup selector.

```xml
<uni:DistributionWithSelectorControl
    SelectedDistribution="{Binding MyDistribution, Mode=TwoWay}"
    ShowPlot="False"
    ShowStatistics="False"/>
```

### DistributionSelectorPopup

A button that opens a distribution selector popup.

```xml
<uni:DistributionSelectorPopup
    SelectedDistribution="{Binding MyDistribution, Mode=TwoWay}"
    ShowPlot="True"
    ShowStatistics="False"/>
```

## Curve Editor Controls

### OrderedDataSelectorControl

A control for editing ordered paired data (X, Y curves).

```xml
<ds:OrderedDataSelectorControl
    SelectedOrderedData="{Binding MyCurve, Mode=TwoWay}"
    OrderY="Descending"
    XColumnHeader="Depth"
    YColumnHeader="Velocity"
    XAxisLabel="Depth (ft)"
    YAxisLabel="Velocity (ft/s)"/>
```

**Key Properties:**

| Property | Type | Description |
|----------|------|-------------|
| `SelectedOrderedData` | OrderedPairedData | The curve data |
| `OrderY` | SortOrder | Ascending, Descending, or None |
| `XColumnHeader` | string | Header for X column |
| `YColumnHeader` | string | Header for Y column |
| `XAxisLabel` | string | Label for X axis |
| `YAxisLabel` | string | Label for Y axis |

### UncertainOrderedDataSelectorControl

A control for editing ordered paired data with uncertainty (distribution on each Y value).

```xml
<ds:UncertainOrderedDataSelectorControl
    SelectedUncertainOrderedData="{Binding MyUncertainCurve, Mode=TwoWay}"
    OrderY="Descending"
    XColumnHeader="Depth"
    YColumnHeader="Velocity"/>
```

**Key Properties:**

| Property | Type | Description |
|----------|------|-------------|
| `SelectedUncertainOrderedData` | UncertainOrderedPairedData | The uncertain curve data |
| `OrderY` | SortOrder | Ascending, Descending, or None |

### UncertainOrderedDataTableEditor

An editable data table for uncertain ordered data with add/remove functionality.

```xml
<ds:UncertainOrderedDataTableEditor
    SelectedUncertainOrderedData="{Binding MyUncertainCurve, Mode=TwoWay}"
    AddRemoveRows="True"
    OrderY="None"
    IsStrictY="False"
    IsStrictX="False"
    XColumnHeader="Depth"
    YColumnHeader="Velocity"/>
```

**Key Properties:**

| Property | Type | Description |
|----------|------|-------------|
| `AddRemoveRows` | bool | Enable row add/remove |
| `IsStrictX` | bool | Enforce strict X ordering |
| `IsStrictY` | bool | Enforce strict Y ordering |

## Time Series Controls

### TimeSeriesTable

A table control for displaying and editing time series data.

```xml
<ds:TimeSeriesTable
    Series="{Binding MyTimeSeries}"
    IsReadOnly="False"
    XColumnHeader="Date"
    YColumnHeader="Flow"/>
```

**Key Properties:**

| Property | Type | Description |
|----------|------|-------------|
| `Series` | TimeSeries | The time series data |
| `IsReadOnly` | bool | Whether the table is editable |
| `XColumnHeader` | string | Header for the date/time column |
| `YColumnHeader` | string | Header for the value column |

**Supported Time Intervals:**
- Irregular (arbitrary timestamps)
- One Minute
- One Hour
- One Day
- Custom intervals

### NumericEntry

A simple numeric entry control for inline editing.

```xml
<ds:NumericEntry Value="{Binding MyValue}"/>
```

## Stratification Controls

### BinDefinitionControl

A control for defining stratification bins for sampling.

```xml
<ds:BinDefinitionControl
    StratificationOptionsCollection="{Binding MyBins, Mode=TwoWay}"/>
```

**Features:**
- Define multiple bin ranges
- Set minimum and maximum values
- Configure number of samples per bin
- Visual validation of overlapping ranges

## Multivariate Controls

### BivariateEmpiricalControl

A control for editing bivariate empirical cumulative distribution functions.

```xml
<ds:BivariateEmpiricalControl
    BivariateCDF="{Binding MyBivariateCDF, Mode=TwoWay}"
    X1Header="Primary Hazard"
    X2Header="Secondary Hazard"/>
```

**Key Properties:**

| Property | Type | Description |
|----------|------|-------------|
| `BivariateCDF` | BivariateEmpirical | The bivariate CDF data |
| `X1Header` | string | Header for primary variable |
| `X2Header` | string | Header for secondary variable |

## Data Classes

### Parameter

Represents a distribution parameter with validation support.

```csharp
var meanParam = new Parameter("Mean", "μ (Mean)", 100.0);
meanParam.PropertyChanged += (s, e) => {
    if (e.PropertyName == nameof(Parameter.Value))
        UpdateDistribution();
};
```

**Properties:**
- `Name` - Internal parameter name
- `DisplayName` - Display name (may include symbols)
- `Value` - Numeric value
- `IsValid` - Validation state
- `ErrorMessage` - Validation error message

### SummaryStatistic

Holds summary statistics for distribution/data comparison.

```csharp
var stat = new SummaryStatistic("Mean", "100.0000", "98.5432");
```

**Properties:**
- `StatName` - Statistic name (e.g., "Mean", "5%")
- `DistStat` - Value from distribution
- `DataStat` - Value from sample data

## Theming

NumericControls supports the Themes library for runtime theme switching.

### Theme Resource Dictionary

Include the theme dictionary in your window or app:

```xml
<ResourceDictionary.MergedDictionaries>
    <ResourceDictionary Source="pack://application:,,,/NumericControls;Component/Themes/NumericControlsTheme.xaml"/>
</ResourceDictionary.MergedDictionaries>
```

### Initialize Themes

In App.xaml.cs:

```csharp
private void Application_Startup(object sender, StartupEventArgs e)
{
    ThemeService.Instance.Initialize(Theme.Light);
    var mainWindow = new MainWindow();
    mainWindow.Show();
}
```

### Switch Themes at Runtime

```csharp
ThemeService.Instance.SetTheme(Theme.Dark);
```

### Available Styles

The NumericControlsTheme.xaml provides:

| Style | Description |
|-------|-------------|
| `NumericControlsDataGridStyle` | Base DataGrid styling |
| `Left_ColumnHeaderStyle` | Left-aligned column header |
| `Right_ColumnHeaderStyle` | Right-aligned column header |
| `WrappedColumnHeaderStyle` | Centered, wrapped header |
| `ReadOnlyTextBackgroundCellStyle` | Read-only text cell |
| `ReadOnlyValueBackgroundCellStyle` | Read-only numeric cell |
| `Right_CellStyle` | Right-aligned editable cell |
| `ParameterValueCellStyle` | Parameter cell with validation |
| `NumericControlsUserControlStyle` | Base UserControl style |

## Internationalization (I18N)

NumericControls uses `NumberFormatHelper` from GenericControls for culture-aware number formatting.

### Features
- Automatic decimal separator detection (`.` vs `,`)
- Thousands separator handling
- Scientific notation support (`1.5e-10`)
- Negative number formatting per culture
- Right-to-left text support

### Example

```csharp
// Culture-aware parsing
if (NumberFormatHelper.TryParseDouble(userInput, out double value))
{
    // value is parsed using current culture
}

// Culture-aware formatting
string formatted = NumberFormatHelper.FormatDouble(value, 4, false);
```

## Best Practices

### 1. Initialize Themes Before Creating Windows

Always initialize the theme service before creating any UI elements:

```csharp
private void Application_Startup(object sender, StartupEventArgs e)
{
    ThemeService.Instance.Initialize(Theme.Light);
    // Now create windows
}
```

### 2. Use Two-Way Binding for Distribution Properties

```xml
<uni:DistributionSelectorControl
    SelectedDistribution="{Binding MyDist, Mode=TwoWay, UpdateSourceTrigger=PropertyChanged}"/>
```

### 3. Provide Sample Data for Better User Experience

When sample data is available, the control can:
- Show a histogram overlay
- Enable "Fit to Data" button
- Display goodness-of-fit statistics

```xml
<uni:DistributionSelectorControl
    SelectedDistribution="{Binding MyDist}"
    SampleData="{Binding MySampleData}"/>
```

### 4. Use SortOrder.None for User-Defined Order

When users should control row order manually:

```xml
<ds:UncertainOrderedDataTableEditor OrderY="None" IsStrictY="False"/>
```

## Demo Application

See the `Demo_NumericControls` project for working examples of all controls. Run the demo to:
- Test distribution selection and parameter editing
- Edit uncertain and standard curves
- Work with time series data
- Configure stratification bins
- See theme switching in action

## Troubleshooting

### Distribution Parameters Not Updating

Ensure you're using `Mode=TwoWay` and `UpdateSourceTrigger=PropertyChanged`:

```xml
SelectedDistribution="{Binding Path=..., Mode=TwoWay, UpdateSourceTrigger=PropertyChanged}"
```

### Theme Not Applying

1. Verify ThemeService.Instance.Initialize() is called before window creation
2. Check that the theme dictionary is merged in Window.Resources or App.Resources
3. Ensure DynamicResource (not StaticResource) is used for theme-aware bindings

### Culture-Specific Number Parsing Fails

The controls use `NumberFormatHelper.TryParseDouble()` which handles most cultures. If issues persist:

```csharp
// Force invariant culture for specific parsing
double.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out value);
```

### Summary Statistics Show Duplicates

This was a known bug fixed in the current version. Ensure you're using the latest code where statistics are initialized once in `InitializeControl()` rather than being re-added on each histogram update.
