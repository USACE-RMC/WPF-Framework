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

using System;
using System.Globalization;
using OxyPlot;
using Xunit;

namespace OxyPlotControls.Tests.Converters;

/// <summary>
/// Tests for DateToNumberConverter.
/// Verifies proper conversion between DateTime values and OxyPlot numeric date representations.
/// </summary>
public class DateToNumberConverterTests
{
    /// <summary>
    /// The converter instance being tested.
    /// </summary>
    private readonly DateToNumberConverter _converter = new();

    #region Convert Tests

    /// <summary>
    /// Tests that Convert returns null when given a null value.
    /// </summary>
    [Fact]
    public void Convert_NullValue_ReturnsNull()
    {
        // Arrange
        object? value = null;

        // Act
        var result = _converter.Convert(value, typeof(DateTime), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.Null(result);
    }

    /// <summary>
    /// Tests that Convert returns the value unchanged when given an incorrect type.
    /// </summary>
    [Fact]
    public void Convert_WrongType_ReturnsValue()
    {
        // Arrange
        var value = "not a double";

        // Act
        var result = _converter.Convert(value, typeof(DateTime), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.Equal("not a double", result);
    }

    /// <summary>
    /// Tests that Convert returns a DateTime when given a valid double value (OxyPlot uses days since year 1900).
    /// </summary>
    [Fact]
    public void Convert_ValidDouble_ReturnsDateTime()
    {
        // Arrange - OxyPlot uses days since year 1900
        var value = 44561.0; // Should be around Jan 1, 2022

        // Act
        var result = _converter.Convert(value, typeof(DateTime), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.IsType<DateTime>(result);
    }

    /// <summary>
    /// Tests that Convert returns an early date when given a zero value.
    /// </summary>
    [Fact]
    public void Convert_ZeroValue_ReturnsEarlyDate()
    {
        // Arrange
        var value = 0.0;

        // Act
        var result = _converter.Convert(value, typeof(DateTime), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.IsType<DateTime>(result);
    }

    /// <summary>
    /// Tests that Convert handles negative values gracefully by converting them to DateTime.
    /// </summary>
    [Fact]
    public void Convert_NegativeValue_HandlesGracefully()
    {
        // Arrange
        var value = -100.0;

        // Act
        var result = _converter.Convert(value, typeof(DateTime), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.IsType<DateTime>(result);
    }

    #endregion

    #region ConvertBack Tests

    /// <summary>
    /// Tests that ConvertBack returns null when given a null value.
    /// </summary>
    [Fact]
    public void ConvertBack_NullValue_ReturnsNull()
    {
        // Arrange
        object? value = null;

        // Act
        var result = _converter.ConvertBack(value, typeof(double), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.Null(result);
    }

    /// <summary>
    /// Tests that ConvertBack returns the value unchanged when given an incorrect type.
    /// </summary>
    [Fact]
    public void ConvertBack_WrongType_ReturnsValue()
    {
        // Arrange
        var value = "not a DateTime";

        // Act
        var result = _converter.ConvertBack(value, typeof(double), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.Equal("not a DateTime", result);
    }

    /// <summary>
    /// Tests that ConvertBack returns a double when given a valid DateTime.
    /// </summary>
    [Fact]
    public void ConvertBack_ValidDateTime_ReturnsDouble()
    {
        // Arrange
        var value = new DateTime(2022, 1, 1);

        // Act
        var result = _converter.ConvertBack(value, typeof(double), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.IsType<double>(result);
    }

    /// <summary>
    /// Tests that ConvertBack returns NaN when given DateTime.MinValue.
    /// </summary>
    [Fact]
    public void ConvertBack_MinValue_ReturnsNaN()
    {
        // Arrange
        var value = DateTime.MinValue;

        // Act
        var result = _converter.ConvertBack(value, typeof(double), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.True(double.IsNaN((double)result!));
    }

    /// <summary>
    /// Tests that ConvertBack returns a valid double when given DateTime.Now.
    /// </summary>
    [Fact]
    public void ConvertBack_NowDate_ReturnsDouble()
    {
        // Arrange
        var value = DateTime.Now;

        // Act
        var result = _converter.ConvertBack(value, typeof(double), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.IsType<double>(result);
        Assert.False(double.IsNaN((double)result!));
    }

    #endregion

    #region Round Trip Tests

    /// <summary>
    /// Tests that Convert and ConvertBack operations round-trip correctly for DateTime values.
    /// </summary>
    [Fact]
    public void Convert_ConvertBack_RoundTrip()
    {
        // Arrange
        var originalDate = new DateTime(2022, 6, 15, 12, 0, 0);
        var doubleValue = OxyPlot.Axes.DateTimeAxis.ToDouble(originalDate);

        // Act
        var converted = _converter.Convert(doubleValue, typeof(DateTime), null, CultureInfo.InvariantCulture);
        var roundTripped = _converter.ConvertBack(converted, typeof(double), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.Equal(doubleValue, (double)roundTripped!, 6);
    }

    /// <summary>
    /// Tests that ConvertBack and Convert operations round-trip correctly for DateTime values.
    /// </summary>
    [Fact]
    public void ConvertBack_Convert_RoundTrip()
    {
        // Arrange
        var originalDate = new DateTime(2023, 3, 20, 8, 30, 0);

        // Act
        var converted = _converter.ConvertBack(originalDate, typeof(double), null, CultureInfo.InvariantCulture);
        var roundTripped = _converter.Convert(converted, typeof(DateTime), null, CultureInfo.InvariantCulture);

        // Assert
        Assert.IsType<DateTime>(roundTripped);
        var resultDate = (DateTime)roundTripped!;
        Assert.Equal(originalDate.Year, resultDate.Year);
        Assert.Equal(originalDate.Month, resultDate.Month);
        Assert.Equal(originalDate.Day, resultDate.Day);
    }

    #endregion
}
