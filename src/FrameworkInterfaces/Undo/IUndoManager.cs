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

namespace FrameworkInterfaces.Undo
{
    /// <summary>
    /// Manages undo/redo stacks for a specific scope (element, collection, or project).
    /// </summary>
    /// <remarks>
    /// <para>
    /// The undo manager maintains two stacks: one for undo operations and one for redo.
    /// When an action is undone, it moves to the redo stack. When a new action is recorded,
    /// the redo stack is cleared.
    /// </para>
    /// <para>
    /// This interface supports Visual Studio-style per-document undo where each open
    /// element has its own undo history.
    /// </para>
    /// <para>
    /// <b> Authors: </b>
    /// <list type="bullet">
    ///     <item> Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil </item>
    /// </list>
    /// </para>
    /// </remarks>
    public interface IUndoManager : INotifyPropertyChanged
    {
        /// <summary>
        /// Gets whether an undo operation is available.
        /// </summary>
        bool CanUndo { get; }

        /// <summary>
        /// Gets whether a redo operation is available.
        /// </summary>
        bool CanRedo { get; }

        /// <summary>
        /// Gets the description of the next undo action (for tooltip/menu display).
        /// </summary>
        /// <value>The description, or null if no undo is available.</value>
        string? UndoDescription { get; }

        /// <summary>
        /// Gets the description of the next redo action (for tooltip/menu display).
        /// </summary>
        /// <value>The description, or null if no redo is available.</value>
        string? RedoDescription { get; }

        /// <summary>
        /// Gets the list of actions that can be undone (most recent first).
        /// </summary>
        /// <remarks>Used for dropdown menu showing undo history.</remarks>
        IReadOnlyList<IUndoableAction> UndoStack { get; }

        /// <summary>
        /// Gets the list of actions that can be redone (most recent first).
        /// </summary>
        /// <remarks>Used for dropdown menu showing redo history.</remarks>
        IReadOnlyList<IUndoableAction> RedoStack { get; }

        /// <summary>
        /// Gets or sets the maximum number of undo levels to maintain.
        /// </summary>
        /// <value>The maximum undo levels. Default is 100.</value>
        int MaxUndoLevels { get; set; }

        /// <summary>
        /// Gets whether an action is currently being executed by the undo manager.
        /// </summary>
        /// <remarks>
        /// Use this to prevent re-recording actions during undo/redo operations.
        /// When true, calls to RecordAction should be ignored.
        /// </remarks>
        bool IsExecutingAction { get; }

        /// <summary>
        /// Gets whether the current state differs from the last save point.
        /// </summary>
        /// <remarks>
        /// This can be used as an alternative to the existing IsDirty tracking.
        /// </remarks>
        bool HasChangedSinceSave { get; }

        /// <summary>
        /// Executes an action and records it on the undo stack.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        /// <remarks>
        /// Use this when you want the undo manager to execute the action.
        /// The action's Execute() method will be called.
        /// </remarks>
        void ExecuteAction(IUndoableAction action);

        /// <summary>
        /// Records an action on the undo stack without executing it.
        /// </summary>
        /// <param name="action">The action to record.</param>
        /// <remarks>
        /// Use this when the action has already been performed and you just
        /// need to record it for undo purposes.
        /// </remarks>
        void RecordAction(IUndoableAction action);

        /// <summary>
        /// Undoes the most recent action.
        /// </summary>
        void Undo();

        /// <summary>
        /// Undoes multiple actions up to and including the specified action.
        /// </summary>
        /// <param name="action">The action to undo to (from the UndoStack).</param>
        void UndoTo(IUndoableAction action);

        /// <summary>
        /// Redoes the most recently undone action.
        /// </summary>
        void Redo();

        /// <summary>
        /// Redoes multiple actions up to and including the specified action.
        /// </summary>
        /// <param name="action">The action to redo to (from the RedoStack).</param>
        void RedoTo(IUndoableAction action);

        /// <summary>
        /// Clears both undo and redo stacks.
        /// </summary>
        /// <remarks>
        /// Call this when loading a new document or when you want to
        /// establish a clean state with no undo history.
        /// </remarks>
        void Clear();

        /// <summary>
        /// Marks the current state as the saved state.
        /// </summary>
        /// <remarks>
        /// Call this after saving to update the HasChangedSinceSave property.
        /// </remarks>
        void MarkSavePoint();

        /// <summary>
        /// Begins a transaction that groups multiple actions into one undo operation.
        /// </summary>
        /// <param name="description">Description for the grouped action.</param>
        /// <returns>A disposable transaction object. Dispose to commit the transaction.</returns>
        /// <example>
        /// <code>
        /// using (undoManager.BeginTransaction("Delete multiple elements"))
        /// {
        ///     foreach (var element in elements)
        ///         element.Delete();
        /// }
        /// // All deletes are now a single undo operation
        /// </code>
        /// </example>
        IDisposable BeginTransaction(string description);

        /// <summary>
        /// Raised when undo/redo state changes (stacks modified, action executed).
        /// </summary>
        event EventHandler StateChanged;
    }
}
