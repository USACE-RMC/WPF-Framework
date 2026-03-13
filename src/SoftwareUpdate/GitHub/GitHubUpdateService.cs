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
using System.Text.Json;
using System.Security;
using System.Security.Cryptography;
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
        /// <summary>
        /// Maximum number of retry attempts for transient network failures.
        /// </summary>
        private const int MaxRetryAttempts = 3;

        /// <summary>
        /// Base delay in milliseconds between retry attempts.
        /// </summary>
        private const int RetryDelayMs = 500;

        /// <summary>
        /// Maximum allowed download size in bytes (500 MB) to prevent denial-of-service
        /// from a compromised server sending an infinite response body.
        /// </summary>
        private const long MaxDownloadSizeBytes = 500L * 1024 * 1024;

        private readonly HttpClient _httpClient;
        private readonly object _lock = new object();
        private UpdateState _state = UpdateState.Idle;
        private UpdateInfo? _availableUpdate;
        private HashSet<string> _skippedVersions = null!; // Assigned by LoadSkippedVersions() in constructor
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
            _httpClient.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue(options.GitHubRepo ?? "", options.CurrentVersion?.ToString() ?? "1.0.0"));
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
        public UpdateInfo? AvailableUpdate
        {
            get { lock (_lock) return _availableUpdate; }
            private set { lock (_lock) _availableUpdate = value; }
        }

        /// <inheritdoc/>
        public event EventHandler<UpdateCheckResult>? UpdateCheckCompleted;

        /// <inheritdoc/>
        public event EventHandler<Exception>? UpdateError;

        /// <inheritdoc/>
        public async Task<UpdateCheckResult> CheckForUpdateAsync(CancellationToken cancellationToken = default)
        {
            State = UpdateState.Checking;

            try
            {
                var apiUrl = $"https://api.github.com/repos/{Options.GitHubOwner}/{Options.GitHubRepo}/releases";
                var response = await GetWithRetryAsync(apiUrl, cancellationToken).ConfigureAwait(false);

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
                var releases = await DeserializeReleasesAsync(jsonStream, cancellationToken).ConfigureAwait(false);

                // Filter releases
                var validReleases = releases
                    .Where(r => !r.Draft)
                    .Where(r => Options.IncludePreReleases || !r.PreRelease)
                    .Where(r => r.TagName != null && SemanticVersion.TryParse(r.TagName, out _))
                    .ToList();

                if (!validReleases.Any())
                {
                    State = UpdateState.UpToDate;
                    var result = UpdateCheckResult.NoUpdateAvailable(Options.CurrentVersion!);
                    RaiseEvent(UpdateCheckCompleted, result);
                    return result;
                }

                // Find matching asset in latest release
                foreach (var release in validReleases.OrderByDescending(r => SemanticVersion.Parse(r.TagName!)))
                {
                    var asset = FindMatchingAsset(release);
                    if (asset == null) continue;

                    var releaseVersion = SemanticVersion.Parse(release.TagName!);

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

                        // Atomically update AvailableUpdate, IsSkipped, and State in a single lock
                        // to prevent another thread from observing an inconsistent state (e.g.,
                        // AvailableUpdate set but State still Checking).
                        bool isSkipped;
                        lock (_lock)
                        {
                            _availableUpdate = updateInfo;
                            isSkipped = _skippedVersions.Contains(releaseVersion.ToString());
                            _state = UpdateState.UpdateAvailable;
                        }

                        var result = UpdateCheckResult.UpdateAvailable(Options.CurrentVersion!, updateInfo, isSkipped);
                        RaiseEvent(UpdateCheckCompleted, result);
                        return result;
                    }
                }

                State = UpdateState.UpToDate;
                var noUpdateResult = UpdateCheckResult.NoUpdateAvailable(Options.CurrentVersion!);
                RaiseEvent(UpdateCheckCompleted, noUpdateResult);
                return noUpdateResult;
            }
            catch (HttpRequestException ex)
            {
                State = UpdateState.Error;
                RaiseEvent(UpdateError, ex);
                var result = UpdateCheckResult.Failed(Options.CurrentVersion!, ex);
                RaiseEvent(UpdateCheckCompleted, result);
                return result;
            }
            catch (JsonException ex)
            {
                State = UpdateState.Error;
                var wrappedException = new InvalidOperationException("Failed to parse GitHub API response.", ex);
                RaiseEvent(UpdateError, wrappedException);
                var result = UpdateCheckResult.Failed(Options.CurrentVersion!, wrappedException);
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
                var result = UpdateCheckResult.Failed(Options.CurrentVersion!, ex);
                RaiseEvent(UpdateCheckCompleted, result);
                return result;
            }
        }

        /// <inheritdoc/>
        public async Task<UpdateDownloadResult> DownloadUpdateAsync(
            UpdateInfo update,
            IProgress<UpdateDownloadProgress>? progress = null,
            CancellationToken cancellationToken = default)
        {
            if (update == null)
                throw new ArgumentNullException(nameof(update));

            State = UpdateState.Downloading;

            try
            {
                // Security: Enforce HTTPS on download URL to prevent MITM attacks
                if (!string.IsNullOrEmpty(update.DownloadUrl))
                {
                    var downloadUri = new Uri(update.DownloadUrl);
                    if (!downloadUri.Scheme.Equals("https", StringComparison.OrdinalIgnoreCase))
                    {
                        throw new SecurityException(
                            $"Download URL must use HTTPS. Received: {downloadUri.Scheme}");
                    }
                }

                // Create temp directory with GUID to prevent predictable path attacks
                var tempDir = Path.Combine(Path.GetTempPath(), "SoftwareUpdate", Guid.NewGuid().ToString("N"));
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

                // FileMode.Create overwrites any existing file atomically — no need
                // for a separate File.Exists/File.Delete check (avoids TOCTOU race).

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

                            // Security: Enforce maximum download size to prevent DoS
                            if (totalRead > MaxDownloadSizeBytes)
                            {
                                throw new InvalidOperationException(
                                    $"Download exceeded maximum allowed size of {MaxDownloadSizeBytes / 1024 / 1024} MB.");
                            }

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

                        // Validate SHA256 checksum if provided
                        if (!string.IsNullOrEmpty(update.Sha256Checksum))
                        {
                            fileStream.Position = 0;
                            using (var sha256 = SHA256.Create())
                            {
                                var hashBytes = sha256.ComputeHash(fileStream);
                                var actualChecksum = BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
                                if (!string.Equals(actualChecksum, update.Sha256Checksum, StringComparison.OrdinalIgnoreCase))
                                {
                                    throw new InvalidOperationException(
                                        $"Checksum validation failed. Expected: {update.Sha256Checksum}, Actual: {actualChecksum}");
                                }
                            }
                        }
                        else
                        {
                            Debug.WriteLine("[GitHubUpdateService] WARNING: No SHA256 checksum provided. Download integrity was not verified.");
                        }

                        State = UpdateState.ReadyToInstall;
                        return UpdateDownloadResult.Successful(tempFilePath, update, totalRead);
                    }
                }
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

            // Security: Validate the updater path resides inside the install directory
            var fullUpdaterPath = Path.GetFullPath(updaterPath);
            var fullInstallDir = Path.GetFullPath(Options.ResolvedInstallDirectory);
            if (!fullUpdaterPath.StartsWith(fullInstallDir + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(Path.GetDirectoryName(fullUpdaterPath), fullInstallDir, StringComparison.OrdinalIgnoreCase))
            {
                throw new SecurityException(
                    $"Updater executable must reside within the install directory. " +
                    $"Updater: '{fullUpdaterPath}', Install dir: '{fullInstallDir}'");
            }

            var currentPid = Process.GetCurrentProcess().Id;
            var targetDir = Options.ResolvedInstallDirectory;
            var mainExe = Options.ResolvedMainExecutableName;

            // Use ArgumentList for safe argument passing (no manual quoting/escaping)
            var startInfo = new ProcessStartInfo
            {
                FileName = updaterPath,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            startInfo.ArgumentList.Add("--pid");
            startInfo.ArgumentList.Add(currentPid.ToString());
            startInfo.ArgumentList.Add("--zip");
            startInfo.ArgumentList.Add(downloadedFilePath);
            startInfo.ArgumentList.Add("--target");
            startInfo.ArgumentList.Add(targetDir);
            startInfo.ArgumentList.Add("--exe");
            startInfo.ArgumentList.Add(mainExe);
            if (Options.CreateBackup)
                startInfo.ArgumentList.Add("--backup");

            var process = Process.Start(startInfo);
            if (process == null)
            {
                throw new InvalidOperationException("Failed to start the updater process.");
            }

            // Dispose resources before exiting the current application
            Dispose();
            Environment.Exit(0);
        }

        /// <inheritdoc/>
        public void SkipVersion(SemanticVersion? version)
        {
            if (version == null) return;

            lock (_lock)
            {
                _skippedVersions.Add(version.ToString());
            }
            SaveSkippedVersions();
        }

        /// <inheritdoc/>
        public bool IsVersionSkipped(SemanticVersion? version)
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
        private GitHubReleaseAsset? FindMatchingAsset(GitHubRelease release)
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

            return release.Assets.FirstOrDefault(a => a.Name != null && regex.IsMatch(a.Name));
        }

        /// <summary>
        /// Deserializes the GitHub releases JSON response.
        /// </summary>
        /// <param name="jsonStream">The JSON stream to deserialize.</param>
        /// <returns>List of GitHub releases.</returns>
        private static async Task<List<GitHubRelease>> DeserializeReleasesAsync(Stream jsonStream, CancellationToken cancellationToken)
        {
            return await JsonSerializer.DeserializeAsync<List<GitHubRelease>>(jsonStream, cancellationToken: cancellationToken).ConfigureAwait(false)
                ?? new List<GitHubRelease>();
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
        /// <remarks>
        /// Data is copied inside the lock and written outside to minimize lock contention.
        /// A temp-file-then-rename strategy is used so the file is never left half-written.
        /// </remarks>
        private void SaveSkippedVersions()
        {
            try
            {
                var filePath = Options.ResolvedSkippedVersionsPath;
                var dir = Path.GetDirectoryName(filePath);
                if (!string.IsNullOrEmpty(dir))
                    Directory.CreateDirectory(dir);

                // Copy the set while holding the lock, then do I/O outside the lock.
                string[] versionsCopy;
                lock (_lock)
                {
                    versionsCopy = new string[_skippedVersions.Count];
                    _skippedVersions.CopyTo(versionsCopy);
                }

                // Write to a temp file then atomically rename to prevent a partial write
                // from corrupting the persisted file.
                var tempPath = filePath + ".tmp";
                File.WriteAllLines(tempPath, versionsCopy);
                File.Move(tempPath, filePath, overwrite: true);
            }
            catch (Exception ex)
            {
                // Log errors saving preferences for debugging
                Debug.WriteLine($"[GitHubUpdateService] Failed to save skipped versions: {ex.Message}");
            }
        }

        /// <summary>
        /// Disposes of the HTTP client and releases managed resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="GitHubUpdateService"/> and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _httpClient?.Dispose();
                }
                _disposed = true;
            }
        }

        /// <summary>
        /// Safely raises an event, catching any exceptions from subscribers.
        /// </summary>
        /// <typeparam name="T">The event argument type.</typeparam>
        /// <param name="handler">The event handler to invoke.</param>
        /// <param name="args">The event arguments.</param>
        private void RaiseEvent<T>(EventHandler<T>? handler, T args)
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

        /// <summary>
        /// Determines whether an HTTP status code is transient and worth retrying.
        /// Only 5xx server errors and 429 (Too Many Requests) are retried; other 4xx
        /// client errors are permanent and should not be retried.
        /// </summary>
        /// <param name="statusCode">The HTTP status code to check.</param>
        /// <returns><c>true</c> if the request should be retried; otherwise, <c>false</c>.</returns>
        private static bool IsTransientStatusCode(HttpStatusCode statusCode)
        {
            return statusCode == HttpStatusCode.TooManyRequests ||
                   ((int)statusCode >= 500 && (int)statusCode < 600);
        }

        /// <summary>
        /// Performs an HTTP GET request with retry logic for transient failures.
        /// Only retries on 5xx server errors, 429 (Too Many Requests), or network-level
        /// failures (<see cref="HttpRequestException"/> with no HTTP status code).
        /// 4xx client errors (except 429) are returned immediately without retrying.
        /// </summary>
        /// <param name="url">The URL to fetch.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The HTTP response message.</returns>
        private async Task<HttpResponseMessage> GetWithRetryAsync(string url, CancellationToken cancellationToken)
        {
            HttpRequestException? lastException = null;

            for (int attempt = 1; attempt <= MaxRetryAttempts; attempt++)
            {
                try
                {
                    var response = await _httpClient.GetAsync(url, cancellationToken).ConfigureAwait(false);

                    // Return immediately for success or non-transient errors (e.g., 4xx other than 429)
                    if (response.IsSuccessStatusCode || !IsTransientStatusCode(response.StatusCode))
                        return response;

                    // Transient HTTP error (5xx or 429) — treat like a network failure and retry
                    Debug.WriteLine($"[GitHubUpdateService] Transient HTTP {(int)response.StatusCode} (attempt {attempt}/{MaxRetryAttempts}), retrying.");

                    if (attempt == MaxRetryAttempts)
                        return response; // Return the error response after all attempts are exhausted

                    response.Dispose();
                }
                catch (HttpRequestException ex)
                {
                    // Network-level failure (no HTTP response received) — always retry
                    lastException = ex;
                    Debug.WriteLine($"[GitHubUpdateService] Network failure (attempt {attempt}/{MaxRetryAttempts}): {ex.Message}");

                    if (attempt == MaxRetryAttempts)
                        throw;
                }

                // Exponential backoff: 500ms, 1000ms, etc.
                var delay = RetryDelayMs * attempt;
                await Task.Delay(delay, cancellationToken).ConfigureAwait(false);
            }

            // Should not reach here, but satisfy the compiler
            throw lastException!;
        }
    }
}
