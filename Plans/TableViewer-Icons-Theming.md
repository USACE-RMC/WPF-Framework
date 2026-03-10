# TableViewer: Icons & Theming Update

## Context

The TableViewer uses a mix of PNG/ICO raster icons and hardcoded colors. Two changes are needed:
1. **Toolbar icons** — Replace raster PNGs/ICOs with shared vector icons from GenericControls `IconDictionary.xaml`
2. **Theme/style** — Make the grid theme-aware using `DynamicResource` colors from the `DataGrid.*` palette, fixing the known scroll-rendering issue

---

## Part 1: Toolbar Icon Replacement

### Goal
Replace all raster `Image` + `BitmapImage` icon references with vector `ContentControl` + `StaticResource` from GenericControls `IconDictionary.xaml`. DatabaseControls already has its own `IconDictionary.xaml` with themed vector icons — some can be reused, but the user specifically wants GenericControls icons for toolbar buttons.

### Icon Mapping (DatabaseControls current → GenericControls target)

| Button | Current Icon Source | GenericControls Resource Key | Notes |
|--------|-------------------|------------------------------|-------|
| SelectAllButton | PNG: `deselect_all_cells_16x.png` / `ClearSelectionIcon_22x22.png` | `SelectAllIcon` (line 129) | Need two states (selected/deselected). GenericControls `SelectAllIcon` has blue accent fill. For deselected state, can use Opacity=0.5 or DatabaseControls `DeselectAllIcon`. |
| CopyButton | Canvas: `{StaticResource CopyIcon}` from TableViewerResources.xaml | `CopyIcon` (line 140) | Already vector but from local ResourceDictionary. Switch to GenericControls version. |
| CopyWithHeadersButton | Canvas: `{StaticResource CopyWithHeadersIcon}` from TableViewerResources.xaml | `CopyWithHeadersIcon` (line 157) | Same — switch source. |
| PasteButton | PNG: `/GenericControls;component/Resources/paste.png` | `PasteIcon` (line 177) | Currently raster from GenericControls. Switch to vector. |
| ExportTableButton | Canvas: `{StaticResource ExportTableIcon}` from TableViewerResources.xaml | No exact match in GenericControls. Keep DatabaseControls `ExportTableIcon` from `IconDictionary.xaml`. | `DownloadIcon` is closest but different semantics. |
| SelectByAttribute | PNG: `SelectByAttribute_16x.png` | No match. Keep or create new vector icon. | Unique to DatabaseControls. |
| ShowAll | PNG: `ShowAllIcon_22x22.png` / `ClearSelectionIconDisabled_22x22.png` | No exact match. Keep DatabaseControls vector icons. | State-swapped in code-behind. |
| ShowSelected | PNG: `ShowSelectedIcon_22x22.png` / `ClearSelectionIconDisabled_22x22.png` | No exact match. Keep DatabaseControls vector icons. | State-swapped in code-behind. |
| DeSelectAll | PNG: `ClearSelectionIcon_22x22.png` / `ClearSelectionIconDisabled_22x22.png` | No exact match. Keep DatabaseControls vector icons. | State-swapped in code-behind. |
| OpenFC | PNG: `calculator_16x.png` | No match. Use DatabaseControls `CalculatorIcon` from `IconDictionary.xaml`. | Already have vector version. |
| Undo | PNG: `Undo.png` / `UndoDisabled.png` | No match. Use DatabaseControls `UndoIcon` from `IconDictionary.xaml`. | State-swapped in code-behind. |
| Redo | PNG: `Redo.png` / `RedoDisabled.png` | No match. Use DatabaseControls `RedoIcon` from `IconDictionary.xaml`. | State-swapped in code-behind. |
| SaveButton | ICO: `Save.ico` / `SaveDisabled.ico` | No match. Use DatabaseControls `SaveIcon` from `IconDictionary.xaml`. | State-swapped in code-behind. |

### Context Menu Icons (code-behind)

