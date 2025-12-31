// Copyright (c) USACE. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading;

namespace SoftwareUpdate.Updater
{
    /// <summary>
    /// Manages the installation of software updates.
    /// </summary>
    internal class InstallationManager
    {
        private readonly UpdaterArguments _args;
        private readonly Action<string> _log;

        public InstallationManager(UpdaterArguments args, Action<string> log)
        {
            _args = args ?? throw new ArgumentNullException(nameof(args));
            _log = log ?? Console.WriteLine;
        }

        /// <summary>
        /// Executes the full update process.
        /// </summary>
        public void Execute()
        {
            _log("Starting update process...");
            _log($"Target directory: {_args.TargetDirectory}");
            _log($"Update package: {_args.ZipPath}");

            // Step 1: Wait for main application to exit
            WaitForProcessExit(_args.ProcessId);

            // Step 2: Create backup if requested
            string backupDir = null;
            if (_args.CreateBackup)
            {
                backupDir = CreateBackup();
            }

            try
            {
                // Step 3: Extract update
                ExtractUpdate();

                // Step 4: Clean up
                CleanUp(backupDir, success: true);

                // Step 5: Restart application
                RestartApplication();
            }
            catch (Exception ex)
            {
                _log($"ERROR: {ex.Message}");

                // Attempt to restore from backup
                if (!string.IsNullOrEmpty(backupDir) && Directory.Exists(backupDir))
                {
                    _log("Attempting to restore from backup...");
                    try
                    {
                        RestoreFromBackup(backupDir);
                        _log("Backup restored successfully.");
                    }
                    catch (Exception restoreEx)
                    {
                        _log($"Failed to restore backup: {restoreEx.Message}");
                        _log($"Backup location: {backupDir}");
                    }
                }

                throw;
            }
        }

        /// <summary>
        /// Waits for the specified process to exit.
        /// </summary>
        private void WaitForProcessExit(int processId)
        {
            _log($"Waiting for process {processId} to exit...");

            try
            {
                var process = Process.GetProcessById(processId);
                var timeout = TimeSpan.FromSeconds(60);
                var stopwatch = Stopwatch.StartNew();

                while (!process.HasExited && stopwatch.Elapsed < timeout)
                {
                    Thread.Sleep(500);
                }

                if (!process.HasExited)
                {
                    _log("Process did not exit within timeout. Attempting to continue...");
                }
                else
                {
                    _log("Process has exited.");
                }
            }
            catch (ArgumentException)
            {
                // Process already exited
                _log("Process has already exited.");
            }

            // Additional wait to ensure file handles are released
            Thread.Sleep(1000);
        }

        /// <summary>
        /// Creates a backup of the current installation.
        /// </summary>
        private string CreateBackup()
        {
            _log("Creating backup...");

            var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            var backupDir = Path.Combine(_args.TargetDirectory, $".backup_{timestamp}");

            Directory.CreateDirectory(backupDir);

            // Copy all files except the backup directory itself
            var sourceDir = new DirectoryInfo(_args.TargetDirectory);
            foreach (var file in sourceDir.GetFiles())
            {
                var destPath = Path.Combine(backupDir, file.Name);
                file.CopyTo(destPath, overwrite: true);
            }

            foreach (var dir in sourceDir.GetDirectories())
            {
                if (dir.Name.StartsWith(".backup_")) continue;

                var destPath = Path.Combine(backupDir, dir.Name);
                CopyDirectory(dir.FullName, destPath);
            }

            _log($"Backup created at: {backupDir}");
            return backupDir;
        }

        /// <summary>
        /// Extracts the update package.
        /// </summary>
        private void ExtractUpdate()
        {
            _log("Extracting update...");

            using (var archive = ZipFile.OpenRead(_args.ZipPath))
            {
                var totalEntries = archive.Entries.Count;
                var processedEntries = 0;

                // Determine if the zip has a single root folder
                var rootFolders = archive.Entries
                    .Where(e => !string.IsNullOrEmpty(e.FullName))
                    .Select(e => e.FullName.Split('/')[0])
                    .Distinct()
                    .ToList();

                var hasSingleRoot = rootFolders.Count == 1 &&
                    archive.Entries.Any(e => e.FullName.StartsWith(rootFolders[0] + "/"));

                var stripPrefix = hasSingleRoot ? rootFolders[0] + "/" : "";

                foreach (var entry in archive.Entries)
                {
                    processedEntries++;

                    // Skip directories
                    if (string.IsNullOrEmpty(entry.Name))
                        continue;

                    // Calculate destination path, stripping root folder if present
                    var entryPath = entry.FullName;
                    if (!string.IsNullOrEmpty(stripPrefix) && entryPath.StartsWith(stripPrefix))
                    {
                        entryPath = entryPath.Substring(stripPrefix.Length);
                    }

                    if (string.IsNullOrEmpty(entryPath))
                        continue;

                    var destPath = Path.Combine(_args.TargetDirectory, entryPath);

                    // Skip backup directories
                    if (destPath.Contains(".backup_"))
                        continue;

                    // Create directory if needed
                    var destDir = Path.GetDirectoryName(destPath);
                    if (!string.IsNullOrEmpty(destDir) && !Directory.Exists(destDir))
                    {
                        Directory.CreateDirectory(destDir);
                    }

                    // Extract file with retries (in case of locked files)
                    ExtractWithRetry(entry, destPath);

                    if (processedEntries % 10 == 0 || processedEntries == totalEntries)
                    {
                        _log($"Extracted {processedEntries}/{totalEntries} files...");
                    }
                }
            }

            _log("Update extracted successfully.");
        }

