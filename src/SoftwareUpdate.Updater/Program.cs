// Copyright (c) USACE. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.IO;
using System.Threading;

namespace SoftwareUpdate.Updater
{
    /// <summary>
    /// Main entry point for the external updater application.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This application is launched by the main application to perform the actual
    /// update installation. It waits for the main application to exit, then extracts
    /// the update package and restarts the application.
    /// </para>
    /// <para>
    /// Usage: SoftwareUpdate.Updater.exe --pid [pid] --zip [path] --target [dir] --exe [name] [--backup]
    /// </para>
    /// </remarks>
    internal class Program
    {
        private static string _logFilePath;

        static int Main(string[] args)
        {
            try
            {
                Console.Title = "Software Update";
                Console.WriteLine("========================================");
                Console.WriteLine("       Software Update in Progress");
                Console.WriteLine("========================================");
                Console.WriteLine();

                // Parse arguments
                var arguments = UpdaterArguments.Parse(args);

                // Set up logging
                SetupLogging(arguments.TargetDirectory);

                // Validate
                Log("Validating arguments...");
                arguments.Validate();
                Log("Arguments validated.");

                // Execute update
                var installer = new InstallationManager(arguments, Log);
                installer.Execute();

                Log("Update completed successfully!");
                Console.WriteLine();
                Console.WriteLine("Press any key to exit...");

                // Auto-close after 3 seconds if no input
                if (!WaitForKeyWithTimeout(3000))
                {
                    Console.WriteLine("Auto-closing...");
                }

                return 0;
            }
            catch (Exception ex)
            {
                Log($"FATAL ERROR: {ex.Message}");
                Log(ex.StackTrace);

                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("========================================");
                Console.WriteLine("           UPDATE FAILED");
                Console.WriteLine("========================================");
                Console.ResetColor();
                Console.WriteLine();
                Console.WriteLine($"Error: {ex.Message}");
                Console.WriteLine();
                Console.WriteLine("The application may need to be reinstalled manually.");
                Console.WriteLine("Check the log file for details.");
                Console.WriteLine();
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey(true);

                return 1;
            }
        }

        /// <summary>
        /// Sets up logging to file.
        /// </summary>
        private static void SetupLogging(string targetDirectory)
        {
            try
            {
                var logDir = Path.Combine(targetDirectory, "logs");
                Directory.CreateDirectory(logDir);

                var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                _logFilePath = Path.Combine(logDir, $"update_{timestamp}.log");
            }
            catch
            {
                // Fall back to temp directory
                _logFilePath = Path.Combine(Path.GetTempPath(), $"update_{Guid.NewGuid():N}.log");
            }
        }

        /// <summary>
        /// Logs a message to console and file.
        /// </summary>
        private static void Log(string message)
        {
            var timestamp = DateTime.Now.ToString("HH:mm:ss");
            var logLine = $"[{timestamp}] {message}";

            Console.WriteLine(logLine);

            try
            {
                if (!string.IsNullOrEmpty(_logFilePath))
                {
                    File.AppendAllText(_logFilePath, logLine + Environment.NewLine);
                }
            }
            catch
            {
                // Ignore logging errors
            }
        }

        /// <summary>
        /// Waits for a key press with timeout.
        /// </summary>
        private static bool WaitForKeyWithTimeout(int milliseconds)
        {
            var startTime = DateTime.Now;
            while ((DateTime.Now - startTime).TotalMilliseconds < milliseconds)
            {
                if (Console.KeyAvailable)
                {
                    Console.ReadKey(true);
                    return true;
                }
                Thread.Sleep(100);
            }
            return false;
        }
    }
}
