using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Win32;
using FrameworkInterfaces;

namespace FrameworkUI.MessageWindow
{
    /// <summary>
    /// Interaction logic for MessageWindowControl.xaml
    /// </summary>
    public partial class MessageWindowControl : UserControl
    {
        /// <summary>
        /// Construct new MessageWindowControl.
        /// </summary>
        public MessageWindowControl()
        {
            InitializeComponent();
            // Add any initialization after the InitializeComponent() call.
            FrameworkInterfaces.Messaging.Messenger.GetInstance().MessagesAdded += MessageWindowControl_MessagesAdded;
            FrameworkInterfaces.Messaging.Messenger.GetInstance().MessagesRemoved += MessageWindowControl_MessagesRemoved;
            MessageWindowControl_MessagesAdded(FrameworkInterfaces.Messaging.Messenger.GetInstance().AllMessageItems().ToArray());
            MyDataGrid.ItemsSource = _filteredMessages;
            UpdateButtonText();
            UpdateClearAllFilterButtonStyle();
            SetColumnHeaderStyles();
        }

        /// <summary>
        /// Dependency property for the message window background color.
        /// </summary>
        public static DependencyProperty BackgroundColorProperty = DependencyProperty.Register(nameof(BackgroundColor), typeof(SolidColorBrush), typeof(MessageWindowControl), new FrameworkPropertyMetadata(new SolidColorBrush(Colors.White)));

        /// <summary>
        /// Get and set the message window background color.
        /// </summary>
        public SolidColorBrush BackgroundColor
        {
            get { return (SolidColorBrush)this.GetValue(BackgroundColorProperty); }
            set { this.SetValue(BackgroundColorProperty, value); }
        }

        /// <summary>
        /// Dependency property for the stack panel background color.
        /// </summary>
        public static DependencyProperty StackPanelBackgroundColorProperty = DependencyProperty.Register(nameof(StackPanelBackgroundColor), typeof(SolidColorBrush), typeof(MessageWindowControl), new FrameworkPropertyMetadata(new SolidColorBrush(Colors.White)));

        /// <summary>
        /// Get and set the stack panel background color.
        /// </summary>
        public SolidColorBrush StackPanelBackgroundColor
        {
            get { return (SolidColorBrush)this.GetValue(StackPanelBackgroundColorProperty); }
            set { this.SetValue(StackPanelBackgroundColorProperty, value); }
        }

