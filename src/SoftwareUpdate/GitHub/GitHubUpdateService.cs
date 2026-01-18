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
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using SoftwareUpdate.Utilities;

namespace SoftwareUpdate.GitHub
{
    /// <summary>
    /// Implementation of <see cref="IUpdateService"/> that checks for updates on GitHub Releases.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b> Authors: </b>
    /// <list type="bullet">
    ///     <item> Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil </item>
    /// </list>
    /// </para>
    /// </remarks>
    public class GitHubUpdateService : IUpdateService, IDisposable
    {
        private readonly HttpClient _httpClient;
        private readonly object _lock = new object();
        private UpdateState _state = UpdateState.Idle;
        private UpdateInfo _availableUpdate;
        private HashSet<string> _skippedVersions;
        private bool _disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="GitHubUpdateService"/> class.
        /// </summary>
        /// <param name="options">The update options.</param>
        public GitHubUpdateService(UpdateOptions options)
        {
            Options = options ?? throw new ArgumentNullException(nameof(options));
            Options.Validate();

            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.github.v3+json"));
            _httpClient.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue(options.GitHubRepo, options.CurrentVersion?.ToString() ?? "1.0.0"));
            _httpClient.Timeout = TimeSpan.FromSeconds(options.RequestTimeoutSeconds);

            if (!string.IsNullOrEmpty(options.GitHubToken))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", options.GitHubToken);
            }

