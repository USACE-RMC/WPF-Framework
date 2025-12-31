# Themes Guide

The Themes library provides a centralized, runtime-switchable theming system for WPF applications. It uses a VS2013-inspired color palette with Light, Dark, and Blue variants.

## Overview

The theming system consists of two main components:

1. **Themes Library** (`Themes.dll`) - Independent theming engine
2. **ThemeManager** (`FrameworkUI`) - Bridge that adds FrameworkUI-specific resources

## Quick Start

### Initialize Theme on Startup

```csharp
// In App.xaml.cs - MUST be called before creating any UI
private void Application_Startup(object sender, StartupEventArgs e)
{
    // Initialize theme system
    FrameworkUI.ThemeManager.SetTheme(FrameworkUI.ThemeColor.Light);

    // Now create your window
    var mainWindow = new MainWindow();
    mainWindow.Show();
}
```

### Switch Themes at Runtime

```csharp
// Switch to dark theme - all controls update automatically
FrameworkUI.ThemeManager.SetTheme(FrameworkUI.ThemeColor.Dark);

// Or use the Themes library directly
Themes.ThemeService.Instance.SetTheme(Themes.Theme.Blue);
```

### Subscribe to Theme Changes

```csharp
// FrameworkUI level
FrameworkUI.ThemeManager.ThemeChanged += (dictionary, color) =>
{
    Console.WriteLine($"Theme changed to: {color}");
};

// Themes library level
Themes.ThemeService.Instance.ThemeChanged += (sender, e) =>
{
    Console.WriteLine($"Changed from {e.OldTheme} to {e.NewTheme}");
};
```

## Available Themes

| Theme | Description | Best For |
|-------|-------------|----------|
| `Light` | White/light gray palette | Well-lit environments, default |
| `Blue` | Blue accent colors | Professional appearance |
| `Dark` | Dark gray/black palette | Low-light, extended use |

## Architecture

### How It Works

```
ThemeManager.SetTheme(ThemeColor.Dark)
       │
       ├──▶ ThemeService.Instance.SetTheme(Theme.Dark)
       │           │
       │           ├──▶ Remove old color dictionary
       │           ├──▶ Add new color dictionary (DarkColors.xaml)
       │           └──▶ Raise ThemeChanged event
       │
       └──▶ Load FrameworkUI-specific resources (DarkTheme.xaml)
```

### Resource Loading Order

1. **Control Templates** (Merged.xaml) - Loaded once at initialization
2. **Color Dictionary** - Swapped when theme changes
3. **FrameworkUI Resources** - Additional styles for MainWindow, etc.

### Why DynamicResource?

All color references use `DynamicResource` instead of `StaticResource`:

```xml
<!-- In control template -->
<Border Background="{DynamicResource Button.Background}">
```

This allows colors to update at runtime when the theme changes, without reloading control templates.

## Using Themes in Custom Controls

### Reference Theme Colors

```xml
<UserControl ...>
    <Grid Background="{DynamicResource Window.Background}">
        <TextBlock Foreground="{DynamicResource Window.Foreground}"
                   Text="Themed text"/>
        <Button Background="{DynamicResource Button.Background}"
                Foreground="{DynamicResource Button.Foreground}"
                Content="Themed button"/>
    </Grid>
</UserControl>
```

### Common Color Keys

| Key | Purpose |
|-----|---------|
| `Window.Background` | Main window background |
| `Window.Foreground` | Main window text |
| `Button.Background` | Button background |
| `Button.Foreground` | Button text |
| `Button.MouseOver.Background` | Button hover state |
| `TextBox.Background` | Text input background |
| `TextBox.Foreground` | Text input text |
| `Menu.Background` | Menu background |
| `TreeView.Background` | Tree view background |
| `DataGrid.Background` | Data grid background |

For a complete list, see `Themes/Core/ColorKeys.cs` or the color XAML files.

## Standalone Usage (Without FrameworkUI)

