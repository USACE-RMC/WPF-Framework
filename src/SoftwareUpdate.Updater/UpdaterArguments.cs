// Copyright (c) USACE. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

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
        public int ProcessId { get; set; }

        /// <summary>
        /// Gets or sets the path to the update zip file.
        /// </summary>
        public string ZipPath { get; set; }

        /// <summary>
        /// Gets or sets the target installation directory.
        /// </summary>
        public string TargetDirectory { get; set; }

        /// <summary>
        /// Gets or sets the main executable name to restart.
        /// </summary>
        public string MainExecutable { get; set; }

        /// <summary>
        /// Gets or sets whether to create a backup before updating.
        /// </summary>
        public bool CreateBackup { get; set; }

        /// <summary>
        /// Parses command-line arguments.
        /// </summary>
        public static UpdaterArguments Parse(string[] args)
        {
            var result = new UpdaterArguments();
            var argDict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            for (int i = 0; i < args.Length; i++)
            {
                var arg = args[i];

                if (arg.StartsWith("--"))
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
                result.ZipPath = zip.Trim('"');
            }

            if (argDict.TryGetValue("target", out var target))
            {
                result.TargetDirectory = target.Trim('"');
            }

            if (argDict.TryGetValue("exe", out var exe))
            {
                result.MainExecutable = exe.Trim('"');
            }

            return result;
        }

        /// <summary>
        /// Validates the arguments.
        /// </summary>
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
    }
}
