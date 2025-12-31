using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace GenericControls
{
    /// <summary>
    /// Represents the individual components of a color (Alpha, Red, Green, Blue).
    /// </summary>
    public enum ColorComponent
    {
        A,
        R,
        G,
        B
    }

    /// <summary>
    /// Converts between a <see cref="SolidColorBrush"/> and a single byte value
    /// corresponding to a specific <see cref="ColorComponent"/> (A, R, G, B).
    /// </summary>
    public class ColorToByteConverter : IValueConverter
    {
        /// <summary>
        /// Gets/sets the color component (A,R,G, or B) to extract or update.
        /// </summary>
        public ColorComponent Component { get; set; } = ColorComponent.R;
        private SolidColorBrush _color;
        /// <summary>
        /// Converts a <see cref="SolidColorBrush"/> to the byte of the selected <see cref="ColorComponent"/>
        /// </summary>
        /// <param name="value">A <see cref="SolidColorBrush"/> to convert.</param>
        /// <param name="targetType"> The expected target type (byte).</param>
        /// <param name="parameter">Not used.</param>
        /// <param name="culture">The culture to use in the converter (not used).</param>
        /// <returns>A byte representing the selected color component, or <c>null</c> if input is invalid.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null)
            {
                _color = null;
                return null;
            }
            _color = value as SolidColorBrush;
            if (_color is null)
                return null;
            // 
            switch (Component)
            {
                case ColorComponent.A:
                    {
                        return _color.Color.A;
                    }
                case ColorComponent.R:
                    {
                        return _color.Color.R;
                    }
                case ColorComponent.G:
                    {
                        return _color.Color.G;
                    }
                case ColorComponent.B:
                    {
                        return _color.Color.B;
                    }

                default:
                    {
                        return (byte)0;
                    }
            }
        }
        /// <summary>
        /// Converts a numeric value back into a <see cref="SolidColorBrush"/>, updating only the selected <see cref="ColorComponent"/>.
        /// </summary>
        /// <param name="value"> A numeric value (0-255) for the selected color component.</param>
        /// <param name="targetType">The expected target type (<see cref="SolidColorBrush"/>).</param>
        /// <param name="parameter">Not used.</param>
        /// <param name="culture">A new <see cref="SolidColorBrush"/> with the updated color component, or a fallback transparent brush if parsing fails.</param>
        /// <returns></returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null)
                return null;
            if (_color is null)
                return new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
            double doubleValue = 0d;
            byte byteValue = 0;
            _ = NumberFormatHelper.TryParseDouble(value.ToString(), out doubleValue);
            if (doubleValue <= 255d && doubleValue >= 0d)
            {
                byteValue = (byte)Math.Round(doubleValue);
            }

            switch (Component)
            {
                case ColorComponent.A:
                    {
                        return new SolidColorBrush(Color.FromArgb(byteValue, _color.Color.R, _color.Color.G, _color.Color.B));
                    }
                case ColorComponent.R:
                    {
                        return new SolidColorBrush(Color.FromArgb(_color.Color.A, byteValue, _color.Color.G, _color.Color.B));
                    }
                case ColorComponent.G:
                    {
                        return new SolidColorBrush(Color.FromArgb(_color.Color.A, _color.Color.R, byteValue, _color.Color.B));
                    }
                case ColorComponent.B:
                    {
                        return new SolidColorBrush(Color.FromArgb(_color.Color.A, _color.Color.R, _color.Color.G, byteValue));
                    }

                default:
                    {
                        return new SolidColorBrush(Color.FromArgb(byteValue, byteValue, byteValue, byteValue));
                    }
            }
        }

    }
}
