/*
* NOTICE:
* The U.S. Army Corps of Engineers, Risk Management Center (USACE-RMC) makes no guarantees about
* the results, or appropriateness of outputs, obtained from this software.
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
*/
using Xunit;

namespace Test_GenericControls;

/// <summary>
/// Tests for bounds validation logic across numeric controls.
/// Tests the BoundsAreExclusive property behavior.
/// </summary>
public class BoundsValidationTests
{
    #region RangeWrapper Tests

    [Fact]
    public void RangeWrapper_DefaultValues_ShouldBeMinMaxDouble()
    {
        var wrapper = new RangeWrapper();

        Assert.Equal(double.MinValue, wrapper.Minimum);
        Assert.Equal(double.MaxValue, wrapper.Maximum);
        Assert.False(wrapper.BoundsAreExclusive);
    }

    [Fact]
    public void RangeWrapper_SetProperties_ShouldReturnCorrectValues()
    {
        var wrapper = new RangeWrapper
        {
            Minimum = 0,
            Maximum = 100,
            BoundsAreExclusive = true
        };

        Assert.Equal(0, wrapper.Minimum);
        Assert.Equal(100, wrapper.Maximum);
        Assert.True(wrapper.BoundsAreExclusive);
    }

    #endregion

    #region RangeValidationRule Tests - Inclusive Bounds (Default)

    [Fact]
    public void RangeValidationRule_ValueAtMinimum_InclusiveBounds_ShouldBeValid()
    {
        var wrapper = new RangeWrapper { Minimum = 0, Maximum = 100, BoundsAreExclusive = false };
        var rule = new RangeValidationRule { Wrapper = wrapper };

        var result = rule.Validate("0", System.Globalization.CultureInfo.InvariantCulture);

        Assert.True(result.IsValid);
    }

    [Fact]
    public void RangeValidationRule_ValueAtMaximum_InclusiveBounds_ShouldBeValid()
    {
        var wrapper = new RangeWrapper { Minimum = 0, Maximum = 100, BoundsAreExclusive = false };
        var rule = new RangeValidationRule { Wrapper = wrapper };

        var result = rule.Validate("100", System.Globalization.CultureInfo.InvariantCulture);

        Assert.True(result.IsValid);
    }

    [Fact]
    public void RangeValidationRule_ValueBelowMinimum_InclusiveBounds_ShouldBeInvalid()
    {
        var wrapper = new RangeWrapper { Minimum = 0, Maximum = 100, BoundsAreExclusive = false };
        var rule = new RangeValidationRule { Wrapper = wrapper };

        var result = rule.Validate("-1", System.Globalization.CultureInfo.InvariantCulture);

        Assert.False(result.IsValid);
    }

    [Fact]
    public void RangeValidationRule_ValueAboveMaximum_InclusiveBounds_ShouldBeInvalid()
    {
        var wrapper = new RangeWrapper { Minimum = 0, Maximum = 100, BoundsAreExclusive = false };
        var rule = new RangeValidationRule { Wrapper = wrapper };

        var result = rule.Validate("101", System.Globalization.CultureInfo.InvariantCulture);

        Assert.False(result.IsValid);
    }

    [Fact]
    public void RangeValidationRule_ValueWithinRange_InclusiveBounds_ShouldBeValid()
    {
        var wrapper = new RangeWrapper { Minimum = 0, Maximum = 100, BoundsAreExclusive = false };
        var rule = new RangeValidationRule { Wrapper = wrapper };

        var result = rule.Validate("50", System.Globalization.CultureInfo.InvariantCulture);

        Assert.True(result.IsValid);
    }

    #endregion

    #region RangeValidationRule Tests - Exclusive Bounds

    [Fact]
    public void RangeValidationRule_ValueAtMinimum_ExclusiveBounds_ShouldBeInvalid()
    {
        var wrapper = new RangeWrapper { Minimum = 0, Maximum = 100, BoundsAreExclusive = true };
        var rule = new RangeValidationRule { Wrapper = wrapper };

        var result = rule.Validate("0", System.Globalization.CultureInfo.InvariantCulture);

        Assert.False(result.IsValid);
    }

    [Fact]
    public void RangeValidationRule_ValueAtMaximum_ExclusiveBounds_ShouldBeInvalid()
    {
        var wrapper = new RangeWrapper { Minimum = 0, Maximum = 100, BoundsAreExclusive = true };
        var rule = new RangeValidationRule { Wrapper = wrapper };

        var result = rule.Validate("100", System.Globalization.CultureInfo.InvariantCulture);

        Assert.False(result.IsValid);
    }

