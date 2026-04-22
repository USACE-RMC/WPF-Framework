/*
* NOTICE:
* The U.S. Army Corps of Engineers, Risk Management Center (USACE-RMC) makes no guarantees about
* the results, or appropriateness of outputs, obtained from this software.
*
* LIST OF CONDITIONS:
* Redistribution and use in source and binary forms, with or without modification, are permitted
* provided that the following conditions are met:
* ● Redistributions of source code must retain the above notice, this list of conditions, and the
* following disclaimer.
* ● Redistributions in binary form must reproduce the above notice, this list of conditions, and
* the following disclaimer in the documentation and/or other materials provided with the distribution.
* ● The names of the U.S. Government, the U.S. Army Corps of Engineers, the Institute for Water
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

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.Json.Serialization;

namespace SoftwareUpdate.GitHub
{
    /// <summary>
    /// Represents a GitHub release from the GitHub API.
    /// </summary>
    internal class GitHubRelease
    {
        /// <summary>
        /// Gets or sets the unique identifier of the release.
        /// </summary>
        [JsonPropertyName("id")]
        public long Id { get; set; }

        /// <summary>
        /// Gets or sets the Git tag name for this release (e.g., "v1.0.0").
        /// </summary>
        [JsonPropertyName("tag_name")]
        public string? TagName { get; set; }

        /// <summary>
        /// Gets or sets the display name of the release.
        /// </summary>
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        /// <summary>
        /// Gets or sets the release notes body in Markdown format.
        /// </summary>
        [JsonPropertyName("body")]
        public string? Body { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this is a draft release.
        /// </summary>
        [JsonPropertyName("draft")]
        public bool Draft { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this is a pre-release version.
        /// </summary>
        [JsonPropertyName("prerelease")]
        public bool PreRelease { get; set; }

        /// <summary>
        /// Gets or sets the ISO 8601 timestamp when the release was published.
        /// </summary>
        [JsonPropertyName("published_at")]
        public string? PublishedAt { get; set; }

        /// <summary>
        /// Gets or sets the URL to the release page on GitHub.
        /// </summary>
        [JsonPropertyName("html_url")]
        public string? HtmlUrl { get; set; }

        /// <summary>
        /// Gets or sets the list of downloadable assets attached to this release.
        /// </summary>
        [JsonPropertyName("assets")]
        public List<GitHubReleaseAsset> Assets { get; set; } = new List<GitHubReleaseAsset>();

        /// <summary>
        /// Gets the published date as a DateTime.
        /// </summary>
        /// <returns>The published date, or <see cref="DateTime.MinValue"/> if parsing fails.</returns>
        public DateTime GetPublishedDateTime()
        {
            if (string.IsNullOrEmpty(PublishedAt))
                return DateTime.MinValue;

            // GitHub returns ISO 8601 timestamps, e.g. "2024-01-15T12:34:56Z"
            // TryParseExact is more precise and avoids locale-dependent parsing ambiguity.
            var iso8601Formats = new[]
            {
                "yyyy-MM-ddTHH:mm:ssZ",
                "yyyy-MM-ddTHH:mm:sszzz",
                "yyyy-MM-ddTHH:mm:ss.fffZ",
                "yyyy-MM-ddTHH:mm:ss.fffzzz"
            };

            if (DateTime.TryParseExact(PublishedAt, iso8601Formats,
                CultureInfo.InvariantCulture,
                DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal,
                out var date))
            {
                return date;
            }

            return DateTime.MinValue;
        }
    }
}
