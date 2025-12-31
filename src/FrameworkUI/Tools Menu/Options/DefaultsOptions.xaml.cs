using GenericControls;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace FrameworkUI
{
    /// <summary>
    /// Interaction logic for DefaultsOptions.xaml
    /// </summary>
    /// <remarks>
    /// <para>
    ///     Authors:
    ///     Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil
    /// </para>
    /// </remarks>
    public partial class DefaultsOptions : UserControl
    {

        /// <summary>
        /// Construct new default options.
        /// </summary>
        public DefaultsOptions()
        {
            // This call is required by the designer.
            InitializeComponent();
            // Add any initialization after the InitializeComponent() call.
        }

        /// <summary>
        /// Dependency property for the location text.
        /// </summary>
        public static DependencyProperty DefaultLocationProperty = DependencyProperty.Register(nameof(DefaultLocation), typeof(string), typeof(DefaultsOptions), new UIPropertyMetadata(""));

        /// <summary>
        /// Gets and sets the folder location text.
        /// </summary>
        public string DefaultLocation
        {
            get { return GetValue(DefaultLocationProperty).ToString(); }
            set { SetValue(DefaultLocationProperty, value); }
        }

        /// <summary>
        /// Dependency property for the default value digits.
        /// </summary>
        public static DependencyProperty DefaultValueDigitsProperty = DependencyProperty.Register(nameof(DefaultValueDigits), typeof(int), typeof(DefaultsOptions), new UIPropertyMetadata(2));

        /// <summary>
        /// Gets and sets the default output value decimal digits.
        /// </summary>
        public int DefaultValueDigits
        {
            get { return (int)GetValue(DefaultValueDigitsProperty); }
            set { SetValue(DefaultValueDigitsProperty, value); }
        }

        public static DependencyProperty CustomOptionsProperty = DependencyProperty.Register(nameof(CustomOptions), typeof(object), typeof(DefaultsOptions), new FrameworkPropertyMetadata(null));
        public object CustomOptions
        {
            get { return GetValue(CustomOptionsProperty); }
            set { SetValue(CustomOptionsProperty, value); }
        }

        /// <summary>
        /// On click, open folder browser to select folder.
        /// </summary>
        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            string selectedLocation = GeneralMethods.FolderBrowserDialog(Window.GetWindow(this), "Project Location", Directory.Exists(DefaultLocation) == true ? DefaultLocation : null);
            if (!string.IsNullOrEmpty(selectedLocation)) DefaultLocation = selectedLocation;
        }

    }
}
