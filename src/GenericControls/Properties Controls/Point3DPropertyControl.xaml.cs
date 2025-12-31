/*
* NOTICE:
* The U.S. Army Corps of Engineers, Risk Management Center (USACE-RMC) makes no guarantees about
* the results, or appropriateness of outputs, obtained from this library.
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
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Media3D;

namespace GenericControls
{
    /// <summary>
    /// A WPF UserControl for binding and editing a 3D point (X,Y,Z) with decimal precision, title, and layout properties.
    /// </summary>
    public partial class Point3DPropertyControl : UserControl, INotifyPropertyChanged
    {
        /// <summary>
        /// Gets/sets the number of decimal places to display for the X, Y, and Z values.
        /// </summary>
        public static DependencyProperty DecimalsProperty = DependencyProperty.Register(nameof(Decimals), typeof(int), typeof(Point3DPropertyControl), new UIPropertyMetadata(5, DecimalsChanged_Callback));
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
        /// Called when the <see cref="Decimals"/> property changes. Updates the display of the coordinate values.
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void DecimalsChanged_Callback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d == null)
                return;
            if (d.GetType() != typeof(Point3DPropertyControl))
                return;
            Point3DPropertyControl thisControl = (Point3DPropertyControl)d;
            // 
            if (e.NewValue == null)
                return;
            if (e.NewValue.GetType() != typeof(int))
                return;
            int newValue = (int)e.NewValue;
            // Update the textboxes with the new values
            // remove the handlers so the property doesn't get triggered for update.
            thisControl.DataPointX.TextChanged -= thisControl.DataPointX_TextChanged;
            thisControl.DataPointY.TextChanged -= thisControl.DataPointY_TextChanged;
            thisControl.DataPointZ.TextChanged -= thisControl.DataPointZ_TextChanged;
            // update the values in the textboxes
            thisControl.DataPointX.Text = NumberFormatHelper.FormatDouble(Math.Round(thisControl.DataPoint.X, newValue));
            thisControl.DataPointY.Text = NumberFormatHelper.FormatDouble(Math.Round(thisControl.DataPoint.Y, newValue));
            thisControl.DataPointZ.Text = NumberFormatHelper.FormatDouble(Math.Round(thisControl.DataPoint.Z, newValue));
            // add the handlers back for updating back to source.
            thisControl.DataPointX.TextChanged += thisControl.DataPointX_TextChanged;
            thisControl.DataPointY.TextChanged += thisControl.DataPointY_TextChanged;
            thisControl.DataPointZ.TextChanged += thisControl.DataPointZ_TextChanged;
        }

        /// <summary>
        /// The dependency property for the 3D point value.
        /// </summary>
        public static DependencyProperty DataPointProperty = DependencyProperty.Register(nameof(DataPoint), typeof(Point3D), typeof(Point3DPropertyControl), new UIPropertyMetadata(new Point3D(double.MinValue, double.MinValue, double.MinValue), DataPointChanged_Callback));

        /// <summary>
        /// Gets/sets the 3D point value (X, Y, Z).
        /// </summary>
        public Point3D DataPoint
        {
            get
            {
                return (Point3D)this.GetValue(DataPointProperty);
            }
            set
            {
                this.SetValue(DataPointProperty, value);
            }
        }

        /// <summary>
        /// Called when the <see cref="DataPoint"/> property changes. Updates the UI text fields.
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void DataPointChanged_Callback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d == null)
                return;
            if (d.GetType() != typeof(Point3DPropertyControl))
                return;
            Point3DPropertyControl thisControl = (Point3DPropertyControl)d;
            // 
            if (e.NewValue == null)
                return;
            if (e.NewValue.GetType() != typeof(Point3D))
                return;
            Point3D newDataPoint = (Point3D)e.NewValue;
            // Update the textboxes with the new values
            // remove the handlers so the property doesn't get triggered for update.
            thisControl.DataPointX.TextChanged -= thisControl.DataPointX_TextChanged;
            thisControl.DataPointY.TextChanged -= thisControl.DataPointY_TextChanged;
            thisControl.DataPointZ.TextChanged -= thisControl.DataPointZ_TextChanged;
            // update the values in the textboxes
            thisControl.DataPointX.Text = NumberFormatHelper.FormatDouble(Math.Round(newDataPoint.X, thisControl.Decimals));
            thisControl.DataPointY.Text = NumberFormatHelper.FormatDouble(Math.Round(newDataPoint.Y, thisControl.Decimals));
            thisControl.DataPointZ.Text = NumberFormatHelper.FormatDouble(Math.Round(newDataPoint.Z, thisControl.Decimals));
            // add the handlers back for updating back to source.
            thisControl.DataPointX.TextChanged += thisControl.DataPointX_TextChanged;
            thisControl.DataPointY.TextChanged += thisControl.DataPointY_TextChanged;
            thisControl.DataPointZ.TextChanged += thisControl.DataPointZ_TextChanged;
        }

        /// <summary>
        /// Identifies the <see cref="Title"/> dependency property.
        /// </summary>
        public static DependencyProperty TitleProperty = DependencyProperty.Register(nameof(Title), typeof(string), typeof(Point3DPropertyControl), new UIPropertyMetadata("Title"));
        /// <summary>
        /// Gets/sets title of the control.
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
        /// Identifies the <see cref="IsReadOnly"/> dependency property.
        /// </summary>
        public static DependencyProperty IsReadOnlyProperty = DependencyProperty.Register(nameof(IsReadOnly), typeof(bool), typeof(Point3DPropertyControl), new UIPropertyMetadata(false));
        /// <summary>
        /// gets/sets the input fields are read-only.
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
        public static DependencyProperty MaxPropertyWidthProperty = DependencyProperty.Register(nameof(MaxPropertyWidth), typeof(double), typeof(Point3DPropertyControl), new UIPropertyMetadata(PropertyDefaults.DefaultMaxPropertyWidth));
        /// <summary>
        /// Gets/sets value of the maximum width for the layout column.
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
        public static DependencyProperty MinPropertyWidthProperty = DependencyProperty.Register(nameof(MinPropertyWidth), typeof(double), typeof(Point3DPropertyControl), new UIPropertyMetadata(PropertyDefaults.DefaultMinPropertyWidth));
        /// <summary>
        /// gets/sets value of the minimum width for the layout column.
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
        public static DependencyProperty PropertyWidthProperty = DependencyProperty.Register(nameof(PropertyWidth), typeof(GridLength), typeof(Point3DPropertyControl), new UIPropertyMetadata(PropertyDefaults.DefaultPropertyWidth));
        /// <summary>
        /// gets/sets the GridLength value for property layout.
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

        private double _actualWidth = 0d;
        /// <summary>
        /// The current actual width of the property display column.
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
        /// Updates the <see cref="ActualPropertyWidth"/> when the control size changes.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ControlSizeChanged(object sender, SizeChangedEventArgs e)
        {
            FrameworkElement el = sender as FrameworkElement;
            ActualPropertyWidth = el.ActualWidth;
        }

        /// <summary>
        /// Identifies the <see cref="ShowLeaderLine"/> dependency property.
        /// </summary>
        public static DependencyProperty ShowLeaderLineProperty = DependencyProperty.Register(nameof(ShowLeaderLine), typeof(bool), typeof(Point3DPropertyControl), new UIPropertyMetadata(true));
        /// <summary>
        /// gets/sets whether a leader line is shown.
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
        /// Identifies the <see cref="ShowTitle"/> dependency property.
        /// </summary>
        public static DependencyProperty ShowTitleProperty = DependencyProperty.Register(nameof(ShowTitle), typeof(bool), typeof(Point3DPropertyControl), new UIPropertyMetadata(true));
        /// <summary>
        /// gets/sets whether the title is visible.
        /// </summary>
        public bool ShowTitle
        {
            get
            {
                return (bool)this.GetValue(ShowTitleProperty);
            }
            set
            {
                this.SetValue(ShowTitleProperty, value);
            }
        }

        /// <summary>
        /// Handles text change in X coordinate field.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataPointX_TextChanged(object sender, TextChangedEventArgs e)
        {
            DataPointChanged();
        }

        /// <summary>
        /// Handles text change in Y coordinate field.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataPointY_TextChanged(object sender, TextChangedEventArgs e)
        {
            DataPointChanged();
        }

        /// <summary>
        /// Handles text change in Z coordinate field.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataPointZ_TextChanged(object sender, TextChangedEventArgs e)
        {
            DataPointChanged();
        }

        /// <summary>
        /// Updates the <see cref="DataPoint"/> value based on the text field inputs.
        /// </summary>
        private void DataPointChanged()
        {
            if (this.DataPointX.IsValidDouble() == false || this.DataPointY.IsValidDouble() == false)
                return;
            // 
            double xValue = this.DataPointX.GetValueAsDouble();
            double yValue = this.DataPointY.GetValueAsDouble();
            double zValue = this.DataPointZ.GetValueAsDouble();
            // 
            DataPoint = new Point3D(xValue, yValue, zValue);
        }
    }
}