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
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using FrameworkInterfaces.Undo;
using FrameworkInterfaces.Undo.Actions;

namespace FrameworkInterfaces
{
    /// <summary>
    /// Provides a base class for project elements that can be stored, modified, and validated.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This abstract class provides common functionality for all project elements including:
    /// <list type="bullet">
    /// <item><description>Name and description management</description></item>
    /// <item><description>Creation and modification date tracking</description></item>
    /// <item><description>Dirty state tracking for unsaved changes</description></item>
    /// <item><description>Validation support with error messaging</description></item>
    /// <item><description>Copy, save, open, and delete operations</description></item>
    /// </list>
    /// </para>
    /// <para>
    /// <b> Authors: </b>
    /// <list type="bullet">
    ///     <item> Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil </item>
    ///     <item> Woodrow Fields, USACE Risk Management Center, woodrow.l.fields@usace.army.mil </item>
    /// </list>
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// public class MyElement : ElementBase
    /// {
    ///     public MyElement(string name, IElementCollection parent) : base(name, parent) { }
    ///     // Implement abstract members...
    /// }
    /// </code>
    /// </example>
    public abstract class ElementBase : IElement
    {
        #region Fields

        /// <summary>
        /// The name of the element.
        /// </summary>
        protected string _name;

        /// <summary>
        /// The name of the element as stored on disk.
        /// </summary>
        protected string _nameOnDisk;

        /// <summary>
        /// The description of the element.
        /// </summary>
        protected string _description;

        /// <summary>
        /// The date when the element was created.
        /// </summary>
        protected DateTime _creationDate;

        /// <summary>
        /// The date when the element was last modified.
        /// </summary>
        protected DateTime _lastModified;

        /// <summary>
        /// Indicates whether the element is in a valid state.
        /// </summary>
        protected bool _isValid;

        /// <summary>
        /// The undo manager for this element. Lazily initialized when first accessed.
        /// </summary>
        private IUndoManager _undoManager;

        /// <summary>
        /// Indicates whether undo recording is enabled for this element.
        /// </summary>
        private bool _isUndoEnabled = true;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ElementBase"/> class.
        /// </summary>
        /// <param name="name">The name of the element.</param>
        /// <param name="parentCollection">The parent collection that contains this element.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="name"/> is null or empty.</exception>
        public ElementBase(string name, IElementCollection parentCollection)
        {
            _name = name ?? string.Empty;
            ParentCollection = parentCollection;
            DisplayName = _name;
        }

        #endregion

        #region Abstract Properties

        /// <summary>
        /// Gets or sets the name of the element.
        /// </summary>
        /// <value>The element name.</value>
        public abstract string Name { get; set; }

        /// <summary>
        /// Gets or sets the description of the element.
        /// </summary>
        /// <value>The element description.</value>
        public abstract string Description { get; set; }

        /// <summary>
        /// Gets the date and time when the element was created.
        /// </summary>
        /// <value>The creation date and time.</value>
        public abstract DateTime CreationDate { get; }

        /// <summary>
        /// Gets the date and time when the element was last modified.
        /// </summary>
        /// <value>The last modification date and time.</value>
        public abstract DateTime LastModified { get; }

        /// <summary>
        /// Gets a value indicating whether the element is in a valid state.
        /// </summary>
        /// <value><c>true</c> if the element is valid; otherwise, <c>false</c>.</value>
        public abstract bool IsValid { get; }

        /// <summary>
        /// Gets the name of the element as stored on disk.
        /// </summary>
        /// <value>The on-disk name of the element.</value>
        public abstract string NameOnDisk { get; }

        /// <summary>
        /// Gets the image representation of the element as a Bitmap.
        /// </summary>
        /// <value>The element's icon or image.</value>
        public abstract Bitmap ElementImage { get; }

        /// <summary>
        /// Gets a value indicating whether the element can be copied from an external application.
        /// </summary>
        /// <value><c>true</c> if the element supports external copying; otherwise, <c>false</c>.</value>
        public abstract bool CanCopyFromExternal { get; }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a value indicating whether the element has unsaved changes.
        /// </summary>
        /// <value><c>true</c> if the element has unsaved changes; otherwise, <c>false</c>.</value>
        public bool IsDirty { get; private set; }

        /// <summary>
        /// Gets the display name of the element.
        /// </summary>
        /// <value>
        /// The display name, which includes an asterisk (*) suffix when the element has unsaved changes.
        /// </value>
        public string DisplayName { get; private set; }

