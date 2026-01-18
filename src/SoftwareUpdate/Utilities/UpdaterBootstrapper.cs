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
using System.Diagnostics;
using System.IO;
using System.Text;

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
