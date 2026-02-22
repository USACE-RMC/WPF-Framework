# DatabaseManager

Multi-format database abstraction library providing a unified interface for reading, editing, and exporting tabular data.

## Purpose

Provides `DatabaseManager` (abstract base) and `DataTableView` (abstract table view) with concrete implementations for SQLite, CSV, DBF, and in-memory DataTables. Supports undo/redo via edit tracking.

## Key Files

- `DatabaseManager.cs` - Abstract base class. Defines `Open()`, `Close()`, `GetTableManager()`, `GetTableNames()`. Static helper `ConvertCsvToSqLite()`.
- `DataTableView.cs` - Abstract table view with edit tracking (undo/redo), column/row operations, cell access, and export to Excel/CSV.
- `SQLiteManager.cs` - SQLite implementation using `System.Data.SQLite`. Supports password-protected databases and custom connection builders.
- `CSVReader.cs` - CSV reader using `TextFieldParser`. Wraps data into `InMemoryReader` internally.
- `DBFReader.cs` - dBASE (.dbf) file reader with binary parsing.
- `InMemoryReader.cs` - Wraps a `DataTable` as a `DatabaseManager` for in-memory operations.
- `MDBReader.cs` - Access (.mdb) reader. **Excluded from build** (`<Compile Remove="MDBReader.cs" />`).

## Table Edits (Undo/Redo)

- `Table Edits/TableEdit.cs` - Abstract base for all edit types.
- `Table Edits/CellEdit.cs` - Single cell change.
- `Table Edits/MultiCellEdit.cs` - Multiple cell changes.
- `Table Edits/RowEdit.cs`, `AddRowEdit.cs`, `AddRowsEdit.cs`, `DeleteRowEdit.cs`, `DeleteRowsEdit.cs` - Row operations.
- `Table Edits/ColumnEdit.cs`, `AddColumnEdit.cs`, `AddColumnsEdit.cs`, `DeleteColumnEdit.cs`, `DeleteColumnsEdit.cs` - Column operations.
- `Table Edits/IColumnEdit.cs` - Interface for column edit metadata.

## DataTableView Key API

- `GetValue(col, row)` / `SetValue(col, row, value)` - Cell access with edit overlay.
- `Undo()` / `Redo()` - Edit history navigation.
- `ApplyEdits()` - Commits pending edits to underlying storage.
- `AddRow()`, `DeleteRow()`, `AddColumn()`, `DeleteColumn()` - Structural operations.
- `ExportToExcel()`, `ExportToCsv()` - Export capabilities.

## Events

- `EditsSaved` / `PreviewEditsSaved` - Fired on `DatabaseManager` when edits are committed.

## Dependencies

- `net9.0` (no Windows dependency)
- `System.Data.SQLite` 2.0.2, `SourceGear.sqlite3` 3.50.4.5
- `ClosedXML` 0.105.0 (Excel export)
- `FastMember` 1.5.0

## Gotchas

- `MDBReader.cs` is excluded from the build due to missing dependencies and Windows-only requirement.
- `CsvReader` loads the entire CSV into memory via `InMemoryReader`.
- `DataTableView` maintains an edit stack with an index pointer for undo/redo -- edits beyond the current pointer are discarded on new edits.
