// Copyright (c) USACE. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace SoftwareUpdate.Utilities
{
    /// <summary>
    /// Utility class for launching the external updater process.
    /// </summary>
    public static class UpdaterBootstrapper
    {
        /// <summary>
        /// Launches the updater process with the specified parameters.
        /// </summary>
        /// <param name="updaterPath">Path to the updater executable.</param>
        /// <param name="zipPath">Path to the downloaded update zip file.</param>
        /// <param name="targetDirectory">Target installation directory.</param>
        /// <param name="mainExecutable">Name of the main executable to restart.</param>
        /// <param name="createBackup">Whether to create a backup before updating.</param>
        /// <param name="exitApplication">Whether to exit the current application after launching.</param>
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

            var arguments = new StringBuilder();
            arguments.Append($"--pid {currentPid} ");
            arguments.Append($"--zip \"{zipPath}\" ");
            arguments.Append($"--target \"{targetDirectory}\" ");
            arguments.Append($"--exe \"{mainExecutable}\" ");

            if (createBackup)
                arguments.Append("--backup ");

            var startInfo = new ProcessStartInfo
            {
                FileName = updaterPath,
                Arguments = arguments.ToString(),
                UseShellExecute = false,
                CreateNoWindow = false
            };

            Process.Start(startInfo);

            if (exitApplication)
            {
                Environment.Exit(0);
            }
        }
    }
}
