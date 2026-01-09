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

using GenericControls;
using Microsoft.VisualBasic;
using FrameworkInterfaces;
using FrameworkUI;
using FrameworkUI.ProjectExplorer;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using FrameworkUI.Demo.Project.Hazard_Elements;
using FrameworkUI.Demo.Project.Undo_Demo;

namespace FrameworkUI.Demo
{
    /// <summary>
    /// Example project node controller that demonstrates FrameworkUI features including custom context menus, element creation, and document management.
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
    public class ExampleProjectNode : FrameworkUIController
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExampleProjectNode"/> class.
        /// </summary>
        /// <param name="project">The project to be managed by this controller.</param>
        public ExampleProjectNode(IProject project) : base(project)
        {

            for (int i = 0; i < ChildNodes.Count; i++)
            {
                ElementNodeCollection elementnodeCollection = ChildNodes[i] as ElementNodeCollection;
                if (elementnodeCollection == null) continue;

                // Custom menu item to add hazard
                var myCustomMenuItem = new MenuItem() { Header = "Add Hazard (Custom Item)...", Icon = new Image() { Source = GeneralMethods.Bitmap2BitmapSource(Properties.Resources.Hazard_Icon) } };
                myCustomMenuItem.Click += (s, e) => { CreateHazardElement(elementnodeCollection); };
                elementnodeCollection.CustomContextItems.Add(myCustomMenuItem);

                // Allow groups to add hazard like the parent node collection
                elementnodeCollection.GroupAdded += (g) =>
                {
                    var gCustomMenuItem = new MenuItem() { Header = "Add Hazard (Custom Item)...", Icon = new Image() { Source = GeneralMethods.Bitmap2BitmapSource(Properties.Resources.Hazard_Icon) } };
                    gCustomMenuItem.Click += (s, e) =>
                    {
                        var newHazard = CreateHazardElement(elementnodeCollection);
                        Node n = elementnodeCollection.ChildNodes.FirstOrDefault(o => o.GetType() == typeof(ElementNode) && ((ElementNode)o).Element == newHazard);
                        n.Move(elementnodeCollection, g, elementnodeCollection.ChildNodes.IndexOf(n), g.ChildNodes.Count);
                    };
                    g.CustomContextItems.Add(gCustomMenuItem);
                };
            }
        }

        /// <summary>
        /// Gets a value indicating whether multiple nodes can be selected simultaneously.
        /// </summary>
        public override bool CanMultiSelect => false;

        /// <summary>
        /// Creates a new hazard element with a user-specified name.
        /// </summary>
        /// <param name="elementNodes">The element node collection to add the new element to.</param>
        /// <returns>The newly created hazard element.</returns>
        private HazardElement CreateHazardElement(ElementNodeCollection elementNodes)
        {
            string newName = CreateNewNameDialog($"Create New {elementNodes.ElementCollection.Name}...", $"{elementNodes.ElementCollection.Name}_{elementNodes.ElementCollection.Count + 1}", elementNodes.ElementCollection.Select(x => x.Name.ToString()).ToList());
            var newHazard = new HazardElement(newName, elementNodes.ElementCollection);
            elementNodes.ElementCollection.Add(newHazard);
            return newHazard;
        }

        /// <summary>
        /// Handles custom menu item click events.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The routed event arguments.</param>
        private void CustomMenuItem_Clicked(object sender, RoutedEventArgs e)
        {
            Interaction.MsgBox("Yay");
        }

        /// <summary>
        /// Shows a dialog for creating a new element name, ensuring uniqueness.
        /// </summary>
        /// <param name="title">The title of the dialog.</param>
        /// <param name="initialName">The initial suggested name.</param>
        /// <param name="existingElementNames">List of existing element names to check for uniqueness.</param>
        /// <returns>The new element name, or empty string if cancelled.</returns>
        private string CreateNewNameDialog(string title, string initialName, List<string> existingElementNames)
        {
            var nameDialog = new GenericControls.NameDialog(50, "", false, existingElementNames.ToArray(), NameTextBox.GetDefaultInvalidCharacters())
            {
                Icon = GeneralMethods.Bitmap2BitmapSource(Properties.Resources.Add),
                Title = title,
                Owner = Window.GetWindow(this),
                Text = initialName
            };
            if (nameDialog.ShowDialog() == true == true)
            {
                return nameDialog.Text;
            }
            else
            {

                return "";
            }
        }

        /// <summary>
        /// Defines custom menu items for the Project menu.
        /// </summary>
        protected override void DefineProjectMenuItems()
        {
            //throw new System.NotImplementedException();
        }

        /// <summary>
        /// Defines custom menu items for the Tools menu.
        /// </summary>
        protected override void DefineToolsMenuItems()
        {
            //throw new System.NotImplementedException();
        }

        /// <summary>
        /// Defines custom menu items for the Help menu.
        /// </summary>
        protected override void DefineHelpMenuItems()
        {
            //throw new System.NotImplementedException();
        }

        /// <summary>
        /// Gets the document control for the specified element.
        /// </summary>
        /// <param name="element">The element to get the document control for.</param>
        /// <returns>The appropriate document control for the element type.</returns>
        public override Control GetDocumentControl(IElement element)
        {
            // Return the appropriate control based on element type
            if (element is UndoDemoElement undoElement)
            {
                var control = new UI.UndoDemoControl();
                control.Element = undoElement;
                return control;
            }

            return new UI.ElementDocumentControl();
        }

        /// <summary>
        /// Called when a document control is closed.
        /// </summary>
        /// <param name="documentControl">The document control that was closed.</param>
        public override void DocumentClosed(UIElement documentControl)
        {
            //throw new System.NotImplementedException();
        }

        /// <summary>
        /// Gets the properties control for the specified element.
        /// </summary>
        /// <param name="element">The element to get the properties control for.</param>
        /// <returns>The properties control for the element.</returns>
        public override Control GetPropertiesControl(IElement element)
        {
            return new UI.ElementPropertiesControl();
            //throw new System.NotImplementedException();
            //return null;
        }

        /// <summary>
        /// Gets the properties control for the specified document control.
        /// </summary>
        /// <param name="documentControl">The document control to get the properties control for.</param>
        /// <returns>The properties control for the document.</returns>
        public override Control GetPropertiesControl(UIElement documentControl)
        {
            return new UI.ElementPropertiesControl();
            //throw new System.NotImplementedException();
            //return null;
        }

        /// <summary>
        /// Defines custom menu items for the Project Explorer.
        /// </summary>
        protected override void DefineProjectExplorerMenuItems()
        {
            //throw new System.NotImplementedException();
        }

        /// <summary>
        /// Called when a properties control is closed.
        /// </summary>
        /// <param name="documentControl">The document control whose properties were closed.</param>
        public override void PropertiesClosed(UIElement documentControl)
        {
            //throw new System.NotImplementedException();
        }

        /// <summary>
        /// Gets the element associated with the specified control.
        /// </summary>
        /// <param name="control">The control to get the element for.</param>
        /// <returns>The element associated with the control, or null if not applicable.</returns>
        public override IElement GetControlElement(UIElement control)
        {
            // Return the element associated with the control for undo/redo support
            if (control is UI.UndoDemoControl undoDemoControl)
            {
                return undoDemoControl.Element;
            }
            return null;
        }
    }
}
