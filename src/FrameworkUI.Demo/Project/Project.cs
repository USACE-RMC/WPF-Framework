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
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using FrameworkUI.Demo.Project.Consequence_Elements;
using FrameworkUI.Demo.Project.Hazard_Elements;
using FrameworkUI.Demo.Project.Response_Elements;
using FrameworkUI.Demo.Project.Undo_Demo;

namespace FrameworkUI.Demo.Project
{
    /// <summary>
    /// Represents a demo project that showcases the ProjectUI framework features. Includes collections for hazard, response, and consequence functions, as well as an Undo Demo collection that demonstrates undo/redo functionality.
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
    public class Project : IProject
    {
        private ReadOnlyCollection<IElementCollection> _readOnlyElementCollections;
        private IElementCollection _hazardFunctions;
        private IElementCollection _responseFunctions;
        private IElementCollection _consequenceFunctions;
        private UndoDemoElementCollection _undoDemoElements;

        /// <summary>
        /// Initializes a new instance of the <see cref="Project"/> class and creates the element collections.
        /// </summary>
        public Project()
        {
            _hazardFunctions = new HazardElementCollection(this);
            _responseFunctions = new ResponseElementCollection(this);
            _consequenceFunctions = new ConsequenceElementCollection(this);
            _undoDemoElements = new UndoDemoElementCollection(this);
            _readOnlyElementCollections = new ReadOnlyCollection<IElementCollection>(new IElementCollection[] {_hazardFunctions, _responseFunctions, _consequenceFunctions, _undoDemoElements });

            //_elementCollection = new ElementCollection(this);

            //_elementCollections.Add(_elementCollection);
            //_elementCollection.Add(new Element("Hazard Function_1", _elementCollection));
            //_elementCollection.Add(new Element("Hazard Function_2", _elementCollection));
            //_elementCollection.Add(new Element("Hazard Function_3", _elementCollection));
            //_elementCollection.Add(new Element("Hazard Function_4", _elementCollection));
            //_elementCollection.Add(new Element("Hazard Function_5", _elementCollection));

        }
        //private ElementCollection _elementCollection;
        //private List<IElementCollection> _elementCollections = new List<IElementCollection>();

        /// <summary>
        /// Gets or sets the full file name of the project.
        /// </summary>
        public string FullFileName { get => "Demo Project"; set => throw new NotImplementedException(); }

        /// <summary>
        /// Gets the read-only collection of element collections in the project.
        /// </summary>
        public ReadOnlyCollection<IElementCollection> ElementCollections
        {
            get
            {
                return _readOnlyElementCollections;
            }
        }

        /// <summary>
        /// Gets the software version.
        /// </summary>
        public string SoftwareVersion => "1.0";

        /// <summary>
        /// Gets the file directory.
        /// </summary>
        public string FileDirectory => "Fake Directory";

        /// <summary>
        /// Gets or sets the AvalonDock layout.
        /// </summary>
        public string AvalonDockLayout { get => "No layout"; set => _=""; }

        /// <summary>
        /// Gets the project image icon.
        /// </summary>
        public Bitmap ProjectImage => Properties.Resources.Hazard_Icon;

        /// <summary>
        /// Gets or sets the name of the project.
        /// </summary>
        public string Name { get => "Demo Project"; set => throw new NotImplementedException(); }

        /// <summary>
        /// Gets or sets the description of the project.
        /// </summary>
        public string Description { get => "Demonstrates ProjectUI framework with undo/redo"; set => throw new NotImplementedException(); }

        /// <summary>
        /// Gets the creation date of the project.
        /// </summary>
        public DateTime CreationDate { get => DateTime.Now; }

        /// <summary>
        /// Gets the last modified date of the project.
        /// </summary>
        public DateTime LastModified { get => DateTime.Now; }

        /// <summary>
        /// Gets a value indicating whether the project has unsaved changes.
        /// </summary>
        public bool IsDirty => false;

        /// <summary>
        /// Gets the name used when saving the project to disk.
        /// </summary>
        public string NameOnDisk => "Demo Project";

        /// <summary>
        /// Gets or sets the project explorer layout.
        /// </summary>
        public string ProjectExplorerLayout { get ; set; }

        /// <summary>
        /// Occurs before an object is saved.
        /// </summary>
        public event PreviewObjectSavedEventHandler PreviewObjectSaved;

        /// <summary>
        /// Occurs when an object is saved.
        /// </summary>
        public event ObjectSavedEventHandler ObjectSaved;

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        //public event AddMessageEventHandler AddMessage;
        //public event RemoveMessageEventHandler RemoveMessage;

        /// <summary>
        /// Closes the project.
        /// </summary>
        public void Close()
        {
            //throw new NotImplementedException();
        }

        /// <summary>
        /// Compacts the project database.
        /// </summary>
        public void Compact()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates a new project at the specified file path.
        /// </summary>
        /// <param name="newFullFileName">The full path where the new project should be created.</param>
        public void CreateNew(string newFullFileName)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Validates the project.
        /// </summary>
        /// <returns>True if the project is valid; otherwise, false.</returns>
        public bool IsValid()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Opens the project from disk.
        /// </summary>
        public void Open()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Optimizes the project database.
        /// </summary>
        public void Optimize()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Saves the project to disk.
        /// </summary>
        public void Save()
        {
            //throw new NotImplementedException();
        }

        /// <summary>
        /// Saves the project to a new file location.
        /// </summary>
        /// <param name="newfullFileName">The full path where the project should be saved.</param>
        public void SaveAs(string newfullFileName)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates a zip archive of the project.
        /// </summary>
        /// <param name="zipFileName">The name of the zip file to create.</param>
        public void ZipProject(string zipFileName)
        {
            throw new NotImplementedException();
        }
    }
}
