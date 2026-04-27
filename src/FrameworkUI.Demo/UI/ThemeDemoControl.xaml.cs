using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Themes;

namespace FrameworkUI.Demo.UI
{
    /// <summary>
    /// User control that demonstrates the runtime theme switching capability provided by the Themes library.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This control demonstrates the runtime theme switching capability provided by
    /// the Themes library. It showcases how all WPF controls automatically update
    /// when the theme is changed via <see cref="ThemeService"/>.
    /// </para>
    /// <para>
    /// The control includes:
    /// </para>
    /// <list type="bullet">
    ///     <item>Theme selection radio buttons (Light, Blue, Dark)</item>
    ///     <item>A comprehensive showcase of styled WPF controls</item>
    ///     <item>Sample data for DataGrid demonstration</item>
    /// </list>
    /// <para>
    /// When a theme is selected, the control calls <see cref="FrameworkUI.ThemeManager.SetTheme"/>
    /// which in turn updates <see cref="ThemeService"/> and loads the appropriate
    /// color palette. All controls using DynamicResource bindings update automatically.
    /// </para>
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
    public partial class ThemeDemoControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ThemeDemoControl"/> class.
        /// </summary>
        public ThemeDemoControl()
        {
            InitializeComponent();
            LoadSampleData();
            UpdateThemeRadioButton();

            Loaded += OnLoaded;
            Unloaded += OnUnloaded;
        }

        /// <summary>
        /// Handles the Loaded event. Subscribes to theme changes.
        /// </summary>
        private void OnLoaded(object sender, System.Windows.RoutedEventArgs e)
        {
            // Subscribe to theme changes to keep UI in sync
            ThemeService.Instance.ThemeChanged += OnThemeChanged;
        }

        /// <summary>
        /// Handles the Unloaded event. Unsubscribes from theme changes to prevent memory leaks.
        /// </summary>
        private void OnUnloaded(object sender, System.Windows.RoutedEventArgs e)
        {
            ThemeService.Instance.ThemeChanged -= OnThemeChanged;
        }

        /// <summary>
        /// Loads sample data into the DataGrid for demonstration purposes.
        /// </summary>
        private void LoadSampleData()
        {
            var sampleData = new List<SampleDataItem>
            {
                new SampleDataItem { Name = "Item A", Value = 100, IsActive = true },
                new SampleDataItem { Name = "Item B", Value = 200, IsActive = false },
                new SampleDataItem { Name = "Item C", Value = 150, IsActive = true },
                new SampleDataItem { Name = "Item D", Value = 300, IsActive = true },
            };
            SampleDataGrid.ItemsSource = sampleData;
        }

        /// <summary>
        /// Updates the radio button selection to match the current theme.
        /// </summary>
        private void UpdateThemeRadioButton()
        {
            var currentTheme = ThemeService.Instance.CurrentTheme;
            switch (currentTheme)
            {
                case Theme.Light:
                    LightThemeRadio.IsChecked = true;
                    break;
                case Theme.Blue:
                    BlueThemeRadio.IsChecked = true;
                    break;
                case Theme.Dark:
                    DarkThemeRadio.IsChecked = true;
                    break;
            }
            UpdateCurrentThemeText(currentTheme);
        }

        /// <summary>
        /// Updates the current theme display text.
        /// </summary>
        /// <param name="theme">The current theme.</param>
        private void UpdateCurrentThemeText(Theme theme)
        {
            CurrentThemeText.Text = $"Current Theme: {theme}";
        }

        /// <summary>
        /// Handles theme changes from external sources.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The theme changed event arguments.</param>
        private void OnThemeChanged(object sender, ThemeChangedEventArgs e)
        {
            // Update the radio button and text when theme changes externally
            Dispatcher.Invoke(() =>
            {
                switch (e.NewTheme)
                {
                    case Theme.Light:
                        LightThemeRadio.IsChecked = true;
                        break;
                    case Theme.Blue:
                        BlueThemeRadio.IsChecked = true;
                        break;
                    case Theme.Dark:
                        DarkThemeRadio.IsChecked = true;
                        break;
                }
                UpdateCurrentThemeText(e.NewTheme);
            });
        }

        /// <summary>
        /// Handles the Checked event of theme radio buttons.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The routed event arguments.</param>
        private void ThemeRadio_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is RadioButton radio && radio.IsChecked == true)
            {
                FrameworkUI.ThemeColor themeColor;

                if (radio == LightThemeRadio)
                {
                    themeColor = FrameworkUI.ThemeColor.Light;
                }
                else if (radio == BlueThemeRadio)
                {
                    themeColor = FrameworkUI.ThemeColor.Blue;
                }
                else if (radio == DarkThemeRadio)
                {
                    themeColor = FrameworkUI.ThemeColor.Dark;
                }
                else
                {
                    return;
                }

                // Use FrameworkUI.ThemeManager which bridges to ThemeService
                // This ensures both Themes library colors AND ProjectUI-specific resources are updated
                FrameworkUI.ThemeManager.SetTheme(themeColor);
            }
        }

        /// <summary>
        /// Sample data class for DataGrid demonstration.
        /// </summary>
        private class SampleDataItem
        {
            /// <summary>
            /// Gets or sets the name of the sample data item.
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// Gets or sets the numeric value of the sample data item.
            /// </summary>
            public int Value { get; set; }

            /// <summary>
            /// Gets or sets a value indicating whether the item is active.
            /// </summary>
            public bool IsActive { get; set; }
        }
    }
}
