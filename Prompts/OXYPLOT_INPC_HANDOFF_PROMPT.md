# Session Goal

Redesign plot settings undo/redo for RMC-BestFit by upgrading OxyPlot.Wpf to support INotifyPropertyChanged and using UndoableStateBridge on Plot objects. This eliminates the broken XML string serialization approach.

## Background — Why This Is Needed

RMC-BestFit has 11 plots (Chronology, Frequency, Seasonality, Density, Histogram, QQ, ACF, PACF, MRL, ModifiedScale, Shape) on InputData. Currently, plot visual settings (title, colors, axis labels, legend position, etc.) are serialized to XML strings and stored as `string` properties on InputData (e.g., `ChronologyPlotSettings`). Undo uses `RecordPropertyChange` on these strings. On undo, `FromXelement` deserializes the XML back onto the Plot object.

**This approach is fundamentally broken:**
- `FromXelement` calls `plot.Series.Clear()`, `plot.Axes.Clear()`, `plot.Annotations.Clear()` — destroying data bindings (ItemsSource) every time
- After FromXelement, the OxyPlotPropertiesControl (properties panel) bindings break and stop reflecting changes
- Multiple attempts to patch this with suppression flags, deferred re-enable, binding refresh, and null guards have all failed
- The XML string approach cannot work because deserializing a full Plot state is destructive to the live WPF control

**The new approach:** Store Plot objects directly on InputData, add INotifyPropertyChanged to OxyPlot.Wpf.Plot (and base Series/Axis/Annotation classes), and use UndoableStateBridge to automatically record/replay individual property changes. This means undo of "change title" just sets `plot.Title = oldValue` — no destructive deserialization.

## Repository Structure

Three repos involved, all local source (not NuGet):

```
C:\GIT\oxyplot\Source\OxyPlot.Wpf\       <- OxyPlot WPF controls (Plot, Series, Axes, Annotations)
C:\GIT\oxyplot\Source\OxyPlot.Wpf.Shared\ <- Shared base classes (PlotViewBase)
C:\GIT\wpf-framework\                     <- WPF framework (OxyPlotControls, UndoableStateBridge)
C:\GIT\rmc-bestfit\                        <- Main app (InputData, InputDataControl)
```

## Architecture Overview

### Current Flow (broken)
```
User edits title in OxyPlotPropertiesControl
  -> PlotChanged event
  -> InputDataControl serializes Plot -> XML string
  -> Element.ChronologyPlotSettings = xmlString (RecordPropertyChange)

Undo:
  -> Element.ChronologyPlotSettings = oldXmlString
  -> FromXelement(plot, oldXml) -- DESTRUCTIVE: clears series/axes, breaks bindings
  -> Bindings in OxyPlotPropertiesControl break after first undo
```

### New Flow (target)
```
User edits title in OxyPlotPropertiesControl
  -> Plot.Title DP changes -> AppearanceChanged callback -> InvalidatePlot
  -> Plot fires PropertyChanged("Title") via INPC
  -> UndoableStateBridge captures old/new value -> records DelegateAction

Undo:
  -> DelegateAction sets plot.Title = oldValue (single property, non-destructive)
  -> DP change fires -> plot re-renders
  -> INPC fires -> OxyPlotPropertiesControl bindings update naturally
  -> No series clearing, no binding breakage
```

## Scope of Changes

### Phase 1: Add INPC to OxyPlot.Wpf.Plot (43 DPs)

**File:** `C:\GIT\oxyplot\Source\OxyPlot.Wpf\Plot.cs`

Plot.cs currently inherits: `Plot : PlotView : PlotViewBase : Control`. It has 43 DependencyProperties, all using `AppearanceChanged` callback that calls `InvalidatePlot(false)`.

**Required changes:**
1. Implement `INotifyPropertyChanged` on `Plot`
2. For each of the 43 DPs, add INPC notification in the `AppearanceChanged` callback (or a new shared callback)
3. Keep existing DP infrastructure intact -- DPs continue to work for XAML binding; INPC is additive for UndoableStateBridge

**Pattern:**
```csharp
public partial class Plot : PlotView, INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    // Existing DP callback -- add INPC notification:
    private static void AppearanceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var plot = (Plot)d;
        plot.InvalidatePlot(false);
        plot.OnPropertyChanged(e.Property.Name);  // NEW: fire INPC
    }
}
```

**The 43 DPs on Plot (grouped):**

Title/Subtitle: Title, Subtitle, TitleColor, SubtitleColor, TitleFont, TitleFontSize, TitleFontWeight, TitlePadding, SubtitleFont, SubtitleFontSize, SubtitleFontWeight

Plot Area: PlotAreaBackground, PlotAreaBorderColor, PlotAreaBorderThickness, TextColor

Legend (20+): IsLegendVisible, LegendBackground, LegendBorder, LegendBorderThickness, LegendItemAlignment, LegendTextColor, LegendTitle, LegendTitleColor, LegendTitleFont, LegendTitleFontSize, LegendTitleFontWeight, LegendItemOrder, LegendItemSpacing, LegendLineSpacing, LegendMargin, LegendMaxHeight, LegendMaxWidth, LegendOrientation, LegendPadding, LegendPlacement, LegendPosition, LegendSymbolLength, LegendSymbolMargin, LegendSymbolPlacement, LegendColumnSpacing

