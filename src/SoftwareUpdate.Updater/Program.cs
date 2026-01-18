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

        /// <summary>
        /// Entry point for the updater application.
        /// </summary>
        /// <param name="args">Command-line arguments specifying the update configuration.</param>
        /// <returns>0 on success, 1 on failure.</returns>
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
        /// <param name="targetDirectory">The target installation directory.</param>
        private static void SetupLogging(string targetDirectory)
        {
            try
            {
                var logDir = Path.Combine(targetDirectory, "logs");
                Directory.CreateDirectory(logDir);

                var timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                _logFilePath = Path.Combine(logDir, $"update_{timestamp}.log");
            }
            catch (Exception ex)
            {
                // Fall back to temp directory
                _logFilePath = Path.Combine(Path.GetTempPath(), $"update_{Guid.NewGuid():N}.log");
                Console.WriteLine($"Warning: Could not create log in target directory: {ex.Message}");
            }
        }

        /// <summary>
        /// Logs a message to console and file.
        /// </summary>
        /// <param name="message">The message to log.</param>
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
            catch (Exception ex)
            {
                // Log to console if file logging fails
                Console.WriteLine($"Warning: Could not write to log file: {ex.Message}");
            }
        }

        /// <summary>
        /// Waits for a key press with timeout.
        /// </summary>
        /// <param name="milliseconds">The timeout in milliseconds.</param>
        /// <returns>True if a key was pressed, false if timeout occurred.</returns>
        private static bool WaitForKeyWithTimeout(int milliseconds)
        {
            var startTime = DateTime.Now;
            while ((DateTime.Now - startTime).TotalMilliseconds < milliseconds)
            {
                try
                {
                    if (Console.KeyAvailable)
                    {
                        Console.ReadKey(true);
                        return true;
                    }
                }
                catch (InvalidOperationException)
                {
                    // Console input is not available (e.g., redirected input stream)
                    return false;
                }
                Thread.Sleep(100);
            }
            return false;
        }
    }
}
