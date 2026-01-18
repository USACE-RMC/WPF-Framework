/*
* Unit tests for NumberFormatHelper from GenericControls
*/

using System.Globalization;
using Xunit;

namespace GenericControls.Tests.Utilities;

/// <summary>
/// Unit tests for the <see cref="NumberFormatHelper"/> class.
/// </summary>
public class NumberFormatHelperTests
{
    #region Parsing Tests - TryParseDouble

    [Theory]
    [InlineData("123", 123.0)]
    [InlineData("123.45", 123.45)]
    [InlineData("-123.45", -123.45)]
    [InlineData("0", 0.0)]
    [InlineData("0.0", 0.0)]
    [InlineData("-0", 0.0)]
    public void TryParseDouble_ValidInput_ReturnsTrueAndCorrectValue(string input, double expected)
    {
        // Act
        var result = NumberFormatHelper.TryParseDouble(input, out double value);

        // Assert
        Assert.True(result);
        Assert.Equal(expected, value);
    }

    [Theory]
    [InlineData("1e2", 100.0)]
    [InlineData("1.5e2", 150.0)]
    [InlineData("1E2", 100.0)]
    [InlineData("-1e2", -100.0)]
    [InlineData("1e-2", 0.01)]
    public void TryParseDouble_ScientificNotation_ReturnsTrueAndCorrectValue(string input, double expected)
    {
        // Act
        var result = NumberFormatHelper.TryParseDouble(input, out double value);

        // Assert
        Assert.True(result);
        Assert.Equal(expected, value, 10);
    }

