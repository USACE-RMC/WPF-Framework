# Software Update System

The SoftwareUpdate library provides automatic update checking and installation for applications distributed via GitHub Releases.

## Overview

The system consists of two components:

1. **SoftwareUpdate** - A library that checks for updates on GitHub and downloads release assets
2. **SoftwareUpdate.Updater** - A standalone console application that performs the actual file replacement and restart

This architecture is necessary because a running application cannot replace its own executable files on Windows.

## Architecture

```
┌─────────────────────────────────────────────────────────────────┐
│                        Main Application                          │
│                                                                  │
│  ┌──────────────┐    ┌─────────────────┐    ┌────────────────┐  │
│  │ UpdateOptions │───▶│ GitHubUpdateSvc │───▶│ IUpdateService │  │
│  └──────────────┘    └─────────────────┘    └────────────────┘  │
│                              │                       │           │
│                              ▼                       ▼           │
│                      GitHub API Check         MainWindow.        │
│                              │              UpdateService        │
│                              ▼                       │           │
│                      Download .zip                   ▼           │
│                              │               Check for Updates   │
│                              ▼                  Menu Item         │
│                      Launch Updater                              │
└─────────────────────────────────────────────────────────────────┘
                               │
                               ▼
┌─────────────────────────────────────────────────────────────────┐
│                    SoftwareUpdate.Updater                        │
│                                                                  │
│  1. Wait for main app to exit (--pid)                           │
│  2. Create backup (optional)                                     │
│  3. Extract zip to target directory                             │
│  4. Launch main executable                                       │
└─────────────────────────────────────────────────────────────────┘
```

## Quick Start

### 1. Add Project Reference

Add a reference to the SoftwareUpdate project in your application's `.csproj`:

```xml
<ProjectReference Include="..\SoftwareUpdate\SoftwareUpdate.csproj">
  <Project>{b2c3d4e5-f6a7-8901-bcde-f23456789012}</Project>
  <Name>SoftwareUpdate</Name>
</ProjectReference>
```

### 2. Configure Update Options

```csharp
using SoftwareUpdate;
using SoftwareUpdate.GitHub;
using SoftwareUpdate.Utilities;

var updateOptions = new UpdateOptions
{
    // GitHub repository settings
    GitHubOwner = "USACE-RMC",
    GitHubRepo = "RMC-BestFit",

    // Current application version
    CurrentVersion = new SemanticVersion(2, 0, 0, "beta.3"),

    // Pattern to match release assets (supports * wildcard)
    AssetNamePattern = "RMC-BestFit.Version.*.zip",

    // Include pre-release versions
    IncludePreReleases = true,

    // Create backup before updating
    CreateBackup = true,

    // Auto-check settings
    AutoCheckOnStartup = true,
    AutoCheckDelayMs = 5000,

    // Main executable name for restart
    MainExecutableName = "RMC-BestFit.exe"
};
```

### 3. Create Update Service and Assign to MainWindow

```csharp
var updateService = new GitHubUpdateService(updateOptions);

// This enables the "Check for Updates" menu item
mainWindow.UpdateService = updateService;
```

### 4. Optional: Auto-Check on Startup

```csharp
private async void AutoCheckForUpdatesAsync(MainWindow mainWindow, IUpdateService updateService, int delayMs)
{
    try
    {
        await Task.Delay(delayMs);
        var result = await updateService.CheckForUpdateAsync();

        if (result.IsUpdateAvailable && result.AvailableUpdate != null)
        {
            if (updateService.IsVersionSkipped(result.AvailableUpdate.Version))
                return;

            await mainWindow.Dispatcher.InvokeAsync(() =>
            {
                var msgResult = MessageBox.Show(
                    $"Version {result.AvailableUpdate.Version} is available. Update now?",
                    "Update Available",
                    MessageBoxButton.YesNo);

                if (msgResult == MessageBoxResult.Yes)
                {
                    // Guide user to menu
                    mainWindow.ShowMessage("Use Tools → Check for Updates to proceed.");
                }
            });
        }
    }
    catch (Exception ex)
    {
        Debug.WriteLine($"Auto-update check failed: {ex.Message}");
    }
}
```

