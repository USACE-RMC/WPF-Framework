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
/// Tests for EmptyStringToNullConverter.
/// Verifies proper conversion between empty strings and null values for WPF binding scenarios.
/// </summary>
public class EmptyStringToNullConverterTests
{
    /// <summary>
    /// The converter instance being tested.
    /// </summary>
    private readonly EmptyStringToNullConverter _converter = new();

    #region Convert Tests

    /// <summary>
    /// Tests that Convert returns null when given a null value.
    /// </summary>
    [Fact]
    public void Convert_NullValue_ReturnsNull()
    {
        // Arrange
        object? value = null;

        // Act
        var result = _converter.Convert(value, typeof(string), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.Null(result);
    }

    /// <summary>
    /// Tests that Convert returns the string value unchanged when given a non-empty string.
    /// </summary>
    [Fact]
    public void Convert_StringValue_ReturnsString()
    {
        // Arrange
        var value = "test string";

        // Act
        var result = _converter.Convert(value, typeof(string), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.Equal("test string", result);
    }

    /// <summary>
    /// Tests that Convert returns an empty string when given an empty string.
    /// </summary>
    [Fact]
    public void Convert_EmptyString_ReturnsEmptyString()
    {
        // Arrange
        var value = "";

        // Act
        var result = _converter.Convert(value, typeof(string), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.Equal("", result);
    }

    /// <summary>
    /// Tests that Convert returns the string representation when given an integer value.
    /// </summary>
    [Fact]
    public void Convert_IntValue_ReturnsStringRepresentation()
    {
        // Arrange
        var value = 42;

        // Act
        var result = _converter.Convert(value, typeof(string), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.Equal("42", result);
    }

    /// <summary>
    /// Tests that Convert returns the string representation when given a double value.
    /// </summary>
    [Fact]
    public void Convert_DoubleValue_ReturnsStringRepresentation()
    {
        // Arrange
        var value = 3.14;

        // Act
        var result = _converter.Convert(value, typeof(string), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.Equal("3.14", result);
    }

    #endregion

    #region ConvertBack Tests

    /// <summary>
    /// Tests that ConvertBack returns null when given a null value.
    /// </summary>
    [Fact]
    public void ConvertBack_NullValue_ReturnsNull()
    {
        // Arrange
        object? value = null;

        // Act
        var result = _converter.ConvertBack(value, typeof(string), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.Null(result);
    }

    /// <summary>
    /// Tests that ConvertBack returns null when given an empty string.
    /// </summary>
    [Fact]
    public void ConvertBack_EmptyString_ReturnsNull()
    {
        // Arrange
        var value = "";

        // Act
        var result = _converter.ConvertBack(value, typeof(string), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.Null(result);
    }

    /// <summary>
    /// Tests that ConvertBack returns the value unchanged when given a non-empty string.
    /// </summary>
    [Fact]
    public void ConvertBack_NonEmptyString_ReturnsValue()
    {
        // Arrange
        var value = "test string";

        // Act
        var result = _converter.ConvertBack(value, typeof(string), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.Equal("test string", result);
    }

    /// <summary>
    /// Tests that ConvertBack returns the value unchanged when given a whitespace string (whitespace is not considered empty).
    /// </summary>
    [Fact]
    public void ConvertBack_WhitespaceString_ReturnsValue()
    {
        // Arrange - whitespace is not empty
        var value = "   ";

        // Act
        var result = _converter.ConvertBack(value, typeof(string), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.Equal("   ", result);
    }

    /// <summary>
    /// Tests that ConvertBack returns the value unchanged when given a non-string value.
    /// </summary>
    [Fact]
    public void ConvertBack_IntValue_ReturnsValue()
    {
        // Arrange
        var value = 42;

        // Act
        var result = _converter.ConvertBack(value, typeof(int), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.Equal(42, result);
    }

    #endregion
}
