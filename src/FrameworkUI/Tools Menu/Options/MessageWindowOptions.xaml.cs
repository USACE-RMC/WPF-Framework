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
using System.Windows.Controls;
using System.Windows.Media;

namespace FrameworkUI
{
    /// <summary>
    /// Interaction logic for MessageWindowOptions.xaml providing message window settings configuration.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b> Authors: </b>
    /// <list type="bullet">
    ///     <item> Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil </item>
    /// </list>
    /// </para>
    /// </remarks>
    public partial class MessageWindowOptions : UserControl
    {

        /// <summary>
        /// Construct new Message Window Options.
        /// </summary>
        public MessageWindowOptions()
        {
            // This call is required by the designer.
            InitializeComponent();
            // Add any initialization after the InitializeComponent() call.
        }

        /// <summary>
        /// Dependency property for the error beep boolean.
        /// </summary>
        public static DependencyProperty ErrorBeepProperty = DependencyProperty.Register(nameof(ErrorBeep), typeof(bool), typeof(MessageWindowOptions), new UIPropertyMetadata(true));

        /// <summary>
        /// Gets or sets whether error messages beep.
        /// </summary>
        public bool ErrorBeep
        {
            get { return (bool)GetValue(ErrorBeepProperty); }
            set { SetValue(ErrorBeepProperty, value); }
        }

        /// <summary>
        /// Dependency property for the warning beep boolean.
        /// </summary>
        public static DependencyProperty WarningBeepProperty = DependencyProperty.Register(nameof(WarningBeep), typeof(bool), typeof(MessageWindowOptions), new UIPropertyMetadata(true));

        /// <summary>
        /// Gets or sets whether warning messages beep.
        /// </summary>
        public bool WarningBeep
        {
            get { return (bool)GetValue(WarningBeepProperty); }
            set { SetValue(WarningBeepProperty, value); }
        }

        /// <summary>
        /// Dependency property for the beep boolean.
        /// </summary>
        public static DependencyProperty MessageBeepProperty = DependencyProperty.Register(nameof(MessageBeep), typeof(bool), typeof(MessageWindowOptions), new UIPropertyMetadata(false));

        /// <summary>
        /// Gets or sets whether messages beep.
        /// </summary>
        public bool MessageBeep
        {
            get { return (bool)GetValue(MessageBeepProperty); }
            set { SetValue(MessageBeepProperty, value); }
        }

        /// <summary>
        /// Dependency property for the event beep boolean.
        /// </summary>
        public static DependencyProperty EventBeepProperty = DependencyProperty.Register(nameof(EventBeep), typeof(bool), typeof(MessageWindowOptions), new UIPropertyMetadata(false));

        /// <summary>
        /// Gets or sets whether event messages beep.
        /// </summary>
        public bool EventBeep
        {
            get { return (bool)GetValue(EventBeepProperty); }
            set { SetValue(EventBeepProperty, value); }
        }

        /// <summary>
        /// Dependency property for the error message color.
        /// </summary>
        public static DependencyProperty ErrorColorProperty = DependencyProperty.Register(nameof(ErrorColor), typeof(SolidColorBrush), typeof(MessageWindowOptions), new UIPropertyMetadata(Brushes.Black));

        /// <summary>
        /// Gets or sets the error message color.
        /// </summary>
        public SolidColorBrush ErrorColor
        {
            get { return (SolidColorBrush)GetValue(ErrorColorProperty); }
            set { SetValue(ErrorColorProperty, value); }
        }

        /// <summary>
        /// Dependency property for the warning message color.
        /// </summary>
        public static DependencyProperty WarningColorProperty = DependencyProperty.Register(nameof(WarningColor), typeof(SolidColorBrush), typeof(MessageWindowOptions), new UIPropertyMetadata(Brushes.Black));

        /// <summary>
        /// Gets or sets the warning message color.
        /// </summary>
        public SolidColorBrush WarningColor
        {
            get { return (SolidColorBrush)GetValue(WarningColorProperty); }
            set { SetValue(WarningColorProperty, value); }
        }

        /// <summary>
        /// Dependency property for the message color.
        /// </summary>
        public static DependencyProperty MessageColorProperty = DependencyProperty.Register(nameof(MessageColor), typeof(SolidColorBrush), typeof(MessageWindowOptions), new UIPropertyMetadata(Brushes.Black));

        /// <summary>
        /// Gets or sets the message color.
        /// </summary>
        public SolidColorBrush MessageColor
        {
            get { return (SolidColorBrush)GetValue(MessageColorProperty); }
            set { SetValue(MessageColorProperty, value); }
        }

        /// <summary>
        /// Dependency property for the event message color.
        /// </summary>
        public static DependencyProperty EventColorProperty = DependencyProperty.Register(nameof(EventColor), typeof(SolidColorBrush), typeof(MessageWindowOptions), new UIPropertyMetadata(Brushes.Black));

        /// <summary>
        /// Gets or sets the event message color.
        /// </summary>
        public SolidColorBrush EventColor
        {
            get { return (SolidColorBrush)GetValue(EventColorProperty); }
            set { SetValue(EventColorProperty, value); }
        }

    }
}
