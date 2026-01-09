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

using System.ComponentModel;
using System.IO;
using System.Windows.Media;

namespace FrameworkInterfaces.Messaging
{
    /// <summary>
    /// The Messenger class provides a centralized messaging system for the application.
    /// This class implements the Singleton pattern to ensure only one instance exists.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The Messenger class manages messages, warnings, errors, and events throughout the application.
    /// Messages are keyed by source and code to prevent duplicates while allowing different sources
    /// to have the same message code.
    /// </para>
    /// <para>
    /// <b> Authors: </b>
    /// <list type="bullet">
    ///     <item> Woodrow Fields, USACE Risk Management Center, woodrow.l.fields@usace.army.mil </item>
    ///     <item> Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil </item>
    /// </list>
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// var messenger = Messenger.GetInstance();
    /// messenger.Add(new BasicMessageItem(MessageType.Error, "An error occurred", sourceObject, "Collection", "Name"));
    /// </code>
    /// </example>
    public class Messenger : INotifyPropertyChanged
    {
        #region Singleton Implementation

        /// <summary>
        /// Lazy initialization for thread-safe singleton pattern.
        /// </summary>
        private static readonly Lazy<Messenger> _lazyInstance = new Lazy<Messenger>(() => new Messenger());

        /// <summary>
        /// Private constructor to prevent external instantiation.
        /// </summary>
        private Messenger() { }

        /// <summary>
        /// Gets the singleton instance of the Messenger class.
        /// This method is thread-safe.
        /// </summary>
        /// <returns>The single instance of the Messenger class.</returns>
        public static Messenger GetInstance()
        {
            return _lazyInstance.Value;
        }

        #endregion

        #region Private Fields

        /// <summary>
        /// Dictionary of messages keyed by source object, then by message code.
        /// This structure allows different sources to have the same message code
        /// while preventing duplicate codes from the same source.
        /// </summary>
        private readonly Dictionary<object, Dictionary<string, IMessageItem>> _messagesBySource = new Dictionary<object, Dictionary<string, IMessageItem>>();

        private bool _writeToFile = false;
        private string _textFileName = string.Empty;
        private bool _showErrors = true;
        private bool _showWarnings = true;
        private bool _showMessages = true;
        private bool _showEvents = true;
        private bool _errorBeep;
        private bool _warningBeep;
        private bool _messageBeep;
        private bool _eventBeep;
        private SolidColorBrush _errorColor = new SolidColorBrush(Colors.Red);
        private SolidColorBrush _warningColor = new SolidColorBrush(Colors.DarkOrange);
        private SolidColorBrush _messageColor = new SolidColorBrush(Colors.Blue);
        private SolidColorBrush _eventColor = new SolidColorBrush(Colors.Black);

        #endregion

        #region Events

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Occurs when one or more messages are added to the messenger.
        /// </summary>
        public event MessageAddedEventHandler MessagesAdded;

        /// <summary>
        /// Delegate for the <see cref="MessagesAdded"/> event.
        /// </summary>
        /// <param name="newMessages">Array of newly added message items.</param>
        public delegate void MessageAddedEventHandler(IMessageItem[] newMessages);

        /// <summary>
        /// Occurs when one or more messages are removed from the messenger.
        /// </summary>
        public event MessageRemovedEventHandler MessagesRemoved;

