/*
* NOTICE:
* The U.S. Army Corps of Engineers, Risk Management Center (USACE-RMC) makes no guarantees about
* the results, or appropriateness of outputs, obtained from this software.
*
* LIST OF CONDITIONS:
* Redistribution and use in source and binary forms, with or without modification, are permitted
* provided that the following conditions are met:
* ? Redistributions of source code must retain the above notice, this list of conditions, and the
* following disclaimer.
* ? Redistributions in binary form must reproduce the above notice, this list of conditions, and
* the following disclaimer in the documentation and/or other materials provided with the distribution.
* ? The names of the U.S. Government, the U.S. Army Corps of Engineers, the Institute for Water
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

using GenericControls;
using FrameworkUI.ProjectExplorer;
using SoftwareUpdate;
using SoftwareUpdate.GitHub;
using SoftwareUpdate.Utilities;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Shell;
using System.Windows.Threading;
using Xceed.Wpf.AvalonDock.Layout;
using FrameworkUI.Demo.UI;

#nullable enable

namespace FrameworkUI.Demo
{
    /// <summary>
    /// Main application class for the FrameworkUI.Demo application. Handles application startup, initialization of the theme system, and configuration of the software update service.
    /// </summary>
    /// <remarks>
    /// <para>
    ///     <b> Authors: </b>
    /// <list type="bullet">
    /// <item><description>
    ///     Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil
    /// </description></item>
    /// <item><description>
    ///     Woodrow Fields, USACE Risk Management Center, woodrow.l.fields@usace.army.mil
    /// </description></item>
    /// </list>
    /// </para>
    /// </remarks>
    public partial class App : Application
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="App"/> class and configures the Windows jump list.
        /// </summary>
        public App()
        {
            // Register assembly resolver to load DLLs from the Libraries subfolder.
            // This must be done before any external assemblies are referenced.
            AssemblyResolver.Register();

            // Set WPF to use the current culture for all bindings (international number support)
            // This ensures StringFormat in XAML bindings uses the user's locale settings
            FrameworkElement.LanguageProperty.OverrideMetadata(
                typeof(FrameworkElement),
                new FrameworkPropertyMetadata(
                    System.Windows.Markup.XmlLanguage.GetLanguage(
                        System.Globalization.CultureInfo.CurrentCulture.IetfLanguageTag)));

            // Set up global exception handlers for debugging
            AppDomain.CurrentDomain.UnhandledException += (s, args) =>
            {
                var ex = args.ExceptionObject as Exception;
                LogException("AppDomain.UnhandledException", ex);
            };

            DispatcherUnhandledException += (s, args) =>
            {
                LogException("Dispatcher.UnhandledException", args.Exception);
                args.Handled = true; // Prevent immediate crash to see the error
            };

            TaskScheduler.UnobservedTaskException += (s, args) =>
            {
                LogException("TaskScheduler.UnobservedTaskException", args.Exception);
            };

            var jList = new JumpList { ShowRecentCategory = false };
            jList.Apply();
            JumpList.SetJumpList(Application.Current, jList);
        }

        /// <summary>
        /// Logs an exception to both debug output and a file for diagnosis.
        /// </summary>
        private static void LogException(string source, Exception? ex)
        {
            var separator = new string('=', 50);
            var message = $"\n{separator}\n{source}\n{separator}\n{ex}\n";
            System.Diagnostics.Debug.WriteLine(message);

            // Also write to a file so we can see it
            try
            {
                var logPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "FrameworkUI_Demo_Error.txt");
                File.AppendAllText(logPath, $"{DateTime.Now:yyyy-MM-dd HH:mm:ss}\n{message}\n\n");
                GenericControls.MessageBox.Show($"Exception logged to: {logPath}\n\n{ex?.Message}", source, MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch { }
        }

        /// <summary>
        /// Handles the application startup event. Initializes the theme system, creates the project model, configures the main window, and sets up the software update service.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The startup event arguments.</param>
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            // Initialize theme system - this must be done before creating any UI
            // ThemeService loads control templates and color palettes from the Themes library
            // ThemeManager bridges to ThemeService and loads ProjectUI-specific resources
            FrameworkUI.ThemeManager.SetTheme(FrameworkUI.ThemeColor.Light);

            FrameworkUI.ShellPublicVariables.SoftwareVersionDate = "December 2025";
            FrameworkUI.ShellPublicVariables.SoftwareExtension = ".fun";
            FrameworkUI.UserSettings.CreateAutoRecoverBackup = false;

            // Create the project model
            DemoProject project = DemoProject.GetInstance();
            var projectNode = new DemoProjectNode(project) { Style = (Style)FindResource("TreeViewItemStyle") };

            project.CreateNewDummyProject();

            // Create and Show the Main Window
            FrameworkUI.MainWindow mainWindow = new FrameworkUI.MainWindow(); //{ ProjectNode = projectNode }; 
            mainWindow.ProjectNode = projectNode;

            //var treeGrid = new Grid();
            ////var treeStyle = (Style)FindResource("TreeViewStyle");
            //var itemStyle = (Style)FindResource("ElementNodeStyle");
            //ExplorerTreeView explorer = new ExplorerTreeView();
            //var layerCollection = new NodeCollection(null, explorer); //{ ShowCreateNewContextItem = false };
            //layerCollection.NodeHeader.HeaderText = "Layers";
            //var g = new NodeGroup(layerCollection, explorer) { IsCheckBoxNode = true };//, Style = itemStyle };
            //g.NodeHeader.HeaderText = "Grouped Items";
            //g.Add(CreateNode("Test 1", g, explorer, itemStyle));
            //g.Add(CreateNode("Test 2", g, explorer, itemStyle));
            //layerCollection.Add(g);
            //layerCollection.Add(CreateNode("Test 3", g, explorer,itemStyle));
            //layerCollection.Add(CreateNode("Test 4", g, explorer, itemStyle));
            //layerCollection.Add(CreateNode("Test 5", g, explorer, itemStyle));
            //layerCollection.Add(CreateNode("Test 6", g, explorer, itemStyle));
            //layerCollection.Add(CreateNode("Test 7", g, explorer, itemStyle));
            //layerCollection.Add(CreateNode("Test 8", g, explorer, itemStyle));
            //explorer.Items.Add(layerCollection);
            //var document = new LayoutDocument() { CanClose = false, IconSource = new BitmapImage(new Uri("pack://application:,,,/FrameworkUI.Demo;component/Resources/Hazard_Icon.png")) };
            //treeGrid.Children.Add(explorer);
            //document.Content = treeGrid;
            //document.ContentId = "MapLayers"; //element.ParentCollection.Name
            //document.Title = "Map Layers";
            //mainWindow.OpenDocument(document, null);

            //// Open the Theme Demo document to demonstrate the new Themes library
            //var themeDemo = new LayoutDocument() { CanClose = false };
            //themeDemo.Content = new UI.ThemeDemoControl();
            //themeDemo.ContentId = "ThemeDemo";
            //themeDemo.Title = "Theme Demo";
            //mainWindow.OpenDocument(themeDemo, null);

            // =================================================================
            // SOFTWARE UPDATE SERVICE EXAMPLE
            // =================================================================
            // This demonstrates how to configure the SoftwareUpdate library
            // for checking updates from GitHub releases.
            //
            // For RMC-BestFit, configure with actual values:
            //   GitHubOwner = "USACE-RMC"
            //   GitHubRepo = "RMC-BestFit"
            //   CurrentVersion = your app's current version
            //   AssetNamePattern = "RMC-BestFit.Version.*.zip"
            // =================================================================

            // Auto-check settings (not part of UpdateOptions, handled locally)
            bool autoCheckOnStartup = true;
            int autoCheckDelayMs = 5000;  // 5 second delay after startup

            // Configure update options for your GitHub repository
            var updateOptions = new UpdateOptions
            {
                // GitHub repository settings
                GitHubOwner = "USACE-RMC",          // Your GitHub organization/user
                GitHubRepo = "Demo-Project",         // Your repository name (change to actual repo)

                // Current application version (parse from assembly or hardcode)
                CurrentVersion = new SemanticVersion(1, 0, 0),

                // Pattern to match release assets (supports * wildcard)
                AssetNamePattern = "Demo-Project.*.zip",

                // Include pre-release versions (beta, alpha, rc)
                IncludePreReleases = false,

                // Create backup before updating
                CreateBackup = true,

                // Name of main executable to restart after update
                MainExecutableName = "FrameworkUI.Demo.exe"
            };

            // Create the update service
            var updateService = new GitHubUpdateService(updateOptions);

            // Assign to MainWindow - this enables the "Check for Updates" menu item
            mainWindow.UpdateService = updateService;

            // Optional: Subscribe to update events for logging or custom handling
            updateService.UpdateCheckCompleted += (s, result) =>
            {
                if (result.IsUpdateAvailable)
                {
                    System.Diagnostics.Debug.WriteLine(
                        $"Update available: {result.Update.Version}");
                }
            };

            updateService.UpdateError += (s, ex) =>
            {
                System.Diagnostics.Debug.WriteLine($"Update error: {ex.Message}");
            };

            // Optional: Trigger auto-check after startup delay
            if (autoCheckOnStartup)
            {
                AutoCheckForUpdatesAsync(mainWindow, updateService, autoCheckDelayMs);
            }

            mainWindow.Show();
        }

        /// <summary>
        /// Creates a simple node for the explorer tree view.
        /// </summary>
        /// <param name="name">The name of the node.</param>
        /// <param name="parent">The parent node.</param>
        /// <param name="explorer">The explorer tree view.</param>
        /// <param name="itemStyle">The style to apply to the node.</param>
        /// <returns>A new <see cref="Node"/> instance.</returns>
        private Node CreateNode(string name, Node parent, ExplorerTreeView explorer, Style itemStyle)
        {
            var n = new SimpleNode(name,parent, explorer) { IsCheckBoxNode = true, Style=itemStyle };
            return n;
        }

        /// <summary>
        /// Automatically checks for updates after a delay and prompts the user if available.
        /// </summary>
        /// <param name="mainWindow">The main application window.</param>
        /// <param name="updateService">The update service to use for checking updates.</param>
        /// <param name="delayMs">The delay in milliseconds before checking for updates.</param>
        private async void AutoCheckForUpdatesAsync(FrameworkUI.MainWindow mainWindow, IUpdateService updateService, int delayMs)
        {
            try
            {
                // Wait for the application to fully initialize
                await Task.Delay(delayMs);

                // Check for updates
                var result = await updateService.CheckForUpdateAsync();

                if (result.IsUpdateAvailable && result.Update != null)
                {
                    // Don't prompt if the user has chosen to skip this version
                    if (updateService.IsVersionSkipped(result.Update.Version))
                        return;

                    // Show update notification on the UI thread
                    await mainWindow.Dispatcher.InvokeAsync(() =>
                    {
                        var message = $"A new version ({result.Update.Version}) is available.\n\n" +
                                     $"You are currently running version {updateService.Options.CurrentVersion}.\n\n" +
                                     "Would you like to download and install the update now?";

                        var msgResult = GenericControls.MessageBox.Show(
                            mainWindow,
                            message,
                            "Update Available",
                            MessageBoxButton.YesNo,
                            MessageBoxImage.Information);

                        if (msgResult == MessageBoxResult.Yes)
                        {
                            // Trigger the same update flow as the menu item
                            // The MainWindow's CheckForUpdates_Click handler will show the full dialog
                            // For auto-update, we just notify - user can use menu to proceed
                            GenericControls.MessageBox.Show(
                                mainWindow,
                                $"Use Tools ? Check for Updates to download version {result.Update.Version}.",
                                "Update Available",
                                MessageBoxButton.OK,
                                MessageBoxImage.Information);
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                // Silently fail for auto-check - don't interrupt user's workflow
                System.Diagnostics.Debug.WriteLine($"Auto-update check failed: {ex.Message}");
            }
        }
    }
}
