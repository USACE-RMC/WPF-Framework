using Xunit;
using SoftwareUpdate;
using SoftwareUpdate.GitHub;

namespace SoftwareUpdate.Tests.GitHub
{
    public class GitHubUpdateServiceTests : IDisposable
    {
        private readonly string _testDir;
        private GitHubUpdateService? _service;

        public GitHubUpdateServiceTests()
        {
            _testDir = Path.Combine(Path.GetTempPath(), "GitHubUpdateServiceTests_" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(_testDir);
        }

        public void Dispose()
        {
            _service?.Dispose();

            if (Directory.Exists(_testDir))
            {
                try
                {
                    Directory.Delete(_testDir, recursive: true);
                }
                catch
                {
                    // Ignore cleanup failures
                }
            }
        }

        private UpdateOptions CreateValidOptions()
        {
            return new UpdateOptions
            {
                GitHubOwner = "TestOwner",
                GitHubRepo = "TestRepo",
                CurrentVersion = SemanticVersion.Parse("1.0.0"),
                SkippedVersionsFilePath = Path.Combine(_testDir, "skipped.txt")
            };
        }

        #region Constructor Tests

        [Fact]
        public void Constructor_NullOptions_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => new GitHubUpdateService(null!));
        }

        [Fact]
        public void Constructor_InvalidOptions_ThrowsArgumentException()
        {
            var options = new UpdateOptions
            {
                GitHubOwner = null,
                GitHubRepo = "TestRepo",
                CurrentVersion = SemanticVersion.Parse("1.0.0")
            };

            Assert.Throws<ArgumentException>(() => new GitHubUpdateService(options));
        }

        [Fact]
        public void Constructor_ValidOptions_CreatesInstance()
        {
            var options = CreateValidOptions();

            _service = new GitHubUpdateService(options);

            Assert.NotNull(_service);
        }

        [Fact]
        public void Constructor_SetsOptionsProperty()
        {
            var options = CreateValidOptions();

            _service = new GitHubUpdateService(options);

            Assert.Same(options, _service.Options);
        }

        [Fact]
        public void Constructor_InitialStateIsIdle()
        {
            var options = CreateValidOptions();

            _service = new GitHubUpdateService(options);

            Assert.Equal(UpdateState.Idle, _service.State);
        }

        [Fact]
        public void Constructor_AvailableUpdateIsNull()
        {
            var options = CreateValidOptions();

            _service = new GitHubUpdateService(options);

            Assert.Null(_service.AvailableUpdate);
        }

        #endregion

        #region SkipVersion Tests

        [Fact]
        public void SkipVersion_AddsVersionToSkippedList()
        {
            var options = CreateValidOptions();
            _service = new GitHubUpdateService(options);
            var version = SemanticVersion.Parse("2.0.0");

            _service.SkipVersion(version);

            Assert.True(_service.IsVersionSkipped(version));
        }

        [Fact]
        public void SkipVersion_NullVersion_DoesNotThrow()
        {
            var options = CreateValidOptions();
            _service = new GitHubUpdateService(options);

            var exception = Record.Exception(() => _service.SkipVersion(null));

            Assert.Null(exception);
        }

        [Fact]
        public void SkipVersion_PersistsToFile()
        {
            var skippedFilePath = Path.Combine(_testDir, "skipped.txt");
            var options = CreateValidOptions();
            options.SkippedVersionsFilePath = skippedFilePath;

            _service = new GitHubUpdateService(options);
            _service.SkipVersion(SemanticVersion.Parse("2.0.0"));

            Assert.True(File.Exists(skippedFilePath));
            var content = File.ReadAllText(skippedFilePath);
            Assert.Contains("2.0.0", content);
        }

        [Fact]
        public void SkipVersion_MultipleVersions_AllSkipped()
        {
            var options = CreateValidOptions();
            _service = new GitHubUpdateService(options);

            _service.SkipVersion(SemanticVersion.Parse("2.0.0"));
            _service.SkipVersion(SemanticVersion.Parse("2.1.0"));
            _service.SkipVersion(SemanticVersion.Parse("3.0.0"));

            Assert.True(_service.IsVersionSkipped(SemanticVersion.Parse("2.0.0")));
            Assert.True(_service.IsVersionSkipped(SemanticVersion.Parse("2.1.0")));
            Assert.True(_service.IsVersionSkipped(SemanticVersion.Parse("3.0.0")));
        }

        #endregion

        #region IsVersionSkipped Tests

