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
using System.Windows.Data;
using System.Windows.Media;
using FrameworkInterfaces;

namespace FrameworkUI.MessageWindow
{
    /// <summary>
    /// A class for a message window item.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b> Authors: </b>
    /// <list type="bullet">
    ///     <item> Woodrow Lee Fields, USACE Risk Management Center, woodrow.l.fields@usace.army.mil </item>
    ///     <item> Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil </item>
    /// </list>
    /// </para>
    /// </remarks>
    public class MessageItem : DependencyObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MessageItem"/> class.
        /// </summary>
        /// <param name="message">The message item to wrap.</param>
        public MessageItem(IMessageItem message)
        {
            _message = message;

            var messenger = FrameworkInterfaces.Messaging.Messenger.GetInstance();
            // Bind the message beep and text foreground color
            if (_message.Type == MessageType.Error)
            {
                BindingOperations.SetBinding(this, MessageItem.BeepProperty, new Binding(nameof(FrameworkInterfaces.Messaging.Messenger.ErrorBeep)) { Source = messenger, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
                BindingOperations.SetBinding(this, MessageItem.ForegroundColorProperty, new Binding(nameof(FrameworkInterfaces.Messaging.Messenger.ErrorColor)) { Source = messenger, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
            }
            else if (_message.Type == MessageType.Warning)
            {
                BindingOperations.SetBinding(this, MessageItem.BeepProperty, new Binding(nameof(FrameworkInterfaces.Messaging.Messenger.WarningBeep)) { Source = messenger, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
                BindingOperations.SetBinding(this, MessageItem.ForegroundColorProperty, new Binding(nameof(FrameworkInterfaces.Messaging.Messenger.WarningColor)) { Source = messenger, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
            }
            else if (_message.Type == MessageType.Message)
            {
                BindingOperations.SetBinding(this, MessageItem.BeepProperty, new Binding(nameof(FrameworkInterfaces.Messaging.Messenger.MessageBeep)) { Source = messenger, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
                BindingOperations.SetBinding(this, MessageItem.ForegroundColorProperty, new Binding(nameof(FrameworkInterfaces.Messaging.Messenger.MessageColor)) { Source = messenger, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
            }
            else if (_message.Type == MessageType.Event)
            {
                BindingOperations.SetBinding(this, MessageItem.BeepProperty, new Binding(nameof(FrameworkInterfaces.Messaging.Messenger.EventBeep)) { Source = messenger, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
                BindingOperations.SetBinding(this, MessageItem.ForegroundColorProperty, new Binding(nameof(FrameworkInterfaces.Messaging.Messenger.EventColor)) { Source = messenger, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged });
            }
        }

        private IMessageItem _message;

        /// <summary>
        /// Click event. On errors or warnings we can add a handler to this.
        /// </summary>
        public IMessageItem Message
        {
            get { return _message; }
        }

        /// <summary>
        /// Dependency property for the beep boolean.
        /// </summary>
        public static DependencyProperty BeepProperty = DependencyProperty.Register(nameof(Beep), typeof(bool), typeof(MessageItem), new UIPropertyMetadata(false));

        /// <summary>
        /// Determines whether the computer should beep when adding the item.
        /// </summary>
        public bool Beep
        {
            get { return (bool)GetValue(BeepProperty); }
            set { SetValue(BeepProperty, value); }
        }

        /// <summary>
        /// Dependency property for the message item foreground color.
        /// </summary>
        public static DependencyProperty ForegroundColorProperty = DependencyProperty.Register(nameof(ForegroundColor), typeof(SolidColorBrush), typeof(MessageItem), new UIPropertyMetadata(new SolidColorBrush(Colors.Black)));

        /// <summary>
        /// Gets or sets the message item foreground (text) color.
        /// </summary>
        public SolidColorBrush ForegroundColor
        {
            get { return (SolidColorBrush)GetValue(ForegroundColorProperty); }
            set { SetValue(ForegroundColorProperty, value); }
        }

    }
}
