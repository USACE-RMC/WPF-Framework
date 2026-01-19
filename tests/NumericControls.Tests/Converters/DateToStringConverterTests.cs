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
    /// Tests for <see cref="DateToStringConverter"/>.
    /// </summary>
    public class DateToStringConverterTests
    {
        /// <summary>
        /// The converter instance used for testing.
        /// </summary>
        private readonly DateToStringConverter _converter = new();

        /// <summary>
        /// Tests that converting a valid date returns a formatted string.
        /// </summary>
        [Fact]
        public void Convert_ValidDate_ReturnsFormattedString()
        {
            // Arrange
            var date = new DateTime(2024, 6, 15, 14, 30, 0);

            // Act
            var result = _converter.Convert(date, typeof(string), null, CultureInfo.InvariantCulture);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<string>(result);
            var resultString = (string)result;
            Assert.Contains("2024", resultString); // Year should be present
        }

        /// <summary>
        /// Tests that converting DateTime.MinValue returns a formatted string.
        /// </summary>
        [Fact]
        public void Convert_MinDate_ReturnsFormattedString()
        {
            // Arrange
            var date = DateTime.MinValue;

            // Act
            var result = _converter.Convert(date, typeof(string), null, CultureInfo.InvariantCulture);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<string>(result);
        }

        /// <summary>
        /// Tests that converting DateTime.MaxValue returns a formatted string.
        /// </summary>
        [Fact]
        public void Convert_MaxDate_ReturnsFormattedString()
        {
            // Arrange
            var date = DateTime.MaxValue;

            // Act
            var result = _converter.Convert(date, typeof(string), null, CultureInfo.InvariantCulture);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<string>(result);
        }

        /// <summary>
        /// Tests that converting back a valid string returns a DateTime.
        /// </summary>
        [Fact]
        public void ConvertBack_ValidString_ReturnsDateTime()
        {
            // Arrange
            var date = new DateTime(2024, 6, 15, 14, 30, 0);
            var dateString = (string)_converter.Convert(date, typeof(string), null, CultureInfo.InvariantCulture);

            // Act
            var result = _converter.ConvertBack(dateString, typeof(DateTime), null, CultureInfo.InvariantCulture);

            // Assert
            Assert.IsType<DateTime>(result);
        }

        /// <summary>
        /// Tests that converting back an invalid string returns default DateTime.
        /// </summary>
        [Fact]
        public void ConvertBack_InvalidString_ReturnsDefaultDateTime()
        {
            // Arrange
            var invalidString = "not a date";

            // Act
            var result = _converter.ConvertBack(invalidString, typeof(DateTime), null, CultureInfo.InvariantCulture);

            // Assert
            Assert.IsType<DateTime>(result);
            Assert.Equal(default(DateTime), result);
        }

        /// <summary>
        /// Tests that converting back an empty string returns default DateTime.
        /// </summary>
        [Fact]
        public void ConvertBack_EmptyString_ReturnsDefaultDateTime()
        {
            // Arrange
            var emptyString = "";

            // Act
            var result = _converter.ConvertBack(emptyString, typeof(DateTime), null, CultureInfo.InvariantCulture);

            // Assert
            Assert.IsType<DateTime>(result);
            Assert.Equal(default(DateTime), result);
        }

        /// <summary>
        /// Tests that round-trip conversion preserves the DateTime value.
        /// </summary>
        [Fact]
        public void RoundTrip_DateTimeConversion_PreservesValue()
        {
            // Arrange
            var originalDate = new DateTime(2024, 3, 21, 10, 45, 0);

            // Act
            var stringValue = _converter.Convert(originalDate, typeof(string), null, CultureInfo.InvariantCulture);
            var roundTrippedDate = (DateTime)_converter.ConvertBack(stringValue, typeof(DateTime), null, CultureInfo.InvariantCulture);

            // Assert - comparing up to minutes (ignoring seconds for format compatibility)
            Assert.Equal(originalDate.Year, roundTrippedDate.Year);
            Assert.Equal(originalDate.Month, roundTrippedDate.Month);
            Assert.Equal(originalDate.Day, roundTrippedDate.Day);
            Assert.Equal(originalDate.Hour, roundTrippedDate.Hour);
            Assert.Equal(originalDate.Minute, roundTrippedDate.Minute);
        }
    }
}
