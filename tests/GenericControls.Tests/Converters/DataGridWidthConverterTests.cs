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
/// Unit tests for DataGridWidthConverter.
/// </summary>
public class DataGridWidthConverterTests
{
    /// <summary>
    /// Converter instance used for testing.
    /// </summary>
    private readonly DataGridWidthConverter _converter = new();

    /// <summary>
    /// Tests convert withvalidwidth returnsadjustedwidth.
    /// </summary>
    [Fact]
    public void Convert_WithValidWidth_ReturnsAdjustedWidth()
    {
        var scrollBarWidth = SystemParameters.VerticalScrollBarWidth;
        var result = _converter.Convert(500.0, typeof(double), null, CultureInfo.InvariantCulture);
        Assert.Equal(500.0 - scrollBarWidth, result);
    }

    /// <summary>
    /// Tests convert withnull returnszero.
    /// </summary>
    [Fact]
    public void Convert_WithNull_ReturnsZero()
    {
        var result = _converter.Convert(null, typeof(double), null, CultureInfo.InvariantCulture);
        Assert.Equal(0.0, result);
    }

    /// <summary>
    /// Tests convert withinvalidstring returnszero.
    /// </summary>
    [Fact]
    public void Convert_WithInvalidString_ReturnsZero()
    {
        var result = _converter.Convert("invalid", typeof(double), null, CultureInfo.InvariantCulture);
        Assert.Equal(0.0, result);
    }

    /// <summary>
    /// Tests convert withstringnumber returnsadjustedwidth.
    /// </summary>
    [Fact]
    public void Convert_WithStringNumber_ReturnsAdjustedWidth()
    {
        var scrollBarWidth = SystemParameters.VerticalScrollBarWidth;
        var result = _converter.Convert("300", typeof(double), null, CultureInfo.InvariantCulture);
        Assert.Equal(300.0 - scrollBarWidth, result);
    }

    /// <summary>
    /// Tests convert withzero returnsnegativescrollbarwidth.
    /// </summary>
    [Fact]
    public void Convert_WithZero_ReturnsNegativeScrollBarWidth()
    {
        var scrollBarWidth = SystemParameters.VerticalScrollBarWidth;
        var result = _converter.Convert(0.0, typeof(double), null, CultureInfo.InvariantCulture);
        Assert.Equal(-scrollBarWidth, result);
    }

    /// <summary>
    /// Tests convertback throwsnotsupportedexception.
    /// </summary>
    [Fact]
    public void ConvertBack_ThrowsNotSupportedException()
    {
        Assert.Throws<NotSupportedException>(() =>
            _converter.ConvertBack(100.0, typeof(double), null, CultureInfo.InvariantCulture));
    }
}
