# SoftwareUpdate.Updater.Tests

Unit tests for the SoftwareUpdate.Updater library.

## Framework

xUnit 2.9.2 with coverlet for code coverage. Targets `net9.0-windows`.

## Key Test Areas

- `InstallationManagerTests.cs` - Tests the installer/update installation manager logic
- `UpdaterArgumentsTests.cs` - Tests command-line argument parsing for the updater process

## How to Run

```
dotnet test tests/SoftwareUpdate.Updater.Tests/SoftwareUpdate.Updater.Tests.csproj
```
