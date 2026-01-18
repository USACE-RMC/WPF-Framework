/*
 * Unit tests for Series control converters
 */

using System.Globalization;
using System.Windows.Media;
using OxyPlot;
using Xunit;

namespace OxyPlotControls.Tests.Converters;

/// <summary>
/// Tests for LineSeriesColorConverter (IMultiValueConverter).
/// </summary>
public class LineSeriesColorConverterTests
{
    private readonly LineSeriesColorConverter _converter = new();

    #region Convert Tests

    [Fact]
    public void Convert_NullColor_ReturnsNull()
    {
        // Arrange
        object[] values = { null!, null };

        // Act
        var result = _converter.Convert(values, typeof(SolidColorBrush), null!, CultureInfo.InvariantCulture);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void Convert_WrongColorType_ReturnsNull()
    {
        // Arrange
        object[] values = { "not a color", null };

        // Act
        var result = _converter.Convert(values, typeof(SolidColorBrush), null!, CultureInfo.InvariantCulture);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void Convert_ValidColorNoSeries_ReturnsBrush()
    {
        // Arrange
        var color = Color.FromArgb(255, 100, 150, 200);
        object[] values = { color, null };

        // Act
        var result = _converter.Convert(values, typeof(SolidColorBrush), null!, CultureInfo.InvariantCulture);

        // Assert
        Assert.IsType<SolidColorBrush>(result);
        var brush = (SolidColorBrush)result!;
        Assert.Equal(color, brush.Color);
    }

    [Fact]
    public void Convert_RedColor_ReturnsRedBrush()
    {
        // Arrange
        var color = Colors.Red;
        object[] values = { color, null };

        // Act
        var result = _converter.Convert(values, typeof(SolidColorBrush), null!, CultureInfo.InvariantCulture);

        // Assert
        Assert.IsType<SolidColorBrush>(result);
        var brush = (SolidColorBrush)result!;
        Assert.Equal(Colors.Red, brush.Color);
    }

    [Fact]
    public void Convert_BlueColor_ReturnsBlueBrush()
    {
        // Arrange
        var color = Colors.Blue;
        object[] values = { color, null };

        // Act
        var result = _converter.Convert(values, typeof(SolidColorBrush), null!, CultureInfo.InvariantCulture);

        // Assert
        Assert.IsType<SolidColorBrush>(result);
        var brush = (SolidColorBrush)result!;
        Assert.Equal(Colors.Blue, brush.Color);
    }

    [Fact]
    public void Convert_TransparentColor_ReturnsTransparentBrush()
    {
        // Arrange
        var color = Color.FromArgb(0, 255, 0, 0);
        object[] values = { color, null };

        // Act
        var result = _converter.Convert(values, typeof(SolidColorBrush), null!, CultureInfo.InvariantCulture);

        // Assert
        Assert.IsType<SolidColorBrush>(result);
        var brush = (SolidColorBrush)result!;
        Assert.Equal(color, brush.Color);
    }

    #endregion

    #region ConvertBack Tests

    [Fact]
    public void ConvertBack_WrongBrushType_ReturnsBlackColor()
    {
        // Arrange - need to first call Convert to set internal series
        var color = Colors.Green;
        object[] values = { color, null };
        _converter.Convert(values, typeof(SolidColorBrush), null!, CultureInfo.InvariantCulture);

        var value = "not a brush";

        // Act
        var result = _converter.ConvertBack(value, new[] { typeof(Color), typeof(object) }, null!, CultureInfo.InvariantCulture);

        // Assert
        Assert.Equal(2, result.Length);
        Assert.Equal(Color.FromArgb(255, 0, 0, 0), result[0]);
    }

    [Fact]
    public void ConvertBack_ValidBrush_ReturnsColorArray()
    {
        // Arrange - need to first call Convert to set internal series
        var color = Colors.Green;
        object[] values = { color, null };
        _converter.Convert(values, typeof(SolidColorBrush), null!, CultureInfo.InvariantCulture);

        var brush = new SolidColorBrush(Colors.Purple);

        // Act
        var result = _converter.ConvertBack(brush, new[] { typeof(Color), typeof(object) }, null!, CultureInfo.InvariantCulture);

        // Assert
        Assert.Equal(2, result.Length);
    }

    #endregion
}

/// <summary>
/// Tests for AreaSeriesColor2Converter (IMultiValueConverter).
/// </summary>
public class AreaSeriesColor2ConverterTests
{
    private readonly AreaSeriesColor2Converter _converter = new();

    #region Convert Tests

    [Fact]
    public void Convert_NullColor_ReturnsNull()
    {
        // Arrange
        object[] values = { null!, null };

        // Act
        var result = _converter.Convert(values, typeof(SolidColorBrush), null!, CultureInfo.InvariantCulture);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void Convert_WrongColorType_ReturnsNull()
    {
        // Arrange
        object[] values = { "not a color", null };

        // Act
        var result = _converter.Convert(values, typeof(SolidColorBrush), null!, CultureInfo.InvariantCulture);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void Convert_ValidColorNoSeries_ReturnsBrush()
    {
        // Arrange
        var color = Color.FromArgb(255, 100, 150, 200);
        object[] values = { color, null };

        // Act
        var result = _converter.Convert(values, typeof(SolidColorBrush), null!, CultureInfo.InvariantCulture);

        // Assert
        Assert.IsType<SolidColorBrush>(result);
        var brush = (SolidColorBrush)result!;
        Assert.Equal(color, brush.Color);
    }

    [Fact]
    public void Convert_GreenColor_ReturnsGreenBrush()
    {
        // Arrange
        var color = Colors.Green;
        object[] values = { color, null };

        // Act
        var result = _converter.Convert(values, typeof(SolidColorBrush), null!, CultureInfo.InvariantCulture);

        // Assert
        Assert.IsType<SolidColorBrush>(result);
        var brush = (SolidColorBrush)result!;
        Assert.Equal(Colors.Green, brush.Color);
    }

    #endregion

    #region ConvertBack Tests

    [Fact]
    public void ConvertBack_WrongBrushType_ReturnsBlackColor()
    {
        // Arrange
        var color = Colors.Green;
        object[] values = { color, null };
        _converter.Convert(values, typeof(SolidColorBrush), null!, CultureInfo.InvariantCulture);

        var value = "not a brush";

        // Act
        var result = _converter.ConvertBack(value, new[] { typeof(Color), typeof(object) }, null!, CultureInfo.InvariantCulture);

        // Assert
        Assert.Equal(2, result.Length);
        Assert.Equal(Color.FromArgb(255, 0, 0, 0), result[0]);
    }

    #endregion
}

/// <summary>
/// Tests for AreaSeriesFillConverter (IMultiValueConverter).
/// </summary>
public class AreaSeriesFillConverterTests
{
    private readonly AreaSeriesFillConverter _converter = new();

    #region Convert Tests

    [Fact]
    public void Convert_NullColor_ReturnsNull()
    {
        // Arrange
        object[] values = { null!, null };

        // Act
        var result = _converter.Convert(values, typeof(SolidColorBrush), null!, CultureInfo.InvariantCulture);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void Convert_WrongColorType_ReturnsNull()
    {
        // Arrange
        object[] values = { "not a color", null };

        // Act
        var result = _converter.Convert(values, typeof(SolidColorBrush), null!, CultureInfo.InvariantCulture);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void Convert_ValidColorNoSeries_ReturnsBrush()
    {
        // Arrange
        var color = Color.FromArgb(200, 100, 150, 200);
        object[] values = { color, null };

        // Act
        var result = _converter.Convert(values, typeof(SolidColorBrush), null!, CultureInfo.InvariantCulture);

        // Assert
        Assert.IsType<SolidColorBrush>(result);
        var brush = (SolidColorBrush)result!;
        Assert.Equal(color, brush.Color);
    }

    [Fact]
    public void Convert_OrangeColor_ReturnsOrangeBrush()
    {
        // Arrange
        var color = Colors.Orange;
        object[] values = { color, null };

        // Act
        var result = _converter.Convert(values, typeof(SolidColorBrush), null!, CultureInfo.InvariantCulture);

        // Assert
        Assert.IsType<SolidColorBrush>(result);
        var brush = (SolidColorBrush)result!;
        Assert.Equal(Colors.Orange, brush.Color);
    }

    #endregion

    #region ConvertBack Tests

    [Fact]
    public void ConvertBack_WrongBrushType_ReturnsBlackColor()
    {
        // Arrange
        var color = Colors.Yellow;
        object[] values = { color, null };
        _converter.Convert(values, typeof(SolidColorBrush), null!, CultureInfo.InvariantCulture);

        var value = "not a brush";

        // Act
        var result = _converter.ConvertBack(value, new[] { typeof(Color), typeof(object) }, null!, CultureInfo.InvariantCulture);

        // Assert
        Assert.Equal(2, result.Length);
        Assert.Equal(Color.FromArgb(255, 0, 0, 0), result[0]);
    }

    #endregion
}

/// <summary>
/// Tests for LineSeriesMarkerFillConverter (IMultiValueConverter).
/// </summary>
public class LineSeriesMarkerFillConverterTests
{
    private readonly LineSeriesMarkerFillConverter _converter = new();

    #region Convert Tests

    [Fact]
    public void Convert_NullColor_ReturnsNull()
    {
        // Arrange
        object[] values = { null!, null };

        // Act
        var result = _converter.Convert(values, typeof(SolidColorBrush), null!, CultureInfo.InvariantCulture);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void Convert_WrongColorType_ReturnsNull()
    {
        // Arrange
        object[] values = { "not a color", null };

        // Act
        var result = _converter.Convert(values, typeof(SolidColorBrush), null!, CultureInfo.InvariantCulture);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void Convert_ValidColorNoSeries_ReturnsBrush()
    {
        // Arrange
        var color = Color.FromArgb(255, 50, 100, 150);
        object[] values = { color, null };

        // Act
        var result = _converter.Convert(values, typeof(SolidColorBrush), null!, CultureInfo.InvariantCulture);

        // Assert
        Assert.IsType<SolidColorBrush>(result);
        var brush = (SolidColorBrush)result!;
        Assert.Equal(color, brush.Color);
    }

    [Fact]
    public void Convert_YellowColor_ReturnsYellowBrush()
    {
        // Arrange
        var color = Colors.Yellow;
        object[] values = { color, null };

        // Act
        var result = _converter.Convert(values, typeof(SolidColorBrush), null!, CultureInfo.InvariantCulture);

        // Assert
        Assert.IsType<SolidColorBrush>(result);
        var brush = (SolidColorBrush)result!;
        Assert.Equal(Colors.Yellow, brush.Color);
    }

    #endregion

    #region ConvertBack Tests

    [Fact]
    public void ConvertBack_WrongBrushType_ReturnsBlackColor()
    {
        // Arrange
        var color = Colors.Cyan;
        object[] values = { color, null };
        _converter.Convert(values, typeof(SolidColorBrush), null!, CultureInfo.InvariantCulture);

        var value = 123;

        // Act
        var result = _converter.ConvertBack(value, new[] { typeof(Color), typeof(object) }, null!, CultureInfo.InvariantCulture);

        // Assert
        Assert.Equal(2, result.Length);
        Assert.Equal(Color.FromArgb(255, 0, 0, 0), result[0]);
    }

    #endregion
}

/// <summary>
/// Tests for LineSeriesMarkerStrokeConverter (IMultiValueConverter).
/// </summary>
public class LineSeriesMarkerStrokeConverterTests
{
    private readonly LineSeriesMarkerStrokeConverter _converter = new();

    #region Convert Tests

    [Fact]
    public void Convert_NullColor_ReturnsNull()
    {
        // Arrange
        object[] values = { null!, null };

        // Act
        var result = _converter.Convert(values, typeof(SolidColorBrush), null!, CultureInfo.InvariantCulture);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void Convert_WrongColorType_ReturnsNull()
    {
        // Arrange
        object[] values = { "not a color", null };

        // Act
        var result = _converter.Convert(values, typeof(SolidColorBrush), null!, CultureInfo.InvariantCulture);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void Convert_ValidColorNoSeries_ReturnsBrush()
    {
        // Arrange
        var color = Color.FromArgb(255, 200, 100, 50);
        object[] values = { color, null };

        // Act
        var result = _converter.Convert(values, typeof(SolidColorBrush), null!, CultureInfo.InvariantCulture);

        // Assert
        Assert.IsType<SolidColorBrush>(result);
        var brush = (SolidColorBrush)result!;
        Assert.Equal(color, brush.Color);
    }

    [Fact]
    public void Convert_MagentaColor_ReturnsMagentaBrush()
    {
        // Arrange
        var color = Colors.Magenta;
        object[] values = { color, null };

        // Act
        var result = _converter.Convert(values, typeof(SolidColorBrush), null!, CultureInfo.InvariantCulture);

        // Assert
        Assert.IsType<SolidColorBrush>(result);
        var brush = (SolidColorBrush)result!;
        Assert.Equal(Colors.Magenta, brush.Color);
    }

    #endregion

    #region ConvertBack Tests

    [Fact]
    public void ConvertBack_WrongBrushType_ReturnsBlackColor()
    {
        // Arrange
        var color = Colors.Lime;
        object[] values = { color, null };
        _converter.Convert(values, typeof(SolidColorBrush), null!, CultureInfo.InvariantCulture);

        var value = 456.78;

        // Act
        var result = _converter.ConvertBack(value, new[] { typeof(Color), typeof(object) }, null!, CultureInfo.InvariantCulture);

        // Assert
        Assert.Equal(2, result.Length);
        Assert.Equal(Color.FromArgb(255, 0, 0, 0), result[0]);
    }

    #endregion
}

/// <summary>
/// Tests for BarSeriesFillConverter (IMultiValueConverter).
/// </summary>
public class BarSeriesFillConverterTests
{
    private readonly BarSeriesFillConverter _converter = new();

    #region Convert Tests

    [Fact]
    public void Convert_NullColor_ReturnsNull()
    {
        // Arrange
        object[] values = { null!, null };

        // Act
        var result = _converter.Convert(values, typeof(SolidColorBrush), null!, CultureInfo.InvariantCulture);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void Convert_WrongColorType_ReturnsNull()
    {
        // Arrange
        object[] values = { "not a color", null };

        // Act
        var result = _converter.Convert(values, typeof(SolidColorBrush), null!, CultureInfo.InvariantCulture);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void Convert_ValidColorNoSeries_ReturnsBrush()
    {
        // Arrange
        var color = Color.FromArgb(255, 75, 125, 175);
        object[] values = { color, null };

        // Act
        var result = _converter.Convert(values, typeof(SolidColorBrush), null!, CultureInfo.InvariantCulture);

        // Assert
        Assert.IsType<SolidColorBrush>(result);
        var brush = (SolidColorBrush)result!;
        Assert.Equal(color, brush.Color);
    }

    [Fact]
    public void Convert_CornflowerBlueColor_ReturnsCornflowerBlueBrush()
    {
        // Arrange
        var color = Colors.CornflowerBlue;
        object[] values = { color, null };

        // Act
        var result = _converter.Convert(values, typeof(SolidColorBrush), null!, CultureInfo.InvariantCulture);

        // Assert
        Assert.IsType<SolidColorBrush>(result);
        var brush = (SolidColorBrush)result!;
        Assert.Equal(Colors.CornflowerBlue, brush.Color);
    }

    #endregion

    #region ConvertBack Tests

    [Fact]
    public void ConvertBack_WrongBrushType_ReturnsBlackColor()
    {
        // Arrange
        var color = Colors.Coral;
        object[] values = { color, null };
        _converter.Convert(values, typeof(SolidColorBrush), null!, CultureInfo.InvariantCulture);

        var value = true;

        // Act
        var result = _converter.ConvertBack(value, new[] { typeof(Color), typeof(object) }, null!, CultureInfo.InvariantCulture);

        // Assert
        Assert.Equal(2, result.Length);
        Assert.Equal(Color.FromArgb(255, 0, 0, 0), result[0]);
    }

    #endregion
}

/// <summary>
/// Tests for BoxPlotSeriesFillConverter (IMultiValueConverter).
/// </summary>
public class BoxPlotSeriesFillConverterTests
{
    private readonly BoxPlotSeriesFillConverter _converter = new();

    #region Convert Tests

    [Fact]
    public void Convert_NullColor_ReturnsNull()
    {
        // Arrange
        object[] values = { null!, null };

        // Act
        var result = _converter.Convert(values, typeof(SolidColorBrush), null!, CultureInfo.InvariantCulture);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void Convert_WrongColorType_ReturnsNull()
    {
        // Arrange
        object[] values = { "not a color", null };

        // Act
        var result = _converter.Convert(values, typeof(SolidColorBrush), null!, CultureInfo.InvariantCulture);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void Convert_ValidColorNoSeries_ReturnsBrush()
    {
        // Arrange
        var color = Color.FromArgb(255, 30, 60, 90);
        object[] values = { color, null };

        // Act
        var result = _converter.Convert(values, typeof(SolidColorBrush), null!, CultureInfo.InvariantCulture);

        // Assert
        Assert.IsType<SolidColorBrush>(result);
        var brush = (SolidColorBrush)result!;
        Assert.Equal(color, brush.Color);
    }

    [Fact]
    public void Convert_TealColor_ReturnsTealBrush()
    {
        // Arrange
        var color = Colors.Teal;
        object[] values = { color, null };

        // Act
        var result = _converter.Convert(values, typeof(SolidColorBrush), null!, CultureInfo.InvariantCulture);

        // Assert
        Assert.IsType<SolidColorBrush>(result);
        var brush = (SolidColorBrush)result!;
        Assert.Equal(Colors.Teal, brush.Color);
    }

    #endregion

    #region ConvertBack Tests

    [Fact]
    public void ConvertBack_WrongBrushType_ReturnsBlackColor()
    {
        // Arrange
        var color = Colors.Salmon;
        object[] values = { color, null };
        _converter.Convert(values, typeof(SolidColorBrush), null!, CultureInfo.InvariantCulture);

        var value = DateTime.Now;

        // Act
        var result = _converter.ConvertBack(value, new[] { typeof(Color), typeof(object) }, null!, CultureInfo.InvariantCulture);

        // Assert
        Assert.Equal(2, result.Length);
        Assert.Equal(Color.FromArgb(255, 0, 0, 0), result[0]);
    }

    #endregion
}

/// <summary>
/// Tests for ScatterSeriesMarkerFillConverter (IMultiValueConverter).
/// </summary>
public class ScatterSeriesMarkerFillConverterTests
{
    private readonly ScatterSeriesMarkerFillConverter _converter = new();

    #region Convert Tests

    [Fact]
    public void Convert_NullColor_ReturnsNull()
    {
        // Arrange
        object[] values = { null!, null };

        // Act
        var result = _converter.Convert(values, typeof(SolidColorBrush), null!, CultureInfo.InvariantCulture);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void Convert_WrongColorType_ReturnsNull()
    {
        // Arrange
        object[] values = { "not a color", null };

        // Act
        var result = _converter.Convert(values, typeof(SolidColorBrush), null!, CultureInfo.InvariantCulture);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void Convert_ValidColorNoSeries_ReturnsBrush()
    {
        // Arrange
        var color = Color.FromArgb(255, 45, 90, 135);
        object[] values = { color, null };

        // Act
        var result = _converter.Convert(values, typeof(SolidColorBrush), null!, CultureInfo.InvariantCulture);

        // Assert
        Assert.IsType<SolidColorBrush>(result);
        var brush = (SolidColorBrush)result!;
        Assert.Equal(color, brush.Color);
    }

    [Fact]
    public void Convert_GoldColor_ReturnsGoldBrush()
    {
        // Arrange
        var color = Colors.Gold;
        object[] values = { color, null };

        // Act
        var result = _converter.Convert(values, typeof(SolidColorBrush), null!, CultureInfo.InvariantCulture);

        // Assert
        Assert.IsType<SolidColorBrush>(result);
        var brush = (SolidColorBrush)result!;
        Assert.Equal(Colors.Gold, brush.Color);
    }

    #endregion

    #region ConvertBack Tests

    [Fact]
    public void ConvertBack_WrongBrushType_ReturnsBlackColor()
    {
        // Arrange
        var color = Colors.Silver;
        object[] values = { color, null };
        _converter.Convert(values, typeof(SolidColorBrush), null!, CultureInfo.InvariantCulture);

        var value = new object();

        // Act
        var result = _converter.ConvertBack(value, new[] { typeof(Color), typeof(object) }, null!, CultureInfo.InvariantCulture);

        // Assert
        Assert.Equal(2, result.Length);
        Assert.Equal(Color.FromArgb(255, 0, 0, 0), result[0]);
    }

    #endregion
}

/// <summary>
/// Tests for ScatterSeriesMarkerStrokeConverter (IMultiValueConverter).
/// </summary>
public class ScatterSeriesMarkerStrokeConverterTests
{
    private readonly ScatterSeriesMarkerStrokeConverter _converter = new();

    #region Convert Tests

    [Fact]
    public void Convert_NullColor_ReturnsNull()
    {
        // Arrange
        object[] values = { null!, null };

        // Act
        var result = _converter.Convert(values, typeof(SolidColorBrush), null!, CultureInfo.InvariantCulture);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void Convert_WrongColorType_ReturnsNull()
    {
        // Arrange
        object[] values = { "not a color", null };

        // Act
        var result = _converter.Convert(values, typeof(SolidColorBrush), null!, CultureInfo.InvariantCulture);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void Convert_ValidColorNoSeries_ReturnsBrush()
    {
        // Arrange
        var color = Color.FromArgb(255, 60, 120, 180);
        object[] values = { color, null };

        // Act
        var result = _converter.Convert(values, typeof(SolidColorBrush), null!, CultureInfo.InvariantCulture);

        // Assert
        Assert.IsType<SolidColorBrush>(result);
        var brush = (SolidColorBrush)result!;
        Assert.Equal(color, brush.Color);
    }

    [Fact]
    public void Convert_IndianRedColor_ReturnsIndianRedBrush()
    {
        // Arrange
        var color = Colors.IndianRed;
        object[] values = { color, null };

        // Act
        var result = _converter.Convert(values, typeof(SolidColorBrush), null!, CultureInfo.InvariantCulture);

        // Assert
        Assert.IsType<SolidColorBrush>(result);
        var brush = (SolidColorBrush)result!;
        Assert.Equal(Colors.IndianRed, brush.Color);
    }

    #endregion

    #region ConvertBack Tests

    [Fact]
    public void ConvertBack_WrongBrushType_ReturnsBlackColor()
    {
        // Arrange
        var color = Colors.MediumPurple;
        object[] values = { color, null };
        _converter.Convert(values, typeof(SolidColorBrush), null!, CultureInfo.InvariantCulture);

        var value = new[] { 1, 2, 3 };

        // Act
        var result = _converter.ConvertBack(value, new[] { typeof(Color), typeof(object) }, null!, CultureInfo.InvariantCulture);

        // Assert
        Assert.Equal(2, result.Length);
        Assert.Equal(Color.FromArgb(255, 0, 0, 0), result[0]);
    }

    #endregion
}

/// <summary>
/// Common tests for all color converters to ensure consistent behavior.
/// </summary>
public class ColorConverterCommonBehaviorTests
{
    [Fact]
    public void AllConverters_HandleAutomaticColor_Consistently()
    {
        // Arrange - OxyPlot uses ARGB(0,0,0,1) to represent automatic color
        var automaticColor = Color.FromArgb(0, 0, 0, 1);
        object[] values = { automaticColor, null };

        // Test LineSeriesColorConverter
        var lineConverter = new LineSeriesColorConverter();
        var lineResult = lineConverter.Convert(values, typeof(SolidColorBrush), null!, CultureInfo.InvariantCulture);
        Assert.IsType<SolidColorBrush>(lineResult);

        // Test AreaSeriesColor2Converter
        var areaColor2Converter = new AreaSeriesColor2Converter();
        var areaColor2Result = areaColor2Converter.Convert(values, typeof(SolidColorBrush), null!, CultureInfo.InvariantCulture);
        Assert.IsType<SolidColorBrush>(areaColor2Result);

        // Test AreaSeriesFillConverter
        var areaFillConverter = new AreaSeriesFillConverter();
        var areaFillResult = areaFillConverter.Convert(values, typeof(SolidColorBrush), null!, CultureInfo.InvariantCulture);
        Assert.IsType<SolidColorBrush>(areaFillResult);

        // Test LineSeriesMarkerFillConverter
        var markerFillConverter = new LineSeriesMarkerFillConverter();
        var markerFillResult = markerFillConverter.Convert(values, typeof(SolidColorBrush), null!, CultureInfo.InvariantCulture);
        Assert.IsType<SolidColorBrush>(markerFillResult);

        // Test BarSeriesFillConverter
        var barConverter = new BarSeriesFillConverter();
        var barResult = barConverter.Convert(values, typeof(SolidColorBrush), null!, CultureInfo.InvariantCulture);
        Assert.IsType<SolidColorBrush>(barResult);

        // Test BoxPlotSeriesFillConverter
        var boxConverter = new BoxPlotSeriesFillConverter();
        var boxResult = boxConverter.Convert(values, typeof(SolidColorBrush), null!, CultureInfo.InvariantCulture);
        Assert.IsType<SolidColorBrush>(boxResult);

        // Test ScatterSeriesMarkerFillConverter
        var scatterFillConverter = new ScatterSeriesMarkerFillConverter();
        var scatterFillResult = scatterFillConverter.Convert(values, typeof(SolidColorBrush), null!, CultureInfo.InvariantCulture);
        Assert.IsType<SolidColorBrush>(scatterFillResult);
    }

    [Fact]
    public void AllConverters_ReturnNull_ForNullColor()
    {
        // Arrange
        object[] values = { null!, null };

        // Test all converters
        Assert.Null(new LineSeriesColorConverter().Convert(values, typeof(SolidColorBrush), null!, CultureInfo.InvariantCulture));
        Assert.Null(new AreaSeriesColor2Converter().Convert(values, typeof(SolidColorBrush), null!, CultureInfo.InvariantCulture));
        Assert.Null(new AreaSeriesFillConverter().Convert(values, typeof(SolidColorBrush), null!, CultureInfo.InvariantCulture));
        Assert.Null(new LineSeriesMarkerFillConverter().Convert(values, typeof(SolidColorBrush), null!, CultureInfo.InvariantCulture));
        Assert.Null(new LineSeriesMarkerStrokeConverter().Convert(values, typeof(SolidColorBrush), null!, CultureInfo.InvariantCulture));
        Assert.Null(new BarSeriesFillConverter().Convert(values, typeof(SolidColorBrush), null!, CultureInfo.InvariantCulture));
        Assert.Null(new BoxPlotSeriesFillConverter().Convert(values, typeof(SolidColorBrush), null!, CultureInfo.InvariantCulture));
        Assert.Null(new ScatterSeriesMarkerFillConverter().Convert(values, typeof(SolidColorBrush), null!, CultureInfo.InvariantCulture));
        Assert.Null(new ScatterSeriesMarkerStrokeConverter().Convert(values, typeof(SolidColorBrush), null!, CultureInfo.InvariantCulture));
    }

    [Fact]
    public void AllConverters_ReturnNull_ForWrongType()
    {
        // Arrange
        object[] values = { "not a color", null };

        // Test all converters
        Assert.Null(new LineSeriesColorConverter().Convert(values, typeof(SolidColorBrush), null!, CultureInfo.InvariantCulture));
        Assert.Null(new AreaSeriesColor2Converter().Convert(values, typeof(SolidColorBrush), null!, CultureInfo.InvariantCulture));
        Assert.Null(new AreaSeriesFillConverter().Convert(values, typeof(SolidColorBrush), null!, CultureInfo.InvariantCulture));
        Assert.Null(new LineSeriesMarkerFillConverter().Convert(values, typeof(SolidColorBrush), null!, CultureInfo.InvariantCulture));
        Assert.Null(new LineSeriesMarkerStrokeConverter().Convert(values, typeof(SolidColorBrush), null!, CultureInfo.InvariantCulture));
        Assert.Null(new BarSeriesFillConverter().Convert(values, typeof(SolidColorBrush), null!, CultureInfo.InvariantCulture));
        Assert.Null(new BoxPlotSeriesFillConverter().Convert(values, typeof(SolidColorBrush), null!, CultureInfo.InvariantCulture));
        Assert.Null(new ScatterSeriesMarkerFillConverter().Convert(values, typeof(SolidColorBrush), null!, CultureInfo.InvariantCulture));
        Assert.Null(new ScatterSeriesMarkerStrokeConverter().Convert(values, typeof(SolidColorBrush), null!, CultureInfo.InvariantCulture));
    }
}
