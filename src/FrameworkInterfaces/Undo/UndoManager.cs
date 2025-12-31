using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using FrameworkInterfaces.Undo.Actions;

namespace FrameworkInterfaces.Undo
{
    /// <summary>
    /// Default implementation of <see cref="IUndoManager"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This class maintains undo and redo stacks and provides methods to execute,
    /// record, undo, and redo actions. It supports action merging for combining
    /// rapid changes and transactions for grouping multiple actions.
    /// </para>
    /// <para>
    ///     Authors:
    ///     Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil
    /// </para>
    /// </remarks>
    public class UndoManager : IUndoManager
    {
        #region Fields

        private readonly Stack<IUndoableAction> _undoStack = new Stack<IUndoableAction>();
        private readonly Stack<IUndoableAction> _redoStack = new Stack<IUndoableAction>();
        private int _savePointIndex = 0;
        private int _currentIndex = 0;
        private int _maxUndoLevels = 100;
        private bool _isExecutingAction = false;
        private CompositeAction _currentTransaction = null;
        private readonly object _lockObject = new object();

        #endregion

        #region Events

        /// <inheritdoc/>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <inheritdoc/>
        public event EventHandler StateChanged;

        #endregion

        #region Properties

        /// <inheritdoc/>
        public bool CanUndo
        {
            get { lock (_lockObject) { return _undoStack.Count > 0; } }
        }

        /// <inheritdoc/>
        public bool CanRedo
        {
            get { lock (_lockObject) { return _redoStack.Count > 0; } }
        }

        /// <inheritdoc/>
        public string UndoDescription
        {
            get { lock (_lockObject) { return CanUndo ? _undoStack.Peek().Description : null; } }
        }

        /// <inheritdoc/>
        public string RedoDescription
        {
            get { lock (_lockObject) { return CanRedo ? _redoStack.Peek().Description : null; } }
        }

        /// <inheritdoc/>
        public IReadOnlyList<IUndoableAction> UndoStack
        {
            get { lock (_lockObject) { return _undoStack.ToList().AsReadOnly(); } }
        }

        /// <inheritdoc/>
        public IReadOnlyList<IUndoableAction> RedoStack
        {
            get { lock (_lockObject) { return _redoStack.ToList().AsReadOnly(); } }
        }

        /// <inheritdoc/>
        public int MaxUndoLevels
        {
            get { return _maxUndoLevels; }
            set
            {
                if (value < 1) value = 1;
                _maxUndoLevels = value;
                TrimUndoStack();
            }
        }

        /// <inheritdoc/>
        public bool IsExecutingAction
        {
            get { return _isExecutingAction; }
        }

        /// <inheritdoc/>
        public bool HasChangedSinceSave
        {
            get { lock (_lockObject) { return _currentIndex != _savePointIndex; } }
        }

        #endregion

        #region Public Methods

