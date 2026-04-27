using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Threading;
using FrameworkInterfaces;

namespace FrameworkUI
{
    /// <summary>
    /// Provides automatic backup functionality for project files.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The AutoBackup class creates periodic backup copies of the project file
    /// at configurable intervals. Backups are created asynchronously using a
    /// background worker to avoid blocking the UI thread.
    /// </para>
    /// <para>
    /// Usage:
    /// <list type="number">
    /// <item><description>Set the <see cref="Project"/> property to the current project</description></item>
    /// <item><description>Call <see cref="Start"/> to begin automatic backups</description></item>
    /// <item><description>Call <see cref="Cancel"/> to stop automatic backups</description></item>
    /// </list>
    /// </para>
    /// <para>
    /// <b> Authors: </b>
    /// <list type="bullet">
    ///     <item> Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil </item>
    /// </list>
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// AutoBackup.Project = myProject;
    /// AutoBackup.ReportProgress += (msg) => Console.WriteLine(msg);
    /// AutoBackup.Start();
    /// // ... later when closing
    /// AutoBackup.Cancel();
    /// </code>
    /// </example>
    public static class AutoBackup
    {
        #region Fields

        private static DispatcherTimer? _timer;
        private static BackgroundWorker? _backgroundWorker;
        private static readonly object _lockObject = new object();

        #endregion

        #region Events

        /// <summary>
        /// Occurs when the backup operation reports progress or completion status.
        /// </summary>
        public static event ReportProgressEventHandler? ReportProgress;

