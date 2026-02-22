# SoftwareUpdate.Tests

Unit tests for the SoftwareUpdate library.

## Framework

xUnit 2.9.2 with coverlet for code coverage. Targets `net9.0-windows`.

## Key Test Areas

- **Core**: `UpdateInfoTests.cs` - Tests update info model; `UpdateCheckResultTests.cs` - Tests check result model; `UpdateDownloadProgressTests.cs` - Tests download progress tracking; `UpdateDownloadResultTests.cs` - Tests download result model; `UpdateOptionsTests.cs` - Tests update configuration options
- **GitHub**: `GitHubUpdateServiceTests.cs` - Tests GitHub release-based update checking
- **Utilities**: `SemanticVersionTests.cs` - Tests semantic version parsing and comparison

## How to Run

```
dotnet test tests/SoftwareUpdate.Tests/SoftwareUpdate.Tests.csproj
```