        /// <summary>
        /// Delegate for the <see cref="MessagesRemoved"/> event.
        /// </summary>
        /// <param name="oldMessages">Array of removed message items.</param>
        public delegate void MessageRemovedEventHandler(IMessageItem[] oldMessages);

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the file path for messages to be written to disk.
        /// </summary>
        /// <value>The full file path for the message log file.</value>
        public string TextFileName
        {
            get => _textFileName;
            set
            {
                if (_textFileName == value) { return; }
                _textFileName = value ?? string.Empty;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(TextFileName)));
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether messages should be written to disk.
        /// </summary>
        /// <value><c>true</c> if messages should be written to disk; otherwise, <c>false</c>.</value>
        public bool WriteToFile
        {
            get => _writeToFile;
            set
            {
                if (_writeToFile == value) { return; }
                _writeToFile = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(WriteToFile)));
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether error messages should be shown in the message window.
        /// </summary>
        /// <value><c>true</c> if error messages should be shown; otherwise, <c>false</c>. Default is <c>true</c>.</value>
        public bool ShowErrors
        {
            get { return _showErrors; }
            set
            {
                if (_showErrors != value)
                {
                    _showErrors = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ShowErrors)));
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether warning messages should be shown in the message window.
        /// </summary>
        /// <value><c>true</c> if warning messages should be shown; otherwise, <c>false</c>. Default is <c>true</c>.</value>
        public bool ShowWarnings
        {
            get { return _showWarnings; }
            set
            {
                if (_showWarnings != value)
                {
                    _showWarnings = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ShowWarnings)));
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether normal messages should be shown in the message window.
        /// </summary>
        /// <value><c>true</c> if normal messages should be shown; otherwise, <c>false</c>. Default is <c>true</c>.</value>
        public bool ShowMessages
        {
            get { return _showMessages; }
            set
            {
                if (_showMessages != value)
                {
                    _showMessages = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ShowMessages)));
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether log event messages should be shown in the message window.
        /// </summary>
        /// <value><c>true</c> if event messages should be shown; otherwise, <c>false</c>. Default is <c>true</c>.</value>
        public bool ShowEvents
        {
            get { return _showEvents; }
            set
            {
                if (_showEvents != value)
                {
                    _showEvents = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ShowEvents)));
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether error messages should trigger an audible beep.
        /// </summary>
        /// <value><c>true</c> if error messages should beep; otherwise, <c>false</c>.</value>
        public bool ErrorBeep
        {
            get { return _errorBeep; }
            set
            {
                if (_errorBeep != value)
                {
                    _errorBeep = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ErrorBeep)));
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether warning messages should trigger an audible beep.
        /// </summary>
        /// <value><c>true</c> if warning messages should beep; otherwise, <c>false</c>.</value>
        public bool WarningBeep
        {
            get { return _warningBeep; }
            set
            {
                if (_warningBeep != value)
                {
                    _warningBeep = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(WarningBeep)));
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether normal messages should trigger an audible beep.
        /// </summary>
        /// <value><c>true</c> if messages should beep; otherwise, <c>false</c>.</value>
        public bool MessageBeep
        {
            get { return _messageBeep; }
            set
            {
                if (_messageBeep != value)
                {
                    _messageBeep = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MessageBeep)));
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether event messages should trigger an audible beep.
        /// </summary>
        /// <value><c>true</c> if event messages should beep; otherwise, <c>false</c>.</value>
        public bool EventBeep
        {
            get { return _eventBeep; }
            set
            {
                if (_eventBeep != value)
                {
                    _eventBeep = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(EventBeep)));
                }
            }
        }

