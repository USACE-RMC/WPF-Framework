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
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace GenericControls
{
    /// <summary>
    /// A custom TextBox control that validates user input based on a character limit,
    /// disallowed characters, disallowed duplicate strings, and blank input rules.
    /// </summary>
    /// <remarks>
    /// <para>
    ///     <b> Authors: </b>
    /// <list type="bullet">
    /// <item><description>
    ///     Woodrow Fields, USACE Risk Management Center, woodrow.l.fields@usace.army.mil
    /// </description></item>
    /// <item><description>
    ///     Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil
    /// </description></item>
    /// </list>
    /// </para>
    /// </remarks>
    public partial class NameTextBox
    {
        /// <summary>
        /// Gets/sets the user-entered text.
        /// </summary>
        public static DependencyProperty TextProperty = DependencyProperty.Register(nameof(Text), typeof(string), typeof(NameTextBox), new FrameworkPropertyMetadata(""));
        /// <summary>
    /// The name string.
    /// </summary>
    /// <returns></returns>
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
        /// gets/sets the maximum number of characters allowed in the text.
        /// Default is 64.
        /// </summary>
        public static DependencyProperty CharacterLimitProperty = DependencyProperty.Register(nameof(CharacterLimit), typeof(int), typeof(NameTextBox), new FrameworkPropertyMetadata(64, ValidationProperty_Callback));
        /// <summary>
    /// Maximum number of characters that the name string can contain. Default is 64 characters.
    /// </summary>
    /// <returns></returns>
        public int CharacterLimit
        {
            get
            {
                return (int)this.GetValue(CharacterLimitProperty);
            }
            set
            {
                this.SetValue(CharacterLimitProperty, value);
            }
        }

        public static DependencyProperty CanBeBlankProperty = DependencyProperty.Register(nameof(CanBeBlank), typeof(bool), typeof(NameTextBox), new FrameworkPropertyMetadata(false, ValidationProperty_Callback));

        /// <summary>
    /// Value indicating if the name string can be blank/empty or not. Default is no.
    /// </summary>
    /// <returns></returns>
        public bool CanBeBlank
        {
            get
            {
                return (bool)this.GetValue(CanBeBlankProperty);
            }
            set
            {
                this.SetValue(CanBeBlankProperty, value);
            }
        }

        public static DependencyProperty InvalidCharactersProperty = DependencyProperty.Register(nameof(InvalidCharacters), typeof(char[]), typeof(NameTextBox), new FrameworkPropertyMetadata(GetDefaultInvalidCharacters(), ValidationProperty_Callback));

        /// <summary>
    /// Array of characters that are invalid. Default is invalid filename characters with the addition of apostrophe, left bracket, and right bracket.
    /// </summary>
    /// <returns></returns>
        public char[] InvalidCharacters
        {
            get
            {
                return (char[])this.GetValue(InvalidCharactersProperty);
            }
            set
            {
                this.SetValue(InvalidCharactersProperty, value);
            }
        }

        public static DependencyProperty InvalidStringsProperty = DependencyProperty.Register(nameof(InvalidStrings), typeof(string[]), typeof(NameTextBox), new FrameworkPropertyMetadata(new string[] { }, ValidationProperty_Callback));
        /// <summary>
    /// Array of strings that are invalid. Default is no invalid strings.
    /// </summary>
    /// <returns></returns>
        public string[] InvalidStrings
        {
            get
            {
                return (string[])this.GetValue(InvalidStringsProperty);
            }
            set
            {
                this.SetValue(InvalidStringsProperty, value);
            }
        }

        public static DependencyProperty IsValidProperty = DependencyProperty.Register(nameof(IsValid), typeof(bool), typeof(NameTextBox), new FrameworkPropertyMetadata(true));
        /// <summary>
    /// Value indicating if the name string can be blank/empty or not. Default is no.
    /// </summary>
    /// <returns></returns>
        public bool IsValid
        {
            get
            {
                return (bool)this.GetValue(IsValidProperty);
            }
            set
            {
                this.SetValue(IsValidProperty, value);
            }
        }

        /// <summary>
        /// Invoked when validation-related properties are changed.
        /// Re-validates the text. 
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void ValidationProperty_Callback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d == null)
                return;
            if (d.GetType() != typeof(NameTextBox))
                return;
            NameTextBox thisControl = (NameTextBox)d;
            thisControl.ValidateText();
        }

        /// <summary>
    /// Shared function to get the default invalid characters for the name textbox. invalid characters includes invalid file name characters, apostraphe, left bracket, and right bracket.
    /// </summary>
    /// <returns>array of default invalid name characters</returns>
        public static char[] GetDefaultInvalidCharacters()
        {
            var invalidCharsList = new List<char>(System.IO.Path.GetInvalidFileNameChars());
            invalidCharsList.Add('\'');
            invalidCharsList.Add('[');
            invalidCharsList.Add(']');
            return invalidCharsList.ToArray();
        }

        /// <summary>
        /// Raised when the user modifies the text in the control.
        /// </summary>
        public event TextChangedEventHandler TextChanged;

        /// <summary>
        /// Delegate signature for the TextChanged event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public delegate void TextChangedEventHandler(object sender, TextChangedEventArgs e);

        /// <summary>
        /// Initializes a new instance of the <see cref="NameTextBox"/> control. 
        /// </summary>
        public NameTextBox()
        {

            // This call is required by the designer.
            this.InitializeComponent();

            // Add any initialization after the InitializeComponent() call.
            ValidateText();
        }

        /// <summary>
        /// Handles the internal TextBox's text change event. Triggers validation.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NameTBox_TextChanged(object sender, TextChangedEventArgs e)
        {

            TextChanged?.Invoke(sender, e);

            ValidateText();
        }

        /// <summary>
        /// Validates the text based on rules for character limit, blank string, 
        /// duplicate names, and invalid characters. Updates the IsValid flag and tooltip.
        /// </summary>
        public void ValidateText()
        {
            this.NameTBox.ToolTip = (object)null;
            // 
            if (this.NameTBox.Text.Length > CharacterLimit & CharacterLimit > -1)
            {
                this.NameTBox.ToolTip = "The name entered is longer (" + this.NameTBox.Text.Length + " characters) than the maximum name length (" + CharacterLimit + " characters).";
                IsValid = false;
                return;
            }
            // 
            if (string.IsNullOrEmpty(this.NameTBox.Text) & CanBeBlank == false)
            {
                this.NameTBox.ToolTip = "The name entered cannot be blank. The name entered must not be blank and must be less than " + CharacterLimit + " characters.";
                IsValid = false;
                return;
            }
            // 
            if (InvalidStrings.Contains(this.NameTBox.Text))
            {
                this.NameTBox.ToolTip = "Name entered already exists and must be unique.";
                IsValid = false;
                return;
            }
            // 
            foreach (char badChar in InvalidCharacters)
            {
                if (this.NameTBox.Text.Contains(badChar))
                {
                    this.NameTBox.ToolTip = "Invalid character in name: '" + badChar + "'";
                    IsValid = false;
                    return;
                }
            }
            // 
            IsValid = true;
        }
        /// <summary>
    /// Get all error messages associated with the text in the name textbox.
    /// </summary>
    /// <returns>A list of error messages.</returns>
        public List<string> GetErrorMessages()
        {
            var errorList = new List<string>();
            if (IsValid == true)
                return errorList;
            // 
            if (this.NameTBox.Text.Length > CharacterLimit & CharacterLimit > -1)
            {
                errorList.Add("The name entered is longer (" + this.NameTBox.Text.Length + " characters) than the maximum name length (" + CharacterLimit + " characters).");
            }
            // 
            if (string.IsNullOrEmpty(this.NameTBox.Text) & CanBeBlank == false)
            {
                errorList.Add("The name entered cannot be blank. The name entered must not be blank and must be less than " + CharacterLimit + " characters.");
            }
            // 
            if (InvalidStrings.Contains(this.NameTBox.Text))
            {
                errorList.Add("Name entered already exists and must be unique.");
            }
            // 
            foreach (char badChar in InvalidCharacters)
            {
                if (this.NameTBox.Text.Contains(badChar))
                {
                    errorList.Add("Invalid character in name: '" + badChar + "'");
                }
            }
            // 
            return errorList;
        }

        /// <summary>
        /// Selects all text in the internal TextBox.
        /// </summary>
        public void SelectAll()
        {
            this.NameTBox.SelectAll();
        }

        /// <summary>
        /// Updates the source binding when Enter is pressed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                TextBox tBox = (TextBox)sender;
                var binding = BindingOperations.GetBindingExpression(tBox, TextBox.TextProperty);
                if (binding is not null)
                    binding.UpdateSource();
            }
        }

    }
}