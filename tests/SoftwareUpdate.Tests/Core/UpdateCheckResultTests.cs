using Xunit;
using SoftwareUpdate;

namespace SoftwareUpdate.Tests.Core
{
    public class UpdateCheckResultTests
    {
        #region Default Property Tests

        [Fact]
        public void DefaultInstance_IsUpdateAvailable_IsFalse()
        {
            var result = new UpdateCheckResult();

            Assert.False(result.IsUpdateAvailable);
        }

        [Fact]
        public void DefaultInstance_Update_IsNull()
        {
            var result = new UpdateCheckResult();

            Assert.Null(result.Update);
        }

        [Fact]
        public void DefaultInstance_CurrentVersion_IsNull()
        {
            var result = new UpdateCheckResult();

            Assert.Null(result.CurrentVersion);
        }

        [Fact]
        public void DefaultInstance_Error_IsNull()
        {
            var result = new UpdateCheckResult();

            Assert.Null(result.Error);
        }

        [Fact]
        public void DefaultInstance_Success_IsTrue()
        {
            var result = new UpdateCheckResult();

            Assert.True(result.Success);
        }

        [Fact]
        public void DefaultInstance_IsSkippedVersion_IsFalse()
        {
            var result = new UpdateCheckResult();

            Assert.False(result.IsSkippedVersion);
        }

        #endregion

        #region Property Set Tests

        [Fact]
        public void IsUpdateAvailable_SetAndGet()
        {
            var result = new UpdateCheckResult { IsUpdateAvailable = true };

            Assert.True(result.IsUpdateAvailable);
        }

        [Fact]
        public void Update_SetAndGet()
        {
            var updateInfo = new UpdateInfo { Version = new SemanticVersion(2, 0, 0) };
            var result = new UpdateCheckResult { Update = updateInfo };

            Assert.Same(updateInfo, result.Update);
        }

        [Fact]
        public void CurrentVersion_SetAndGet()
        {
            var version = new SemanticVersion(1, 0, 0);
            var result = new UpdateCheckResult { CurrentVersion = version };

            Assert.Same(version, result.CurrentVersion);
        }

        [Fact]
        public void Error_SetAndGet()
        {
            var error = new Exception("Test error");
            var result = new UpdateCheckResult { Error = error };

            Assert.Same(error, result.Error);
        }

        [Fact]
        public void IsSkippedVersion_SetAndGet()
        {
            var result = new UpdateCheckResult { IsSkippedVersion = true };

            Assert.True(result.IsSkippedVersion);
        }

        #endregion

        #region Success Property Tests

        [Fact]
        public void Success_WhenErrorIsNull_ReturnsTrue()
        {
            var result = new UpdateCheckResult { Error = null };

            Assert.True(result.Success);
        }

        [Fact]
        public void Success_WhenErrorIsSet_ReturnsFalse()
        {
            var result = new UpdateCheckResult { Error = new Exception("Error") };

            Assert.False(result.Success);
        }

        #endregion

        #region NoUpdateAvailable Factory Tests

        [Fact]
        public void NoUpdateAvailable_SetsCurrentVersion()
        {
            var version = new SemanticVersion(1, 0, 0);

            var result = UpdateCheckResult.NoUpdateAvailable(version);

            Assert.Same(version, result.CurrentVersion);
        }

        [Fact]
        public void NoUpdateAvailable_SetsIsUpdateAvailableToFalse()
        {
            var version = new SemanticVersion(1, 0, 0);

            var result = UpdateCheckResult.NoUpdateAvailable(version);

            Assert.False(result.IsUpdateAvailable);
        }

        [Fact]
        public void NoUpdateAvailable_UpdateIsNull()
        {
            var version = new SemanticVersion(1, 0, 0);

            var result = UpdateCheckResult.NoUpdateAvailable(version);

            Assert.Null(result.Update);
        }

        [Fact]
        public void NoUpdateAvailable_ErrorIsNull()
        {
            var version = new SemanticVersion(1, 0, 0);

            var result = UpdateCheckResult.NoUpdateAvailable(version);

            Assert.Null(result.Error);
            Assert.True(result.Success);
        }

