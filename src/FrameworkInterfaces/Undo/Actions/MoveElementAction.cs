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
    /// Represents moving an element within a collection that can be undone.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This action captures the original and new positions of an element
    /// so that undo can restore it to the original position.
    /// </para>
    /// <para>
    /// <b> Authors: </b>
    /// <list type="bullet">
    ///     <item> Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil </item>
    /// </list>
    /// </para>
    /// </remarks>
    public class MoveElementAction : IUndoableAction
    {
        #region Fields

        private readonly IElementCollection _collection;
        private readonly IElement _element;
        private readonly int _oldIndex;
        private readonly int _newIndex;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="MoveElementAction"/> class.
        /// </summary>
        /// <param name="collection">The collection containing the element.</param>
        /// <param name="element">The element that was moved.</param>
        /// <param name="oldIndex">The original index of the element.</param>
        /// <param name="newIndex">The new index of the element.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown when <paramref name="collection"/> or <paramref name="element"/> is null.
        /// </exception>
        public MoveElementAction(IElementCollection collection, IElement element, int oldIndex, int newIndex)
        {
            _collection = collection ?? throw new ArgumentNullException(nameof(collection));
            _element = element ?? throw new ArgumentNullException(nameof(element));
            _oldIndex = oldIndex;
            _newIndex = newIndex;
            Timestamp = DateTime.Now;
        }

        #endregion

        #region Properties

        /// <inheritdoc/>
        public string Description => $"Move {_element.DisplayName ?? _element.Name ?? "element"}";

        /// <inheritdoc/>
        public DateTime Timestamp { get; }

        /// <inheritdoc/>
        public object Target => _collection;

        /// <summary>
        /// Gets the element that was moved.
        /// </summary>
        public IElement Element => _element;

        /// <summary>
        /// Gets the collection containing the element.
        /// </summary>
        public IElementCollection Collection => _collection;

        /// <summary>
        /// Gets the original index of the element.
        /// </summary>
        public int OldIndex => _oldIndex;

        /// <summary>
        /// Gets the new index of the element.
        /// </summary>
        public int NewIndex => _newIndex;

        #endregion

        #region Public Methods

        /// <inheritdoc/>
        public void Execute()
        {
            // Move the element to the new position
            _collection.MoveElement(_element, _oldIndex, _newIndex);
        }

        /// <inheritdoc/>
        public void Undo()
        {
            // Move the element back to the original position
            _collection.MoveElement(_element, _newIndex, _oldIndex);
        }

        /// <inheritdoc/>
        public bool CanMergeWith(IUndoableAction other)
        {
            // Move actions don't merge
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
