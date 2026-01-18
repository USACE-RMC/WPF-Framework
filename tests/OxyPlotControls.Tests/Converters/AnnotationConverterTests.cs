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
using OxyPlot;
using Xunit;

namespace OxyPlotControls.Tests.Converters;

/// <summary>
/// Tests for DataPointToPointConverter.
/// Verifies proper conversion between OxyPlot DataPoint and WPF Point objects.
/// </summary>
public class DataPointToPointConverterTests
{
    /// <summary>
    /// The converter instance being tested.
    /// </summary>
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

    /// <summary>
    /// Tests that Convert returns a WPF Point when given a valid DataPoint.
    /// </summary>
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

    /// <summary>
    /// Tests that Convert returns a zero Point when given a zero DataPoint.
    /// </summary>
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

    /// <summary>
    /// Tests that Convert correctly handles negative DataPoint values.
    /// </summary>
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

    /// <summary>
    /// Tests that Convert correctly handles large DataPoint values.
    /// </summary>
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

    /// <summary>
    /// Tests that ConvertBack returns a DataPoint when given a valid WPF Point.
    /// </summary>
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

    /// <summary>
    /// Tests that ConvertBack returns a zero DataPoint when given a zero WPF Point.
    /// </summary>
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

    /// <summary>
    /// Tests that ConvertBack correctly handles negative WPF Point values.
    /// </summary>
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
/// Verifies proper conversion between OxyPlot ScreenVector and WPF Point objects.
/// </summary>
public class ScreenVectorToPointConverterTests
{
    /// <summary>
    /// The converter instance being tested.
    /// </summary>
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
/// Verifies proper conversion between OxyPlot HorizontalAlignment and WPF HorizontalAlignment enums.
/// </summary>
public class OxyHorizontalAlignmentConverterTests
{
    /// <summary>
    /// The converter instance being tested.
    /// </summary>
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
/// Verifies proper conversion between OxyPlot VerticalAlignment and WPF VerticalAlignment enums.
/// </summary>
public class OxyVerticalAlignmentConverterTests
{
    /// <summary>
    /// The converter instance being tested.
    /// </summary>
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
