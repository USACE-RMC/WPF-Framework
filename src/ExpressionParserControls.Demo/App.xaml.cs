using System.Windows;
using Themes;

namespace ExpressionParserControls.Demo
{
    /// <summary>
    /// Represents the WPF application for testing the Expression Parser library.
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            ThemeService.Instance.Initialize(Theme.Light);

            var mainWindow = new MainWindow();
            mainWindow.Show();
        }
    }
}
