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
using Xunit;

namespace OxyPlotControls.Tests.Converters;

/// <summary>
/// Tests for ReverseAxisConverter (IMultiValueConverter).
/// Verifies proper conversion between axis position values and reversed axis orientation.
/// </summary>
public class ReverseAxisConverterTests
{
    /// <summary>
    /// The converter instance being tested.
    /// </summary>
    private readonly ReverseAxisConverter _converter = new();

    #region Convert Tests

    /// <summary>
    /// Tests that Convert returns false when given normal axis orientation (start position less than end position).
    /// </summary>
    [Fact]
    public void Convert_NormalOrientation_ReturnsFalse()
    {
        // Arrange - start position 0, end position 1 is normal orientation
        object[] values = { 0.0, 1.0, null! };

        // Act
        var result = _converter.Convert(values, typeof(bool), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.False((bool)result);
    }

    /// <summary>
    /// Tests that Convert returns false when given reversed orientation positions but a null axis object.
    /// </summary>
    [Fact]
    public void Convert_ReversedOrientation_WithNullAxis_ReturnsFalse()
    {
        // Arrange - end position < start position would be reversed, but axis is null
        // The converter returns false early when axis is null
        object[] values = { 1.0, 0.0, null! };

        // Act
        var result = _converter.Convert(values, typeof(bool), null, CultureInfo.InvariantCulture);

        // Assert - Returns false because axis is null (converter short-circuits)
        Assert.False((bool)result);
    }

    /// <summary>
    /// Tests that Convert returns false when start and end positions are equal.
    /// </summary>
    [Fact]
    public void Convert_EqualPositions_ReturnsFalse()
    {
        // Arrange - equal positions means not reversed
        object[] values = { 0.5, 0.5, null! };

        // Act
        var result = _converter.Convert(values, typeof(bool), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.False((bool)result);
    }

    /// <summary>
    /// Tests that Convert handles null start position gracefully by parsing it as 0.
    /// </summary>
    [Fact]
    public void Convert_NullStartPosition_HandlesParsing()
    {
        // Arrange
        object[] values = { null!, 1.0, null! };

        // Act
        var result = _converter.Convert(values, typeof(bool), null, CultureInfo.InvariantCulture);

        // Assert - Both parse to 0, 0 < 1 is not reversed
        Assert.False((bool)result);
    }

    /// <summary>
    /// Tests that Convert correctly handles negative position values.
    /// </summary>
    [Fact]
    public void Convert_NegativePositions_ReturnsCorrectly()
    {
        // Arrange
        object[] values = { -1.0, 1.0, null! };

        // Act
        var result = _converter.Convert(values, typeof(bool), null, CultureInfo.InvariantCulture);

        // Assert - -1 < 1 is not reversed
        Assert.False((bool)result);
    }

    /// <summary>
    /// Tests that Convert returns false for partial reversal when axis is null.
    /// </summary>
    [Fact]
    public void Convert_PartialReversal_WithNullAxis_ReturnsFalse()
    {
        // Arrange - end position < start position would be reversed, but axis is null
        // The converter returns false early when axis is null
        object[] values = { 0.8, 0.2, null! };

        // Act
        var result = _converter.Convert(values, typeof(bool), null, CultureInfo.InvariantCulture);

        // Assert - Returns false because axis is null (converter short-circuits)
        Assert.False((bool)result);
    }

    #endregion

    #region ConvertBack Tests

    /// <summary>
    /// Tests that ConvertBack returns default axis position values when axis is null.
    /// </summary>
    [Fact]
    public void ConvertBack_NullAxis_ReturnsDefaults()
    {
        // Note: First must call Convert to set internal _axis
        object[] values = { 0.0, 1.0, null! };
        _converter.Convert(values, typeof(bool), null, CultureInfo.InvariantCulture);

        // Arrange
        object value = false;

        // Act
        var result = _converter.ConvertBack(value, new[] { typeof(double), typeof(double), typeof(object) }, null, CultureInfo.InvariantCulture);

        // Assert
        Assert.Equal(3, result.Length);
        Assert.Equal(0.0, result[0]);
        Assert.Equal(1.0, result[1]);
    }

    #endregion
}
