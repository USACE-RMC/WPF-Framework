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
using Demo_FrameworkUI.Project.Consequence_Elements;
using Demo_FrameworkUI.Project.Hazard_Elements;
using Demo_FrameworkUI.Project.Response_Elements;
using Demo_FrameworkUI.Project.Undo_Demo;

namespace Demo_FrameworkUI.Project
{
    /// <summary>
    /// This is a demo project that showcases the ProjectUI framework features.
    /// Includes an Undo Demo collection that demonstrates undo/redo functionality.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b> Authors: </b>
    /// <list type="bullet">
    ///     <item> Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil </item>
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

        public string FullFileName { get => "Demo Project"; set => throw new NotImplementedException(); }

        public ReadOnlyCollection<IElementCollection> ElementCollections
        {
            get
            {
                return _readOnlyElementCollections;
            }
        }

        public string SoftwareVersion => "1.0";

        public string FileDirectory => "Fake Directory";

        public string AvalonDockLayout { get => "No layout"; set => _=""; }

        public Bitmap ProjectImage => Properties.Resources.Hazard_Icon;

        public string Name { get => "Demo Project"; set => throw new NotImplementedException(); }
        public string Description { get => "Demonstrates ProjectUI framework with undo/redo"; set => throw new NotImplementedException(); }

        public DateTime CreationDate { get => DateTime.Now; }

        public DateTime LastModified { get => DateTime.Now; }

        public bool IsDirty => false;

        public string NameOnDisk => "Demo Project";

        public string ProjectExplorerLayout { get ; set; }

        public event PreviewObjectSavedEventHandler PreviewObjectSaved;
        public event ObjectSavedEventHandler ObjectSaved;
        public event PropertyChangedEventHandler PropertyChanged;
        //public event AddMessageEventHandler AddMessage;
        //public event RemoveMessageEventHandler RemoveMessage;

        public void Close()
        {
            //throw new NotImplementedException();
        }

        public void Compact()
        {
            throw new NotImplementedException();
        }

        public void CreateNew(string newFullFileName)
        {
            throw new NotImplementedException();
        }

        public bool IsValid()
        {
            throw new NotImplementedException();
        }

        public void Open()
        {
            throw new NotImplementedException();
        }

        public void Optimize()
        {
            throw new NotImplementedException();
        }

        public void Save()
        {
            //throw new NotImplementedException();
        }

        public void SaveAs(string newfullFileName)
        {
            throw new NotImplementedException();
        }

        public void ZipProject(string zipFileName)
        {
            throw new NotImplementedException();
        }
    }
}
