using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FrameworkInterfaces;

namespace Demo_ProjectUI.Project.Response_Elements
{
    class ResponseElementCollection : ElementCollectionBase
    {

        public ResponseElementCollection(IProject parentProject) : base(parentProject)
        {
            Add(new ResponseElement("Response Function_1", this));
            Add(new ResponseElement("Response Function_2", this));
            Add(new ResponseElement("Response Function_3", this));
            Add(new ResponseElement("Response Function_4", this));
            Add(new ResponseElement("Response Function_5", this));
        }

        public override string Name
        {
            get
            {
                return "Response Functions";
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
            ElementList.Add((ResponseElement)item);
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
            ElementList.Insert(index, (ResponseElement)item);
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
