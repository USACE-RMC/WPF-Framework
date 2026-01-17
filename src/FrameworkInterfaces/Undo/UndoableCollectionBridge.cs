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
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using FrameworkInterfaces.Undo.Actions;

namespace FrameworkInterfaces.Undo
{
    /// <summary>
    /// Bridges an <see cref="ObservableCollection{T}"/> to the undo/redo system by automatically
    /// recording collection changes as undoable actions.
    /// </summary>
    /// <typeparam name="T">The type of elements in the collection.</typeparam>
    /// <remarks>
    /// <para>
    /// This class subscribes to the <see cref="ObservableCollection{T}.CollectionChanged"/> event
    /// and creates appropriate <see cref="IUndoableAction"/> instances for each change type
    /// (Add, Remove, Replace, Reset, Move). These actions are recorded with the
    /// <see cref="IUndoManager"/> for undo/redo support.
    /// </para>
    /// <para>
    /// The bridge maintains a shadow copy of the collection to support Reset (Clear) operations,
    /// which do not provide the removed items in their event arguments.
    /// </para>
    /// <para>
    /// The bridge checks <see cref="IUndoManager.IsExecutingAction"/> to avoid recording
    /// changes that occur during undo/redo operations, preventing infinite recursion.
    /// </para>
    /// <para>
    /// This class implements <see cref="IDisposable"/> and should be disposed when the
    /// collection is no longer needed to prevent memory leaks from event subscriptions.
    /// </para>
    /// <para>
    /// <b> Authors: </b>
    /// <list type="bullet">
    ///     <item> Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil </item>
    /// </list>
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// // Create the bridge in your element constructor
    /// public class MyElement : ElementBase, IUndoableElement
    /// {
    ///     private ObservableCollection&lt;double&gt; _values;
    ///     private UndoableCollectionBridge&lt;double&gt; _valuesBridge;
    ///
    ///     public MyElement()
    ///     {
    ///         _values = new ObservableCollection&lt;double&gt;();
    ///         _valuesBridge = new UndoableCollectionBridge&lt;double&gt;(
    ///             _values,
    ///             () => IsUndoEnabled ? _undoManager : null,
    ///             "values"
    ///         );
    ///     }
    /// }
    /// </code>
    /// </example>
    public class UndoableCollectionBridge<T> : IDisposable
    {
        #region Fields

        /// <summary>
        /// The collection being monitored for changes.
        /// </summary>
        private readonly ObservableCollection<T> _collection;

        /// <summary>
        /// A function that returns the current undo manager, or null if undo is disabled.
        /// Using a delegate allows dynamic checking of IsUndoEnabled.
        /// </summary>
        private readonly Func<IUndoManager> _getUndoManager;

        /// <summary>
        /// A human-readable description of the collection for use in undo action descriptions.
        /// </summary>
        private readonly string _collectionDescription;

        /// <summary>
        /// The target object that owns this collection, used for undo action association.
        /// </summary>
        private readonly object _target;

        /// <summary>
        /// A shadow copy of the collection used to capture items during Reset (Clear) operations.
        /// </summary>
        private List<T> _shadowCopy;

        /// <summary>
        /// Indicates whether this instance has been disposed.
        /// </summary>
        private bool _disposed;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="UndoableCollectionBridge{T}"/> class.
        /// </summary>
        /// <param name="collection">
        /// The <see cref="ObservableCollection{T}"/> to monitor for changes.
        /// </param>
        /// <param name="getUndoManager">
        /// A function that returns the <see cref="IUndoManager"/> to use for recording actions,
        /// or null if undo is currently disabled. This allows dynamic checking of undo state.
        /// </param>
        /// <param name="collectionDescription">
        /// A human-readable description of the collection (e.g., "probability ordinates", "items").
        /// This is used in the descriptions of undo actions.
        /// </param>
        /// <param name="target">
        /// Optional. The target object that owns this collection. This is passed to the
        /// <see cref="IUndoableAction.Target"/> property of created actions.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="collection"/> or <paramref name="getUndoManager"/> is null.
        /// </exception>
        public UndoableCollectionBridge(
            ObservableCollection<T> collection,
            Func<IUndoManager> getUndoManager,
            string collectionDescription = "collection",
            object target = null)
        {
            _collection = collection ?? throw new ArgumentNullException(nameof(collection));
            _getUndoManager = getUndoManager ?? throw new ArgumentNullException(nameof(getUndoManager));
            _collectionDescription = collectionDescription ?? "collection";
            _target = target;

            // Initialize the shadow copy with current collection contents
            _shadowCopy = new List<T>(_collection);

            // Subscribe to collection changes
            _collection.CollectionChanged += OnCollectionChanged;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the collection being monitored by this bridge.
        /// </summary>
        /// <value>
        /// The <see cref="ObservableCollection{T}"/> that this bridge is attached to.
        /// </value>
        public ObservableCollection<T> Collection => _collection;

        #endregion

        #region Event Handlers

