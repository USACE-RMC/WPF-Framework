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
using System.Windows.Controls;
using Xunit;

namespace GenericControls.Tests.Converters;

/// <summary>
/// Unit tests for StringToDoubleConverter.
/// </summary>
public class StringToDoubleConverterTests
{
    private readonly StringToDoubleConverter _converter = new();

    [Fact]
    public void Convert_WithDouble_ReturnsString()
    {
        var result = _converter.Convert(123.45, typeof(string), null, CultureInfo.InvariantCulture);
        Assert.Equal("123.45", result);
    }

    [Fact]
    public void Convert_WithInteger_ReturnsString()
    {
        var result = _converter.Convert(42, typeof(string), null, CultureInfo.InvariantCulture);
        Assert.Equal("42", result);
    }

    [Fact]
    public void Convert_WithNull_ReturnsNull()
    {
        var result = _converter.Convert(null, typeof(string), null, CultureInfo.InvariantCulture);
        Assert.Null(result);
    }

    [Fact]
    public void Convert_WithZero_ReturnsZeroString()
    {
        var result = _converter.Convert(0.0, typeof(string), null, CultureInfo.InvariantCulture);
        Assert.Equal("0", result);
    }

    [Fact]
    public void Convert_WithNegativeNumber_ReturnsNegativeString()
    {
        var result = _converter.Convert(-99.5, typeof(string), null, CultureInfo.InvariantCulture);
        Assert.Equal("-99.5", result);
    }

    [Fact]
    public void ConvertBack_WithValidString_ReturnsDouble()
    {
        var result = _converter.ConvertBack("123.45", typeof(double), null, CultureInfo.InvariantCulture);
        Assert.Equal(123.45, result);
    }

    [Fact]
    public void ConvertBack_WithNull_ReturnsNull()
    {
        var result = _converter.ConvertBack(null, typeof(double), null, CultureInfo.InvariantCulture);
        Assert.Null(result);
    }

    [Fact]
    public void ConvertBack_WithInvalidString_ReturnsZero()
    {
        var result = _converter.ConvertBack("not a number", typeof(double), null, CultureInfo.InvariantCulture);
        Assert.Equal(0.0, result);
    }

    [Fact]
    public void ConvertBack_WithEmptyString_ReturnsZero()
    {
        var result = _converter.ConvertBack("", typeof(double), null, CultureInfo.InvariantCulture);
        Assert.Equal(0.0, result);
    }

    [Fact]
    public void ConvertBack_WithScientificNotation_ReturnsDouble()
    {
        var result = _converter.ConvertBack("1.5e2", typeof(double), null, CultureInfo.InvariantCulture);
        Assert.Equal(150.0, result);
    }

    [Fact]
    public void ConvertBack_WithNegativeNumber_ReturnsNegativeDouble()
    {
        var result = _converter.ConvertBack("-42.5", typeof(double), null, CultureInfo.InvariantCulture);
        Assert.Equal(-42.5, result);
    }
}

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

    [Fact]
    public void Convert_WithValueAtLowerBound_ReturnsTrue()
    {
        var converter = new InRangeConverter { LowerBound = 0, UpperBound = 100 };
        var result = converter.Convert(0.0, typeof(bool), null, CultureInfo.InvariantCulture);
        Assert.Equal(true, result);
    }

    [Fact]
    public void Convert_WithValueAtUpperBound_ReturnsTrue()
    {
        var converter = new InRangeConverter { LowerBound = 0, UpperBound = 100 };
        var result = converter.Convert(100.0, typeof(bool), null, CultureInfo.InvariantCulture);
        Assert.Equal(true, result);
    }

    [Fact]
    public void Convert_WithValueBelowRange_ReturnsFalse()
    {
        var converter = new InRangeConverter { LowerBound = 0, UpperBound = 100 };
        var result = converter.Convert(-1.0, typeof(bool), null, CultureInfo.InvariantCulture);
        Assert.Equal(false, result);
    }

    [Fact]
    public void Convert_WithValueAboveRange_ReturnsFalse()
    {
        var converter = new InRangeConverter { LowerBound = 0, UpperBound = 100 };
        var result = converter.Convert(101.0, typeof(bool), null, CultureInfo.InvariantCulture);
        Assert.Equal(false, result);
    }

    [Fact]
    public void Convert_WithNull_ReturnsFalse()
    {
        var converter = new InRangeConverter { LowerBound = 0, UpperBound = 100 };
        var result = converter.Convert(null, typeof(bool), null, CultureInfo.InvariantCulture);
        Assert.Equal(false, result);
    }

    [Fact]
    public void Convert_WithInvalidType_ReturnsFalse()
    {
        var converter = new InRangeConverter { LowerBound = 0, UpperBound = 100 };
        var result = converter.Convert("not a number", typeof(bool), null, CultureInfo.InvariantCulture);
        Assert.Equal(false, result);
    }

    [Fact]
    public void Convert_WithDefaultBounds_ValueInRange_ReturnsTrue()
    {
        var converter = new InRangeConverter();
        var result = converter.Convert(0.0, typeof(bool), null, CultureInfo.InvariantCulture);
        Assert.Equal(true, result);
    }

    [Fact]
    public void Convert_WithNegativeBounds_ReturnsCorrectResult()
    {
        var converter = new InRangeConverter { LowerBound = -100, UpperBound = -10 };
        var result = converter.Convert(-50.0, typeof(bool), null, CultureInfo.InvariantCulture);
        Assert.Equal(true, result);
    }

    [Fact]
    public void Convert_WithStringNumber_ReturnsTrue()
    {
        var converter = new InRangeConverter { LowerBound = 0, UpperBound = 100 };
        var result = converter.Convert("50", typeof(bool), null, CultureInfo.InvariantCulture);
        Assert.Equal(true, result);
    }

    [Fact]
    public void ConvertBack_ThrowsNotImplementedException()
    {
        var converter = new InRangeConverter();
        Assert.Throws<NotImplementedException>(() =>
            converter.ConvertBack(true, typeof(double), null, CultureInfo.InvariantCulture));
    }
}

