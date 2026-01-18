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
/// Tests for LineSeriesColorConverter (IMultiValueConverter).
/// Verifies proper conversion between Color values and WPF SolidColorBrush objects for line series.
/// </summary>
public class LineSeriesColorConverterTests
{
    /// <summary>
    /// The converter instance being tested.
    /// </summary>
    private readonly LineSeriesColorConverter _converter = new();

    #region Convert Tests

    /// <summary>
    /// Tests that Convert returns null when given a null color value.
    /// </summary>
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

    /// <summary>
    /// Tests that Convert returns null when given an incorrect type.
    /// </summary>
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

    /// <summary>
    /// Tests that Convert returns a SolidColorBrush with the correct color when given a valid color with no series object.
    /// </summary>
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

    /// <summary>
    /// Tests that Convert returns a red brush when given the Red color.
    /// </summary>
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

    /// <summary>
    /// Tests that Convert returns a blue brush when given the Blue color.
    /// </summary>
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

    /// <summary>
    /// Tests that Convert returns a brush with the correct transparency when given a transparent color.
    /// </summary>
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

    /// <summary>
    /// Tests that ConvertBack returns black color when given an incorrect brush type.
    /// </summary>
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

    /// <summary>
    /// Tests that ConvertBack returns a color array when given a valid brush.
    /// </summary>
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
/// Verifies proper conversion between Color values and WPF SolidColorBrush objects for area series secondary color.
/// </summary>
public class AreaSeriesColor2ConverterTests
{
    /// <summary>
    /// The converter instance being tested.
    /// </summary>
    private readonly AreaSeriesColor2Converter _converter = new();

    #region Convert Tests

    /// <summary>
    /// Tests that Convert returns null when given a null color value.
    /// </summary>
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

    /// <summary>
    /// Tests that Convert returns null when given an incorrect type.
    /// </summary>
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

    /// <summary>
    /// Tests that Convert returns a SolidColorBrush with the correct color when given a valid color with no series object.
    /// </summary>
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

    /// <summary>
    /// Tests that Convert returns a green brush when given the Green color.
    /// </summary>
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

    /// <summary>
    /// Tests that ConvertBack returns black color when given an incorrect brush type.
    /// </summary>
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
/// Verifies proper conversion between Color values and WPF SolidColorBrush objects for area series fill color.
/// </summary>
public class AreaSeriesFillConverterTests
{
    /// <summary>
    /// The converter instance being tested.
    /// </summary>
    private readonly AreaSeriesFillConverter _converter = new();

    #region Convert Tests

    /// <summary>
    /// Tests that Convert returns null when given a null color value.
    /// </summary>
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

    /// <summary>
    /// Tests that Convert returns null when given an incorrect type.
    /// </summary>
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

    /// <summary>
    /// Tests that Convert returns a SolidColorBrush with the correct color when given a valid color with no series object.
    /// </summary>
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

    /// <summary>
    /// Tests that Convert returns an orange brush when given the Orange color.
    /// </summary>
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

    /// <summary>
    /// Tests that ConvertBack returns black color when given an incorrect brush type.
    /// </summary>
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
/// Verifies proper conversion between Color values and WPF SolidColorBrush objects for line series marker fill.
/// </summary>
public class LineSeriesMarkerFillConverterTests
{
    /// <summary>
    /// The converter instance being tested.
    /// </summary>
    private readonly LineSeriesMarkerFillConverter _converter = new();

    #region Convert Tests

    /// <summary>
    /// Tests that Convert returns null when given a null color value.
    /// </summary>
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

    /// <summary>
    /// Tests that Convert returns null when given an incorrect type.
    /// </summary>
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

    /// <summary>
    /// Tests that Convert returns a SolidColorBrush with the correct color when given a valid color with no series object.
    /// </summary>
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

    /// <summary>
    /// Tests that Convert returns a yellow brush when given the Yellow color.
    /// </summary>
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

    /// <summary>
    /// Tests that ConvertBack returns black color when given an incorrect brush type.
    /// </summary>
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
/// Verifies proper conversion between Color values and WPF SolidColorBrush objects for line series marker stroke.
/// </summary>
public class LineSeriesMarkerStrokeConverterTests
{
    /// <summary>
    /// The converter instance being tested.
    /// </summary>
    private readonly LineSeriesMarkerStrokeConverter _converter = new();

    #region Convert Tests

    /// <summary>
    /// Tests that Convert returns null when given a null color value.
    /// </summary>
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

    /// <summary>
    /// Tests that Convert returns null when given an incorrect type.
    /// </summary>
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

