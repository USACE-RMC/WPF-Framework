using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows.Threading;
using Microsoft.VisualBasic;
using FrameworkInterfaces;

namespace FrameworkUI
{
    /// <summary>
    /// A utility class for compacting and optimizing the project file.
    /// </summary>
    /// <remarks>
    /// <para>
    ///     Authors:
    ///     Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil
    /// </para>
    /// </remarks>
    public class FileSizeManager
    {
        #region Constants

        /// <summary>
        /// Bytes in one terabyte (1024^4).
        /// </summary>
        private const ulong BytesPerTerabyte = 1099511627776UL;

        /// <summary>
        /// Bytes in one gigabyte (1024^3).
        /// </summary>
        private const ulong BytesPerGigabyte = 1073741824UL;

        /// <summary>
        /// Bytes in one megabyte (1024^2).
        /// </summary>
        private const ulong BytesPerMegabyte = 1048576UL;

        /// <summary>
        /// Bytes in one kilobyte (1024).
        /// </summary>
        private const ulong BytesPerKilobyte = 1024UL;

        #endregion

        #region Fields

        private static IProject _project;
        private static string _fileSizeBefore;
        private static string _fileSizeAfter;
        private static DispatcherTimer _timer;
        private static FrameworkUI.CompactProgressControl _progressControl;
        private static BackgroundWorker _backgroundWorker;

        #endregion

        #region Events

        public static event ReportProgressEventHandler ReportProgress;
        public delegate void ReportProgressEventHandler(string message);

        #endregion

        #region Public Methods

        /// <summary>
        /// Compact and optimize the project file.
        /// </summary>
        /// <param name="project">Project as IProject.</param>
        public static void CompactAndOptimizeFile(IProject project)
        {
            // Set the project file
            _project = project;
            // Get the 'before' file size text
            _fileSizeBefore = GetFileSizeText(_project.FullFileName);

            // Begin compaction
            ShellPublicVariables.CompactionInProgress = true;

            // Clean up existing timer if any
            if (_timer != null)
            {
                _timer.Stop();
                _timer.Tick -= Timer_Tick;
            }

            // Start the timer
            _timer = new DispatcherTimer() { Interval = TimeSpan.FromSeconds(0.1d) };
            _timer.Tick += Timer_Tick;
            _timer.IsEnabled = true;
            _timer.Start();

            // Clean up existing background worker if any
            if (_backgroundWorker != null)
            {
                _backgroundWorker.DoWork -= BackgroundWorker_Dowork;
                _backgroundWorker.ProgressChanged -= BackgroundWorker_ProgressChanged;
                _backgroundWorker.RunWorkerCompleted -= BackgroundWorker_WorkerComplete;
                _backgroundWorker.Dispose();
            }

            // Create compact background worker
            _backgroundWorker = new BackgroundWorker();
            _backgroundWorker.WorkerSupportsCancellation = true;
            _backgroundWorker.WorkerReportsProgress = true;
            _backgroundWorker.DoWork += BackgroundWorker_Dowork;
            _backgroundWorker.ProgressChanged += BackgroundWorker_ProgressChanged;
            _backgroundWorker.RunWorkerCompleted += BackgroundWorker_WorkerComplete;
            _backgroundWorker.RunWorkerAsync();

            // Open the progress bar control
            _progressControl = new FrameworkUI.CompactProgressControl();
            _progressControl.ProgressText.Text = "Analyzing Project File...";
            _progressControl.ProgressBar.IsIndeterminate = true;
            _progressControl.ProgressBar.Minimum = 0d;
            _progressControl.ProgressBar.Maximum = 100d;
            _progressControl.ProgressBar.Value = 0d;
            _progressControl.ShowDialog();
        }

        /// <summary>
        /// Background worker for performing file compaction.
        /// </summary>
        private static void BackgroundWorker_Dowork(object sender, DoWorkEventArgs e)
        {
            // Sleep for 1 second to give appearance that the compaction is doing some work
            Thread.Sleep(1000);
            // Update progress bar to compacting
            ((BackgroundWorker)sender).ReportProgress(0);
            _project.Compact();
            Thread.Sleep(1000);
            // 
            // update progress bar to optimizing
            ((BackgroundWorker)sender).ReportProgress(100);
            // Sleep for 3 seconds to give appearance that the optimizing is doing some work
            Thread.Sleep(3000);
            _project.Optimize();
        }

        /// <summary>
        /// Background worker progress changed.
        /// </summary>
        private static void BackgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            // This gives the perception that the optimization is taking some time and actually doing something meaningful.
            if (e.ProgressPercentage == 0)
            {
                _progressControl.ProgressText.Text = "Compacting...";
                _progressControl.ProgressBar.Value = 0d;
                _progressControl.ProgressBar.IsIndeterminate = false;
                _progressControl.UpdateLayout();
            }
            else if (e.ProgressPercentage == 100)
            {
                _progressControl.ProgressText.Text = "Optimizing...";
                _progressControl.ProgressBar.Value = 100d;
                _progressControl.ProgressBar.IsIndeterminate = true;
                _progressControl.UpdateLayout();
            }
        }

