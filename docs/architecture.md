# Architecture Overview

This document describes the architecture of the WPF-Framework, including project dependencies, design patterns, and key components.

## Project Dependencies

```
┌─────────────────────────────────────────────────────────────┐
│                      Demo_FrameworkUI                        │
│                    (Example Application)                     │
└─────────────────────────────────────────────────────────────┘
                              │
              ┌───────────────┼───────────────┐
              ▼               ▼               ▼
┌─────────────────┐  ┌─────────────┐  ┌─────────────┐
│   FrameworkUI   │  │   Themes    │  │ GenericCtrls│
│  (WPF UI Lib)   │  │ (Theming)   │  │  (Controls) │
└─────────────────┘  └─────────────┘  └─────────────┘
         │                   │
         ▼                   │
┌─────────────────┐          │
│FrameworkInterfaces│◄─────────┘
│ (Core Abstracts)│
└─────────────────┘
```

### Dependency Rules

1. **FrameworkInterfaces** - No dependencies on other WPF-Framework libraries
2. **Themes** - No dependencies (fully independent)
3. **FrameworkUI** - Depends on FrameworkInterfaces and Themes
4. **Demo_FrameworkUI** - Depends on all libraries (for demonstration)

## Design Patterns

### Singleton Pattern

Several core services use the singleton pattern for application-wide state:

| Class | Access Method | Thread-Safe |
|-------|---------------|-------------|
| `Messenger` | `Messenger.GetInstance()` | Yes (Lazy<T>) |
| `ThemeService` | `ThemeService.Instance` | Yes (Lazy<T>) |

```csharp
// Thread-safe singleton implementation
private static readonly Lazy<ThemeService> _instance =
    new Lazy<ThemeService>(() => new ThemeService(),
        LazyThreadSafetyMode.ExecutionAndPublication);

public static ThemeService Instance => _instance.Value;
```

### Model-View-Controller (MVC)

The framework follows an MVC-like pattern:

- **Model**: `IProject`, `IElement`, `IElementCollection`
- **View**: WPF controls, `MainWindow`, document editors
- **Controller**: `FrameworkUIController` subclasses

### Command Pattern

The undo/redo system uses the Command pattern:

```csharp
// IUndoableAction represents a reversible command
public interface IUndoableAction
{
    string Description { get; }
    void Execute();
    void Undo();
}

// UndoManager maintains stacks of commands
public class UndoManager : IUndoManager
{
    private Stack<IUndoableAction> _undoStack;
    private Stack<IUndoableAction> _redoStack;
}
```

### Observer Pattern

Property change notifications use `INotifyPropertyChanged`:

```csharp
public abstract class ElementBase : IElement, INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;

    protected void NotifyPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
```

## Core Components

### FrameworkInterfaces

The foundation layer containing:

| Component | Purpose |
|-----------|---------|
| `IProject` | Root project interface |
| `IElement` | Individual element interface |
| `IElementCollection` | Collection of elements |
| `ElementBase` | Abstract base with common functionality |
| `Messenger` | Centralized messaging system |
| `UndoManager` | Undo/redo stack management |

### FrameworkUI

The WPF UI layer containing:

| Component | Purpose |
|-----------|---------|
| `MainWindow` | Application shell with docking |
| `FrameworkUIController` | Bridge between model and UI |
| `Node` hierarchy | Tree view representation |
| `ThemeManager` | Theme bridge to Themes library |
| `UserSettings` | Persistent preferences |

### Themes

Independent theming library:

| Component | Purpose |
|-----------|---------|
| `ThemeService` | Singleton theme manager |
| `Theme` enum | Light, Blue, Dark |
| Color dictionaries | Theme color definitions |
| Control templates | Styled WPF controls |

## Data Flow

### Project Loading

```
User opens file
       │
       ▼
┌──────────────────┐
│  MainWindow      │ ── Calls ──▶ ProjectNode.Load()
└──────────────────┘
       │
       ▼
┌──────────────────┐
│FrameworkUIController│ ── Deserializes ──▶ IProject
└──────────────────┘
       │
       ▼
┌──────────────────┐
│  ElementNodes    │ ◀── Created for each ── IElement
└──────────────────┘
```