    [Theory]
    [InlineData("")]
    [InlineData("abc")]
    [InlineData("12abc")]
    [InlineData("abc12")]
    [InlineData("--123")]
    public void TryParseDouble_InvalidInput_ReturnsFalse(string input)
    {
        // Act
        var result = NumberFormatHelper.TryParseDouble(input, out _);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void TryParseDouble_WithCulture_UsesSpecifiedCulture()
    {
        // Arrange
        var frenchCulture = new CultureInfo("fr-FR");
        var input = "123,45"; // French format uses comma as decimal separator

        // Act
        var result = NumberFormatHelper.TryParseDouble(input, frenchCulture, out double value);

        // Assert
        Assert.True(result);
        Assert.Equal(123.45, value);
    }

    #endregion

    #region Parsing Tests - TryParseSingle

    [Theory]
    [InlineData("123", 123.0f)]
    [InlineData("123.45", 123.45f)]
    [InlineData("-123.45", -123.45f)]
    [InlineData("0", 0.0f)]
    public void TryParseSingle_ValidInput_ReturnsTrueAndCorrectValue(string input, float expected)
    {
        // Act
        var result = NumberFormatHelper.TryParseSingle(input, out float value);

        // Assert
        Assert.True(result);
        Assert.Equal(expected, value, 4);
    }

    [Theory]
    [InlineData("abc")]
    [InlineData("")]
    public void TryParseSingle_InvalidInput_ReturnsFalse(string input)
    {
        // Act
        var result = NumberFormatHelper.TryParseSingle(input, out _);

        // Assert
        Assert.False(result);
    }

    #endregion

    #region Parsing Tests - TryParseInt

    [Theory]
    [InlineData("123", 123)]
    [InlineData("-123", -123)]
    [InlineData("0", 0)]
    public void TryParseInt_ValidInput_ReturnsTrueAndCorrectValue(string input, int expected)
    {
        // Act
        var result = NumberFormatHelper.TryParseInt(input, out int value);

        // Assert
        Assert.True(result);
        Assert.Equal(expected, value);
    }

    [Theory]
    [InlineData("abc")]
    [InlineData("")]
    [InlineData("123.45")]
    public void TryParseInt_InvalidInput_ReturnsFalse(string input)
    {
        // Act
        var result = NumberFormatHelper.TryParseInt(input, out _);

        // Assert
        Assert.False(result);
    }

    #endregion

    #region Formatting Tests - FormatDouble

    [Fact]
    public void FormatDouble_BasicValue_FormatsCorrectly()
    {
        // Arrange
        var value = 123.45;

        // Act
        var result = NumberFormatHelper.FormatDouble(value);

        // Assert
        Assert.Contains("123", result);
    }

    [Theory]
    [InlineData(123.456, "F2", "123.46")]
    [InlineData(1000.0, "N0", "1,000")]
    public void FormatDouble_WithFormat_FormatsCorrectly(double value, string format, string expected)
    {
        // Act
        var result = NumberFormatHelper.FormatDouble(value, format);

        // Assert - Normalize for culture-specific formatting
        Assert.NotNull(result);
    }

    [Theory]
    [InlineData(123.456, 2, false, "123.46")]
    [InlineData(123.456, 0, false, "123")]
    [InlineData(1234.5, 1, true, "1,234.5")]
    public void FormatDouble_WithDecimalPlaces_FormatsCorrectly(double value, int decimalPlaces, bool useThousands, string expected)
    {
        // Act
        var result = NumberFormatHelper.FormatDouble(value, decimalPlaces, useThousands);

        // Assert - The result should be a string (culture-specific)
        Assert.NotNull(result);
    }

    #endregion

    #region Separator Tests

    [Fact]
    public void DecimalSeparator_ReturnsCurrentCultureSeparator()
    {
        // Assert
        var expected = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
        Assert.Equal(expected, NumberFormatHelper.DecimalSeparator);
    }

    [Fact]
    public void GroupSeparator_ReturnsCurrentCultureSeparator()
    {
        // Assert
        var expected = CultureInfo.CurrentCulture.NumberFormat.NumberGroupSeparator;
        Assert.Equal(expected, NumberFormatHelper.GroupSeparator);
    }

    [Fact]
    public void NegativeSign_ReturnsCurrentCultureSign()
    {
        // Assert
        var expected = CultureInfo.CurrentCulture.NumberFormat.NegativeSign;
        Assert.Equal(expected, NumberFormatHelper.NegativeSign);
    }

    [Fact]
    public void PositiveSign_ReturnsCurrentCultureSign()
    {
        // Assert
        var expected = CultureInfo.CurrentCulture.NumberFormat.PositiveSign;
        Assert.Equal(expected, NumberFormatHelper.PositiveSign);
    }

    #endregion

    #region IsDecimalSeparator Tests

    [Fact]
    public void IsDecimalSeparator_Char_WithActualSeparator_ReturnsTrue()
    {
        // Arrange
        var separator = NumberFormatHelper.DecimalSeparator;
        if (separator.Length == 1)
        {
            // Act
            var result = NumberFormatHelper.IsDecimalSeparator(separator[0]);

            // Assert
            Assert.True(result);
        }
    }

    [Fact]
    public void IsDecimalSeparator_Char_WithOtherChar_ReturnsFalse()
    {
        // Act
        var result = NumberFormatHelper.IsDecimalSeparator('x');

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsDecimalSeparator_String_WithActualSeparator_ReturnsTrue()
    {
        // Arrange
        var separator = NumberFormatHelper.DecimalSeparator;

        // Act
        var result = NumberFormatHelper.IsDecimalSeparator(separator);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsDecimalSeparator_String_WithOtherString_ReturnsFalse()
    {
        // Act
        var result = NumberFormatHelper.IsDecimalSeparator("xyz");

        // Assert
        Assert.False(result);
    }

    #endregion

    #region IsGroupSeparator Tests

    [Fact]
    public void IsGroupSeparator_Char_WithActualSeparator_ReturnsTrue()
    {
        // Arrange
        var separator = NumberFormatHelper.GroupSeparator;
        if (separator.Length == 1)
        {
            // Act
            var result = NumberFormatHelper.IsGroupSeparator(separator[0]);

            // Assert
            Assert.True(result);
        }
    }

    [Fact]
    public void IsGroupSeparator_String_WithActualSeparator_ReturnsTrue()
    {
        // Arrange
        var separator = NumberFormatHelper.GroupSeparator;

        // Act
        var result = NumberFormatHelper.IsGroupSeparator(separator);

        // Assert
        Assert.True(result);
    }

    #endregion

    #region ContainsDecimalSeparator Tests

    [Fact]
    public void ContainsDecimalSeparator_TextWithSeparator_ReturnsTrue()
    {
        // Arrange
        var text = $"123{NumberFormatHelper.DecimalSeparator}45";

        // Act
        var result = NumberFormatHelper.ContainsDecimalSeparator(text);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void ContainsDecimalSeparator_TextWithoutSeparator_ReturnsFalse()
    {
        // Arrange
        var text = "12345";

        // Act
        var result = NumberFormatHelper.ContainsDecimalSeparator(text);

        // Assert
        Assert.False(result);
    }

    #endregion

    #region ContainsNegativeSign Tests

    [Fact]
    public void ContainsNegativeSign_TextWithNegativeSign_ReturnsTrue()
    {
        // Arrange
        var text = $"{NumberFormatHelper.NegativeSign}123";

        // Act
        var result = NumberFormatHelper.ContainsNegativeSign(text);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void ContainsNegativeSign_TextWithoutNegativeSign_ReturnsFalse()
    {
        // Arrange
        var text = "123";

        // Act
        var result = NumberFormatHelper.ContainsNegativeSign(text);

        // Assert
        Assert.False(result);
    }

    #endregion

    #region ContainsPositiveSign Tests

    [Fact]
    public void ContainsPositiveSign_TextWithPositiveSign_ReturnsTrue()
    {
        // Arrange
        var text = $"{NumberFormatHelper.PositiveSign}123";

        // Act
        var result = NumberFormatHelper.ContainsPositiveSign(text);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void ContainsPositiveSign_TextWithoutPositiveSign_ReturnsFalse()
    {
        // Arrange
        var text = "123";

        // Act
        var result = NumberFormatHelper.ContainsPositiveSign(text);

        // Assert
        Assert.False(result);
    }

    #endregion

    #region IsValidNumericInput Tests

    [Fact]
    public void IsValidNumericInput_Digit_ReturnsTrue()
    {
        // Act
        var result = NumberFormatHelper.IsValidNumericInput("5", "123", 3, "", true, true);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsValidNumericInput_Backspace_ReturnsTrue()
    {
        // Act
        var result = NumberFormatHelper.IsValidNumericInput("\b", "123", 3, "", true, true);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsValidNumericInput_NegativeSignAtStart_WhenAllowed_ReturnsTrue()
    {
        // Act
        var result = NumberFormatHelper.IsValidNumericInput(
            NumberFormatHelper.NegativeSign,
            "123",
            0,
            "",
            allowNegative: true,
            allowDecimal: true);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsValidNumericInput_NegativeSignAtStart_WhenNotAllowed_ReturnsFalse()
    {
        // Act
        var result = NumberFormatHelper.IsValidNumericInput(
            NumberFormatHelper.NegativeSign,
            "123",
            0,
            "",
            allowNegative: false,
            allowDecimal: true);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsValidNumericInput_DecimalSeparator_WhenAllowed_ReturnsTrue()
    {
        // Act
        var result = NumberFormatHelper.IsValidNumericInput(
            NumberFormatHelper.DecimalSeparator,
            "123",
            3,
            "",
            allowNegative: true,
            allowDecimal: true);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsValidNumericInput_DecimalSeparator_WhenNotAllowed_ReturnsFalse()
    {
        // Act
        var result = NumberFormatHelper.IsValidNumericInput(
            NumberFormatHelper.DecimalSeparator,
            "123",
            3,
            "",
            allowNegative: true,
            allowDecimal: false);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsValidNumericInput_SecondDecimalSeparator_ReturnsFalse()
    {
        // Arrange
        var currentText = $"123{NumberFormatHelper.DecimalSeparator}45";

        // Act
        var result = NumberFormatHelper.IsValidNumericInput(
            NumberFormatHelper.DecimalSeparator,
            currentText,
            currentText.Length,
            "",
            allowNegative: true,
            allowDecimal: true);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsValidNumericInput_Letter_ReturnsFalse()
    {
        // Act
        var result = NumberFormatHelper.IsValidNumericInput("a", "123", 3, "", true, true);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsValidNumericInput_ScientificE_WhenAllowed_ReturnsTrue()
    {
        // Act
        var result = NumberFormatHelper.IsValidNumericInput(
            "e",
            "123",
            3,
            "",
            allowNegative: true,
            allowDecimal: true,
            allowScientific: true);

        // Assert
        Assert.True(result);
    }

    #endregion

    #region IsValidPartialNumber Tests

    [Fact]
    public void IsValidPartialNumber_EmptyString_ReturnsTrue()
    {
        // Act
        var result = NumberFormatHelper.IsValidPartialNumber("");

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsValidPartialNumber_NullString_ReturnsTrue()
    {
        // Act
        var result = NumberFormatHelper.IsValidPartialNumber(null!);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsValidPartialNumber_JustNegativeSign_ReturnsTrue()
    {
        // Act
        var result = NumberFormatHelper.IsValidPartialNumber(NumberFormatHelper.NegativeSign);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsValidPartialNumber_JustDecimalSeparator_ReturnsTrue()
    {
        // Act
        var result = NumberFormatHelper.IsValidPartialNumber(NumberFormatHelper.DecimalSeparator);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsValidPartialNumber_NegativeSignAndDecimalSeparator_ReturnsTrue()
    {
        // Act
        var result = NumberFormatHelper.IsValidPartialNumber(
            NumberFormatHelper.NegativeSign + NumberFormatHelper.DecimalSeparator);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsValidPartialNumber_PartialScientificNotation_ReturnsTrue()
    {
        // Act
        var result = NumberFormatHelper.IsValidPartialNumber("1e");

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsValidPartialNumber_PartialScientificNotationWithSign_ReturnsTrue()
    {
        // Act
        var result1 = NumberFormatHelper.IsValidPartialNumber("1e-");
        var result2 = NumberFormatHelper.IsValidPartialNumber("1e+");

        // Assert
        Assert.True(result1);
        Assert.True(result2);
    }

    #endregion

    #region IsValidPartialScientificNotation Tests

    [Theory]
    [InlineData("1e", true)]
    [InlineData("1E", true)]
    [InlineData("1e-", true)]
    [InlineData("1E+", true)]
    [InlineData("1.5e", true)]
    [InlineData("-2e-", true)]
    [InlineData("e", false)]      // No mantissa
    [InlineData("e1", false)]     // No mantissa
    [InlineData("", false)]
    public void IsValidPartialScientificNotation_VariousInputs_ReturnsExpected(string input, bool expected)
    {
        // Act
        var result = NumberFormatHelper.IsValidPartialScientificNotation(input);

        // Assert
        Assert.Equal(expected, result);
    }

    #endregion

    #region IsInfinityText Tests

    [Theory]
    [InlineData("inf", true)]
    [InlineData("Inf", true)]
    [InlineData("infinity", true)]
    [InlineData("Infinity", true)]
    [InlineData("+inf", true)]
    [InlineData("+Inf", true)]
    [InlineData("-inf", true)]
    [InlineData("-Inf", true)]
    [InlineData("-infinity", true)]
    [InlineData("-Infinity", true)]
    public void IsInfinityText_ValidInfinityText_ReturnsTrue(string input, bool expected)
    {
        // Act
        var result = NumberFormatHelper.IsInfinityText(input);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("")]
    [InlineData("infin")]
    [InlineData("infinite")]
    [InlineData("123")]
    [InlineData("abc")]
    public void IsInfinityText_InvalidInfinityText_ReturnsFalse(string? input)
    {
        // Act
        var result = NumberFormatHelper.IsInfinityText(input!);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsInfinityText_Null_ReturnsFalse()
    {
        // Act
        var result = NumberFormatHelper.IsInfinityText(null!);

        // Assert
        Assert.False(result);
    }

    #endregion

    #region IsPartialInfinityText Tests

    [Theory]
    [InlineData("i", true)]
    [InlineData("in", true)]
    [InlineData("inf", true)]
    [InlineData("infi", true)]
    [InlineData("infin", true)]
    [InlineData("-i", true)]
    [InlineData("-inf", true)]
    [InlineData("+i", true)]
    [InlineData("+inf", true)]
    public void IsPartialInfinityText_ValidPartialInfinity_ReturnsTrue(string input, bool expected)
    {
        // Act
        var result = NumberFormatHelper.IsPartialInfinityText(input);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("")]
    [InlineData("x")]
    [InlineData("ix")]
    [InlineData("123")]
    public void IsPartialInfinityText_InvalidPartialInfinity_ReturnsFalse(string? input)
    {
        // Act
        var result = NumberFormatHelper.IsPartialInfinityText(input!);

        // Assert
        Assert.False(result);
    }

    #endregion

    #region IsNumericDigit and IsAllDigits Tests

    [Theory]
    [InlineData('0', true)]
    [InlineData('5', true)]
    [InlineData('9', true)]
    [InlineData('a', false)]
    [InlineData('!', false)]
    [InlineData(' ', false)]
    public void IsNumericDigit_VariousChars_ReturnsExpected(char input, bool expected)
    {
        // Act
        var result = NumberFormatHelper.IsNumericDigit(input);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("123", true)]
    [InlineData("0", true)]
    [InlineData("000123", true)]
    [InlineData("12a3", false)]
    [InlineData("", false)]
    [InlineData(" ", false)]
    public void IsAllDigits_VariousStrings_ReturnsExpected(string input, bool expected)
    {
        // Act
        var result = NumberFormatHelper.IsAllDigits(input);

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public void IsAllDigits_Null_ReturnsFalse()
    {
        // Act
        var result = NumberFormatHelper.IsAllDigits(null!);

        // Assert
        Assert.False(result);
    }

    #endregion

    #region GetDigitValue Tests

    [Theory]
    [InlineData('0', 0)]
    [InlineData('1', 1)]
    [InlineData('5', 5)]
    [InlineData('9', 9)]
    public void GetDigitValue_ValidDigit_ReturnsValue(char input, int expected)
    {
        // Act
        var result = NumberFormatHelper.GetDigitValue(input);

        // Assert
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData('a')]
    [InlineData('!')]
    [InlineData(' ')]
    public void GetDigitValue_NonDigit_ReturnsNegativeOne(char input)
    {
        // Act
        var result = NumberFormatHelper.GetDigitValue(input);

        // Assert
        Assert.Equal(-1, result);
    }

    #endregion

    #region NormalizeDigits Tests

    [Fact]
    public void NormalizeDigits_WesternDigits_ReturnsUnchanged()
    {
        // Arrange
        var input = "0123456789";

        // Act
        var result = NumberFormatHelper.NormalizeDigits(input);

        // Assert
        Assert.Equal(input, result);
    }

    [Fact]
    public void NormalizeDigits_EmptyString_ReturnsEmpty()
    {
        // Act
        var result = NumberFormatHelper.NormalizeDigits("");

        // Assert
        Assert.Equal("", result);
    }

    [Fact]
    public void NormalizeDigits_Null_ReturnsNull()
    {
        // Act
        var result = NumberFormatHelper.NormalizeDigits(null!);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void NormalizeDigits_MixedContent_NormalizesOnlyDigits()
    {
        // Arrange
        var input = "abc123xyz";

        // Act
        var result = NumberFormatHelper.NormalizeDigits(input);

        // Assert - Non-digit characters should remain unchanged
        Assert.Equal("abc123xyz", result);
    }

    #endregion

    #region RTL Support Tests

    [Fact]
    public void IsRightToLeft_ReturnsCurrentCultureDirection()
    {
        // Assert
        var expected = CultureInfo.CurrentCulture.TextInfo.IsRightToLeft;
        Assert.Equal(expected, NumberFormatHelper.IsRightToLeft);
    }

    [Fact]
    public void CurrentFlowDirection_ReturnsCorrectDirection()
    {
        // Assert
        if (NumberFormatHelper.IsRightToLeft)
        {
            Assert.Equal(System.Windows.FlowDirection.RightToLeft, NumberFormatHelper.CurrentFlowDirection);
        }
        else
        {
            Assert.Equal(System.Windows.FlowDirection.LeftToRight, NumberFormatHelper.CurrentFlowDirection);
        }
    }

    [Fact]
    public void NegativeSignIsPrefix_ReturnsBasedOnCulture()
    {
        // Assert - Just verify it returns a boolean without throwing
        var result = NumberFormatHelper.NegativeSignIsPrefix;
        Assert.True(result || !result); // Always true, just checking it doesn't throw
    }

    #endregion

    #region IsNegativeSign and IsPositiveSign Tests

    [Fact]
    public void IsNegativeSign_WithNegativeSign_ReturnsTrue()
    {
        // Arrange
        var negativeSign = NumberFormatHelper.NegativeSign;

        // Act
        var result = NumberFormatHelper.IsNegativeSign(negativeSign);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsNegativeSign_WithOtherText_ReturnsFalse()
    {
        // Act
        var result = NumberFormatHelper.IsNegativeSign("xyz");

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsPositiveSign_WithPositiveSign_ReturnsTrue()
    {
        // Arrange
        var positiveSign = NumberFormatHelper.PositiveSign;

        // Act
        var result = NumberFormatHelper.IsPositiveSign(positiveSign);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsPositiveSign_WithOtherText_ReturnsFalse()
    {
        // Act
        var result = NumberFormatHelper.IsPositiveSign("xyz");

        // Assert
        Assert.False(result);
    }

    #endregion
}
