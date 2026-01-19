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
