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
using Xunit;

namespace OxyPlotControls.Tests.Converters;

/// <summary>
/// Tests for SolidColorBrushConverter.
/// Verifies proper bidirectional conversion between SolidColorBrush and Brush types.
/// </summary>
public class SolidColorBrushConverterTests
{
    /// <summary>
    /// The converter instance being tested.
    /// </summary>
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