/// <summary>
/// Unit tests for DoubleToThicknessConverter.
/// </summary>
public class DoubleToThicknessConverterTests
{
    [Fact]
    public void Convert_WithDouble_AllSidesEnabled_ReturnsUniformThickness()
    {
        var converter = new DoubleToThicknessConverter
        {
            IsLeft = true,
            IsTop = true,
            IsRight = true,
            IsBottom = true
        };
        var result = (Thickness)converter.Convert(10.0, typeof(Thickness), null, CultureInfo.InvariantCulture)!;
        Assert.Equal(new Thickness(10, 10, 10, 10), result);
    }

    [Fact]
    public void Convert_WithDouble_OnlyLeftEnabled_ReturnsLeftOnlyThickness()
    {
        var converter = new DoubleToThicknessConverter
        {
            IsLeft = true,
            IsTop = false,
            IsRight = false,
            IsBottom = false,
            Top = 5,
            Right = 5,
            Bottom = 5
        };
        var result = (Thickness)converter.Convert(10.0, typeof(Thickness), null, CultureInfo.InvariantCulture)!;
        Assert.Equal(10, result.Left);
        Assert.Equal(5, result.Top);
        Assert.Equal(5, result.Right);
        Assert.Equal(5, result.Bottom);
    }

    [Fact]
    public void Convert_WithNull_ReturnsNull()
    {
        var converter = new DoubleToThicknessConverter();
        var result = converter.Convert(null, typeof(Thickness), null, CultureInfo.InvariantCulture);
        Assert.Null(result);
    }

    [Fact]
    public void Convert_WithInvalidString_ReturnsNull()
    {
        var converter = new DoubleToThicknessConverter();
        var result = converter.Convert("invalid", typeof(Thickness), null, CultureInfo.InvariantCulture);
        Assert.Null(result);
    }

    [Fact]
    public void Convert_WithStringNumber_ReturnsThickness()
    {
        var converter = new DoubleToThicknessConverter();
        var result = (Thickness)converter.Convert("15", typeof(Thickness), null, CultureInfo.InvariantCulture)!;
        Assert.Equal(new Thickness(15, 15, 15, 15), result);
    }

    [Fact]
    public void ConvertBack_WithThickness_LeftEnabled_ReturnsLeft()
    {
        var converter = new DoubleToThicknessConverter { IsLeft = true };
        var thickness = new Thickness(10, 20, 30, 40);
        var result = converter.ConvertBack(thickness, typeof(double), null, CultureInfo.InvariantCulture);
        Assert.Equal(10.0, result);
    }

    [Fact]
    public void ConvertBack_WithThickness_RightEnabled_ReturnsRight()
    {
        var converter = new DoubleToThicknessConverter
        {
            IsLeft = false,
            IsRight = true
        };
        var thickness = new Thickness(10, 20, 30, 40);
        var result = converter.ConvertBack(thickness, typeof(double), null, CultureInfo.InvariantCulture);
        Assert.Equal(30.0, result);
    }

    [Fact]
    public void ConvertBack_WithThickness_TopEnabled_ReturnsTop()
    {
        var converter = new DoubleToThicknessConverter
        {
            IsLeft = false,
            IsRight = false,
            IsTop = true
        };
        var thickness = new Thickness(10, 20, 30, 40);
        var result = converter.ConvertBack(thickness, typeof(double), null, CultureInfo.InvariantCulture);
        Assert.Equal(20.0, result);
    }

    [Fact]
    public void ConvertBack_WithThickness_BottomEnabled_ReturnsBottom()
    {
        var converter = new DoubleToThicknessConverter
        {
            IsLeft = false,
            IsRight = false,
            IsTop = false,
            IsBottom = true
        };
        var thickness = new Thickness(10, 20, 30, 40);
        var result = converter.ConvertBack(thickness, typeof(double), null, CultureInfo.InvariantCulture);
        Assert.Equal(40.0, result);
    }

    [Fact]
    public void ConvertBack_WithThickness_NoSidesEnabled_ReturnsAverage()
    {
        var converter = new DoubleToThicknessConverter
        {
            IsLeft = false,
            IsRight = false,
            IsTop = false,
            IsBottom = false
        };
        var thickness = new Thickness(10, 20, 30, 40);
        var result = converter.ConvertBack(thickness, typeof(double), null, CultureInfo.InvariantCulture);
        Assert.Equal(25.0, result); // (10 + 20 + 30 + 40) / 4
    }

    [Fact]
    public void ConvertBack_WithNull_ReturnsNull()
    {
        var converter = new DoubleToThicknessConverter();
        var result = converter.ConvertBack(null, typeof(double), null, CultureInfo.InvariantCulture);
        Assert.Null(result);
    }
}

