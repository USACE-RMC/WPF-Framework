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

using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using GenericControls;
using FrameworkInterfaces;

namespace FrameworkUI.ProjectExplorer
{

    /// <summary>
    /// Element node class.
    /// </summary>
    /// <remarks>
    /// <para>
    ///     Authors:
    ///     Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil
    /// </para>
    /// </remarks>
    public class ElementNode : Node
    {

        /// <summary>
        /// Construct a new element node.
        /// </summary>
        /// <param name="element">The project IElement.</param>
        /// <param name="parentNode">The parent node.</param>
        /// <param name="parentTreeView">The parent tree view.</param>
        public ElementNode(IElement element, Node parentNode, ProjectExplorerTreeView parentTreeView) : base(parentNode, parentTreeView)
        {
            // Set Properties
            Element = element;
            AllowDrop = true;
            IsReadOnly = false;

            // Node Header Appearance
            NodeHeader.ShowToolTip = true;
            NodeHeader.StaticImage = GeneralMethods.Bitmap2BitmapSource(element.ElementImage);
            NodeHeader.ExpandedImage = GeneralMethods.Bitmap2BitmapSource(element.ElementImage);

            // Event Handlers
            _editMenuItem.Click += Edit_Click;
            _copyMenuItem.Click += Copy_Click;
            _renameMenuItem.Click += Rename_Click;
            _deleteMenuItem.Click += Delete_Click;
            _customContextItems.Add(_editMenuItem);
            _customContextItems.Add(_copyMenuItem);
            _customContextItems.Add(_renameMenuItem);
            _customContextItems.Add(_deleteMenuItem);

            PreviewMouseRightButtonDown += Me_PreviewMouseRightButtonDown;
            MouseDoubleClick += Me_MouseDoubleClick;
            KeyDown += Me_KeyDown;

            // Set bindings
            NodeHeader.SetBinding(NodeHeader.HeaderTextProperty, new Binding(nameof(Element.Name)) { Source = Element, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged, Mode = BindingMode.TwoWay });
            NodeHeader.SetBinding(NodeHeader.HeaderDescriptionProperty, new Binding(nameof(Element.Description)) { Source = Element, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged, Mode = BindingMode.OneWay });
            NodeHeader.SetBinding(NodeHeader.ShowAsteriskProperty, new Binding(nameof(Element.IsDirty)) { Source = Element, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged, Mode = BindingMode.OneWay });
        }

        /// <summary>
        /// Context menu items.
        /// </summary>
        //private readonly MenuItem _groupMenuItem = new MenuItem() { Header = "Group", Icon = new Image() { Source = GeneralMethods.Bitmap2BitmapSource(Properties.Resources.Group) } };
        private readonly MenuItem _editMenuItem = new MenuItem() { Header = "Edit...", InputGestureText = "Ctrl+E", Icon = new Image() { Source = GeneralMethods.Bitmap2BitmapSource(Properties.Resources.EditWindow) } };
        private readonly MenuItem _copyMenuItem = new MenuItem() { Header = "Copy...", InputGestureText = "Ctrl+C", Icon = new Image() { Source = GeneralMethods.Bitmap2BitmapSource(Properties.Resources.Copy) } };
        private readonly MenuItem _renameMenuItem = new MenuItem() { Header = "Rename...", InputGestureText = "F2", Icon = new Image() { Source = GeneralMethods.Bitmap2BitmapSource(Properties.Resources.Rename) } };
        private readonly MenuItem _deleteMenuItem = new MenuItem() { Header = "Delete...", InputGestureText = "Del", Icon = new Image() { Source = GeneralMethods.Bitmap2BitmapSource(Properties.Resources.Delete) } };

        /// <summary>
        /// Event is raised when the element node is selected.
        /// </summary>
        public event ActivateEventHandler Activate;

        /// <summary>
        /// Event is raised when the element node is selected.
        /// </summary>
        /// <param name="element">The project IElement.</param>
        public delegate void ActivateEventHandler(IElement element);

