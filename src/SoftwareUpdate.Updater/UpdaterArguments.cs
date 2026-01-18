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

namespace SoftwareUpdate.Updater
{
    /// <summary>
    /// Parses and holds command-line arguments for the updater.
    /// </summary>
    internal class UpdaterArguments
    {
        /// <summary>
        /// Gets or sets the process ID of the main application to wait for.
        /// </summary>
        /// <value>The process ID. Defaults to 0 if not specified.</value>
        public int ProcessId { get; set; }

        /// <summary>
        /// Gets or sets the path to the update zip file.
        /// </summary>
        /// <value>The zip file path, or <c>null</c> if not specified. Required by <see cref="Validate"/>.</value>
        public string ZipPath { get; set; }

        /// <summary>
        /// Gets or sets the target installation directory.
        /// </summary>
        /// <value>The target directory path, or <c>null</c> if not specified. Required by <see cref="Validate"/>.</value>
        public string TargetDirectory { get; set; }

        /// <summary>
        /// Gets or sets the main executable name to restart.
        /// </summary>
        /// <value>The executable name, or <c>null</c> if not specified. Required by <see cref="Validate"/>.</value>
        public string MainExecutable { get; set; }

        /// <summary>
        /// Gets or sets whether to create a backup before updating.
        /// </summary>
        public bool CreateBackup { get; set; }

        /// <summary>
        /// Parses command-line arguments.
        /// </summary>
        /// <param name="args">The command-line arguments array.</param>
        /// <returns>Parsed updater arguments.</returns>
        public static UpdaterArguments Parse(string[] args)
        {
            var result = new UpdaterArguments();
            var argDict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            for (int i = 0; i < args.Length; i++)
            {
                var arg = args[i];

                if (arg.StartsWith("--") && arg.Length > 2)
                {
                    var key = arg.Substring(2).ToLower();

                    // Check for flags without values
                    if (key == "backup")
                    {
                        result.CreateBackup = true;
                        continue;
                    }

                    // Get the value
                    if (i + 1 < args.Length && !args[i + 1].StartsWith("--"))
                    {
                        argDict[key] = args[++i];
                    }
                }
            }

            // Parse values
            if (argDict.TryGetValue("pid", out var pidStr) && int.TryParse(pidStr, out var pid))
            {
                result.ProcessId = pid;
            }

            if (argDict.TryGetValue("zip", out var zip))
            {
                result.ZipPath = UnquoteArgument(zip);
            }

            if (argDict.TryGetValue("target", out var target))
            {
                result.TargetDirectory = UnquoteArgument(target);
            }

            if (argDict.TryGetValue("exe", out var exe))
            {
                result.MainExecutable = UnquoteArgument(exe);
            }

            return result;
        }

        /// <summary>
        /// Validates the arguments.
        /// </summary>
        /// <remarks>
        /// Note: File existence checks are point-in-time validations. The zip file or target directory
        /// could be modified or deleted between validation and actual use (TOCTOU). Callers should
        /// handle <see cref="System.IO.FileNotFoundException"/> during installation.
        /// </remarks>
        public void Validate()
        {
            var errors = new List<string>();

            if (ProcessId <= 0)
                errors.Add("--pid is required and must be a valid process ID");

            if (string.IsNullOrEmpty(ZipPath))
                errors.Add("--zip is required");
            else if (!System.IO.File.Exists(ZipPath))
                errors.Add($"Zip file not found: {ZipPath}");

            if (string.IsNullOrEmpty(TargetDirectory))
                errors.Add("--target is required");
            else if (!System.IO.Directory.Exists(TargetDirectory))
                errors.Add($"Target directory not found: {TargetDirectory}");

            if (string.IsNullOrEmpty(MainExecutable))
                errors.Add("--exe is required");

            if (errors.Count > 0)
            {
                throw new ArgumentException(string.Join(Environment.NewLine, errors));
            }
        }

        /// <summary>
        /// Properly unquotes an argument value, handling quoted strings safely.
        /// </summary>
        /// <param name="value">The argument value to unquote.</param>
        /// <returns>The unquoted value.</returns>
        private static string UnquoteArgument(string value)
        {
            if (string.IsNullOrEmpty(value))
                return value;

            // Only strip quotes if they are balanced at start and end
            if (value.Length >= 2 && value[0] == '"' && value[value.Length - 1] == '"')
            {
                return value.Substring(1, value.Length - 2);
            }

            return value;
        }
    }
}
