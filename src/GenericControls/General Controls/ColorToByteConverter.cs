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
    /// <remarks>
    /// <para>
    /// <b> Authors: </b>
    /// <list type="bullet">
    ///     <item> Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil </item>
    /// </list>
    /// </para>
    /// </remarks>
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
    /// <remarks>
    /// <para>
    /// <b> Authors: </b>
    /// <list type="bullet">
    ///     <item> Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil </item>
    /// </list>
    /// </para>
    /// </remarks>
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
