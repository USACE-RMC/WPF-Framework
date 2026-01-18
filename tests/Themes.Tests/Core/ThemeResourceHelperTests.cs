using Xunit;
using Themes;

namespace Themes.Tests.Core
{
    public class ThemeResourceHelperTests
    {
        #region Constants Tests

        [Fact]
        public void ControlTemplatesUri_IsNotNullOrEmpty()
        {
            Assert.False(string.IsNullOrEmpty(ThemeResourceHelper.ControlTemplatesUri));
        }

        [Fact]
        public void ControlTemplatesUri_IsPackUri()
        {
            Assert.StartsWith("pack://application", ThemeResourceHelper.ControlTemplatesUri);
        }

        [Fact]
        public void LightColorsUri_IsNotNullOrEmpty()
        {
            Assert.False(string.IsNullOrEmpty(ThemeResourceHelper.LightColorsUri));
        }

        [Fact]
        public void BlueColorsUri_IsNotNullOrEmpty()
        {
            Assert.False(string.IsNullOrEmpty(ThemeResourceHelper.BlueColorsUri));
        }

        [Fact]
        public void DarkColorsUri_IsNotNullOrEmpty()
        {
            Assert.False(string.IsNullOrEmpty(ThemeResourceHelper.DarkColorsUri));
        }

        [Fact]
        public void AllColorUris_ArePackUris()
        {
            Assert.StartsWith("pack://application", ThemeResourceHelper.LightColorsUri);
            Assert.StartsWith("pack://application", ThemeResourceHelper.BlueColorsUri);
            Assert.StartsWith("pack://application", ThemeResourceHelper.DarkColorsUri);
        }

        [Fact]
        public void AllColorUris_ContainThemesComponent()
        {
            Assert.Contains("/Themes;component/", ThemeResourceHelper.LightColorsUri);
            Assert.Contains("/Themes;component/", ThemeResourceHelper.BlueColorsUri);
            Assert.Contains("/Themes;component/", ThemeResourceHelper.DarkColorsUri);
        }

        #endregion

        #region GetColorDictionaryUri Tests

        [Fact]
        public void GetColorDictionaryUri_Light_ReturnsLightColorsUri()
        {
            var uri = ThemeResourceHelper.GetColorDictionaryUri(Theme.Light);

            Assert.Equal(ThemeResourceHelper.LightColorsUri, uri);
        }

        [Fact]
        public void GetColorDictionaryUri_Blue_ReturnsBlueColorsUri()
        {
            var uri = ThemeResourceHelper.GetColorDictionaryUri(Theme.Blue);

            Assert.Equal(ThemeResourceHelper.BlueColorsUri, uri);
        }

        [Fact]
        public void GetColorDictionaryUri_Dark_ReturnsDarkColorsUri()
        {
            var uri = ThemeResourceHelper.GetColorDictionaryUri(Theme.Dark);

            Assert.Equal(ThemeResourceHelper.DarkColorsUri, uri);
        }

        [Fact]
        public void GetColorDictionaryUri_InvalidTheme_ThrowsArgumentOutOfRangeException()
        {
            var invalidTheme = (Theme)999;

            Assert.Throws<ArgumentOutOfRangeException>(() =>
                ThemeResourceHelper.GetColorDictionaryUri(invalidTheme));
        }

        [Theory]
        [InlineData(Theme.Light)]
        [InlineData(Theme.Blue)]
        [InlineData(Theme.Dark)]
        public void GetColorDictionaryUri_AllValidThemes_ReturnNonEmptyString(Theme theme)
        {
            var uri = ThemeResourceHelper.GetColorDictionaryUri(theme);

            Assert.False(string.IsNullOrEmpty(uri));
        }

        #endregion

        #region ParseTheme Tests

        [Fact]
        public void ParseTheme_Light_ReturnsLightTheme()
        {
            var theme = ThemeResourceHelper.ParseTheme("Light");

            Assert.Equal(Theme.Light, theme);
        }

        [Fact]
        public void ParseTheme_Blue_ReturnsBlueTheme()
        {
            var theme = ThemeResourceHelper.ParseTheme("Blue");

            Assert.Equal(Theme.Blue, theme);
        }

