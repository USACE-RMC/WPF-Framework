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
using System.Threading;
using System.Windows;

namespace Themes
{
    /// <summary>
    /// Provides a thread-safe singleton service for managing application themes.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The <see cref="ThemeService"/> class is the central hub for theme management
    /// in applications using this library. It maintains the current theme state,
    /// manages theme resource dictionaries, and notifies subscribers of theme changes.
    /// </para>
    /// <para>
    /// This class implements the singleton pattern using <see cref="Lazy{T}"/> with
    /// <see cref="LazyThreadSafetyMode.ExecutionAndPublication"/> to ensure thread-safe
    /// initialization. All public members are thread-safe.
    /// </para>
    /// <para>
    ///     <b>Usage:</b>
    /// </para>
    /// <para>
    /// During application startup, call <see cref="Initialize"/> with the desired
    /// initial theme. This loads the control templates and applies the initial color
    /// palette.
    /// </para>
    /// <para>
    /// To change themes at runtime, call <see cref="SetTheme"/>. All controls using
    /// <c>DynamicResource</c> bindings will automatically update.
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
    /// // In App.xaml.cs OnStartup
    /// protected override void OnStartup(StartupEventArgs e)
    /// {
    ///     base.OnStartup(e);
    ///
    ///     // Initialize with the user's preferred theme
    ///     ThemeService.Instance.Initialize(Theme.Light);
    /// }
    ///
    /// // In options dialog
    /// private void ApplyTheme_Click(object sender, RoutedEventArgs e)
    /// {
    ///     ThemeService.Instance.SetTheme(Theme.Dark);
    /// }
    /// </code>
    /// </example>
    public sealed class ThemeService : IThemeService
    {
        #region Singleton Implementation

        /// <summary>
        /// Lazy initializer for thread-safe singleton creation.
        /// </summary>
        private static readonly Lazy<ThemeService> _instance =
            new Lazy<ThemeService>(() => new ThemeService(), LazyThreadSafetyMode.ExecutionAndPublication);

        /// <summary>
        /// Gets the singleton instance of the <see cref="ThemeService"/>.
        /// </summary>
        /// <value>The singleton <see cref="ThemeService"/> instance.</value>
        public static ThemeService Instance => _instance.Value;

        /// <summary>
        /// Prevents external instantiation of the <see cref="ThemeService"/> class.
        /// </summary>
        private ThemeService()
        {
            _currentTheme = Theme.Light;
        }

        #endregion

        #region Fields

        /// <summary>
        /// Lock object for thread-safe state access.
        /// </summary>
        private readonly object _lock = new object();

        /// <summary>
        /// The current theme.
        /// </summary>
        private Theme _currentTheme;

        /// <summary>
        /// Indicates whether the service has been initialized.
        /// </summary>
        private bool _isInitialized;

        /// <summary>
        /// The currently loaded control templates resource dictionary.
        /// Null until <see cref="Initialize"/> is called.
        /// </summary>
        private ResourceDictionary? _controlTemplatesDictionary;

        /// <summary>
        /// The currently loaded color resource dictionary.
        /// Null until <see cref="Initialize"/> is called.
        /// </summary>
        private ResourceDictionary? _currentColorDictionary;

        #endregion

        #region Properties

        /// <inheritdoc/>
        public Theme CurrentTheme
        {
            get
            {
                lock (_lock)
                {
                    return _currentTheme;
                }
            }
        }

        /// <inheritdoc/>
        public bool IsInitialized
        {
            get
            {
                lock (_lock)
                {
                    return _isInitialized;
                }
            }
        }

        #endregion

        #region Events

        /// <inheritdoc/>
        public event EventHandler<ThemeChangedEventArgs> ThemeChanged;

        /// <summary>
        /// Raises the <see cref="ThemeChanged"/> event.
        /// </summary>
        /// <param name="e">The event arguments.</param>
        private void OnThemeChanged(ThemeChangedEventArgs e)
        {
            ThemeChanged?.Invoke(this, e);
        }

        #endregion

        #region Public Methods

        /// <inheritdoc/>
        public void Initialize(Theme initialTheme)
        {
            lock (_lock)
            {
                if (_isInitialized)
                {
                    return;
                }

                var app = Application.Current;
                if (app == null)
                {
                    throw new InvalidOperationException(
                        "Cannot initialize ThemeService when Application.Current is null. " +
                        "Ensure this method is called after the Application object has been created.");
                }

                // Load control templates (done once, never changes)
                _controlTemplatesDictionary = new ResourceDictionary
                {
                    Source = new Uri(ThemeResourceHelper.ControlTemplatesUri, UriKind.Absolute)
                };
                app.Resources.MergedDictionaries.Add(_controlTemplatesDictionary);

                // Load initial color palette
                _currentColorDictionary = new ResourceDictionary
                {
                    Source = new Uri(ThemeResourceHelper.GetColorDictionaryUri(initialTheme), UriKind.Absolute)
                };
                app.Resources.MergedDictionaries.Add(_currentColorDictionary);

                _currentTheme = initialTheme;
                _isInitialized = true;
            }
        }

        /// <inheritdoc/>
        public void SetTheme(Theme theme)
        {
            // Check if we need to marshal to UI thread
            var app = Application.Current;
            if (app == null)
            {
                throw new InvalidOperationException(
                    "Cannot set theme when Application.Current is null.");
            }

            if (!app.Dispatcher.CheckAccess())
            {
                app.Dispatcher.BeginInvoke(() => SetTheme(theme));
                return;
            }

            Theme oldTheme;
            ResourceDictionary newColorDictionary;

            lock (_lock)
            {
                if (!_isInitialized)
                {
                    throw new InvalidOperationException(
                        "ThemeService has not been initialized. Call Initialize() before SetTheme().");
                }

                if (_currentTheme == theme)
                {
                    return;
                }

                oldTheme = _currentTheme;
                _currentTheme = theme;

                // Remove old color dictionary
                if (_currentColorDictionary != null)
                {
                    app.Resources.MergedDictionaries.Remove(_currentColorDictionary);
                }

                // Create and add new color dictionary
                newColorDictionary = new ResourceDictionary
                {
                    Source = new Uri(ThemeResourceHelper.GetColorDictionaryUri(theme), UriKind.Absolute)
                };
                app.Resources.MergedDictionaries.Add(newColorDictionary);
                _currentColorDictionary = newColorDictionary;
            }

            // Raise event outside the lock to prevent deadlocks
            OnThemeChanged(new ThemeChangedEventArgs(oldTheme, theme, newColorDictionary));
        }

        #endregion
    }
}
