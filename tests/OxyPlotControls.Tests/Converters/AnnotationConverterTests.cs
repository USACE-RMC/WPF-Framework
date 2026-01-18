/*
 * Unit tests for AnnotationControl converters
 */

using System.Globalization;
using OxyPlot;
using Xunit;

namespace OxyPlotControls.Tests.Converters;

/// <summary>
/// Tests for DataPointToPointConverter.
/// </summary>
public class DataPointToPointConverterTests
{
    private readonly DataPointToPointConverter _converter = new();

    #region Convert Tests

    [Fact]
    public void Convert_NullValue_ReturnsNull()
    {
        // Arrange
        object? value = null;

        // Act
        var result = _converter.Convert(value, typeof(System.Windows.Point), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void Convert_WrongType_ReturnsNull()
    {
        // Arrange
        var value = "not a DataPoint";

        // Act
        var result = _converter.Convert(value, typeof(System.Windows.Point), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void Convert_ValidDataPoint_ReturnsPoint()
    {
        // Arrange
        var dataPoint = new DataPoint(10.5, 20.5);

        // Act
        var result = _converter.Convert(dataPoint, typeof(System.Windows.Point), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.IsType<System.Windows.Point>(result);
        var point = (System.Windows.Point)result!;
        Assert.Equal(10.5, point.X);
        Assert.Equal(20.5, point.Y);
    }

    [Fact]
    public void Convert_ZeroDataPoint_ReturnsZeroPoint()
    {
        // Arrange
        var dataPoint = new DataPoint(0, 0);

        // Act
        var result = _converter.Convert(dataPoint, typeof(System.Windows.Point), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.IsType<System.Windows.Point>(result);
        var point = (System.Windows.Point)result!;
        Assert.Equal(0, point.X);
        Assert.Equal(0, point.Y);
    }

    [Fact]
    public void Convert_NegativeDataPoint_ReturnsNegativePoint()
    {
        // Arrange
        var dataPoint = new DataPoint(-10.5, -20.5);

        // Act
        var result = _converter.Convert(dataPoint, typeof(System.Windows.Point), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.IsType<System.Windows.Point>(result);
        var point = (System.Windows.Point)result!;
        Assert.Equal(-10.5, point.X);
        Assert.Equal(-20.5, point.Y);
    }

    [Fact]
    public void Convert_LargeValuesDataPoint_ReturnsCorrectPoint()
    {
        // Arrange
        var dataPoint = new DataPoint(1000000.123, 2000000.456);

        // Act
        var result = _converter.Convert(dataPoint, typeof(System.Windows.Point), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.IsType<System.Windows.Point>(result);
        var point = (System.Windows.Point)result!;
        Assert.Equal(1000000.123, point.X);
        Assert.Equal(2000000.456, point.Y);
    }

    #endregion

    #region ConvertBack Tests

    [Fact]
    public void ConvertBack_NullValue_ReturnsNull()
    {
        // Arrange
        object? value = null;

        // Act
        var result = _converter.ConvertBack(value, typeof(DataPoint), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void ConvertBack_WrongType_ReturnsNull()
    {
        // Arrange
        var value = "not a Point";

        // Act
        var result = _converter.ConvertBack(value, typeof(DataPoint), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void ConvertBack_ValidPoint_ReturnsDataPoint()
    {
        // Arrange
        var point = new System.Windows.Point(10.5, 20.5);

        // Act
        var result = _converter.ConvertBack(point, typeof(DataPoint), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.IsType<DataPoint>(result);
        var dataPoint = (DataPoint)result!;
        Assert.Equal(10.5, dataPoint.X);
        Assert.Equal(20.5, dataPoint.Y);
    }

    [Fact]
    public void ConvertBack_ZeroPoint_ReturnsZeroDataPoint()
    {
        // Arrange
        var point = new System.Windows.Point(0, 0);

        // Act
        var result = _converter.ConvertBack(point, typeof(DataPoint), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.IsType<DataPoint>(result);
        var dataPoint = (DataPoint)result!;
        Assert.Equal(0, dataPoint.X);
        Assert.Equal(0, dataPoint.Y);
    }

    [Fact]
    public void ConvertBack_NegativePoint_ReturnsNegativeDataPoint()
    {
        // Arrange
        var point = new System.Windows.Point(-10.5, -20.5);

        // Act
        var result = _converter.ConvertBack(point, typeof(DataPoint), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.IsType<DataPoint>(result);
        var dataPoint = (DataPoint)result!;
        Assert.Equal(-10.5, dataPoint.X);
        Assert.Equal(-20.5, dataPoint.Y);
    }

    #endregion

    #region Round Trip Tests

    [Fact]
    public void Convert_ConvertBack_RoundTrip()
    {
        // Arrange
        var originalDataPoint = new DataPoint(123.456, 789.012);

        // Act
        var converted = _converter.Convert(originalDataPoint, typeof(System.Windows.Point), null, CultureInfo.InvariantCulture);
        var roundTripped = _converter.ConvertBack(converted, typeof(DataPoint), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.IsType<DataPoint>(roundTripped);
        var result = (DataPoint)roundTripped!;
        Assert.Equal(originalDataPoint.X, result.X);
        Assert.Equal(originalDataPoint.Y, result.Y);
    }

    #endregion
}

/// <summary>
/// Tests for ScreenVectorToPointConverter.
/// </summary>
public class ScreenVectorToPointConverterTests
{
    private readonly ScreenVectorToPointConverter _converter = new();

    #region Convert Tests

    [Fact]
    public void Convert_NullValue_ReturnsNull()
    {
        // Arrange
        object? value = null;

        // Act
        var result = _converter.Convert(value, typeof(System.Windows.Point), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void Convert_WrongType_ReturnsNull()
    {
        // Arrange
        var value = "not a ScreenVector";

        // Act
        var result = _converter.Convert(value, typeof(System.Windows.Point), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void Convert_ValidScreenVector_ReturnsPoint()
    {
        // Arrange
        var screenVector = new ScreenVector(10.5, 20.5);

        // Act
        var result = _converter.Convert(screenVector, typeof(System.Windows.Point), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.IsType<System.Windows.Point>(result);
        var point = (System.Windows.Point)result!;
        Assert.Equal(10.5, point.X);
        Assert.Equal(20.5, point.Y);
    }

    [Fact]
    public void Convert_ZeroScreenVector_ReturnsZeroPoint()
    {
        // Arrange
        var screenVector = new ScreenVector(0, 0);

        // Act
        var result = _converter.Convert(screenVector, typeof(System.Windows.Point), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.IsType<System.Windows.Point>(result);
        var point = (System.Windows.Point)result!;
        Assert.Equal(0, point.X);
        Assert.Equal(0, point.Y);
    }

    [Fact]
    public void Convert_NegativeScreenVector_ReturnsNegativePoint()
    {
        // Arrange
        var screenVector = new ScreenVector(-10.5, -20.5);

        // Act
        var result = _converter.Convert(screenVector, typeof(System.Windows.Point), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.IsType<System.Windows.Point>(result);
        var point = (System.Windows.Point)result!;
        Assert.Equal(-10.5, point.X);
        Assert.Equal(-20.5, point.Y);
    }

    #endregion

    #region ConvertBack Tests

    [Fact]
    public void ConvertBack_NullValue_ReturnsNull()
    {
        // Arrange
        object? value = null;

        // Act
        var result = _converter.ConvertBack(value, typeof(ScreenVector), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void ConvertBack_WrongType_ReturnsNull()
    {
        // Arrange
        var value = "not a Point";

        // Act
        var result = _converter.ConvertBack(value, typeof(ScreenVector), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void ConvertBack_ValidPoint_ReturnsScreenVector()
    {
        // Arrange
        var point = new System.Windows.Point(10.5, 20.5);

        // Act
        var result = _converter.ConvertBack(point, typeof(ScreenVector), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.IsType<ScreenVector>(result);
        var screenVector = (ScreenVector)result!;
        Assert.Equal(10.5, screenVector.X);
        Assert.Equal(20.5, screenVector.Y);
    }

    [Fact]
    public void ConvertBack_ZeroPoint_ReturnsZeroScreenVector()
    {
        // Arrange
        var point = new System.Windows.Point(0, 0);

        // Act
        var result = _converter.ConvertBack(point, typeof(ScreenVector), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.IsType<ScreenVector>(result);
        var screenVector = (ScreenVector)result!;
        Assert.Equal(0, screenVector.X);
        Assert.Equal(0, screenVector.Y);
    }

    [Fact]
    public void ConvertBack_NegativePoint_ReturnsNegativeScreenVector()
    {
        // Arrange
        var point = new System.Windows.Point(-10.5, -20.5);

        // Act
        var result = _converter.ConvertBack(point, typeof(ScreenVector), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.IsType<ScreenVector>(result);
        var screenVector = (ScreenVector)result!;
        Assert.Equal(-10.5, screenVector.X);
        Assert.Equal(-20.5, screenVector.Y);
    }

    #endregion

    #region Round Trip Tests

    [Fact]
    public void Convert_ConvertBack_RoundTrip()
    {
        // Arrange
        var originalVector = new ScreenVector(123.456, 789.012);

        // Act
        var converted = _converter.Convert(originalVector, typeof(System.Windows.Point), null, CultureInfo.InvariantCulture);
        var roundTripped = _converter.ConvertBack(converted, typeof(ScreenVector), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.IsType<ScreenVector>(roundTripped);
        var result = (ScreenVector)roundTripped!;
        Assert.Equal(originalVector.X, result.X);
        Assert.Equal(originalVector.Y, result.Y);
    }

    #endregion
}

/// <summary>
/// Tests for OxyHorizontalAlignmentConverter.
/// </summary>
public class OxyHorizontalAlignmentConverterTests
{
    private readonly OxyHorizontalAlignmentConverter _converter = new();

    #region Convert Tests

    [Fact]
    public void Convert_NullValue_ReturnsCenter()
    {
        // Arrange
        object? value = null;

        // Act
        var result = _converter.Convert(value, typeof(System.Windows.HorizontalAlignment), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.Equal(System.Windows.HorizontalAlignment.Center, result);
    }

    [Fact]
    public void Convert_WrongType_ReturnsCenter()
    {
        // Arrange
        var value = "not an alignment";

        // Act
        var result = _converter.Convert(value, typeof(System.Windows.HorizontalAlignment), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.Equal(System.Windows.HorizontalAlignment.Center, result);
    }

    [Fact]
    public void Convert_OxyLeft_ReturnsWpfLeft()
    {
        // Arrange
        var value = OxyPlot.HorizontalAlignment.Left;

        // Act
        var result = _converter.Convert(value, typeof(System.Windows.HorizontalAlignment), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.Equal(System.Windows.HorizontalAlignment.Left, result);
    }

    [Fact]
    public void Convert_OxyCenter_ReturnsWpfCenter()
    {
        // Arrange
        var value = OxyPlot.HorizontalAlignment.Center;

        // Act
        var result = _converter.Convert(value, typeof(System.Windows.HorizontalAlignment), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.Equal(System.Windows.HorizontalAlignment.Center, result);
    }

    [Fact]
    public void Convert_OxyRight_ReturnsWpfRight()
    {
        // Arrange
        var value = OxyPlot.HorizontalAlignment.Right;

        // Act
        var result = _converter.Convert(value, typeof(System.Windows.HorizontalAlignment), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.Equal(System.Windows.HorizontalAlignment.Right, result);
    }

    #endregion

    #region ConvertBack Tests

    [Fact]
    public void ConvertBack_NullValue_ReturnsCenter()
    {
        // Arrange
        object? value = null;

        // Act
        var result = _converter.ConvertBack(value, typeof(OxyPlot.HorizontalAlignment), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.Equal(OxyPlot.HorizontalAlignment.Center, result);
    }

    [Fact]
    public void ConvertBack_WrongType_ReturnsCenter()
    {
        // Arrange
        var value = "not an alignment";

        // Act
        var result = _converter.ConvertBack(value, typeof(OxyPlot.HorizontalAlignment), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.Equal(OxyPlot.HorizontalAlignment.Center, result);
    }

    [Fact]
    public void ConvertBack_WpfLeft_ReturnsOxyLeft()
    {
        // Arrange
        var value = System.Windows.HorizontalAlignment.Left;

        // Act
        var result = _converter.ConvertBack(value, typeof(OxyPlot.HorizontalAlignment), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.Equal(OxyPlot.HorizontalAlignment.Left, result);
    }

    [Fact]
    public void ConvertBack_WpfCenter_ReturnsOxyCenter()
    {
        // Arrange
        var value = System.Windows.HorizontalAlignment.Center;

        // Act
        var result = _converter.ConvertBack(value, typeof(OxyPlot.HorizontalAlignment), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.Equal(OxyPlot.HorizontalAlignment.Center, result);
    }

    [Fact]
    public void ConvertBack_WpfRight_ReturnsOxyRight()
    {
        // Arrange
        var value = System.Windows.HorizontalAlignment.Right;

        // Act
        var result = _converter.ConvertBack(value, typeof(OxyPlot.HorizontalAlignment), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.Equal(OxyPlot.HorizontalAlignment.Right, result);
    }

    [Fact]
    public void ConvertBack_WpfStretch_ReturnsOxyCenter()
    {
        // Arrange - Stretch is not supported in OxyPlot, should map to Center
        var value = System.Windows.HorizontalAlignment.Stretch;

        // Act
        var result = _converter.ConvertBack(value, typeof(OxyPlot.HorizontalAlignment), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.Equal(OxyPlot.HorizontalAlignment.Center, result);
    }

    #endregion

    #region Round Trip Tests

    [Fact]
    public void Convert_ConvertBack_RoundTrip_Left()
    {
        // Arrange
        var original = OxyPlot.HorizontalAlignment.Left;

        // Act
        var converted = _converter.Convert(original, typeof(System.Windows.HorizontalAlignment), null, CultureInfo.InvariantCulture);
        var roundTripped = _converter.ConvertBack(converted, typeof(OxyPlot.HorizontalAlignment), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.Equal(original, roundTripped);
    }

    [Fact]
    public void Convert_ConvertBack_RoundTrip_Center()
    {
        // Arrange
        var original = OxyPlot.HorizontalAlignment.Center;

        // Act
        var converted = _converter.Convert(original, typeof(System.Windows.HorizontalAlignment), null, CultureInfo.InvariantCulture);
        var roundTripped = _converter.ConvertBack(converted, typeof(OxyPlot.HorizontalAlignment), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.Equal(original, roundTripped);
    }

    [Fact]
    public void Convert_ConvertBack_RoundTrip_Right()
    {
        // Arrange
        var original = OxyPlot.HorizontalAlignment.Right;

        // Act
        var converted = _converter.Convert(original, typeof(System.Windows.HorizontalAlignment), null, CultureInfo.InvariantCulture);
        var roundTripped = _converter.ConvertBack(converted, typeof(OxyPlot.HorizontalAlignment), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.Equal(original, roundTripped);
    }

    #endregion
}

/// <summary>
/// Tests for OxyVerticalAlignmentConverter.
/// </summary>
public class OxyVerticalAlignmentConverterTests
{
    private readonly OxyVerticalAlignmentConverter _converter = new();

    #region Convert Tests

    [Fact]
    public void Convert_NullValue_ReturnsCenter()
    {
        // Arrange
        object? value = null;

        // Act
        var result = _converter.Convert(value, typeof(System.Windows.VerticalAlignment), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.Equal(System.Windows.VerticalAlignment.Center, result);
    }

    [Fact]
    public void Convert_WrongType_ReturnsCenter()
    {
        // Arrange
        var value = "not an alignment";

        // Act
        var result = _converter.Convert(value, typeof(System.Windows.VerticalAlignment), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.Equal(System.Windows.VerticalAlignment.Center, result);
    }

    [Fact]
    public void Convert_OxyTop_ReturnsWpfTop()
    {
        // Arrange
        var value = OxyPlot.VerticalAlignment.Top;

        // Act
        var result = _converter.Convert(value, typeof(System.Windows.VerticalAlignment), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.Equal(System.Windows.VerticalAlignment.Top, result);
    }

    [Fact]
    public void Convert_OxyMiddle_ReturnsWpfCenter()
    {
        // Arrange
        var value = OxyPlot.VerticalAlignment.Middle;

        // Act
        var result = _converter.Convert(value, typeof(System.Windows.VerticalAlignment), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.Equal(System.Windows.VerticalAlignment.Center, result);
    }

    [Fact]
    public void Convert_OxyBottom_ReturnsWpfBottom()
    {
        // Arrange
        var value = OxyPlot.VerticalAlignment.Bottom;

        // Act
        var result = _converter.Convert(value, typeof(System.Windows.VerticalAlignment), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.Equal(System.Windows.VerticalAlignment.Bottom, result);
    }

    #endregion

    #region ConvertBack Tests

    [Fact]
    public void ConvertBack_NullValue_ReturnsMiddle()
    {
        // Arrange
        object? value = null;

        // Act
        var result = _converter.ConvertBack(value, typeof(OxyPlot.VerticalAlignment), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.Equal(OxyPlot.VerticalAlignment.Middle, result);
    }

    [Fact]
    public void ConvertBack_WrongType_ReturnsMiddle()
    {
        // Arrange
        var value = "not an alignment";

        // Act
        var result = _converter.ConvertBack(value, typeof(OxyPlot.VerticalAlignment), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.Equal(OxyPlot.VerticalAlignment.Middle, result);
    }

    [Fact]
    public void ConvertBack_WpfTop_ReturnsOxyTop()
    {
        // Arrange
        var value = System.Windows.VerticalAlignment.Top;

        // Act
        var result = _converter.ConvertBack(value, typeof(OxyPlot.VerticalAlignment), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.Equal(OxyPlot.VerticalAlignment.Top, result);
    }

    [Fact]
    public void ConvertBack_WpfCenter_ReturnsOxyMiddle()
    {
        // Arrange
        var value = System.Windows.VerticalAlignment.Center;

        // Act
        var result = _converter.ConvertBack(value, typeof(OxyPlot.VerticalAlignment), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.Equal(OxyPlot.VerticalAlignment.Middle, result);
    }

    [Fact]
    public void ConvertBack_WpfBottom_ReturnsOxyBottom()
    {
        // Arrange
        var value = System.Windows.VerticalAlignment.Bottom;

        // Act
        var result = _converter.ConvertBack(value, typeof(OxyPlot.VerticalAlignment), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.Equal(OxyPlot.VerticalAlignment.Bottom, result);
    }

    [Fact]
    public void ConvertBack_WpfStretch_ReturnsOxyMiddle()
    {
        // Arrange - Stretch is not supported in OxyPlot, should map to Middle
        var value = System.Windows.VerticalAlignment.Stretch;

        // Act
        var result = _converter.ConvertBack(value, typeof(OxyPlot.VerticalAlignment), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.Equal(OxyPlot.VerticalAlignment.Middle, result);
    }

    #endregion

    #region Round Trip Tests

    [Fact]
    public void Convert_ConvertBack_RoundTrip_Top()
    {
        // Arrange
        var original = OxyPlot.VerticalAlignment.Top;

        // Act
        var converted = _converter.Convert(original, typeof(System.Windows.VerticalAlignment), null, CultureInfo.InvariantCulture);
        var roundTripped = _converter.ConvertBack(converted, typeof(OxyPlot.VerticalAlignment), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.Equal(original, roundTripped);
    }

    [Fact]
    public void Convert_ConvertBack_RoundTrip_Middle()
    {
        // Arrange
        var original = OxyPlot.VerticalAlignment.Middle;

        // Act
        var converted = _converter.Convert(original, typeof(System.Windows.VerticalAlignment), null, CultureInfo.InvariantCulture);
        var roundTripped = _converter.ConvertBack(converted, typeof(OxyPlot.VerticalAlignment), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.Equal(original, roundTripped);
    }

    [Fact]
    public void Convert_ConvertBack_RoundTrip_Bottom()
    {
        // Arrange
        var original = OxyPlot.VerticalAlignment.Bottom;

        // Act
        var converted = _converter.Convert(original, typeof(System.Windows.VerticalAlignment), null, CultureInfo.InvariantCulture);
        var roundTripped = _converter.ConvertBack(converted, typeof(OxyPlot.VerticalAlignment), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.Equal(original, roundTripped);
    }

    #endregion
}
