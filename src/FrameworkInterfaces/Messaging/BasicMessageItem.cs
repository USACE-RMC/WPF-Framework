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

namespace FrameworkInterfaces
{
    /// <summary>
    /// Represents a basic message item for the messaging system.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This class provides a concrete implementation of <see cref="IMessageItem"/> for
    /// general-purpose messaging including errors, warnings, messages, and events.
    /// Messages can be associated with source objects to enable navigation to the
    /// source when the message is clicked.
    /// </para>
    /// <para>
    /// <b> Authors: </b>
    /// <list type="bullet">
    ///     <item> Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil </item>
    ///     <item> Woodrow Lee Fields, USACE Risk Management Center, woodrow.l.fields@usace.army.mil </item>
    /// </list>
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// var errorMessage = new BasicMessageItem(
    ///     MessageType.Error,
    ///     "Validation failed",
    ///     sourceElement,
    ///     "Elements",
    ///     "MyElement",
    ///     "Name",
    ///     "EL-ERR-001");
    /// Messenger.GetInstance().Add(errorMessage);
    /// </code>
    /// </example>
    public class BasicMessageItem : IMessageItem, IEquatable<IMessageItem>
    {
        #region Fields

        private Action<IMessageItem>? _messageAction;
        private MessageType _type;
        private string _code = "MI-EVT-000";
        private string? _description;
        private object? _source;
        private string? _sourceCollectionName;
        private string? _sourceName;
        private string? _parameterName;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BasicMessageItem"/> class with default values.
        /// </summary>
        public BasicMessageItem()
        {
            TimeStamp = DateTime.Now.ToString("HH:mm:ss");
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BasicMessageItem"/> class with specified properties.
        /// </summary>
        /// <param name="type">The type of message (Error, Warning, Message, or Event).</param>
        /// <param name="description">The message description text.</param>
        /// <param name="source">The source object that generated this message.</param>
        /// <param name="sourceCollectionName">The name of the collection containing the source.</param>
        /// <param name="sourceName">The name of the source element.</param>
        /// <param name="parameterName">
        /// Optional. The name of the parameter associated with the message. Default is <c>null</c>.
        /// </param>
        /// <param name="code">
        /// Optional. The unique code identifying this message. Default is "MI-EVT-000".
        /// </param>
        /// <remarks>
        /// The source information (collection name, element name, and parameter name) enables
        /// navigation to the source when the message is clicked in the UI.
        /// </remarks>
        public BasicMessageItem(
            MessageType type,
            string description,
            object source,
            string sourceCollectionName,
            string sourceName,
            string? parameterName = null,
            string code = "MI-EVT-000")
        {
            Type = type;
            Description = description;
            Source = source;
            SourceCollectionName = sourceCollectionName;
            SourceName = sourceName;
            ParameterName = parameterName;
            Code = code;
            TimeStamp = DateTime.Now.ToString("HH:mm:ss");
        }

        #endregion

        #region Events

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the action to be invoked when the message is clicked.
        /// </summary>
        /// <value>An action that receives the message item when invoked.</value>
        public Action<IMessageItem>? MessageAction
        {
            get { return _messageAction; }
            set
            {
                if (_messageAction == null || !_messageAction.Equals(value))
                {
                    _messageAction = value;
                    RaisePropertyChange(nameof(MessageAction));
                }
            }
        }

        /// <summary>
        /// Gets or sets the unique code identifying this message.
        /// </summary>
        /// <value>
        /// A string code in the format "XX-XXX-###" (e.g., "MI-EVT-000").
        /// Default is "MI-EVT-000" for events.
        /// </value>
        public string Code
        {
            get { return _code; }
            set
            {
                if (_code != value)
                {
                    _code = value ?? "MI-EVT-000";
                    RaisePropertyChange(nameof(Code));
                }
            }
        }

        /// <summary>
        /// Gets or sets the type of message.
        /// </summary>
        /// <value>A <see cref="MessageType"/> value indicating the severity or category.</value>
        public MessageType Type
        {
            get { return _type; }
            set
            {
                if (_type != value)
                {
                    _type = value;
                    RaisePropertyChange(nameof(Type));
                }
            }
        }

        /// <summary>
        /// Gets or sets the description text of the message.
        /// </summary>
        /// <value>A human-readable description of the message.</value>
        public string? Description
        {
            get { return _description; }
            set
            {
                if (_description != value)
                {
                    _description = value;
                    RaisePropertyChange(nameof(Description));
                }
            }
        }

        /// <summary>
        /// Gets or sets the source object that generated this message.
        /// </summary>
        /// <value>The object that is the source of the message, or <c>null</c>.</value>
        public object? Source
        {
            get { return _source; }
            set
            {
                if (_source == null || !_source.Equals(value))
                {
                    _source = value;
                    RaisePropertyChange(nameof(Source));
                }
            }
        }

        /// <summary>
        /// Gets or sets the name of the collection containing the source element.
        /// </summary>
        /// <value>The collection name, used for navigation to the source.</value>
        public string? SourceCollectionName
        {
            get { return _sourceCollectionName; }
            set
            {
                if (_sourceCollectionName != value)
                {
                    _sourceCollectionName = value;
                    RaisePropertyChange(nameof(SourceCollectionName));
                }
            }
        }

        /// <summary>
        /// Gets or sets the name of the source element.
        /// </summary>
        /// <value>The element name, used for navigation to the source.</value>
        public string? SourceName
        {
            get { return _sourceName; }
            set
            {
                if (_sourceName != value)
                {
                    _sourceName = value;
                    RaisePropertyChange(nameof(SourceName));
                }
            }
        }

        /// <summary>
        /// Gets or sets the name of the parameter associated with the message.
        /// </summary>
        /// <value>The parameter name, or <c>null</c> if not applicable.</value>
        public string? ParameterName
        {
            get { return _parameterName; }
            set
            {
                if (_parameterName != value)
                {
                    _parameterName = value;
                    RaisePropertyChange(nameof(ParameterName));
                }
            }
        }

        /// <summary>
        /// Gets the timestamp when the message was created.
        /// </summary>
        /// <value>A string in "HH:mm:ss" format representing the creation time.</value>
        public string TimeStamp { get; private set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Raises the <see cref="PropertyChanged"/> event.
        /// </summary>
        /// <param name="propertyName">The name of the property that changed.</param>
        public void RaisePropertyChange(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Converts this message item to a text representation.
        /// </summary>
        /// <returns>A formatted string containing all message properties.</returns>
        public string ToText()
        {
            return $"Type: {Type}; Time: {TimeStamp}; Description: {Description}; " +
                   $"Source: {SourceCollectionName}; Name: {SourceName}; Parameter: {ParameterName}.";
        }

        /// <summary>
        /// Returns a string representation of this message item.
        /// </summary>
        /// <returns>A formatted string containing all message properties.</returns>
        public override string ToString()
        {
            return ToText();
        }

        #endregion

        #region Equality Members

        /// <summary>
        /// Determines whether the specified object is equal to the current message item.
        /// </summary>
        /// <param name="obj">The object to compare with the current message item.</param>
        /// <returns>
        /// <c>true</c> if the specified object is an <see cref="IMessageItem"/> and is equal
        /// to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object? obj)
        {
            if (obj == null) return false;

            // Use safe cast instead of direct cast to prevent InvalidCastException
            if (obj is IMessageItem message)
            {
                return Equals(message);
            }

            return false;
        }

        /// <summary>
        /// Determines whether the specified message item is equal to the current message item.
        /// </summary>
        /// <param name="other">The message item to compare with the current message item.</param>
        /// <returns>
        /// <c>true</c> if the messages have the same code and equivalent sources;
        /// otherwise, <c>false</c>.
        /// </returns>
        /// <remarks>
        /// Two message items are considered equal if they have the same code and their
        /// sources are equivalent. Source comparison depends on the source type:
        /// <list type="bullet">
        /// <item><description>For <see cref="IElement"/> sources: parent collection name and element name must match</description></item>
        /// <item><description>For <see cref="IProject"/> sources: project names must match</description></item>
        /// </list>
        /// </remarks>
        public bool Equals(IMessageItem? other)
        {
            if (other == null) return false;

            // Compare codes first (null-safe)
            if (!string.Equals(Code, other.Code, StringComparison.Ordinal))
            {
                return false;
            }

            // Both sources must be non-null for comparison
            if (Source == null || other.Source == null)
            {
                return false;
            }

            // Compare IElement sources
            if (Source is IElement element && other.Source is IElement otherElement)
            {
                // Null-safe comparison of parent collection names
                string? elementCollectionName = element.ParentCollection?.Name;
                string? otherElementCollectionName = otherElement.ParentCollection?.Name;

                return string.Equals(elementCollectionName, otherElementCollectionName, StringComparison.Ordinal) &&
                       string.Equals(element.Name, otherElement.Name, StringComparison.Ordinal);
            }

            // Compare IProject sources
            if (Source is IProject project && other.Source is IProject otherProject)
            {
                return string.Equals(project.Name, otherProject.Name, StringComparison.Ordinal);
            }

            // Source types don't match
            return false;
        }

        /// <summary>
        /// Returns a hash code for this message item.
        /// </summary>
        /// <returns>
        /// A hash code based on the message code and source information.
        /// </returns>
        /// <remarks>
        /// The hash code is computed to be consistent with <see cref="Equals(IMessageItem)"/>.
        /// </remarks>
        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;

                // Include code in hash
                hash = hash * 31 + (Code?.GetHashCode() ?? 0);

                // Include source-specific information
                if (Source is IElement element)
                {
                    hash = hash * 31 + (element.ParentCollection?.Name?.GetHashCode() ?? 0);
                    hash = hash * 31 + (element.Name?.GetHashCode() ?? 0);
                }
                else if (Source is IProject project)
                {
                    hash = hash * 31 + (project.Name?.GetHashCode() ?? 0);
                }
                else if (Source != null)
                {
                    hash = hash * 31 + Source.GetHashCode();
                }

                return hash;
            }
        }

        /// <summary>
        /// Determines whether two message items are equal.
        /// </summary>
        /// <param name="left">The first message item to compare.</param>
        /// <param name="right">The second message item to compare.</param>
        /// <returns><c>true</c> if the message items are equal; otherwise, <c>false</c>.</returns>
        public static bool operator ==(BasicMessageItem left, BasicMessageItem right)
        {
            if (ReferenceEquals(left, null))
            {
                return ReferenceEquals(right, null);
            }
            return left.Equals(right);
        }

        /// <summary>
        /// Determines whether two message items are not equal.
        /// </summary>
        /// <param name="left">The first message item to compare.</param>
        /// <param name="right">The second message item to compare.</param>
        /// <returns><c>true</c> if the message items are not equal; otherwise, <c>false</c>.</returns>
        public static bool operator !=(BasicMessageItem left, BasicMessageItem right)
        {
            return !(left == right);
        }

        #endregion
    }
}
