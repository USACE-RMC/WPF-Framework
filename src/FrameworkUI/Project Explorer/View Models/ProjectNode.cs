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
using FrameworkInterfaces;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml.Linq;

namespace FrameworkUI.ProjectExplorer
{
    /// <summary>
    /// Project node class.
    /// </summary>
    /// <remarks>
    /// <para>
    ///     Authors:
    ///     Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil
    /// </para>
    /// </remarks>
    public abstract class ProjectNode : Node
    {

        /// <summary>
        /// Construct new project node based on IProject.
        /// </summary>
        /// <param name="project">Project as IProject.</param>
        public ProjectNode(IProject project) : base(null, null)
        {
            // Set Properties
            Project = project;
            IsExpanded = true;
            AllowDrop = true;
            IsReadOnly = true;

            // Node Header Appearance
            NodeHeader.ShowToolTip = true;
            NodeHeader.SetResourceReference(NodeHeader.StaticImageProperty, "ApplicationImage");
            NodeHeader.SetResourceReference(NodeHeader.ExpandedImageProperty, "ApplicationImage");
            NodeHeader.MouseLeftButtonUp += NodeHeader_MouseLeftButtonUp;

            // Set Bindings
            NodeHeader.SetBinding(NodeHeader.HeaderTextProperty, new Binding(nameof(Project.Name)) { Source = Project });
            NodeHeader.SetBinding(NodeHeader.HeaderDescriptionProperty, new Binding(nameof(Project.Description)) { Source = Project });
            NodeHeader.SetBinding(NodeHeader.ShowAsteriskProperty, new Binding(nameof(Project.IsDirty)) { Source = Project });

        }

        #region Members

        /// <summary>
        /// Event raised when the node is clicked.
        /// </summary>
        public event OnClickEventHandler OnClick;

        /// <summary>
        /// Delegate for the OnClick event.
        /// </summary>
        public delegate void OnClickEventHandler();

