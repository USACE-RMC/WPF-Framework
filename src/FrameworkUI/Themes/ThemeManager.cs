/*
* NOTICE:
* The U.S. Army Corps of Engineers, Risk Management Center (USACE-RMC) makes no guarantees about
* the results, or appropriateness of outputs, obtained from this software.
*
* LIST OF CONDITIONS:
* Redistribution and use in source and binary forms, with or without modification, are permitted
* provided that the following conditions are met:
* ● Redistributions of source code must retain the above notice, this list of conditions, and the
* following disclaimer.
* ● Redistributions in binary form must reproduce the above notice, this list of conditions, and
* the following disclaimer in the documentation and/or other materials provided with the distribution.
* ● The names of the U.S. Government, the U.S. Army Corps of Engineers, the Institute for Water
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

// Copyright (c) USACE. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Windows;

namespace FrameworkUI
{
    /// <summary>
    /// A utility class for managing themes in FrameworkUI.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This class provides theme management for the ProjectUI library and bridges
    /// to the <see cref="Themes.ThemeService"/> for centralized theme management
    /// across all dependent libraries.
    /// </para>
    /// <para>
    /// When <see cref="SetTheme"/> is called, it:
    /// </para>
    /// <list type="number">
    ///     <item>Updates the <see cref="Themes.ThemeService"/> singleton (initializing it if needed)</item>
    ///     <item>Loads ProjectUI-specific theme resources</item>
    ///     <item>Raises the <see cref="ThemeChanged"/> event</item>
    /// </list>
    /// <para>
    /// Other libraries can subscribe to <see cref="Themes.ThemeService.ThemeChanged"/>
    /// to receive notifications when the application theme changes.
    /// </para>
    /// <para>
    ///     <b>Authors:</b>
    ///     <list type="bullet">
    ///         <item>Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil</item>
    ///     </list>
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// // Set the application theme
    /// ThemeManager.SetTheme(ThemeColor.Dark);
    ///
    /// // Subscribe to theme changes
    /// ThemeManager.ThemeChanged += (dict, color) =>
    /// {
    ///     // Handle theme change
    /// };
    /// </code>
    /// </example>
    public class ThemeManager
    {
        #region Events

        /// <summary>
        /// The theme changed event handler delegate.
        /// </summary>
        /// <param name="newThemeDictionary">The new theme dictionary containing ProjectUI-specific resources.</param>
        /// <param name="newThemeColor">The new theme color.</param>
        public delegate void ThemeChangedEventHandler(ResourceDictionary newThemeDictionary, ThemeColor newThemeColor);

        /// <summary>
        /// Occurs when the theme has been changed.
        /// </summary>
        public static event ThemeChangedEventHandler ThemeChanged;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the current theme color.
        /// </summary>
        /// <value>The current <see cref="ThemeColor"/>.</value>
        public static ThemeColor CurrentThemeColor { get; private set; } = ThemeColor.Light;

        #endregion

        #region Public Methods

        /// <summary>
        /// Updates the application theme to the specified theme color.
        /// </summary>
        /// <param name="theme">The theme color to set.</param>
        /// <remarks>
        /// <para>
        /// This method performs the following operations:
        /// </para>
        /// <list type="number">
        ///     <item>Initializes or updates the <see cref="Themes.ThemeService"/> singleton</item>
        ///     <item>Removes any previously loaded ProjectUI theme dictionaries</item>
        ///     <item>Loads the new ProjectUI-specific theme dictionary</item>
        ///     <item>Raises the <see cref="ThemeChanged"/> event</item>
        /// </list>
        /// <para>
        /// ProjectUI-specific resources include MainWindow styles, MessageWindow styles,
        /// and other UI components unique to this library.
        /// </para>
        /// </remarks>
        public static void SetTheme(ThemeColor theme)
        {
            // Convert to Themes.Theme enum and update ThemeService
            var themesTheme = ConvertToThemesTheme(theme);

            // Initialize or set theme on ThemeService
            if (!Themes.ThemeService.Instance.IsInitialized)
            {
                Themes.ThemeService.Instance.Initialize(themesTheme);
            }
            else
            {
                Themes.ThemeService.Instance.SetTheme(themesTheme);
            }

            // Create a new resource dictionary for ProjectUI-specific resources
            var themeDictionary = new ResourceDictionary();
            string blueString = "/ProjectUI;component/Themes/VS2013/BlueTheme.xaml";
            string darkString = "/ProjectUI;component/Themes/VS2013/DarkTheme.xaml";
            string lightString = "/ProjectUI;component/Themes/VS2013/LightTheme.xaml";

            // Remove old ProjectUI theme dictionaries from the current application
            RemoveProjectUIThemeDictionaries(blueString, darkString, lightString);

            // Set the new theme dictionary source
            switch (theme)
            {
                case ThemeColor.Blue:
                    themeDictionary.Source = new Uri(blueString, UriKind.RelativeOrAbsolute);
                    break;
                case ThemeColor.Dark:
                    themeDictionary.Source = new Uri(darkString, UriKind.RelativeOrAbsolute);
                    break;
                case ThemeColor.Light:
                default:
                    themeDictionary.Source = new Uri(lightString, UriKind.RelativeOrAbsolute);
                    break;
            }

            // Reset the application theme dictionary
            Application.Current.Resources.MergedDictionaries.Add(themeDictionary);

            // Update current theme
            CurrentThemeColor = theme;

            // Raise the theme changed event
            ThemeChanged?.Invoke(themeDictionary, theme);
        }

        /// <summary>
        /// Converts a <see cref="ThemeColor"/> to a <see cref="Themes.Theme"/>.
        /// </summary>
        /// <param name="themeColor">The ProjectUI theme color.</param>
        /// <returns>The corresponding <see cref="Themes.Theme"/> value.</returns>
        public static Themes.Theme ConvertToThemesTheme(ThemeColor themeColor)
        {
            switch (themeColor)
            {
                case ThemeColor.Blue:
                    return Themes.Theme.Blue;
                case ThemeColor.Dark:
                    return Themes.Theme.Dark;
                case ThemeColor.Light:
                default:
                    return Themes.Theme.Light;
            }
        }

        /// <summary>
        /// Converts a <see cref="Themes.Theme"/> to a <see cref="ThemeColor"/>.
        /// </summary>
        /// <param name="theme">The Themes library theme.</param>
        /// <returns>The corresponding <see cref="ThemeColor"/> value.</returns>
        public static ThemeColor ConvertFromThemesTheme(Themes.Theme theme)
        {
            switch (theme)
            {
                case Themes.Theme.Blue:
                    return ThemeColor.Blue;
                case Themes.Theme.Dark:
                    return ThemeColor.Dark;
                case Themes.Theme.Light:
                default:
                    return ThemeColor.Light;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Removes existing ProjectUI theme dictionaries from the application resources.
        /// </summary>
        /// <param name="blueString">The blue theme URI string.</param>
        /// <param name="darkString">The dark theme URI string.</param>
        /// <param name="lightString">The light theme URI string.</param>
        private static void RemoveProjectUIThemeDictionaries(string blueString, string darkString, string lightString)
        {
            for (int i = Application.Current.Resources.MergedDictionaries.Count - 1; i >= 0; i -= 1)
            {
                var source = Application.Current.Resources.MergedDictionaries[i].Source;
                if (source != null)
                {
                    var originalString = source.OriginalString;
                    if (originalString.Contains(blueString) ||
                        originalString.Contains(darkString) ||
                        originalString.Contains(lightString))
                    {
                        Application.Current.Resources.MergedDictionaries.RemoveAt(i);
                    }
                }
            }
        }

        #endregion
    }
}
