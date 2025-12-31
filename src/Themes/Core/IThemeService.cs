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

namespace Themes
{
    /// <summary>
    /// Defines the contract for a theme management service that provides
    /// application-wide theme switching capabilities.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The theme service is responsible for managing the current application theme
    /// and notifying subscribers when the theme changes. This allows multiple WPF
    /// libraries and components to respond to theme changes independently.
    /// </para>
    /// <para>
    /// Implementations should be thread-safe and support being accessed from
    /// multiple threads, though theme changes should always be applied on the
    /// UI thread.
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
    /// // Subscribe to theme changes
    /// ThemeService.Instance.ThemeChanged += (sender, e) =>
    /// {
    ///     Console.WriteLine($"Theme changed from {e.OldTheme} to {e.NewTheme}");
    /// };
    ///
    /// // Change the theme
    /// ThemeService.Instance.SetTheme(Theme.Dark);
    /// </code>
    /// </example>
    public interface IThemeService
    {
        /// <summary>
        /// Gets the currently active theme.
        /// </summary>
        /// <value>The current <see cref="Theme"/> applied to the application.</value>
        Theme CurrentTheme { get; }

        /// <summary>
        /// Gets a value indicating whether the theme service has been initialized.
        /// </summary>
        /// <value>
        /// <c>true</c> if <see cref="Initialize"/> has been called and the service
        /// is ready to manage themes; otherwise, <c>false</c>.
        /// </value>
        bool IsInitialized { get; }

        /// <summary>
        /// Occurs when the application theme has changed.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This event is raised after the theme color resources have been applied
        /// to the application's merged dictionaries. Subscribers can use this event
        /// to perform additional theme-related updates, such as:
        /// </para>
        /// <list type="bullet">
        ///     <item>Updating third-party controls that don't support <c>DynamicResource</c></item>
        ///     <item>Refreshing cached color values</item>
        ///     <item>Applying theme-specific logic</item>
        /// </list>
        /// <para>
        /// This event is always raised on the UI thread.
        /// </para>
        /// </remarks>
        event EventHandler<ThemeChangedEventArgs> ThemeChanged;

        /// <summary>
        /// Initializes the theme service and applies the specified initial theme.
        /// </summary>
        /// <param name="initialTheme">The theme to apply during initialization.</param>
        /// <remarks>
        /// <para>
        /// This method should be called once during application startup, typically
        /// in the <c>App.xaml.cs</c> constructor or <c>OnStartup</c> method.
        /// </para>
        /// <para>
        /// Calling this method multiple times has no effect after the first call.
        /// </para>
        /// </remarks>
        /// <exception cref="InvalidOperationException">
        /// Thrown if called when <c>Application.Current</c> is null.
        /// </exception>
        void Initialize(Theme initialTheme);

        /// <summary>
        /// Changes the application theme to the specified theme.
        /// </summary>
        /// <param name="theme">The theme to apply.</param>
        /// <remarks>
        /// <para>
        /// This method swaps the color resource dictionary in the application's
        /// merged dictionaries and raises the <see cref="ThemeChanged"/> event.
        /// </para>
        /// <para>
        /// If the specified theme is the same as the current theme, this method
        /// does nothing.
        /// </para>
        /// <para>
        /// This method is thread-safe and will marshal to the UI thread if called
        /// from a background thread.
        /// </para>
        /// </remarks>
        /// <exception cref="InvalidOperationException">
        /// Thrown if called before <see cref="Initialize"/> has been called.
        /// </exception>
        void SetTheme(Theme theme);
    }
}
