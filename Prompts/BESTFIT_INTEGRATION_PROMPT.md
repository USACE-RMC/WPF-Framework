# Session Goal

Integrate OxyPlot.Wpf INPC + UndoableStateBridge into RMC-BestFit for non-destructive plot settings undo/redo. Replace the broken XML string serialization approach with direct property-level undo.

## Prerequisites (Already Completed)

1. **INotifyPropertyChanged added to OxyPlot.Wpf** — Plot, Series, Axis, Annotation base classes now fire `PropertyChanged` from `AppearanceChanged` callbacks (NOT from `DataChanged` — performance constraint).

2. **PlotChanged event chain removed from OxyPlotControls** — No more `PlotChanged` event bubbling. INPC on the Plot/Series/Axis/Annotation objects replaces this.

3. **Serialization moved to OxyPlot.Wpf** (if completed) — `PlotSerializer.ToXelement()`/`FromXelement()` available directly from OxyPlot.Wpf.

## Architecture Overview

### Current Flow (broken — being replaced)
```
User edits title in OxyPlotPropertiesControl
  -> Binding updates Plot.Title DP
  -> PlotChanged event fires (NOW REMOVED)
  -> InputDataControl serializes Plot -> XML string
  -> Element.ChronologyPlotSettings = xmlString (RecordPropertyChange)

Undo:
  -> Element.ChronologyPlotSettings = oldXmlString
  -> FromXelement(plot, oldXml) -- DESTRUCTIVE: clears series/axes, breaks bindings
```

### New Flow (target)
```
User edits title in OxyPlotPropertiesControl
  -> Binding updates Plot.Title DP
  -> AppearanceChanged callback fires InvalidatePlot(false) + OnPropertyChanged("Title")
  -> UndoableStateBridge captures old/new value via INPC -> records DelegateAction

Undo:
  -> DelegateAction sets plot.Title = oldValue (via PropertyInfo.SetValue)
  -> DP change fires -> plot re-renders
  -> Two-way binding updates OxyPlotPropertiesControl UI naturally
  -> No series clearing, no binding breakage
```

## Repository Structure

```
C:\GIT\oxyplot\Source\OxyPlot.Wpf\           <- INPC already added
C:\GIT\wpf-framework\src\FrameworkInterfaces\ <- UndoableStateBridge (read-only reference)
C:\GIT\wpf-framework\src\OxyPlotControls\     <- PlotChanged removed
C:\GIT\rmc-bestfit\src\                        <- TARGET: integration changes
```

## Implementation Phases

### Phase 1: Add OxyPlot.Wpf Reference to RMC.BestFit.UI

**File:** `C:\GIT\rmc-bestfit\src\RMC.BestFit.UI\RMC.BestFit.UI.csproj`

Add project references to OxyPlot, OxyPlot.Wpf, OxyPlot.Wpf.Shared (same as OxyPlotControls references).

### Phase 2: Add Plot Object Properties to InputData

**File:** `C:\GIT\rmc-bestfit\src\RMC.BestFit.UI\Elements\InputData\InputData.cs`

Replace 11 XML string properties with Plot object properties:

```csharp
// OLD (remove):
private string _chronologyPlotSettings;
public string ChronologyPlotSettings
{
    get => _chronologyPlotSettings;
    set { RecordPropertyChange(ref _chronologyPlotSettings, value); }
}

// NEW (add):
private OxyPlot.Wpf.Plot _chronologyPlot;
public OxyPlot.Wpf.Plot ChronologyPlot
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

The 11 plots to replace:
1. ChronologyPlot (was ChronologyPlotSettings)
2. FrequencyPlot (was FrequencyPlotSettings)
3. SeasonalityPlot (was SeasonalityPlotSettings)
4. DensityPlot (was DensityPlotSettings)
5. HistogramPlot (was HistogramPlotSettings)
6. QQPlot (was QQPlotSettings)
7. ACFPlot (was ACFPlotSettings)
8. PACFPlot (was PACFPlotSettings)
9. MRLPlot (was MRLPlotSettings)
10. ModifiedScalePlot (was ModifiedScalePlotSettings)
11. ShapePlot (was ShapePlotSettings)

### Phase 3: Set Up UndoableStateBridge Per Plot

**File:** `C:\GIT\rmc-bestfit\src\RMC.BestFit.UI\Elements\InputData\InputData.cs`

```csharp
private UndoableStateBridge _chronologyPlotBridge;
// ... repeat for all 11