| Menu Item | Current Source | Target | Line |
|-----------|---------------|--------|------|
| Sort Ascending | `GenericControls;.../SortASCFilter.png` | `SortAscFilterIcon` (GC line 225) | 3148 |
| Sort Descending | `GenericControls;.../SortDSCFilter.png` | `SortDescFilterIcon` (GC line 245) | 3153 |
| Remove Sort | `GenericControls;.../ClearFilter.png` | `ClearFilterIcon` (GC line 265) | 3158 |
| Summary Statistics | `DatabaseControls;.../SummaryStatistics_16x.png` | DatabaseControls `StatisticsIcon` | 3163 |
| Find | `DatabaseControls;.../QuickFind_16x.png` | DatabaseControls `FindIcon` | 3168 |
| Field Calculator | `DatabaseControls;.../calculator_16x.png` | DatabaseControls `CalculatorIcon` | 3176 |
| Delete Columns | `GenericControls;.../delete_column.png` | `DeleteColumnIcon` (GC line 111) | 3183 |
| Delete Rows | `GenericControls;.../delete_row.png` | `DeleteRowIcon` (GC line 69) | 2454 |
| Copy (grid menu) | `GenericControls;.../copy.png` | `CopyIcon` (GC line 140) | 3414 |
| Copy with Headers | `GenericControls;.../copy_w_headers.png` | `CopyWithHeadersIcon` (GC line 157) | 3418 |
| Paste (grid menu) | `GenericControls;.../paste.png` | `PasteIcon` (GC line 177) | 3424 |

### Files to Edit

#### 1. `src/DatabaseControls/TableViewer.xaml` — Toolbar XAML (lines 29-312)

**Merge GenericControls IconDictionary** — Add to UserControl.Resources (line 14-21):
```xml
<ResourceDictionary.MergedDictionaries>
    <ResourceDictionary Source="Resources/TableViewerResources.xaml"/>
    <ResourceDictionary Source="/GenericControls;component/Resources/IconDictionary.xaml"/>
</ResourceDictionary.MergedDictionaries>
```

**SelectAllButton (lines 43-72)** — Replace Image+DataTrigger with ContentControl:
- Currently uses Image with DataTrigger binding to AllCellsSelected
- Replace with `ContentControl Content="{StaticResource SelectAllIcon}"` for selected state
- For deselected, use DatabaseControls `DeselectAllIcon`
- Can use DataTrigger on AllCellsSelected to swap Content