        /// <summary>
        /// Get project as IProject.
        /// </summary>
        public IProject Project { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// This method is called in the load method to define custom context menu items in the project explorer. 
        /// </summary>
        protected abstract void DefineProjectExplorerMenuItems();

        /// <summary>
        /// Gets the element node for a given element within a specific element collection.
        /// </summary>
        /// <param name="elementCollectionName">Name of the element collection to search.</param>
        /// <param name="element">The element to find.</param>
        /// <returns>The ElementNode if found, or null if not found.</returns>
        public ElementNode GetElementNode(string elementCollectionName, IElement element)
        {
            foreach (var nodeCollection in ChildNodes)
            {
                if (nodeCollection as ElementNodeCollection == null) continue;
                if (((ElementNodeCollection)nodeCollection).ElementCollection.Name == elementCollectionName)
                    return ElementNode.FindElementNode(element, nodeCollection);
            }
            return null;
        }



        /// <summary>
        /// Get a project element by name for a given element collection.
        /// </summary>
        /// <param name="elementCollectionName">Name of the element collection that contains the element.</param>
        /// <param name="elementName">Name of the element to get.</param>
        public IElement GetElement(string elementCollectionName, string elementName)
        {
            foreach (var nodeCollection in ChildNodes)
            {
                if (nodeCollection as ElementNodeCollection == null) continue;
                if (((ElementNodeCollection)nodeCollection).ElementCollection.Name == elementCollectionName)
                {
                    foreach (var element in ((ElementNodeCollection)nodeCollection).ElementCollection)
                    {
                        if (element.Name == elementName) return element;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Check if the element matches the node.
        /// </summary>
        /// <param name="node">Node to check.</param>
        /// <param name="element">Element to check for.</param>
        /// <param name="matches">True if it matches, false if it doesn't.</param>
        public void ElementMatches(Node node, IElement element, ref bool matches)
        {
            if (matches == true) return;
            if (node as ElementNode != null)
            {
                if (((ElementNode)node).Element.Name == element.Name)
                {
                    matches = true;
                    return;
                }
            }
            if (node as ElementNodeCollection != null || node as ElementNodeGroup != null || node as NodeGroup != null)
            {
                foreach (var child in node.ChildNodes)
                {
                    ElementMatches(child, element, ref matches);
                }
            }

        }

        /// <summary>
        /// Load all of the child nodes. 
        /// </summary>
        public virtual void Load()
        {

            //var tempNodes = new ObservableCollection<Node>();
            for (int i = ChildNodes.Count - 1; i >= 0; i--)
            {
                Node n = ChildNodes[i];
                ChildNodes.RemoveAt(i);
                if (n as ElementNodeCollection == null) continue;
                ((ElementNodeCollection)n).ElementCollection = null;
            }

            bool loadStandard = true;

            // See if there is an XML layout       
            if (Project.ProjectExplorerLayout != null && Project.ProjectExplorerLayout != "")
            {
                // Cross check the XML layout with the tree and make necessary adjustments
                // if the XML has extra items that are not on disk, then the XML will be discarded and it will load the standard way.
                loadStandard = false;
                try
                {
                    // load layout
                    var layout = XElement.Parse(Project.ProjectExplorerLayout);
                    foreach (XElement xElement in layout.Elements())
                    {
                        var node = NodeFromXElement(xElement, this);
                        if (node == null)
                        {
                            // node failed to load from XML, so load the standard way.
                            loadStandard = true;
                            break;
                        }
                        else
                        {
                            ChildNodes.Add(node);
                        }
                    }

                    // Now check if the disk has items the XML does not. 
                    foreach (var child in ChildNodes)
                    {
                        if (child as ElementNodeCollection != null)
                        {
                            for (int i = 0; i < Project.ElementCollections.Count; i++)
                            {
                                if (((ElementNodeCollection)child).ElementCollection.Name == Project.ElementCollections[i].Name)
                                {
                                    for (int j = 0; j < Project.ElementCollections[i].Count; j++)
                                    {
                                        bool itMatches = false;
                                        ElementMatches(child, Project.ElementCollections[i][j], ref itMatches);

                                        if (itMatches == false)
                                        {
                                            // the XML doesn't match the disk, so add correct node from disk to the end of the collection
                                            ((ElementNodeCollection)child).Add(new ElementNode(Project.ElementCollections[i][j], child, (ProjectExplorerTreeView)ParentTreeView));
                                        }
                                    }
                                }
                            }
                        }
                    }

                    if (ChildNodes.Count != Project.ElementCollections.Count)
                        loadStandard = true;

                }
                catch
                {
                    //nodes failed to load from XML, so load the standard way.
                    loadStandard = true;
                }

            }

            // Check if we need to load the standard way
            if (loadStandard == true)
            {
                // Add child element collections
                foreach (var nodeCollection in ChildNodes)
                {
                    if (nodeCollection as ElementNodeCollection == null) continue;
                    ((ElementNodeCollection)nodeCollection).ElementCollection = null;
                }
                for (int i = ChildNodes.Count - 1; i >= 0; i--)
                {
                    ChildNodes.RemoveAt(i);
                }
                for (int i = 0; i < Project.ElementCollections.Count; i++)
                {
                    var elementNodeCollection = new ElementNodeCollection(this, (ProjectExplorerTreeView)ParentTreeView) { ElementCollection = Project.ElementCollections[i] };
                    ChildNodes.Add(elementNodeCollection);
                }
            }

            ResetItemsSource();
            Items.Refresh();

            // Define context menu items
            DefineProjectExplorerMenuItems();
        }

        /// <summary>
        /// Save the project explorer tree view to XElement. 
        /// </summary>
        public XElement SaveToXElement()
        {
            return NodeToXElement(this);
        }

        /// <summary>
        /// Write the node to an XElement.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <returns>An XElement representation of the node.</returns>
        private XElement NodeToXElement(Node node)
        {
            // Create parent node
            var result = new XElement("Node");

            // Set attributes
            if (node as ProjectNode != null)
            {
                result.SetAttributeValue("NodeType", nameof(ProjectNode));
            }
            else if (node as ElementNodeCollection != null)
            {
                result.SetAttributeValue("NodeType", nameof(ElementNodeCollection));

            }
            else if (node as ElementNodeGroup != null)
            {
                result.SetAttributeValue("NodeType", nameof(ElementNodeGroup));
            }
            else if (node as NodeGroup != null)
            {
                result.SetAttributeValue("NodeType", nameof(NodeGroup));
            }
            else if (node as ElementNode != null)
            {
                result.SetAttributeValue("NodeType", nameof(ElementNode));
            }
            else
            {
                result.SetAttributeValue("NodeType", nameof(Node));
            }
            result.SetAttributeValue("Name", node.NodeHeader.HeaderText);
            result.SetAttributeValue(nameof(node.IsExpanded), node.IsExpanded);

            // Add child nodes
            foreach (Node child in node.ChildNodes)
            {
                result.Add(NodeToXElement(child));
            }

            return result;
        }

        /// <summary>
        /// Create a node from XElement
        /// </summary>
        /// <param name="xElement">The XElement to read.</param>
        /// <param name="parentNode">The parent node to add to.</param>
        /// <returns>A Node created from the XElement, or null if the element cannot be parsed.</returns>
        protected Node NodeFromXElement(XElement xElement, Node parentNode)
        {
            if (xElement.Attribute("NodeType") != null && xElement.Attribute("Name") != null)
            {
                Node node;

                // Element Node Collection
                if (xElement.Attribute("NodeType").Value == nameof(ElementNodeCollection))
                {
                    // Set dummy node a placeholder, then look for element node collection
                    node = new SimpleNode("", parentNode, ParentTreeView);
                    for (int i = 0; i < Project.ElementCollections.Count; i++)
                    {
                        if (Project.ElementCollections[i].Name == xElement.Attribute("Name").Value)
                        {
                            node = new ElementNodeCollection(this, (ProjectExplorerTreeView)ParentTreeView) { ElementCollection = Project.ElementCollections[i] };
                            bool expand = true;
                            bool.TryParse(xElement.Attribute(nameof(node.IsExpanded)).Value, out expand);
                            node.IsExpanded = expand;
                            node.ChildNodes.Clear();
                            break;
                        }
                    }
                    if (node as ElementNodeCollection == null) return null;
                }
                // Element Node Group
                else if (xElement.Attribute("NodeType").Value == nameof(ElementNodeGroup))
                {
                    node = new ElementNodeGroup(parentNode, (ProjectExplorerTreeView)ParentTreeView);
                    node.NodeHeader.HeaderText = xElement.Attribute("Name").Value;
                    bool expand = true;
                    bool.TryParse(xElement.Attribute(nameof(node.IsExpanded)).Value, out expand);
                    node.IsExpanded = expand;
                }
                // Node Group
                else if (xElement.Attribute("NodeType").Value == nameof(NodeGroup))
                {
                    node = new NodeGroup(parentNode, (ProjectExplorerTreeView)ParentTreeView);
                    node.NodeHeader.HeaderText = xElement.Attribute("Name").Value;
                    bool expand = true;
                    bool.TryParse(xElement.Attribute(nameof(node.IsExpanded)).Value, out expand);
                    node.IsExpanded = expand;
                }
                // Element Node
                else if (xElement.Attribute("NodeType").Value == nameof(ElementNode))
                {
                    // Set dummy node a placeholder, then look for element
                    node = new SimpleNode("", parentNode, ParentTreeView);
                    Node tempParent = parentNode;

                    // Find the parent element node collection (with null check to prevent infinite loop)
                    while (tempParent != null)
                    {
                        if (tempParent is ElementNodeCollection elementNodeCollection)
                        {
                            // Now search for the IElementCollection with the same name
                            for (int i = 0; i < Project.ElementCollections.Count; i++)
                            {
                                if (Project.ElementCollections[i].Name == elementNodeCollection.NodeHeader.HeaderText)
                                {
                                    // Now find the IElement with the same name
                                    for (int j = 0; j < Project.ElementCollections[i].Count; j++)
                                    {
                                        if (Project.ElementCollections[i][j].Name == xElement.Attribute("Name").Value)
                                        {
                                            node = new ElementNode(Project.ElementCollections[i][j], parentNode, (ProjectExplorerTreeView)ParentTreeView);
                                            break;
                                        }
                                    }
                                    break;
                                }
                            }
                            break;
                        }
                        tempParent = tempParent.ParentNode;
                    }

                    if (!(node is ElementNode)) return null;
                }
                // Basic Node
                else
                {
                    node = new SimpleNode("", parentNode, ParentTreeView);
                }

                // Do recursion to load child nodes
                foreach (XElement child in xElement.Elements())
                {
                    var newNode = NodeFromXElement(child, node);
                    if (newNode == null) return null;

                    if (newNode as ElementNode != null && newNode.ParentNode as ElementNodeCollection != null)
                    {
                        ((ElementNodeCollection)newNode.ParentNode).Add((ElementNode)newNode);
                    }
                    else if (newNode as ElementNodeGroup != null && newNode.ParentNode as ElementNodeCollection != null)
                    {
                        var elementNodeCollection = ((ElementNodeGroup)newNode).GetNodeCollection();
                        var group = (ElementNodeGroup)newNode;
                        elementNodeCollection.AddGroup(group);
                        //elementNodeCollection.ChildNodes.Add(group);
                        //elementNodeCollection.RecursiveAddElementNode(group);
                        //elementNodeCollection.AddGroup((ElementNodeGroup)newNode);
                    }
                    else if (newNode as NodeGroup != null && newNode.ParentNode as ElementNodeCollection != null)
                    {
                        var elementNodeCollection = ((NodeGroup)newNode).GetNodeCollection();
                        var group = (NodeGroup)newNode;
                        elementNodeCollection.AddGroup(group);
                        //elementNodeCollection.ChildNodes.Add(group);
                        //elementNodeCollection.RecursiveAddElementNode(group);
                        //elementNodeCollection.AddGroup((ElementNodeGroup)newNode);
                    }
                    else
                    {
                        node.ChildNodes.Add(newNode);
                    }

                }

                return node;
            }
            else
            {
                // Return nothing if XML is not readable
                return null;
            }
        }

        /// <summary>
        /// When the Project Node header is left clicked, raise OnClick event.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event data.</param>
        private void NodeHeader_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left) OnClick?.Invoke();
            e.Handled = true;
        }

        /// <summary>
        /// Raises the set properties control event. 
        /// </summary>
        /// <param name="propertyControl">The property control to set.</param>
        //protected void RaiseSetPropertiesControl(Control propertyControl)
        //{
        //    SetPropertiesControl?.Invoke(propertyControl);
        //}

        /// <summary>
        /// Raises the close property control event.
        /// </summary>
        /// <param name="propertyControl">The property control to close.</param>
        //protected void RaiseClosePropertiesControl(Control propertyControl)
        //{
        //    ClosePropertiesControl?.Invoke(propertyControl);
        //}

        #endregion

    }
}
