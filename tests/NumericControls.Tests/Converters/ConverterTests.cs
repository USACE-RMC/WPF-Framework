/*
 * Unit tests for converters in the NumericControls library.
 * Tests DoubleToFontFamilyConverter, DateToStringConverter, and MathFunctionType converters.
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
        private readonly DoubleToFontFamilyConverter _converter = new();

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
        private readonly DateToStringConverter _converter = new();

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
        private readonly MathFunctionTypeToNameConverter _converter = new();

        [Fact]
        public void GetName_Logarithm_ReturnsLogarithmicTransform()
        {
            // Act
            var result = MathFunctionTypeToNameConverter.GetName(MathFunctionType.Logarithm);

            // Assert
            Assert.Equal("Logarithmic Transform", result);
        }

        [Fact]
        public void GetName_Add_ReturnsAdd()
        {
            // Act
            var result = MathFunctionTypeToNameConverter.GetName(MathFunctionType.Add);

            // Assert
            Assert.Equal("Add", result);
        }

        [Fact]
        public void GetName_Subtract_ReturnsSubtract()
        {
            // Act
            var result = MathFunctionTypeToNameConverter.GetName(MathFunctionType.Subtract);

            // Assert
            Assert.Equal("Subtract", result);
        }

        [Fact]
        public void GetName_Multiply_ReturnsMultiply()
        {
            // Act
            var result = MathFunctionTypeToNameConverter.GetName(MathFunctionType.Multiply);

            // Assert
            Assert.Equal("Multiply", result);
        }

        [Fact]
        public void GetName_Divide_ReturnsDivide()
        {
            // Act
            var result = MathFunctionTypeToNameConverter.GetName(MathFunctionType.Divide);

            // Assert
            Assert.Equal("Divide", result);
        }

        [Fact]
        public void GetName_Exponentiate_ReturnsExponentiate()
        {
            // Act
            var result = MathFunctionTypeToNameConverter.GetName(MathFunctionType.Exponentiate);

            // Assert
            Assert.Equal("Exponentiate", result);
        }

        [Fact]
        public void GetName_Inverse_ReturnsInverse()
        {
            // Act
            var result = MathFunctionTypeToNameConverter.GetName(MathFunctionType.Inverse);

            // Assert
            Assert.Equal("Inverse", result);
        }

        [Fact]
        public void GetName_Replace_ReturnsReplace()
        {
            // Act
            var result = MathFunctionTypeToNameConverter.GetName(MathFunctionType.Replace);

            // Assert
            Assert.Equal("Replace", result);
        }

        [Fact]
        public void GetName_Interpolate_ReturnsInterpolate()
        {
            // Act
            var result = MathFunctionTypeToNameConverter.GetName(MathFunctionType.Interpolate);

            // Assert
            Assert.Equal("Interpolate", result);
        }

        [Fact]
        public void Convert_NullValue_ReturnsNull()
        {
            // Act
            var result = _converter.Convert(null, typeof(string), null, CultureInfo.InvariantCulture);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void Convert_InvalidType_ReturnsNull()
        {
            // Act
            var result = _converter.Convert("not a MathFunctionType", typeof(string), null, CultureInfo.InvariantCulture);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void Convert_ValidMathFunctionType_ReturnsName()
        {
            // Act
            var result = _converter.Convert(MathFunctionType.Add, typeof(string), null, CultureInfo.InvariantCulture);

            // Assert
            Assert.Equal("Add", result);
        }

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
        private readonly MathFunctionTypeToTooltipConverter _converter = new();

        [Fact]
        public void GetTooltip_Add_ReturnsAddTooltip()
        {
            // Act
            var result = MathFunctionTypeToTooltipConverter.GetTooltip(MathFunctionType.Add);

            // Assert
            Assert.Contains("Add a constant to values", result);
            Assert.Contains("Missing values are kept as missing", result);
        }

        [Fact]
        public void GetTooltip_Subtract_ReturnsSubtractTooltip()
        {
            // Act
            var result = MathFunctionTypeToTooltipConverter.GetTooltip(MathFunctionType.Subtract);

            // Assert
            Assert.Contains("Subtract a constant from values", result);
        }

        [Fact]
        public void GetTooltip_Multiply_ReturnsMultiplyTooltip()
        {
            // Act
            var result = MathFunctionTypeToTooltipConverter.GetTooltip(MathFunctionType.Multiply);

            // Assert
            Assert.Contains("Multiply values by a constant", result);
        }

        [Fact]
        public void GetTooltip_Divide_ReturnsDivideTooltip()
        {
            // Act
            var result = MathFunctionTypeToTooltipConverter.GetTooltip(MathFunctionType.Divide);

            // Assert
            Assert.Contains("Divide values by a constant", result);
        }

        [Fact]
        public void GetTooltip_Exponentiate_ReturnsExponentiateTooltip()
        {
            // Act
            var result = MathFunctionTypeToTooltipConverter.GetTooltip(MathFunctionType.Exponentiate);

            // Assert
            Assert.Contains("Raise values to a constant power", result);
        }

        [Fact]
        public void GetTooltip_Logarithm_ReturnsLogTooltip()
        {
            // Act
            var result = MathFunctionTypeToTooltipConverter.GetTooltip(MathFunctionType.Logarithm);

            // Assert
            Assert.Contains("Log transform values", result);
        }

        [Fact]
        public void GetTooltip_Inverse_ReturnsInverseTooltip()
        {
            // Act
            var result = MathFunctionTypeToTooltipConverter.GetTooltip(MathFunctionType.Inverse);

            // Assert
            Assert.Contains("Replace values by its inverse (1/x)", result);
            Assert.Contains("Zero values are set to missing", result);
        }

        [Fact]
        public void GetTooltip_Replace_ReturnsReplaceTooltip()
        {
            // Act
            var result = MathFunctionTypeToTooltipConverter.GetTooltip(MathFunctionType.Replace);

            // Assert
            Assert.Contains("Replace missing data", result);
        }

        [Fact]
        public void GetTooltip_Interpolate_ReturnsInterpolateTooltip()
        {
            // Act
            var result = MathFunctionTypeToTooltipConverter.GetTooltip(MathFunctionType.Interpolate);

            // Assert
            Assert.Contains("Interpolate missing data", result);
        }

        [Fact]
        public void Convert_NullValue_ReturnsNull()
        {
            // Act
            var result = _converter.Convert(null, typeof(string), null, CultureInfo.InvariantCulture);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void Convert_InvalidType_ReturnsNull()
        {
            // Act
            var result = _converter.Convert(123, typeof(string), null, CultureInfo.InvariantCulture);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void Convert_ValidMathFunctionType_ReturnsTooltip()
        {
            // Act
            var result = _converter.Convert(MathFunctionType.Add, typeof(string), null, CultureInfo.InvariantCulture);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<string>(result);
        }

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