### Theme Switching

```
User selects theme
       │
       ▼
┌──────────────────┐
│  ThemeManager    │ ── Calls ──▶ ThemeService.SetTheme()
│   (FrameworkUI)    │
└──────────────────┘
       │
       ├──▶ Removes old color dictionary
       ├──▶ Adds new color dictionary
       └──▶ Raises ThemeChanged event
              │
              ▼
       All DynamicResource bindings update automatically
```

### Undo/Redo Flow

```
User changes property
       │
       ▼
┌──────────────────┐
│SetPropertyWithUndo│ ── Creates ──▶ PropertyChangeAction
└──────────────────┘
       │
       ▼
┌──────────────────┐
│   UndoManager    │ ── Pushes to ──▶ Undo Stack
└──────────────────┘
       │
User presses Ctrl+Z
       │
       ▼
┌──────────────────┐
│   UndoManager    │ ── Pops and calls ──▶ action.Undo()
└──────────────────┘
       │
       ▼
┌──────────────────┐
│PropertyChangeAction│ ── Restores ──▶ Original value
└──────────────────┘
```

## Node Hierarchy

The project explorer uses a hierarchical node system:

```
Node (abstract base)
├── ProjectNode         - Root project node
├── ElementNode         - Represents an IElement
├── NodeCollection      - Container for elements
├── NodeGroup           - User-created grouping
└── SimpleNode          - Basic custom node
```

### Node Responsibilities

| Node Type | Responsibilities |
|-----------|------------------|
| `Node` | Selection, rename, drag-drop, context menu |
| `ProjectNode` | Project-level operations, child loading |
| `ElementNode` | Element binding, document opening |
| `NodeCollection` | Add/remove elements, group management |
| `NodeGroup` | Grouping, expand/collapse |

## Message System

The messaging system provides application-wide notifications:

```
┌─────────────┐     ┌─────────────┐     ┌─────────────────┐
│   Source    │────▶│  Messenger  │────▶│ MessageWindow   │
│  (Element)  │     │ (Singleton) │     │   (Display)     │
└─────────────┘     └─────────────┘     └─────────────────┘
                           │
                           ▼
                    Keyed by Source + Code
                    (prevents duplicates)
```

### Message Types

| Type | Purpose | Default Color |
|------|---------|---------------|
| `Error` | Critical issues | Red |
| `Warning` | Potential problems | Orange |
| `Message` | Informational | Blue |
| `Event` | User actions | Green |

## Threading Considerations

### UI Thread Requirements

- All WPF UI updates must occur on the UI thread
- `ThemeService.SetTheme()` automatically marshals to UI thread
- Use `Dispatcher.Invoke()` for cross-thread UI updates

### Thread-Safe Components

| Component | Thread Safety |
|-----------|---------------|
| `ThemeService` | Full (lock-based) |
| `Messenger` | Lazy initialization only |
| `UndoManager` | Not thread-safe (UI thread only) |

## Extension Points

### Custom Elements

Extend `ElementBase` to create new element types:

```csharp
public class CustomElement : ElementBase, IUndoableElement
{
    // Add custom properties and behavior
}
```

### Custom Controllers

Extend `FrameworkUIController` to customize:

- Menu items
- Context menus
- Document editors
- Properties panels

### Custom Themes

Add new color dictionaries following the existing pattern in `Themes/Resources/Colors/`.

## File Formats

### User Settings (XML)

```xml
<UserSettings>
    <ColorTheme>Light</ColorTheme>
    <SaveWindowLayout>true</SaveWindowLayout>
    <MaxRecentFileItems>10</MaxRecentFileItems>
    <ShowUndoRedoButtons>true</ShowUndoRedoButtons>
    <!-- ... -->
</UserSettings>
```

### Window Layout (AvalonDock XML)

Persisted automatically by Xceed.Wpf.AvalonDock for docking state restoration.
