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
using System.Windows.Media;
using OxyPlot;
using Xunit;

namespace OxyPlotControls.Tests.Converters;

/// <summary>
/// Common tests for all color converters to ensure consistent behavior.
/// Validates that all converters handle automatic colors and error conditions consistently.
/// </summary>
public class ColorConverterCommonBehaviorTests
{
    /// <summary>
    /// Tests that all converters handle OxyPlot's automatic color (ARGB 0,0,0,1) consistently.
    /// </summary>
    [Fact]
    public void AllConverters_HandleAutomaticColor_Consistently()
    {
        // Arrange - OxyPlot uses ARGB(0,0,0,1) to represent automatic color
        var automaticColor = Color.FromArgb(0, 0, 0, 1);
        object[] values = { automaticColor, null! };

        // Test LineSeriesColorConverter
        var lineConverter = new LineSeriesColorConverter();
        var lineResult = lineConverter.Convert(values, typeof(SolidColorBrush), null!, CultureInfo.InvariantCulture);
        Assert.IsType<SolidColorBrush>(lineResult);

        // Test AreaSeriesColor2Converter
        var areaColor2Converter = new AreaSeriesColor2Converter();
        var areaColor2Result = areaColor2Converter.Convert(values, typeof(SolidColorBrush), null!, CultureInfo.InvariantCulture);
        Assert.IsType<SolidColorBrush>(areaColor2Result);

        // Test AreaSeriesFillConverter
        var areaFillConverter = new AreaSeriesFillConverter();
        var areaFillResult = areaFillConverter.Convert(values, typeof(SolidColorBrush), null!, CultureInfo.InvariantCulture);
        Assert.IsType<SolidColorBrush>(areaFillResult);

        // Test LineSeriesMarkerFillConverter
        var markerFillConverter = new LineSeriesMarkerFillConverter();
        var markerFillResult = markerFillConverter.Convert(values, typeof(SolidColorBrush), null!, CultureInfo.InvariantCulture);
        Assert.IsType<SolidColorBrush>(markerFillResult);

        // Test BarSeriesFillConverter
        var barConverter = new BarSeriesFillConverter();
        var barResult = barConverter.Convert(values, typeof(SolidColorBrush), null!, CultureInfo.InvariantCulture);
        Assert.IsType<SolidColorBrush>(barResult);

        // Test BoxPlotSeriesFillConverter
        var boxConverter = new BoxPlotSeriesFillConverter();
        var boxResult = boxConverter.Convert(values, typeof(SolidColorBrush), null!, CultureInfo.InvariantCulture);
        Assert.IsType<SolidColorBrush>(boxResult);

        // Test ScatterSeriesMarkerFillConverter
        var scatterFillConverter = new ScatterSeriesMarkerFillConverter();
        var scatterFillResult = scatterFillConverter.Convert(values, typeof(SolidColorBrush), null!, CultureInfo.InvariantCulture);
        Assert.IsType<SolidColorBrush>(scatterFillResult);
    }

    /// <summary>
    /// Tests that all converters return null when given a null color value.
    /// </summary>
    [Fact]
    public void AllConverters_ReturnNull_ForNullColor()
    {
        // Arrange
        object[] values = { null!, null! };

        // Test all converters
        Assert.Null(new LineSeriesColorConverter().Convert(values, typeof(SolidColorBrush), null!, CultureInfo.InvariantCulture));
        Assert.Null(new AreaSeriesColor2Converter().Convert(values, typeof(SolidColorBrush), null!, CultureInfo.InvariantCulture));
        Assert.Null(new AreaSeriesFillConverter().Convert(values, typeof(SolidColorBrush), null!, CultureInfo.InvariantCulture));
        Assert.Null(new LineSeriesMarkerFillConverter().Convert(values, typeof(SolidColorBrush), null!, CultureInfo.InvariantCulture));
        Assert.Null(new LineSeriesMarkerStrokeConverter().Convert(values, typeof(SolidColorBrush), null!, CultureInfo.InvariantCulture));
        Assert.Null(new BarSeriesFillConverter().Convert(values, typeof(SolidColorBrush), null!, CultureInfo.InvariantCulture));
        Assert.Null(new BoxPlotSeriesFillConverter().Convert(values, typeof(SolidColorBrush), null!, CultureInfo.InvariantCulture));
        Assert.Null(new ScatterSeriesMarkerFillConverter().Convert(values, typeof(SolidColorBrush), null!, CultureInfo.InvariantCulture));
        Assert.Null(new ScatterSeriesMarkerStrokeConverter().Convert(values, typeof(SolidColorBrush), null!, CultureInfo.InvariantCulture));
    }

    /// <summary>
    /// Tests that all converters return null when given an incorrect type.
    /// </summary>
    [Fact]
    public void AllConverters_ReturnNull_ForWrongType()
    {
        // Arrange
        object[] values = { "not a color", null! };

        // Test all converters
        Assert.Null(new LineSeriesColorConverter().Convert(values, typeof(SolidColorBrush), null!, CultureInfo.InvariantCulture));
        Assert.Null(new AreaSeriesColor2Converter().Convert(values, typeof(SolidColorBrush), null!, CultureInfo.InvariantCulture));
        Assert.Null(new AreaSeriesFillConverter().Convert(values, typeof(SolidColorBrush), null!, CultureInfo.InvariantCulture));
        Assert.Null(new LineSeriesMarkerFillConverter().Convert(values, typeof(SolidColorBrush), null!, CultureInfo.InvariantCulture));
        Assert.Null(new LineSeriesMarkerStrokeConverter().Convert(values, typeof(SolidColorBrush), null!, CultureInfo.InvariantCulture));
        Assert.Null(new BarSeriesFillConverter().Convert(values, typeof(SolidColorBrush), null!, CultureInfo.InvariantCulture));
        Assert.Null(new BoxPlotSeriesFillConverter().Convert(values, typeof(SolidColorBrush), null!, CultureInfo.InvariantCulture));
        Assert.Null(new ScatterSeriesMarkerFillConverter().Convert(values, typeof(SolidColorBrush), null!, CultureInfo.InvariantCulture));
        Assert.Null(new ScatterSeriesMarkerStrokeConverter().Convert(values, typeof(SolidColorBrush), null!, CultureInfo.InvariantCulture));
    }
}
