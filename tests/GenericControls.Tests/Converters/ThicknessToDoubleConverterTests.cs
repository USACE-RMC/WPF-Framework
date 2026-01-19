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
using System.Windows;
using Xunit;

namespace GenericControls.Tests.Converters;

/// <summary>
/// Unit tests for ThicknessToDoubleConverter.
/// </summary>
public class ThicknessToDoubleConverterTests
{
    /// <summary>
    /// Converter instance used for testing.
    /// </summary>
    private readonly ThicknessToDoubleConverter _converter = new();

    /// <summary>
    /// Tests convert withuniformthickness returnsvalue.
    /// </summary>
    [Fact]
    public void Convert_WithUniformThickness_ReturnsValue()
    {
        var thickness = new Thickness(10);
        var result = _converter.Convert(thickness, typeof(double), null, CultureInfo.InvariantCulture);
        Assert.Equal(10.0, result);
    }

    /// <summary>
    /// Tests convert withnonuniformthickness returnsaverage.
    /// </summary>
    [Fact]
    public void Convert_WithNonUniformThickness_ReturnsAverage()
    {
        var thickness = new Thickness(10, 20, 30, 40);
        var result = _converter.Convert(thickness, typeof(double), null, CultureInfo.InvariantCulture);
        Assert.Equal(25.0, result); // (10 + 20 + 30 + 40) / 4
    }

    /// <summary>
    /// Tests convert withnull returnsnull.
    /// </summary>
    [Fact]
    public void Convert_WithNull_ReturnsNull()
    {
        var result = _converter.Convert(null, typeof(double), null, CultureInfo.InvariantCulture);
        Assert.Null(result);
    }

    /// <summary>
    /// Tests convert withzerothickness returnszero.
    /// </summary>
    [Fact]
    public void Convert_WithZeroThickness_ReturnsZero()
    {
        var thickness = new Thickness(0);
        var result = _converter.Convert(thickness, typeof(double), null, CultureInfo.InvariantCulture);
        Assert.Equal(0.0, result);
    }

    /// <summary>
    /// Tests convertback withdouble returnsuniformthickness.
    /// </summary>
    [Fact]
    public void ConvertBack_WithDouble_ReturnsUniformThickness()
    {
        var result = (Thickness)_converter.ConvertBack(15.0, typeof(Thickness), null, CultureInfo.InvariantCulture)!;
        Assert.Equal(new Thickness(15), result);
    }

    /// <summary>
    /// Tests convertback withnull returnsnull.
    /// </summary>
    [Fact]
    public void ConvertBack_WithNull_ReturnsNull()
    {
        var result = _converter.ConvertBack(null, typeof(Thickness), null, CultureInfo.InvariantCulture);
        Assert.Null(result);
    }

    /// <summary>
    /// Tests convertback withinvalidstring returnsnull.
    /// </summary>
    [Fact]
    public void ConvertBack_WithInvalidString_ReturnsNull()
    {
        var result = _converter.ConvertBack("invalid", typeof(Thickness), null, CultureInfo.InvariantCulture);
        Assert.Null(result);
    }

    /// <summary>
    /// Tests convertback withstringnumber returnsthickness.
    /// </summary>
    [Fact]
    public void ConvertBack_WithStringNumber_ReturnsThickness()
    {
        var result = (Thickness)_converter.ConvertBack("20", typeof(Thickness), null, CultureInfo.InvariantCulture)!;
        Assert.Equal(new Thickness(20), result);
    }
}