/// <summary>
/// Unit tests for ThicknessToDoubleConverter.
/// </summary>
public class ThicknessToDoubleConverterTests
{
    private readonly ThicknessToDoubleConverter _converter = new();

    [Fact]
    public void Convert_WithUniformThickness_ReturnsValue()
    {
        var thickness = new Thickness(10);
        var result = _converter.Convert(thickness, typeof(double), null, CultureInfo.InvariantCulture);
        Assert.Equal(10.0, result);
    }

    [Fact]
    public void Convert_WithNonUniformThickness_ReturnsAverage()
    {
        var thickness = new Thickness(10, 20, 30, 40);
        var result = _converter.Convert(thickness, typeof(double), null, CultureInfo.InvariantCulture);
        Assert.Equal(25.0, result); // (10 + 20 + 30 + 40) / 4
    }

    [Fact]
    public void Convert_WithNull_ReturnsNull()
    {
        var result = _converter.Convert(null, typeof(double), null, CultureInfo.InvariantCulture);
        Assert.Null(result);
    }

    [Fact]
    public void Convert_WithZeroThickness_ReturnsZero()
    {
        var thickness = new Thickness(0);
        var result = _converter.Convert(thickness, typeof(double), null, CultureInfo.InvariantCulture);
        Assert.Equal(0.0, result);
    }

    [Fact]
    public void ConvertBack_WithDouble_ReturnsUniformThickness()
    {
        var result = (Thickness)_converter.ConvertBack(15.0, typeof(Thickness), null, CultureInfo.InvariantCulture)!;
        Assert.Equal(new Thickness(15), result);
    }

    [Fact]
    public void ConvertBack_WithNull_ReturnsNull()
    {
        var result = _converter.ConvertBack(null, typeof(Thickness), null, CultureInfo.InvariantCulture);
        Assert.Null(result);
    }

    [Fact]
    public void ConvertBack_WithInvalidString_ReturnsNull()
    {
        var result = _converter.ConvertBack("invalid", typeof(Thickness), null, CultureInfo.InvariantCulture);
        Assert.Null(result);
    }

    [Fact]
    public void ConvertBack_WithStringNumber_ReturnsThickness()
    {
        var result = (Thickness)_converter.ConvertBack("20", typeof(Thickness), null, CultureInfo.InvariantCulture)!;
        Assert.Equal(new Thickness(20), result);
    }
}

/// <summary>
/// Unit tests for DoubleToDataGridLengthConverter.
/// </summary>
public class DoubleToDataGridLengthConverterTests
{
    private readonly DoubleToDataGridLengthConverter _converter = new();

    [Fact]
    public void Convert_DoubleToDataGridLength_ReturnsDataGridLength()
    {
        var result = _converter.Convert(100.0, typeof(DataGridLength), null, CultureInfo.InvariantCulture);
        Assert.IsType<DataGridLength>(result);
        Assert.Equal(100.0, ((DataGridLength)result).Value);
    }

    [Fact]
    public void Convert_DataGridLengthToDouble_ReturnsDouble()
    {
        var dgLength = new DataGridLength(150.0);
        var result = _converter.Convert(dgLength, typeof(double), null, CultureInfo.InvariantCulture);
        Assert.Equal(150.0, result);
    }

    [Fact]
    public void Convert_NullToDataGridLength_ReturnsAuto()
    {
        var result = _converter.Convert(null, typeof(DataGridLength), null, CultureInfo.InvariantCulture);
        Assert.Equal(DataGridLength.Auto, result);
    }

    [Fact]
    public void Convert_InvalidTypeToDataGridLength_ReturnsAuto()
    {
        var result = _converter.Convert("invalid", typeof(DataGridLength), null, CultureInfo.InvariantCulture);
        Assert.Equal(DataGridLength.Auto, result);
    }

    [Fact]
    public void Convert_InvalidTypeToDouble_ReturnsNaN()
    {
        var result = _converter.Convert("invalid", typeof(double), null, CultureInfo.InvariantCulture);
        Assert.True(double.IsNaN((double)result!));
    }

    [Fact]
    public void Convert_WithOtherTargetType_ReturnsNull()
    {
        var result = _converter.Convert(100.0, typeof(string), null, CultureInfo.InvariantCulture);
        Assert.Null(result);
    }

    [Fact]
    public void ConvertBack_DoubleToDataGridLength_ReturnsDataGridLength()
    {
        var result = _converter.ConvertBack(75.0, typeof(DataGridLength), null, CultureInfo.InvariantCulture);
        Assert.IsType<DataGridLength>(result);
        Assert.Equal(75.0, ((DataGridLength)result).Value);
    }

    [Fact]
    public void ConvertBack_DataGridLengthToDouble_ReturnsDouble()
    {
        var dgLength = new DataGridLength(200.0);
        var result = _converter.ConvertBack(dgLength, typeof(double), null, CultureInfo.InvariantCulture);
        Assert.Equal(200.0, result);
    }

    [Fact]
    public void Convert_ZeroDouble_ReturnsDataGridLengthWithZero()
    {
        var result = _converter.Convert(0.0, typeof(DataGridLength), null, CultureInfo.InvariantCulture);
        Assert.IsType<DataGridLength>(result);
        Assert.Equal(0.0, ((DataGridLength)result).Value);
    }
}

/// <summary>
/// Unit tests for DoubleToGridLengthConverter.
/// </summary>
public class DoubleToGridLengthConverterTests
{
    private readonly DoubleToGridLengthConverter _converter = new();