        [Fact]
        public void IsVersionSkipped_NullVersion_ReturnsFalse()
        {
            var options = CreateValidOptions();
            _service = new GitHubUpdateService(options);

            Assert.False(_service.IsVersionSkipped(null));
        }

        [Fact]
        public void IsVersionSkipped_NotSkippedVersion_ReturnsFalse()
        {
            var options = CreateValidOptions();
            _service = new GitHubUpdateService(options);

            Assert.False(_service.IsVersionSkipped(SemanticVersion.Parse("99.0.0")));
        }

        [Fact]
        public void IsVersionSkipped_LoadsFromFile()
        {
            var skippedFilePath = Path.Combine(_testDir, "skipped.txt");
            File.WriteAllLines(skippedFilePath, new[] { "2.0.0", "3.0.0" });

            var options = CreateValidOptions();
            options.SkippedVersionsFilePath = skippedFilePath;

            _service = new GitHubUpdateService(options);

            Assert.True(_service.IsVersionSkipped(SemanticVersion.Parse("2.0.0")));
            Assert.True(_service.IsVersionSkipped(SemanticVersion.Parse("3.0.0")));
            Assert.False(_service.IsVersionSkipped(SemanticVersion.Parse("1.0.0")));
        }

        [Fact]
        public void IsVersionSkipped_CaseInsensitive()
        {
            var skippedFilePath = Path.Combine(_testDir, "skipped.txt");
            File.WriteAllLines(skippedFilePath, new[] { "2.0.0-BETA" });

            var options = CreateValidOptions();
            options.SkippedVersionsFilePath = skippedFilePath;

            _service = new GitHubUpdateService(options);

            // SemanticVersion.ToString() should match regardless of case
            Assert.True(_service.IsVersionSkipped(SemanticVersion.Parse("2.0.0-beta")));
        }

        #endregion

        #region ClearSkippedVersions Tests

        [Fact]
        public void ClearSkippedVersions_RemovesAllSkippedVersions()
        {
            var options = CreateValidOptions();
            _service = new GitHubUpdateService(options);

            _service.SkipVersion(SemanticVersion.Parse("2.0.0"));
            _service.SkipVersion(SemanticVersion.Parse("3.0.0"));
            _service.ClearSkippedVersions();

            Assert.False(_service.IsVersionSkipped(SemanticVersion.Parse("2.0.0")));
            Assert.False(_service.IsVersionSkipped(SemanticVersion.Parse("3.0.0")));
        }

        [Fact]
        public void ClearSkippedVersions_ClearsFile()
        {
            var skippedFilePath = Path.Combine(_testDir, "skipped.txt");
            var options = CreateValidOptions();
            options.SkippedVersionsFilePath = skippedFilePath;

            _service = new GitHubUpdateService(options);
            _service.SkipVersion(SemanticVersion.Parse("2.0.0"));
            _service.ClearSkippedVersions();

            var content = File.ReadAllText(skippedFilePath);
            Assert.Empty(content.Trim());
        }

        [Fact]
        public void ClearSkippedVersions_WhenEmpty_DoesNotThrow()
        {
            var options = CreateValidOptions();
            _service = new GitHubUpdateService(options);

            var exception = Record.Exception(() => _service.ClearSkippedVersions());

            Assert.Null(exception);
        }

        #endregion

        #region DownloadUpdateAsync Validation Tests

