using System;

namespace FrameworkUI
{
    /// <summary>
    /// A static class for public shared shell variables.
    /// </summary>
    /// <remarks>
    /// <para>
    ///     Authors:
    ///     Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil
    /// </para>
    /// </remarks>
    public static class ShellPublicVariables
    {
        /// <summary>
        /// Names of standard dock Windows for AvalonDock
        /// </summary>
        public const string ProjectExplorerContentID = "ProjectExplorer";
        public const string MessageWindowContentID = "MessageWindow";
        public const string PropertiesWindowContentID = "PropertiesWindow";
        public const string ProjectExplorerTitle = "  Project Explorer";
        public const string MapExplorerTitle = "  Map Explorer";
        public const string MessageWindowTitle = "  Message Window";
        public const string PropertiesWindowTitle = "  Properties";

        /// <summary>
        /// The folder path for user settings
        /// </summary>
        public static string UserSettingsFolderPath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ApplicationAttributes.CompanyName + @"\" + ApplicationAttributes.ProductName + " " + Version.Parse(ApplicationAttributes.Version).Major + @"\Settings\");

        /// <summary>
        /// File path of the user settings.
        /// </summary>
        public static string UserSettingsFilePath = UserSettingsFolderPath + "UserSettings.xml";

        /// <summary>
        /// The folder path for layout.
        /// </summary>
        public static string AvalonDockLayoutFolderPath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ApplicationAttributes.CompanyName + @"\" + ApplicationAttributes.ProductName + " " + Version.Parse(ApplicationAttributes.Version).Major + @"\Layout\");

        /// <summary>
        /// File path of the default AvalonDock layout.
        /// </summary>
        public static string DefaultAvalonDockLayoutFilePath = AvalonDockLayoutFolderPath + "AvalonDock.xml";

        /// <summary>
        /// The folder path for recent file list.
        /// </summary>
        public static string RecentFileListFolderPath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ApplicationAttributes.CompanyName + @"\" + ApplicationAttributes.ProductName + " " + Version.Parse(ApplicationAttributes.Version).Major + @"\RecentFiles\");

        /// <summary>
        /// File path of the recent file list.
        /// </summary>
        public static string RecentFileListFilePath = RecentFileListFolderPath + "RecentFileList.xml";

        /// <summary>
        /// The folder path for message log.
        /// </summary>
        public static string MessageLogFolderPath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ApplicationAttributes.CompanyName + @"\" + ApplicationAttributes.ProductName + " " + Version.Parse(ApplicationAttributes.Version).Major + @"\MessageLog\");

        /// <summary>
        /// File path of the message log.
        /// </summary>
        public static string MessageLogFilePath = MessageLogFolderPath + "MessageLog.txt";

        /// <summary>
        /// Software version and name variables
        /// </summary>
        public static string SoftwareVersion = ApplicationAttributes.Version;
        public static string SoftwareVersionDate = "March 2020";
        public static string SoftwareName = ApplicationAttributes.Title;
        public static string SoftwareExtension = ".tra";
        public static string BackupExtension = ".bak";

        /// <summary>
        /// Filter for file dialogs.
        /// </summary>
        public static string FileDialogFilter
        {
            get { return $"{SoftwareName} Files (*{SoftwareExtension})|*{SoftwareExtension}"; }
        }

        /// <summary>
        /// Filter for backup file dialog.
        /// </summary>
        public static string BackupFileDialogFilter
        {
            get { return $"{SoftwareName} BAK Files (*{SoftwareExtension}{BackupExtension})|*{SoftwareExtension}{BackupExtension}"; }
        }

        /// <summary>
        /// Determines if a simulation is in progress.
        /// </summary>
        public static bool SimulationInProgress = false;

        /// <summary>
        /// Determines if a simulation has been canceled.
        /// </summary>
        public static bool SimulationIsCanceled = false;

        /// <summary>
        /// Determines if a a Bayesian estimation file is being copied.
        /// These files are large and can take a long time to copy.
        /// </summary>
        public static bool CopyingSimulationFile = false;

        /// <summary>
        /// Determines if a project file is being compressed.
        /// </summary>
        public static bool CompactionInProgress;

        /// <summary>
        /// Determines if file can be dropped in and opened.
        /// </summary>
        public static bool IsDroppableFile = false;

    }
}
