/*
 * Unit tests for GeneralPlotControl converters
 */

using System.Globalization;
using System.Windows.Media;
using Xunit;

namespace OxyPlotControls.Tests.Converters;

/// <summary>
/// Tests for OxyAutomaticColorConverter.
/// </summary>
public class OxyAutomaticColorConverterTests
{
    private readonly OxyAutomaticColorConverter _converter = new();

    #region Convert Tests

    [Fact]
    public void Convert_NullValue_ReturnsNull()
    {
        // Arrange
        object? value = null;

        // Act
        var result = _converter.Convert(value!, typeof(SolidColorBrush), null!, CultureInfo.InvariantCulture);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void Convert_WrongType_ReturnsNull()
    {
        // Arrange
        var value = "not a color";

        // Act
        var result = _converter.Convert(value, typeof(SolidColorBrush), null!, CultureInfo.InvariantCulture);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void Convert_NormalColor_ReturnsSolidColorBrush()
    {
        // Arrange
        var color = Color.FromArgb(255, 100, 150, 200);

        // Act
        var result = _converter.Convert(color, typeof(SolidColorBrush), null!, CultureInfo.InvariantCulture);

        // Assert
        Assert.IsType<SolidColorBrush>(result);
        var brush = (SolidColorBrush)result!;
        Assert.Equal(color, brush.Color);
    }

    [Fact]
    public void Convert_AutomaticColor_ReturnsBlackBrush()
    {
        // Arrange - OxyPlot uses ARGB(0,0,0,1) to represent automatic color
        var automaticColor = Color.FromArgb(0, 0, 0, 1);

        // Act
        var result = _converter.Convert(automaticColor, typeof(SolidColorBrush), null!, CultureInfo.InvariantCulture);

        // Assert
        Assert.IsType<SolidColorBrush>(result);
        var brush = (SolidColorBrush)result!;
        Assert.Equal(Color.FromArgb(255, 0, 0, 0), brush.Color);
    }

    [Fact]
    public void Convert_RedColor_ReturnsRedBrush()
    {
        // Arrange
        var color = Colors.Red;

        // Act
        var result = _converter.Convert(color, typeof(SolidColorBrush), null!, CultureInfo.InvariantCulture);

        // Assert
        Assert.IsType<SolidColorBrush>(result);
        var brush = (SolidColorBrush)result!;
        Assert.Equal(Colors.Red, brush.Color);
    }

    [Fact]
    public void Convert_TransparentColor_ReturnsSolidColorBrush()
    {
        // Arrange
        var color = Color.FromArgb(0, 255, 0, 0);

        // Act
        var result = _converter.Convert(color, typeof(SolidColorBrush), null!, CultureInfo.InvariantCulture);

        // Assert
        Assert.IsType<SolidColorBrush>(result);
        var brush = (SolidColorBrush)result!;
        Assert.Equal(color, brush.Color);
    }

    #endregion

    #region ConvertBack Tests

    [Fact]
    public void ConvertBack_SolidColorBrush_ReturnsColor()
    {
        // Arrange
        var brush = new SolidColorBrush(Colors.Blue);

        // Act
        var result = _converter.ConvertBack(brush, typeof(Color), null!, CultureInfo.InvariantCulture);

        // Assert
        Assert.IsType<Color>(result);
        Assert.Equal(Colors.Blue, (Color)result);
    }

    [Fact]
    public void ConvertBack_RedBrush_ReturnsRedColor()
    {
        // Arrange
        var brush = new SolidColorBrush(Colors.Red);

        // Act
        var result = _converter.ConvertBack(brush, typeof(Color), null!, CultureInfo.InvariantCulture);

        // Assert
        Assert.Equal(Colors.Red, (Color)result);
    }

    [Fact]
    public void ConvertBack_CustomColor_ReturnsCorrectColor()
    {
        // Arrange
        var color = Color.FromArgb(128, 50, 100, 150);
        var brush = new SolidColorBrush(color);

        // Act
        var result = _converter.ConvertBack(brush, typeof(Color), null!, CultureInfo.InvariantCulture);

        // Assert
        Assert.Equal(color, (Color)result);
    }

    #endregion
}

/// <summary>
/// Tests for OxyDefaultFontSizeConverter.
/// </summary>
public class OxyDefaultFontSizeConverterTests
{
    private readonly OxyDefaultFontSizeConverter _converter = new();

    #region Convert Tests

    [Fact]
    public void Convert_NullValue_ReturnsDefault12()
    {
        // Arrange
        object? value = null;

        // Act
        var result = _converter.Convert(value!, typeof(double), null!, CultureInfo.InvariantCulture);

        // Assert
        Assert.Equal(12.0, result);
    }

    [Fact]
    public void Convert_WrongType_ReturnsDefault12()
    {
        // Arrange
        var value = "not a double";

        // Act
        var result = _converter.Convert(value, typeof(double), null!, CultureInfo.InvariantCulture);

        // Assert
        Assert.Equal(12.0, result);
    }

    [Fact]
    public void Convert_NaNValue_ReturnsDefault12()
    {
        // Arrange
        var value = double.NaN;

        // Act
        var result = _converter.Convert(value, typeof(double), null!, CultureInfo.InvariantCulture);

        // Assert
        Assert.Equal(12.0, result);
    }

    [Fact]
    public void Convert_PositiveInfinity_ReturnsDefault12()
    {
        // Arrange
        var value = double.PositiveInfinity;

        // Act
        var result = _converter.Convert(value, typeof(double), null!, CultureInfo.InvariantCulture);

        // Assert
        Assert.Equal(12.0, result);
    }

    [Fact]
    public void Convert_NegativeInfinity_ReturnsDefault12()
    {
        // Arrange
        var value = double.NegativeInfinity;

        // Act
        var result = _converter.Convert(value, typeof(double), null!, CultureInfo.InvariantCulture);

        // Assert
        Assert.Equal(12.0, result);
    }

    [Fact]
    public void Convert_ValidFontSize_ReturnsValue()
    {
        // Arrange
        var value = 14.5;

        // Act
        var result = _converter.Convert(value, typeof(double), null!, CultureInfo.InvariantCulture);

        // Assert
        Assert.Equal(14.5, result);
    }

    [Fact]
    public void Convert_ZeroValue_ReturnsZero()
    {
        // Arrange
        var value = 0.0;

        // Act
        var result = _converter.Convert(value, typeof(double), null!, CultureInfo.InvariantCulture);

        // Assert
        Assert.Equal(0.0, result);
    }

    [Fact]
    public void Convert_LargeFontSize_ReturnsValue()
    {
        // Arrange
        var value = 72.0;

        // Act
        var result = _converter.Convert(value, typeof(double), null!, CultureInfo.InvariantCulture);

        // Assert
        Assert.Equal(72.0, result);
    }

    [Fact]
    public void Convert_SmallFontSize_ReturnsValue()
    {
        // Arrange
        var value = 8.0;

        // Act
        var result = _converter.Convert(value, typeof(double), null!, CultureInfo.InvariantCulture);

        // Assert
        Assert.Equal(8.0, result);
    }

    #endregion

    #region ConvertBack Tests

    [Fact]
    public void ConvertBack_NullValue_ReturnsNaN()
    {
        // Arrange
        object? value = null;

        // Act
        var result = _converter.ConvertBack(value!, typeof(double), null!, CultureInfo.InvariantCulture);

        // Assert
        Assert.True(double.IsNaN((double)result));
    }

    [Fact]
    public void ConvertBack_WrongType_ReturnsNaN()
    {
        // Arrange
        var value = "not a double";

        // Act
        var result = _converter.ConvertBack(value, typeof(double), null!, CultureInfo.InvariantCulture);

        // Assert
        Assert.True(double.IsNaN((double)result));
    }

    [Fact]
    public void ConvertBack_DefaultValue12_ReturnsNaN()
    {
        // Arrange
        var value = 12.0;

        // Act
        var result = _converter.ConvertBack(value, typeof(double), null!, CultureInfo.InvariantCulture);

        // Assert
        Assert.True(double.IsNaN((double)result));
    }

    [Fact]
    public void ConvertBack_NonDefaultValue_ReturnsValue()
    {
        // Arrange
        var value = 14.0;

        // Act
        var result = _converter.ConvertBack(value, typeof(double), null!, CultureInfo.InvariantCulture);

        // Assert
        Assert.Equal(14.0, (double)result);
    }

    [Fact]
    public void ConvertBack_ZeroValue_ReturnsZero()
    {
        // Arrange
        var value = 0.0;

        // Act
        var result = _converter.ConvertBack(value, typeof(double), null!, CultureInfo.InvariantCulture);

        // Assert
        Assert.Equal(0.0, (double)result);
    }

    [Fact]
    public void ConvertBack_LargeValue_ReturnsValue()
    {
        // Arrange
        var value = 72.0;

        // Act
        var result = _converter.ConvertBack(value, typeof(double), null!, CultureInfo.InvariantCulture);

        // Assert
        Assert.Equal(72.0, (double)result);
    }

    #endregion
}

/// <summary>
/// Tests for SolidColorBrushConverter.
/// </summary>
public class SolidColorBrushConverterTests
{
    private readonly SolidColorBrushConverter _converter = new();

    #region Convert Tests

    [Fact]
    public void Convert_NullValue_ReturnsNull()
    {
        // Arrange
        object? value = null;

        // Act
        var result = _converter.Convert(value!, typeof(SolidColorBrush), null!, CultureInfo.InvariantCulture);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void Convert_SolidColorBrush_ReturnsSameBrush()
    {
        // Arrange
        var brush = new SolidColorBrush(Colors.Red);

        // Act
        var result = _converter.Convert(brush, typeof(SolidColorBrush), null!, CultureInfo.InvariantCulture);

        // Assert
        Assert.Same(brush, result);
    }

    [Fact]
    public void Convert_BlueColorBrush_ReturnsCorrectBrush()
    {
        // Arrange
        var brush = new SolidColorBrush(Colors.Blue);

        // Act
        var result = _converter.Convert(brush, typeof(SolidColorBrush), null!, CultureInfo.InvariantCulture);

        // Assert
        Assert.IsType<SolidColorBrush>(result);
        Assert.Equal(Colors.Blue, ((SolidColorBrush)result!).Color);
    }

    [Fact]
    public void Convert_CustomColorBrush_ReturnsCorrectBrush()
    {
        // Arrange
        var color = Color.FromArgb(200, 100, 150, 200);
        var brush = new SolidColorBrush(color);

        // Act
        var result = _converter.Convert(brush, typeof(SolidColorBrush), null!, CultureInfo.InvariantCulture);

        // Assert
        Assert.IsType<SolidColorBrush>(result);
        Assert.Equal(color, ((SolidColorBrush)result!).Color);
    }

    #endregion

    #region ConvertBack Tests

    [Fact]
    public void ConvertBack_NullValue_ReturnsNull()
    {
        // Arrange
        object? value = null;

        // Act
        var result = _converter.ConvertBack(value!, typeof(Brush), null!, CultureInfo.InvariantCulture);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void ConvertBack_SolidColorBrush_ReturnsBrush()
    {
        // Arrange
        var brush = new SolidColorBrush(Colors.Green);

        // Act
        var result = _converter.ConvertBack(brush, typeof(Brush), null!, CultureInfo.InvariantCulture);

        // Assert
        Assert.IsType<SolidColorBrush>(result);
        Assert.Equal(Colors.Green, ((SolidColorBrush)result!).Color);
    }

    [Fact]
    public void ConvertBack_CustomColorBrush_ReturnsBrush()
    {
        // Arrange
        var color = Color.FromArgb(128, 50, 100, 150);
        var brush = new SolidColorBrush(color);

        // Act
        var result = _converter.ConvertBack(brush, typeof(Brush), null!, CultureInfo.InvariantCulture);

        // Assert
        Assert.IsType<SolidColorBrush>(result);
        Assert.Equal(color, ((SolidColorBrush)result!).Color);
    }

    #endregion

    #region Round Trip Tests

    [Fact]
    public void Convert_ConvertBack_RoundTrip()
    {
        // Arrange
        var originalBrush = new SolidColorBrush(Colors.Purple);

        // Act
        var converted = _converter.Convert(originalBrush, typeof(SolidColorBrush), null!, CultureInfo.InvariantCulture);
        var roundTripped = _converter.ConvertBack(converted!, typeof(Brush), null!, CultureInfo.InvariantCulture);

        // Assert
        Assert.IsType<SolidColorBrush>(roundTripped);
        Assert.Equal(originalBrush.Color, ((SolidColorBrush)roundTripped!).Color);
    }

    #endregion
}