        /// <summary>
        /// Event is raised when the element node is double-clicked, or if edit is clicked in the context menu.
        /// </summary>
        public event EditEventHandler Edit;

        /// <summary>
        /// Event is raised when the element node is double-clicked, or if edit is clicked in the context menu.
        /// </summary>
        /// <param name="element">The project IElement.</param>
        public delegate void EditEventHandler(IList<IElement> element);

        /// <summary>
        /// Event is raised when copy is clicked in the context menu.
        /// </summary>
        public event CopyEventHandler Copy;

        /// <summary>
        /// Event is raised when copy is clicked in the context menu.
        /// </summary>
        /// <param name="element">The project IElement.</param>
        public delegate void CopyEventHandler(IElement element);

        /// <summary>
        /// Event is raised when delete is clicked in the context menu.
        /// </summary>
        public event DeleteEventHandler Delete;

        /// <summary>
        /// Event is raised when delete is clicked in the context menu.
        /// </summary>
        /// <param name="element">The project IElement.</param>
        public delegate void DeleteEventHandler(IList<IElement> element);

        /// <summary>
        /// Gets the node element.
        /// </summary>
        public IElement Element { get; private set; }

        /// <summary>
        /// Gets a value indicating whether this node can be multi-selected with other nodes.
        /// </summary>
        public override bool CanMultiSelect => true;

        /// <summary>
        /// Get the element node collection that contains this node.
        /// </summary>
        public ElementNodeCollection GetElementNodeCollection()
        {
            ElementNodeCollection elementNodeCollection = null;
            Node parentNode = ParentNode;
            do
            {
                if (parentNode as ElementNodeCollection != null)
                {
                    elementNodeCollection = (ElementNodeCollection)parentNode;
                    break;
                }
                else if (parentNode as ProjectNode != null)
                {
                    break;
                }
                parentNode = parentNode.ParentNode;

            } while (true);
            return elementNodeCollection;
        }

        /// <summary>
        /// Get the element node group that contains this node.
        /// </summary>
        /// <returns>The parent ElementNodeGroup, or null if not found.</returns>
        private ElementNodeGroup GetElementNodeGroup()
        {
            ElementNodeGroup elementNodeGroup = null;
            Node parentNode = ParentNode;
            do
            {
                if (parentNode as ElementNodeGroup != null)
                {
                    elementNodeGroup = (ElementNodeGroup)parentNode;
                    break;
                }
                else if (parentNode as ElementNodeCollection != null)
                {
                    break;
                }
                else if (parentNode as ProjectNode != null)
                {
                    break;
                }
                parentNode = parentNode.ParentNode;
            } while (true);
            return elementNodeGroup;
        }

        /// <summary>
        /// On select, edit mode = false. Raise the edit or activate event.
        /// </summary>
        protected override void Me_Selected(object sender, RoutedEventArgs e)
        {
            Activate?.Invoke(Element);
        }

        /// <summary>
        /// Gets the drag text representation of this element node for drag-drop operations.
        /// </summary>
        /// <returns>A formatted string containing element information for drag-drop operations.</returns>
        public new string GetDragText()
        {
            return nameof(ElementNode) + ">" + Element.Name + ">" + Element.GetType().ToString() + ">" + Element.ParentCollection.Name + ">" + Element.ParentCollection.ParentProject.FullFileName + ">" + Element.CanCopyFromExternal;
        }

        /// <summary>
        /// On preview mouse right click, select node and show context menu.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        private void Me_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (ParentTreeView.SelectedNodes.Count > 1)
            {
                _editMenuItem.Visibility = Visibility.Visible;
                _copyMenuItem.Visibility = Visibility.Collapsed;
                _renameMenuItem.Visibility = Visibility.Collapsed;
                _deleteMenuItem.Visibility = Visibility.Visible;
            }
            else
            {
                _editMenuItem.Visibility = Visibility.Visible;
                _copyMenuItem.Visibility = Visibility.Visible;
                _renameMenuItem.Visibility = Visibility.Visible;
                _deleteMenuItem.Visibility = Visibility.Visible;
            }

        }