Other: DefaultPlotCursor

### Phase 2: Add INPC to Base Series, Axis, Annotation Classes

These base classes define DPs that child controls bind to. Adding INPC here covers all subtypes.

**Files:**
- `C:\GIT\oxyplot\Source\OxyPlot.Wpf\Series\Series.cs` -- 6 DPs (Color, Title, RenderInLegend, TrackerFormatString, TrackerKey, EdgeRenderingMode)
- `C:\GIT\oxyplot\Source\OxyPlot.Wpf\Axes\Axis.cs` -- 62 DPs (Title, Position, Minimum, Maximum, MajorStep, MinorStep, StringFormat, Font, FontSize, colors, gridlines, ticks, etc.)
- `C:\GIT\oxyplot\Source\OxyPlot.Wpf\Annotations\Annotation.cs` -- base annotation DPs

**Same pattern as Plot:** Add `INotifyPropertyChanged`, fire `OnPropertyChanged(e.Property.Name)` in existing `AppearanceChanged` callbacks.

**Subtype-specific DPs:** There are ~35 series types, ~16 axis types, ~14 annotation types with their own DPs. These should also fire INPC from their `AppearanceChanged` callbacks. The pattern is mechanical -- add `OnPropertyChanged(e.Property.Name)` to each existing callback. No new callbacks needed.

**Total DP count across all files: ~540.** But the pattern is identical for each -- it's a mechanical change, not a design change.

### Phase 3: Add Plot Properties to InputData (RMC.BestFit.UI)

**File:** `C:\GIT\rmc-bestfit\src\RMC.BestFit.UI\Elements\InputData\InputData.cs`

**Add project references:**
- RMC.BestFit.UI.csproj -> add references to OxyPlot, OxyPlot.Wpf, OxyPlot.Wpf.Shared

**Replace 11 XML string properties with 11 Plot object properties:**
```csharp
// OLD:
private string _chronologyPlotSettings;
public string ChronologyPlotSettings { get => ...; set => RecordPropertyChange(...); }

// NEW:
private Plot _chronologyPlot;
public Plot ChronologyPlot
{
    get => _chronologyPlot;
    set
    {
        if (_chronologyPlot == value) return;
        _chronologyPlot = value;
        RaisePropertyChange(nameof(ChronologyPlot));
    }
}
```

**Add UndoableStateBridge for each Plot:**
```csharp
private UndoableStateBridge _chronologyPlotBridge;

private void SetupPlotBridges()
{
    _chronologyPlotBridge = new UndoableStateBridge(
        _chronologyPlot,
        () => IsUndoEnabled ? UndoManager : null,
        "chronology plot",
        this,
        excludedProperties: new[] { "ActualWidth", "ActualHeight", "IsVisible", ... }
    );
    // ... repeat for all 11 plots
}
```

**Open/Save changes:**
- Open(): Create Plot objects, deserialize from XML using `FromXelement`, then setup bridges with `IsUndoEnabled = false`
- Save(): Serialize Plot objects to XML using `ToXelement` for database storage

### Phase 4: Rewire InputDataControl

**File:** `C:\GIT\rmc-bestfit\src\RMC.BestFit.App\GUI\InputData\InputDataControl.xaml.cs`

**Key changes:**
1. Remove all 11 `Load*PlotSettings()` methods (no more XML -> Plot deserialization)
2. Remove all plot settings handling from `ElementPropertyChanged`
3. Remove the `OnPlotChanged` -> debounce -> serialize -> property assignment flow
4. Instead, InputDataControl.ChronologyPlot (XAML `x:Name`) IS `Element.ChronologyPlot` -- same object reference
5. The UndoableStateBridge handles undo automatically via INPC
6. OxyPlotPropertiesControl binds to the same Plot object -- changes propagate naturally

**The XAML Plot elements need to reference the Element's Plot objects:**
- Either bind `DataContext` of the plot container to `Element.ChronologyPlot`
- Or assign in code-behind: `ChronologyPlot = Element.ChronologyPlot` in `ElementCallback`

### Phase 5: Clean Up OxyPlotControls (wpf-framework)

**Files in `C:\GIT\wpf-framework\src\OxyPlotControls\`:**
- Remove `RefreshPlotBindings()` from OxyPlotPropertiesControl (no longer needed)
- Keep `SuppressPlotChanged` infrastructure for initialization suppression
- `OxyPlotSettingsSerializer.ToXelement` / `FromXelement` remain for Open/Save serialization, but are no longer used for undo

## Key Files Reference

### OxyPlot.Wpf (modify)
```
C:\GIT\oxyplot\Source\OxyPlot.Wpf\
+-- Plot.cs                          <- 43 DPs, add INPC (MAIN TARGET)
+-- PlotView.cs                      <- 1 DP
+-- Series\
|   +-- Series.cs                    <- 6 DPs base class, add INPC
|   +-- LineSeries.cs                <- subtype DPs
|   +-- ScatterSeries.cs             <- subtype DPs
|   +-- ScatterPointSeries.cs        <- custom series (may be in wpf-framework)
|   +-- AreaSeries.cs
|   +-- HistogramSeries.cs
|   +-- ... (35 series types total)
+-- Axes\
|   +-- Axis.cs                      <- 62 DPs base class, add INPC
|   +-- LinearAxis.cs
|   +-- DateTimeAxis.cs
|   +-- ... (16 axis types total)
+-- Annotations\
    +-- Annotation.cs                <- base class, add INPC
    +-- ... (14 annotation types total)