        /// <summary>
        /// Gets the parent element collection that contains this element.
        /// </summary>
        /// <value>The parent collection, or <c>null</c> if this element is not in a collection.</value>
        public IElementCollection ParentCollection { get; private set; }

        /// <summary>
        /// Gets the undo manager for this element.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The undo manager is lazily initialized when first accessed. Each element
        /// has its own undo manager, enabling Visual Studio-style per-document undo
        /// where Ctrl+Z operates on the active document.
        /// </para>
        /// <para>
        /// Derived classes can override this property to provide a custom implementation
        /// or to share an undo manager with other elements.
        /// </para>
        /// </remarks>
        public virtual IUndoManager UndoManager
        {
            get
            {
                if (_undoManager == null)
                {
                    _undoManager = new UndoManager();
                }
                return _undoManager;
            }
        }

        /// <summary>
        /// Gets or sets whether undo recording is enabled for this element.
        /// </summary>
        /// <remarks>
        /// Set to <c>false</c> to temporarily disable undo recording, for example
        /// during bulk operations, loading from disk, or undo/redo operations.
        /// Default is <c>true</c>.
        /// </remarks>
        public bool IsUndoEnabled
        {
            get { return _isUndoEnabled; }
            set { _isUndoEnabled = value; }
        }

        #endregion

        #region Events

        /// <summary>
        /// Occurs before the element is deleted, allowing cancellation.
        /// </summary>
        public event PreviewDeletedEventHandler PreviewDeleted;

        /// <summary>
        /// Occurs after the element has been deleted.
        /// </summary>
        public event DeletedEventHandler Deleted;

        /// <summary>
        /// Occurs before the element is saved, allowing cancellation.
        /// </summary>
        public event PreviewObjectSavedEventHandler PreviewObjectSaved;

        /// <summary>
        /// Occurs after the element has been saved.
        /// </summary>
        public event ObjectSavedEventHandler ObjectSaved;

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Protected Methods

