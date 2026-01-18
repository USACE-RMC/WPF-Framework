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
using System.Windows.Media;
using OxyPlot;
using Xunit;

namespace OxyPlotControls.Tests.Converters;

/// <summary>
/// Tests for ReverseAxisConverter (IMultiValueConverter).
/// Verifies proper conversion between axis position values and reversed axis orientation.
/// </summary>
public class ReverseAxisConverterTests
{
    /// <summary>
    /// The converter instance being tested.
    /// </summary>
    private readonly ReverseAxisConverter _converter = new();

    #region Convert Tests

    /// <summary>
    /// Tests that Convert returns false when given normal axis orientation (start position less than end position).
    /// </summary>
    [Fact]
    public void Convert_NormalOrientation_ReturnsFalse()
    {
        // Arrange - start position 0, end position 1 is normal orientation
        object[] values = { 0.0, 1.0, null! };

        // Act
        var result = _converter.Convert(values, typeof(bool), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.False((bool)result);
    }

    /// <summary>
    /// Tests that Convert returns false when given reversed orientation positions but a null axis object.
    /// </summary>
    [Fact]
    public void Convert_ReversedOrientation_WithNullAxis_ReturnsFalse()
    {
        // Arrange - end position < start position would be reversed, but axis is null
        // The converter returns false early when axis is null
        object[] values = { 1.0, 0.0, null! };

        // Act
        var result = _converter.Convert(values, typeof(bool), null, CultureInfo.InvariantCulture);

        // Assert - Returns false because axis is null (converter short-circuits)
        Assert.False((bool)result);
    }

    /// <summary>
    /// Tests that Convert returns false when start and end positions are equal.
    /// </summary>
    [Fact]
    public void Convert_EqualPositions_ReturnsFalse()
    {
        // Arrange - equal positions means not reversed
        object[] values = { 0.5, 0.5, null! };

        // Act
        var result = _converter.Convert(values, typeof(bool), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.False((bool)result);
    }

    /// <summary>
    /// Tests that Convert handles null start position gracefully by parsing it as 0.
    /// </summary>
    [Fact]
    public void Convert_NullStartPosition_HandlesParsing()
    {
        // Arrange
        object[] values = { null!, 1.0, null! };

        // Act
        var result = _converter.Convert(values, typeof(bool), null, CultureInfo.InvariantCulture);

        // Assert - Both parse to 0, 0 < 1 is not reversed
        Assert.False((bool)result);
    }

    /// <summary>
    /// Tests that Convert correctly handles negative position values.
    /// </summary>
    [Fact]
    public void Convert_NegativePositions_ReturnsCorrectly()
    {
        // Arrange
        object[] values = { -1.0, 1.0, null! };

        // Act
        var result = _converter.Convert(values, typeof(bool), null, CultureInfo.InvariantCulture);

        // Assert - -1 < 1 is not reversed
        Assert.False((bool)result);
    }

    /// <summary>
    /// Tests that Convert returns false for partial reversal when axis is null.
    /// </summary>
    [Fact]
    public void Convert_PartialReversal_WithNullAxis_ReturnsFalse()
    {
        // Arrange - end position < start position would be reversed, but axis is null
        // The converter returns false early when axis is null
        object[] values = { 0.8, 0.2, null! };

        // Act
        var result = _converter.Convert(values, typeof(bool), null, CultureInfo.InvariantCulture);

        // Assert - Returns false because axis is null (converter short-circuits)
        Assert.False((bool)result);
    }

    #endregion

    #region ConvertBack Tests

    /// <summary>
    /// Tests that ConvertBack returns default axis position values when axis is null.
    /// </summary>
    [Fact]
    public void ConvertBack_NullAxis_ReturnsDefaults()
    {
        // Note: First must call Convert to set internal _axis
        object[] values = { 0.0, 1.0, null! };
        _converter.Convert(values, typeof(bool), null, CultureInfo.InvariantCulture);

        // Arrange
        object value = false;

        // Act
        var result = _converter.ConvertBack(value, new[] { typeof(double), typeof(double), typeof(object) }, null, CultureInfo.InvariantCulture);

        // Assert
        Assert.Equal(3, result.Length);
        Assert.Equal(0.0, result[0]);
        Assert.Equal(1.0, result[1]);
    }

    #endregion
}

/// <summary>
/// Tests for OxyLineStyleToDashArrayConverter.
/// Verifies proper conversion between OxyPlot LineStyle values and WPF DoubleCollection dash arrays.
/// </summary>
public class OxyLineStyleToDashArrayConverterTests
{
    /// <summary>
    /// The converter instance being tested.
    /// </summary>
    private readonly OxyLineStyleToDashArrayConverter _converter = new();

    #region Convert Tests

    /// <summary>
    /// Tests that Convert returns a default dash array when given a null value.
    /// </summary>
    [Fact]
    public void Convert_NullValue_ReturnsDefaultDashArray()
    {
        // Arrange
        object? value = null;

        // Act
        var result = _converter.Convert(value, typeof(DoubleCollection), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<DoubleCollection>(result);
    }

    /// <summary>
    /// Tests that Convert returns a default dash array when given an incorrect type.
    /// </summary>
    [Fact]
    public void Convert_WrongType_ReturnsDefaultDashArray()
    {
        // Arrange
        var value = "not a LineStyle";

        // Act
        var result = _converter.Convert(value, typeof(DoubleCollection), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<DoubleCollection>(result);
    }

    /// <summary>
    /// Tests that Convert returns an empty dash array for Solid line style.
    /// </summary>
    [Fact]
    public void Convert_SolidLineStyle_ReturnsEmptyDashArray()
    {
        // Arrange
        var value = OxyPlot.LineStyle.Solid;

        // Act
        var result = _converter.Convert(value, typeof(DoubleCollection), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.NotNull(result);
        var dashArray = result as DoubleCollection;
        Assert.NotNull(dashArray);
        Assert.Empty(dashArray);
    }

    /// <summary>
    /// Tests that Convert returns an empty dash array for Automatic line style.
    /// </summary>
    [Fact]
    public void Convert_AutomaticLineStyle_ReturnsEmptyDashArray()
    {
        // Arrange
        var value = OxyPlot.LineStyle.Automatic;

        // Act
        var result = _converter.Convert(value, typeof(DoubleCollection), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.NotNull(result);
        var dashArray = result as DoubleCollection;
        Assert.NotNull(dashArray);
        Assert.Empty(dashArray);
    }

    /// <summary>
    /// Tests that Convert returns an empty dash array for None line style.
    /// </summary>
    [Fact]
    public void Convert_NoneLineStyle_ReturnsEmptyDashArray()
    {
        // Arrange
        var value = OxyPlot.LineStyle.None;

        // Act
        var result = _converter.Convert(value, typeof(DoubleCollection), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.NotNull(result);
        var dashArray = result as DoubleCollection;
        Assert.NotNull(dashArray);
        // None also renders as solid (empty dash array) in this implementation
        Assert.Empty(dashArray);
    }

    /// <summary>
    /// Tests that Convert returns a non-empty dash array for Dash line style.
    /// </summary>
    [Fact]
    public void Convert_DashLineStyle_ReturnsNonEmptyDashArray()
    {
        // Arrange
        var value = OxyPlot.LineStyle.Dash;

        // Act
        var result = _converter.Convert(value, typeof(DoubleCollection), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.NotNull(result);
        var dashArray = result as DoubleCollection;
        Assert.NotNull(dashArray);
        Assert.NotEmpty(dashArray);
    }

    /// <summary>
    /// Tests that Convert returns a non-empty dash array for Dot line style.
    /// </summary>
    [Fact]
    public void Convert_DotLineStyle_ReturnsNonEmptyDashArray()
    {
        // Arrange
        var value = OxyPlot.LineStyle.Dot;

        // Act
        var result = _converter.Convert(value, typeof(DoubleCollection), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.NotNull(result);
        var dashArray = result as DoubleCollection;
        Assert.NotNull(dashArray);
        Assert.NotEmpty(dashArray);
    }

    /// <summary>
    /// Tests that Convert returns a non-empty dash array for DashDot line style.
    /// </summary>
    [Fact]
    public void Convert_DashDotLineStyle_ReturnsNonEmptyDashArray()
    {
        // Arrange
        var value = OxyPlot.LineStyle.DashDot;

        // Act
        var result = _converter.Convert(value, typeof(DoubleCollection), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.NotNull(result);
        var dashArray = result as DoubleCollection;
        Assert.NotNull(dashArray);
        Assert.NotEmpty(dashArray);
    }

    /// <summary>
    /// Tests that Convert returns a non-empty dash array for DashDashDot line style.
    /// </summary>
    [Fact]
    public void Convert_DashDashDotLineStyle_ReturnsNonEmptyDashArray()
    {
        // Arrange
        var value = OxyPlot.LineStyle.DashDashDot;

        // Act
        var result = _converter.Convert(value, typeof(DoubleCollection), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.NotNull(result);
        var dashArray = result as DoubleCollection;
        Assert.NotNull(dashArray);
        Assert.NotEmpty(dashArray);
    }

    /// <summary>
    /// Tests that Convert returns a non-empty dash array for DashDotDot line style.
    /// </summary>
    [Fact]
    public void Convert_DashDotDotLineStyle_ReturnsNonEmptyDashArray()
    {
        // Arrange
        var value = OxyPlot.LineStyle.DashDotDot;

        // Act
        var result = _converter.Convert(value, typeof(DoubleCollection), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.NotNull(result);
        var dashArray = result as DoubleCollection;
        Assert.NotNull(dashArray);
        Assert.NotEmpty(dashArray);
    }

    /// <summary>
    /// Tests that Convert returns a non-empty dash array for LongDash line style.
    /// </summary>
    [Fact]
    public void Convert_LongDashLineStyle_ReturnsNonEmptyDashArray()
    {
        // Arrange
        var value = OxyPlot.LineStyle.LongDash;

        // Act
        var result = _converter.Convert(value, typeof(DoubleCollection), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.NotNull(result);
        var dashArray = result as DoubleCollection;
        Assert.NotNull(dashArray);
        Assert.NotEmpty(dashArray);
    }

    /// <summary>
    /// Tests that Convert returns a non-empty dash array for LongDashDot line style.
    /// </summary>
    [Fact]
    public void Convert_LongDashDotLineStyle_ReturnsNonEmptyDashArray()
    {
        // Arrange
        var value = OxyPlot.LineStyle.LongDashDot;

        // Act
        var result = _converter.Convert(value, typeof(DoubleCollection), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.NotNull(result);
        var dashArray = result as DoubleCollection;
        Assert.NotNull(dashArray);
        Assert.NotEmpty(dashArray);
    }

    /// <summary>
    /// Tests that Convert returns a non-empty dash array for LongDashDotDot line style.
    /// </summary>
    [Fact]
    public void Convert_LongDashDotDotLineStyle_ReturnsNonEmptyDashArray()
    {
        // Arrange
        var value = OxyPlot.LineStyle.LongDashDotDot;

        // Act
        var result = _converter.Convert(value, typeof(DoubleCollection), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.NotNull(result);
        var dashArray = result as DoubleCollection;
        Assert.NotNull(dashArray);
        Assert.NotEmpty(dashArray);
    }

    #endregion

    #region ConvertBack Tests

    /// <summary>
    /// Tests that ConvertBack returns None line style when given a null value.
    /// </summary>
    [Fact]
    public void ConvertBack_NullValue_ReturnsNone()
    {
        // Arrange
        object? value = null;

        // Act
        var result = _converter.ConvertBack(value, typeof(OxyPlot.LineStyle), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.Equal(OxyPlot.LineStyle.None, result);
    }

    /// <summary>
    /// Tests that ConvertBack returns None line style when given an incorrect type.
    /// </summary>
    [Fact]
    public void ConvertBack_WrongType_ReturnsNone()
    {
        // Arrange
        var value = "not a DoubleCollection";

        // Act
        var result = _converter.ConvertBack(value, typeof(OxyPlot.LineStyle), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.Equal(OxyPlot.LineStyle.None, result);
    }

    /// <summary>
    /// Tests that ConvertBack returns Solid line style when given an empty dash array.
    /// </summary>
    [Fact]
    public void ConvertBack_EmptyDashArray_ReturnsSolid()
    {
        // Arrange
        var value = new DoubleCollection();

        // Act
        var result = _converter.ConvertBack(value, typeof(OxyPlot.LineStyle), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.Equal(OxyPlot.LineStyle.Solid, result);
    }

    /// <summary>
    /// Tests that ConvertBack returns Dash line style when given a dash pattern (4, 1).
    /// </summary>
    [Fact]
    public void ConvertBack_DashPattern_ReturnsDash()
    {
        // Arrange - Dash pattern is 4, 1
        var value = new DoubleCollection { 4, 1 };

        // Act
        var result = _converter.ConvertBack(value, typeof(OxyPlot.LineStyle), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.Equal(OxyPlot.LineStyle.Dash, result);
    }

    /// <summary>
    /// Tests that ConvertBack returns Dot line style when given a dot pattern (1, 1).
    /// </summary>
    [Fact]
    public void ConvertBack_DotPattern_ReturnsDot()
    {
        // Arrange - Dot pattern is 1, 1
        var value = new DoubleCollection { 1, 1 };

        // Act
        var result = _converter.ConvertBack(value, typeof(OxyPlot.LineStyle), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.Equal(OxyPlot.LineStyle.Dot, result);
    }

    /// <summary>
    /// Tests that ConvertBack returns DashDot line style when given a dash-dot pattern (4, 1, 1, 1).
    /// </summary>
    [Fact]
    public void ConvertBack_DashDotPattern_ReturnsDashDot()
    {
        // Arrange - DashDot pattern is 4, 1, 1, 1
        var value = new DoubleCollection { 4, 1, 1, 1 };

        // Act
        var result = _converter.ConvertBack(value, typeof(OxyPlot.LineStyle), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.Equal(OxyPlot.LineStyle.DashDot, result);
    }

    /// <summary>
    /// Tests that ConvertBack returns None line style when given an unrecognized dash pattern.
    /// </summary>
    [Fact]
    public void ConvertBack_UnrecognizedPattern_ReturnsNone()
    {
        // Arrange - Custom pattern that doesn't match any known style
        var value = new DoubleCollection { 7, 7, 7, 7, 7 };

        // Act
        var result = _converter.ConvertBack(value, typeof(OxyPlot.LineStyle), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.Equal(OxyPlot.LineStyle.None, result);
    }

    #endregion

    #region Round Trip Tests

    /// <summary>
    /// Tests that Convert and ConvertBack operations round-trip correctly for various line styles.
    /// </summary>
    /// <param name="lineStyle">The line style to test.</param>
    [Theory]
    [InlineData(LineStyle.Solid)]
    [InlineData(LineStyle.Dash)]
    [InlineData(LineStyle.Dot)]
    [InlineData(LineStyle.DashDot)]
    [InlineData(LineStyle.DashDotDot)]
    [InlineData(LineStyle.LongDash)]
    [InlineData(LineStyle.LongDashDot)]
    [InlineData(LineStyle.LongDashDotDot)]
    public void Convert_ConvertBack_RoundTrip(LineStyle lineStyle)
    {
        // Act
        var converted = _converter.Convert(lineStyle, typeof(DoubleCollection), null, CultureInfo.InvariantCulture);
        var roundTripped = _converter.ConvertBack(converted, typeof(LineStyle), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.Equal(lineStyle, roundTripped);
    }

    #endregion
}

/// <summary>
/// Tests for EmptyStringToNullConverter.
/// Verifies proper conversion between empty strings and null values for WPF binding scenarios.
/// </summary>
public class EmptyStringToNullConverterTests
{
    /// <summary>
    /// The converter instance being tested.
    /// </summary>
    private readonly EmptyStringToNullConverter _converter = new();

    #region Convert Tests

    /// <summary>
    /// Tests that Convert returns null when given a null value.
    /// </summary>
    [Fact]
    public void Convert_NullValue_ReturnsNull()
    {
        // Arrange
        object? value = null;

        // Act
        var result = _converter.Convert(value, typeof(string), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.Null(result);
    }

    /// <summary>
    /// Tests that Convert returns the string value unchanged when given a non-empty string.
    /// </summary>
    [Fact]
    public void Convert_StringValue_ReturnsString()
    {
        // Arrange
        var value = "test string";

        // Act
        var result = _converter.Convert(value, typeof(string), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.Equal("test string", result);
    }

    /// <summary>
    /// Tests that Convert returns an empty string when given an empty string.
    /// </summary>
    [Fact]
    public void Convert_EmptyString_ReturnsEmptyString()
    {
        // Arrange
        var value = "";

        // Act
        var result = _converter.Convert(value, typeof(string), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.Equal("", result);
    }

    /// <summary>
    /// Tests that Convert returns the string representation when given an integer value.
    /// </summary>
    [Fact]
    public void Convert_IntValue_ReturnsStringRepresentation()
    {
        // Arrange
        var value = 42;

        // Act
        var result = _converter.Convert(value, typeof(string), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.Equal("42", result);
    }

    /// <summary>
    /// Tests that Convert returns the string representation when given a double value.
    /// </summary>
    [Fact]
    public void Convert_DoubleValue_ReturnsStringRepresentation()
    {
        // Arrange
        var value = 3.14;

        // Act
        var result = _converter.Convert(value, typeof(string), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.Equal("3.14", result);
    }

    #endregion

    #region ConvertBack Tests

    /// <summary>
    /// Tests that ConvertBack returns null when given a null value.
    /// </summary>
    [Fact]
    public void ConvertBack_NullValue_ReturnsNull()
    {
        // Arrange
        object? value = null;

        // Act
        var result = _converter.ConvertBack(value, typeof(string), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.Null(result);
    }

    /// <summary>
    /// Tests that ConvertBack returns null when given an empty string.
    /// </summary>
    [Fact]
    public void ConvertBack_EmptyString_ReturnsNull()
    {
        // Arrange
        var value = "";

        // Act
        var result = _converter.ConvertBack(value, typeof(string), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.Null(result);
    }

    /// <summary>
    /// Tests that ConvertBack returns the value unchanged when given a non-empty string.
    /// </summary>
    [Fact]
    public void ConvertBack_NonEmptyString_ReturnsValue()
    {
        // Arrange
        var value = "test string";

        // Act
        var result = _converter.ConvertBack(value, typeof(string), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.Equal("test string", result);
    }

    /// <summary>
    /// Tests that ConvertBack returns the value unchanged when given a whitespace string (whitespace is not considered empty).
    /// </summary>
    [Fact]
    public void ConvertBack_WhitespaceString_ReturnsValue()
    {
        // Arrange - whitespace is not empty
        var value = "   ";

        // Act
        var result = _converter.ConvertBack(value, typeof(string), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.Equal("   ", result);
    }

    /// <summary>
    /// Tests that ConvertBack returns the value unchanged when given a non-string value.
    /// </summary>
    [Fact]
    public void ConvertBack_IntValue_ReturnsValue()
    {
        // Arrange
        var value = 42;

        // Act
        var result = _converter.ConvertBack(value, typeof(int), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.Equal(42, result);
    }

    #endregion
}

/// <summary>
/// Tests for DateToNumberConverter.
/// Verifies proper conversion between DateTime values and OxyPlot numeric date representations.
/// </summary>
public class DateToNumberConverterTests
{
    /// <summary>
    /// The converter instance being tested.
    /// </summary>
    private readonly DateToNumberConverter _converter = new();

    #region Convert Tests

    /// <summary>
    /// Tests that Convert returns null when given a null value.
    /// </summary>
    [Fact]
    public void Convert_NullValue_ReturnsNull()
    {
        // Arrange
        object? value = null;

        // Act
        var result = _converter.Convert(value, typeof(DateTime), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.Null(result);
    }

    /// <summary>
    /// Tests that Convert returns the value unchanged when given an incorrect type.
    /// </summary>
    [Fact]
    public void Convert_WrongType_ReturnsValue()
    {
        // Arrange
        var value = "not a double";

        // Act
        var result = _converter.Convert(value, typeof(DateTime), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.Equal("not a double", result);
    }

    /// <summary>
    /// Tests that Convert returns a DateTime when given a valid double value (OxyPlot uses days since year 1900).
    /// </summary>
    [Fact]
    public void Convert_ValidDouble_ReturnsDateTime()
    {
        // Arrange - OxyPlot uses days since year 1900
        var value = 44561.0; // Should be around Jan 1, 2022

        // Act
        var result = _converter.Convert(value, typeof(DateTime), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.IsType<DateTime>(result);
    }

    /// <summary>
    /// Tests that Convert returns an early date when given a zero value.
    /// </summary>
    [Fact]
    public void Convert_ZeroValue_ReturnsEarlyDate()
    {
        // Arrange
        var value = 0.0;

        // Act
        var result = _converter.Convert(value, typeof(DateTime), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.IsType<DateTime>(result);
    }

    /// <summary>
    /// Tests that Convert handles negative values gracefully by converting them to DateTime.
    /// </summary>
    [Fact]
    public void Convert_NegativeValue_HandlesGracefully()
    {
        // Arrange
        var value = -100.0;

        // Act
        var result = _converter.Convert(value, typeof(DateTime), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.IsType<DateTime>(result);
    }

    #endregion

    #region ConvertBack Tests

    /// <summary>
    /// Tests that ConvertBack returns null when given a null value.
    /// </summary>
    [Fact]
    public void ConvertBack_NullValue_ReturnsNull()
    {
        // Arrange
        object? value = null;

        // Act
        var result = _converter.ConvertBack(value, typeof(double), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.Null(result);
    }

    /// <summary>
    /// Tests that ConvertBack returns the value unchanged when given an incorrect type.
    /// </summary>
    [Fact]
    public void ConvertBack_WrongType_ReturnsValue()
    {
        // Arrange
        var value = "not a DateTime";

        // Act
        var result = _converter.ConvertBack(value, typeof(double), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.Equal("not a DateTime", result);
    }

    /// <summary>
    /// Tests that ConvertBack returns a double when given a valid DateTime.
    /// </summary>
    [Fact]
    public void ConvertBack_ValidDateTime_ReturnsDouble()
    {
        // Arrange
        var value = new DateTime(2022, 1, 1);

        // Act
        var result = _converter.ConvertBack(value, typeof(double), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.IsType<double>(result);
    }

    /// <summary>
    /// Tests that ConvertBack returns NaN when given DateTime.MinValue.
    /// </summary>
    [Fact]
    public void ConvertBack_MinValue_ReturnsNaN()
    {
        // Arrange
        var value = DateTime.MinValue;

        // Act
        var result = _converter.ConvertBack(value, typeof(double), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.True(double.IsNaN((double)result!));
    }

    /// <summary>
    /// Tests that ConvertBack returns a valid double when given DateTime.Now.
    /// </summary>
    [Fact]
    public void ConvertBack_NowDate_ReturnsDouble()
    {
        // Arrange
        var value = DateTime.Now;

        // Act
        var result = _converter.ConvertBack(value, typeof(double), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.IsType<double>(result);
        Assert.False(double.IsNaN((double)result!));
    }

    #endregion

    #region Round Trip Tests

    /// <summary>
    /// Tests that Convert and ConvertBack operations round-trip correctly for DateTime values.
    /// </summary>
    [Fact]
    public void Convert_ConvertBack_RoundTrip()
    {
        // Arrange
        var originalDate = new DateTime(2022, 6, 15, 12, 0, 0);
        var doubleValue = OxyPlot.Axes.DateTimeAxis.ToDouble(originalDate);

        // Act
        var converted = _converter.Convert(doubleValue, typeof(DateTime), null, CultureInfo.InvariantCulture);
        var roundTripped = _converter.ConvertBack(converted, typeof(double), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.Equal(doubleValue, (double)roundTripped!, 6);
    }

    /// <summary>
    /// Tests that ConvertBack and Convert operations round-trip correctly for DateTime values.
    /// </summary>
    [Fact]
    public void ConvertBack_Convert_RoundTrip()
    {
        // Arrange
        var originalDate = new DateTime(2023, 3, 20, 8, 30, 0);

        // Act
        var converted = _converter.ConvertBack(originalDate, typeof(double), null, CultureInfo.InvariantCulture);
        var roundTripped = _converter.Convert(converted, typeof(DateTime), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.IsType<DateTime>(roundTripped);
        var resultDate = (DateTime)roundTripped!;
        Assert.Equal(originalDate.Year, resultDate.Year);
        Assert.Equal(originalDate.Month, resultDate.Month);
        Assert.Equal(originalDate.Day, resultDate.Day);
    }

    #endregion
}
