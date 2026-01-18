using Xunit;
using SoftwareUpdate;

namespace SoftwareUpdate.Tests.Core
{
    public class UpdateOptionsTests
    {
        #region Default Values Tests

        [Fact]
        public void DefaultInstance_GitHubOwner_IsNull()
        {
            var options = new UpdateOptions();

            Assert.Null(options.GitHubOwner);
        }

        [Fact]
        public void DefaultInstance_GitHubRepo_IsNull()
        {
            var options = new UpdateOptions();

            Assert.Null(options.GitHubRepo);
        }

        [Fact]
        public void DefaultInstance_CurrentVersion_IsNull()
        {
            var options = new UpdateOptions();

            Assert.Null(options.CurrentVersion);
        }

        [Fact]
        public void DefaultInstance_AssetNamePattern_IsDefaultZip()
        {
            var options = new UpdateOptions();

            Assert.Equal("*.zip", options.AssetNamePattern);
        }

        [Fact]
        public void DefaultInstance_IncludePreReleases_IsFalse()
        {
            var options = new UpdateOptions();

            Assert.False(options.IncludePreReleases);
        }

        [Fact]
        public void DefaultInstance_CreateBackup_IsTrue()
        {
            var options = new UpdateOptions();

            Assert.True(options.CreateBackup);
        }

        [Fact]
        public void DefaultInstance_RequestTimeoutSeconds_Is30()
        {
            var options = new UpdateOptions();

            Assert.Equal(30, options.RequestTimeoutSeconds);
        }

        [Fact]
        public void DefaultInstance_GitHubToken_IsNull()
        {
            var options = new UpdateOptions();

            Assert.Null(options.GitHubToken);
        }

        #endregion

        #region Property Set Tests

        [Fact]
        public void GitHubOwner_SetAndGet()
        {
            var options = new UpdateOptions { GitHubOwner = "USACE-RMC" };

            Assert.Equal("USACE-RMC", options.GitHubOwner);
        }

        [Fact]
        public void GitHubRepo_SetAndGet()
        {
            var options = new UpdateOptions { GitHubRepo = "RMC-BestFit" };

            Assert.Equal("RMC-BestFit", options.GitHubRepo);
        }

        [Fact]
        public void CurrentVersion_SetAndGet()
        {
            var version = new SemanticVersion(1, 0, 0);
            var options = new UpdateOptions { CurrentVersion = version };

            Assert.Same(version, options.CurrentVersion);
        }

        [Fact]
        public void AssetNamePattern_SetAndGet()
        {
            var options = new UpdateOptions { AssetNamePattern = "MyApp-*.zip" };

            Assert.Equal("MyApp-*.zip", options.AssetNamePattern);
        }

        [Fact]
        public void IncludePreReleases_SetAndGet()
        {
            var options = new UpdateOptions { IncludePreReleases = true };

            Assert.True(options.IncludePreReleases);
        }

        [Fact]
        public void GitHubToken_SetAndGet()
        {
            var options = new UpdateOptions { GitHubToken = "ghp_token123" };

            Assert.Equal("ghp_token123", options.GitHubToken);
        }

        [Fact]
        public void CreateBackup_SetAndGet()
        {
            var options = new UpdateOptions { CreateBackup = false };

            Assert.False(options.CreateBackup);
        }

        [Fact]
        public void RequestTimeoutSeconds_SetAndGet()
        {
            var options = new UpdateOptions { RequestTimeoutSeconds = 60 };

            Assert.Equal(60, options.RequestTimeoutSeconds);
        }

        #endregion

        #region Validate Tests

        [Fact]
        public void Validate_NullGitHubOwner_ThrowsArgumentException()
        {
            var options = new UpdateOptions
            {
                GitHubOwner = null,
                GitHubRepo = "ValidRepo",
                CurrentVersion = new SemanticVersion(1, 0, 0)
            };

            var ex = Assert.Throws<ArgumentException>(() => options.Validate());
            Assert.Contains("GitHubOwner", ex.Message);
        }

        [Fact]
        public void Validate_EmptyGitHubOwner_ThrowsArgumentException()
        {
            var options = new UpdateOptions
            {
                GitHubOwner = "",
                GitHubRepo = "ValidRepo",
                CurrentVersion = new SemanticVersion(1, 0, 0)
            };

            var ex = Assert.Throws<ArgumentException>(() => options.Validate());
            Assert.Contains("GitHubOwner", ex.Message);
        }

        [Fact]
        public void Validate_InvalidGitHubOwner_ThrowsArgumentException()
        {
            var options = new UpdateOptions
            {
                GitHubOwner = "-invalid",
                GitHubRepo = "ValidRepo",
                CurrentVersion = new SemanticVersion(1, 0, 0)
            };

            var ex = Assert.Throws<ArgumentException>(() => options.Validate());
            Assert.Contains("alphanumeric", ex.Message);
        }

        [Fact]
        public void Validate_NullGitHubRepo_ThrowsArgumentException()
        {
            var options = new UpdateOptions
            {
                GitHubOwner = "ValidOwner",
                GitHubRepo = null,
                CurrentVersion = new SemanticVersion(1, 0, 0)
            };

            var ex = Assert.Throws<ArgumentException>(() => options.Validate());
            Assert.Contains("GitHubRepo", ex.Message);
        }

