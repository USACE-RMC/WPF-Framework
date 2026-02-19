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
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using OxyPlot;
using Wpf = OxyPlot.Wpf;

namespace OxyPlotControls
{
    /// <summary>
    /// A user control that provides UI for editing general plot properties such as title, subtitle,
    /// plot area styling, and background settings for an OxyPlot chart.
    /// </summary>
    public partial class GeneralPlotControl : UserControl
    {
        #region Dependency Properties

        /// <summary>
        /// Identifies the <see cref="Plot"/> dependency property.
        /// </summary>
        public static DependencyProperty PlotProperty = DependencyProperty.Register(
            nameof(Plot), typeof(Wpf.Plot), typeof(GeneralPlotControl),
            new PropertyMetadata(null, OnPlotPropertyChanged));

        /// <summary>
        /// Gets or sets the OxyPlot Plot control that this control edits.
        /// </summary>
        public Wpf.Plot Plot
        {
            get { return (Wpf.Plot)GetValue(PlotProperty); }
            set { SetValue(PlotProperty, value); }
        }

        /// <summary>
        /// Identifies the <see cref="ExpanderStyle"/> dependency property.
        /// </summary>
        public static DependencyProperty ExpanderStyleProperty = DependencyProperty.Register(
            nameof(ExpanderStyle), typeof(Style), typeof(GeneralPlotControl));

        /// <summary>
        /// Gets or sets the style applied to expander controls within this control.
        /// </summary>
        public Style ExpanderStyle
        {
            get { return (Style)GetValue(ExpanderStyleProperty); }
            set { SetValue(ExpanderStyleProperty, value); }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="GeneralPlotControl"/> class.
        /// </summary>
        public GeneralPlotControl()
        {
            InitializeComponent();

        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Called when the Plot dependency property changes.
        /// Forces a layout refresh to sync bindings to the new Plot.
        /// </summary>
        /// <param name="d">The dependency object (GeneralPlotControl instance).</param>
        /// <param name="e">The event arguments containing old and new values.</param>
        private static void OnPlotPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is GeneralPlotControl control && e.NewValue != null)
            {
                // Force layout update to sync bindings
                control.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Loaded, new Action(() =>
                {
                    control.UpdateLayout();
                }));
            }
        }

        #endregion
    }

    /// <summary>
    /// A value converter that handles OxyPlot's automatic color representation.
    /// OxyPlot uses ARGB(0,0,0,1) to represent an automatic color, which this converter
    /// translates to black for display purposes.
    /// </summary>
    public class OxyAutomaticColorConverter : IValueConverter
    {
        /// <summary>
        /// Converts an OxyPlot color to a WPF SolidColorBrush, handling the automatic color case.
        /// </summary>
        /// <param name="value">The color value to convert.</param>
        /// <param name="targetType">The target type (not used).</param>
        /// <param name="parameter">Additional parameter (not used).</param>
        /// <param name="culture">The culture to use for conversion.</param>
        /// <returns>A SolidColorBrush representing the color, with automatic colors converted to black.</returns>
        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return value;
            if (value.GetType() != typeof(Color)) return null;
            Color c = (Color)value;
            var oxyCol = OxyColor.FromArgb(c.A, c.R, c.G, c.B);
            if (oxyCol.IsAutomatic()) return new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            return new SolidColorBrush(c);
        }

        /// <summary>
        /// Converts a SolidColorBrush back to a Color.
        /// </summary>
        /// <param name="value">The brush to convert.</param>
        /// <param name="targetType">The target type (not used).</param>
        /// <param name="parameter">Additional parameter (not used).</param>
        /// <param name="culture">The culture to use for conversion.</param>
        /// <returns>The Color from the SolidColorBrush.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((SolidColorBrush)value).Color;
        }
    }

    /// <summary>
    /// A value converter that provides a default font size when the value is NaN or invalid.
    /// Returns 12.0 as the default font size.
    /// </summary>
    public class OxyDefaultFontSizeConverter : IValueConverter
    {
        /// <summary>
        /// Converts a font size value, returning a default of 12.0 for NaN or invalid values.
        /// </summary>
        /// <param name="value">The font size value to convert.</param>
        /// <param name="targetType">The target type (not used).</param>
        /// <param name="parameter">Additional parameter (not used).</param>
        /// <param name="culture">The culture to use for conversion.</param>
        /// <returns>The font size, or 12.0 if the value is NaN, infinite, or invalid.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return 12.0;
            if (value.GetType() != typeof(double)) return 12.0;
            double doubleVal = (double)value;
            if (double.IsNaN(doubleVal) || double.IsInfinity(doubleVal)) return 12.0;
            return doubleVal;
        }

        /// <summary>
        /// Converts a font size back, returning NaN if the value is the default 12.0.
        /// </summary>
        /// <param name="value">The font size value to convert back.</param>
        /// <param name="targetType">The target type (not used).</param>
        /// <param name="parameter">Additional parameter (not used).</param>
        /// <param name="culture">The culture to use for conversion.</param>
        /// <returns>The font size, or NaN if the value is null, invalid, or 12.0.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return double.NaN;
            if (value.GetType() != typeof(double)) return double.NaN;
            double doubleVal = (double)value;
            if (doubleVal == 12.0) return double.NaN;
            return doubleVal;
        }
    }

    /// <summary>
    /// A simple value converter that casts between Brush and SolidColorBrush types.
    /// </summary>
    public class SolidColorBrushConverter : IValueConverter
    {
        /// <summary>
        /// Converts a value to a SolidColorBrush.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <param name="targetType">The target type (not used).</param>
        /// <param name="parameter">Additional parameter (not used).</param>
        /// <param name="culture">The culture to use for conversion.</param>
        /// <returns>The value cast to SolidColorBrush, or null if the value is null.</returns>
        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null;
            return (SolidColorBrush)value;
        }

        /// <summary>
        /// Converts a SolidColorBrush back to a Brush.
        /// </summary>
        /// <param name="value">The value to convert back.</param>
        /// <param name="targetType">The target type (not used).</param>
        /// <param name="parameter">Additional parameter (not used).</param>
        /// <param name="culture">The culture to use for conversion.</param>
        /// <returns>The value cast to Brush, or null if the value is null.</returns>
        public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null;
            return (Brush)value;
        }
    }
}
