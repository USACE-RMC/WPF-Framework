# WPF-Framework

A WPF (Windows Presentation Foundation) framework for building project management applications with hierarchical tree structures, messaging systems, undo/redo support, software updates, and extensible UI components.

## Overview

WPF-Framework provides a foundation for creating Windows desktop applications that manage projects containing hierarchical elements. It includes:

- **FrameworkUI**: Main application shell with docking windows, project explorer, and messaging
- **FrameworkInterfaces**: Core interfaces for projects, elements, and undo/redo support
- **Themes**: Independent theming system with VS2013-style themes (Light, Dark, Blue)
- **GenericControls**: Reusable WPF controls (ColorPicker, NumericTextBox, etc.)
- **NumericControls**: Specialized controls for numeric data visualization
- **SoftwareUpdate**: GitHub-based automatic update system

## Project Structure

```
WPF-Framework/
├── src/
│   ├── FrameworkInterfaces/      # Core interfaces and base classes
│   │   ├── Messaging/            # Messenger, IMessageItem, BasicMessageItem
│   │   ├── Project/              # IProject, IElement, ElementBase
│   │   ├── Undo/                 # IUndoManager, UndoManager, action classes
│   │   └── Utilities/            # Extension methods and utilities
│   ├── FrameworkUI/              # Main WPF UI library
│   │   ├── Main Window/          # MainWindow, FrameworkUIController
│   │   ├── Project Explorer/     # Tree view nodes and view models
│   │   ├── Message Window/       # Message display control
│   │   ├── Recent Files/         # Recent files menu functionality
│   │   ├── Themes/               # ThemeManager bridging to Themes library
│   │   ├── Tools Menu/           # Auto-backup and file management
│   │   └── User Settings/        # User preferences management
│   ├── Themes/                   # Independent theming library
│   │   ├── Core/                 # ThemeService, Theme enum, interfaces
│   │   └── Resources/            # Color palettes and control templates
│   ├── GenericControls/          # Reusable WPF controls
│   ├── NumericControls/          # Numeric data visualization controls
│   ├── SoftwareUpdate/           # GitHub-based update system
│   ├── Demo_FrameworkUI/         # FrameworkUI demo application
│   ├── Demo_GenericControls/     # GenericControls demo application
│   └── Demo_NumericControls/     # NumericControls demo application
└── docs/                         # Documentation
```

## Requirements

- .NET 9.0 (Windows)
- Visual Studio 2022 or later
- Windows 10 or later

## Dependencies

- **Xceed.Wpf.AvalonDock** - Docking window management
- **System.Drawing.Common** (v9.0.0) - Drawing support for .NET

## Quick Start

### Building the Solution

1. Open `WPF-Framework.sln` in Visual Studio
2. Restore NuGet packages
3. Build the solution (F6 or Build > Build Solution)
4. Run `Demo_FrameworkUI` to see the framework in action

### Basic Usage

```csharp
// Initialize theming (do this before creating UI)
FrameworkUI.ThemeManager.SetTheme(FrameworkUI.ThemeColor.Light);

// Create your project and controller
var project = new MyProject();
var controller = new MyProjectController(project);

// Create and show the main window
var mainWindow = new FrameworkUI.MainWindow();
mainWindow.ProjectNode = controller;
mainWindow.Show();
```

### Creating a Project Controller

```csharp
public class MyProjectController : FrameworkUIController
{
    public MyProjectController(IProject project) : base(project) { }

    protected override void DefineProjectMenuItems() { }
    protected override void DefineToolsMenuItems() { }
    protected override void DefineHelpMenuItems() { }

    public override Control GetDocumentControl(IElement element)
    {
        return new MyDocumentControl { DataContext = element };
    }

    public override Control GetPropertiesControl(IElement element)
    {
        return new MyPropertiesControl { DataContext = element };
    }

    // ... implement other abstract methods
}
```

## Documentation

Comprehensive documentation is available in the [docs/](docs/) folder:

| Document | Description |
|----------|-------------|
| [Getting Started](docs/getting-started.md) | Step-by-step guide to building your first application |
| [Architecture](docs/architecture.md) | System architecture and project dependencies |
| [Themes](docs/themes.md) | Using the Themes library for runtime theme switching |
| [Undo/Redo](docs/undo-redo.md) | Implementing undo/redo in your elements |
| [Generic Controls](docs/generic-controls.md) | Using the GenericControls library |
| [Numeric Controls](docs/numeric-controls.md) | Using the NumericControls library |
| [Software Update](docs/software-update.md) | Implementing automatic updates |
| [Migration Guide](docs/migration-guide.md) | Upgrading from previous versions |

## Key Features

### Theme Support

Runtime theme switching with three built-in themes:

```csharp
// Switch themes at runtime - all controls update automatically
FrameworkUI.ThemeManager.SetTheme(FrameworkUI.ThemeColor.Dark);

// Or use ThemeService directly
Themes.ThemeService.Instance.SetTheme(Themes.Theme.Dark);

// Subscribe to theme changes
Themes.ThemeService.Instance.ThemeChanged += (sender, args) => {
    Console.WriteLine($"Theme changed to: {args.NewTheme}");
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

### Software Update System

Automatic updates from GitHub releases:

```csharp
var updateOptions = new UpdateOptions
{
    GitHubOwner = "USACE-RMC",
    GitHubRepo = "MyApp",
    CurrentVersion = new SemanticVersion(1, 0, 0),
    AssetNamePattern = "MyApp.*.zip"
};

var updateService = new GitHubUpdateService(updateOptions);
mainWindow.UpdateService = updateService;
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
| `IUpdateService` | Software update service |

### Key Classes

| Class | Description |
|-------|-------------|
| `ElementBase` | Abstract base class for elements with undo support |
| `FrameworkUIController` | Abstract controller for project UI |
| `Messenger` | Singleton messaging system |
| `UndoManager` | Undo/redo stack management |
| `ThemeService` | Singleton theme management |
| `ThemeManager` | FrameworkUI theme bridge |
| `GitHubUpdateService` | GitHub-based update service |
| `Node` | Base class for tree view nodes |

## License

This project is developed by the U.S. Army Corps of Engineers, Risk Management Center (USACE-RMC).

See the license header in source files for terms and conditions.

## Authors

- **Haden Smith** - USACE Risk Management Center
- **Woodrow Fields** - USACE Risk Management Center

## Acknowledgments

- Xceed for the AvalonDock library
- Microsoft for the WPF framework