        /// <summary>
        /// Handles the <see cref="ObservableCollection{T}.CollectionChanged"/> event
        /// by creating and recording appropriate undo actions.
        /// </summary>
        /// <param name="sender">The source of the event (the collection).</param>
        /// <param name="e">
        /// The <see cref="NotifyCollectionChangedEventArgs"/> containing information
        /// about the change that occurred.
        /// </param>
        /// <remarks>
        /// This method:
        /// <list type="bullet">
        /// <item>Checks if undo is enabled and not currently executing an undo/redo operation</item>
        /// <item>Creates the appropriate action based on the change type</item>
        /// <item>Records the action with the undo manager</item>
        /// <item>Updates the shadow copy to reflect the current collection state</item>
        /// </list>
        /// </remarks>
        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var undoManager = _getUndoManager();

            // If undo is disabled or we're executing an undo/redo, just update the shadow and return
            if (undoManager == null || undoManager.IsExecutingAction)
            {
                UpdateShadowCopy();
                return;
            }

            // Create the appropriate action based on the change type
            IUndoableAction action = CreateActionForChange(e);

            // Record the action if one was created
            if (action != null)
            {
                undoManager.RecordAction(action);
            }

            // Update the shadow copy to match the current collection state
            UpdateShadowCopy();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Creates an <see cref="IUndoableAction"/> appropriate for the given collection change.
        /// </summary>
        /// <param name="e">
        /// The <see cref="NotifyCollectionChangedEventArgs"/> describing the change.
        /// </param>
        /// <returns>
        /// An <see cref="IUndoableAction"/> that can undo and redo the change,
        /// or null if no action could be created.
        /// </returns>
        /// <remarks>
        /// This method handles the following change types:
        /// <list type="bullet">
        /// <item><see cref="NotifyCollectionChangedAction.Add"/>: Item(s) added to collection</item>
        /// <item><see cref="NotifyCollectionChangedAction.Remove"/>: Item(s) removed from collection</item>
        /// <item><see cref="NotifyCollectionChangedAction.Replace"/>: Item(s) replaced in collection</item>
        /// <item><see cref="NotifyCollectionChangedAction.Reset"/>: Collection cleared (uses shadow copy)</item>
        /// <item><see cref="NotifyCollectionChangedAction.Move"/>: Item moved within collection</item>
        /// </list>
        /// </remarks>
        private IUndoableAction CreateActionForChange(NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    return CreateAddAction(e);

                case NotifyCollectionChangedAction.Remove:
                    return CreateRemoveAction(e);

                case NotifyCollectionChangedAction.Replace:
                    return CreateReplaceAction(e);

                case NotifyCollectionChangedAction.Reset:
                    return CreateResetAction();

                case NotifyCollectionChangedAction.Move:
                    return CreateMoveAction(e);

                default:
                    return null;
            }
        }

        /// <summary>
        /// Creates an action to undo/redo an Add operation.
        /// </summary>
        /// <param name="e">The event arguments containing the added items and their starting index.</param>
        /// <returns>A <see cref="DelegateAction"/> that removes the items on undo and re-adds them on redo.</returns>
        /// <remarks>
        /// The undo operation removes items from the collection at their original indices.
        /// The redo operation re-inserts the items at their original indices.
        /// </remarks>
        private IUndoableAction CreateAddAction(NotifyCollectionChangedEventArgs e)
        {
            // Capture the added items and their starting index
            var addedItems = e.NewItems?.Cast<T>().ToList() ?? new List<T>();
            int startIndex = e.NewStartingIndex;

            if (addedItems.Count == 0) return null;

            string description = addedItems.Count == 1
                ? $"Add to {_collectionDescription}"
                : $"Add {addedItems.Count} items to {_collectionDescription}";

            return new DelegateAction(
                description,
                execute: () =>
                {
                    // Re-add the items at their original positions
                    for (int i = 0; i < addedItems.Count; i++)
                    {
                        _collection.Insert(startIndex + i, addedItems[i]);
                    }
                },
                undo: () =>
                {
                    // Remove the items (in reverse order to maintain indices)
                    for (int i = addedItems.Count - 1; i >= 0; i--)
                    {
                        _collection.RemoveAt(startIndex + i);
                    }
                },
                target: _target
            );
        }

        /// <summary>
        /// Creates an action to undo/redo a Remove operation.
        /// </summary>
        /// <param name="e">The event arguments containing the removed items and their original starting index.</param>
        /// <returns>A <see cref="DelegateAction"/> that re-inserts the items on undo and removes them on redo.</returns>
        /// <remarks>
        /// The undo operation re-inserts items at their original indices.
        /// The redo operation removes the items again.
        /// </remarks>
        private IUndoableAction CreateRemoveAction(NotifyCollectionChangedEventArgs e)
        {
            // Capture the removed items and their original index
            var removedItems = e.OldItems?.Cast<T>().ToList() ?? new List<T>();
            int startIndex = e.OldStartingIndex;

            if (removedItems.Count == 0) return null;

            string description = removedItems.Count == 1
                ? $"Remove from {_collectionDescription}"
                : $"Remove {removedItems.Count} items from {_collectionDescription}";

            return new DelegateAction(
                description,
                execute: () =>
                {
                    // Remove the items (in reverse order to maintain indices)
                    for (int i = removedItems.Count - 1; i >= 0; i--)
                    {
                        _collection.RemoveAt(startIndex + i);
                    }
                },
                undo: () =>
                {
                    // Re-insert the items at their original positions
                    for (int i = 0; i < removedItems.Count; i++)
                    {
                        _collection.Insert(startIndex + i, removedItems[i]);
                    }
                },
                target: _target
            );
        }