    [Fact]
    public void Convert_DoubleToGridLength_ReturnsGridLength()
    {
        var result = _converter.Convert(100.0, typeof(GridLength), null, CultureInfo.InvariantCulture);
        Assert.IsType<GridLength>(result);
        Assert.Equal(100.0, ((GridLength)result).Value);
    }

    [Fact]
    public void Convert_GridLengthToDouble_ReturnsDouble()
    {
        var gridLength = new GridLength(150.0);
        var result = _converter.Convert(gridLength, typeof(double), null, CultureInfo.InvariantCulture);
        Assert.Equal(150.0, result);
    }

    [Fact]
    public void Convert_NullToGridLength_ReturnsAuto()
    {
        var result = _converter.Convert(null, typeof(GridLength), null, CultureInfo.InvariantCulture);
        Assert.Equal(GridLength.Auto, result);
    }

    [Fact]
    public void Convert_InvalidTypeToGridLength_ReturnsAuto()
    {
        var result = _converter.Convert("invalid", typeof(GridLength), null, CultureInfo.InvariantCulture);
        Assert.Equal(GridLength.Auto, result);
    }

    [Fact]
    public void Convert_InvalidTypeToDouble_ReturnsNaN()
    {
        var result = _converter.Convert("invalid", typeof(double), null, CultureInfo.InvariantCulture);
        Assert.True(double.IsNaN((double)result!));
    }

    [Fact]
    public void Convert_WithOtherTargetType_ReturnsNull()
    {
        var result = _converter.Convert(100.0, typeof(string), null, CultureInfo.InvariantCulture);
        Assert.Null(result);
    }

    [Fact]
    public void ConvertBack_DoubleToGridLength_ReturnsGridLength()
    {
        var result = _converter.ConvertBack(75.0, typeof(GridLength), null, CultureInfo.InvariantCulture);
        Assert.IsType<GridLength>(result);
        Assert.Equal(75.0, ((GridLength)result).Value);
    }

    [Fact]
    public void ConvertBack_GridLengthToDouble_ReturnsDouble()
    {
        var gridLength = new GridLength(200.0);
        var result = _converter.ConvertBack(gridLength, typeof(double), null, CultureInfo.InvariantCulture);
        Assert.Equal(200.0, result);
    }

    [Fact]
    public void Convert_ZeroDouble_ReturnsGridLengthWithZero()
    {
        var result = _converter.Convert(0.0, typeof(GridLength), null, CultureInfo.InvariantCulture);
        Assert.IsType<GridLength>(result);
        Assert.Equal(0.0, ((GridLength)result).Value);
    }
}

/// <summary>
/// Unit tests for DoubleToCornerRadiusConverter.
/// </summary>
public class DoubleToCornerRadiusConverterTests
{
    private readonly DoubleToCornerRadiusConverter _converter = new();

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

    [Fact]
    public void Convert_CornerRadiusToDouble_ReturnsBottomLeft()
    {
        var cornerRadius = new CornerRadius(5, 10, 15, 20);
        var result = _converter.Convert(cornerRadius, typeof(double), null, CultureInfo.InvariantCulture);
        Assert.Equal(20.0, result); // BottomLeft
    }

    [Fact]
    public void Convert_NullToCornerRadius_ReturnsEmptyCornerRadius()
    {
        var result = _converter.Convert(null, typeof(CornerRadius), null, CultureInfo.InvariantCulture);
        Assert.Equal(new CornerRadius(), result);
    }

    [Fact]
    public void Convert_InvalidTypeToCornerRadius_ReturnsEmptyCornerRadius()
    {
        var result = _converter.Convert("invalid", typeof(CornerRadius), null, CultureInfo.InvariantCulture);
        Assert.Equal(new CornerRadius(), result);
    }

    [Fact]
    public void Convert_InvalidTypeToDouble_ReturnsNaN()
    {
        var result = _converter.Convert("invalid", typeof(double), null, CultureInfo.InvariantCulture);
        Assert.True(double.IsNaN((double)result!));
    }

    [Fact]
    public void Convert_WithOtherTargetType_ReturnsNull()
    {
        var result = _converter.Convert(10.0, typeof(string), null, CultureInfo.InvariantCulture);
        Assert.Null(result);
    }

    [Fact]
    public void ConvertBack_DoubleToCornerRadius_ReturnsCornerRadius()
    {
        var result = _converter.ConvertBack(15.0, typeof(CornerRadius), null, CultureInfo.InvariantCulture);
        Assert.IsType<CornerRadius>(result);
        var cr = (CornerRadius)result;
        Assert.Equal(15.0, cr.TopLeft);
    }

    [Fact]
    public void ConvertBack_CornerRadiusToDouble_ReturnsDouble()
    {
        var cornerRadius = new CornerRadius(25);
        var result = _converter.ConvertBack(cornerRadius, typeof(double), null, CultureInfo.InvariantCulture);
        Assert.Equal(25.0, result);
    }

    [Fact]
    public void Convert_ZeroDouble_ReturnsZeroCornerRadius()
    {
        var result = _converter.Convert(0.0, typeof(CornerRadius), null, CultureInfo.InvariantCulture);
        Assert.IsType<CornerRadius>(result);
        var cr = (CornerRadius)result;
        Assert.Equal(0.0, cr.TopLeft);
    }
}

/// <summary>
/// Unit tests for DataGridWidthConverter.
/// </summary>
public class DataGridWidthConverterTests
{
    private readonly DataGridWidthConverter _converter = new();

