// Copyright (c) USACE. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace SoftwareUpdate
{
    /// <summary>
    /// Represents the progress of an update download operation.
    /// </summary>
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
        private static string FormatBytes(long bytes)
        {
            if (bytes < 1024) return $"{bytes} B";
            if (bytes < 1024 * 1024) return $"{bytes / 1024.0:F1} KB";
            if (bytes < 1024 * 1024 * 1024) return $"{bytes / 1024.0 / 1024.0:F1} MB";
            return $"{bytes / 1024.0 / 1024.0 / 1024.0:F2} GB";
        }
    }
}
