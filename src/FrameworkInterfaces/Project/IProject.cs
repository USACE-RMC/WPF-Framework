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
using System.Windows.Media;

namespace FrameworkInterfaces
{

    /// <summary>
    /// This is an interface for managing projects.
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
    public interface IProject : IMetaData, ISave
    {

        /// <summary>
        /// Gets and sets the full project file name, including the directory path.
        /// </summary>
        string FullFileName { get; set; }

        /// <summary>
        /// Gets the collection of element collections.
        /// </summary>
        ReadOnlyCollection<IElementCollection>? ElementCollections { get; }

        /// <summary>
        /// Gets the software version the project was last saved with.
        /// </summary>
        /// <remarks>
        /// Applications should check this version against the current software version
        /// and handle any necessary data migration when loading older projects.
        /// </remarks>
        string SoftwareVersion { get; }

        /// <summary>
        /// Gets the directory in which the file is located.
        /// </summary>
        string? FileDirectory { get; }

        /// <summary>
        /// Gets and sets the AvalonDock layout string.
        /// </summary>
        string AvalonDockLayout { get; set; }

        /// <summary>
        /// Gets and sets the Project Explorer layout string.
        /// </summary>
        string ProjectExplorerLayout { get; set; }

        /// <summary>
        /// Gets a value indicating whether the window or project-explorer layout has
        /// been modified since the last save. Layout changes are tracked separately
        /// from <see cref="ISave.IsDirty"/> so that persisting window state does not
        /// imply a model edit and does not stamp <see cref="IMetaData.LastModified"/>.
        /// </summary>
        bool LayoutDirty { get; }

        /// <summary>
        /// Gets the last-known-good <see cref="AvalonDockLayout"/> loaded from disk.
        /// Consumers should try this as a fallback when the current
        /// <see cref="AvalonDockLayout"/> fails to deserialize, before falling back to
        /// the default layout. Empty string if no previous value was captured.
        /// </summary>
        string AvalonDockLayoutPrevious { get; }

        /// <summary>
        /// Gets the last-known-good <see cref="ProjectExplorerLayout"/> loaded from disk.
        /// Consumers should try this as a fallback when the current
        /// <see cref="ProjectExplorerLayout"/> fails to parse, before falling back to
        /// the standard element-collection order. Empty string if no previous value was captured.
        /// </summary>
        string ProjectExplorerLayoutPrevious { get; }

        /// <summary>
        /// Gets the project image as an ImageSource. This image is used to automatically set icons.
        /// </summary>
        ImageSource ProjectImage { get; }

        /// <summary>
        /// Create a new project.
        /// </summary>
        /// <param name="newFullFileName">The name of the new project.</param>
        void CreateNew(string newFullFileName);

        /// <summary>
        /// Close the project and dispose of any virtual memory.
        /// </summary>
        void Close();

        /// <summary>
        /// Save the project as a new project.
        /// </summary>
        /// <param name="newFullFileName">The full file name of the new project.</param>
        void SaveAs(string newFullFileName);

        /// <summary>
        /// Zip the project file.
        /// </summary>
        /// <param name="zipFileName">The full file name of the zip file.</param>
        void ZipProject(string zipFileName);

        /// <summary>
        /// Determines if the specified property is valid. If no property name is input, all properties are validated.
        /// </summary>
        /// <param name="propertyName">Optional. The name of the property to validate.</param>
        bool IsValid();

        /// <summary>
        /// Compacts the project file.
        /// </summary>
        void Compact();

        /// <summary>
        /// Optimize the project file.
        /// </summary>
        void Optimize();

    }
}
