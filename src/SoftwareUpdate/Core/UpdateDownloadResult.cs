// Copyright (c) USACE. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace SoftwareUpdate
{
    /// <summary>
    /// Represents the result of downloading a software update.
    /// </summary>
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
