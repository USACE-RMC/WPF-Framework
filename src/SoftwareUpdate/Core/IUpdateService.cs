using System;
using System.Threading;
using System.Threading.Tasks;

namespace SoftwareUpdate
{
    /// <summary>
    /// Defines the contract for a software update service that can check for,
    /// download, and install application updates.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Implementations of this interface provide the core functionality for
    /// software updates, including checking remote sources for new versions,
    /// downloading update packages, and coordinating the installation process.
    /// </para>
    /// <para>
    /// The typical update flow is:
    /// <list type="number">
    ///     <item>Call <see cref="CheckForUpdateAsync"/> to check for available updates</item>
    ///     <item>If update available, call <see cref="DownloadUpdateAsync"/> to download</item>
    ///     <item>Call <see cref="InstallUpdateAndRestart"/> to apply the update</item>
    /// </list>
    /// </para>
    /// <para>
    /// <b> Authors: </b>
    /// <list type="bullet">
    ///     <item> Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil </item>
    /// </list>
    /// </para>
    /// </remarks>
    public interface IUpdateService
    {
        /// <summary>
        /// Gets the update options/configuration for this service.
        /// </summary>
        UpdateOptions Options { get; }

        /// <summary>
        /// Gets the current state of the update service.
        /// </summary>
        UpdateState State { get; }

        /// <summary>
        /// Gets the most recently checked update information, if any.
        /// </summary>
        UpdateInfo? AvailableUpdate { get; }

        /// <summary>
        /// Checks for available updates asynchronously.
        /// </summary>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result
        /// contains the <see cref="UpdateCheckResult"/> with update information.
        /// </returns>
        Task<UpdateCheckResult> CheckForUpdateAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Downloads the specified update to a temporary location.
        /// </summary>
        /// <param name="update">The update information to download.</param>
        /// <param name="progress">Optional progress reporter for download progress.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result
        /// contains the <see cref="UpdateDownloadResult"/> with the download path.
        /// </returns>
        Task<UpdateDownloadResult> DownloadUpdateAsync(
            UpdateInfo update,
            IProgress<UpdateDownloadProgress>? progress = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Installs the downloaded update and restarts the application.
        /// </summary>
        /// <param name="downloadedFilePath">Path to the downloaded update package.</param>
        /// <remarks>
        /// <para>
        /// This method will launch an external updater process and then exit
        /// the current application. The updater will:
        /// <list type="number">
        ///     <item>Wait for this application to fully exit</item>
        ///     <item>Create a backup of the current installation</item>
        ///     <item>Extract the update package</item>
        ///     <item>Restart the application</item>
        /// </list>
        /// </para>
        /// </remarks>
        void InstallUpdateAndRestart(string downloadedFilePath);

        /// <summary>
        /// Marks a specific version as skipped so the user won't be prompted again.
        /// </summary>
        /// <param name="version">The version to skip.</param>
        void SkipVersion(SemanticVersion? version);

        /// <summary>
        /// Checks if a specific version has been marked as skipped.
        /// </summary>
        /// <param name="version">The version to check.</param>
        /// <returns><c>true</c> if the version is skipped; otherwise, <c>false</c>.</returns>
        bool IsVersionSkipped(SemanticVersion? version);

        /// <summary>
        /// Clears all skipped versions.
        /// </summary>
        void ClearSkippedVersions();

        /// <summary>
        /// Occurs when an update check completes.
        /// </summary>
        event EventHandler<UpdateCheckResult>? UpdateCheckCompleted;

        /// <summary>
        /// Occurs when an error occurs during update operations.
        /// </summary>
        event EventHandler<Exception>? UpdateError;
    }
}