private static readonly string[] PlotIncludedProperties = new[]
{
    "Title", "Subtitle", "TitleColor", "SubtitleColor",
    "TitleFont", "TitleFontSize", "TitleFontWeight", "TitlePadding",
    "SubtitleFont", "SubtitleFontSize", "SubtitleFontWeight",
    "PlotAreaBackground", "PlotAreaBorderColor", "PlotAreaBorderThickness",
    "TextColor", "IsLegendVisible", "LegendBackground", "LegendBorder",
    "LegendBorderThickness", "LegendItemAlignment", "LegendTextColor",
    "LegendTitle", "LegendTitleColor", "LegendTitleFont",
    "LegendTitleFontSize", "LegendTitleFontWeight", "LegendItemOrder",
    "LegendItemSpacing", "LegendLineSpacing", "LegendMargin",
    "LegendMaxHeight", "LegendMaxWidth", "LegendOrientation",
    "LegendPadding", "LegendPlacement", "LegendPosition",
    "LegendSymbolLength", "LegendSymbolMargin", "LegendSymbolPlacement",
    "LegendColumnSpacing", "LegendFont", "LegendFontSize", "LegendFontWeight",
    "Background", "Padding"
};

private void SetupPlotBridge(ref UndoableStateBridge bridge, OxyPlot.Wpf.Plot plot, string description)
{
    bridge?.Dispose();
    if (plot == null) return;

    bridge = new UndoableStateBridge(
        plot,
        () => IsUndoEnabled ? UndoManager : null,
        description,
        target: this,
        includedProperties: PlotIncludedProperties
    );
}

private void SetupAllPlotBridges()
{
    SetupPlotBridge(ref _chronologyPlotBridge, _chronologyPlot, "chronology plot");
    SetupPlotBridge(ref _frequencyPlotBridge, _frequencyPlot, "frequency plot");
    // ... repeat for all 11
}
```

### Phase 4: Update Open/Save

**Open (loading from database):**
```csharp
// During element load/open:
using (_chronologyPlotBridge?.SuspendRecording())
{
    if (!string.IsNullOrEmpty(savedXml))
    {
        PlotSerializer.FromXelement(_chronologyPlot, XElement.Parse(savedXml));
    }
}
// After suspend disposes, shadow values are updated to current state
```

**Save (persisting to database):**
```csharp
// During element save:
string xml = PlotSerializer.ToXelement(_chronologyPlot).ToString();
element.UpdateChronologyPlotSettings(xml);
```

### Phase 5: Rewire InputDataControl

**File:** `C:\GIT\rmc-bestfit\src\RMC.BestFit.App\GUI\InputData\InputDataControl.xaml.cs`

Key changes:
1. Remove all `Load*PlotSettings()` methods (no more XML → Plot deserialization in the control)
2. Remove all plot settings handling from `ElementPropertyChanged`
3. Remove the empty `OnPropertiesControlPlotChanged` handler from MainProjectNode
4. The Plot objects in InputDataControl should reference Element's Plot objects (same instance)
5. OxyPlotPropertiesControl binds to the same Plot object — changes propagate naturally via DPs

### Phase 6: Series/Axis/Annotation Bridges (Optional — Deferred)

For individual Series, Axis, and Annotation visual property undo, create per-object bridges:

```csharp
private static readonly string[] SeriesIncludedProperties = new[]
{
    "Title", "Color", "FillColor", "Visibility", "RenderInLegend",
    "StrokeThickness", "LineStyle", "MarkerType", "MarkerFill",
    "MarkerStroke", "MarkerSize", "MarkerStrokeThickness", "MarkerResolution",
    "Color2", "ColorHi", "ColorLo", "LineStyle2", "LineStyleLo", "LineStyleHi",
    "VerticalLineStyle", "VerticalStrokeThickness", "BarWidth", "ColumnWidth",
    "Stroke", "StrokeColor", "IncreasingColor", "DecreasingColor",
    "Limit", "LimitLo", "LimitHi", "CandleWidth", "BoxWidth", "Fill",
    "ShowMedianAsDot", "MedianPointSize", "MedianThickness",
    "OutlierType", "OutlierSize", "ShowBox", "WhiskerWidth",
    "ErrorBarColor", "ErrorBarStopWidth", "ErrorBarStrokeThickness", "MinimumErrorSize"
};