        /// <summary>
        /// Sets the dirty state of the element and updates the display name accordingly.
        /// </summary>
        /// <param name="value"><c>true</c> to mark the element as dirty; <c>false</c> to mark it as clean.</param>
        protected void SetIsDirty(bool value)
        {
            if (IsDirty != value)
            {
                IsDirty = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsDirty)));
            }

            if (IsDirty)
            {
                string dirtyName = Name + "*";
                if (DisplayName != dirtyName)
                {
                    DisplayName = dirtyName;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DisplayName)));
                }
            }
            else if (DisplayName != Name)
            {
                DisplayName = Name;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DisplayName)));
            }
        }

        /// <summary>
        /// Raises the <see cref="PropertyChanged"/> event and optionally sets the dirty state.
        /// </summary>
        /// <param name="propertyName">The name of the property that changed.</param>
        /// <param name="isDirty">
        /// <c>true</c> to mark the element as dirty after the property change;
        /// <c>false</c> to leave the dirty state unchanged. Default is <c>true</c>.
        /// </param>
        /// <remarks>
        /// This method is provided for backwards compatibility. For undo support,
        /// use <see cref="RecordPropertyChange"/> instead.
        /// </remarks>
        protected void RaisePropertyChange(string propertyName, bool isDirty = true)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            SetIsDirty(isDirty);
        }

        /// <summary>
        /// Records a property change for undo support and raises the <see cref="PropertyChanged"/> event.
        /// </summary>
        /// <param name="propertyName">The name of the property that changed.</param>
        /// <param name="oldValue">The previous value of the property.</param>
        /// <param name="newValue">The new value of the property.</param>
        /// <param name="isDirty">
        /// <c>true</c> to mark the element as dirty after the property change;
        /// <c>false</c> to leave the dirty state unchanged. Default is <c>true</c>.
        /// </param>
        /// <remarks>
        /// <para>
        /// Call this method from property setters to enable undo/redo support.
        /// If <see cref="IsUndoEnabled"/> is <c>false</c> or the undo manager is currently
        /// executing an action (during undo/redo), the change will not be recorded.
        /// </para>
        /// <para>
        /// Example usage in a derived class:
        /// <code>
        /// public override string Name
        /// {
        ///     get => _name;
        ///     set
        ///     {
        ///         if (_name != value)
        ///         {
        ///             var oldValue = _name;
        ///             _name = value;
        ///             ValidateName();
        ///             RecordPropertyChange(nameof(Name), oldValue, value);
        ///         }
        ///     }
        /// }
        /// </code>
        /// </para>
        /// </remarks>
        protected void RecordPropertyChange(string propertyName, object oldValue, object newValue, bool isDirty = true)
        {
            // Record the action for undo if enabled and not currently executing an undo/redo
            if (IsUndoEnabled && !UndoManager.IsExecutingAction && !Equals(oldValue, newValue))
            {
                var action = new PropertyChangeAction(this, propertyName, oldValue, newValue);
                UndoManager.RecordAction(action);
            }

            // Always raise the property changed event and update dirty state
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            SetIsDirty(isDirty);
        }

        /// <summary>
        /// Clears the undo history for this element.
        /// </summary>
        /// <remarks>
        /// Call this after loading data from disk or when you want to establish
        /// a clean state with no undo history.
        /// </remarks>
        protected void ClearUndoHistory()
        {
            _undoManager?.Clear();
        }

        /// <summary>
        /// Marks the current state as the saved state in the undo manager.
        /// </summary>
        /// <remarks>
        /// Call this after successfully saving the element. This updates the
        /// <see cref="IUndoManager.HasChangedSinceSave"/> property.
        /// </remarks>
        protected void MarkUndoSavePoint()
        {
            _undoManager?.MarkSavePoint();
        }

        /// <summary>
        /// Raises the <see cref="PreviewObjectSaved"/> event.
        /// </summary>
        /// <param name="sender">The object to be saved.</param>
        /// <param name="cancel">
        /// When this method returns, contains <c>true</c> if the save operation should be cancelled;
        /// otherwise, <c>false</c>.
        /// </param>
        protected void RaisePreviewObjectSaved(ISave sender, ref bool cancel)
        {
            PreviewObjectSaved?.Invoke(sender, ref cancel);
        }

        /// <summary>
        /// Raises the <see cref="ObjectSaved"/> event.
        /// </summary>
        /// <param name="sender">The object that was saved.</param>
        protected void RaiseObjectSaved(ISave sender)
        {
            ObjectSaved?.Invoke(sender);
        }

        /// <summary>
        /// Raises the <see cref="PreviewDeleted"/> event.
        /// </summary>
        /// <param name="element">The element to be deleted.</param>
        /// <param name="cancel">
        /// When this method returns, contains <c>true</c> if the deletion should be cancelled;
        /// otherwise, <c>false</c>.
        /// </param>
        protected void RaisePreviewDeleted(IElement element, ref bool cancel)
        {
            PreviewDeleted?.Invoke(element, ref cancel);
        }

        /// <summary>
        /// Raises the <see cref="Deleted"/> event.
        /// </summary>
        /// <param name="element">The element that was deleted.</param>
        protected void RaiseDeleted(IElement element)
        {
            Deleted?.Invoke(element);
        }

        /// <summary>
        /// Validates the element's name and generates appropriate error messages.
        /// </summary>
        /// <param name="invalidCharacters">An array of characters that are not allowed in the element name.</param>
        /// <param name="maxCharacters">The maximum number of characters allowed for the element name.</param>
        /// <param name="errorCodeSource">
        /// The prefix for error codes, typically in the format "XX-XXX" which will be combined
        /// with error numbers (e.g., "XX-XXX-ERR-001").
        /// </param>
        /// <returns><c>true</c> if the name is valid; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// <para>
        /// This method performs the following validations:
        /// <list type="bullet">
        /// <item><description>ERR-001: Name cannot be blank</description></item>
        /// <item><description>ERR-002: Name cannot exceed maximum character limit</description></item>
        /// <item><description>ERR-003: Name must be unique within the parent collection</description></item>
        /// <item><description>ERR-004: Name cannot contain invalid characters</description></item>
        /// </list>
        /// </para>
        /// <para>
        /// Error messages are automatically added to or removed from the <see cref="Messaging.Messenger"/> singleton.
        /// </para>
        /// </remarks>
        protected bool ValidateName(char[] invalidCharacters, int maxCharacters, string errorCodeSource)
        {
            bool valid = true;
            var nameMessages = new List<BasicMessageItem>();

            // Get Parent Name
            string parentCollectionName = ParentCollection?.Name ?? "Study Element";

            // Check if name is null or empty
            if (string.IsNullOrEmpty(_name))
            {
                valid = false;
                nameMessages.Add(new BasicMessageItem(
                    MessageType.Error,
                    $"The name of the {parentCollectionName} cannot be blank.",
                    this, parentCollectionName, _name ?? string.Empty, nameof(Name),
                    $"{errorCodeSource}-ERR-001"));
            }
            else
            {
                Messaging.Messenger.GetInstance().Remove(new BasicMessageItem(
                    MessageType.Error,
                    $"The name of the {parentCollectionName} cannot be blank.",
                    this, parentCollectionName, _name, nameof(Name),
                    $"{errorCodeSource}-ERR-001"));

                // Only check length and characters if name is not null/empty
                if (_name.Length > maxCharacters)
                {
                    valid = false;
                    nameMessages.Add(new BasicMessageItem(
                        MessageType.Error,
                        $"The name of the {parentCollectionName} cannot exceed {maxCharacters} characters.",
                        this, parentCollectionName, _name, nameof(Name),
                        $"{errorCodeSource}-ERR-002"));
                }
                else
                {
                    Messaging.Messenger.GetInstance().Remove(new BasicMessageItem(
                        MessageType.Error,
                        $"The name of the {parentCollectionName} cannot exceed {maxCharacters} characters.",
                        this, parentCollectionName, _name, nameof(Name),
                        $"{errorCodeSource}-ERR-002"));
                }

                // Get list of invalid names, can't allow duplicate naming
                var invalidNames = new List<string>();
                if (ParentCollection != null)
                {
                    invalidNames = ParentCollection.GetElementNames(this);
                }

                if (invalidNames.Contains(_name))
                {
                    valid = false;
                    nameMessages.Add(new BasicMessageItem(
                        MessageType.Error,
                        $"A {parentCollectionName} with the name '{_name}' already exists and must be unique.",
                        this, parentCollectionName, _name, nameof(Name),
                        $"{errorCodeSource}-ERR-003"));
                }
                else
                {
                    Messaging.Messenger.GetInstance().Remove(new BasicMessageItem(
                        MessageType.Error,
                        $"A {parentCollectionName} with the name '{_name}' already exists and must be unique.",
                        this, parentCollectionName, _name, nameof(Name),
                        $"{errorCodeSource}-ERR-003"));
                }

                // Check for invalid characters
                bool containsBadChar = false;
                if (invalidCharacters != null)
                {
                    foreach (char badChar in invalidCharacters)
                    {
                        if (_name.Contains(badChar.ToString()))
                        {
                            valid = false;
                            string badChars = string.Join(" ", invalidCharacters
                                .Where(c => !char.IsWhiteSpace(c) && !char.IsControl(c)));
                            nameMessages.Add(new BasicMessageItem(
                                MessageType.Error,
                                $"Invalid character in {parentCollectionName} name: '{badChar}'. Invalid characters are: {badChars}",
                                this, parentCollectionName, _name, nameof(Name),
                                $"{errorCodeSource}-ERR-004"));
                            containsBadChar = true;
                            break;
                        }
                    }
                }

                if (!containsBadChar)
                {
                    Messaging.Messenger.GetInstance().Remove(new BasicMessageItem(
                        MessageType.Error, string.Empty,
                        this, parentCollectionName, _name, nameof(Name),
                        $"{errorCodeSource}-ERR-004"));
                }
            }

            if (nameMessages.Count > 0)
            {
                Messaging.Messenger.GetInstance().Add(nameMessages);
            }

            return valid;
        }

        #endregion

        #region Abstract Methods

        /// <summary>
        /// Loads element data from disk.
        /// </summary>
        public abstract void Open();

        /// <summary>
        /// Saves the element data to disk.
        /// </summary>
        public abstract void Save();

        /// <summary>
        /// Creates a deep copy of the element.
        /// </summary>
        /// <param name="newName">
        /// Optional new name for the cloned element. If <c>null</c>, a unique name is generated.
        /// </param>
        /// <returns>A new <see cref="IElement"/> that is a deep copy of this element.</returns>
        public abstract IElement Copy(string newName = null);

        /// <summary>
        /// Copies an element from an external project file.
        /// </summary>
        /// <param name="itemName">The name of the item to copy from the external project.</param>
        /// <param name="fullFileName">The full file path of the external project file.</param>
        /// <returns>
        /// A new <see cref="IElement"/> copied from the external project,
        /// or <c>null</c> if the copy operation fails.
        /// </returns>
        public abstract IElement CopyFromExternal(string itemName, string fullFileName);

        /// <summary>
        /// Deletes the element from disk and removes it from the parent element collection.
        /// </summary>
        public abstract void Delete();

        #endregion
    }
}
