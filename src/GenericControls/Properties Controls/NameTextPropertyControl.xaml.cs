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
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GenericControls
{

    public partial class NameTextPropertyControl :UserControl, INotifyPropertyChanged
    {


        #region Construction

        #endregion

        #region Members

        /// <summary>
    /// Dependency property for the character limit property.
    /// </summary>
        public static DependencyProperty CharacterLimitProperty = DependencyProperty.Register(nameof(CharacterLimit), typeof(int), typeof(NameTextPropertyControl), new FrameworkPropertyMetadata(64));

        /// <summary>
    /// Maximum number of characters that the name string can contain. Default is 64 characters.
    /// </summary>
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

        /// <summary>
    /// Dependency property for the existing names property.
    /// </summary>
        public static DependencyProperty ExistingNamesProperty = DependencyProperty.Register(nameof(ExistingNames), typeof(string[]), typeof(NameTextPropertyControl), new FrameworkPropertyMetadata(new string[] { }));

        /// <summary>
    /// Array of strings that are invalid. Default is no invalid strings.
    /// </summary>
        public string[] ExistingNames
        {
            get
            {
                return (string[])this.GetValue(ExistingNamesProperty);
            }
            set
            {
                this.SetValue(ExistingNamesProperty, value);
            }
        }

        /// <summary>
    /// Dependency property for the invalid characters property. 
    /// </summary>
        public static DependencyProperty InvalidCharactersProperty = DependencyProperty.Register(nameof(InvalidCharacters), typeof(char[]), typeof(NameTextPropertyControl), new FrameworkPropertyMetadata(NameTextBox.GetDefaultInvalidCharacters()));

        /// <summary>
    /// Array of characters that are invalid. Default is invalid filename characters with the addition of apostrophe, left bracket, and right bracket.
    /// </summary>
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

        /// <summary>
    /// Dependency property for the text property.
    /// </summary>
        public static DependencyProperty TextProperty = DependencyProperty.Register(nameof(Text), typeof(string), typeof(NameTextPropertyControl), new UIPropertyMetadata(""));

        /// <summary>
    /// Gets and sets the text.
    /// </summary>
        public string Text
        {
            get
            {
                return (string)this.GetValue(TextProperty);
            }
            set
            {
                this.SetValue(TextProperty, value);
            }
        }

        /// <summary>
    /// Dependency property for the title property.
    /// </summary>
        public static DependencyProperty TitleProperty = DependencyProperty.Register(nameof(Title), typeof(string), typeof(NameTextPropertyControl), new UIPropertyMetadata("Title"));

        /// <summary>
    /// Gets and sets the title.
    /// </summary>
        public string Title
        {
            get
            {
                return (string)this.GetValue(TitleProperty);
            }
            set
            {
                this.SetValue(TitleProperty, value);
            }
        }


        /// <summary>
    /// Dependency property for the max width property.
    /// </summary>
        public static DependencyProperty PropertyWidthProperty = DependencyProperty.Register(nameof(PropertyWidth), typeof(GridLength), typeof(NameTextPropertyControl), new UIPropertyMetadata(PropertyDefaults.DefaultPropertyWidth));

        /// <summary>
    /// Gets and sets the max width of the control.
    /// </summary>
        public GridLength PropertyWidth
        {
            get
            {
                return (GridLength)this.GetValue(PropertyWidthProperty);
            }
            set
            {
                this.SetValue(PropertyWidthProperty, value);
            }
        }

        /// <summary>
    /// Dependency property for the max width property.
    /// </summary>
        public static DependencyProperty MaxPropertyWidthProperty = DependencyProperty.Register(nameof(MaxPropertyWidth), typeof(double), typeof(NameTextPropertyControl), new UIPropertyMetadata(PropertyDefaults.DefaultMaxPropertyWidth));

        /// <summary>
    /// Gets and sets the max width of the control.
    /// </summary>
        public double MaxPropertyWidth
        {
            get
            {
                return (double)this.GetValue(MaxPropertyWidthProperty);
            }
            set
            {
                this.SetValue(MaxPropertyWidthProperty, value);
            }
        }

        /// <summary>
    /// Dependency property for the min width property.
    /// </summary>
        public static DependencyProperty MinPropertyWidthProperty = DependencyProperty.Register(nameof(MinPropertyWidth), typeof(double), typeof(NameTextPropertyControl), new UIPropertyMetadata(PropertyDefaults.DefaultMinPropertyWidth));

        /// <summary>
    /// Gets and sets the min width of the control.
    /// </summary>
        public double MinPropertyWidth
        {
            get
            {
                return (double)this.GetValue(MinPropertyWidthProperty);
            }
            set
            {
                this.SetValue(MinPropertyWidthProperty, value);
            }
        }

        /// <summary>
    /// Dependency property for the show leader line property.
    /// </summary>
        public static DependencyProperty ShowLeaderLineProperty = DependencyProperty.Register(nameof(ShowLeaderLine), typeof(bool), typeof(NameTextPropertyControl), new UIPropertyMetadata(true));

        /// <summary>
    /// Determines of the leader line should be visible. 
    /// </summary>
        public bool ShowLeaderLine
        {
            get
            {
                return (bool)this.GetValue(ShowLeaderLineProperty);
            }
            set
            {
                this.SetValue(ShowLeaderLineProperty, value);
            }
        }

        #endregion
        private double _actualWidth = 0d;
        /// <summary>
        /// Get current rendered width of the control.
        /// </summary>
        public double ActualPropertyWidth
        {
            get
            {
                return _actualWidth;
            }
            private set
            {
                if (_actualWidth != value)
                {
                    _actualWidth = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ActualPropertyWidth)));
                }
            }
        }

        /// <summary>
        /// Event raised when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Updates the <see cref="ActualPropertyWidth"/> when the control is resized.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ControlSizeChanged(object sender, SizeChangedEventArgs e)
        {
            FrameworkElement el = sender as FrameworkElement;
            ActualPropertyWidth = el.ActualWidth;
        }

        /// <summary>
    /// Place focus on the text box and set the caret position.
    /// </summary>
    /// <param name="caretIndex"></param>
        public void TextBoxFocus(int caretIndex)
        {
            Keyboard.Focus(this.NameTextBox.NameTBox);
            this.NameTextBox.NameTBox.CaretIndex = caretIndex;
            this.NameTextBox.NameTBox.Focus();
        }

        /// <summary>
        /// Allows to traverse in the NameTextBox when Enter is pressed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NameTextBox_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                // Dim tBox As NameTextBox = DirectCast(sender, NameTextBox)
                // Dim binding As BindingExpression = BindingOperations.GetBindingExpression(tBox, NameTextBox.TextProperty)
                // If binding IsNot Nothing Then binding.UpdateSource()
                ((UIElement)e.OriginalSource).MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
            }
        }
    }
}