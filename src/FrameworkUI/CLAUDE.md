## Purpose
Complete WPF application shell providing AvalonDock layout, project explorer, message window, recent files, user settings, and theme bridging.

## Key Files
- `Main Window/MainWindow.xaml.cs` — Application shell. ThemeChanged handler swaps color dictionaries on MainWindow.Resources.MergedDictionaries.
- `Themes/ThemeManager.cs` — Static bridge to Themes.ThemeService. Loads FrameworkUI-specific XAML themes, raises ThemeChanged event.
- `Project Explorer/View Models/ProjectExplorerTreeView.cs` — Extends ExplorerTreeView. ProjectNode dependency property. Drag-drop code is commented out (WIP).
- `Recent Files/RecentFiles.cs` — Extends Separator. XML persistence + Windows JumpList integration. Dynamic menu item generation on submenu open.
- `User Settings/UserSettings.cs` — All-static properties persisted to XML. Categories: General, File Management, Message Window, Defaults.
- `Message Window/Views/MessageWindowControl.xaml.cs` — Subscribes to Messenger singleton. Filtered message list with toggle buttons. Export to text. Thread-safe via Dispatcher.

## Dependencies
- **FrameworkInterfaces** — IProject, IElement, Messenger, IUndoManager
- **Themes** — ThemeService, Theme enum, ThemeChangedEventArgs
- **AvalonDock** — Xceed.Wpf.AvalonDock (layout management)
- .NET 9 (net9.0-windows), WPF

## Patterns
- **Two-layer theme system**: ThemeManager (FrameworkUI) bridges to ThemeService (Themes). ThemeManager loads FrameworkUI-specific resources on top of core theme.
- **Static UserSettings**: All properties are static with XML Load()/Save(). ValueStringFormat derived from DefaultValueDigits.
- **RecentFiles as menu control**: Inherits Separator, generates MenuItem children dynamically. Overflow shows "More Files..." dialog.
- **Message filtering**: MessageWindowControl maintains filtered ObservableCollection, toggled by Error/Warning/Message/Event buttons.
- **Messenger subscription**: MessageWindowControl subscribes to Messenger.MessageAdded/MessageRemoved/MessagesCleared events.

## Gotchas
- Theme dictionaries go on MainWindow.Resources.MergedDictionaries, NOT DockingManager.Resources — floating windows are separate Win32 windows.
- ThemeManager.SetTheme() must be called before any UI renders or theme resources will be missing.
- ProjectExplorerTreeView drag-drop is mostly commented out — do not assume it works.
- RecentFiles.InsertFile() auto-trims list to MaxRecentFileItems from UserSettings.
- UserSettings.Load() silently handles missing/corrupt XML and uses defaults.
- MessageWindowControl uses Dispatcher.Invoke for thread-safe UI updates from Messenger events.
