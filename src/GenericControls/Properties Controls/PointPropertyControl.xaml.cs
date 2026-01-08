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

namespace GenericControls
{
    /// <summary>
    /// A WPF control for displaying and editing a 2D <see cref="Point"/> with configurable precision and layout properties.
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
    public partial class PointPropertyControl:UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PointPropertyControl"/> class.
        /// </summary>
        public PointPropertyControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Identifies the <see cref="Decimals"/> dependency property.
        /// </summary>
        public static DependencyProperty DecimalsProperty = DependencyProperty.Register(nameof(Decimals), typeof(int), typeof(PointPropertyControl), new UIPropertyMetadata(5, InitializeControl));
        /// <summary>
        /// gets/sets the number of decimal places to display for the point values.
        /// </summary>
        public int Decimals
        {
            get
            {
                return (int)this.GetValue(DecimalsProperty);
            }
            set
            {
                this.SetValue(DecimalsProperty, value);
            }
        }

        /// <summary>
        /// Identifies the <see cref="DataPoint"/> dependency property.
        /// </summary>
        public static DependencyProperty DataPointProperty = DependencyProperty.Register(nameof(DataPoint), typeof(Point), typeof(PointPropertyControl), new UIPropertyMetadata(new Point(0d, 0d), InitializeControl));
        /// <summary>
        /// Gets/sets the 2D point value.
        /// </summary>
        public Point DataPoint
        {
            get
            {
                return (Point)this.GetValue(DataPointProperty);
            }
            set
            {
                this.SetValue(DataPointProperty, value);
            }
        }

        /// <summary>
        /// Intitializes the control when a dependency property changes.
        /// Updates the UI and prevents recursive updates.
        /// </summary>
        /// <param name="d">The dependency object.</param>
        /// <param name="e">The event arguments.</param>
        private static void InitializeControl(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d == null)
                return;
            if (d.GetType() != typeof(PointPropertyControl))
                return;
            PointPropertyControl thisControl = (PointPropertyControl)d;

            // Check if child controls exist yet (may be called before InitializeComponent)
            if (thisControl.DataPointX == null || thisControl.DataPointY == null)
                return;

            Point newDataPoint = thisControl.DataPoint;

            // Update the textboxes with the new values
            // Remove the handlers so the property doesn't get triggered for update.
            thisControl.DataPointX.TextChanged -= thisControl.DataPointX_TextChanged;
            thisControl.DataPointY.TextChanged -= thisControl.DataPointY_TextChanged;
            // Update the values in the textboxes
            thisControl.DataPointX.Text = NumberFormatHelper.FormatDouble(Math.Round(newDataPoint.X, thisControl.Decimals));
            thisControl.DataPointY.Text = NumberFormatHelper.FormatDouble(Math.Round(newDataPoint.Y, thisControl.Decimals));
            // Add the handlers back for updating back to source.
            thisControl.DataPointX.TextChanged += thisControl.DataPointX_TextChanged;
            thisControl.DataPointY.TextChanged += thisControl.DataPointY_TextChanged;
        }

        /// <summary>
        /// Identifies the <see cref="Title"/> dependency property.
        /// </summary>
        public static DependencyProperty TitleProperty = DependencyProperty.Register(nameof(Title), typeof(string), typeof(PointPropertyControl), new UIPropertyMetadata("Title"));
        /// <summary>
        /// Gets/sets the title to display next to the control.
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
        /// Identifies the <see cref="IsReadOnly"/> dependency property
        /// </summary>
        public static DependencyProperty IsReadOnlyProperty = DependencyProperty.Register(nameof(IsReadOnly), typeof(bool), typeof(PointPropertyControl), new UIPropertyMetadata(false));
        /// <summary>
        /// gets/sets a value indicating whether the control is read-only.
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

        /// <summary>
        /// Identifies the <see cref="MaxPropertyWidth"/> dependency property.
        /// </summary>
        public static DependencyProperty MaxPropertyWidthProperty = DependencyProperty.Register(nameof(MaxPropertyWidth), typeof(double), typeof(PointPropertyControl), new UIPropertyMetadata(PropertyDefaults.DefaultMaxPropertyWidth));
        /// <summary>
        /// gets/sets the maximum width of the property label.
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
        public static DependencyProperty MinPropertyWidthProperty = DependencyProperty.Register(nameof(MinPropertyWidth), typeof(double), typeof(PointPropertyControl), new UIPropertyMetadata(PropertyDefaults.DefaultMinPropertyWidth));
        /// <summary>
        /// gets/sets the minimum width of the property label.
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
        public static DependencyProperty PropertyWidthProperty = DependencyProperty.Register(nameof(PropertyWidth), typeof(GridLength), typeof(PointPropertyControl), new UIPropertyMetadata(PropertyDefaults.DefaultPropertyWidth));
        /// <summary>
        /// gets/sets the grid width for the label column in the control layout.
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
        public static DependencyProperty ShowLeaderLineProperty = DependencyProperty.Register(nameof(ShowLeaderLine), typeof(bool), typeof(PointPropertyControl), new UIPropertyMetadata(true));
        /// <summary>
        /// gets/sets a value indicating on whether to show the leader line in the layout.
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
        /// Handles changes to the X value textbox and updates the <see cref="DataPoint"/>
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void DataPointX_TextChanged(object sender, TextChangedEventArgs e)
        {
            DataPointChanged();
        }

        /// <summary>
        /// Handles changes to the Y value textbox and updates the <see cref="DataPoint"/>
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void DataPointY_TextChanged(object sender, TextChangedEventArgs e)
        {
            DataPointChanged();
        }

        /// <summary>
        /// Updates the <see cref="DataPoint"/> value from the text boxes if the input is valid.
        /// </summary>
        private void DataPointChanged()
        {
            if (this.DataPointX.IsValidDouble() == false || this.DataPointY.IsValidDouble() == false)
                return; // Point = Point.Undefined
                        // 
            double xValue = this.DataPointX.GetValueAsDouble();
            double yValue = this.DataPointY.GetValueAsDouble();
            // 
            DataPoint = new Point(xValue, yValue);
        }
    }
}