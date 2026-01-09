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

using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using FrameworkInterfaces.Undo;
using FrameworkInterfaces.Undo.Actions;

namespace FrameworkInterfaces
{
    /// <summary>
    /// This is a base class for project element collections.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b> Authors: </b>
    /// <list type="bullet">
    ///     <item> Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil </item>
    ///     <item> Woodrow Fields, USACE Risk Management Center, woodrow.l.fields@usace.army.mil </item>
    /// </list>
    /// </para>
    /// </remarks>
    public abstract class ElementCollectionBase : IElementCollection
    {

        /// <summary>
        /// Construct new element collection with name and parent project.
        /// </summary>
        /// <param name="parentProject">Parent project of the collection.</param>
        public ElementCollectionBase(IProject parentProject)
        {
            ParentProject = parentProject;
            _readOnlyElementCollections = new ReadOnlyCollection<IElementCollection>(_elementCollections);
        }

        /// <summary>
        /// The element collection.
        /// </summary>
        protected readonly List<IElement> ElementList = new List<IElement>();

        /// <summary>
        /// Get and set the name of the element collection.
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// Gets the parent project of the element collection.
        /// </summary>
        public IProject ParentProject { get; private set; }

        protected List<IElementCollection> _elementCollections = new List<IElementCollection>();
        private ReadOnlyCollection<IElementCollection> _readOnlyElementCollections;
        protected bool _savingAll = false;
        protected bool _opening = false;
        protected string _nameOnDisk;

        /// <summary>
        /// The undo manager for collection-level operations. Lazily initialized.
        /// </summary>
        private IUndoManager _undoManager;

        /// <summary>
        /// Indicates whether undo recording is enabled for this collection.
        /// </summary>
        private bool _isUndoEnabled = true;

        ///<summary>
        /// Gets and sets the read only collection of element collections.
        ///</summary>
        public ReadOnlyCollection<IElementCollection> ElementCollections
        {
            get { return _readOnlyElementCollections; }
            set { _readOnlyElementCollections = value; }
        }

        /// <summary>
        /// Get and sets the element at the specific index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get or set.</param>
        public IElement this[int index]
        {
            get { return ElementList[index]; }
            set { ElementList[index] = value; }
        }

        /// <summary>
        /// Get the number of elements contained in the collection.
        /// </summary>
        public int Count
        {
            get { return ElementList.Count; }
        }

        /// <summary>
        /// Gets whether the list is read only.
        /// </summary>
        public bool IsReadOnly
        {
            get { return false; }
        }

        /// <summary>
        /// Gets whether the element collection is dirty.
        /// </summary>
        public bool IsDirty { get; private set; }

        /// <summary>
        /// Represents the name as it appears on disk.
        /// </summary>
        public string NameOnDisk
        {
            get { return _nameOnDisk; }
        }

        /// <summary>
        /// Gets the undo manager for collection-level operations (add, remove, move).
        /// </summary>
        /// <remarks>
        /// <para>
        /// The undo manager is lazily initialized when first accessed. This manager
        /// tracks collection-level operations such as adding, removing, or moving elements.
        /// </para>
        /// <para>
        /// For element property changes, use the element's own <see cref="ElementBase.UndoManager"/>.
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
        /// Gets or sets whether undo recording is enabled for this collection.
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