        [Fact]
        public void ParseTheme_Dark_ReturnsDarkTheme()
        {
            var theme = ThemeResourceHelper.ParseTheme("Dark");

            Assert.Equal(Theme.Dark, theme);
        }

        [Theory]
        [InlineData("light")]
        [InlineData("LIGHT")]
        [InlineData("Light")]
        [InlineData("LiGhT")]
        public void ParseTheme_IsCaseInsensitive(string themeName)
        {
            var theme = ThemeResourceHelper.ParseTheme(themeName);

            Assert.Equal(Theme.Light, theme);
        }

        [Fact]
        public void ParseTheme_Null_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                ThemeResourceHelper.ParseTheme(null!));
        }

        [Fact]
        public void ParseTheme_EmptyString_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                ThemeResourceHelper.ParseTheme(""));
        }

        [Fact]
        public void ParseTheme_WhitespaceOnly_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                ThemeResourceHelper.ParseTheme("   "));
        }

        [Fact]
        public void ParseTheme_InvalidThemeName_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() =>
                ThemeResourceHelper.ParseTheme("InvalidTheme"));
        }

        [Fact]
        public void ParseTheme_InvalidThemeName_ExceptionMessageContainsValidValues()
        {
            var ex = Assert.Throws<ArgumentException>(() =>
                ThemeResourceHelper.ParseTheme("InvalidTheme"));

            Assert.Contains("Light", ex.Message);
            Assert.Contains("Blue", ex.Message);
            Assert.Contains("Dark", ex.Message);
        }

        #endregion

        #region TryParseTheme Tests

        [Fact]
        public void TryParseTheme_Light_ReturnsTrue()
        {
            var result = ThemeResourceHelper.TryParseTheme("Light", out var theme);

            Assert.True(result);
            Assert.Equal(Theme.Light, theme);
        }

        [Fact]
        public void TryParseTheme_Blue_ReturnsTrue()
        {
            var result = ThemeResourceHelper.TryParseTheme("Blue", out var theme);

            Assert.True(result);
            Assert.Equal(Theme.Blue, theme);
        }

        [Fact]
        public void TryParseTheme_Dark_ReturnsTrue()
        {
            var result = ThemeResourceHelper.TryParseTheme("Dark", out var theme);

            Assert.True(result);
            Assert.Equal(Theme.Dark, theme);
        }

        [Theory]
        [InlineData("dark")]
        [InlineData("DARK")]
        [InlineData("Dark")]
        [InlineData("DaRk")]
        public void TryParseTheme_IsCaseInsensitive(string themeName)
        {
            var result = ThemeResourceHelper.TryParseTheme(themeName, out var theme);

            Assert.True(result);
            Assert.Equal(Theme.Dark, theme);
        }

        [Fact]
        public void TryParseTheme_Null_ReturnsFalse()
        {
            var result = ThemeResourceHelper.TryParseTheme(null!, out var theme);

            Assert.False(result);
            Assert.Equal(Theme.Light, theme); // Default
        }

        [Fact]
        public void TryParseTheme_EmptyString_ReturnsFalse()
        {
            var result = ThemeResourceHelper.TryParseTheme("", out var theme);

            Assert.False(result);
            Assert.Equal(Theme.Light, theme); // Default
        }

        [Fact]
        public void TryParseTheme_WhitespaceOnly_ReturnsFalse()
        {
            var result = ThemeResourceHelper.TryParseTheme("   ", out var theme);

            Assert.False(result);
            Assert.Equal(Theme.Light, theme); // Default
        }

        [Fact]
        public void TryParseTheme_InvalidThemeName_ReturnsFalse()
        {
            var result = ThemeResourceHelper.TryParseTheme("InvalidTheme", out var theme);

            Assert.False(result);
            Assert.Equal(Theme.Light, theme); // Default
        }

        [Fact]
        public void TryParseTheme_DoesNotThrow()
        {
            var exception = Record.Exception(() =>
            {
                ThemeResourceHelper.TryParseTheme(null!, out _);
                ThemeResourceHelper.TryParseTheme("", out _);
                ThemeResourceHelper.TryParseTheme("InvalidTheme", out _);
            });

            Assert.Null(exception);
        }

        #endregion
    }
}
