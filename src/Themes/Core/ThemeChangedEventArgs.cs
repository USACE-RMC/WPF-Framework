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

using System;
using System.Windows;

namespace Themes
{
    /// <summary>
    /// Provides data for the <see cref="IThemeService.ThemeChanged"/> event.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This class contains information about a theme change, including the new theme
    /// and the associated color resource dictionary. Subscribers can use this information
    /// to perform additional theme-related updates that cannot be handled through
    /// <c>DynamicResource</c> bindings alone.
    /// </para>
    /// <para>
    ///     <b>Authors:</b>
    ///     <list type="bullet">
    ///         <item>Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil</item>
    ///     </list>
    /// </para>
    /// </remarks>
    public class ThemeChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ThemeChangedEventArgs"/> class.
        /// </summary>
        /// <param name="oldTheme">The previous theme before the change.</param>
        /// <param name="newTheme">The new theme after the change.</param>
        /// <param name="colorDictionary">The resource dictionary containing the new theme colors.</param>
        public ThemeChangedEventArgs(Theme oldTheme, Theme newTheme, ResourceDictionary colorDictionary)
        {
            OldTheme = oldTheme;
            NewTheme = newTheme;
            ColorDictionary = colorDictionary;
        }

        /// <summary>
        /// Gets the previous theme before the change.
        /// </summary>
        /// <value>The <see cref="Theme"/> that was active before the theme change.</value>
        public Theme OldTheme { get; }

        /// <summary>
        /// Gets the new theme after the change.
        /// </summary>
        /// <value>The <see cref="Theme"/> that is now active after the theme change.</value>
        public Theme NewTheme { get; }

        /// <summary>
        /// Gets the resource dictionary containing the new theme colors.
        /// </summary>
        /// <value>
        /// The <see cref="ResourceDictionary"/> containing color brush definitions for the new theme.
        /// This can be used by subscribers that need direct access to theme resources.
        /// </value>
        public ResourceDictionary ColorDictionary { get; }
    }
}
