## Purpose
Software update checking and downloading via GitHub Releases API, with SemVer 2.0 version comparison and external updater process launching.

## Key Files
- `Core/IUpdateService.cs` — Interface: CheckForUpdateAsync, DownloadUpdateAsync, InstallUpdateAndRestart, SkipVersion, events.
- `GitHub/GitHubUpdateService.cs` — Full IUpdateService implementation. GitHub API v3, retry with exponential backoff, SHA256 checksum validation, asset matching via glob-to-regex.
- `Core/UpdateOptions.cs` — Configuration: GitHubOwner/Repo, CurrentVersion, AssetNamePattern (default *.zip), IncludePreReleases, GitHubToken, timeouts. Resolved properties compute defaults from assembly info.
- `Core/UpdateInfo.cs` — Data class: Version, DownloadUrl, ReleaseNotes, Sha256Checksum, AssetName, IsPreRelease, etc.
- `Utilities/SemanticVersion.cs` — SemVer 2.0 implementation. Supports v-prefix, pre-release precedence, Parse/TryParse, comparison operators.
- `Utilities/UpdaterBootstrapper.cs` — Static LaunchUpdater(). Builds CLI args (--pid, --zip, --target, --exe, --backup) and spawns updater process.

## Dependencies
- .NET 9 (net9.0-windows)
- System.Net.Http (HttpClient for GitHub API)
- System.Text.Json (JSON deserialization of GitHub releases)
- No internal project dependencies

## Patterns
- **Retry with exponential backoff**: 3 attempts at 500ms/1000ms delays for transient HTTP failures.
- **SHA256 checksum validation**: Downloads validated against checksum from release notes (pattern: `SHA256: <hex>`).
- **Asset glob matching**: AssetNamePattern converted to regex for matching release assets.
- **State machine**: UpdateState enum tracks Idle/Checking/UpdateAvailable/Downloading/ReadyToInstall/Error.
- **Skipped versions**: Persisted to file at SkippedVersionsFilePath. Checked before reporting available updates.
- **Version comparison**: SemanticVersion implements IComparable with pre-release having lower precedence than release.

## Gotchas
- InstallUpdateAndRestart() calls Environment.Exit(0) after launching updater — no cleanup code runs after this call.
- GitHubToken is optional but needed for private repos or to avoid rate limits (60 req/hr unauthenticated).
- SHA256 checksum is optional — if not found in release notes, validation is skipped silently.
- UpdateOptions resolved properties (InstallDirectory, MainExecutableName) use Assembly.GetEntryAssembly() — returns null in unit tests.
- Rate limit detection returns specific error message but does NOT auto-retry — caller must handle.
- Downloads go to temp directory; cleanup happens in the updater process, not here.