```

### WPF-Framework (modify)
```
C:\GIT\wpf-framework\src\
+-- OxyPlotControls\
|   +-- OxyplotPropertiesControl.xaml.cs  <- Clean up RefreshPlotBindings
|   +-- OxyPlotToolbar.xaml.cs
|   +-- OxyPlotSettingsSerializer.cs      <- Keep for Open/Save
|   +-- GeneralPlotControl.xaml(.cs)      <- Binds to Plot DPs (no change needed)
|   +-- LegendControl.xaml(.cs)
|   +-- Axes\AxesControl.xaml(.cs)
|   +-- Axes\AxisControl.xaml(.cs)
|   +-- Series\SeriesControl.xaml(.cs)
|   +-- Annotations\AnnotationControl.xaml(.cs)
+-- FrameworkInterfaces\Undo\
    +-- UndoableStateBridge.cs            <- Requires INPC (read-only reference)
    +-- UndoableCollectionBridge.cs       <- For Series/Axes/Annotations collections
```

### RMC-BestFit (modify)
```
C:\GIT\rmc-bestfit\src\
+-- RMC.BestFit.UI\
|   +-- RMC.BestFit.UI.csproj            <- Add OxyPlot references
|   +-- Elements\InputData\
|       +-- InputData.cs                  <- Replace string props with Plot objects + bridges
+-- RMC.BestFit.App\GUI\InputData\
    +-- InputDataControl.xaml             <- Plot elements reference Element's Plots
    +-- InputDataControl.xaml.cs          <- Remove Load/Serialize, rewire to shared Plot objects
```

## UndoableStateBridge API (for reference)

```csharp
// Constructor
new UndoableStateBridge(
    source: INotifyPropertyChanged,           // Plot object (after INPC added)
    getUndoManager: Func<IUndoManager?>,      // () => IsUndoEnabled ? UndoManager : null
    sourceDescription: string,                 // "chronology plot"
    target: object?,                           // InputData element
    includedProperties: string[]?,             // whitelist (OR)
    excludedProperties: string[]?              // blacklist
);

// Key members:
bridge.Dispose();                              // Unsubscribe from INPC
bridge.SuspendRecording();                     // Returns IDisposable; suppresses recording
bridge.ClearShadowValues();                    // Reset shadow state
```

## Important Constraints

1. **Cannot compile .NET in Claude environment.** User compiles locally and reports errors.
2. **All .cs files need XML documentation** (summary, param, returns, remarks).
3. **All .cs files need license header.**
4. **Plot.cs already has 43 DPs** -- the INPC addition must not break existing functionality. DPs continue to work for XAML. INPC is additive.
5. **Series/Axes/Annotations are ObservableCollections on Plot** -- their CollectionChanged events drive `SyncLogicalTree` and `InvalidatePlot`. UndoableCollectionBridge can monitor these for add/remove undo.
6. **ItemsSource on Series is data, not visual** -- exclude from undo. Only visual properties (colors, markers, line styles, titles) should be bridged.
7. **Performance:** UndoableStateBridge replays property changes one at a time. For batch undo (e.g., reverting a complex plot style change), each property set triggers `InvalidatePlot`. Consider whether `SuspendRecording()` or batching is needed.
8. **Permissions:** Claude MUST present implementation plan and wait for user approval before writing any code. Always present what files will change and why.

## Verification Plan

1. Add INPC to Plot.cs -> compile OxyPlot.Wpf -> verify existing tests pass
2. Add INPC to Series/Axis/Annotation base classes -> compile -> verify
3. Add Plot properties to InputData -> compile RMC.BestFit.UI
4. Rewire InputDataControl -> compile RMC-BestFit.exe
5. Test: Edit title -> verify title updates on plot
6. Test: Ctrl+Z -> verify title reverts (single property set, no FromXelement)
7. Test: Ctrl+Z multiple times -> verify each step reverts correctly
8. Test: Edit in OxyPlotPropertiesControl -> verify properties panel stays in sync
9. Test: Close and reopen element -> verify settings persist (Open/Save still uses XML serialization)
10. Test: Edit axis title, legend position, series color -> verify each undoes correctly

## What NOT to Change

- **OxyPlot.Core** (internal model) -- no changes needed
- **OxyPlotSettingsSerializer** -- keep for Open/Save serialization
- **Existing XAML bindings** in OxyPlotControls -- they already bind to DPs, which still work
- **Rendering pipeline** -- `InvalidatePlot` / `SynchronizeProperties` / `Render` remain unchanged
