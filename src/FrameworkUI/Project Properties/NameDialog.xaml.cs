/*
* NOTICE:
* The U.S. Army Corps of Engineers, Risk Management Center (USACE-RMC) makes no guarantees about
* the results, or appropriateness of outputs, obtained from this software.
*
* LIST OF CONDITIONS:
* Redistribution and use in source and binary forms, with or without modification, are permitted
* provided that the following conditions are met:
* ? Redistributions of source code must retain the above notice, this list of conditions, and the
* following disclaimer.
* ? Redistributions in binary form must reproduce the above notice, this list of conditions, and
* the following disclaimer in the documentation and/or other materials provided with the distribution.
* ? The names of the U.S. Government, the U.S. Army Corps of Engineers, the Institute for Water
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
using System.Windows.Input;

namespace FrameworkUI
{
    /// <summary>
    /// Interaction logic for NameDialog.xaml
    /// </summary>
    public partial class NameDialog : Window
    {
        /// <summary>
        /// Initializes a new instance of the NameDialog class.
        /// </summary>
        public NameDialog()
        {
            InitializeComponent();
            // Required window functionality
            CommandBindings.Add(new CommandBinding(SystemCommands.CloseWindowCommand, OnCloseWindow));
        }

        /// <summary>
        /// Initializes a new instance of the NameDialog class with a character limit.
        /// </summary>
        /// <param name="charLimit">The maximum number of characters allowed.</param>
        public NameDialog(int charLimit)
        {
            InitializeComponent();
            // Required window functionality
            CommandBindings.Add(new CommandBinding(SystemCommands.CloseWindowCommand, OnCloseWindow));
            NameTBox.CharacterLimit = charLimit;
        }

        /// <summary>
        /// Initializes a new instance of the NameDialog class with validation parameters.
        /// </summary>
        /// <param name="charLimit">The maximum number of characters allowed.</param>
        /// <param name="initialText">The initial text to display.</param>
        /// <param name="canBeBlank">Whether the text can be blank.</param>
        /// <param name="existingNames">Array of existing names that are not allowed.</param>
        /// <param name="invalidCharacters">Optional array of characters that are not allowed.</param>
        public NameDialog(int charLimit, string initialText, bool canBeBlank, string[] existingNames, char[]? invalidCharacters = null)
        {
            InitializeComponent();
            // Required window functionality
            CommandBindings.Add(new CommandBinding(SystemCommands.CloseWindowCommand, OnCloseWindow));
            NameTBox.CharacterLimit = charLimit;
            NameTBox.CanBeBlank = canBeBlank;
            NameTBox.InvalidStrings = existingNames;
            if (invalidCharacters != null) NameTBox.InvalidCharacters = invalidCharacters;
            Text = initialText;
        }

        /// <summary>
        /// Dependency property for the Text property.
        /// </summary>
        public static DependencyProperty TextProperty = DependencyProperty.Register(nameof(Text), typeof(string), typeof(NameDialog), new FrameworkPropertyMetadata(""));

        /// <summary>
        /// Gets or sets the text value.
        /// </summary>
        public string Text
        {
            get { return (string)GetValue(TextProperty); }

            set{ SetValue(TextProperty, value);}
        }

        /// <summary>
        /// Dependency property for the InnerContent property.
        /// </summary>
        public static DependencyProperty InnerContentProperty = DependencyProperty.Register(nameof(InnerContent), typeof(object), typeof(NameDialog), new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Gets or sets the inner content of the dialog.
        /// </summary>
        public object InnerContent
        {
            get { return GetValue(InnerContentProperty); }

            set { SetValue(InnerContentProperty, value);}
        }

        /// <summary>
        /// Handles the window close command.
        /// </summary>
        /// <param name="target">The command target.</param>
        /// <param name="e">The executed routed event arguments.</param>
        private void OnCloseWindow(object target, ExecutedRoutedEventArgs e)
        {
            SystemCommands.CloseWindow(this);
        }

        /// <summary>
        /// Handles the OK button click event.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The routed event arguments.</param>
        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            if (NameTBox.IsValid == false)
            {
                GenericControls.MessageBox.Show("Invalid name for the following reasons:" + Environment.NewLine + "\t" + "- " + string.Join("\t" + "- ", this.NameTBox.GetErrorMessages()), "Invalid Name", MessageBoxButton.OKCancel, MessageBoxImage.Error);
                return;
            }
            //
            DialogResult = true;
            Close();
        }

        /// <summary>
        /// Handles the Cancel button click event.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The routed event arguments.</param>
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        /// <summary>
        /// Handles the content rendered event to validate and focus the name text box.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void NameDialog_ContentRendered(object sender, EventArgs e)
        {
            NameTBox.ValidateText();
            NameTBox.NameTBox.Focus();
            NameTBox.NameTBox.CaretIndex = NameTBox.Text.Length;
        }

    }
}
