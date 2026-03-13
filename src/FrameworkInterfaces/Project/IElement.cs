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

using System.Drawing;

namespace FrameworkInterfaces
{

    /// <summary>
    /// Delegate method to raise event before the element has been deleted.
    /// </summary>
    /// <param name="element">The element to be deleted.</param>
    /// <param name="cancel">Determines if the deletion should be canceled.</param>
    public delegate void PreviewDeletedEventHandler(IElement element, ref bool cancel);

    /// <summary>
    /// Delegate method to raise event when the element has been deleted.
    /// </summary>
    /// <param name="element">The element that was deleted.</param>
    public delegate void DeletedEventHandler(IElement element);

    /// <summary>
    /// This is an interface for project elements.
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
    public interface IElement : IMetaData, ISave
    {

        /// <summary>
        /// Gets the display name of the element. This can be different from the Name.
        /// </summary>
        string DisplayName { get; }

        /// <summary>
        /// Gets the parent element collection.
        /// </summary>
        IElementCollection ParentCollection { get; }

        /// <summary>
        /// Gets the element image as a Bitmap. This image is used to automatically set icons.
        /// </summary>
        Bitmap ElementImage { get; }

        /// <summary>
        /// Determines if the element can be copied from an external application.
        /// </summary>
        bool CanCopyFromExternal { get; }

        /// <summary>
        /// Returns if the specified element is valid.
        /// </summary>
        bool IsValid { get; }

        /// <summary>
        /// Event is raised before the element has been deleted.
        /// </summary>
        event PreviewDeletedEventHandler PreviewDeleted;

        /// <summary>
        /// Event is raised when the element has been deleted.
        /// </summary>
        event DeletedEventHandler Deleted;

        /// <summary>
        /// Copy the element. You can optionally provide a new name when copying the element.
        /// </summary>
        /// <param name="newName">Optional. New name of the copied element.</param>
        /// <returns>A deep copy of the element.</returns>
        IElement Copy(string? newName = null);

        /// <summary>
        /// Copy the element from an external project to disk within the current project.
        /// </summary>
        /// <param name="itemName">The item to copy from.</param>
        /// <param name="fullFileName">The full file name of the project to copy from.</param>
        /// <returns>A deep copy of the element.</returns>
        IElement CopyFromExternal(string itemName, string fullFileName);

        /// <summary>
        /// Delete the element and remove all data from disk.
        /// </summary>
        void Delete();

    }
}