        /// <summary>
        /// Gets or sets the color used to display error messages.
        /// </summary>
        /// <value>A <see cref="SolidColorBrush"/> for error message display. Default is Red.</value>
        public SolidColorBrush ErrorColor
        {
            get { return _errorColor; }
            set
            {
                if (value == null) return;
                if (_errorColor == null || _errorColor.Color != value.Color || _errorColor.Opacity != value.Opacity)
                {
                    _errorColor = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ErrorColor)));
                }
            }
        }

        /// <summary>
        /// Gets or sets the color used to display warning messages.
        /// </summary>
        /// <value>A <see cref="SolidColorBrush"/> for warning message display. Default is DarkOrange.</value>
        public SolidColorBrush WarningColor
        {
            get { return _warningColor; }
            set
            {
                if (value == null) return;
                if (_warningColor == null || _warningColor.Color != value.Color || _warningColor.Opacity != value.Opacity)
                {
                    _warningColor = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(WarningColor)));
                }
            }
        }

        /// <summary>
        /// Gets or sets the color used to display normal messages.
        /// </summary>
        /// <value>A <see cref="SolidColorBrush"/> for message display. Default is Blue.</value>
        public SolidColorBrush MessageColor
        {
            get { return _messageColor; }
            set
            {
                if (value == null) return;
                if (_messageColor == null || _messageColor.Color != value.Color || _messageColor.Opacity != value.Opacity)
                {
                    _messageColor = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MessageColor)));
                }
            }
        }

        /// <summary>
        /// Gets or sets the color used to display event messages.
        /// </summary>
        /// <value>A <see cref="SolidColorBrush"/> for event message display. Default is Black.</value>
        public SolidColorBrush EventColor
        {
            get { return _eventColor; }
            set
            {
                if (value == null) return;
                if (_eventColor == null || _eventColor.Color != value.Color || _eventColor.Opacity != value.Opacity)
                {
                    _eventColor = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(EventColor)));
                }
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Adds a message item to the messenger.
        /// </summary>
        /// <param name="item">The message item to add.</param>
        /// <remarks>
        /// For event messages, the code is automatically made unique by appending a counter if necessary.
        /// For other message types, duplicate messages (same source and code) are ignored.
        /// </remarks>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="item"/> is null.</exception>
        public void Add(IMessageItem item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));

            // If it's an event then need to ensure the item code is unique
            if (item.Type == MessageType.Event)
            {
                if (!_messagesBySource.ContainsKey(item.Source))
                {
                    _messagesBySource.Add(item.Source, new Dictionary<string, IMessageItem>());
                }

                // Ensure that the event code is unique by appending counter if needed
                var srcMsgs = _messagesBySource[item.Source];
                string originalCode = item.Code;
                string code = originalCode;
                int counter = 1;

                while (srcMsgs.ContainsKey(code))
                {
                    code = $"{originalCode}{counter}";
                    counter++;
                }
                item.Code = code;

                srcMsgs.Add(item.Code, item);
                MessagesAdded?.Invoke(new IMessageItem[] { item });
            }
            else
            {
                // For non-event messages, ignore duplicates
                if (_messagesBySource.ContainsKey(item.Source) && _messagesBySource[item.Source].ContainsKey(item.Code))
                {
                    return;
                }

                if (!_messagesBySource.ContainsKey(item.Source))
                {
                    _messagesBySource.Add(item.Source, new Dictionary<string, IMessageItem>());
                }

                _messagesBySource[item.Source].Add(item.Code, item);
                MessagesAdded?.Invoke(new IMessageItem[] { item });
            }
        }

        /// <summary>
        /// Adds a collection of message items to the messenger.
        /// </summary>
        /// <param name="items">The collection of message items to add.</param>
        /// <remarks>
        /// Duplicate messages (same source and code) are ignored.
        /// All successfully added messages trigger a single <see cref="MessagesAdded"/> event.
        /// </remarks>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="items"/> is null.</exception>
        public void Add(IEnumerable<IMessageItem> items)
        {
            if (items == null) throw new ArgumentNullException(nameof(items));

            var newMessages = new List<IMessageItem>();
            foreach (IMessageItem item in items)
            {
                if (item == null) continue;

                if (_messagesBySource.ContainsKey(item.Source) && _messagesBySource[item.Source].ContainsKey(item.Code))
                {
                    continue;
                }

                if (!_messagesBySource.ContainsKey(item.Source))
                {
                    _messagesBySource.Add(item.Source, new Dictionary<string, IMessageItem>());
                }

                _messagesBySource[item.Source].Add(item.Code, item);
                newMessages.Add(item);
            }

            if (newMessages.Count > 0)
            {
                MessagesAdded?.Invoke(newMessages.ToArray());
            }
        }

        /// <summary>
        /// Removes a message from the messenger.
        /// </summary>
        /// <param name="message">The message item to remove.</param>
        /// <returns><c>true</c> if the message was successfully removed; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="message"/> is null.</exception>
        public bool Remove(IMessageItem message)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));

            if (!_messagesBySource.ContainsKey(message.Source)) { return false; }
            if (!_messagesBySource[message.Source].ContainsKey(message.Code)) { return false; }

            bool removed = _messagesBySource[message.Source].Remove(message.Code);
            if (removed)
            {
                MessagesRemoved?.Invoke(new IMessageItem[] { message });
            }
            return removed;
        }

        /// <summary>
        /// Removes all messages from the messenger.
        /// </summary>
        public void Clear()
        {
            List<IMessageItem> allMessages = AllMessageItems();
            _messagesBySource.Clear();
            MessagesRemoved?.Invoke(allMessages.ToArray());
        }

        /// <summary>
        /// Removes all messages from a specific source.
        /// </summary>
        /// <param name="source">The source object whose messages should be cleared.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="source"/> is null.</exception>
        public void Clear(object source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            if (!_messagesBySource.ContainsKey(source)) { return; }

            IMessageItem[] allMessages = _messagesBySource[source].Values.ToArray();
            _messagesBySource[source].Clear();
            MessagesRemoved?.Invoke(allMessages);
        }

        /// <summary>
        /// Gets all message items currently stored in the messenger.
        /// </summary>
        /// <returns>A list of all message items from all sources.</returns>
        public List<IMessageItem> AllMessageItems()
        {
            var allMessages = new List<IMessageItem>();
            foreach (var source in _messagesBySource)
            {
                allMessages.AddRange(source.Value.Values);
            }
            return allMessages;
        }

        /// <summary>
        /// Exports all messages to a text file.
        /// </summary>
        /// <param name="fileName">The full path of the file to export to.</param>
        /// <remarks>
        /// If the file already exists, it will be overwritten with the current messages.
        /// </remarks>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="fileName"/> is null or empty.</exception>
        /// <exception cref="IOException">Thrown when an I/O error occurs during file operations.</exception>
        public void ExportToTextFile(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                throw new ArgumentNullException(nameof(fileName));

            // Ensure directory exists
            string directory = Path.GetDirectoryName(fileName);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            // Use using statement for proper resource disposal
            using (var writer = new StreamWriter(fileName, false))
            {
                foreach (IMessageItem message in AllMessageItems())
                {
                    writer.WriteLine(message.ToText());
                }
            }
        }

        #endregion
    }
}
