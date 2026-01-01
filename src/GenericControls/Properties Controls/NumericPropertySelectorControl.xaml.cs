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
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace GenericControls
{
    /// <summary>
    /// A control that allows users to select or enter a numeric value from a predefined list.
    /// Supports validation, editable input, and custom formatting.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b> Authors: </b>
    /// <list type="bullet">
    ///     <item> Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil </item>
    /// </list>
    /// </para>
    /// </remarks>
    public partial class NumericPropertySelectorControl : UserControl, INotifyPropertyChanged
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NumericPropertySelectorControl"/> class.
        /// </summary>
        public NumericPropertySelectorControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Identifies the <see cref="SelectedNumber"/>
        /// </summary>
        public static DependencyProperty SelectedNumberProperty = DependencyProperty.Register(nameof(SelectedNumber), typeof(double), typeof(NumericPropertySelectorControl), new UIPropertyMetadata(0d));
        /// <summary>
        /// gets/sets the currently selected number.
        /// </summary>
        public double SelectedNumber
        {
            get
            {
                return (double)this.GetValue(SelectedNumberProperty);
            }
            set
            {
                this.SetValue(SelectedNumberProperty, value);
            }
        }

        /// <summary>
        /// Identifies the <see cref="NumericOptions"/> dependency property.
        /// </summary>
        public static DependencyProperty NumericOptionsProperty = DependencyProperty.Register(nameof(NumericOptions), typeof(IList<double>), typeof(NumericPropertySelectorControl), new PropertyMetadata(new List<double>(new[] { 0d, 1d, 2d, 3d, 4d, 5d })));
        /// <summary>
        /// gets/sets the list of numeric options available for selection. 
        /// </summary>
        public IList<double> NumericOptions
        {
            get
            {
                return (IList<double>)this.GetValue(NumericOptionsProperty);
            }
            set
            {
                this.SetValue(NumericOptionsProperty, value);
            }
        }

        /// <summary>
        /// Identifies the <see cref="IsEditable"/> dependency property.
        /// </summary>
        public static DependencyProperty IsEditableProperty = DependencyProperty.Register(nameof(IsEditable), typeof(bool), typeof(NumericPropertySelectorControl), new PropertyMetadata(true));
        /// <summary>
        /// gets/sets a value indicating whether the combo box is editable.
        /// </summary>
        public bool IsEditable
        {
            get
            {
                return (bool)this.GetValue(IsEditableProperty);
            }
            set
            {
                this.SetValue(IsEditableProperty, value);
            }
        }

        /// <summary>
        /// Identifies the <see cref="CanHaveNegative"/> dependency property.
        /// </summary>
        public static DependencyProperty CanHaveNegativeProperty = DependencyProperty.Register(nameof(CanHaveNegative), typeof(bool), typeof(NumericPropertySelectorControl), new PropertyMetadata(true));
        /// <summary>
        /// gets/sets a value indicating whether negative numbers are allowed.
        /// </summary>
        public bool CanHaveNegative
        {
            get
            {
                return (bool)this.GetValue(CanHaveNegativeProperty);
            }
            set
            {
                this.SetValue(CanHaveNegativeProperty, value);
            }
        }

        /// <summary>
        /// Identifies the <see cref="Title"/> dependency property.
        /// </summary>
        public static DependencyProperty TitleProperty = DependencyProperty.Register(nameof(Title), typeof(string), typeof(NumericPropertySelectorControl), new UIPropertyMetadata("Title"));
        /// <summary>
        /// Gets/sets the title label for the control.
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
        public static DependencyProperty MaxPropertyWidthProperty = DependencyProperty.Register(nameof(MaxPropertyWidth), typeof(double), typeof(NumericPropertySelectorControl), new UIPropertyMetadata(PropertyDefaults.DefaultMaxPropertyWidth));
        /// <summary>
        /// gets/sets the maximum allowed width for the property field.
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
        public static DependencyProperty MinPropertyWidthProperty = DependencyProperty.Register(nameof(MinPropertyWidth), typeof(double), typeof(NumericPropertySelectorControl), new UIPropertyMetadata(PropertyDefaults.DefaultMinPropertyWidth));
        /// <summary>
        /// gets/sets the minimum allowed width for the property field.
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
        public static DependencyProperty PropertyWidthProperty = DependencyProperty.Register(nameof(PropertyWidth), typeof(GridLength), typeof(NumericPropertySelectorControl), new UIPropertyMetadata(PropertyDefaults.DefaultPropertyWidth));
        /// <summary>
        /// gets/sets the width of the value field.
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
        public static DependencyProperty ShowLeaderLineProperty = DependencyProperty.Register(nameof(ShowLeaderLine), typeof(bool), typeof(NumericPropertySelectorControl), new UIPropertyMetadata(true));
        /// <summary>
        /// gets/sets a value indicating whether a leader line should be displayed.
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
        /// Gets the current rendered width of the property control.
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
        /// <inheritdoc/>
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Updates property width if control size is changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ControlSizeChanged(object sender, SizeChangedEventArgs e)
        {
            FrameworkElement el = sender as FrameworkElement;
            ActualPropertyWidth = el.ActualWidth;
        }

        /// <summary>
        /// ComboBox preview text input.
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

            e.Handled = !NumberFormatHelper.IsValidNumericInput(
                e.Text,
                comboBox.Text,
                selectionStart,
                selectedText,
                CanHaveNegative,
                allowDecimal: true,
                allowScientific: false);
        }

        /// <summary>
        /// Combo box preview when Space is pressed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ComboBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
                e.Handled = true;
        }

        /// <summary>
        /// Workaround to allow single-click editing in editable ComboBox within DataGrid cells.
        /// Some contexts (e.g., DataGrid TemplateColumn, AvalonDock) consume MouseUp events
        /// before they reach the ComboBox TextBox.
        /// </summary>
        private bool _previewUp = false;

        private void NumericPropertySelector_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (IsEditable == false)
                return;
            _previewUp = true;
        }

        private void NumericPropertySelector_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (IsEditable == false)
                return;
            if (_previewUp == true)
            {
                TextBox tb = (TextBox)this.NumericComboBox.Template.FindName("PART_EditableTextBox", this.NumericComboBox);
                tb.Focus();
            }
            _previewUp = false;
        }

    }

    /// <summary>
    /// Converts between double values and their string representations for use in data bindings.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b> Authors: </b>
    /// <list type="bullet">
    ///     <item> Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil </item>
    /// </list>
    /// </para>
    /// </remarks>
    public class DoubleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return Binding.DoNothing;
            double d;
            if (NumberFormatHelper.TryParseDouble(value.ToString(), out d) == false)
                return Binding.DoNothing;
            return d;
        }
    }
}