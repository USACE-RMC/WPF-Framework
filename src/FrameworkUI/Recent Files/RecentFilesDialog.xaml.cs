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
        /// Initializes a new instance of the <see cref="RecentFilesDialog"/> class.
        /// </summary>
        public RecentFilesDialog()
        {
            // This call is required by the designer.
            InitializeComponent();
            // Add any initialization after the InitializeComponent() call.
            CommandBindings.Add(new CommandBinding(SystemCommands.CloseWindowCommand, OnCloseWindow));
        }

        /// <summary>
        /// Handles the close window command.
        /// </summary>
        /// <param name="target">The command target.</param>
        /// <param name="e">The event arguments.</param>
        private void OnCloseWindow(object target, ExecutedRoutedEventArgs e)
        {
            SystemCommands.CloseWindow(this);
        }

        /// <summary>
        /// Dependency property for the recent files list.
        /// </summary>
        public static DependencyProperty FilesProperty = DependencyProperty.Register(nameof(Files), typeof(RecentFiles), typeof(RecentFilesDialog), new UIPropertyMetadata(null));

        /// <summary>
        /// Gets or sets the recent files list.
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
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event arguments.</param>
        private void OpenButton_Click(object sender, RoutedEventArgs e)
        {
            if (MyDataGrid.SelectedItem == null) return;
            Files.OpenItem(((RecentFileItem)MyDataGrid.SelectedItem).FilePath);
            Close();
        }

        /// <summary>
        /// Clear all recent files.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event arguments.</param>
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
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event arguments.</param>
        private void MyDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            OpenButton.IsEnabled = MyDataGrid.SelectedItem != null;
        }

    }
}
