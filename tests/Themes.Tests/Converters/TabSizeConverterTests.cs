using Xunit;
using Themes;
using System.Globalization;

namespace Themes.Tests.Converters
{
    /// <summary>
    /// Tests for TabSizeConverter.
    /// Note: This converter requires a WPF TabControl as input, making it difficult to test
    /// in isolation. These tests verify basic behavior like ConvertBack throwing.
    /// Full integration testing would require WPF context.
    /// </summary>
    public class TabSizeConverterTests
    {
        #region ConvertBack Tests

        [Fact]
        public void ConvertBack_ThrowsNotImplementedException()
        {
            var converter = new TabSizeConverter();

            Assert.Throws<NotImplementedException>(() =>
                converter.ConvertBack(50.0, new[] { typeof(object), typeof(double) }, null, CultureInfo.InvariantCulture));
        }

        #endregion

        #region Convert Edge Cases

        [Fact]
        public void Convert_NullValues_ThrowsNullReferenceException()
        {
            var converter = new TabSizeConverter();

            // The converter casts values[0] to TabControl, so null values will throw
            Assert.Throws<NullReferenceException>(() =>
                converter.Convert(null!, typeof(double), null, CultureInfo.InvariantCulture));
        }

        [Fact]
        public void Convert_EmptyValues_ThrowsIndexOutOfRangeException()
        {
            var converter = new TabSizeConverter();

            // The converter accesses values[0], so empty array throws IndexOutOfRange
            Assert.Throws<IndexOutOfRangeException>(() =>
                converter.Convert(Array.Empty<object>(), typeof(double), null, CultureInfo.InvariantCulture));
        }

        [Fact]
        public void Convert_NonTabControlValue_ThrowsInvalidCastException()
        {
            var converter = new TabSizeConverter();

            // The converter casts values[0] to TabControl
            Assert.Throws<InvalidCastException>(() =>
                converter.Convert(new object[] { 100.0, 5 }, typeof(double), null, CultureInfo.InvariantCulture));
        }

        #endregion
    }
}
