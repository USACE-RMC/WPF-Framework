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

using FrameworkInterfaces;
using System.Windows;
using System.Windows.Data;

namespace FrameworkUI.ProjectExplorer
{

    /// <summary>
    /// Element node collection class.
    /// </summary>
    /// <remarks>
    /// <para>
    ///     Authors:
    ///     Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil
    /// </para>
    /// </remarks>
    public class ElementNodeCollection : NodeCollection
    {

        /// <summary>
        /// Construct new element node collection.
        /// </summary>
        /// <param name="parentNode">The parent node.</param>
        /// <param name="parentTreeView">The parent tree view.</param>
        public ElementNodeCollection(Node parentNode, ProjectExplorerTreeView parentTreeView) : base(parentNode, parentTreeView)
        {

        }

        /// <summary>
        /// Dependency property for ElementCollection.
        /// </summary>
        public static DependencyProperty ElementCollectionProperty = DependencyProperty.Register(nameof(ElementCollection), typeof(IElementCollection), typeof(ElementNodeCollection), new FrameworkPropertyMetadata(null, ElementCollection_PropertyChangedCallback));

        /// <summary>
        /// Element collection property callback.
        /// </summary>
        /// <param name="d">The dependency object.</param>
        /// <param name="e">The event data.</param>
        private static void ElementCollection_PropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d == null) return;
            if (d.GetType() != typeof(ElementNodeCollection)) return;
            ElementNodeCollection thisControl = (ElementNodeCollection)d;

            // Get the old value and remove any handlers
            IElementCollection oldValue = null;
            oldValue = e.OldValue as IElementCollection;
            // Remove handlers
            if (oldValue != null)
            {
                oldValue.ElementRemoved -= thisControl.ElementRemoved;
                oldValue.ElementAdded -= thisControl.ElementAdded;
            }

            // Clear collections in preparation for new value.
            thisControl.ChildNodes.Clear();
            BindingOperations.ClearBinding(thisControl.NodeHeader, NodeHeader.HeaderTextProperty);

            // Get the new value
            IElementCollection newElementCollection = null;
            newElementCollection = e.NewValue as IElementCollection;
            if (newElementCollection == null) return;

            // Set binding for header text
            var binding = new Binding(nameof(IElementCollection.Name)) { Source = newElementCollection, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged, NotifyOnSourceUpdated = true, Mode = BindingMode.OneWay };
            BindingOperations.SetBinding(thisControl.NodeHeader, NodeHeader.HeaderTextProperty, binding);

            // Create Element Nodes
            foreach (var element in newElementCollection)
                thisControl.Add(new ElementNode(element, thisControl, (ProjectExplorerTreeView)thisControl.ParentTreeView), false);

            // Child element collections.
            if (newElementCollection.ElementCollections != null)
            {
                for (int i = 0; i < newElementCollection.ElementCollections.Count; i++)
                    thisControl.ChildNodes.Add(new ElementNodeCollection(thisControl, (ProjectExplorerTreeView)thisControl.ParentTreeView) { ElementCollection = newElementCollection.ElementCollections[i], ParentTreeView = thisControl.ParentTreeView });
            }

            // Add handlers
            newElementCollection.ElementRemoved += thisControl.ElementRemoved;
            newElementCollection.ElementAdded += thisControl.ElementAdded; 
            thisControl.ResetItemsSource();
        }

        /// <summary>
        /// The IElementCollection property that defines the nodes to be added under the tree view item header.
        /// </summary>
        public IElementCollection ElementCollection
        {
            get { return (IElementCollection)GetValue(ElementCollectionProperty); }
            set { SetValue(ElementCollectionProperty, value); }
        }

        /// <summary>
        /// When an IElement is removed, update the element node collection.
        /// </summary>
        /// <param name="element">IElement to remove.</param>
        private void ElementRemoved(IElement element)
        {
            var node = ElementNode.FindElementNode(element, this);
            if (node == null) return;
            node.ParentNode.ChildNodes.Remove(node);
            node.ParentNode.ResetItemsSource();
        }

        /// <summary>
        /// When an IElement is added, update the element node collection.
        /// </summary>
        /// <param name="element">IElement to add.</param>
        private void ElementAdded(IElement element)
        {
            //int newIndex = ElementCollection.IndexOf(element);
            //Insert(newIndex, new ElementNode(element, this, ParentTreeView));
            if (ElementNode.FindElementNode(element, this)==null)
            {
                Add(new ElementNode(element, this, (ProjectExplorerTreeView)ParentTreeView));
            }
            //else if (ElementNodeAddedParentNode as ElementNodeGroup != null)
            //{
            //    var elementNode = new ElementNode(element, ElementNodeAddedParentNode, (ProjectExplorerTreeView)ParentTreeView);
            //    NodeAdded?.Invoke(elementNode);
            //    ((ElementNodeGroup)ElementNodeAddedParentNode).Add(elementNode);
            //}
        }
    }
}
