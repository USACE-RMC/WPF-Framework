## Purpose
Central theme management service providing Light, Blue, and Dark VS2013-style themes with swappable color dictionaries and shared control templates.

## Key Files
- `Core/ThemeService.cs` — Sealed singleton (`Lazy<T>` with ExecutionAndPublication). Initialize() loads control templates once; SetTheme() swaps color dictionary. Auto-marshals to UI thread.
- `Core/IThemeService.cs` — Interface: CurrentTheme, IsInitialized, ThemeChanged event, Initialize(Theme), SetTheme(Theme).
- `Core/Theme.cs` — Enum: Light, Blue, Dark.
- `Core/ThemeChangedEventArgs.cs` — EventArgs: OldTheme, NewTheme, ColorDictionary (ResourceDictionary).
- `Core/ThemeResourceHelper.cs` — Pack URI constants for control templates and color dictionaries. GetColorDictionaryUri(Theme), ParseTheme/TryParseTheme.
- `Core/ColorKeys.cs` — ~100 string constants for theme color resource keys following Control.State.Property naming.
- `Resources/Controls/` — XAML control templates (loaded once at initialization).
- `Resources/Colors/LightColors.xaml`, `BlueColors.xaml`, `DarkColors.xaml` — Swappable color dictionaries.

## Dependencies
- .NET 9 (net9.0-windows), WPF
- No internal project dependencies

## Patterns
- **Singleton with lazy init**: `ThemeService.Instance` uses `Lazy<T>(LazyThreadSafetyMode.ExecutionAndPublication)`.
- **Two-phase loading**: Initialize() loads control templates XAML once + initial colors. SetTheme() only swaps the color dictionary afterward.
- **Thread-safe theme switching**: SetTheme() uses lock + Dispatcher.Invoke to marshal to UI thread. ThemeChanged event raised outside lock.
- **Color key convention**: Constants in ColorKeys.cs follow `Control.State.Property` (e.g., `Button.Static.Background`, `TextBox.Focus.Border`).
- **Pack URI resources**: All XAML loaded via pack://application URIs defined in ThemeResourceHelper.

## Gotchas
- ThemeService.Initialize() must be called before SetTheme() — calling SetTheme() on uninitialized service throws.
- Control templates are loaded ONCE at initialization and never swapped — only color dictionaries change on theme switch.
- ThemeChanged event fires outside the lock to prevent deadlocks from subscriber callbacks.
- ColorKeys are plain strings, not ComponentResourceKeys — use DynamicResource with string keys in XAML.
- DynamicResource does NOT work inside DrawingBrush/GeometryDrawing (Freezables outside visual tree). Use Canvas > Path instead.
