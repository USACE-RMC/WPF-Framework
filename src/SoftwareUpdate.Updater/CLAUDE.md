## Purpose
Standalone console application that applies software updates by replacing files while the main application is closed. Launched by SoftwareUpdate's UpdaterBootstrapper.

## Key Files
- `Program.cs` — Entry point. Parses CLI args, validates, sets up file logging (`{target}/logs/update_{timestamp}.log`), runs InstallationManager. Returns 0 on success, 1 on failure. Auto-closes after 3 seconds.
- `InstallationManager.cs` — Core update logic: (1) wait for main app process exit (60s timeout), (2) create backup, (3) extract zip to target, (4) clean up old backups, (5) restart app. Restores from backup on failure.
- `UpdaterArguments.cs` — CLI parser for --pid, --zip, --target, --exe, --backup (flag). Validate() checks required args. UnquoteArgument() handles quoted paths.

## Dependencies
- .NET 9 (net9.0-windows), self-contained console app
- System.IO.Compression (ZipFile extraction)
- No internal project dependencies (standalone executable)

## Patterns
- **Process wait with timeout**: Waits up to 60 seconds for main app (by PID) to exit before proceeding.
- **Backup before replace**: Creates `.backup_{timestamp}` directory alongside target. Keeps only 2 most recent backups.
- **Atomic-ish update**: Backup -> Extract -> Clean up. On any failure, restores entire backup directory.
- **Smart zip extraction**: Detects single root folder in zip and extracts its contents directly (skips wrapper folder).
- **Retry for locked files**: Retries file operations that fail due to locked files during extraction.
- **Path traversal protection**: Validates all zip entry paths stay within target directory.
- **Structured logging**: All operations logged to timestamped file for post-mortem debugging.

## Gotchas
- This is a SEPARATE PROCESS — it runs after Environment.Exit(0) in the main app. No shared state.
- Process wait uses PID from --pid arg. If PID is wrong or already exited, proceeds immediately.
- Backup is only created when --backup flag is passed. Without it, failed updates cannot be restored.
- The updater keeps only 2 most recent backups — older ones are deleted during cleanup.
- Path traversal check: zip entries with `..` in path are skipped with a warning, not treated as fatal errors.
- Console auto-closes after 3 seconds on success — user may not see output unless logging is checked.