        /// <summary>
        /// On mouse double click, raise edit event.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        private void Me_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && IsInEditMode == false)
            {
                Edit?.Invoke(new[] { Element });
            }
            e.Handled = true;
        }

        /// <summary>
        /// Handles the node key down event.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        private void Me_KeyDown(object sender, KeyEventArgs e)
        {
            // Ctrl + E = Edit
            if (e.Key == Key.E && (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)))
            {
                var elements = ParentTreeView.SelectedNodes
                    .OfType<ElementNode>()
                    .Select(n => n.Element)
                    .ToList();
                Edit?.Invoke(elements);
                ParentTreeView.ClearSelection();
            }

            // Ctrl + C = Copy
            else if (e.Key == Key.C && (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)))
            {
                Copy?.Invoke(Element);
            }

            // F2 = Rename
            else if (e.Key == Key.F2)
            {
                if (IsInEditMode == false)
                {
                    IsInEditMode = true;
                }
            }

            // Enter = Stop Renaming OR Edit
            else if (e.Key == Key.Enter)
            {
                bool wasEditing = IsInEditMode;
                UpdateRenameTextBox();
                if (IsSelected == true && wasEditing==false)
                {
                    Edit?.Invoke(new[] { Element });
                }
            }

            // Esc = Cancel Renaming
            else if (e.Key == Key.Escape)
            {
                IsInEditMode = false;
            }

            // Delete = Delete
            else if (e.Key == Key.Delete)
            {
                var elements = ParentTreeView.SelectedNodes
                    .OfType<ElementNode>()
                    .Select(n => n.Element)
                    .ToList();
                Delete?.Invoke(elements);
                ParentTreeView.ClearSelection();
            }
        }

        #region Context menu click methods

     
        /// <summary>
        /// On click, raise edit event.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            var elements = ParentTreeView.SelectedNodes
                .OfType<ElementNode>()
                .Select(n => n.Element)
                .ToList();
            Edit?.Invoke(elements);
            ParentTreeView.ClearSelection();
            e.Handled = true;
        }

        /// <summary>
        /// On click, raise copy event.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        private void Copy_Click(object sender, RoutedEventArgs e)
        {
            var nodeCollection = GetElementNodeCollection();
            //if (nodeCollection != null && nodeCollection != ParentNode)
            //    nodeCollection.ElementNodeAddedParentNode = ParentNode;

            Copy?.Invoke(Element);
            e.Handled = true;
        }

        /// <summary>
        /// On click, raise rename event.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        private void Rename_Click(object sender, RoutedEventArgs e)
        {
            IsInEditMode = true;
            e.Handled = true;
        }

        /// <summary>
        /// On click, raise delete event.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            var elements = ParentTreeView.SelectedNodes
                .OfType<ElementNode>()
                .Select(n => n.Element)
                .ToList();
            Delete?.Invoke(elements);
            ParentTreeView.ClearSelection();
            e.Handled = true;
        }

        #endregion
        /// <summary>
        /// Find the element node for a given element within a node.
        /// </summary>
        /// <param name="element">IElement to find the node for.</param>
        /// <param name="node">Node to search in.</param>
        /// <returns>The ElementNode if found, or null if not found.</returns>
        public static ElementNode FindElementNode(IElement element, Node node)
        {
            foreach (Node child in node.ChildNodes)
            {
                if (child as ElementNode != null && ((ElementNode)child).Element !=null && ((ElementNode)child).Element.Equals(element))//.Name == element.Name)
                {
                    return (ElementNode)child;
                }
                else
                {
                    var elementNode = FindElementNode(element, child);
                    if (elementNode != null) { return elementNode; }
                }
            }
            return null;
        }
    }
}
