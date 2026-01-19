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
