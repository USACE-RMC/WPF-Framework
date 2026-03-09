# DatabaseControls

## Purpose
WPF controls for viewing, editing, searching, and analyzing database tables backed by DatabaseManager, including a field calculator for expression-based column operations.

## Key Files
- `TableViewer.xaml.cs` - Main table viewing control with virtualized rows, cell/row/column selection, sorting, copy/paste, and context menus
- `FindAndReplace.xaml.cs` - Column-scoped search dialog with case-sensitive and whole-word matching
- `ColumnStatsWindow.xaml.cs` - Summary statistics window (numeric or alphabetic) for a selected column
- `Column Stats/NumericColumnStats.xaml.cs` - Numeric statistics display (min, max, mean, std dev, etc.)
- `Column Stats/AlphabeticColumnStats.xaml.cs` - Text statistics display (count, unique, frequency)
- `Column Stats/Classification.cs` - Data classification helper for statistics
- `FieldCalculator/FieldCalculator.xaml.cs` - Expression-based field calculator window; create/update columns or select rows by attribute
- `FieldCalculator/CalculatorHelpWindow.xaml.cs` - Help documentation for calculator expressions
- `FieldCalculator/ErrorWindow.xaml.cs` - Expression evaluation error display
- `FieldCalculator/ErrorItem.cs` - Error item model for field calculator errors
- `Resources/TableViewerResources.xaml` - Styles and templates for the table viewer
- `Themes/DatabaseControlsTheme.xaml` - Theme resource dictionary

## Dependencies
- **DatabaseManager** (project reference) - `DataTableView` data access layer
- **ExpressionParser** (project reference) - Expression parsing for field calculator
- **ExpressionParserControls** (project reference) - Expression editor UI for field calculator
- **GenericControls** (project reference) - MessageBox, MetroWindow base classes
- **OxyPlotControls** (project reference) - Plot integration
- **Themes** (project reference) - Theme colors
- **Numerics.dll** (external) - Statistical functions for column stats
- **OxyPlot / OxyPlot.Wpf** (project reference) - Chart rendering

## Patterns
- TableViewer uses its own virtualized rendering (not WPF DataGrid) with custom selection modes (cell, row, column, all, edit)
- FieldCalculator operates on `DataTableView` from DatabaseManager and uses ExpressionParser for formula evaluation
- ColumnStatsWindow auto-detects numeric vs alphabetic columns and shows the appropriate stats panel
- Nullable is enabled in this project

## Gotchas
- TableViewer is NOT built on CopyPasteDataGrid; it has its own completely custom grid rendering and selection
- FieldCalculator can operate in two modes: column creation/update OR "select by attribute" (row filtering)
- `ExpressionTextBox.cs` and `SelectByAttribute.xaml.cs` are explicitly excluded from compilation in the csproj
