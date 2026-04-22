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

namespace FrameworkInterfaces.Undo
{
    /// <summary>
    /// Represents an element that supports undo/redo operations.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This interface extends <see cref="IElement"/> to add undo/redo support.
    /// Implementing this interface is optional - elements that don't implement it
    /// will continue to work without undo support.
    /// </para>
    /// <para>
    /// When implementing this interface, property setters should call
    /// <see cref="ElementBase.RecordPropertyChange"/> to record changes
    /// for undo purposes.
    /// </para>
    /// <para>
    /// <b> Authors: </b>
    /// <list type="bullet">
    ///     <item> Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil </item>
    /// </list>
    /// </para>
    /// </remarks>
    public interface IUndoableElement : IElement
    {
        /// <summary>
        /// Gets the undo manager for this element.
        /// </summary>
        /// <remarks>
        /// Each element has its own undo manager, enabling Visual Studio-style
        /// per-document undo where Ctrl+Z operates on the active document.
        /// </remarks>
        IUndoManager UndoManager { get; }

        /// <summary>
        /// Gets or sets whether undo recording is enabled for this element.
        /// </summary>
        /// <remarks>
        /// Set to false to temporarily disable undo recording, for example
        /// during bulk operations or loading from disk.
        /// </remarks>
        bool IsUndoEnabled { get; set; }
    }
}
