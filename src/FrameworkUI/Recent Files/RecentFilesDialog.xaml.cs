using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace FrameworkUI
{
    /// <summary>
    /// Interaction logic for RecentFilesDialog.xaml
    /// </summary>
    /// <remarks>
    /// <para>
    ///     Authors:
    ///     Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil
    /// </para>
    /// </remarks>
    public partial class RecentFilesDialog : Window
    {

        /// <summary>
        /// Construct a new recent files dialog.
        /// </summary>
        public RecentFilesDialog()
        {
            // This call is required by the designer.
            InitializeComponent();
            // Add any initialization after the InitializeComponent() call.
            CommandBindings.Add(new CommandBinding(SystemCommands.CloseWindowCommand, OnCloseWindow));
        }

        private void OnCloseWindow(object target, ExecutedRoutedEventArgs e)
        {
            SystemCommands.CloseWindow(this);
        }

        /// <summary>
        /// Dependency property for the recent files list.
        /// </summary>
        public static DependencyProperty FilesProperty = DependencyProperty.Register(nameof(Files), typeof(RecentFiles), typeof(RecentFilesDialog), new UIPropertyMetadata(null));

        /// <summary>
        /// Gets and sets the recent files list.
        /// </summary>
        public RecentFiles Files
        {
            get { return (RecentFiles)GetValue(FilesProperty); }
            set { SetValue(FilesProperty, value); }
        }

        /// <summary>
        /// Dependency property for the project file image. 
        /// </summary>
        public static DependencyProperty FileImageProperty = DependencyProperty.Register(nameof(FileImage), typeof(ImageSource), typeof(RecentFilesDialog), new UIPropertyMetadata(null));

        /// <summary>
        /// The project file image. 
        /// </summary>
        public ImageSource FileImage
        {
            get { return (ImageSource)GetValue(FileImageProperty); }
            set { SetValue(FileImageProperty, value); }
        }

        /// <summary>
        /// Open the clicked recent file.
        /// </summary>
        private void OpenButton_Click(object sender, RoutedEventArgs e)
        {
            if (MyDataGrid.SelectedItem == null) return;
            Files.OpenItem(((RecentFileItem)MyDataGrid.SelectedItem).FilePath);
            Close();
        }

        /// <summary>
        /// Clear all recent files.
        /// </summary>
        private void ClearListButton_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to clear the Recent Files List? This action is permanent.", "Clear the Recent Files List?", MessageBoxButton.YesNoCancel, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                Files.ClearAll();
                Close();
            }
        }

        /// <summary>
        /// When the selection is changed, update open button. 
        /// </summary>
        private void MyDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            OpenButton.IsEnabled = MyDataGrid.SelectedItem != null;
        }

    }
}