        /// <summary>
        /// Report status when the compression worker has completed.
        /// </summary>
        private static void BackgroundWorker_WorkerComplete(object sender, RunWorkerCompletedEventArgs e)
        {
            // Stop and clean up timer
            if (_timer != null)
            {
                _timer.Stop();
                _timer.IsEnabled = false;
                _timer.Tick -= Timer_Tick;
            }

            // Stop and clean up worker
            if (_backgroundWorker != null)
            {
                _backgroundWorker.CancelAsync();
                _backgroundWorker.DoWork -= BackgroundWorker_Dowork;
                _backgroundWorker.ProgressChanged -= BackgroundWorker_ProgressChanged;
                _backgroundWorker.RunWorkerCompleted -= BackgroundWorker_WorkerComplete;
                _backgroundWorker.Dispose();
                _backgroundWorker = null;
            }

            _progressControl?.Close();
            ShellPublicVariables.CompactionInProgress = false;

            // Get new file size
            _fileSizeAfter = GetFileSizeText(_project.FullFileName);

            // Report final compression
            string message;
            if (_fileSizeBefore == _fileSizeAfter)
            {
                message = "The file was optimized. However, there was no unused space to compact in '" + _project.Name + "', so the file size remains unchanged at " + _fileSizeAfter + ".";
            }
            else
            {
                message = "The file was compacted and optimized. The project '" + _project.Name + "' was compacted from " + _fileSizeBefore + " to " + _fileSizeAfter + ".";
            }

            ReportProgress?.Invoke(message);
        }

        /// <summary>
        /// Timer used to update the progress bar.
        /// </summary>
        private static void Timer_Tick(object sender, EventArgs e)
        {
            // Get the temp journal file name
            string tempFilename = _project.FullFileName + "-journal";

            try
            {
                // Look for temp journal file
                if (!File.Exists(tempFilename)) return;

                // Update text and progress bar
                long tempFileSize = GetFileSize(tempFilename);
                long originalFileSize = GetFileSize(_project.FullFileName);

                string tempFileSizeText = (tempFileSize < originalFileSize)
                    ? FormatBytes((ulong)tempFileSize)
                    : _fileSizeBefore;

                _progressControl.ProgressBar.IsIndeterminate = false;
                _progressControl.ProgressText.Text = "Compacting " + tempFileSizeText + " of " + _fileSizeBefore;
                if (originalFileSize > 0)
                {
                    _progressControl.ProgressBar.Value = (1d - (originalFileSize - tempFileSize) / (double)originalFileSize) * _progressControl.ProgressBar.Maximum;
                }
            }
            catch (IOException)
            {
                // File may have been deleted or is in use between exists check and read - ignore
            }
            catch (UnauthorizedAccessException)
            {
                // File access denied - ignore
            }
        }

        /// <summary>
        /// Gets the file size of the project file. Returns a long.
        /// </summary>
        /// <param name="fileName">The full file name.</param>
        public static long GetFileSize(string fileName)
        {
            return new FileInfo(fileName).Length; 
        }

        /// <summary>
        /// Gets the file size of the file in a standard text format; e.g., 12.14 MB.
        /// </summary>
        /// <param name="fileName">The full file name.</param>
        public static string GetFileSizeText(string fileName)
        {
            return FormatBytes((ulong)GetFileSize(fileName));
        }

        /// <summary>
        /// Formats bytes as a human-readable string with appropriate unit (TB, GB, MB, KB, or bytes).
        /// </summary>
        /// <param name="bytes">File size in bytes.</param>
        /// <returns>A formatted string representing the file size (e.g., "1.50 GB").</returns>
        public static string FormatBytes(ulong bytes)
        {
            try
            {
                double value;
                string unit;

                if (bytes >= BytesPerTerabyte)
                {
                    value = bytes / (double)BytesPerTerabyte;
                    unit = "TB";
                }
                else if (bytes >= BytesPerGigabyte)
                {
                    value = bytes / (double)BytesPerGigabyte;
                    unit = "GB";
                }
                else if (bytes >= BytesPerMegabyte)
                {
                    value = bytes / (double)BytesPerMegabyte;
                    unit = "MB";
                }
                else if (bytes >= BytesPerKilobyte)
                {
                    value = bytes / (double)BytesPerKilobyte;
                    unit = "KB";
                }
                else
                {
                    value = bytes;
                    unit = "bytes";
                }

                return Strings.FormatNumber(value, 2) + " " + unit;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"FileSizeManager.FormatBytes failed for value {bytes}: {ex.Message}");
                return "0 bytes";
            }
        }

        #endregion
    }
}
