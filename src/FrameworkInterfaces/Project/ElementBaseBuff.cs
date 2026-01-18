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
using System.Drawing;


namespace FrameworkInterfaces
{
    /// <summary>
    /// Provides a buffered base class for project elements with basic property management and validation.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b> Authors: </b>
    /// <list type="bullet">
    ///     <item> Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil </item>
    /// </list>
    /// </para>
    /// </remarks>
    public abstract class ElementBaseBuff : IElement
    {
        /// <summary>
        /// The name of the element.
        /// </summary>
        protected string _name;

        /// <summary>
        /// The display name of the element, including dirty state indicator.
        /// </summary>
        protected string _displayName;

        /// <summary>
        /// The name of the element as it appears on disk.
        /// </summary>
        protected string _nameOnDisk;

        /// <summary>
        /// The description of the element.
        /// </summary>
        protected string _description;

        /// <summary>
        /// The date and time when the element was created.
        /// </summary>
        protected DateTime _creationDate;

        /// <summary>
        /// The date and time when the element was last modified.
        /// </summary>
        protected DateTime _lastModified;

        /// <summary>
        /// Indicates whether the element is in a valid state.
        /// </summary>
        protected bool _isValid;

        /// <summary>
        /// Indicates whether the element has unsaved changes.
        /// </summary>
        protected bool _isDirty;

        /// <summary>
        /// The parent collection that contains this element.
        /// </summary>
        private readonly IElementCollection _parentCollection;

        /// <summary>
        /// Construct a new element.
        /// </summary>
        /// <param name="name">The name of the element.</param>
        /// <param name="parentCollection">The parent collection of the element.</param>
        public ElementBaseBuff(string name, IElementCollection parentCollection)
        {
            _name = name;
            _parentCollection = parentCollection;

            _nameOnDisk = name;
            _displayName = name;
            _creationDate = DateTime.Now;
            _lastModified = DateTime.Now;
            _isValid = true;
            _isDirty = false;
        }

        /// <summary>
        /// Gets or sets the name of the element.
        /// </summary>
        [Category("Meta Data")]
        [DisplayName("Name")]
        [Description("The name of the element.")]
        [Browsable(true)]
        public string Name { get => _name; set => Methods.SetString(value, ref _name, PropertyChanged, this); }

        /// <summary>
        /// Gets or sets the description of the element.
        /// </summary>
        [Category("Meta Data")]
        [DisplayName("Description")]
        [Description("The description of the element.")]
        [Browsable(true)]
        public string Description { get => _description; set => Methods.SetString(value, ref _description, PropertyChanged, this); }

        /// <summary>
        /// Gets the date and time when the element was first created.
        /// </summary>
        [Category("Meta Data")]
        [DisplayName("Creation Date")]
        [Description("The date and time when the hydraulic data was first created.")]
        [Browsable(true)]
        public DateTime CreationDate { get => _creationDate; }

        /// <summary>
        /// Gets or sets the date and time when the element was last modified.
        /// </summary>
        [Category("Meta Data")]
        [DisplayName("Last Edited")]
        [Description("The date and time when the hydraulic data was last modified.")]
        [Browsable(true)]
        public DateTime LastModified { get => _lastModified; protected set => Methods.SetDateTime(value, ref _lastModified, PropertyChanged, this); }

        /// <summary>
        /// Gets the name of the element as it appears on disk.
        /// </summary>
        public string NameOnDisk { get => _nameOnDisk; }


        /// <summary>
        /// Gets whether the element is dirty.
        /// </summary>
        public bool IsDirty { get => _isDirty; protected set => Methods.SetBoolean(value, ref _isDirty, PropertyChanged, this, SetDisplayNameDirty); }