**CopyButton (lines 74-86)** — Change `{StaticResource CopyIcon}` source:
- Currently loads from TableViewerResources.xaml CopyIcon (Canvas with hardcoded #FF000000)
- Change to GenericControls `CopyIcon` (Viewbox with DynamicResource ToolbarIconForeground)
- Remove Viewbox wrapper since GC icons are already Viewbox

**CopyWithHeadersButton (lines 88-100)** — Same pattern as Copy

**PasteButton (lines 102-116)** — Replace Image PNG with GenericControls `PasteIcon`:
- Currently: `<Image Source="/GenericControls;component/Resources/paste.png"/>`
- Change to: `<ContentControl Content="{StaticResource PasteIcon}"/>`

**ExportTableButton (lines 118-131)** — Keep but source from DatabaseControls IconDictionary:
- Currently loads from TableViewerResources.xaml
- Change to DatabaseControls `IconDictionary.xaml` version (already themed with DynamicResource)

**SelectByAttribute (lines 148-162)** — Keep PNG or create new vector icon

**ShowAll/ShowSelected/DeSelectAll (lines 164-217)** — Convert from Image to ContentControl:
- These have state-swapped icons in code-behind
- Strategy: Use Opacity for disabled state rather than separate icon images
- Initial content: vector icon from DatabaseControls IconDictionary
- Enabled/Disabled: Set Button.IsEnabled + Opacity instead of swapping Image.Source

**OpenFC (lines 234-249)** — Replace PNG with DatabaseControls `CalculatorIcon`

**Undo (lines 251-269)** — Replace PNG with DatabaseControls `UndoIcon`:
- Disabled state via `Button.IsEnabled=False` + Opacity trigger

**Redo (lines 271-289)** — Same as Undo with `RedoIcon`

**SaveButton (lines 291-309)** — Replace ICO with DatabaseControls `SaveIcon`

#### 2. `src/DatabaseControls/TableViewer.xaml.cs` — Code-behind icon swaps

**State-swapped toolbar buttons** — Remove all `((Image)Button.Content).Source = new BitmapImage(...)` calls:

Lines to modify (convert from Image.Source swap to ContentControl.Content swap or Opacity changes):
- **ShowAll**: Lines 2766, 2791, 2826
- **ShowSelected**: Lines 2815, 2858, 2865, 4175, 4190
- **DeSelectAll**: Lines 2817, 2860, 2867, 4177, 4192
- **Undo**: Lines 4074, 4082
- **Redo**: Lines 4089, 4095
- **SaveButton**: Lines 4075, 4083

Strategy: The `TableViewerToolbarButtonStyle` in DatabaseControlsTheme.xaml already has `Opacity=0.5` trigger for `IsEnabled=False`. So we only need `Button.IsEnabled = true/false` — no icon swapping needed for enabled/disabled states.

For ShowAll/ShowSelected which change _meaning_ (not just enabled/disabled):
- Use `ContentControl.Content` swap with `FindResource("IconKey")`

**Context menu icons** — Convert from BitmapImage to vector resource lookups:
- Lines 2454, 3148, 3153, 3158, 3163, 3168, 3176, 3183, 3414, 3418, 3424
- Pattern: `Icon = new Image { Source = new BitmapImage(new Uri("pack://...")) }`
- Replace with: `Icon = FindResource("CopyIcon")` (returns Viewbox directly for GC icons)

#### 3. `src/DatabaseControls/Resources/TableViewerResources.xaml` — Remove migrated icons

Remove `CopyIcon`, `CopyWithHeadersIcon`, `ExportTableIcon`, `SortArrow` canvas definitions (lines 29-136) since they're now sourced from IconDictionary files.

Keep `DefaultCellStyle` and `VerticalSeparatorStyle`.

---

## Part 2: Theme/Style the Grid

### Current State

`DatabaseControlsTheme.xaml` line 174 has:
```xml
<!-- DIAGNOSTIC: Style with NO DynamicResource to test if that's the scroll issue -->
```
And line 206-207:
```xml
<!-- NOTE: Implicit style removed - causes scroll rendering issues when applied implicitly. -->
```

**The `TableViewerStyle` currently uses hardcoded colors (no DynamicResource).** The Demo doesn't apply it at all (line 120-130 of Demo MainWindow.xaml — no Style attribute).

### Root Cause of Scroll Rendering Issue

The TableViewer uses a **custom virtualized grid** (NOT WPF DataGrid). The visual structure is:

```
HorizontalViewerGrid (Grid)
├── ColumnHeadersGrid (Grid, row 0)         — ColumnHeader (Border) objects
├── RowColorGrid (Grid, row 1)              — Rectangle objects for alternating row backgrounds
├── GridPanel (Grid, row 1, overlaid)       — Cell (Border) objects with transparent background
└── GridLinesCanvas (Canvas, row 1, overlaid) — Line objects for grid lines
```

Cell rendering uses direct property assignments (NOT style bindings):
- `SelectCell()` (line 1644): `cell.Background = SelectedColor; cell.Foreground = SelectedForegroundColor;`
- `DeSelectCell()` (line 1649): `cell.Background = DeSelectedColor; cell.Foreground = DeSelectedForegroundColor;`
- `SetActiveCell()` (line 1661): `cell.Background = ActiveCellBackground; cell.Foreground = ActiveCellForeground;`
- `UpdateRowHeaders()` (line 1440): `rect.Fill = alternate ? AlternateRowColor : RowColor;`

These all read from DependencyProperties on the TableViewer. **The DP values themselves will correctly reflect DynamicResource bindings from the style** — the issue was likely the implicit style mechanism causing re-layout during scroll, not the DynamicResource brush values.

### Solution: Named Style with DynamicResource Colors

#### 1. `src/DatabaseControls/Themes/DatabaseControlsTheme.xaml` — Update `TableViewerStyle`

**Lines 174-204**: Convert hardcoded colors to `DynamicResource` bindings:

```xml
<Style x:Key="TableViewerStyle" TargetType="{x:Type local:TableViewer}">
    <!-- Header Styles (already use DynamicResource internally) -->
    <Setter Property="ColumnHeaderTextblockStyle" Value="{StaticResource ColumnHeaderTextStyle}"/>
    <Setter Property="ColumnHeaderBorderStyle" Value="{StaticResource ColumnHeaderBorderStyle}"/>
    <Setter Property="RowHeaderTextblockStyle" Value="{StaticResource RowHeaderTextStyle}"/>
    <Setter Property="RowHeaderBorderStyle" Value="{StaticResource RowHeaderBorderStyle}"/>
    <Setter Property="CellTextblockStyle" Value="{StaticResource CellTextStyle}"/>
    <!-- Theme-aware colors -->
    <Setter Property="Foreground" Value="{DynamicResource DataGrid.Static.Foreground}"/>
    <Setter Property="Background" Value="{DynamicResource DataGrid.Static.Background}"/>
    <Setter Property="BorderBrush" Value="{DynamicResource DataGrid.Static.Border}"/>
    <Setter Property="BorderThickness" Value="1"/>
    <!-- Row Colors -->
    <Setter Property="RowColor" Value="{DynamicResource DataGrid.Row.Background}"/>
    <Setter Property="AlternateRowColor" Value="{DynamicResource DataGrid.Row.Alternating.Background}"/>
    <!-- Grid Line Colors -->
    <Setter Property="RowLineColor" Value="{DynamicResource DataGrid.GridLines}"/>
    <Setter Property="ColumnLineColor" Value="{DynamicResource DataGrid.GridLines}"/>
    <Setter Property="RowLineThickness" Value="1"/>
    <Setter Property="ColumnLineThickness" Value="1"/>
    <!-- Selection Colors -->
    <Setter Property="SelectedColor" Value="{DynamicResource DataGrid.Row.Selection.Background}"/>
    <Setter Property="SelectedForegroundColor" Value="{DynamicResource DataGrid.Row.Selection.Foreground}"/>
    <Setter Property="DeSelectedColor" Value="Transparent"/>
    <Setter Property="DeSelectedForegroundColor" Value="{DynamicResource DataGrid.Static.Foreground}"/>
    <Setter Property="ActiveCellBackground" Value="{DynamicResource DataGrid.Cell.Focus.Border}"/>
    <Setter Property="ActiveCellForeground" Value="{DynamicResource DataGrid.Row.Selection.Foreground}"/>
    <!-- Default Row Height -->
    <Setter Property="RowHeight" Value="22"/>
</Style>
```

**Add implicit style** (line ~205) — BUT with the scroll fix in place:
```xml
<Style TargetType="{x:Type local:TableViewer}" BasedOn="{StaticResource TableViewerStyle}"/>
```

#### 2. `src/DatabaseControls/TableViewer.xaml.cs` — Fix Scroll Rendering

**The scroll rendering problem**: When styles are applied, every scroll event triggers `DeSelectAllCells()` which iterates ALL visible cells and sets `Background = DeSelectedColor`. This is O(rows × columns) direct property assignments on every scroll tick.

The fix is NOT to avoid DynamicResource — it's to make the deselection more efficient and avoid unnecessary re-renders.

**Lines 1635-1640 (`DeSelectAllCells`)** — Currently sets Background on EVERY cell:
```csharp
private void DeSelectAllCells()
{
    for (int i = 0; i < DataView.ColumnNames.Count(); i++)
        for (int j = 0; j < GridPanel.RowDefinitions.Count; j++)
            DeSelectCell(i, j);
}
```

**Key insight**: Since cells use `Brushes.Transparent` as the deselected background (overlaid on RowColorGrid), and cell Background is only set by `SelectCell`/`DeSelectCell`/`SetActiveCell`, we can:
1. Only deselect cells that are actually selected (tracked in `_selectedCellIndices`, `_selectedDataRowIndices`, `_selectedColumnIndices`)
2. Or, since the scroll shifts text but not backgrounds, we can clear ALL backgrounds once and only repaint selected ones

The current approach already works (clear all, repaint selected) — but the O(rows×cols) clear on every scroll tick combined with DynamicResource style re-evaluation may cause flicker.

**Proposed fix**: Instead of iterating all cells in `DeSelectAllCells()`, batch the clear:
```csharp
private void DeSelectAllCells()
{
    var deselectedBg = DeSelectedColor;
    var deselectedFg = DeSelectedForegroundColor;
    int colCount = DataView.ColumnNames.Count();
    for (int j = 0; j < GridPanel.RowDefinitions.Count; j++)
    {
        for (int i = 0; i < colCount; i++)
        {
            var cell = (Cell)GridPanel.Children[j * colCount + i];
            cell.Background = deselectedBg;
            cell.Foreground = deselectedFg;
        }
    }
}
```
(Cache DP values outside the loop to avoid repeated DP lookups.)

**Lines 1642-1651 (`SelectCell`/`DeSelectCell`)** — Cache column count:
```csharp
private void SelectCell(int columnIndex, int rowIndex)
{
    var cell = (Cell)GridPanel.Children[rowIndex * DataView.ColumnNames.Count() + columnIndex];
    cell.Background = SelectedColor;
    cell.Foreground = SelectedForegroundColor;
}
```

**Lines 1294-1371 (`AddRow`)** — Cell creation at line 4329:
```csharp
Background = Brushes.Transparent;  // line 4329
```
Change to read from the TableViewer's `DeSelectedColor` DP? No — `Transparent` is correct because RowColorGrid provides the background color layer underneath.

**Lines 1361-1367 (`AddRow` row color)** — Already reads from DPs:
```csharp
Brush fillColor = RowColor;
if (_rowId![...] % 2 != 0) fillColor = AlternateRowColor;
rect.Fill = fillColor;
```
This is fine — RowColor/AlternateRowColor will come from DynamicResource via style.

**Lines 1336-1343 (`AddRow` grid lines)** — Already reads from DPs:
```csharp
StrokeThickness = RowLineThickness,
Stroke = RowLineColor
```
Fine — will come from DynamicResource via style.

#### 3. `src/DatabaseControls/TableViewer.xaml` — Grid Panel Background

**Line 363** (GridPanel): Needs `Background="Transparent"` (should already be set — verify).

**Line 362** (RowColorGrid): Should be `Background="{Binding Background, ElementName=TableViewerControl}"` to pick up the themed background.

**Lines 315-380** (TableControlGrid area): The outer Grid should use themed background.

#### 4. `src/DatabaseControls/TableViewer.xaml.cs` — Sort Glyphs (lines ~4237-4293)

The `ColumnHeader` class creates sort arrow Polygon objects with `Brushes.Black` fill:
```csharp
Points = new PointCollection { ... },
Fill = Brushes.Black  // Hardcoded
```
Change to use `{DynamicResource DataGrid.Header.Foreground}` by reading from the ColumnHeader's inherited foreground, or pass the brush via a DP.

#### 5. `src/DatabaseControls/TableViewer.xaml.cs` — Cell class default Foreground

**Line 4317**:
```csharp
public static readonly DependencyProperty ForegroundProperty = DependencyProperty.Register(
    nameof(Foreground), typeof(Brush), typeof(Cell), new PropertyMetadata(Brushes.Black));
```
The default `Brushes.Black` is overridden by `DeSelectCell()` on every scroll, so this is fine as a fallback.

#### 6. `src/DatabaseControls.Demo/MainWindow.xaml` — Apply style (or rely on implicit)

If implicit style works after the scroll fix, no change needed. Otherwise:
```xml
<dtview:TableViewer Style="{StaticResource TableViewerStyle}" .../>
```

### Theme Color Key Reference

All keys already defined in `src/Themes/Resources/Colors/{Light,Blue,Dark}Colors.xaml`:

| DP Property | Theme Key |
|------------|-----------|
| SelectedColor | `DataGrid.Row.Selection.Background` |
| SelectedForegroundColor | `DataGrid.Row.Selection.Foreground` |
| DeSelectedColor | Transparent (literal) |
| DeSelectedForegroundColor | `DataGrid.Static.Foreground` |
| ActiveCellBackground | `DataGrid.Cell.Focus.Border` |
| ActiveCellForeground | `DataGrid.Row.Selection.Foreground` |
| RowColor | `DataGrid.Row.Background` |
| AlternateRowColor | `DataGrid.Row.Alternating.Background` |
| RowLineColor | `DataGrid.GridLines` |
| ColumnLineColor | `DataGrid.GridLines` |
| Background | `DataGrid.Static.Background` |
| Foreground | `DataGrid.Static.Foreground` |
| BorderBrush | `DataGrid.Static.Border` |

---

## Critical File Index

| File | Lines | What to Change |
|------|-------|---------------|
| `src/DatabaseControls/TableViewer.xaml` | 14-21 | Merge GenericControls IconDictionary |
| `src/DatabaseControls/TableViewer.xaml` | 43-312 | Replace all toolbar button icons |
| `src/DatabaseControls/TableViewer.xaml` | 362-363 | RowColorGrid/GridPanel background bindings |
| `src/DatabaseControls/TableViewer.xaml.cs` | 1635-1651 | Optimize DeSelectAllCells/SelectCell/DeSelectCell |
| `src/DatabaseControls/TableViewer.xaml.cs` | 2454 | Context menu: DeleteRow icon |
| `src/DatabaseControls/TableViewer.xaml.cs` | 2766-2867 | ShowAll/ShowSelected/DeSelectAll icon state swaps |
| `src/DatabaseControls/TableViewer.xaml.cs` | 3148-3184 | Column header context menu icons |
| `src/DatabaseControls/TableViewer.xaml.cs` | 3414-3425 | Grid right-click context menu icons |
| `src/DatabaseControls/TableViewer.xaml.cs` | 4074-4095 | Undo/Redo/Save icon state swaps |
| `src/DatabaseControls/TableViewer.xaml.cs` | 4175-4192 | UpdateSelectionButtonStates icon swaps |
| `src/DatabaseControls/TableViewer.xaml.cs` | 4237-4293 | ColumnHeader sort glyph Brushes.Black |
| `src/DatabaseControls/Themes/DatabaseControlsTheme.xaml` | 174-207 | TableViewerStyle: hardcoded → DynamicResource |
| `src/DatabaseControls/Resources/TableViewerResources.xaml` | 29-136 | Remove migrated Canvas icons |
| `src/DatabaseControls/Resources/IconDictionary.xaml` | ALL | Already has themed vector icons — reuse for DB-specific icons |
| `src/GenericControls/Resources/IconDictionary.xaml` | 129-265 | Source for Copy/Paste/Sort/Delete icons |
| `src/DatabaseControls.Demo/MainWindow.xaml` | 120-130 | May need `Style="{StaticResource TableViewerStyle}"` |

---

## Verification

1. `dotnet build WPF-Framework.sln` — 0 errors
2. Run DatabaseControls.Demo:
   - All toolbar buttons show vector icons (no pixelated PNGs)
   - Icons respond to theme changes (Light/Blue/Dark)
   - Undo/Redo/Save correctly dim when disabled
   - ShowAll/ShowSelected icons swap correctly
   - Right-click context menus show vector icons
3. Theme switching test:
   - Switch themes (Light → Dark → Blue → Light)
   - Grid background, row colors, selection colors, header colors all update
   - Scroll up and down — no rendering artifacts, alternating rows correct
   - Select cells, rows, columns — selection colors match theme
   - Grid lines visible and correctly colored
4. `dotnet test WPF-Framework.sln` — all existing tests pass
