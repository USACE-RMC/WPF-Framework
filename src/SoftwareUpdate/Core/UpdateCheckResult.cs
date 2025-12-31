// Copyright (c) USACE. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace SoftwareUpdate
{
    /// <summary>
    /// Represents the result of checking for software updates.
    /// </summary>
    public class UpdateCheckResult
    {
        /// <summary>
        /// Gets or sets whether an update is available.
        /// </summary>
        public bool IsUpdateAvailable { get; set; }

        /// <summary>
        /// Gets or sets the available update information.
        /// Null if no update is available or if an error occurred.
        /// </summary>
        public UpdateInfo Update { get; set; }

        /// <summary>
        /// Gets or sets the current version of the application.
        /// </summary>
        public SemanticVersion CurrentVersion { get; set; }

        /// <summary>
        /// Gets or sets any error that occurred during the check.
        /// </summary>
        public Exception Error { get; set; }

        /// <summary>
        /// Gets whether the update check completed successfully (no errors).
        /// </summary>
        public bool Success => Error == null;

        /// <summary>
        /// Gets or sets whether the available update version has been skipped by the user.
        /// </summary>
        public bool IsSkippedVersion { get; set; }

        /// <summary>
        /// Creates a successful result indicating no update is available.
        /// </summary>
        /// <param name="currentVersion">The current application version.</param>
        public static UpdateCheckResult NoUpdateAvailable(SemanticVersion currentVersion)
        {
            return new UpdateCheckResult
            {
                IsUpdateAvailable = false,
                CurrentVersion = currentVersion
            };
        }

        /// <summary>
        /// Creates a successful result with an available update.
        /// </summary>
        /// <param name="currentVersion">The current application version.</param>
        /// <param name="update">The available update information.</param>
        /// <param name="isSkipped">Whether this version was skipped by the user.</param>
        public static UpdateCheckResult UpdateAvailable(SemanticVersion currentVersion, UpdateInfo update, bool isSkipped = false)
        {
            return new UpdateCheckResult
            {
                IsUpdateAvailable = true,
                CurrentVersion = currentVersion,
                Update = update,
                IsSkippedVersion = isSkipped
            };
        }

        /// <summary>
        /// Creates a failed result with an error.
        /// </summary>
        /// <param name="currentVersion">The current application version.</param>
        /// <param name="error">The exception that occurred.</param>
        public static UpdateCheckResult Failed(SemanticVersion currentVersion, Exception error)
        {
            return new UpdateCheckResult
            {
                IsUpdateAvailable = false,
                CurrentVersion = currentVersion,
                Error = error
            };
        }
    }
}
