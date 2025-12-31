# ProjectControls

A WPF (Windows Presentation Foundation) framework for building project management applications with hierarchical tree structures, messaging systems, undo/redo support, and extensible UI components.

## Overview

ProjectControls provides a foundation for creating Windows desktop applications that manage projects containing hierarchical elements. It includes:

- **Project Explorer**: A tree-based interface for navigating and managing project elements
- **Messaging System**: Centralized logging and notification system for errors, warnings, and events
- **Undo/Redo System**: Full undo/redo support for element properties and collection operations
- **Themes Library**: Independent theming system with VS2013-style themes (Light, Dark, Blue)
- **User Settings**: Persistent application settings with XML serialization
- **Auto-Backup**: Automatic project backup functionality

## Project Structure

```
ProjectControls/
├── FrameworkInterfaces/          # Core interfaces and base classes
│   ├── Messaging/              # Messenger, IMessageItem, BasicMessageItem
│   ├── Project/                # IProject, IElement, ElementBase
│   ├── Undo/                   # IUndoManager, UndoManager, action classes
│   └── Utilities/              # Extension methods and utilities
├── FrameworkUI/                  # Main WPF UI library
│   ├── Main Window/            # MainWindow with undo/redo support
│   ├── Project Explorer/       # Tree view nodes and view models
│   ├── Message Window/         # Message display control
│   ├── Recent Files/           # Recent files menu functionality
│   ├── Themes/                 # ThemeManager bridging to Themes library
│   ├── Tools Menu/             # Auto-backup and file management
│   └── User Settings/          # User preferences management
├── Themes/                     # Independent theming library
│   ├── Core/                   # ThemeService, Theme enum, interfaces
│   └── Resources/              # Color palettes and control templates
├── Demo_ProjectUI/             # Example application with demos
│   ├── UI/                     # ThemeDemoControl, UndoDemoControl
│   └── Project/                # Sample element implementations
└── docs/                       # Documentation
```

## Requirements

- .NET Framework 4.8.1
- Visual Studio 2019 or later
- Windows 10 or later

## Dependencies

- **Xceed.Wpf.AvalonDock** (v3.5.3) - Docking window management
- **GenericControls** - Custom WPF controls library

## Quick Start

### Building the Solution

1. Open `ProjectControls.sln` in Visual Studio
2. Restore NuGet packages
3. Build the solution (F6 or Build > Build Solution)
4. Run `Demo_ProjectUI` to see the framework in action

### Basic Usage

```csharp
// Initialize theming (do this before creating UI)
ThemeManager.SetTheme(ThemeColor.Light);

// Create your project and controller
var project = new MyProject();
var controller = new MyProjectController(project);

// Create and show the main window
var mainWindow = new MainWindow();
mainWindow.ProjectNode = controller;
mainWindow.Show();
```

## Documentation

Comprehensive documentation is available in the [docs/](docs/) folder:

| Document | Description |
|----------|-------------|
| [Getting Started](docs/getting-started.md) | Step-by-step guide to building your first application |
| [Architecture](docs/architecture.md) | System architecture and project dependencies |
| [Themes](docs/themes.md) | Using the Themes library for runtime theme switching |
| [Undo/Redo](docs/undo-redo.md) | Implementing undo/redo in your elements |
| [Migration Guide](docs/migration-guide.md) | Upgrading from previous versions |

## Key Features

### Theme Support

Runtime theme switching with three built-in themes:

```csharp
// Switch themes at runtime - all controls update automatically
ThemeManager.SetTheme(ThemeColor.Dark);

// Subscribe to theme changes
ThemeManager.ThemeChanged += (dict, color) => {
    // Handle theme change
};
```

### Undo/Redo System

Built-in undo/redo for element properties:

```csharp
public class MyElement : ElementBase, IUndoableElement
{
    private string _customValue;

    public string CustomValue
    {
        get => _customValue;
        set => SetPropertyWithUndo(ref _customValue, value, nameof(CustomValue));
    }

    // Ctrl+Z and Ctrl+Y work automatically in MainWindow
}
```

### Messaging System

Centralized application-wide messaging:

```csharp
var messenger = Messenger.GetInstance();

messenger.Add(new BasicMessageItem(
    MessageType.Error,
    "Validation failed",
    sourceElement,
    "Elements",
    "MyElement",
    "PropertyName",
    "ERR-001"));
```

## API Reference

### Key Interfaces

| Interface | Description |
|-----------|-------------|
| `IProject` | Represents a project with element collections |
| `IElement` | Represents an element within a project |
| `IElementCollection` | Collection of elements |
| `IMessageItem` | Message item for the messaging system |
| `IUndoManager` | Manages undo/redo operations |
| `IUndoableElement` | Element that supports undo/redo |
| `IThemeService` | Theme management service |

### Key Classes

| Class | Description |
|-------|-------------|
| `ElementBase` | Abstract base class for elements with undo support |
| `Messenger` | Singleton messaging system |
| `UndoManager` | Undo/redo stack management |
| `ThemeService` | Singleton theme management |
| `ThemeManager` | FrameworkUI theme bridge |
| `Node` | Base class for tree view nodes |

## Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

### Code Style

- Use XML documentation comments for all public APIs
- Follow C# naming conventions
- Keep methods focused and single-purpose
- Handle null cases explicitly

## License

This project is developed by the USACE Risk Management Center and is licensed under the MIT License.

## Authors

- **Haden Smith** - USACE Risk Management Center
- **Woodrow Fields** - USACE Risk Management Center

## Acknowledgments

- Xceed for the AvalonDock library
- Microsoft for the WPF framework
