/*
* NOTICE:
* The U.S. Army Corps of Engineers, Risk Management Center (USACE-RMC) makes no guarantees about
* the results, or appropriateness of outputs, obtained from this software.
*
* LIST OF CONDITIONS:
* Redistribution and use in source and binary forms, with or without modification, are permitted
* provided that the following conditions are met:
* - Redistributions of source code must retain the above notice, this list of conditions, and the
* following disclaimer.
* - Redistributions in binary form must reproduce the above notice, this list of conditions, and
* the following disclaimer in the documentation and/or other materials provided with the distribution.
* - The names of the U.S. Government, the U.S. Army Corps of Engineers, the Institute for Water
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

using System.Globalization;
using System.Windows;
using Numerics.Data;
using NumericControls;
using Xunit;

namespace NumericControls.Tests.Converters
{
    /// <summary>
    /// Tests for <see cref="DoubleToFontFamilyConverter"/>.
    /// </summary>
    public class DoubleToFontFamilyConverterTests
    {
        /// <summary>
        /// The converter instance used for testing.
        /// </summary>
        private readonly DoubleToFontFamilyConverter _converter = new();

        /// <summary>
        /// Tests that converting a normal value returns Normal font style.
        /// </summary>
        [Fact]
        public void Convert_NormalValue_ReturnsNormalFontStyle()
        {
            // Arrange
            double value = 42.0;

            // Act
            var result = _converter.Convert(value, typeof(FontStyle), null, CultureInfo.InvariantCulture);

            // Assert
            Assert.Equal(FontStyles.Normal, result);
        }

        /// <summary>
        /// Tests that converting NaN returns Italic font style.
        /// </summary>
        [Fact]
        public void Convert_NaN_ReturnsItalicFontStyle()
        {
            // Arrange
            double value = double.NaN;

            // Act
            var result = _converter.Convert(value, typeof(FontStyle), null, CultureInfo.InvariantCulture);

            // Assert
            Assert.Equal(FontStyles.Italic, result);
        }

        /// <summary>
        /// Tests that converting Positive Infinity returns Italic font style.
        /// </summary>
        [Fact]
        public void Convert_PositiveInfinity_ReturnsItalicFontStyle()
        {
            // Arrange
            double value = double.PositiveInfinity;

            // Act
            var result = _converter.Convert(value, typeof(FontStyle), null, CultureInfo.InvariantCulture);

            // Assert
            Assert.Equal(FontStyles.Italic, result);
        }

        /// <summary>
        /// Tests that converting Negative Infinity returns Italic font style.
        /// </summary>
        [Fact]
        public void Convert_NegativeInfinity_ReturnsItalicFontStyle()
        {
            // Arrange
            double value = double.NegativeInfinity;

            // Act
            var result = _converter.Convert(value, typeof(FontStyle), null, CultureInfo.InvariantCulture);

            // Assert
            Assert.Equal(FontStyles.Italic, result);
        }

        /// <summary>
        /// Tests that converting zero returns Normal font style.
        /// </summary>
        [Fact]
        public void Convert_Zero_ReturnsNormalFontStyle()
        {
            // Arrange
            double value = 0.0;

            // Act
            var result = _converter.Convert(value, typeof(FontStyle), null, CultureInfo.InvariantCulture);

            // Assert
            Assert.Equal(FontStyles.Normal, result);
        }

        /// <summary>
        /// Tests that converting a negative value returns Normal font style.
        /// </summary>
        [Fact]
        public void Convert_NegativeValue_ReturnsNormalFontStyle()
        {
            // Arrange
            double value = -123.456;

            // Act
            var result = _converter.Convert(value, typeof(FontStyle), null, CultureInfo.InvariantCulture);

            // Assert
            Assert.Equal(FontStyles.Normal, result);
        }

        /// <summary>
        /// Tests that converting Double.MaxValue returns Normal font style.
        /// </summary>
        [Fact]
        public void Convert_MaxValue_ReturnsNormalFontStyle()
        {
            // Arrange
            double value = double.MaxValue;

            // Act
            var result = _converter.Convert(value, typeof(FontStyle), null, CultureInfo.InvariantCulture);

            // Assert
            Assert.Equal(FontStyles.Normal, result);
        }

        /// <summary>
        /// Tests that converting Double.MinValue returns Normal font style.
        /// </summary>
        [Fact]
        public void Convert_MinValue_ReturnsNormalFontStyle()
        {
            // Arrange
            double value = double.MinValue;

            // Act
            var result = _converter.Convert(value, typeof(FontStyle), null, CultureInfo.InvariantCulture);

            // Assert
            Assert.Equal(FontStyles.Normal, result);
        }

        /// <summary>
        /// Tests that ConvertBack throws NotImplementedException.
        /// </summary>
        [Fact]
        public void ConvertBack_ThrowsNotImplementedException()
        {
            // Act & Assert
            Assert.Throws<NotImplementedException>(() =>
                _converter.ConvertBack(FontStyles.Normal, typeof(double), null, CultureInfo.InvariantCulture));
        }
    }
}
