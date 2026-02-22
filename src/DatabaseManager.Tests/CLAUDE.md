# DatabaseManager.Tests

Unit tests for the DatabaseManager library.

## Framework

xUnit 2.9.2 with coverlet for code coverage.

## Key Test Areas

- `DataTableViewTests.cs` - Tests DataTableView cell access, column/row operations, undo/redo, and edit tracking
- `DatabaseManagerTests.cs` - Tests abstract DatabaseManager methods and static helpers (e.g., CSV-to-SQLite conversion)
- `SQLiteManagerTests.cs` - Tests SQLite-specific operations: open/close, table creation, reading, writing
- `InMemoryReaderTests.cs` - Tests InMemoryReader wrapping DataTables
- `TableEditTests.cs` - Tests edit types: CellEdit, RowEdit, ColumnEdit, and their interactions
- `EdgeCaseTests.cs` - Tests boundary conditions and error handling
- `IntegrationTests.cs` - End-to-end tests across multiple components

## How to Run

```
dotnet test src/DatabaseManager.Tests/DatabaseManager.Tests.csproj
```
