using System.Collections.ObjectModel;
using System.Drawing;

namespace FrameworkInterfaces
{

    /// <summary>
    /// This is an interface for managing projects.
    /// </summary>
    /// <remarks>
    /// <para>
    ///     Authors:
    ///     Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil
    ///     Woodrow Fields, USACE Risk Management Center, woodrow.l.fields@usace.army.mil
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
        ReadOnlyCollection<IElementCollection> ElementCollections { get; }

        /// <summary>
        /// Gets the software version the project was last saved with. If the version, is outdated, convert to new version.
        /// </summary>
        string SoftwareVersion { get; }

        /// <summary>
        /// Gets the directory in which the file is located.
        /// </summary>
        string FileDirectory { get; }

        /// <summary>
        /// Gets and sets the AvalonDock layout string.
        /// </summary>
        string AvalonDockLayout { get; set; }

        /// <summary>
        /// Gets and sets the Project Explorer layout string.
        /// </summary>
        string ProjectExplorerLayout { get; set; }

        /// <summary>
        /// Gets the project image as a Bitmap. This image is used to automatically set icons.
        /// </summary>
        Bitmap ProjectImage { get; }

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
        /// <param name="newfullFileName">The full file name of the new project.</param>
        void SaveAs(string newfullFileName);

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
