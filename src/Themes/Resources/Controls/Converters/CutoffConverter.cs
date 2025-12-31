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
using System.Windows.Data;

namespace Themes
{
    /// <summary>
    /// Converts a numeric value to a boolean indicating whether it is below a cutoff threshold.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This converter is used in control templates to conditionally show or hide
    /// elements based on size thresholds. It supports both <see cref="double"/>
    /// and <see cref="GridLength"/> values.
    /// </para>
    /// <para>
    ///     <b>Authors:</b>
    ///     <list type="bullet">
    ///         <item>Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil</item>
    ///     </list>
    /// </para>
    /// </remarks>
    public class CutoffConverter : IValueConverter
    {
        /// <summary>
        /// Gets or sets the cutoff threshold value.
        /// </summary>
        /// <value>The cutoff threshold. Values below this return true.</value>
        public double Cutoff { get; set; }

        /// <summary>
        /// Converts a numeric value to a boolean indicating if it's below the cutoff.
        /// </summary>
        /// <param name="value">The value to compare. Must be a double or GridLength.</param>
        /// <param name="targetType">The target type (expected to be bool).</param>
        /// <param name="parameter">Optional parameter (not used).</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>
        /// <c>true</c> if the value is less than <see cref="Cutoff"/>; otherwise, <c>false</c>.
        /// Returns <c>false</c> if the value is null or not a supported type.
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return false;

            if (value is double doubleValue)
            {
                return doubleValue < Cutoff;
            }

            if (value is GridLength gridLength)
            {
                return gridLength.Value < Cutoff;
            }

            return false;
        }

        /// <summary>
        /// Not implemented. This converter only supports one-way binding.
        /// </summary>
        /// <exception cref="NotImplementedException">Always thrown.</exception>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
