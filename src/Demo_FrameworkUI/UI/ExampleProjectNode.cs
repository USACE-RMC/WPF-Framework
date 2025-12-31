using GenericControls;
using Microsoft.VisualBasic;
using FrameworkInterfaces;
using FrameworkUI;
using FrameworkUI.ProjectExplorer;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Demo_ProjectUI.Project.Hazard_Elements;
using Demo_ProjectUI.Project.Undo_Demo;

namespace Demo_ProjectUI
{
    public class ExampleProjectNode : ProjectUIController
    {
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

        public override bool CanMultiSelect => false;

        private HazardElement CreateHazardElement(ElementNodeCollection elementNodes)
        {
            string newName = CreateNewNameDialog($"Create New {elementNodes.ElementCollection.Name}...", $"{elementNodes.ElementCollection.Name}_{elementNodes.ElementCollection.Count + 1}", elementNodes.ElementCollection.Select(x => x.Name.ToString()).ToList());
            var newHazard = new HazardElement(newName, elementNodes.ElementCollection);
            elementNodes.ElementCollection.Add(newHazard);
            return newHazard;
        }

        private void CustomMenuItem_Clicked(object sender, RoutedEventArgs e)
        {
            Interaction.MsgBox("Yay");
        }

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

        protected override void DefineProjectMenuItems()
        {
            //throw new System.NotImplementedException();
        }

        protected override void DefineToolsMenuItems()
        {
            //throw new System.NotImplementedException();
        }

        protected override void DefineHelpMenuItems()
        {
            //throw new System.NotImplementedException();
        }

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

        public override void DocumentClosed(UIElement documentControl)
        {
            //throw new System.NotImplementedException();
        }

        public override Control GetPropertiesControl(IElement element)
        {
            return new UI.ElementPropertiesControl();
            //throw new System.NotImplementedException();
            //return null;
        }

        public override Control GetPropertiesControl(UIElement documentControl)
        {
            return new UI.ElementPropertiesControl();
            //throw new System.NotImplementedException();
            //return null;
        }

        protected override void DefineProjectExplorerMenuItems()
        {
            //throw new System.NotImplementedException();
        }

        public override void PropertiesClosed(UIElement documentControl)
        {
            //throw new System.NotImplementedException();
        }

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
