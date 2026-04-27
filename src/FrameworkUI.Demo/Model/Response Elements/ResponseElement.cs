using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using FrameworkInterfaces;

namespace FrameworkUI.Demo
{
    /// <summary>
    /// Represents a response element in the demo project.
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
    internal class ResponseElement : ElementBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResponseElement"/> class.
        /// </summary>
        /// <param name="name">The name of the response element.</param>
        /// <param name="parentCollection">The parent collection that contains this element.</param>
        public ResponseElement(string name, IElementCollection parentCollection) : base(name, parentCollection)
        {
            Name = name;
            _description = "Response Element";
            _creationDate = DateTime.Now;
            _lastModified = _creationDate;
            SetIsDirty(false);
        }

        /// <summary>
        /// Gets or sets the name of the response element.
        /// </summary>
        public override string Name { get; set; }

        /// <summary>
        /// Gets or sets the description of the response element.
        /// </summary>
        public override string Description { get => _description; set => _description = value; }

        /// <summary>
        /// Gets the creation date of the response element.
        /// </summary>
        public override DateTime CreationDate => _creationDate;

        /// <summary>
        /// Gets the last modified date of the response element.
        /// </summary>
        public override DateTime LastModified => _lastModified;

        /// <summary>
        /// Gets the name used when saving the element to disk.
        /// </summary>
        public override string NameOnDisk => "Response Element";

        /// <summary>
        /// Gets the image icon representing the element.
        /// </summary>
        private static readonly Lazy<ImageSource> s_icon = new(() => { var img = new BitmapImage(new Uri("pack://application:,,,/FrameworkUI.Demo;component/Resources/Hazard_Icon.png")); img.Freeze(); return img; });
        public override ImageSource ElementImage => s_icon.Value;

        /// <summary>
        /// Gets a value indicating whether this element can be copied from an external project.
        /// </summary>
        public override bool CanCopyFromExternal => true;

        /// <summary>
        /// Gets a value indicating whether the element is valid.
        /// </summary>
        public override bool IsValid { get => true; }

        /// <summary>
        /// Creates a copy of the response element.
        /// </summary>
        /// <param name="newName">The name for the copied element. If null, a default name will be used.</param>
        /// <returns>A copy of the element, or null if copying is not implemented.</returns>
        public override IElement Copy(string newName = null)
        {
            //throw new NotImplementedException();
            return null;
        }

        /// <summary>
        /// Copies an element from an external project file.
        /// </summary>
        /// <param name="itemName">The name of the item to copy.</param>
        /// <param name="fullFileName">The full path to the external file.</param>
        /// <returns>A copy of the external element, or null if not implemented.</returns>
        public override IElement CopyFromExternal(string itemName, string fullFileName)
        {
            //throw new NotImplementedException();
            return null;
        }

        /// <summary>
        /// Deletes the response element.
        /// </summary>
        public override void Delete()
        {
            //throw new NotImplementedException();
        }


        /// <summary>
        /// Opens the response element for editing.
        /// </summary>
        public override void Open()
        {
            //throw new NotImplementedException();
        }

        /// <summary>
        /// Saves the response element to disk.
        /// </summary>
        public override void Save()
        {
            //throw new NotImplementedException();
        }
    }
}