    [Fact]
    public void Convert_WithValidWidth_ReturnsAdjustedWidth()
    {
        var scrollBarWidth = SystemParameters.VerticalScrollBarWidth;
        var result = _converter.Convert(500.0, typeof(double), null, CultureInfo.InvariantCulture);
        Assert.Equal(500.0 - scrollBarWidth, result);
    }

    [Fact]
    public void Convert_WithNull_ReturnsZero()
    {
        var result = _converter.Convert(null, typeof(double), null, CultureInfo.InvariantCulture);
        Assert.Equal(0.0, result);
    }

    [Fact]
    public void Convert_WithInvalidString_ReturnsZero()
    {
        var result = _converter.Convert("invalid", typeof(double), null, CultureInfo.InvariantCulture);
        Assert.Equal(0.0, result);
    }

    [Fact]
    public void Convert_WithStringNumber_ReturnsAdjustedWidth()
    {
        var scrollBarWidth = SystemParameters.VerticalScrollBarWidth;
        var result = _converter.Convert("300", typeof(double), null, CultureInfo.InvariantCulture);
        Assert.Equal(300.0 - scrollBarWidth, result);
    }

    [Fact]
    public void Convert_WithZero_ReturnsNegativeScrollBarWidth()
    {
        var scrollBarWidth = SystemParameters.VerticalScrollBarWidth;
        var result = _converter.Convert(0.0, typeof(double), null, CultureInfo.InvariantCulture);
        Assert.Equal(-scrollBarWidth, result);
    }

    [Fact]
    public void ConvertBack_ThrowsNotSupportedException()
    {
        Assert.Throws<NotSupportedException>(() =>
            _converter.ConvertBack(100.0, typeof(double), null, CultureInfo.InvariantCulture));
    }
}

/// <summary>
/// Unit tests for VectorToPointConverter.
/// </summary>
public class VectorToPointConverterTests
{
    private readonly VectorToPointConverter _converter = new();

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

    [Fact]
    public void Convert_WithNull_ReturnsNull()
    {
        var result = _converter.Convert(null, typeof(Point), null, CultureInfo.InvariantCulture);
        Assert.Null(result);
    }

    [Fact]
    public void Convert_WithInvalidType_ReturnsNull()
    {
        var result = _converter.Convert("invalid", typeof(Point), null, CultureInfo.InvariantCulture);
        Assert.Null(result);
    }

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

    [Fact]
    public void ConvertBack_WithNull_ReturnsNull()
    {
        var result = _converter.ConvertBack(null, typeof(Vector), null, CultureInfo.InvariantCulture);
        Assert.Null(result);
    }

    [Fact]
    public void ConvertBack_WithInvalidType_ReturnsNull()
    {
        var result = _converter.ConvertBack("invalid", typeof(Vector), null, CultureInfo.InvariantCulture);
        Assert.Null(result);
    }
}

/// <summary>
/// Unit tests for TimeTextConverter.
/// </summary>
public class TimeTextConverterTests
{
    [Fact]
    public void Convert_Hour12Hour_ReturnsFormattedHour()
    {
        var converter = new TimeTextConverter { TimeType = TimeTextConverter.TimeTarget.Hour, Is24Hour = false };
        var dateTime = new DateTime(2023, 1, 1, 14, 30, 45);
        var result = converter.Convert(dateTime, typeof(string), null, CultureInfo.InvariantCulture);
        Assert.Equal("02", result);
    }

    [Fact]
    public void Convert_Hour24Hour_ReturnsFormattedHour()
    {
        var converter = new TimeTextConverter { TimeType = TimeTextConverter.TimeTarget.Hour, Is24Hour = true };
        var dateTime = new DateTime(2023, 1, 1, 14, 30, 45);
        var result = converter.Convert(dateTime, typeof(string), null, CultureInfo.InvariantCulture);
        Assert.Equal("14", result);
    }

    [Fact]
    public void Convert_Minute_ReturnsFormattedMinute()
    {
        var converter = new TimeTextConverter { TimeType = TimeTextConverter.TimeTarget.Minute };
        var dateTime = new DateTime(2023, 1, 1, 14, 5, 45);
        var result = converter.Convert(dateTime, typeof(string), null, CultureInfo.InvariantCulture);
        Assert.Equal("05", result);
    }

    [Fact]
    public void Convert_Second_ReturnsFormattedSecond()
    {
        var converter = new TimeTextConverter { TimeType = TimeTextConverter.TimeTarget.Second };
        var dateTime = new DateTime(2023, 1, 1, 14, 30, 9);
        var result = converter.Convert(dateTime, typeof(string), null, CultureInfo.InvariantCulture);
        Assert.Equal("09", result);
    }

    [Fact]
    public void Convert_Meridian12Hour_ReturnsMeridian()
    {
        var converter = new TimeTextConverter { TimeType = TimeTextConverter.TimeTarget.Meridian, Is24Hour = false };
        var dateTime = new DateTime(2023, 1, 1, 14, 30, 45);
        var result = converter.Convert(dateTime, typeof(string), null, CultureInfo.InvariantCulture);
        Assert.Equal("PM", result);
    }

    [Fact]
    public void Convert_Meridian24Hour_ReturnsEmpty()
    {
        var converter = new TimeTextConverter { TimeType = TimeTextConverter.TimeTarget.Meridian, Is24Hour = true };
        var dateTime = new DateTime(2023, 1, 1, 14, 30, 45);
        var result = converter.Convert(dateTime, typeof(string), null, CultureInfo.InvariantCulture);
        Assert.Equal("", result);
    }

