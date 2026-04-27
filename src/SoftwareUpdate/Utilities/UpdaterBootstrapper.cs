using System;
using System.Diagnostics;
using System.IO;

namespace SoftwareUpdate.Utilities
{
    /// <summary>
    /// Utility class for launching the external updater process.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b> Authors: </b>
    /// <list type="bullet">
    ///     <item> Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil </item>
    /// </list>
    /// </para>
    /// </remarks>
    public static class UpdaterBootstrapper
    {
        /// <summary>
        /// Launches the external updater process to perform an application update.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This method spawns the SoftwareUpdate.Updater.exe process which handles the actual file replacement.
        /// The updater will wait for the current process to exit before extracting files, then optionally
        /// restart the main application.
        /// </para>
        /// <para>
        /// If <paramref name="exitApplication"/> is true (the default), this method will call
        /// <see cref="Environment.Exit(int)"/> after launching the updater. Callers should ensure
        /// all resources are properly disposed before calling this method, or set
        /// <paramref name="exitApplication"/> to false and handle application shutdown manually.
        /// </para>
        /// </remarks>
        /// <param name="updaterPath">
        /// The full path to the updater executable (SoftwareUpdate.Updater.exe).
        /// Must be an existing file.
        /// </param>
        /// <param name="zipPath">
        /// The full path to the downloaded update zip file containing the new application files.
        /// Must be an existing file.
        /// </param>
        /// <param name="targetDirectory">
        /// The target installation directory where the update will be extracted.
        /// This is typically the application's installation folder.
        /// </param>
        /// <param name="mainExecutable">
        /// The name of the main executable file to restart after the update completes.
        /// This should be just the filename, not a full path (e.g., "MyApp.exe").
        /// </param>
        /// <param name="createBackup">
        /// If true, the updater will create a backup of the current installation before applying the update.
        /// Defaults to true.
        /// </param>
        /// <param name="exitApplication">
        /// If true, this method will call <see cref="Environment.Exit(int)"/> after launching the updater.
        /// If false, the caller is responsible for terminating the application. Defaults to true.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="updaterPath"/>, <paramref name="zipPath"/>,
        /// <paramref name="targetDirectory"/>, or <paramref name="mainExecutable"/> is null or empty.
        /// </exception>
        /// <exception cref="FileNotFoundException">
        /// Thrown when the updater executable or update zip file does not exist.
        /// </exception>
        public static void LaunchUpdater(
            string updaterPath,
            string zipPath,
            string targetDirectory,
            string mainExecutable,
            bool createBackup = true,
            bool exitApplication = true)
        {
            if (string.IsNullOrEmpty(updaterPath))
                throw new ArgumentNullException(nameof(updaterPath));
            if (string.IsNullOrEmpty(zipPath))
                throw new ArgumentNullException(nameof(zipPath));
            if (string.IsNullOrEmpty(targetDirectory))
                throw new ArgumentNullException(nameof(targetDirectory));
            if (string.IsNullOrEmpty(mainExecutable))
                throw new ArgumentNullException(nameof(mainExecutable));

            if (!File.Exists(updaterPath))
                throw new FileNotFoundException("Updater executable not found.", updaterPath);
            if (!File.Exists(zipPath))
                throw new FileNotFoundException("Update zip file not found.", zipPath);

            var currentPid = Process.GetCurrentProcess().Id;

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
            startInfo.ArgumentList.Add(zipPath);
            startInfo.ArgumentList.Add("--target");
            startInfo.ArgumentList.Add(targetDirectory);
            startInfo.ArgumentList.Add("--exe");
            startInfo.ArgumentList.Add(mainExecutable);
            if (createBackup)
                startInfo.ArgumentList.Add("--backup");

            var process = Process.Start(startInfo);
            if (process == null)
            {
                throw new InvalidOperationException("Failed to start the updater process.");
            }

            if (exitApplication)
            {
                Environment.Exit(0);
            }
        }
    }
}
