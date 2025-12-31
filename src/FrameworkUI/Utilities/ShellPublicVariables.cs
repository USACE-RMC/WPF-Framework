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

namespace FrameworkUI
{
    /// <summary>
    /// A static class for public shared shell variables.
    /// </summary>
    /// <remarks>
    /// <para>
    ///     Authors:
/// <para>
/// <b> Authors: </b>
/// <list type="bullet">
///     <item> Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil </item>
/// </list>
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
