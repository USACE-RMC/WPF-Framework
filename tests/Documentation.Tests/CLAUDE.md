# Documentation.Tests

Compilable code snippets that verify documentation examples stay in sync with the framework API. These are not runtime tests -- they validate that doc snippets compile against the current API.

## Key Files
- `Snippets/GettingStartedSnippets.cs` - IProject, IElement, and MainWindow setup examples
- `Snippets/DagSnippets.cs` - DAG graph and node usage examples
- `Snippets/DatabaseSnippets.cs` - DatabaseManager and DataTableView examples
- `Snippets/GenericControlSnippets.cs` - GenericControls usage examples
- `Snippets/NumericControlSnippets.cs` - NumericControls usage examples
- `Snippets/OxyPlotSnippets.cs` - OxyPlot chart configuration examples
- `Snippets/ThemeSnippets.cs` - Theme switching examples
- `Snippets/UndoRedoSnippets.cs` - Undo/redo system examples
- `Snippets/SoftwareUpdateSnippets.cs` - Software update examples

## Dependencies
References most framework projects (FrameworkInterfaces, FrameworkUI, Themes, GenericControls, NumericControls, OxyPlotControls, DatabaseManager, DatabaseControls, ExpressionParser, ExpressionParserControls, DAG, DAGControls, SoftwareUpdate).

## Notes
- Uses `#pragma warning disable` extensively since snippets intentionally have unused variables and nullable gaps
- Not a test runner project -- no test framework package references; compilation success is the test