        /// <summary>
        /// Private sub used to set display name as dirty with a * or not.
        /// </summary>
        /// <param name="prop">property name.</param>
        private void SetDisplayNameDirty(string prop)
        {
            if (_isDirty == true)
            {
                if (DisplayName != Name + "*")
                {
                    DisplayName = Name + "*";
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DisplayName)));
                }
            }
            else if (DisplayName != Name)
            {
                DisplayName = Name;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(DisplayName)));
            }
        }

        /// <summary>
        /// Gets whether the element is valid or not.
        /// </summary>
        public bool IsValid { get => _isValid; protected set => Methods.SetBoolean(value, ref _isValid, PropertyChanged, this); }

        /// <summary>
        /// Gets the display name of the element.
        /// </summary>
        public string DisplayName { get => _displayName; private set => Methods.SetString(value, ref _displayName, PropertyChanged, this); }

        /// <summary>
        /// Gets the parent element collection.
        /// </summary>
        public IElementCollection ParentCollection { get => _parentCollection; }

        /// <summary>
        /// Gets the element image as a Bitmap.
        /// </summary>
        public abstract Bitmap ElementImage { get; }

        /// <summary>
        /// Determines if the element can be copied from an external application.
        /// </summary>
        public abstract bool CanCopyFromExternal { get; }

        /// <summary>
        /// Event is raised before the element has been deleted.
        /// </summary>
        public event PreviewDeletedEventHandler PreviewDeleted;

        /// <summary>
        /// Event is raised when the element has been deleted.
        /// </summary>
        public event DeletedEventHandler Deleted;

        /// <summary>
        /// Event is raised event before an object has been saved.
        /// </summary>
        public event PreviewObjectSavedEventHandler PreviewObjectSaved;

        /// <summary>
        /// Event is raised when the object has been saved.
        /// </summary>
        public event ObjectSavedEventHandler ObjectSaved;

        /// <summary>
        /// Event is raised whenever a property changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raise property changed event.
        /// </summary>
        /// <param name="propertyName">Name of property that changed.</param>
        /// <param name="isDirty">True to mark element as dirty.</param>
        protected void RaisePropertyChange(string propertyName, bool isDirty = true)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            IsDirty = isDirty;//SetIsDirty(isDirty);
        }

        /// <summary>
        /// Event is raised event before an object has been saved.
        /// </summary>
        /// <param name="sender">The object to be saved.</param>
        /// <param name="cancel">Determines if the save should be canceled.</param>
        protected void RaisePreviewObjectSaved(ISave sender, ref bool cancel)
        {
            PreviewObjectSaved?.Invoke(sender, ref cancel);
        }

        /// <summary>
        /// Event is raised when the object has been saved.
        /// </summary>
        /// <param name="sender">The object that was saved.</param>
        protected void RaiseObjectSaved(ISave sender)
        {
            ObjectSaved?.Invoke(sender);
        }

        /// <summary>
        /// Event is raised before the element has been deleted.
        /// </summary>
        /// <param name="element">The element to be deleted.</param>
        /// <param name="cancel">Determines if the deletion should be canceled.</param>
        protected void RaisePreviewDeleted(IElement element, ref bool cancel)
        {
            PreviewDeleted?.Invoke(element, ref cancel);
        }

        /// <summary>
        /// Event is raised when the element has been deleted.
        /// </summary>
        /// <param name="element">The element that was deleted.</param>
        protected void RaiseDeleted(IElement element)
        {
            Deleted?.Invoke(element);
        }

        /// <summary>
        /// Loads element data from disk.
        /// </summary>
        public abstract void Open();

        /// <summary>
        /// Save the element data to disk.
        /// </summary>
        public abstract void Save();

        /// <summary>
        /// Clones the element. You can optionally provide a new name when cloning the element.
        /// </summary>
        /// <param name="newName">Optional. New name of the cloned element.</param>
        /// <returns>A deep copy of the element.</returns>
        public abstract IElement Copy(string newName = null);

        /// <summary>
        /// Copy the object from an external project to disk within the current project.
        /// </summary>
        /// <param name="itemName">The item to copy from.</param>
        /// <param name="fullFileName">The full file name of the project to copy from.</param>
        /// <returns>A deep copy of the element.</returns>
        public abstract IElement CopyFromExternal(string itemName, string fullFileName);

        /// <summary>
        /// Delete the element from disk and from the parent element collection.
        /// </summary>
        public abstract void Delete();
    }
}
