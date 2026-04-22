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

namespace GenericControls
{
    /// <summary>
    /// A user control that displays a titled checkbox allowing the user to toggle a boolean property.
    /// Includes optional leader line and customizable checked/unchecked events.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b> Authors: </b>
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
    public partial class BooleanPropertyControl:UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BooleanPropertyControl"/> class.
        /// </summary>
        public BooleanPropertyControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Dependency property for the selected state of the checkbox.
        /// </summary>
        public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.Register(nameof(IsSelected), typeof(bool), typeof(BooleanPropertyControl), new UIPropertyMetadata(true));
        /// <summary>
        /// Gets or sets whether the checkbox is selected.
        /// </summary>
        public bool IsSelected
        {
            get
            {
                return (bool)this.GetValue(IsSelectedProperty);
            }
            set
            {
                this.SetValue(IsSelectedProperty, value);
            }
        }

        /// <summary>
        /// Dependency property for the display title associated with the control. 
        /// </summary>
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(nameof(Title), typeof(string), typeof(BooleanPropertyControl), new UIPropertyMetadata("Title"));
        /// <summary>
        /// Gets or sets the title text shown next to the checkbox.
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
        /// Dependency property to control the visibility of the leader line.
        /// </summary>
        public static readonly DependencyProperty ShowLeaderLineProperty = DependencyProperty.Register(nameof(ShowLeaderLine), typeof(bool), typeof(BooleanPropertyControl), new UIPropertyMetadata(true));
        /// <summary>
        /// Gets or sets whether the leader line should be visible.
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

        /// <summary>
        /// Occurs when the checkbox is checked 
        /// </summary>
        public event CheckedEventHandler Checked;

        /// <summary>
        /// Delegate for the <see cref="Checked"/> event.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event arguments.</param>
        public delegate void CheckedEventHandler(object sender, RoutedEventArgs e);

        /// <summary>
        /// Occurs when the checkbox is unchecked.
        /// </summary>
        public event UncheckedEventHandler Unchecked;

        /// <summary>
        /// Delegate for the <see cref="Unchecked"/> event.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event arguments.</param>
        public delegate void UncheckedEventHandler(object sender, RoutedEventArgs e);

        /// <summary>
        /// Handles the <see cref="Checked"/> event and invokes the event.
        /// </summary>
        /// <param name="sender">The checkbox triggering the event.</param>
        /// <param name="e">The event arguments.</param>
        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            Checked?.Invoke(this, e);
        }

        /// <summary>
        /// Handles the <see cref="Unchecked"/> event and invokes the event.
        /// </summary>
        /// <param name="sender">The checkbox triggering the event.</param>
        /// <param name="e">The event arguments.</param>
        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            Unchecked?.Invoke(this, e);
        }
    }
}