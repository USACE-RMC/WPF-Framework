using Xunit;
using Themes;
using System.Globalization;

namespace Themes.Tests.Converters
{
    public class TabSizeConverterTests
    {
        #region Convert Tests

        [Theory]
        [InlineData(100.0, 5, 20.0)]
        [InlineData(200.0, 4, 50.0)]
        [InlineData(150.0, 3, 50.0)]
        [InlineData(100.0, 1, 100.0)]
        public void Convert_DividesWidthByTabCount(double width, int tabCount, double expected)
        {
            var converter = new TabSizeConverter();

            var result = converter.Convert(
                new object[] { width, tabCount },
                typeof(double),
                null,
                CultureInfo.InvariantCulture);

            Assert.Equal(expected, (double)result);
        }

        [Fact]
        public void Convert_ZeroTabCount_ReturnsWidth()
        {
            var converter = new TabSizeConverter();

            var result = converter.Convert(
                new object[] { 100.0, 0 },
                typeof(double),
                null,
                CultureInfo.InvariantCulture);

            Assert.Equal(100.0, (double)result);
        }

        [Fact]
        public void Convert_NegativeTabCount_ReturnsWidth()
        {
            var converter = new TabSizeConverter();

            var result = converter.Convert(
                new object[] { 100.0, -1 },
                typeof(double),
                null,
                CultureInfo.InvariantCulture);

            Assert.Equal(100.0, (double)result);
        }

        [Fact]
        public void Convert_NullValues_ReturnsDependencyPropertyUnsetValue()
        {
            var converter = new TabSizeConverter();

            var result = converter.Convert(
                null!,
                typeof(double),
                null,
                CultureInfo.InvariantCulture);

            Assert.Equal(System.Windows.DependencyProperty.UnsetValue, result);
        }

        [Fact]
        public void Convert_EmptyValues_ReturnsDependencyPropertyUnsetValue()
        {
            var converter = new TabSizeConverter();

            var result = converter.Convert(
                Array.Empty<object>(),
                typeof(double),
                null,
                CultureInfo.InvariantCulture);

            Assert.Equal(System.Windows.DependencyProperty.UnsetValue, result);
        }

        [Fact]
        public void Convert_OnlyOneValue_ReturnsDependencyPropertyUnsetValue()
        {
            var converter = new TabSizeConverter();

            var result = converter.Convert(
                new object[] { 100.0 },
                typeof(double),
                null,
                CultureInfo.InvariantCulture);

            Assert.Equal(System.Windows.DependencyProperty.UnsetValue, result);
        }

        [Fact]
        public void Convert_NonNumericWidth_ReturnsDependencyPropertyUnsetValue()
        {
            var converter = new TabSizeConverter();

            var result = converter.Convert(
                new object[] { "invalid", 5 },
                typeof(double),
                null,
                CultureInfo.InvariantCulture);

            Assert.Equal(System.Windows.DependencyProperty.UnsetValue, result);
        }

        [Fact]
        public void Convert_NonNumericTabCount_ReturnsDependencyPropertyUnsetValue()
        {
            var converter = new TabSizeConverter();

            var result = converter.Convert(
                new object[] { 100.0, "invalid" },
                typeof(double),
                null,
                CultureInfo.InvariantCulture);

            Assert.Equal(System.Windows.DependencyProperty.UnsetValue, result);
        }

        #endregion

        #region ConvertBack Tests

        [Fact]
        public void ConvertBack_ThrowsNotImplementedException()
        {
            var converter = new TabSizeConverter();

            Assert.Throws<NotImplementedException>(() =>
                converter.ConvertBack(50.0, new[] { typeof(double), typeof(int) }, null, CultureInfo.InvariantCulture));
        }

        #endregion
    }
}