        /// <summary>
        /// Dependency property for the stack panel separator style.
        /// </summary>
        public static DependencyProperty StackPanelSeperatorStyleProperty = DependencyProperty.Register(nameof(StackPanelSeperatorStyle), typeof(Style), typeof(MessageWindowControl), new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Gets and sets the stack panel separator style.
        /// </summary>
        public Style StackPanelSeperatorStyle
        {
            get { return (Style)this.GetValue(StackPanelSeperatorStyleProperty); }
            set { this.SetValue(StackPanelSeperatorStyleProperty, value); }
        }

        /// <summary>
        /// Dependency property for the stack panel button style.
        /// </summary>
        public static DependencyProperty StackPanelButtonStyleProperty = DependencyProperty.Register(nameof(StackPanelButtonStyle), typeof(Style), typeof(MessageWindowControl), new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Gets and sets the stack panel button style.
        /// </summary>
        public Style StackPanelButtonStyle
        {
            get { return (Style)this.GetValue(StackPanelButtonStyleProperty); }
            set { this.SetValue(StackPanelButtonStyleProperty, value); }
        }

        /// <summary>
        /// Dependency property for the stack panel pressed button style.
        /// </summary>
        public static DependencyProperty StackPanelPressedButtonStyleProperty = DependencyProperty.Register(nameof(StackPanelPressedButtonStyle), typeof(Style), typeof(MessageWindowControl), new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Gets and sets the stack panel pressed button style.
        /// </summary>
        public Style StackPanelPressedButtonStyle
        {
            get { return (Style)this.GetValue(StackPanelPressedButtonStyleProperty); }
            set { this.SetValue(StackPanelPressedButtonStyleProperty, value); }
        }

        /// <summary>
        /// Dependency property for the center header column style.
        /// </summary>
        public static DependencyProperty CenterColumnHeaderStyleProperty = DependencyProperty.Register(nameof(CenterColumnHeaderStyle), typeof(Style), typeof(MessageWindowControl), new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Gets and sets the center header column style.
        /// </summary>
        public Style CenterColumnHeaderStyle
        {
            get { return (Style)this.GetValue(CenterColumnHeaderStyleProperty); }
            set { this.SetValue(CenterColumnHeaderStyleProperty, value); }
        }

        /// <summary>
        /// Dependency property for the left column header style.
        /// </summary>
        public static DependencyProperty LeftColumnHeaderStyleProperty = DependencyProperty.Register(nameof(LeftColumnHeaderStyle), typeof(Style), typeof(MessageWindowControl), new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Gets and sets the left column header style.
        /// </summary>
        public Style LeftColumnHeaderStyle
        {
            get { return (Style)this.GetValue(LeftColumnHeaderStyleProperty); }
            set { this.SetValue(LeftColumnHeaderStyleProperty, value); }
        }

        private List<IMessageItem> _messages = new List<IMessageItem>();
        private List<MessageItem> _filteredMessages = new List<MessageItem>();
        private int _errorItemCount = 0;
        private int _warningItemCount = 0;
        private int _eventItemCount = 0;
        private int _messageItemCount = 0;
        private bool _showErrors = true;
        private bool _showWarnings = true;
        private bool _showMessages = false;
        private bool _showEvents = false;

        private void MessageWindowControl_MessagesRemoved(IMessageItem[] oldMessages)
        {
            foreach (IMessageItem oldMessage in oldMessages)
            {
                if (oldMessage.Type == MessageType.Error)
                {
                    _errorItemCount -= 1;
                    if (_showErrors)
                    {
                        for (int i = _filteredMessages.Count() - 1; i >= 0; i--)
                        {
                            if (_filteredMessages[i].Message.Code == oldMessage.Code && _filteredMessages[i].Message.Source == oldMessage.Source) { _filteredMessages.RemoveAt(i); }
                        }
                    }
                }
                else if (oldMessage.Type == MessageType.Warning)
                {
                    _warningItemCount -= 1;
                    if (_showWarnings)
                    {
                        for (int i = _filteredMessages.Count() - 1; i >= 0; i--)
                        {
                            if (_filteredMessages[i].Message.Code == oldMessage.Code && _filteredMessages[i].Message.Source == oldMessage.Source) { _filteredMessages.RemoveAt(i); }
                        }
                    }
                }
                else if (oldMessage.Type == MessageType.Message)
                {
                    _messageItemCount -= 1;
                    if (_showMessages)
                    {
                        for (int i = _filteredMessages.Count() - 1; i >= 0; i--)
                        {
                            if (_filteredMessages[i].Message.Code == oldMessage.Code && _filteredMessages[i].Message.Source == oldMessage.Source) { _filteredMessages.RemoveAt(i); }
                        }
                    }
                }
                else if (oldMessage.Type == MessageType.Event)
                {
                    _eventItemCount -= 1;
                    if (_showEvents)
                    {
                        for (int i = _filteredMessages.Count() - 1; i >= 0; i--)
                        {
                            if (_filteredMessages[i].Message.Code == oldMessage.Code && _filteredMessages[i].Message.Source == oldMessage.Source) { _filteredMessages.RemoveAt(i); }
                        }
                    }
                }
                //
                _messages.Remove(oldMessage);
            }
            //
            UpdateButtonText();
            MyDataGrid.Items.Refresh();
        }

        private void MessageWindowControl_MessagesAdded(IMessageItem[] newMessages)
        {
            foreach (IMessageItem newMessage in newMessages)
            {
                if (newMessage.Type == MessageType.Error)
                {
                    _errorItemCount += 1;
                    if (_showErrors) { _filteredMessages.Add(new MessageItem(newMessage)); }
                }
                else if (newMessage.Type == MessageType.Warning)
                {
                    _warningItemCount += 1;
                    if (_showWarnings) { _filteredMessages.Add(new MessageItem(newMessage)); }
                }
                else if (newMessage.Type == MessageType.Message)
                {
                    _messageItemCount += 1;
                    if (_showMessages) { _filteredMessages.Add(new MessageItem(newMessage)); }
                }
                else if (newMessage.Type == MessageType.Event)
                {
                    _eventItemCount += 1;
                    if (_showEvents) { _filteredMessages.Add(new MessageItem(newMessage)); }
                }
                //
                _messages.Add(newMessage);
            }
            MyDataGrid.Items.Refresh();
            UpdateButtonText();
        }

        /// <summary>
        /// Update the button tool tip and text.
        /// </summary>
        private void UpdateButtonText()
        {
            if (_showErrors == true)
            {
                ErrorsButtonText.Text = $" {_errorItemCount} Errors";
                ErrorsButton.ToolTip = "Hide Errors";
            }
            else
            {
                ErrorsButtonText.Text = " 0 of " + _errorItemCount + " Errors";
                ErrorsButton.ToolTip = "Show Errors";
            }
            if (_showWarnings == true)
            {
                WarningsButtonText.Text = " " + _warningItemCount + " Warnings";
                WarningsButton.ToolTip = "Hide Warnings";
            }
            else
            {
                WarningsButtonText.Text = " 0 of " + _warningItemCount + " Warnings";
                WarningsButton.ToolTip = "Show Warnings";
            }
            if (_showMessages == true)
            {
                MessageButtonText.Text = " " + _messageItemCount + " Messages";
                MessagesButton.ToolTip = "Hide Messages";
            }
            else
            {
                MessageButtonText.Text = " 0 of " + _messageItemCount + " Messages";
                MessagesButton.ToolTip = "Show Messages";
            }
            if (_showEvents == true)
            {
                EventsButtonText.Text = _eventItemCount + " Events";
                EventsButton.ToolTip = "Hide Log Events";
            }
            else
            {
                EventsButtonText.Text = "0 of " + _eventItemCount + " Events";
                EventsButton.ToolTip = "Show Log Events";
            }
        }

        /// <summary>
        /// Update the clear all filter button style.
        /// </summary>
        private void UpdateClearAllFilterButtonStyle()
        {
            if (_showErrors == false || _showWarnings == false || _showMessages == false || _showEvents == false)
            {
                ClearFiltersButton.IsEnabled = true;
                ClearFiltersButton.Style = StackPanelPressedButtonStyle;
                return;
            }
            ClearFiltersButton.IsEnabled = false;
            ClearFiltersButton.Style = StackPanelButtonStyle;
        }

        private void RefreshFilteredItems()
        {
            _filteredMessages.Clear();
            foreach (IMessageItem msg in _messages)
            {
                if (msg.Type == MessageType.Error && _showErrors) { _filteredMessages.Add(new MessageItem(msg)); }
                if (msg.Type == MessageType.Warning && _showWarnings) { _filteredMessages.Add(new MessageItem(msg)); }
                if (msg.Type == MessageType.Message && _showMessages) { _filteredMessages.Add(new MessageItem(msg)); }
                if (msg.Type == MessageType.Event && _showEvents) { _filteredMessages.Add(new MessageItem(msg)); }
            }
            //
            MyDataGrid.Items.Refresh();
        }

        /// <summary>
        /// On click, update message window filter.
        /// </summary>
        private void ErrorsButton_Click(object sender, RoutedEventArgs e)
        {
            if (_showErrors == true)
            {
                _showErrors = false;
                ErrorsButton.Style = StackPanelButtonStyle;
                for (int i = _filteredMessages.Count() - 1; i >= 0; i--)
                {
                    if (_filteredMessages[i].Message.Type == MessageType.Error) { _filteredMessages.RemoveAt(i); }
                }
                //
                MyDataGrid.Items.Refresh();
            }
            else
            {
                _showErrors = true;
                ErrorsButton.Style = StackPanelPressedButtonStyle;
                //
                RefreshFilteredItems();
            }
            //
            UpdateClearAllFilterButtonStyle();
            MyDataGrid.Focus();
        }

        /// <summary>
        /// On click, update message window filter.
        /// </summary>
        private void WarningsButton_Click(object sender, RoutedEventArgs e)
        {
            if (_showWarnings == true)
            {
                _showWarnings = false;
                WarningsButton.Style = StackPanelButtonStyle;
                for (int i = _filteredMessages.Count() - 1; i >= 0; i--)
                {
                    if (_filteredMessages[i].Message.Type == MessageType.Warning) { _filteredMessages.RemoveAt(i); }
                }
                //
                MyDataGrid.Items.Refresh();
            }
            else
            {
                _showWarnings = true;
                WarningsButton.Style = StackPanelPressedButtonStyle;
                //
                RefreshFilteredItems();
            }
            UpdateClearAllFilterButtonStyle();
            MyDataGrid.Focus();
        }

        /// <summary>
        /// On click, update message window filter.
        /// </summary>
        private void MessagesButton_Click(object sender, RoutedEventArgs e)
        {
            if (_showMessages == true)
            {
                _showMessages = false;
                MessagesButton.Style = StackPanelButtonStyle;
                for (int i = _filteredMessages.Count() - 1; i >= 0; i--)
                {
                    if (_filteredMessages[i].Message.Type == MessageType.Message) { _filteredMessages.RemoveAt(i); }
                }
                //
                MyDataGrid.Items.Refresh();
            }
            else
            {
                _showMessages = true;
                MessagesButton.Style = StackPanelPressedButtonStyle;
                //
                RefreshFilteredItems();
            }
            UpdateClearAllFilterButtonStyle();
            MyDataGrid.Focus();
        }

        /// <summary>
        /// On click, update message window filter.
        /// </summary>
        private void EventsButton_Click(object sender, RoutedEventArgs e)
        {
            if (_showEvents == true)
            {
                _showEvents = false;
                EventsButton.Style = StackPanelButtonStyle;
                for (int i = _filteredMessages.Count() - 1; i >= 0; i--)
                {
                    if (_filteredMessages[i].Message.Type == MessageType.Event) { _filteredMessages.RemoveAt(i); }
                }
                //
                MyDataGrid.Items.Refresh();
            }
            else
            {
                _showEvents = true;
                EventsButton.Style = StackPanelPressedButtonStyle;
                //
                RefreshFilteredItems();
            }
            UpdateClearAllFilterButtonStyle();
            MyDataGrid.Focus();
        }

        /// <summary>
        /// Clear all message window filters.
        /// </summary>
        private void ClearFiltersButton_Click(object sender, RoutedEventArgs e)
        {
            _showErrors = true;
            _showWarnings = true;
            _showMessages = true;
            _showEvents = true;
            RefreshFilteredItems();
            //Messenger.GetInstance().ClearAllFilters();
            // Update styles
            ErrorsButton.Style = StackPanelPressedButtonStyle;
            WarningsButton.Style = StackPanelPressedButtonStyle;
            MessagesButton.Style = StackPanelPressedButtonStyle;
            EventsButton.Style = StackPanelPressedButtonStyle;

            UpdateClearAllFilterButtonStyle();
            MyDataGrid.Focus();
        }

        /// <summary>
        /// Export message window to text file.
        /// </summary>
        private void ExportToTextFileButton_Click(object sender, RoutedEventArgs e)
        {
            // open save dialog form
            var SaveFileDialog = new SaveFileDialog()
            {
                Title = "Export Message Window to Text File",
                Filter = "Text Files (*.txt)|*.text"
            };
            if (SaveFileDialog.ShowDialog() == true)
            {
                FrameworkInterfaces.Messaging.Messenger.GetInstance().ExportToTextFile(SaveFileDialog.FileName);
            }
            MyDataGrid.Focus();
        }

        /// <summary>
        /// Clear data grid selection when focus is lost.
        /// </summary>
        private void MyDataGrid_LostFocus(object sender, RoutedEventArgs e)
        {
            MyDataGrid.UnselectAll();
        }

        /// <summary>
        /// On double click, execute action for message window item click.
        /// </summary>
        private void MyDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (MyDataGrid.SelectedItems == null || MyDataGrid.SelectedItems.Count != 1) return;
            MessageItem messageItem = (MessageItem)MyDataGrid.SelectedItem;
            if (messageItem.Message.MessageAction != null) messageItem.Message.MessageAction.Invoke(messageItem.Message);
        }

        /// <summary>
        /// Set the data grid column header styles.
        /// </summary>
        private void SetColumnHeaderStyles()
        {
            IconColumn.HeaderStyle = CenterColumnHeaderStyle;
            TimeStampColumn.HeaderStyle = CenterColumnHeaderStyle;
            DescriptionColumn.HeaderStyle = LeftColumnHeaderStyle;
            SourceColumn.HeaderStyle = LeftColumnHeaderStyle;
            NameColumn.HeaderStyle = LeftColumnHeaderStyle;
            ParameterColumn.HeaderStyle = LeftColumnHeaderStyle;
        }


    }
}
