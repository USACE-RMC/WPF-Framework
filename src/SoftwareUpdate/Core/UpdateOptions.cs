// Copyright (c) USACE. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.IO;
using System.Reflection;

namespace SoftwareUpdate
{
    /// <summary>
    /// Configuration options for the software update service.
    /// </summary>
    public class UpdateOptions
    {
        /// <summary>
        /// Gets or sets the GitHub repository owner (e.g., "USACE-RMC").
        /// </summary>
        public string GitHubOwner { get; set; }

        /// <summary>
        /// Gets or sets the GitHub repository name (e.g., "RMC-BestFit").
        /// </summary>
        public string GitHubRepo { get; set; }

        /// <summary>
        /// Gets or sets the current version of the application.
        /// </summary>
        public SemanticVersion CurrentVersion { get; set; }

        /// <summary>
        /// Gets or sets a pattern to match the release asset filename.
        /// Supports wildcards (* and ?). Default is "*.zip".
        /// </summary>
        /// <example>
        /// "RMC-BestFit.Version.*.zip"
        /// </example>
        public string AssetNamePattern { get; set; } = "*.zip";

        /// <summary>
        /// Gets or sets whether to include pre-release versions in update checks.
        /// Default is false.
        /// </summary>
        public bool IncludePreReleases { get; set; } = false;

        /// <summary>
        /// Gets or sets an optional GitHub personal access token for authenticated requests.
        /// Useful for private repositories or to avoid rate limiting.
        /// </summary>
        public string GitHubToken { get; set; }

        /// <summary>
        /// Gets or sets the directory where the application is installed.
        /// If not specified, uses the directory containing the main executable.
        /// </summary>
        public string InstallDirectory { get; set; }

        /// <summary>
        /// Gets or sets the name of the main executable to restart after update.
        /// If not specified, uses the current process executable name.
        /// </summary>
        public string MainExecutableName { get; set; }

        /// <summary>
        /// Gets or sets whether to create a backup before updating.
        /// Default is true.
        /// </summary>
        public bool CreateBackup { get; set; } = true;

        /// <summary>
        /// Gets or sets the path to store skipped version preferences.
        /// Default is in the application's local app data folder.
        /// </summary>
        public string SkippedVersionsFilePath { get; set; }

        /// <summary>
        /// Gets or sets the timeout for HTTP requests in seconds.
        /// Default is 30 seconds.
        /// </summary>
        public int RequestTimeoutSeconds { get; set; } = 30;

        /// <summary>
        /// Gets or sets the path to the external updater executable.
        /// If not specified, looks for "SoftwareUpdate.Updater.exe" in the application directory.
        /// </summary>
        public string UpdaterExecutablePath { get; set; }

        /// <summary>
        /// Gets the resolved install directory.
        /// </summary>
        public string ResolvedInstallDirectory
        {
            get
            {
                if (!string.IsNullOrEmpty(InstallDirectory))
                    return InstallDirectory;

                var assembly = Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly();
                return Path.GetDirectoryName(assembly.Location);
            }
        }

        /// <summary>
        /// Gets the resolved main executable name.
        /// </summary>
        public string ResolvedMainExecutableName
        {
            get
            {
                if (!string.IsNullOrEmpty(MainExecutableName))
                    return MainExecutableName;

                var assembly = Assembly.GetEntryAssembly();
                if (assembly != null)
                    return Path.GetFileName(assembly.Location);

                return AppDomain.CurrentDomain.FriendlyName;
            }
        }

        /// <summary>
        /// Gets the resolved updater executable path.
        /// </summary>
        public string ResolvedUpdaterPath
        {
            get
            {
                if (!string.IsNullOrEmpty(UpdaterExecutablePath))
                    return UpdaterExecutablePath;

                return Path.Combine(ResolvedInstallDirectory, "SoftwareUpdate.Updater.exe");
            }
        }

        /// <summary>
        /// Gets the resolved skipped versions file path.
        /// </summary>
        public string ResolvedSkippedVersionsPath
        {
            get
            {
                if (!string.IsNullOrEmpty(SkippedVersionsFilePath))
                    return SkippedVersionsFilePath;

                var appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                var appName = GitHubRepo ?? "SoftwareUpdate";
                return Path.Combine(appData, appName, "skipped_versions.txt");
            }
        }

        /// <summary>
        /// Validates the options and throws if invalid.
        /// </summary>
        public void Validate()
        {
            if (string.IsNullOrWhiteSpace(GitHubOwner))
                throw new ArgumentException("GitHubOwner is required.", nameof(GitHubOwner));

            if (string.IsNullOrWhiteSpace(GitHubRepo))
                throw new ArgumentException("GitHubRepo is required.", nameof(GitHubRepo));

            if (CurrentVersion == null)
                throw new ArgumentException("CurrentVersion is required.", nameof(CurrentVersion));
        }
    }
}
