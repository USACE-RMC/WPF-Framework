using Xunit;
using Themes;

namespace Themes.Tests.Core
{
    public class ThemeTests
    {
        #region Enum Value Tests

        [Fact]
        public void Theme_HasLightValue()
        {
            Assert.True(Enum.IsDefined(typeof(Theme), Theme.Light));
        }

        [Fact]
        public void Theme_HasBlueValue()
        {
            Assert.True(Enum.IsDefined(typeof(Theme), Theme.Blue));
        }

        [Fact]
        public void Theme_HasDarkValue()
        {
            Assert.True(Enum.IsDefined(typeof(Theme), Theme.Dark));
        }

        [Fact]
        public void Theme_HasThreeValues()
        {
            var values = Enum.GetValues(typeof(Theme));

            Assert.Equal(3, values.Length);
        }

        #endregion

        #region Enum Ordering Tests

        [Fact]
        public void Theme_Light_IsFirstValue()
        {
            Assert.Equal(0, (int)Theme.Light);
        }

        [Fact]
        public void Theme_Blue_IsSecondValue()
        {
            Assert.Equal(1, (int)Theme.Blue);
        }

        [Fact]
        public void Theme_Dark_IsThirdValue()
        {
            Assert.Equal(2, (int)Theme.Dark);
        }

        #endregion

        #region ToString Tests

        [Fact]
        public void Theme_Light_ToStringReturnsLight()
        {
            Assert.Equal("Light", Theme.Light.ToString());
        }

        [Fact]
        public void Theme_Blue_ToStringReturnsBlue()
        {
            Assert.Equal("Blue", Theme.Blue.ToString());
        }

        [Fact]
        public void Theme_Dark_ToStringReturnsDark()
        {
            Assert.Equal("Dark", Theme.Dark.ToString());
        }

        #endregion

        #region Parse Tests

        [Theory]
        [InlineData("Light", Theme.Light)]
        [InlineData("Blue", Theme.Blue)]
        [InlineData("Dark", Theme.Dark)]
        public void Theme_CanBeParsedFromString(string name, Theme expected)
        {
            var theme = Enum.Parse<Theme>(name);

            Assert.Equal(expected, theme);
        }

        [Theory]
        [InlineData("0", Theme.Light)]
        [InlineData("1", Theme.Blue)]
        [InlineData("2", Theme.Dark)]
        public void Theme_CanBeParsedFromNumericString(string value, Theme expected)
        {
            var theme = Enum.Parse<Theme>(value);

            Assert.Equal(expected, theme);
        }

        #endregion

        #region Equality Tests

        [Fact]
        public void Theme_SameValues_AreEqual()
        {
            Theme a = Theme.Dark;
            Theme b = Theme.Dark;

            Assert.Equal(a, b);
            Assert.True(a == b);
        }

        [Fact]
        public void Theme_DifferentValues_AreNotEqual()
        {
            Theme a = Theme.Light;
            Theme b = Theme.Dark;

            Assert.NotEqual(a, b);
            Assert.True(a != b);
        }

        #endregion

        #region Default Value Test

        [Fact]
        public void Theme_DefaultValue_IsLight()
        {
            Theme defaultTheme = default;

            Assert.Equal(Theme.Light, defaultTheme);
        }

        #endregion
    }
}
