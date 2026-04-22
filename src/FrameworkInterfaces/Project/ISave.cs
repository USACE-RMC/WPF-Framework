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

using System.ComponentModel;

namespace FrameworkInterfaces
{

    /// <summary>
    /// Delegate method to raise event before an object has been saved.
    /// </summary>
    /// <param name="sender">The object that will be saved.</param>
    /// <param name="cancel">Determines if the save should be canceled.</param>
    public delegate void PreviewObjectSavedEventHandler(ISave sender, ref bool cancel);

    /// <summary>
    /// Delegate method to raise event when an object has been saved.
    /// </summary>
    /// <param name="sender">The object that was saved.</param>
    public delegate void ObjectSavedEventHandler(ISave sender);

    /// <summary>
    /// This is an interface for managing data IO.
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
    /// This is a placeholder and will likely need to be updated in the future. The challenge lies with formats to save to. If it is a database (e.g. sqlite) then each item can save independently.
    /// If it is another format like XML then the whole document needs to be saved at once and each IElement implementation can't be saved independently.
    /// </para>
    /// </remarks>
    public interface ISave : INotifyPropertyChanged
    {

        /// <summary>
        /// Determines if there are unsaved changes. If IsDirty = True, then there are unsaved changes.
        /// </summary>
        bool IsDirty { get; }

        /// <summary>
        /// Represents the name as it appears on disk.
        /// </summary>
        string NameOnDisk { get; }

        /// <summary>
        /// Event is raised event before an object has been saved.
        /// </summary>
        event PreviewObjectSavedEventHandler PreviewObjectSaved;

        /// <summary>
        /// Event is raised when the object has been saved.
        /// </summary>
        event ObjectSavedEventHandler ObjectSaved;

        /// <summary>
        /// Load data from disk.
        /// </summary>
        void Open();

        /// <summary>
        /// Save data to disk.
        /// </summary>
        void Save();


    }
}
