using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace FrameworkUI
{
    /// <summary>
    /// Interaction logic for GeneralOptions.xaml
    /// </summary>
    /// <remarks>
    /// <para>
    ///     Authors:
    ///     Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil
    /// </para>
    /// </remarks>
    public partial class GeneralOptions : UserControl
    {

        /// <summary>
        /// Constructs a new general options control.
        /// </summary>
        public GeneralOptions()
        {
            // This call is required by the designer.
            InitializeComponent();
            // Add any initialization after the InitializeComponent() call.
        }

        /// <summary>
        /// Theme list for combobox.
        /// </summary>
        public List<string> ThemeList { get; set; } = new List<string>(new[] { "Blue", "Light" }); 

        /// <summary>
        /// Dependency property for the color theme.
        /// </summary>
        public static DependencyProperty ColorThemeProperty = DependencyProperty.Register(nameof(ColorTheme), typeof(string), typeof(GeneralOptions), new PropertyMetadata("Light"));

        /// <summary>
        /// Gets and sets the color theme.
        /// </summary>
        public string ColorTheme
        {
            get { return GetValue(ColorThemeProperty).ToString(); }
            set { SetValue(ColorThemeProperty, value); }
        }

        /// <summary>
        /// Dependency property for whether to save window layout.
        /// </summary>
        public static DependencyProperty SaveWindowLayoutProperty = DependencyProperty.Register(nameof(SaveWindowLayout), typeof(bool), typeof(GeneralOptions), new UIPropertyMetadata(true));

        /// <summary>
        /// Gets and sets whether to save window layout.
        /// </summary>
        public bool SaveWindowLayout
        {
            get { return (bool)GetValue(SaveWindowLayoutProperty); }
            set { SetValue(SaveWindowLayoutProperty, value); }
        }

        /// <summary>
        /// Dependency property for the number of window menu items to show.
        /// </summary>
        public static DependencyProperty MaxWindowMenuItemsProperty = DependencyProperty.Register(nameof(MaxWindowMenuItems), typeof(int), typeof(GeneralOptions), new UIPropertyMetadata(10));

        /// <summary>
        /// Gets and sets the number of window menu items to show.
        /// </summary>
        public int MaxWindowMenuItems
        {
            get { return (int)GetValue(MaxWindowMenuItemsProperty); }
            set { SetValue(MaxWindowMenuItemsProperty, value); }
        }

        /// <summary>
        /// Dependency property for the number of window menu items to show.
        /// </summary>
        public static DependencyProperty MaxRecentFileItemsProperty = DependencyProperty.Register(nameof(MaxRecentFileItems), typeof(int), typeof(GeneralOptions), new UIPropertyMetadata(10));

        /// <summary>
        /// Gets and sets the number of recent project items to show.
        /// </summary>
        public int MaxRecentFileItems
        {
            get { return (int)GetValue(MaxRecentFileItemsProperty); }
            set { SetValue(MaxRecentFileItemsProperty, value); }
        }

    }
}
