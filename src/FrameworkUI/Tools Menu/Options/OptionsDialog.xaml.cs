using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using FrameworkUI.MessageWindow;

namespace FrameworkUI
{
    /// <summary>
    /// Interaction logic for OptionsDialog.xaml
    /// </summary>
    /// <remarks>
    /// <para>
    ///     Authors:
    ///     Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil
    /// </para>
    /// </remarks>
    public partial class OptionsDialog : Window
    {
        public event RoutedEventHandler Apply_Click;

        /// <summary>
        /// Create new instance of the options dialog.
        /// </summary>
        /// <param name="mainWindow"></param>
        public OptionsDialog(MainWindow mainWindow)
        {
            // This call is required by the designer.
            InitializeComponent();
            // Add any initialization after the InitializeComponent() call.
            CommandBindings.Add(new CommandBinding(SystemCommands.CloseWindowCommand, OnCloseWindow));
            _mainWindow = mainWindow;
            Owner = _mainWindow;
        }

        private void OnCloseWindow(object target, ExecutedRoutedEventArgs e)
        {
            SystemCommands.CloseWindow(this);
        }

        private MainWindow _mainWindow;

        /// <summary>
        /// On load, set options from user settings.
        /// </summary>
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
            if (UserSettings.CreateAutoRecoverBackup != currentAutoRecover | UserSettings.AutoRecoverInterval != currentAutoRecoverTime)
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
        private void btn_Cancel_Click(object sender, RoutedEventArgs e)
        {
            // Close dialog
            DialogResult = false;
            Close();
        }

        /// <summary>
        /// On closing, activate the Main Window.
        /// </summary>
        private void OptionsDialog_Closing(object sender, CancelEventArgs e)
        {
            if (Owner != null) Owner.Activate();
        }

    }
}
