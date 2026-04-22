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

namespace SoftwareUpdate
{
    /// <summary>
    /// Represents the result of checking for software updates.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b> Authors: </b>
    /// <list type="bullet">
    ///     <item> Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil </item>
    /// </list>
    /// </para>
    /// </remarks>
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
        public UpdateInfo? Update { get; set; }

        /// <summary>
        /// Gets or sets the current version of the application.
        /// </summary>
        public SemanticVersion? CurrentVersion { get; set; }

        /// <summary>
        /// Gets or sets any error that occurred during the check.
        /// </summary>
        public Exception? Error { get; set; }

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
        /// <returns>An UpdateCheckResult indicating no update is available.</returns>
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
        /// <returns>An UpdateCheckResult indicating an update is available.</returns>
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
        /// <returns>A failed UpdateCheckResult.</returns>
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
