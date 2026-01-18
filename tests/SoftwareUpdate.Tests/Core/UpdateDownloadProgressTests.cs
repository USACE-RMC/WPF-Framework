using Xunit;
using SoftwareUpdate;

namespace SoftwareUpdate.Tests.Core
{
    public class UpdateDownloadProgressTests
    {
        #region Property Tests

        [Fact]
        public void BytesDownloaded_CanBeSetAndRetrieved()
        {
            var progress = new UpdateDownloadProgress { BytesDownloaded = 1024 };

            Assert.Equal(1024, progress.BytesDownloaded);
        }

        [Fact]
        public void TotalBytes_CanBeSetAndRetrieved()
        {
            var progress = new UpdateDownloadProgress { TotalBytes = 2048 };

            Assert.Equal(2048, progress.TotalBytes);
        }

        [Fact]
        public void BytesPerSecond_CanBeSetAndRetrieved()
        {
            var progress = new UpdateDownloadProgress { BytesPerSecond = 1024 * 1024 };

            Assert.Equal(1024 * 1024, progress.BytesPerSecond);
        }

        #endregion

        #region ProgressPercentage Tests

        [Fact]
        public void ProgressPercentage_WhenTotalBytesZero_ReturnsNegativeOne()
        {
            var progress = new UpdateDownloadProgress
            {
                BytesDownloaded = 1024,
                TotalBytes = 0
            };

            Assert.Equal(-1, progress.ProgressPercentage);
        }

        [Fact]
        public void ProgressPercentage_WhenTotalBytesNegative_ReturnsNegativeOne()
        {
            var progress = new UpdateDownloadProgress
            {
                BytesDownloaded = 1024,
                TotalBytes = -1
            };

            Assert.Equal(-1, progress.ProgressPercentage);
        }

        [Fact]
        public void ProgressPercentage_WhenHalfComplete_ReturnsFifty()
        {
            var progress = new UpdateDownloadProgress
            {
                BytesDownloaded = 500,
                TotalBytes = 1000
            };

            Assert.Equal(50.0, progress.ProgressPercentage);
        }

        [Fact]
        public void ProgressPercentage_WhenComplete_ReturnsHundred()
        {
            var progress = new UpdateDownloadProgress
            {
                BytesDownloaded = 1000,
                TotalBytes = 1000
            };

            Assert.Equal(100.0, progress.ProgressPercentage);
        }

        [Fact]
        public void ProgressPercentage_WhenNotStarted_ReturnsZero()
        {
            var progress = new UpdateDownloadProgress
            {
                BytesDownloaded = 0,
                TotalBytes = 1000
            };

            Assert.Equal(0.0, progress.ProgressPercentage);
        }

        [Theory]
        [InlineData(250, 1000, 25.0)]
        [InlineData(750, 1000, 75.0)]
        [InlineData(333, 1000, 33.3)]
        [InlineData(1, 3, 33.333333333333336)]
        public void ProgressPercentage_CalculatesCorrectly(long downloaded, long total, double expected)
        {
            var progress = new UpdateDownloadProgress
            {
                BytesDownloaded = downloaded,
                TotalBytes = total
            };

            Assert.Equal(expected, progress.ProgressPercentage, precision: 10);
        }

        #endregion

        #region ProgressText Tests

        [Fact]
        public void ProgressText_WhenTotalKnown_IncludesAllInfo()
        {
            var progress = new UpdateDownloadProgress
            {
                BytesDownloaded = 512 * 1024, // 512 KB
                TotalBytes = 1024 * 1024 // 1 MB
            };

            var text = progress.ProgressText;

            Assert.Contains("512", text);
            Assert.Contains("KB", text);
            Assert.Contains("1", text);
            Assert.Contains("MB", text);
            Assert.Contains("50%", text);
        }

        [Fact]
        public void ProgressText_WhenTotalUnknown_ShowsOnlyDownloaded()
        {
            var progress = new UpdateDownloadProgress
            {
                BytesDownloaded = 1024 * 1024,
                TotalBytes = 0
            };

            var text = progress.ProgressText;

            Assert.Contains("1", text);
            Assert.Contains("MB", text);
            Assert.DoesNotContain("/", text);
            Assert.DoesNotContain("%", text);
        }