        [Fact]
        public void Validate_InvalidGitHubRepo_ThrowsArgumentException()
        {
            var options = new UpdateOptions
            {
                GitHubOwner = "ValidOwner",
                GitHubRepo = "invalid-",
                CurrentVersion = new SemanticVersion(1, 0, 0)
            };

            var ex = Assert.Throws<ArgumentException>(() => options.Validate());
            Assert.Contains("alphanumeric", ex.Message);
        }

        [Fact]
        public void Validate_NullCurrentVersion_ThrowsArgumentException()
        {
            var options = new UpdateOptions
            {
                GitHubOwner = "ValidOwner",
                GitHubRepo = "ValidRepo",
                CurrentVersion = null
            };

            var ex = Assert.Throws<ArgumentException>(() => options.Validate());
            Assert.Contains("CurrentVersion", ex.Message);
        }

        [Fact]
        public void Validate_ValidOptions_DoesNotThrow()
        {
            var options = new UpdateOptions
            {
                GitHubOwner = "USACE-RMC",
                GitHubRepo = "RMC-BestFit",
                CurrentVersion = new SemanticVersion(1, 0, 0)
            };

            var exception = Record.Exception(() => options.Validate());

            Assert.Null(exception);
        }

        [Theory]
        [InlineData("a")]
        [InlineData("ValidOwner")]
        [InlineData("valid-owner")]
        [InlineData("valid123")]
        [InlineData("a1b2c3d4e5f6g7h8i9j0k1l2m3n4o5p6q7r8")]
        public void Validate_ValidOwnerFormats_DoesNotThrow(string owner)
        {
            var options = new UpdateOptions
            {
                GitHubOwner = owner,
                GitHubRepo = "ValidRepo",
                CurrentVersion = new SemanticVersion(1, 0, 0)
            };

            var exception = Record.Exception(() => options.Validate());

            Assert.Null(exception);
        }

        [Theory]
        [InlineData("-invalid")]
        [InlineData("invalid-")]
        [InlineData("in valid")]
        [InlineData("in_valid")]
        [InlineData("invalid@owner")]
        public void Validate_InvalidOwnerFormats_ThrowsArgumentException(string owner)
        {
            var options = new UpdateOptions
            {
                GitHubOwner = owner,
                GitHubRepo = "ValidRepo",
                CurrentVersion = new SemanticVersion(1, 0, 0)
            };

            Assert.Throws<ArgumentException>(() => options.Validate());
        }

        #endregion

        #region Resolved Properties Tests

        [Fact]
        public void ResolvedInstallDirectory_WhenSet_ReturnsSetValue()
        {
            var options = new UpdateOptions { InstallDirectory = @"C:\MyApp" };

            Assert.Equal(@"C:\MyApp", options.ResolvedInstallDirectory);
        }

        [Fact]
        public void ResolvedInstallDirectory_WhenNull_ReturnsExecutingAssemblyDirectory()
        {
            var options = new UpdateOptions { InstallDirectory = null };

            Assert.False(string.IsNullOrEmpty(options.ResolvedInstallDirectory));
        }

        [Fact]
        public void ResolvedMainExecutableName_WhenSet_ReturnsSetValue()
        {
            var options = new UpdateOptions { MainExecutableName = "MyApp.exe" };

            Assert.Equal("MyApp.exe", options.ResolvedMainExecutableName);
        }

        [Fact]
        public void ResolvedMainExecutableName_WhenNull_ReturnsSomething()
        {
            var options = new UpdateOptions { MainExecutableName = null };

            Assert.False(string.IsNullOrEmpty(options.ResolvedMainExecutableName));
        }

        [Fact]
        public void ResolvedUpdaterPath_WhenSet_ReturnsSetValue()
        {
            var options = new UpdateOptions { UpdaterExecutablePath = @"C:\Updater\Updater.exe" };

            Assert.Equal(@"C:\Updater\Updater.exe", options.ResolvedUpdaterPath);
        }

        [Fact]
        public void ResolvedUpdaterPath_WhenNull_CombinesWithInstallDirectory()
        {
            var options = new UpdateOptions { UpdaterExecutablePath = null };

            Assert.Contains("SoftwareUpdate.Updater.exe", options.ResolvedUpdaterPath);
        }

        [Fact]
        public void ResolvedSkippedVersionsPath_WhenSet_ReturnsSetValue()
        {
            var options = new UpdateOptions { SkippedVersionsFilePath = @"C:\Custom\skipped.txt" };

            Assert.Equal(@"C:\Custom\skipped.txt", options.ResolvedSkippedVersionsPath);
        }

        [Fact]
        public void ResolvedSkippedVersionsPath_WhenNull_UsesAppDataAndRepo()
        {
            var options = new UpdateOptions
            {
                SkippedVersionsFilePath = null,
                GitHubRepo = "MyTestRepo"
            };

            Assert.Contains("MyTestRepo", options.ResolvedSkippedVersionsPath);
            Assert.Contains("skipped_versions.txt", options.ResolvedSkippedVersionsPath);
        }

        [Fact]
        public void ResolvedSkippedVersionsPath_WhenRepoNull_UsesSoftwareUpdate()
        {
            var options = new UpdateOptions
            {
                SkippedVersionsFilePath = null,
                GitHubRepo = null
            };

            Assert.Contains("SoftwareUpdate", options.ResolvedSkippedVersionsPath);
        }

        #endregion
    }
}
