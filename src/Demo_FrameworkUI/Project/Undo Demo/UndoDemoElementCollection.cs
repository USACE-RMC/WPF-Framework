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

using FrameworkInterfaces;

namespace Demo_FrameworkUI.Project.Undo_Demo
{
    /// <summary>
    /// A demo element collection that showcases the undo/redo functionality for collection operations. This collection demonstrates how to use RecordAddElement and RecordRemoveElement for undo support.
    /// </summary>
    /// <remarks>
    /// <para>
    ///     <b> Authors: </b>
    /// <list type="bullet">
    /// <item><description>
    ///     Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil
    /// </description></item>
    /// <item><description>
    ///     Woodrow Fields, USACE Risk Management Center, woodrow.l.fields@usace.army.mil
    /// </description></item>
    /// </list>
    /// </para>
    /// </remarks>
    public class UndoDemoElementCollection : ElementCollectionBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UndoDemoElementCollection"/> class and adds initial demo elements.
        /// </summary>
        /// <param name="parentProject">The parent project that contains this collection.</param>
        public UndoDemoElementCollection(IProject parentProject) : base(parentProject)
        {
            // Add some initial demo elements
            Add(new UndoDemoElement("Demo Element 1", this));
            Add(new UndoDemoElement("Demo Element 2", this));
            Add(new UndoDemoElement("Demo Element 3", this));

            // Mark as clean after initialization
            SetIsDirty(false);
        }

        /// <summary>
        /// Gets the name of the collection.
        /// </summary>
        public override string Name => "Undo Demo Elements";

        /// <summary>
        /// Opens the collection and loads elements from disk. Clears the undo history after loading.
        /// </summary>
        public override void Open()
        {
            _opening = true;
            // Simulate reading from disk
            // ...
            _opening = false;
            ClearUndoHistory();
            SetIsDirty(false);
        }

        /// <summary>
        /// Adds an element to the collection with undo support.
        /// </summary>
        public override void Add(IElement item)
        {
            item.Deleted += ElementDeleted;
            ElementList.Add((UndoDemoElement)item);

            // Record the add action for undo support (only if not opening)
            if (!_opening)
            {
                RecordAddElement(item);
                SetIsDirty(true);
            }

            RaiseElementAddedEvent(item);
        }

        /// <summary>
        /// Inserts an element at the specified index with undo support.
        /// </summary>
        public override void Insert(int index, IElement item)
        {
            item.Deleted += ElementDeleted;
            ElementList.Insert(index, (UndoDemoElement)item);

            // Record the add action for undo support
            RecordAddElement(item, index);
            SetIsDirty(true);

            RaiseElementAddedEvent(item);
        }

        /// <summary>
        /// Removes an element from the collection with undo support.
        /// </summary>
        public override bool Remove(IElement item)
        {
            if (item == null) return false;

            int index = ElementList.IndexOf((UndoDemoElement)item);
            if (index < 0) return false;

            // Record the remove action for undo support before actually removing
            RecordRemoveElement(item);

            item.Deleted -= ElementDeleted;
            ElementList.Remove((UndoDemoElement)item);
            SetIsDirty(true);

            RaiseElementRemovedEvent(item);
            return true;
        }

        /// <summary>
        /// Removes the element at the specified index with undo support.
        /// </summary>
        public override void RemoveAt(int index)
        {
            if (index < 0 || index >= ElementList.Count) return;

            var item = ElementList[index];

            // Record the remove action for undo support
            RecordRemoveElement(item);

            item.Deleted -= ElementDeleted;
            ElementList.RemoveAt(index);
            SetIsDirty(true);

            RaiseElementRemovedEvent(item);
        }

        /// <summary>
        /// Moves an element within the collection with undo support.
        /// </summary>
        /// <param name="oldIndex">The current index of the element.</param>
        /// <param name="newIndex">The new index where the element should be moved.</param>
        public void MoveElement(int oldIndex, int newIndex)
        {
            if (oldIndex < 0 || oldIndex >= ElementList.Count) return;
            if (newIndex < 0 || newIndex >= ElementList.Count) return;
            if (oldIndex == newIndex) return;

            var item = ElementList[oldIndex];

            // Record the move action for undo support
            RecordMoveElement(item, oldIndex, newIndex);

            ElementList.RemoveAt(oldIndex);
            ElementList.Insert(newIndex, (UndoDemoElement)item);
            SetIsDirty(true);
        }

        /// <summary>
        /// Deletes the collection and all its elements.
        /// </summary>
        public override void Delete()
        {
            // Clear all elements
            foreach (var element in ElementList)
            {
                element.Delete();
            }
            ElementList.Clear();
        }

        /// <summary>
        /// Inserts an element from an external project at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which the element should be inserted.</param>
        /// <param name="elementName">The name of the element to insert.</param>
        /// <param name="elementType">The type of the element.</param>
        /// <param name="fullFileName">The full path to the external project file.</param>
        public override void InsertFromExternalProject(int index, string elementName, string elementType, string fullFileName)
        {
            Insert(index, new UndoDemoElement(elementName, this));
        }

        /// <summary>
        /// Creates a new demo element with undo support and adds it to the collection.
        /// </summary>
        /// <param name="name">The name of the new element.</param>
        /// <returns>The newly created element.</returns>
        public UndoDemoElement CreateNewElement(string name)
        {
            var element = new UndoDemoElement(name, this);
            Add(element);
            return element;
        }
    }
}
