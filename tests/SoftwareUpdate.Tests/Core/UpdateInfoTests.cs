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
* the following disclaimer in the documentation and/or materials provided with the distribution.
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
using SoftwareUpdate;

namespace SoftwareUpdate.Tests.Core
{
    /// <summary>
    /// Provides unit tests for the <see cref="UpdateInfo"/> class, verifying properties,
    /// default values, and ToString formatting for update information.
    /// </summary>
    public class UpdateInfoTests
    {
        #region Property Tests

        /// <summary>
        /// Verifies that the Version property can be set and retrieved correctly.
        /// </summary>
        [Fact]
        public void Version_CanBeSetAndRetrieved()
        {
            var version = SemanticVersion.Parse("1.2.3");
            var info = new UpdateInfo { Version = version };

            Assert.Equal(version, info.Version);
        }

        /// <summary>
        /// Verifies that the Name property can be set and retrieved correctly.
        /// </summary>
        [Fact]
        public void Name_CanBeSetAndRetrieved()
        {
            var info = new UpdateInfo { Name = "Test Release" };

            Assert.Equal("Test Release", info.Name);
        }

        /// <summary>
        /// Verifies that the DownloadUrl property can be set and retrieved correctly.
        /// </summary>
        [Fact]
        public void DownloadUrl_CanBeSetAndRetrieved()
        {
            var url = "https://example.com/download.zip";
            var info = new UpdateInfo { DownloadUrl = url };

            Assert.Equal(url, info.DownloadUrl);
        }

        /// <summary>
        /// Verifies that the ReleaseNotes property can be set and retrieved correctly.
        /// </summary>
        [Fact]
        public void ReleaseNotes_CanBeSetAndRetrieved()
        {
            var notes = "## Changelog\n- Fixed bugs\n- Added features";
            var info = new UpdateInfo { ReleaseNotes = notes };

            Assert.Equal(notes, info.ReleaseNotes);
        }

        /// <summary>
        /// Verifies that the PublishedAt property can be set and retrieved correctly.
        /// </summary>
        [Fact]
        public void PublishedAt_CanBeSetAndRetrieved()
        {
            var date = new DateTime(2024, 6, 15, 12, 0, 0);
            var info = new UpdateInfo { PublishedAt = date };

            Assert.Equal(date, info.PublishedAt);
        }

        /// <summary>
        /// Verifies that the DownloadSize property can be set and retrieved correctly.
        /// </summary>
        [Fact]
        public void DownloadSize_CanBeSetAndRetrieved()
        {
            var info = new UpdateInfo { DownloadSize = 1024 * 1024 * 50 }; // 50 MB

            Assert.Equal(1024 * 1024 * 50, info.DownloadSize);
        }

        /// <summary>
        /// Verifies that the AssetName property can be set and retrieved correctly.
        /// </summary>
        [Fact]
        public void AssetName_CanBeSetAndRetrieved()
        {
            var info = new UpdateInfo { AssetName = "MyApp-v1.0.0-win-x64.zip" };

            Assert.Equal("MyApp-v1.0.0-win-x64.zip", info.AssetName);
        }

        /// <summary>
        /// Verifies that the IsPreRelease property can be set and retrieved correctly.
        /// </summary>
        [Fact]
        public void IsPreRelease_CanBeSetAndRetrieved()
        {
            var info = new UpdateInfo { IsPreRelease = true };

            Assert.True(info.IsPreRelease);
        }

        /// <summary>
        /// Verifies that the IsPreRelease property defaults to false.
        /// </summary>
        [Fact]
        public void IsPreRelease_DefaultIsFalse()
        {
            var info = new UpdateInfo();

            Assert.False(info.IsPreRelease);
        }

        /// <summary>
        /// Verifies that the ReleasePageUrl property can be set and retrieved correctly.
        /// </summary>
        [Fact]
        public void ReleasePageUrl_CanBeSetAndRetrieved()
        {
            var url = "https://github.com/owner/repo/releases/tag/v1.0.0";
            var info = new UpdateInfo { ReleasePageUrl = url };

            Assert.Equal(url, info.ReleasePageUrl);
        }

        /// <summary>
        /// Verifies that the Sha256Checksum property can be set and retrieved correctly.
        /// </summary>
        [Fact]
        public void Sha256Checksum_CanBeSetAndRetrieved()
        {
            var checksum = "abcdef1234567890abcdef1234567890abcdef1234567890abcdef1234567890";
            var info = new UpdateInfo { Sha256Checksum = checksum };

            Assert.Equal(checksum, info.Sha256Checksum);
        }

        #endregion

        #region ToString Tests

        /// <summary>
        /// Verifies that ToString returns name and version when download size is not specified.
        /// </summary>
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

        /// <summary>
        /// Verifies that ToString includes formatted download size when specified.
        /// </summary>
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

        /// <summary>
        /// Verifies that ToString formats small download sizes correctly.
        /// </summary>
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

        /// <summary>
        /// Verifies that ToString formats large download sizes correctly.
        /// </summary>
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

        /// <summary>
        /// Verifies that ToString treats negative download size as no size.
        /// </summary>
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

        /// <summary>
        /// Verifies that ToString handles null name and version gracefully.
        /// </summary>
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

        /// <summary>
        /// Verifies that a new instance has Version initialized to null.
        /// </summary>
        [Fact]
        public void NewInstance_HasNullVersion()
        {
            var info = new UpdateInfo();

            Assert.Null(info.Version);
        }

        /// <summary>
        /// Verifies that a new instance has Name initialized to null.
        /// </summary>
        [Fact]
        public void NewInstance_HasNullName()
        {
            var info = new UpdateInfo();

            Assert.Null(info.Name);
        }

        /// <summary>
        /// Verifies that a new instance has DownloadUrl initialized to null.
        /// </summary>
        [Fact]
        public void NewInstance_HasNullDownloadUrl()
        {
            var info = new UpdateInfo();

            Assert.Null(info.DownloadUrl);
        }

        /// <summary>
        /// Verifies that a new instance has DownloadSize initialized to zero.
        /// </summary>
        [Fact]
        public void NewInstance_HasZeroDownloadSize()
        {
            var info = new UpdateInfo();

            Assert.Equal(0, info.DownloadSize);
        }

        /// <summary>
        /// Verifies that a new instance has PublishedAt initialized to default DateTime value.
        /// </summary>
        [Fact]
        public void NewInstance_HasDefaultPublishedAt()
        {
            var info = new UpdateInfo();

            Assert.Equal(default(DateTime), info.PublishedAt);
        }

        #endregion
    }
}
