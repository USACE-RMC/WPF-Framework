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
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GenericControls
{
    /// <summary>
    /// A WPF user control that allows the user to selecta line width from a predefined set or via direct input.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b> Authors: </b>
    /// <list type="bullet">
    ///     <item> Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil </item>
    /// </list>
    /// </para>
    /// </remarks>
    public partial class LineWidthSelectorControl :UserControl, INotifyPropertyChanged
    {
        /// <summary>
        /// Identifies the <see cref="SelectedWidth"/> dependency property.
        /// </summary>
        public static DependencyProperty SelectedWidthProperty = DependencyProperty.Register(nameof(SelectedWidth), typeof(double), typeof(LineWidthSelectorControl), new UIPropertyMetadata(0d));
        /// <summary>
        /// gets/sets the selected line width
        /// </summary>
        public double SelectedWidth
        {
            get
            {
                return (double)this.GetValue(SelectedWidthProperty);
            }
            set
            {
                this.SetValue(SelectedWidthProperty, value);
            }
        }

        /// <summary>
        /// Identifies the <see cref="WidthOptions"/> dependency property.
        /// </summary>
        public static DependencyProperty WidthOptionsProperty = DependencyProperty.Register(nameof(WidthOptions), typeof(IList<double>), typeof(LineWidthSelectorControl), new PropertyMetadata(new List<double>(new[] { 0d, 1d, 2d, 3d, 4d, 5d })));
        /// <summary>
        /// gets/sets the list of selectable line width options.
        /// </summary>
        public IList<double> WidthOptions
        {
            get
            {
                return (IList<double>)this.GetValue(WidthOptionsProperty);
            }
            set
            {
                this.SetValue(WidthOptionsProperty, value);
            }
        }

        /// <summary>
        /// Identifies the <see cref="Title"/> dependency property.
        /// </summary>
        public static DependencyProperty TitleProperty = DependencyProperty.Register(nameof(Title), typeof(string), typeof(LineWidthSelectorControl), new UIPropertyMetadata("Title"));
        /// <summary>
        /// gets/sets the label text shown next to the width selector.
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
        /// Identifies the <see cref="MaxPropertyWidth"/> dependency property.
        /// </summary>
        public static DependencyProperty MaxPropertyWidthProperty = DependencyProperty.Register(nameof(MaxPropertyWidth), typeof(double), typeof(LineWidthSelectorControl), new UIPropertyMetadata(PropertyDefaults.DefaultMaxPropertyWidth));
        /// <summary>
        /// gets/sets the maximum width of the label area.
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
        /// Identifies the <see cref="MinPropertyWidth"/> dependency property.
        /// </summary>
        public static DependencyProperty MinPropertyWidthProperty = DependencyProperty.Register(nameof(MinPropertyWidth), typeof(double), typeof(LineWidthSelectorControl), new UIPropertyMetadata(PropertyDefaults.DefaultMinPropertyWidth));
        /// <summary>
        /// gets/sets the column width allocated to the property selector.
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
        /// Identifies the <see cref="PropertyWidth"/> dependency property. 
        /// </summary>
        public static DependencyProperty PropertyWidthProperty = DependencyProperty.Register(nameof(PropertyWidth), typeof(GridLength), typeof(LineWidthSelectorControl), new UIPropertyMetadata(PropertyDefaults.DefaultPropertyWidth));
        /// <summary>
        /// gets/sets the column width allocated to the property selector.
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
        /// Identifies the <see cref="ShowLeaderLine"/> dependency property.
        /// </summary>
        public static DependencyProperty ShowLeaderLineProperty = DependencyProperty.Register(nameof(ShowLeaderLine), typeof(bool), typeof(LineWidthSelectorControl), new UIPropertyMetadata(true));
        /// <summary>
        /// gets/sets a value indicating whether a line should visually connect the title to control.
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

        private double _actualWidth = 0d;
        /// <summary>
        /// Gets the current rendered width of the control.
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
        /// Updates the <see cref="ActualPropertyWidth"/> when th control is resized.
        /// </summary>
        /// <param name="sender">The sender (framework element)</param>
        /// <param name="e">Size changed event data.</param>
        private void ControlSizeChanged(object sender, SizeChangedEventArgs e)
        {
            FrameworkElement el = sender as FrameworkElement;
            ActualPropertyWidth = el.ActualWidth;
        }

        /// <summary>
        /// Restricts ComboBox text input to digits, decimal points, and prevents invalid characters like space.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ComboBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            // Get the TextBox from the ComboBox template to check cursor position
            TextBox editableTextBox = comboBox.Template.FindName("PART_EditableTextBox", comboBox) as TextBox;
            int selectionStart = editableTextBox?.SelectionStart ?? 0;
            string selectedText = editableTextBox?.SelectedText;

            // Line width is positive only, no negative values
            e.Handled = !NumberFormatHelper.IsValidNumericInput(
                e.Text,
                comboBox.Text,
                selectionStart,
                selectedText,
                allowNegative: false,
                allowDecimal: true,
                allowScientific: false);
        }

        /// <summary>
        /// Prevents spacebar key input in the ComboBox.
        /// </summary>
        /// <param name="sender">The ComboBox receiving key input.</param>
        /// <param name="e">Key event data.</param>
        private void ComboBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
                e.Handled = true;
        }
    }
}