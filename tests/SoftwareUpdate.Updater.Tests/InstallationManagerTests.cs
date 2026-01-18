/*
* NOTICE:
* The U.S. Army Corps of Engineers, Risk Management Center (USACE-RMC) makes no guarantees about
* the results, or appropriateness of outputs, obtained from this software.
*
* LIST OF CONDITIONS:
* Redistribution and use in source and binary forms, with or without modification, are permitted
* provided that the following conditions are met:
* - Redistributions of source code must retain the above notice, this list of conditions, and the
* following disclaimer.
* - Redistributions in binary form must reproduce the above notice, this list of conditions, and
* the following disclaimer in the documentation and/or other materials provided with the distribution.
* - The names of the U.S. Government, the U.S. Army Corps of Engineers, the Institute for Water
* Resources, or the Risk Management Center may not be used to endorse or promote products derived
* from this software without specific prior written permission. Nor may the names of its contributors
* be used to endorse or promote products derived from this software without specific prior
* written permission.
*
* DISCLAIMER:
* THIS SOFTWARE IS PROVIDED BY THE U.S. ARMY CORPS OF ENGINEERS RISK MANAGEMENT CENTER
* (USACE-RMC) "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO,
* THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
* DISCLAIMED. IN NO EVENT SHALL USACE-RMC BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL,
* SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
* PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS
* INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT
* LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF
* THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/

using Xunit;
using SoftwareUpdate.Updater;
using System.IO.Compression;
using System.Reflection;

namespace SoftwareUpdate.Updater.Tests
{
    /// <summary>
    /// Test suite for the InstallationManager class, validating update installation, backup, and restore functionality.
    /// Implements IDisposable to ensure proper cleanup of test directories.
    /// </summary>
    public class InstallationManagerTests : IDisposable
    {
        /// <summary>
        /// The temporary directory path used for all test file operations. Created uniquely for each test instance.
        /// </summary>
        private readonly string _testDir;

        /// <summary>
        /// Collection of log messages captured during test execution for verification purposes.
        /// </summary>
        private readonly List<string> _logs = new();

        /// <summary>
        /// Initializes a new instance of the InstallationManagerTests class.
        /// Creates a unique temporary directory for test isolation.
        /// </summary>
        public InstallationManagerTests()
        {
            _testDir = Path.Combine(Path.GetTempPath(), "InstallManagerTests_" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(_testDir);
        }

        /// <summary>
        /// Performs cleanup operations after test execution.
        /// Deletes the temporary test directory and all its contents.
        /// </summary>
        public void Dispose()
        {
            // Clean up test directory
            if (Directory.Exists(_testDir))
            {
                try
                {
                    Directory.Delete(_testDir, recursive: true);
                }
                catch
                {
                    // Ignore cleanup failures in tests
                }
            }
        }

        /// <summary>
        /// Logs a message to the internal log collection for test verification.
        /// </summary>
        /// <param name="message">The message to log.</param>
        private void Log(string message) => _logs.Add(message);

        #region Constructor Tests

        /// <summary>
        /// Verifies that the InstallationManager constructor throws ArgumentNullException when args parameter is null.
        /// </summary>
        /// <exception cref="ArgumentNullException">Expected exception when args is null.</exception>
        [Fact]
        public void Constructor_NullArgs_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new InstallationManager(null!, Log));
        }

        /// <summary>
        /// Verifies that the InstallationManager constructor accepts a null log action without throwing.
        /// </summary>
        [Fact]
        public void Constructor_NullLog_DoesNotThrow()
        {
            var args = new UpdaterArguments
            {
                ProcessId = 1,
                ZipPath = "test.zip",
                TargetDirectory = _testDir,
                MainExecutable = "test.exe"
            };

            var exception = Record.Exception(() => new InstallationManager(args, null!));

            Assert.Null(exception);
        }

        /// <summary>
        /// Verifies that the InstallationManager constructor successfully creates an instance with valid arguments.
        /// </summary>
        [Fact]
        public void Constructor_ValidArgs_CreatesInstance()
        {
            var args = new UpdaterArguments
            {
                ProcessId = 1,
                ZipPath = "test.zip",
                TargetDirectory = _testDir,
                MainExecutable = "test.exe"
            };

            var manager = new InstallationManager(args, Log);

            Assert.NotNull(manager);
        }

        #endregion

        #region ParseBackupTimestamp Tests (via Reflection)

        /// <summary>
        /// Verifies that ParseBackupTimestamp correctly parses backup directory names with valid timestamp format.
        /// </summary>
        /// <param name="dirName">The backup directory name to parse.</param>
        /// <param name="year">Expected year value.</param>
        /// <param name="month">Expected month value.</param>
        /// <param name="day">Expected day value.</param>
        /// <param name="hour">Expected hour value.</param>
        /// <param name="minute">Expected minute value.</param>
        /// <param name="second">Expected second value.</param>
        [Theory]
        [InlineData(".backup_20240115_120000", 2024, 1, 15, 12, 0, 0)]
        [InlineData(".backup_20231225_235959", 2023, 12, 25, 23, 59, 59)]
        [InlineData(".backup_20240101_000000", 2024, 1, 1, 0, 0, 0)]
        public void ParseBackupTimestamp_ValidFormat_ReturnsDateTime(
            string dirName, int year, int month, int day, int hour, int minute, int second)
        {
            var result = InvokeParseBackupTimestamp(dirName);

            Assert.NotNull(result);
            Assert.Equal(new DateTime(year, month, day, hour, minute, second), result.Value);
        }

        /// <summary>
        /// Verifies that ParseBackupTimestamp correctly handles backup directories with numeric suffixes.
        /// </summary>
        /// <param name="dirName">The backup directory name to parse.</param>
        /// <param name="year">Expected year value.</param>
        /// <param name="month">Expected month value.</param>
        /// <param name="day">Expected day value.</param>
        /// <param name="hour">Expected hour value.</param>
        /// <param name="minute">Expected minute value.</param>
        /// <param name="second">Expected second value.</param>
        [Theory]
        [InlineData(".backup_20240115_120000_1", 2024, 1, 15, 12, 0, 0)]
        [InlineData(".backup_20240115_120000_99", 2024, 1, 15, 12, 0, 0)]
        public void ParseBackupTimestamp_WithSuffix_ReturnsDateTime(
            string dirName, int year, int month, int day, int hour, int minute, int second)
        {
            var result = InvokeParseBackupTimestamp(dirName);

            Assert.NotNull(result);
            Assert.Equal(new DateTime(year, month, day, hour, minute, second), result.Value);
        }

        /// <summary>
        /// Verifies that ParseBackupTimestamp returns null for invalid or malformed backup directory names.
        /// </summary>
        /// <param name="dirName">The invalid backup directory name to parse.</param>
        [Theory]
        [InlineData("")]
        [InlineData("backup_20240115_120000")]
        [InlineData(".backup_")]
        [InlineData(".backup_invalid")]
        [InlineData(".backup_2024")]
        [InlineData(".backup_20241315_120000")] // Invalid month 13
        public void ParseBackupTimestamp_InvalidFormat_ReturnsNull(string dirName)
        {
            var result = InvokeParseBackupTimestamp(dirName);

            Assert.Null(result);
        }

        /// <summary>
        /// Verifies that ParseBackupTimestamp returns null when given a null path.
        /// </summary>
        [Fact]
        public void ParseBackupTimestamp_NullPath_ReturnsNull()
        {
            var result = InvokeParseBackupTimestamp(null!);

            Assert.Null(result);
        }

        /// <summary>
        /// Helper method to invoke the private static ParseBackupTimestamp method via reflection.
        /// </summary>
        /// <param name="backupPath">The backup directory path to parse.</param>
        /// <returns>The parsed DateTime if successful; otherwise, null.</returns>
        private static DateTime? InvokeParseBackupTimestamp(string backupPath)
        {
            var method = typeof(InstallationManager)
                .GetMethod("ParseBackupTimestamp", BindingFlags.NonPublic | BindingFlags.Static);
            return (DateTime?)method?.Invoke(null, new object[] { backupPath });
        }

        #endregion

        #region CreateBackup Tests (Integration)

        /// <summary>
        /// Verifies that the CreateBackup functionality creates a backup directory with all target files.
        /// </summary>
        [Fact]
        public void CreateBackup_CreatesBackupDirectory()
        {
            var targetDir = CreateSubDir("target");
            var zipPath = CreateTestZip("update.zip", new[] { ("file.txt", "content") });

            // Create some files in target directory
            File.WriteAllText(Path.Combine(targetDir, "existing.txt"), "original");
            Directory.CreateDirectory(Path.Combine(targetDir, "subdir"));
            File.WriteAllText(Path.Combine(targetDir, "subdir", "nested.txt"), "nested");

            var args = new UpdaterArguments
            {
                ProcessId = Environment.ProcessId, // Use current process (already running)
                ZipPath = zipPath,
                TargetDirectory = targetDir,
                MainExecutable = "test.exe",
                CreateBackup = true
            };

            var manager = new InstallationManager(args, Log);

            // Execute will fail because ProcessId points to current running process,
            // but backup should still be created
            try { manager.Execute(); } catch { }

            // Check backup was created
            var backupDirs = Directory.GetDirectories(targetDir, ".backup_*");
            Assert.NotEmpty(backupDirs);

            // Check backup contains original files
            var backupDir = backupDirs[0];
            Assert.True(File.Exists(Path.Combine(backupDir, "existing.txt")));
            Assert.True(File.Exists(Path.Combine(backupDir, "subdir", "nested.txt")));
        }

        /// <summary>
        /// Verifies that CreateBackup skips existing backup directories and does not include them in new backups.
        /// </summary>
        [Fact]
        public void CreateBackup_SkipsExistingBackupDirectories()
        {
            var targetDir = CreateSubDir("target");
            var zipPath = CreateTestZip("update.zip", new[] { ("file.txt", "content") });

            // Create an existing backup directory
            var existingBackup = Path.Combine(targetDir, ".backup_20240101_000000");
            Directory.CreateDirectory(existingBackup);
            File.WriteAllText(Path.Combine(existingBackup, "old.txt"), "old backup");

            // Create a new file to backup
            File.WriteAllText(Path.Combine(targetDir, "current.txt"), "current");

            var args = new UpdaterArguments
            {
                ProcessId = Environment.ProcessId,
                ZipPath = zipPath,
                TargetDirectory = targetDir,
                MainExecutable = "test.exe",
                CreateBackup = true
            };

            var manager = new InstallationManager(args, Log);
            try { manager.Execute(); } catch { }

            // Find the new backup (not the existing one)
            var backupDirs = Directory.GetDirectories(targetDir, ".backup_*")
                .Where(d => d != existingBackup)
                .ToList();

            Assert.Single(backupDirs);

            // The new backup should NOT contain the old backup directory
            var newBackup = backupDirs[0];
            Assert.False(Directory.Exists(Path.Combine(newBackup, ".backup_20240101_000000")));
            Assert.True(File.Exists(Path.Combine(newBackup, "current.txt")));
        }

        #endregion

        #region ExtractUpdate Path Traversal Tests

        /// <summary>
        /// Verifies that ExtractUpdate protects against path traversal attacks by skipping malicious entries.
        /// </summary>
        [Fact]
        public void ExtractUpdate_PathTraversalAttempt_SkipsEntry()
        {
            var targetDir = CreateSubDir("target");
            var parentFile = Path.Combine(_testDir, "escaped.txt");

            // Create a zip with path traversal attempt
            var zipPath = CreatePathTraversalZip("malicious.zip");

            var args = new UpdaterArguments
            {
                ProcessId = -1, // Invalid PID - process won't be found
                ZipPath = zipPath,
                TargetDirectory = targetDir,
                MainExecutable = "test.exe",
                CreateBackup = false
            };

            var manager = new InstallationManager(args, Log);
            try { manager.Execute(); } catch { }

            // The escaped file should NOT exist outside target directory
            Assert.False(File.Exists(parentFile));

            // Verify the log contains the security message
            Assert.Contains(_logs, l => l.Contains("potentially dangerous path"));
        }

        /// <summary>
        /// Verifies that ExtractUpdate correctly extracts files with normal, safe paths.
        /// </summary>
        [Fact]
        public void ExtractUpdate_NormalPaths_ExtractsCorrectly()
        {
            var targetDir = CreateSubDir("target");
            var zipPath = CreateTestZip("update.zip", new[]
            {
                ("file.txt", "root file"),
                ("subdir/nested.txt", "nested file"),
                ("subdir/deep/file.txt", "deep file")
            });

            var args = new UpdaterArguments
            {
                ProcessId = -1, // Invalid PID - process won't be found
                ZipPath = zipPath,
                TargetDirectory = targetDir,
                MainExecutable = "test.exe",
                CreateBackup = false
            };

            var manager = new InstallationManager(args, Log);
            try { manager.Execute(); } catch { }

            // Verify files were extracted
            Assert.True(File.Exists(Path.Combine(targetDir, "file.txt")));
            Assert.True(File.Exists(Path.Combine(targetDir, "subdir", "nested.txt")));
            Assert.True(File.Exists(Path.Combine(targetDir, "subdir", "deep", "file.txt")));
        }

        /// <summary>
        /// Verifies that ExtractUpdate strips a single root folder from archive entries (common in GitHub releases).
        /// </summary>
        [Fact]
        public void ExtractUpdate_SingleRootFolder_StripsRoot()
        {
            var targetDir = CreateSubDir("target");

            // Create a zip with single root folder (common for GitHub releases)
            var zipPath = CreateTestZip("release.zip", new[]
            {
                ("MyApp-v1.0.0/app.exe", "executable"),
                ("MyApp-v1.0.0/data/config.json", "config")
            });

            var args = new UpdaterArguments
            {
                ProcessId = -1,
                ZipPath = zipPath,
                TargetDirectory = targetDir,
                MainExecutable = "app.exe",
                CreateBackup = false
            };

            var manager = new InstallationManager(args, Log);
            try { manager.Execute(); } catch { }

            // Files should be at root of target, not in MyApp-v1.0.0 subfolder
            Assert.True(File.Exists(Path.Combine(targetDir, "app.exe")));
            Assert.True(File.Exists(Path.Combine(targetDir, "data", "config.json")));
            Assert.False(Directory.Exists(Path.Combine(targetDir, "MyApp-v1.0.0")));
        }

        /// <summary>
        /// Verifies that ExtractUpdate skips backup directories found within update archives.
        /// </summary>
        [Fact]
        public void ExtractUpdate_SkipsBackupDirectories()
        {
            var targetDir = CreateSubDir("target");

            // Create a zip that somehow contains backup directories
            var zipPath = CreateTestZip("update.zip", new[]
            {
                ("file.txt", "content"),
                (".backup_old/file.txt", "should be skipped"),
                ("subdir/.backup_test/file.txt", "also skipped")
            });

            var args = new UpdaterArguments
            {
                ProcessId = -1,
                ZipPath = zipPath,
                TargetDirectory = targetDir,
                MainExecutable = "test.exe",
                CreateBackup = false
            };

            var manager = new InstallationManager(args, Log);
            try { manager.Execute(); } catch { }

            // Normal file should exist
            Assert.True(File.Exists(Path.Combine(targetDir, "file.txt")));

            // Backup directories should not be created
            Assert.False(Directory.Exists(Path.Combine(targetDir, ".backup_old")));
            Assert.False(Directory.Exists(Path.Combine(targetDir, "subdir", ".backup_test")));
        }

        #endregion

        #region RestoreFromBackup Tests

        /// <summary>
        /// Verifies that RestoreFromBackup successfully restores files from a backup directory to the target.
        /// </summary>
        [Fact]
        public void RestoreFromBackup_RestoresFiles()
        {
            var targetDir = CreateSubDir("target");
            var backupDir = CreateSubDir("backup");

            // Create backup files
            File.WriteAllText(Path.Combine(backupDir, "restored.txt"), "backup content");
            var backupSubDir = Path.Combine(backupDir, "subdir");
            Directory.CreateDirectory(backupSubDir);
            File.WriteAllText(Path.Combine(backupSubDir, "nested.txt"), "nested backup");

            // Call RestoreFromBackup via reflection
            var args = new UpdaterArguments
            {
                ProcessId = 1,
                ZipPath = "test.zip",
                TargetDirectory = targetDir,
                MainExecutable = "test.exe"
            };

            var manager = new InstallationManager(args, Log);
            InvokeRestoreFromBackup(manager, backupDir);

            // Verify files were restored
            Assert.True(File.Exists(Path.Combine(targetDir, "restored.txt")));
            Assert.Equal("backup content", File.ReadAllText(Path.Combine(targetDir, "restored.txt")));
            Assert.True(File.Exists(Path.Combine(targetDir, "subdir", "nested.txt")));
        }

        /// <summary>
        /// Helper method to invoke the private RestoreFromBackup method via reflection.
        /// </summary>
        /// <param name="manager">The InstallationManager instance.</param>
        /// <param name="backupDir">The backup directory path to restore from.</param>
        private static void InvokeRestoreFromBackup(InstallationManager manager, string backupDir)
        {
            var method = typeof(InstallationManager)
                .GetMethod("RestoreFromBackup", BindingFlags.NonPublic | BindingFlags.Instance);
            method?.Invoke(manager, new object[] { backupDir });
        }

        #endregion

        #region CopyDirectory Tests

        /// <summary>
        /// Verifies that CopyDirectory recursively copies all files and subdirectories.
        /// </summary>
        [Fact]
        public void CopyDirectory_CopiesAllContents()
        {
            var sourceDir = CreateSubDir("source");
            var destDir = Path.Combine(_testDir, "dest");

            // Create source structure
            File.WriteAllText(Path.Combine(sourceDir, "file1.txt"), "content1");
            File.WriteAllText(Path.Combine(sourceDir, "file2.txt"), "content2");
            var subDir = Path.Combine(sourceDir, "sub");
            Directory.CreateDirectory(subDir);
            File.WriteAllText(Path.Combine(subDir, "nested.txt"), "nested");
            var deepDir = Path.Combine(subDir, "deep");
            Directory.CreateDirectory(deepDir);
            File.WriteAllText(Path.Combine(deepDir, "deep.txt"), "deep content");

            // Call CopyDirectory via reflection
            var args = new UpdaterArguments
            {
                ProcessId = 1,
                ZipPath = "test.zip",
                TargetDirectory = _testDir,
                MainExecutable = "test.exe"
            };

            var manager = new InstallationManager(args, Log);
            InvokeCopyDirectory(manager, sourceDir, destDir);

            // Verify all files were copied
            Assert.True(File.Exists(Path.Combine(destDir, "file1.txt")));
            Assert.True(File.Exists(Path.Combine(destDir, "file2.txt")));
            Assert.True(File.Exists(Path.Combine(destDir, "sub", "nested.txt")));
            Assert.True(File.Exists(Path.Combine(destDir, "sub", "deep", "deep.txt")));

            // Verify content
            Assert.Equal("content1", File.ReadAllText(Path.Combine(destDir, "file1.txt")));
            Assert.Equal("deep content", File.ReadAllText(Path.Combine(destDir, "sub", "deep", "deep.txt")));
        }

        /// <summary>
        /// Helper method to invoke the private CopyDirectory method via reflection.
        /// </summary>
        /// <param name="manager">The InstallationManager instance.</param>
        /// <param name="source">The source directory path to copy from.</param>
        /// <param name="dest">The destination directory path to copy to.</param>
        private static void InvokeCopyDirectory(InstallationManager manager, string source, string dest)
        {
            var method = typeof(InstallationManager)
                .GetMethod("CopyDirectory", BindingFlags.NonPublic | BindingFlags.Instance);
            method?.Invoke(manager, new object[] { source, dest });
        }

        #endregion

        #region Logging Tests

        /// <summary>
        /// Verifies that Execute method logs appropriate progress messages during execution.
        /// </summary>
        [Fact]
        public void Execute_LogsProgressMessages()
        {
            var targetDir = CreateSubDir("target");
            var zipPath = CreateTestZip("update.zip", new[] { ("file.txt", "content") });

            var args = new UpdaterArguments
            {
                ProcessId = -1,
                ZipPath = zipPath,
                TargetDirectory = targetDir,
                MainExecutable = "test.exe",
                CreateBackup = false
            };

            var manager = new InstallationManager(args, Log);
            try { manager.Execute(); } catch { }

            Assert.Contains(_logs, l => l.Contains("Starting update process"));
            Assert.Contains(_logs, l => l.Contains("Target directory"));
            Assert.Contains(_logs, l => l.Contains("Update package"));
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Creates a subdirectory within the test directory.
        /// </summary>
        /// <param name="name">The name of the subdirectory to create.</param>
        /// <returns>The full path to the created subdirectory.</returns>
        private string CreateSubDir(string name)
        {
            var path = Path.Combine(_testDir, name);
            Directory.CreateDirectory(path);
            return path;
        }

        /// <summary>
        /// Creates a test ZIP archive with the specified entries.
        /// </summary>
        /// <param name="name">The name of the ZIP file to create.</param>
        /// <param name="entries">An array of tuples containing entry paths and their content.</param>
        /// <returns>The full path to the created ZIP file.</returns>
        private string CreateTestZip(string name, (string path, string content)[] entries)
        {
            var zipPath = Path.Combine(_testDir, name);

            using (var archive = ZipFile.Open(zipPath, ZipArchiveMode.Create))
            {
                foreach (var (entryPath, content) in entries)
                {
                    var entry = archive.CreateEntry(entryPath);
                    using var writer = new StreamWriter(entry.Open());
                    writer.Write(content);
                }
            }

            return zipPath;
        }

        /// <summary>
        /// Creates a malicious ZIP archive containing path traversal attempts for security testing.
        /// </summary>
        /// <param name="name">The name of the ZIP file to create.</param>
        /// <returns>The full path to the created ZIP file.</returns>
        private string CreatePathTraversalZip(string name)
        {
            var zipPath = Path.Combine(_testDir, name);

            using (var archive = ZipFile.Open(zipPath, ZipArchiveMode.Create))
            {
                // Normal file
                var normalEntry = archive.CreateEntry("normal.txt");
                using (var writer = new StreamWriter(normalEntry.Open()))
                {
                    writer.Write("normal content");
                }

                // Path traversal attempt
                var maliciousEntry = archive.CreateEntry("../escaped.txt");
                using (var writer = new StreamWriter(maliciousEntry.Open()))
                {
                    writer.Write("malicious content");
                }

                // Another traversal attempt
                var deepTraversal = archive.CreateEntry("subdir/../../also_escaped.txt");
                using (var writer = new StreamWriter(deepTraversal.Open()))
                {
                    writer.Write("also malicious");
                }
            }

            return zipPath;
        }

        #endregion
    }
}