    [Fact]
    public void Convert_WithInvalidType_ReturnsEmptyString()
    {
        var converter = new TimeTextConverter { TimeType = TimeTextConverter.TimeTarget.Hour };
        var result = converter.Convert("not a datetime", typeof(string), null, CultureInfo.InvariantCulture);
        Assert.Equal("", result);
    }

    [Fact]
    public void Convert_MorningMeridian_ReturnsAM()
    {
        var converter = new TimeTextConverter { TimeType = TimeTextConverter.TimeTarget.Meridian, Is24Hour = false };
        var dateTime = new DateTime(2023, 1, 1, 9, 30, 0);
        var result = converter.Convert(dateTime, typeof(string), null, CultureInfo.InvariantCulture);
        Assert.Equal("AM", result);
    }

    [Fact]
    public void Convert_Midnight_ReturnsCorrectHour12Hour()
    {
        var converter = new TimeTextConverter { TimeType = TimeTextConverter.TimeTarget.Hour, Is24Hour = false };
        var dateTime = new DateTime(2023, 1, 1, 0, 0, 0);
        var result = converter.Convert(dateTime, typeof(string), null, CultureInfo.InvariantCulture);
        Assert.Equal("12", result);
    }

    [Fact]
    public void Convert_Midnight_ReturnsCorrectHour24Hour()
    {
        var converter = new TimeTextConverter { TimeType = TimeTextConverter.TimeTarget.Hour, Is24Hour = true };
        var dateTime = new DateTime(2023, 1, 1, 0, 0, 0);
        var result = converter.Convert(dateTime, typeof(string), null, CultureInfo.InvariantCulture);
        Assert.Equal("00", result);
    }

    [Fact]
    public void ConvertBack_ThrowsNotImplementedException()
    {
        var converter = new TimeTextConverter();
        Assert.Throws<NotImplementedException>(() =>
            converter.ConvertBack("12", typeof(DateTime), null, CultureInfo.InvariantCulture));
    }
}

/// <summary>
/// Unit tests for TabSizeConverter.
/// </summary>
public class TabSizeConverterTests
{
    [Fact]
    public void ConvertBack_ThrowsNotImplementedException()
    {
        var converter = new TabSizeConverter();
        Assert.Throws<NotImplementedException>(() =>
            converter.ConvertBack(100.0, new[] { typeof(object) }, null, CultureInfo.InvariantCulture));
    }

    // Note: Testing Convert requires a TabControl with ActualWidth set, which is difficult
    // to test in unit tests without a UI context. The converter calculation is:
    // width = tabControl.ActualWidth / tabControl.Items.Count
    // if width < 12, return 0
    // return width - (tabControl.Items.Count + 1)
}

/// <summary>
/// Unit tests for DoubleToNAConverter.
/// </summary>
public class DoubleToNAConverterTests
{
    private readonly DoubleToNAConverter _converter = new();

    [Fact]
    public void Convert_WithValidDouble_ReturnsDouble()
    {
        var result = _converter.Convert(123.45, typeof(string), null, CultureInfo.InvariantCulture);
        Assert.Equal(123.45, result);
    }

    [Fact]
    public void Convert_WithNull_ReturnsNA()
    {
        var result = _converter.Convert(null, typeof(string), null, CultureInfo.InvariantCulture);
        Assert.Equal("N/A", result);
    }

    [Fact]
    public void Convert_WithNaN_ReturnsNA()
    {
        var result = _converter.Convert(double.NaN, typeof(string), null, CultureInfo.InvariantCulture);
        Assert.Equal("N/A", result);
    }

    [Fact]
    public void Convert_WithPositiveInfinity_ReturnsPlusInfinity()
    {
        var result = _converter.Convert(double.PositiveInfinity, typeof(string), null, CultureInfo.InvariantCulture);
        Assert.Equal("+\u221E", result); // +infinity symbol
    }

    [Fact]
    public void Convert_WithNegativeInfinity_ReturnsMinusInfinity()
    {
        var result = _converter.Convert(double.NegativeInfinity, typeof(string), null, CultureInfo.InvariantCulture);
        Assert.Equal("-\u221E", result); // -infinity symbol
    }

    [Fact]
    public void Convert_WithInvalidType_ReturnsNA()
    {
        var result = _converter.Convert("not a double", typeof(string), null, CultureInfo.InvariantCulture);
        Assert.Equal("N/A", result);
    }

    [Fact]
    public void Convert_WithZero_ReturnsZero()
    {
        var result = _converter.Convert(0.0, typeof(string), null, CultureInfo.InvariantCulture);
        Assert.Equal(0.0, result);
    }

    [Fact]
    public void Convert_WithNegativeNumber_ReturnsNumber()
    {
        var result = _converter.Convert(-50.5, typeof(string), null, CultureInfo.InvariantCulture);
        Assert.Equal(-50.5, result);
    }

    [Fact]
    public void ConvertBack_WithNull_ReturnsNaN()
    {
        var result = _converter.ConvertBack(null, typeof(double), null, CultureInfo.InvariantCulture);
        Assert.True(double.IsNaN((double)result!));
    }

    [Fact]
    public void ConvertBack_WithNA_ReturnsNaN()
    {
        var result = _converter.ConvertBack("N/A", typeof(double), null, CultureInfo.InvariantCulture);
        Assert.True(double.IsNaN((double)result!));
    }

