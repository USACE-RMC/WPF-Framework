# Migration Guide

This guide helps you upgrade existing applications to the latest version of WPF-Framework.

## Breaking Changes Summary

| Change | Impact | Action Required |
|--------|--------|-----------------|
| `IMessageItem.ToText()` | Interface change | Implement method or use `BasicMessageItem` |
| Themes library dependency | New runtime DLL | Add `Themes.dll` to deployment |
| `Test_ProjectUI` renamed | Project name change | Update references to `Demo_FrameworkUI` |

## Migrating from Previous Versions

### Step 1: Add Themes Library Reference

The `FrameworkUI` library now depends on the `Themes` library.

**If using DLL references:**

Add `Themes.dll` to your project references and ensure it's copied to your output directory.

**If using project references:**

```xml
<ProjectReference Include="..\Themes\Themes.csproj">
  <Project>{A1B2C3D4-E5F6-7890-ABCD-EF1234567890}</Project>
  <Name>Themes</Name>
</ProjectReference>
```

### Step 2: Update Theme Initialization

**Old approach (still works but deprecated):**

```xml
<!-- App.xaml -->
<ResourceDictionary Source="pack://application:,,,/FrameworkUI;component/Themes/VS2013/LightTheme.xaml"/>
```

**New approach (recommended):**

```xml
<!-- App.xaml - remove manual theme dictionary -->
<ResourceDictionary.MergedDictionaries>
    <ResourceDictionary Source="pack://application:,,,/FrameworkUI;component/Icons/IconDictionary.xaml"/>
    <!-- Theme loaded by ThemeManager.SetTheme() -->
</ResourceDictionary.MergedDictionaries>
```

```csharp
// App.xaml.cs - initialize theme before creating UI
private void Application_Startup(object sender, StartupEventArgs e)
{
    // NEW: Initialize theme system first
    FrameworkUI.ThemeManager.SetTheme(FrameworkUI.ThemeColor.Light);

    // Then create your window
    var mainWindow = new MainWindow();
    mainWindow.Show();
}
```

### Step 3: Update IMessageItem Implementations

If you have custom classes implementing `IMessageItem` directly, add the new `ToText()` method:

```csharp
public class MyMessageItem : IMessageItem
{
    // ... existing implementation ...

    // NEW: Add this method
    public string ToText()
    {
        return $"[{TimeStamp}] [{Type}] {Code}: {Description} (Source: {SourceName})";
    }
}
```

**Alternative:** Switch to using `BasicMessageItem` which already implements this method.

### Step 4: Update Project References (if using Test_ProjectUI)

The example project has been renamed:

- **Old name:** `Test_ProjectUI`
- **New name:** `Demo_FrameworkUI`

Update any build scripts or references accordingly.

## New Features Available After Migration

### Undo/Redo Support

Your elements can now support undo/redo:

```csharp
public class MyElement : ElementBase, IUndoableElement
{
    private string _value;

    public string Value
    {
        get => _value;
        set => SetPropertyWithUndo(ref _value, value, nameof(Value));
    }
}
```

See [Undo/Redo Guide](undo-redo.md) for complete documentation.

### Runtime Theme Switching

Users can now switch themes without restarting:

```csharp
// Switch themes at runtime
ThemeManager.SetTheme(ThemeColor.Dark);

// Subscribe to changes
ThemeManager.ThemeChanged += (dict, color) =>
{
    // Update your UI if needed
};
```

See [Themes Guide](themes.md) for complete documentation.

### New UserSettings Property

A new setting controls undo/redo button visibility:

```csharp
// Show/hide undo buttons on toolbar
UserSettings.ShowUndoRedoButtons = true;
```

Existing user settings files will use the default value (`true`).

## Backwards Compatibility

### Preserved APIs

The following APIs work exactly as before:

- All `IProject`, `IElement`, `IElementCollection` interfaces
- All `ElementBase` public methods and properties
- `Messenger` singleton and all methods
- `ThemeManager.SetTheme()` method signature
- `UserSettings` load/save (new properties have defaults)
- All `Node` classes and their methods

### Deprecated Patterns

**Manual theme dictionary loading** - While still functional, prefer using `ThemeManager.SetTheme()` for proper initialization:

```csharp
// Deprecated (but works)
Application.Current.Resources.MergedDictionaries.Add(
    new ResourceDictionary { Source = new Uri("...LightTheme.xaml") });

// Preferred
ThemeManager.SetTheme(ThemeColor.Light);
```

## Troubleshooting Migration Issues

### "Could not load file or assembly 'Themes'"

**Cause:** Missing Themes.dll at runtime.

**Solution:** Ensure Themes.dll is in your output directory. If using project references, verify the reference is correct. If using DLL references, set "Copy Local" to True.

### Theme not applying after update

**Cause:** Theme initialization happening after UI creation.

**Solution:** Move `ThemeManager.SetTheme()` to the very beginning of `Application_Startup`:

```csharp
private void Application_Startup(object sender, StartupEventArgs e)
{
    ThemeManager.SetTheme(ThemeColor.Light);  // FIRST!
    // ... rest of initialization
}
```

### "does not implement interface member 'IMessageItem.ToText()'"

**Cause:** Custom `IMessageItem` implementation missing new method.

**Solution:** Add the `ToText()` method to your implementation:

```csharp
public string ToText()
{
    return $"[{TimeStamp}] {Type}: {Description}";
}
```

### Undo/Redo buttons not appearing

**Cause:** `ShowUndoRedoButtons` setting is false.

**Solution:** Check `UserSettings.ShowUndoRedoButtons` or delete the user settings file to reset to defaults.

### Build errors after updating solution

**Cause:** Missing project references or incorrect build order.

**Solution:**
1. Verify solution includes the Themes project
2. Check build order: FrameworkInterfaces → Themes → FrameworkUI → Demo_FrameworkUI
3. Clean and rebuild the entire solution

## Checklist

Use this checklist when upgrading:

- [ ] Added Themes library reference
- [ ] Updated App.xaml (removed manual theme dictionary)
- [ ] Added `ThemeManager.SetTheme()` to Application_Startup
- [ ] Implemented `ToText()` in any custom `IMessageItem` classes
- [ ] Updated references from `Test_ProjectUI` to `Demo_FrameworkUI`
- [ ] Tested with all three themes (Light, Blue, Dark)
- [ ] Verified undo/redo works in MainWindow (Ctrl+Z, Ctrl+Y)
- [ ] Clean build with no warnings

## Getting Help

If you encounter issues not covered here:

1. Check the [Architecture](architecture.md) document for system overview
2. Review `Demo_FrameworkUI` for working examples
3. Examine the source code - all public APIs have XML documentation