        /// <summary>
        /// Extracts a single entry with retry logic for locked files.
        /// </summary>
        private void ExtractWithRetry(ZipArchiveEntry entry, string destPath, int maxRetries = 3)
        {
            for (int attempt = 1; attempt <= maxRetries; attempt++)
            {
                try
                {
                    entry.ExtractToFile(destPath, overwrite: true);
                    return;
                }
                catch (IOException) when (attempt < maxRetries)
                {
                    _log($"File locked, retrying ({attempt}/{maxRetries}): {Path.GetFileName(destPath)}");
                    Thread.Sleep(1000 * attempt);
                }
            }

            // Final attempt - let exception propagate
            entry.ExtractToFile(destPath, overwrite: true);
        }

        /// <summary>
        /// Restores files from backup.
        /// </summary>
        private void RestoreFromBackup(string backupDir)
        {
            var sourceDir = new DirectoryInfo(backupDir);

            foreach (var file in sourceDir.GetFiles())
            {
                var destPath = Path.Combine(_args.TargetDirectory, file.Name);
                file.CopyTo(destPath, overwrite: true);
            }

            foreach (var dir in sourceDir.GetDirectories())
            {
                var destPath = Path.Combine(_args.TargetDirectory, dir.Name);
                CopyDirectory(dir.FullName, destPath);
            }
        }

        /// <summary>
        /// Cleans up temporary files.
        /// </summary>
        private void CleanUp(string backupDir, bool success)
        {
            _log("Cleaning up...");

            // Delete the downloaded zip
            try
            {
                if (File.Exists(_args.ZipPath))
                {
                    File.Delete(_args.ZipPath);
                    _log("Deleted update package.");
                }
            }
            catch (Exception ex)
            {
                _log($"Warning: Could not delete update package: {ex.Message}");
            }

            // If successful, optionally delete old backups (keep last 2)
            if (success)
            {
                try
                {
                    var backupDirs = Directory.GetDirectories(_args.TargetDirectory, ".backup_*")
                        .OrderByDescending(d => d)
                        .Skip(2) // Keep 2 most recent
                        .ToList();

                    foreach (var dir in backupDirs)
                    {
                        Directory.Delete(dir, recursive: true);
                        _log($"Deleted old backup: {Path.GetFileName(dir)}");
                    }
                }
                catch (Exception ex)
                {
                    _log($"Warning: Could not clean old backups: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Restarts the main application.
        /// </summary>
        private void RestartApplication()
        {
            _log("Restarting application...");

            var exePath = Path.Combine(_args.TargetDirectory, _args.MainExecutable);

            if (!File.Exists(exePath))
            {
                _log($"Warning: Main executable not found at {exePath}");
                _log("Update completed. Please restart the application manually.");
                return;
            }

            var startInfo = new ProcessStartInfo
            {
                FileName = exePath,
                WorkingDirectory = _args.TargetDirectory,
                UseShellExecute = true
            };

            Process.Start(startInfo);
            _log("Application restarted.");
        }

        /// <summary>
        /// Recursively copies a directory.
        /// </summary>
        private void CopyDirectory(string sourceDir, string destDir)
        {
            Directory.CreateDirectory(destDir);

            foreach (var file in Directory.GetFiles(sourceDir))
            {
                var destFile = Path.Combine(destDir, Path.GetFileName(file));
                File.Copy(file, destFile, overwrite: true);
            }

            foreach (var dir in Directory.GetDirectories(sourceDir))
            {
                var destSubDir = Path.Combine(destDir, Path.GetFileName(dir));
                CopyDirectory(dir, destSubDir);
            }
        }
    }
}