    [Fact]
    public void ConvertBack_WithNANoSlash_ReturnsNaN()
    {
        var result = _converter.ConvertBack("NA", typeof(double), null, CultureInfo.InvariantCulture);
        Assert.True(double.IsNaN((double)result!));
    }

    [Fact]
    public void ConvertBack_WithInfinitySymbol_ReturnsPositiveInfinity()
    {
        var result = _converter.ConvertBack("\u221E", typeof(double), null, CultureInfo.InvariantCulture);
        Assert.True(double.IsPositiveInfinity((double)result!));
    }

    [Fact]
    public void ConvertBack_WithNegativeInfinitySymbol_ReturnsNegativeInfinity()
    {
        var result = _converter.ConvertBack("-\u221E", typeof(double), null, CultureInfo.InvariantCulture);
        Assert.True(double.IsNegativeInfinity((double)result!));
    }

    [Fact]
    public void ConvertBack_WithValidNumber_ReturnsDouble()
    {
        var result = _converter.ConvertBack("42.5", typeof(double), null, CultureInfo.InvariantCulture);
        Assert.Equal(42.5, result);
    }

    [Fact]
    public void ConvertBack_WithInvalidString_ReturnsNaN()
    {
        var result = _converter.ConvertBack("invalid", typeof(double), null, CultureInfo.InvariantCulture);
        Assert.True(double.IsNaN((double)result!));
    }

    [Fact]
    public void ConvertBack_WithNonStringType_ReturnsValue()
    {
        var result = _converter.ConvertBack(123, typeof(double), null, CultureInfo.InvariantCulture);
        Assert.Equal(123, result);
    }

    [Fact]
    public void ConvertBack_WithInfText_ReturnsPositiveInfinity()
    {
        var result = _converter.ConvertBack("inf", typeof(double), null, CultureInfo.InvariantCulture);
        Assert.True(double.IsPositiveInfinity((double)result!));
    }

    [Fact]
    public void ConvertBack_WithNegativeInfText_ReturnsNegativeInfinity()
    {
        var result = _converter.ConvertBack("-inf", typeof(double), null, CultureInfo.InvariantCulture);
        Assert.True(double.IsNegativeInfinity((double)result!));
    }
}

/// <summary>
/// Unit tests for StringToNAConverter.
/// </summary>
public class StringToNAConverterTests
{
    private readonly StringToNAConverter _converter = new();

    [Fact]
    public void Convert_WithValidNumericString_ReturnsString()
    {
        var result = _converter.Convert("123.45", typeof(string), null, CultureInfo.InvariantCulture);
        Assert.Equal("123.45", result);
    }

    [Fact]
    public void Convert_WithNull_ReturnsNA()
    {
        var result = _converter.Convert(null, typeof(string), null, CultureInfo.InvariantCulture);
        Assert.Equal("N/A", result);
    }

    [Fact]
    public void Convert_WithNonString_ReturnsNA()
    {
        var result = _converter.Convert(123.45, typeof(string), null, CultureInfo.InvariantCulture);
        Assert.Equal("N/A", result);
    }

    [Fact]
    public void Convert_WithNaNString_ReturnsNA()
    {
        var result = _converter.Convert("NaN", typeof(string), null, CultureInfo.InvariantCulture);
        Assert.Equal("N/A", result);
    }

    [Fact]
    public void Convert_WithInfinityString_ReturnsNA()
    {
        var result = _converter.Convert("Infinity", typeof(string), null, CultureInfo.InvariantCulture);
        Assert.Equal("N/A", result);
    }

    [Fact]
    public void Convert_WithNegativeInfinityString_ReturnsNA()
    {
        var result = _converter.Convert("-Infinity", typeof(string), null, CultureInfo.InvariantCulture);
        Assert.Equal("N/A", result);
    }

    [Fact]
    public void Convert_WithInvalidNumericString_ReturnsNA()
    {
        var result = _converter.Convert("not a number", typeof(string), null, CultureInfo.InvariantCulture);
        Assert.Equal("N/A", result);
    }

    [Fact]
    public void Convert_WithEmptyString_ReturnsNA()
    {
        var result = _converter.Convert("", typeof(string), null, CultureInfo.InvariantCulture);
        Assert.Equal("N/A", result);
    }

    [Fact]
    public void Convert_WithZeroString_ReturnsZeroString()
    {
        var result = _converter.Convert("0", typeof(string), null, CultureInfo.InvariantCulture);
        Assert.Equal("0", result);
    }

    [Fact]
    public void Convert_WithNegativeNumberString_ReturnsString()
    {
        var result = _converter.Convert("-42.5", typeof(string), null, CultureInfo.InvariantCulture);
        Assert.Equal("-42.5", result);
    }

    [Fact]
    public void ConvertBack_ThrowsNotImplementedException()
    {
        Assert.Throws<NotImplementedException>(() =>
            _converter.ConvertBack("123", typeof(string), null, CultureInfo.InvariantCulture));
    }
}

/// <summary>
/// Unit tests for HorizontalAlignmentToTextAlignmentConverter.
/// </summary>
public class HorizontalAlignmentToTextAlignmentConverterTests
{
    private readonly HorizontalAlignmentToTextAlignmentConverter _converter = HorizontalAlignmentToTextAlignmentConverter.Instance;

