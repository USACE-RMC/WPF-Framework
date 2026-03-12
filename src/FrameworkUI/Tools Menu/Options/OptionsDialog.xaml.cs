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

using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using GenericControls;

namespace FrameworkUI
{
    /// <summary>
    /// Interaction logic for OptionsDialog.xaml providing application settings dialog.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b> Authors: </b>
    /// <list type="bullet">
    ///     <item> Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil </item>
    /// </list>
    /// </para>
    /// </remarks>
    public partial class OptionsDialog : MetroWindow
    {
        /// <summary>
        /// Occurs when the Apply button is clicked.
        /// </summary>
        public event RoutedEventHandler? Apply_Click;

        /// <summary>
        /// Initializes a new instance of the <see cref="OptionsDialog"/> class.
        /// </summary>
        /// <param name="mainWindow">The parent main window.</param>
        public OptionsDialog(MainWindow mainWindow)
        {
            // This call is required by the designer.
            InitializeComponent();
            // Add any initialization after the InitializeComponent() call.
            _mainWindow = mainWindow;
            Owner = _mainWindow;
        }

        /// <summary>
        /// Reference to the main window.
        /// </summary>
        private MainWindow _mainWindow;

        /// <summary>
        /// On load, set options from user settings.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event arguments.</param>
        private void OptionsDialog_Loaded(object sender, RoutedEventArgs e)
        {
            // Load General Settings
            GeneralOptions.ColorTheme = UserSettings.ColorTheme.ToString();
            GeneralOptions.SaveWindowLayout = UserSettings.SaveWindowLayout;
            GeneralOptions.MaxWindowMenuItems = UserSettings.MaxWindowMenuItems;
            GeneralOptions.MaxRecentFileItems = UserSettings.MaxRecentFileItems;

            // Load File Management Settings
            FileManagementOptions.CompressProjectFileOnClose = UserSettings.CompressProjectFileOnClose;
            FileManagementOptions.CreateAutoRecoverBackup = UserSettings.CreateAutoRecoverBackup;
            FileManagementOptions.AutoRecoverInterval = UserSettings.AutoRecoverInterval;
            FileManagementOptions.KeepLastBackupVersion = UserSettings.KeepLastBackupVersion;

            // Load Message Window Settings
            MessageWindowOptions.MessageBeep = UserSettings.MessageBeep;
            MessageWindowOptions.WarningBeep = UserSettings.WarningBeep;
            MessageWindowOptions.ErrorBeep = UserSettings.ErrorBeep;
            MessageWindowOptions.EventBeep = UserSettings.EventBeep;
            MessageWindowOptions.MessageColor = new SolidColorBrush(Color.FromArgb(UserSettings.MessageColor.A, UserSettings.MessageColor.R, UserSettings.MessageColor.G, UserSettings.MessageColor.B));
            MessageWindowOptions.WarningColor = new SolidColorBrush(Color.FromArgb(UserSettings.WarningColor.A, UserSettings.WarningColor.R, UserSettings.WarningColor.G, UserSettings.WarningColor.B));
            MessageWindowOptions.ErrorColor = new SolidColorBrush(Color.FromArgb(UserSettings.ErrorColor.A, UserSettings.ErrorColor.R, UserSettings.ErrorColor.G, UserSettings.ErrorColor.B));
            MessageWindowOptions.EventColor = new SolidColorBrush(Color.FromArgb(UserSettings.EventColor.A, UserSettings.EventColor.R, UserSettings.EventColor.G, UserSettings.EventColor.B));

            // Load Defaults Settings
            DefaultsOptions.DefaultLocation = UserSettings.DefaultLocation;
            DefaultsOptions.DefaultValueDigits = UserSettings.DefaultValueDigits;
        }

