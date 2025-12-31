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
    /// Represents the result of downloading a software update.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b> Authors: </b>
    /// <list type="bullet">
    ///     <item> Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil </item>
    /// </list>
    /// </para>
    /// </remarks>
    public class UpdateDownloadResult
    {
        /// <summary>
        /// Gets or sets whether the download was successful.
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Gets or sets the path to the downloaded file.
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// Gets or sets any error that occurred during download.
        /// </summary>
        public Exception Error { get; set; }

        /// <summary>
        /// Gets or sets whether the download was cancelled by the user.
        /// </summary>
        public bool WasCancelled { get; set; }

        /// <summary>
        /// Gets or sets the total bytes downloaded.
        /// </summary>
        public long BytesDownloaded { get; set; }

        /// <summary>
        /// Gets or sets the update information that was downloaded.
        /// </summary>
        public UpdateInfo Update { get; set; }

        /// <summary>
        /// Creates a successful download result.
        /// </summary>
        /// <param name="filePath">Path to the downloaded file.</param>
        /// <param name="update">The update that was downloaded.</param>
        /// <param name="bytesDownloaded">Total bytes downloaded.</param>
        /// <returns>A successful download result.</returns>
        public static UpdateDownloadResult Successful(string filePath, UpdateInfo update, long bytesDownloaded)
        {
            return new UpdateDownloadResult
            {
                Success = true,
                FilePath = filePath,
                Update = update,
                BytesDownloaded = bytesDownloaded
            };
        }

        /// <summary>
        /// Creates a failed download result.
        /// </summary>
        /// <param name="error">The exception that occurred.</param>
        /// <returns>A failed download result.</returns>
        public static UpdateDownloadResult Failed(Exception error)
        {
            return new UpdateDownloadResult
            {
                Success = false,
                Error = error
            };
        }

        /// <summary>
        /// Creates a cancelled download result.
        /// </summary>
        /// <param name="bytesDownloaded">Bytes downloaded before cancellation.</param>
        /// <returns>A cancelled download result.</returns>
        public static UpdateDownloadResult Cancelled(long bytesDownloaded = 0)
        {
            return new UpdateDownloadResult
            {
                Success = false,
                WasCancelled = true,
                BytesDownloaded = bytesDownloaded
            };
        }
    }
}
