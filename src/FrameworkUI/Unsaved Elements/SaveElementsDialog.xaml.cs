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

using System.Collections.ObjectModel;
using System.Windows;
using GenericControls;

namespace FrameworkUI
{
    /// <summary>
    /// Interaction logic for SaveElementsDialog.xaml
    /// </summary>
    /// <remarks>
    /// <para>
    ///     Authors:
    ///     Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil
    /// </para>
    /// </remarks>
    public partial class SaveElementsDialog : MetroWindow
    {
        /// <summary>
        /// Construct a new save elements dialog.
        /// </summary>
        public SaveElementsDialog()
        {
            // This call is required by the designer.
            InitializeComponent();
        }

        /// <summary>
        /// Dependency property for the save elements list.
        /// </summary>
        public static DependencyProperty ElementItemsProperty = DependencyProperty.Register(nameof(ElementItems), typeof(ObservableCollection<UnsavedElement>), typeof(SaveElementsDialog), new UIPropertyMetadata(null));

        /// <summary>
        /// Gets or sets the save elements list.
        /// </summary>
        public ObservableCollection<UnsavedElement> ElementItems
        {
            get { return (ObservableCollection<UnsavedElement>)GetValue(ElementItemsProperty); }
            set
            {
                SetValue(ElementItemsProperty, value);
                MyDataGrid.ItemsSource = null;
                MyDataGrid.ItemsSource = ElementItems;
            }
        }

        /// <summary>
        /// Enumeration of dialog result types.
        /// </summary>
        public enum DialogResultType
        {
            YesSave,
            NoSave,
            Cancel
        }

        /// <summary>
        /// Gets the save elements dialog result.
        /// </summary>
        public DialogResultType Result { get; private set; } = DialogResultType.Cancel;

        /// <summary>
        /// On Save, save all elements, set dialog result to true and close.
        /// </summary>
        private void Btn_Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Save all elements
                for (int i = 0; i < ElementItems.Count; i++)
                    ElementItems[i].Element.Save();
                Result = DialogResultType.YesSave;
            }
            catch (Exception ex)
            {
                Result = DialogResultType.Cancel;
                MessageBox.Show(ex.ToString());
            }
            Close();
        }

        /// <summary>
        /// On Don't Save, set dialog result to true and close. Do not save.
        /// </summary>
        private void Btn_DontSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Instead of saving, restore the last saved inputs from disk.
                for (int i = 0; i < ElementItems.Count; i++)
                    ElementItems[i].Element.Open();
                Result = DialogResultType.NoSave;
            }
            catch (Exception ex)
            {
                Result = DialogResultType.Cancel;
                MessageBox.Show(ex.ToString());
            }
            Close();
        }

        /// <summary>
        /// On Cancel, set dialog result to false and close.
        /// </summary>
        private void Btn_Cancel_Click(object sender, RoutedEventArgs e)
        {
            Result = DialogResultType.Cancel;
            Close();
        }

    }
}
