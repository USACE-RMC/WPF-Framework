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
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;

namespace SoftwareUpdate
{
    /// <summary>
    /// Configuration options for the software update service.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b> Authors: </b>
    /// <list type="bullet">
    ///     <item> Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil </item>
    /// </list>
    /// </para>
    /// </remarks>
    public class UpdateOptions
    {
        // GitHub username/repo format: alphanumeric, hyphens allowed (not at start/end), max 39 chars for usernames
        private static readonly Regex GitHubNamePattern = new Regex(
            @"^[a-zA-Z0-9]([a-zA-Z0-9-]{0,37}[a-zA-Z0-9])?$",
            RegexOptions.Compiled);

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

            if (!GitHubNamePattern.IsMatch(GitHubOwner))
                throw new ArgumentException(
                    "GitHubOwner must contain only alphanumeric characters and hyphens, " +
                    "cannot start or end with a hyphen, and must be 1-39 characters.",
                    nameof(GitHubOwner));

            if (string.IsNullOrWhiteSpace(GitHubRepo))
                throw new ArgumentException("GitHubRepo is required.", nameof(GitHubRepo));

            if (!GitHubNamePattern.IsMatch(GitHubRepo))
                throw new ArgumentException(
                    "GitHubRepo must contain only alphanumeric characters and hyphens, " +
                    "cannot start or end with a hyphen, and must be 1-39 characters.",
                    nameof(GitHubRepo));

            if (CurrentVersion == null)
                throw new ArgumentException("CurrentVersion is required.", nameof(CurrentVersion));
        }
    }
}
