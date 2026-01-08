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

namespace GenericControls
{


    #region Boolean Converters

    /// <summary>
    /// Reverses a boolean value in both directions (true <-> false).
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
    public class ReverseBooleanConverter : IValueConverter
    {
        /// <summary>
        /// Converts a boolean value to its inverse.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return value;
            if (value.GetType() != typeof(bool))
                return null;
            return !(bool)value;
        }

        /// <summary>
        /// Converts a boolean value back to its inverse.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return value;
            if (value.GetType() != typeof(bool))
                return null;
            return !(bool)value;
        }
    }

    /// <summary>
    /// Converts a boolean value to a Color, using specified value for true and false.
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
    public class BooleanToColorConverter : IValueConverter
    {
        /// <summary>
        /// gets/sets the color to return when the value is true.
        /// </summary>
        public Color TrueValue { get; set; }

        /// <summary>
        /// gets/sets the color to return when the value is false.
        /// </summary>
        public Color FalseValue { get; set; }

        /// <summary>
        /// gets/sets the color to return when the value is false.
        /// </summary>
        public BooleanToColorConverter()
        {
            // set defaults
            TrueValue = Colors.Black;
            FalseValue = Colors.Transparent;
        }

        /// <summary>
        /// Converts a boolean to its corresponding Color.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;
            if (value.GetType() != typeof(bool))
                return null;
            return (bool)value ? TrueValue : FalseValue;
        }

        /// <summary>
        /// Converts a Color back to a boolean based on matching TrueValue or FalseValue
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;
            if (value.GetType() == typeof(Color))
            {
                if (Color.AreClose((Color)value, TrueValue))
                    return true;
                if (Color.AreClose((Color)value, FalseValue))
                    return false;
            }
            // 
            return null;
        }
    }

    /// <summary>
    /// Converts a boolean value to a Brush, using specfied values for true and false.
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
    public class BooleanToBrushConverter : IValueConverter
    {
        /// <summary>
        /// gets/sets the brush to return when the value is true.
        /// </summary>
        public Brush TrueValue { get; set; }

        /// <summary>
        /// gets/sets the brush to return when the value is false.
        /// </summary>
        public Brush FalseValue { get; set; }

        /// <summary>
        /// gets/sets the brush to return when the value is false.
        /// </summary>
        public BooleanToBrushConverter()
        {
            // set defaults
            TrueValue = Brushes.Black;
            FalseValue = Brushes.Transparent;
        }

        /// <summary>
        /// Converts a boolean to its corresponding Brush.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;
            if (value.GetType() != typeof(bool))
                return null;
            return (bool)value ? TrueValue : FalseValue;
        }

        /// <summary>
        /// Converts a Brush back to a boolean by comparing to TrueValue or FalseValue.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;
            if (value.GetType() == typeof(Brush))
            {
                if (value.Equals(TrueValue))
                    return true;
                if (value.Equals(FalseValue))
                    return false;
            }
            // 
            return null;
        }
    }

    /// <summary>
    /// Converts a boolean value to a string, using specified true/false strings.
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
    public class BooleanToTextConverter : IValueConverter
    {
        /// <summary>
        /// gets/sets the string returned for true.
        /// </summary>
        public string TrueValue { get; set; }

        /// <summary>
        /// gets/sets the string returned for false.
        /// </summary>
        public string FalseValue { get; set; }

        /// <summary>
        /// Constructor for converter to blank strings for true and false value 
        /// </summary>
        public BooleanToTextConverter()
        {
            // set defaults
            TrueValue = "";
            FalseValue = "";
        }

        /// <summary>
        /// Converts a boolean to its associated string representation.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;
            if (value.GetType() != typeof(bool))
                return null;
            return (bool)value ? TrueValue : FalseValue;
        }

        /// <summary>
        /// Converts a string back to a boolean by matching against TrueValue or FalseValue
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;
            if (value.GetType() == typeof(string))
            {
                if (((string)value ?? "") == (TrueValue ?? ""))
                    return true;
                if (((string)value ?? "") == (FalseValue ?? ""))
                    return false;
            }
            else
            {
                string castValue = value.ToString();
                if ((castValue ?? "") == (TrueValue ?? ""))
                    return true;
                if ((castValue ?? "") == (FalseValue ?? ""))
                    return false;
            }
            // 
            return null;
        }
    }

    /// <summary>
    /// Converts a boolean value to a double, using a specified true/false values.
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
    public class BooleanToDoubleConverter : IValueConverter
    {
        /// <summary>
        /// gets/sets the double value for true.
        /// </summary>
        public double TrueValue { get; set; }

        /// <summary>
        /// gets/sets the double value for false.
        /// </summary>
        public double FalseValue { get; set; }

        /// <summary>
        /// Constructor setting defaults for true/false value 
        /// </summary>
        public BooleanToDoubleConverter()
        {
            // set defaults
            TrueValue = 0d;
            FalseValue = 0d;
        }

        /// <summary>
        /// Converts a boolean to its corresponding double value.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;
            if (value.GetType() != typeof(bool))
                return null;
            return (bool)value ? TrueValue : FalseValue;
        }

        /// <summary>
        /// Converts a double back to a boolean if it matches either the TrueValue or FalseValue.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;
            if (value.GetType() == typeof(double))
            {
                if ((double)value == TrueValue)
                    return true;
                if ((double)value == FalseValue)
                    return false;
            }
            else
            {
                double castValue;
                if (NumberFormatHelper.TryParseDouble(value.ToString(), out castValue))
                {
                    if (castValue == TrueValue)
                        return true;
                    if (castValue == FalseValue)
                        return false;
                }
            }
            return null;
        }
    }

    /// <summary>
    /// Converts a <see cref="Visibility"/> value to a boolean based on configured values for each visibility state.
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
    public class VisibilityToBooleanConverter : IValueConverter
    {
        /// <summary>
        /// Boolean value associated with <see cref="Visibility.Collapsed"/>
        /// </summary>
        public bool CollapsedValue { get; set; }

        /// <summary>
        /// Boolean value associated with <see cref="Visibility.Hidden"/>
        /// </summary>
        public bool HiddenValue { get; set; }

        /// <summary>
        /// Boolean value associated with <see cref="Visibility.Visible"/>
        /// </summary>
        public bool VisibleValue { get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        public VisibilityToBooleanConverter()
        {
            // set defaults
            CollapsedValue = false;
            HiddenValue = false;
            VisibleValue = true;
        }

        /// <summary>
        /// Converts a <see cref="Visibility"/> value to a boolean based on matching configured states.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || value.GetType() != typeof(Visibility))
                return null;
            switch ((Visibility)value)
            {
                case Visibility.Collapsed:
                    {
                        return CollapsedValue;
                    }
                case Visibility.Hidden:
                    {
                        return HiddenValue;
                    }
                case Visibility.Visible:
                    {
                        return VisibleValue;
                    }

                default:
                    {
                        return null;
                    }
            }
        }

        /// <summary>
        /// Converts a boolean back to a <see cref="Visibility"/> value based on configured boolean-state mappings.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // This simple logic won't work with the three state system. For my cases currently it doesn't matter. In the future this might have to be fixed probably by using some private variables to keep track of the converted state.
            if (Equals(value, CollapsedValue))
                return Visibility.Collapsed;
            if (Equals(value, HiddenValue))
                return Visibility.Hidden;
            if (Equals(value, VisibleValue))
                return Visibility.Visible;
            // 
            return null;
        }
    }

    #endregion
    /// <summary>
    /// Returns true if a numeric value is within the specified inclusive range.
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
    public class InRangeConverter : IValueConverter
    {
        /// <summary>
        /// Lower inclusive bound.
        /// </summary>
        public double LowerBound { get; set; }

        /// <summary>
        /// Upper inclusive bound.
        /// </summary>
        public double UpperBound { get; set; }

        /// <summary>
        /// Default constructor for min/max values for the range.
        /// </summary>
        public InRangeConverter()
        {
            // set defaults
            LowerBound = double.MinValue;
            UpperBound = double.MaxValue;
        }

        /// <summary>
        /// Returns true if the input value falls within the [LowerBound, UpperBound] range.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return false;
            double data;
            if (!NumberFormatHelper.TryParseDouble(value.ToString(), out data))
                return false;
            if (data < LowerBound || data > UpperBound)
                return false;
            return true;
        }

        /// <summary>
        /// ConvertBack is not implemented for this converter.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Always returns <see cref="Visibility.Visible"/> regardless of input.
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
    public class AlwaysVisibleConverter : IValueConverter
    {
        /// <summary>
        /// Always returns <see cref="Visibility.Visible"/>.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Visibility.Visible;
        }

        /// <summary>
        /// ConvertBack is not implemented.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Converts a <see cref="DateTime"/> value to a formatted time segment string (hour, minute, second, or meridian)
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
    public class TimeTextConverter : DependencyObject, IValueConverter
    {
        /// <summary>
        /// Gets/sets which part of the time is being targeted.
        /// </summary>
        public TimeTarget TimeType { get; set; } = TimeTarget.Hour;

        /// <summary>
        /// gets/sets whether to use 24-hour time format.
        /// </summary>
        public static DependencyProperty Is24HourProperty = DependencyProperty.Register(nameof(Is24Hour), typeof(bool), typeof(TimeTextConverter), new UIPropertyMetadata(false));

        /// <summary>
        /// Gets/sets whether to use 24-hour time format.
        /// </summary>
        public bool Is24Hour
        {
            get
            {
                return (bool)GetValue(Is24HourProperty);
            }
            set
            {
                SetValue(Is24HourProperty, value);
            }
        }

        /// <summary>
        /// Represents the time component to convert.
        /// </summary>
        public enum TimeTarget
        {
            /// <summary>Hour component.</summary>
            Hour,
            /// <summary>Minute component.</summary>
            Minute,
            /// <summary>Second component.</summary>
            Second,
            /// <summary>AM/PM meridian indicator.</summary>
            Meridian
        }

        /// <summary>
        /// Converts a <see cref="DateTime"/> to a string for the configured <see cref="TimeTarget"/>
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value.GetType() != typeof(DateTime))
                return "";
            switch (TimeType)
            {
                case TimeTarget.Hour:
                    {
                        if (Is24Hour)
                            return ((DateTime)value).ToString("HH", CultureInfo.InvariantCulture);
                        return ((DateTime)value).ToString("hh", CultureInfo.InvariantCulture);
                    }
                case TimeTarget.Minute:
                    {
                        return ((DateTime)value).ToString("mm", CultureInfo.InvariantCulture);
                    }
                case TimeTarget.Second:
                    {
                        return ((DateTime)value).ToString("ss", CultureInfo.InvariantCulture);
                    }
                case TimeTarget.Meridian:
                    {
                        if (Is24Hour)
                            return "";
                        return ((DateTime)value).ToString("tt", CultureInfo.InvariantCulture);
                    }

                default:
                    {
                        return "";
                    }
            }
        }

        /// <summary>
        /// ConvertBack is not implemented.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Adjusts a width value to account for the vertical scrollbar width in a <see cref="DataGrid"/>
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
    public class DataGridWidthConverter : IValueConverter
    {
        /// <summary>
        /// Subtracts the system scrollbar width from the total width.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return 0d;
            double width;
            if (!NumberFormatHelper.TryParseDouble(value.ToString(), out width))
                return 0d;
            return width - SystemParameters.VerticalScrollBarWidth;
        }

        /// <summary>
        /// ConvertBack is not supported.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        /// <exception cref="NotSupportedException"></exception>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    /// <summary>
    /// Converts a double value to a <see cref="Thickness"/> with optional side inclusion flags.
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
    public class DoubleToThicknessConverter : IValueConverter
    {

        /// <summary>Gets or sets the left thickness value.</summary>
        public double Left { get; set; } = 0d;
        /// <summary>Gets or sets the top thickness value.</summary>
        public double Top { get; set; } = 0d;
        /// <summary>Gets or sets the right thickness value.</summary>
        public double Right { get; set; } = 0d;
        /// <summary>Gets or sets the bottom thickness value.</summary>
        public double Bottom { get; set; } = 0d;
        //
        /// <summary>Gets or sets whether the left side is affected.</summary>
        public bool IsLeft { get; set; } = true;
        /// <summary>Gets or sets whether the right side is affected.</summary>
        public bool IsRight { get; set; } = true;
        /// <summary>Gets or sets whether the top side is affected.</summary>
        public bool IsTop { get; set; } = true;
        /// <summary>Gets or sets whether the bottom side is affected.</summary>
        public bool IsBottom { get; set; } = true;

        /// <summary>
        /// Converts a double to a <see cref="Thickness"/> using the specified side flags.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;
            double dblValue;
            if (NumberFormatHelper.TryParseDouble(value.ToString(), out dblValue) == false)
                return null;
            if (IsLeft)
                Left = dblValue;
            if (IsTop)
                Top = dblValue;
            if (IsRight)
                Right = dblValue;
            if (IsBottom)
                Bottom = dblValue;
            return new Thickness(Left, Top, Right, Bottom);
        }

        /// <summary>
        /// Converts a <see cref="Thickness"/> back to a double, using the first enabled side or the average of all sides.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return value;
            Thickness t = (Thickness)value;
            if (IsLeft)
                return t.Left;
            if (IsRight)
                return t.Right;
            if (IsTop)
                return t.Top;
            if (IsBottom)
                return t.Bottom;
            // if all else fails average them.
            return (t.Left + t.Right + t.Top + t.Bottom) / 4d;
        }
    }

    /// <summary>
    /// Converts a <see cref="Thickness"/> to the average of all four sides.
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
    public class ThicknessToDoubleConverter : IValueConverter
    {
        /// <summary>
        /// Retirms the average of Left, Top, Right, and Bottom thickness values.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return value;
            Thickness t = (Thickness)value;
            return (t.Left + t.Right + t.Top + t.Bottom) / 4d;
        }

        /// <summary>
        /// Converts a double back to a uniform <see cref="Thickness"/>
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;
            double dblValue;
            if (NumberFormatHelper.TryParseDouble(value.ToString(), out dblValue) == false)
                return null;
            return new Thickness(dblValue);
        }
    }

    /// <summary>
    /// Converts a <see cref="Color"/> to a <see cref="SolidColorBrush"/> and vice versa.
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
    public class ColorToSolidBrushConverter : IValueConverter
    {
        /// <summary>
        /// Converts a <see cref="Color"/> to a <see cref="SolidColorBrush"/>
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;
            return new SolidColorBrush((Color)value);
        }

        /// <summary>
        /// Converts a <see cref="SolidColorBrush"/> back to a <see cref="Color"/>
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;
            return ((SolidColorBrush)value).Color;
        }
    }

    /// <summary>
    /// Converts a <see cref="System.Drawing.Color"/> to a <see cref="SolidColorBrush"/> and vice versa.
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
    public class DrawingColorToSolidColorBrushConverter : IValueConverter
    {
        /// <summary>
        ///  Converts a <see cref="System.Drawing.Color"/> to a <see cref="SolidColorBrush"/>
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;
            if (value.GetType() != typeof(System.Drawing.Color))
                return null;
            System.Drawing.Color c = (System.Drawing.Color)value;
            return new SolidColorBrush(Color.FromArgb(c.A, c.R, c.G, c.B));
        }

        /// <summary>
        /// Converts <see cref="SolidColorBrush"/> back to <see cref="System.Drawing.Color"/>.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;
            SolidColorBrush scb = value as SolidColorBrush;
            if (scb == null)
                return null;
            var c = scb.Color;
            return System.Drawing.Color.FromArgb(c.A, c.R, c.G, c.B);
        }
    }

    /// <summary>
    /// Converts a string or font name to a <see cref="FontFamily"/>.
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
    public class FontToFontFamilyConverter : IValueConverter
    {
        /// <summary>
        /// Converts a string to a <see cref="FontFamily"/> object.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return value;
            return new FontFamily(value.ToString());
        }

        /// <summary>
        /// Converts a <see cref="FontFamily"/> back to its string name.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return value;
            return ((FontFamily)value).Source;
        }
    }

    /// <summary>
    /// Convertts a <see cref="FontFamily"/> to a string and vice versa.
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
    public class FontFamilyToFontStringConverter : IValueConverter
    {
        /// <summary>
        /// Converts a <see cref="FontFamily"/> to its string name.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return value;
            return ((FontFamily)value).Source;
        }

        /// <summary>
        /// Converts a string name to a <see cref="FontFamily"/>
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return value;
            return new FontFamily(value.ToString());
        }
    }

    /// <summary>
    /// Converts between integer and double values.
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
    public class IntToDoubleConverter : IValueConverter
    {
        /// <summary>
        /// Converts an integer to a double.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;
            return (double)value;
        }

        /// <summary>
        /// Converts a double to an integer.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;
            return (int)value;
        }

    }

    /// <summary>
    /// Converts between a double and its string representation.
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
    public class DoubleToStringConverter : IValueConverter
    {
        /// <summary>
        /// Converts a string to a double.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;
            double stringDouble;
            double.TryParse(value.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out stringDouble);
            return stringDouble;
        }

        /// <summary>
        /// Converts a double back to its string representation.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;
            return value.ToString();
        }
    }

    /// <summary>
    /// Converts between string and double types.
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
    public class StringToDoubleConverter : IValueConverter
    {
        /// <summary>
        /// Converts a double to its string representation.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;
            return value.ToString();
        }

        /// <summary>
        /// Converts a string to a double.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;
            double stringDouble;
            double.TryParse(value.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out stringDouble);
            return stringDouble;
        }
    }

    /// <summary>
    /// Converts between <see cref="double"/> and <see cref="DataGridLength"/> values.
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
    public class DoubleToDataGridLengthConverter : IValueConverter
    {
        /// <summary>
        /// Converts a <see cref="double"/> to a <see cref="DataGridLength"/>, or vice versa.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType == typeof(DataGridLength))
            {
                if (value is null)
                    return DataGridLength.Auto;
                if (value is double)
                    return new DataGridLength((double)value);
                return DataGridLength.Auto;
            }

            if (targetType == typeof(double))
            {
                if (value is DataGridLength)
                    return ((DataGridLength)value).Value;
                return double.NaN;
            }

            return null;
        }

        /// <summary>
        /// Converts between <see cref="double"/> and <see cref="DataGridLength"/>
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ConvertBack(value, targetType, parameter, culture);
        }
    }

    /// <summary>
    /// Converts between <see cref="double"/> and <see cref="GridLength"/> values.
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
    public class DoubleToGridLengthConverter : IValueConverter
    {
        /// <summary>
        /// Converts between <see cref="double"/> and <see cref="GridLength"/>
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType == typeof(GridLength))
            {
                if (value is null)
                    return GridLength.Auto;
                if (value is double)
                    return new GridLength((double)value);
                return GridLength.Auto;
            }

            if (targetType == typeof(double))
            {
                if (value is GridLength)
                    return ((GridLength)value).Value;
                return double.NaN;
            }

            return null;
        }

        /// <summary>
        /// Converts a <see cref="double"/> to a <see cref="GridLength"/>, or vice versa.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ConvertBack(value, targetType, parameter, culture);
        }
    }

    /// <summary>
    /// Converts between <see cref="double"/> and <see cref="CornerRadius"/> values.
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
    public class DoubleToCornerRadiusConverter : IValueConverter
    {
        /// <summary>
        /// Converts a <see cref="CornerRadius"/> to its bottom-left value, or a <see cref="double"/>
        /// to a <see cref="CornerRadius"/>
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType == typeof(CornerRadius))
            {
                if (value is null)
                    return new CornerRadius();
                if (value is double)
                    return new CornerRadius((double)value);
                return new CornerRadius();
            }

            if (targetType == typeof(double))
            {
                if (value is CornerRadius)
                    return ((CornerRadius)value).BottomLeft;
                return double.NaN;
            }

            return null;
        }

        /// <summary>
        /// Converts a <see cref="double"/> to a <see cref="CornerRadius"/> or vice versa.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ConvertBack(value, targetType, parameter, culture);
        }
    }

    /// <summary>
    /// Converts between <see cref="Vector"/> and <see cref="Point"/>.
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
    public class VectorToPointConverter : IValueConverter
    {
        /// <summary>
        /// Converts a <see cref="Vector"/> to a <see cref="Point"/> with the same X and Y values.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return value;
            if (value.GetType() != typeof(Vector))
                return null;
            Vector sv = (Vector)value;
            // 
            return new Point(sv.X, sv.Y);
        }

        /// <summary>
        /// Converts a <see cref="Point"/> to a <see cref="Vector"/> with the same X and Y values.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return value;
            if (value.GetType() != typeof(Point))
                return null;
            Point p = (Point)value;
            // 
            return new Vector(p.X, p.Y);
        }
    }

    /// <summary>
    /// Computes the width of a single tab in a <see cref="TabControl"/> based on the number of items and actual width.
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
    public class TabSizeConverter : IMultiValueConverter
    {
        /// <summary>
        /// Calculates the size of each tab by dividing the TabControl's width by its item count.
        /// </summary>
        /// <param name="values"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            TabControl tabControl = (TabControl)values[0];
            double width = tabControl.ActualWidth / tabControl.Items.Count;
            if (width < 12d)
                return 0;
            return width - (tabControl.Items.Count + 1);
        }

        /// <summary>
        /// ConvertBack is not implmented.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetTypes"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Converts special <see cref="double"/> values to string representations such as "N/A" or infinity, and vice versa.
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
    public class DoubleToNAConverter : IValueConverter
    {
        /// <summary>
        /// Converts special or invalid double values (NaN, Infinity) to correct symbols.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null)
                return "N/A";
            if (value.GetType() != typeof(double))
                return "N/A";
            double val = (double)value;
            // 
            if (double.IsNaN(val))
                return "N/A";
            if (double.IsPositiveInfinity(val))
                return "+∞";
            if (double.IsNegativeInfinity(val))
                return "-∞";
            if (double.IsInfinity(val))
                return "∞";
            // 
            return val;
        }

        /// <summary>
        /// Converts string representations of special values back to <see cref="double"/> values.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null)
                return double.NaN;
            if (value.GetType() == typeof(string))
            {
                string val = (string)value;
                if (val == "NA")
                    return double.NaN;
                if (val == "N/A")
                    return double.NaN;
                if (NumberFormatHelper.IsInfinityText(val))
                {
                    if (val.StartsWith("-"))
                        return double.NegativeInfinity;
                    return double.PositiveInfinity;
                }

                // If value is not a number, return NaN
                double dblVal;
                if (NumberFormatHelper.TryParseDouble(val, out dblVal) == false)
                {
                    return double.NaN;
                }
                return dblVal;
            }
            return value;
        }
    }


    /// <summary>
    /// Converts strings representing special numeric values to "N/A" if they are invalid or special (NaN, inf).
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
    public class StringToNAConverter : IValueConverter
    {
        /// <summary>
        /// Converts a string to "N/A" if it represents a NaN or infinite numeric value.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null)
                return "N/A";
            if (value.GetType() != typeof(string))
                return "N/A";
            string val = (string)value;
            // 
            double doubleVal;
            if (!double.TryParse(val, out doubleVal))
                return "N/A";
            if (double.IsNaN(doubleVal))
                return "N/A";
            if (double.IsInfinity(doubleVal))
                return "N/A";
            // 
            return val;
        }

        /// <summary>
        /// ConvertBack is not implemented.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Converts a <see cref="SolidColorBrush"/> to a lighter semi-transparent variant used for gridlines.
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
    public class GridlineColorLightConverter : IValueConverter
    {
        /// <summary>
        /// Reduces alpha to 20% if the color is already semi-transparent; otherwise 
        /// sets alpha to 51 (20%)
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null || value.GetType() != typeof(SolidColorBrush))
            {
                return new SolidColorBrush(Color.FromArgb(51, 0, 0, 0));
            }

            SolidColorBrush c = (SolidColorBrush)value;
            if (c.Color.A < 51)
            {
                return new SolidColorBrush(Color.FromArgb((byte)Math.Round(c.Color.A * 0.2d), c.Color.R, c.Color.G, c.Color.B));
            }
            else
            {
                return new SolidColorBrush(Color.FromArgb(51, c.Color.R, c.Color.G, c.Color.B));
            }

        }

        /// <summary>
        /// ConvertBack is not implemented.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}