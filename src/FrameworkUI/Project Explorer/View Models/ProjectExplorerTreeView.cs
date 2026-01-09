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

namespace FrameworkUI.ProjectExplorer
{

    /// <summary>
    /// Project Explorer TreeView
    /// </summary>
    /// <remarks>
    /// <para>
    ///     Authors:
    ///     Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil
    ///     Woodrow Fields, USACE Risk Management Center, woodrow.l.fields@usace.army.mil
    /// </para>
    /// </remarks>
    public class ProjectExplorerTreeView : ExplorerTreeView
    {

        /// <summary>
        /// Construct new project explorer tree view.
        /// </summary>
        public ProjectExplorerTreeView()
        {
            //// Add handlers
            //PreviewMouseRightButtonDown += Me_PreviewMouseRightButtonDown;
            //PreviewMouseLeftButtonDown += Me_PreviewMouseLeftButtonDown;
            ////PreviewMouseLeftButtonUp += Me_PreviewMouseLeftButtonUp;
            //PreviewMouseDown += Me_PreviewMouseDown;
            //PreviewMouseMove += Me_PreviewMouseMove;
            //DragOver += Me_DragOver;
            //DragLeave += Me_DragLeave;
            //Drop += Me_Drop;
            //AllowDrop = true;
            //AllowMultiSelect = true;
        }

        //public event UserClickedNonElementNodeEventHandler UserClickedNonElementNode;

        //public delegate void UserClickedNonElementNodeEventHandler();

        /// <summary>
        /// Private variables used for drag and drop.
        /// </summary>
        //private bool _canStartDragDrop = false;
        //private Point _startPoint;
        //private InsertionAdorner _insertionAdorner;
        //private ElementNode _startElementNode;
        //private Node _dragNode = null;
        //private List<ElementNode> _selectedElementNodes = new List<ElementNode>();

        ///// <summary>
        ///// Determines if the user can select multiple element nodes at once.
        ///// </summary>
        //public bool AllowMultiSelect { get; set; }

        ///// <summary>
        ///// Gets the list of selected element nodes.
        ///// </summary>
        //public List<ElementNode> SelectedElementNodes
        //{
        //    get { return _selectedElementNodes; }
        //}


        /// <summary>
        /// Dependency property for the project node.
        /// </summary>
        public static DependencyProperty ProjectNodeProperty = DependencyProperty.Register(nameof(ProjectNode), typeof(ProjectNode), typeof(ProjectExplorerTreeView), new FrameworkPropertyMetadata(null, ProjectNode_PropertyChangedCallback));

        /// <summary>
        /// Gets or sets the project node.
        /// </summary>
        public ProjectNode ProjectNode
        {
            get { return (ProjectNode)GetValue(ProjectNodeProperty); }
            set { SetValue(ProjectNodeProperty, value); }
        }

        /// <summary>
        /// When the project node is changed, update the tree view items.
        /// </summary>
        /// <param name="d">The dependency object.</param>
        /// <param name="e">The event data.</param>
        private static void ProjectNode_PropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d == null) return;
            if (d.GetType() != typeof(ProjectExplorerTreeView)) return;
            ProjectExplorerTreeView thisControl = (ProjectExplorerTreeView)d;

            // Get the new value
            ProjectNode newValue = null;
            newValue = e.NewValue as ProjectNode;
            if (newValue == null) return;

