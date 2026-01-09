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
using System.Windows;
using Themes;

namespace GenericControls.Demo
{
    /// <summary>
    /// Represents the WPF application entry point for the GenericControls demo.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Application-level events, such as Startup, Exit, and DispatcherUnhandledException
    /// can be handled in this file.
    /// </para>
    /// <para>
    ///     <b> Authors: </b>
    /// <list type="bullet">
    /// <item><description>
    ///     Woodrow Fields, USACE Risk Management Center, woodrow.l.fields@usace.army.mil
    /// </description></item>
    /// <item><description>
    ///     Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil
    /// </description></item>
    /// </list>
    /// </para>
    /// </remarks>
    public partial class App
    {
        /// <summary>
        /// Handles the application Startup event. Initializes the theme system
        /// and creates the main window.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Using the Startup event instead of StartupUri ensures that theme resources
        /// (control templates and colors) are loaded before any XAML windows are parsed.
        /// This prevents "Cannot find resource" errors for theme-dependent styles.
        /// </para>
        /// </remarks>
        /// <param name="sender">The application instance.</param>
        /// <param name="e">Startup event arguments.</param>
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            // Initialize the theme system with the Light theme as default
            // This loads control templates and color resources BEFORE MainWindow is created
            ThemeService.Instance.Initialize(Theme.Light);

            // Create and show the main window after theme resources are loaded
            var mainWindow = new MainWindow();
            mainWindow.Show();
        }
    }
}