        #endregion

        #region UpdateAvailable Factory Tests

        [Fact]
        public void UpdateAvailable_SetsCurrentVersion()
        {
            var currentVersion = new SemanticVersion(1, 0, 0);
            var updateInfo = new UpdateInfo { Version = new SemanticVersion(2, 0, 0) };

            var result = UpdateCheckResult.UpdateAvailable(currentVersion, updateInfo);

            Assert.Same(currentVersion, result.CurrentVersion);
        }

        [Fact]
        public void UpdateAvailable_SetsUpdate()
        {
            var currentVersion = new SemanticVersion(1, 0, 0);
            var updateInfo = new UpdateInfo { Version = new SemanticVersion(2, 0, 0) };

            var result = UpdateCheckResult.UpdateAvailable(currentVersion, updateInfo);

            Assert.Same(updateInfo, result.Update);
        }

        [Fact]
        public void UpdateAvailable_SetsIsUpdateAvailableToTrue()
        {
            var currentVersion = new SemanticVersion(1, 0, 0);
            var updateInfo = new UpdateInfo { Version = new SemanticVersion(2, 0, 0) };

            var result = UpdateCheckResult.UpdateAvailable(currentVersion, updateInfo);

            Assert.True(result.IsUpdateAvailable);
        }

        [Fact]
        public void UpdateAvailable_DefaultIsSkipped_IsFalse()
        {
            var currentVersion = new SemanticVersion(1, 0, 0);
            var updateInfo = new UpdateInfo { Version = new SemanticVersion(2, 0, 0) };

            var result = UpdateCheckResult.UpdateAvailable(currentVersion, updateInfo);

            Assert.False(result.IsSkippedVersion);
        }

        [Fact]
        public void UpdateAvailable_WithIsSkipped_SetsIsSkippedVersion()
        {
            var currentVersion = new SemanticVersion(1, 0, 0);
            var updateInfo = new UpdateInfo { Version = new SemanticVersion(2, 0, 0) };

            var result = UpdateCheckResult.UpdateAvailable(currentVersion, updateInfo, isSkipped: true);

            Assert.True(result.IsSkippedVersion);
        }

        [Fact]
        public void UpdateAvailable_ErrorIsNull()
        {
            var currentVersion = new SemanticVersion(1, 0, 0);
            var updateInfo = new UpdateInfo { Version = new SemanticVersion(2, 0, 0) };

            var result = UpdateCheckResult.UpdateAvailable(currentVersion, updateInfo);

            Assert.Null(result.Error);
            Assert.True(result.Success);
        }

        #endregion

        #region Failed Factory Tests

        [Fact]
        public void Failed_SetsCurrentVersion()
        {
            var version = new SemanticVersion(1, 0, 0);
            var error = new Exception("Test error");

            var result = UpdateCheckResult.Failed(version, error);

            Assert.Same(version, result.CurrentVersion);
        }

        [Fact]
        public void Failed_SetsError()
        {
            var version = new SemanticVersion(1, 0, 0);
            var error = new Exception("Test error");

            var result = UpdateCheckResult.Failed(version, error);

            Assert.Same(error, result.Error);
        }

        [Fact]
        public void Failed_SetsIsUpdateAvailableToFalse()
        {
            var version = new SemanticVersion(1, 0, 0);
            var error = new Exception("Test error");

            var result = UpdateCheckResult.Failed(version, error);

            Assert.False(result.IsUpdateAvailable);
        }

        [Fact]
        public void Failed_SuccessIsFalse()
        {
            var version = new SemanticVersion(1, 0, 0);
            var error = new Exception("Test error");

            var result = UpdateCheckResult.Failed(version, error);

            Assert.False(result.Success);
        }

        [Fact]
        public void Failed_UpdateIsNull()
        {
            var version = new SemanticVersion(1, 0, 0);
            var error = new Exception("Test error");

            var result = UpdateCheckResult.Failed(version, error);

            Assert.Null(result.Update);
        }

        #endregion
    }
}
