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
using System.Linq;

namespace FrameworkInterfaces.Undo.Actions
{
    /// <summary>
    /// Represents a group of actions that should be undone/redone as a single operation.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Use this class to group multiple related actions into a single undo operation.
    /// For example, deleting multiple elements can be grouped so that Undo restores
    /// all of them at once.
    /// </para>
    /// <para>
    /// <b> Authors: </b>
    /// <list type="bullet">
    ///     <item> Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil </item>
    /// </list>
    /// </para>
    /// </remarks>
    public class CompositeAction : IUndoableAction
    {
        #region Fields

        private readonly List<IUndoableAction> _actions = new List<IUndoableAction>();
        private readonly string _description;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="CompositeAction"/> class.
        /// </summary>
        /// <param name="description">Description for the grouped action.</param>
        public CompositeAction(string description)
        {
            _description = description ?? "Multiple actions";
            Timestamp = DateTime.Now;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CompositeAction"/> class with existing actions.
        /// </summary>
        /// <param name="description">Description for the grouped action.</param>
        /// <param name="actions">The actions to include in this composite.</param>
        public CompositeAction(string description, IEnumerable<IUndoableAction> actions)
            : this(description)
        {
            if (actions != null)
            {
                _actions.AddRange(actions);
            }
        }

        #endregion

        #region Properties

        /// <inheritdoc/>
        public string Description => _description;

        /// <inheritdoc/>
        public DateTime Timestamp { get; }

        /// <inheritdoc/>
        public object Target => _actions.FirstOrDefault()?.Target;

        /// <summary>
        /// Gets the list of actions in this composite.
        /// </summary>
        public IReadOnlyList<IUndoableAction> Actions => _actions.AsReadOnly();

        #endregion

        #region Public Methods

        /// <summary>
        /// Adds an action to this composite.
        /// </summary>
        /// <param name="action">The action to add.</param>
        public void AddAction(IUndoableAction action)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));
            _actions.Add(action);
        }

        /// <inheritdoc/>
        public void Execute()
        {
            // Execute all actions in order
            foreach (var action in _actions)
            {
                action.Execute();
            }
        }

        /// <inheritdoc/>
        public void Undo()
        {
            // Undo all actions in reverse order
            for (int i = _actions.Count - 1; i >= 0; i--)
            {
                _actions[i].Undo();
            }
        }

        /// <inheritdoc/>
        public bool CanMergeWith(IUndoableAction other)
        {
            // Composite actions don't merge with other actions
            return false;
        }

        /// <inheritdoc/>
        public IUndoableAction MergeWith(IUndoableAction other)
        {
            return this;
        }

        #endregion
    }
}