        /// <inheritdoc/>
        public void ExecuteAction(IUndoableAction action)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));
            if (_isExecutingAction) return;

            try
            {
                _isExecutingAction = true;
                action.Execute();
            }
            finally
            {
                _isExecutingAction = false;
            }

            RecordActionInternal(action);
        }

        /// <inheritdoc/>
        public void RecordAction(IUndoableAction action)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));
            if (_isExecutingAction) return;

            RecordActionInternal(action);
        }

        /// <inheritdoc/>
        public void Undo()
        {
            IUndoableAction action;

            lock (_lockObject)
            {
                if (!CanUndo) return;
                action = _undoStack.Pop();
            }

            try
            {
                _isExecutingAction = true;
                action.Undo();
            }
            finally
            {
                _isExecutingAction = false;
            }

            lock (_lockObject)
            {
                _redoStack.Push(action);
                _currentIndex--;
            }

            OnStateChanged();
        }

        /// <inheritdoc/>
        public void UndoTo(IUndoableAction action)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));

            while (CanUndo)
            {
                bool isTarget;
                lock (_lockObject)
                {
                    isTarget = _undoStack.Peek() == action;
                }

                Undo();

                if (isTarget) break;
            }
        }

        /// <inheritdoc/>
        public void Redo()
        {
            IUndoableAction action;

            lock (_lockObject)
            {
                if (!CanRedo) return;
                action = _redoStack.Pop();
            }

            try
            {
                _isExecutingAction = true;
                action.Execute();
            }
            finally
            {
                _isExecutingAction = false;
            }

            lock (_lockObject)
            {
                _undoStack.Push(action);
                _currentIndex++;
            }

            OnStateChanged();
        }

        /// <inheritdoc/>
        public void RedoTo(IUndoableAction action)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));

            while (CanRedo)
            {
                bool isTarget;
                lock (_lockObject)
                {
                    isTarget = _redoStack.Peek() == action;
                }

                Redo();

                if (isTarget) break;
            }
        }

        /// <inheritdoc/>
        public void Clear()
        {
            lock (_lockObject)
            {
                _undoStack.Clear();
                _redoStack.Clear();
                _currentIndex = 0;
                _savePointIndex = 0;
                _currentTransaction = null;
            }

            OnStateChanged();
        }

        /// <inheritdoc/>
        public void MarkSavePoint()
        {
            lock (_lockObject)
            {
                _savePointIndex = _currentIndex;
            }

            OnStateChanged();
        }

        /// <inheritdoc/>
        public IDisposable BeginTransaction(string description)
        {
            lock (_lockObject)
            {
                if (_currentTransaction != null)
                {
                    // Nested transactions - just return a no-op disposable
                    return new NestedTransactionScope();
                }

                _currentTransaction = new CompositeAction(description);
                return new TransactionScope(this);
            }
        }

        #endregion

        #region Internal Methods

        /// <summary>
        /// Commits the current transaction, adding it to the undo stack.
        /// </summary>
        internal void CommitTransaction()
        {
            CompositeAction composite;

            lock (_lockObject)
            {
                if (_currentTransaction == null) return;

                composite = _currentTransaction;
                _currentTransaction = null;

                if (composite.Actions.Count == 0) return;
            }

            RecordActionInternal(composite);
        }

        /// <summary>
        /// Rolls back the current transaction, undoing all its actions.
        /// </summary>
        internal void RollbackTransaction()
        {
            CompositeAction composite;

            lock (_lockObject)
            {
                if (_currentTransaction == null) return;

                composite = _currentTransaction;
                _currentTransaction = null;
            }

            // Undo all actions in reverse order
            try
            {
                _isExecutingAction = true;
                foreach (var action in composite.Actions.AsEnumerable().Reverse())
                {
                    action.Undo();
                }
            }
            finally
            {
                _isExecutingAction = false;
            }
        }

        #endregion

        #region Private Methods

        private void RecordActionInternal(IUndoableAction action)
        {
            lock (_lockObject)
            {
                // If in a transaction, add to composite instead
                if (_currentTransaction != null)
                {
                    _currentTransaction.AddAction(action);
                    return;
                }

                // Try to merge with previous action (for rapid typing, etc.)
                if (_undoStack.Count > 0 && _undoStack.Peek().CanMergeWith(action))
                {
                    var previous = _undoStack.Pop();
                    var merged = previous.MergeWith(action);
                    _undoStack.Push(merged);
                }
                else
                {
                    _undoStack.Push(action);
                }

                // Clear redo stack on new action
                _redoStack.Clear();
                _currentIndex++;

                // Trim undo stack if over limit
                TrimUndoStack();
            }

            OnStateChanged();
        }

        private void TrimUndoStack()
        {
            lock (_lockObject)
            {
                while (_undoStack.Count > _maxUndoLevels)
                {
                    // Convert to list, remove oldest, rebuild stack
                    var list = _undoStack.ToList();
                    list.RemoveAt(list.Count - 1);
                    _undoStack.Clear();
                    for (int i = list.Count - 1; i >= 0; i--)
                    {
                        _undoStack.Push(list[i]);
                    }
                }
            }
        }

        private void OnStateChanged()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CanUndo)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(CanRedo)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(UndoDescription)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(RedoDescription)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(HasChangedSinceSave)));
            StateChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion

        #region Nested Classes

        /// <summary>
        /// Disposable scope for managing transactions.
        /// </summary>
        private class TransactionScope : IDisposable
        {
            private readonly UndoManager _manager;
            private bool _disposed = false;

            public TransactionScope(UndoManager manager)
            {
                _manager = manager;
            }

            public void Dispose()
            {
                if (!_disposed)
                {
                    _manager.CommitTransaction();
                    _disposed = true;
                }
            }
        }

        /// <summary>
        /// No-op disposable for nested transactions.
        /// </summary>
        private class NestedTransactionScope : IDisposable
        {
            public void Dispose() { }
        }

        #endregion
    }
}