The Themes library can be used independently:

```csharp
using Themes;

// In your App.xaml.cs
protected override void OnStartup(StartupEventArgs e)
{
    base.OnStartup(e);

    // Initialize with a theme
    ThemeService.Instance.Initialize(Theme.Light);
}

// Later, to change themes
ThemeService.Instance.SetTheme(Theme.Dark);
```

### App.xaml Setup for Standalone

```xml
<Application ...>
    <Application.Resources>
        <!-- Theme resources are loaded by ThemeService.Initialize() -->
        <!-- Do NOT add theme dictionaries here manually -->
    </Application.Resources>
</Application>
```

## Creating Custom Themes

### Step 1: Create Color Dictionary

Create a new XAML file in `Themes/Resources/Colors/`:

```xml
<!-- CustomColors.xaml -->
<ResourceDictionary xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
                    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml">

    <!-- Window colors -->
    <SolidColorBrush x:Key="Window.Background" Color="#FF1E1E2E"/>
    <SolidColorBrush x:Key="Window.Foreground" Color="#FFCDD6F4"/>

    <!-- Button colors -->
    <SolidColorBrush x:Key="Button.Background" Color="#FF313244"/>
    <SolidColorBrush x:Key="Button.Foreground" Color="#FFCDD6F4"/>
    <!-- ... define all required color keys ... -->

</ResourceDictionary>
```

### Step 2: Add Theme Enum Value

In `Themes/Core/Theme.cs`:

```csharp
public enum Theme
{
    Light,
    Blue,
    Dark,
    Custom  // Add your new theme
}
```

### Step 3: Register Color Dictionary URI

In `Themes/Core/ThemeResourceHelper.cs`:

```csharp
public const string CustomColorsUri =
    "pack://application:,,,/Themes;component/Resources/Colors/CustomColors.xaml";

private static readonly Dictionary<Theme, string> ThemeColorUris = new Dictionary<Theme, string>
{
    { Theme.Light, LightColorsUri },
    { Theme.Blue, BlueColorsUri },
    { Theme.Dark, DarkColorsUri },
    { Theme.Custom, CustomColorsUri }  // Add mapping
};
```

## Styled Controls

The Themes library provides templates for these standard WPF controls:

- Button, ToggleButton, RepeatButton
- TextBox, PasswordBox
- CheckBox, RadioButton
- ComboBox
- ListBox, ListView
- DataGrid
- TreeView
- TabControl
- Menu, ContextMenu, MenuItem
- ToolBar
- ScrollBar, ScrollViewer
- Slider
- ProgressBar
- Expander
- GroupBox
- Label
- ToolTip
- Separator
- GridSplitter
- StatusBar

## Best Practices

### DO

- Initialize themes before creating UI
- Use `DynamicResource` for all theme color references
- Test your UI with all three themes
- Handle theme changes in custom controls if needed

### DON'T

- Hardcode colors in XAML or code
- Use `StaticResource` for theme colors
- Merge theme dictionaries manually in App.xaml
- Create UI before initializing themes

## Troubleshooting

### Controls not themed

Ensure the theme is initialized before any UI is created:

```csharp
// This is WRONG
var window = new MainWindow();
ThemeManager.SetTheme(ThemeColor.Dark);

// This is CORRECT
ThemeManager.SetTheme(ThemeColor.Dark);
var window = new MainWindow();
```

### Theme switch doesn't update all controls

Make sure you're using `DynamicResource`, not `StaticResource`:

```xml
<!-- WRONG - won't update at runtime -->
<Border Background="{StaticResource Window.Background}"/>

<!-- CORRECT - updates when theme changes -->
<Border Background="{DynamicResource Window.Background}"/>
```

### Missing color keys

If you get "resource not found" errors, check that:
1. The Themes assembly is referenced
2. `ThemeService.Initialize()` or `ThemeManager.SetTheme()` was called
3. The color key exists in all theme color dictionaries
