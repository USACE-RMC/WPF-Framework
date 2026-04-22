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
/// Unit tests for HorizontalAlignmentToTextAlignmentConverter.
/// </summary>
public class HorizontalAlignmentToTextAlignmentConverterTests
{
    /// <summary>
    /// Converter instance used for testing.
    /// </summary>
    private readonly HorizontalAlignmentToTextAlignmentConverter _converter = HorizontalAlignmentToTextAlignmentConverter.Instance;

    /// <summary>
    /// Tests convert left returnstextalignmentleft.
    /// </summary>
    [Fact]
    public void Convert_Left_ReturnsTextAlignmentLeft()
    {
        var result = _converter.Convert(HorizontalAlignment.Left, typeof(TextAlignment), null, CultureInfo.InvariantCulture);
        Assert.Equal(TextAlignment.Left, result);
    }

    /// <summary>
    /// Tests convert right returnstextalignmentright.
    /// </summary>
    [Fact]
    public void Convert_Right_ReturnsTextAlignmentRight()
    {
        var result = _converter.Convert(HorizontalAlignment.Right, typeof(TextAlignment), null, CultureInfo.InvariantCulture);
        Assert.Equal(TextAlignment.Right, result);
    }

    /// <summary>
    /// Tests convert center returnstextalignmentcenter.
    /// </summary>
    [Fact]
    public void Convert_Center_ReturnsTextAlignmentCenter()
    {
        var result = _converter.Convert(HorizontalAlignment.Center, typeof(TextAlignment), null, CultureInfo.InvariantCulture);
        Assert.Equal(TextAlignment.Center, result);
    }

    /// <summary>
    /// Tests convert stretch returnstextalignmentcenter.
    /// </summary>
    [Fact]
    public void Convert_Stretch_ReturnsTextAlignmentCenter()
    {
        var result = _converter.Convert(HorizontalAlignment.Stretch, typeof(TextAlignment), null, CultureInfo.InvariantCulture);
        Assert.Equal(TextAlignment.Center, result);
    }

    /// <summary>
    /// Tests convert invalidtype returnstextalignmentcenter.
    /// </summary>
    [Fact]
    public void Convert_InvalidType_ReturnsTextAlignmentCenter()
    {
        var result = _converter.Convert("invalid", typeof(TextAlignment), null, CultureInfo.InvariantCulture);
        Assert.Equal(TextAlignment.Center, result);
    }

    /// <summary>
    /// Tests convert null returnstextalignmentcenter.
    /// </summary>
    [Fact]
    public void Convert_Null_ReturnsTextAlignmentCenter()
    {
        var result = _converter.Convert(null, typeof(TextAlignment), null, CultureInfo.InvariantCulture);
        Assert.Equal(TextAlignment.Center, result);
    }

    /// <summary>
    /// Tests convertback left returnshorizontalalignmentleft.
    /// </summary>
    [Fact]
    public void ConvertBack_Left_ReturnsHorizontalAlignmentLeft()
    {
        var result = _converter.ConvertBack(TextAlignment.Left, typeof(HorizontalAlignment), null, CultureInfo.InvariantCulture);
        Assert.Equal(HorizontalAlignment.Left, result);
    }

    /// <summary>
    /// Tests convertback right returnshorizontalalignmentright.
    /// </summary>
    [Fact]
    public void ConvertBack_Right_ReturnsHorizontalAlignmentRight()
    {
        var result = _converter.ConvertBack(TextAlignment.Right, typeof(HorizontalAlignment), null, CultureInfo.InvariantCulture);
        Assert.Equal(HorizontalAlignment.Right, result);
    }

    /// <summary>
    /// Tests convertback center returnshorizontalalignmentcenter.
    /// </summary>
    [Fact]
    public void ConvertBack_Center_ReturnsHorizontalAlignmentCenter()
    {
        var result = _converter.ConvertBack(TextAlignment.Center, typeof(HorizontalAlignment), null, CultureInfo.InvariantCulture);
        Assert.Equal(HorizontalAlignment.Center, result);
    }

    /// <summary>
    /// Tests convertback justify returnshorizontalalignmentcenter.
    /// </summary>
    [Fact]
    public void ConvertBack_Justify_ReturnsHorizontalAlignmentCenter()
    {
        var result = _converter.ConvertBack(TextAlignment.Justify, typeof(HorizontalAlignment), null, CultureInfo.InvariantCulture);
        Assert.Equal(HorizontalAlignment.Center, result);
    }

    /// <summary>
    /// Tests convertback invalidtype returnshorizontalalignmentcenter.
    /// </summary>
    [Fact]
    public void ConvertBack_InvalidType_ReturnsHorizontalAlignmentCenter()
    {
        var result = _converter.ConvertBack("invalid", typeof(HorizontalAlignment), null, CultureInfo.InvariantCulture);
        Assert.Equal(HorizontalAlignment.Center, result);
    }

    /// <summary>
    /// Tests convertback null returnshorizontalalignmentcenter.
    /// </summary>
    [Fact]
    public void ConvertBack_Null_ReturnsHorizontalAlignmentCenter()
    {
        var result = _converter.ConvertBack(null, typeof(HorizontalAlignment), null, CultureInfo.InvariantCulture);
        Assert.Equal(HorizontalAlignment.Center, result);
    }

    /// <summary>
    /// Tests instance issingleton.
    /// </summary>
    [Fact]
    public void Instance_IsSingleton()
    {
        Assert.Same(HorizontalAlignmentToTextAlignmentConverter.Instance, HorizontalAlignmentToTextAlignmentConverter.Instance);
    }
}
