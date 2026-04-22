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

namespace FrameworkUI
{
    /// <summary>
    /// Enumeration of available theme colors for the ProjectUI library.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This enumeration defines the available visual themes that can be applied
    /// to the ProjectUI library. Each theme follows the Visual Studio 2013 design
    /// language and provides a consistent color palette for all controls.
    /// </para>
    /// <para>
    /// Use <see cref="ThemeManager.SetTheme"/> to change the current theme at runtime.
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
    /// // Apply the dark theme
    /// ThemeManager.SetTheme(ThemeColor.Dark);
    /// </code>
    /// </example>
    public enum ThemeColor
    {
        /// <summary>
        /// Blue theme with blue accent colors and a light background.
        /// This theme is inspired by Visual Studio 2013 Blue theme.
        /// </summary>
        Blue,

        /// <summary>
        /// Dark theme with dark gray backgrounds and light text.
        /// This theme is inspired by Visual Studio 2013 Dark theme.
        /// </summary>
        Dark,

        /// <summary>
        /// Light theme with light gray backgrounds and dark text.
        /// This theme is inspired by Visual Studio 2013 Light theme.
        /// </summary>
        Light
    }
}
