using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace FrameworkUI
{
    /// <summary>
    /// Interaction logic for OpenWindowsDialog.xaml
    /// </summary>
    /// <remarks>
    /// <para>
    ///     Authors:
    ///     Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil
    /// </para>
    /// </remarks>
    public partial class OpenWindowsDialog : Window
    {
        /// <summary>
        /// Construct a new open windows dialog.
        /// </summary>
        public OpenWindowsDialog()
        {
            // This call is required by the designer.
            InitializeComponent();
            // Add any initialization after the InitializeComponent() call.
            // Required window functionality
            CommandBindings.Add(new CommandBinding(SystemCommands.CloseWindowCommand, OnCloseWindow));

        }

        private void OnCloseWindow(object target, ExecutedRoutedEventArgs e)
        {
            SystemCommands.CloseWindow(this);
        }


        /// <summary>
        /// Dependency property for the open windows list.
        /// </summary>
        public static DependencyProperty WindowsProperty = DependencyProperty.Register(nameof(Windows), typeof(OpenWindows), typeof(OpenWindowsDialog), new UIPropertyMetadata(null));

        /// <summary>
        /// Gets and sets the open windows list.
        /// </summary>
        public OpenWindows Windows
        {
            get { return (OpenWindows)GetValue(WindowsProperty); }
            set
            {
                SetValue(WindowsProperty, value);
                MyDataGrid.ItemsSource = null;
                MyDataGrid.ItemsSource = Windows.Collection;
            }
        }

        /// <summary>
        /// Activate selected window.
        /// </summary>
        private void ActivateButton_Click(object sender, RoutedEventArgs e)
        {
            ((OpenWindowItem)MyDataGrid.SelectedItem).Document.IsActive = true;
        }

        /// <summary>
        /// Save selected windows.
        /// </summary>
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < MyDataGrid.SelectedItems.Count; i++)
                ((OpenWindowItem)MyDataGrid.SelectedItems[i]).Element.Save();
        }

        /// <summary>
        /// Close selected windows.
        /// </summary>
        private void CloseWindwowsButton_Click(object sender, RoutedEventArgs e)
        {
            for (int i = MyDataGrid.SelectedItems.Count - 1; i >= 0; i -= 1)
            {
                int index = Windows.WindowIndexOf(((OpenWindowItem)MyDataGrid.SelectedItems[i]).Document);
                Windows.Close(index);
            }
        }

        /// <summary>
        /// When row is selected, update buttons.
        /// </summary>
        private void MyDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MyDataGrid.SelectedItems.Count <= 0)
            {
                ActivateButton.IsEnabled = false;
                SaveButton.IsEnabled = false;
                CloseWindwowsButton.IsEnabled = false;
            }
            else if (MyDataGrid.SelectedItems.Count == 1)
            {
                ActivateButton.IsEnabled = true;
                SaveButton.IsEnabled = false;
                CloseWindwowsButton.IsEnabled = true;
                if (((OpenWindowItem)MyDataGrid.SelectedItem).Element.IsDirty == true)
                {
                    SaveButton.IsEnabled = true;
                }
            }
            else if (MyDataGrid.SelectedItems.Count > 1)
            {
                ActivateButton.IsEnabled = false;
                SaveButton.IsEnabled = false;
                CloseWindwowsButton.IsEnabled = true;
                for (int i = 0; i < MyDataGrid.SelectedItems.Count; i++)
                {
                    if (((OpenWindowItem)MyDataGrid.SelectedItems[i]).Element.IsDirty == true)
                    {
                        SaveButton.IsEnabled = true;
                        break;
                    }
                }
            }
        }

    }
}
