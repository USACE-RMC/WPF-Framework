using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows.Threading;
using FrameworkInterfaces;

namespace FrameworkUI
{
    /// <summary>
    /// A utility class for compacting and optimizing the project file.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b> Authors: </b>
    /// <list type="bullet">
    ///     <item> Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil </item>
    /// </list>
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

        private static IProject? _project;
        private static string? _fileSizeBefore;
        private static string? _fileSizeAfter;
        private static DispatcherTimer? _timer;
        private static FrameworkUI.CompactProgressControl? _progressControl;
        private static BackgroundWorker? _backgroundWorker;

        #endregion

        #region Events

        /// <summary>
        /// Occurs when the file size management operation reports progress or completion status.
        /// </summary>
        public static event ReportProgressEventHandler? ReportProgress;

        /// <summary>
        /// Delegate for the <see cref="ReportProgress"/> event.
        /// </summary>
        /// <param name="message">A message describing the progress or status.</param>
        public delegate void ReportProgressEventHandler(string message);

        #endregion

        #region Public Methods

        /// <summary>
        /// Compact and optimize the project file.
        /// </summary>
        /// <param name="project">The project to compact and optimize.</param>
        public static void CompactAndOptimizeFile(IProject project)
        {
            // Set the project file
            _project = project;
            // Get the 'before' file size text. Guard against a missing file (new / unsaved
            // project, file moved between open and compact) - FileInfo.Length throws
            // FileNotFoundException on an absent path, on the UI thread.
            if (!string.IsNullOrEmpty(_project.FullFileName) && File.Exists(_project.FullFileName))
            {
                _fileSizeBefore = GetFileSizeText(_project.FullFileName);
            }
            else
            {
                _fileSizeBefore = string.Empty;
            }

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
            _progressControl.Closing += ProgressControl_Closing;
            _progressControl.ShowDialog();
        }

        /// <summary>
        /// Background worker for performing file compaction.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event arguments.</param>
        private static void BackgroundWorker_Dowork(object? sender, DoWorkEventArgs e)
        {
            if (sender is not BackgroundWorker worker || _project == null) return;
            // Update progress bar to compacting
            worker.ReportProgress(0);
            _project.Compact();
            // Update progress bar to optimizing
            worker.ReportProgress(100);
            _project.Optimize();
        }

        /// <summary>
        /// Background worker progress changed.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event arguments.</param>
        private static void BackgroundWorker_ProgressChanged(object? sender, ProgressChangedEventArgs e)
        {
            if (_progressControl == null) return;
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
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event arguments.</param>
        private static void BackgroundWorker_WorkerComplete(object? sender, RunWorkerCompletedEventArgs e)
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

            if (_progressControl != null)
            {
                _progressControl.Closing -= ProgressControl_Closing;
                _progressControl.Close();
                _progressControl = null;
            }
            if (_timer != null) { _timer.Stop(); _timer.Tick -= Timer_Tick; _timer = null; }
            ShellPublicVariables.CompactionInProgress = false;

            if (_project == null) return;

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
        /// Handles the progress control window closing event.
        /// Cancels the background worker when the user closes the window.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event arguments.</param>
        private static void ProgressControl_Closing(object? sender, CancelEventArgs e)
        {
            _backgroundWorker?.CancelAsync();
            if (_timer != null) { _timer.Stop(); _timer.Tick -= Timer_Tick; _timer = null; }
            ShellPublicVariables.CompactionInProgress = false;
        }

        /// <summary>
        /// Timer used to update the progress bar.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event arguments.</param>
        private static void Timer_Tick(object? sender, EventArgs e)
        {
            if (_project == null || _progressControl == null) return;

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
                    : _fileSizeBefore ?? string.Empty;

                _progressControl.ProgressBar.IsIndeterminate = false;
                _progressControl.ProgressText.Text = "Compacting " + tempFileSizeText + " of " + (_fileSizeBefore ?? string.Empty);
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
        /// <returns>The file size in bytes.</returns>
        public static long GetFileSize(string fileName)
        {
            return new FileInfo(fileName).Length; 
        }

        /// <summary>
        /// Gets the file size of the file in a standard text format; e.g., 12.14 MB.
        /// </summary>
        /// <param name="fileName">The full file name.</param>
        /// <returns>A formatted string representing the file size.</returns>
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

                return value.ToString("N2") + " " + unit;
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
