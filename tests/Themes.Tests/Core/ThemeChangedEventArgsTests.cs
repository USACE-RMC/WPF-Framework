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

using Xunit;
using Themes;
using System.Windows;

namespace Themes.Tests.Core
{
    /// <summary>
    /// Unit tests for the ThemeChangedEventArgs class, which provides data for theme change events.
    /// </summary>
    public class ThemeChangedEventArgsTests
    {
        #region Constructor Tests

        /// <summary>
        /// Verifies that the constructor sets the OldTheme property correctly.
        /// </summary>
        [Fact]
        public void Constructor_SetsOldTheme()
        {
            var args = new ThemeChangedEventArgs(Theme.Light, Theme.Dark, null!);

            Assert.Equal(Theme.Light, args.OldTheme);
        }

        /// <summary>
        /// Verifies that the constructor sets the NewTheme property correctly.
        /// </summary>
        [Fact]
        public void Constructor_SetsNewTheme()
        {
            var args = new ThemeChangedEventArgs(Theme.Light, Theme.Dark, null!);

            Assert.Equal(Theme.Dark, args.NewTheme);
        }

        /// <summary>
        /// Verifies that the constructor sets the ColorDictionary property correctly.
        /// </summary>
        [Fact]
        public void Constructor_SetsColorDictionary()
        {
            var dictionary = new ResourceDictionary();
            var args = new ThemeChangedEventArgs(Theme.Light, Theme.Dark, dictionary);

            Assert.Same(dictionary, args.ColorDictionary);
        }

        /// <summary>
        /// Verifies that the constructor accepts a null value for the ColorDictionary parameter.
        /// </summary>
        [Fact]
        public void Constructor_AcceptsNullColorDictionary()
        {
            var args = new ThemeChangedEventArgs(Theme.Light, Theme.Dark, null!);

            Assert.Null(args.ColorDictionary);
        }

        #endregion

        #region Property Tests

        /// <summary>
        /// Verifies that the properties preserve different combinations of theme changes.
        /// </summary>
        /// <param name="oldTheme">The old theme value.</param>
        /// <param name="newTheme">The new theme value.</param>
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

        /// <summary>
        /// Verifies that the properties allow the same value for both OldTheme and NewTheme.
        /// </summary>
        [Fact]
        public void Properties_AllowSameOldAndNewTheme()
        {
            var args = new ThemeChangedEventArgs(Theme.Blue, Theme.Blue, null!);

            Assert.Equal(Theme.Blue, args.OldTheme);
            Assert.Equal(Theme.Blue, args.NewTheme);
        }

        #endregion

        #region Inheritance Tests

        /// <summary>
        /// Verifies that ThemeChangedEventArgs inherits from EventArgs.
        /// </summary>
        [Fact]
        public void ThemeChangedEventArgs_InheritsFromEventArgs()
        {
            var args = new ThemeChangedEventArgs(Theme.Light, Theme.Dark, null!);

            Assert.IsAssignableFrom<EventArgs>(args);
        }

        #endregion
    }
}
