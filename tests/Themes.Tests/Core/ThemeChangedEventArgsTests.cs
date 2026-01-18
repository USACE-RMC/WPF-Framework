using Xunit;
using Themes;
using System.Windows;

namespace Themes.Tests.Core
{
    public class ThemeChangedEventArgsTests
    {
        #region Constructor Tests

        [Fact]
        public void Constructor_SetsOldTheme()
        {
            var args = new ThemeChangedEventArgs(Theme.Light, Theme.Dark, null!);

            Assert.Equal(Theme.Light, args.OldTheme);
        }

        [Fact]
        public void Constructor_SetsNewTheme()
        {
            var args = new ThemeChangedEventArgs(Theme.Light, Theme.Dark, null!);

            Assert.Equal(Theme.Dark, args.NewTheme);
        }

        [Fact]
        public void Constructor_SetsColorDictionary()
        {
            var dictionary = new ResourceDictionary();
            var args = new ThemeChangedEventArgs(Theme.Light, Theme.Dark, dictionary);

            Assert.Same(dictionary, args.ColorDictionary);
        }

        [Fact]
        public void Constructor_AcceptsNullColorDictionary()
        {
            var args = new ThemeChangedEventArgs(Theme.Light, Theme.Dark, null!);

            Assert.Null(args.ColorDictionary);
        }

        #endregion

        #region Property Tests

        [Theory]
        [InlineData(Theme.Light, Theme.Dark)]
        [InlineData(Theme.Dark, Theme.Light)]
        [InlineData(Theme.Blue, Theme.Dark)]
        [InlineData(Theme.Light, Theme.Blue)]
        public void Properties_PreserveDifferentThemeCombinations(Theme oldTheme, Theme newTheme)
        {
            var args = new ThemeChangedEventArgs(oldTheme, newTheme, null!);

            Assert.Equal(oldTheme, args.OldTheme);
            Assert.Equal(newTheme, args.NewTheme);
        }

        [Fact]
        public void Properties_AllowSameOldAndNewTheme()
        {
            var args = new ThemeChangedEventArgs(Theme.Blue, Theme.Blue, null!);

            Assert.Equal(Theme.Blue, args.OldTheme);
            Assert.Equal(Theme.Blue, args.NewTheme);
        }

        #endregion

        #region Inheritance Tests

        [Fact]
        public void ThemeChangedEventArgs_InheritsFromEventArgs()
        {
            var args = new ThemeChangedEventArgs(Theme.Light, Theme.Dark, null!);

            Assert.IsAssignableFrom<EventArgs>(args);
        }

        #endregion
    }
}
