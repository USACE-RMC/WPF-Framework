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
using GenericControls;

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
    public partial class OpenWindowsDialog : MetroWindow
    {
        /// <summary>
        /// Construct a new open windows dialog.
        /// </summary>
        public OpenWindowsDialog()
        {
            // This call is required by the designer.
            InitializeComponent();
        }

        /// <summary>
        /// Dependency property for the open windows list.
        /// </summary>
        public static DependencyProperty WindowsProperty = DependencyProperty.Register(nameof(Windows), typeof(OpenWindows), typeof(OpenWindowsDialog), new UIPropertyMetadata(null));

        /// <summary>
        /// Gets or sets the open windows list.
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