        /// <summary>
        /// Private sub used to set is dirty.
        /// </summary>
        /// <param name="value">Determines if IsDirty = True or False.</param>
        protected void SetIsDirty(bool value)
        {
            if (IsDirty != value)
            {
                IsDirty = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsDirty)));
            }
        }

        /// <summary>
        /// Event is raised when adding an element.
        /// </summary>
        public event ElementAddedEventHandler ElementAdded;

        /// <summary>
        /// Event is raised when removing an element.
        /// </summary>
        public event ElementRemovedEventHandler ElementRemoved;

        /// <summary>
        /// Event is raised event before an object has been saved.
        /// </summary>
        public event PreviewObjectSavedEventHandler PreviewObjectSaved;

        /// <summary>
        /// Event is raised when the object has been saved.
        /// </summary>
        /// <param name="sender">The object that was saved.</param>
        public event ObjectSavedEventHandler ObjectSaved;

        /// <summary>
        /// Event is raised whenever a property changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        

        /// <summary>
        /// Raise property changed event.
        /// </summary>
        /// <param name="propertyName">Name of property that changed.</param>
        public void RaisePropertyChange(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            SetIsDirty(true);
        }

        /// <summary>
        /// Load all elements into the collection from disk.
        /// </summary>
        public abstract void Open();

        /// <summary>
        /// Save all elements in the collection to disk.
        /// </summary>
        public virtual void Save()
        {
            bool cancel = false;
            PreviewObjectSaved?.Invoke(this, ref cancel);
            if (cancel == true) return;

            _savingAll = true;
            if (IsDirty == true)
            {
                // Save all elements in order as the order may have changed.
                for (int i = 0; i < ElementList.Count; i++)
                    ElementList[i].Save();
            }
            else
            {
                // Element collection is unchanged, so
                // only save elements that are dirty
                for (int i = 0; i < ElementList.Count; i++)
                {
                    if (ElementList[i].IsDirty == true)
                        ElementList[i].Save();
                }
            }

            // Save child element collections
            foreach (var child in _elementCollections)
            {
                child.Save();
            }

            SetIsDirty(false);
            ObjectSaved?.Invoke(this);
            _savingAll = false;
        }

        /// <summary>
        /// Move element within collection.
        /// </summary>
        /// <param name="element">The element to move.</param>
        /// <param name="startIndex">The start position of the element to move.</param>
        /// <param name="endIndex">The end position of the element to move.</param>
        public virtual void MoveElement(IElement element, int startIndex, int endIndex)
        {
            ElementList.RemoveAt(startIndex);
            ElementList.Insert(endIndex, element);
            SetIsDirty(true);
        }


        /// <summary>
        /// Sort the elements in the collection using a specified comparer.
        /// </summary>
        /// <param name="sortAction">Comparer.</param>
        public void Sort(IComparer<IElement> sortAction)
        {
            ElementList.Sort(sortAction);
            SetIsDirty(true);
        }

        /// <summary>
        /// Add element to the collection.
        /// </summary>
        /// <param name="item">Element to add.</param>
        public abstract void Add(IElement item);

        /// <summary>
        /// Copy the element from an external project to disk within the current project.
        /// </summary>
        /// <param name="index">The zero-based index at which the element should be inserted.</param>
        /// <param name="elementName">The element name.</param>
        /// <param name="elementType">The element type.</param>
        /// <param name="fullFileName">The full file name of the project to copy from.</param>
        public abstract void InsertFromExternalProject(int index, string elementName, string elementType, string fullFileName);

        /// <summary>
        /// Inserts an element into the collection at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which the element should be inserted.</param>
        /// <param name="item">Element to insert.</param>
        public abstract void Insert(int index, IElement item);

        /// <summary>
        /// Removes the first occurrence of the specified data object.
        /// </summary>
        /// <param name="item">The object to remove from the collection.</param>
        public bool Remove(IElement item)
        {
            if (ElementList.Remove(item) == true)
            {
                item.PropertyChanged -= ElementPropertyChanged;
                item.Deleted -= ElementDeleted;
                RaisePropertyChange(nameof(ElementList));
                SetIsDirty(true);
                RaiseElementRemoved(item);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Remove element at the specified index of the collection.
        /// </summary>
        /// <param name="index">The zero-based index of the element to remove.</param>
        public void RemoveAt(int index)
        {
            Remove(ElementList[index]);
        }

        /// <summary>
        /// Removed all elements from the collection.
        /// </summary>
        public void Clear()
        {
            for (int i = ElementList.Count - 1; i >= 0; i -= 1)
            {
                Remove(ElementList[i]);
            }
            foreach (var collection in ElementCollections)
            {
                collection.Clear();
            }
        }

        /// <summary>
        /// Searches for the specified object and returns the zero-based index of the first occurrence within the entire collection.
        /// </summary>
        /// <param name="item">The object to locate in the collection.</param>
        public int IndexOf(IElement item)
        {
            return ElementList.IndexOf(item);
        }

        /// <summary>
        /// Determines whether an element is the collection.
        /// </summary>
        /// <param name="item">The item to locate.</param>
        public bool Contains(IElement item)
        {
            return ElementList.Contains(item);
        }

        /// <summary>
        /// Copies the entire collection to a compatible one-dimensional array starting at the specified index of the target array.
        /// </summary>
        /// <param name="array">The one-dimensional array that is the destination of the copied elements.</param>
        /// <param name="arrayIndex">The zero-based index in the array at which copying begins.</param>
        public void CopyTo(IElement[] array, int arrayIndex)
        {
            ElementList.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Delete the element collection and remove all data from disk.
        /// </summary>
        public abstract void Delete();

        public IEnumerator<IElement> GetEnumerator()
        {
            return ElementList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// The element was deleted, so remove element from collection.
        /// </summary>
        protected void ElementDeleted(IElement sender)
        {
            Remove(sender);
        }

        /// <summary>
        /// A property in the element has changed.
        /// </summary>
        protected void ElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName  == nameof(IElement.Name))
            {
                RaisePropertyChange(e.PropertyName);
            }
        }

        /// <summary>
        /// Event is raised event before an object has been saved.
        /// </summary>
        /// <param name="sender">The object to be saved.</param>
        /// <param name="cancel">Determines if the save should be canceled.</param>
        protected void RaisePreviewObjectSaved(ISave sender, ref bool cancel)
        {
            PreviewObjectSaved?.Invoke(sender, ref cancel);
        }

        /// <summary>
        /// Raise element collection saved.
        /// </summary>
        protected void RaiseObjectSaved()
        {
            ObjectSaved?.Invoke(this);
        }

        /// <summary>
        /// Raise element added event.
        /// </summary>
        /// <param name="element">Element that was added.</param>
        protected void RaiseElementAddedEvent(IElement element)
        {
            ElementAdded?.Invoke(element);
        }

        /// <summary>
        /// Raise element removed event.
        /// </summary>
        /// <param name="element">Element that was removed.</param>
        protected void RaiseElementRemoved(IElement element)
        {
            ElementRemoved?.Invoke(element);
        }

        /// <summary>
        /// Records an element addition for undo support.
        /// </summary>
        /// <param name="element">The element that was added.</param>
        /// <param name="index">The index where the element was inserted.</param>
        /// <remarks>
        /// Call this method after adding an element to enable undo/redo support.
        /// If <see cref="IsUndoEnabled"/> is <c>false</c> or the undo manager is currently
        /// executing an action (during undo/redo), the change will not be recorded.
        /// </remarks>
        protected void RecordAddElement(IElement element, int index = -1)
        {
            if (IsUndoEnabled && !UndoManager.IsExecutingAction)
            {
                var action = new AddElementAction(this, element, index >= 0 ? index : ElementList.IndexOf(element));
                UndoManager.RecordAction(action);
            }
        }

        /// <summary>
        /// Records an element removal for undo support.
        /// </summary>
        /// <param name="element">The element that will be removed.</param>
        /// <remarks>
        /// Call this method BEFORE removing an element to capture its index.
        /// If <see cref="IsUndoEnabled"/> is <c>false</c> or the undo manager is currently
        /// executing an action (during undo/redo), the change will not be recorded.
        /// </remarks>
        protected void RecordRemoveElement(IElement element)
        {
            if (IsUndoEnabled && !UndoManager.IsExecutingAction)
            {
                var action = new RemoveElementAction(this, element);
                UndoManager.RecordAction(action);
            }
        }

        /// <summary>
        /// Records an element move for undo support.
        /// </summary>
        /// <param name="element">The element that was moved.</param>
        /// <param name="oldIndex">The original index of the element.</param>
        /// <param name="newIndex">The new index of the element.</param>
        /// <remarks>
        /// Call this method after moving an element to enable undo/redo support.
        /// If <see cref="IsUndoEnabled"/> is <c>false</c> or the undo manager is currently
        /// executing an action (during undo/redo), the change will not be recorded.
        /// </remarks>
        protected void RecordMoveElement(IElement element, int oldIndex, int newIndex)
        {
            if (IsUndoEnabled && !UndoManager.IsExecutingAction && oldIndex != newIndex)
            {
                var action = new MoveElementAction(this, element, oldIndex, newIndex);
                UndoManager.RecordAction(action);
            }
        }

        /// <summary>
        /// Clears the undo history for this collection.
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
        /// Call this after successfully saving the collection. This updates the
        /// <see cref="IUndoManager.HasChangedSinceSave"/> property.
        /// </remarks>
        protected void MarkUndoSavePoint()
        {
            _undoManager?.MarkSavePoint();
        }

    }
}