            LoadSkippedVersions();
        }

        /// <inheritdoc/>
        public UpdateOptions Options { get; }

        /// <inheritdoc/>
        public UpdateState State
        {
            get { lock (_lock) return _state; }
            private set { lock (_lock) _state = value; }
        }

        /// <inheritdoc/>
        public UpdateInfo AvailableUpdate
        {
            get { lock (_lock) return _availableUpdate; }
            private set { lock (_lock) _availableUpdate = value; }
        }

        /// <inheritdoc/>
        public event EventHandler<UpdateCheckResult> UpdateCheckCompleted;

        /// <inheritdoc/>
        public event EventHandler<Exception> UpdateError;

        /// <inheritdoc/>
        public async Task<UpdateCheckResult> CheckForUpdateAsync(CancellationToken cancellationToken = default)
        {
            State = UpdateState.Checking;

            try
            {
                var apiUrl = $"https://api.github.com/repos/{Options.GitHubOwner}/{Options.GitHubRepo}/releases";
                var response = await _httpClient.GetAsync(apiUrl, cancellationToken).ConfigureAwait(false);

                // Check for rate limiting before calling EnsureSuccessStatusCode
                if (response.StatusCode == HttpStatusCode.Forbidden)
                {
                    var message = "GitHub API rate limit exceeded. ";
                    if (string.IsNullOrEmpty(Options.GitHubToken))
                    {
                        message += "Consider providing a GitHubToken to increase the rate limit.";
                    }
                    else
                    {
                        message += "Please wait before making additional requests.";
                    }
                    throw new HttpRequestException(message);
                }

                response.EnsureSuccessStatusCode();

                var jsonStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
                var releases = DeserializeReleases(jsonStream);

                // Filter releases
                var validReleases = releases
                    .Where(r => !r.Draft)
                    .Where(r => Options.IncludePreReleases || !r.PreRelease)
                    .Where(r => SemanticVersion.TryParse(r.TagName, out _))
                    .ToList();

                if (!validReleases.Any())
                {
                    State = UpdateState.UpToDate;
                    var result = UpdateCheckResult.NoUpdateAvailable(Options.CurrentVersion);
                    RaiseEvent(UpdateCheckCompleted, result);
                    return result;
                }

                // Find matching asset in latest release
                foreach (var release in validReleases.OrderByDescending(r => SemanticVersion.Parse(r.TagName)))
                {
                    var asset = FindMatchingAsset(release);
                    if (asset == null) continue;

                    var releaseVersion = SemanticVersion.Parse(release.TagName);

                    if (releaseVersion > Options.CurrentVersion)
                    {
                        var updateInfo = new UpdateInfo
                        {
                            Version = releaseVersion,
                            Name = release.Name ?? release.TagName,
                            DownloadUrl = asset.BrowserDownloadUrl,
                            DownloadSize = asset.Size,
                            AssetName = asset.Name,
                            ReleaseNotes = release.Body,
                            PublishedAt = release.GetPublishedDateTime(),
                            IsPreRelease = release.PreRelease,
                            ReleasePageUrl = release.HtmlUrl
                        };

                        AvailableUpdate = updateInfo;
                        var isSkipped = IsVersionSkipped(releaseVersion);
                        State = UpdateState.UpdateAvailable;

                        var result = UpdateCheckResult.UpdateAvailable(Options.CurrentVersion, updateInfo, isSkipped);
                        RaiseEvent(UpdateCheckCompleted, result);
                        return result;
                    }
                }

                State = UpdateState.UpToDate;
                var noUpdateResult = UpdateCheckResult.NoUpdateAvailable(Options.CurrentVersion);
                RaiseEvent(UpdateCheckCompleted, noUpdateResult);
                return noUpdateResult;
            }
            catch (HttpRequestException ex)
            {
                State = UpdateState.Error;
                RaiseEvent(UpdateError, ex);
                var result = UpdateCheckResult.Failed(Options.CurrentVersion, ex);
                RaiseEvent(UpdateCheckCompleted, result);
                return result;
            }
            catch (SerializationException ex)
            {
                State = UpdateState.Error;
                var wrappedException = new InvalidOperationException("Failed to parse GitHub API response.", ex);
                RaiseEvent(UpdateError, wrappedException);
                var result = UpdateCheckResult.Failed(Options.CurrentVersion, wrappedException);
                RaiseEvent(UpdateCheckCompleted, result);
                return result;
            }
            catch (OperationCanceledException)
            {
                State = UpdateState.Idle;
                throw;
            }
            catch (Exception ex)
            {
                State = UpdateState.Error;
                RaiseEvent(UpdateError, ex);
                var result = UpdateCheckResult.Failed(Options.CurrentVersion, ex);
                RaiseEvent(UpdateCheckCompleted, result);
                return result;
            }
        }

        /// <inheritdoc/>
        public async Task<UpdateDownloadResult> DownloadUpdateAsync(
            UpdateInfo update,
            IProgress<UpdateDownloadProgress> progress = null,
            CancellationToken cancellationToken = default)
        {
            if (update == null)
                throw new ArgumentNullException(nameof(update));

            State = UpdateState.Downloading;

            try
            {
                // Create temp directory for download
                var tempDir = Path.Combine(Path.GetTempPath(), "SoftwareUpdate", Options.GitHubRepo);
                Directory.CreateDirectory(tempDir);

                // Validate asset name to prevent path traversal attacks
                var safeAssetName = Path.GetFileName(update.AssetName);
                if (string.IsNullOrEmpty(safeAssetName) ||
                    safeAssetName.Contains("..") ||
                    safeAssetName != update.AssetName)
                {
                    throw new ArgumentException($"Invalid asset name: {update.AssetName}", nameof(update));
                }

                var tempFilePath = Path.Combine(tempDir, safeAssetName);

                // Delete existing file if present
                if (File.Exists(tempFilePath))
                    File.Delete(tempFilePath);

                using (var response = await _httpClient.GetAsync(update.DownloadUrl, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false))
                {
                    response.EnsureSuccessStatusCode();

                    var totalBytes = response.Content.Headers.ContentLength ?? update.DownloadSize;
                    var downloadProgress = new UpdateDownloadProgress { TotalBytes = totalBytes };

                    using (var contentStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
                    using (var fileStream = new FileStream(tempFilePath, FileMode.Create, FileAccess.Write, FileShare.None, 8192, true))
                    {
                        var buffer = new byte[8192];
                        long totalRead = 0;
                        int bytesRead;
                        var stopwatch = Stopwatch.StartNew();
                        long lastReportedBytes = 0;
                        var lastReportTime = stopwatch.Elapsed;

                        while ((bytesRead = await contentStream.ReadAsync(buffer, 0, buffer.Length, cancellationToken).ConfigureAwait(false)) > 0)
                        {
                            await fileStream.WriteAsync(buffer, 0, bytesRead, cancellationToken).ConfigureAwait(false);
                            totalRead += bytesRead;

                            // Report progress
                            if (progress != null)
                            {
                                var elapsed = stopwatch.Elapsed - lastReportTime;
                                if (elapsed.TotalMilliseconds >= 100) // Report every 100ms max
                                {
                                    var bytesInInterval = totalRead - lastReportedBytes;
                                    downloadProgress.BytesDownloaded = totalRead;
                                    downloadProgress.BytesPerSecond = bytesInInterval / elapsed.TotalSeconds;
                                    progress.Report(downloadProgress);

                                    lastReportedBytes = totalRead;
                                    lastReportTime = stopwatch.Elapsed;
                                }
                            }
                        }

                        // Final progress report
                        if (progress != null)
                        {
                            downloadProgress.BytesDownloaded = totalRead;
                            progress.Report(downloadProgress);
                        }
                    }
                }

                // Validate SHA256 checksum if provided
                if (!string.IsNullOrEmpty(update.Sha256Checksum))
                {
                    var actualChecksum = ComputeSha256Checksum(tempFilePath);
                    if (!string.Equals(actualChecksum, update.Sha256Checksum, StringComparison.OrdinalIgnoreCase))
                    {
                        File.Delete(tempFilePath);
                        throw new InvalidOperationException(
                            $"Checksum validation failed. Expected: {update.Sha256Checksum}, Actual: {actualChecksum}");
                    }
                }

                State = UpdateState.ReadyToInstall;
                return UpdateDownloadResult.Successful(tempFilePath, update, update.DownloadSize);
            }
            catch (OperationCanceledException)
            {
                State = UpdateState.Idle;
                return UpdateDownloadResult.Cancelled();
            }
            catch (Exception ex)
            {
                State = UpdateState.Error;
                RaiseEvent(UpdateError, ex);
                return UpdateDownloadResult.Failed(ex);
            }
        }

        /// <inheritdoc/>
        public void InstallUpdateAndRestart(string downloadedFilePath)
        {
            if (string.IsNullOrEmpty(downloadedFilePath))
                throw new ArgumentNullException(nameof(downloadedFilePath));

            if (!File.Exists(downloadedFilePath))
                throw new FileNotFoundException("Downloaded update file not found.", downloadedFilePath);

            State = UpdateState.Installing;

            var updaterPath = Options.ResolvedUpdaterPath;
            if (!File.Exists(updaterPath))
            {
                throw new FileNotFoundException(
                    $"Updater executable not found at '{updaterPath}'. " +
                    "Ensure SoftwareUpdate.Updater.exe is deployed with your application.",
                    updaterPath);
            }

            var currentPid = Process.GetCurrentProcess().Id;
            var targetDir = Options.ResolvedInstallDirectory;
            var mainExe = Options.ResolvedMainExecutableName;

            var arguments = new StringBuilder();
            arguments.Append($"--pid {currentPid} ");
            arguments.Append($"--zip \"{downloadedFilePath}\" ");
            arguments.Append($"--target \"{targetDir}\" ");
            arguments.Append($"--exe \"{mainExe}\" ");

            if (Options.CreateBackup)
                arguments.Append("--backup ");

            var startInfo = new ProcessStartInfo
            {
                FileName = updaterPath,
                Arguments = arguments.ToString(),
                UseShellExecute = false,
                CreateNoWindow = false
            };

            Process.Start(startInfo);

            // Dispose resources before exiting the current application
            Dispose();
            Environment.Exit(0);
        }

        /// <inheritdoc/>
        public void SkipVersion(SemanticVersion version)
        {
            if (version == null) return;

            lock (_lock)
            {
                _skippedVersions.Add(version.ToString());
            }
            SaveSkippedVersions();
        }

        /// <inheritdoc/>
        public bool IsVersionSkipped(SemanticVersion version)
        {
            if (version == null) return false;

            lock (_lock)
            {
                return _skippedVersions.Contains(version.ToString());
            }
        }

        /// <inheritdoc/>
        public void ClearSkippedVersions()
        {
            lock (_lock)
            {
                _skippedVersions.Clear();
            }
            SaveSkippedVersions();
        }

        /// <summary>
        /// Finds an asset matching the configured pattern.
        /// </summary>
        /// <param name="release">The GitHub release to search.</param>
        /// <returns>The matching asset, or null if no match found.</returns>
        private GitHubReleaseAsset FindMatchingAsset(GitHubRelease release)
        {
            if (release.Assets == null || !release.Assets.Any())
                return null;

            var pattern = Options.AssetNamePattern;
            if (string.IsNullOrEmpty(pattern))
                pattern = "*.zip";

            // Convert glob pattern to regex
            var regexPattern = "^" + Regex.Escape(pattern)
                .Replace("\\*", ".*")
                .Replace("\\?", ".") + "$";

            var regex = new Regex(regexPattern, RegexOptions.IgnoreCase);

            return release.Assets.FirstOrDefault(a => regex.IsMatch(a.Name));
        }

        /// <summary>
        /// Deserializes the GitHub releases JSON response.
        /// </summary>
        /// <param name="jsonStream">The JSON stream to deserialize.</param>
        /// <returns>List of GitHub releases.</returns>
        private List<GitHubRelease> DeserializeReleases(Stream jsonStream)
        {
            var serializer = new DataContractJsonSerializer(typeof(List<GitHubRelease>));
            return (List<GitHubRelease>)serializer.ReadObject(jsonStream);
        }

        /// <summary>
        /// Computes the SHA256 checksum of a file.
        /// </summary>
        /// <param name="filePath">The path to the file.</param>
        /// <returns>The SHA256 checksum as a lowercase hexadecimal string.</returns>
        private static string ComputeSha256Checksum(string filePath)
        {
            using (var sha256 = SHA256.Create())
            using (var stream = File.OpenRead(filePath))
            {
                var hashBytes = sha256.ComputeHash(stream);
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
            }
        }

        /// <summary>
        /// Loads skipped versions from disk.
        /// </summary>
        private void LoadSkippedVersions()
        {
            _skippedVersions = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            try
            {
                var filePath = Options.ResolvedSkippedVersionsPath;
                if (File.Exists(filePath))
                {
                    var lines = File.ReadAllLines(filePath);
                    foreach (var line in lines)
                    {
                        if (!string.IsNullOrWhiteSpace(line))
                            _skippedVersions.Add(line.Trim());
                    }
                }
            }
            catch (Exception ex)
            {
                // Log errors loading preferences for debugging
                Debug.WriteLine($"[GitHubUpdateService] Failed to load skipped versions: {ex.Message}");
            }
        }

        /// <summary>
        /// Saves skipped versions to disk.
        /// </summary>
        private void SaveSkippedVersions()
        {
            try
            {
                var filePath = Options.ResolvedSkippedVersionsPath;
                var dir = Path.GetDirectoryName(filePath);
                if (!string.IsNullOrEmpty(dir))
                    Directory.CreateDirectory(dir);

                lock (_lock)
                {
                    File.WriteAllLines(filePath, _skippedVersions);
                }
            }
            catch (Exception ex)
            {
                // Log errors saving preferences for debugging
                Debug.WriteLine($"[GitHubUpdateService] Failed to save skipped versions: {ex.Message}");
            }
        }

        /// <summary>
        /// Disposes of the HTTP client.
        /// </summary>
        public void Dispose()
        {
            if (!_disposed)
            {
                _httpClient?.Dispose();
                _disposed = true;
            }
        }

        /// <summary>
        /// Safely raises an event, catching any exceptions from subscribers.
        /// </summary>
        /// <typeparam name="T">The event argument type.</typeparam>
        /// <param name="handler">The event handler to invoke.</param>
        /// <param name="args">The event arguments.</param>
        private void RaiseEvent<T>(EventHandler<T> handler, T args)
        {
            if (handler == null) return;

            try
            {
                handler(this, args);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[GitHubUpdateService] Event handler threw exception: {ex.Message}");
            }
        }
    }
}