        [Fact]
        public void ProgressText_SmallBytes_FormatsAsBytes()
        {
            var progress = new UpdateDownloadProgress
            {
                BytesDownloaded = 500,
                TotalBytes = 0
            };

            var text = progress.ProgressText;

            Assert.Contains("500 B", text);
        }

        [Fact]
        public void ProgressText_Kilobytes_FormatsAsKB()
        {
            var progress = new UpdateDownloadProgress
            {
                BytesDownloaded = 2048,
                TotalBytes = 0
            };

            var text = progress.ProgressText;

            Assert.Contains("KB", text);
        }

        [Fact]
        public void ProgressText_Megabytes_FormatsAsMB()
        {
            var progress = new UpdateDownloadProgress
            {
                BytesDownloaded = 5 * 1024 * 1024,
                TotalBytes = 0
            };

            var text = progress.ProgressText;

            Assert.Contains("5.0 MB", text);
        }

        [Fact]
        public void ProgressText_Gigabytes_FormatsAsGB()
        {
            var progress = new UpdateDownloadProgress
            {
                BytesDownloaded = 2L * 1024 * 1024 * 1024,
                TotalBytes = 0
            };

            var text = progress.ProgressText;

            Assert.Contains("GB", text);
        }

        #endregion

        #region SpeedText Tests

        [Fact]
        public void SpeedText_WhenZero_ReturnsEmpty()
        {
            var progress = new UpdateDownloadProgress { BytesPerSecond = 0 };

            Assert.Equal("", progress.SpeedText);
        }

        [Fact]
        public void SpeedText_WhenNegative_ReturnsEmpty()
        {
            var progress = new UpdateDownloadProgress { BytesPerSecond = -100 };

            Assert.Equal("", progress.SpeedText);
        }

        [Fact]
        public void SpeedText_WhenPositive_IncludesPerSecond()
        {
            var progress = new UpdateDownloadProgress { BytesPerSecond = 1024 * 1024 };

            var text = progress.SpeedText;

            Assert.Contains("/s", text);
        }

        [Fact]
        public void SpeedText_LowSpeed_FormatsAsBytes()
        {
            var progress = new UpdateDownloadProgress { BytesPerSecond = 500 };

            var text = progress.SpeedText;

            Assert.Contains("B/s", text);
        }

        [Fact]
        public void SpeedText_MediumSpeed_FormatsAsKB()
        {
            var progress = new UpdateDownloadProgress { BytesPerSecond = 50 * 1024 };

            var text = progress.SpeedText;

            Assert.Contains("KB/s", text);
        }

        [Fact]
        public void SpeedText_HighSpeed_FormatsAsMB()
        {
            var progress = new UpdateDownloadProgress { BytesPerSecond = 10 * 1024 * 1024 };

            var text = progress.SpeedText;

            Assert.Contains("MB/s", text);
        }

        [Fact]
        public void SpeedText_VeryHighSpeed_FormatsAsGB()
        {
            var progress = new UpdateDownloadProgress { BytesPerSecond = 2L * 1024 * 1024 * 1024 };

            var text = progress.SpeedText;

            Assert.Contains("GB/s", text);
        }

        #endregion

        #region Default Values Tests

        [Fact]
        public void NewInstance_HasZeroBytesDownloaded()
        {
            var progress = new UpdateDownloadProgress();

            Assert.Equal(0, progress.BytesDownloaded);
        }

        [Fact]
        public void NewInstance_HasZeroTotalBytes()
        {
            var progress = new UpdateDownloadProgress();

            Assert.Equal(0, progress.TotalBytes);
        }

        [Fact]
        public void NewInstance_HasZeroBytesPerSecond()
        {
            var progress = new UpdateDownloadProgress();

            Assert.Equal(0, progress.BytesPerSecond);
        }

        #endregion
    }
}
