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
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace FrameworkUI.ProjectExplorer
{
    /// <summary>
    /// Simple node class representing a basic tree view node.
    /// </summary>
    /// <remarks>
    /// <para>
    ///     Authors:
    ///     Haden Smith, USACE Risk Management Center, cole.h.smith@usace.army.mil
    /// </para>
    /// </remarks>
    public class SimpleNode : Node
    {
        private string _nodeName = "Node Name";

        /// <summary>
        /// Construct a new simple node.
        /// </summary>
        /// <param name="name">The name of the node.</param>
        /// <param name="parentNode">The parent node.</param>
        /// <param name="parentTreeView">The parent tree view.</param>
        public SimpleNode(string name, Node? parentNode, ExplorerTreeView? parentTreeView) : base(parentNode, parentTreeView)
        {
            AllowDrop = true;
            IsReadOnly = false;
            _nodeName = name;

            // Node Header Appearance
            //NodeHeader.ShowToolTip = true;
            NodeHeader.SetResourceReference(NodeHeader.StaticImageProperty, "MoveDownImage");
            NodeHeader.SetResourceReference(NodeHeader.ExpandedImageProperty, "MoveUpImage");

            // Event Handlers

            // Set bindings           
            NodeHeader.SetBinding(NodeHeader.HeaderTextProperty, new Binding(nameof(NodeName)) { Source = this, UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged, Mode = BindingMode.TwoWay });
        }

        /// <summary>
        /// Gets a value indicating whether this node can be multi-selected with other nodes.
        /// </summary>
        public override bool CanMultiSelect => true;

        /// <summary>
        /// Gets or sets the node name.
        /// </summary>
        public string NodeName
        {
            get { return _nodeName; }
            set
            {
                if (value == _nodeName) { return; }
                _nodeName = value;
                RaisePropertyChange(nameof(NodeName));
            }
        }
    }
}
