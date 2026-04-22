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

namespace GenericControls.Tests.Converters;

/// <summary>
/// Unit tests for StringToNAConverter.
/// </summary>
public class StringToNAConverterTests
{
    /// <summary>
    /// Converter instance used for testing.
    /// </summary>
    private readonly StringToNAConverter _converter = new();

    /// <summary>
    /// Tests convert withvalidnumericstring returnsstring.
    /// </summary>
    [Fact]
    public void Convert_WithValidNumericString_ReturnsString()
    {
        var result = _converter.Convert("123.45", typeof(string), null, CultureInfo.InvariantCulture);
        Assert.Equal("123.45", result);
    }

    /// <summary>
    /// Tests convert withnull returnsna.
    /// </summary>
    [Fact]
    public void Convert_WithNull_ReturnsNA()
    {
        var result = _converter.Convert(null, typeof(string), null, CultureInfo.InvariantCulture);
        Assert.Equal("N/A", result);
    }

    /// <summary>
    /// Tests convert withnonstring returnsna.
    /// </summary>
    [Fact]
    public void Convert_WithNonString_ReturnsNA()
    {
        var result = _converter.Convert(123.45, typeof(string), null, CultureInfo.InvariantCulture);
        Assert.Equal("N/A", result);
    }

    /// <summary>
    /// Tests convert withnanstring returnsna.
    /// </summary>
    [Fact]
    public void Convert_WithNaNString_ReturnsNA()
    {
        var result = _converter.Convert("NaN", typeof(string), null, CultureInfo.InvariantCulture);
        Assert.Equal("N/A", result);
    }

    /// <summary>
    /// Tests convert withinfinitystring returnsna.
    /// </summary>
    [Fact]
    public void Convert_WithInfinityString_ReturnsNA()
    {
        var result = _converter.Convert("Infinity", typeof(string), null, CultureInfo.InvariantCulture);
        Assert.Equal("N/A", result);
    }

    /// <summary>
    /// Tests convert withnegativeinfinitystring returnsna.
    /// </summary>
    [Fact]
    public void Convert_WithNegativeInfinityString_ReturnsNA()
    {
        var result = _converter.Convert("-Infinity", typeof(string), null, CultureInfo.InvariantCulture);
        Assert.Equal("N/A", result);
    }

    /// <summary>
    /// Tests convert withinvalidnumericstring returnsna.
    /// </summary>
    [Fact]
    public void Convert_WithInvalidNumericString_ReturnsNA()
    {
        var result = _converter.Convert("not a number", typeof(string), null, CultureInfo.InvariantCulture);
        Assert.Equal("N/A", result);
    }

    /// <summary>
    /// Tests convert withemptystring returnsna.
    /// </summary>
    [Fact]
    public void Convert_WithEmptyString_ReturnsNA()
    {
        var result = _converter.Convert("", typeof(string), null, CultureInfo.InvariantCulture);
        Assert.Equal("N/A", result);
    }

    /// <summary>
    /// Tests convert withzerostring returnszerostring.
    /// </summary>
    [Fact]
    public void Convert_WithZeroString_ReturnsZeroString()
    {
        var result = _converter.Convert("0", typeof(string), null, CultureInfo.InvariantCulture);
        Assert.Equal("0", result);
    }

    /// <summary>
    /// Tests convert withnegativenumberstring returnsstring.
    /// </summary>
    [Fact]
    public void Convert_WithNegativeNumberString_ReturnsString()
    {
        var result = _converter.Convert("-42.5", typeof(string), null, CultureInfo.InvariantCulture);
        Assert.Equal("-42.5", result);
    }

    /// <summary>
    /// Tests convertback throwsnotimplementedexception.
    /// </summary>
    [Fact]
    public void ConvertBack_ThrowsNotImplementedException()
    {
        Assert.Throws<NotImplementedException>(() =>
            _converter.ConvertBack("123", typeof(string), null, CultureInfo.InvariantCulture));
    }
}