        /// <summary>
        /// Delegate for the <see cref="ReportProgress"/> event.
        /// </summary>
        /// <param name="message">A message describing the progress or status.</param>
        public delegate void ReportProgressEventHandler(string message);

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the project to be backed up.
        /// </summary>
        /// <value>The <see cref="IProject"/> instance to back up.</value>
        public static IProject? Project { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Starts the automatic backup process.
        /// </summary>
        /// <remarks>
        /// <para>
        /// If a backup timer is already running, it will be stopped and restarted
        /// with the current <see cref="UserSettings.AutoRecoverInterval"/> value.
        /// </para>
        /// <para>
        /// Backups are created at the interval specified in <see cref="UserSettings.AutoRecoverInterval"/>
        /// (in minutes).
        /// </para>
        /// </remarks>
        public static void Start()
        {
            // DispatcherTimer must be created on the UI thread. Marshal if necessary.
            var dispatcher = Application.Current?.Dispatcher;
            if (dispatcher != null && !dispatcher.CheckAccess())
            {
                dispatcher.Invoke(Start);
                return;
            }

            lock (_lockObject)
            {
                // Clean up existing timer if any
                if (_timer != null)
                {
                    _timer.Stop();
                    _timer.Tick -= Timer_Tick;
                    _timer = null;
                }

                // Create and start new timer
                _timer = new DispatcherTimer
                {
                    Interval = TimeSpan.FromMinutes(UserSettings.AutoRecoverInterval)
                };
                _timer.Tick += Timer_Tick;
                _timer.Start();
            }
        }

        /// <summary>
        /// Stops the automatic backup process and releases resources.
        /// </summary>
        /// <remarks>
        /// This method should be called when closing the project or application
        /// to ensure proper cleanup of timer and background worker resources.
        /// </remarks>
        public static void Cancel()
        {
            lock (_lockObject)
            {
                if (_timer != null)
                {
                    _timer.Stop();
                    _timer.Tick -= Timer_Tick;
                    _timer = null;
                }

                if (_backgroundWorker != null)
                {
                    if (_backgroundWorker.IsBusy)
                    {
                        _backgroundWorker.CancelAsync();
                    }
                    _backgroundWorker.DoWork -= BackgroundWorker_DoWork;
                    _backgroundWorker.RunWorkerCompleted -= BackgroundWorker_WorkerComplete;
                    _backgroundWorker.Dispose();
                    _backgroundWorker = null;
                }
            }
        }

        /// <summary>
        /// Creates a backup copy of the current project file.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The backup file is created by copying the current project file to a new file
        /// with the <see cref="ShellPublicVariables.BackupExtension"/> appended to the filename.
        /// </para>
        /// <para>
        /// Any existing backup file will be deleted before the new backup is created.
        /// </para>
        /// </remarks>
        public static void CreateBackupProjectFile()
        {
            if (Project == null) return;
            if (string.IsNullOrEmpty(Project.FullFileName)) return;
            if (!File.Exists(Project.FullFileName)) return;

            try
            {
                // Delete existing backup first
                DeleteBackupProjectFile();

                // Create backup copy
                string backupPath = Project.FullFileName + ShellPublicVariables.BackupExtension;
                File.Copy(Project.FullFileName, backupPath, overwrite: true);
            }
            catch (IOException ex)
            {
                // File may be in use - log but don't interrupt user workflow
                Debug.WriteLine($"AutoBackup.CreateBackupProjectFile: IOException - {ex.Message}");
            }
            catch (UnauthorizedAccessException ex)
            {
                // No permission - log but don't interrupt user workflow
                Debug.WriteLine($"AutoBackup.CreateBackupProjectFile: UnauthorizedAccessException - {ex.Message}");
            }
        }

        /// <summary>
        /// Deletes the backup project file if it exists.
        /// </summary>
        /// <remarks>
        /// This method is typically called when the project is saved successfully
        /// and the backup is no longer needed, or before creating a new backup.
        /// </remarks>
        public static void DeleteBackupProjectFile()
        {
            if (Project == null) return;
            if (string.IsNullOrEmpty(Project.FullFileName)) return;

            try
            {
                string backupPath = Project.FullFileName + ShellPublicVariables.BackupExtension;
                if (File.Exists(backupPath))
                {
                    File.Delete(backupPath);
                }
            }
            catch (IOException ex)
            {
                // File may be in use - log but don't interrupt user workflow
                Debug.WriteLine($"AutoBackup.DeleteBackupProjectFile: IOException - {ex.Message}");
            }
            catch (UnauthorizedAccessException ex)
            {
                // No permission - log but don't interrupt user workflow
                Debug.WriteLine($"AutoBackup.DeleteBackupProjectFile: UnauthorizedAccessException - {ex.Message}");
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Handles the timer tick event by initiating an asynchronous backup operation.
        /// </summary>
        private static void Timer_Tick(object? sender, EventArgs e)
        {
            lock (_lockObject)
            {
                // Don't start a new backup if one is already running
                if (_backgroundWorker != null && _backgroundWorker.IsBusy)
                {
                    return;
                }

                // Clean up previous background worker
                if (_backgroundWorker != null)
                {
                    _backgroundWorker.DoWork -= BackgroundWorker_DoWork;
                    _backgroundWorker.RunWorkerCompleted -= BackgroundWorker_WorkerComplete;
                    _backgroundWorker.Dispose();
                }

                // Create new background worker
                _backgroundWorker = new BackgroundWorker
                {
                    WorkerSupportsCancellation = true
                };
                _backgroundWorker.DoWork += BackgroundWorker_DoWork;
                _backgroundWorker.RunWorkerCompleted += BackgroundWorker_WorkerComplete;
                _backgroundWorker.RunWorkerAsync();
            }
        }

        /// <summary>
        /// Performs the backup operation in the background thread.
        /// </summary>
        private static void BackgroundWorker_DoWork(object? sender, DoWorkEventArgs e)
        {
            CreateBackupProjectFile();
        }

        /// <summary>
        /// Handles completion of the backup operation.
        /// </summary>
        private static void BackgroundWorker_WorkerComplete(object? sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled) return;

            string projectName = Project?.Name ?? "Unknown";
            ReportProgress?.Invoke($"A backup file for the project '{projectName}' was saved.");
        }

        #endregion
    }
}
