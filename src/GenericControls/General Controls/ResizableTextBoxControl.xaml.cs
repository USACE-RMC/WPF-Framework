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
using System.Windows.Data;
using System.Windows.Input;

namespace GenericControls
{
    /// <summary>
    /// A user control that wraps resizable text box with support for read-only mode and 
    /// dynamic height adjustment using a drag handle.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b> Authors: </b>
    /// <list type="bullet">
    ///     <item> Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil </item>
    /// </list>
    /// </para>
    /// </remarks>
    public partial class ResizableTextBoxControl
    {
        /// <summary>
        /// Dependency property for the text content of the control.
        /// </summary>
        public static DependencyProperty TextProperty = DependencyProperty.Register(nameof(Text), typeof(string), typeof(ResizableTextBoxControl), new UIPropertyMetadata(""));
        /// <summary>
        /// Gets/sets the text content of the control.
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
        /// Dependency property for determining whether the text box is read-only.
        /// </summary>
        public static DependencyProperty IsReadOnlyProperty = DependencyProperty.Register(nameof(IsReadOnly), typeof(bool), typeof(ResizableTextBoxControl), new UIPropertyMetadata(false));
        /// <summary>
        /// gets/sets a value indicating whether the text box is read-only.
        /// </summary>
        public bool IsReadOnly
        {
            get
            {
                return (bool)this.GetValue(IsReadOnlyProperty);
            }
            set
            {
                this.SetValue(IsReadOnlyProperty, value);
            }
        }

        // Private _isResizing As Boolean = False
        // Private _startPosition As Point

        // Private Sub ResizeGripper_MouseLeftButtonDown(sender As Object, e As MouseButtonEventArgs)
        // If Mouse.Capture(ResizeGripper) Then
        // _isResizing = True
        // _startPosition = e.GetPosition(Me)
        // End If
        // End Sub

        // Private Sub ResizeGripper_MouseMove(sender As Object, e As MouseEventArgs)
        // If _isResizing Then
        // Dim currentPosition As Point = e.GetPosition(Me)
        // Dim diffY As Double = currentPosition.Y - _startPosition.Y
        // Height += diffY
        // _startPosition = currentPosition
        // End If
        // End Sub

        // Private Sub ResizeGripper_MouseLeftButtonUp(sender As Object, e As MouseButtonEventArgs)
        // If _isResizing = True Then
        // _isResizing = False
        // End If
        // End Sub

        /// <summary>
        /// Handles the drag movement of the resize thumb and adjusts the control's height accordingly.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ResizeThumb_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
        {
            double newHeight = this.ActualHeight + e.VerticalChange;
            if (newHeight < 18d)
                newHeight = 18d;
            if (newHeight > this.MaxHeight)
                newHeight = this.MaxHeight;
            this.Height = newHeight;
        }

        /// <summary>
        /// Updates the data binding source when Enter is pressed in the text box.
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