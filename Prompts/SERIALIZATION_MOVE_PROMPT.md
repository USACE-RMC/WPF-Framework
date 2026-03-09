# Session Goal

Move all OxyPlot plot settings serialization from OxyPlotControls (wpf-framework) to OxyPlot.Wpf (oxyplot repo). This puts serialization where it belongs — alongside the Plot/Series/Axis/Annotation classes it operates on.

## Background — Why This Is Needed

Plot settings serialization (`ToXelement`/`FromXelement`) is currently scattered across OxyPlotControls UI controls as static methods. Each control (GeneralPlotControl, LegendControl, AxisControl, etc.) has static serialization methods that read/write Plot DependencyProperties to XML. This creates an unnecessary coupling — the serialization logic has zero dependency on the UI controls or wpf-framework, it only uses OxyPlot.Wpf types and standard WPF/XML types.

Moving serialization to OxyPlot.Wpf:
- Eliminates circular knowledge (UI controls knowing about serialization)
- Makes serialization available to any consumer of OxyPlot.Wpf (not just OxyPlotControls users)
- Enables RMC.BestFit.UI to reference serialization directly without going through OxyPlotControls

## Repository Structure

```
C:\GIT\oxyplot\Source\OxyPlot.Wpf\       <- Target: new serializer classes go here
C:\GIT\wpf-framework\src\OxyPlotControls\ <- Source: serialization methods to extract
```

## Current Architecture

### OxyPlotSettingsSerializer.cs (orchestrator)
**File:** `C:\GIT\wpf-framework\src\OxyPlotControls\OxyPlotSettingsSerializer.cs`

- `ToXelement(Plot)` — calls 5 sub-serializers to build complete XML
- `FromXelement(Plot, XElement)` — calls 5 sub-deserializers to restore from XML
- 14 static helper methods for XML attribute parsing (GetColorAttribute, GetDoubleAttribute, etc.)
- Tag: `OxyplotPropertiesTag = "OxyplotProperties"`

### Sub-serializers (static methods on UI controls)

| Control | Tag | ToXElement Method | FromXElement Method | Operates On |
|---------|-----|-------------------|---------------------|-------------|
| `GeneralPlotControl` | `"General"` | `GeneralPropertiesToXElement(Plot)` | `XElementToGeneralProperties(Plot, XElement)` | Plot title, subtitle, background, border, padding, plot area |
| `LegendControl` | `"Legend"` | `LegendPropertiesToXElement(Plot)` | `XElementToLegendProperties(Plot, XElement)` | Plot legend properties (~28 properties) |
| `AxesControl` | `"Axes"` | `AxesPropertiesToXElement(Plot)` | `XElementToAxesProperties(Plot, XElement)` | Iterates Plot.Axes, delegates to AxisControl |
| `AxisControl` | `"Axis"` | `AxisPropertiesToXElement(Axis)` | `XElementToAxisProperties(XElement, Axis?)` returns Axis? | All axis types and their ~50 properties |
| `GenericSeriesControl` | `"Series"` | `SeriesPropertiesToXElement(Plot)` + `SeriesPropertiesToXElement(Series)` | `XElementToSeriesProperties(Plot, XElement)` + `XElementToSeriesProperties(XElement)` returns Series? | All series types and their properties |
| `AnnotationSelectorControl` | `"Annotations"` | `AnnotationsPropertiesToXElement(Plot)` | `XElementToAnnotationsProperties(Plot, XElement)` | Iterates Plot.Annotations, delegates to AnnotationControl |
| `AnnotationControl` | `"Annotation"` | `AnnotationPropertiesToXElement(TextualAnnotation)` | `XElementToAnnotationProperties(XElement)` returns TextualAnnotation? | All annotation types |

### Extension Methods (data type conversions)
**File:** `C:\GIT\wpf-framework\src\OxyPlotControls\Extensions.cs`

Serialization-related extensions (need to move):
- `ToPrettyText(ScreenVector)` / `FromPrettyVectorText(string)`
- `ToPrettyText(DataPoint)` / `FromPrettyDataText(string)`
- `ToPrettyText(ScreenPoint)` / `FromPrettyScreenText(string)`
- `ToPrettyText(Vector)` / `FromPrettyVectorString(string)`
- `ToXElement(DataPoint)` / `PointFromXElement(XElement)`
- `ToXElement(IList<DataPoint>, string)` / `PointsFromXElement(XElement)`

