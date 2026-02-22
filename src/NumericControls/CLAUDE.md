# NumericControls

## Purpose
Specialized WPF controls for numeric data entry, statistical distributions, ordered curves, time series, uncertain data, probability ordinates, and stratified sampling -- built for engineering and risk analysis applications.

## Key Files
- `Data/Ordered Curve Editor/OrderedDataTableEditor.xaml.cs` - Editor for ordered X-Y curve data with ValidationDataGrid
- `Data/Ordered Curve Editor/OrderedDataSelectorControl.xaml.cs` - Selector UI for ordered data sources
- `Data/Ordered Curve Editor/OrdinateRowItem.cs` - Row item for ordered data with validation rules
- `Data/Time Series Editor/TimeSeriesTable.xaml.cs` - Time series data entry with date-value pairs
- `Data/Time Series Editor/MathEditorControl.xaml.cs` - Math operations editor for time series transforms
- `Data/Uncertain Curve Editor/UncertainOrderedDataTableEditor.xaml.cs` - Curve editor with distribution-per-ordinate uncertainty
- `Data/Uncertain Curve Editor/UncertainTableEditor.xaml.cs` - General uncertain data table editor
- `Data/Uncertain Curve Editor/DistributionRowItem.cs` - Row item holding a distribution for each data point
- `Data/Probability Ordinates/ProbabilityOrdinatesControl.xaml.cs` - Probability ordinate entry (exceedance probabilities)
- `Data/Probability Ordinates/ProbabilityOrdinateRowItem.cs` - Row item with probability validation
- `Distributions/Univariate/Distribution Selector/DistributionSelectorControl.xaml.cs` - UI for selecting and configuring univariate distributions
- `Distributions/Univariate/Distribution Selector/Selector.xaml.cs` - Distribution type dropdown selector
- `Distributions/Multivariate/BivariateEmpiricalControl.xaml.cs` - Bivariate empirical distribution editor
- `Sampling/Stratification Binning/BinDefinitionControl.xaml.cs` - Stratification bin definition UI

## Dependencies
- **GenericControls** (project reference) - ValidationDataGrid, NumericTextBox, property controls
- **OxyPlotControls** (project reference) - Plot integration for distribution previews
- **Themes** (project reference) - Theme resources
- **Numerics.dll** (external) - Statistical distribution types and math functions
- **OxyPlot.dll / OxyPlot.Wpf.dll** (external) - Chart rendering

## Patterns
- Data editors use ValidationDataGrid with custom DataGridRowItem subclasses (OrdinateRowItem, TimeSeriesRowItem, etc.)
- Distribution controls bind to Numerics library distribution types via dependency properties
- Uncertain curve editors combine ordered data with per-ordinate distribution specifications
- Namespace is `NumericControls`

## Gotchas
- Numerics.dll is referenced from a sibling repo (`../../../numerics/`) -- must be built first
- OxyPlot DLLs are referenced from sibling repo (`../../../oxyplot/`) -- must be built first
- Distribution selector populates from Numerics library types at runtime; missing DLL causes empty selector
- Time series editor assumes DateTime-keyed data; non-DateTime keys will not work
