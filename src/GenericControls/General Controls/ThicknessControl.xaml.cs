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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace GenericControls
{
    /// <summary>
    /// A user control for adjusting individual sides of a <see cref="Thickness"/> value (Left, Top, Right, Bottom).
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
    public partial class ThicknessControl:UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ThicknessControl"/> class.
        /// </summary>
        public ThicknessControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Dependency property for the selected <see cref="Thickness"/>
        /// </summary>
        public static DependencyProperty ThicknessProperty = DependencyProperty.Register(nameof(SelectedThickness), typeof(Thickness), typeof(ThicknessControl), new UIPropertyMetadata(new Thickness(1d)));
        /// <summary>
        /// gets/sets the selected <see cref="Thickness"/> value.
        /// </summary>
        public Thickness SelectedThickness
        {
            get
            {
                return (Thickness)this.GetValue(ThicknessProperty);
            }
            set
            {
                this.SetValue(ThicknessProperty, value);
            }
        }

        /// <summary>
        /// Handles input preview on the text boxes to ensure only valid numeric input is accepted.
        /// Allows digits and one decimal point. Blocks spaces and non-numeric characters.
        /// </summary>
        /// <param name="sender">The text box receiving input.</param>
        /// <param name="e">The text composition event arguments.</param>
        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            TextBox tBox = (TextBox)sender;
            // Thickness values are positive only, so we don't allow negative
            e.Handled = !NumberFormatHelper.IsValidNumericInput(
                e.Text,
                tBox.Text,
                tBox.SelectionStart,
                tBox.SelectedText,
                allowNegative: false,
                allowDecimal: true,
                allowScientific: false);
        }

        /// <summary>
        /// Previews the space key from being used inside the thickness text boxes.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The key event arguments.</param>
        private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
                e.Handled = true;
        }

        /// <summary>
        /// Updates the corresponding side of the <see cref="SelectedThickness"/> when any thickness text box value is changed.
        /// </summary>
        /// <param name="sender">The text box that was changed.</param>
        /// <param name="e">The text changed event arguments.</param>
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox tBox = (TextBox)sender;
            double dblValue;
            if (NumberFormatHelper.TryParseDouble(tBox.Text, out dblValue))
            {
                switch (tBox.Name ?? "")
                {
                    case var @case when @case == (this.LeftThicknessTextBox.Name ?? ""):
                        {
                            SelectedThickness = new Thickness(dblValue, SelectedThickness.Top, SelectedThickness.Right, SelectedThickness.Bottom);
                            break;
                        }
                    case var case1 when case1 == (this.TopThicknessTextBox.Name ?? ""):
                        {
                            SelectedThickness = new Thickness(SelectedThickness.Left, dblValue, SelectedThickness.Right, SelectedThickness.Bottom);
                            break;
                        }
                    case var case2 when case2 == (this.RightThicknessTextBox.Name ?? ""):
                        {
                            SelectedThickness = new Thickness(SelectedThickness.Left, SelectedThickness.Top, dblValue, SelectedThickness.Bottom);
                            break;
                        }
                    case var case3 when case3 == (this.BottomThicknessTextBox.Name ?? ""):
                        {
                            SelectedThickness = new Thickness(SelectedThickness.Left, SelectedThickness.Top, SelectedThickness.Right, dblValue);
                            break;
                        }
                }
            }
        }
    }
}