Non-serialization extensions (stay in OxyPlotControls):
- `CopyBinding()`, `IsBound()` — WPF binding helpers
- `FromAxisProperties()` — axis property copying for UI
- `BindingConvertor` class — XAML binding converter
- `EditorHelper` class — type descriptor registration

## Target Architecture

### New files in `C:\GIT\oxyplot\Source\OxyPlot.Wpf\`

```
Serialization\
├── PlotSerializer.cs          <- Top-level orchestrator + helper methods + General + Legend
├── AxisSerializer.cs          <- Axis property serialization (all axis types)
├── SeriesSerializer.cs        <- Series property serialization (all series types)
├── AnnotationSerializer.cs    <- Annotation property serialization (all annotation types)
└── SerializerExtensions.cs    <- DataPoint/ScreenPoint/ScreenVector conversion extensions
```

### PlotSerializer.cs API

```csharp
namespace OxyPlot.Wpf.Serialization
{
    public static class PlotSerializer
    {
        public static readonly string OxyplotPropertiesTag = "OxyplotProperties";
        public static readonly string GeneralPropertiesTag = "General";
        public static readonly string LegendPropertiesTag = "Legend";

        // Top-level
        public static XElement ToXelement(Plot plot);
        public static void FromXelement(Plot plot, XElement element);

        // General (title, subtitle, background, plot area)
        public static XElement GeneralPropertiesToXElement(Plot plot);
        public static void XElementToGeneralProperties(Plot plot, XElement element);

        // Legend
        public static XElement LegendPropertiesToXElement(Plot plot);
        public static void XElementToLegendProperties(Plot plot, XElement element);

        // XML attribute helpers (moved from OxyPlotSettingsSerializer)
        public static bool GetColorAttribute(XElement el, string attributeName, out Color c);
        public static bool GetBrushAttribute(XElement el, string attributeName, BrushConverter converter, out Brush? b);
        public static bool GetStringAttribute(XElement el, string attributeName, out string? s);
        public static bool GetDoubleAttribute(XElement el, string attributeName, out double d);
        public static bool GetIntegerAttribute(XElement el, string attributeName, out int i);
        public static bool GetBooleanAttribute(XElement el, string attributeName, out bool b);
        public static bool GetFontFamilyAttribute(XElement el, string attributeName, FontFamilyConverter converter, out FontFamily? ff);
        public static bool GetFontWeightAttribute(XElement el, string attributeName, FontWeightConverter converter, out FontWeight fw);
        public static bool GetThicknessAttribute(XElement el, string attributeName, ThicknessConverter converter, out Thickness t);
        public static bool GetEnumAttribute<TEnum>(XElement el, string attributeName, out TEnum e) where TEnum : struct;
        public static bool GetDataPointAttribute(XElement el, string attributeName, out DataPoint dp);
        public static bool GetScreenVectorAttribute(XElement el, string attributeName, out ScreenVector vp);
        public static bool GetScreenPointAttribute(XElement el, string attributeName, out ScreenPoint vp);
        public static bool GetVectorAttribute(XElement el, string attributeName, out Vector v);
    }
}
```

## Dependencies

The serialization code has **zero** wpf-framework dependencies:
- Uses only `System.Windows.*` WPF types (Color, Brush, FontFamily, FontWeight, Thickness, Vector)
- Uses only `System.Xml.Linq` for XML
- Uses only `OxyPlot.*` and `OxyPlot.Wpf.*` types
- Does NOT reference GenericControls, Themes, FrameworkInterfaces, or any framework project

## Implementation Steps

### Step 1: Create SerializerExtensions.cs
Move the data type conversion extensions from `Extensions.cs` to new file in OxyPlot.Wpf.