private static readonly string[] AxisIncludedProperties = new[]
{
    "Title", "TitleColor", "TitleFont", "TitleFontSize", "TitleFontWeight",
    "AxisTitleDistance", "Unit", "TextColor", "Font", "FontSize", "FontWeight",
    "Angle", "AxisTickToLabelDistance", "UseSuperExponentialFormat",
    "AxislineColor", "AxislineStyle", "AxislineThickness", "AxisDistance",
    "PositionAtZeroCrossing", "Minimum", "Maximum",
    "MajorGridlineColor", "MajorGridlineStyle", "MajorGridlineThickness",
    "MajorStep", "MajorTickSize", "MinorGridlineColor", "MinorGridlineStyle",
    "MinorGridlineThickness", "MinorStep", "MinorTickSize",
    "TickStyle", "TicklineColor", "StartPosition", "EndPosition",
    "GapWidth", "IsTickCentered", "PowerPadding"
};
```

Since series/axes are managed by the owner app (BestFit), bridges would be created when the plot is set up and disposed on element close.

## UndoableStateBridge API Reference

```csharp
// Constructor
new UndoableStateBridge(
    source: INotifyPropertyChanged,           // Plot object
    getUndoManager: Func<IUndoManager?>,      // () => IsUndoEnabled ? UndoManager : null
    sourceDescription: string,                 // "chronology plot"
    target: object?,                           // InputData element (for action association)
    includedProperties: string[]?,             // WHITELIST of properties to monitor
    excludedProperties: string[]?              // blacklist (use one or the other, not both)
);

// Key behaviors:
// - Skips recording when undoManager.IsExecutingAction is true (prevents re-recording during undo)
// - Uses reflection (PropertyInfo) to get/set values via CLR property wrappers
// - Shadow values track "before" state for each property
// - Creates DelegateAction with execute (re-apply new) and undo (restore old) lambdas

bridge.SuspendRecording();    // Returns IDisposable; pauses recording, updates shadows on dispose
bridge.Dispose();             // Unsubscribe from INPC
bridge.ClearShadowValues();   // Reset shadow state
```

**File:** `C:\GIT\wpf-framework\src\FrameworkInterfaces\Undo\UndoableStateBridge.cs`

## Key Constraints

1. **Use `includedProperties` whitelist** — Plot inherits hundreds of properties from Control/FrameworkElement. Only monitor the ~42 OxyPlot-specific DPs.
2. **SuspendRecording during load** — When deserializing saved settings via `FromXelement`, suspend recording to avoid filling the undo stack with initialization changes.
3. **Same Plot object instance** — InputDataControl's Plot and OxyPlotPropertiesControl's Plot must reference the same object. Changes flow through the shared DP system.
4. **INPC fires only from AppearanceChanged** — DataChanged (ItemsSource, collection changes, data fields) does NOT fire INPC. This is by design for performance.
5. **Series/Axis add/remove is NOT covered** — Only visual property changes on existing objects are tracked. Collection mutations are handled by the owner app.

## Verification Plan

1. Compile RMC.BestFit after all changes
2. Open an element with saved plot settings → verify plots render correctly
3. Edit title → Ctrl+Z → verify title reverts (single property, non-destructive)
4. Edit legend position → Ctrl+Z → verify it reverts
5. Edit multiple properties → Ctrl+Z multiple times → verify each reverts
6. Edit in OxyPlotPropertiesControl → verify control stays in sync after undo
7. Close and reopen element → verify settings persist (Save uses PlotSerializer)
8. Check undo stack descriptions → verify they show meaningful text
