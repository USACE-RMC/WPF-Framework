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

namespace FrameworkInterfaces.Undo.Actions
{
    /// <summary>
    /// Represents an undoable action that uses delegates for execution and undo logic.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This class provides a flexible way to create undoable actions without defining
    /// a new class for each action type. It accepts <see cref="Action"/> delegates
    /// for both the execute and undo operations.
    /// </para>
    /// <para>
    /// Use this class when you need to capture simple undo/redo behavior without
    /// the overhead of creating a specialized action class. For more complex scenarios
    /// involving merging or specific state tracking, consider creating a dedicated
    /// implementation of <see cref="IUndoableAction"/>.
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
    /// // Example: Create an action to add an item to a collection
    /// var item = new MyItem();
    /// var action = new DelegateAction(
    ///     "Add Item",
    ///     () => collection.Add(item),
    ///     () => collection.Remove(item),
    ///     collection
    /// );
    /// undoManager.RecordAction(action);
    /// </code>
    /// </example>
    public class DelegateAction : IUndoableAction
    {
        #region Fields

        /// <summary>
        /// The delegate to invoke when executing or redoing the action.
        /// </summary>
        private readonly Action _execute;

        /// <summary>
        /// The delegate to invoke when undoing the action.
        /// </summary>
        private readonly Action _undo;

        /// <summary>
        /// The human-readable description of this action.
        /// </summary>
        private readonly string _description;

        /// <summary>
        /// The target object this action applies to.
        /// </summary>
        private readonly object _target;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="DelegateAction"/> class.
        /// </summary>
        /// <param name="description">
        /// A human-readable description of the action for display in UI (e.g., "Add Item", "Remove Row").
        /// </param>
        /// <param name="execute">
        /// The delegate to invoke when executing or redoing the action.
        /// This delegate should perform the forward operation.
        /// </param>
        /// <param name="undo">
        /// The delegate to invoke when undoing the action.
        /// This delegate should reverse the effects of the execute delegate.
        /// </param>
        /// <param name="target">
        /// Optional. The target object this action applies to. Used to associate
        /// the action with a specific element or collection for undo stack management.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="execute"/> or <paramref name="undo"/> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Thrown when <paramref name="description"/> is null or empty.
        /// </exception>
        public DelegateAction(string description, Action execute, Action undo, object target = null)
        {
            if (string.IsNullOrEmpty(description))
            {
                throw new ArgumentException("Description cannot be null or empty.", nameof(description));
            }

            _description = description;
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _undo = undo ?? throw new ArgumentNullException(nameof(undo));
            _target = target;

            Timestamp = DateTime.Now;
        }

        #endregion

        #region Properties

        /// <inheritdoc/>
        /// <value>
        /// The human-readable description provided during construction.
        /// </value>
        public string Description => _description;

        /// <inheritdoc/>
        /// <value>
        /// The time when this action was created.
        /// </value>
        public DateTime Timestamp { get; }

        /// <inheritdoc/>
        /// <value>
        /// The target object provided during construction, or null if not specified.
        /// </value>
        public object Target => _target;

        #endregion

        #region Public Methods

        /// <inheritdoc/>
        /// <remarks>
        /// Invokes the execute delegate provided during construction.
        /// This method is called for both initial execution (via <see cref="IUndoManager.ExecuteAction"/>)
        /// and for redo operations.
        /// </remarks>
        public void Execute()
        {
            _execute();
        }

        /// <inheritdoc/>
        /// <remarks>
        /// Invokes the undo delegate provided during construction.
        /// This method reverses the effects of <see cref="Execute"/>.
        /// </remarks>
        public void Undo()
        {
            _undo();
        }

        /// <inheritdoc/>
        /// <remarks>
        /// <see cref="DelegateAction"/> does not support merging with other actions.
        /// This method always returns <c>false</c>. If you need merge support,
        /// create a custom implementation of <see cref="IUndoableAction"/>.
        /// </remarks>
        /// <returns>Always returns <c>false</c>.</returns>
        public bool CanMergeWith(IUndoableAction other)
        {
            // DelegateAction does not support merging
            return false;
        }

        /// <inheritdoc/>
        /// <remarks>
        /// Since <see cref="DelegateAction"/> does not support merging,
        /// this method simply returns the current instance unchanged.
        /// </remarks>
        /// <returns>The current instance.</returns>
        public IUndoableAction MergeWith(IUndoableAction other)
        {
            // Return this action unchanged since merging is not supported
            return this;
        }

        #endregion
    }
}
