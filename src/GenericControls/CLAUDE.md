# GenericControls

## Purpose
Reusable WPF control library providing themed windows, data grids with copy/paste, numeric inputs, color picker, date/time controls, property editors, adorners, converters, and file management dialogs.

## Key Files
- `Windows/MetroWindow.cs` - Base class for Metro-styled windows; binds SystemCommands for min/max/close
- `Windows/MetroDialogWindow.cs` - Base class for dialogs (close-only, no resize, center-owner)
- `Windows/MessageBox.cs` - Theme-aware replacement for System.Windows.MessageBox with "Don't show again" support
- `DataGrid/CopyPasteDataGrid.cs` - Extended DataGrid with clipboard copy/paste, row add/insert/delete, sorting, and context menus
- `DataGrid/ValidationDataGrid.cs` - Extends CopyPasteDataGrid with per-cell validation via DataGridRowItem/PropertyRule
- `General Controls/NumericTextBox.xaml.cs` - Numeric input with range validation, supports int/double/scientific notation
- `General Controls/ColorPicker.xaml.cs` - HSV color picker with swatches, sliders, and ARGB support
- `General Controls/NumericUpDown.xaml` - Numeric spinner control
- `File Management/FolderBrowser.cs` - WPF folder browser using COM IFileDialog (Vista+) with legacy fallback
- `File Management/RecentFileList.cs` - MRU file list management
- `Utilities/Converters.cs` - ~20 IValueConverters (bool-to-visibility, color, thickness, font, etc.)
- `Utilities/NumberFormatHelper.cs` - Culture-aware numeric parsing and validation helpers
- `Properties Controls/` - ~25 property editor controls (color, font, numeric, alignment, file selector, etc.)
- `DataGrid/ValidationDataGridObjects/DataGridRowItem.cs` - Base row item with RuleMap for cell-level validation
- `Adorners/DragAdorner.cs` - Drag-and-drop visual adorner

## Dependencies
- **Themes** (project reference) - MetroWindowStyle, MetroDialogStyle, theme colors
- **System.Drawing.Common** (NuGet) - Drawing.Color interop in converters

## Patterns
- All windows derive from MetroWindow or MetroDialogWindow and use `Style="{DynamicResource MetroWindowStyle}"`
- ValidationDataGrid uses DataGridRowItem subclasses with PropertyRule-based RuleMap for cell validation
- CopyPasteDataGrid requires ItemsSource to implement IList; row type inferred from first item or set via RowType property
- Converters are designed for XAML StaticResource usage; some expose singleton Instance properties
- Namespace is `GenericControls` (flat, no sub-namespaces)

## Gotchas
- Do NOT set `DataContext = this` in UserControl constructors -- it breaks external bindings from parent controls
- CopyPasteDataGrid paste requires continuous cell selection; non-continuous selection shows an error
- NumericTextBox uses NumberFormatHelper for culture-aware parsing -- always use it instead of raw double.TryParse
- ColorPicker.Color property is SolidColorBrush, not System.Windows.Media.Color
- FolderBrowser uses COM interop and requires STA thread
