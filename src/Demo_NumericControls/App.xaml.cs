using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Themes;

namespace Demo_NumericControls
{
    /// <summary>
    /// Interaction logic for App.xaml.
    /// Demonstrates the NumericControls library with theme support.
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Handles the application startup event.
        /// Initializes the theme system before creating any UI elements.
        /// </summary>
        /// <param name="sender">The event source.</param>
        /// <param name="e">Event arguments containing startup information.</param>
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            // Initialize the theme system with Light theme
            // This must be called before creating any windows
            ThemeService.Instance.Initialize(Theme.Light);

            // Create and show the main window
            var mainWindow = new MainWindow();
            mainWindow.Show();
        }
    }
}
