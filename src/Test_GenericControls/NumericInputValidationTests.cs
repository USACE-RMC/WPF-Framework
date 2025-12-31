/**
* NOTICE:
* The U.S. Army Corps of Engineers, Risk Management Center (USACE-RMC) makes no guarantees about
* the results, or appropriateness of outputs, obtained from this library.
*
* LIST OF CONDITIONS:
* Redistribution and use in source and binary forms, with or without modification, are permitted
* provided that the following conditions are met:
* ● Redistributions of source code must retain the above notice, this list of conditions, and the
* following disclaimer.
* ● Redistributions in binary form must reproduce the above notice, this list of conditions, and
* the following disclaimer in the documentation and/or other materials provided with the distribution.
* ● The names of the U.S. Government, the U.S. Army Corps of Engineers, the Institute for Water
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
* **/
using System.Globalization;
using Xunit;

namespace Test_GenericControls;

/// <summary>
/// Tests for numeric input validation and parsing edge cases.
/// </summary>
public class NumericInputValidationTests
{
    #region Parsing Edge Cases

    [Theory]
    [InlineData("0", 0)]
    [InlineData("1", 1)]
    [InlineData("-1", -1)]
    [InlineData("123", 123)]
    [InlineData("123.456", 123.456)]
    [InlineData("-123.456", -123.456)]
    [InlineData(".5", 0.5)]
    [InlineData("-.5", -0.5)]
    [InlineData("1e10", 1e10)]
    [InlineData("1E10", 1e10)]
    [InlineData("1.5e10", 1.5e10)]
    [InlineData("-1.5e-10", -1.5e-10)]
    public void DoubleParse_ValidFormats_ShouldSucceed(string input, double expected)
    {
        bool success = double.TryParse(input, NumberStyles.Any, CultureInfo.InvariantCulture, out double result);

        Assert.True(success);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("")]
    [InlineData("abc")]
    [InlineData("1.2.3")]
    [InlineData("--1")]
    [InlineData("1-")]
    [InlineData("1..2")]
    public void DoubleParse_InvalidFormats_ShouldFail(string input)
    {
        bool success = double.TryParse(input, NumberStyles.Any, CultureInfo.InvariantCulture, out _);

        Assert.False(success);
    }

    #endregion

    #region Special Values

    [Fact]
    public void DoubleNaN_Comparisons_ShouldBehaveCorrectly()
    {
        double nan = double.NaN;

        // NaN is not equal to anything, including itself
        Assert.False(nan == nan);
        Assert.True(nan != nan);
        Assert.True(double.IsNaN(nan));

        // NaN comparisons always return false
        Assert.False(nan < 0);
        Assert.False(nan > 0);
        Assert.False(nan <= 0);
        Assert.False(nan >= 0);
    }

    [Fact]
    public void DoubleInfinity_Comparisons_ShouldBehaveCorrectly()
    {
        double posInf = double.PositiveInfinity;
        double negInf = double.NegativeInfinity;

        Assert.True(double.IsPositiveInfinity(posInf));
        Assert.True(double.IsNegativeInfinity(negInf));

        // Positive infinity is greater than any finite number
        Assert.True(posInf > double.MaxValue);
        Assert.False(posInf < double.MaxValue);

        // Negative infinity is less than any finite number
        Assert.True(negInf < double.MinValue);
        Assert.False(negInf > double.MinValue);
    }

    [Fact]
    public void DoubleMinMax_ShouldNotOverflow()
    {
        double max = double.MaxValue;
        double min = double.MinValue;

        // These should not throw
        bool maxValid = max < double.PositiveInfinity;
        bool minValid = min > double.NegativeInfinity;

        Assert.True(maxValid);
        Assert.True(minValid);
    }

    #endregion

    #region Boundary Value Tests

    [Fact]
    public void RangeValidation_AtExactBoundary_InclusiveShouldPass()
    {
        double min = 0;
        double max = 100;
        double value = 0; // exactly at minimum

        bool isValid = value >= min && value <= max;

        Assert.True(isValid);
    }

    [Fact]
    public void RangeValidation_AtExactBoundary_ExclusiveShouldFail()
    {
        double min = 0;
        double max = 100;
        double value = 0; // exactly at minimum

        bool isValid = value > min && value < max;

        Assert.False(isValid);
    }

    [Fact]
    public void RangeValidation_VeryCloseToMinBoundary_ExclusiveShouldPass()
    {
        double min = 0;
        double max = 100;
        double value = double.Epsilon; // smallest positive value greater than 0

        bool isValid = value > min && value < max;

        Assert.True(isValid);
    }

    [Fact]
    public void RangeValidation_NegativeZero_ShouldBeTreatedAsZero()
    {
        double min = 0;
        double max = 100;
        double negativeZero = -0.0;

        // -0.0 equals 0.0 in IEEE 754
        Assert.Equal(0.0, negativeZero);

        bool isValidInclusive = negativeZero >= min && negativeZero <= max;
        Assert.True(isValidInclusive);
    }

    #endregion

    #region Precision Tests

    [Fact]
    public void DoublePrecision_ShouldHandleSmallDifferences()
    {
        double a = 0.1 + 0.2;
        double b = 0.3;

        // Direct equality may fail due to floating point precision
        // This is expected behavior
        Assert.NotEqual(a, b); // 0.1 + 0.2 != 0.3 in IEEE 754

        // Use tolerance for approximate equality
        double tolerance = 1e-10;
        Assert.True(Math.Abs(a - b) < tolerance);
    }

    [Fact]
    public void RangeValidation_WithFloatingPointPrecisionIssues_ShouldConsider()
    {
        // This demonstrates a potential edge case with exclusive bounds
        double min = 0.1;
        double max = 0.3;
        double value = 0.1 + 0.2; // Should be 0.3 but isn't exactly

        bool isValidExclusive = value > min && value < max;

        // Due to floating point precision, 0.1 + 0.2 is slightly > 0.3
        // so this might unexpectedly pass the exclusive check
        // This is a known limitation of floating point comparison
        Assert.False(isValidExclusive); // value > 0.3, so it fails
    }

    #endregion

    #region Culture-Specific Parsing

    [Fact]
    public void InvariantCulture_DecimalSeparator_ShouldBePeriod()
    {
        var culture = CultureInfo.InvariantCulture;

        Assert.Equal(".", culture.NumberFormat.NumberDecimalSeparator);
    }

    [Fact]
    public void GermanCulture_DecimalSeparator_ShouldBeComma()
    {
        var culture = new CultureInfo("de-DE");

        Assert.Equal(",", culture.NumberFormat.NumberDecimalSeparator);
    }

    [Fact]
    public void ParseNumber_DifferentCultures_ShouldRespectSeparator()
    {
        string germanNumber = "1,5";
        string usNumber = "1.5";

        var germanCulture = new CultureInfo("de-DE");
        var usCulture = new CultureInfo("en-US");

        double.TryParse(germanNumber, NumberStyles.Any, germanCulture, out double germanResult);
        double.TryParse(usNumber, NumberStyles.Any, usCulture, out double usResult);

        Assert.Equal(1.5, germanResult);
        Assert.Equal(1.5, usResult);
    }

    #endregion
}
