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
using System;
using System.Linq;
using System.Windows;

namespace GenericControls
{
    /// <summary>
    /// A dialog for inputting a name with optional validation rules such as character limits, disallowing duplicates, or invalid characters.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b> Authors: </b>
    /// <list type="bullet">
    ///     <item> Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil </item>
    /// </list>
    /// </para>
    /// </remarks>
    public partial class NameDialog
    {
        /// <summary>
        /// Identifies the <see cref="Text"/> dependency property.
        /// </summary>
        public static DependencyProperty TextProperty = DependencyProperty.Register(nameof(Text), typeof(string), typeof(NameDialog), new FrameworkPropertyMetadata(""));
        /// <summary>
        /// Gets/sets the input text from the name textbox.
        /// </summary>
        public string Text
        {
            get
            {
                return (this.GetValue(TextProperty)?.ToString());
            }
            set
            {
                this.SetValue(TextProperty, value);
            }
        }

        /// <summary>
        /// Identifies the <see cref="InnerContent"/> dependency property. 
        /// </summary>
        public static DependencyProperty InnerContentProperty = DependencyProperty.Register(nameof(InnerContent), typeof(object), typeof(NameDialog), new FrameworkPropertyMetadata(null));
        /// <summary>
        /// Gets/sets optional additional content to be shown in the dialog.
        /// </summary>
        public object InnerContent
        {
            get
            {
                return this.GetValue(InnerContentProperty);
            }
            set
            {
                this.SetValue(InnerContentProperty, value);
            }
        }

        /// <summary>
        /// Default constructor. Initializes the dialog with default behavior.
        /// </summary>
        public NameDialog()
        {

            // This call is required by the designer.
            this.InitializeComponent();
            this.ContentRendered += NameDialog_ContentRendered;

            // Add any initialization after the InitializeComponent() call.

        }

        /// <summary>
        /// Constructor that allows setting a character limit.
        /// </summary>
        /// <param name="CharLimit">Maximum number of characters allowed in the name input.</param>
        public NameDialog(int CharLimit)
        {
            this.InitializeComponent();
            this.NameTBox.CharacterLimit = CharLimit;
            this.ContentRendered += NameDialog_ContentRendered;
        }

        /// <summary>
        /// Constructor allowing specification of validation rules for the name input.
        /// </summary>
        /// <param name="charLimit">Maximum number of characters allowed.</param>
        /// <param name="initialText">Initial text to populate the input field.</param>
        /// <param name="canBeBlank">Whether the name field can be left blank.</param>
        /// <param name="existingNames">Array of names that are considered duplicates.</param>
        /// <param name="invalidCharacters">Optional array of characters that are not allowed in the name.</param>
        public NameDialog(int charLimit, string initialText, bool canBeBlank, string[] existingNames, char[] invalidCharacters = null)
        {
            this.InitializeComponent();

            this.NameTBox.CharacterLimit = charLimit;
            this.NameTBox.CanBeBlank = canBeBlank;
            this.NameTBox.InvalidStrings = existingNames;
            if (!(invalidCharacters == null))
                this.NameTBox.InvalidCharacters = invalidCharacters;
            Text = initialText;
            this.ContentRendered += NameDialog_ContentRendered;
        }

        /// <summary>
        /// Handles the OK button click event. Validates the input and closes the dialog if valid.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.NameTBox.IsValid == false)
            {
                MessageBox.Show("Invalid name for the following reasons:" + Environment.NewLine + "\t- " + string.Join("\t- ", this.NameTBox.GetErrorMessages()), "Invalid Name", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            // 
            this.DialogResult = true;
            this.Close();
        }

        /// <summary>
        /// Handles the Cancel button click event. Closes the dialog without saving.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        /// <summary>
        /// Event handler for when the content is rendered. Initializes focus and validation.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NameDialog_ContentRendered(object sender, EventArgs e)
        {
            this.NameTBox.ValidateText();
            this.NameTBox.NameTBox.Focus();
            this.NameTBox.NameTBox.CaretIndex = this.NameTBox.Text.Length;
        }
    }
}