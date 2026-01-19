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
/// Unit tests for DoubleToCornerRadiusConverter.
/// </summary>
public class DoubleToCornerRadiusConverterTests
{
    /// <summary>
    /// Converter instance used for testing.
    /// </summary>
    private readonly DoubleToCornerRadiusConverter _converter = new();

    /// <summary>
    /// Tests convert doubletocornerradius returnsuniformcornerradius.
    /// </summary>
    [Fact]
    public void Convert_DoubleToCornerRadius_ReturnsUniformCornerRadius()
    {
        var result = _converter.Convert(10.0, typeof(CornerRadius), null, CultureInfo.InvariantCulture);
        Assert.IsType<CornerRadius>(result);
        var cr = (CornerRadius)result;
        Assert.Equal(10.0, cr.TopLeft);
        Assert.Equal(10.0, cr.TopRight);
        Assert.Equal(10.0, cr.BottomRight);
        Assert.Equal(10.0, cr.BottomLeft);
    }

    /// <summary>
    /// Tests convert cornerradiustodouble returnsbottomleft.
    /// </summary>
    [Fact]
    public void Convert_CornerRadiusToDouble_ReturnsBottomLeft()
    {
        var cornerRadius = new CornerRadius(5, 10, 15, 20);
        var result = _converter.Convert(cornerRadius, typeof(double), null, CultureInfo.InvariantCulture);
        Assert.Equal(20.0, result); // BottomLeft
    }

    /// <summary>
    /// Tests convert nulltocornerradius returnsemptycornerradius.
    /// </summary>
    [Fact]
    public void Convert_NullToCornerRadius_ReturnsEmptyCornerRadius()
    {
        var result = _converter.Convert(null, typeof(CornerRadius), null, CultureInfo.InvariantCulture);
        Assert.Equal(new CornerRadius(), result);
    }

    /// <summary>
    /// Tests convert invalidtypetocornerradius returnsemptycornerradius.
    /// </summary>
    [Fact]
    public void Convert_InvalidTypeToCornerRadius_ReturnsEmptyCornerRadius()
    {
        var result = _converter.Convert("invalid", typeof(CornerRadius), null, CultureInfo.InvariantCulture);
        Assert.Equal(new CornerRadius(), result);
    }

    /// <summary>
    /// Tests convert invalidtypetodouble returnsnan.
    /// </summary>
    [Fact]
    public void Convert_InvalidTypeToDouble_ReturnsNaN()
    {
        var result = _converter.Convert("invalid", typeof(double), null, CultureInfo.InvariantCulture);
        Assert.True(double.IsNaN((double)result!));
    }

    /// <summary>
    /// Tests convert withothertargettype returnsnull.
    /// </summary>
    [Fact]
    public void Convert_WithOtherTargetType_ReturnsNull()
    {
        var result = _converter.Convert(10.0, typeof(string), null, CultureInfo.InvariantCulture);
        Assert.Null(result);
    }

    /// <summary>
    /// Tests convertback doubletocornerradius returnscornerradius.
    /// </summary>
    [Fact]
    public void ConvertBack_DoubleToCornerRadius_ReturnsCornerRadius()
    {
        var result = _converter.ConvertBack(15.0, typeof(CornerRadius), null, CultureInfo.InvariantCulture);
        Assert.IsType<CornerRadius>(result);
        var cr = (CornerRadius)result;
        Assert.Equal(15.0, cr.TopLeft);
    }

    /// <summary>
    /// Tests convertback cornerradiustodouble returnsdouble.
    /// </summary>
    [Fact]
    public void ConvertBack_CornerRadiusToDouble_ReturnsDouble()
    {
        var cornerRadius = new CornerRadius(25);
        var result = _converter.ConvertBack(cornerRadius, typeof(double), null, CultureInfo.InvariantCulture);
        Assert.Equal(25.0, result);
    }

    /// <summary>
    /// Tests convert zerodouble returnszerocornerradius.
    /// </summary>
    [Fact]
    public void Convert_ZeroDouble_ReturnsZeroCornerRadius()
    {
        var result = _converter.Convert(0.0, typeof(CornerRadius), null, CultureInfo.InvariantCulture);
        Assert.IsType<CornerRadius>(result);
        var cr = (CornerRadius)result;
        Assert.Equal(0.0, cr.TopLeft);
    }
}
