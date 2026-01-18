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
/// Unit tests for InRangeConverter.
/// </summary>
public class InRangeConverterTests
{
    [Fact]
    public void Convert_WithValueInRange_ReturnsTrue()
    {
        var converter = new InRangeConverter { LowerBound = 0, UpperBound = 100 };
        var result = converter.Convert(50.0, typeof(bool), null, CultureInfo.InvariantCulture);
        Assert.Equal(true, result);
    }

    /// <summary>
    /// Tests convert withvalueatlowerbound returnstrue.
    /// </summary>
    [Fact]
    public void Convert_WithValueAtLowerBound_ReturnsTrue()
    {
        var converter = new InRangeConverter { LowerBound = 0, UpperBound = 100 };
        var result = converter.Convert(0.0, typeof(bool), null, CultureInfo.InvariantCulture);
        Assert.Equal(true, result);
    }

    /// <summary>
    /// Tests convert withvalueatupperbound returnstrue.
    /// </summary>
    [Fact]
    public void Convert_WithValueAtUpperBound_ReturnsTrue()
    {
        var converter = new InRangeConverter { LowerBound = 0, UpperBound = 100 };
        var result = converter.Convert(100.0, typeof(bool), null, CultureInfo.InvariantCulture);
        Assert.Equal(true, result);
    }

    /// <summary>
    /// Tests convert withvaluebelowrange returnsfalse.
    /// </summary>
    [Fact]
    public void Convert_WithValueBelowRange_ReturnsFalse()
    {
        var converter = new InRangeConverter { LowerBound = 0, UpperBound = 100 };
        var result = converter.Convert(-1.0, typeof(bool), null, CultureInfo.InvariantCulture);
        Assert.Equal(false, result);
    }

    /// <summary>
    /// Tests convert withvalueaboverange returnsfalse.
    /// </summary>
    [Fact]
    public void Convert_WithValueAboveRange_ReturnsFalse()
    {
        var converter = new InRangeConverter { LowerBound = 0, UpperBound = 100 };
        var result = converter.Convert(101.0, typeof(bool), null, CultureInfo.InvariantCulture);
        Assert.Equal(false, result);
    }

    /// <summary>
    /// Tests convert withnull returnsfalse.
    /// </summary>
    [Fact]
    public void Convert_WithNull_ReturnsFalse()
    {
        var converter = new InRangeConverter { LowerBound = 0, UpperBound = 100 };
        var result = converter.Convert(null, typeof(bool), null, CultureInfo.InvariantCulture);
        Assert.Equal(false, result);
    }

    /// <summary>
    /// Tests convert withinvalidtype returnsfalse.
    /// </summary>
    [Fact]
    public void Convert_WithInvalidType_ReturnsFalse()
    {
        var converter = new InRangeConverter { LowerBound = 0, UpperBound = 100 };
        var result = converter.Convert("not a number", typeof(bool), null, CultureInfo.InvariantCulture);
        Assert.Equal(false, result);
    }

    /// <summary>
    /// Tests convert withdefaultbounds valueinrange returnstrue.
    /// </summary>
    [Fact]
    public void Convert_WithDefaultBounds_ValueInRange_ReturnsTrue()
    {
        var converter = new InRangeConverter();
        var result = converter.Convert(0.0, typeof(bool), null, CultureInfo.InvariantCulture);
        Assert.Equal(true, result);
    }

    /// <summary>
    /// Tests convert withnegativebounds returnscorrectresult.
    /// </summary>
    [Fact]
    public void Convert_WithNegativeBounds_ReturnsCorrectResult()
    {
        var converter = new InRangeConverter { LowerBound = -100, UpperBound = -10 };
        var result = converter.Convert(-50.0, typeof(bool), null, CultureInfo.InvariantCulture);
        Assert.Equal(true, result);
    }

    /// <summary>
    /// Tests convert withstringnumber returnstrue.
    /// </summary>
    [Fact]
    public void Convert_WithStringNumber_ReturnsTrue()
    {
        var converter = new InRangeConverter { LowerBound = 0, UpperBound = 100 };
        var result = converter.Convert("50", typeof(bool), null, CultureInfo.InvariantCulture);
        Assert.Equal(true, result);
    }

    /// <summary>
    /// Tests convertback throwsnotimplementedexception.
    /// </summary>
    [Fact]
    public void ConvertBack_ThrowsNotImplementedException()
    {
        var converter = new InRangeConverter();
        Assert.Throws<NotImplementedException>(() =>
            converter.ConvertBack(true, typeof(double), null, CultureInfo.InvariantCulture));
    }
}
