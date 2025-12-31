using FrameworkInterfaces;

namespace Demo_ProjectUI.Project.Hazard_Elements
{
    public class HazardElementCollection : ElementCollectionBase
    {

        public HazardElementCollection(IProject parentProject) : base(parentProject)
        {
            Add(new HazardElement("Hazard Functions_1", this));
            Add(new HazardElement("Hazard Functions_2", this));
            Add(new HazardElement("Hazard Functions_3", this));
            Add(new HazardElement("Hazard Functions_4", this));
            Add(new HazardElement("Hazard Functions_5", this));
        }

        public override string Name
        {
            get
            {
                return "Hazard Functions";
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
            ElementList.Add((HazardElement)item);
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
            ElementList.Insert(index, (HazardElement)item);
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
            Insert(index, new HazardElement(elementName, this));
            //throw new System.NotImplementedException();
        }
    }
}
