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

namespace OxyPlotControls.Tests.Converters;

/// <summary>
/// Tests for OxyVerticalAlignmentConverter.
/// Verifies pass-through and swap behavior for WPF VerticalAlignment values.
/// </summary>
public class OxyVerticalAlignmentConverterTests
{
    private readonly OxyVerticalAlignmentConverter _converter = new();

    #region Convert Tests

    [Fact]
    public void Convert_NullValue_ReturnsCenter()
    {
        var result = _converter.Convert(null, typeof(VerticalAlignment), null, CultureInfo.InvariantCulture);
        Assert.Equal(VerticalAlignment.Center, result);
    }

    [Fact]
    public void Convert_WrongType_ReturnsCenter()
    {
        var result = _converter.Convert("not an alignment", typeof(VerticalAlignment), null, CultureInfo.InvariantCulture);
        Assert.Equal(VerticalAlignment.Center, result);
    }

    [Fact]
    public void Convert_Top_ReturnsTop()
    {
        var result = _converter.Convert(VerticalAlignment.Top, typeof(VerticalAlignment), null, CultureInfo.InvariantCulture);
        Assert.Equal(VerticalAlignment.Top, result);
    }

    [Fact]
    public void Convert_Center_ReturnsCenter()
    {
        var result = _converter.Convert(VerticalAlignment.Center, typeof(VerticalAlignment), null, CultureInfo.InvariantCulture);
        Assert.Equal(VerticalAlignment.Center, result);
    }

    [Fact]
    public void Convert_Bottom_ReturnsBottom()
    {
        var result = _converter.Convert(VerticalAlignment.Bottom, typeof(VerticalAlignment), null, CultureInfo.InvariantCulture);
        Assert.Equal(VerticalAlignment.Bottom, result);
    }

    [Fact]
    public void Convert_Stretch_ReturnsStretch()
    {
        var result = _converter.Convert(VerticalAlignment.Stretch, typeof(VerticalAlignment), null, CultureInfo.InvariantCulture);
        Assert.Equal(VerticalAlignment.Stretch, result);
    }

    #endregion

    #region Convert with Swap Tests

    [Fact]
    public void Convert_SwapTop_ReturnsBottom()
    {
        var result = _converter.Convert(VerticalAlignment.Top, typeof(VerticalAlignment), "Swap", CultureInfo.InvariantCulture);
        Assert.Equal(VerticalAlignment.Bottom, result);
    }

    [Fact]
    public void Convert_SwapBottom_ReturnsTop()
    {
        var result = _converter.Convert(VerticalAlignment.Bottom, typeof(VerticalAlignment), "Swap", CultureInfo.InvariantCulture);
        Assert.Equal(VerticalAlignment.Top, result);
    }

    [Fact]
    public void Convert_SwapCenter_ReturnsCenter()
    {
        var result = _converter.Convert(VerticalAlignment.Center, typeof(VerticalAlignment), "Swap", CultureInfo.InvariantCulture);
        Assert.Equal(VerticalAlignment.Center, result);
    }

    #endregion

    #region ConvertBack Tests

    [Fact]
    public void ConvertBack_NullValue_ReturnsCenter()
    {
        var result = _converter.ConvertBack(null, typeof(VerticalAlignment), null, CultureInfo.InvariantCulture);
        Assert.Equal(VerticalAlignment.Center, result);
    }

    [Fact]
    public void ConvertBack_WrongType_ReturnsCenter()
    {
        var result = _converter.ConvertBack("not an alignment", typeof(VerticalAlignment), null, CultureInfo.InvariantCulture);
        Assert.Equal(VerticalAlignment.Center, result);
    }

    [Fact]
    public void ConvertBack_Top_ReturnsTop()
    {
        var result = _converter.ConvertBack(VerticalAlignment.Top, typeof(VerticalAlignment), null, CultureInfo.InvariantCulture);
        Assert.Equal(VerticalAlignment.Top, result);
    }

    [Fact]
    public void ConvertBack_Bottom_ReturnsBottom()
    {
        var result = _converter.ConvertBack(VerticalAlignment.Bottom, typeof(VerticalAlignment), null, CultureInfo.InvariantCulture);
        Assert.Equal(VerticalAlignment.Bottom, result);
    }

    [Fact]
    public void ConvertBack_SwapTop_ReturnsBottom()
    {
        var result = _converter.ConvertBack(VerticalAlignment.Top, typeof(VerticalAlignment), "Swap", CultureInfo.InvariantCulture);
        Assert.Equal(VerticalAlignment.Bottom, result);
    }

    [Fact]
    public void ConvertBack_SwapBottom_ReturnsTop()
    {
        var result = _converter.ConvertBack(VerticalAlignment.Bottom, typeof(VerticalAlignment), "Swap", CultureInfo.InvariantCulture);
        Assert.Equal(VerticalAlignment.Top, result);
    }

    #endregion

    #region Round Trip Tests

    [Theory]
    [InlineData(VerticalAlignment.Top)]
    [InlineData(VerticalAlignment.Center)]
    [InlineData(VerticalAlignment.Bottom)]
    [InlineData(VerticalAlignment.Stretch)]
    public void Convert_ConvertBack_RoundTrip(VerticalAlignment alignment)
    {
        var converted = _converter.Convert(alignment, typeof(VerticalAlignment), null, CultureInfo.InvariantCulture);
        var roundTripped = _converter.ConvertBack(converted, typeof(VerticalAlignment), null, CultureInfo.InvariantCulture);
        Assert.Equal(alignment, roundTripped);
    }

    [Theory]
    [InlineData(VerticalAlignment.Top)]
    [InlineData(VerticalAlignment.Center)]
    [InlineData(VerticalAlignment.Bottom)]
    public void Convert_ConvertBack_SwapRoundTrip(VerticalAlignment alignment)
    {
        var converted = _converter.Convert(alignment, typeof(VerticalAlignment), "Swap", CultureInfo.InvariantCulture);
        var roundTripped = _converter.ConvertBack(converted, typeof(VerticalAlignment), "Swap", CultureInfo.InvariantCulture);
        Assert.Equal(alignment, roundTripped);
    }

    #endregion
}
