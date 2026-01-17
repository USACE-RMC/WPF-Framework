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
using System.ComponentModel;
using System.Windows.Input;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace FrameworkUI.ProjectExplorer
{
    /// <summary>
    /// Node group class.
    /// </summary>
    /// <remarks>
    /// <para>
    ///     Authors:
    ///     Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil
    ///     Woodrow Fields, USACE Risk Management Center, woodrow.l.fields@usace.army.mil
    /// </para>
    /// </remarks>
    public class NodeGroup : Node
    {
        private readonly MenuItem _unGroupMenuItem = new MenuItem() { Header = "Ungroup", Icon = new Image() { Source = Application.Current.FindResource("UngroupImage") as ImageSource } };
        private readonly MenuItem _renameMenuItem = new MenuItem() { Header = "Rename...", InputGestureText = "F2", Icon = new Image() { Source = Application.Current.FindResource("RenameImage") as ImageSource } };
        private readonly MenuItem _groupMenuItem = new MenuItem() { Header = "Add Group", Icon = new Image() { Source = GeneralMethods.Bitmap2BitmapSource(Properties.Resources.Group) } };
        private readonly MenuItem _sortASCMenuItem = new MenuItem() { Name = "sortASC", Header = "Sort Ascending", Icon = new Image() { Source = Application.Current.FindResource("SortAscendingImage") as ImageSource } };
        private readonly MenuItem _sortDSCMenuItem = new MenuItem() { Name = "sortDSC", Header = "Sort Descending", Icon = new Image() { Source = Application.Current.FindResource("SortDescendingImage") as ImageSource } };

        /// <summary>
        /// Construct a new node group.
        /// </summary>
        /// <param name="parentNode">The parent node.</param>
        /// <param name="parentTreeView">The parent tree view.</param>
        public NodeGroup(Node parentNode, ExplorerTreeView parentTreeView) : base(parentNode, parentTreeView)
        {
            // Set Properties
            IsExpanded = true;
            AllowDrop = true;
            IsReadOnly = false;

            // Node Header Appearance
            int groupCount = 0;
            bool uniqueName = false;
            string name = "";
            do
            {
                groupCount += 1;
                name = "New Group " + groupCount;
                bool unique = true;
                foreach (var child in ParentNode.ChildNodes)
                {
                    if (child as NodeGroup != null && child.NodeHeader.HeaderText == name) { unique = false; break; }
                }

                uniqueName = unique;
            } while (uniqueName == false);

            NodeHeader.HeaderText = name;
            NodeHeader.ShowToolTip = false;
            NodeHeader.StaticImage = GeneralMethods.Bitmap2BitmapSource(Properties.Resources.Group);
            NodeHeader.ExpandedImage = GeneralMethods.Bitmap2BitmapSource(Properties.Resources.Group);

            // Event Handlers
            _unGroupMenuItem.Click += Ungroup_Click;
            _renameMenuItem.Click += Rename_Click;
            _groupMenuItem.Click += (sender, e) => AddGroup(new NodeGroup(this, (ExplorerTreeView)ParentTreeView));
            _sortASCMenuItem.Click += Sort_Click;
            _sortDSCMenuItem.Click += Sort_Click;

            // Set up context menu
            _collectionContextItems.Add(_groupMenuItem);
            _collectionContextItems.Add(_unGroupMenuItem);
            _collectionContextItems.Add(_renameMenuItem);
            _collectionContextItems.Add(_sortASCMenuItem);
            _collectionContextItems.Add(_sortDSCMenuItem);

            KeyDown += Me_KeyDown;
        }

        /// <summary>
        /// Gets a value indicating whether this node can be multi-selected with other nodes.
        /// </summary>
        public override bool CanMultiSelect => true;

        /// <summary>
        /// Handles the node key down event.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        private void Me_KeyDown(object sender, KeyEventArgs e)
        {
            // F2 = Rename
            if (e.Key == Key.F2)
            {
                if (IsInEditMode == false)
                {
                    IsInEditMode = true;
                }
            }
            // Enter = Stop Renaming OR Edit
            else if (e.Key == Key.Enter)
            {
                UpdateRenameTextBox();
            }
            // Esc = Cancel Renaming
            else if (e.Key == Key.Escape)
            {
                IsInEditMode = false;
            }
        }

        /// <summary>
        /// Returns a list of invalid node names.
        /// </summary>
        /// <returns>A list of names that are already in use by sibling node groups.</returns>
        protected override List<string> GetInvalidNodeNames()
        {
            // group names need to be unique within each parent node. 
            var invalidNames = new List<string>();
            for (int i = 0; i < ParentNode.ChildNodes.Count; i++)
            {
                if (ParentNode.ChildNodes[i] as NodeGroup != null)
                {
                    if (ParentNode.ChildNodes[i].NodeHeader.HeaderText != NodeHeader.HeaderText)
                    {
                        invalidNames.Add(ParentNode.ChildNodes[i].NodeHeader.HeaderText);
                    }
                }
            }
            return invalidNames;
        }

        /// <summary>
        /// Add a node to the collection.
        /// </summary>
        /// <param name="node">The node to add.</param>
        public void Add(Node node, bool refreshItems = true)
        {
            node.ParentNode = this;
            node.ParentTreeView = ParentTreeView;
            ChildNodes.Add(node);
            if (refreshItems) Items.Refresh();         
        }

        /// <summary>
        /// Add a new empty group under this group. 
        /// </summary>
        /// <param name="nodeGroup">The node group.</param>
        public void AddGroup(NodeGroup nodeGroup)
        {
            ChildNodes.Add(nodeGroup);
            nodeGroup.IsSelected = true;
            nodeGroup.IsInEditMode = true;
            var nodeCollection = GetNodeCollection();
            if (nodeCollection != null) nodeCollection.RaiseGroupAdded(nodeGroup);
        }

        /// <summary>
        /// Ungroup the items and move them to the parent node.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        private void Ungroup_Click(object sender, RoutedEventArgs e)
        {
            // Remove items from the group, and add items to the parent
            int index = ParentNode.ChildNodes.IndexOf(this);
            for (int i = 0; i < ChildNodes.Count; i++) { ParentNode.ChildNodes.Insert(index + i, ChildNodes[i]); }
            ChildNodes.Clear();

            // remove group from parent node
            ParentNode.ChildNodes.Remove(this);
            ParentNode.ResetItemsSource();
            var nodeCollection = GetNodeCollection();
            if (nodeCollection != null) nodeCollection.RaiseGroupRemoved(this);

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
        /// On click, sort nodes in ascending or descending order.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        private void Sort_Click(object sender, RoutedEventArgs e)
        {
            if (((MenuItem)sender).Name == "sortASC")
            {
                Sort(ListSortDirection.Ascending);
            }
            else if (((MenuItem)sender).Name == "sortDSC")
            {
                Sort(ListSortDirection.Descending);
            }

            e.Handled = true;
        }

    }
}