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

using OxyPlot;
using OxyPlot.Wpf.Serialization;
using Xunit;

namespace OxyPlotControls.Tests.Extensions;

/// <summary>
/// Tests for ScreenVector extension methods.
/// </summary>
public class ScreenVectorExtensionsTests
{
    [Fact]
    public void ToPrettyText_SimpleValues_ReturnsFormattedString()
    {
        // Arrange
        var vector = new ScreenVector(10.5, 20.5);

        // Act
        var result = vector.ToPrettyText();

        // Assert
        Assert.Equal("10.5, 20.5", result);
    }

    [Fact]
    public void ToPrettyText_ZeroValues_ReturnsFormattedString()
    {
        // Arrange
        var vector = new ScreenVector(0, 0);

        // Act
        var result = vector.ToPrettyText();

        // Assert
        Assert.Equal("0, 0", result);
    }

    [Fact]
    public void ToPrettyText_NegativeValues_ReturnsFormattedString()
    {
        // Arrange
        var vector = new ScreenVector(-10.5, -20.5);

        // Act
        var result = vector.ToPrettyText();

        // Assert
        Assert.Equal("-10.5, -20.5", result);
    }

    [Fact]
    public void FromPrettyVectorText_ValidString_ReturnsScreenVector()
    {
        // Arrange
        var text = "10.5, 20.5";

        // Act
        var result = text.FromPrettyVectorText();

        // Assert
        Assert.Equal(10.5, result.X);
        Assert.Equal(20.5, result.Y);
    }

    [Fact]
    public void FromPrettyVectorText_ZeroValues_ReturnsScreenVector()
    {
        // Arrange
        var text = "0, 0";

        // Act
        var result = text.FromPrettyVectorText();

        // Assert
        Assert.Equal(0, result.X);
        Assert.Equal(0, result.Y);
    }

    [Fact]
    public void FromPrettyVectorText_NegativeValues_ReturnsScreenVector()
    {
        // Arrange
        var text = "-10.5, -20.5";

        // Act
        var result = text.FromPrettyVectorText();

        // Assert
        Assert.Equal(-10.5, result.X);
        Assert.Equal(-20.5, result.Y);
    }

    [Fact]
    public void FromPrettyVectorText_InvalidFormat_ReturnsDefault()
    {
        // Arrange
        var text = "invalid";

        // Act
        var result = text.FromPrettyVectorText();

        // Assert
        Assert.Equal(0, result.X);
        Assert.Equal(0, result.Y);
    }

    [Fact]
    public void FromPrettyVectorText_InvalidXValue_ReturnsDefault()
    {
        // Arrange
        var text = "abc, 20.5";

        // Act
        var result = text.FromPrettyVectorText();

        // Assert
        Assert.Equal(0, result.X);
        Assert.Equal(0, result.Y);
    }

    [Fact]
    public void FromPrettyVectorText_InvalidYValue_ReturnsDefault()
    {
        // Arrange
        var text = "10.5, xyz";

        // Act
        var result = text.FromPrettyVectorText();

        // Assert
        Assert.Equal(0, result.X);
        Assert.Equal(0, result.Y);
    }

    [Fact]
    public void ToPrettyText_FromPrettyVectorText_RoundTrip()
    {
        // Arrange
        var original = new ScreenVector(123.456, 789.012);

        // Act
        var text = original.ToPrettyText();
        var result = text.FromPrettyVectorText();

        // Assert
        Assert.Equal(original.X, result.X, 10);
        Assert.Equal(original.Y, result.Y, 10);
    }
}