    /// <summary>
    /// Tests that Convert returns a SolidColorBrush with the correct color when given a valid color with no series object.
    /// </summary>
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

    /// <summary>
    /// Tests that Convert returns a magenta brush when given the Magenta color.
    /// </summary>
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

    /// <summary>
    /// Tests that ConvertBack returns black color when given an incorrect brush type.
    /// </summary>
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
/// Verifies proper conversion between Color values and WPF SolidColorBrush objects for bar series fill.
/// </summary>
public class BarSeriesFillConverterTests
{
    /// <summary>
    /// The converter instance being tested.
    /// </summary>
    private readonly BarSeriesFillConverter _converter = new();

    #region Convert Tests

    /// <summary>
    /// Tests that Convert returns null when given a null color value.
    /// </summary>
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

    /// <summary>
    /// Tests that Convert returns null when given an incorrect type.
    /// </summary>
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

    /// <summary>
    /// Tests that Convert returns a SolidColorBrush with the correct color when given a valid color with no series object.
    /// </summary>
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

    /// <summary>
    /// Tests that Convert returns a cornflower blue brush when given the CornflowerBlue color.
    /// </summary>
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

    /// <summary>
    /// Tests that ConvertBack returns black color when given an incorrect brush type.
    /// </summary>
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
/// Verifies proper conversion between Color values and WPF SolidColorBrush objects for box plot series fill.
/// </summary>
public class BoxPlotSeriesFillConverterTests
{
    /// <summary>
    /// The converter instance being tested.
    /// </summary>
    private readonly BoxPlotSeriesFillConverter _converter = new();

    #region Convert Tests

    /// <summary>
    /// Tests that Convert returns null when given a null color value.
    /// </summary>
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

    /// <summary>
    /// Tests that Convert returns null when given an incorrect type.
    /// </summary>
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

    /// <summary>
    /// Tests that Convert returns a SolidColorBrush with the correct color when given a valid color with no series object.
    /// </summary>
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

    /// <summary>
    /// Tests that Convert returns a teal brush when given the Teal color.
    /// </summary>
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

    /// <summary>
    /// Tests that ConvertBack returns black color when given an incorrect brush type.
    /// </summary>
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
/// Verifies proper conversion between Color values and WPF SolidColorBrush objects for scatter series marker fill.
/// </summary>
public class ScatterSeriesMarkerFillConverterTests
{
    /// <summary>
    /// The converter instance being tested.
    /// </summary>
    private readonly ScatterSeriesMarkerFillConverter _converter = new();

    #region Convert Tests

    /// <summary>
    /// Tests that Convert returns null when given a null color value.
    /// </summary>
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

    /// <summary>
    /// Tests that Convert returns null when given an incorrect type.
    /// </summary>
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

    /// <summary>
    /// Tests that Convert returns a SolidColorBrush with the correct color when given a valid color with no series object.
    /// </summary>
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

    /// <summary>
    /// Tests that Convert returns a gold brush when given the Gold color.
    /// </summary>
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

    /// <summary>
    /// Tests that ConvertBack returns black color when given an incorrect brush type.
    /// </summary>
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
/// Verifies proper conversion between Color values and WPF SolidColorBrush objects for scatter series marker stroke.
/// </summary>
public class ScatterSeriesMarkerStrokeConverterTests
{
    /// <summary>
    /// The converter instance being tested.
    /// </summary>
    private readonly ScatterSeriesMarkerStrokeConverter _converter = new();

    #region Convert Tests

    /// <summary>
    /// Tests that Convert returns null when given a null color value.
    /// </summary>
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

    /// <summary>
    /// Tests that Convert returns null when given an incorrect type.
    /// </summary>
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

    /// <summary>
    /// Tests that Convert returns a SolidColorBrush with the correct color when given a valid color with no series object.
    /// </summary>
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

    /// <summary>
    /// Tests that Convert returns an Indian red brush when given the IndianRed color.
    /// </summary>
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

    /// <summary>
    /// Tests that ConvertBack returns black color when given an incorrect brush type.
    /// </summary>
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
/// Validates that all converters handle automatic colors and error conditions consistently.
/// </summary>
public class ColorConverterCommonBehaviorTests
{
    /// <summary>
    /// Tests that all converters handle OxyPlot's automatic color (ARGB 0,0,0,1) consistently.
    /// </summary>
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

    /// <summary>
    /// Tests that all converters return null when given a null color value.
    /// </summary>
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

    /// <summary>
    /// Tests that all converters return null when given an incorrect type.
    /// </summary>
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
