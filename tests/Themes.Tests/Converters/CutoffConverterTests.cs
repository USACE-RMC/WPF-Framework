using Xunit;
using Themes;
using System.Windows;
using System.Globalization;

namespace Themes.Tests.Converters
{
    public class CutoffConverterTests
    {
        #region Property Tests

        [Fact]
        public void Cutoff_DefaultIsZero()
        {
            var converter = new CutoffConverter();

            Assert.Equal(0, converter.Cutoff);
        }

        [Theory]
        [InlineData(100)]
        [InlineData(50.5)]
        [InlineData(0)]
        [InlineData(-10)]
        public void Cutoff_SetAndGet(double value)
        {
            var converter = new CutoffConverter { Cutoff = value };

            Assert.Equal(value, converter.Cutoff);
        }

        #endregion

        #region Convert with Double Tests

        [Fact]
        public void Convert_DoubleBelowCutoff_ReturnsTrue()
        {
            var converter = new CutoffConverter { Cutoff = 100 };

            var result = converter.Convert(50.0, typeof(bool), null, CultureInfo.InvariantCulture);

            Assert.True((bool)result);
        }

        [Fact]
        public void Convert_DoubleEqualToCutoff_ReturnsFalse()
        {
            var converter = new CutoffConverter { Cutoff = 100 };

            var result = converter.Convert(100.0, typeof(bool), null, CultureInfo.InvariantCulture);

            Assert.False((bool)result);
        }

        [Fact]
        public void Convert_DoubleAboveCutoff_ReturnsFalse()
        {
            var converter = new CutoffConverter { Cutoff = 100 };

            var result = converter.Convert(150.0, typeof(bool), null, CultureInfo.InvariantCulture);

            Assert.False((bool)result);
        }

        [Fact]
        public void Convert_NegativeDouble_BelowCutoff()
        {
            var converter = new CutoffConverter { Cutoff = 0 };

            var result = converter.Convert(-10.0, typeof(bool), null, CultureInfo.InvariantCulture);

            Assert.True((bool)result);
        }

        [Fact]
        public void Convert_ZeroDouble_EqualToZeroCutoff()
        {
            var converter = new CutoffConverter { Cutoff = 0 };

            var result = converter.Convert(0.0, typeof(bool), null, CultureInfo.InvariantCulture);

            Assert.False((bool)result);
        }

        #endregion

        #region Convert with GridLength Tests

        [Fact]
        public void Convert_GridLengthBelowCutoff_ReturnsTrue()
        {
            var converter = new CutoffConverter { Cutoff = 100 };
            var gridLength = new GridLength(50);

            var result = converter.Convert(gridLength, typeof(bool), null, CultureInfo.InvariantCulture);

            Assert.True((bool)result);
        }

        [Fact]
        public void Convert_GridLengthEqualToCutoff_ReturnsFalse()
        {
            var converter = new CutoffConverter { Cutoff = 100 };
            var gridLength = new GridLength(100);

            var result = converter.Convert(gridLength, typeof(bool), null, CultureInfo.InvariantCulture);

            Assert.False((bool)result);
        }

        [Fact]
        public void Convert_GridLengthAboveCutoff_ReturnsFalse()
        {
            var converter = new CutoffConverter { Cutoff = 100 };
            var gridLength = new GridLength(200);

            var result = converter.Convert(gridLength, typeof(bool), null, CultureInfo.InvariantCulture);

            Assert.False((bool)result);
        }

        #endregion

        #region Convert with Null and Invalid Types Tests

        [Fact]
        public void Convert_NullValue_ReturnsFalse()
        {
            var converter = new CutoffConverter { Cutoff = 100 };

            var result = converter.Convert(null, typeof(bool), null, CultureInfo.InvariantCulture);

            Assert.False((bool)result);
        }

        [Fact]
        public void Convert_StringValue_ReturnsFalse()
        {
            var converter = new CutoffConverter { Cutoff = 100 };

            var result = converter.Convert("50", typeof(bool), null, CultureInfo.InvariantCulture);

            Assert.False((bool)result);
        }

        [Fact]
        public void Convert_IntValue_ReturnsFalse()
        {
            var converter = new CutoffConverter { Cutoff = 100 };

            var result = converter.Convert(50, typeof(bool), null, CultureInfo.InvariantCulture);

            Assert.False((bool)result);
        }

        [Fact]
        public void Convert_ObjectValue_ReturnsFalse()
        {
            var converter = new CutoffConverter { Cutoff = 100 };

            var result = converter.Convert(new object(), typeof(bool), null, CultureInfo.InvariantCulture);

            Assert.False((bool)result);
        }

        #endregion

        #region ConvertBack Tests

        [Fact]
        public void ConvertBack_ThrowsNotImplementedException()
        {
            var converter = new CutoffConverter();

            Assert.Throws<NotImplementedException>(() =>
                converter.ConvertBack(true, typeof(double), null, CultureInfo.InvariantCulture));
        }

        #endregion

        #region Edge Cases Tests

        [Fact]
        public void Convert_VerySmallDouble_BelowCutoff()
        {
            var converter = new CutoffConverter { Cutoff = 1 };

            var result = converter.Convert(0.0001, typeof(bool), null, CultureInfo.InvariantCulture);

            Assert.True((bool)result);
        }

        [Fact]
        public void Convert_VeryLargeDouble_AboveCutoff()
        {
            var converter = new CutoffConverter { Cutoff = 1000000 };

            var result = converter.Convert(999999.99, typeof(bool), null, CultureInfo.InvariantCulture);

            Assert.True((bool)result);
        }

        [Fact]
        public void Convert_DoubleMaxValue_AboveCutoff()
        {
            var converter = new CutoffConverter { Cutoff = 1000 };

            var result = converter.Convert(double.MaxValue, typeof(bool), null, CultureInfo.InvariantCulture);

            Assert.False((bool)result);
        }

        [Fact]
        public void Convert_DoubleMinValue_BelowCutoff()
        {
            var converter = new CutoffConverter { Cutoff = 1000 };

            var result = converter.Convert(double.MinValue, typeof(bool), null, CultureInfo.InvariantCulture);

            Assert.True((bool)result);
        }

        #endregion
    }
}
