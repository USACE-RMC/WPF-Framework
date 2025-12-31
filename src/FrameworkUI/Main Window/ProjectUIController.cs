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

using FrameworkUI.ProjectExplorer;
using FrameworkInterfaces;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace FrameworkUI
{
    /// <summary>
    /// A class for controlling the FrameworkUI.
    /// </summary>
    /// <remarks>
    /// <para>
    ///     Authors:
    ///     Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil
    /// </para>
    /// </remarks>
    public abstract class ProjectUIController : ProjectNode
    {

        /// <summary>
        /// Construct new ProjectUI controller.
        /// </summary>
        /// <param name="project">Project as IProject.</param>
        public ProjectUIController(IProject project): base(project)
        {
            // Load child nodes in the project node
            Load();

            // Define ProjectUI menu items
            DefineProjectMenuItems();
            DefineToolsMenuItems();
            DefineHelpMenuItems();
        }

        #region Members

        /// <summary>
        /// Event is raised when the properties control is set.
        /// </summary>
        public event SetPropertiesControlEventHandler SetPropertiesControl;
        public delegate void SetPropertiesControlEventHandler(Control propertyControl);

        /// <summary>
        /// Event is raised when the properties control is closed. 
        /// </summary>
        public event ClosePropertiesControlEventHandler ClosePropertiesControl;
        public delegate void ClosePropertiesControlEventHandler(Control propertyControl);

        /// <summary>
        /// The project explorer project node.
        /// </summary>
        public ProjectNode ProjectNode { get; private set; }

        /// <summary>
        /// Defines the default Avalon Dock layout. 
        /// </summary>
        public string DefaultAvalonDockLayout { get; private set; }


        protected readonly List<MenuItem> _projectMenuItems = new List<MenuItem>();
        protected readonly List<MenuItem> _toolsMenuItems = new List<MenuItem>();
        protected readonly List<MenuItem> _helpMenuItems = new List<MenuItem>();
        protected readonly List<MenuItem> _customMenuItems = new List<MenuItem>();

        /// <summary>
        /// The custom project menu items to be set on the application menu bar.
        /// </summary>
        public ReadOnlyCollection<MenuItem> ProjectMenuItems => new ReadOnlyCollection<MenuItem>(_projectMenuItems);

        /// <summary>
        /// The custom tools menu items to be set on the application menu bar.
        /// </summary>
        public ReadOnlyCollection<MenuItem> ToolsMenuItems => new ReadOnlyCollection<MenuItem>(_toolsMenuItems);

        /// <summary>
        /// The custom help menu items to be set on the application menu bar.
        /// </summary>
        public ReadOnlyCollection<MenuItem> HelpMenuItems => new ReadOnlyCollection<MenuItem>(_helpMenuItems);

        /// <summary>
        /// The custom menu items to be set on the application menu bar. These are set in between the Tools and Window menu. E.g., "Mapping". 
        /// </summary>
        public ReadOnlyCollection<MenuItem> CustomMenuStripItems => new ReadOnlyCollection<MenuItem>(_customMenuItems);

        #endregion

        #region Methods

        /// <summary>
        /// This method is called in the base constructor and used to define the custom Project menu items set in the application menu bar.
        /// </summary>
        protected abstract void DefineProjectMenuItems();

        /// <summary>
        /// This method is called in the base constructor and used to define the custom Tools menu items set in the application menu bar.
        /// </summary>
        protected abstract void DefineToolsMenuItems();

        /// <summary>
        /// This method is called in the base constructor and used to define the custom Help menu items set in the application menu bar.
        /// </summary>
        protected abstract void DefineHelpMenuItems();

        /// <summary>
        /// Get a new instance of the Document Control for a project element.
        /// </summary>
        /// <param name="element">The project element.</param>
        /// <returns></returns>
        public abstract Control GetDocumentControl(IElement element);

        /// <summary>
        /// When the document control is closed, this method can be called to perform extra tasks such as updating plot settings.
        /// </summary>
        /// <param name="documentControl">The document control for a project element.</param>
        public abstract void DocumentClosed(UIElement documentControl);

        /// <summary>
        /// When the properties control is closes, this method can be called to perform extra tasks. 
        /// </summary>
        /// <param name="propertiesControl">The properties control for a project element.</param>
        public abstract void PropertiesClosed(UIElement propertiesControl);

        /// <summary>
        /// Get a new instance of the Properties Control for a project element.
        /// </summary>
        /// <param name="documentControl">The document control for a project element.</param>
        public abstract Control GetPropertiesControl(UIElement documentControl);

        /// <summary>
        /// Get a new instance of the Properties Control for a project element.
        /// </summary>
        /// <param name="element">The project element.</param>
        public abstract Control GetPropertiesControl(IElement element);

        /// <summary>
        /// Gets the model element from the specified control. The control will generally be from a document or properties control.
        /// </summary>
        /// <param name="control">Document or properties control to get element from.</param>
        public abstract IElement GetControlElement(UIElement control);

        /// <summary>
        /// Raises the set properties control event. 
        /// </summary>
        /// <param name="propertyControl">The property control to set.</param>
        protected void RaiseSetPropertiesControl(Control propertyControl)
        {
            SetPropertiesControl?.Invoke(propertyControl);
        }

        /// <summary>
        /// Raises the close property control event.
        /// </summary>
        /// <param name="propertyControl">The property control to close.</param>
        protected void RaiseClosePropertiesControl(Control propertyControl)
        {
            ClosePropertiesControl?.Invoke(propertyControl);
        }

        #endregion
    }
}