        /// <summary>
        /// On Apply, set dialog result to true and close.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event arguments.</param>
        private void btn_Apply_Click(object sender, RoutedEventArgs e)
        {
            // Record the current theme:
            string currentColorTheme = UserSettings.ColorTheme;
            // Record the AutoRecover settings
            bool currentAutoRecover = UserSettings.CreateAutoRecoverBackup;
            int currentAutoRecoverTime = UserSettings.AutoRecoverInterval;

            // Update General Settings
            UserSettings.ColorTheme = GeneralOptions.ColorTheme.ToString();
            UserSettings.SaveWindowLayout = GeneralOptions.SaveWindowLayout;
            UserSettings.MaxWindowMenuItems = GeneralOptions.MaxWindowMenuItems;
            UserSettings.MaxRecentFileItems = GeneralOptions.MaxRecentFileItems;

            // Update File Management Settings
            UserSettings.CompressProjectFileOnClose = FileManagementOptions.CompressProjectFileOnClose;
            UserSettings.CreateAutoRecoverBackup = FileManagementOptions.CreateAutoRecoverBackup;
            UserSettings.AutoRecoverInterval = FileManagementOptions.AutoRecoverInterval;
            UserSettings.KeepLastBackupVersion = FileManagementOptions.KeepLastBackupVersion;

            // Update Message Window Settings
            var messenger = FrameworkInterfaces.Messaging.Messenger.GetInstance();
            UserSettings.MessageBeep = MessageWindowOptions.MessageBeep;
            UserSettings.WarningBeep = MessageWindowOptions.WarningBeep;
            UserSettings.ErrorBeep = MessageWindowOptions.ErrorBeep;
            UserSettings.EventBeep = MessageWindowOptions.EventBeep;
            messenger.MessageBeep = UserSettings.MessageBeep;
            messenger.WarningBeep = UserSettings.WarningBeep;
            messenger.ErrorBeep = UserSettings.ErrorBeep;
            messenger.EventBeep = UserSettings.EventBeep;

            SolidColorBrush solidBrush = (SolidColorBrush)MessageWindowOptions.MessageColor;
            UserSettings.MessageColor = System.Drawing.Color.FromArgb(solidBrush.Color.A, solidBrush.Color.R, solidBrush.Color.G, solidBrush.Color.B);
            messenger.MessageColor = solidBrush;

            solidBrush = (SolidColorBrush)MessageWindowOptions.WarningColor;
            UserSettings.WarningColor = System.Drawing.Color.FromArgb(solidBrush.Color.A, solidBrush.Color.R, solidBrush.Color.G, solidBrush.Color.B);
            messenger.WarningColor = solidBrush;

            solidBrush = (SolidColorBrush)MessageWindowOptions.ErrorColor;
            UserSettings.ErrorColor = System.Drawing.Color.FromArgb(solidBrush.Color.A, solidBrush.Color.R, solidBrush.Color.G, solidBrush.Color.B);
            messenger.ErrorColor = solidBrush;

            solidBrush = (SolidColorBrush)MessageWindowOptions.EventColor;
            UserSettings.EventColor = System.Drawing.Color.FromArgb(solidBrush.Color.A, solidBrush.Color.R, solidBrush.Color.G, solidBrush.Color.B);
            messenger.EventColor = solidBrush;

            // Update Defaults Settings
            UserSettings.DefaultLocation = DefaultsOptions.DefaultLocation;
            UserSettings.DefaultValueDigits = DefaultsOptions.DefaultValueDigits;

            // Save settings
            UserSettings.Save(ShellPublicVariables.UserSettingsFilePath);

            // Update Color Theme if needed
            if (UserSettings.ColorTheme != currentColorTheme)
            {
                if (UserSettings.ColorTheme == "Blue")
                {
                    ThemeManager.SetTheme(ThemeColor.Blue);
                }
                else if (UserSettings.ColorTheme == "Dark")
                {
                    ThemeManager.SetTheme(ThemeColor.Dark);
                }
                else if (UserSettings.ColorTheme == "Light")
                {
                    ThemeManager.SetTheme(ThemeColor.Light);
                }
            }
            // Update AutoRecover if needed
            if (UserSettings.CreateAutoRecoverBackup != currentAutoRecover || UserSettings.AutoRecoverInterval != currentAutoRecoverTime)
            {
                if (UserSettings.CreateAutoRecoverBackup == true)
                {
                    AutoBackup.Cancel();
                    AutoBackup.Start();
                }
                else if (UserSettings.CreateAutoRecoverBackup == false)
                {
                    AutoBackup.Cancel();
                }
            }
            // Update Recent File Menu
            _mainWindow.RecentFiles.NumberOfFilesToDisplay = UserSettings.MaxRecentFileItems;
            // Update Windows Menu
            _mainWindow.OpenWindows.NumberOfWindowsToDisplay = UserSettings.MaxWindowMenuItems;
            // Pass Event
            Apply_Click?.Invoke(sender, e);
        }

        /// <summary>
        /// On Cancel, set dialog result to false and close.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event arguments.</param>
        private void btn_Cancel_Click(object sender, RoutedEventArgs e)
        {
            // Close dialog
            DialogResult = false;
            Close();
        }

        /// <summary>
        /// On closing, activate the Main Window.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event arguments.</param>
        private void OptionsDialog_Closing(object sender, CancelEventArgs e)
        {
            if (Owner != null) Owner.Activate();
        }

    }
}