    [Fact]
    public void RangeValidationRule_ValueJustAboveMinimum_ExclusiveBounds_ShouldBeValid()
    {
        var wrapper = new RangeWrapper { Minimum = 0, Maximum = 100, BoundsAreExclusive = true };
        var rule = new RangeValidationRule { Wrapper = wrapper };

        var result = rule.Validate("0.001", System.Globalization.CultureInfo.InvariantCulture);

        Assert.True(result.IsValid);
    }

    [Fact]
    public void RangeValidationRule_ValueJustBelowMaximum_ExclusiveBounds_ShouldBeValid()
    {
        var wrapper = new RangeWrapper { Minimum = 0, Maximum = 100, BoundsAreExclusive = true };
        var rule = new RangeValidationRule { Wrapper = wrapper };

        var result = rule.Validate("99.999", System.Globalization.CultureInfo.InvariantCulture);

        Assert.True(result.IsValid);
    }

    [Fact]
    public void RangeValidationRule_ValueWithinRange_ExclusiveBounds_ShouldBeValid()
    {
        var wrapper = new RangeWrapper { Minimum = 0, Maximum = 100, BoundsAreExclusive = true };
        var rule = new RangeValidationRule { Wrapper = wrapper };

        var result = rule.Validate("50", System.Globalization.CultureInfo.InvariantCulture);

        Assert.True(result.IsValid);
    }

    #endregion

    #region RangeValidationRule Edge Cases

    [Fact]
    public void RangeValidationRule_EmptyString_ShouldBeValid()
    {
        var wrapper = new RangeWrapper { Minimum = 0, Maximum = 100 };
        var rule = new RangeValidationRule { Wrapper = wrapper };

        var result = rule.Validate("", System.Globalization.CultureInfo.InvariantCulture);

        // Empty string parses to 0, which is valid in range 0-100
        Assert.True(result.IsValid);
    }

    [Fact]
    public void RangeValidationRule_InvalidText_ShouldBeInvalid()
    {
        var wrapper = new RangeWrapper { Minimum = 0, Maximum = 100 };
        var rule = new RangeValidationRule { Wrapper = wrapper };

        var result = rule.Validate("abc", System.Globalization.CultureInfo.InvariantCulture);

        Assert.False(result.IsValid);
    }

    [Fact]
    public void RangeValidationRule_NegativeRange_ShouldWork()
    {
        var wrapper = new RangeWrapper { Minimum = -100, Maximum = -10, BoundsAreExclusive = false };
        var rule = new RangeValidationRule { Wrapper = wrapper };

        var resultInRange = rule.Validate("-50", System.Globalization.CultureInfo.InvariantCulture);
        var resultAtMin = rule.Validate("-100", System.Globalization.CultureInfo.InvariantCulture);
        var resultAtMax = rule.Validate("-10", System.Globalization.CultureInfo.InvariantCulture);
        var resultBelowMin = rule.Validate("-101", System.Globalization.CultureInfo.InvariantCulture);
        var resultAboveMax = rule.Validate("-9", System.Globalization.CultureInfo.InvariantCulture);

        Assert.True(resultInRange.IsValid);
        Assert.True(resultAtMin.IsValid);
        Assert.True(resultAtMax.IsValid);
        Assert.False(resultBelowMin.IsValid);
        Assert.False(resultAboveMax.IsValid);
    }

    [Fact]
    public void RangeValidationRule_DecimalValues_ShouldWork()
    {
        var wrapper = new RangeWrapper { Minimum = 0.5, Maximum = 1.5, BoundsAreExclusive = false };
        var rule = new RangeValidationRule { Wrapper = wrapper };

        var resultInRange = rule.Validate("1.0", System.Globalization.CultureInfo.InvariantCulture);
        var resultAtMin = rule.Validate("0.5", System.Globalization.CultureInfo.InvariantCulture);
        var resultAtMax = rule.Validate("1.5", System.Globalization.CultureInfo.InvariantCulture);
        var resultBelowMin = rule.Validate("0.4", System.Globalization.CultureInfo.InvariantCulture);
        var resultAboveMax = rule.Validate("1.6", System.Globalization.CultureInfo.InvariantCulture);

        Assert.True(resultInRange.IsValid);
        Assert.True(resultAtMin.IsValid);
        Assert.True(resultAtMax.IsValid);
        Assert.False(resultBelowMin.IsValid);
        Assert.False(resultAboveMax.IsValid);
    }

    [Fact]
    public void RangeValidationRule_LargeNumbers_ShouldWork()
    {
        var wrapper = new RangeWrapper { Minimum = 1e10, Maximum = 1e12, BoundsAreExclusive = false };
        var rule = new RangeValidationRule { Wrapper = wrapper };

        var resultInRange = rule.Validate("1e11", System.Globalization.CultureInfo.InvariantCulture);

        Assert.True(resultInRange.IsValid);
    }

    #endregion
}
