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
/// Unit tests for DoubleToNAConverter.
/// </summary>
public class DoubleToNAConverterTests
{
    /// <summary>
    /// Converter instance used for testing.
    /// </summary>
    private readonly DoubleToNAConverter _converter = new();

    /// <summary>
    /// Tests convert withvaliddouble returnsdouble.
    /// </summary>
    [Fact]
    public void Convert_WithValidDouble_ReturnsDouble()
    {
        var result = _converter.Convert(123.45, typeof(string), null, CultureInfo.InvariantCulture);
        Assert.Equal(123.45, result);
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
    /// Tests convert withnan returnsna.
    /// </summary>
    [Fact]
    public void Convert_WithNaN_ReturnsNA()
    {
        var result = _converter.Convert(double.NaN, typeof(string), null, CultureInfo.InvariantCulture);
        Assert.Equal("N/A", result);
    }

    /// <summary>
    /// Tests convert withpositiveinfinity returnsplusinfinity.
    /// </summary>
    [Fact]
    public void Convert_WithPositiveInfinity_ReturnsPlusInfinity()
    {
        var result = _converter.Convert(double.PositiveInfinity, typeof(string), null, CultureInfo.InvariantCulture);
        Assert.Equal("+\u221E", result); // +infinity symbol
    }

    /// <summary>
    /// Tests convert withnegativeinfinity returnsminusinfinity.
    /// </summary>
    [Fact]
    public void Convert_WithNegativeInfinity_ReturnsMinusInfinity()
    {
        var result = _converter.Convert(double.NegativeInfinity, typeof(string), null, CultureInfo.InvariantCulture);
        Assert.Equal("-\u221E", result); // -infinity symbol
    }

    /// <summary>
    /// Tests convert withinvalidtype returnsna.
    /// </summary>
    [Fact]
    public void Convert_WithInvalidType_ReturnsNA()
    {
        var result = _converter.Convert("not a double", typeof(string), null, CultureInfo.InvariantCulture);
        Assert.Equal("N/A", result);
    }

    /// <summary>
    /// Tests convert withzero returnszero.
    /// </summary>
    [Fact]
    public void Convert_WithZero_ReturnsZero()
    {
        var result = _converter.Convert(0.0, typeof(string), null, CultureInfo.InvariantCulture);
        Assert.Equal(0.0, result);
    }

    /// <summary>
    /// Tests convert withnegativenumber returnsnumber.
    /// </summary>
    [Fact]
    public void Convert_WithNegativeNumber_ReturnsNumber()
    {
        var result = _converter.Convert(-50.5, typeof(string), null, CultureInfo.InvariantCulture);
        Assert.Equal(-50.5, result);
    }

    /// <summary>
    /// Tests convertback withnull returnsnan.
    /// </summary>
    [Fact]
    public void ConvertBack_WithNull_ReturnsNaN()
    {
        var result = _converter.ConvertBack(null, typeof(double), null, CultureInfo.InvariantCulture);
        Assert.True(double.IsNaN((double)result!));
    }

    /// <summary>
    /// Tests convertback withna returnsnan.
    /// </summary>
    [Fact]
    public void ConvertBack_WithNA_ReturnsNaN()
    {
        var result = _converter.ConvertBack("N/A", typeof(double), null, CultureInfo.InvariantCulture);
        Assert.True(double.IsNaN((double)result!));
    }

    /// <summary>
    /// Tests convertback withnanoslash returnsnan.
    /// </summary>
    [Fact]
    public void ConvertBack_WithNANoSlash_ReturnsNaN()
    {
        var result = _converter.ConvertBack("NA", typeof(double), null, CultureInfo.InvariantCulture);
        Assert.True(double.IsNaN((double)result!));
    }

    /// <summary>
    /// Tests convertback withinfinitysymbol returnspositiveinfinity.
    /// </summary>
    [Fact]
    public void ConvertBack_WithInfinitySymbol_ReturnsPositiveInfinity()
    {
        var result = _converter.ConvertBack("\u221E", typeof(double), null, CultureInfo.InvariantCulture);
        Assert.True(double.IsPositiveInfinity((double)result!));
    }

    /// <summary>
    /// Tests convertback withnegativeinfinitysymbol returnsnegativeinfinity.
    /// </summary>
    [Fact]
    public void ConvertBack_WithNegativeInfinitySymbol_ReturnsNegativeInfinity()
    {
        var result = _converter.ConvertBack("-\u221E", typeof(double), null, CultureInfo.InvariantCulture);
        Assert.True(double.IsNegativeInfinity((double)result!));
    }

    /// <summary>
    /// Tests convertback withvalidnumber returnsdouble.
    /// </summary>
    [Fact]
    public void ConvertBack_WithValidNumber_ReturnsDouble()
    {
        var result = _converter.ConvertBack("42.5", typeof(double), null, CultureInfo.InvariantCulture);
        Assert.Equal(42.5, result);
    }

    /// <summary>
    /// Tests convertback withinvalidstring returnsnan.
    /// </summary>
    [Fact]
    public void ConvertBack_WithInvalidString_ReturnsNaN()
    {
        var result = _converter.ConvertBack("invalid", typeof(double), null, CultureInfo.InvariantCulture);
        Assert.True(double.IsNaN((double)result!));
    }

    /// <summary>
    /// Tests convertback withnonstringtype returnsvalue.
    /// </summary>
    [Fact]
    public void ConvertBack_WithNonStringType_ReturnsValue()
    {
        var result = _converter.ConvertBack(123, typeof(double), null, CultureInfo.InvariantCulture);
        Assert.Equal(123, result);
    }

    /// <summary>
    /// Tests convertback withinftext returnspositiveinfinity.
    /// </summary>
    [Fact]
    public void ConvertBack_WithInfText_ReturnsPositiveInfinity()
    {
        var result = _converter.ConvertBack("inf", typeof(double), null, CultureInfo.InvariantCulture);
        Assert.True(double.IsPositiveInfinity((double)result!));
    }

    /// <summary>
    /// Tests convertback withnegativeinftext returnsnegativeinfinity.
    /// </summary>
    [Fact]
    public void ConvertBack_WithNegativeInfText_ReturnsNegativeInfinity()
    {
        var result = _converter.ConvertBack("-inf", typeof(double), null, CultureInfo.InvariantCulture);
        Assert.True(double.IsNegativeInfinity((double)result!));
    }
}
