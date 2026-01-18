using Xunit;
using SoftwareUpdate;

namespace SoftwareUpdate.Tests.Core
{
    public class UpdateInfoTests
    {
        #region Property Tests

        [Fact]
        public void Version_CanBeSetAndRetrieved()
        {
            var version = SemanticVersion.Parse("1.2.3");
            var info = new UpdateInfo { Version = version };

            Assert.Equal(version, info.Version);
        }

        [Fact]
        public void Name_CanBeSetAndRetrieved()
        {
            var info = new UpdateInfo { Name = "Test Release" };

            Assert.Equal("Test Release", info.Name);
        }

        [Fact]
        public void DownloadUrl_CanBeSetAndRetrieved()
        {
            var url = "https://example.com/download.zip";
            var info = new UpdateInfo { DownloadUrl = url };

            Assert.Equal(url, info.DownloadUrl);
        }

        [Fact]
        public void ReleaseNotes_CanBeSetAndRetrieved()
        {
            var notes = "## Changelog\n- Fixed bugs\n- Added features";
            var info = new UpdateInfo { ReleaseNotes = notes };

            Assert.Equal(notes, info.ReleaseNotes);
        }

        [Fact]
        public void PublishedAt_CanBeSetAndRetrieved()
        {
            var date = new DateTime(2024, 6, 15, 12, 0, 0);
            var info = new UpdateInfo { PublishedAt = date };

            Assert.Equal(date, info.PublishedAt);
        }

        [Fact]
        public void DownloadSize_CanBeSetAndRetrieved()
        {
            var info = new UpdateInfo { DownloadSize = 1024 * 1024 * 50 }; // 50 MB

            Assert.Equal(1024 * 1024 * 50, info.DownloadSize);
        }

        [Fact]
        public void AssetName_CanBeSetAndRetrieved()
        {
            var info = new UpdateInfo { AssetName = "MyApp-v1.0.0-win-x64.zip" };

            Assert.Equal("MyApp-v1.0.0-win-x64.zip", info.AssetName);
        }

        [Fact]
        public void IsPreRelease_CanBeSetAndRetrieved()
        {
            var info = new UpdateInfo { IsPreRelease = true };

            Assert.True(info.IsPreRelease);
        }

        [Fact]
        public void IsPreRelease_DefaultIsFalse()
        {
            var info = new UpdateInfo();

            Assert.False(info.IsPreRelease);
        }

        [Fact]
        public void ReleasePageUrl_CanBeSetAndRetrieved()
        {
            var url = "https://github.com/owner/repo/releases/tag/v1.0.0";
            var info = new UpdateInfo { ReleasePageUrl = url };

            Assert.Equal(url, info.ReleasePageUrl);
        }

        [Fact]
        public void Sha256Checksum_CanBeSetAndRetrieved()
        {
            var checksum = "abcdef1234567890abcdef1234567890abcdef1234567890abcdef1234567890";
            var info = new UpdateInfo { Sha256Checksum = checksum };

            Assert.Equal(checksum, info.Sha256Checksum);
        }

        #endregion

        #region ToString Tests

        [Fact]
        public void ToString_WithNoDownloadSize_ReturnsNameAndVersion()
        {
            var info = new UpdateInfo
            {
                Name = "Test Release",
                Version = SemanticVersion.Parse("2.0.0"),
                DownloadSize = 0
            };

            var result = info.ToString();

            Assert.Equal("Test Release (2.0.0)", result);
        }

        [Fact]
        public void ToString_WithDownloadSize_IncludesFormattedSize()
        {
            var info = new UpdateInfo
            {
                Name = "Test Release",
                Version = SemanticVersion.Parse("2.0.0"),
                DownloadSize = 1024 * 1024 * 10 // 10 MB
            };

            var result = info.ToString();

            Assert.Contains("Test Release (2.0.0)", result);
            Assert.Contains("10.0 MB", result);
        }

        [Fact]
        public void ToString_WithSmallDownloadSize_FormatsCorrectly()
        {
            var info = new UpdateInfo
            {
                Name = "Small Update",
                Version = SemanticVersion.Parse("1.0.1"),
                DownloadSize = 512 * 1024 // 0.5 MB
            };

            var result = info.ToString();

            Assert.Contains("0.5 MB", result);
        }

        [Fact]
        public void ToString_WithLargeDownloadSize_FormatsCorrectly()
        {
            var info = new UpdateInfo
            {
                Name = "Large Update",
                Version = SemanticVersion.Parse("3.0.0"),
                DownloadSize = 1024L * 1024 * 1024 // 1 GB
            };

            var result = info.ToString();

            Assert.Contains("1024.0 MB", result);
        }

        [Fact]
        public void ToString_WithNegativeDownloadSize_TreatsAsNoSize()
        {
            var info = new UpdateInfo
            {
                Name = "Test",
                Version = SemanticVersion.Parse("1.0.0"),
                DownloadSize = -1
            };

            var result = info.ToString();

            Assert.Equal("Test (1.0.0)", result);
        }

        [Fact]
        public void ToString_WithNullNameAndVersion_HandlesGracefully()
        {
            var info = new UpdateInfo
            {
                Name = null,
                Version = null,
                DownloadSize = 0
            };

            var result = info.ToString();

            Assert.Equal(" ()", result);
        }

        #endregion

        #region Default Values Tests

        [Fact]
        public void NewInstance_HasNullVersion()
        {
            var info = new UpdateInfo();

            Assert.Null(info.Version);
        }

        [Fact]
        public void NewInstance_HasNullName()
        {
            var info = new UpdateInfo();

            Assert.Null(info.Name);
        }

        [Fact]
        public void NewInstance_HasNullDownloadUrl()
        {
            var info = new UpdateInfo();

            Assert.Null(info.DownloadUrl);
        }

        [Fact]
        public void NewInstance_HasZeroDownloadSize()
        {
            var info = new UpdateInfo();

            Assert.Equal(0, info.DownloadSize);
        }

        [Fact]
        public void NewInstance_HasDefaultPublishedAt()
        {
            var info = new UpdateInfo();

            Assert.Equal(default(DateTime), info.PublishedAt);
        }

        #endregion
    }
}
