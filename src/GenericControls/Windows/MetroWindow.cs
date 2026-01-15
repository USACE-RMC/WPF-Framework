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

using System.Windows;
using System.Windows.Input;

namespace GenericControls
{
    /// <summary>
    /// A base class for Metro-styled windows with custom chrome and window buttons.
    /// Provides automatic command binding setup for minimize, maximize, restore, and close operations.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This class sets up the necessary command bindings for the MetroWindowStyle defined in the Themes library.
    /// Windows deriving from this class will automatically have working minimize, maximize, restore, and close buttons.
    /// </para>
    /// <para>
    /// To use this class:
    /// <list type="number">
    /// <item><description>Create a window that inherits from MetroWindow</description></item>
    /// <item><description>Set the Style to "{DynamicResource MetroWindowStyle}" in XAML</description></item>
    /// <item><description>Ensure the Themes library is initialized before the window is created</description></item>
    /// </list>
    /// </para>
    /// <para>
    ///     <b> Authors: </b>
    /// <list type="bullet">
    /// <item><description>
    ///     Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil
    /// </description></item>
    /// <item><description>
    ///     Woodrow Fields, USACE Risk Management Center, woodrow.l.fields@usace.army.mil
    /// </description></item>
    /// </list>
    /// </para>
    /// </remarks>
    public class MetroWindow : Window
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MetroWindow"/> class.
        /// Sets up command bindings for window operations.
        /// </summary>
        public MetroWindow()
        {
            // Set up command bindings for window chrome buttons
            CommandBindings.Add(new CommandBinding(SystemCommands.CloseWindowCommand, OnCloseWindow));
            CommandBindings.Add(new CommandBinding(SystemCommands.MaximizeWindowCommand, OnMaximizeWindow, OnCanResizeWindow));
            CommandBindings.Add(new CommandBinding(SystemCommands.MinimizeWindowCommand, OnMinimizeWindow, OnCanMinimizeWindow));
            CommandBindings.Add(new CommandBinding(SystemCommands.RestoreWindowCommand, OnRestoreWindow, OnCanResizeWindow));
        }

        /// <summary>
        /// Handles the close window command.
        /// </summary>
        /// <param name="sender">The command source.</param>
        /// <param name="e">The event arguments.</param>
        protected virtual void OnCloseWindow(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.CloseWindow(this);
        }

        /// <summary>
        /// Handles the maximize window command.
        /// </summary>
        /// <param name="sender">The command source.</param>
        /// <param name="e">The event arguments.</param>
        protected virtual void OnMaximizeWindow(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.MaximizeWindow(this);
        }

        /// <summary>
        /// Handles the minimize window command.
        /// </summary>
        /// <param name="sender">The command source.</param>
        /// <param name="e">The event arguments.</param>
        protected virtual void OnMinimizeWindow(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.MinimizeWindow(this);
        }

        /// <summary>
        /// Handles the restore window command.
        /// </summary>
        /// <param name="sender">The command source.</param>
        /// <param name="e">The event arguments.</param>
        protected virtual void OnRestoreWindow(object sender, ExecutedRoutedEventArgs e)
        {
            SystemCommands.RestoreWindow(this);
        }

        /// <summary>
        /// Determines whether the window can be resized.
        /// </summary>
        /// <param name="sender">The command source.</param>
        /// <param name="e">The event arguments.</param>
        protected virtual void OnCanResizeWindow(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ResizeMode == ResizeMode.CanResize || ResizeMode == ResizeMode.CanResizeWithGrip;
        }

        /// <summary>
        /// Determines whether the window can be minimized.
        /// </summary>
        /// <param name="sender">The command source.</param>
        /// <param name="e">The event arguments.</param>
        protected virtual void OnCanMinimizeWindow(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ResizeMode != ResizeMode.NoResize;
        }
    }
}
