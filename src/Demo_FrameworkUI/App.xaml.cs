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

using GenericControls;
using FrameworkUI.ProjectExplorer;
using SoftwareUpdate;
using SoftwareUpdate.GitHub;
using SoftwareUpdate.Utilities;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Shell;
using Xceed.Wpf.AvalonDock.Layout;

namespace Demo_FrameworkUI
{
    /// <summary>
    /// Main application class for the Demo_FrameworkUI application. Handles application startup, initialization of the theme system, and configuration of the software update service.
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
            var jList = new JumpList { ShowRecentCategory = false };
            jList.Apply();
            JumpList.SetJumpList(Application.Current, jList);
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
            Project.Project project = new Project.Project();//TotalRisk.Project.GetInstance()
            var projectNode = new ExampleProjectNode(project);//{ Style = (Style)FindResource("TreeViewItemStyle") }

            // Create and Show the Main Window
            FrameworkUI.MainWindow mainWindow = new FrameworkUI.MainWindow(); //{ ProjectNode = projectNode }; 
            mainWindow.ProjectNode = projectNode;

            var treeGrid = new Grid();
            //var treeStyle = (Style)FindResource("TreeViewStyle");
            var itemStyle = (Style)FindResource("ElementNodeStyle");
            ExplorerTreeView explorer = new ExplorerTreeView();
            var layerCollection = new NodeCollection(null, explorer); //{ ShowCreateNewContextItem = false };
            layerCollection.NodeHeader.HeaderText = "Layers";
            var g = new NodeGroup(layerCollection, explorer) { IsCheckBoxNode = true };//, Style = itemStyle };
            g.NodeHeader.HeaderText = "Grouped Items";
            g.Add(CreateNode("Test 1", g, explorer, itemStyle));
            g.Add(CreateNode("Test 2", g, explorer, itemStyle));
            layerCollection.Add(g);
            layerCollection.Add(CreateNode("Test 3", g, explorer,itemStyle));
            layerCollection.Add(CreateNode("Test 4", g, explorer, itemStyle));
            layerCollection.Add(CreateNode("Test 5", g, explorer, itemStyle));
            layerCollection.Add(CreateNode("Test 6", g, explorer, itemStyle));
            layerCollection.Add(CreateNode("Test 7", g, explorer, itemStyle));
            layerCollection.Add(CreateNode("Test 8", g, explorer, itemStyle));
            explorer.Items.Add(layerCollection);
            var document = new LayoutDocument() { CanClose = false, IconSource = new BitmapImage(new Uri("pack://application:,,/Demo_FrameworkUI;component/Resources/Hazard_Icon.png")) };
            treeGrid.Children.Add(explorer);
            document.Content = treeGrid;
            document.ContentId = "MapLayers"; //element.ParentCollection.Name
            document.Title = "Map Layers";

            mainWindow.OpenDocument(document, null);

            // Open the Theme Demo document to demonstrate the new Themes library
            var themeDemo = new LayoutDocument() { CanClose = false };
            themeDemo.Content = new UI.ThemeDemoControl();
            themeDemo.ContentId = "ThemeDemo";
            themeDemo.Title = "Theme Demo";
            mainWindow.OpenDocument(themeDemo, null);

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

                // Auto-check for updates on startup (delay in milliseconds)
                AutoCheckOnStartup = true,
                AutoCheckDelayMs = 5000,  // 5 second delay after startup

                // Name of main executable to restart after update
                MainExecutableName = "Demo_FrameworkUI.exe"
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
                        $"Update available: {result.AvailableUpdate.Version}");
                }
            };

            updateService.UpdateError += (s, ex) =>
            {
                System.Diagnostics.Debug.WriteLine($"Update error: {ex.Message}");
            };

            // Optional: Trigger auto-check after startup delay
            if (updateOptions.AutoCheckOnStartup)
            {
                AutoCheckForUpdatesAsync(mainWindow, updateService, updateOptions.AutoCheckDelayMs);
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

                if (result.IsUpdateAvailable && result.AvailableUpdate != null)
                {
                    // Don't prompt if the user has chosen to skip this version
                    if (updateService.IsVersionSkipped(result.AvailableUpdate.Version))
                        return;

                    // Show update notification on the UI thread
                    await mainWindow.Dispatcher.InvokeAsync(() =>
                    {
                        var message = $"A new version ({result.AvailableUpdate.Version}) is available.\n\n" +
                                     $"You are currently running version {updateService.Options.CurrentVersion}.\n\n" +
                                     "Would you like to download and install the update now?";

                        var msgResult = MessageBox.Show(
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
                            mainWindow.ShowMessage(
                                $"Use Tools → Check for Updates to download version {result.AvailableUpdate.Version}.",
                                "Update Available");
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