        /// <summary>
        /// Creates an action to undo/redo a Replace operation.
        /// </summary>
        /// <param name="e">The event arguments containing the old and new items.</param>
        /// <returns>A <see cref="DelegateAction"/> that restores old values on undo and applies new values on redo.</returns>
        /// <remarks>
        /// The undo operation replaces the new items with the old items.
        /// The redo operation replaces the old items with the new items.
        /// </remarks>
        private IUndoableAction CreateReplaceAction(NotifyCollectionChangedEventArgs e)
        {
            // Capture the old and new items
            var oldItems = e.OldItems?.Cast<T>().ToList() ?? new List<T>();
            var newItems = e.NewItems?.Cast<T>().ToList() ?? new List<T>();
            int startIndex = e.NewStartingIndex;

            if (oldItems.Count == 0 || newItems.Count == 0) return null;

            string description = oldItems.Count == 1
                ? $"Change {_collectionDescription}"
                : $"Change {oldItems.Count} items in {_collectionDescription}";

            return new DelegateAction(
                description,
                execute: () =>
                {
                    // Replace with new items
                    for (int i = 0; i < newItems.Count; i++)
                    {
                        _collection[startIndex + i] = newItems[i];
                    }
                },
                undo: () =>
                {
                    // Replace with old items
                    for (int i = 0; i < oldItems.Count; i++)
                    {
                        _collection[startIndex + i] = oldItems[i];
                    }
                },
                target: _target
            );
        }

        /// <summary>
        /// Creates an action to undo/redo a Reset (Clear) operation.
        /// </summary>
        /// <returns>A <see cref="DelegateAction"/> that restores all items on undo and clears on redo.</returns>
        /// <remarks>
        /// The Reset action is raised when <see cref="ObservableCollection{T}.Clear"/> is called,
        /// but the event arguments do not contain the removed items. This is why we maintain
        /// a shadow copy of the collection - to capture the items that were present before the clear.
        /// </remarks>
        private IUndoableAction CreateResetAction()
        {
            // Capture the shadow copy before it gets updated
            var clearedItems = new List<T>(_shadowCopy);

            if (clearedItems.Count == 0) return null;

            string description = $"Clear {_collectionDescription}";

            return new DelegateAction(
                description,
                execute: () =>
                {
                    // Clear the collection
                    _collection.Clear();
                },
                undo: () =>
                {
                    // Restore all cleared items
                    foreach (var item in clearedItems)
                    {
                        _collection.Add(item);
                    }
                },
                target: _target
            );
        }

        /// <summary>
        /// Creates an action to undo/redo a Move operation.
        /// </summary>
        /// <param name="e">The event arguments containing the old and new indices.</param>
        /// <returns>A <see cref="DelegateAction"/> that moves items back on undo and forward on redo.</returns>
        /// <remarks>
        /// The undo operation moves the item back to its original index.
        /// The redo operation moves the item to the new index again.
        /// </remarks>
        private IUndoableAction CreateMoveAction(NotifyCollectionChangedEventArgs e)
        {
            int oldIndex = e.OldStartingIndex;
            int newIndex = e.NewStartingIndex;

            if (oldIndex == newIndex) return null;

            string description = $"Move item in {_collectionDescription}";

            return new DelegateAction(
                description,
                execute: () =>
                {
                    _collection.Move(oldIndex, newIndex);
                },
                undo: () =>
                {
                    _collection.Move(newIndex, oldIndex);
                },
                target: _target
            );
        }

        /// <summary>
        /// Updates the shadow copy to match the current collection state.
        /// </summary>
        /// <remarks>
        /// This method should be called after every collection change to keep
        /// the shadow copy synchronized. The shadow copy is essential for
        /// handling Reset (Clear) operations.
        /// </remarks>
        private void UpdateShadowCopy()
        {
            _shadowCopy = new List<T>(_collection);
        }

        #endregion

        #region IDisposable Implementation

        /// <summary>
        /// Releases all resources used by the <see cref="UndoableCollectionBridge{T}"/>.
        /// </summary>
        /// <remarks>
        /// This method unsubscribes from the <see cref="ObservableCollection{T}.CollectionChanged"/>
        /// event to prevent memory leaks. Always call <see cref="Dispose"/> when you are finished
        /// using the bridge, or use a using statement.
        /// </remarks>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="UndoableCollectionBridge{T}"/>
        /// and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">
        /// true to release both managed and unmanaged resources;
        /// false to release only unmanaged resources.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // Unsubscribe from collection events
                    _collection.CollectionChanged -= OnCollectionChanged;

                    // Clear the shadow copy
                    _shadowCopy?.Clear();
                    _shadowCopy = null;
                }

                _disposed = true;
            }
        }

        #endregion
    }
}
