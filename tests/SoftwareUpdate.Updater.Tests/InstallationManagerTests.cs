using Xunit;
using SoftwareUpdate.Updater;
using System.IO.Compression;
using System.Reflection;

namespace SoftwareUpdate.Updater.Tests
{
    public class InstallationManagerTests : IDisposable
    {
        private readonly string _testDir;
        private readonly List<string> _logs = new();

        public InstallationManagerTests()
        {
            _testDir = Path.Combine(Path.GetTempPath(), "InstallManagerTests_" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(_testDir);
        }

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

        private void Log(string message) => _logs.Add(message);

        #region Constructor Tests

        [Fact]
        public void Constructor_NullArgs_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                new InstallationManager(null!, Log));
        }

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

        [Fact]
        public void ParseBackupTimestamp_NullPath_ReturnsNull()
        {
            var result = InvokeParseBackupTimestamp(null!);

            Assert.Null(result);
        }

        private static DateTime? InvokeParseBackupTimestamp(string backupPath)
        {
            var method = typeof(InstallationManager)
                .GetMethod("ParseBackupTimestamp", BindingFlags.NonPublic | BindingFlags.Static);
            return (DateTime?)method?.Invoke(null, new object[] { backupPath });
        }

        #endregion

        #region CreateBackup Tests (Integration)

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

        private static void InvokeRestoreFromBackup(InstallationManager manager, string backupDir)
        {
            var method = typeof(InstallationManager)
                .GetMethod("RestoreFromBackup", BindingFlags.NonPublic | BindingFlags.Instance);
            method?.Invoke(manager, new object[] { backupDir });
        }

        #endregion

        #region CopyDirectory Tests

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

        private static void InvokeCopyDirectory(InstallationManager manager, string source, string dest)
        {
            var method = typeof(InstallationManager)
                .GetMethod("CopyDirectory", BindingFlags.NonPublic | BindingFlags.Instance);
            method?.Invoke(manager, new object[] { source, dest });
        }

        #endregion

        #region Logging Tests

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

        private string CreateSubDir(string name)
        {
            var path = Path.Combine(_testDir, name);
            Directory.CreateDirectory(path);
            return path;
        }

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
