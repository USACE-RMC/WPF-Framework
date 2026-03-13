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

using System.Collections.ObjectModel;

namespace FrameworkInterfaces
{

    /// <summary>
    /// Delegate method to raise event when adding an element.
    /// </summary>
    /// <param name="element">The new element to add.</param>
    public delegate void ElementAddedEventHandler(IElement element);

    /// <summary>
    /// Delegate method to raise event when removing an element.
    /// </summary>
    /// <param name="element">The element to remove.</param>
    public delegate void ElementRemovedEventHandler(IElement element);

    /// <summary>
    /// This is an interface for a collection of project elements.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b> Authors: </b>
    /// <list type="bullet">
    ///     <item> Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil </item>
    ///     <item> Woodrow Fields, USACE Risk Management Center, woodrow.l.fields@usace.army.mil </item>
    /// </list>
    /// </para>
    /// <para>
    /// The project element collection interface is designed for interacting with TreeView items that contain all of the sub-tree items for a given data type (e.g. Hazards).
    /// </para>
    /// </remarks>
    public interface IElementCollection : IList<IElement>, ISave
    {

        /// <summary>
        /// Gets and the name of the element collection.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the parent project of the element collection.
        /// </summary>
        IProject ParentProject { get; }

        /// <summary>
        /// Gets and sets the read only collection of element collections.
        /// </summary>
        ReadOnlyCollection<IElementCollection> ElementCollections { get; set; }

        /// <summary>
        /// Event is raised when adding an element.
        /// </summary>
        event ElementAddedEventHandler ElementAdded;

        /// <summary>
        /// Event is raised when removing an element.
        /// </summary>
        event ElementRemovedEventHandler ElementRemoved;

        /// <summary>
        /// Sort the collection.
        /// </summary>
        /// <param name="sortAction">Method to compare two objects.</param>
        void Sort(IComparer<IElement> sortAction);

        /// <summary>
        /// Move element within collection.
        /// </summary>
        /// <param name="element">The element to move.</param>
        /// <param name="startIndex">The start position of the element to move.</param>
        /// <param name="endIndex">The end position of the element to move.</param>
        void MoveElement(IElement element, int startIndex, int endIndex);

        /// <summary>
        /// Delete the element collection and remove all data from disk.
        /// </summary>
        void Delete();

        /// <summary>
        /// Copy the element from an external project to disk within the current project.
        /// </summary>
        /// <param name="index">The zero-based index at which the element should be inserted.</param>
        /// <param name="elementName">The element name.</param>
        /// <param name="elementType">The element type.</param>
        /// <param name="fullFileName">The full file name of the project to copy from.</param>
        void InsertFromExternalProject(int index, string elementName, string elementType, string fullFileName);

    }
}
