// Copyright (c) USACE. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Text.RegularExpressions;

namespace SoftwareUpdate
{
    /// <summary>
    /// Represents a semantic version number following the SemVer 2.0 specification.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Semantic versioning uses the format MAJOR.MINOR.PATCH[-PRERELEASE][+BUILD].
    /// Examples: 1.0.0, 2.1.0-beta.1, 3.0.0-alpha+build.123
    /// </para>
    /// <para>
    /// Pre-release versions have lower precedence than normal versions.
    /// For example: 1.0.0-alpha &lt; 1.0.0-beta &lt; 1.0.0
    /// </para>
    /// </remarks>
    public class SemanticVersion : IComparable<SemanticVersion>, IEquatable<SemanticVersion>
    {
        // Regex pattern for semantic version parsing
        private static readonly Regex VersionPattern = new Regex(
            @"^v?(?<major>\d+)\.(?<minor>\d+)(?:\.(?<patch>\d+))?(?:-(?<prerelease>[0-9A-Za-z\-\.]+))?(?:\+(?<build>[0-9A-Za-z\-\.]+))?$",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        /// <summary>
        /// Gets the major version number.
        /// </summary>
        public int Major { get; }

        /// <summary>
        /// Gets the minor version number.
        /// </summary>
        public int Minor { get; }

        /// <summary>
        /// Gets the patch version number.
        /// </summary>
        public int Patch { get; }

        /// <summary>
        /// Gets the pre-release identifier (e.g., "alpha", "beta.1").
        /// </summary>
        public string PreRelease { get; }

        /// <summary>
        /// Gets the build metadata.
        /// </summary>
        public string BuildMetadata { get; }

        /// <summary>
        /// Gets whether this is a pre-release version.
        /// </summary>
        public bool IsPreRelease => !string.IsNullOrEmpty(PreRelease);

        /// <summary>
        /// Initializes a new instance of the <see cref="SemanticVersion"/> class.
        /// </summary>
        public SemanticVersion(int major, int minor, int patch = 0, string preRelease = null, string buildMetadata = null)
        {
            if (major < 0) throw new ArgumentOutOfRangeException(nameof(major));
            if (minor < 0) throw new ArgumentOutOfRangeException(nameof(minor));
            if (patch < 0) throw new ArgumentOutOfRangeException(nameof(patch));

            Major = major;
            Minor = minor;
            Patch = patch;
            PreRelease = preRelease;
            BuildMetadata = buildMetadata;
        }

        /// <summary>
        /// Parses a version string into a <see cref="SemanticVersion"/>.
        /// </summary>
        /// <param name="versionString">The version string to parse.</param>
        /// <returns>The parsed semantic version.</returns>
        /// <exception cref="FormatException">Thrown if the string is not a valid semantic version.</exception>
        public static SemanticVersion Parse(string versionString)
        {
            if (string.IsNullOrWhiteSpace(versionString))
                throw new ArgumentNullException(nameof(versionString));

            if (TryParse(versionString, out var version))
                return version;

            throw new FormatException($"'{versionString}' is not a valid semantic version.");
        }

        /// <summary>
        /// Attempts to parse a version string into a <see cref="SemanticVersion"/>.
        /// </summary>
        /// <param name="versionString">The version string to parse.</param>
        /// <param name="version">The parsed version, or null if parsing failed.</param>
        /// <returns><c>true</c> if parsing succeeded; otherwise, <c>false</c>.</returns>
        public static bool TryParse(string versionString, out SemanticVersion version)
        {
            version = null;

            if (string.IsNullOrWhiteSpace(versionString))
                return false;

            var match = VersionPattern.Match(versionString.Trim());
            if (!match.Success)
                return false;

            var major = int.Parse(match.Groups["major"].Value);
            var minor = int.Parse(match.Groups["minor"].Value);
            var patch = match.Groups["patch"].Success ? int.Parse(match.Groups["patch"].Value) : 0;
            var preRelease = match.Groups["prerelease"].Success ? match.Groups["prerelease"].Value : null;
            var build = match.Groups["build"].Success ? match.Groups["build"].Value : null;

            version = new SemanticVersion(major, minor, patch, preRelease, build);
            return true;
        }

        /// <summary>
        /// Creates a <see cref="SemanticVersion"/> from a <see cref="System.Version"/>.
        /// </summary>
        public static SemanticVersion FromVersion(Version version)
        {
            if (version == null) throw new ArgumentNullException(nameof(version));
            return new SemanticVersion(
                version.Major,
                Math.Max(0, version.Minor),
                Math.Max(0, version.Build));
        }

        /// <summary>
        /// Compares this version to another.
        /// </summary>
        public int CompareTo(SemanticVersion other)
        {
            if (other == null) return 1;

            // Compare major.minor.patch
            var result = Major.CompareTo(other.Major);
            if (result != 0) return result;

            result = Minor.CompareTo(other.Minor);
            if (result != 0) return result;

            result = Patch.CompareTo(other.Patch);
            if (result != 0) return result;

            // Pre-release versions have lower precedence
            if (IsPreRelease && !other.IsPreRelease) return -1;
            if (!IsPreRelease && other.IsPreRelease) return 1;

            // Compare pre-release identifiers
            if (IsPreRelease && other.IsPreRelease)
            {
                return ComparePreRelease(PreRelease, other.PreRelease);
            }

            return 0;
        }

        private static int ComparePreRelease(string a, string b)
        {
            var partsA = a.Split('.');
            var partsB = b.Split('.');

            var maxParts = Math.Max(partsA.Length, partsB.Length);
            for (int i = 0; i < maxParts; i++)
            {
                if (i >= partsA.Length) return -1;
                if (i >= partsB.Length) return 1;

                var partA = partsA[i];
                var partB = partsB[i];

                var aIsNum = int.TryParse(partA, out var numA);
                var bIsNum = int.TryParse(partB, out var numB);

                if (aIsNum && bIsNum)
                {
                    var cmp = numA.CompareTo(numB);
                    if (cmp != 0) return cmp;
                }
                else if (aIsNum)
                {
                    return -1; // Numeric has lower precedence
                }
                else if (bIsNum)
                {
                    return 1;
                }
                else
                {
                    var cmp = string.Compare(partA, partB, StringComparison.OrdinalIgnoreCase);
                    if (cmp != 0) return cmp;
                }
            }

            return 0;
        }

        /// <inheritdoc/>
        public bool Equals(SemanticVersion other)
        {
            if (other == null) return false;
            return CompareTo(other) == 0;
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            return Equals(obj as SemanticVersion);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            unchecked
            {
                var hash = 17;
                hash = hash * 31 + Major;
                hash = hash * 31 + Minor;
                hash = hash * 31 + Patch;
                hash = hash * 31 + (PreRelease?.ToLowerInvariant().GetHashCode() ?? 0);
                return hash;
            }
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            var result = $"{Major}.{Minor}.{Patch}";
            if (!string.IsNullOrEmpty(PreRelease))
                result += $"-{PreRelease}";
            if (!string.IsNullOrEmpty(BuildMetadata))
                result += $"+{BuildMetadata}";
            return result;
        }

        /// <summary>
        /// Returns the version string with a "v" prefix.
        /// </summary>
        public string ToTagString() => $"v{this}";

        // Operators
        public static bool operator ==(SemanticVersion left, SemanticVersion right)
        {
            if (ReferenceEquals(left, null)) return ReferenceEquals(right, null);
            return left.Equals(right);
        }

        public static bool operator !=(SemanticVersion left, SemanticVersion right) => !(left == right);

        public static bool operator <(SemanticVersion left, SemanticVersion right)
        {
            if (left == null) return right != null;
            return left.CompareTo(right) < 0;
        }

        public static bool operator >(SemanticVersion left, SemanticVersion right)
        {
            if (left == null) return false;
            return left.CompareTo(right) > 0;
        }

        public static bool operator <=(SemanticVersion left, SemanticVersion right) => !(left > right);

        public static bool operator >=(SemanticVersion left, SemanticVersion right) => !(left < right);
    }
}
