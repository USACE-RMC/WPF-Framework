using System;
using System.ComponentModel;
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
    ///     Authors:
    ///     Woodrow Lee Fields, USACE Risk Management Center, woodrow.l.fields@usace.army.mil
    ///     Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil
    /// </para>
    /// </remarks>
    public class MessageItem : DependencyObject
    {
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
        /// Gets and sets the message item foreground (text) color.
        /// </summary>
        public SolidColorBrush ForegroundColor
        {
            get { return (SolidColorBrush)GetValue(ForegroundColorProperty); }
            set { SetValue(ForegroundColorProperty, value); }
        }

    }
}
