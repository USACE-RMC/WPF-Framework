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
/// Unit tests for DoubleToGridLengthConverter.
/// </summary>
public class DoubleToGridLengthConverterTests
{
    /// <summary>
    /// Converter instance used for testing.
    /// </summary>
    private readonly DoubleToGridLengthConverter _converter = new();

    /// <summary>
    /// Tests convert doubletogridlength returnsgridlength.
    /// </summary>
    [Fact]
    public void Convert_DoubleToGridLength_ReturnsGridLength()
    {
        var result = _converter.Convert(100.0, typeof(GridLength), null, CultureInfo.InvariantCulture);
        Assert.IsType<GridLength>(result);
        Assert.Equal(100.0, ((GridLength)result).Value);
    }

    /// <summary>
    /// Tests convert gridlengthtodouble returnsdouble.
    /// </summary>
    [Fact]
    public void Convert_GridLengthToDouble_ReturnsDouble()
    {
        var gridLength = new GridLength(150.0);
        var result = _converter.Convert(gridLength, typeof(double), null, CultureInfo.InvariantCulture);
        Assert.Equal(150.0, result);
    }

    /// <summary>
    /// Tests convert nulltogridlength returnsauto.
    /// </summary>
    [Fact]
    public void Convert_NullToGridLength_ReturnsAuto()
    {
        var result = _converter.Convert(null, typeof(GridLength), null, CultureInfo.InvariantCulture);
        Assert.Equal(GridLength.Auto, result);
    }

    /// <summary>
    /// Tests convert invalidtypetogridlength returnsauto.
    /// </summary>
    [Fact]
    public void Convert_InvalidTypeToGridLength_ReturnsAuto()
    {
        var result = _converter.Convert("invalid", typeof(GridLength), null, CultureInfo.InvariantCulture);
        Assert.Equal(GridLength.Auto, result);
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
        var result = _converter.Convert(100.0, typeof(string), null, CultureInfo.InvariantCulture);
        Assert.Null(result);
    }

    /// <summary>
    /// Tests convertback doubletogridlength returnsgridlength.
    /// </summary>
    [Fact]
    public void ConvertBack_DoubleToGridLength_ReturnsGridLength()
    {
        var result = _converter.ConvertBack(75.0, typeof(GridLength), null, CultureInfo.InvariantCulture);
        Assert.IsType<GridLength>(result);
        Assert.Equal(75.0, ((GridLength)result).Value);
    }

    /// <summary>
    /// Tests convertback gridlengthtodouble returnsdouble.
    /// </summary>
    [Fact]
    public void ConvertBack_GridLengthToDouble_ReturnsDouble()
    {
        var gridLength = new GridLength(200.0);
        var result = _converter.ConvertBack(gridLength, typeof(double), null, CultureInfo.InvariantCulture);
        Assert.Equal(200.0, result);
    }

    /// <summary>
    /// Tests convert zerodouble returnsgridlengthwithzero.
    /// </summary>
    [Fact]
    public void Convert_ZeroDouble_ReturnsGridLengthWithZero()
    {
        var result = _converter.Convert(0.0, typeof(GridLength), null, CultureInfo.InvariantCulture);
        Assert.IsType<GridLength>(result);
        Assert.Equal(0.0, ((GridLength)result).Value);
    }
}
