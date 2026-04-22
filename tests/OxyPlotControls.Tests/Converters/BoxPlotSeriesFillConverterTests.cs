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
        object[] values = { null!, null! };

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
        object[] values = { "not a color", null! };

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
        object[] values = { color, null! };

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
        object[] values = { color, null! };

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
        object[] values = { color, null! };
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