## Semantic Versioning

The library uses full [Semantic Versioning 2.0](https://semver.org/) with pre-release support:

```csharp
// Parse from string
var version = SemanticVersion.Parse("2.0.0-beta.3");

// Create programmatically
var version = new SemanticVersion(2, 0, 0, "beta.3");

// Comparison
if (newVersion > currentVersion) { /* update available */ }
```

Pre-release identifiers are compared according to SemVer rules:
- `1.0.0-alpha` < `1.0.0-alpha.1` < `1.0.0-beta` < `1.0.0-rc.1` < `1.0.0`

## GitHub Release Setup

For the update system to work, your GitHub releases must:

1. Use semantic version tags (e.g., `v2.0.0`, `v2.0.0-beta.3`)
2. Include a zip file asset matching your `AssetNamePattern`
3. Not be marked as draft (drafts are ignored)

Example release URL structure:
```
https://github.com/USACE-RMC/RMC-BestFit/releases/download/v2.0-beta.3/RMC-BestFit.Version.2.0.Beta-3.zip
```

## UpdateOptions Reference

| Property | Type | Description |
|----------|------|-------------|
| `GitHubOwner` | string | GitHub organization or username |
| `GitHubRepo` | string | Repository name |
| `CurrentVersion` | SemanticVersion | Current installed version |
| `AssetNamePattern` | string | Glob pattern for matching release assets |
| `IncludePreReleases` | bool | Whether to include pre-release versions |
| `CreateBackup` | bool | Create backup before updating |
| `AutoCheckOnStartup` | bool | Auto-check for updates on app start |
| `AutoCheckDelayMs` | int | Delay before auto-check (milliseconds) |
| `MainExecutableName` | string | Executable to launch after update |
| `UpdaterExecutableName` | string | Name of updater executable (default: SoftwareUpdate.Updater.exe) |

## IUpdateService Interface

```csharp
public interface IUpdateService
{
    UpdateOptions Options { get; }
    UpdateState State { get; }
    UpdateInfo AvailableUpdate { get; }

    Task<UpdateCheckResult> CheckForUpdateAsync(CancellationToken cancellationToken = default);
    Task<UpdateDownloadResult> DownloadUpdateAsync(UpdateInfo update, IProgress<UpdateDownloadProgress> progress = null, CancellationToken cancellationToken = default);
    void InstallUpdateAndRestart(string downloadedFilePath);

    void SkipVersion(SemanticVersion version);
    bool IsVersionSkipped(SemanticVersion version);
    void ClearSkippedVersions();

    event EventHandler<UpdateCheckResult> UpdateCheckCompleted;
    event EventHandler<Exception> UpdateError;
}
```

## Update States

The `UpdateState` enum tracks the current state of the update process:

- `Idle` - No update activity
- `Checking` - Checking for updates
- `UpdateAvailable` - An update is available
- `Downloading` - Downloading update
- `ReadyToInstall` - Download complete, ready to install
- `Installing` - Installing update

## Backup and Recovery

When `CreateBackup = true`, the updater creates a backup before installation:

1. Backup location: `{AppDir}_backup_{timestamp}`
2. If installation fails, the backup remains for manual recovery
3. Successful updates do not automatically delete backups

## Deployment

Ensure `SoftwareUpdate.Updater.exe` is deployed alongside your main application:

```
YourApp/
├── YourApp.exe
├── SoftwareUpdate.dll
├── SoftwareUpdate.Updater.exe  ← Required for updates
└── ... other files
```

## Error Handling

The library handles common error scenarios:

- Network errors during check/download
- Invalid version formats
- Missing release assets
- Failed installations (backup preserved)

Subscribe to the `UpdateError` event for custom error handling:

```csharp
updateService.UpdateError += (s, ex) =>
{
    Logger.Error($"Update failed: {ex.Message}");
};
```

## Security Considerations

- All downloads use HTTPS
- The library does not execute downloaded content directly
- Zip extraction is performed by a separate process
- Consider adding checksum verification for production use
