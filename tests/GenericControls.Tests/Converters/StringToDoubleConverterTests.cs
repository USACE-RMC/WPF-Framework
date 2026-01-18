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
/// Unit tests for StringToDoubleConverter.
/// </summary>
public class StringToDoubleConverterTests
{
    /// <summary>
    /// Converter instance used for testing.
    /// </summary>
    private readonly StringToDoubleConverter _converter = new();

    /// <summary>
    /// Tests convert withdouble returnsstring.
    /// </summary>
    [Fact]
    public void Convert_WithDouble_ReturnsString()
    {
        var result = _converter.Convert(123.45, typeof(string), null, CultureInfo.InvariantCulture);
        Assert.Equal("123.45", result);
    }

    /// <summary>
    /// Tests convert withinteger returnsstring.
    /// </summary>
    [Fact]
    public void Convert_WithInteger_ReturnsString()
    {
        var result = _converter.Convert(42, typeof(string), null, CultureInfo.InvariantCulture);
        Assert.Equal("42", result);
    }

    /// <summary>
    /// Tests convert withnull returnsnull.
    /// </summary>
    [Fact]
    public void Convert_WithNull_ReturnsNull()
    {
        var result = _converter.Convert(null, typeof(string), null, CultureInfo.InvariantCulture);
        Assert.Null(result);
    }

    /// <summary>
    /// Tests convert withzero returnszerostring.
    /// </summary>
    [Fact]
    public void Convert_WithZero_ReturnsZeroString()
    {
        var result = _converter.Convert(0.0, typeof(string), null, CultureInfo.InvariantCulture);
        Assert.Equal("0", result);
    }

    /// <summary>
    /// Tests convert withnegativenumber returnsnegativestring.
    /// </summary>
    [Fact]
    public void Convert_WithNegativeNumber_ReturnsNegativeString()
    {
        var result = _converter.Convert(-99.5, typeof(string), null, CultureInfo.InvariantCulture);
        Assert.Equal("-99.5", result);
    }

    /// <summary>
    /// Tests convertback withvalidstring returnsdouble.
    /// </summary>
    [Fact]
    public void ConvertBack_WithValidString_ReturnsDouble()
    {
        var result = _converter.ConvertBack("123.45", typeof(double), null, CultureInfo.InvariantCulture);
        Assert.Equal(123.45, result);
    }

    /// <summary>
    /// Tests convertback withnull returnsnull.
    /// </summary>
    [Fact]
    public void ConvertBack_WithNull_ReturnsNull()
    {
        var result = _converter.ConvertBack(null, typeof(double), null, CultureInfo.InvariantCulture);
        Assert.Null(result);
    }

    /// <summary>
    /// Tests convertback withinvalidstring returnszero.
    /// </summary>
    [Fact]
    public void ConvertBack_WithInvalidString_ReturnsZero()
    {
        var result = _converter.ConvertBack("not a number", typeof(double), null, CultureInfo.InvariantCulture);
        Assert.Equal(0.0, result);
    }

    /// <summary>
    /// Tests convertback withemptystring returnszero.
    /// </summary>
    [Fact]
    public void ConvertBack_WithEmptyString_ReturnsZero()
    {
        var result = _converter.ConvertBack("", typeof(double), null, CultureInfo.InvariantCulture);
        Assert.Equal(0.0, result);
    }

    /// <summary>
    /// Tests convertback withscientificnotation returnsdouble.
    /// </summary>
    [Fact]
    public void ConvertBack_WithScientificNotation_ReturnsDouble()
    {
        var result = _converter.ConvertBack("1.5e2", typeof(double), null, CultureInfo.InvariantCulture);
        Assert.Equal(150.0, result);
    }

    /// <summary>
    /// Tests convertback withnegativenumber returnsnegativedouble.
    /// </summary>
    [Fact]
    public void ConvertBack_WithNegativeNumber_ReturnsNegativeDouble()
    {
        var result = _converter.ConvertBack("-42.5", typeof(double), null, CultureInfo.InvariantCulture);
        Assert.Equal(-42.5, result);
    }
}
