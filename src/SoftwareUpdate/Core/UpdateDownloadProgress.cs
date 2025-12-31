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

namespace SoftwareUpdate
{
    /// <summary>
    /// Represents the progress of an update download operation.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b> Authors: </b>
    /// <list type="bullet">
    ///     <item> Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil </item>
    /// </list>
    /// </para>
    /// </remarks>
    public class UpdateDownloadProgress
    {
        /// <summary>
        /// Gets or sets the number of bytes downloaded so far.
        /// </summary>
        public long BytesDownloaded { get; set; }

        /// <summary>
        /// Gets or sets the total number of bytes to download.
        /// May be -1 if the total size is unknown.
        /// </summary>
        public long TotalBytes { get; set; }

        /// <summary>
        /// Gets the download progress as a percentage (0-100).
        /// Returns -1 if total size is unknown.
        /// </summary>
        public double ProgressPercentage
        {
            get
            {
                if (TotalBytes <= 0) return -1;
                return (double)BytesDownloaded / TotalBytes * 100.0;
            }
        }

        /// <summary>
        /// Gets or sets the current download speed in bytes per second.
        /// </summary>
        public double BytesPerSecond { get; set; }

        /// <summary>
        /// Gets a formatted string of the download progress.
        /// </summary>
        public string ProgressText
        {
            get
            {
                var downloaded = FormatBytes(BytesDownloaded);
                if (TotalBytes > 0)
                {
                    var total = FormatBytes(TotalBytes);
                    return $"{downloaded} / {total} ({ProgressPercentage:F0}%)";
                }
                return downloaded;
            }
        }

        /// <summary>
        /// Gets a formatted string of the download speed.
        /// </summary>
        public string SpeedText
        {
            get
            {
                if (BytesPerSecond <= 0) return "";
                return $"{FormatBytes((long)BytesPerSecond)}/s";
            }
        }

        /// <summary>
        /// Formats a byte count into a human-readable string.
        /// </summary>
        /// <param name="bytes">The number of bytes to format.</param>
        /// <returns>A human-readable string representation (e.g., "1.5 MB").</returns>
        private static string FormatBytes(long bytes)
        {
            if (bytes < 1024) return $"{bytes} B";
            if (bytes < 1024 * 1024) return $"{bytes / 1024.0:F1} KB";
            if (bytes < 1024 * 1024 * 1024) return $"{bytes / 1024.0 / 1024.0:F1} MB";
            return $"{bytes / 1024.0 / 1024.0 / 1024.0:F2} GB";
        }
    }
}
