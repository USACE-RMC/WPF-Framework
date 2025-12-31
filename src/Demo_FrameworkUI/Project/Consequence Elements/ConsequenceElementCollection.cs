using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FrameworkInterfaces;

namespace Demo_ProjectUI.Project.Consequence_Elements
{
    class ConsequenceElementCollection : ElementCollectionBase
    {

        public ConsequenceElementCollection(IProject parentProject) : base(parentProject)
        {
            Add(new ConsequenceElement("Consequence Function_1", this));
            Add(new ConsequenceElement("Consequence Function_2", this));
            Add(new ConsequenceElement("Consequence Function_3", this));
            Add(new ConsequenceElement("Consequence Function_4", this));
            Add(new ConsequenceElement("Consequence Function_5", this));
        }

        public override string Name
        {
            get
            {
                return "Consequence Functions";
            }
        }

        public override void Open()
        {
            _opening = true;
            // read from disk
            _opening = false;
        }

        public override void Add(IElement item)
        {
            item.Deleted += ElementDeleted;
            //item.AddMessage += RaiseAddMessage;
            //item.RemoveMessage += RaiseRemoveMessage;
            ElementList.Add((ConsequenceElement)item);
            // Save to disk
            if (_opening == false)
            {
                //item.IsValid();
                SetIsDirty(true);
            }
            RaiseElementAddedEvent(item);
        }

        public override void Insert(int index, IElement item)
        {
            item.Deleted += ElementDeleted;
            //item.AddMessage += RaiseAddMessage;
            //item.RemoveMessage += RaiseRemoveMessage;
            ElementList.Insert(index, (ConsequenceElement)item);
            // save to disk
            SetIsDirty(true);
            RaiseElementAddedEvent(item);
        }

        public override void Delete()
        {
            // Delete from disk
        }

        public override void InsertFromExternalProject(int index, string elementName, string elementType, string fullFileName)
        {
            throw new System.NotImplementedException();
        }
    }
}
