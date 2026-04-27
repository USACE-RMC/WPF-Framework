using System.Globalization;
using Xunit;

namespace GenericControls.Tests.Converters;

/// <summary>
/// Unit tests for StringToNAConverter.
/// </summary>
public class StringToNAConverterTests
{
    /// <summary>
    /// Converter instance used for testing.
    /// </summary>
    private readonly StringToNAConverter _converter = new();

    /// <summary>
    /// Tests convert withvalidnumericstring returnsstring.
    /// </summary>
    [Fact]
    public void Convert_WithValidNumericString_ReturnsString()
    {
        var result = _converter.Convert("123.45", typeof(string), null, CultureInfo.InvariantCulture);
        Assert.Equal("123.45", result);
    }

    /// <summary>
    /// Tests convert withnull returnsna.
    /// </summary>
    [Fact]
    public void Convert_WithNull_ReturnsNA()
    {
        var result = _converter.Convert(null, typeof(string), null, CultureInfo.InvariantCulture);
        Assert.Equal("N/A", result);
    }

    /// <summary>
    /// Tests convert withnonstring returnsna.
    /// </summary>
    [Fact]
    public void Convert_WithNonString_ReturnsNA()
    {
        var result = _converter.Convert(123.45, typeof(string), null, CultureInfo.InvariantCulture);
        Assert.Equal("N/A", result);
    }

    /// <summary>
    /// Tests convert withnanstring returnsna.
    /// </summary>
    [Fact]
    public void Convert_WithNaNString_ReturnsNA()
    {
        var result = _converter.Convert("NaN", typeof(string), null, CultureInfo.InvariantCulture);
        Assert.Equal("N/A", result);
    }

    /// <summary>
    /// Tests convert withinfinitystring returnsna.
    /// </summary>
    [Fact]
    public void Convert_WithInfinityString_ReturnsNA()
    {
        var result = _converter.Convert("Infinity", typeof(string), null, CultureInfo.InvariantCulture);
        Assert.Equal("N/A", result);
    }

    /// <summary>
    /// Tests convert withnegativeinfinitystring returnsna.
    /// </summary>
    [Fact]
    public void Convert_WithNegativeInfinityString_ReturnsNA()
    {
        var result = _converter.Convert("-Infinity", typeof(string), null, CultureInfo.InvariantCulture);
        Assert.Equal("N/A", result);
    }

    /// <summary>
    /// Tests convert withinvalidnumericstring returnsna.
    /// </summary>
    [Fact]
    public void Convert_WithInvalidNumericString_ReturnsNA()
    {
        var result = _converter.Convert("not a number", typeof(string), null, CultureInfo.InvariantCulture);
        Assert.Equal("N/A", result);
    }

    /// <summary>
    /// Tests convert withemptystring returnsna.
    /// </summary>
    [Fact]
    public void Convert_WithEmptyString_ReturnsNA()
    {
        var result = _converter.Convert("", typeof(string), null, CultureInfo.InvariantCulture);
        Assert.Equal("N/A", result);
    }

    /// <summary>
    /// Tests convert withzerostring returnszerostring.
    /// </summary>
    [Fact]
    public void Convert_WithZeroString_ReturnsZeroString()
    {
        var result = _converter.Convert("0", typeof(string), null, CultureInfo.InvariantCulture);
        Assert.Equal("0", result);
    }

    /// <summary>
    /// Tests convert withnegativenumberstring returnsstring.
    /// </summary>
    [Fact]
    public void Convert_WithNegativeNumberString_ReturnsString()
    {
        var result = _converter.Convert("-42.5", typeof(string), null, CultureInfo.InvariantCulture);
        Assert.Equal("-42.5", result);
    }

    /// <summary>
    /// Tests convertback throwsnotimplementedexception.
    /// </summary>
    [Fact]
    public void ConvertBack_ThrowsNotImplementedException()
    {
        Assert.Throws<NotImplementedException>(() =>
            _converter.ConvertBack("123", typeof(string), null, CultureInfo.InvariantCulture));
    }
}