    [Fact]
    public void Convert_Left_ReturnsTextAlignmentLeft()
    {
        var result = _converter.Convert(HorizontalAlignment.Left, typeof(TextAlignment), null, CultureInfo.InvariantCulture);
        Assert.Equal(TextAlignment.Left, result);
    }

    [Fact]
    public void Convert_Right_ReturnsTextAlignmentRight()
    {
        var result = _converter.Convert(HorizontalAlignment.Right, typeof(TextAlignment), null, CultureInfo.InvariantCulture);
        Assert.Equal(TextAlignment.Right, result);
    }

    [Fact]
    public void Convert_Center_ReturnsTextAlignmentCenter()
    {
        var result = _converter.Convert(HorizontalAlignment.Center, typeof(TextAlignment), null, CultureInfo.InvariantCulture);
        Assert.Equal(TextAlignment.Center, result);
    }

    [Fact]
    public void Convert_Stretch_ReturnsTextAlignmentCenter()
    {
        var result = _converter.Convert(HorizontalAlignment.Stretch, typeof(TextAlignment), null, CultureInfo.InvariantCulture);
        Assert.Equal(TextAlignment.Center, result);
    }

    [Fact]
    public void Convert_InvalidType_ReturnsTextAlignmentCenter()
    {
        var result = _converter.Convert("invalid", typeof(TextAlignment), null, CultureInfo.InvariantCulture);
        Assert.Equal(TextAlignment.Center, result);
    }

    [Fact]
    public void Convert_Null_ReturnsTextAlignmentCenter()
    {
        var result = _converter.Convert(null, typeof(TextAlignment), null, CultureInfo.InvariantCulture);
        Assert.Equal(TextAlignment.Center, result);
    }

    [Fact]
    public void ConvertBack_Left_ReturnsHorizontalAlignmentLeft()
    {
        var result = _converter.ConvertBack(TextAlignment.Left, typeof(HorizontalAlignment), null, CultureInfo.InvariantCulture);
        Assert.Equal(HorizontalAlignment.Left, result);
    }

    [Fact]
    public void ConvertBack_Right_ReturnsHorizontalAlignmentRight()
    {
        var result = _converter.ConvertBack(TextAlignment.Right, typeof(HorizontalAlignment), null, CultureInfo.InvariantCulture);
        Assert.Equal(HorizontalAlignment.Right, result);
    }

    [Fact]
    public void ConvertBack_Center_ReturnsHorizontalAlignmentCenter()
    {
        var result = _converter.ConvertBack(TextAlignment.Center, typeof(HorizontalAlignment), null, CultureInfo.InvariantCulture);
        Assert.Equal(HorizontalAlignment.Center, result);
    }

    [Fact]
    public void ConvertBack_Justify_ReturnsHorizontalAlignmentCenter()
    {
        var result = _converter.ConvertBack(TextAlignment.Justify, typeof(HorizontalAlignment), null, CultureInfo.InvariantCulture);
        Assert.Equal(HorizontalAlignment.Center, result);
    }

    [Fact]
    public void ConvertBack_InvalidType_ReturnsHorizontalAlignmentCenter()
    {
        var result = _converter.ConvertBack("invalid", typeof(HorizontalAlignment), null, CultureInfo.InvariantCulture);
        Assert.Equal(HorizontalAlignment.Center, result);
    }

    [Fact]
    public void ConvertBack_Null_ReturnsHorizontalAlignmentCenter()
    {
        var result = _converter.ConvertBack(null, typeof(HorizontalAlignment), null, CultureInfo.InvariantCulture);
        Assert.Equal(HorizontalAlignment.Center, result);
    }

    [Fact]
    public void Instance_IsSingleton()
    {
        Assert.Same(HorizontalAlignmentToTextAlignmentConverter.Instance, HorizontalAlignmentToTextAlignmentConverter.Instance);
    }
}

/// <summary>
/// Unit tests for AlwaysVisibleConverter.
/// </summary>
public class AlwaysVisibleConverterTests
{
    private readonly AlwaysVisibleConverter _converter = new();

    [Fact]
    public void Convert_WithAnyValue_ReturnsVisible()
    {
        var result = _converter.Convert(123, typeof(Visibility), null, CultureInfo.InvariantCulture);
        Assert.Equal(Visibility.Visible, result);
    }

    [Fact]
    public void Convert_WithNull_ReturnsVisible()
    {
        var result = _converter.Convert(null, typeof(Visibility), null, CultureInfo.InvariantCulture);
        Assert.Equal(Visibility.Visible, result);
    }

    [Fact]
    public void Convert_WithString_ReturnsVisible()
    {
        var result = _converter.Convert("any string", typeof(Visibility), null, CultureInfo.InvariantCulture);
        Assert.Equal(Visibility.Visible, result);
    }

    [Fact]
    public void Convert_WithBoolean_ReturnsVisible()
    {
        var result = _converter.Convert(false, typeof(Visibility), null, CultureInfo.InvariantCulture);
        Assert.Equal(Visibility.Visible, result);
    }

    [Fact]
    public void Convert_WithObject_ReturnsVisible()
    {
        var result = _converter.Convert(new object(), typeof(Visibility), null, CultureInfo.InvariantCulture);
        Assert.Equal(Visibility.Visible, result);
    }

    [Fact]
    public void ConvertBack_ThrowsNotImplementedException()
    {
        Assert.Throws<NotImplementedException>(() =>
            _converter.ConvertBack(Visibility.Visible, typeof(object), null, CultureInfo.InvariantCulture));
    }
}