            newValue.ParentTreeView = thisControl;
            thisControl.Items.Clear();
            thisControl.Items.Add(newValue);
            thisControl.Items.Refresh();
            thisControl.UpdateLayout();

        }

        #region Multi-Selection

        ///// <summary>
        ///// Clear all selections
        ///// </summary>
        //public void ClearSelection()
        //{
        //    if (this != null) ClearTreeViewItemsControlSelection(Items, ItemContainerGenerator);
        //    for (int i = _selectedElementNodes.Count - 1; i >= 0; i -= 1)
        //    {
        //        _selectedElementNodes[i].IsNodeSelected = false;
        //        _selectedElementNodes.RemoveAt(i);
        //    }
        //}

        ///// <summary>
        ///// Recursive routine used to deselect all items.
        ///// </summary>
        //private void ClearTreeViewItemsControlSelection(ItemCollection itemCollection, ItemContainerGenerator itemContainerGenerator)
        //{
        //    if (itemCollection != null && itemContainerGenerator != null)
        //    {
        //        for (int i = 0; i < itemCollection.Count; i++)
        //        {
        //            TreeViewItem tvi = itemContainerGenerator.ContainerFromIndex(i) as TreeViewItem;
        //            if (tvi != null)
        //            {
        //                ClearTreeViewItemsControlSelection(tvi.Items, tvi.ItemContainerGenerator);
        //                tvi.IsSelected = false;
        //            }
        //        }
        //    }
        //}

        //private void Me_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        //{
        //    TreeViewItem treeViewItem = VisualUpwardSearch(e.OriginalSource as DependencyObject);

        //    if (treeViewItem != null)
        //    {
        //        if (treeViewItem as ElementNodeCollection != null || treeViewItem as ElementNodeGroup != null)
        //        {
        //            treeViewItem.Focus();
        //            UpdateLayout();
        //        }
        //        //e.Handled = true;
        //    }
        //}

        //private void Me_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        //{
        //   // throw new NotImplementedException();
        //}

        ///// <summary>
        ///// Get the starting mouse position on the preview mouse left button down event
        ///// </summary>
        //private void Me_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        //{
        //    var item = FindTreeViewItem(e.OriginalSource as DependencyObject);
        //    if (item as ProjectNode != null || item as ElementNodeCollection != null)
        //    {
        //        // A node is going to be clicked
        //        if (Keyboard.Modifiers == ModifierKeys.Control || Keyboard.Modifiers == ModifierKeys.Shift)
        //        {
        //            // Cannot multi-select on project node, element node collection or element node group.
        //            // Can only single select
        //            // e.Handled = true;
        //        }
        //        else
        //        {
        //            // Allow user to select node.
        //            ClearSelection();
        //            if (item as ProjectNode != null || item as ElementNodeCollection != null) UserClickedNonElementNode();
        //            //UserClickedNonElementNode?.Invoke();
        //        }
        //    }
        //    else if (item as ElementNodeGroup != null)
        //    {
        //        // A node is going to be clicked
        //        if (Keyboard.Modifiers == ModifierKeys.Control || Keyboard.Modifiers == ModifierKeys.Shift)
        //        {
        //            // Cannot multi-select on project node, element node collection or element node group.
        //            // Can only single select
        //            // e.Handled = true;
        //        }
        //        else
        //        {
        //            // Allow user to select node.
        //            ClearSelection();
        //            if (item as ProjectNode != null || item as ElementNodeCollection != null) UserClickedNonElementNode();
        //            //UserClickedNonElementNode?.Invoke();
        //        }
        //    }
        //    else if (item as ElementNode != null)
        //    {
        //        // Element node is going to be selected
        //        ElementNode elementNode = (ElementNode)item; //FindTreeViewItem(e.OriginalSource as DependencyObject);
        //        // Select the node
        //        if (Keyboard.Modifiers == ModifierKeys.Control)
        //        {
        //            // The user wants to select multiple nodes at random
        //            if (_selectedElementNodes.Count == 0)
        //            {
        //                // This is the first element
        //                if (SelectedItem as ElementNode != null)
        //                {
        //                    ((ElementNode)SelectedItem).IsNodeSelected = true;
        //                    _selectedElementNodes.Add((ElementNode)SelectedItem);
        //                }

        //                elementNode.IsNodeSelected = true;
        //                _startElementNode = elementNode;
        //                _selectedElementNodes.Add(elementNode);
        //            }
        //            else if (_selectedElementNodes.IndexOf(elementNode) < 0)
        //            {
        //                elementNode.IsNodeSelected = true;
        //                _startElementNode = elementNode;
        //                _selectedElementNodes.Add(elementNode);
        //            }
        //            else
        //            {
        //                elementNode.IsNodeSelected = false;
        //                _selectedElementNodes.Remove(elementNode);
        //                if (_selectedElementNodes.Count > 0)
        //                {
        //                    _startElementNode = _selectedElementNodes.Last();
        //                    _selectedElementNodes.Last().Focus();
        //                }
        //                else
        //                {
        //                    var elementNodeCollection = FindAncestor<ElementNodeCollection>((DependencyObject)e.OriginalSource);
        //                    elementNodeCollection.Focus();
        //                }
        //            }
        //        }
        //        else if (Keyboard.Modifiers == ModifierKeys.Shift)
        //        {
        //            // The user wants to select multiple nodes continuously
        //            if (_selectedElementNodes.Count == 0)
        //            {
        //                // This is the first element
        //                if (SelectedItem as ElementNode != null)
        //                {
        //                    ((ElementNode)SelectedItem).IsNodeSelected = true;
        //                    _selectedElementNodes.Add((ElementNode)SelectedItem);
        //                }
        //                elementNode.IsNodeSelected = true;
        //                _startElementNode = elementNode;
        //                _selectedElementNodes.Add(elementNode);
        //            }
        //            else if (_selectedElementNodes.Count > 0)
        //            {
        //                // if (elementNode.Element.ParentCollection.Name == _startElementNode.Element.ParentCollection.Name)
        //                if (elementNode.ParentNode == _startElementNode.ParentNode)
        //                {
        //                    var parentNode = FindAncestor<Node>((DependencyObject)e.OriginalSource);
        //                    int startIndex = _startElementNode.ParentNode.ChildNodes.IndexOf(_startElementNode);
        //                    int endIndex = elementNode.ParentNode.ChildNodes.IndexOf(elementNode);
        //                    ClearSelection();
        //                    for (int i = Math.Min(startIndex, endIndex); i <= Math.Max(startIndex, endIndex); i++)
        //                    {
        //                        //ElementNode node = (ElementNode)parentNode.Items[i];
        //                        ElementNode node = (ElementNode)elementNode.ParentNode.ChildNodes[i];
        //                        node.IsNodeSelected = true;
        //                        _selectedElementNodes.Add(node);
        //                    }
        //                }
        //            }
        //        }
        //        else
        //        {
        //            // The user wants to select a single node.
        //            ClearSelection();
        //            elementNode.IsNodeSelected = true;
        //            _startElementNode = elementNode;
        //            _selectedElementNodes.Add(elementNode);
        //        }
        //    }
        //    else
        //    {
        //        // The user is going to click into white space
        //        UserClickedNonElementNode();
        //        //UserClickedNonElementNode?.Invoke();
        //    }

        //    UpdateLayout();
        //}

        //private void UserClickedNonElementNode()
        //{
        //    foreach (var treeItem in Items)
        //    {
        //        if (typeof(Node).IsAssignableFrom(treeItem.GetType()))
        //        {
        //            UserClickedNonElementNode((Node)treeItem);
        //        }
        //    }
        //}

        //private void UserClickedNonElementNode(Node n)
        //{
        //    n.UserClickedWhiteSpace();
        //    foreach (var item in n.ChildNodes)
        //    {
        //        UserClickedNonElementNode(item);
        //    }
        //}

        #endregion

        #region Drag-Drop

        ///// <summary>
        ///// Determines if drag-drop can be initiated.
        ///// </summary>
        //private void Me_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        //{
        //    _canStartDragDrop = false;
        //    _dragNode = null;

        //    // First check if we are pressing over a node, and if that node can be drag-dropped.
        //    var node = FindTreeViewItem(e.OriginalSource as DependencyObject) as Node;

        //    // If node is valid and left-button is pressed, then initiate drag-drop
        //    if (node != null && node.AllowDrop && e.LeftButton == MouseButtonState.Pressed)
        //    {
        //        // Allow drag drop
        //        _canStartDragDrop = true;
        //        // Get the start point for element move operations
        //        _startPoint = e.GetPosition(this);
        //        // Set the node to be dragged and dropped
        //        _dragNode = node;
        //    }
        //}

        /// <summary>
        /// Initiate drag-drop when item is mouse is dragging.
        ///// </summary>
        //private void Me_PreviewMouseMove(object sender, MouseEventArgs e)
        //{
        //    var node = FindTreeViewItem(e.OriginalSource as DependencyObject) as Node;
        //    if (node == null) return;

        //    // First, check if left button is pressed and if drag drop is allowed. 
        //    if (e.LeftButton == MouseButtonState.Pressed && _canStartDragDrop == true)
        //    {
        //        // Next, Check if the mouse has moved sufficiently in the horizontal or vertical direction. 
        //        var position = e.GetPosition(this);
        //        if (Math.Abs(position.X - _startPoint.X) > SystemParameters.MinimumHorizontalDragDistance || Math.Abs(position.Y - _startPoint.Y) > SystemParameters.MinimumVerticalDragDistance)
        //        {
        //            // Next, check if the mouse is being moved within an element node.
        //            if (_dragNode as ElementNode != null)
        //            {
        //                // Finally, check if there is only one element node selected.
        //                if (_selectedElementNodes.Count == 1)
        //                {
        //                    // Do Drag Drop
        //                    // Get the element node to drag-drop.
        //                    var dragNode = _dragNode as ElementNode;
        //                    if (dragNode == null) return;
        //                    if (dragNode.IsInEditMode == true) return;
        //                    // Set the drag data.
        //                    var dragData = new DataObject(dragNode);
        //                    {
        //                        // the drag data text is set because and element node could be dragged-dropped from an external instance.
        //                        dragData.SetText(nameof(ElementNode) + ">" + dragNode.Element.Name + ">" + dragNode.Element.GetType().ToString() + ">" + dragNode.Element.ParentCollection.Name + ">" + dragNode.Element.ParentCollection.ParentProject.FullFileName + ">" + dragNode.Element.CanCopyFromExternal);
        //                    }
        //                    DragDrop.DoDragDrop(dragNode, dragData, DragDropEffects.Move);
        //                }
        //            }
        //            // Check if the mouse is being moved within an element node group.
        //            else if (_dragNode as ElementNodeGroup != null)
        //            {
        //                // Do Drag Drop
        //                // Get the element node group to drag-drop.
        //                var dragNode = _dragNode as ElementNodeGroup;
        //                if (dragNode == null) return;
        //                if (dragNode.IsInEditMode == true) return;
        //                // Set the drag data.
        //                var parentCollection = dragNode.GetElementNodeCollection().ElementCollection;
        //                var dragData = new DataObject(dragNode);
        //                {
        //                    // the drag data text is set because and element node could be dragged-dropped from an external instance.
        //                    dragData.SetText(nameof(ElementNodeGroup) + ">" + dragNode.NodeHeader.HeaderText + ">" + nameof(ElementNodeGroup) + ">" + parentCollection.Name + ">" + parentCollection.ParentProject.FullFileName + ">" + false);
        //                }
        //                DragDrop.DoDragDrop(dragNode, dragData, DragDropEffects.Move);
        //            }

        //        }
        //    }
        //}

        ///// <summary>
        ///// On drag over, check if the drop target collection type is valid and set the adorner.
        ///// </summary>
        //private void Me_DragOver(object sender, DragEventArgs e)
        //{
        //    e.Effects = DragDropEffects.None;
        //    // Get drop target.
        //    if (e.OriginalSource == null) { e.Handled = true; return; }
        //    var dropTarget = FindAncestor<Node>((DependencyObject)e.OriginalSource);

        //    // Get drag elements parent element collection
        //    ElementNodeCollection dragParentCollection = null;
        //    if (_dragNode == null)
        //    {
        //        // Get drag data text and see if this is an element node or element node group
        //        if (e.Data == null) { e.Handled = true; return; }
        //        var data = e.Data.GetData(DataFormats.Text);
        //        if (data == null) { e.Handled = true; return; }

        //        var dataString = data.ToString().Split('>');
        //        if (dataString.Count() < 6) { e.Handled = true; return; }
        //        string eParentCollectionName = dataString[3];
        //        bool canCopyFromExternal = false;
        //        bool.TryParse(dataString[5], out canCopyFromExternal);
        //        if (canCopyFromExternal == false) { e.Handled = true; return; }
        //        foreach (ElementNodeCollection c in ProjectNode.ChildNodes)
        //        {
        //            if (c.ElementCollection.Name == eParentCollectionName)
        //            {
        //                dragParentCollection = c;
        //                break;
        //            }
        //        }
        //    }
        //    else
        //    {
        //        if (_dragNode.GetType() == typeof(ElementNode)) { dragParentCollection = ((ElementNode)_dragNode).GetElementNodeCollection(); }
        //        else if (_dragNode.GetType() == typeof(ElementNodeGroup)) { dragParentCollection = ((ElementNodeGroup)_dragNode).GetElementNodeCollection(); }
        //        else { throw new NotImplementedException("Node type not supported to drag."); }
        //    }

        //    e.Effects = DragDropEffects.Move;

        //    if (dropTarget == null)
        //    {
        //        var k = e.GetPosition(ProjectNode);
        //        if (k.Y > ProjectNode.ActualHeight)
        //        {
        //            if (_insertionAdorner != null) { _insertionAdorner.Detach(); }
        //            AdornerLayer lyr = AdornerLayer.GetAdornerLayer(dragParentCollection);
        //            if (lyr != null) { _insertionAdorner = new InsertionAdorner(true, false, dragParentCollection, lyr); }
        //        }
        //        else
        //        {
        //            if (_insertionAdorner != null)
        //            {
        //                _insertionAdorner.Detach();
        //                _insertionAdorner = null;
        //            }
        //            e.Effects = DragDropEffects.None;
        //        }
        //    }
        //    else if (dropTarget as ElementNodeCollection != null)
        //    {
        //        if (dragParentCollection.Equals(dropTarget))
        //        {
        //            if (_insertionAdorner != null) { _insertionAdorner.Detach(); }
        //            AdornerLayer lyr = AdornerLayer.GetAdornerLayer(dropTarget.NodeHeader);
        //            if (lyr != null) { _insertionAdorner = new InsertionAdorner(true, false, dropTarget.NodeHeader, lyr); }
        //        }
        //        else if (ProjectNode.Items.IndexOf(dragParentCollection) > ProjectNode.Items.IndexOf((ElementNodeCollection)dropTarget))
        //        {
        //            if (_insertionAdorner != null) { _insertionAdorner.Detach(); }
        //            AdornerLayer lyr = AdornerLayer.GetAdornerLayer(dragParentCollection.NodeHeader);
        //            if (lyr != null) { _insertionAdorner = new InsertionAdorner(true, false, dragParentCollection.NodeHeader, lyr); }
        //        }
        //        else
        //        {
        //            if (_insertionAdorner != null) { _insertionAdorner.Detach(); }
        //            AdornerLayer lyr = AdornerLayer.GetAdornerLayer(dragParentCollection);
        //            if (lyr != null) { _insertionAdorner = new InsertionAdorner(true, false, dragParentCollection, lyr); }

        //        }
        //    }
        //    else if (dropTarget as ElementNode != null || dropTarget as ElementNodeGroup != null)
        //    {
        //        ElementNodeCollection dropParentCollection = null;
        //        if (dropTarget.GetType() == typeof(ElementNode)) { dropParentCollection = ((ElementNode)dropTarget).GetElementNodeCollection(); }
        //        else if (dropTarget.GetType() == typeof(ElementNodeGroup)) { dropParentCollection = ((ElementNodeGroup)dropTarget).GetElementNodeCollection(); }

        //        if (dragParentCollection.Equals(dropParentCollection))
        //        {
        //            if (_insertionAdorner != null) { _insertionAdorner.Detach(); }
        //            AdornerLayer lyr = AdornerLayer.GetAdornerLayer(dropTarget.NodeHeader);
        //            if (lyr != null) { _insertionAdorner = new InsertionAdorner(true, IsInTopHalf(dropTarget, e.GetPosition(dropTarget)), dropTarget.NodeHeader, lyr); }
        //        }
        //        else if (ProjectNode.Items.IndexOf(dragParentCollection) > ProjectNode.Items.IndexOf(dropParentCollection))
        //        {
        //            if (_insertionAdorner != null) { _insertionAdorner.Detach(); }
        //            AdornerLayer lyr = AdornerLayer.GetAdornerLayer(dragParentCollection.NodeHeader);
        //            if (lyr != null) { _insertionAdorner = new InsertionAdorner(true, false, dragParentCollection.NodeHeader, lyr); }
        //        }
        //        else
        //        {
        //            if (_insertionAdorner != null) { _insertionAdorner.Detach(); }
        //            AdornerLayer lyr = AdornerLayer.GetAdornerLayer(dragParentCollection);
        //            if (lyr != null) { _insertionAdorner = new InsertionAdorner(true, false, dragParentCollection, lyr); }
        //        }
        //    }
        //    else if (dropTarget as ProjectNode != null)
        //    {
        //        if (_insertionAdorner != null) { _insertionAdorner.Detach(); }
        //        AdornerLayer lyr = AdornerLayer.GetAdornerLayer(dragParentCollection.NodeHeader);
        //        if (lyr != null) { _insertionAdorner = new InsertionAdorner(true, false, dragParentCollection.NodeHeader, lyr); }
        //    }
        //    else
        //    {
        //        e.Effects = DragDropEffects.None;
        //    }

        //    e.Handled = true;
        //}

        ///// <summary>
        ///// On drag leave, check if the treeview is still a target. Remove adorner if not.
        ///// </summary>
        //private void Me_DragLeave(object sender, DragEventArgs e)
        //{
        //    // If the drag leave event has left the treeview then remove the adorner.
        //    // This doesn't work if you flick the mouse quickly to the right of the treeview. I am not sure the best way to handle it since the drag event captures the mouse.
        //    if (e.Source.GetType() == typeof(FrameworkUI.ProjectExplorer.ProjectExplorerTreeView))
        //    {
        //        if (_insertionAdorner != null)
        //        {
        //            _insertionAdorner.Detach();
        //            _insertionAdorner = null;
        //        }
        //        return;
        //    }

        //    var mp = e.GetPosition(this);
        //    if (mp.X >= this.ActualWidth || mp.X <= 0)
        //    {
        //        if (_insertionAdorner != null)
        //        {
        //            _insertionAdorner.Detach();
        //            _insertionAdorner = null;
        //        }
        //        return;
        //    }

        //    if (mp.Y >= this.ActualHeight || mp.Y <= 0)
        //    {
        //        if (_insertionAdorner != null)
        //        {
        //            _insertionAdorner.Detach();
        //            _insertionAdorner = null;
        //        }
        //        return;
        //    }
        //}

        ///// <summary>
        ///// On drop, move or copy the element.
        ///// </summary>
        //private void Me_Drop(object sender, DragEventArgs e)
        //{
        //    // Remove insertion adorner
        //    if (_insertionAdorner != null) { _insertionAdorner.Detach(); }
        //    _insertionAdorner = null;
        //    e.Effects = DragDropEffects.None;

        //    if (e.OriginalSource == null) { _dragNode = null; e.Handled = true; return; }
        //    var dropTarget = FindAncestor<Node>((DependencyObject)e.OriginalSource);

        //    // Get drag element collection
        //    ElementNodeCollection dragParentCollection = null;
        //    string eName = "";
        //    string elementType = "";
        //    string eParentProjectName = "";
        //    if (_dragNode == null)
        //    {
        //        // Get drag data text and see if this is an element node or element node group
        //        if (e.Data == null) { e.Handled = true; return; }
        //        var data = e.Data.GetData(DataFormats.Text);
        //        if (data == null) { e.Handled = true; return; }

        //        var dataString = data.ToString().Split('>');
        //        if (dataString.Count() < 6) { e.Handled = true; return; }
        //        eName = dataString[1];
        //        elementType = dataString[2];
        //        string eParentCollectionName = dataString[3];
        //        eParentProjectName = dataString[4];
        //        bool canCopyFromExternal = false;
        //        bool.TryParse(dataString[5], out canCopyFromExternal);
        //        if (canCopyFromExternal == false) { e.Handled = true; return; }
        //        foreach (ElementNodeCollection c in ProjectNode.ChildNodes)
        //        {
        //            if (c.ElementCollection.Name == eParentCollectionName)
        //            {
        //                dragParentCollection = c;
        //                break;
        //            }
        //        }
        //        dragParentCollection.ElementCollection.InsertFromExternalProject(dragParentCollection.ElementCollection.Count, eName, elementType, eParentProjectName); // Insert into the bottom of the list
        //        _dragNode = ElementNode.FindElementNode(dragParentCollection.ElementCollection[dragParentCollection.ElementCollection.Count - 1], dragParentCollection);
        //        if (_dragNode == null) { e.Handled = true; return; }
        //    }
        //    else
        //    {
        //        if (_dragNode.GetType() == typeof(ElementNode)) { dragParentCollection = ((ElementNode)_dragNode).GetElementNodeCollection(); }
        //        else if (_dragNode.GetType() == typeof(ElementNodeGroup)) { dragParentCollection = ((ElementNodeGroup)_dragNode).GetElementNodeCollection(); }
        //        else { throw new NotImplementedException("Node type not supported to drag."); }
        //    }

        //    if (dropTarget == null)
        //    {
        //        var k = e.GetPosition(ProjectNode);
        //        if (k.Y > ProjectNode.ActualHeight)
        //        {
        //            _dragNode.Move(_dragNode.ParentNode, dragParentCollection, _dragNode.ParentNode.Items.IndexOf(_dragNode), dragParentCollection.Items.Count); // Insert into the bottom of the list
        //        }
        //    }
        //    else if (dropTarget as ElementNodeCollection != null)
        //    {

        //        if (dragParentCollection.Equals(dropTarget))
        //        {
        //            _dragNode.Move(_dragNode.ParentNode, dragParentCollection, _dragNode.ParentNode.Items.IndexOf(_dragNode), 0); // Insert into the top of the list
        //        }
        //        else if (ProjectNode.Items.IndexOf(dragParentCollection) > ProjectNode.Items.IndexOf((ElementNodeCollection)dropTarget))
        //        {
        //            _dragNode.Move(_dragNode.ParentNode, dragParentCollection, _dragNode.ParentNode.Items.IndexOf(_dragNode), 0); // Insert into the top of the list
        //        }
        //        else
        //        {
        //            _dragNode.Move(_dragNode.ParentNode, dragParentCollection, _dragNode.ParentNode.Items.IndexOf(_dragNode), dragParentCollection.Items.Count); // Insert into the bottom of the list
        //        }
        //    }
        //    else if (dropTarget as ElementNode != null || dropTarget as ElementNodeGroup != null)
        //    {
        //        ElementNodeCollection dropParentCollection = null;
        //        if (dropTarget.GetType() == typeof(ElementNode)) { dropParentCollection = ((ElementNode)dropTarget).GetElementNodeCollection(); }
        //        else if (dropTarget.GetType() == typeof(ElementNodeGroup)) { dropParentCollection = ((ElementNodeGroup)dropTarget).GetElementNodeCollection(); }

        //        if (dragParentCollection.Equals(dropParentCollection))
        //        {
        //            int dragIndex = _dragNode.ParentNode.Items.IndexOf(_dragNode);
        //            bool firstHalf = IsInTopHalf(dropTarget, e.GetPosition(dropTarget));
        //            if (dropTarget as ElementNodeGroup != null && firstHalf == false)
        //            {
        //                _dragNode.Move(_dragNode.ParentNode, dropTarget, dragIndex, 0);
        //            }
        //            else
        //            {
        //                int dropIndex = dropTarget.ParentNode.Items.IndexOf(dropTarget);
        //                if (firstHalf == false) { dropIndex += 1; }
        //                if (dropTarget.ParentNode.Equals(_dragNode.ParentNode) && dragIndex < dropIndex) { dropIndex -= 1; }
        //                _dragNode.Move(_dragNode.ParentNode, dropTarget.ParentNode, dragIndex, dropIndex);
        //            }
        //        }
        //        else if (ProjectNode.Items.IndexOf(dragParentCollection) > ProjectNode.Items.IndexOf(dropParentCollection))
        //        {
        //            _dragNode.Move(_dragNode.ParentNode, dragParentCollection, _dragNode.ParentNode.Items.IndexOf(_dragNode), 0); // Insert into the top of the list
        //        }
        //        else
        //        {
        //            _dragNode.Move(_dragNode.ParentNode, dragParentCollection, _dragNode.ParentNode.Items.IndexOf(_dragNode), dragParentCollection.Items.Count); // Insert into the bottom of the list
        //        }
        //    }
        //    else if (dropTarget as ProjectNode != null)
        //    {
        //        _dragNode.Move(_dragNode.ParentNode, dragParentCollection, _dragNode.ParentNode.Items.IndexOf(_dragNode), 0); // Insert into the top of the list
        //    }

        //    _dragNode = null;
        //    e.Handled = true;
        //}

        #endregion

        #region Support

        ///// <summary>
        ///// Determines if the mouse is in the first half of the tree view item.
        ///// </summary>
        //private bool IsInTopHalf(Node dropTarget, Point mousePosition)
        //{
        //    return mousePosition.Y < dropTarget.NodeHeader.ActualHeight / 2d;
        //}

        ///// <summary>
        ///// Helper method to search for ancestors up the visual tree.
        ///// </summary>
        //public T FindAncestor<T>(DependencyObject current) where T : DependencyObject
        //{
        //    do
        //    {
        //        if (current is T) return (T)current;
        //        current = VisualTreeHelper.GetParent(current);
        //    }
        //    while (current != null);
        //    return null;
        //}

        ///// <summary>
        ///// Helper method to search for parent tree view item.
        ///// </summary>
        //public TreeViewItem FindTreeViewItem(DependencyObject dependencyObject)
        //{
        //    if (!(dependencyObject is Visual || dependencyObject is Visual3D)) return null;
        //    TreeViewItem treeViewItem = dependencyObject as TreeViewItem;
        //    if (treeViewItem != null) return treeViewItem;
        //    return FindTreeViewItem(VisualTreeHelper.GetParent(dependencyObject));
        //}

        //static TreeViewItem VisualUpwardSearch(DependencyObject source)
        //{
        //    while (source != null && !(source is TreeViewItem))
        //        source = VisualTreeHelper.GetParent(source);
        //    return source as TreeViewItem;
        //}

        #endregion


    }
}