        [Fact]
        public async Task DownloadUpdateAsync_NullUpdate_ThrowsArgumentNullException()
        {
            var options = CreateValidOptions();
            _service = new GitHubUpdateService(options);

            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                _service.DownloadUpdateAsync(null!));
        }

        [Fact]
        public async Task DownloadUpdateAsync_InvalidAssetName_ThrowsArgumentException()
        {
            var options = CreateValidOptions();
            _service = new GitHubUpdateService(options);

            var update = new UpdateInfo
            {
                AssetName = "../malicious.zip", // Path traversal attempt
                DownloadUrl = "https://example.com/file.zip"
            };

            await Assert.ThrowsAsync<ArgumentException>(() =>
                _service.DownloadUpdateAsync(update));
        }

        [Fact]
        public async Task DownloadUpdateAsync_AssetNameWithDoubleDots_ThrowsArgumentException()
        {
            var options = CreateValidOptions();
            _service = new GitHubUpdateService(options);

            var update = new UpdateInfo
            {
                AssetName = "file..zip", // Contains double dots
                DownloadUrl = "https://example.com/file.zip"
            };

            await Assert.ThrowsAsync<ArgumentException>(() =>
                _service.DownloadUpdateAsync(update));
        }

        #endregion

        #region InstallUpdateAndRestart Validation Tests

        [Fact]
        public void InstallUpdateAndRestart_NullFilePath_ThrowsArgumentNullException()
        {
            var options = CreateValidOptions();
            _service = new GitHubUpdateService(options);

            Assert.Throws<ArgumentNullException>(() =>
                _service.InstallUpdateAndRestart(null!));
        }

        [Fact]
        public void InstallUpdateAndRestart_EmptyFilePath_ThrowsArgumentNullException()
        {
            var options = CreateValidOptions();
            _service = new GitHubUpdateService(options);

            Assert.Throws<ArgumentNullException>(() =>
                _service.InstallUpdateAndRestart(string.Empty));
        }

        [Fact]
        public void InstallUpdateAndRestart_NonExistentFile_ThrowsFileNotFoundException()
        {
            var options = CreateValidOptions();
            _service = new GitHubUpdateService(options);

            Assert.Throws<FileNotFoundException>(() =>
                _service.InstallUpdateAndRestart("/nonexistent/file.zip"));
        }

        #endregion

        #region Dispose Tests

        [Fact]
        public void Dispose_CanBeCalledMultipleTimes()
        {
            var options = CreateValidOptions();
            _service = new GitHubUpdateService(options);

            var exception = Record.Exception(() =>
            {
                _service.Dispose();
                _service.Dispose();
            });

            Assert.Null(exception);
        }

        #endregion

        #region State Property Tests

        [Fact]
        public void State_IsThreadSafe()
        {
            var options = CreateValidOptions();
            _service = new GitHubUpdateService(options);

            // Simple test - just verify reading state from multiple threads doesn't throw
            var tasks = Enumerable.Range(0, 10)
                .Select(_ => Task.Run(() => _ = _service.State))
                .ToArray();

            var exception = Record.Exception(() => Task.WaitAll(tasks));

            Assert.Null(exception);
        }

        #endregion

        #region AvailableUpdate Property Tests

        [Fact]
        public void AvailableUpdate_IsThreadSafe()
        {
            var options = CreateValidOptions();
            _service = new GitHubUpdateService(options);

            // Simple test - just verify reading from multiple threads doesn't throw
            var tasks = Enumerable.Range(0, 10)
                .Select(_ => Task.Run(() => _ = _service.AvailableUpdate))
                .ToArray();

            var exception = Record.Exception(() => Task.WaitAll(tasks));

            Assert.Null(exception);
        }

        #endregion

        #region Skipped Versions File Handling Tests

        [Fact]
        public void Constructor_MissingSkippedVersionsFile_DoesNotThrow()
        {
            var options = CreateValidOptions();
            options.SkippedVersionsFilePath = Path.Combine(_testDir, "nonexistent", "skipped.txt");

            var exception = Record.Exception(() => _service = new GitHubUpdateService(options));

            Assert.Null(exception);
        }

        [Fact]
        public void Constructor_EmptySkippedVersionsFile_DoesNotThrow()
        {
            var skippedFilePath = Path.Combine(_testDir, "skipped.txt");
            File.WriteAllText(skippedFilePath, "");

            var options = CreateValidOptions();
            options.SkippedVersionsFilePath = skippedFilePath;

            var exception = Record.Exception(() => _service = new GitHubUpdateService(options));

            Assert.Null(exception);
        }

        [Fact]
        public void Constructor_SkippedVersionsFileWithWhitespace_IgnoresEmpty()
        {
            var skippedFilePath = Path.Combine(_testDir, "skipped.txt");
            File.WriteAllLines(skippedFilePath, new[] { "", "  ", "2.0.0", "  ", "" });

            var options = CreateValidOptions();
            options.SkippedVersionsFilePath = skippedFilePath;

            _service = new GitHubUpdateService(options);

            Assert.True(_service.IsVersionSkipped(SemanticVersion.Parse("2.0.0")));
        }

        [Fact]
        public void SkipVersion_CreatesDirectoryIfNeeded()
        {
            var skippedFilePath = Path.Combine(_testDir, "subdir", "deep", "skipped.txt");
            var options = CreateValidOptions();
            options.SkippedVersionsFilePath = skippedFilePath;

            _service = new GitHubUpdateService(options);
            _service.SkipVersion(SemanticVersion.Parse("2.0.0"));

            Assert.True(File.Exists(skippedFilePath));
        }

        #endregion
    }
}