### Step 2: Create PlotSerializer.cs
- Move all 14 helper methods from `OxyPlotSettingsSerializer`
- Move `GeneralPropertiesToXElement` / `XElementToGeneralProperties` from `GeneralPlotControl`
- Move `LegendPropertiesToXElement` / `XElementToLegendProperties` from `LegendControl`
- Add `ToXelement` / `FromXelement` orchestrators

### Step 3: Create AxisSerializer.cs
- Move `AxisPropertiesToXElement(Axis)` from `AxisControl`
- Move `XElementToAxisProperties(XElement, Axis?)` from `AxisControl`
- Add `AxesPropertiesToXElement(Plot)` / `XElementToAxesProperties(Plot, XElement)` from `AxesControl`

### Step 4: Create SeriesSerializer.cs
- Move `SeriesPropertiesToXElement(Series)` / `XElementToSeriesProperties(XElement)` from `GenericSeriesControl`
- Move `SeriesPropertiesToXElement(Plot)` / `XElementToSeriesProperties(Plot, XElement)` from `GenericSeriesControl`
- Move BarSeries-specific methods from `BarSeriesControl` if they exist

### Step 5: Create AnnotationSerializer.cs
- Move `AnnotationPropertiesToXElement(TextualAnnotation)` / `XElementToAnnotationProperties(XElement)` from `AnnotationControl`
- Add `AnnotationsPropertiesToXElement(Plot)` / `XElementToAnnotationsProperties(Plot, XElement)` from `AnnotationSelectorControl`

### Step 6: Update OxyPlotControls
- Remove static serialization methods from all UI controls
- Update `OxyPlotSettingsSerializer` to delegate to `PlotSerializer` (or remove entirely)
- Remove moved extension methods from `Extensions.cs`

### Step 7: Update rmc-bestfit
- Change `OxyPlotSettingsSerializer.ToXelement()` → `PlotSerializer.ToXelement()`
- Change `OxyPlotSettingsSerializer.FromXelement()` → `PlotSerializer.FromXelement()`
- Update using statements

## Key Files to Read (Source)

```
C:\GIT\wpf-framework\src\OxyPlotControls\OxyPlotSettingsSerializer.cs     <- orchestrator + helpers
C:\GIT\wpf-framework\src\OxyPlotControls\GeneralPlotControl.xaml.cs       <- General serialization (~lines 186-378)
C:\GIT\wpf-framework\src\OxyPlotControls\LegendControl.xaml.cs            <- Legend serialization (~lines 265-465)
C:\GIT\wpf-framework\src\OxyPlotControls\Axes\AxisControl.xaml.cs         <- Axis serialization (large, ~500 lines)
C:\GIT\wpf-framework\src\OxyPlotControls\Axes\AxesControl.xaml.cs         <- Axes collection orchestrator
C:\GIT\wpf-framework\src\OxyPlotControls\Series\GenericSeriesControl.xaml.cs <- Series serialization (large)
C:\GIT\wpf-framework\src\OxyPlotControls\Series\BarSeriesControl.xaml.cs  <- BarSeries-specific serialization
C:\GIT\wpf-framework\src\OxyPlotControls\Annotations\AnnotationControl.xaml.cs <- Annotation serialization
C:\GIT\wpf-framework\src\OxyPlotControls\Annotations\AnnotationSelectorControl.xaml.cs <- Annotations orchestrator
C:\GIT\wpf-framework\src\OxyPlotControls\Extensions.cs                    <- Data type conversion extensions
```

## Constraints

1. **All .cs files need XML documentation** (summary, param, returns, remarks).
2. **All .cs files need OxyPlot license header.**
3. **Backward compatibility**: Deserialization must handle old attribute names (e.g., "Color" vs "TitleColor" in GeneralProperties).
4. **XML format must be identical** to current output — existing saved plot settings in databases must continue to load.
5. **Culture-invariant**: All numeric parsing uses `CultureInfo.InvariantCulture`.

## Verification

1. Compile OxyPlot.Wpf with new serializer classes
2. Compile OxyPlotControls after removing old serialization methods
3. Round-trip test: `PlotSerializer.ToXelement(plot)` then `PlotSerializer.FromXelement(plot2, xml)` — verify identical properties
4. Load existing saved plot settings from rmc-bestfit database — verify they deserialize correctly
