using Xunit;
using SoftwareUpdate;

namespace SoftwareUpdate.Tests.Utilities
{
    public class SemanticVersionTests
    {
        #region Constructor Tests

        [Fact]
        public void Constructor_SetsProperties()
        {
            var version = new SemanticVersion(1, 2, 3);

            Assert.Equal(1, version.Major);
            Assert.Equal(2, version.Minor);
            Assert.Equal(3, version.Patch);
            Assert.Null(version.PreRelease);
            Assert.Null(version.BuildMetadata);
        }

        [Fact]
        public void Constructor_WithPreRelease()
        {
            var version = new SemanticVersion(1, 0, 0, "alpha");

            Assert.Equal("alpha", version.PreRelease);
            Assert.True(version.IsPreRelease);
        }

        [Fact]
        public void Constructor_WithBuildMetadata()
        {
            var version = new SemanticVersion(1, 0, 0, null, "build.123");

            Assert.Equal("build.123", version.BuildMetadata);
        }

        [Fact]
        public void Constructor_NegativeMajor_ThrowsArgumentOutOfRangeException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                new SemanticVersion(-1, 0, 0));
        }

        [Fact]
        public void Constructor_NegativeMinor_ThrowsArgumentOutOfRangeException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                new SemanticVersion(1, -1, 0));
        }

        [Fact]
        public void Constructor_NegativePatch_ThrowsArgumentOutOfRangeException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                new SemanticVersion(1, 0, -1));
        }

        [Fact]
        public void Constructor_DefaultPatchIsZero()
        {
            var version = new SemanticVersion(1, 2);

            Assert.Equal(0, version.Patch);
        }

        #endregion

        #region Parse Tests

        [Theory]
        [InlineData("1.0.0", 1, 0, 0)]
        [InlineData("2.1.3", 2, 1, 3)]
        [InlineData("10.20.30", 10, 20, 30)]
        [InlineData("1.0", 1, 0, 0)]
        public void Parse_ValidVersionStrings(string input, int expectedMajor, int expectedMinor, int expectedPatch)
        {
            var version = SemanticVersion.Parse(input);

            Assert.Equal(expectedMajor, version.Major);
            Assert.Equal(expectedMinor, version.Minor);
            Assert.Equal(expectedPatch, version.Patch);
        }

        [Theory]
        [InlineData("v1.0.0", 1, 0, 0)]
        [InlineData("V2.1.3", 2, 1, 3)]
        public void Parse_WithVPrefix(string input, int expectedMajor, int expectedMinor, int expectedPatch)
        {
            var version = SemanticVersion.Parse(input);

            Assert.Equal(expectedMajor, version.Major);
            Assert.Equal(expectedMinor, version.Minor);
            Assert.Equal(expectedPatch, version.Patch);
        }

        [Theory]
        [InlineData("1.0.0-alpha", "alpha")]
        [InlineData("1.0.0-beta.1", "beta.1")]
        [InlineData("1.0.0-rc.1.2", "rc.1.2")]
        public void Parse_WithPreRelease(string input, string expectedPreRelease)
        {
            var version = SemanticVersion.Parse(input);

            Assert.Equal(expectedPreRelease, version.PreRelease);
        }

        [Theory]
        [InlineData("1.0.0+build.123", "build.123")]
        [InlineData("1.0.0+20130313144700", "20130313144700")]
        public void Parse_WithBuildMetadata(string input, string expectedBuild)
        {
            var version = SemanticVersion.Parse(input);

            Assert.Equal(expectedBuild, version.BuildMetadata);
        }

        [Fact]
        public void Parse_WithPreReleaseAndBuildMetadata()
        {
            var version = SemanticVersion.Parse("1.0.0-alpha+build.123");

            Assert.Equal("alpha", version.PreRelease);
            Assert.Equal("build.123", version.BuildMetadata);
        }

        [Fact]
        public void Parse_NullString_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => SemanticVersion.Parse(null!));
        }

        [Fact]
        public void Parse_EmptyString_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => SemanticVersion.Parse(string.Empty));
        }

        [Theory]
        [InlineData("invalid")]
        [InlineData("1")]
        [InlineData("1.0.0.0")]
        [InlineData("a.b.c")]
        public void Parse_InvalidFormat_ThrowsFormatException(string input)
        {
            Assert.Throws<FormatException>(() => SemanticVersion.Parse(input));
        }

        #endregion

        #region TryParse Tests

        [Fact]
        public void TryParse_ValidString_ReturnsTrue()
        {
            var result = SemanticVersion.TryParse("1.0.0", out var version);

            Assert.True(result);
            Assert.NotNull(version);
            Assert.Equal(1, version!.Major);
        }

        [Fact]
        public void TryParse_InvalidString_ReturnsFalse()
        {
            var result = SemanticVersion.TryParse("invalid", out var version);

            Assert.False(result);
            Assert.Null(version);
        }

        [Fact]
        public void TryParse_NullString_ReturnsFalse()
        {
            var result = SemanticVersion.TryParse(null!, out var version);

            Assert.False(result);
            Assert.Null(version);
        }

        [Fact]
        public void TryParse_EmptyString_ReturnsFalse()
        {
            var result = SemanticVersion.TryParse(string.Empty, out var version);

            Assert.False(result);
            Assert.Null(version);
        }

        #endregion

        #region FromVersion Tests

        [Fact]
        public void FromVersion_ConvertsSystemVersion()
        {
            var systemVersion = new Version(1, 2, 3);
            var semver = SemanticVersion.FromVersion(systemVersion);

            Assert.Equal(1, semver.Major);
            Assert.Equal(2, semver.Minor);
            Assert.Equal(3, semver.Patch);
        }

        [Fact]
        public void FromVersion_NullVersion_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                SemanticVersion.FromVersion(null!));
        }

        [Fact]
        public void FromVersion_HandlesNegativeBuildAsZero()
        {
            var systemVersion = new Version(1, 2);
            var semver = SemanticVersion.FromVersion(systemVersion);

            Assert.Equal(0, semver.Patch);
        }

        #endregion

        #region CompareTo Tests

        [Fact]
        public void CompareTo_NullReturnsPositive()
        {
            var version = new SemanticVersion(1, 0, 0);

            Assert.True(version.CompareTo(null) > 0);
        }

        [Fact]
        public void CompareTo_EqualVersionsReturnsZero()
        {
            var v1 = new SemanticVersion(1, 2, 3);
            var v2 = new SemanticVersion(1, 2, 3);

            Assert.Equal(0, v1.CompareTo(v2));
        }

        [Theory]
        [InlineData(2, 0, 0, 1, 0, 0)]
        [InlineData(1, 2, 0, 1, 1, 0)]
        [InlineData(1, 0, 2, 1, 0, 1)]
        public void CompareTo_GreaterVersionReturnsPositive(int m1, int mi1, int p1, int m2, int mi2, int p2)
        {
            var v1 = new SemanticVersion(m1, mi1, p1);
            var v2 = new SemanticVersion(m2, mi2, p2);

            Assert.True(v1.CompareTo(v2) > 0);
        }

        [Theory]
        [InlineData(1, 0, 0, 2, 0, 0)]
        [InlineData(1, 1, 0, 1, 2, 0)]
        [InlineData(1, 0, 1, 1, 0, 2)]
        public void CompareTo_LesserVersionReturnsNegative(int m1, int mi1, int p1, int m2, int mi2, int p2)
        {
            var v1 = new SemanticVersion(m1, mi1, p1);
            var v2 = new SemanticVersion(m2, mi2, p2);

            Assert.True(v1.CompareTo(v2) < 0);
        }

        [Fact]
        public void CompareTo_PreReleaseIsLessThanRelease()
        {
            var prerelease = new SemanticVersion(1, 0, 0, "alpha");
            var release = new SemanticVersion(1, 0, 0);

            Assert.True(prerelease.CompareTo(release) < 0);
            Assert.True(release.CompareTo(prerelease) > 0);
        }

        [Theory]
        [InlineData("1.0.0-alpha", "1.0.0-beta")]
        [InlineData("1.0.0-alpha.1", "1.0.0-alpha.2")]
        [InlineData("1.0.0-1", "1.0.0-2")]
        public void CompareTo_PreReleaseComparison(string less, string greater)
        {
            var v1 = SemanticVersion.Parse(less);
            var v2 = SemanticVersion.Parse(greater);

            Assert.True(v1.CompareTo(v2) < 0);
        }

        [Fact]
        public void CompareTo_NumericPreReleaseComesBeforeAlphanumeric()
        {
            var numeric = SemanticVersion.Parse("1.0.0-1");
            var alpha = SemanticVersion.Parse("1.0.0-alpha");

            Assert.True(numeric.CompareTo(alpha) < 0);
        }

        [Fact]
        public void CompareTo_ShorterPreReleaseIsLess()
        {
            var shorter = SemanticVersion.Parse("1.0.0-alpha");
            var longer = SemanticVersion.Parse("1.0.0-alpha.1");

            Assert.True(shorter.CompareTo(longer) < 0);
        }

        #endregion

        #region Equality Tests

        [Fact]
        public void Equals_SameVersionReturnsTrue()
        {
            var v1 = new SemanticVersion(1, 2, 3);
            var v2 = new SemanticVersion(1, 2, 3);

            Assert.True(v1.Equals(v2));
        }

        [Fact]
        public void Equals_DifferentVersionReturnsFalse()
        {
            var v1 = new SemanticVersion(1, 2, 3);
            var v2 = new SemanticVersion(1, 2, 4);

            Assert.False(v1.Equals(v2));
        }

        [Fact]
        public void Equals_NullReturnsFalse()
        {
            var v1 = new SemanticVersion(1, 2, 3);

            Assert.False(v1.Equals(null));
        }

        [Fact]
        public void Equals_Object_SameVersionReturnsTrue()
        {
            var v1 = new SemanticVersion(1, 2, 3);
            object v2 = new SemanticVersion(1, 2, 3);

            Assert.True(v1.Equals(v2));
        }

        [Fact]
        public void Equals_Object_NonSemanticVersionReturnsFalse()
        {
            var v1 = new SemanticVersion(1, 2, 3);

            Assert.False(v1.Equals("1.2.3"));
        }

        [Fact]
        public void GetHashCode_SameVersionsSameHashCode()
        {
            var v1 = new SemanticVersion(1, 2, 3);
            var v2 = new SemanticVersion(1, 2, 3);

            Assert.Equal(v1.GetHashCode(), v2.GetHashCode());
        }

        [Fact]
        public void GetHashCode_DifferentVersionsDifferentHashCode()
        {
            var v1 = new SemanticVersion(1, 2, 3);
            var v2 = new SemanticVersion(1, 2, 4);

            Assert.NotEqual(v1.GetHashCode(), v2.GetHashCode());
        }

        #endregion

        #region Operator Tests

        [Fact]
        public void OperatorEquals_EqualVersionsReturnsTrue()
        {
            var v1 = new SemanticVersion(1, 2, 3);
            var v2 = new SemanticVersion(1, 2, 3);

            Assert.True(v1 == v2);
        }

        [Fact]
        public void OperatorEquals_BothNullReturnsTrue()
        {
            SemanticVersion? v1 = null;
            SemanticVersion? v2 = null;

            Assert.True(v1 == v2);
        }

        [Fact]
        public void OperatorEquals_LeftNullReturnsFalse()
        {
            SemanticVersion? v1 = null;
            var v2 = new SemanticVersion(1, 2, 3);

            Assert.False(v1 == v2);
        }

        [Fact]
        public void OperatorNotEquals_DifferentVersionsReturnsTrue()
        {
            var v1 = new SemanticVersion(1, 2, 3);
            var v2 = new SemanticVersion(1, 2, 4);

            Assert.True(v1 != v2);
        }

        [Fact]
        public void OperatorLessThan_LesserVersionReturnsTrue()
        {
            var v1 = new SemanticVersion(1, 0, 0);
            var v2 = new SemanticVersion(2, 0, 0);

            Assert.True(v1 < v2);
        }

        [Fact]
        public void OperatorLessThan_LeftNullReturnsTrue()
        {
            SemanticVersion? v1 = null;
            var v2 = new SemanticVersion(1, 0, 0);

            Assert.True(v1 < v2);
        }

        [Fact]
        public void OperatorLessThan_BothNullReturnsFalse()
        {
            SemanticVersion? v1 = null;
            SemanticVersion? v2 = null;

            Assert.False(v1 < v2);
        }

        [Fact]
        public void OperatorGreaterThan_GreaterVersionReturnsTrue()
        {
            var v1 = new SemanticVersion(2, 0, 0);
            var v2 = new SemanticVersion(1, 0, 0);

            Assert.True(v1 > v2);
        }

        [Fact]
        public void OperatorGreaterThan_LeftNullReturnsFalse()
        {
            SemanticVersion? v1 = null;
            var v2 = new SemanticVersion(1, 0, 0);

            Assert.False(v1 > v2);
        }

        [Fact]
        public void OperatorLessThanOrEqual_EqualVersionsReturnsTrue()
        {
            var v1 = new SemanticVersion(1, 0, 0);
            var v2 = new SemanticVersion(1, 0, 0);

            Assert.True(v1 <= v2);
        }

        [Fact]
        public void OperatorGreaterThanOrEqual_EqualVersionsReturnsTrue()
        {
            var v1 = new SemanticVersion(1, 0, 0);
            var v2 = new SemanticVersion(1, 0, 0);

            Assert.True(v1 >= v2);
        }

        #endregion

        #region ToString Tests

        [Fact]
        public void ToString_BasicVersion()
        {
            var version = new SemanticVersion(1, 2, 3);

            Assert.Equal("1.2.3", version.ToString());
        }

        [Fact]
        public void ToString_WithPreRelease()
        {
            var version = new SemanticVersion(1, 0, 0, "alpha");

            Assert.Equal("1.0.0-alpha", version.ToString());
        }

        [Fact]
        public void ToString_WithBuildMetadata()
        {
            var version = new SemanticVersion(1, 0, 0, null, "build.123");

            Assert.Equal("1.0.0+build.123", version.ToString());
        }

        [Fact]
        public void ToString_WithPreReleaseAndBuildMetadata()
        {
            var version = new SemanticVersion(1, 0, 0, "alpha", "build.123");

            Assert.Equal("1.0.0-alpha+build.123", version.ToString());
        }

        [Fact]
        public void ToTagString_AddsVPrefix()
        {
            var version = new SemanticVersion(1, 2, 3);

            Assert.Equal("v1.2.3", version.ToTagString());
        }

        #endregion

        #region IsPreRelease Tests

        [Fact]
        public void IsPreRelease_WithPreRelease_ReturnsTrue()
        {
            var version = new SemanticVersion(1, 0, 0, "alpha");

            Assert.True(version.IsPreRelease);
        }

        [Fact]
        public void IsPreRelease_WithoutPreRelease_ReturnsFalse()
        {
            var version = new SemanticVersion(1, 0, 0);

            Assert.False(version.IsPreRelease);
        }

        [Fact]
        public void IsPreRelease_EmptyPreRelease_ReturnsFalse()
        {
            var version = new SemanticVersion(1, 0, 0, "");

            Assert.False(version.IsPreRelease);
        }

        #endregion
    }
}
