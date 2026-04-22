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

namespace SoftwareUpdate
{
    /// <summary>
    /// Contains information about an available software update.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b> Authors: </b>
    /// <list type="bullet">
    ///     <item> Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil </item>
    /// </list>
    /// </para>
    /// </remarks>
    public class UpdateInfo
    {
        /// <summary>
        /// Gets or sets the version of the update.
        /// </summary>
        public SemanticVersion? Version { get; set; }

        /// <summary>
        /// Gets or sets the display name of the release (e.g., "v2.0.0").
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Gets or sets the URL to download the update package.
        /// </summary>
        /// <remarks>
        /// Ideally this would be typed as <see cref="Uri"/> to enforce well-formed URLs at the
        /// type level, but it is kept as <see cref="string"/> to avoid cascading changes across
        /// all call sites that pass it directly to <see cref="System.Net.Http.HttpClient"/> and
        /// <see cref="System.Uri"/> constructors.
        /// </remarks>
        public string? DownloadUrl { get; set; }

        /// <summary>
        /// Gets or sets the release notes (typically in Markdown format).
        /// </summary>
        public string? ReleaseNotes { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the release was published.
        /// </summary>
        public DateTime PublishedAt { get; set; }

        /// <summary>
        /// Gets or sets the size of the download in bytes.
        /// </summary>
        public long DownloadSize { get; set; }

        /// <summary>
        /// Gets or sets the name of the downloadable asset file.
        /// </summary>
        public string? AssetName { get; set; }

        /// <summary>
        /// Gets or sets whether this is a pre-release version.
        /// </summary>
        public bool IsPreRelease { get; set; }

        /// <summary>
        /// Gets or sets the URL to the release page on GitHub.
        /// </summary>
        public string? ReleasePageUrl { get; set; }

        /// <summary>
        /// Gets or sets the optional SHA256 checksum for verification.
        /// </summary>
        public string? Sha256Checksum { get; set; }

        /// <summary>
        /// Returns a string representation of the update info.
        /// </summary>
        /// <returns>A formatted string showing name, version, and download size.</returns>
        public override string ToString()
        {
            if (DownloadSize <= 0)
            {
                return $"{Name} ({Version})";
            }
            return $"{Name} ({Version}) - {DownloadSize / 1024.0 / 1024.0:F1} MB";
        }
    }
}
