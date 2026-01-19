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
/// Unit tests for VectorToPointConverter.
/// </summary>
public class VectorToPointConverterTests
{
    /// <summary>
    /// Converter instance used for testing.
    /// </summary>
    private readonly VectorToPointConverter _converter = new();

    /// <summary>
    /// Tests convert withvector returnspoint.
    /// </summary>
    [Fact]
    public void Convert_WithVector_ReturnsPoint()
    {
        var vector = new Vector(10, 20);
        var result = _converter.Convert(vector, typeof(Point), null, CultureInfo.InvariantCulture);
        Assert.IsType<Point>(result);
        var point = (Point)result;
        Assert.Equal(10, point.X);
        Assert.Equal(20, point.Y);
    }

    /// <summary>
    /// Tests convert withnull returnsnull.
    /// </summary>
    [Fact]
    public void Convert_WithNull_ReturnsNull()
    {
        var result = _converter.Convert(null, typeof(Point), null, CultureInfo.InvariantCulture);
        Assert.Null(result);
    }

    /// <summary>
    /// Tests convert withinvalidtype returnsnull.
    /// </summary>
    [Fact]
    public void Convert_WithInvalidType_ReturnsNull()
    {
        var result = _converter.Convert("invalid", typeof(Point), null, CultureInfo.InvariantCulture);
        Assert.Null(result);
    }

    /// <summary>
    /// Tests convert withzerovector returnszeropoint.
    /// </summary>
    [Fact]
    public void Convert_WithZeroVector_ReturnsZeroPoint()
    {
        var vector = new Vector(0, 0);
        var result = _converter.Convert(vector, typeof(Point), null, CultureInfo.InvariantCulture);
        Assert.IsType<Point>(result);
        var point = (Point)result;
        Assert.Equal(0, point.X);
        Assert.Equal(0, point.Y);
    }

    /// <summary>
    /// Tests convert withnegativevector returnsnegativepoint.
    /// </summary>
    [Fact]
    public void Convert_WithNegativeVector_ReturnsNegativePoint()
    {
        var vector = new Vector(-5, -15);
        var result = _converter.Convert(vector, typeof(Point), null, CultureInfo.InvariantCulture);
        Assert.IsType<Point>(result);
        var point = (Point)result;
        Assert.Equal(-5, point.X);
        Assert.Equal(-15, point.Y);
    }

    /// <summary>
    /// Tests convertback withpoint returnsvector.
    /// </summary>
    [Fact]
    public void ConvertBack_WithPoint_ReturnsVector()
    {
        var point = new Point(30, 40);
        var result = _converter.ConvertBack(point, typeof(Vector), null, CultureInfo.InvariantCulture);
        Assert.IsType<Vector>(result);
        var vector = (Vector)result;
        Assert.Equal(30, vector.X);
        Assert.Equal(40, vector.Y);
    }

    /// <summary>
    /// Tests convertback withnull returnsnull.
    /// </summary>
    [Fact]
    public void ConvertBack_WithNull_ReturnsNull()
    {
        var result = _converter.ConvertBack(null, typeof(Vector), null, CultureInfo.InvariantCulture);
        Assert.Null(result);
    }

    /// <summary>
    /// Tests convertback withinvalidtype returnsnull.
    /// </summary>
    [Fact]
    public void ConvertBack_WithInvalidType_ReturnsNull()
    {
        var result = _converter.ConvertBack("invalid", typeof(Vector), null, CultureInfo.InvariantCulture);
        Assert.Null(result);
    }
}
