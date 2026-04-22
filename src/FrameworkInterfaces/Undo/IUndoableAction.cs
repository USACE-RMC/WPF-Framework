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

namespace FrameworkInterfaces.Undo
{
    /// <summary>
    /// Represents an action that can be undone and redone.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This interface defines the contract for all undoable actions in the application.
    /// Implementations should capture all state needed to execute and reverse the action.
    /// </para>
    /// <para>
    /// <b> Authors: </b>
    /// <list type="bullet">
    ///     <item> Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil </item>
    /// </list>
    /// </para>
    /// </remarks>
    public interface IUndoableAction
    {
        /// <summary>
        /// Gets a human-readable description of the action for display in UI.
        /// </summary>
        /// <example>"Change Name", "Delete Element", "Add Hazard"</example>
        string Description { get; }

        /// <summary>
        /// Gets the timestamp when the action was created.
        /// </summary>
        DateTime Timestamp { get; }

        /// <summary>
        /// Gets the target object this action applies to.
        /// </summary>
        /// <remarks>
        /// This is typically an IElement, IElementCollection, or IProject.
        /// Used to determine which undo stack the action belongs to.
        /// </remarks>
        object? Target { get; }

        /// <summary>
        /// Executes the action. Called for initial execution and redo.
        /// </summary>
        void Execute();

        /// <summary>
        /// Reverses the action.
        /// </summary>
        void Undo();

        /// <summary>
        /// Determines if this action can be merged with another action.
        /// </summary>
        /// <remarks>
        /// Used for combining rapid changes (e.g., keystrokes while typing)
        /// into a single undo operation.
        /// </remarks>
        /// <param name="other">The other action to potentially merge with.</param>
        /// <returns>True if the actions can be merged; otherwise false.</returns>
        bool CanMergeWith(IUndoableAction other);

        /// <summary>
        /// Merges another action into this one.
        /// </summary>
        /// <param name="other">The action to merge.</param>
        /// <returns>A new merged action, or this action if merge modifies in place.</returns>
        IUndoableAction MergeWith(IUndoableAction other);
    }
}
