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
using System.Windows;
using System.Windows.Controls;

namespace GenericControls
{
    /// <summary>
    /// A user control that allows editing of a <see cref="GridLength"/>, including both value and unit type (Auto, Pixel, Star).
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b> Authors: </b>
    /// <list type="bullet">
    ///     <item> Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil </item>
    /// </list>
    /// </para>
    /// </remarks>
    public partial class GridLengthControl:UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GridLengthControl"/> class.
        /// </summary>
        public GridLengthControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Provides a list of available <see cref="GridUnitType"/> options for use in UI bindings.
        /// </summary>
        public static List<GridUnitType> GridLengthUnitOptions { get; private set; } = new List<GridUnitType>((GridUnitType[])Enum.GetValues(typeof(GridUnitType)));

        /// <summary>
        /// Identifies the <see cref="GridLength"/> dependency property.
        /// </summary>
        public static DependencyProperty GridLengthProperty = DependencyProperty.Register(nameof(GridLength), typeof(GridLength), typeof(GridLengthControl), new FrameworkPropertyMetadata(GridLength.Auto, GridLengthPropertyCallback));
        /// <summary>
        /// Callback when <see cref="GridLength"/> is changed. Updates individual components.
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>    
        private static void GridLengthPropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d == null)
                return;
            if (d.GetType() != typeof(GridLengthControl))
                return;
            GridLengthControl thisControl = (GridLengthControl)d;
            // 
            if (e.NewValue == null)
                return;
            if (e.NewValue.GetType() != typeof(GridLength))
                return;
            GridLength newGridLength = (GridLength)e.NewValue;
            if (thisControl.GridLengthValue != newGridLength.Value)
                thisControl.GridLengthValue = newGridLength.Value;
            if (thisControl.GridLengthUnit != newGridLength.GridUnitType)
                thisControl.GridLengthUnit = newGridLength.GridUnitType;
            // End If
        }

        /// <summary>
        /// Gets/sets the combined <see cref="GridLength"/> (value + unit).
        /// </summary>
        public GridLength GridLength
        {
            get
            {
                return (GridLength)this.GetValue(GridLengthProperty);
            }
            set
            {
                this.SetValue(GridLengthProperty, value);
            }
        }
        /// <summary>
        /// Identifies the <see cref="GridLengthValue"/> dependency property.
        /// </summary>
        public static DependencyProperty GridLengthValueProperty = DependencyProperty.Register(nameof(GridLengthValue), typeof(double), typeof(GridLengthControl), new FrameworkPropertyMetadata(100d, GridLengthValuePropertyCallback));
        /// <summary>
        /// Callback when <see cref="GridLengthValue"/> is changed. Triggers update to <see cref="GridLength"/>
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void GridLengthValuePropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d == null)
                return;
            if (d.GetType() != typeof(GridLengthControl))
                return;
            GridLengthControl thisControl = (GridLengthControl)d;
            // 
            thisControl.UpdateGridLengthProperty();
        }
        /// <summary>
        /// gets/sets the numeric value of the <see cref="GridLength"/>
        /// </summary>
        public double GridLengthValue
        {
            get
            {
                return (double)this.GetValue(GridLengthValueProperty);
            }
            set
            {
                this.SetValue(GridLengthValueProperty, value);
            }
        }

        /// <summary>
        /// Identifies the <see cref="GridLengthUnit"/> dependency property.
        /// </summary>
        public static DependencyProperty GridLengthUnitProperty = DependencyProperty.Register(nameof(GridLengthUnit), typeof(GridUnitType), typeof(GridLengthControl), new FrameworkPropertyMetadata(GridUnitType.Auto, GridLengthUnitPropertyCallback));
        /// <summary>
        /// Callback when <see cref="GridLengthUnit"/> is changed. Triggers update to <see cref="GridLength"/>
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void GridLengthUnitPropertyCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d == null)
                return;
            if (d.GetType() != typeof(GridLengthControl))
                return;
            GridLengthControl thisControl = (GridLengthControl)d;
            // 
            thisControl.UpdateGridLengthProperty();
        }

        /// <summary>
        /// Gets/sets the unit type of the <see cref="GridLength"/> (e.g., Auto, Pixel, Star).
        /// </summary>
        public GridUnitType GridLengthUnit
        {
            get
            {
                return (GridUnitType)(int)this.GetValue(GridLengthUnitProperty);
            }
            set
            {
                this.SetValue(GridLengthUnitProperty, value);
            }
        }
        /// <summary>
        /// Updates the combined <see cref="GridLength"/> property when the value or unit changes. 
        /// </summary>
        private void UpdateGridLengthProperty()
        {
            // Refresh the gridlength
            GridLength = new GridLength(GridLengthValue, GridLengthUnit);
        }

        /// <summary>
        /// Reserved for future use (currently no-op).
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {

        }
    }
}