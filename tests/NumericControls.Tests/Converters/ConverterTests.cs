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

    /// <summary>
    /// Tests for <see cref="MathFunctionTypeToNameConverter"/>.
    /// </summary>
    public class MathFunctionTypeToNameConverterTests
    {
        /// <summary>
        /// The converter instance used for testing.
        /// </summary>
        private readonly MathFunctionTypeToNameConverter _converter = new();

        /// <summary>
        /// Tests that GetName returns "Logarithmic Transform" for Logarithm function type.
        /// </summary>
        [Fact]
        public void GetName_Logarithm_ReturnsLogarithmicTransform()
        {
            // Act
            var result = MathFunctionTypeToNameConverter.GetName(MathFunctionType.Logarithm);

            // Assert
            Assert.Equal("Logarithmic Transform", result);
        }

        /// <summary>
        /// Tests that GetName returns "Add" for Add function type.
        /// </summary>
        [Fact]
        public void GetName_Add_ReturnsAdd()
        {
            // Act
            var result = MathFunctionTypeToNameConverter.GetName(MathFunctionType.Add);

            // Assert
            Assert.Equal("Add", result);
        }

        /// <summary>
        /// Tests that GetName returns "Subtract" for Subtract function type.
        /// </summary>
        [Fact]
        public void GetName_Subtract_ReturnsSubtract()
        {
            // Act
            var result = MathFunctionTypeToNameConverter.GetName(MathFunctionType.Subtract);

            // Assert
            Assert.Equal("Subtract", result);
        }

        /// <summary>
        /// Tests that GetName returns "Multiply" for Multiply function type.
        /// </summary>
        [Fact]
        public void GetName_Multiply_ReturnsMultiply()
        {
            // Act
            var result = MathFunctionTypeToNameConverter.GetName(MathFunctionType.Multiply);

            // Assert
            Assert.Equal("Multiply", result);
        }

        /// <summary>
        /// Tests that GetName returns "Divide" for Divide function type.
        /// </summary>
        [Fact]
        public void GetName_Divide_ReturnsDivide()
        {
            // Act
            var result = MathFunctionTypeToNameConverter.GetName(MathFunctionType.Divide);

            // Assert
            Assert.Equal("Divide", result);
        }

        /// <summary>
        /// Tests that GetName returns "Exponentiate" for Exponentiate function type.
        /// </summary>
        [Fact]
        public void GetName_Exponentiate_ReturnsExponentiate()
        {
            // Act
            var result = MathFunctionTypeToNameConverter.GetName(MathFunctionType.Exponentiate);

            // Assert
            Assert.Equal("Exponentiate", result);
        }

        /// <summary>
        /// Tests that GetName returns "Inverse" for Inverse function type.
        /// </summary>
        [Fact]
        public void GetName_Inverse_ReturnsInverse()
        {
            // Act
            var result = MathFunctionTypeToNameConverter.GetName(MathFunctionType.Inverse);

            // Assert
            Assert.Equal("Inverse", result);
        }

        /// <summary>
        /// Tests that GetName returns "Replace" for Replace function type.
        /// </summary>
        [Fact]
        public void GetName_Replace_ReturnsReplace()
        {
            // Act
            var result = MathFunctionTypeToNameConverter.GetName(MathFunctionType.Replace);

            // Assert
            Assert.Equal("Replace", result);
        }

        /// <summary>
        /// Tests that GetName returns "Interpolate" for Interpolate function type.
        /// </summary>
        [Fact]
        public void GetName_Interpolate_ReturnsInterpolate()
        {
            // Act
            var result = MathFunctionTypeToNameConverter.GetName(MathFunctionType.Interpolate);

            // Assert
            Assert.Equal("Interpolate", result);
        }

        /// <summary>
        /// Tests that Convert returns null when value is null.
        /// </summary>
        [Fact]
        public void Convert_NullValue_ReturnsNull()
        {
            // Act
            var result = _converter.Convert(null, typeof(string), null, CultureInfo.InvariantCulture);

            // Assert
            Assert.Null(result);
        }

        /// <summary>
        /// Tests that Convert returns null when value is not a MathFunctionType.
        /// </summary>
        [Fact]
        public void Convert_InvalidType_ReturnsNull()
        {
            // Act
            var result = _converter.Convert("not a MathFunctionType", typeof(string), null, CultureInfo.InvariantCulture);

            // Assert
            Assert.Null(result);
        }

        /// <summary>
        /// Tests that Convert returns the correct name for a valid MathFunctionType.
        /// </summary>
        [Fact]
        public void Convert_ValidMathFunctionType_ReturnsName()
        {
            // Act
            var result = _converter.Convert(MathFunctionType.Add, typeof(string), null, CultureInfo.InvariantCulture);

            // Assert
            Assert.Equal("Add", result);
        }

        /// <summary>
        /// Tests that ConvertBack throws NotImplementedException.
        /// </summary>
        [Fact]
        public void ConvertBack_ThrowsNotImplementedException()
        {
            // Act & Assert
            Assert.Throws<NotImplementedException>(() =>
                _converter.ConvertBack("Add", typeof(MathFunctionType), null, CultureInfo.InvariantCulture));
        }
    }

    /// <summary>
    /// Tests for <see cref="MathFunctionTypeToTooltipConverter"/>.
    /// </summary>
    public class MathFunctionTypeToTooltipConverterTests
    {
        /// <summary>
        /// The converter instance used for testing.
        /// </summary>
        private readonly MathFunctionTypeToTooltipConverter _converter = new();

        /// <summary>
        /// Tests that GetTooltip returns the correct tooltip for Add function type.
        /// </summary>
        [Fact]
        public void GetTooltip_Add_ReturnsAddTooltip()
        {
            // Act
            var result = MathFunctionTypeToTooltipConverter.GetTooltip(MathFunctionType.Add);

            // Assert
            Assert.Contains("Add a constant to values", result);
            Assert.Contains("Missing values are kept as missing", result);
        }

        /// <summary>
        /// Tests that GetTooltip returns the correct tooltip for Subtract function type.
        /// </summary>
        [Fact]
        public void GetTooltip_Subtract_ReturnsSubtractTooltip()
        {
            // Act
            var result = MathFunctionTypeToTooltipConverter.GetTooltip(MathFunctionType.Subtract);

            // Assert
            Assert.Contains("Subtract a constant from values", result);
        }

        /// <summary>
        /// Tests that GetTooltip returns the correct tooltip for Multiply function type.
        /// </summary>
        [Fact]
        public void GetTooltip_Multiply_ReturnsMultiplyTooltip()
        {
            // Act
            var result = MathFunctionTypeToTooltipConverter.GetTooltip(MathFunctionType.Multiply);

            // Assert
            Assert.Contains("Multiply values by a constant", result);
        }

        /// <summary>
        /// Tests that GetTooltip returns the correct tooltip for Divide function type.
        /// </summary>
        [Fact]
        public void GetTooltip_Divide_ReturnsDivideTooltip()
        {
            // Act
            var result = MathFunctionTypeToTooltipConverter.GetTooltip(MathFunctionType.Divide);

            // Assert
            Assert.Contains("Divide values by a constant", result);
        }

        /// <summary>
        /// Tests that GetTooltip returns the correct tooltip for Exponentiate function type.
        /// </summary>
        [Fact]
        public void GetTooltip_Exponentiate_ReturnsExponentiateTooltip()
        {
            // Act
            var result = MathFunctionTypeToTooltipConverter.GetTooltip(MathFunctionType.Exponentiate);

            // Assert
            Assert.Contains("Raise values to a constant power", result);
        }

        /// <summary>
        /// Tests that GetTooltip returns the correct tooltip for Logarithm function type.
        /// </summary>
        [Fact]
        public void GetTooltip_Logarithm_ReturnsLogTooltip()
        {
            // Act
            var result = MathFunctionTypeToTooltipConverter.GetTooltip(MathFunctionType.Logarithm);

            // Assert
            Assert.Contains("Log transform values", result);
        }

        /// <summary>
        /// Tests that GetTooltip returns the correct tooltip for Inverse function type.
        /// </summary>
        [Fact]
        public void GetTooltip_Inverse_ReturnsInverseTooltip()
        {
            // Act
            var result = MathFunctionTypeToTooltipConverter.GetTooltip(MathFunctionType.Inverse);

            // Assert
            Assert.Contains("Replace values by its inverse (1/x)", result);
            Assert.Contains("Zero values are set to missing", result);
        }

        /// <summary>
        /// Tests that GetTooltip returns the correct tooltip for Replace function type.
        /// </summary>
        [Fact]
        public void GetTooltip_Replace_ReturnsReplaceTooltip()
        {
            // Act
            var result = MathFunctionTypeToTooltipConverter.GetTooltip(MathFunctionType.Replace);

            // Assert
            Assert.Contains("Replace missing data", result);
        }

        /// <summary>
        /// Tests that GetTooltip returns the correct tooltip for Interpolate function type.
        /// </summary>
        [Fact]
        public void GetTooltip_Interpolate_ReturnsInterpolateTooltip()
        {
            // Act
            var result = MathFunctionTypeToTooltipConverter.GetTooltip(MathFunctionType.Interpolate);

            // Assert
            Assert.Contains("Interpolate missing data", result);
        }

        /// <summary>
        /// Tests that Convert returns null when value is null.
        /// </summary>
        [Fact]
        public void Convert_NullValue_ReturnsNull()
        {
            // Act
            var result = _converter.Convert(null, typeof(string), null, CultureInfo.InvariantCulture);

            // Assert
            Assert.Null(result);
        }

        /// <summary>
        /// Tests that Convert returns null when value is not a MathFunctionType.
        /// </summary>
        [Fact]
        public void Convert_InvalidType_ReturnsNull()
        {
            // Act
            var result = _converter.Convert(123, typeof(string), null, CultureInfo.InvariantCulture);

            // Assert
            Assert.Null(result);
        }

        /// <summary>
        /// Tests that Convert returns a tooltip string for a valid MathFunctionType.
        /// </summary>
        [Fact]
        public void Convert_ValidMathFunctionType_ReturnsTooltip()
        {
            // Act
            var result = _converter.Convert(MathFunctionType.Add, typeof(string), null, CultureInfo.InvariantCulture);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<string>(result);
        }

        /// <summary>
        /// Tests that ConvertBack throws NotImplementedException.
        /// </summary>
        [Fact]
        public void ConvertBack_ThrowsNotImplementedException()
        {
            // Act & Assert
            Assert.Throws<NotImplementedException>(() =>
                _converter.ConvertBack("tooltip", typeof(MathFunctionType), null, CultureInfo.InvariantCulture));
        }
    }

    /// <summary>
    /// Tests for <see cref="MathFunctionTypeToIconConverter"/>.
    /// </summary>
    public class MathFunctionTypeToIconConverterTests
    {
        /// <summary>
        /// Tests that GetIconResourceKey returns "PlusIcon" for Add function type.
        /// </summary>
        [Fact]
        public void GetIconResourceKey_Add_ReturnsPlusIcon()
        {
            // Act
            var result = MathFunctionTypeToIconConverter.GetIconResourceKey(MathFunctionType.Add);

            // Assert
            Assert.Equal("PlusIcon", result);
        }

        [Fact]
        public void GetIconResourceKey_Subtract_ReturnsMinusIcon()
        {
            // Act
            var result = MathFunctionTypeToIconConverter.GetIconResourceKey(MathFunctionType.Subtract);

            // Assert
            Assert.Equal("MinusIcon", result);
        }

        [Fact]
        public void GetIconResourceKey_Multiply_ReturnsMultiplyIcon()
        {
            // Act
            var result = MathFunctionTypeToIconConverter.GetIconResourceKey(MathFunctionType.Multiply);

            // Assert
            Assert.Equal("MultiplyIcon", result);
        }

        [Fact]
        public void GetIconResourceKey_Divide_ReturnsDivideIcon()
        {
            // Act
            var result = MathFunctionTypeToIconConverter.GetIconResourceKey(MathFunctionType.Divide);

            // Assert
            Assert.Equal("DivideIcon", result);
        }

        [Fact]
        public void GetIconResourceKey_Exponentiate_ReturnsExpIcon()
        {
            // Act
            var result = MathFunctionTypeToIconConverter.GetIconResourceKey(MathFunctionType.Exponentiate);

            // Assert
            Assert.Equal("ExpIcon", result);
        }

        [Fact]
        public void GetIconResourceKey_Logarithm_ReturnsLogIcon()
        {
            // Act
            var result = MathFunctionTypeToIconConverter.GetIconResourceKey(MathFunctionType.Logarithm);

            // Assert
            Assert.Equal("LogIcon", result);
        }

        [Fact]
        public void GetIconResourceKey_Inverse_ReturnsInvertIcon()
        {
            // Act
            var result = MathFunctionTypeToIconConverter.GetIconResourceKey(MathFunctionType.Inverse);

            // Assert
            Assert.Equal("InvertIcon", result);
        }

        [Fact]
        public void GetIconResourceKey_Replace_ReturnsReplaceIcon()
        {
            // Act
            var result = MathFunctionTypeToIconConverter.GetIconResourceKey(MathFunctionType.Replace);

            // Assert
            Assert.Equal("ReplaceIcon", result);
        }

        [Fact]
        public void GetIconResourceKey_Interpolate_ReturnsInterpolateIcon()
        {
            // Act
            var result = MathFunctionTypeToIconConverter.GetIconResourceKey(MathFunctionType.Interpolate);

            // Assert
            Assert.Equal("InterpolateIcon", result);
        }

        [Fact]
        public void GetIconResourceKey_AllFunctionTypes_ReturnValidKeys()
        {
            // Act & Assert
            foreach (MathFunctionType functionType in Enum.GetValues(typeof(MathFunctionType)))
            {
                var result = MathFunctionTypeToIconConverter.GetIconResourceKey(functionType);
                Assert.NotNull(result);
                Assert.NotEmpty(result);
            }
        }
    }
}
