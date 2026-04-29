using System.Windows;
using Themes;

namespace ExpressionParserControls.Demo
{
    /// <summary>
    /// Represents the WPF application for testing the Expression Parser library.
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            // Set WPF to use the current culture for all bindings (international number support)
            // This ensures StringFormat in XAML bindings uses the user's locale settings
            FrameworkElement.LanguageProperty.OverrideMetadata(
                typeof(FrameworkElement),
                new FrameworkPropertyMetadata(
                    System.Windows.Markup.XmlLanguage.GetLanguage(
                        System.Globalization.CultureInfo.CurrentCulture.IetfLanguageTag)));
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            ThemeService.Instance.Initialize(Theme.Light);

            var mainWindow = new MainWindow();
            mainWindow.Show();
        }
    }
}
