# WPF Framework

[![CI](https://github.com/USACE-RMC/WPF-Framework/actions/workflows/Integration.yml/badge.svg)](https://github.com/USACE-RMC/WPF-Framework/actions/workflows/Integration.yml)
[![License: 0BSD](https://img.shields.io/badge/License-0BSD-blue.svg)](LICENSE)

WPF Framework is a free and open-source .NET 10.0 application framework for building desktop project management applications, developed by the U.S. Army Corps of Engineers Risk Management Center ([USACE-RMC](https://www.rmc.usace.army.mil/)). It provides a complete application shell with docking layout, project explorer, theme switching, undo/redo, and specialized controls for charting, databases, expression parsing, and directed acyclic graphs.

## Supported Frameworks

| Platform | Version |
|----------|---------|
| .NET | 10.0 |
| OS | Windows 10+ |

WPF Framework is currently distributed as source only. NuGet package publishing is planned for a future release; until then, clone the repository and reference the individual library projects directly, or build them as local NuGet packages.

The framework depends on the [Numerics](https://github.com/USACE-RMC/Numerics) library (built separately and referenced via `HintPath`). See [Prerequisites](#prerequisites) for the expected layout.

## Solution Structure

| Folder | Projects | Description |
|--------|----------|-------------|
| **Core** | FrameworkInterfaces, FrameworkUI, Themes | Core contracts, application shell, and theming engine |
| **Controls** | GenericControls, NumericControls, OxyPlotControls, DatabaseControls, ExpressionParserControls, DAGControls | Reusable WPF control libraries |
| **Models** | DatabaseManager, ExpressionParser, OxyPlot, OxyPlot.Wpf, OxyPlot.Wpf.Shared, DAG | Platform-agnostic model and engine libraries |
| **Support** | SoftwareUpdate, SoftwareUpdate.Updater | GitHub Releases-based auto-update system |
| **AvalonDock** | Xceed.Wpf.AvalonDock, Xceed.Wpf.AvalonDock.Themes.VS2013 | Modified VS2013-themed docking layout (vendored fork) |
| **Demos** | FrameworkUI.Demo, GenericControls.Demo, NumericControls.Demo, OxyPlotControls.Demo, DatabaseControls.Demo, ExpressionParserControls.Demo, DAG.Demo | Interactive demo applications |
| **Tests** | OxyPlot.ExampleLibrary + 13 test projects | Example chart models and xunit, MSTest, and NUnit test suites |

## Quick Start

### Prerequisites

- [.NET 10.0 SDK](https://dotnet.microsoft.com/download)
- Visual Studio 2022 (17.12 or later)
- Windows 10 or later

### Build and Test

```bash
dotnet build WPF-Framework.sln
dotnet test WPF-Framework.sln
```

### Minimal Application

```csharp
// In App.xaml.cs — initialize theme before creating any UI
FrameworkUI.ThemeManager.SetTheme(FrameworkUI.ThemeColor.Light);

// Create project and controller
var project = new MyProject();
var controller = new MyProjectController(project);

// Show the main window
var mainWindow = new FrameworkUI.MainWindow();
mainWindow.ProjectNode = controller;
mainWindow.Show();
```

Run `FrameworkUI.Demo` for a complete working example.

## Documentation

Comprehensive documentation is available in the [docs/](docs/index.md) folder:

| Document | Description |
|----------|-------------|
| [Getting Started](docs/getting-started.md) | Step-by-step guide to building your first application |
| [Architecture](docs/architecture.md) | Solution structure, dependencies, and design patterns |
| [Themes](docs/themes.md) | Runtime theme switching with Light, Dark, and Blue themes |
| [Undo/Redo](docs/undo-redo.md) | Property and collection change tracking with undo support |
| [Generic Controls](docs/generic-controls.md) | NumericTextBox, ColorPicker, CopyPasteDataGrid, and more |
| [Numeric Controls](docs/numeric-controls.md) | Distribution selectors, curve editors, and time series tables |
| [OxyPlot Controls](docs/oxyplot-controls.md) | Interactive charting with toolbar, property editors, and serialization |
| [Database Controls](docs/database-controls.md) | Table viewing, field calculation, and expression parsing |
| [DAG Controls](docs/dag-controls.md) | Directed acyclic graph editing with visual flow canvas |
| [Software Update](docs/software-update.md) | Automatic updates from GitHub Releases |
| [Migration Guide](docs/migration-guide.md) | Upgrading from previous versions |

## Key Features

### Theme Switching

Three built-in themes with runtime switching — all controls update automatically via `DynamicResource` bindings:

```csharp
FrameworkUI.ThemeManager.SetTheme(FrameworkUI.ThemeColor.Dark);
```

### Undo/Redo

Built-in undo/redo for element properties with automatic change tracking:

```csharp
public string CustomValue
{
    get => _customValue;
    set
    {
        if (_customValue != value)
        {
            var oldValue = _customValue;
            _customValue = value;
            RecordPropertyChange(nameof(CustomValue), oldValue, value);
        }
    }
}
```

### Docking Layout

VS2013-style docking with tabbed documents, auto-hide panels, and floating windows powered by a vendored AvalonDock fork.

### Charting

Vendored OxyPlot fork with a full toolbar (pan, zoom, annotation, export), property editors, theme integration, and XML serialization.

### Database Management

Multi-format database abstraction supporting SQLite, CSV, DBF, and in-memory tables with undo/redo edit tracking and Excel/CSV export.

### Software Updates

Automatic update checking and installation from GitHub Releases with SemVer 2.0, SHA256 checksum validation, and backup/recovery.

## Support

USACE-RMC is committed to maintaining and supporting WPF Framework with regular updates, bug fixes, and enhancements. The framework is under active development and serves as the shared foundation for our suite of desktop engineering applications.

The repository includes extensive unit tests across 13 test projects that also serve as usage examples for the classes and methods in the libraries.

## Applications

WPF Framework powers the following USACE-RMC desktop applications:

- [RMC-BestFit](https://github.com/USACE-RMC/RMC-BestFit) — Bayesian estimation and fitting for flood frequency analysis
- [RMC-RFA](https://github.com/USACE-RMC/RMC-RFA) — Reservoir frequency analysis for flood hazard assessments
- [RMC-TotalRisk](https://github.com/USACE-RMC/RMC-TotalRisk) — Quantitative risk analysis for dam and levee safety
- [LifeSim](https://github.com/USACE-RMC/LifeSim) — Life loss consequence estimation and evacuation simulation

## Related Libraries

- [Numerics](https://github.com/USACE-RMC/Numerics) — .NET library for numerical computing, statistical analysis, and Bayesian inference (required dependency for NumericControls and DatabaseControls)

## Contributing

Contributions are welcome. See [CONTRIBUTING.md](CONTRIBUTING.md) for guidelines.

## License

This project is licensed under the Zero-Clause BSD (0BSD) license — a permissive license with no attribution or notice requirements. See [LICENSE](LICENSE) for details.

## Authors

- **Haden Smith** — USACE Risk Management Center
- **Woodrow Fields** — USACE Risk Management Center
- **Julian Gonzalez** — USACE Risk Management Center

## Acknowledgments

- [Xceed](https://github.com/xceedsoftware/wpftoolkit) for the AvalonDock docking library (vendored and modified)
- [OxyPlot](https://github.com/oxyplot/oxyplot) contributors for the charting library (vendored fork with custom serialization)
