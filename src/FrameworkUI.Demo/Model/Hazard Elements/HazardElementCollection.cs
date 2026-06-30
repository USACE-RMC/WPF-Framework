using FrameworkInterfaces;

namespace FrameworkUI.Demo
{
    /// <summary>
    /// Collection of hazard function elements for the demo project.
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
    public class HazardElementCollection : ElementCollectionBase
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="HazardElementCollection"/> class.
        /// </summary>
        /// <param name="parentProject">The parent project that contains this collection.</param>
        public HazardElementCollection(IProject parentProject) : base(parentProject)
        {
        }

        /// <summary>
        /// Gets the name of the collection.
        /// </summary>
        public override string Name
        {
            get
            {
                return "Hazard Functions";
            }
        }

        /// <summary>
        /// Opens the collection and loads elements from disk.
        /// </summary>
        public override void Open()
        {
            _opening = true;
            // read from disk
            _opening = false;
        }

        /// <summary>
        /// Adds a hazard element to the collection.
        /// </summary>
        /// <param name="item">The element to add to the collection.</param>
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

        /// <summary>
        /// Inserts a hazard element at the specified index in the collection.
        /// </summary>
        /// <param name="index">The zero-based index at which the element should be inserted.</param>
        /// <param name="item">The element to insert.</param>
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

        /// <summary>
        /// Deletes the collection from disk.
        /// </summary>
        public override void Delete()
        {
            // Delete from disk
        }

        /// <summary>
        /// Inserts an element from an external project at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which the element should be inserted.</param>
        /// <param name="elementName">The name of the element to insert.</param>
        /// <param name="elementType">The type of the element.</param>
        /// <param name="fullFileName">The full path to the external project file.</param>
        public override void InsertFromExternalProject(int index, string elementName, string elementType, string fullFileName)
        {
            Insert(index, new HazardElement(elementName, this));
            //throw new System.NotImplementedException();
        }
    }
}
