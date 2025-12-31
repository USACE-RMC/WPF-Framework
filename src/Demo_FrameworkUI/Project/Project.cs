using FrameworkInterfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using Demo_ProjectUI.Project.Consequence_Elements;
using Demo_ProjectUI.Project.Hazard_Elements;
using Demo_ProjectUI.Project.Response_Elements;
using Demo_ProjectUI.Project.Undo_Demo;

namespace Demo_ProjectUI.Project
{
    /// <summary>
    /// This is a demo project that showcases the ProjectUI framework features.
    /// Includes an Undo Demo collection that demonstrates undo/redo functionality.
    /// </summary>
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
