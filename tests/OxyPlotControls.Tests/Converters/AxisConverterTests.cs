/*
 * Unit tests for AxisControl converters
 */

using System.Globalization;
using System.Windows.Media;
using OxyPlot;
using Xunit;

namespace OxyPlotControls.Tests.Converters;

/// <summary>
/// Tests for ReverseAxisConverter (IMultiValueConverter).
/// </summary>
public class ReverseAxisConverterTests
{
    private readonly ReverseAxisConverter _converter = new();

    #region Convert Tests

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

    [Fact]
    public void Convert_ReversedOrientation_ReturnsTrue()
    {
        // Arrange - end position < start position is reversed
        object[] values = { 1.0, 0.0, null! };

        // Act
        var result = _converter.Convert(values, typeof(bool), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.True((bool)result);
    }

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

    [Fact]
    public void Convert_PartialReversal_ReturnsTrue()
    {
        // Arrange
        object[] values = { 0.8, 0.2, null! };

        // Act
        var result = _converter.Convert(values, typeof(bool), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.True((bool)result);
    }

    #endregion

    #region ConvertBack Tests

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
/// </summary>
public class OxyLineStyleToDashArrayConverterTests
{
    private readonly OxyLineStyleToDashArrayConverter _converter = new();

    #region Convert Tests

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
/// </summary>
public class EmptyStringToNullConverterTests
{
    private readonly EmptyStringToNullConverter _converter = new();

    #region Convert Tests

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
/// </summary>
public class DateToNumberConverterTests
{
    private readonly DateToNumberConverter _converter = new();

    #region Convert Tests

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
